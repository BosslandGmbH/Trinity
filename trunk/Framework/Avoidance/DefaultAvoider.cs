using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework.Avoidance.Settings;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects;
using Trinity.Settings;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;


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
            float minDistance = 15f,
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

        //public static AvoidanceSetting Settings => Core.Avoidance.Settings;

        public static DateTime KiteStutterCooldownEndTime = DateTime.MinValue;
        public static DateTime LastAvoidTime = DateTime.MinValue;
        public static DateTime LastKiteTime = DateTime.MinValue;

        public TimeSpan TimeSinceLastAvoid => DateTime.UtcNow.Subtract(LastAvoidTime);
        public TimeSpan TimeSinceLastKite => DateTime.UtcNow.Subtract(LastKiteTime);
        public bool IsKiteOnCooldown => Combat.Routines.Current.KiteDistance > 0 && Core.Avoidance.Avoider.TimeSinceLastKite.TotalMilliseconds < Combat.Routines.Current.KiteStutterDuration;

        private readonly PerFrameCachedValue<bool> _shouldKite = new PerFrameCachedValue<bool>(GetShouldKite);
        private readonly PerFrameCachedValue<bool> _shouldAvoid = new PerFrameCachedValue<bool>(GetShouldAvoid);

        public bool ShouldAvoid => _shouldAvoid.Value;

        public bool ShouldKite => _shouldKite.Value;

        public static AvoidanceSettings Settings => Core.Avoidance.Settings;

        private static bool ShouldAvoidNormal
        {
            get
            {
                if (Settings.Entries == null)
                    return false;

                if (!Settings.Entries.Any(a => a.IsEnabled))
                    return false;

                //if (Settings.OnlyAvoidWhileInGrifts && (!Core.Rift.IsInRift || ZetaDia.Storage.CurrentRiftType != RiftType.Greater))
                //    return false;

                if (Core.Avoidance.GridEnricher.ActiveAvoidanceSnoIds == null || !Core.Avoidance.GridEnricher.ActiveAvoidanceSnoIds.Any())
                    return false;

                if (Core.Avoidance.Grid.IsStandingInFlags(AvoidanceFlags.NoAvoid) && PlayerMover.IsBlocked)
                    return false;

                if (PlayerMover.IsBlocked && Core.BlockedCheck.BlockedTime.TotalMilliseconds > 8000)
                {
                    Core.Logger.Log(LogCategory.Avoidance, "Not Avoiding because blocked");
                    return false;
                }

                if (Combat.Targeting.CurrentTarget != null && Combat.Targeting.CurrentTarget.Distance < 10f && GizmoProximityTypes.Contains(Combat.Targeting.CurrentTarget.Type))
                {
                    Core.Logger.Log(LogCategory.Avoidance, "Not Avoiding because gizmo nearby");
                    return false;
                }

                if (Core.Avoidance.GridEnricher.HighestNodeWeight >= 2 &&
                    Core.Avoidance.NearbyStats.HighestWeight >= Settings.MinimumHighestNodeWeightTrigger &&
                    Core.Avoidance.NearbyStats.WeightPctTotal >= Settings.MinimumNearbyWeightPctTotalTrigger &&
                    Core.Avoidance.NearbyStats.WeightPctAvg >= Settings.AvoiderNearbyPctAvgTrigger)
                {
                    Core.Logger.Log(LogCategory.Avoidance, "Avoidance Local PctAvg: {0:0.00} / {1:0.00} PctTotal={2:0.00} / {3:0.00} Highest={4} / {5} ({6} Nodes, AbsHighest={7})",
                        Core.Avoidance.NearbyStats.WeightPctAvg,
                        Settings.AvoiderNearbyPctAvgTrigger,
                        Core.Avoidance.NearbyStats.WeightPctTotal,
                        Settings.MinimumNearbyWeightPctTotalTrigger,
                        Core.Avoidance.NearbyStats.HighestWeight,
                        Settings.MinimumHighestNodeWeightTrigger,
                        Core.Avoidance.NearbyStats.NodesTotal,
                        Core.Avoidance.GridEnricher.HighestNodeWeight);

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
                if (TargetZDif < 8 && Settings.Entries != null && Settings.Entries.Any(a => a.IsEnabled))
                {
                    var standingInCritical = Core.Grids.Avoidance.IsStandingInFlags(AvoidanceFlags.CriticalAvoidance);
                    if (standingInCritical)
                    {
                        Core.Logger.Log(LogCategory.Avoidance, "IsStandingInFlags... CriticalAvoidance");
                        LastAvoidTime = DateTime.UtcNow;
                        return true;
                    }

                    if (Combat.Targeting.CurrentTarget != null && Core.Grids.Avoidance.IsIntersectedByFlags(ZetaDia.Me.Position, Combat.Targeting.CurrentTarget.Position, AvoidanceFlags.CriticalAvoidance))
                    {
                        TargetUtil.ClearCurrentTarget("Current Target Intersects Critical Avoidance.");
                    }

                    if (Core.Grids.Avoidance.IsPathingOverFlags(AvoidanceFlags.CriticalAvoidance))
                    {
                        Core.Logger.Log(LogCategory.Avoidance, "IsPathingOverFlags... CriticalAvoidance");
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
            get { return Combat.Targeting.CurrentTarget == null ? 0 : Math.Abs(Combat.Targeting.CurrentTarget.Position.Z - ZetaDia.Me.Position.Z); }
        }

        public Vector3 SafeSpot
        {
            get
            {
                if (!Core.Avoidance.Grid.IsLocationInFlags(_safeSpot, AvoidanceFlags.Avoidance) && _safeSpot.Distance(ZetaDia.Me.Position) < 12f)
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
                nodes = Core.Avoidance.GridEnricher.SafeNodesByDistance.Where(p => p.Distance >= minDistance && p.Distance <= maxDistance);
            }
            else
            {
                nodes = Core.Avoidance.GridEnricher.SafeNodesByDistance.Where(p =>
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
            if (Core.Buffs.HasInvulnerableShrine)
                return false;

            if (Combat.Targeting.CurrentTarget != null && Combat.Targeting.CurrentTarget.IsTreasureGoblin)
                return false;

            if (Core.Player.IsInTown)
                return false;

            //if (!Core.Settings.Combat.Misc.AvoidAoEOutOfCombat && !Combat.IsInCombat)
            //    return false;

            if (ShouldAvoidCritical)
                return true;

            if (Combat.Routines.Current?.ShouldIgnoreAvoidance() ?? false)
            {
                Core.Logger.Log(LogCategory.Avoidance, "Not Avoiding because routine has said no");
                return false;
            }

            if (ShouldAvoidNormal)
                return true;

            return false;
        }

        private static bool GetShouldKite()
        {
            if (Core.Buffs.HasInvulnerableShrine)
                return false;

            if (Core.Player.IsInTown)
                return false;

            if (!Combat.IsInCombat)
                return false;

            if (Combat.Targeting.CurrentTarget != null && Combat.Targeting.CurrentTarget.IsTreasureGoblin)
                return false;

            if (Combat.Routines.Current.ShouldIgnoreKiting())
            {
                Core.Logger.Log(LogCategory.Avoidance, "Not Kiting because routine has said no");
                return false;
            }

            if (Combat.Targeting.CurrentTarget.Distance < 10f && GizmoProximityTypes.Contains(Combat.Targeting.CurrentTarget.Type))
            {
                Core.Logger.Log(LogCategory.Avoidance, "Not Kiting because gizmo nearby");
                return false;
            }

            if (DateTime.UtcNow < KiteStutterCooldownEndTime)
            {
                Core.Logger.Log(LogCategory.Avoidance, "Kite On Cooldown");
                return false;
            }

            var currentTarget = Combat.Targeting.CurrentTarget;
            var isCloseLargeMonster = currentTarget.Distance < 12f && (currentTarget.MonsterSize != MonsterSize.Big || currentTarget.MonsterSize != MonsterSize.Boss);

            if (PlayerMover.IsBlocked && !isCloseLargeMonster && Core.BlockedCheck.BlockedTime.TotalMilliseconds > 8000 && !Core.Avoidance.InCriticalAvoidance(ZetaDia.Me.Position))
            {
                Core.Logger.Log(LogCategory.Avoidance, "Not kiting because blocked");
                return false;
            }

            var playerHealthPct = Core.Player.CurrentHealthPct * 100;
            if (playerHealthPct > 50)
            {
                // Restrict kiting when the current target is on the edge of line of sight
                // This should help with flip-flopping around corners and doorways.

                var from = Core.Player.Position;
                var to = Combat.Targeting.CurrentTarget.Position;

                var losAngleA = MathEx.WrapAngle((float)(MathUtil.FindDirectionRadian(from, to) - Math.Round(Math.PI, 5) / 2));
                var losPositionA = MathEx.GetPointAt(from, Combat.Targeting.CurrentTarget.Distance, losAngleA);
                if (!Core.Avoidance.Grid.CanRayCast(from, losPositionA))
                    return false;

                var losAngleB = MathEx.WrapAngle((float)(MathUtil.FindDirectionRadian(from, to) + Math.Round(Math.PI, 5) / 2));
                var losPositionB = MathEx.GetPointAt(from, Combat.Targeting.CurrentTarget.Distance, losAngleB);
                if (!Core.Avoidance.Grid.CanRayCast(from, losPositionB))
                    return false;
            }

            var isAtKiteHealth = playerHealthPct <= Combat.Routines.Current.KiteHealthPct;
            if (isAtKiteHealth && TargetZDif < 4 && Combat.Routines.Current.KiteMode != KiteMode.Never)
            {
                var canSeeTarget = Combat.Targeting.CurrentTarget == null || Core.Avoidance.Grid.CanRayCast(ZetaDia.Me.Position, Combat.Targeting.CurrentTarget.Position);
                if (canSeeTarget)
                {
                    if (Core.Grids.Avoidance.IsStandingInFlags(AvoidanceFlags.KiteFrom))
                    {
                        if (DateTime.UtcNow.Subtract(LastKiteTime).TotalMilliseconds > Combat.Routines.Current.KiteStutterDelay)
                        {
                            Core.Logger.Log(LogCategory.Avoidance, "Kite Shutter Triggered");
                            LastKiteTime = DateTime.UtcNow;
                            KiteStutterCooldownEndTime = DateTime.UtcNow.AddMilliseconds(Combat.Routines.Current.KiteStutterDuration);
                            return true;
                        }

                        Core.Logger.Log(LogCategory.Avoidance, "IsStandingInFlags... KiteFromNode");
                        LastAvoidTime = DateTime.UtcNow;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}