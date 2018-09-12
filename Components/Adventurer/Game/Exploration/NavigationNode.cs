using System;
using Zeta.Common;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public interface IDetailNode : INode
    {
        IGroupNode ExplorationNode { get; }
        byte AStarValue { get; set; }
    }

    public class NavigationNode : INode, IEquatable<NavigationNode>, IDetailNode
    {
        private readonly WorldScene _scene;
        private readonly ExplorationNode _explorationNode;

        public Vector3 NavigableCenter { get; private set; }
        public Vector2 NavigableCenter2D { get; private set; }
        public Vector2 Center { get; private set; }
        public Vector2 TopLeft { get; private set; }
        public Vector2 BottomLeft { get; private set; }
        public Vector2 TopRight { get; private set; }
        public Vector2 BottomRight { get; private set; }

        public WorldScene Scene => _scene;
        public IGroupNode ExplorationNode => _explorationNode;

        //public bool IsVisited { get { return _explorationNode.IsVisited; } }
        //public bool IsIgnored { get { return _explorationNode.IsVisited; } }
        public int LevelAreaId => _scene.LevelAreaId;

        //public float Distance2DSqr { get { return NavigableCenter2D.DistanceSqr(AdvDia.MyPosition.ToVector2()); } }
        public int DynamicWorldId => _scene.DynamicWorldId;

        public NodeFlags NodeFlags { get; set; }

        //public uint CustomFlags { get; set; }
        public GridPoint GridPoint { get; set; }

        public NavigationNode(Vector3 center, float boxSize, ExplorationNode node, WorldSceneCell cell)
        {
            if (node != null)
            {
                _explorationNode = node;
                _scene = node.Scene;
            }

            if (cell != null)
            {
                NodeFlags = (NodeFlags)cell.NavCellFlags;
            }

            var halfSize = (float)boxSize / 2;
            Center = center.ToVector2();
            TopLeft = Center + new Vector2(-(halfSize), -(halfSize));
            BottomLeft = Center + new Vector2(-(halfSize), halfSize);
            TopRight = Center + new Vector2(halfSize, -(halfSize));
            BottomRight = Center + new Vector2(halfSize, halfSize);
            NavigableCenter = center;
            NavigableCenter2D = Center;
        }

        public bool Equals(NavigationNode other)
        {
            return LevelAreaId == other.LevelAreaId && Center == other.Center;
        }

        public override int GetHashCode()
        {
            return Center.GetHashCode();
        }

        public byte AStarValue { get; set; }
    }
}