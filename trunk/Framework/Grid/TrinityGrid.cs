using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Combat;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Modules;
using Zeta.Common;
using Zeta.Game;
using Direction = Trinity.Components.Adventurer.Game.Exploration.Direction;

using NodeFlags = Trinity.Components.Adventurer.Game.Exploration.NodeFlags;
using SceneData = Trinity.Components.Adventurer.Game.Exploration.SceneData;

namespace Trinity.Framework.Grid
{
    public sealed class TrinityGrid : Grid<AvoidanceNode>
    {
        static TrinityGrid()
        {
            Flags = Enum.GetValues(typeof(AvoidanceFlags)).Cast<AvoidanceFlags>().ToList();
        }

        private static List<AvoidanceFlags> Flags { get; set; }

        private const int Bounds = 2500;

        public override float BoxSize => 2.5f;
        public override int GridBounds => Bounds;
        public static TrinityGrid Instance => GetWorldGrid();

        public bool IsUpdatingNodes { get; set; }

        private static TrinityGrid _currentGrid;

        public static TrinityGrid GetWorldGrid()
        {
            if (_currentGrid == null)
            {
                _currentGrid = new TrinityGrid();
            }
            else if (_currentGrid == null || ZetaDia.Globals.WorldId != _currentGrid.WorldDynamicId)
            {
                _currentGrid = new TrinityGrid();
            }
            return _currentGrid;
        }

        public static TrinityGrid GetUnsafeGrid() => _currentGrid;

        public bool IsValidGridWorldPosition(Vector3 position)
        {
            return position.X > 0 && position.Y > 0 && position != Vector3.Zero && position.X < (MaxX * BoxSize) && position.Y < (MaxY * BoxSize);
        }

        public bool IsPopulated { get; set; }

