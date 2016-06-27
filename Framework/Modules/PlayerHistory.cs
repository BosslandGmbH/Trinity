using System;
using System.Linq;
using Trinity.Framework.Objects.Enums;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Framework.Modules
{
    public class PlayerHistory : Module
    {
        public class PositionHistory : IEquatable<PositionHistory>
        {
            public PositionHistory()
            {
                if (ZetaDia.Me != null && ZetaDia.Me.IsValid)
                {
                    Position = TrinityPlugin.Player.Position;
                    RecordedAt = DateTime.UtcNow;
                    WorldId = TrinityPlugin.Player.WorldSnoId;
                }
            }

            public Vector3 Position;
            public DateTime RecordedAt;
            public int WorldId;

            public bool Equals(PositionHistory other)
            {
                return Position == other.Position && WorldId == other.WorldId;
            }
        } 

        private const int CacheLimit = 50;
        private const int RecentPositionsLimit = 20;
        private CacheField<Vector3> _centroid = new CacheField<Vector3>(UpdateSpeed.Fast);
        public DateTime LastRecordedTime;
        public Vector3 LastRecordedPosition;
        public IndexedList<PositionHistory> Cache = new IndexedList<PositionHistory>();
        public IndexedList<Vector3> RecentPositions = new IndexedList<Vector3>();

        protected override void OnPulse()
        {
            using (new PerformanceLogger("Utilty.PlayerHistory.Pulse"))
            {
                AddPosition();
            }
        }
        
        protected override void OnWorldChanged()
        {
            RecentPositions.Clear();
            _centroid.Clear();
        }

        public void AddPosition(float distance = 5f)
        {
            MaintainCache();

            if (Cache.Any(p => DateTime.UtcNow.Subtract(p.RecordedAt).TotalMilliseconds < 250))
                return;

            var myPosition = TrinityPlugin.Player.Position;

            if (Cache.Any(p => p.Position.Distance(myPosition) < distance))
                return;

            if (myPosition == Vector3.Zero)
                return;

            RecentPositions.Add(myPosition);

            Cache.Add(new PositionHistory());       
        }

        public void MaintainCache()
        {
            var worldId = TrinityPlugin.Player.WorldSnoId;

            Cache.RemoveAll(p => p.WorldId != worldId);

            while (RecentPositions.Count > RecentPositionsLimit)
                RecentPositions.RemoveAt(0);

            while (Cache.Count > CacheLimit)
                Cache.RemoveAt(0);
        }

        public Vector3 Centroid
        {
            get { return _centroid.GetValue(() => MathUtil.Centroid(RecentPositions)); }
        }


    }
}
