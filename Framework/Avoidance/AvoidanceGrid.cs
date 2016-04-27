using System;
using System.Collections.Generic;
using System.Linq;
using Adventurer.Game.Exploration;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance
{
    public sealed class AvoidanceGrid : Grid<AvoidanceNode>
    {
        private static AvoidanceGrid _currentGrid;

        public static AvoidanceGrid GetWorldGrid()
        {
            if (_currentGrid == null)
            {
                _currentGrid = new AvoidanceGrid();
                return _currentGrid;
            }

            if (_currentGrid == null || _currentGrid == null || ZetaDia.WorldId != _currentGrid.WorldDynamicId)
            {
                _currentGrid = new AvoidanceGrid();
            }

            if (DateTime.UtcNow.Subtract(_currentGrid.Created).TotalSeconds > 10 && _currentGrid.NearestNode == null)
            {
                _currentGrid = new AvoidanceGrid();
            }

            return _currentGrid;
        }

        public static AvoidanceGrid Instance
        {
            get { return GetWorldGrid(); }
        }

        private const float NodeBoxSize = 2.5f;
        private const int Bounds = 2500;

        public override bool CanRayCast(Vector3 @from, Vector3 to)
        {
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).All(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowProjectile));
        }

        public override bool CanRayWalk(Vector3 @from, Vector3 to)
        {
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).All(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk));
        }

        public bool IsIntersectedByFlags(Vector3 @from, Vector3 to, params AvoidanceFlags[] flags)
        {
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).Any(node => node != null && flags != null && flags.Any(f => node.AvoidanceFlags.HasFlag(f)));
        }

        //public IEnumerable<AvoidanceNode> GetProjectedRectangle(Vector3 origin, float distance, float rotationRadians, float xSize, float ySize)
        //{
        //    var projectedCenter = MathEx.GetPointAt(origin, distance, rotationRadians);
        //    return GetRectangle(projectedCenter, xSize, ySize).Where(node => node != null && node.NodeFlags.HasFlag(NodeFlags.AllowWalk));
        //}

        //public IEnumerable<AvoidanceNode> GetRectangle(Vector3 center, float xSize, float ySize)
        //{
        //    var x = (int)center.X;
        //    var y = (int)center.Y;

        //    var xGrid = ToGridDistance(xSize);
        //    var yGrid = ToGridDistance(xSize);

        //    int lConerX = x - 4, lConerY = y - 4; //coords of top-left conner

        //    for (int i = lConerX; i < lConerX + xGrid; i++)
        //    {
        //        for (int j = lConerY; j < lConerY + yGrid; j++)
        //        {
        //            if(IsValidNodePosition(i, j))
        //                yield return InnerGrid[i, j];
        //        }
        //    }
        //}

        internal IEnumerable<AvoidanceNode> GetRayLineAsNodes(Vector3 from, Vector3 to)
        {
            return GetRayLine(from, to).Select(point => InnerGrid[point.X, point.Y]).Where(n => n != null);
        }

        public override void Reset()
        {
            
        }

        public override float BoxSize
        {
            get { return NodeBoxSize; }             
        }

        public override int GridBounds
        {
            get { return Bounds; }
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

            var gridRadiusMax = Math.Round(maxDistance / NodeBoxSize, 0, MidpointRounding.AwayFromZero);
            var gridRadiusMin = Math.Round(minDistance / NodeBoxSize, 0, MidpointRounding.AwayFromZero);

            if (includeThisNode && condition(startNode))
                nodes.Add(startNode);

            var i = 1;
            var curNode = startNode;

            while (i <= gridRadiusMax + 1)
            {
                var edgeLength = i * 2 - 1;
                var final = i >= gridRadiusMax - 1;
                var isFinalEdge = i == gridRadiusMax + 1;
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


        //static TrinityGrid()
        //{
        //    //https://msdn.microsoft.com/en-us/library/system.runtime.gcsettings.largeobjectheapcompactionmode.aspx
        //    // TrinityNode[,] is on the large object heap.
        //    GameEvents.OnWorldChanged += (sender, args) => ResetAll();
        //}

        //public const float NodeBoxSize = 2.5f;
        //private const int GRID_BOUNDS = 2500;
        //private static readonly ConcurrentDictionary<int, Lazy<TrinityGrid>> WorldGrids = new ConcurrentDictionary<int, Lazy<TrinityGrid>>();
        //public static float HighestNodeWeight;
        //public static int MaxWeight { get; set; }
        //public static int MaxRange = 100;

        //private static TrinityGrid _currentGrid;
        //public static TrinityGrid CurrentGrid
        //{
        //    get { return _currentGrid ?? (_currentGrid = GetWorldGrid(ZetaDia.WorldId)); }
        //}

        //public override float BoxSize
        //{
        //    get { return NodeBoxSize; }
        //}

        //public override int GridBounds
        //{
        //    get { return GRID_BOUNDS; }
        //}

        //public static TrinityGrid GetWorldGrid(int worldDynamicId)
        //{
        //    return WorldGrids.GetOrAdd(worldDynamicId, new Lazy<TrinityGrid>(() => new TrinityGrid())).Value;
        //}

        //public static void ResetAll()
        //{            
        //    WorldGrids.Clear();
        //    _currentGrid = null;
        //}

        //public static List<TrinityNode> CurrentNodes { get; set; }

        //public override bool CanRayCast(Vector3 from, Vector3 to)
        //{
        //    return GetRayLine(from, to).Any(point =>
        //    {
        //        var node = CurrentGrid.GetNearestNode(point);
        //        return node != null && node.TrinityNodeFlags.HasFlag(TrinityNodeFlags.AllowProjectile);
        //    });
        //}

        //public override bool CanRayWalk(Vector3 from, Vector3 to)
        //{
        //    return GetRayLine(from, to).Any(point =>
        //    {
        //        var node = CurrentGrid.GetNearestNode(point);
        //        return node != null && node.TrinityNodeFlags.HasFlag(TrinityNodeFlags.AllowWalk);
        //    });
        //}

        //private IEnumerable<GridPoint> GetRayLine(Vector3 from, Vector3 to, float limitDistance = -1f)
        //{
        //    if (limitDistance > 0)
        //        to = MathEx.CalculatePointFrom(from, to, limitDistance);

        //    var gridFrom = ToGridPoint(from);
        //    var gridTo = ToGridPoint(to);

        //    return Bresenham.GetPointsOnLine(gridFrom, gridTo);
        //}

        //public static bool IsValidNodePosition(int x, int y)
        //{
        //    return x > 0 && x < CurrentGrid.GridMaxX && y > 0 && y < CurrentGrid.GridMaxY;
        //}

        //public static List<TrinityNode> GetNodesInDirection(float angleDegrees, float distanceYards, TrinityNode origin = null)
        //{
        //    var originPos = origin != null ? origin.Position : Trinity.Player.Position;
        //    var projectedPoint = MathEx.GetPointAt(originPos, distanceYards, MathUtil.ToRadians(angleDegrees));
        //    var nodes = GetNodesBetween(GetNearestNode(projectedPoint));
        //    return nodes;
        //}

        //public static List<TrinityNode> GetNodesInDirection(Direction direction, float distanceYards, TrinityNode origin = null)
        //{
        //    var angleDegrees = MathUtil.GetHeadingFromCardinal(direction);
        //    var originPos = origin != null ? origin.Position : Trinity.Player.Position;
        //    var projectedPoint = MathEx.GetPointAt(originPos, distanceYards, MathUtil.ToRadians(angleDegrees));
        //    var nodes = GetNodesBetween(GetNearestNode(projectedPoint), origin);
        //    return nodes;
        //}

        //internal static TrinityNode GetNearestNode(Vector3 position)
        //{
        //    return CurrentGrid.GetNearestNodeToPosition(position);
        //}

        //internal static bool IsNodeInFlag(TrinityNodeFlags trinityNodeFlags, TrinityNode trinityNode)
        //{
        //    return trinityNode != null && trinityNode.TrinityNodeFlags.HasFlag(trinityNodeFlags);
        //}

        //internal static bool IsInLineOfSight(Vector3 to)
        //{
        //    var from = Trinity.Player.Position;
        //    var offsetToNode = GetNearestNodeInPlayerDirection(to);
        //    var nodes = GetNodesBetween(from, offsetToNode != null ? offsetToNode.Position : to);

        //    if (nodes.Count <= 2)
        //    {
        //        if (nodes.Select(node => nodes.First()).Any(node => node.TrinityNodeFlags.HasFlag(TrinityNodeFlags.AllowWalk) && !node.ObstacleIds.Any()))
        //        {
        //            return true;
        //        }
        //    }

        //    for (var i = 0; i < nodes.Count; i++)
        //    {
        //        if (i == 0 || i == nodes.Count - 1)
        //            continue;

        //        var node = nodes[i];

        //        if (node == null)
        //            continue;

        //        if (!node.TrinityNodeFlags.HasFlag(TrinityNodeFlags.AllowWalk))
        //            return false;

        //        if (node.TrinityNodeFlags.HasFlag(TrinityNodeFlags.NavigationBlocking))
        //            return false;
        //    }

        //    return true;
        //}

        //public static TrinityNode GetNearestNodeInPlayerDirection(Vector3 fromPosition)
        //{
        //    var directionDegree = MathUtil.FindDirectionDegree(fromPosition, Trinity.Player.Position);
        //    return GetNearestNodeInDirection(fromPosition, directionDegree);
        //}

        //public static TrinityNode GetNearestNodeInDirection(Vector3 fromPosition, float headingDegrees)
        //{
        //    return GetNearestNodeInDirection(fromPosition.X, fromPosition.Y, headingDegrees);
        //}

        //public static TrinityNode GetNearestNodeInDirection(float x, float y, float headingDegrees)
        //{
        //    var offsetPoint = MathEx.GetPointAt(new Vector3(x, y, 0), NodeBoxSize, MathUtil.ToRadians(headingDegrees));
        //    return CurrentGrid.GetNearestNodeToPosition(offsetPoint);
        //}

        //public bool IsPositionIntersectedByFlags(Vector3 destination, AvoidanceFlags flags, Vector3 originPos = new Vector3())
        //{
        //    originPos = originPos != Vector3.Zero ? originPos : Trinity.Player.Position;

        //    if (destination == Vector3.Zero)
        //        return false;

        //    var nodes = GetRayLineAsNodes(destination, originPos).Where(n => n.AvoidanceFlags.HasFlag(flags));

        //    return nodes.Any();
        //}

        //public static bool IsInCriticalAvoidance(Vector3 position)
        //{
        //    return IsInCriticalAvoidance(GetNearestNode(position));
        //}

        //private static bool IsInCriticalAvoidance(TrinityNode node)
        //{
        //    return node.TrinityNodeFlags.HasFlag(TrinityNodeFlags.CriticalAvoidance);
        //}

        //public static List<TrinityNode> GetNodesBetween(TrinityNode destination, TrinityNode origin = null, int limit = 40)
        //{
        //    var originPos = origin != null ? origin.Position : Trinity.Player.Position;

        //    if (destination == null)
        //        return new List<TrinityNode>();

        //    return GetNodesBetween(destination.Position, originPos, limit);
        //}

        //public List<AvoidanceNode> GetNodesBetween(Vector3 destinationPos, Vector3 originPos, int limit = 40, Func<AvoidanceNode, bool> untilCondition = null)
        //{
        //    var nodes = new List<AvoidanceNode>();
        //    var currentPos = originPos;
        //    var i = 0;

        //    while (i < limit && !(Math.Abs(currentPos.X - destinationPos.X) < 1 && Math.Abs(currentPos.Y - destinationPos.Y) < 1))
        //    {
        //        currentPos = Bresenham.GetNextLinePoint(currentPos, destinationPos);

        //        var currentNode = IsValidNodePosition((int)currentPos.X, (int)currentPos.Y) ? InnerGrid[(int)currentPos.X, (int)currentPos.Y] : GetNearestNode(currentPos);

        //        i++;

        //        if (currentNode == null)
        //            continue;

        //        nodes.Add(currentNode);

        //        if (untilCondition != null && untilCondition(currentNode))
        //            return nodes;
        //    }

        //    return nodes;
        //}

        //internal static bool IsNodeIntersectedByFlags(TrinityNodeFlags trinityNodeFlags, TrinityNode trinityNode, TrinityNode origin = null, int distanceLimit = 40)
        //{
        //    if (trinityNode == null)
        //        return false;

        //    if (origin == null)
        //        origin = GetNearestNode(Trinity.Player.Position);

        //    return GetNodesBetween(trinityNode.NavigableCenter, origin.NavigableCenter, distanceLimit, n => n != null && n.TrinityNodeFlags.HasFlag(trinityNodeFlags)) != null;
        //}

        //internal static bool IsStandingInFlag(TrinityNodeFlags trinityNodeFlags, Vector3 targetPosition = new Vector3())
        //{
        //    if (targetPosition == Vector3.Zero)
        //        targetPosition = Trinity.Player.Position;

        //    return CurrentGrid.GetNearestNodeToPosition(targetPosition).TrinityNodeFlags.HasFlag(trinityNodeFlags);
        //}

        //public static bool IsStandingInFlag(TrinityNodeFlags trinityNodeFlags, TrinityNode node)
        //{
        //    return node.TrinityNodeFlags.HasFlag(trinityNodeFlags);
        //}

        //internal static IEnumerable<TrinityNode> GetAdjacentNodes(TrinityNode node)
        //{
        //    return CurrentGrid.GetNeighbors(node);
        //}

        //internal static IEnumerable<TrinityNode> GetAdjacentNodes(Vector3 position)
        //{
        //    return CurrentGrid.GetNeighbors(GetNearestNode(position));
        //}

        //public static List<TrinityNode> FindNodes(IActor trinityObject, Func<TrinityNode, bool> condition = null, float maxDistance = 40f, float minDistance = 0f)
        //{
        //    //return CurrentGrid.FindNodes(trinityObject.NearestNode, CurrentGrid.ToGridDistance(minDistance), CurrentGrid.ToGridDistance(maxDistance), condition);
        //    return FindNodes(trinityObject.Position, condition, maxDistance, minDistance);
        //}

        ////public static List<TrinityNode> FindNodes(Vector3 position, Func<TrinityNode, bool> condition = null, float maxDistance = 40f, float minDistance = 0f)
        ////{
        ////    return CurrentGrid.FindNodes(CurrentGrid.GetNearestNodeToPosition(position), CurrentGrid.ToGridDistance(minDistance), CurrentGrid.ToGridDistance(maxDistance), condition);
        ////}

        //public static List<TrinityNode> FindNodes(Vector3 position, Func<TrinityNode, bool> condition, float maxDistance = 30f, float minDistance = -1f, bool includeThisNode = true)
        //{
        //    var nodes = new List<TrinityNode>();

        //    maxDistance = maxDistance >= 0 ? maxDistance + 1 : 30f;
        //    minDistance = minDistance >= 0 ? minDistance : -1f;

        //    if (position == Vector3.Zero)
        //        return nodes;

        //    var startNode = GetNearestNode(position);
        //    if (startNode == null)
        //        return nodes;

        //    var gridRadiusMax = Math.Round(maxDistance / NodeBoxSize, 0, MidpointRounding.AwayFromZero);
        //    var gridRadiusMin = Math.Round(minDistance / NodeBoxSize, 0, MidpointRounding.AwayFromZero);

        //    if (includeThisNode && condition(startNode))
        //        nodes.Add(startNode);

        //    var i = 1;
        //    var currentNode = startNode;

        //    while (i <= gridRadiusMax + 1) // +1 is for the additional side to be checked at the end
        //    {
        //        var edgeLength = i * 2 - 1;

        //        // Check distance on last cycle of edges, else it would always be a square
        //        var isFinalCycle = i >= gridRadiusMax - 1;

        //        // Allow an additional side to be checked at the end
        //        var isFinalEdge = i == gridRadiusMax + 1;
        //        var lessThanMinimumRange = i <= gridRadiusMin;

        //        var numCheckedOnEdge = 0;
        //        while (numCheckedOnEdge < edgeLength)
        //        {
        //            currentNode = _currentGrid.GetNodeInDirection(currentNode, Direction.South);
        //            if (currentNode != null && condition(currentNode))
        //            {
        //                // on last cycle each point is checked for distance to center, shaping the square into a circle
        //                if (!lessThanMinimumRange && (!isFinalCycle || position.Distance(currentNode.Position) < maxDistance))
        //                    nodes.Add(currentNode);
        //            }
        //            numCheckedOnEdge++;
        //        }

        //        if (isFinalEdge)
        //            return nodes;

        //        numCheckedOnEdge = 0;
        //        while (numCheckedOnEdge < edgeLength)
        //        {
        //            currentNode = _currentGrid.GetNodeInDirection(currentNode, Direction.West);
        //            if (currentNode != null && condition(currentNode))
        //            {
        //                if (!lessThanMinimumRange && (!isFinalCycle || position.Distance(currentNode.Position) < maxDistance))
        //                    nodes.Add(currentNode);
        //            }
        //            numCheckedOnEdge++;
        //        }

        //        // 3rd and 4th edges need to run +1 length to make a spiral.
        //        edgeLength = edgeLength + 1;

        //        numCheckedOnEdge = 0;
        //        while (numCheckedOnEdge < edgeLength)
        //        {
        //            currentNode = _currentGrid.GetNodeInDirection(currentNode, Direction.North);
        //            if (currentNode != null && condition(currentNode))
        //            {
        //                if (!lessThanMinimumRange && (!isFinalCycle || position.Distance(currentNode.Position) < maxDistance))
        //                    nodes.Add(currentNode);
        //            }
        //            numCheckedOnEdge++;
        //        }

        //        numCheckedOnEdge = 0;
        //        while (numCheckedOnEdge < edgeLength)
        //        {
        //            currentNode = _currentGrid.GetNodeInDirection(currentNode, Direction.East);
        //            if (currentNode != null && condition(currentNode))
        //            {
        //                if (!lessThanMinimumRange && (!isFinalCycle || position.Distance(currentNode.Position) < maxDistance))
        //                    nodes.Add(currentNode);
        //            }
        //            numCheckedOnEdge++;
        //        }

        //        i++;
        //    }

        //    return nodes;         
        //}


        //public static void FlagNodesAndAdjacent(IEnumerable<TrinityNode> nodes, TrinityNodeFlags flags)
        //{
        //    foreach (var node in nodes)
        //    {
        //        if (node == null || !node.TrinityNodeFlags.HasFlag(TrinityNodeFlags.AllowWalk))
        //            continue;

        //        node.AddNodeFlags(flags);

        //        foreach (var adjacentNode in GetAdjacentNodes(node))
        //        {
        //            if (adjacentNode == null)
        //                continue;

        //            adjacentNode.AddNodeFlags(flags);
        //        }
        //    }
        //}

        //public static bool UpdateFlags()
        //{
        //    using (new PerformanceLogger("UpdateFlags"))
        //    {

        //        var nodePool = CurrentGrid.GetNodesInRadius(Trinity.Player.Position, MaxRange);

        //        try
        //        {
        //            if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld)
        //                return false;

        //            if (nodePool == null || !nodePool.Any())
        //                return false;

        //            nodePool.ForEach(n =>
        //            {
        //                if (n != null)
        //                    n.Reset();
        //            });

        //            //Telegraph.TelegraphedObjectHashes.Clear();
        //            HighestNodeWeight = 0;
        //            UpdateMonsterFlags();
        //            UpdateAvoidanceFlags();
        //            UpdateGlobeFlags();
        //            UpdateBacktrackFlags();
        //            UpdateNavigationFlags();
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Log("Exception in UpdateFlags {0}", ex);

        //            if (ex is CoroutineStoppedException)
        //                throw;
        //        }

        //        CurrentNodes = nodePool;
        //    }

        //    return false;
        //}

        //private static void UpdateGlobeFlags()
        //{
        //    //// Make nodes near health globes safer
        //    //foreach (var globe in TrinDia.Cache.Globes)
        //    //{
        //    //    if (globe.Type == ObjectType.HealthGlobe)
        //    //        continue;

        //    //    var nodes = FindNodes(globe, node => true, globe.Radius, 0);
        //    //    if (!nodes.Any())
        //    //        continue;

        //    //    foreach (var node in nodes)
        //    //    {
        //    //        node.Weight--;
        //    //        node.AddNodeFlags(TrinityNodeFlags.Health);
        //    //    }
        //    //}
        //}

        //private static void UpdateNavigationFlags()
        //{
        //    //// Make sure nodes by closed doors are flagged
        //    //foreach (var door in TrinDia.Cache.Doors)
        //    //{
        //    //    if (door.IsUsed)
        //    //        continue;

        //    //    var nearestNode = GetNearestNode(door.Position);
        //    //    if (nearestNode == null)
        //    //        continue;

        //    //    nearestNode.AddNodeFlags(TrinityNodeFlags.NavigationBlocking);

        //    //    var nodes = FindNodes(door, node => true, door.Radius, 0);
        //    //    if (!nodes.Any())
        //    //        continue;

        //    //    foreach (var node in nodes)
        //    //    {
        //    //        node.ObstacleIds.Add(door.RActorId);
        //    //        node.AddNodeFlags(TrinityNodeFlags.NavigationBlocking);
        //    //    }
        //    //}

        //    //// Flag barricades as navigation blocking
        //    //foreach (var barricade in TrinDia.Cache.Barricades)
        //    //{
        //    //    if (barricade.IsDestroyed)
        //    //        continue;

        //    //    var nearestNode = GetNearestNode(barricade.Position);
        //    //    if (nearestNode == null)
        //    //        continue;

        //    //    nearestNode.AddNodeFlags(TrinityNodeFlags.NavigationBlocking);

        //    //    var nodes = FindNodes(barricade, node => true, barricade.Radius, 0);
        //    //    if (!nodes.Any())
        //    //        continue;

        //    //    foreach (var node in nodes)
        //    //    {
        //    //        node.ObstacleIds.Add(barricade.RActorId);
        //    //        node.AddNodeFlags(TrinityNodeFlags.NavigationBlocking);
        //    //    }
        //    //}
        //}

        //private static void UpdateBacktrackFlags()
        //{
        //    //// Make the path we came from safer
        //    //foreach (var cachedPos in PositionHistory.Cache)
        //    //{
        //    //    if (cachedPos.WorldId != TrinDia.CurrentWorldSnoId)
        //    //        continue;

        //    //    var nearestNode = GetNearestNode(cachedPos.Position);
        //    //    if (nearestNode == null)
        //    //        continue;

        //    //    foreach (var node in nearestNode.AdjacentNodes)
        //    //    {
        //    //        if (node.TrinityNodeFlags.HasFlag(TrinityNodeFlags.Backtrack))
        //    //        {
        //    //            // Clear path when we are actively backtracking.
        //    //            if (TrinDia.Avoidance.IsAvoiding && node.Distance < 20f)
        //    //                node.RemoveNodeFlags(TrinityNodeFlags.Backtrack);

        //    //            continue;
        //    //        }

        //    //        node.Weight--;
        //    //        node.AddNodeFlags(TrinityNodeFlags.Backtrack);
        //    //    }

        //    //    nearestNode.Weight--;
        //    //    nearestNode.AddNodeFlags(TrinityNodeFlags.Backtrack);
        //    //}
        //}

        //private static void UpdateAvoidanceFlags()
        //{
        //    if (!Trinity.Settings.Advanced.UseExperimentalAvoidance)
        //        return;

        //    foreach (var avoidance in Core.Avoidance.Current)
        //    {
        //        Logger.Log("TrinityGrid UpdateAvoidanceFlags for {0} Parts={1} Actors={2}", 
        //            avoidance.Data.Name, avoidance.Data.Parts.Count, avoidance.Actors.Count);

        //        avoidance.Handler.UpdateNodes(CurrentGrid, avoidance);
        //    }

        //    //foreach (var avoidance in CacheData.Avoidances)
        //    //{
        //    //    var nearestNode = GetNearestNode(avoidance.Position);
        //    //    if (nearestNode == null)
        //    //        continue;

        //    //    if (AvoidanceManager.IsPlayerImmune(avoidance.AvoidanceType))
        //    //        continue;

        //    //    //if (avoidance.AvoidanceType == AvoidanceType.PoisonDeathCloud)
        //    //    //    Telegraph.TelegraphCircleAvoidance(avoidance, avoidance.Radius);

        //    //    //if (avoidance.AvoidanceType == AvoidanceType.PoisonEnchanted)
        //    //    //    Telegraph.TelegraphPoisonEnchanted(avoidance);

        //    //    //if (avoidance.AvoidanceType == AvoidanceType.Arcane)
        //    //    //    Telegraph.TelegraphArcane(avoidance);

        //    //    //if (avoidance.AvoidanceType == AvoidanceType.MorluMeteor)
        //    //    //    Telegraph.TelegraphCircleAvoidance(avoidance, avoidance.Radius);

        //    //    var nodes = FindNodes(avoidance, node => true, avoidance.Radius, 0);
        //    //    if (!nodes.Any())
        //    //        continue;

        //    //    //var isCritical = AvoidanceManager.IsCriticalAvoidance(avoidance);
        //    //    //if (isCritical && mainGridProvider != null)
        //    //    //{
        //    //    //    // This should help prevent DB Navigator from picking a path that goes through bad stuff
        //    //    //    mainGridProvider.AddCellWeightingObstacle(avoidance.ActorSnoId, avoidance.Radius);
        //    //    //}

        //    //    foreach (var node in nodes)
        //    //    {
        //    //        node.Weight++;
        //    //        node.AddNodeFlags(TrinityNodeFlags.Avoidance);
        //    //        node.AvoidanceIds.Add(avoidance.RActorGuid);

        //    //        //if (isCritical)
        //    //        //{
        //    //            node.AddNodeFlags(TrinityNodeFlags.CriticalAvoidance);
        //    //            node.Weight += 5;
        //    //        //}


        //    //        if (node.Weight > HighestNodeWeight)
        //    //            HighestNodeWeight = node.Weight;
        //    //    }
        //    //}
        //}

        //private static void UpdateMonsterFlags()
        //{
        //    foreach (var monster in Trinity.ObjectCache.Where(o => o.ActorType == ActorType.Monster))
        //    {
        //        //if (CombatManager.CurrentCombatSettings.IgnoreElites)
        //        //    continue;

        //        //if (monster.IsTelegraphingBeam)
        //        //    Telegraph.TelegraphBeamAvoidance(monster.Position, monster.Movement.GetHeadingDegreesFromMemory(), monster.TelegraphDistance);

        //        //if (monster.IsTelegraphingCircle)
        //        //    Telegraph.TelegraphCircleAvoidance(monster, monster.TelegraphDistance);

        //        //if (monster.MonsterAffixes.Contains(TrinityMonsterAffix.FireChains) && monster.MonsterQualityLevel == MonsterQuality.Minion)
        //        //    Telegraph.TelegraphFireChains(monster);

        //        var nearestNode = GetNearestNode(monster.Position);
        //        if (nearestNode == null)
        //            continue;

        //        nearestNode.AddNodeFlags(TrinityNodeFlags.NavigationBlocking);

        //        var nodes = FindNodes(monster, node => true, monster.Radius*0.8f, 0);
        //        if (!nodes.Any())
        //            continue;

        //        foreach (var node in nodes)
        //        {
        //            node.Weight++;
        //            node.AddNodeFlags(TrinityNodeFlags.Monster);
        //            node.MonsterIds.Add(monster.RActorGuid);

        //            if (node.Weight > HighestNodeWeight)
        //                HighestNodeWeight = node.Weight;
        //        }
        //    }
        //}

    }
}