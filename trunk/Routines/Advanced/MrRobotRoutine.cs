using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;
using Microsoft.Scripting.Utils;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Logger = Trinity.Framework.Helpers.Logger;
using Trinity.Routines.Barbarian;
using Trinity.Settings;
using Trinity.UI;
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

        public TrinityPower GetOffensivePower()
        {
            var primarySkills = new List<Skill>(); 
            var secondarySkills = new List<Skill>();
            var otherSkills = new List<Skill>();

            foreach (var skill in SkillUtils.Active)
            {
                if (skill.IsGeneratorOrPrimary)
                {
                    primarySkills.Add(skill);
                    continue;
                }
                if (skill.IsAttackSpender)
                {
                    secondarySkills.Add(skill);
                    continue;
                }
                otherSkills.Add(skill);          
            }

            var other = FindSkill(otherSkills);
            if (other != null)
            {
                var otherPower = ToPower(other);
                Logger.Log(LogCategory.Routine, $"Selecting Other Power {otherPower}");
                return otherPower;
            }

            var secondary = FindSkill(secondarySkills);
            if (secondary != null)
            {
                var secondaryPower = ToPower(secondary);
                Logger.Log(LogCategory.Routine, $"Selecting Other Power {secondaryPower}");
                return secondaryPower;
            }

            var primary = FindSkill(primarySkills);
            if (primary != null)
            {
                var primaryPower = ToPower(primary);
                Logger.Log(LogCategory.Routine, $"Selecting Other Power {primaryPower}");
                return primaryPower;
            }

            return null;
        }

        private SkillFindResult FindSkill(IEnumerable<Skill> skills, Func<Skill, SkillSettings, bool> condition = null)
        {
            foreach (var skill in skills)
            {
                if (skill == null)
                    return null;

                if (!skill.CanCast())
                    return null;

                var settings = GetSettingsForSkill(skill);
                if (settings == null)
                    return null;

                if (condition != null && !condition(skill, settings))
                    return null;

                if (!AllowedToUse(settings, skill))
                    return null;

                var target = GetTarget(settings, skill);
                if (target == null)
                    return null;

                var result = new SkillFindResult
                {
                    Skill = skill,
                    Settings = settings,
                    Target = target,
                };

                return result;
            }
            return null;
        }

        private Target GetTarget(SkillSettings settings, Skill skill)
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

        private TrinityPower ToPower(SkillFindResult s)
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

        public class SkillFindResult
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

        public TrinityPower GetBuffPower()
        {
            var buffs = SkillUtils.Active.Where(s => s.Cooldown == TimeSpan.Zero && !s.IsGeneratorOrPrimary);
            var buff = FindSkill(buffs);
            if (buff != null)
            {
                var buffPower = ToPower(buff);
                Logger.Log($"Selecting Buff Power {buffPower}");
                return buffPower;
            }
            return null;
        }

        public TrinityPower GetDestructiblePower()
        {           
            //var destructibleSkill = SkillUtils.Active.Where(s => s.CanCast() && s.IsDamaging).OrderByDescending(s => s.IsPrimary).FirstOrDefault();
            //if (destructibleSkill != null)
            //{
            //    Logger.Log(LogCategory.Routine, $"Selected Destructible Power: {destructibleSkill}");
            //    return new TrinityPower(destructibleSkill, 2f);
            //}
            return DefaultPower;
        }

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
            private FullyObservableCollection<SkillSettings> _activeSkills;

            [IgnoreDataMember]
            public FullyObservableCollection<SkillSettings> ActiveSkills
            {
                get { return _activeSkills; }
                set { SetField(ref _activeSkills, value); }
            }

            [DataMember]
            public List<SkillSettings> Skills
            {
                get { return _skills; }
                set { SetField(ref _skills, value); }
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
            }

            private void UpdateSkills(bool reset = false)
            {
                // We may have loaded settings for all skills from any class.
                // Active skill is just the subset currently in use by the hero.
                // But we still need to persist settings for all skills used to-date.
                // Also need to populate with reference skill info for the pretty UI Icons etc.

                if (!ZetaDia.Service.IsInGame)
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

                var availableSkills = Skills.Where(s => Core.Hotbar.ActivePowers.Any(p => s.SNOPower == p));
                ActiveSkills = new FullyObservableCollection<SkillSettings>(availableSkills);
            }

            public void LoadSettingsFromJson(string json)
            {
                JsonSerializer.Deserialize(json, this);
                UpdateSkills();
            }

            private static MrRobotSettings OnDataContextRequested(MrRobotSettings settings)
            {
                // Called when settings window is opened.
                // Need to refresh the list of active skills.
                settings.UpdateSkills();
                return settings;
            }

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>("MrRobotSettings.xaml");
            public object GetDataContext() => OnDataContextRequested(this);
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => LoadSettingsFromJson(code);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}


