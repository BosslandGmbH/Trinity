using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using Trinity.DbProvider;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects;
using Zeta.Game;

namespace Trinity.Framework.Modules
{
    public class BlockedCheck : Module
    {
        protected override int UpdateIntervalMs => 100;

        protected override void OnPulse()
        {
            if (PlayerMover.CanMoveUnhindered)
            {
                BlockedStart = default(DateTime);
                IsBlocked = false;
                return;
            }

            Nodes = Core.Grids.Avoidance.GetNodesInRadius(Core.Player.Position, 10f)
                .Where(n => n.IsWalkable && Core.Grids.Avoidance.IsInPlayerFacingDirection(n.NavigableCenter,90d)).ToList();

            NodeCount = Nodes.Count;
            ClearNodeCount = Nodes.Count(n => n.IsWalkable && !n.AvoidanceFlags.HasFlag(AvoidanceFlags.Monster));
            BlockedPct = (ClearNodeCount / (double) Nodes.Count)*100;
            MoveSpeed = Core.PlayerHistory.GetYardsPerSecond();

            var isBlocked = MoveSpeed <= 5f && BlockedPct <= 65;
            if (IsBlocked != isBlocked)
            {
                BlockedStart = isBlocked ? DateTime.UtcNow : default(DateTime);
            }

            IsBlocked = isBlocked;
            //Logger.Warn(ToString());
        }

        public DateTime BlockedStart { get; private set; }
        public TimeSpan BlockedTime => IsBlocked ? DateTime.UtcNow.Subtract(BlockedStart) : TimeSpan.Zero;
        public List<AvoidanceNode> Nodes { get; private set; } = new List<AvoidanceNode>();
        public double MoveSpeed { get; private set; }
        public double BlockedPct { get; private set; }
        public int NodeCount { get; private set; }
        public int ClearNodeCount { get; private set; }
        public bool IsBlocked { get; private set; }

        public override string ToString() => $"{GetType().Name}: {(IsBlocked ? "BLOCKED" : "")} ClearNodes={ClearNodeCount} ({BlockedPct:N2}%) Speed:{MoveSpeed:N2} Duration:{BlockedTime}";
    }
}
