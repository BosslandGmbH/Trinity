using System;
using System.Collections.Generic;
using System.Linq;
using Adventurer.Game.Exploration;
using Trinity.DbProvider;
using Trinity.Framework.Avoidance.Structures;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance
{
    public sealed class AvoidanceGrid : Grid<AvoidanceNode>
    {
        static AvoidanceGrid()
        {
            Flags = Enum.GetValues(typeof(AvoidanceFlags)).Cast<AvoidanceFlags>().ToList();
        }

        private static List<AvoidanceFlags> Flags { get; set; }

        private const int Bounds = 2500;        

        public override float BoxSize => 2.5f;
        public override int GridBounds => Bounds;
        public static AvoidanceGrid Instance => GetWorldGrid();

        private static AvoidanceGrid _currentGrid;

        public static AvoidanceGrid GetWorldGrid()
        {
            if (_currentGrid == null)
            {
                _currentGrid = new AvoidanceGrid();
                return _currentGrid;
            }

            if (_currentGrid == null || ZetaDia.WorldId != _currentGrid.WorldDynamicId)
            {
                _currentGrid = new AvoidanceGrid();
            }

            if (DateTime.UtcNow.Subtract(_currentGrid.Created).TotalSeconds > 10 && _currentGrid.NearestNode == null)
            {
                _currentGrid = new AvoidanceGrid();
            }

            return _currentGrid;
        }

        public override bool CanRayCast(Vector3 @from, Vector3 to)
        {
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).All(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowProjectile));
        }

        public override bool CanRayWalk(Vector3 @from, Vector3 to)
        {
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).All(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk) && node.IsWalkable && !node.NodeFlags.HasFlag(NodeFlags.NearWall));
        }

        public bool IsIntersectedByFlags(Vector3 @from, Vector3 to, params AvoidanceFlags[] flags)
        {
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).Any(node => node != null && flags != null && flags.Any(f => node.AvoidanceFlags.HasFlag(f)));
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
            var nodes = newNodes.ExplorationNodes.SelectMany(n => n.Nodes, (p, c) => new AvoidanceNode(c)).ToList();

            UpdateInnerGrid(nodes);

            foreach (var node in nodes)
            {
                if(GetNeighbors(node).Any(n => (n.NodeFlags & NodeFlags.AllowWalk) == 0))
                {
                    node.NodeFlags |= NodeFlags.NearWall;
                }
            }

            Logger.LogVerbose("Avoidance Grid updated");
        }

        public void FlagNodes(IEnumerable<AvoidanceNode> nodes, AvoidanceFlags flags, int weightModification = 0)
        {
            foreach (var node in nodes.Where(node => node != null && node.AvoidanceFlags.HasFlag(AvoidanceFlags.AllowWalk)))
            {
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
                    if(!TryGetNextNode(position, condition, Direction.East, maxDistance, subMin, final, nodes, ref curNode, ref numChecked))
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
            return MathUtil.GetRelativeAngularVariance(TrinityPlugin.Player.Position, Core.PlayerHistory.Centroid, position) <= degreesDifferenceAllowed;
        }

        public bool IsInPlayerFacingDirection(Vector3 position, double degreesDifferenceAllowed)
        {
            var pointInFront = MathEx.GetPointAt(TrinityPlugin.Player.Position, 10f, TrinityPlugin.Player.Rotation);
            return MathUtil.GetRelativeAngularVariance(TrinityPlugin.Player.Position, pointInFront, position) <= degreesDifferenceAllowed;
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

            var nodes = new List<AvoidanceNode> {nearest};
            nodes.AddRange(nearest.AdjacentNodes);
            return nodes.Any(n => n != null && flags.Any(f => n.AvoidanceFlags.HasFlag(f)));
        }

        public HashSet<AvoidanceFlags> GetAvoidanceFlags(Vector3 location)
        {
            var flags = new HashSet<AvoidanceFlags>();
            var nearest = GetNearestNode(location);
            if (nearest == null)
                return flags;
            
            var nodes = new HashSet<AvoidanceNode>(nearest.AdjacentNodes) { nearest };            
            foreach (var flag in nodes.SelectMany(node => Flags.Where(flag => node.AvoidanceFlags.HasFlag(flag))))
            {
                flags.Add(flag);
            }            
            return flags;
        }

        public bool IsPathingOverFlags(params AvoidanceFlags[] flags)
        {
            if (PlayerMover.NavigationProvider == null || PlayerMover.NavigationProvider.CurrentPath == null)
                return false;

            var playerPosition = ZetaDia.Me.Position;
            var currentPath = PlayerMover.NavigationProvider.CurrentPath.TakeWhile(p => p.Distance(playerPosition) < AvoidanceManager.MaxDistance);
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

