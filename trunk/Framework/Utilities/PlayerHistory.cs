using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Objects.Enums;
using Trinity.Helpers;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Framework.Utilities
{
    public class PlayerHistory : Utility
    {
        public class PositionHistory : IEquatable<PositionHistory>
        {
            public PositionHistory()
            {
                if (ZetaDia.Me != null && ZetaDia.Me.IsValid)
                {
                    Position = Trinity.Player.Position;
                    RecordedAt = DateTime.UtcNow;
                    WorldId = Trinity.Player.WorldID;
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

        private const int CacheLimit = 150;
        private const int RecentPositionsLimit = 20;
        private CacheField<Vector3> _centroid = new CacheField<Vector3>(UpdateSpeed.Fast);
        public DateTime LastRecordedTime;
        public Vector3 LastRecordedPosition;
        public List<PositionHistory> Cache = new List<PositionHistory>();
        public List<Vector3> RecentPositions = new List<Vector3>();

        protected override void OnPulse()
        {
            AddPosition();
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

            var myPosition = Trinity.Player.Position;

            if (Cache.Any(p => p.Position.Distance(myPosition) < distance))
                return;

            if (myPosition == Vector3.Zero)
                return;

            RecentPositions.Add(myPosition);

            Cache.Add(new PositionHistory());       
        }

        public void MaintainCache()
        {
            var worldId = Trinity.Player.WorldID;

            if (RecentPositions.Count > RecentPositionsLimit)
                RecentPositions.RemoveAt(0);

            if (Cache.Count > CacheLimit)
                Cache.RemoveAt(0);

            Cache.RemoveAll(p => p.WorldId != worldId);
        }

        public Vector3 Centroid
        {
            get { return _centroid.GetValue(() => MathUtil.Centroid(RecentPositions)); }
        }


    }
}
