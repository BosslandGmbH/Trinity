using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;

namespace Trinity.Routines.Wizard
{
    public sealed class WizardParalysisQvazmirArchon : WizardBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Vyr/Rasha Archon Paralysis (Speedfarm)";
        public string Description => "Wizard routine intended for high-level grifts / t13 farm";
        public string Author => "Unexpected/TwoCigars/Qvazmir";
        public string Version => "0.4";
        public string Url => "http://www.diablofans.com/builds/85544-s9-2-4-3-paralysis-vyrrasha-t13-speed-rifts";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.TalRashasElements, SetBonus.Third },
                //{ Sets.VyrsAmazingArcana, SetBonus.Second }
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Wizard.Teleport, null },
                { Skills.Wizard.FrostNova, null }
            },
            Items = new List<Item>
            {
                Legendary.ManaldHeal
            },
        };
        public override KiteMode KiteMode => KiteMode.Always;
        public override float KiteDistance => 15f;

        public override Func<bool> ShouldIgnoreAvoidance => () => IsArchonActive;
        #endregion
        public TrinityPower GetOffensivePower()
        {
            var bestClusterUnit = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;

            if (IsArchonActive)
            {
                //if (CanTeleport)
                //    Teleport(TargetUtil.GetBestClusterPoint());

                if (Skills.Wizard.ArchonDisintegrationWave.CanCast())
                {
                    return ArchonDisintegrationWave(bestClusterUnit);
                }

                return null;
            }
            //frost nova to proc tal rasha

            if (TrySpecialPower(out var power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            if (IsNoPrimary)
                return Walk(CurrentTarget);

            return null;
        }

        protected override bool ShouldArchon()
        {
            if (!Skills.Wizard.Archon.CanCast())
                return false;

            if (!HasTalRashaStacks)
                return false;

            if (Player.IsInTown)
                return false;

            return true;
        }


        //cast frost nova frequently to proc Tal's
        protected override bool ShouldFrostNova()
        {
            if (!Skills.Wizard.FrostNova.CanCast())
                return false;

            return true;
        }

        protected override bool ShouldTeleport(out Vector3 position)
        {
            //Teleport into clusters if calamity is equipped

            position = Vector3.Zero;

            if (!Skills.Wizard.Teleport.CanCast())
                return false;

            if (Skills.Wizard.Teleport.TimeSinceUse < 2000)
                return false;

            var closeEnoughToBestCluster = TargetUtil.GetBestClusterUnit(15f, 50f)?.Distance < 15f;
            if (!IsInCombat && closeEnoughToBestCluster)
                return false;

            //if (!IsArchonActive)
            //    return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        public TrinityPower GetDefensivePower() => null;

        public TrinityPower GetBuffPower()
        {
            //Make bot get 4 stacks tal & pop archon off cd. Made for speedfarm purposes.
            if (!HasTalRashaStacks && Skills.Wizard.Archon.CanCast() && !Player.IsInTown)
            {
                if (Skills.Wizard.FrostNova.CanCast())
                    return FrostNova();
                else if (Skills.Wizard.Teleport.CanCast())
                    return Teleport(Player.Position);
                else if (Skills.Wizard.ArcaneTorrent.CanCast())
                    return ArcaneTorrent(Player.Actor);
            }
            else
            {
                if (ShouldArchon())
                    return Archon();
            }

            return DefaultBuffPower();
        }

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            //Make bot get 4 stacks tal & pop archon off cd. Made for speedfarm purposes.
            if (!HasTalRashaStacks && Skills.Wizard.Archon.CanCast() && !Player.IsInTown)
            {
                if (Skills.Wizard.FrostNova.CanCast())
                    return FrostNova();
                else if (Skills.Wizard.Teleport.CanCast())
                    return Teleport(Player.Position);
                else if (Skills.Wizard.ArcaneTorrent.CanCast())
                    return ArcaneTorrent(Player.Actor);
            }
            else
            {
                if (ShouldArchon())
                    return Archon();
            }

            if (AllowedToUse(Settings.Teleport, Skills.Wizard.Teleport) && CanTeleportTo(destination))
                return Teleport(destination);

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WizardParalysisQvazmirArchonSettings Settings { get; } = new WizardParalysisQvazmirArchonSettings();

        public sealed class WizardParalysisQvazmirArchonSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _teleport;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(6)]
            public int ClusterSize
            {
                get => _clusterSize;
                set => SetField(ref _clusterSize, value);
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get => _emergencyHealthPct;
                set => SetField(ref _emergencyHealthPct, value);
            }

            public SkillSettings Teleport
            {
                get => _teleport;
                set => SetField(ref _teleport, value);
            }

            #region Skill Defaults

            private static readonly SkillSettings TeleportDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 200,
                Reasons = UseReasons.Blocked
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Teleport = TeleportDefaults.Clone();
            }

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this, true);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}