        public override bool CanRayCast(Vector3 @from, Vector3 to)
        {
            try
            {
                if (!IsPopulated) return false;
                if (!IsValidGridWorldPosition(@from) || !IsValidGridWorldPosition(to)) return false;
                return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).All(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowProjectile));
            }
            catch (Exception ex)
            {
                Core.Logger.Error($"Exception in CanRayCast from={@from} to={to} {ex}");
            }
            return false;
        }


        public bool UnsafeCanRayCast(Vector3 @from, Vector3 to)
        {
            if (@from == Vector3.Zero || to == Vector3.Zero) return false;
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).All(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowProjectile));
        }

        public bool CanRayWalk(TrinityActor targetActor)
        {
            if (!IsPopulated) return false;
            var playerPosition = MathEx.GetPointAt(Core.Player.Position, Core.Player.Radius, Core.Player.Rotation);
            var targetPosition = MathEx.GetPointAt(targetActor.Position, targetActor.Radius, MathEx.WrapAngle((float)(Core.Player.Rotation + Math.PI)));

            if (!IsValidGridWorldPosition(playerPosition) || !IsValidGridWorldPosition(targetPosition)) return false;
            return GetRayLine(playerPosition, targetPosition).Select(point => InnerGrid[point.X, point.Y]).All(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk));
        }

        public bool CanRayWalk(TrinityActor targetActor, float radiusDegrees)
        {
            return CanRayWalk(Core.Player.Position, targetActor.Position, radiusDegrees);
        }

        public bool CanRayWalk(Vector3 from, Vector3 to,  float radiusDegrees)
        {
            var radiusRadians = MathEx.ToRadians(radiusDegrees);
            var angleTo = (float)MathUtil.FindDirectionRadian(from, to);
            var dist = from.Distance(to);

            var losAngleA = MathEx.WrapAngle(angleTo - radiusRadians);
            var losPositionA = MathEx.GetPointAt(from, dist, losAngleA);
            if (!CanRayWalk(from, losPositionA))
                return false;

            var losAngleB = MathEx.WrapAngle(angleTo + radiusRadians);
            var losPositionB = MathEx.GetPointAt(from, dist, losAngleB);
            if (!CanRayWalk(from, losPositionB))
                return false;

            return CanRayWalk(from, losPositionB);
        }


        public bool UnsafeCanRayWalk(TrinityActor targetActor)
        {
            if (targetActor.Position == Vector3.Zero) return false;
            return GetRayLine(Core.Player.Position, targetActor.Position).Select(point => InnerGrid[point.X, point.Y]).All(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk));
        }

        public override bool CanRayWalk(Vector3 @from, Vector3 to)
        {
            if (!IsValidGridWorldPosition(@from) || !IsValidGridWorldPosition(to)) return false;
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).All(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk) && !node.NodeFlags.HasFlag(NodeFlags.NearWall));
        }

        public bool IsIntersectedByFlags(Vector3 @from, Vector3 to, params AvoidanceFlags[] flags)
        {
            if (!IsValidGridWorldPosition(@from) || !IsValidGridWorldPosition(to)) return false;
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).Any(node => node != null && flags != null && flags.Any(f => node.AvoidanceFlags.HasFlag(f)));
        }

        public Vector3 GetPathCastPosition(float maxDistance, bool updatePath = false)
        {
            return GetFurthestPathPosition(Core.DBNavProvider.CurrentPath, maxDistance, RayType.Cast, updatePath);
        }

        public Vector3 GetPathWalkPosition(float maxDistance, bool updatePath = false)
        {
            return GetFurthestPathPosition(Core.DBNavProvider.CurrentPath, maxDistance, RayType.Walk, updatePath);
        }

        public Vector3 GetPathPosition(float maxDistance, bool updatePath = false)
        {
            return GetFurthestPathPosition(Core.DBNavProvider.CurrentPath, maxDistance, RayType.None, updatePath);
        }

        /// <summary>
        /// Mark all future points on the current path that are walkable/castable as visited.
        /// </summary>
        /// <param name="maxDistance">beyond this range nodes will be left alone</param>
        /// <param name="type">the type of check to make</param>
        public void AdvanceNavigatorPath(float maxDistance, RayType type)
        {
            var path = Core.DBNavProvider.CurrentPath;
            if (path == null || path.Count == 0)
                return;

            var startPosition = ZetaDia.Me.Position;
            for (int i = path.Index; i < path.Count; i++)
            {
                var point = path[i];
                if (startPosition.Distance(point) > maxDistance ||
                    type == RayType.Cast && !CanRayCast(startPosition, point) ||
                    type == RayType.Walk && !CanRayWalk(startPosition, point))
                {
                    path.Index = i;
                    break;
                }
            }
        }

        private Vector3 GetFurthestPathPosition(IndexedList<Vector3> path, float maxDistance, RayType type, bool updatePath)
        {
            if (path == null || path.Count == 0)
                return Vector3.Zero;

            Vector3 startPosition = Core.Player.Position;
            Vector3 reachablePosition = startPosition;
            Vector3 unreachablePosition = path.LastOrDefault();

            // Find closest valid path point;
            for (int i = path.Index; i < path.Count; i++)
            {
                var point = path[i];
                if (startPosition.Distance(point) > maxDistance || type == RayType.Cast && !CanRayCast(startPosition, point) || type == RayType.Walk && !CanRayWalk(startPosition, point))
                {
                    if (updatePath)
                    {
                        path.Index = i;
                    }
                    unreachablePosition = point;
                    break;
                }
                reachablePosition = point;
            }

            var distance = reachablePosition.Distance(unreachablePosition);
            const float incrementDistance = 2f;
            var totalSegments = distance / incrementDistance;

            // Find closest valid portion of path.
            for (int i = 0; i < totalSegments; i++)
            {
                var point = MathEx.CalculatePointFrom(unreachablePosition, reachablePosition, i * incrementDistance);
                if (startPosition.Distance(point) > maxDistance || type == RayType.Cast && !CanRayCast(startPosition, point) || type == RayType.Walk && !CanRayWalk(startPosition, point))
                    break;

                reachablePosition = point;
            }

            return reachablePosition;
        }

        public bool RayFromTargetMissingFlagsCount(Vector3 origin, Vector3 target, int requiredFlagCount, params AvoidanceFlags[] flags)
        {
            var flagTolerenceCounts = flags.ToDictionary(k => k, v => 0);
            if (!IsValidGridWorldPosition(origin) || !IsValidGridWorldPosition(target)) return false;
            foreach (var node in GetRayLine(target, origin).Select(point => InnerGrid[point.X, point.Y]))
            {
                if (node == null || flags == null)
                    break;

                foreach (var flag in flags)
                {
                    if (!node.AvoidanceFlags.HasFlag(flag))
                    {
                        flagTolerenceCounts[flag]++;
                        if (flagTolerenceCounts[flag] >= requiredFlagCount)
                            return true;
                    }
                }
            }
            return false;
        }

        internal IEnumerable<AvoidanceNode> GetRayLineAsNodes(Vector3 from, Vector3 to)
        {
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).Where(n => n != null);
        }

        public IEnumerable<AvoidanceNode> GetConeAsNodes(Vector3 position, float arcWidthDegrees, float radius, float rotationRadians)
        {
            //GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).Where(n => n != null)

            var rotationDegrees = MathEx.ToDegrees(rotationRadians);
            var arcStartPosition = MathEx.GetPointAt(position, radius, (float)MathUtil.DegreeToRadian(MathUtil.FixAngleTo360(rotationDegrees - arcWidthDegrees / 2)));
            var arcEndPosition = MathEx.GetPointAt(position, radius, (float)MathUtil.DegreeToRadian(MathUtil.FixAngleTo360(rotationDegrees + arcWidthDegrees / 2)));
            return new List<AvoidanceNode>();

            // todo: add method to get adjacent node at angle
            // todo: write iterator to either spiral and test every point in triangle or possibly:
            // test nodes at around @ distance starting in direction until non-triangle nodes found at either end
            // increment distance and repeat until back edge found.
            // possibly using MathUtil.IsNaivePointInTriangle();
        }

        public override void Reset()
        {
        }

        protected override void OnUpdated(SceneData newNodes)
        {
            IsUpdatingNodes = true;

            var sw = Stopwatch.StartNew();
            var nodeCount = 0;

            var gridName = GetType().Name;

            foreach (var scene in newNodes.Scenes)
            {
                var nodes = scene.ExplorationNodes.SelectMany(n => n.Nodes, (p, c) => new AvoidanceNode(c)).ToList();

                Core.Logger.Verbose($"[{gridName}] Updating grid for scene '{scene.SceneHash}' with {scene.ExplorationNodes.Count} new nodes");

                UpdateInnerGrid(nodes);

                foreach (var node in nodes)
                {
                    nodeCount++;
                    if (GetNeighbors(node).Any(n => (n.NodeFlags & NodeFlags.AllowWalk) == 0))
                    {
                        node.NodeFlags |= NodeFlags.NearWall;
                    }
                }
            }

            IsUpdatingNodes = false;
            IsPopulated = true;

            sw.Stop();
            Core.Logger.Verbose($"Avoidance Grid updated NewNodes={nodeCount} NearestNodeFound={NearestNode != null} Time={sw.Elapsed.TotalMilliseconds}ms");
        }

        public void FlagNodes(IEnumerable<AvoidanceNode> nodes, AvoidanceFlags flags, int weightModification = 0)
        {
            foreach (var node in nodes.Where(node => node != null && node.AvoidanceFlags.HasFlag(AvoidanceFlags.AllowWalk)))
            {
                node.Weight += weightModification;
                node.AddNodeFlags(flags);
            }
        }

        public void FlagAvoidanceNodes(IEnumerable<AvoidanceNode> nodes, AvoidanceFlags flags, Avoidance.Structures.Avoidance avoidance, int weightModification = 0)
        {
            var type = avoidance.Definition.Type;
            var hashCode = avoidance.GetHashCode();

            foreach (var node in nodes.Where(node => node != null && node.AvoidanceFlags.HasFlag(AvoidanceFlags.AllowWalk)))
            {
                if (type != AvoidanceType.None)
                {
                    node.AvoidanceTypes.Add(type);
                }
                node.AvoidanceHashCodes.Add(hashCode);
                node.Weight += weightModification;
                node.AddNodeFlags(flags);
            }
        }

        public AvoidanceNode GetClosestWalkableNodeTo(Vector3 position)
        {
            return GetNodesInRadius(position, node => node.IsWalkable).OrderBy(n => n.NavigableCenter.Distance(position)).FirstOrDefault();
        }

        public List<AvoidanceNode> GetConnectedNodes(Vector3 position, Func<AvoidanceNode, bool> condition, float maxDistance = 30f, float minDistance = -1f, bool includeThisNode = true)
        {
            var nodes = new List<AvoidanceNode>();

            maxDistance = maxDistance >= 0 ? maxDistance + 1 : 30f;
            minDistance = minDistance >= 0 ? minDistance : -1f;

            if (position == Vector3.Zero)
                return nodes;

            var startNode = GetNearestNode(position);
            if (startNode == null)
                return nodes;

            var gridRadiusMax = Math.Round(maxDistance / 2.5, 0, MidpointRounding.AwayFromZero);
            var gridRadiusMin = Math.Round(minDistance / 2.5, 0, MidpointRounding.AwayFromZero);

            if (includeThisNode && condition(startNode))
                nodes.Add(startNode);

            var i = 1;
            var curNode = startNode;

            while (i <= gridRadiusMax + 1)
            {
                var edgeLength = i * 2 - 1;
                var final = i >= gridRadiusMax - 1;
                var isFinalEdge = Math.Abs(i - (gridRadiusMax + 1)) < double.Epsilon;
                var subMin = i <= gridRadiusMin;
                var numChecked = 0;
                while (numChecked < edgeLength)
                {
                    if (!TryGetNextNode(position, condition, Direction.South, maxDistance, subMin, final, nodes, ref curNode, ref numChecked))
                        return nodes;
                }
                if (isFinalEdge) return nodes;
                numChecked = 0;
                while (numChecked < edgeLength)
                {
                    if (!TryGetNextNode(position, condition, Direction.West, maxDistance, subMin, final, nodes, ref curNode, ref numChecked))
                        return nodes;
                }
                edgeLength = edgeLength + 1;
                numChecked = 0;
                while (numChecked < edgeLength)
                {
                    if (!TryGetNextNode(position, condition, Direction.North, maxDistance, subMin, final, nodes, ref curNode, ref numChecked))
                        return nodes;
                }
                numChecked = 0;
                while (numChecked < edgeLength)
                {
                    if (!TryGetNextNode(position, condition, Direction.East, maxDistance, subMin, final, nodes, ref curNode, ref numChecked))
                        return nodes;
                }
                i++;
            }
            return nodes;
        }

        public AvoidanceNode GetNearestNode(Vector3 position, Func<AvoidanceNode, bool> condition, float maxDistance = 30f, float minDistance = -1f, bool includeThisNode = true)
        {
            var nodes = new List<AvoidanceNode>();

            maxDistance = maxDistance >= 0 ? maxDistance + 1 : 30f;
            minDistance = minDistance >= 0 ? minDistance : -1f;

            if (position == Vector3.Zero)
                return null;

            var startNode = GetNearestNode(position);
            if (startNode == null)
                return null;

            var gridRadiusMax = Math.Round(maxDistance / 2.5, 0, MidpointRounding.AwayFromZero);
            var gridRadiusMin = Math.Round(minDistance / 2.5, 0, MidpointRounding.AwayFromZero);

            if (includeThisNode && condition(startNode))
                nodes.Add(startNode);

            var i = 1;
            var curNode = startNode;

            while (i <= gridRadiusMax + 1)
            {
                var edgeLength = i * 2 - 1;
                var final = i >= gridRadiusMax - 1;
                var isFinalEdge = Math.Abs(i - (gridRadiusMax + 1)) < double.Epsilon;
                var subMin = i <= gridRadiusMin;
                var numChecked = 0;
                while (numChecked < edgeLength)
                {
                    if (TryGetNextNode(position, condition, Direction.South, maxDistance, subMin, final, nodes, ref curNode, ref numChecked))
                        return nodes.First();
                }
                if (isFinalEdge) return nodes.FirstOrDefault();
                numChecked = 0;
                while (numChecked < edgeLength)
                {
                    if (TryGetNextNode(position, condition, Direction.West, maxDistance, subMin, final, nodes, ref curNode, ref numChecked))
                        return nodes.First();
                }
                edgeLength = edgeLength + 1;
                numChecked = 0;
                while (numChecked < edgeLength)
                {
                    if (TryGetNextNode(position, condition, Direction.North, maxDistance, subMin, final, nodes, ref curNode, ref numChecked))
                        return nodes.First();
                }
                numChecked = 0;
                while (numChecked < edgeLength)
                {
                    if (TryGetNextNode(position, condition, Direction.East, maxDistance, subMin, final, nodes, ref curNode, ref numChecked))
                        return nodes.First();
                }
                i++;
            }
            return nodes.FirstOrDefault();
        }

        private static bool TryGetNextNode(Vector3 position, Func<AvoidanceNode, bool> condition, Direction direction, float maxDistance, bool lessThanMinimumRange, bool isFinalCycle, List<AvoidanceNode> nodes, ref AvoidanceNode currentNode, ref int numCheckedOnEdge)
        {
            currentNode = _currentGrid.GetNodeInDirection(currentNode, direction);
            if (currentNode == null || !condition(currentNode))
                return false;

            if (!lessThanMinimumRange && (!isFinalCycle || position.Distance(currentNode.NavigableCenter) < maxDistance))
                nodes.Add(currentNode);

            numCheckedOnEdge++;
            return true;
        }

        public bool IsInKiteDirection(Vector3 position, double degreesDifferenceAllowed)
        {
            return MathUtil.GetRelativeAngularVariance(Core.Player.Position, Core.PlayerHistory.Centroid, position) <= degreesDifferenceAllowed;
        }

        public bool IsInPlayerFacingDirection(Vector3 position, double degreesDifferenceAllowed)
        {
            var pointInFront = MathEx.GetPointAt(Core.Player.Position, 10f, Core.Player.Rotation);
            return MathUtil.GetRelativeAngularVariance(Core.Player.Position, pointInFront, position) <= degreesDifferenceAllowed;
        }

        public bool IsStandingInFlags(params AvoidanceFlags[] flags)
        {
            return IsLocationInFlags(ZetaDia.Me.Position, flags);
        }

        public bool IsLocationInFlags(Vector3 location, params AvoidanceFlags[] flags)
        {
            var nearest = GetNearestNode(location);
            if (nearest == null)
                return false;

            var nodes = new List<AvoidanceNode> { nearest };
            nodes.AddRange(nearest.AdjacentNodes);
            return nodes.Any(n => n != null && flags.Any(f => n.AvoidanceFlags.HasFlag(f)));
        }

        //public HashSet<AvoidanceFlags> GetAvoidanceFlags(Vector3 location)
        //{
        //    var flags = new HashSet<AvoidanceFlags>();
        //    var nearest = GetNearestNode(location);
        //    if (nearest == null)
        //        return flags;

        //    var nodes = new HashSet<AvoidanceNode>(nearest.AdjacentNodes) { nearest };
        //    foreach (var flag in nodes.SelectMany(node => Flags.Where(flag => node.AvoidanceFlags.HasFlag(flag))))
        //    {
        //        flags.Add(flag);
        //    }
        //    return flags;
        //}

        public bool IsPathingOverFlags(params AvoidanceFlags[] flags)
        {
            if (PlayerMover.NavigationProvider == null || PlayerMover.NavigationProvider.CurrentPath == null)
                return false;

            var playerPosition = ZetaDia.Me.Position;
            var currentPath = PlayerMover.NavigationProvider.CurrentPath.TakeWhile(p => p.Distance(playerPosition) < GridEnricher.MaxDistance);
            if (!currentPath.Any())
                return false;

            var overFlags = currentPath.Any(p => Core.Avoidance.Grid.IsIntersectedByFlags(ZetaDia.Me.Position, p, flags));
            return overFlags;
        }

        public AvoidanceNode GetNodeAt(int x, int y)
        {
            //return GetNearestNode(new GridPoint(x, y));

            if (!IsValidNodePosition(x, y))
                return default(AvoidanceNode);

            return InnerGrid[x, y];
        }

        public bool CanStandAt(Vector3 position)
        {
            var node = GetNearestNode(position);
            return node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk) && node.IsWalkable;
        }
    }
}