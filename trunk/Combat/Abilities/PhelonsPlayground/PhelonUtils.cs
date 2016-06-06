using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Movement;
using Trinity.Technicals;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Trinity.Config;
using Trinity.Reference;

namespace Trinity.Combat.Abilities.PhelonsPlayground
{
    class PhelonUtils
    {
        internal static List<TrinityCacheObject> SafeList(bool objectsInAoe = false)
        {
            return
                TrinityPlugin.ObjectCache.Where(x => !x.IsPlayer && (!x.IsUnit || x.IsUnit && x.HitPoints > 0) &&
                                                     (objectsInAoe || !Core.Avoidance.InAvoidance(x.Position))).ToList();
        }

        internal static Vector3 BestBuffPosition(float maxRange, bool objectsInAoe = false)
        {
            if (ClosestSancAndOcc(maxRange, objectsInAoe) != Vector3.Zero &&
                ClosestSancAndOcc(maxRange, objectsInAoe).Distance(TrinityPlugin.Player.Position) < maxRange)
                return ClosestSancAndOcc(maxRange, objectsInAoe);

            if (ClosestSanctuary(maxRange, objectsInAoe) != Vector3.Zero &&
                ClosestSanctuary(maxRange, objectsInAoe).Distance(TrinityPlugin.Player.Position) < maxRange)
                return ClosestSanctuary(maxRange, objectsInAoe);

            return ClosestOcculous(maxRange, objectsInAoe) != Vector3.Zero &&
                   ClosestOcculous(maxRange, objectsInAoe).Distance(TrinityPlugin.Player.Position) < maxRange
                ? ClosestOcculous(maxRange, objectsInAoe)
                : PhelonTargeting.BestAoeUnit(45, true).Position;
        }

        internal static List<TrinityCacheObject> BestShrine(float range = 25f, bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.Type == TrinityObjectType.Shrine
                    orderby u.Distance
                    select u).ToList();
        }

        internal static Vector3 BestDpsPosition(float maxRange, bool objectsInAoe = false)
        {
            return
                BestBuffPosition(maxRange, objectsInAoe)
                    .Distance(PhelonTargeting.BestAoeUnit(45f, objectsInAoe).Position) < maxRange
                    ? BestBuffPosition(maxRange, objectsInAoe)
                    : PhelonTargeting.BestAoeUnit(45f, objectsInAoe).Position;
        }

        internal static Vector3 BestWalkLocation(float maxRange, bool objectsInAoe = false)
        {
                if (ClosestGlobe(maxRange, objectsInAoe) != null)
                    return ClosestGlobe(maxRange, objectsInAoe).Position;
                var shrine = BestShrine(35, objectsInAoe).FirstOrDefault();
                if (Legendary.NemesisBracers.IsEquipped && shrine != null)
                    return shrine.Position;
                // Prevent Default Attack
                if (TrinityPlugin.CurrentTarget.Type != TrinityObjectType.Destructible &&
                    TrinityPlugin.CurrentTarget.Type != TrinityObjectType.Shrine &&
                    TrinityPlugin.CurrentTarget.Type != TrinityObjectType.HealthGlobe)
                {
                    //Logger.Log("Prevent Primary Attack ");
                    var targetPosition = TargetUtil.GetLoiterPosition(TrinityPlugin.CurrentTarget, 20f);
                    // return new TrinityPower(SNOPower.Walk, 7f, targetPosition);
                    return targetPosition;
                }
                return TrinityPlugin.CurrentTarget.Position;
        }

        internal static List<TrinityCacheObject> MobsBetweenRange(float startRange = 15f, float endRange = 25)
        {
            return (from u in SafeList(true)
                where u.IsUnit && u.IsFullyValid() &&
                      u.Position.Distance(TrinityPlugin.Player.Position) <= endRange &&
                      u.Position.Distance(TrinityPlugin.Player.Position) >= startRange
                select u).ToList();
        }

