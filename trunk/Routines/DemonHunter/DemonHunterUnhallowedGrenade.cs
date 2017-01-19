﻿using System;
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
    public sealed class DemonHunterUnhallowedGrenade : DemonHunterBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Unhallowed Essense Grenade";
        public string Description => "Specialized combat routine for the Unhallowed Essense Grenade Build.";
        public string Author => "HitmanReborn";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/demon-hunter-grenade-build-with-unhallowed-essence-patch-2-4-3-season-9";

        /// <summary>
        /// The amount of time spent pausing between kite movements, during this time the bot will be free to attack.
        /// A longer duration will result in more damage dealt but also more damage taken.
        /// </summary>
        public override int KiteStutterDuration => 1500;

        /// <summary>
        /// The amount of time spent kiting before pausing to 'stutter'.
        /// A longer delay will result in the bot moving further away each time it kites.
        /// </summary>
        public override int KiteStutterDelay => 1000;

        public Build BuildRequirements => new Build
        {            
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.UnhallowedEssence, SetBonus.Third }
            },
            Items = new List<Item>
            {
                Legendary.HellcatWaistguard,
                Legendary.DepthDiggers
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.DemonHunter.Grenade, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityActor target;
            TrinityPower power;

            if (ShouldSmokeScreen())
                return SmokeScreen();

            if (TrySpecialPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            return null;
        }

        public TrinityPower GetBuffPower()
        {
            if (Skills.DemonHunter.Vengeance.CanCast())
                return Vengeance();
				
			if (Skills.DemonHunter.FanOfKnives.CanCast())
				return FanOfKnives();

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
            if (!Player.IsInTown && AllowedToUse(Settings.Vault, Skills.DemonHunter.Vault) && CanVaultTo(destination))
                return Vault(destination);

            return Walk(destination);
        }

        #region Settings

        public override float KiteDistance => Settings.KiteDistance;
        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public DemonHunterUnhallowedGrenadeSettings Settings { get; } = new DemonHunterUnhallowedGrenadeSettings();

        public sealed class DemonHunterUnhallowedGrenadeSettings : NotifyBase, IDynamicSetting
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
                UseMode = UseTime.Default,
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
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this, true);
            public void Reset() => LoadDefaults();
            public void Save() { }
     
            #endregion
        }

        #endregion

    }
}