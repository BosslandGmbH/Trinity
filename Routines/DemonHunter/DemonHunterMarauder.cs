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
using Trinity.Framework.Modules;
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
    public sealed class DemonHunterMarauder : DemonHunterBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Marauder Cluster Arrow Routine";
        public string Description => "A focus on sentry placement and cluster arrow from range.";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/demon-hunter-sentry-cluster-arrow-build-with-the-embodiment-of-the-marauder-set-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.EmbodimentOfTheMarauder, SetBonus.Third }
            },
            Items = new List<Item>
            {
                Legendary.Manticore,
                Legendary.BombardiersRucksack
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.DemonHunter.ClusterArrow, null },
            },
        };

        #endregion

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

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();


        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (!Player.IsInTown && AllowedToUse(Settings.Vault, Skills.DemonHunter.Vault) && CanVaultTo(destination))
                return Vault(destination);            
            
            if (!Player.IsInTown && CanStrafeTo(destination))
                return Strafe(destination);

            return Walk(destination);
        }

        #region Settings

        public override float KiteDistance => Settings.KiteDistance;
        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public DemonHunterMarauderSettings Settings { get; } = new DemonHunterMarauderSettings();

        public sealed class DemonHunterMarauderSettings : NotifyBase, IDynamicSetting
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


