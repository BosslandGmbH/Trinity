using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;


namespace Trinity.Routines.Barbarian
{
    public sealed class BarbarianIKHota : BarbarianBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Barbarian IK Hammer of the Ancients";
        public string Description => "Empowered by the Immortal King set, this Barbarian build emphasises melee devastation and pulverises his enemies to the ground with Hammer of the Ancients Hammer of the Ancients, while maintaining a vast array of buffs and warriors at his side. ";
        public string Author => "xzjv & Nesox";
        public string Version => "0.2";
        public string Url => "https://www.icy-veins.com/d3/barbarian-hota-build-with-immortal-king-patch-2-6-1-season-12";

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
            TrinityActor target;
            Vector3 position;

            if (ShouldHammerOfTheAncients(out target))
                return HammerOfTheAncients(target);

            if (ShouldFuriousCharge(out position))
                return FuriousCharge(position);

            return Walk(Avoider.SafeSpot);
        }

        #region Furious Charge

        protected override bool ShouldFuriousCharge(out Vector3 position)
        {
            // Credit: phelon's raekor.

            position = Vector3.Zero;
            TrinityActor target = null;

            if (!Skills.Barbarian.FuriousCharge.CanCast())
                return false;

            var targetGoal = Math.Floor(5 * Core.Player.CooldownReductionPct);
            TrinityActor bestPierce = TargetUtil.GetBestPierceTarget(45);
            var bestPierceCount = bestPierce?.NearbyUnitsWithinDistance(7) ?? 0;
            TrinityActor bestTarget = TargetUtil.BestAoeUnit(45, true);
            var bestTargetCount = bestTarget?.NearbyUnitsWithinDistance(7) ?? 0;
            TrinityActor bestCluster = TargetUtil.GetBestClusterUnit(7, 45);
            var bestClusterCount = bestCluster?.NearbyUnitsWithinDistance(7) ?? 0;

            if (!Core.Buffs.HasCastingShrine)
            {
                if (bestTarget != null && TargetUtil.PierceHitsMonster(bestTarget.Position))
                {
                    if (bestTargetCount == 1 || bestTargetCount >= targetGoal)
                    {
                        position = GetPositionBehind(bestTarget.Position);
                        return true;
                    }
                }

                if (bestPierce != null && bestCluster != null && TargetUtil.PierceHitsMonster(bestPierce.Position) && TargetUtil.PierceHitsMonster(bestCluster.Position))
                {
                    if (bestPierceCount == 1 || bestPierceCount >= targetGoal &&
                        bestClusterCount == 1 || bestClusterCount >= targetGoal)
                    {
                        if (bestClusterCount > bestPierceCount)
                        {
                            position = GetPositionBehind(bestCluster.Position);
                            return true;
                        }
                        position = GetPositionBehind(bestPierce.Position);
                        return true;
                    }
                    if (bestPierceCount != 1 && bestPierceCount < targetGoal &&
                        (bestClusterCount == 1 || bestClusterCount >= targetGoal))
                    {
                        position = GetPositionBehind(bestCluster.Position);
                        return true;
                    }
                }

                if (bestPierce != null && TargetUtil.PierceHitsMonster(bestPierce.Position))
                {
                    if (bestClusterCount != 1 && bestClusterCount < targetGoal &&
                        (bestPierceCount == 1 || bestPierceCount >= targetGoal))
                    {
                        position = GetPositionBehind(bestPierce.Position);
                        return true;
                    }
                }

            }

            position = GetPositionBehind(CurrentTarget.Position);
            return true;
        }

        private static Vector3 GetPositionBehind(Vector3 position)
        {
            return MathEx.CalculatePointFrom(position, Player.Position, Player.Position.Distance(position) + 4f);
        }


        #endregion

        #region Hammer of the Ancients

        protected override bool ShouldHammerOfTheAncients(out TrinityActor target)
        {
            target = null;
            var skill = Skills.Barbarian.HammerOfTheAncients;

            if (!skill.CanCast() || Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            target = TargetUtil.ClosestUnit(30f, u => u.IsInLineOfSight) ?? CurrentTarget;
            return target != null;
        }

        #endregion

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (CanChargeTo(destination) && AllowedToUse(Settings.FuriousCharge, Skills.Barbarian.FuriousCharge))
            {
                if (IsInCombat && TargetUtil.PierceHitsMonster(destination) || Player.Position.Distance(destination) > 20f)
                {
                    return FuriousCharge(destination);
                }
            }
            return Walk(destination);
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();
        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetBuffPower()
        {
            if (Player.IsInTown)
                return null;

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

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public BarbarianIKHotaSettings Settings { get; } = new BarbarianIKHotaSettings();

        public sealed class BarbarianIKHotaSettings : NotifyBase, IDynamicSetting
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

            #region FuriousCharge

            public SkillSettings FuriousCharge
            {
                get { return _furiousCharge; }
                set { SetField(ref _furiousCharge, value); }
            }

            private static readonly SkillSettings VaultDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 1000,
                Reasons = UseReasons.Blocked
            };

            private SkillSettings _furiousCharge;

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                FuriousCharge = VaultDefaults.Clone();
            }

            #endregion

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


