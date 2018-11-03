using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Reference;
using Trinity.Modules;
using Trinity.Routines;
using Trinity.Settings;
using Trinity.UI.Visualizer.RadarCanvas;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


namespace Trinity.Components.Combat.Resources
{
    public static class TargetUtil
    {
        internal static TrinityActor Monk
        {
            get
            {
                try
                {
                    var monk = Core.Targets.FirstOrDefault(x => x.InternalName.ToLower().Contains("monk"));
                    //if (monk == null)
                    //    Core.Logger.Log("Unable to find Monk.  Where did he go?");
                    return monk;
                }
                catch (Exception)
                {
                    Core.Logger.Log("Unable to find Monk.  Error?");
                    //return CombatBase.IsInParty ? TargetUtil.GetClosestUnit(25) : null;
                    return null;
                }
            }
        }

        internal static TrinityActor PullCharacter
        {
            get
            {
                return
                    Core.Targets.OrderBy(y => y.NearbyUnitsWithinDistance(20))
                        .FirstOrDefault(x => x.IsPlayer);
            }
        }

        internal static List<TrinityActor> UnitsAroundPuller(Vector3 pullLocation, float groupRadius = 20,
            bool includeUnitsInAoe = true)
        {
            return
                (from u in SafeList(includeUnitsInAoe)
                 where u.IsUnit && u.Position.Distance(pullLocation) <= groupRadius
                 select u).ToList();
        }

        internal static List<TrinityActor> UnitsAroundPlayer(float groupRadius = 20, bool includeUnitsInAoe = true)
        {
            return
                (from u in SafeList(includeUnitsInAoe)
                 where u.IsUnit && u.Position.Distance(Core.Player.Position) <= groupRadius
                 select u).ToList();
        }

        internal static List<TrinityActor> UnitsToPull(Vector3 pullLocation, float groupRadius = 15,
            int groupCount = 1, float searchRange = 45, bool includeUnitsInAoe = true)
        {
            return
                (from u in SafeList(includeUnitsInAoe)
                 where u.IsUnit && u.CanCastTo() && u.HasBeenInLoS &&
                       !UnitsAroundPuller(pullLocation, 20, includeUnitsInAoe)
                           .Select(x => x.AcdId)
                           .Contains(u.AcdId) &&
                       !UnitsAroundPlayer(10, includeUnitsInAoe)
                           .Select(x => x.AcdId)
                           .Contains(u.AcdId) &&
                       u.Position.Distance(pullLocation) <= searchRange
                 orderby u.Distance
                 select u).ToList();
        }

        public static TrinityActor BestAoeUnit(float range = 45, bool includeInAoE = false)
        {
            return BestEliteInRange(range, includeInAoE) ??
                   GetBestClusterUnit(7, range, false, includeInAoE) ??
                   CurrentTarget;
        }

        public static TrinityActor BestTarget(float searchRange = 45f, bool includeInAoE = false)
        {
            return CurrentTarget.Type == TrinityObjectType.Shrine || CurrentTarget.IsTreasureGoblin ||
                   CurrentTarget.IsTreasureGoblin || CurrentTarget.Type == TrinityObjectType.HealthGlobe ||
                   CurrentTarget.IsElite
                ? CurrentTarget
                : BestAoeUnit(searchRange, true) ?? CurrentTarget;
        }

        internal static List<TrinityActor> SafeList(bool objectsInAoe = false)
        {
            return
                Core.Targets.Where(x => !x.IsPlayer && (!x.IsUnit || x.IsUnit && x.HitPoints > 0) &&
                                                     (objectsInAoe || !Core.Avoidance.InAvoidance(x.Position))).ToList();
        }

        internal static bool BestBuffPosition(float maxRange, Vector3 fromLocation, bool objectsInAoe, out Vector3 location)
        {
            location = Vector3.Zero;
            var closestSancAndOcc = ClosestSancAndOcc(maxRange, fromLocation, objectsInAoe);
            if (closestSancAndOcc != Vector3.Zero && Core.Avoidance.Grid.CanRayCast(fromLocation, closestSancAndOcc))
            {
                location = closestSancAndOcc;
                return true;
            }
            var closestOcc = ClosestOcculous(maxRange, fromLocation, objectsInAoe);
            if (closestOcc != Vector3.Zero && Core.Avoidance.Grid.CanRayCast(fromLocation, closestOcc))
            {
                location = closestOcc;
                return true;
            }
            var closestSanc = ClosestSanctuary(maxRange, fromLocation, objectsInAoe);
            if (closestSanc != Vector3.Zero && Core.Avoidance.Grid.CanRayCast(fromLocation, closestSanc))
            {
                location = closestSanc;
                return true;
            }
            return false;
        }

        internal static bool BestTankLocation(float maxRange, bool objectsInAoe, out Vector3 location)
        {
            location = Vector3.Zero;

            var closestOcc = ClosestOcculous(maxRange, Player.Position, objectsInAoe);

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
                 where u.Distance <= range &&
                       u.Type == TrinityObjectType.Shrine
                 orderby u.Distance
                 select u).ToList();
        }

        internal static Vector3 BestDpsPosition(Vector3 location, float searchRange = 12f, bool objectsInAoe = false)
        {
            Vector3 bestBuffPosition;
            return BestBuffPosition(searchRange, location, objectsInAoe, out bestBuffPosition)
                ? bestBuffPosition
                : location;
        }

