using System;
using System.Collections.Generic;
using Adventurer.Game.Exploration;
using Trinity.Framework.Grid;
using Zeta.Common;

namespace Trinity.Framework.Avoidance
{
    public class AvoidanceNode : INode
    {
        public AvoidanceNode(IDetailNode n)
        {
            BottomRight = n.BottomRight;
            BottomLeft = n.BottomLeft;
            Center = n.Center;
            DynamicWorldId = n.DynamicWorldId;
            ExplorationNode = n.ExplorationNode;
            GridPoint = n.GridPoint;
            NavigableCenter = n.NavigableCenter;
            NavigableCenter2D = n.NavigableCenter2D;
            NodeFlags = n.NodeFlags;
            TopLeft = n.TopLeft;
            TopRight = n.TopRight;
            BottomRight = n.BottomRight;
            ExplorationNode = n.ExplorationNode;
            AStarValue = n.AStarValue;

            ResetFlags();
        }

        public AvoidanceFlags AvoidanceFlags { get; set; }
        public byte AStarValue { get; set; }
        public IGroupNode ExplorationNode { get; set; }
        public Vector3 NavigableCenter { get; private set; }
        public Vector2 NavigableCenter2D { get; private set; }
        public Vector2 Center { get; private set; }
        public Vector2 TopLeft { get; private set; }
        public Vector2 BottomLeft { get; private set; }
        public Vector2 TopRight { get; private set; }
        public Vector2 BottomRight { get; private set; }
        public int DynamicWorldId { get; private set; }
        public int LevelAreaId { get; private set; }
        public NodeFlags NodeFlags { get; set; }
        public GridPoint GridPoint { get; set; }
        public int Weight { get; set; }

        public bool AddNodeFlags(AvoidanceFlags flags)
        {
            if (flags != AvoidanceFlags.None)
            {
                AvoidanceFlags |= flags;
                return true;
            }
            return false;
        }

        public bool RemoveNodeFlags(AvoidanceFlags flags)
        {
            if (flags != AvoidanceFlags.None)
            {
                AvoidanceFlags &= ~flags;
                return true;
            }
            return false;
        }

        private void ResetFlags()
        {
            AvoidanceFlags = 0;

            if (NodeFlags.HasFlag(NodeFlags.AllowWalk))
            {
                AvoidanceFlags |= AvoidanceFlags.AllowWalk;
            }
            if (NodeFlags.HasFlag(NodeFlags.AllowFlier))
            {
                AvoidanceFlags |= AvoidanceFlags.AllowFlier;
            }
            if (NodeFlags.HasFlag(NodeFlags.AllowProjectile))
            {
                AvoidanceFlags |= AvoidanceFlags.AllowProjectile;
            }
        }

        public void Reset()
        {
            Weight = 0;
            ResetFlags();
        }

        //public HashSet<long> MonsterIds = new HashSet<long>();
        //public HashSet<long> AvoidanceIds = new HashSet<long>();
        //public HashSet<long> ObstacleIds = new HashSet<long>();

        public float WeightPct
        {
            get
            {
                var max = Core.Avoidance.HighestNodeWeight;
                return max != 0 ? Weight / (float)max : 0;
            }
        }

        private DateTime _lastUpdatedDistance = DateTime.MinValue;
        private float _cachedDistance;
        public float Distance
        {
            get
            {
                var now = DateTime.UtcNow;
                if (_cachedDistance > 0 && now.Subtract(_lastUpdatedDistance).TotalMilliseconds < 250)
                    return _cachedDistance;

                _lastUpdatedDistance = now;
                return _cachedDistance = NavigableCenter.Distance(Trinity.Player.Position);
            }
        }

        //internal void Reset()
        //{
        //    MonsterIds.Clear();
        //    AvoidanceIds.Clear();
        //    ObstacleIds.Clear();
        //    Weight = 0;
        //    ResetNodeFlags();
        //}

        private List<AvoidanceNode> _adjacentNodes;
        public List<AvoidanceNode> AdjacentNodes
        {
            get { return _adjacentNodes ?? (_adjacentNodes = AvoidanceGrid.Instance.GetNeighbors(this)); }
            set { _adjacentNodes = value; }
        }

        public float NearbyWeightPct { get; set; }

        //public List<AvoidanceNode> AdjacentNodes = new List<AvoidanceNode>();

    }

}
