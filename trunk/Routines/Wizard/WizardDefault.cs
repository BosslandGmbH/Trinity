using System;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Routines.Crusader;
using Trinity.UI;
using Zeta.Common;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Wizard
{
    public sealed class WizardDefault : WizardBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Wizard Generic Routine";
        public string Description => "Generic class support, casts all spells whenever possible";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => string.Empty;
        public Build BuildRequirements => null;

        public override Func<bool> ShouldIgnoreAvoidance => () => Settings.DontAvoidInArchonForm && IsArchonActive;
        public override Func<bool> ShouldIgnoreKiting => () => Settings.DontAvoidInArchonForm && IsArchonActive;

        #endregion

        /// <summary>
        /// Only cast in combat and the target is a unit
        /// </summary>
        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;

            if (IsArchonActive)
            {
                if (ShouldArchonStrike(out target))
                    return ArchonStrike(target);

                if (ShouldArchonDisintegrationWave(out target))
                    return ArchonDisintegrationWave(target);

                return null;
            }

            if (TrySpecialPower(out power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            if (IsNoPrimary)
                return Walk(CurrentTarget);

            return null;
        }

        /// <summary>
        /// Only cast when avoiding.
        /// </summary>
        public TrinityPower GetDefensivePower()
        {
            return GetBuffPower();
        }

        /// <summary>
        /// Cast always, in and out of combat.
        /// </summary>
        public TrinityPower GetBuffPower()
        {
            TrinityPower power;            

            if (IsArchonActive)
            {
                if (ShouldArchonSlowTime())
                    return ArchonSlowTime();

                if (ShouldArchonBlast())
                    return ArchonBlast();
            }

            if (TryArmor(out power))
                return power;

            if (ShouldMagicWeapon())
                return MagicWeapon();

            if (ShouldFamiliar())
                return Familiar();

            return null;
        }

        /// <summary>
        /// Only cast on destructibles/barricades
        /// </summary>
        public TrinityPower GetDestructiblePower()
        {
            return DefaultDestructiblePower();
        }

        /// <summary>
        /// Cast by all plugins for all movement.        
        /// </summary>
        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (AllowedToUse(Settings.Teleport, Skills.Wizard.Teleport) && CanTeleportTo(destination))
                return Teleport(destination);

            return Walk(destination);
        }

        protected override bool ShouldArchon()
        {
            if (!AllowedToUse(Settings.Archon, Skills.Wizard.Archon))
                return false;

            if (Sets.ChantodosResolve.IsFullyEquipped && ChantodosStacks < 20)
                return false;

            return base.ShouldArchon();
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WizardDefaultSettings Settings { get; } = new WizardDefaultSettings();

        public sealed class WizardDefaultSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _teleport;
            private SkillSettings _archon;
            private bool _dontAvoidInArchonForm;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(1)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get { return _emergencyHealthPct; }
                set { SetField(ref _emergencyHealthPct, value); }
            }

            public SkillSettings Teleport
            {
                get { return _teleport; }
                set { SetField(ref _teleport, value); }
            }

            public SkillSettings Archon
            {
                get { return _archon; }
                set { SetField(ref _archon, value); }
            }

            [DefaultValue(true)]
            public bool DontAvoidInArchonForm
            {
                get { return _dontAvoidInArchonForm; }
                set { SetField(ref _dontAvoidInArchonForm, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings TeleportDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 1000,
                Reasons = UseReasons.Blocked
            };

            private static readonly SkillSettings ArchonDefaults = new SkillSettings
            {
                UseMode = UseTime.Selective,
                Reasons = UseReasons.Elites | UseReasons.Surrounded
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Teleport = TeleportDefaults.Clone();
                Archon = ArchonDefaults.Clone();
            }

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}


