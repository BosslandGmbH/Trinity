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
using Trinity.Components.Combat.Abilities;
using Trinity.Config;
using Trinity.Config.Combat;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Modules;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Attributes;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Reference;
using Trinity.Settings;
using Trinity.Technicals;
using Trinity.UI;
using Trinity.UIComponents;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Routines.DemonHunter
{
    public sealed class DemonHunterUnhallowed : DemonHunterBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Unhallowed Essense";
        public string Description => "Specialized combat routine for the Unhallowed Essense Multishot Build.";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/demon-hunter-multishot-fire-build-with-yangs-recurve-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {            
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.UnhallowedEssence, SetBonus.Third }
            },
            Items = new List<Item>
            {
                Legendary.YangsRecurve,
                Legendary.DeadMansLegacy
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.DemonHunter.Multishot, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityActor target;
            TrinityPower power;

            if (ShouldSmokeScreen())
                return SmokeScreen();

            if (ShouldRefreshBastiansGenerator && TryPrimaryPower(out power))
                return power;

            if (TrySpecialPower(out power))
                return power;

            if (TryMoveToBuffedSpot(out power, 30f, 16f))
                return power;

            if (ShouldMultishot(out target))
                return Multishot(target);

            if (TryPrimaryPower(out power))
                return power;

            return null;
        }

        public TrinityPower GetBuffPower()
        {
            if (Skills.DemonHunter.Vengeance.CanCast())
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
        public TrinityPower GetDefensivePower()
        {
            if (ShouldCaltrops())
                return Caltrops();

            if (ShouldShadowPower())
                return ShadowPower();

            return null;
        }

        public TrinityPower GetDestructiblePower()
        {
            return DefaultDestructiblePower();
        }

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (AllowedToUse(Settings.Vault, Skills.DemonHunter.Vault) && CanVaultTo(destination))
                return Vault(destination);

            return Walk(destination);
        }

        protected override bool ShouldMultishot(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.Multishot.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(80f))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected override bool ShouldEvasiveFire(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.EvasiveFire.CanCast())
                return false;

            if (!ShouldRefreshBastiansGenerator)
                return false;

            if (!TargetUtil.AnyMobsInRange(60f))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        #region Settings

        public override float KiteDistance => Settings.KiteDistance;
        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public DemonHunterUnhallowedSettings Settings { get; } = new DemonHunterUnhallowedSettings();

        public sealed class DemonHunterUnhallowedSettings : NotifyBase, IDynamicSetting
        {
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
            
            [DefaultValue(15f)]
            public float KiteDistance
            {
                get { return _kiteDistance; }
                set { SetField(ref _kiteDistance, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings DefaultVaultSettings = new SkillSettings
            {                
                SecondaryResourcePct = 90f,
                UseTime = UseTime.AnyTime,
                RecastDelayMs = 1000,
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Vault = DefaultVaultSettings.Clone();
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