        internal static TrinityCacheObject GetFarthestClusterUnit(float aoe_radius = 25f, float maxRange = 65f,
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

        internal static TrinityCacheObject BestAuraUnit(SNOPower aura, float maxSearchRange = 65f,
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

        internal static TrinityCacheObject BestPierceOrClusterUnit(float clusterRadius = 15f, float maxSearchRange = 65f,
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

        internal static List<TrinityCacheObject> TargetsInFrontOfMe(float maxRange, bool ignoreUnitsInAoE = false,
            bool ignoreElites = false)
        {
            return (from u in SafeList(ignoreElites)
                where u.IsUnit &&
                      u.RadiusDistance <= maxRange && u.IsInLineOfSight() &&
                      !(ignoreUnitsInAoE && u.IsStandingInAvoidance) &&
                      !(ignoreElites && u.IsEliteRareUnique)
                orderby u.CountUnitsInFront() descending
                select u).ToList();
        }

        internal static TrinityCacheObject GetBestPierceTarget(float maxRange, bool ignoreUnitsInAoE = false,
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
        internal static TrinityCacheObject GetBestClusterUnit(
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
                          !(ignoreElites && u.IsEliteRareUnique) &&
                          (!inLineOfSight || u.IsInLineOfSight()) &&
                          u.Distance <= maxSearchRange && !u.IsSafeSpot
                    orderby
                        u.NearbyUnitsWithinDistance(clusterRadius) descending,
                        u.HitPointsPct descending
                    select u).ToList();

            return clusterUnits.FirstOrDefault();
        }

        public static TrinityCacheObject BestEliteInRange(float range, bool objectsInAoe = false)
        {
            return (from u in SafeList(objectsInAoe)
                where u.IsUnit &&
                      u.IsBossOrEliteRareUnique &&
                      u.Distance <= range
                orderby
                    u.NearbyUnitsWithinDistance(range) descending,
                    u.HitPointsPct descending
                select u).FirstOrDefault();

        }

        internal static Vector3 ClosestOcculous(float maxRange, bool objectsInAoe = false)
        {
            var trinityCacheObject = GetOculusBuffDiaObjects(maxRange, objectsInAoe).FirstOrDefault();
            return trinityCacheObject?.Position ?? Vector3.Zero;
        }

        internal static List<TrinityCacheObject> GetOculusBuffDiaObjects(float range = 25f, bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.ActorSNO == 433966
                    orderby u.Distance
                    select u).ToList();
        }

        internal static Vector3 ClosestSancAndOcc(float maxRange, bool objectsInAoe = false)
        {
            foreach (var item in GetInnerSanctuaryDiaObjects(maxRange).Select(x => x.Position).ToList())
            {
                var occPoint = GetOculusBuffDiaObjects(maxRange).OrderBy(x => x.Distance)
                    .Select(y => y.Position)
                    .OrderBy(z => !Core.Avoidance.InAvoidance(z))
                    .FirstOrDefault(z => z.Distance2D(item) < 7);
                if (occPoint != Vector3.Zero)
                    return MathEx.CalculatePointFrom(item, occPoint, item.Distance2D(occPoint)/2);
            }
            return Vector3.Zero;
        }

        internal static Vector3 ClosestSanctuary(float maxRange, bool objectsInAoe = false)
        {
            var trinityCacheObject = GetInnerSanctuaryDiaObjects(maxRange, objectsInAoe).FirstOrDefault();
            return trinityCacheObject?.Position ?? Vector3.Zero;
        }

        internal static List<TrinityCacheObject> GetInnerSanctuaryDiaObjects(float range = 25f,
            bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.ActorSNO == 320136
                    orderby u.Distance
                    select u).ToList();
        }

        internal static TrinityCacheObject ClosestGlobe(float distance = 45, bool objectsInAoe = false)
        {
            return (from u in SafeList(objectsInAoe)
                where
                    (u.Type == TrinityObjectType.HealthGlobe || u.Type == TrinityObjectType.PowerGlobe) &&
                    u.RadiusDistance <= distance
                select u).FirstOrDefault();
        }

        internal static bool WithInDistance(TrinityCacheObject actor, TrinityCacheObject actor2, float distance,
            bool objectsInAoe = false)
        {
            return
                SafeList(objectsInAoe).Any(
                    m => m.ActorSNO == actor.ActorSNO && m.Position.Distance(actor2.Position) <= distance);
        }

        internal static bool WithInDistance(TrinityCacheObject actor, Vector3 unitLocation, float distance,
            bool objectsInAoe = false)
        {
            return
                SafeList(objectsInAoe).Any(
                    m => m.ActorSNO == actor.ActorSNO && m.Position.Distance(unitLocation) <= distance);
        }

        internal static List<TrinityCacheObject> GetDiaObjects(uint actorSNO, float range = 25f,
            bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.ActorSNO == actorSNO
                    orderby u.Distance
                    select u).ToList();
        }

        internal static Vector3 GetDiaObjectBestClusterPoint(uint actorSNO, float radius = 15f, float maxRange = 45f,
            bool useWeights = true, bool includeUnitsInAoe = true, bool objectsInAoe = false)
        {
            var clusterUnits =
                (from u in SafeList(objectsInAoe)
                    where u.ActorSNO == actorSNO &&
                          u.RadiusDistance <= maxRange
                    orderby u.NearbyUnitsWithinDistance(radius),
                        u.Distance descending
                    select u.Position).ToList();

            return clusterUnits.Any() ? clusterUnits.FirstOrDefault() : Vector3.Zero;
        }

        internal static List<TrinityCacheObject> GetTwisterDiaObjects(float range = 25f, bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.ActorSNO == 322236
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
                    where u.ActorSNO == 322236 &&
                          u.RadiusDistance <= maxRange
                    orderby u.NearbyUnitsWithinDistance(radius),
                        u.Distance descending
                    select u.Position).ToList();

            return clusterUnits.Any() ? clusterUnits.FirstOrDefault() : Vector3.Zero;
        }

        public static Vector3 PointBehind(Vector3 point, float maxRange = 45, bool objectsInAoe = false)
        {
            var properDistance = point.Distance2D(TrinityPlugin.Player.Position) - maxRange;
            //return MathEx.GetPointAt(point, properDistance, TrinityPlugin.Player.Rotation);
            return MathEx.CalculatePointFrom(TrinityPlugin.Player.Position, point, properDistance);
        }
    }
}