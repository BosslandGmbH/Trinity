using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.UI.Visualizer;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public static class ExplorationHelpers
    {
        public static Vector3 PriorityPosition { get; private set; }

        public static void ClearExplorationPriority()
        {
            PriorityPosition = Vector3.Zero;
        }

        public static void SetExplorationPriority(Vector3 position)
        {
            if (position == PriorityPosition)
                return;

            PriorityPosition = position;

            VisualizerViewModel.DebugPosition = position;

            Core.Logger.Warn($"Setting priority exploration position to '{position}' {position.Distance(Core.Player.Position)} yards away!");

            foreach (var connection in Core.Scenes.CurrentScene.GetConnectedScenes(Core.Scenes.GetScene(position)))
            {
                connection.Scene.Nodes.ForEach(n => n.Priority = true);
            }
        }

        public static bool IsInPriorityDirection(Vector3 position, double degreesDifferenceAllowed)
        {
            if (PriorityPosition == Vector3.Zero) return false;

            return MathUtil.GetRelativeAngularVariance(Core.Player.Position, PriorityPosition, position) <= degreesDifferenceAllowed;
        }

        public static bool IsInSceneConnectionDirection(Vector3 position, double degreesDifferenceAllowed)
        {
            return Core.Scenes.CurrentScene.ExitPositions.Values.Any(p => MathUtil.GetRelativeAngularVariance(Core.Player.Position, p, position) <= degreesDifferenceAllowed);
        }

        public static bool IsInUnexploredScene(Vector3 position) => GetScene(position) != null;

        public static WorldScene GetScene(Vector3 position, Func<WorldScene, bool> func = null)
        {
            return Core.Scenes.Where(s => s.ExitPositions.Count > 1).FirstOrDefault(s => s.IsInScene(position) && (func == null || func(s)));
        }

        public static ExplorationNode NearestWeightedUnvisitedNode(HashSet<int> levelAreaIds, List<string> ignoreScenes = null)
        {
            var worldId = AdvDia.CurrentWorldDynamicId;
            var myPosition = AdvDia.MyPosition;

            using (new PerformanceLogger("NearestUnvisitedNodeLocation", true))
            {
                var nearestNode = ExplorationGrid.Instance.GetNearestWalkableNodeToPosition(myPosition);

                if (Core.Rift.IsInRift && !Core.BlockedCheck.IsBlocked && !Core.StuckHandler.IsStuck)
                {
                    // In rift prefer the highest weight nodes because we dont need to explore everything, just need to find the exit.    
                    var closestUnvisitedNodes = ExplorationGrid.Instance.WalkableNodes
                        .Where(n => !n.IsIgnored && !n.IsVisited && !n.IsBlacklisted && n.HasEnoughNavigableCells &&
                                    n.DynamicWorldId == worldId && levelAreaIds.Contains(n.LevelAreaId))
                        .OrderByDescending(n => n.Priority)
                        .ThenBy(n => n.Distance)
                        .ThenByDescending(PriorityDistanceFormula);

                    var closestUnvisitedNode = closestUnvisitedNodes.FirstOrDefault();
                    if (closestUnvisitedNode != null)
                    {
                        var weight = PriorityDistanceFormula(closestUnvisitedNode);
                        Core.Logger.Debug(LogCategory.Exploration, $"Explore: Best Rift Weighted Node: [{closestUnvisitedNode.NavigableCenter.X},{closestUnvisitedNode.NavigableCenter.Y},{closestUnvisitedNode.NavigableCenter.Z}] Dist:{closestUnvisitedNode.Distance} Weight: {weight} {(closestUnvisitedNode.Priority ? "(Priority)" : "")} ");
                        return closestUnvisitedNode;
                    }
                }

                // Try for nodes nearby walkable nodes first
                for (var i = 3; i <= 4; i++)
                {
                    var closestUnvisitedNodes = ExplorationGrid.Instance.GetNeighbors(nearestNode, i)
                        .Where(n => !n.IsIgnored && !n.IsVisited && !n.IsBlacklisted && n.HasEnoughNavigableCells &&
                                    n.DynamicWorldId == worldId && levelAreaIds.Contains(n.LevelAreaId) &&
                                    Core.Grids.CanRayWalk(myPosition, n.NavigableCenter))
                        .OrderByDescending(n => n.Priority)
                        .ThenBy(n => n.Distance)
                        .ThenByDescending(PriorityDistanceFormula);

                    var closestUnvisitedNode = closestUnvisitedNodes.FirstOrDefault();
                    if (closestUnvisitedNode != null)
                    {
                        Core.Logger.Debug(LogCategory.Exploration, $"Explore: Selected Nearby Node: [{closestUnvisitedNode.NavigableCenter.X},{closestUnvisitedNode.NavigableCenter.Y},{closestUnvisitedNode.NavigableCenter.Z}] Dist:{closestUnvisitedNode.Distance} {(closestUnvisitedNode.Priority ? "(Priority)" : "")} ");
                        return closestUnvisitedNode;
                    }
                }

                // Try any nearby nodes
                for (var i = 3; i <= 6; i++)
                {
                    var closestUnvisitedNode = ExplorationGrid.Instance.GetNeighbors(nearestNode, i)
                        .Where(n => !n.IsIgnored && !n.IsVisited && !n.IsBlacklisted && n.HasEnoughNavigableCells &&
                        n.DynamicWorldId == worldId && levelAreaIds.Contains(n.LevelAreaId))
                        .OrderByDescending(PriorityDistanceFormula)
                        .FirstOrDefault();

                    if (closestUnvisitedNode != null)
                    {
                        Core.Logger.Debug(LogCategory.Exploration, $"Explore: Selected Nearby Node: [{closestUnvisitedNode.NavigableCenter.X},{closestUnvisitedNode.NavigableCenter.Y},{closestUnvisitedNode.NavigableCenter.Z}] Dist:{closestUnvisitedNode.Distance} {(closestUnvisitedNode.Priority ? "(Priority)" : "")} ");
                        return closestUnvisitedNode;
                    }
                }

                // Try any univisted node by distance.
                var node = ExplorationGrid.Instance.WalkableNodes.Where(n =>
                        !n.IsIgnored &&
                        !n.IsVisited &&
                        !n.IsBlacklisted &&
                        n.NavigableCenter.DistanceSqr(myPosition) > 20
                    )
                    .OrderByDescending(PriorityDistanceFormula)
                    .FirstOrDefault();

                if (node != null)
                {
                    Core.Logger.Debug(LogCategory.Exploration, $"Explore: Selected Nearby Node: [{node.NavigableCenter.X},{node.NavigableCenter.Y},{node.NavigableCenter.Z}] Dist:{node.Distance} {(node.Priority ? "(Priority)" : "")} ");
                    return node;
                }

                if (ExplorationGrid.Instance.NearestNode != null && !levelAreaIds.Contains(ZetaDia.CurrentLevelAreaSnoId))
                {
                    Core.Logger.Debug("[ExplorationLogic] Adventurer is trying to find nodes that are not in this LevelArea. DefinedIds='{0}' CurrentId='{0}'. Marking current area's nodes as valid.", string.Join(", ", levelAreaIds), ZetaDia.CurrentLevelAreaSnoId);
                    levelAreaIds.Add(ZetaDia.CurrentLevelAreaSnoId);

                    // Ignore level area match
                    node = ExplorationGrid.Instance.WalkableNodes
                    .Where(n =>
                        !n.IsIgnored &&
                        !n.IsVisited &&
                        !n.IsBlacklisted &&
                        n.DynamicWorldId == worldId &&
                        n.NavigableCenter.DistanceSqr(myPosition) > 100)
                    .OrderByDescending(PriorityDistanceFormula)
                    .FirstOrDefault();
                }

                if (node == null)
                {
                    var allNodes = ExplorationGrid.Instance.WalkableNodes.Count(n => levelAreaIds.Contains(n.LevelAreaId));
                    var unvisitedNodes = ExplorationGrid.Instance.WalkableNodes.Count(n => !n.IsVisited && levelAreaIds.Contains(n.LevelAreaId));

                    Core.Logger.Log("[ExplorationLogic] Couldn't find any unvisited nodes. Current AdvDia.LevelAreaSnoIdId: {0}, " +
                                "ZetaDia.CurrentLevelAreaSnoId: {3}, Total Nodes: {1} Unvisited Nodes: {2} Searching In [{4}] HasNavServerData={5}",
                                AdvDia.CurrentLevelAreaId, allNodes, unvisitedNodes,
                                ZetaDia.CurrentLevelAreaSnoId, string.Join(", ", levelAreaIds),
                                AdvDia.MainGridProvider.Width != 0);

                    //Core.Scenes.Reset();
                }
                Core.Logger.Debug(LogCategory.Exploration, $"Explore: Selected Nearby Node: [{node?.NavigableCenter.X},{node?.NavigableCenter.Y},{node?.NavigableCenter.Z}] Dist:{node?.Distance} Priority: {node?.Priority} ");

                return node;
            }
        }

        public static double PriorityDistanceFormula(ExplorationNode n)
        {
            var directionMultiplier = IsInPriorityDirection(n.NavigableCenter, 30) ? 4.5 : 1;
            var sceneConnectionDirectionMultiplier = IsInSceneConnectionDirection(n.NavigableCenter, 30) ? 1.25 : 1;
            var nodeInPrioritySceneMultiplier = n.Priority ? 2.25 : 0;
            var baseDistanceFactor = 50 / n.NavigableCenter.Distance(AdvDia.MyPosition) * 10;
            var canRayWalk = Core.Grids.CanRayWalk(AdvDia.MyPosition, n.NavigableCenter) ? 1.75 : 1;

            var edgeMultiplier = 1d;
            var visitedMultiplier = 1d;
            var exitSceneMultiplier = 1d;
            var exploredPercent = ExplorationGrid.Instance.WalkableNodes.Count(x => x.Scene.HasBeenVisited) /
                                  ExplorationGrid.Instance.WalkableNodes.Count();

            // for now.. restrict this group of checks from effecting bounties.
            if (Core.Rift.IsInRift)
            {
                var isInExitScene = n.Scene.Name.Contains("Exit");
                exitSceneMultiplier = isInExitScene ? 100 / n.Distance : 1;
                visitedMultiplier = n.Scene.HasBeenVisited && !isInExitScene ? 0.01f : 1f;

                // Ignore dead end scenes.
                if (n.Scene.ExitPositions.Count <= 1 && !isInExitScene)
                    return 0;

                if (!Core.Grids.CanRayWalk(Core.Player.Position, n.NavigableCenter))
                    return 0;

                // Lower weight for scenes near the edge of an open style map.
                edgeMultiplier = (n.Scene.Name.Contains("Border") || n.Scene.Name.Contains("Edge")) && n.Distance < 100 &&
                                 exploredPercent > 0.85
                    ? 2.5
                    : 1;
            }

            return baseDistanceFactor * exitSceneMultiplier *
                   directionMultiplier * sceneConnectionDirectionMultiplier
                   * (n.UnvisitedWeight + nodeInPrioritySceneMultiplier) * visitedMultiplier * edgeMultiplier *
                   canRayWalk;
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

        private static WorldScene _currentScene;

        public static void MarkNodesAsVisited(IEnumerable<string> sceneNames)
        {
            MarkSceneNodes(sceneNames, true);
        }

        public static void MarkNodesAsUnvisited(IEnumerable<string> sceneNames)
        {
            MarkSceneNodes(sceneNames, false);
        }

        public static void MarkSceneNodes(IEnumerable<string> sceneNames, bool asVisited)
        {
            if (sceneNames == null)
                return;

            sceneNames = sceneNames.Select(s => s.ToLowerInvariant()).ToList();
            foreach (var scene in Core.Scenes)
            {
                var fullname = scene.Name.ToLowerInvariant();
                if (sceneNames.Any(s => fullname.Contains(s)))
                {
                    foreach (var node in scene.Nodes)
                    {
                        node.IsVisited = asVisited;
                    }
                    Core.Logger.Debug($"Marked {scene.Nodes.Count} exploration nodes for scene {fullname} ({scene.SnoId}) as visited");
                }
            }
        }

        public static void UpdateIgnoreRegions()
        {
            var currentScene = Core.Scenes.CurrentScene;
            if (currentScene != null && currentScene != _currentScene)
            {
                MarkIgnoreRegionsAsVisited(currentScene);
                foreach (var connectedScene in currentScene.ConnectedScenes())
                {
                    MarkIgnoreRegionsAsVisited(connectedScene.Scene);
                }
                _currentScene = currentScene;
            }
        }

        public static bool MarkIgnoreRegionsAsVisited(WorldScene scene)
        {
            if (scene?.Nodes == null) return false;
            int modifiedNodes = 0;
            foreach (var node in scene.Nodes)
            {
                if (node == null) continue;
                if (scene.IgnoreRegions.Contains(node.NavigableCenter))
                {
                    modifiedNodes++;
                    node.IsVisited = true;
                }
            }
            Core.Logger.Debug($"Marked {modifiedNodes} exploration nodes for ignore regions in {scene.Name} ({scene.SnoId}) as visited");
            return modifiedNodes > 0;
        }

        public static void MarkNodesAsVisited(IEnumerable<WorldScene> scenes)
        {
            var sceneLookup = new HashSet<int>(scenes.Select(s => s.SnoId));
            ExplorationGrid.Instance.WalkableNodes.Where(s => sceneLookup.Contains(s.Scene.SnoId)).ForEach(s => s.IsVisited = true);
        }

        public static List<Vector3> GetFourPointsInEachDirection(Vector3 center, int radius)
        {
            var result = new List<Vector3>();
            var nearestNode = ExplorationGrid.Instance.GetNearestNode(center);
            if (nearestNode == null) return result;
            return
                ExplorationGrid.GetExplorationNodesInRadius(nearestNode, radius)
                    .Where(n => n.HasEnoughNavigableCells & n.NavigableCenter.Distance(AdvDia.MyPosition) > 10)
                    .Select(n => n.NavigableCenter)
                    .ToList();
        }
    }
}