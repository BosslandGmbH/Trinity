using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Attributes;
using Trinity.Reference;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.DemonHunter
{
    public sealed class DemonHunterDefault : DemonHunterBase, IRoutine
    {
        #region Definition

        public string DisplayName => "DemonHunter Generic Routine";
        public string Description => "Generic class support, casts all spells whenever possible";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => string.Empty;
        public Build BuildRequirements => null;

        #endregion

        /// <summary>
        /// Only cast in combat and the target is a unit
        /// </summary>
        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            Vector3 position;

            if (AllowedToUse(Settings.Vault, Skills.DemonHunter.Vault) && ShouldVault(out position))
                return Vault(position);

            if (ShouldRefreshBastiansGenerator && TryPrimaryPower(out power))
                return power;

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
            if (ShouldSpikeTrap())
                return SpikeTrap();

            if (Runes.DemonHunter.FanOfDaggers.IsActive && ShouldFanOfKnives())
                return FanOfKnives();            

            if (ShouldCaltrops())
                return Caltrops();

            if (ShouldShadowPower())
                return ShadowPower();

            if (ShouldSmokeScreen())
                return SmokeScreen();

            return null;
        }

        /// <summary>
        /// Cast always, in and out of combat.
        /// </summary>
        public TrinityPower GetBuffPower()
        {
            if (AllowedToUse(Settings.Vengeance, Skills.DemonHunter.Vengeance) && ShouldVengeance())
                return Vengeance();

            if (ShouldShadowPower())
                return ShadowPower();

            if (ShouldSmokeScreen())
                return SmokeScreen();

            if (ShouldPreparation())
                return Preparation();

            if (ShouldCompanion())
                return Companion();

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
            if (!Player.IsInTown && AllowedToUse(Settings.Vault, Skills.DemonHunter.Vault) && CanVaultTo(destination))
                return Vault(destination);            
            
            if (CanStrafeTo(destination))
                return Strafe(destination);

            return Walk(destination);
        }

        #region Settings

        public override float KiteDistance => Settings.KiteDistance;
        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public DemonHunterDefaultSettings Settings { get; } = new DemonHunterDefaultSettings();

        public sealed class DemonHunterDefaultSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _vengeance;
            private SkillSettings _vault;
            private float _kiteDistance;
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

            public SkillSettings Vault
            {
                get { return _vault; }
                set { SetField(ref _vault, value); }
            }

            public SkillSettings Vengeance
            {
                get { return _vengeance; }
                set { SetField(ref _vengeance, value); }
            }

            [DefaultValue(15f)]
            public float KiteDistance
            {
                get { return _kiteDistance; }
                set { SetField(ref _kiteDistance, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings VaultDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 1000,
                PrimaryResourcePct = 90f,
            };

            private static readonly SkillSettings VengeanceDefaults = new SkillSettings
            {
                UseMode = UseTime.Selective,
                Reasons = UseReasons.Elites | UseReasons.HealthEmergency,
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Vault = VaultDefaults.Clone();
                Vengeance = VengeanceDefaults.Clone();
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


