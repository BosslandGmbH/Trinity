using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Combat.Abilities;
using Trinity.DbProvider;
using Trinity.Framework.Avoidance.Structures;
using TrinityCoroutines;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance
{
    public interface IAvoider
    {
        bool IsAvoiding { get; }
        bool ShouldAvoid { get; }
        Task<bool> Avoid();
        bool TryGetSafeSpot(out Vector3 position, Func<AvoidanceNode, bool> condition = null);
    }

    /// <summary>
    /// Avoider is responsible for detecting if player should avoid and for moving to a safe position.
    /// </summary>
    public class DefaultAvoider : IAvoider
    {
        public AvoidanceMode Mode;

        public double DestinationChangeLimitMs = 25;

        public double AvoidCooldownMs = 25;

        public AvoidanceNode CurrentDestination;

        public bool IsAvoiding { get; set; }

        bool IAvoider.ShouldAvoid => ShouldAvoid();

        public async Task<bool> Avoid()
        {
            if (ShouldAvoid())
            {
                Vector3 safespot;
                if (!TryGetSafeSpot(out safespot))
                {
                    IsAvoiding = false;
                    return false;
                }

                await AddToSafeSpotToCache(safespot);
                IsAvoiding = true;
                return false;
            }

            IsAvoiding = false;
            return false;
        }

        public bool ShouldAvoid()
        {
            if (!TrinityPlugin.Settings.Avoidance.Avoidances.Any(a => a.IsEnabled))
                return false;

            if (TrinityPlugin.Player.IsInTown)
                return false;

            if (TrinityPlugin.Settings.Combat.Misc.AvoidAoEOutOfCombat && !CombatBase.IsInCombat)
                return false;

            // todo, allow a way of disabling avoidance for monk with serentiy, barb with ironskin, ghost walking WD

            // todo animations
          //  if (c_diaObject is DiaUnit && Settings.Combat.Misc.AvoidAOE &&
        //    DataDictionary.AvoidanceAnimations.Contains(new DoubleInt(CurrentCacheObject.ActorSNO, (int)c_diaObject.CommonData.CurrentAnimation)))


                if (Core.Grids.Avoidance.IsStandingInFlags(AvoidanceFlags.CriticalAvoidance))
            {
                Logger.Log("IsStandingInFlags... CriticalAvoidance");
                return true;
            }

            if (Core.Grids.Avoidance.IsPathingOverFlags(AvoidanceFlags.CriticalAvoidance))
            {
                Logger.Log("IsPathingOverFlags... CriticalAvoidance");
                return true;
            }

            if (Core.Avoidance.NearbyNodes.Any(n => n.AvoidanceFlags.HasFlag(AvoidanceFlags.Gizmo)) && PlayerMover.IsBlocked)
            {
                return false;
            }

            if (Core.Avoidance.HighestNodeWeight >= 2 &&
                Core.Avoidance.NearbyStats.HighestWeight >= TrinityPlugin.Settings.Avoidance.MinimumHighestNodeWeightTrigger &&
                Core.Avoidance.NearbyStats.WeightPctTotal >= TrinityPlugin.Settings.Avoidance.MinimumNearbyWeightPctTotalTrigger &&
                Core.Avoidance.NearbyStats.WeightPctAvg >= TrinityPlugin.Settings.Avoidance.AvoiderNearbyPctAvgTrigger)
            {
                Logger.Log("Avoidance Local PctAvg: {0:0.00} / {1:0.00} PctTotal={2:0.00} / {3:0.00} Highest={4} / {5} ({6} Nodes, AbsHighest={7})",
                    Core.Avoidance.NearbyStats.WeightPctAvg,
                    TrinityPlugin.Settings.Avoidance.AvoiderNearbyPctAvgTrigger,
                    Core.Avoidance.NearbyStats.WeightPctTotal,
                    TrinityPlugin.Settings.Avoidance.MinimumNearbyWeightPctTotalTrigger,
                    Core.Avoidance.NearbyStats.HighestWeight,
                    TrinityPlugin.Settings.Avoidance.MinimumHighestNodeWeightTrigger,
                    Core.Avoidance.NearbyStats.NodesTotal,
                    Core.Avoidance.HighestNodeWeight);

                return true;
            }

            return false;
        }

        public async Task<bool> AddToSafeSpotToCache(Vector3 safespot)
        {
            var highestPriorityObject = TrinityPlugin.ObjectCache.Where(o => o.Weight > TrinityPlugin.Weighting.MaxWeight * 0.8 && (CombatBase.IsDoingGoblinKamakazi && o.IsTreasureGoblin || o.ActorType == ActorType.Gizmo)).OrderByDescending(o => o.Weight).FirstOrDefault();
            var weight = highestPriorityObject?.Weight ?? TrinityPlugin.Weighting.MaxWeight;

            var t = TrinityPlugin.ObjectCache.OrderByDescending(o => o.Weight).FirstOrDefault();
            if (t != null && CombatBase.IsDoingGoblinKamakazi && t.IsTreasureGoblin || t.Type == TrinityObjectType.Door && t.Distance < 10f)
            {
                return false;
            }

            TrinityPlugin.ObjectCache.RemoveAll(o => o.IsSafeSpot);
            TrinityPlugin.ObjectCache.Add(new TrinityCacheObject()
            {
                InternalName = "Avoidance Safe Spot",
                Position = safespot,
                Weight = weight,
                Type = TrinityObjectType.Avoidance,
                IsSafeSpot = true,
            });

            return false;
        }

        public bool TryGetSafeSpot(out Vector3 safeSpot, Func<AvoidanceNode, bool> condition = null)
        {
            var nodes = Core.Avoidance.SafeNodesByDistance;
            var safeSpotNode = condition == null ? nodes.FirstOrDefault() : nodes.FirstOrDefault(condition);
            if (safeSpotNode != null)
            {
                safeSpot = safeSpotNode.NavigableCenter;
                return true;
            }
            safeSpot = Vector3.Zero;
            return false;
        }

        private async Task<bool> MoveToDestination()
        {
            if (CurrentDestination.Distance >= 4f)
            {
                if (CurrentDestination.Distance < 10f)
                {
                    Logger.LogVerbose("Moving (Basic) to Avoidance Position {0} Distance={1}", CurrentDestination.NavigableCenter, CurrentDestination.Distance);
                    Navigator.PlayerMover.MoveTowards(CurrentDestination.NavigableCenter);
                    return true;
                }

                Logger.LogVerbose
                    ("Moving (PathFinder) to Avoidance Position {0} Distance={1}", CurrentDestination.NavigableCenter, CurrentDestination.Distance);
                var result = await CommonCoroutines.MoveTo(CurrentDestination.NavigableCenter, "AvoidPosition");
                switch (result)
                {
                    case MoveResult.PathGenerationFailed:
                    case MoveResult.UnstuckAttempt:
                        Logger.LogVerbose("PathFinding Failed", CurrentDestination.NavigableCenter, CurrentDestination.Distance);
                        CurrentDestination = null;
                        return true;
                }
            }
            return false;
        }

        private bool IsCurrentPathAvoidanceDestination()
        {
            if (CurrentDestination == null || PlayerMover.NavigationProvider == null || PlayerMover.NavigationProvider.CurrentPath == null)
                return false;

            return CurrentDestination.NavigableCenter == PlayerMover.NavigationProvider.CurrentPathDest;
        }

    }

    public enum AvoidanceMode
    {
        None = 0,
        StayClose,
        Meander
    }
}


