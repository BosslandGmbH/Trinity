using System;
using System.Collections.Generic;
using System.Linq;
using Zeta.Common;
using Zeta.Game.Internals.SNO;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public interface IGroupNode : INode
    {
        int NavigableCellCount { get; }
        bool HasEnoughNavigableCells { get; }
        List<IDetailNode> Nodes { get; }
        bool IsKnown { get; set; }
        bool IsVisited { get; set; }
        int UnvisitedWeight { get; }
        bool IsIgnored { get; set; }
        byte AStarValue { get; set; }

        List<IGroupNode> GetNeighbors(int distance, bool includeSelf = false);

        IDetailNode NavigableCenterNode { get; set; }
    }

    public class ExplorationNode : INode, IEquatable<ExplorationNode>, IGroupNode
    {
        private bool _isIgnored;
        private readonly float _boxSize;
        private readonly float _boxTolerance;
        private readonly WorldScene _scene;

        public IDetailNode NavigableCenterNode { get; set; }
        public Vector3 NavigableCenter { get { return NavigableCenterNode != null ? NavigableCenterNode.NavigableCenter : Center.ToVector3(); } }
        public Vector2 NavigableCenter2D { get { return NavigableCenterNode != null ? NavigableCenterNode.NavigableCenter2D : Center; } }
        public Vector2 Center { get; private set; }
        public Vector2 TopLeft { get; private set; }
        public Vector2 BottomLeft { get; private set; }
        public Vector2 TopRight { get; private set; }
        public Vector2 BottomRight { get; set; }
        public int NavigableCellCount { get; private set; }

        //public float FillPercentage { get; private set; }
        public bool HasEnoughNavigableCells { get; private set; }

        public List<IDetailNode> Nodes { get; private set; }
        public WorldScene Scene { get { return _scene; } }
        public bool IsKnown { get; set; }
        public bool IsVisited { get; set; }
        public NodeFlags NodeFlags { get; set; }

        //public uint CustomFlags { get; set; }
        public GridPoint GridPoint { get; set; }

        public int LevelAreaId
        {
            get { return _scene.LevelAreaId; }
        }

        public float Distance2DSqr
        {
            get { return NavigableCenter2D.DistanceSqr(AdvDia.MyPosition.ToVector2()); }
        }

        public float Distance
        {
            get { return NavigableCenter.DistanceSqr(AdvDia.MyPosition); }
        }

        public int DynamicWorldId
        {
            get { return _scene.DynamicWorldId; }
        }

        public bool IsInNode(Vector3 position)
        {
            return position.X >= TopLeft.X && position.X <= BottomRight.X && position.Y >= TopLeft.Y && position.Y <= BottomRight.Y;
        }

        public bool IsBlacklisted { get; private set; }

        public ExplorationNode(Vector2 center, float boxSize, float boxTolerance, WorldScene scene)
        {
            _boxSize = boxSize;
            _boxTolerance = boxTolerance;
            _scene = scene;

            var halfSize = (float)boxSize / 2;
            //_maxCellsCount = _boxSize / 2.5 / 2;

            Center = center;
            TopLeft = Center + new Vector2(-(halfSize), -(halfSize));
            BottomLeft = Center + new Vector2(-(halfSize), halfSize);
            TopRight = Center + new Vector2(halfSize, -(halfSize));
            BottomRight = Center + new Vector2(halfSize, halfSize);
            NavigableCenterNode = null;
            IsBlacklisted = Scene.BlacklistedPositions.Any(IsInNode);
            Calculate(this);
        }

        public bool IsNextTo(ExplorationNode other)
        {
            return Math.Abs(Center.Distance(other.Center) - _boxSize) < 1;
        }

        private static void Calculate(ExplorationNode instance)
        {
            var navBoxSize = ExplorationData.NavigationNodeBoxSize;
            var searchBeginning = (float)navBoxSize / 2;
            var cellCount = instance._boxSize / navBoxSize;
            var maxCellsCount = cellCount * cellCount;

            instance.Nodes = new List<IDetailNode>();

            var walkableNodes = new List<NavigationNode>();
            var walkableExcludingNearWall = new List<NavigationNode>();

            for (var x = instance.TopLeft.X + searchBeginning; x <= instance.BottomRight.X; x = x + navBoxSize)
            {
                for (var y = instance.TopLeft.Y + searchBeginning; y <= instance.BottomRight.Y; y = y + navBoxSize)
                {
                    var cell = instance._scene.Cells.FirstOrDefault(c => c.IsInCell(x, y));
                    //var z = AdvDia.MainGridProvider.GetHeight(new Vector2(x,y));
                    if (cell != null)
                    {
                        var navNode = new NavigationNode(new Vector3(x, y, cell.Z), navBoxSize, instance, cell);

                        instance.Nodes.Add(navNode);

                        var isOnEdge = instance.Scene.Min.X == cell.MinX || instance.Scene.Min.Y == cell.MinY ||
                                       instance.Scene.Max.X == cell.MaxX || instance.Scene.Max.Y == cell.MaxY;

                        if (isOnEdge)
                        {
                            instance.IsEdgeNode = true;
                        }

                        if (cell.NavCellFlags.HasFlag(NavCellFlags.AllowWalk))
                        {
                            if (isOnEdge)
                            {
                                instance.IsConnectionNode = true;
                            }

                            walkableNodes.Add(navNode);
                            if (!navNode.NodeFlags.HasFlag(NodeFlags.NearWall))
                            {
                                walkableExcludingNearWall.Add(navNode);
                            }
                        }
                    }
                }
            }

            instance.NavigableCellCount = walkableExcludingNearWall.Count;
            //instance.FillPercentage = instance.NavigableCellCount / (float)maxCellsCount;
            instance.FillPercentage = instance.NavigableCellCount / (float)maxCellsCount;
            instance.HasEnoughNavigableCells = instance.FillPercentage >= instance._boxTolerance;
            if (walkableExcludingNearWall.Count > 0)
            {
                instance.NavigableCenterNode = walkableExcludingNearWall.OrderBy(ncp => ncp.NavigableCenter.DistanceSqr(instance.Center.ToVector3())).First();
            }
        }

        public float FillPercentage { get; set; }

        public bool IsConnectionNode { get; set; }

        public bool IsEdgeNode { get; set; }

        public List<IGroupNode> GetNeighbors(int distance, bool includeSelf = false)
        {
            var neighbors = ExplorationGrid.Instance.GetNeighbors(this, distance).Cast<IGroupNode>().ToList();

            if (includeSelf)
            {
                neighbors.Add(this);
            }
            return neighbors;
        }

        public int UnvisitedWeight
        {
            get
            {
                return GetNeighbors(1, true).Count(n => n.HasEnoughNavigableCells && !n.IsVisited);
            }
        }

        public bool IsIgnored
        {
            get { return _isIgnored; }
            set
            {
                if (_isIgnored) return;
                _isIgnored = value;
            }
        }

        public bool IsCurrentDestination { get; set; }

        public bool Equals(ExplorationNode other)
        {
            return LevelAreaId == other.LevelAreaId && Center == other.Center;
        }

        public override int GetHashCode()
        {
            return Center.GetHashCode();
        }

        public byte AStarValue { get; set; }
        public int FailedNavigationAttempts { get; set; }
        public bool Priority { get; set; }
    }
}