using System;
using System.Collections.Generic;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Avoidance.Structures;
using Zeta.Common;

namespace Trinity.Framework.Grid
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
            IsWalkable = n.NodeFlags.HasFlag(NodeFlags.AllowWalk);
            DefaultFlags = GetDefaultFlags(NodeFlags);
            ResetFlags();
        }

        public bool IsWalkable;
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
        public AvoidanceFlags DefaultFlags { get; set; }

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
            AvoidanceFlags = DefaultFlags;
        }

        public AvoidanceFlags GetDefaultFlags(NodeFlags nodeFlags)
        {
            var flags = AvoidanceFlags.None;

            if (nodeFlags.HasFlag(NodeFlags.AllowWalk))
            {
                flags |= AvoidanceFlags.AllowWalk;
            }
            if (nodeFlags.HasFlag(NodeFlags.NearWall))
            {
                flags |= AvoidanceFlags.NearWall;
            }
            if (nodeFlags.HasFlag(NodeFlags.AllowFlier))
            {
                flags |= AvoidanceFlags.AllowFlier;
            }
            if (nodeFlags.HasFlag(NodeFlags.AllowProjectile))
            {
                flags |= AvoidanceFlags.AllowProjectile;
            }
            if (nodeFlags.HasFlag(NodeFlags.RoundedCorner0) ||
                nodeFlags.HasFlag(NodeFlags.RoundedCorner1) ||
                nodeFlags.HasFlag(NodeFlags.RoundedCorner2) ||
                nodeFlags.HasFlag(NodeFlags.RoundedCorner3))
            {
                flags |= AvoidanceFlags.RoundedCorner;
            }

            return flags;
        }

        public AvoidanceNode Reset()
        {
            HostileMonsterCount = 0;
            Weight = 0;
            ResetFlags();
            AvoidanceTypes.Clear();
            return this;
        }

        //public HashSet<long> MonsterIds = new HashSet<long>();
        //public HashSet<long> AvoidanceIds = new HashSet<long>();
        //public HashSet<long> ObstacleIds = new HashSet<long>();

        public float WeightPct
        {
            get
            {
                var max = Core.Avoidance.GridEnricher.HighestNodeWeight;
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
                return _cachedDistance = NavigableCenter.Distance(Core.Player.Position);
            }
        }

        private List<AvoidanceNode> _adjacentNodes;

        public List<AvoidanceNode> AdjacentNodes
        {
            get { return _adjacentNodes ?? (_adjacentNodes = TrinityGrid.Instance.GetNeighbors(this)); }
            set { _adjacentNodes = value; }
        }

        public float NearbyWeightPct { get; set; }
        public int HostileMonsterCount { get; set; }
        public List<AvoidanceType> AvoidanceTypes { get; set; } = new List<AvoidanceType>();
        public List<int> AvoidanceHashCodes { get; set; } = new List<int>();

        public override int GetHashCode()
        {
            return unchecked((int)Center.X * 31 ^ (int)Center.Y * 79);
        }
    }
}