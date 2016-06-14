using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Cache;
using Trinity.Combat.Abilities;
using Trinity.Config;
using Trinity.Config.Combat;
using Trinity.DbProvider;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Technicals;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance
{
    public interface IAvoider
    {
        bool ShouldAvoid { get; }
        bool ShouldKite { get; }
        TimeSpan TimeSinceLastAvoid { get; }
        TimeSpan TimeSinceLastKite { get; }
        bool IsKiteOnCooldown { get; }
        Vector3 SafeSpot { get; }

        bool TryGetSafeSpot(out Vector3 position,
            float minDistance = 10f,
            float maxDistance = 100f,
            Vector3 origin = default(Vector3),
            Func<AvoidanceNode, bool> condition = null);
    }

    /// <summary>
    ///     Avoider is responsible for detecting if player should avoid
    /// </summary>
    public class DefaultAvoider : IAvoider
    {       
        private Vector3 _safeSpot;

        public static HashSet<TrinityObjectType> GizmoProximityTypes = new HashSet<TrinityObjectType>
        {
            TrinityObjectType.Barricade,
            TrinityObjectType.Door,
            TrinityObjectType.Barricade,
            TrinityObjectType.Shrine
        };

        public static AvoidanceSetting Settings => TrinityPlugin.Settings.Avoidance;
        public static DateTime KiteStutterCooldownEndTime = DateTime.MinValue;
        public static DateTime LastAvoidTime = DateTime.MinValue;
        public static DateTime LastKiteTime = DateTime.MinValue;

        public TimeSpan TimeSinceLastAvoid => DateTime.UtcNow.Subtract(LastAvoidTime);
        public TimeSpan TimeSinceLastKite => DateTime.UtcNow.Subtract(LastKiteTime);
        public bool IsKiteOnCooldown => Settings.KiteDistance > 0 && Core.Avoidance.Avoider.TimeSinceLastKite.TotalMilliseconds < Settings.KiteStutterDuration;

        private readonly PerFrameCachedValue<bool> _shouldKite = new PerFrameCachedValue<bool>(GetShouldKite);
        private readonly PerFrameCachedValue<bool> _shouldAvoid = new PerFrameCachedValue<bool>(GetShouldAvoid);

        public bool ShouldAvoid => _shouldAvoid.Value;

        public bool ShouldKite => _shouldKite.Value;

        private static bool ShouldAvoidNormal
        {
            get
            {
                if (Settings.Avoidances == null)
                    return false;

                if (!Settings.Avoidances.Any(a => a.IsEnabled))
                    return false;

                if (Settings.OnlyAvoidWhileInGrifts && (!RiftProgression.IsInRift || ZetaDia.CurrentRift.Type != RiftType.Greater))
                    return false;

                if (Core.Avoidance.ActiveAvoidanceSnoIds == null || !Core.Avoidance.ActiveAvoidanceSnoIds.Any())
                    return false;

                if (Core.Avoidance.Grid.IsStandingInFlags(AvoidanceFlags.NoAvoid) && PlayerMover.IsBlocked)
                    return false;

                if (PlayerMover.IsBlocked && PlayerMover.BlockedTimeMs > 8000)
                {
                    Logger.Log(LogCategory.Avoidance, "Not Avoiding because blocked");
                    return false;
                }

                if (CombatBase.CurrentTarget != null && CombatBase.CurrentTarget.Distance < 10f && GizmoProximityTypes.Contains(CombatBase.CurrentTarget.Type))
                {
                    Logger.Log(LogCategory.Avoidance, "Not Kiting because gizmo nearby");
                    return false;
                }

                if (Core.Avoidance.HighestNodeWeight >= 2 &&
                    Core.Avoidance.NearbyStats.HighestWeight >= Settings.MinimumHighestNodeWeightTrigger &&
                    Core.Avoidance.NearbyStats.WeightPctTotal >= Settings.MinimumNearbyWeightPctTotalTrigger &&
                    Core.Avoidance.NearbyStats.WeightPctAvg >= Settings.AvoiderNearbyPctAvgTrigger)
                {
                    Logger.Log(LogCategory.Avoidance, "Avoidance Local PctAvg: {0:0.00} / {1:0.00} PctTotal={2:0.00} / {3:0.00} Highest={4} / {5} ({6} Nodes, AbsHighest={7})",
                        Core.Avoidance.NearbyStats.WeightPctAvg,
                        Settings.AvoiderNearbyPctAvgTrigger,
                        Core.Avoidance.NearbyStats.WeightPctTotal,
                        Settings.MinimumNearbyWeightPctTotalTrigger,
                        Core.Avoidance.NearbyStats.HighestWeight,
                        Settings.MinimumHighestNodeWeightTrigger,
                        Core.Avoidance.NearbyStats.NodesTotal,
                        Core.Avoidance.HighestNodeWeight);

                    LastAvoidTime = DateTime.UtcNow;
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private static bool ShouldAvoidCritical
        {
            get
            {
                if (TargetZDif < 8 && Settings.Avoidances != null && Settings.Avoidances.Any(a => a.IsEnabled))
                {
                    var standingInCritical = Core.Grids.Avoidance.IsStandingInFlags(AvoidanceFlags.CriticalAvoidance);
                    if (standingInCritical)
                    {
                        Logger.Log(LogCategory.Avoidance, "IsStandingInFlags... CriticalAvoidance");
                        LastAvoidTime = DateTime.UtcNow;
                        return true;
                    }

                    if (CombatBase.CurrentTarget != null && Core.Grids.Avoidance.IsIntersectedByFlags(ZetaDia.Me.Position, CombatBase.CurrentTarget.Position, AvoidanceFlags.CriticalAvoidance))
                    {
                        TargetUtil.ClearCurrentTarget("Current Target Intersects Critical Avoidance.");
                    }

                    if (Core.Grids.Avoidance.IsPathingOverFlags(AvoidanceFlags.CriticalAvoidance))
                    {
                        Logger.Log(LogCategory.Avoidance, "IsPathingOverFlags... CriticalAvoidance");
                        Navigator.Clear();
                        LastAvoidTime = DateTime.UtcNow;
                        return true;
                    }
                }
                return false;
            }
        }

        private static float TargetZDif
        {
            get { return CombatBase.CurrentTarget == null ? 0 : Math.Abs(CombatBase.CurrentTarget.Position.Z - ZetaDia.Me.Position.Z); }
        }

        public Vector3 SafeSpot
        {
            get
            {
                if (!Core.Avoidance.Grid.IsLocationInFlags(_safeSpot) && _safeSpot.Distance(ZetaDia.Me.Position) < 5f)
                    return _safeSpot;

                TryGetSafeSpot(out _safeSpot);
                return _safeSpot;
            }
        }

        public bool TryGetSafeSpot(out Vector3 safeSpot, float minDistance = 0f, float maxDistance = 100f, Vector3 origin = default(Vector3), Func<AvoidanceNode, bool> condition = null)
        {
            IEnumerable<AvoidanceNode> nodes;

            if (origin == default(Vector3))
            {
                nodes = Core.Avoidance.SafeNodesByDistance.Where(p => p.Distance >= minDistance && p.Distance <= maxDistance);
            }
            else
            {
                nodes = Core.Avoidance.SafeNodesByDistance.Where(p =>
                {
                    var distance = p.NavigableCenter.Distance(origin);
                    return distance >= minDistance && distance <= maxDistance;
                });
            }

            var safeSpotNode = condition == null ? nodes.FirstOrDefault() : nodes.FirstOrDefault(condition);
            if (safeSpotNode != null)
            {
                safeSpot = safeSpotNode.NavigableCenter;
                return true;
            }
            safeSpot = Vector3.Zero;
            return false;
        }

        public static bool GetShouldAvoid()
        {
            if (CacheData.BuffsCache.Instance.HasInvulnerableShrine)
                return false;

            if (CombatBase.IsDoingGoblinKamakazi)
                return false;

            if (TrinityPlugin.Player.IsInTown)
                return false;

            if (!TrinityPlugin.Settings.Combat.Misc.AvoidAoEOutOfCombat && !CombatBase.IsInCombat)
                return false;

            if (CombatBase.IsDoingGoblinKamakazi)
                return false;

            if (ShouldAvoidCritical)
                return true;

            if (CombatBase.IsWaitingForPower())
            {
                Logger.Log(LogCategory.Avoidance, "Not Avoiding because routine needs to cast a power");
                return false;
            }

            if (ShouldAvoidNormal)
                return true;

            return false;
        }

        private static bool GetShouldKite()
        {
            if (CacheData.BuffsCache.Instance.HasInvulnerableShrine)
                return false;

            if (TrinityPlugin.Player.IsInTown)
                return false;

            if (!CombatBase.IsInCombat)
                return false;
            
            if (CombatBase.IsDoingGoblinKamakazi)
            {
                Logger.Log(LogCategory.Avoidance, "Not Kiting because goblin kamakazi");
                return false;
            }

            if (CombatBase.IsWaitingForPower())
            {
                Logger.Log(LogCategory.Avoidance, "Not Kiting because routine needs to cast a power");
                return false;
            }

            if (CombatBase.CurrentTarget.Distance < 10f && GizmoProximityTypes.Contains(CombatBase.CurrentTarget.Type))
            {
                Logger.Log(LogCategory.Avoidance, "Not Kiting because gizmo nearby");
                return false;
            }

            if (DateTime.UtcNow < KiteStutterCooldownEndTime)
            {
                Logger.Log(LogCategory.Avoidance, "Kite On Cooldown");
                return false;
            }

            if (PlayerMover.IsBlocked && PlayerMover.BlockedTimeMs > 8000 && !Core.Avoidance.InCriticalAvoidance(ZetaDia.Me.Position))
            {
                Logger.Log(LogCategory.Avoidance, "Not kiting because blocked");
                return false;
            }

            var playerHealthPct = TrinityPlugin.Player.CurrentHealthPct*100;
            if (playerHealthPct > 50)
            {
                // Restrict kiting when the current target is on the edge of line of sight
                // This should help with flip-flopping around corners and doorways.

                var from = TrinityPlugin.Player.Position;
                var to = CombatBase.CurrentTarget.Position;

                var losAngleA = MathEx.WrapAngle((float)(MathUtil.FindDirectionRadian(from, to) - Math.Round(Math.PI, 5) / 2));
                var losPositionA = MathEx.GetPointAt(from, CombatBase.CurrentTarget.Distance, losAngleA);
                if (!Core.Avoidance.Grid.CanRayCast(from, losPositionA))
                    return false;

                var losAngleB = MathEx.WrapAngle((float)(MathUtil.FindDirectionRadian(from, to) + Math.Round(Math.PI, 5) / 2));
                var losPositionB = MathEx.GetPointAt(from, CombatBase.CurrentTarget.Distance, losAngleB);
                if (!Core.Avoidance.Grid.CanRayCast(from, losPositionB))
                    return false;
            }

            var isAtKiteHealth = playerHealthPct <= Settings.KiteHealth;
            if (isAtKiteHealth && TargetZDif < 4 && Settings.KiteMode != KiteMode.Never)
            {
                var canSeeTarget = CombatBase.CurrentTarget == null || Core.Avoidance.Grid.CanRayCast(ZetaDia.Me.Position, CombatBase.CurrentTarget.Position);
                if (canSeeTarget)
                {
                    if (Core.Grids.Avoidance.IsStandingInFlags(AvoidanceFlags.KiteFrom))
                    {
                        if (DateTime.UtcNow.Subtract(LastKiteTime).TotalMilliseconds > Settings.KiteStutterDelay)
                        {
                            Logger.Log(LogCategory.Avoidance, "Kite Shutter Triggered");
                            LastKiteTime = DateTime.UtcNow;
                            KiteStutterCooldownEndTime = DateTime.UtcNow.AddMilliseconds(Settings.KiteStutterDuration);
                            return true;
                        }

                        Logger.Log(LogCategory.Avoidance, "IsStandingInFlags... KiteFromNode");
                        LastAvoidTime = DateTime.UtcNow;
                        return true;
                    }
                } 
            }
            return false;
        }
    }
}