        internal static Vector3 BestTankPosition(float maxRange, bool objectsInAoe = false)
        {
            var bestTarget = BestAoeUnit(maxRange, objectsInAoe).Position;
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
            if (TrinityCombat.Targeting.CurrentTarget.Type != TrinityObjectType.Destructible &&
                TrinityCombat.Targeting.CurrentTarget.Type != TrinityObjectType.Shrine &&
                TrinityCombat.Targeting.CurrentTarget.Type != TrinityObjectType.HealthGlobe)
            {
                //Core.Logger.Log("Prevent Primary Attack ");
                var targetPosition = TargetUtil.GetLoiterPosition(TrinityCombat.Targeting.CurrentTarget, 20f);
                // return new TrinityPower(SNOPower.Walk, 7f, targetPosition);
                return targetPosition;
            }
            return TrinityCombat.Targeting.CurrentTarget.Position;
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
                           u.Distance <= maxRange &&
                           u.NearbyUnitsWithinDistance(aoe_radius) >= count
                     orderby u.NearbyUnitsWithinDistance(aoe_radius),
                         u.Distance descending
                     select u).FirstOrDefault();
            }
        }

        internal static List<TrinityActor> AuraUnits(SNOPower aura, float maxSearchRange = 65f,
            bool addUnitsInAoE = false)
        {
            return (from u in SafeList(addUnitsInAoE)
                    where u.IsUnit &&
                          u.Distance <= maxSearchRange &&
                          u.HasBeenInLoS && !u.HasDebuff(aura)
                    orderby u.NearbyUnitsWithinDistance(),
                        u.Distance
                    select u).ToList();
        }

        internal static TrinityActor BestAuraUnit(SNOPower aura, float maxSearchRange = 65f,
            bool addUnitsInAoE = false)
        {
            return (from u in SafeList(addUnitsInAoE)
                    where u.IsUnit &&
                          u.Distance <= maxSearchRange &&
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
                          u.Distance <= maxRange && u.IsInLineOfSight &&
                          !(ignoreUnitsInAoE && u.IsInAvoidance) &&
                          !(ignoreElites && u.IsElite)
                    orderby u.CountUnitsInFront() descending
                    select u).ToList();
        }

        internal static List<TrinityActor> TargetsInFrontOfMe(float maxRange, float skillRadius, bool ignoreUnitsInAoE = false,
            bool ignoreElites = false)
        {
            return (from u in SafeList(ignoreElites)
                    where (u.IsUnit || u.IsDestroyable) &&
                          u.Distance <= maxRange && u.IsInLineOfSight &&
                          !(ignoreUnitsInAoE && u.IsInAvoidance) &&
                          !(ignoreElites && u.IsElite)
                    orderby u.CountUnitsInFront(skillRadius) descending
                    select u).ToList();
        }

        public static List<TrinityActor> UnitsBetweenLocations(Vector3 fromLocation, Vector3 toLocation)
        {
            return
                (from u in Core.Targets
                 where u.IsUnit &&
                       MathUtil.IntersectsPath(u.Position, u.Radius, fromLocation, toLocation)
                 select u).ToList();
        }

        internal static List<TrinityActor> FreezeTargetsInFrontOfMe(float maxRange, float skillRadius,
            bool ignoreUnitsInAoE = false, bool ignoreElites = false)
        {
            return (from u in SafeList(ignoreElites)
                where u.IsUnit &&
                      u.Distance <= maxRange && u.IsInLineOfSight &&
                      !(ignoreUnitsInAoE && (u.IsInAvoidance ||
                        u.IsAvoidanceOnPath || u.IsCriticalAvoidanceOnPath)) &&
                      !(ignoreElites && u.IsElite) && !u.IsFrozen

                orderby u.CountUnitsInFront(skillRadius) descending
                select u).ToList();
        }

        internal static TrinityActor GetBestPierceTarget(float maxRange, bool ignoreUnitsInAoE = false,
            bool ignoreElites = false)
        {
            var result = TargetsInFrontOfMe(maxRange, ignoreUnitsInAoE, ignoreElites).FirstOrDefault();
            return result ?? BestAoeUnit(maxRange, !ignoreUnitsInAoE);
        }

        internal static TrinityActor GetBestPierceTarget(float maxRange, float skillRadius, bool ignoreUnitsInAoE = false,
            bool ignoreElites = false)
        {
            var result = TargetsInFrontOfMe(maxRange, skillRadius, ignoreUnitsInAoE, ignoreElites).FirstOrDefault();
            return result ?? BestAoeUnit(maxRange, !ignoreUnitsInAoE);
        }

        internal static Vector3 GetBestPiercePoint(float maxRange, bool ignoreUnitsInAoE = false, bool ignoreElites = false)
        {
            var unit = GetBestPierceTarget(maxRange, ignoreUnitsInAoE, ignoreElites);
            return unit?.Position ?? Vector3.Zero;
        }

        internal static Vector3 GetBestPiercePoint(float maxRange, float skillRadius, bool ignoreUnitsInAoE = false, bool ignoreElites = false)
        {
            var unit = GetBestPierceTarget(maxRange, skillRadius, ignoreUnitsInAoE, ignoreElites);
            return unit?.Position ?? Vector3.Zero;
        }

        public static bool PierceHitsMonster(Vector3 position)
        {
            if (position.Distance(Player.Position) > 30f)
                position = MathEx.CalculatePointFrom(Player.Position, position, 30f);

            return TrinityGrid.Instance.IsIntersectedByFlags(Player.Position, position, AvoidanceFlags.Monster);
        }

        internal static Vector3 FreezePiercePoint(float maxRange, float skillRadius,
            bool ignoreUnitsInAoE = false, bool ignoreElites = false)
        {
            var unit = FreezeTargetsInFrontOfMe(maxRange, skillRadius, ignoreUnitsInAoE, ignoreElites).FirstOrDefault();
            return unit?.Position ?? Vector3.Zero;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="clusterRadius">Cluster Radius</param>
        /// <param name="maxSearchRange">Unit Max Distance</param>
        /// <param name="useWeights">Include Mobs with Weight or not</param>
        /// <param name="includeUnitsInAoe">Include mobs in AoE or not</param>
        /// <param name="ignoreElites">Ingore elites or not</param>
        /// <param name="inLineOfSight">Requires Line of Sight</param>
        /// <returns></returns>
        internal static TrinityActor GetBestClusterUnit(
            float clusterRadius = 15f, float maxSearchRange = 65f, bool useWeights = true, bool includeUnitsInAoe = true,
            bool ignoreElites = false, bool inLineOfSight = false)
        {
            return GetBestClusterUnits(clusterRadius, maxSearchRange, useWeights, includeUnitsInAoe, ignoreElites, inLineOfSight).FirstOrDefault();
        }

        internal static IEnumerable<TrinityActor> GetBestClusterUnits(
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
                 select u);

            return clusterUnits;
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

        internal static Vector3 ClosestOcculous(float maxRange, Vector3 fromLocation, bool objectsInAoe = false)
        {
            var TrinityActor = GetOculusBuffDiaObjects(maxRange, fromLocation, objectsInAoe).OrderBy(x => x.Distance).FirstOrDefault();
            return TrinityActor?.Position ?? Vector3.Zero;
        }

        internal static List<TrinityActor> GetOculusBuffDiaObjects(float range, Vector3 fromLocation, bool objectsInAoe = false)
        {
            //[1FABA194] Type: ClientEffect Name: p2_itemPassive_unique_ring_017_dome-58267 ActorSnoId: 433966, Distance: 24.701

            return
                (from u in SafeList(objectsInAoe)
                 where fromLocation.Distance2D(u.Position) <= range &&
                       u.ActorSnoId == 433966
                 orderby u.Distance
                 select u).ToList();
        }

        internal static Vector3 ClosestSancAndOcc(float maxRange, Vector3 fromLocation, bool objectsInAoe = false)
        {
            foreach (var item in GetInnerSanctuaryDiaObjects(maxRange, fromLocation, objectsInAoe).Select(x => x.Position).ToList())
            {
                var occPoint = GetOculusBuffDiaObjects(maxRange, fromLocation, objectsInAoe).OrderBy(x => x.Distance)
                    .Select(y => y.Position)
                    .FirstOrDefault(z => z.Distance2D(item) < 3);
                if (occPoint != Vector3.Zero)
                    return MathEx.CalculatePointFrom(item, occPoint, item.Distance2D(occPoint) / 2);
            }
            return Vector3.Zero;
        }

        internal static Vector3 ClosestSanctuary(float maxRange, Vector3 fromLocation, bool objectsInAoe = false)
        {
            var TrinityActor = GetInnerSanctuaryDiaObjects(maxRange, fromLocation, objectsInAoe).OrderBy(x => x.Distance).FirstOrDefault();
            return TrinityActor?.Position ?? Vector3.Zero;
        }

        internal static List<TrinityActor> GetInnerSanctuaryDiaObjects(float range, Vector3 fromLocation,
            bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                 where fromLocation.Distance2D(u.Position) <= range &&
                       u.ActorSnoId == 320136
                 orderby u.Distance
                 select u).ToList();
        }

        internal static TrinityActor ClosestGlobe(float distance = 45, bool objectsInAoe = false)
        {
            return (from u in SafeList(objectsInAoe)
                    where
                        (u.Type == TrinityObjectType.HealthGlobe || u.Type == TrinityObjectType.PowerGlobe) &&
                        u.Distance <= distance
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
                 where u.Distance <= range &&
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
                       u.Distance <= maxRange
                 orderby u.NearbyUnitsWithinDistance(radius),
                     u.Distance descending
                 select u.Position).ToList();

            return clusterUnits.Any() ? clusterUnits.FirstOrDefault() : Vector3.Zero;
        }

        internal static List<TrinityActor> GetTwisterDiaObjects(float range = 25f, bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                 where u.Distance <= range &&
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
                       u.Distance <= maxRange
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

        public static void ClearCurrentTarget(string reason)
        {
            if (TrinityCombat.Targeting.CurrentTarget != null)
            {
                var clearString = "Clearing CURRENT TARGET: " + reason +
                        $"{Environment.NewLine} Name: {CurrentTarget.InternalName} Type: {CurrentTarget.Type} SNO: {CurrentTarget.ActorSnoId} Distance: {CurrentTarget.Distance} " +
                        $"{Environment.NewLine} Weight: {CurrentTarget.Weight} Info: {CurrentTarget.WeightInfo}";
                Core.Logger.Verbose(LogCategory.Weight, clearString);
                //Combat.Targeting.CurrentTarget = null;
            }
        }

        #region Helper fields

        private static List<TrinityActor> ObjectCache => Core.Targets.Entries;

        private static PlayerCache Player => Core.Player;

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

        private static TrinityActor CurrentTarget => TrinityCombat.Targeting.CurrentTarget;

        private static HashSet<SNOPower> Hotbar => Core.Hotbar.ActivePowers;

        #endregion Helper fields

        public static int CountUnitsBehind(TrinityActor actor, float range)
        {
            return
                (from u in ObjectCache
                 where u.RActorId != actor.RActorId &&
                       u.IsUnit &&
                       MathUtil.IntersectsPath(actor.Position, actor.Radius, Core.Player.Position, u.Position)
                 select u).Count();
        }

        public static int CountUnitsInFront(TrinityActor actor)
        {
            return
                (from u in Core.Targets
                 where u.RActorId != actor.RActorId &&
                       u.IsUnit &&
                       MathUtil.IntersectsPath(u.Position, u.Radius, Core.Player.Position, actor.Position)
                 select u).Count();
        }

        public static int CountUnitsInFront(TrinityActor actor, float skillRadius)
        {
            return
                (from u in Core.Targets
                 where u.RActorId != actor.RActorId &&
                       u.IsUnit &&
                       MathUtil.IntersectsPath(u.Position, u.Radius + skillRadius, Core.Player.Position, actor.Position)
                 select u).Count();
        }

        public static bool IsFacing(TrinityActor actor, Vector3 targetPosition, float arcDegrees = 70f)
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

        public static int NearbyUnitsWithinDistance(TrinityActor actor, float range = 5f, bool unitsOnly = true)
        {
            if (unitsOnly && actor.Type != TrinityObjectType.Unit)
                return 0;

            return Core.Targets.Count(u => u.RActorId != actor.RActorId && u.IsUnit && u.Position.Distance(actor.Position) <= range && u.HasBeenInLoS);          
        }

        public static IEnumerable<TrinityActor> NearbyTargets(TrinityActor actor, float range = 5f)
        {
            return Core.Targets.Where(u => u.RActorId != actor.RActorId && u.IsUnit && u.Position.Distance(actor.Position) <= range && u.HasBeenInLoS);
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
        /// <param name="arcDegrees"></param>
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
            if (Core.Settings.Weighting.EliteWeighting == SettingMode.Disabled)
            {
                return
                    (from u in ObjectCache
                     where u.IsUnit &&
                     !u.IsElite &&
                     u.Weight > 0 &&
                     u.RadiusDistance <= TrinityCombat.Routines.Current.ClusterRadius
                     select u).Count() >= TrinityCombat.Routines.Current.TrashRange;
            }
            return
                (from u in ObjectCache
                 where u.IsUnit && u.IsValid &&
                       u.Weight > 0 &&
                       u.IsElite &&
                       u.RadiusDistance <= range
                 select u).Any();
        }

        /// <summary>
        /// Checks to make sure there's at least one valid cluster with the minimum monster count
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="minCount"></param>
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

            if (forceElites && ObjectCache.Any(u => u.IsUnit && u.IsElite && u.RadiusDistance < maxRange))
                return true;

            var clusterCheck =
                (from u in ObjectCache
                 where u.IsUnit && u.IsValid &&
                 u.RadiusDistance <= maxRange &&
                 u.NearbyUnitsWithinDistance(radius) - 1 >= minCount
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

            if (ObjectCache.Any(u => u.IsUnit && u.IsElite && u.RadiusDistance < 200))
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

        //internal static List<TrinityActor> TargetsInFrontOfMe(float maxRange, bool ignoreUnitsInAoE = false, bool ignoreElites = false)
        //{
        //    return (from u in ObjectCache
        //            where u.IsUnit &&
        //            u.RadiusDistance <= maxRange && u.HasBeenInLoS &&
        //            !(ignoreUnitsInAoE && u.IsStandingInAvoidance) &&
        //            !(ignoreElites && u.IsElite)
        //            orderby u.CountUnitsInFront() descending,
        //            u.IsElite descending
        //            select u).ToList();
        //}
        //internal static TrinityActor GetBestPierceTarget(float maxRange, bool ignoreUnitsInAoE = false, bool ignoreElites = false)
        //{
        //    var result = TargetsInFrontOfMe(maxRange, ignoreUnitsInAoE, ignoreElites).FirstOrDefault();
        //    if (result != null)
        //        return result;

        //    if (CurrentTarget != null)
        //        return CurrentTarget;

        //    return GetBestClusterUnit(15f, maxRange, 1, true, !ignoreUnitsInAoE, ignoreElites);
        //}

        internal static TrinityActor GetBestArcTarget(float maxRange, float arcDegrees)
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

        //private static Vector3 GetBestAoEMovementPosition()
        //{
        //    Vector3 _bestMovementPosition = Vector3.Zero;

        //    if (HealthGlobeExists(25) && Player.CurrentHealthPct < Core.Settings.Combat.Barbarian.HealthGlobeLevel)
        //        _bestMovementPosition = GetBestHealthGlobeClusterPoint(7, 25);
        //    else if (PowerGlobeExists(25))
        //        _bestMovementPosition = GetBestPowerGlobeClusterPoint(7, 25);
        //    else if (GetFarthestClusterUnit(7, 25, 4) != null && !CurrentTarget.IsElite && !CurrentTarget.IsTreasureGoblin)
        //        _bestMovementPosition = GetFarthestClusterUnit(7, 25).Position;
        //    else if (_bestMovementPosition == Vector3.Zero)
        //        _bestMovementPosition = CurrentTarget.Position;

        //    return _bestMovementPosition;
        //}

        //internal static TrinityActor GetFarthestClusterUnit(float aoe_radius = 25f, float maxRange = 65f, int count = 1, bool useWeights = true, bool includeUnitsInAoe = true)
        //{
        //    if (aoe_radius < 1f)
        //        aoe_radius = 1f;
        //    if (maxRange > 300f)
        //        maxRange = 300f;

        //    using (new PerformanceLogger("TargetUtil.GetFarthestClusterUnit"))
        //    {
        //        TrinityActor bestClusterUnit;
        //        var clusterUnits =
        //            (from u in ObjectCache
        //             where ((useWeights && u.Weight > 0) || !useWeights) &&
        //             (includeUnitsInAoe || !UnitOrPathInAoE(u)) &&
        //             u.RadiusDistance <= maxRange &&
        //             u.NearbyUnitsWithinDistance(aoe_radius) >= count
        //             orderby u.Type != TrinityObjectType.HealthGlobe && u.Type != TrinityObjectType.PowerGlobe,
        //             u.NearbyUnitsWithinDistance(aoe_radius),
        //             u.Distance descending
        //             select u).ToList();

        //        if (clusterUnits.Any())
        //            bestClusterUnit = clusterUnits.FirstOrDefault();
        //        else if (Combat.Targeting.CurrentTarget != null)
        //            bestClusterUnit = Combat.Targeting.CurrentTarget;
        //        else
        //            bestClusterUnit = default(TrinityActor);

        //        return bestClusterUnit;
        //    }
        //}
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
                bestClusterPoint = Core.Player.Position;

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
                bestClusterPoint = Core.Player.Position;

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

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="radius">Cluster Radius</param>
        ///// <param name="maxRange">Unit Max Distance</param>
        ///// <param name="count">Minimum number of mobs</param>
        ///// <param name="useWeights">Include Mobs with Weight or not</param>
        ///// <param name="includeUnitsInAoe">Include mobs in AoE or not</param>
        ///// <param name="ignoreElites">Ingore elites or not/param>
        ///// <returns></returns>
        //internal static TrinityActor GetBestClusterUnit(
        //    float radius = 15f, float maxRange = 65f, int count = 1, bool useWeights = true, bool includeUnitsInAoe = true, bool ignoreElites = false)
        //{
        //    if (radius < 1f)
        //        radius = 1f;
        //    if (maxRange > 300f)
        //        maxRange = 300f;

        //    TrinityActor bestClusterUnit;
        //    var clusterUnits =
        //        (from u in ObjectCache
        //         where u.IsUnit &&
        //         ((useWeights && u.Weight > 0) || !useWeights) &&
        //         (includeUnitsInAoe || !UnitOrPathInAoE(u)) &&
        //         !(ignoreElites && u.IsElite) &&
        //         u.RadiusDistance <= maxRange && !u.IsSafeSpot
        //         orderby u.IsTrashMob,
        //          u.NearbyUnitsWithinDistance(radius) descending,
        //          u.Distance,
        //          u.HitPointsPct descending
        //         select u).ToList();

        //    if (clusterUnits.Any())
        //        bestClusterUnit = clusterUnits.FirstOrDefault();
        //    else if (Combat.Targeting.CurrentTarget != null)
        //        bestClusterUnit = Combat.Targeting.CurrentTarget;
        //    else
        //        bestClusterUnit = default(TrinityActor);

        //    return bestClusterUnit;
        //}

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

            bool includeHealthGlobes = ObjectCache.Any(g => g.Type == TrinityObjectType.HealthGlobe && g.Weight > 0) &&
                                       (!PlayerMover.IsBlocked);

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
            else if (TrinityCombat.Targeting.CurrentTarget != null)
                bestClusterPoint = TrinityCombat.Targeting.CurrentTarget.Position;
            else
                bestClusterPoint = Core.Player.Position;

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
        /// <param name="useWeights"></param>
        /// <returns></returns>
        internal static bool AnyMobsInRange(float range = 10f, bool useWeights = true)
        {
            return AnyMobsInRange(range, 1, useWeights);
        }

        /// <summary>
        /// Fast check to see if there are any attackable units within a certain distance
        /// </summary>
        /// <param name="range"></param>
        /// <param name="minCount"></param>
        /// <param name="useWeights"></param>
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
        internal static IEnumerable<TrinityActor> UnitsInRangeOfPosition(Vector3 position, float range = 15f)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                        u.Weight > 0 &&
                        u.Position.Distance(position) <= range
                    select u);
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
            if (Core.Settings.Weighting.EliteWeighting == SettingMode.Disabled)
                return false;

            if (range < 5f)
                range = 5f;
            return (from o in ObjectCache
                    where o.IsUnit &&
                    o.IsElite &&
                    o.RadiusDistance <= range
                    select o).Any();
        }

        /// <summary>
        /// Fast check to see if there are any attackable Elite units within a certain distance
        /// </summary>
        /// <param name="range"></param>
        /// <param name="minCount"></param>
        /// <returns></returns>
        internal static bool AnyElitesInRange(float range = 10f, int minCount = 1)
        {
            if (Core.Settings.Weighting.EliteWeighting == SettingMode.Disabled)
                return false;

            if (range < 5f)
                range = 5f;
            if (minCount < 1)
                minCount = 1;
            return (from o in ObjectCache
                    where o.IsUnit &&
                    o.IsElite &&
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
                                        u.IsElite &&
                                        u.Position.Distance(position) <= range
                                select u).Count();

            return inRangeCount >= unitsRequired;
        }

        /// <summary>
        /// Count of elites within range of position
        /// </summary>
        internal static int ElitesInRange(float range = 15f, Vector3 position = default(Vector3))
        {
            if (position == Vector3.Zero)
                position = Core.Player.Position;

            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                            u.Weight > 0 &&
                            u.IsElite &&
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
            return TrinityCombat.Targeting.CurrentTarget != null && TrinityCombat.Targeting.CurrentTarget.IsElite && TrinityCombat.Targeting.CurrentTarget.RadiusDistance <= range;
        }

        /// <summary>
        /// Finds an optimal position for using Monk Tempest Rush out of combat
        /// </summary>
        /// <returns></returns>
        internal static Vector3 FindTempestRushTarget()
        {
            //Vector3 target = PlayerMover.LastMoveToTarget;
            //Vector3 myPos = ZetaDia.Me.Position;

            //if (Combat.Targeting.CurrentTarget != null && NavHelper.CanRayCast(myPos, target))
            //{
            //    target = Combat.Targeting.CurrentTarget.Position;
            //}

            //float distance = target.Distance(myPos);

            //if (distance < 30f)
            //{
            //    double direction = MathUtil.FindDirectionRadian(myPos, target);
            //    target = MathEx.GetPointAt(myPos, 40f, (float)direction);
            //}

            //return target;
            return TrinityCombat.Targeting.CurrentTarget?.Position ?? Vector3.Zero;
        }

        // Special Zig-Zag movement for whirlwind/tempest
        /// <summary>
        /// Finds an optimal position for Barbarian Whirlwind, Monk Tempest Rush, or Demon Hunter Strafe
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="ringDistance"></param>
        /// <param name="randomizeDistance"></param>
        /// <returns></returns>
        internal static Vector3 GetZigZagTarget(Vector3 origin, float ringDistance = 20f, bool randomizeDistance = false)
        {
            if (CurrentTarget == null)
                return Player.Position;

            if (origin.Distance(Player.Position) > ringDistance)
                return origin;

            const float minDistance = 5f;
            Vector3 myPos = Player.Position;

            Vector3 zigZagPoint;

            bool useTargetBasedZigZag = false;
            float maxDistance = 35f;
            int minTargets = 2;

            //if (Core.Player.ActorClass == ActorClass.Crusader)
            //{
            //    useTargetBasedZigZag = true;
            //}

            if (Core.Player.ActorClass == ActorClass.Monk)
            {
                maxDistance = 20f;
                minTargets = 3;
                useTargetBasedZigZag = true;
            }
            if (Core.Player.ActorClass == ActorClass.Barbarian)
            {
                useTargetBasedZigZag = true;
            }

            if (useTargetBasedZigZag && ObjectCache.Count(o => o.IsUnit) >= minTargets)
            {
                bool attackInAoe = true;
                var clusterPoint = GetBestClusterPoint(ringDistance, ringDistance, false, attackInAoe);
                if (clusterPoint.Distance(Player.Position) >= minDistance)
                {
                    Core.Logger.Log(LogCategory.Movement, "Returning ZigZag: BestClusterPoint {0} r-dist={1} t-dist={2}", clusterPoint, ringDistance, clusterPoint.Distance(Player.Position));
                    return clusterPoint;
                }

                List<TrinityActor> zigZagTargetList;
                if (attackInAoe)
                {
                    zigZagTargetList =
                        (from u in ObjectCache
                         where u.IsUnit && u.Distance < maxDistance && u.Weight > 0
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
                    if (TrinityGrid.Instance.CanRayCast(zigZagPoint) && zigZagPoint.Distance(Player.Position) >= minDistance)
                    {
                        Core.Logger.Log(LogCategory.Movement, "Returning ZigZag: TargetBased {0} r-dist={1} t-dist={2}", zigZagPoint, ringDistance, zigZagPoint.Distance(Player.Position));
                        return zigZagPoint;
                    }
                }
            }

            float highestWeightFound = Single.NegativeInfinity;
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
                    zigZagPoint.Z = Core.DBGridProvider.GetHeight(zigZagPoint.ToVector2());

                    // Make sure we're actually zig-zagging our target, except if we're kiting

                    float targetCircle = CurrentTarget.Radius;
                    if (targetCircle <= 5f)
                        targetCircle = 5f;
                    if (targetCircle > 10f)
                        targetCircle = 10f;

                    bool intersectsPath = MathUtil.IntersectsPath(CurrentTarget.Position, targetCircle, myPos, zigZagPoint);
                    if (RoutineBase.IsKitingEnabled && !intersectsPath)
                        continue;

                    //// if we're kiting, lets not actualy run through monsters
                    //if (CombatBase.KiteDistance > 0 && CacheData.MonsterObstacles.Any(m => m.Position.Distance(zigZagPoint) <= CombatBase.KiteDistance))
                    //    continue;

                    // Ignore point if any AoE in this point position
                    if (Core.Avoidance.Grid.IsLocationInFlags(zigZagPoint, AvoidanceFlags.Avoidance))
                        continue;

                    // Make sure this point is in LoS/walkable (not around corners or into a wall)
                    bool canRayCast = !Navigator.Raycast(Player.Position, zigZagPoint);
                    if (!canRayCast)
                        continue;

                    float distanceToPoint = zigZagPoint.Distance(myPos);

                    // Lots of weight for points further away from us (e.g. behind our CurrentTarget)
                    pointWeight *= distanceToPoint;

                    // Add weight for any units in this point
                    int monsterCount = ObjectCache.Count(u => u.IsUnit && u.Position.Distance(zigZagPoint) <= Math.Max(u.Radius, 10f) && u.Weight > 0);
                    if (monsterCount > 0)
                        pointWeight *= monsterCount;

                    //Core.Logger.Log(LogCategory.Movement, "ZigZag Point: {0} distance={1:0} distaceFromTarget={2:0} intersectsPath={3} weight={4:0} monsterCount={5}",
                    //    zigZagPoint, distanceToPoint, distanceFromTargetToPoint, intersectsPath, pointWeight, monsterCount);

                    // Use this one if it's more weight, or we haven't even found one yet, or if same weight as another with a random chance
                    if (pointWeight > highestWeightFound)
                    {
                        highestWeightFound = pointWeight;

                        //if (Core.Settings.Combat.Misc.UseNavMeshTargeting)
                        //{
                        bestLocation = new Vector3(zigZagPoint.X, zigZagPoint.Y, Core.DBGridProvider.GetHeight(zigZagPoint.ToVector2()));
                        //}
                        //else
                        //{
                        //    bestLocation = new Vector3(zigZagPoint.X, zigZagPoint.Y, zigZagPoint.Z + 4);
                        //}
                    }
                }
            }
            Core.Logger.Log(LogCategory.Movement, "Returning ZigZag: RandomXY {0} r-dist={1} t-dist={2}", bestLocation, ringDistance, bestLocation.Distance(Player.Position));
            return bestLocation;
        }

        /// <summary>
        /// Checks to see if a given Unit is standing in AoE, or if the direct paht-line to the unit goes through AoE
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        internal static bool UnitOrPathInAoE(TrinityActor u)
        {
            return Core.Avoidance.InAvoidance(u.Position) || PathToActorIntersectsAoe(u);
        }

        internal static bool IsPositionOnMonster(Vector3 position, float radiusDistance = 0f)
        {
            if (position == Vector3.Zero)
                return false;

            return Core.Targets.Any(m => m.IsUnit && m.IsHostile && m.Position.Distance(position) <= m.CollisionRadius + radiusDistance);
        }

        /// <summary>
        /// Checks to see if the path-line to a unit goes through AoE
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static bool PathToActorIntersectsAoe(TrinityActor obj)
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
        internal static bool IsUnitsWithDebuff(float range, Vector3 position, SNOPower power, int unitsRequiredWithDebuff = 1)
        {
            var unitsWithDebuff = (from u in ObjectCache
                                   where u.IsUnit && u.IsValid &&
                                          u.Weight > 0 &&
                                          u.Position.Distance(position) <= range &&
                                          SpellTracker.IsUnitTracked(u.AcdId, power)
                                   select u).ToList();

            // Make sure units exist
            //unitsWithDebuff.RemoveAll(u =>
            //{
            //    var acd = ZetaDia.Actors.GetACDById(u.AcdId);
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
                                       u.HasBeenInLoS
                                select u).ToList();

            var unitsWithoutDebuff = (from u in ObjectCache
                                      where u.IsUnit && u.IsValid &&
                                             u.Weight > 0 &&
                                             u.RadiusDistance <= range &&
                                             u.HasBeenInLoS &&
                                             !u.HasDebuff(power)
                                      select u).ToList();

            //Core.Logger.Log(LogCategory.Behavior, "{0}/{1} units without debuff {2} in {3} range", unitsWithoutDebuff.Count, unitsInRange.Count, power, range);

            return unitsWithoutDebuff.Count >= unitsRequiredWithoutDebuff;
        }

        /// <summary>
        /// Checks if for units without a debuff
        /// </summary>
        internal static TrinityActor ClosestUnit(float range, Func<TrinityActor, bool> condition = null)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                            u.Weight > 0 &&
                            u.RadiusDistance <= range &&
                            (condition == null || condition(u)) &&
                             u.HasBeenInLoS
                    orderby u.IsElite, u.RadiusDistance
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

            //Core.Logger.Debug("{0} of {6} mobs between {1} and {2} yards ({3:f2}%), needed={4}% result={5}", totalWithinBand, bandMinRange, bandMaxRange, percentWithinBand, percentage, percentWithinBand >= percentage, totalWithinMaxRange);

            return percentWithinBand >= percentage;
        }

        internal static TrinityActor GetBestHarvestTarget(float skillRange, float maxRange = 30f)
        {
            TrinityActor harvestTarget =
            (from u in ObjectCache
             where u.IsUnit && u.IsValid &&
             u.RadiusDistance <= maxRange &&
             u.IsElite &&
             u.HasBuffVisualEffect
             orderby u.NearbyUnitsWithinDistance(skillRange) descending
             select u).FirstOrDefault();
            if (harvestTarget != null)
                return harvestTarget;

            return (from u in ObjectCache
                    where u.IsUnit &&
                    u.RadiusDistance <= maxRange &&
                    u.HasBuffVisualEffect
                    orderby u.NearbyUnitsWithinDistance(skillRange) descending
                    select u).FirstOrDefault();
        }

        public static Vector3 BestSentryPosition()
        {
            var sentries = SpellHistory.History.Where(s => s.Power.SNOPower == SNOPower.DemonHunter_Sentry && s.TimeSinceUse.TotalSeconds < 12).ToList();
            var sentryPositions = new HashSet<Vector3>(sentries.Select(b => b.TargetPosition));
            Func<Vector3, bool> IsValidPosition = pos => !sentryPositions.Any(b => b.Distance(pos) <= 12f);

            var clusterPosition = TargetUtil.GetBestClusterPoint();
            if (IsValidPosition(clusterPosition))
            {
                return clusterPosition;
            }
            if (IsValidPosition(TrinityCombat.Targeting.CurrentTarget.Position))
            {
                return TrinityCombat.Targeting.CurrentTarget.Position;
            }
            if (IsValidPosition(Player.Position))
            {
                return Player.Position;
            }

            var closestUnit = TargetUtil.GetClosestUnit().Position;
            if (IsValidPosition(closestUnit))
            {
                return closestUnit;
            }
            return Player.Position;
        }

        internal static bool IsPercentOfMobsDebuffed(SNOPower power, float maxRange = 30f, float minPercent = 0.5f)
        {
            return DebuffedPercent(power, maxRange) >= minPercent;
        }

        internal static float DebuffedPercent(SNOPower power, float maxRange = 30f)
        {
            List<TrinityActor> output;
            return DebuffedPercent(power, maxRange, out output);
        }

        internal static float DebuffedPercent(SNOPower power, float maxRange, out List<TrinityActor> notDebuffed)
        {
            var total = 0;
            var debuffed = 0;

            notDebuffed = new List<TrinityActor>();

            foreach (var u in ObjectCache)
            {
                var isValidUnit = u != null && u.IsUnit && u.Attributes != null && !u.IsPlayer && u.RadiusDistance <= maxRange && u.HasBeenInLoS;
                if (!isValidUnit)
                    continue;

                total++;

                if (u.Attributes != null && !u.HasDebuff(power))
                {
                    notDebuffed.Add(u);
                }
                else
                {
                    debuffed++;
                }
            }

            if (total == 0) return 0;

            var pct = (float)(debuffed) / total;

            Core.Logger.Log(LogCategory.Behavior, "{0} out of {1} mobs have {3} ({2:0.##}%)", debuffed, total, pct * 100, power);

            return pct;

            //var debuffed = new HashSet<int>((from u in ObjectCache
            //                where u.IsUnit && u.IsValid && !u.IsPlayer &&
            //                u.RadiusDistance <= maxRange &&
            //                u.Attributes.Powers.ContainsKey(power) &&
            //                u.HasBeenInLoS
            //                select u.AcdId));

            //var all = (from u in ObjectCache
            //           where u.IsUnit && !u.IsPlayer &&
            //           u.RadiusDistance <= maxRange &&
            //           u.HasBeenInLoS
            //           select u).ToList();

            //notDebuffed = all.Where(p => !debuffed.Contains(p.AcdId)).ToList();

            //if (all.Count <= 0)
            //    return 0;

            //var pct = (float)debuffed.Count / all.Count;

            //Core.Logger.Log(LogCategory.Behavior, "{0} out of {1} mobs have {3} ({2:0.##}%)", debuffed, all, pct * 100, power);

            //return pct;
        }

        internal static int UnitsWithDebuff(SNOPower power, float maxRange = 30f)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                    u.RadiusDistance <= maxRange &&
                    u.HasDebuff(power)
                    select u).Count();
        }

        internal static int UnitsWithDebuff(IEnumerable<SNOPower> powers, float maxRange = 30f)
        {
            return (from u in ObjectCache
                    where u.IsUnit && u.IsValid &&
                    u.RadiusDistance <= maxRange &&
                    powers.Any(u.HasDebuff)
                    select u).Count();
        }

        internal static List<TrinityActor> UnitsWithoutDebuff(List<SNOPower> powers, float maxRange = 30f, IEnumerable<TrinityActor> units = null)
        {
            if (units == null)
                units = ObjectCache;

            return (from u in units
                    where u.IsUnit && u.IsValid &&
                    u.RadiusDistance <= maxRange &&
                    !powers.Any(u.HasDebuff) && !powers.Any(p => SpellTracker.IsUnitTracked(u.AcdId, p))
                    select u).ToList();
        }

        internal static List<TrinityActor> UnitsWithoutDebuff(SNOPower power, float maxRange = 30f, IEnumerable<TrinityActor> units = null)
            => UnitsWithoutDebuff(new List<SNOPower> { power }, maxRange, units);

        internal static IEnumerable<TrinityActor> UnitsWithDebuff(IEnumerable<SNOPower> powers, IEnumerable<TrinityActor> units)
        {
            return (from u in units
                    where u.IsUnit && u.IsValid &&
                          powers.Any(u.HasDebuff)
                    select u);
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

        internal static int DebuffCount(IEnumerable<SNOPower> powers, IEnumerable<TrinityActor> units)
        {
            return (from u in units
                    where u.IsUnit && u.IsValid &&
                    powers.Any(u.HasDebuff)
                    select powers.Count(u.HasDebuff)
                    ).Sum();
        }

        internal static TrinityActor LowestHealthTarget(float range, Vector3 position = new Vector3(), SNOPower withoutDebuff = SNOPower.None)
        {
            if (position == new Vector3())
                position = Player.Position;

            TrinityActor lowestHealthTarget;
            var unitsByHealth = (from u in ObjectCache
                                 where u.IsUnit && u.IsValid &&
                                        u.Weight > 0 &&
                                        u.Position.Distance(position) <= range &&
                                        (withoutDebuff == SNOPower.None || (!SpellTracker.IsUnitTracked(u.AcdId, withoutDebuff) && !u.HasDebuff(withoutDebuff)))
                                 //!CacheData.MonsterObstacles.Any(m => MathUtil.IntersectsPath(m.Position, m.Radius, u.Position, Player.Position))
                                 orderby u.HitPoints ascending
                                 select u).ToList();

            if (unitsByHealth.Any())
                lowestHealthTarget = unitsByHealth.FirstOrDefault();
            else if (TrinityCombat.Targeting.CurrentTarget != null)
                lowestHealthTarget = TrinityCombat.Targeting.CurrentTarget;
            else
                lowestHealthTarget = default(TrinityActor);

            return lowestHealthTarget;
        }

        internal static TrinityActor BestExploadingPalmTarget(float range, Vector3 position = new Vector3())
        {
            if (position == new Vector3())
                position = Player.Position;

            TrinityActor lowestHealthTarget;
            var unitsByHealth = (from u in ObjectCache
                                 where u.IsUnit && u.IsValid &&
                                        u.Weight > 0 &&
                                        u.Position.Distance(position) <= range &&
                                        !SpellTracker.IsUnitTracked(u.AcdId, SNOPower.Monk_ExplodingPalm) &&
                                        //!CacheData.MonsterObstacles.Any(m => MathUtil.IntersectsPath(m.Position, m.Radius, u.Position, Player.Position)) &&
                                        u.IsInLineOfSight
                                 orderby u.HitPoints ascending
                                 select u).ToList();

            if (unitsByHealth.Any())
                lowestHealthTarget = unitsByHealth.FirstOrDefault();
            else if (TrinityCombat.Targeting.CurrentTarget != null)
                lowestHealthTarget = TrinityCombat.Targeting.CurrentTarget;
            else
                lowestHealthTarget = default(TrinityActor);

            return lowestHealthTarget;
        }

        /// <summary>
        /// Finds a target with low hitpoints and without the exploding palm debuff
        /// </summary>
        internal static TrinityActor BestExplodingPalmTarget(float range)
        {
            var units = (from u in ObjectCache
                         where u.IsUnit && u.IsValid &&
                                u.Weight > 0 &&
                                u.Position.Distance(Player.Position) <= range &&
                                !u.Attributes.Powers.ContainsKey(SNOPower.Monk_ExplodingPalm)
                         orderby u.HitPoints
                         select u).ToList();

            return units.Any() ? units.FirstOrDefault() : null;
        }

        /// <summary>
        /// Necromancer's Decrepify has two distinc debuff SNOPowers, both of which are not always applied for some reason,
        /// so this will also check if the target is already slowed to avoid casting on enemies that are already cursed.
        /// </summary>
        internal static TrinityActor BestDecrepifyTarget (float range)
        {
            var units = (from u in ObjectCache
                         where u.IsUnit && u.IsValid &&
                         u.Position.Distance(Player.Position) <= range &&
                         !(u.Attributes.Powers.ContainsKey(SNOPower.P6_Necro_Decrepify) ||
                         u.Attributes.Powers.ContainsKey(SNOPower.P6_Necro_PassiveManager_Decrepify) ||
                         u.Attributes.IsSlowed)
                         orderby u.RadiusDistance descending
                         select u).ToList();

            return units.Any() ? units.FirstOrDefault() : null;
        }

        internal static TrinityActor BestTargetWithoutDebuff(float range, SNOPower debuff, Vector3 position = default(Vector3))
        {
            return BestTargetWithoutDebuffs(range, new List<SNOPower> { debuff }, position);
        }


        internal static TrinityActor BestTargetWithoutDebuffs(float range, IEnumerable<SNOPower> debuffs, Vector3 position = default(Vector3))
        {
            if (position == new Vector3())
                position = Player.Position;

            TrinityActor target;
            var unitsByWeight = (from u in ObjectCache
                                 where u.IsUnit && u.IsValid &&
                                        u.Weight > 0 &&
                                        u.Position.Distance(position) <= range &&
                                        !debuffs.All(u.HasDebuff)
                                 orderby u.Weight descending
                                 select u).ToList();

            if (unitsByWeight.Any())
                target = unitsByWeight.FirstOrDefault();
            else if (TrinityCombat.Targeting.CurrentTarget != null)
                target = TrinityCombat.Targeting.CurrentTarget;
            else
                target = default(TrinityActor);

            return target;
        }

        internal static TrinityActor GetDashStrikeFarthestTarget(float maxRange, float procDistance = 33f, int arcDegrees = 0)
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

            bool includeHealthGlobes = false;

            Vector3 bestClusterPoint;
            var clusterUnits =
                (from u in ObjectCache
                 where (u.IsUnit || (includeHealthGlobes && u.Type == TrinityObjectType.HealthGlobe)) &&
                       ((useWeights && u.Weight > 0) || !useWeights) &&
                       (includeUnitsInAoe || !UnitOrPathInAoE(u)) &&
                       u.RadiusDistance <= maxRange && u.Distance >= procDistance
                 orderby u.Type != TrinityObjectType.HealthGlobe,
                     // if it's a globe this will be false and sorted at the top
                     !u.IsElite,
                     u.NearbyUnitsWithinDistance(radius) descending,
                     u.Distance,
                     u.HitPointsPct descending
                 select u.Position).ToList();

            if (clusterUnits.Any())
                bestClusterPoint = clusterUnits.FirstOrDefault();
            else
                bestClusterPoint = Core.Player.Position;

            return bestClusterPoint;
        }

        internal static TrinityActor GetClosestUnit(float maxDistance = 100f)
        {
            var result =
                (from u in ObjectCache
                 where u.IsUnit && u.IsValid && u.Weight > 0 && u.RadiusDistance <= maxDistance
                 orderby u.Distance
                 select u).FirstOrDefault();

            return result;
        }

        internal static bool AnyBossesInRange(float range)
        {
            return NumBossInRangeOfPosition(Core.Player.Position, range) > 0;
        }

        public static bool AvoidancesInRange(float radius)
        {
            return Core.Avoidance.InAvoidance(Player.Position);
        }

        public static TrinityActor BestEliteInRange(float range)
        {
            return (from u in ObjectCache
                    where u.IsUnit &&
                    u.IsElite &&
                    u.Distance <= range
                    orderby
                     u.NearbyUnitsWithinDistance(range) descending,
                     u.Distance,
                     u.HitPointsPct descending
                    select u).FirstOrDefault();
        }

        public static List<TrinityActor> GetHighValueRiftTargets(float maxRange, double minValuePercent)
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

        private static double GetRiftValueWithinDistance(TrinityActor obj, float distance)
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

        public static Vector3 Centroid(IEnumerable<Vector3> points)
        {
            var result = Vector3.Zero;
            foreach (var point in points)
                result = result + point;
            result /= points.Count();
            return result;
        }

        public static Vector3 GetLoiterPosition(TrinityActor target, float radiusDistance)
        {
            if (target == null)
                return Vector3.Zero;

            var circlePositions = StuckHandler.GetCirclePoints(16, target.AxialRadius + radiusDistance, target.Position);
            circlePositions.AddRange(StuckHandler.GetCirclePoints(16, target.AxialRadius + radiusDistance + 15f, target.Position));

            bool IsValid(Vector3 p) => TrinityGrid.Instance.CanRayCast(p) &&
                                       !IsPositionOnMonster(p, radiusDistance);

            var validPositions = circlePositions.Where(IsValid).ToList();
            if (!validPositions.Any())
            {
                // Try with points around the player instead
                circlePositions = StuckHandler.GetCirclePoints(12, 30f, Player.Position);
                validPositions = circlePositions.Where(IsValid).ToList();
            }

            validPositions = validPositions.OrderBy(p => p.Distance(Player.Position)).ToList();
            return validPositions.FirstOrDefault();

            //// Estimate based on the target size how far away we should be from it.
            //var walkTarget = target?.Position ?? GetBestClusterPoint();
            //var targetRadius = target?.Radius + radiusDistance ?? 11f + radiusDistance;

            //// Get points in a circle around the target and make sure they are walkable and there's no monsters near them.
            //var circlePositions = StuckHandler.GetCirclePoints(8, targetRadius, walkTarget);
            //circlePositions.AddRange(StuckHandler.GetCirclePoints(8, targetRadius + 12f, walkTarget));
            //Func<Vector3, bool> isValid = p => NavHelper.CanRayCast(p) && !IsPositionOnMonster(p) && p.Distance(Player.Position) > 8f;
            //var validPositions = circlePositions.Where(isValid);

            //if (!validPositions.Any())
            //{
            //    // Try with points around the player instead
            //    circlePositions = StuckHandler.GetCirclePoints(12, 30f, Player.Position);
            //    validPositions = circlePositions.Where(isValid);
            //}

            //validPositions = validPositions.OrderBy(p => p.Distance(Player.Position));

            //// Draw it on the visualizer
            //RadarDebug.Draw(validPositions, 50);
            //return validPositions.FirstOrDefault();
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

            bool IsValid(Vector3 p) => TrinityGrid.Instance.CanRayCast(p) && !IsPositionOnMonster(p);

            var circlePositions = StuckHandler.GetCirclePoints(8, 10, Player.Position);

            if (distance >= 20)
                circlePositions.AddRange(StuckHandler.GetCirclePoints(8, 20, Player.Position));

            if (distance >= 30)
                circlePositions.AddRange(StuckHandler.GetCirclePoints(8, 30, Player.Position));

            var closestMonster = GetClosestUnit();
            var proximityTarget = closestMonster?.Position ?? Player.Position;
            var validPositions = circlePositions.Where(IsValid).OrderByDescending(p => p.Distance(proximityTarget));

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

        private static readonly HashSet<ActorAttributeType> _debuffSlots = new HashSet<ActorAttributeType>
        {
            ActorAttributeType.PowerBuff0VisualEffect,
            ActorAttributeType.PowerBuff0VisualEffectNone,
            ActorAttributeType.PowerBuff0VisualEffectA,
            ActorAttributeType.PowerBuff0VisualEffectB,
            ActorAttributeType.PowerBuff0VisualEffectC,
            ActorAttributeType.PowerBuff0VisualEffectD,
            ActorAttributeType.PowerBuff0VisualEffectE,
            ActorAttributeType.PowerBuff1VisualEffectNone,
            ActorAttributeType.PowerBuff1VisualEffectC,
            ActorAttributeType.PowerBuff2VisualEffectNone,
            ActorAttributeType.PowerBuff2VisualEffectE,
            ActorAttributeType.PowerBuff3VisualEffectNone,
            ActorAttributeType.PowerBuff3VisualEffectE,
            ActorAttributeType.PowerBuff4VisualEffectNone,
            ActorAttributeType.PowerBuff4VisualEffectC,
            ActorAttributeType.PowerBuff4VisualEffectNone,
            ActorAttributeType.PowerBuff1VisualEffectA,
            ActorAttributeType.PowerBuff1VisualEffectB,
            ActorAttributeType.PowerBuff1VisualEffectD,
            ActorAttributeType.PowerBuff1VisualEffectE,
            ActorAttributeType.PowerBuff2VisualEffectA,
            ActorAttributeType.PowerBuff2VisualEffectB,
            ActorAttributeType.PowerBuff2VisualEffectC,
            ActorAttributeType.PowerBuff2VisualEffectD,
            ActorAttributeType.PowerBuff3VisualEffectA,
            ActorAttributeType.PowerBuff3VisualEffectB,
            ActorAttributeType.PowerBuff3VisualEffectC,
            ActorAttributeType.PowerBuff3VisualEffectD,
        };

        public static bool HasDebuff(this TrinityActor obj, SNOPower debuffSNO)
        {
            if (obj?.CommonData == null || !obj.IsValid)
                return false;

            if (SpellTracker.IsUnitTracked(obj.AcdId, debuffSNO))
                return true;

            // todo: AttributesWrapper needs to be updated, throwing exceptions so the objects probably changed.
            //if (obj.Attributes.Powers.ContainsKey(debuffSNO))
            //    return true;

            //if (obj.Attributes.Powers.ContainsKey(debuffSNO))
            //    return true;

            //ActorAttributeType att;
            //Enum.TryParse("PowerBuff0VisualEffect" + Skills.Monk.ExplodingPalm.CurrentRune.TypeId, out att);

            ////todo this is needlessly slow checking all these attributes, trace the spells being used and record only the required slots
            ////or better yet, cache collection of attributes in buff visual effect slots keyed by snopower

            //var sno = (int)debuffSNO;

            try
            {
                return _debuffSlots.Any(attr => obj.Attributes.GetAttribute<bool>(attr, (int)debuffSNO));
            }
            catch (Exception)
            {
            }

            return false;
        }

        //private static double GetRiftValueWithinDistance(TrinityActor obj, float distance)
        //{
        //    return ObjectCache.Where(u => u.Position.Distance(obj.Position) <= distance).Sum(u => u.RiftValuePct);
        //}

        public static Vector3 GetCentroid(IEnumerable<Vector3> points)
        {
            var result = points.Aggregate(Vector3.Zero, (current, point) => current + point);
            result /= points.Count();
            return result;
        }

        public static IEnumerable<TrinityActor> FindPets(PetType pet0, float withinRange = 500f, Vector3 ofLocation = default(Vector3), bool ownedByMe = true)
        {
            if (ofLocation == Vector3.Zero)
                ofLocation = Player.Position;

            return Core.Actors.Actors.Where(u
                => (!ownedByMe || u.IsSummonedByPlayer)
                   && u.PetType == PetType.Pet0
                   && u.Position.Distance(ofLocation) <= withinRange);
        }

        public static IEnumerable<TrinityActor> UnitsWithinRangeOfPet(PetType type, float rangeFromPet)
        {
            var petLocations = FindPets(type, 80f, Core.Player.Position).Select(p => p.Position).ToList();
            return Core.Targets.Where(u => u.IsUnit && petLocations.Any(p => p.Distance(u.Position) <= rangeFromPet));
        }

        private static IEnumerable<TrinityPlayer> Players => Core.Actors.Actors.OfType<TrinityPlayer>();

        public static bool AnyPlayer(Func<TrinityPlayer, bool> condition)
        {
            return Players.Any(condition);
        }

        public static TrinityActor BestLOSEliteInRange(float range, bool objectsInAoe = false)
        {
            return (from u in SafeList(objectsInAoe)
                    where u.IsUnit &&
                          u.IsElite &&
                          u.IsInLineOfSight &&
                          u.Distance <= range
                    orderby
                        u.NearbyUnitsWithinDistance(15) descending,
                        u.HitPointsPct descending
                    select u).FirstOrDefault();
        }

        public static TrinityActor BestRangedAoeUnit(float clusterRadius = 7, float maxSearchRange = 50, int unitCount = 3, bool useWeights = false, bool includeUnitsInAoE = true)
        {
            return BestLOSEliteInRange(maxSearchRange, includeUnitsInAoE) ??
                    GetFarthestClusterUnit(clusterRadius, maxSearchRange, unitCount, useWeights, includeUnitsInAoE);
        }

        public static Vector3 GetBestCorpsePoint(float rangeFromPlayer, float targetRegionRadius)
        {
            var corpses = GetCorpses(rangeFromPlayer).ToList();
            var clusterCorpse = (from u in corpses
                                 where u.IsInLineOfSight
                                 where NearbyTargets(u).Any()
                                 orderby
                        NearbyUnitsWithinDistance(u, targetRegionRadius, false) descending,
                        u.Distance ascending 
                    select u).FirstOrDefault();

            if (clusterCorpse != null)
                return clusterCorpse.Position;

            //if (corpses.Any())
            //    return corpses.First().Position;

            return Vector3.Zero;
        }

        public static int CorpseCount(float radius)
            => Core.Actors.Count(a => GameData.NecroCorpseSnoIds.Contains(a.ActorSnoId) && a.Distance <= radius);

        public static int CorpseCount(float radius, Vector3 position)
            => Core.Actors.Count(a => GameData.NecroCorpseSnoIds.Contains(a.ActorSnoId) && a.Position.Distance(position) <= radius);

        public static IEnumerable<TrinityActor> GetCorpses(float radius)
            => Core.Actors.Where(a => GameData.NecroCorpseSnoIds.Contains(a.ActorSnoId) && a.Distance <= radius);

        public static IEnumerable<TrinityActor> GetCorpses(float radius, Vector3 position)
            => Core.Actors.Where(a => GameData.NecroCorpseSnoIds.Contains(a.ActorSnoId) && a.Position.Distance(position) <= radius);


    }
}