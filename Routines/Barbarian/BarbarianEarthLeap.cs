using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Routines.Crusader;
using Trinity.Routines.DemonHunter;
using Trinity.UI;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Barbarian
{
    public sealed class BarbarianEarthLeap : BarbarianBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Barbarian Earth LeapQuake";
        public string Description => "This routine uses furious charge to move around and attack, building fury for Ancient Spear.";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.diablofans.com/builds/78024-2-4-1-boulder-raekor-barb-90";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.MightOfTheEarth, SetBonus.Third },
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Barbarian.Earthquake, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;
            Vector3 position;

            // Build Specific

            if (ShouldLeap(out position))
                return Leap(position);
           
            if (ShouldAncientSpear(out target))
                return AncientSpear(target);

            // Fallback to defaults in case of minor variations.

            if (TrySpecialPower(out power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;
            
            return Walk(Avoider.SafeSpot);
        }
     
        protected override bool ShouldLeap(out Vector3 position)
        {
            position = Vector3.Zero;
            if (!Skills.Barbarian.Leap.CanCast())
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected override bool ShouldAncientSpear(out TrinityActor target)
        {
            target = null;

            if (!Skills.Barbarian.AncientSpear.CanCast())
                return false;

            target = TargetUtil.BestAoeUnit(60, true).IsInLineOfSight
                ? TargetUtil.BestAoeUnit(60, true)
                : TargetUtil.GetBestClusterUnit(10, 60, false, true, false, true);

            if (target == null)
                return false;

            return target.Distance <= 60 && Player.PrimaryResourcePct > 0.95;
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();
        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();
        public TrinityPower GetBuffPower() => DefaultBuffPower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {           
            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public BarbarianEarthLeapSettings Settings { get; } = new BarbarianEarthLeapSettings();

        public sealed class BarbarianEarthLeapSettings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(8)]
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


