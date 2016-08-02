using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground
{
    class PhelonUtils
    {
        internal static List<TrinityActor> SafeList(bool objectsInAoe = false)
        {
            return
                Trinity.TrinityPlugin.Targets.Where(x => !x.IsPlayer && (!x.IsUnit || x.IsUnit && x.HitPoints > 0) &&
                                                     (objectsInAoe || !Core.Avoidance.InAvoidance(x.Position))).ToList();
        }

        internal static bool BestBuffPosition(float maxRange, Vector3 fromLocation, bool objectsInAoe, out Vector3 location)
        {
            location = Vector3.Zero;
            var closestSancAndOcc = ClosestSancAndOcc(maxRange, objectsInAoe);
            if (closestSancAndOcc != Vector3.Zero)
            {
                location = closestSancAndOcc;
                return true;
            }
            var closestOcc = ClosestOcculous(maxRange, objectsInAoe);
            if (closestOcc != Vector3.Zero)
            {
                location = closestOcc;
                return true;
            }
            var closestSanc = ClosestSanctuary(maxRange, objectsInAoe);
            if (closestSanc != Vector3.Zero)
            {
                location = closestSanc;
                return true;
            }
            return false;
        }

        internal static bool BestTankLocation(float maxRange, bool objectsInAoe, out Vector3 location)
        {
            location = Vector3.Zero;

            var closestOcc = ClosestOcculous(maxRange, objectsInAoe);

            if (closestOcc != Vector3.Zero)
            {
                location = closestOcc;
                return true;
            }

            return false;
        }

        internal static List<TrinityActor> BestShrine(float range = 25f, bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.Type == TrinityObjectType.Shrine
                    orderby u.Distance
                    select u).ToList();
        }

        internal static Vector3 BestDpsPosition(float maxRange, float searchRange = 12f, bool objectsInAoe = false)
        {
            var bestTarget = PhelonTargeting.BestAoeUnit(maxRange, objectsInAoe).Position;
            Vector3 bestBuffPosition = Vector3.Zero;
            return BestBuffPosition(maxRange, bestTarget, objectsInAoe, out bestBuffPosition)
                ? bestBuffPosition
                : bestTarget;
        }

        internal static Vector3 BestTankPosition(float maxRange, bool objectsInAoe = false)
        {
            var bestTarget = PhelonTargeting.BestAoeUnit(maxRange, objectsInAoe).Position;
            Vector3 bestTankPosition = Vector3.Zero;
            return BestTankLocation(maxRange, objectsInAoe, out bestTankPosition)
                ? bestTankPosition
                : bestTarget;
        }

        internal static Vector3 BestWalkLocation(float maxRange, bool objectsInAoe = false)
        {
                if (ClosestGlobe(maxRange, objectsInAoe) != null)
                    return ClosestGlobe(maxRange, objectsInAoe).Position;
                var shrine = BestShrine(35, objectsInAoe).FirstOrDefault();
                if (Legendary.NemesisBracers.IsEquipped && shrine != null)
                    return shrine.Position;
                // Prevent Default Attack
                if (Trinity.TrinityPlugin.CurrentTarget.Type != TrinityObjectType.Destructible &&
                    Trinity.TrinityPlugin.CurrentTarget.Type != TrinityObjectType.Shrine &&
                    Trinity.TrinityPlugin.CurrentTarget.Type != TrinityObjectType.HealthGlobe)
                {
                    //Logger.Log("Prevent Primary Attack ");
                    var targetPosition = TargetUtil.GetLoiterPosition(Trinity.TrinityPlugin.CurrentTarget, 20f);
                    // return new TrinityPower(SNOPower.Walk, 7f, targetPosition);
                    return targetPosition;
                }
                return Trinity.TrinityPlugin.CurrentTarget.Position;
        }

        internal static List<TrinityActor> MobsBetweenRange(float startRange = 15f, float endRange = 25)
        {
            return (from u in SafeList(true)
                where u.IsUnit && u.IsValid &&
                      u.Position.Distance(Core.Player.Position) <= endRange &&
                      u.Position.Distance(Core.Player.Position) >= startRange
                select u).ToList();
        }

        internal static TrinityActor GetFarthestClusterUnit(float aoe_radius = 25f, float maxRange = 65f,
            int count = 1, bool useWeights = true, bool includeUnitsInAoe = true)
        {
            using (new PerformanceLogger("TargetUtil.GetFarthestClusterUnit"))
            {
                return
                    (from u in SafeList(includeUnitsInAoe)
                        where ((useWeights && u.Weight > 0) || !useWeights) &&
                              u.IsUnit && u.HasBeenInLoS &&
                              u.RadiusDistance <= maxRange &&
                              u.NearbyUnitsWithinDistance(aoe_radius) >= count
                        orderby u.NearbyUnitsWithinDistance(aoe_radius),
                            u.Distance descending
                        select u).FirstOrDefault();
            }
        }

        internal static TrinityActor BestAuraUnit(SNOPower aura, float maxSearchRange = 65f,
            bool addUnitsInAoE = false)
        {
            return (from u in SafeList(addUnitsInAoE)
                where u.IsUnit &&
                      u.RadiusDistance <= maxSearchRange &&
                      u.HasBeenInLoS && !u.HasDebuff(aura)
                orderby u.NearbyUnitsWithinDistance(),
                    u.Distance
                select u).FirstOrDefault();
        }

        internal static TrinityActor BestPierceOrClusterUnit(float clusterRadius = 15f, float maxSearchRange = 65f,
            bool includeInAoE = true)
        {
            var clusterUnit = GetBestClusterUnit(clusterRadius, maxSearchRange, !
                includeInAoE);
            var pierceUnit = GetBestPierceTarget(maxSearchRange, !includeInAoE);

            if (clusterUnit == null && pierceUnit == null)
                return null;

            if (clusterUnit == null)
                return pierceUnit;

            if (pierceUnit == null)
                return clusterUnit;

            return clusterUnit.NearbyUnitsWithinDistance(10) > pierceUnit.CountUnitsInFront()
                ? clusterUnit
                : pierceUnit;
        }

        internal static List<TrinityActor> TargetsInFrontOfMe(float maxRange, bool ignoreUnitsInAoE = false,
            bool ignoreElites = false)
        {
            return (from u in SafeList(ignoreElites)
                where u.IsUnit &&
                      u.RadiusDistance <= maxRange && u.IsInLineOfSight &&
                      !(ignoreUnitsInAoE && u.IsStandingInAvoidance) &&
                      !(ignoreElites && u.IsElite)
                orderby u.CountUnitsInFront() descending
                select u).ToList();
        }

        internal static TrinityActor GetBestPierceTarget(float maxRange, bool ignoreUnitsInAoE = false,
            bool ignoreElites = false)
        {
            var result = TargetsInFrontOfMe(maxRange, ignoreUnitsInAoE, ignoreElites).FirstOrDefault();
            return result ?? PhelonTargeting.BestAoeUnit(maxRange, !ignoreUnitsInAoE);
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
        internal static TrinityActor GetBestClusterUnit(
            float clusterRadius = 15f, float maxSearchRange = 65f, bool useWeights = true, bool includeUnitsInAoe = true,
            bool ignoreElites = false, bool inLineOfSight = false)
        {
            if (clusterRadius < 1f)
                clusterRadius = 1f;
            if (maxSearchRange > 60f)
                maxSearchRange = 60;

            var clusterUnits =
                (from u in SafeList(includeUnitsInAoe)
                    where u.IsUnit && //u.HasBeenInLoS &&
                          ((useWeights && u.Weight > 0) || !useWeights) &&
                          !(ignoreElites && u.IsElite) &&
                          (!inLineOfSight || u.IsInLineOfSight) &&
                          u.Distance <= maxSearchRange && !u.IsSafeSpot
                    orderby
                        u.NearbyUnitsWithinDistance(clusterRadius) descending,
                        u.HitPointsPct descending
                    select u).ToList();

            return clusterUnits.FirstOrDefault();
        }

        public static TrinityActor BestEliteInRange(float range, bool objectsInAoe = false)
        {
            return (from u in SafeList(objectsInAoe)
                where u.IsUnit &&
                      u.IsElite &&
                      u.Distance <= range
                orderby
                    u.NearbyUnitsWithinDistance(15) descending,
                    u.HitPointsPct descending
                select u).FirstOrDefault();

        }

        public static TrinityActor ClosestTargetToOcculous(float range, bool objectsInAoe = false)
        {
            return (from u in SafeList(objectsInAoe)
                    where u.IsUnit &&
                          u.Distance <= range
                    orderby
                        ClosestOcculous(30f).Distance(u.Position) descending
                    select u).FirstOrDefault();
        }

        internal static Vector3 ClosestOcculous(float maxRange, bool objectsInAoe = false)
        {
            var TrinityActor = GetOculusBuffDiaObjects(maxRange, objectsInAoe).FirstOrDefault();
            return TrinityActor?.Position ?? Vector3.Zero;
        }

        internal static List<TrinityActor> GetOculusBuffDiaObjects(float range = 25f, bool objectsInAoe = false)
        {
            //[1FABA194] Type: ClientEffect Name: p2_itemPassive_unique_ring_017_dome-58267 ActorSnoId: 433966, Distance: 24.701

            return
                (from u in SafeList(objectsInAoe)
                    where u.Distance <= range &&
                          u.ActorSnoId == 433966
                    orderby u.Distance
                    select u).ToList();
        }

        internal static Vector3 ClosestSancAndOcc(float maxRange, bool objectsInAoe = false)
        {
            foreach (var item in GetInnerSanctuaryDiaObjects(maxRange).Select(x => x.Position).ToList())
            {
                var occPoint = GetOculusBuffDiaObjects(maxRange).OrderBy(x => x.Distance)
                    .Select(y => y.Position)
                    .FirstOrDefault(z => z.Distance2D(item) < 3);
                if (occPoint != Vector3.Zero)
                    return MathEx.CalculatePointFrom(item, occPoint, item.Distance2D(occPoint) / 2);
            }
            return Vector3.Zero;
        }

        internal static Vector3 ClosestSanctuary(float maxRange, bool objectsInAoe = false)
        {
            var TrinityActor = GetInnerSanctuaryDiaObjects(maxRange, objectsInAoe).FirstOrDefault();
            return TrinityActor?.Position ?? Vector3.Zero;
        }

        internal static List<TrinityActor> GetInnerSanctuaryDiaObjects(float range = 25f,
            bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.ActorSnoId == 320136
                    orderby u.Distance
                    select u).ToList();
        }

        internal static TrinityActor ClosestGlobe(float distance = 45, bool objectsInAoe = false)
        {
            return (from u in SafeList(objectsInAoe)
                where
                    (u.Type == TrinityObjectType.HealthGlobe || u.Type == TrinityObjectType.PowerGlobe) &&
                    u.RadiusDistance <= distance
                select u).FirstOrDefault();
        }

        internal static bool WithInDistance(TrinityActor actor, TrinityActor actor2, float distance,
            bool objectsInAoe = false)
        {
            return
                SafeList(objectsInAoe).Any(
                    m => m.ActorSnoId == actor.ActorSnoId && m.Position.Distance(actor2.Position) <= distance);
        }

        internal static bool WithInDistance(TrinityActor actor, Vector3 unitLocation, float distance,
            bool objectsInAoe = false)
        {
            return
                SafeList(objectsInAoe).Any(
                    m => m.ActorSnoId == actor.ActorSnoId && m.Position.Distance(unitLocation) <= distance);
        }

        internal static List<TrinityActor> GetDiaObjects(uint actorSNO, float range = 25f,
            bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.ActorSnoId == actorSNO
                    orderby u.Distance
                    select u).ToList();
        }

        internal static Vector3 GetDiaObjectBestClusterPoint(uint actorSNO, float radius = 15f, float maxRange = 45f,
            bool useWeights = true, bool includeUnitsInAoe = true, bool objectsInAoe = false)
        {
            var clusterUnits =
                (from u in SafeList(objectsInAoe)
                    where u.ActorSnoId == actorSNO &&
                          u.RadiusDistance <= maxRange
                    orderby u.NearbyUnitsWithinDistance(radius),
                        u.Distance descending
                    select u.Position).ToList();

            return clusterUnits.Any() ? clusterUnits.FirstOrDefault() : Vector3.Zero;
        }

        internal static List<TrinityActor> GetTwisterDiaObjects(float range = 25f, bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.ActorSnoId == 322236
                    orderby u.Distance
                    select u).ToList();
        }

        internal static Vector3 GetBestTwsiterClusterPoint(float radius = 15f, float maxRange = 45f,
            bool useWeights = true, bool includeUnitsInAoe = true, bool objectsInAoe = false)
        {
            if (radius < 5f)
                radius = 5f;
            if (maxRange > 75f)
                maxRange = 75f;

            var clusterUnits =
                (from u in SafeList(objectsInAoe)
                    where u.ActorSnoId == 322236 &&
                          u.RadiusDistance <= maxRange
                    orderby u.NearbyUnitsWithinDistance(radius),
                        u.Distance descending
                    select u.Position).ToList();

            return clusterUnits.Any() ? clusterUnits.FirstOrDefault() : Vector3.Zero;
        }

        public static Vector3 PointBehind(Vector3 point, float maxRange = 45, bool objectsInAoe = false)
        {
            var properDistance = point.Distance2D(Core.Player.Position) - maxRange;
            //return MathEx.GetPointAt(point, properDistance, Core.Player.Rotation);
            return MathEx.CalculatePointFrom(Core.Player.Position, point, properDistance);
        }

        public static Vector3 SlightlyForwardPosition(float distance = 4f)
        {
            return MathEx.GetPointAt(Core.Avoidance.Avoider.SafeSpot, distance, Core.Player.Rotation);
        }
    }
}