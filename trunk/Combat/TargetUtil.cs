using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Combat;
using Trinity.Combat.Abilities;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Movement;
using Trinity.Objects;
using Trinity.Technicals;
using Trinity.UI.UIComponents.RadarCanvas;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity
{
    class TargetUtil
    {
        public static void ClearCurrentTarget(string reason)
        {
            if (TrinityPlugin.CurrentTarget != null)
            {
                var clearString = "Clearing CURRENT TARGET: " + reason +
                        $"{Environment.NewLine} Name: {CurrentTarget.InternalName} Type: {CurrentTarget.Type} SNO: {CurrentTarget.ActorSNO} Distance: {CurrentTarget.Distance} " +
                        $"{Environment.NewLine} Weight: {CurrentTarget.Weight} Info: {CurrentTarget.WeightInfo}";
                Logger.LogVerbose(clearString);
                TrinityPlugin.CurrentTarget = null;
            }
        }

        #region Helper fields

        private static List<TrinityCacheObject> ObjectCache
        {
            get
            {
                return TrinityPlugin.ObjectCache;
            }
        }
        private static CacheData.PlayerCache Player
        {
            get
            {
                return CacheData.Player;
            }
        }
        private static bool AnyTreasureGoblinsPresent
        {
            get
            {
                if (ObjectCache != null)
                    return ObjectCache.Any(u => u.IsTreasureGoblin);
                else
                    return false;
            }
        }
        private static TrinityCacheObject CurrentTarget
        {
            get
            {
                return TrinityPlugin.CurrentTarget;
            }
        }
        private static HashSet<SNOPower> Hotbar
        {
            get
            {
                return CacheData.Hotbar.ActivePowers;
            }
        }

        #endregion

        public static int CountUnitsBehind(TrinityCacheObject actor, float range)
        {
            return
                (from u in ObjectCache
                 where u.RActorGuid != actor.RActorGuid &&
                       u.IsUnit &&
                       MathUtil.IntersectsPath(actor.Position, actor.Radius, TrinityPlugin.Player.Position, u.Position)
                 select u).Count();
        }

        public static int CountUnitsInFront(TrinityCacheObject actor)
        {
            return
                (from u in TrinityPlugin.ObjectCache
                 where u.RActorGuid != actor.RActorGuid &&
                       u.IsUnit &&
                       MathUtil.IntersectsPath(u.Position, u.Radius, TrinityPlugin.Player.Position, actor.Position)
                 select u).Count();
        }

        public static bool IsFacing(TrinityCacheObject actor, Vector3 targetPosition, float arcDegrees = 70f)
        {
            if (actor.DirectionVector != Vector2.Zero)
            {
                var u = targetPosition - actor.Position;
                u.Z = 0f;
                var v = new Vector3(actor.DirectionVector.X, actor.DirectionVector.Y, 0f);
                var result = ((MathEx.ToDegrees(Vector3.AngleBetween(u, v)) <= arcDegrees) ? 1 : 0) != 0;
                return result;
            }
            return false;
        }

        public static int NearbyUnitsWithinDistance(TrinityCacheObject actor, float range = 5f)
        {
            using (new PerformanceLogger("CacheObject.UnitsNear"))
            {
                if (actor.Type != TrinityObjectType.Unit)
                    return 0;

                return TrinityPlugin.ObjectCache
                    .Count(u => u.RActorGuid != actor.RActorGuid && u.IsUnit && u.Position.Distance(actor.Position) <= range && u.HasBeenInLoS);
            }
        }

        /// <summary>
        /// Gets the number of units facing player
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static int UnitsFacingPlayer(float range)
        {
            return
                (from u in ObjectCache
                 where u.IsUnit &&
                 u.IsFacingPlayer
                 select u).Count();
        }

        /// <summary>
        /// Gets the number of units player is facing
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static int UnitsPlayerFacing(float range, float arcDegrees = 70f)
        {
            return
                (from u in ObjectCache
                 where u.IsUnit &&
                 u.IsPlayerFacing(arcDegrees)
                 select u).Count();
        }

        /// <summary>
        /// If ignoring elites, checks to see if enough trash trash pack are around
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static bool EliteOrTrashInRange(float range)
        {
            if (CombatBase.IgnoringElites)
            {
                return
                    (from u in ObjectCache
                     where u.IsUnit &&
                     !u.IsEliteRareUnique &&
                     u.Weight > 0 &&
                     u.RadiusDistance <= CombatBase.CombatOverrides.EffectiveTrashRadius
                     select u).Count() >= CombatBase.CombatOverrides.EffectiveTrashSize;
            }
            else
            {
                return
                    (from u in ObjectCache
                     where u.IsUnit && u.IsValid &&
                     u.Weight > 0 &&
                     u.IsBossOrEliteRareUnique &&
                     u.RadiusDistance <= range
                     select u).Any();
            }

        }


        /// <summary>
        /// Checks to make sure there's at least one valid cluster with the minimum monster count
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="maxRange"></param>
        /// <param name="minCount"></param>
        /// <param name="forceElites"></param>
        /// <returns></returns>
        internal static bool ClusterExists(float radius = 15f, int minCount = 2)
        {
            return ClusterExists(radius, 300f, minCount, false);
        }
        /// <summary>
        /// Checks to make sure there's at least one valid cluster with the minimum monster count
        /// </summary>
        internal static bool ClusterExists(float radius = 15f, float maxRange = 90f, int minCount = 2, bool forceElites = true)
        {
            if (radius < 2f)
                radius = 2f;
            if (maxRange > 300f)
                maxRange = 300f;
            if (minCount < 1)
                minCount = 1;

            if (forceElites && ObjectCache.Any(u => u.IsUnit && u.IsBossOrEliteRareUnique && u.RadiusDistance < maxRange))
                return true;

            var clusterCheck =
                (from u in ObjectCache
                 where u.IsUnit && u.IsValid &&
                 u.RadiusDistance <= maxRange &&
                 u.NearbyUnitsWithinDistance(radius)-1 >= minCount
                 select u).Any();

            return clusterCheck;
        }
        /// <summary>
        /// Return a cluster of specified size and radius
        /// </summary>
        internal static Vector3 GetClusterPoint(float clusterRadius = 15f, int minCount = 2)
        {
            if (clusterRadius < 5f)
                clusterRadius = 5f;
            if (minCount < 1)
                minCount = 1;

            if (CurrentTarget == null)
                return Player.Position;

            if (ObjectCache.Any(u => u.IsUnit && u.IsBossOrEliteRareUnique && u.RadiusDistance < 200))
                return CurrentTarget.Position;

            var clusterUnit =
                (from u in ObjectCache
                 where u.IsUnit && u.IsValid &&
                 u.RadiusDistance <= 200 &&
                 u.NearbyUnitsWithinDistance(clusterRadius) >= minCount
                 orderby u.NearbyUnitsWithinDistance(clusterRadius)
                 select u).FirstOrDefault();

            if (clusterUnit == null)
                return CurrentTarget.Position;

            return clusterUnit.Position;
        }

        internal static List<TrinityCacheObject> TargetsInFrontOfMe(float maxRange, bool ignoreUnitsInAoE = false, bool ignoreElites = false)
        {
            return (from u in ObjectCache
                    where u.IsUnit &&
                    u.RadiusDistance <= maxRange && u.HasBeenInLoS &&
                    !(ignoreUnitsInAoE && u.IsStandingInAvoidance) &&
                    !(ignoreElites && u.IsEliteRareUnique)
                    orderby u.CountUnitsInFront() descending,
                    u.IsEliteRareUnique descending
                    select u).ToList();
        }
        internal static TrinityCacheObject GetBestPierceTarget(float maxRange, bool ignoreUnitsInAoE = false, bool ignoreElites = false)
        {
            var result = TargetsInFrontOfMe(maxRange, ignoreUnitsInAoE, ignoreElites).FirstOrDefault();
            if (result != null)
                return result;

            if (CurrentTarget != null)
                return CurrentTarget;

            return GetBestClusterUnit(15f, maxRange, 1, true, !ignoreUnitsInAoE, ignoreElites);
        }

        internal static TrinityCacheObject GetBestArcTarget(float maxRange, float arcDegrees)
        {
            var result =
                (from u in ObjectCache
                 where u.IsUnit &&
                 u.RadiusDistance <= maxRange && !u.IsSafeSpot
                 orderby u.CountUnitsInFront() descending
                 select u).FirstOrDefault();

            if (result != null)
                return result;

            if (CurrentTarget != null)
                return CurrentTarget;
            return GetBestClusterUnit(15f, maxRange);
        }

        private static Vector3 GetBestAoEMovementPosition()
        {
            Vector3 _bestMovementPosition = Vector3.Zero;

            if (HealthGlobeExists(25) && Player.CurrentHealthPct < TrinityPlugin.Settings.Combat.Barbarian.HealthGlobeLevel)
                _bestMovementPosition = GetBestHealthGlobeClusterPoint(7, 25);
            else if (PowerGlobeExists(25))
                _bestMovementPosition = GetBestPowerGlobeClusterPoint(7, 25);
            else if (GetFarthestClusterUnit(7, 25, 4) != null && !CurrentTarget.IsEliteRareUnique && !CurrentTarget.IsTreasureGoblin)
                _bestMovementPosition = GetFarthestClusterUnit(7, 25).Position;
            else if (_bestMovementPosition == Vector3.Zero)
                _bestMovementPosition = CurrentTarget.Position;

            return _bestMovementPosition;
        }

        internal static TrinityCacheObject GetFarthestClusterUnit(float aoe_radius = 25f, float maxRange = 65f, int count = 1, bool useWeights = true, bool includeUnitsInAoe = true)
        {
            if (aoe_radius < 1f)
                aoe_radius = 1f;
            if (maxRange > 300f)
                maxRange = 300f;

            using (new PerformanceLogger("TargetUtil.GetFarthestClusterUnit"))
            {
                TrinityCacheObject bestClusterUnit;
                var clusterUnits =
                    (from u in ObjectCache
                     where ((useWeights && u.Weight > 0) || !useWeights) &&
                     (includeUnitsInAoe || !UnitOrPathInAoE(u)) &&
                     u.RadiusDistance <= maxRange &&
                     u.NearbyUnitsWithinDistance(aoe_radius) >= count
                     orderby u.Type != TrinityObjectType.HealthGlobe && u.Type != TrinityObjectType.PowerGlobe,
                     u.NearbyUnitsWithinDistance(aoe_radius),
                     u.Distance descending
                     select u).ToList();

                if (clusterUnits.Any())
                    bestClusterUnit = clusterUnits.FirstOrDefault();
                else if (TrinityPlugin.CurrentTarget != null)
                    bestClusterUnit = TrinityPlugin.CurrentTarget;
                else
                    bestClusterUnit = default(TrinityCacheObject);

                return bestClusterUnit;
            }
        }
        /// <summary>
        /// Finds the optimal cluster position, works regardless if there is a cluster or not (will return single unit position if not). This is not a K-Means cluster, but rather a psuedo cluster based
        /// on the number of other monsters within a radius of any given unit
        /// </summary>
        /// <param name="radius">The maximum distance between monsters to be considered part of a cluster</param>
        /// <param name="maxRange">The maximum unit range to include, units further than this will not be checked as a cluster center but may be included in a cluster</param>
        /// <param name="useWeights">Whether or not to included un-weighted (ignored) targets in the cluster finding</param>
        /// <param name="includeUnitsInAoe">Checks the cluster point for AoE effects</param>
        /// <returns>The Vector3 position of the unit that is the ideal "center" of a cluster</returns>
        internal static Vector3 GetBestHealthGlobeClusterPoint(float radius = 15f, float maxRange = 65f, bool useWeights = true, bool includeUnitsInAoe = true)
        {
            if (radius < 5f)
                radius = 5f;
            if (maxRange > 30f)
                maxRange = 30f;

            Vector3 bestClusterPoint;
            var clusterUnits =
                (from u in ObjectCache
                 where u.Type == TrinityObjectType.HealthGlobe &&
                 (includeUnitsInAoe || !UnitOrPathInAoE(u)) &&
                 u.RadiusDistance <= maxRange
                 orderby u.NearbyUnitsWithinDistance(radius),
                 u.Distance descending
                 select u.Position).ToList();

            if (clusterUnits.Any())
                bestClusterPoint = clusterUnits.FirstOrDefault();
            else
                bestClusterPoint = TrinityPlugin.Player.Position;

            return bestClusterPoint;

        }
        /// <summary>
        /// Finds the optimal cluster position, works regardless if there is a cluster or not (will return single unit position if not). This is not a K-Means cluster, but rather a psuedo cluster based
        /// on the number of other monsters within a radius of any given unit
        /// </summary>
        /// <param name="radius">The maximum distance between monsters to be considered part of a cluster</param>
        /// <param name="maxRange">The maximum unit range to include, units further than this will not be checked as a cluster center but may be included in a cluster</param>
        /// <param name="useWeights">Whether or not to included un-weighted (ignored) targets in the cluster finding</param>
        /// <param name="includeUnitsInAoe">Checks the cluster point for AoE effects</param>
        /// <returns>The Vector3 position of the unit that is the ideal "center" of a cluster</returns>
        internal static Vector3 GetBestPowerGlobeClusterPoint(float radius = 15f, float maxRange = 65f, bool useWeights = true, bool includeUnitsInAoe = true)
        {
            if (radius < 5f)
                radius = 5f;
            if (maxRange > 30f)
                maxRange = 30f;

            Vector3 bestClusterPoint;
            var clusterUnits =
                (from u in ObjectCache
                 where u.Type == TrinityObjectType.PowerGlobe &&
                 (includeUnitsInAoe || !UnitOrPathInAoE(u)) &&
                 u.RadiusDistance <= maxRange
                 orderby u.NearbyUnitsWithinDistance(radius),
                 u.Distance descending
                 select u.Position).ToList();

            if (clusterUnits.Any())
                bestClusterPoint = clusterUnits.FirstOrDefault();
            else
                bestClusterPoint = TrinityPlugin.Player.Position;

            return bestClusterPoint;
        }
        /// <summary>
        /// Checks to see if there is a health globe around to grab
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        internal static bool HealthGlobeExists(float radius = 15f)
        {
            var clusterCheck =
                (from u in ObjectCache
                 where u.Type == TrinityObjectType.HealthGlobe && !UnitOrPathInAoE(u) &&
                 u.RadiusDistance <= radius
                 select u).Any();

            return clusterCheck;
        }

        /// <summary>
        /// Checks to see if there is a health globe around to grab
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        internal static bool PowerGlobeExists(float radius = 15f)
        {
            var clusterCheck =
                (from u in ObjectCache
                 where u.Type == TrinityObjectType.PowerGlobe && !UnitOrPathInAoE(u) &&
                 u.RadiusDistance <= radius
                 select u).Any();

            return clusterCheck;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius">Cluster Radius</param>
        /// <param name="maxRange">Unit Max Distance</param>
        /// <param name="count">Minimum number of mobs</param>
        /// <param name="useWeights">Include Mobs with Weight or not</param>
        /// <param name="includeUnitsInAoe">Include mobs in AoE or not</param>
        /// <param name="ignoreElites">Ingore elites or not/param>
        /// <returns></returns>
        internal static TrinityCacheObject GetBestClusterUnit(
            float radius = 15f, float maxRange = 65f, int count = 1, bool useWeights = true, bool includeUnitsInAoe = true, bool ignoreElites = false)
        {
            if (radius < 1f)
                radius = 1f;
            if (maxRange > 300f)
                maxRange = 300f;

            TrinityCacheObject bestClusterUnit;
            var clusterUnits =
                (from u in ObjectCache
                 where u.IsUnit &&
                 ((useWeights && u.Weight > 0) || !useWeights) &&
                 (includeUnitsInAoe || !UnitOrPathInAoE(u)) &&
                 !(ignoreElites && u.IsEliteRareUnique) &&
                 u.RadiusDistance <= maxRange && !u.IsSafeSpot
                 orderby u.IsTrashMob,
                  u.NearbyUnitsWithinDistance(radius) descending,
                  u.Distance,
                  u.HitPointsPct descending
                 select u).ToList();

            if (clusterUnits.Any())
                bestClusterUnit = clusterUnits.FirstOrDefault();
            else if (TrinityPlugin.CurrentTarget != null)
                bestClusterUnit = TrinityPlugin.CurrentTarget;
            else
                bestClusterUnit = default(TrinityCacheObject);

            return bestClusterUnit;
        }

        /// <summary>
        /// Finds the optimal cluster position, works regardless if there is a cluster or not (will return single unit position if not). This is not a K-Means cluster, but rather a psuedo cluster based
        /// on the number of other monsters within a radius of any given unit
        /// </summary>
        /// <param name="radius">The maximum distance between monsters to be considered part of a cluster</param>
        /// <param name="maxRange">The maximum unit range to include, units further than this will not be checked as a cluster center but may be included in a cluster</param>
        /// <param name="useWeights">Whether or not to included un-weighted (ignored) targets in the cluster finding</param>
        /// <param name="includeUnitsInAoe">Checks the cluster point for AoE effects</param>
        /// <returns>The Vector3 position of the unit that is the ideal "center" of a cluster</returns>
        internal static Vector3 GetBestClusterPoint(float radius = 15f, float maxRange = 65f, bool useWeights = true, bool includeUnitsInAoe = true)
        {
            if (radius < 5f)
                radius = 5f;
            if (maxRange > 300f)
                maxRange = 300f;

            bool includeHealthGlobes = TrinityPlugin.Settings.Combat.Misc.CollectHealthGlobe &&
                                       ObjectCache.Any(g => g.Type == TrinityObjectType.HealthGlobe && g.Weight > 0) &&
                                       (ClassMover.IsSpecialMovementReady || !PlayerMover.IsBlocked);


            Vector3 bestClusterPoint;
            var clusterUnits =
                (from u in ObjectCache
                 where (u.IsUnit || (includeHealthGlobes && u.Type == TrinityObjectType.HealthGlobe)) &&
                 ((useWeights && u.Weight > 0) || !useWeights) &&
                 (includeUnitsInAoe || !UnitOrPathInAoE(u)) &&
                 u.RadiusDistance <= maxRange
                 orderby u.Type != TrinityObjectType.HealthGlobe, // if it's a globe this will be false and sorted at the top
                 u.IsTrashMob,
                 u.NearbyUnitsWithinDistance(radius) descending,
                 u.Distance,
                 u.HitPointsPct descending
                 select u.Position).ToList();

            if (clusterUnits.Any())
                bestClusterPoint = clusterUnits.FirstOrDefault();
            else if (TrinityPlugin.CurrentTarget != null)
                bestClusterPoint = TrinityPlugin.CurrentTarget.Position;
            else
                bestClusterPoint = TrinityPlugin.Player.Position;

            return bestClusterPoint;
        }
        /// <summary>
        /// Fast check to see if there are any attackable units within a certain distance
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static bool AnyMobsInRange(float range = 10f)
        {
            return AnyMobsInRange(range, 1);
        }
        /// <summary>
        /// Fast check to see if there are any attackable units within a certain distance
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static bool AnyMobsInRange(float range = 10f, bool useWeights = true)
        {
            return AnyMobsInRange(range, 1, useWeights);
        }
        /// <summary>
        /// Fast check to see if there are any attackable units within a certain distance
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static bool AnyMobsInRange(float range = 10f, int minCount = 1, bool useWeights = true)
        {
            if (range < 5f)
                range = 5f;
            if (minCount < 1)
                minCount = 1;
            return (from o in ObjectCache
                where o.IsUnit && o.HitPoints > 0 &&
                     ((useWeights && o.Weight > 0) || !useWeights) &&
                    o.RadiusDistance <= range
                    select o).Count() >= minCount;
        }
        /// <summary>
        /// Checks if there are any mobs in range of the specified position
        /// </summary>
        internal static bool AnyMobsInRangeOfPosition(Vector3 position, float range = 15f, int unitsRequired = 1)
        {
            var inRangeCount = (from u in ObjectCache
                                where u.IsUnit && u.IsValid &&
                                        u.Weight > 0 &&
                                        u.Position.Distance(position) <= range
                                select u).Count();

            return inRangeCount >= unitsRequired;
        }
        /// <summary>
        /// Checks if there are any mobs in range of the specified position
        /// </summary>
        internal static int NumMobsInRangeOfPosition(Vector3 position, float range = 15f)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                            u.Weight > 0 &&
                            u.Position.Distance(position) <= range
                    select u).Count();
        }
        /// <summary>
        /// Checks if there are any mobs in range of the specified position
        /// </summary>
        internal static int NumMobsInRange(float range = 15f)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                            u.Weight > 0 &&
                            u.Position.Distance(Player.Position) <= range
                    select u).Count();
        }
        /// <summary>
        /// Checks if there are any bosses in range of the specified position
        /// </summary>
        internal static int NumBossInRangeOfPosition(Vector3 position, float range = 15f)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                            u.Weight > 0 &&
                            u.IsBoss &&
                            u.Position.Distance(position) <= range
                    select u).Count();
        }
        /// <summary>
        /// Returns list of units within the specified range
        /// </summary>
        internal static List<TrinityCacheObject> ListUnitsInRangeOfPosition(Vector3 position, float range = 15f)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                        u.Weight > 0 &&
                        u.Position.Distance(position) <= range
                    select u).ToList();
        }

        internal static bool AnyTrashInRange(float range = 10f, int minCount = 1, bool useWeights = true)
        {
            if (range < 5f)
                range = 5f;
            if (minCount < 1)
                minCount = 1;
            return (from o in ObjectCache
                    where o.IsUnit && o.IsTrashMob &&
                     ((useWeights && o.Weight > 0) || !useWeights) &&
                    o.RadiusDistance <= range
                    select o).Count() >= minCount;
        }
        /// <summary>
        /// Fast check to see if there are any attackable Elite units within a certain distance
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static bool AnyElitesInRange(float range = 10f)
        {
            if (CombatBase.IgnoringElites)
                return false;

            if (range < 5f)
                range = 5f;
            return (from o in ObjectCache
                    where o.IsUnit &&
                    o.IsBossOrEliteRareUnique &&
                    o.RadiusDistance <= range
                    select o).Any();
        }
        /// <summary>
        /// Fast check to see if there are any attackable Elite units within a certain distance
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static bool AnyElitesInRange(float range = 10f, int minCount = 1)
        {
            if (CombatBase.IgnoringElites)
                return false;

            if (range < 5f)
                range = 5f;
            if (minCount < 1)
                minCount = 1;
            return (from o in ObjectCache
                    where o.IsUnit &&
                    o.IsBossOrEliteRareUnique &&
                    o.RadiusDistance <= range
                    select o).Count() >= minCount;
        }
        /// <summary>
        /// Checks if there are any mobs in range of the specified position
        /// </summary>
        internal static bool AnyElitesInRangeOfPosition(Vector3 position, float range = 15f, int unitsRequired = 1)
        {
            var inRangeCount = (from u in ObjectCache
                                where u.IsUnit && u.IsValid &&
                                        u.Weight > 0 &&
                                        u.IsBossOrEliteRareUnique &&
                                        u.Position.Distance(position) <= range
                                select u).Count();

            return inRangeCount >= unitsRequired;
        }
        /// <summary>
        /// Count of elites within range of position
        /// </summary>
        internal static int NumElitesInRangeOfPosition(Vector3 position, float range = 15f)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                            u.Weight > 0 &&
                            u.IsBossOrEliteRareUnique &&
                            u.Position.Distance(position) <= range
                    select u).Count();
        }
        /// <summary>
        /// Returns true if there is any elite units within the given range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal static bool IsEliteTargetInRange(float range = 10f)
        {
            if (range < 5f)
                range = 5f;
            return TrinityPlugin.CurrentTarget != null && TrinityPlugin.CurrentTarget.IsBossOrEliteRareUnique && TrinityPlugin.CurrentTarget.RadiusDistance <= range;
        }

        /// <summary>
        /// Finds an optimal position for using Monk Tempest Rush out of combat
        /// </summary>
        /// <returns></returns>
        internal static Vector3 FindTempestRushTarget()
        {
            Vector3 target = PlayerMover.LastMoveToTarget;
            Vector3 myPos = ZetaDia.Me.Position;

            if (TrinityPlugin.CurrentTarget != null && NavHelper.CanRayCast(myPos, target))
            {
                target = TrinityPlugin.CurrentTarget.Position;
            }

            float distance = target.Distance(myPos);

            if (distance < 30f)
            {
                double direction = MathUtil.FindDirectionRadian(myPos, target);
                target = MathEx.GetPointAt(myPos, 40f, (float)direction);
            }

            return target;
        }

        // Special Zig-Zag movement for whirlwind/tempest
        /// <summary>
        /// Finds an optimal position for Barbarian Whirlwind, Monk Tempest Rush, or Demon Hunter Strafe
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="ringDistance"></param>
        /// <param name="randomizeDistance"></param>
        /// <returns></returns>
        internal static Vector3 GetZigZagTarget(Vector3 origin, float ringDistance, bool randomizeDistance = false)
        {
            if (CurrentTarget == null)
                return Player.Position;

            const float minDistance = 5f;
            Vector3 myPos = Player.Position;

            Vector3 zigZagPoint;

            bool useTargetBasedZigZag = false;
            float maxDistance = 35f;
            int minTargets = 2;

            if (TrinityPlugin.Player.ActorClass == ActorClass.Monk)
            {
                maxDistance = 20f;
                minTargets = 3;
                useTargetBasedZigZag = TrinityPlugin.Settings.Combat.Monk.TargetBasedZigZag;
            }
            if (TrinityPlugin.Player.ActorClass == ActorClass.Barbarian)
            {
                useTargetBasedZigZag = TrinityPlugin.Settings.Combat.Barbarian.TargetBasedZigZag;
            }

            if (useTargetBasedZigZag && ObjectCache.Count(o => o.IsUnit) >= minTargets)
            {
                bool attackInAoe = TrinityPlugin.Settings.Combat.Misc.KillMonstersInAoE;
                var clusterPoint = GetBestClusterPoint(ringDistance, ringDistance, false, attackInAoe);
                if (clusterPoint.Distance(Player.Position) >= minDistance)
                {
                    Logger.Log(LogCategory.Movement, "Returning ZigZag: BestClusterPoint {0} r-dist={1} t-dist={2}", clusterPoint, ringDistance, clusterPoint.Distance(Player.Position));
                    return clusterPoint;
                }


                List<TrinityCacheObject> zigZagTargetList;
                if (attackInAoe)
                {
                    zigZagTargetList =
                        (from u in ObjectCache
                         where u.IsUnit && u.Distance < maxDistance
                         select u).ToList();
                }
                else
                {
                    zigZagTargetList =
                        (from u in ObjectCache
                         where u.IsUnit && u.Distance < maxDistance && !UnitOrPathInAoE(u)
                         select u).ToList();
                }

                if (zigZagTargetList.Count() >= minTargets)
                {
                    zigZagPoint = zigZagTargetList.OrderByDescending(u => u.Distance).FirstOrDefault().Position;
                    if (NavHelper.CanRayCast(zigZagPoint) && zigZagPoint.Distance(Player.Position) >= minDistance)
                    {
                        Logger.Log(LogCategory.Movement, "Returning ZigZag: TargetBased {0} r-dist={1} t-dist={2}", zigZagPoint, ringDistance, zigZagPoint.Distance(Player.Position));
                        return zigZagPoint;
                    }
                }
            }

            float highestWeightFound = float.NegativeInfinity;
            Vector3 bestLocation = origin;

            // the unit circle always starts at 0 :)
            const double min = 0;
            // the maximum size of a unit circle
            const double max = 2 * Math.PI;
            // the number of times we will iterate around the circle to find points
            const double piSlices = 16;

            // We will do several "passes" to make sure we can get a point that we can least zig-zag to
            // The total number of points tested will be piSlices * distancePasses.Count
            List<float> distancePasses = new List<float> { ringDistance * 1 / 2, ringDistance * 3 / 4, ringDistance };

            foreach (float distance in distancePasses)
            {
                for (double direction = min; direction < max; direction += (Math.PI / piSlices))
                {
                    // Starting weight is 1
                    float pointWeight = 1f;

                    // Find a new XY
                    zigZagPoint = MathEx.GetPointAt(origin, distance, (float)direction);
                    // Get the Z
                    zigZagPoint.Z = TrinityPlugin.MainGridProvider.GetHeight(zigZagPoint.ToVector2());

                    // Make sure we're actually zig-zagging our target, except if we're kiting

                    float targetCircle = CurrentTarget.Radius;
                    if (targetCircle <= 5f)
                        targetCircle = 5f;
                    if (targetCircle > 10f)
                        targetCircle = 10f;

                    bool intersectsPath = MathUtil.IntersectsPath(CurrentTarget.Position, targetCircle, myPos, zigZagPoint);
                    if (CombatBase.KiteDistance <= 0 && !intersectsPath)
                        continue;

                    // if we're kiting, lets not actualy run through monsters
                    if (CombatBase.KiteDistance > 0 && CacheData.MonsterObstacles.Any(m => m.Position.Distance(zigZagPoint) <= CombatBase.KiteDistance))
                        continue;

                    // Ignore point if any AoE in this point position
                    if(Core.Avoidance.Grid.IsLocationInFlags(zigZagPoint, AvoidanceFlags.Avoidance))
                        continue;

                    // Make sure this point is in LoS/walkable (not around corners or into a wall)
                    bool canRayCast = !Navigator.Raycast(Player.Position, zigZagPoint);
                    if (!canRayCast)
                        continue;

                    float distanceToPoint = zigZagPoint.Distance(myPos);

                    // Lots of weight for points further away from us (e.g. behind our CurrentTarget)
                    pointWeight *= distanceToPoint;

                    // Add weight for any units in this point
                    int monsterCount = ObjectCache.Count(u => u.IsUnit && u.Position.Distance(zigZagPoint) <= Math.Max(u.Radius, 10f));
                    if (monsterCount > 0)
                        pointWeight *= monsterCount;

                    //Logger.Log(LogCategory.Movement, "ZigZag Point: {0} distance={1:0} distaceFromTarget={2:0} intersectsPath={3} weight={4:0} monsterCount={5}",
                    //    zigZagPoint, distanceToPoint, distanceFromTargetToPoint, intersectsPath, pointWeight, monsterCount);

                    // Use this one if it's more weight, or we haven't even found one yet, or if same weight as another with a random chance
                    if (pointWeight > highestWeightFound)
                    {
                        highestWeightFound = pointWeight;

                        if (TrinityPlugin.Settings.Combat.Misc.UseNavMeshTargeting)
                        {
                            bestLocation = new Vector3(zigZagPoint.X, zigZagPoint.Y, TrinityPlugin.MainGridProvider.GetHeight(zigZagPoint.ToVector2()));
                        }
                        else
                        {
                            bestLocation = new Vector3(zigZagPoint.X, zigZagPoint.Y, zigZagPoint.Z + 4);
                        }
                    }
                }
            }
            Logger.Log(LogCategory.Movement, "Returning ZigZag: RandomXY {0} r-dist={1} t-dist={2}", bestLocation, ringDistance, bestLocation.Distance(Player.Position));
            return bestLocation;
        }

        /// <summary>
        /// Checks to see if a given Unit is standing in AoE, or if the direct paht-line to the unit goes through AoE
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        internal static bool UnitOrPathInAoE(TrinityCacheObject u)
        {
            return Core.Avoidance.InAvoidance(u.Position) || PathToActorIntersectsAoe(u);
        }

        internal static bool IsPositionOnMonster(Vector3 position, bool useWeights = true)
        {
            if (position == Vector3.Zero)
                return false;

            return TrinityPlugin.ObjectCache.Any(m => m.Weight > 0 && m.IsUnit && m.Position.Distance(position) <= m.Radius * 0.85);
        }

        internal static bool IsActorOnMonster(TrinityCacheObject actor)
        {
            if (actor == null)
                return false;

            return CacheData.MonsterObstacles.Any(m => m.Position.Distance(actor.Position) <= m.Radius * 0.6);
        }

        /// <summary>
        /// Checks to see if the path-line to a unit goes through AoE
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static bool PathToActorIntersectsAoe(TrinityCacheObject obj)
        {
            if (obj == null)
                return false;

            return Core.Avoidance.Grid.IsIntersectedByFlags(obj.Position, Player.Position, AvoidanceFlags.Avoidance);
            //return CacheData.TimeBoundAvoidance.Any(aoe =>
            //    MathUtil.IntersectsPath(aoe.Position, aoe.Radius, obj.Position, Player.Position));
        }


        /// <summary>
        /// Checks if spell is tracked on any unit within range of specified position
        /// </summary>
        internal static bool IsUnitWithDebuffInRangeOfPosition(float range, Vector3 position, SNOPower power, int unitsRequiredWithDebuff = 1)
        {
            var unitsWithDebuff = (from u in ObjectCache
                                   where u.IsUnit && u.IsValid &&
                                          u.Weight > 0 &&
                                          u.Position.Distance(position) <= range &&
                                          SpellTracker.IsUnitTracked(u.ACDGuid, power)
                                   select u).ToList();

            // Make sure units exist
            //unitsWithDebuff.RemoveAll(u =>
            //{
            //    var acd = ZetaDia.Actors.GetACDById(u.ACDId);
            //    return acd == null || !acd.IsValid;
            //});

            return unitsWithDebuff.Count >= unitsRequiredWithDebuff;
        }

        /// <summary>
        /// Checks if for units without a debuff
        /// </summary>
        internal static bool IsUnitWithoutDebuffWithinRange(float range, SNOPower power, int unitsRequiredWithoutDebuff = 1)
        {
            var unitsInRange = (from u in ObjectCache
                                      where u.IsUnit && u.IsValid &&
                                             u.Weight > 0 &&
                                             u.RadiusDistance <= range &&
                                             u.HasBeenRaycastable && u.HasBeenInLoS
                                      select u).ToList();

            var unitsWithoutDebuff = (from u in ObjectCache
                                   where u.IsUnit && u.IsValid &&
                                          u.Weight > 0 &&
                                          u.RadiusDistance <= range &&
                                          u.HasBeenRaycastable && u.HasBeenInLoS &&
                                          !u.HasDebuff(power)
                                   select u).ToList();

            //Logger.Log(LogCategory.Behavior, "{0}/{1} units without debuff {2} in {3} range", unitsWithoutDebuff.Count, unitsInRange.Count, power, range);

            return unitsWithoutDebuff.Count >= unitsRequiredWithoutDebuff;
        }

        /// <summary>
        /// Checks if for units without a debuff
        /// </summary>
        internal static TrinityCacheObject ClosestUnit(float range, Func<TrinityCacheObject, bool> condition)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                            u.Weight > 0 &&
                            u.RadiusDistance <= range && 
                            condition == null || condition(u) &&
                            u.HasBeenRaycastable && u.HasBeenInLoS
                    orderby u.IsBossOrEliteRareUnique, u.RadiusDistance 
                    select u).FirstOrDefault();

        }

        /// <summary>
        /// Creates a circular band or donut shape around player between min and max range and calculates the number of monsters inside.
        /// </summary>
        /// <param name="bandMinRange">Starting range for the band - monsters outside this value</param>
        /// <param name="bandMaxRange">Ending range for the band - monsters inside this value</param>
        /// <param name="percentage">Percentrage of monsters within bandMaxRange that must be within the band</param>
        /// <returns>True if at least specified percentage of monsters are within the band</returns>
        internal static bool IsPercentUnitsWithinBand(float bandMinRange = 10f, float bandMaxRange = 10f, double percentage = 50)
        {
            if (bandMinRange > bandMaxRange) bandMinRange = bandMaxRange;
            if (bandMaxRange < bandMinRange) bandMaxRange = bandMinRange;
            if (percentage < 0 || percentage > 100) percentage = 75;
            if (percentage < 1) percentage = percentage * 100;

            var totalWithinMaxRange = (from o in ObjectCache
                                       where o.IsUnit && o.Weight > 0 &&
                                       o.RadiusDistance <= bandMaxRange
                                       select o).Count();

            var totalWithinMinRange = (from o in ObjectCache
                                       where o.IsUnit && o.Weight > 0 &&
                                       o.RadiusDistance <= bandMinRange
                                       select o).Count();

            var totalWithinBand = totalWithinMaxRange - totalWithinMinRange;

            double percentWithinBand = ((double)totalWithinBand / (double)totalWithinMaxRange) * 100;

            //Logger.LogDebug("{0} of {6} mobs between {1} and {2} yards ({3:f2}%), needed={4}% result={5}", totalWithinBand, bandMinRange, bandMaxRange, percentWithinBand, percentage, percentWithinBand >= percentage, totalWithinMaxRange);

            return percentWithinBand >= percentage;
        }


        internal static TrinityCacheObject GetBestHarvestTarget(float skillRange, float maxRange = 30f)
        {
            TrinityCacheObject harvestTarget =
            (from u in ObjectCache
             where u.IsUnit && u.IsValid &&
             u.RadiusDistance <= maxRange &&
             u.IsBossOrEliteRareUnique &&
             u.CommonData.GetAttribute<int>(ActorAttributeType.BuffVisualEffect) != 0
             orderby u.NearbyUnitsWithinDistance(skillRange) descending
             select u).FirstOrDefault();
            if (harvestTarget != null)
                return harvestTarget;

            return (from u in ObjectCache
                    where u.IsUnit &&
                    u.RadiusDistance <= maxRange &&
                    u.CommonData.GetAttribute<int>(ActorAttributeType.BuffVisualEffect) != 0
                    orderby u.NearbyUnitsWithinDistance(skillRange) descending
                    select u).FirstOrDefault();
        }

        internal static bool IsPercentOfMobsDebuffed(SNOPower power, float maxRange = 30f, float minPercent = 0.5f)
        {
            return PercentOfMobsDebuffed(power, maxRange) >= minPercent;
        }

        internal static float PercentOfMobsDebuffed(SNOPower power, float maxRange = 30f)
        {
            var debuffed = (from u in ObjectCache
                            where u.IsUnit && u.IsValid &&
                            u.Weight > 0 &&
                            u.RadiusDistance <= maxRange &&
                            u.HasDebuff(power) &&
                            u.HasBeenRaycastable && u.HasBeenInLoS
                            select u).Count();

            var all = (from u in ObjectCache
                       where u.IsUnit &&
                       u.RadiusDistance <= maxRange &&
                       u.HasBeenRaycastable && u.HasBeenInLoS
                       select u).Count();

            if (all <= 0)
                return 0;

            var pct = (float)debuffed / all;

            Logger.Log(LogCategory.Behavior, "{0} out of {1} mobs have {3} ({2:0.##}%)", debuffed, all, pct*100, power);

            return pct;
        }

        internal static int MobsWithDebuff(SNOPower power, float maxRange = 30f)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                    u.RadiusDistance <= maxRange &&
                    u.HasDebuff(power)
                    select u).Count();
        }

        internal static int MobsWithDebuff(IEnumerable<SNOPower> powers, float maxRange = 30f)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                    u.RadiusDistance <= maxRange &&
                    powers.Any(u.HasDebuff)
                    select u).Count();
        }

        internal static int MobsWithDebuff(IEnumerable<SNOPower> powers, IEnumerable<TrinityCacheObject> units)
        {
            return (from u in units
                    where u.IsUnit && u.IsValid &&
                    powers.Any(u.HasDebuff)
                    select u).Count();
        }

        internal static int DebuffCount(IEnumerable<SNOPower> powers, float maxRange = 30f)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                    u.RadiusDistance <= maxRange &&
                    powers.Any(u.HasDebuff)
                    select powers.Count(u.HasDebuff)
                    ).Sum();
        }

        internal static int DebuffCount(IEnumerable<SNOPower> powers, IEnumerable<TrinityCacheObject> units)
        {
            return (from u in units
                    where u.IsUnit && u.IsValid &&
                    powers.Any(u.HasDebuff)
                    select powers.Count(u.HasDebuff)
                    ).Sum();
        }


        internal static TrinityCacheObject LowestHealthTarget(float range, Vector3 position = new Vector3(), SNOPower debuff = SNOPower.None)
        {
            if (position == new Vector3())
                position = Player.Position;

            TrinityCacheObject lowestHealthTarget;
            var unitsByHealth = (from u in ObjectCache
                                 where u.IsUnit && u.IsValid &&
                                        u.Weight > 0 &&
                                        u.Position.Distance(position) <= range &&
                                        (debuff == SNOPower.None || !SpellTracker.IsUnitTracked(u.ACDGuid, debuff)) &&
                                        !CacheData.MonsterObstacles.Any(m => MathUtil.IntersectsPath(m.Position, m.Radius, u.Position, Player.Position))
                                 orderby u.HitPoints ascending
                                 select u).ToList();

            if (unitsByHealth.Any())
                lowestHealthTarget = unitsByHealth.FirstOrDefault();
            else if (TrinityPlugin.CurrentTarget != null)
                lowestHealthTarget = TrinityPlugin.CurrentTarget;
            else
                lowestHealthTarget = default(TrinityCacheObject);

            return lowestHealthTarget;
        }

        internal static TrinityCacheObject BestExploadingPalmTarget(float range, Vector3 position = new Vector3())
        {
            if (position == new Vector3())
                position = Player.Position;

            TrinityCacheObject lowestHealthTarget;
            var unitsByHealth = (from u in ObjectCache
                                 where u.IsUnit && u.IsValid &&
                                        u.Weight > 0 &&
                                        u.Position.Distance(position) <= range &&
                                        !SpellTracker.IsUnitTracked(u.ACDGuid, SNOPower.Monk_ExplodingPalm) &&
                                        !CacheData.MonsterObstacles.Any(m => MathUtil.IntersectsPath(m.Position, m.Radius, u.Position, Player.Position)) &&
                                        u.IsInLineOfSight
                                 orderby u.HitPoints ascending
                                 select u).ToList();

            if (unitsByHealth.Any())
                lowestHealthTarget = unitsByHealth.FirstOrDefault();
            else if (TrinityPlugin.CurrentTarget != null)
                lowestHealthTarget = TrinityPlugin.CurrentTarget;
            else
                lowestHealthTarget = default(TrinityCacheObject);

            return lowestHealthTarget;
        }

        internal static TrinityCacheObject BestTargetWithoutDebuffs(float range, IEnumerable<SNOPower> debuffs, Vector3 position = new Vector3())
        {
            if (position == new Vector3())
                position = Player.Position;

            TrinityCacheObject target;
            var unitsByWeight = (from u in ObjectCache
                                 where u.IsUnit && u.IsValid &&
                                        u.Weight > 0 &&
                                        u.Position.Distance(position) <= range &&
                                        !debuffs.All(u.HasDebuff)
                                 orderby u.Weight descending
                                 select u).ToList();

            if (unitsByWeight.Any())
                target = unitsByWeight.FirstOrDefault();

            else if (TrinityPlugin.CurrentTarget != null)
                target = TrinityPlugin.CurrentTarget;
            else
                target = default(TrinityCacheObject);

            return target;
        }

        internal static TrinityCacheObject GetDashStrikeFarthestTarget(float maxRange, float procDistance = 33f, int arcDegrees = 0)
        {
            var result =
                (from u in ObjectCache
                 where u.IsUnit && u.Distance >= procDistance && u.IsValid &&
                 u.RadiusDistance <= maxRange
                 orderby u.RadiusDistance descending
                 select u).FirstOrDefault();
            return result;
        }

        internal static Vector3 GetDashStrikeBestClusterPoint(float radius = 15f, float maxRange = 65f,
            float procDistance = 33f, bool useWeights = true, bool includeUnitsInAoe = true)
        {
            if (radius < 5f)
                radius = 5f;
            if (maxRange > 300f)
                maxRange = 300f;

            bool includeHealthGlobes = TrinityPlugin.Settings.Combat.Misc.CollectHealthGlobe &&
                                       ObjectCache.Any(g => g.Type == TrinityObjectType.HealthGlobe && g.Weight > 0) &&
                                       (ClassMover.IsSpecialMovementReady || !PlayerMover.IsBlocked);
            //switch (TrinityPlugin.Player.ActorClass)
            //{
            //    case ActorClass.Barbarian:
            //        includeHealthGlobes = CombatBase.Hotbar.Contains(SNOPower.Barbarian_Whirlwind) &&
            //                              TrinityPlugin.Settings.Combat.Misc.CollectHealthGlobe &&
            //                              ObjectCache.Any(g => g.Type == TrinityObjectType.HealthGlobe && g.Weight > 0);
            //        break;
            //}

            Vector3 bestClusterPoint;
            var clusterUnits =
                (from u in ObjectCache
                 where (u.IsUnit || (includeHealthGlobes && u.Type == TrinityObjectType.HealthGlobe)) &&
                       ((useWeights && u.Weight > 0) || !useWeights) &&
                       (includeUnitsInAoe || !UnitOrPathInAoE(u)) &&
                       u.RadiusDistance <= maxRange && u.Distance >= procDistance
                 orderby u.Type != TrinityObjectType.HealthGlobe,
                     // if it's a globe this will be false and sorted at the top
                     !u.IsBossOrEliteRareUnique,
                     u.NearbyUnitsWithinDistance(radius) descending,
                     u.Distance,
                     u.HitPointsPct descending
                 select u.Position).ToList();

            if (clusterUnits.Any())
                bestClusterPoint = clusterUnits.FirstOrDefault();
            else
                bestClusterPoint = TrinityPlugin.Player.Position;

            return bestClusterPoint;
        }


        internal static TrinityCacheObject GetClosestUnit(float maxDistance = 100f)
        {
            var result =
                (from u in ObjectCache
                 where u.IsUnit && u.IsValid && u.Weight > 0 && u.RadiusDistance <= maxDistance
                 orderby u.RadiusDistance
                 select u).FirstOrDefault();

            return result;
        }

        internal static bool AnyBossesInRange(float range)
        {
            return NumBossInRangeOfPosition(TrinityPlugin.Player.Position, range) > 0;
        }

        public static bool AvoidancesInRange(float radius)
        {
            return CacheData.TimeBoundAvoidance.Any(aoe => aoe.Position.Distance(Player.Position) <= radius);
        }

        public static TrinityCacheObject BestEliteInRange(float range)
        {
            return (from u in ObjectCache
                 where u.IsUnit &&
                 u.IsBossOrEliteRareUnique &&
                 u.Distance <= range 
                 orderby 
                  u.NearbyUnitsWithinDistance(range) descending,
                  u.Distance,
                  u.HitPointsPct descending
                 select u).FirstOrDefault();

        }

        public static List<TrinityCacheObject> GetHighValueRiftTargets(float maxRange, double minValuePercent)
        {     
            return (from u in ObjectCache
                    where u.IsUnit && u.Distance <= maxRange && u.RiftValuePct >= minValuePercent
                    orderby
                     u.RiftValuePct descending,
                     u.Distance,
                     u.HitPointsPct descending
                    select u).ToList();
        }

        public static Vector3 GetBestRiftValueClusterPoint(float maxRange, double minValuePercent, float clusterRadius = 16f)
        {
            var unitPositions = (from u in ObjectCache
                                 where u.IsUnit && u.Distance <= maxRange && u.RiftValuePct >= minValuePercent
                                 orderby
                                  GetRiftValueWithinDistance(u, clusterRadius) descending,
                                  u.Distance,
                                  u.HitPointsPct descending
                                 select u.Position).ToList();

            if (!unitPositions.Any())
                return GetBestClusterPoint();

            var sample = 1;
            if (unitPositions.Count > 3)
                sample = 2;
            else if (unitPositions.Count > 5)
                sample = 3;
            else if (unitPositions.Count > 8)
                sample = 4;
            else if (unitPositions.Count > 12)
                sample = 5;

            return Centroid(unitPositions.Take(sample).ToList());
        }

        private static double GetRiftValueWithinDistance(TrinityCacheObject obj, float distance)
        {
            return ObjectCache.Where(u => u.Position.Distance(obj.Position) <= distance).Sum(u => u.RiftValuePct);
        }

        public static double GetRiftValueWithinDistance(Vector3 position, float distance)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                        u.Weight > 0 &&
                        u.RiftValuePct > 0 &&
                        u.Position.Distance(position) <= distance
                    select u).Sum(u => u.RiftValuePct);
        }

        public static Vector3 Centroid(List<Vector3> points)
        {
            var result = points.Aggregate(Vector3.Zero, (current, point) => current + point);
            result /= points.Count();
            return result;
        }

        public static Vector3 GetLoiterPosition(TrinityCacheObject target, float radiusDistance)
        {
            // Estimate based on the target size how far away we should be from it.
            var walkTarget = target != null ? target.Position : GetBestClusterPoint();
            var targetRadius = target != null ? target.Radius + radiusDistance : 11f + radiusDistance;

            // Get points in a circle around the target and make sure they are walkable and there's no monsters near them.
            var circlePositions = StuckHandler.GetCirclePoints(8, targetRadius, walkTarget);
            circlePositions.AddRange(StuckHandler.GetCirclePoints(8, targetRadius + 12f, walkTarget));
            Func<Vector3, bool> isValid = p => NavHelper.CanRayCast(p) && !IsPositionOnMonster(p) && p.Distance(Player.Position) > 8f;
            var validPositions = circlePositions.Where(isValid);

            if (!validPositions.Any())
            {
                // Try with points around the player instead
                circlePositions = StuckHandler.GetCirclePoints(12, 30f, Player.Position);
                validPositions = circlePositions.Where(isValid);
            }

            validPositions = validPositions.OrderBy(p => p.Distance(Player.Position));

            // Draw it on the visualizer
            RadarDebug.Draw(validPositions, 50);
            return validPositions.FirstOrDefault();
        }

        private static Vector3 _lastSafeSpotPosition = Vector3.Zero;
        private static DateTime _lastSafeSpotPositionTime = DateTime.MinValue;
        public static Vector3 GetSafeSpotPosition(float distance)
        {
            // Maximum speed of changing safe spots is every 2s
            if (DateTime.UtcNow.Subtract(_lastSafeSpotPositionTime).TotalSeconds < 2)
                return _lastSafeSpotPosition;

            // If we have a position already and its still within range and still no monster there, keep it.
            var safeSpotIsClose = _lastSafeSpotPosition.Distance(Player.Position) < distance;
            var safeSpotHasNoMonster = !IsPositionOnMonster(_lastSafeSpotPosition);
            if (_lastSafeSpotPosition != Vector3.Zero && safeSpotIsClose && safeSpotHasNoMonster)
                return _lastSafeSpotPosition;

            Func<Vector3, bool> isValid = p => NavHelper.CanRayCast(p) && !IsPositionOnMonster(p);

            var circlePositions = StuckHandler.GetCirclePoints(8, 10, Player.Position);

            if (distance >= 20)
                circlePositions.AddRange(StuckHandler.GetCirclePoints(8, 20, Player.Position));

            if (distance >= 30)
                circlePositions.AddRange(StuckHandler.GetCirclePoints(8, 30, Player.Position));

            var closestMonster = GetClosestUnit();
            var proximityTarget = closestMonster != null ? closestMonster.Position : Player.Position;
            var validPositions = circlePositions.Where(isValid).OrderByDescending(p => p.Distance(proximityTarget));

            RadarDebug.Draw(validPositions);

            _lastSafeSpotPosition = validPositions.FirstOrDefault();
            _lastSafeSpotPositionTime = DateTime.UtcNow;
            return _lastSafeSpotPosition;
        }

        //public static Vector3 GetBestRiftValueClusterPoint(float maxRange, double minValuePercent, float clusterRadius = 16f)
        //{
        //    List<Vector3> positions;
        //    Vector3 bestRiftValueClusterPoint;
        //    double clusterValue;

        //    if (GetValueClusterUnits(maxRange, minValuePercent, clusterRadius,
        //        out positions, out bestRiftValueClusterPoint, out clusterValue))
        //        return bestRiftValueClusterPoint;

        //    return GetCentroid(positions);
        //}

        internal static bool GetValueClusterUnits(

            float maxRange, double minValuePercent, float clusterRadius,
            out List<Vector3> positions,
            out Vector3 bestRiftValueClusterPoint,
            out double clusterValue)
        {
            var units = (from u in ObjectCache
                         where u.IsUnit && u.Distance <= maxRange &&
                         u.RiftValuePct > 0 &&
                         u.RiftValuePct >= minValuePercent
                         orderby
                             GetRiftValueWithinDistance(u, clusterRadius / 2) descending,
                             u.Distance,
                             u.HitPointsPct descending
                         select u).ToList();

            if (!units.Any())
            {
                bestRiftValueClusterPoint = GetBestClusterPoint();
                positions = new List<Vector3>();
                clusterValue = -1;
                return true;
            }

            var total = 0d;
            var unitPositions = new List<Vector3>();
            var sampleSize = GetClusterSampleSize(units.Count);
            var usedSamples = 0;

            for (int index = 0; index < units.Count; index++)
            {
                var unit = units[index];

                if (usedSamples >= sampleSize)
                    break;

                if (!units.Any(u => u.Position.Distance(unit.Position) < clusterRadius))
                    continue;

                usedSamples++;
                total += unit.RiftValuePct;
                unitPositions.Add(unit.Position);
            }

            clusterValue = total;
            positions = unitPositions;
            bestRiftValueClusterPoint = Centroid(unitPositions);
            return false;
        }

        private static int GetClusterSampleSize(int clusterUnitCount)
        {
            var sample = 1;
            if (clusterUnitCount > 3)
                sample = 3;
            else if (clusterUnitCount > 5)
                sample = 4;
            else if (clusterUnitCount > 12)
                sample = 6;
            return sample;
        }


        //private static double GetRiftValueWithinDistance(TrinityCacheObject obj, float distance)
        //{
        //    return ObjectCache.Where(u => u.Position.Distance(obj.Position) <= distance).Sum(u => u.RiftValuePct);
        //}

        //public static Vector3 GetCentroid(List<Vector3> points)
        //{
        //    var result = points.Aggregate(Vector3.Zero, (current, point) => current + point);
        //    result /= points.Count();
        //    return result;
        //}
    }

}


