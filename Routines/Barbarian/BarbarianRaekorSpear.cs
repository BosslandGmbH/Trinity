using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;


namespace Trinity.Routines.Barbarian
{
    public sealed class BarbarianRaekorSpear : BarbarianBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Barbarian Raekor Ancient Spear";
        public string Description => "This routine uses furious charge to move around and attack, building fury for Ancient Spear.";
        public string Author => "Phelon"; // (Ported)
        public string Version => "0.1";
        public string Url => "http://www.diablofans.com/builds/78024-2-4-1-boulder-raekor-barb-90";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.TheLegacyOfRaekor, SetBonus.Third },
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Barbarian.FuriousCharge, null },
                { Skills.Barbarian.AncientSpear, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;
            Vector3 position;

            // Build Specific

            if (ShouldAncientSpear(out target))
                return AncientSpear(target);

            if (TrySpecialPower(out power))
                return power;

            if (ShouldFuriousCharge(out position))
                return FuriousCharge(position);

            // Fallback to defaults in case of minor variations.

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            return Walk(TargetUtil.GetLoiterPosition(TargetUtil.GetBestClusterUnit(), 10f));
        }

        protected override bool ShouldAncientSpear(out TrinityActor target)
        {
            // Credit: phelon's raekor.

            target = null;

            if (!Skills.Barbarian.AncientSpear.CanCast())
                return false;

            if (Sets.TheLegacyOfRaekor.IsFullyEquipped && RaekorDamageStacks < 5)
                return false;

            target = TargetUtil.BestAoeUnit(60, true).IsInLineOfSight
                ? TargetUtil.BestAoeUnit(60, true)
                : TargetUtil.GetBestClusterUnit(10, 60, false, true, false, true);

            if (target == null)
                return false;

            return target.Distance <= 60 && Player.PrimaryResourcePct > 0.95;
        }

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

        public TrinityPower GetDefensivePower() => GetBuffPower();

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetBuffPower()
        {
            if (ShouldIgnorePain())
                return IgnorePain();

            if (ShouldSprint())
                return Sprint();

            if (ShouldBattleRage())
                return BattleRage();

            if (ShouldWarCry())
                return WarCry();

            if (ShouldCallOfTheAncients())
                return CallOfTheAncients();
            
            if (ShouldWrathOfTheBerserker())
                return WrathOfTheBerserker();

            return null;
        }

        private readonly List<TrinityObjectType> _importantActors = new List<TrinityObjectType>
        {
            TrinityObjectType.ProgressionGlobe,
            TrinityObjectType.HealthGlobe,
            TrinityObjectType.ProgressionGlobe,
        };

        public TrinityPower GetMovementPower(Vector3 destination)
        {           
            if (CanChargeTo(destination) && Skills.Barbarian.FuriousCharge.TimeSinceUse > 500)
            {
                // Limit movement casts so we have stacks to charge units and build more stacks.

                var chargeStacks = Skills.Barbarian.FuriousCharge.BuffStacks;
                var isImportantTarget = CurrentTarget != null && _importantActors.Contains(CurrentTarget.Type);

                if (IsInCombat && (chargeStacks == 3 || isImportantTarget && chargeStacks > 1) || TargetUtil.PierceHitsMonster(destination))
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
        public BarbarianRaekorSpearSettings Settings { get; } = new BarbarianRaekorSpearSettings();

        public sealed class BarbarianRaekorSpearSettings : NotifyBase, IDynamicSetting
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
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this, true);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}


