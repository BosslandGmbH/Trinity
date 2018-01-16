using System;
using Trinity.Framework;
using System.Collections.Generic;
using System.Linq;
using Trinity.DbProvider;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects;

namespace Trinity.Modules
{
    /// <summary>
    /// Checks the space in front of the player for monsters and other blocking objects
    /// </summary>
    public class BlockedCheck : Module
    {
        protected override int UpdateIntervalMs => 100;

        protected override void OnPulse()
        {
            MoveSpeed = Core.PlayerHistory.GetYardsPerSecond();

            if (PlayerMover.CanMoveUnhindered)
                // && !ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true).Any(x => x.ACDId != Core.Player.AcdId))
            {
                IsBlocked = false;
                return;
            }

            Nodes = Core.Grids.Avoidance.GetNodesInRadius(Core.Player.Position, 10f)
                .Where(n => n.IsWalkable && Core.Grids.Avoidance.IsInPlayerFacingDirection(n.NavigableCenter,90d)).ToList();

            NodeCount = Nodes.Count;
            ClearNodeCount = Nodes.Count(n => n.IsWalkable && !n.AvoidanceFlags.HasFlag(AvoidanceFlags.Monster));
            BlockedPct = (ClearNodeCount / (double) Nodes.Count)*100;
            
            var isBlocked = MoveSpeed <= 5f && BlockedPct <= 65;
            if (IsBlocked != isBlocked)
            {
                BlockedStart = isBlocked ? DateTime.UtcNow : default(DateTime);
            }

            IsBlocked = isBlocked;
            //Core.Logger.Warn(ToString());
        }

        public DateTime BlockedStart { get; private set; }
        public TimeSpan BlockedTime => IsBlocked ? DateTime.UtcNow.Subtract(BlockedStart) : TimeSpan.Zero;
        public List<AvoidanceNode> Nodes { get; private set; } = new List<AvoidanceNode>();
        public double MoveSpeed { get; private set; }
        public double BlockedPct { get; private set; }
        public int NodeCount { get; private set; }
        public int ClearNodeCount { get; private set; }
        public bool IsBlocked { get; private set; }

        public void Clear()
        {
            BlockedStart = default(DateTime);
            Nodes.Clear();
            NodeCount = 0;
            BlockedPct = 0;
            ClearNodeCount = 0;
            IsBlocked = false;
        }

        public override string ToString() => $"{GetType().Name}: {(IsBlocked ? "BLOCKED" : "")} ClearNodes={ClearNodeCount} ({BlockedPct:N2}%) Speed:{MoveSpeed:N2} Duration:{BlockedTime}";
    }
}
