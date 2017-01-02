using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Scripting.Utils;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Attributes;
using Trinity.Reference;
using Logger = Trinity.Framework.Helpers.Logger;
using Trinity.Routines.Barbarian;
using Trinity.Settings;
using Trinity.UI;
using Trinity.UI.UIComponents;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Routines.Advanced
{
    public sealed class MrRobotRoutine : RoutineBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Mr Robot Routine";
        public string Description => "Experimental Routine, BETA! WORK IN PROGRESS!";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => string.Empty;
        public Build BuildRequirements => null;
        public ActorClass Class => ActorClass.Invalid;
        public ActorClass CurrentClass => ZetaDia.Service.Hero.Class;

        #endregion

        public override bool SetWeight(TrinityActor cacheObject)
        {
            var weightType = WeightSettingsHelper.GetWeightType(cacheObject);
            var startingWeight = cacheObject.Weight;
            var weight = 0d;

            foreach (var weightRule in Settings.WeightOverrides.Where(r => r.IsEnabled && r.ActorWeightType == weightType))
            {
                weight += weightRule.Eval(cacheObject, cacheObject.Weight);
            }

            var weightChanged = Math.Abs(startingWeight - weight) > float.Epsilon;
            if (weightChanged)
            {
                cacheObject.Weight = weight;
                return true;
            }
            return false;
        }

        public TrinityPower GetOffensivePower()
        {
            ValidatedSkill vSkill;

            if (Settings.ActiveSkills == null || Settings.Weights == null)
            {
                Settings.Update();
            }

            foreach (var skill in Settings.ActiveSkills)
            {
                if (TryValidateSkill(skill, out vSkill))
                {
                    return ToPower(vSkill);
                }
            }

            return null;
        }

        private ValidatedSkill FindSkill(IEnumerable<Skill> skills, Predicate<SkillSettings> condition = null)
        {
            return skills.Select(s => ValidateSkill(GetSettingsForSkill(s), condition)).FirstOrDefault();
        }

        private bool TryValidateSkill(SkillSettings skillSettings, out ValidatedSkill validatedSkill)
        {
            return (validatedSkill = ValidateSkill(skillSettings)) != null;
        }

        private ValidatedSkill ValidateSkill(SkillSettings settings, Predicate<SkillSettings> condition = null)
        {
            var skill = settings.Skill;
            if (skill == null)
                return null;

            if (!skill.CanCast())
                return null;

            if (condition != null && !condition(settings))
                return null;

            if (!AllowedToUse(settings, skill))
                return null;

            var target = GetTarget(settings);
            if (target == null)
                return null;

            var result = new ValidatedSkill
            {
                Skill = skill,
                Settings = settings,
                Target = target,
            };

            return result;
        }

        private Target GetTarget(SkillSettings settings)
        {
            var maxRange = Math.Max(TrashRange, EliteRange);
            Target target = null;

            if (settings.Target.HasFlag(UseTarget.Default))
                target = new Target(CurrentTarget);

            else if (settings.Target.HasFlag(UseTarget.CurrentTarget))
                target = new Target(CurrentTarget);

            else if (settings.Target.HasFlag(UseTarget.BestCluster))
                target = new Target(TargetUtil.GetBestClusterUnit(maxRange));

            else if (settings.Target.HasFlag(UseTarget.ClosestMonster))
                target = new Target(TargetUtil.ClosestUnit(maxRange));

            else if (settings.Target.HasFlag(UseTarget.ElitesOnly))
                target = new Target(TargetUtil.BestEliteInRange(maxRange));

            else if (settings.Target.HasFlag(UseTarget.Self))
                target = new Target(Player.Actor);

            else if (settings.Target.HasFlag(UseTarget.SafeSpot))
                target = new Target(Avoider.SafeSpot);

            return target;
        }

        private TrinityPower ToPower(ValidatedSkill s)
        {
            var onSelf = !s.Skill.IsDamaging;
            var power = new TrinityPower
            {
                SNOPower = s.Skill.SNOPower,
                IsCastOnSelf = onSelf,
                MinimumRange = Math.Max(5f, s.Settings.CastRange),
                TargetAcdId = onSelf ? s.Target.AcdId : -1,
                TargetPosition = onSelf ? Player.Position : s.Target.Position,
            };
            return power;
        }

        #region Supporting Objects

        public class ValidatedSkill
        {
            public Skill Skill;
            public SkillSettings Settings;
            public Target Target;
        }

        public class Target
        {
            public Target() { }
            public Target(TrinityActor actor)
            {
                if (actor == null)
                    return;

                Position = actor.Position;
                AcdId = actor.AcdId;
            }
            public Target(Vector3 position)
            {
                Position = position;
            }
            public Vector3 Position;
            public int AcdId = -1;
        }

        #endregion

        public TrinityPower GetDefensivePower() => null;

        public TrinityPower GetBuffPower() => null;

        public TrinityPower GetDestructiblePower()
        {
            foreach (var skillEntry in Settings.ActiveSkills)
            {
                if (!OffensiveSkillTargetTypes.Contains(skillEntry.Target) || !skillEntry.Skill.CanCast())
                    continue;

                return new TrinityPower(skillEntry.Skill.SNOPower, skillEntry.CastRange, CurrentTarget.AcdId);
            }
            return DefaultPower;
        }

        private HashSet<UseTarget> OffensiveSkillTargetTypes { get; } = new HashSet<UseTarget>
        {
            UseTarget.BestCluster,
            UseTarget.CurrentTarget,
            UseTarget.ClosestMonster,
            UseTarget.ElitesOnly,
        };

        #region Movement

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (Core.Grids.CanRayCast(destination))
            {
                foreach (var skill in MovementSkills.Where(s => s.CanCast()))
                {
                    var settings = GetSettingsForSkill(skill);
                    if (AllowedToUse(settings, skill))
                    {
                        return ToMovementPower(skill, destination);
                    }
                }
            }
            return Walk(destination);
        }

        public HashSet<Skill> MovementSkills = new HashSet<Skill>
        {
            Skills.Monk.DashingStrike,
            Skills.DemonHunter.Vault,
            Skills.DemonHunter.Strafe,
            Skills.Barbarian.FuriousCharge,
            Skills.Barbarian.Whirlwind,
        };

        private TrinityPower ToMovementPower(Skill skill, Vector3 position)
        {
            var power = new TrinityPower
            {
                SNOPower = skill.SNOPower,
                TargetPosition = position,
            };
            return power;
        }

        #endregion

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public int PrimaryEnergyReserve => Settings.PrimaryEnergyReserve;
        public int SecondaryEnergyReserve => Settings.SecondaryEnergyReserve;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;
        public KiteMode KiteMode => Settings.KiteMode;
        public float KiteDistance => Settings.KiteDistance;
        public int KiteStutterDuration => Settings.KiteStutterDuration;
        public int KiteStutterDelay => Settings.KiteStutterDelay;
        public int KiteHealthPct => Settings.KiteHealthPct;
        public float TrashRange => Settings.TrashRange;
        public float EliteRange => Settings.EliteRange;
        public float HealthGlobeRange => Settings.HealthGlobeRange;
        public float ShrineRange => Settings.ShrineRange;

        public Func<bool> ShouldIgnoreNonUnits { get; } = () => false;
        public Func<bool> ShouldIgnorePackSize { get; } = () => false;
        public Func<bool> ShouldIgnoreAvoidance { get; } = () => false;
        public Func<bool> ShouldIgnoreKiting { get; } = () => false;
        public Func<bool> ShouldIgnoreFollowing { get; } = () => false;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public MrRobotSettings Settings { get; } = new MrRobotSettings();
        public SkillSettings GetSettingsForSkill(Skill skill) =>
            Settings.Skills.FirstOrDefault(s => s.SNOPower == skill.SNOPower);

        [DataContract]
        public sealed class MrRobotSettings : NotifyBase, IDynamicSetting
        {
            #region DragDrop Handler

            public class UpdateOrderDropHandler : DefaultDropHandler
            {
                public override void Drop(IDropInfo dropInfo)
                {
                    base.Drop(dropInfo);
                    var items = dropInfo.TargetCollection.OfType<SkillSettings>().ToList();
                    for (var index = 0; index < items.Count; index++)
                    {
                        var skillSettings = items[index];
                        skillSettings.Order = index;
                    }
                }
            }

            [IgnoreDataMember]
            public UpdateOrderDropHandler DropHandler { get; set; } = new UpdateOrderDropHandler();

            #endregion

            private int _clusterSize;
            private float _emergencyHealthPct;
            private KiteMode _kiteMode;
            private float _kiteDistance;
            private int _kiteStutterDuration;
            private int _kiteStutterDelay;
            private int _kiteHealthPct;
            private float _trashRange;
            private float _eliteRange;
            private float _healthGlobeRange;
            private float _shrineRange;
            private int _secondaryEnergyReserve;
            private int _primaryEnergyReserve;

            private List<SkillSettings> _skills;
            private List<WeightSettings> _weights;

            private FullyObservableCollection<SkillSettings> _activeSkills;
            private FullyObservableCollection<WeightSettings> _weightOverrides;

            [IgnoreDataMember]
            public FullyObservableCollection<SkillSettings> ActiveSkills
            {
                get { return _activeSkills; }
                set { SetField(ref _activeSkills, value); }
            }

            [IgnoreDataMember]
            public FullyObservableCollection<WeightSettings> WeightOverrides
            {
                get { return _weightOverrides; }
                set { SetField(ref _weightOverrides, value); }
            }

            [DataMember]
            public List<SkillSettings> Skills
            {
                get { return _skills; }
                set { SetField(ref _skills, value); }
            }

            [DataMember]
            public List<WeightSettings> Weights
            {
                get { return _weights; }
                set { SetField(ref _weights, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(8)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get { return _emergencyHealthPct; }
                set { SetField(ref _emergencyHealthPct, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(KiteMode.Never)]
            public KiteMode KiteMode
            {
                get { return _kiteMode; }
                set { SetField(ref _kiteMode, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(15f)]
            public float KiteDistance
            {
                get { return _kiteDistance; }
                set { SetField(ref _kiteDistance, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(800)]
            public int KiteStutterDuration
            {
                get { return _kiteStutterDuration; }
                set { SetField(ref _kiteStutterDuration, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(800)]
            public int KiteStutterDelay
            {
                get { return _kiteStutterDelay; }
                set { SetField(ref _kiteStutterDelay, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(100)]
            public int KiteHealthPct
            {
                get { return _kiteHealthPct; }
                set { SetField(ref _kiteHealthPct, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(80f)]
            public float TrashRange
            {
                get { return _trashRange; }
                set { SetField(ref _trashRange, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(80f)]
            public float EliteRange
            {
                get { return _eliteRange; }
                set { SetField(ref _eliteRange, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(80f)]
            public float HealthGlobeRange
            {
                get { return _healthGlobeRange; }
                set { SetField(ref _healthGlobeRange, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(80f)]
            public float ShrineRange
            {
                get { return _shrineRange; }
                set { SetField(ref _shrineRange, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(50)]
            public int SecondaryEnergyReserve
            {
                get { return _secondaryEnergyReserve; }
                set { SetField(ref _secondaryEnergyReserve, value); }
            }

            [DataMember(EmitDefaultValue = false)]
            [DefaultValue(50)]
            public int PrimaryEnergyReserve
            {
                get { return _primaryEnergyReserve; }
                set { SetField(ref _primaryEnergyReserve, value); }
            }

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                UpdateSkills(true);
                UpdateWeights(true);
            }

            private void UpdateSkills(bool reset = false)
            {
                // We may have loaded settings for all skills from any class.
                // Active skill is just the subset currently in use by the hero.
                // But we still need to persist settings for all skills used to-date.
                // Also need to populate with reference skill info for the pretty UI Icons etc.

                if (!Player.IsInGame && !Core.IsEnabled || !ZetaDia.IsInGame)
                    return;

                var activeSettings = SkillUtils.Active.Select(s => new SkillSettings(s)).ToList();
                if (Skills == null || !Skills.Any() || reset)
                {
                    Skills = new List<SkillSettings>(activeSettings);
                }
                else
                {
                    Skills.ForEach(s => s.SetReferenceSkill(SkillUtils.GetSkillByPower(s.SNOPower)));
                    Skills.AddRange(activeSettings.Where(b => Skills.All(a => a.SNOPower != b.SNOPower)));
                }

                var availableSkills = Skills.Where(s => Core.Hotbar.ActivePowers.Any(p => s.SNOPower == p)).OrderBy(s => s.Order);
                ActiveSkills = new FullyObservableCollection<SkillSettings>(availableSkills);
            }

            private void UpdateWeights(bool reset = false)
            {
                if (!Player.IsInGame && !Core.IsEnabled || !ZetaDia.IsInGame)
                    return;

                if (Weights == null || !Weights.Any() || reset)
                    Weights = WeightSettingsHelper.GetDefaults();

                WeightOverrides = new FullyObservableCollection<WeightSettings>(Weights);
            }

            public void Update()
            {
                UpdateSkills();
                UpdateWeights();
            }

            public void LoadSettingsFromJson(string json)
            {
                JsonSerializer.Deserialize(json, this);
                Update();
            }

            private static MrRobotSettings OnDataContextRequested(MrRobotSettings settings)
            {
                // Called when settings window is opened.
                // Need to refresh the list of active skills.
                settings.UpdateSkills();
                settings.UpdateWeights();
                return settings;
            }

            public void OnSave()
            {
                Logger.LogNormal("Mr Robot Saved.");

                foreach (var weightSetting in WeightOverrides)
                {
                    weightSetting.Compile();
                }
            }

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>("MrRobotSettings.xaml");
            public object GetDataContext() => OnDataContextRequested(this);
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => LoadSettingsFromJson(code);
            public void Reset() => LoadDefaults();
            public void Save() => OnSave();

            #endregion
        }

        #endregion
    }
}


