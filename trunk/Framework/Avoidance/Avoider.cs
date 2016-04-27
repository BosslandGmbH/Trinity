using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.DbProvider;
using TrinityCoroutines;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance
{

    /// <summary>
    /// Avoider is responsible for detecting if player should avoid and for moving to a safe position.
    /// </summary>
    public class Avoider
    {
        public static AvoidanceMode Mode;

        public static double DestinationChangeLimitMs = 25;

        public static double AvoidCooldownMs = 25;

        public static AvoidanceNode CurrentDestination;

        private static DateTime _lastSafeSpotSelected = DateTime.MinValue;

        private static DateTime _lastArrivedAtSafeSpot = DateTime.MinValue;

        private static Dictionary<Vector3, float> BlacklistedAreas = new Dictionary<Vector3, float>();

        private static Dictionary<Vector3, int> _pathFinderBadLocationCounts = new Dictionary<Vector3, int>();

        /// <summary>
        /// Determine if the avoider should take control and move to satefy
        /// </summary>
        public static bool ShouldAvoid()
        {
            if (!Trinity.Settings.Advanced.UseExperimentalAvoidance)
                return false;

            if (Trinity.Player.IsInTown)
                return false;

            //if (PlayerMover.IsBlocked && PlayerMover.BlockedTimeMs > 1000 && IsCurrentPathAvoidanceDestination() && IsPathingOverFlags(AvoidanceFlags.Monster))
            //{
            //    Logger.Log("Can't get to avoidance spot");
            //    return false;
            //}

            if (IsStandingInFlags(AvoidanceFlags.CriticalAvoidance))
            {                
                return true;
            }

            // Once avoider has released control, trinity or the profile or anything else may have decided to move over avoidance.
            if (IsPathingOverFlags(AvoidanceFlags.CriticalAvoidance))
            {
                return true;
            }

            if (Core.Avoidance.NearbyNodes.Any(n => n.AvoidanceFlags.HasFlag(AvoidanceFlags.Gizmo)) && PlayerMover.IsBlocked)
            {
                return false;
            }

            if (Core.Avoidance.HighestNodeWeight >= 2 &&
                Core.Avoidance.NearbyStats.HighestWeight >= Trinity.Settings.Avoidance.MinimumHighestNodeWeightTrigger &&
                Core.Avoidance.NearbyStats.WeightPctTotal >= Trinity.Settings.Avoidance.MinimumNearbyWeightPctTotalTrigger &&
                Core.Avoidance.NearbyStats.WeightPctAvg >= Trinity.Settings.Avoidance.AvoiderNearbyPctAvgTrigger)
            {
                Logger.Log("Avoidance Local PctAvg: {0:0.00} / {1:0.00} PctTotal={2:0.00} / {3:0.00} Highest={4} / {5} ({6} Nodes, AbsHighest={7})",
                    Core.Avoidance.NearbyStats.WeightPctAvg,
                    Trinity.Settings.Avoidance.AvoiderNearbyPctAvgTrigger,
                    Core.Avoidance.NearbyStats.WeightPctTotal,
                    Trinity.Settings.Avoidance.MinimumNearbyWeightPctTotalTrigger,
                    Core.Avoidance.NearbyStats.HighestWeight,
                    Trinity.Settings.Avoidance.MinimumHighestNodeWeightTrigger,
                    Core.Avoidance.NearbyStats.NodesTotal,
                    Core.Avoidance.HighestNodeWeight);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Responsible for analyzing the current nodes nearby and moving to a safe position
        /// </summary>
        public static async Task<bool> MoveToSafeSpot()
        {
            Logger.Log("Avoider Called");

            var pos = Trinity.Player.Position;

            if (!Core.Avoidance.CurrentNodes.Any())
                return true;

            var msSinceLastSafe = DateTime.UtcNow.Subtract(_lastArrivedAtSafeSpot).TotalMilliseconds;
            if (msSinceLastSafe < AvoidCooldownMs)
                return true;

            var msSinceLastSelected = DateTime.UtcNow.Subtract(_lastSafeSpotSelected).TotalMilliseconds;
            if (msSinceLastSelected > DestinationChangeLimitMs && CurrentDestination != null)
            {
                var targetNodes = Core.Avoidance.CurrentNodes.Where(n => n != null && n.Distance > 30f && !n.AvoidanceFlags.HasFlag(AvoidanceFlags.Monster));

                AvoidanceNode targetNode;

                if (Mode == AvoidanceMode.StayClose)
                {
                    targetNode = targetNodes.OrderBy(n => n.Weight).FirstOrDefault(n => !Core.Avoidance.Grid.IsIntersectedByFlags(pos, n.NavigableCenter, AvoidanceFlags.Monster, AvoidanceFlags.Avoidance));
                }
                else
                    targetNode = targetNodes.OrderByDescending(n => n.Weight).FirstOrDefault(n => !Core.Avoidance.Grid.IsIntersectedByFlags(pos, n.NavigableCenter, AvoidanceFlags.Monster, AvoidanceFlags.Avoidance));


                var foundBetterDestination = targetNode != null && CurrentDestination != null && CurrentDestination.NavigableCenter.Distance(targetNode.NavigableCenter) > 10f;

                if (targetNode != null && (CurrentDestination == null || CurrentDestination.Distance < 4f || foundBetterDestination))
                {
                    _lastSafeSpotSelected = DateTime.UtcNow;
                    CurrentDestination = targetNode;
                }    
            }

            if (CurrentDestination == null)
                CurrentDestination = Core.Avoidance.KiteCentroidNodes.FirstOrDefault();

            if (CurrentDestination == null)
                CurrentDestination = Core.Avoidance.Grid.GetNodesInRadius(pos, n => !n.AvoidanceFlags.HasFlag(AvoidanceFlags.Avoidance), 100f).FirstOrDefault();

            if (CurrentDestination == null)
                return true;

            if (await MoveToDestination())
                return false;

            _lastArrivedAtSafeSpot = DateTime.UtcNow;
            Logger.LogVerbose("Arrived at Avoidance Position");
            return true;
        }

        /// <summary>
        /// This movement must be called every tick for movement to continue.
        /// Which means its not locked in to moving to the destination. 
        /// Which is an ideal approach for responsive combat kiting.
        /// </summary>
        private static async Task<bool> MoveToDestination()
        {
            if (CurrentDestination.Distance >= 4f)
            {
                if (CurrentDestination.Distance < 10f)
                {
                    Logger.LogVerbose("Moving (Basic) to Avoidance Position {0} Distance={1}", CurrentDestination.NavigableCenter, CurrentDestination.Distance);
                    Navigator.PlayerMover.MoveTowards(CurrentDestination.NavigableCenter);
                    return true;
                }

                Logger.LogVerbose("Moving (PathFinder) to Avoidance Position {0} Distance={1}", CurrentDestination.NavigableCenter, CurrentDestination.Distance);
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

        ///// <summary>
        ///// Responsible for analyzing the current nodes nearby and moving to a safe position
        ///// </summary>
        //public static async Task<bool> MoveToSafeSpot()
        //{
        //    //var targetNode = Core.Avoidance.KiteNodes.OrderBy(n => n.Weight).ThenBy(n => n.Distance).FirstOrDefault();
        //    //if (targetNode == null || targetNode.Distance > 80f)
        //    //{
        //    //    targetNode = Core.Avoidance.SafeNodes.OrderBy(n => n.Weight).ThenBy(n => n.Distance).FirstOrDefault();
        //    //    if (targetNode == null || targetNode.Distance > 80f)
        //    //    {
        //    //        Logger.Log("No Kite or Safe Nodes Found");
        //    //        return true;
        //    //    }
        //    //}

        //    //var targetNode = Core.Avoidance.Grid.GetConnectedNodes(Trinity.Player.Position,
        //    //    node => Core.Avoidance.Grid.CanRayWalk(Trinity.Player.Position, node.NavigableCenter) && node.Weight <= 0 && !node.AvoidanceFlags.HasFlag(AvoidanceFlags.Monster), 60f, 12f)
        //    //    .OrderBy(n => n.NearbyWeightPct).FirstOrDefault();

        //    Logger.Log("Avoider Called");

        //    var pos = Trinity.Player.Position;

        //    if (!Core.Avoidance.CurrentNodes.Any())
        //        return true;

        //    var isInCritical = IsStandingInCriticalAvoidance();

        //    var msSinceLastSafe = DateTime.UtcNow.Subtract(LastArrivedAtSafeSpot).TotalMilliseconds;
        //    if (isInCritical && msSinceLastSafe < AvoidCooldownMs)
        //        return true;

        //    var distance = CurrentDestination != null ? CurrentDestination.Distance : 0;

        //    var stayOnPathOutOfCritical = isInCritical && CurrentDestination != null && distance > 10f && !CurrentDestination.AvoidanceFlags.HasFlag(AvoidanceFlags.CriticalAvoidance);

        //    var msSinceLastSelected = DateTime.UtcNow.Subtract(LastSafeSpotSelected).TotalMilliseconds;

        //    if (!stayOnPathOutOfCritical && msSinceLastSelected > DestinationChangeLimitMs || PlayerMover.IsBlocked)
        //    {   
        //        var targetNodes = Core.Avoidance.CurrentNodes.Where(n => n != null && n.Distance > 30f && !n.AvoidanceFlags.HasFlag(AvoidanceFlags.Monster) && !n.AvoidanceFlags.HasFlag(AvoidanceFlags.CriticalAvoidance));

        //        AvoidanceNode targetNode;

        //        if (Mode == AvoidanceMode.StayClose)
        //        {                    
        //            targetNode = targetNodes.OrderBy(n => n.Weight).FirstOrDefault(n => !Core.Avoidance.Grid.IsIntersectedByFlags(pos, n.NavigableCenter, AvoidanceFlags.Monster, AvoidanceFlags.Avoidance));
        //        }
        //        else
        //            targetNode = targetNodes.OrderByDescending(n => n.Weight).FirstOrDefault(n => !Core.Avoidance.Grid.IsIntersectedByFlags(pos, n.NavigableCenter, AvoidanceFlags.Monster, AvoidanceFlags.Avoidance, AvoidanceFlags.CriticalAvoidance));

        //        var foundBetterDestination = targetNode != null && CurrentDestination != null && CurrentDestination.NavigableCenter.Distance(targetNode.NavigableCenter) > 10f;// && 

        //        if ((CurrentDestination == null || distance < 4f || foundBetterDestination) && targetNode != null)
        //        {
        //            Logger.Log("Changed distination to {0} Distance={1}", targetNode, targetNode.Distance);
        //            LastSafeSpotSelected = DateTime.UtcNow;
        //            CurrentDestination = targetNode;
        //        }

        //        if (CurrentDestination == null)
        //        {
        //            Logger.Log("No Destination Found, moving to kite centroid");

        //            if (Core.Avoidance.KiteCentroidNodes.Any())
        //                CurrentDestination = Core.Avoidance.KiteCentroidNodes.FirstOrDefault();

        //            if (CurrentDestination == null)
        //                return true;
        //        }
        //    }

        //    if (distance >= 4f)
        //    {
        //        if (distance < 10f)
        //        {
        //            Logger.LogVerbose("Moving (Basic) to Avoidance Position {0} Distance={1}", CurrentDestination.NavigableCenter, CurrentDestination.Distance);
        //            Navigator.PlayerMover.MoveTowards(CurrentDestination.NavigableCenter);
        //            return false;
        //        }

        //        // Double check that navigator isnt trying to path us accross really bad stuff
        //        var isNavigatorPath = PlayerMover.NavigationProvider != null && PlayerMover.NavigationProvider.CurrentPath != null && PlayerMover.NavigationProvider.CurrentPath.Count > 0;
        //        if (isNavigatorPath && PlayerMover.NavigationProvider.CurrentPath.Any(p => Core.Avoidance.Grid.IsIntersectedByFlags(Trinity.Player.Position, p, AvoidanceFlags.CriticalAvoidance)))
        //        {
        //            Logger.Log("Navi, Navi, Navi, you have been a bad monkey!");

        //            var first = new Vector3();
        //            foreach (var p in Core.Avoidance.SafeCentroidPositions)
        //            {
        //                if (p.Distance(Trinity.Player.Position) > 16f && Core.Avoidance.Grid.IsIntersectedByFlags(Trinity.Player.Position, p, AvoidanceFlags.CriticalAvoidance))
        //                {
        //                    first = p;
        //                    break;
        //                }
        //            }

        //            if (first != Vector3.Zero)
        //            {
        //                Logger.Log("Using a backup position!");
        //                Navigator.PlayerMover.MoveTowards(first);
        //            }
        //            else
        //            {
        //                Logger.Log("Staying put!");
        //                Navigator.PlayerMover.MoveTowards(Trinity.Player.Position);
        //            }
        //            return false;
        //        }

        //        Logger.LogVerbose("Moving (PathFinder) to Avoidance Position {0} Distance={1}", CurrentDestination.NavigableCenter, CurrentDestination.Distance);
        //        var result = await CommonCoroutines.MoveTo(CurrentDestination.NavigableCenter, "AvoidPosition");
        //        switch (result)
        //        {
        //            case MoveResult.PathGenerationFailed:
        //            case MoveResult.UnstuckAttempt:
        //                Logger.LogVerbose("PathFinding Failed", CurrentDestination.NavigableCenter, CurrentDestination.Distance);
        //                CurrentDestination = null;
        //                return true;
        //        }
        //        return false;
        //    }

        //    LastArrivedAtSafeSpot = DateTime.UtcNow;            
        //    Logger.LogVerbose("Arrived at Avoidance Position");
        //    return true;
        //}

        private static bool IsStandingInFlags(params AvoidanceFlags[] flags)
        {
            return Core.Avoidance.NearbyNodes.Any(n => flags.Any(f => n.AvoidanceFlags.HasFlag(f)));
        }

        private static bool IsPathingOverFlags(params AvoidanceFlags[] flags)
        {
            if (PlayerMover.NavigationProvider == null || PlayerMover.NavigationProvider.CurrentPath == null)
                return false;

            var currentPath = PlayerMover.NavigationProvider.CurrentPath;
            if (currentPath.Count <= 0)
                return false;

            var overFlags = currentPath.Any(p => Core.Avoidance.Grid.IsIntersectedByFlags(Trinity.Player.Position, p, flags));
            return overFlags;
        }

        private static bool IsCurrentPathAvoidanceDestination()
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


