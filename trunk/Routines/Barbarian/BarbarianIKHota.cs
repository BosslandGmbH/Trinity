﻿using System;
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
    public sealed class BarbarianIKHota : BarbarianBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Barbarian IK Hammer of the Ancients";
        public string Description => "Empowered by the Immortal King set, this Barbarian build emphasises melee devastation and pulverises his enemies to the ground with Hammer of the Ancients Hammer of the Ancients, while maintaining a vast array of buffs and warriors at his side. ";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/barbarian-hota-fire-build-with-immortal-king-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.ImmortalKingsCall, SetBonus.Third },
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Barbarian.HammerOfTheAncients, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;
            Vector3 position;

            if (ShouldFuriousCharge(out position))
                return FuriousCharge(position);

            if (ShouldLeap(out position))
                return Leap(position);

            if (TrySpecialPower(out power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;
            
            return Walk(Avoider.SafeSpot);
        }

        protected override bool ShouldFuriousCharge(out Vector3 position)
        {
            position = Vector3.Zero;
            var skill = Skills.Barbarian.FuriousCharge;           

            if (!skill.CanCast())
                return false;
            
            if (skill.TimeSinceUse < 2000)
                return false;

            if (!TargetUtil.AnyMobsInRange(60f) && !IsNoPrimary)
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();
        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetBuffPower()
        {
            TrinityPower trinityPower;
            if (TryProcBandOfMight(out trinityPower))
                return trinityPower;

            return DefaultBuffPower();
        }        

        private static bool TryProcBandOfMight(out TrinityPower power)
        {
            power = null;

            if (Player.IsInTown)
                return false;

            if (!Legendary.BandOfMight.IsEquipped)
                return false;

            if (IsBandOfMightBuffActive && Skills.Barbarian.FuriousCharge.TimeSinceUse < 7500)
                return false;               
        
            if (Skills.Barbarian.FuriousCharge.CanCast())
            {          
                power = FuriousCharge(TargetUtil.GetBestClusterPoint());
                return true;               
            }

            if (Skills.Barbarian.GroundStomp.CanCast())
            {
                power = GroundStomp(Player.Position);
                return true;
            }

            if (Skills.Barbarian.Leap.CanCast())
            {
                power = Leap(TargetUtil.GetBestClusterPoint());
                return true;
            }

            return false;
        }

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (CanChargeTo(destination) && Skills.Barbarian.FuriousCharge.TimeSinceUse > 500)
            {
                if (IsInCombat && TargetUtil.PierceHitsMonster(destination) || Player.Position.Distance(destination) > 20f)
                {   
                    return FuriousCharge(destination);
                }
            }
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


