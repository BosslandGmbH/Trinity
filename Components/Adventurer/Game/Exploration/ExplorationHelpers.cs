using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public static class ExplorationHelpers
    {

        //public static AdventurerNode NearestWeightedUnvisitedNodeLocation(HashSet<int> levelAreaIds = null)
        //{

        //    using (new PerformanceLogger("NearestWeightedUnvisitedNodeLocation", true))
        //    {
        //        return NodesStorage.CurrentWorldNodes.Where(n => !n.Visited && n.NavigableCenter.Distance2DSqr(AdvDia.MyPosition) > 100 && n.NavigableCenter.IsInLevelAreas(levelAreaIds))
        //            .OrderByDescending(n => n.NearbyVisitedNodesCount)
        //            .ThenBy(n => n.NavigableCenter.Distance2DSqr(AdvDia.MyPosition))
        //            .FirstOrDefault();
        //    }
        //}


        public static ExplorationNode NearestWeightedUnvisitedNode(HashSet<int> levelAreaIds, List<string> ignoreScenes = null)
        {
            var dynamicWorldId = AdvDia.CurrentWorldDynamicId;
            var myPosition = AdvDia.MyPosition;
            ExplorationNode node = null;
            using (new PerformanceLogger("NearestUnvisitedNodeLocation", true))
            {
                var nearestNode = ExplorationGrid.Instance.GetNearestWalkableNodeToPosition(myPosition);
                for (var i = 3; i <= 4; i++)
                {
                    var closestUnvisitedNode = ExplorationGrid.Instance.GetNeighbors(nearestNode, i).Cast<ExplorationNode>()
                        .Where(n => !n.IsIgnored && !n.IsVisited && !n.IsBlacklisted && n.HasEnoughNavigableCells && 
                        n.DynamicWorldId == dynamicWorldId && levelAreaIds.Contains(n.LevelAreaId) && 
                        NavigationGrid.Instance.CanRayWalk(myPosition, n.NavigableCenter))
                        .OrderBy(n => n.Distance)
                        .FirstOrDefault();
                    if (closestUnvisitedNode != null)
                    {
                        node = closestUnvisitedNode;
                    }
                }

                if (node == null)
                {
                    Logger.DebugSetting("Closest Unvisited Node Selection Failed");

                    //var nodes = ExplorationGrid.Instance.WalkableNodes.Where(
                    //    n =>
                    //        !n.IsVisited && n.WorldId == dynamicWorldId &&
                    //        n.NavigableCenter.DistanceSqr(myPosition) > 100 &&
                    //        levelAreaIds.Contains(n.LevelAreaSnoIdId) && n.GetNeighbors(3).Count(nn => n.HasEnoughNavigableCells) > 1)
                    //    .OrderBy(n =>  n.NavigableCenter.DistanceSqr(myPosition))
                    //    .Take(10)
                    //    .ToList();
                    //if (nodes.Count > 1)
                    //{
                    //    var min = nodes.Min(n => n.NavigableCenter.Distance(myPosition));
                    //    var max = nodes.Max(n => n.NavigableCenter.Distance(myPosition));
                    //    var averageDistance = (max + min) / 2 + 1;
                    //    node =
                    //        nodes.Where(n => n.NavigableCenter.Distance(myPosition) < averageDistance)
                    //            .OrderBy(n => n.UnvisitedWeight)
                    //            .FirstOrDefault();
                    //}
                    //else if (nodes.Count == 1)
                    //{
                    //    node = nodes[0];
                    //}
                    node =
                        ExplorationGrid.Instance.WalkableNodes
                        .Where(n =>
                            !n.IsIgnored &&
                            !n.IsVisited &&
                            !n.IsBlacklisted &&
                            n.DynamicWorldId == dynamicWorldId &&
                            n.NavigableCenter.DistanceSqr(myPosition) > 100 &&
                            levelAreaIds.Contains(n.LevelAreaId))
                        .OrderByDescending(n => (1 / n.NavigableCenter.Distance(AdvDia.MyPosition)) * n.UnvisitedWeight)
                        .FirstOrDefault();
                    //if (node != null)
                    //{
                    //    Logger.Debug("[ExplorationLogic] Picked a node using unvisited weighting method (Node Distance: {0}, Node Weight: {1})", node.NavigableCenter.Distance(AdvDia.MyPosition), node.UnvisitedWeight);
                    //}
                }

                if (node == null && ExplorationGrid.Instance.NearestNode != null && !levelAreaIds.Contains(ZetaDia.CurrentLevelAreaSnoId))
                {
                    Logger.Debug("[ExplorationLogic] Adventurer is trying to find nodes that are not in this LevelArea. DefinedIds='{0}' CurrentId='{0}'. Marking current area's nodes as valid.", string.Join(", ", levelAreaIds), ZetaDia.CurrentLevelAreaSnoId);                     
                    levelAreaIds.Add(ZetaDia.CurrentLevelAreaSnoId);

                    // Ignore level area match   
                    node = ExplorationGrid.Instance.WalkableNodes
                    .Where(n =>
                        !n.IsIgnored &&
                        !n.IsVisited &&
                        !n.IsBlacklisted &&
                        n.DynamicWorldId == dynamicWorldId &&
                        n.NavigableCenter.DistanceSqr(myPosition) > 100)
                    .OrderByDescending(n => (1 / n.NavigableCenter.Distance(AdvDia.MyPosition)) * n.UnvisitedWeight)
                    .FirstOrDefault();
                }

                if (node == null)
                {
                    var allNodes = ExplorationGrid.Instance.WalkableNodes.Count(n => levelAreaIds.Contains(n.LevelAreaId));
                    var unvisitedNodes = ExplorationGrid.Instance.WalkableNodes.Count(n => !n.IsVisited && levelAreaIds.Contains(n.LevelAreaId));

                    Logger.Info("[ExplorationLogic] Couldn't find any unvisited nodes. Current AdvDia.LevelAreaSnoIdId: {0}, " +
                                "ZetaDia.CurrentLevelAreaSnoId: {3}, Total Nodes: {1} Unvisited Nodes: {2} Searching In [{4}] HasNavServerData={5}", 
                                AdvDia.CurrentLevelAreaId, allNodes, unvisitedNodes, 
                                ZetaDia.CurrentLevelAreaSnoId, string.Join(", ", levelAreaIds),
                                AdvDia.MainGridProvider.Width != 0);

                    ScenesStorage.Reset();
                    BotMain.PauseFor(TimeSpan.FromSeconds(1));
                }
                return node;
            }
        }

        /// <summary>
        /// Defines areas of a scene where exploration nodes should be blacklisted
        /// Index by SceneSnoId (Can be found on InfoDumping Tab). Each WorldScene grabs this list of BlacklistedPositions
        /// When Exploration nodes are created within a scene, they are marked 'IsBlacklsited' if any position is inside their bounds.
        /// </summary>
        public static Dictionary<int, HashSet<Vector3>> BlacklistedPositionsBySceneSnoId = new Dictionary<int, HashSet<Vector3>>
        {
            { 53711, new HashSet<Vector3>
                {
                    // Dulgur oasis scene with decking area in the bottom left of map.
                    new Vector3(811.1603f,646.8987f,1.828806f), 
                    new Vector3(814.6924f,668.8029f,1.828797f),
                    new Vector3(833.3029f,652.3396f,1.828796f)
                }
            }  
        };                     

        public static List<Vector3> GetFourPointsInEachDirection(Vector3 center, int radius)
        {
            var result = new List<Vector3>();
            var nearestNode = ExplorationGrid.Instance.GetNearestNode(center) as ExplorationNode;
            if (nearestNode == null) return result;
            return ExplorationGrid.GetExplorationNodesInRadius(nearestNode, radius).Where(n => n.HasEnoughNavigableCells & n.NavigableCenter.Distance(AdvDia.MyPosition) > 10).Select(n => n.NavigableCenter).ToList();
            try
            {
                var node1 = ExplorationGrid.Instance.GetNearestWalkableNodeToPosition(new Vector3(center.X - radius, center.Y + radius, 0));
                if (node1 != null)
                {
                    result.Add(node1.NavigableCenter);
                }
                var node2 = ExplorationGrid.Instance.GetNearestWalkableNodeToPosition(new Vector3(center.X + radius, center.Y + radius, 0));
                if (node2 != null)
                {
                    result.Add(node2.NavigableCenter);
                }

                var node3 = ExplorationGrid.Instance.GetNearestWalkableNodeToPosition(new Vector3(center.X + radius, center.Y - radius, 0));
                if (node3 != null)
                {
                    result.Add(node3.NavigableCenter);
                }

                var node4 = ExplorationGrid.Instance.GetNearestWalkableNodeToPosition(new Vector3(center.X - radius, center.Y - radius, 0));
                if (node4 != null)
                {
                    result.Add(node4.NavigableCenter);
                }
            }
            catch (Exception)
            {

            }

            return result;
        }

    }
}
