using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Zeta.Common;
using Zeta.Game;

namespace Trinity
{
    public class PositionCache : IEquatable<PositionCache>
    {
        public Vector3 Position { get; set; }
        public DateTime RecordedAt { get; set; }
        public int WorldId { get; set; }

        public PositionCache()
        {
            if (ZetaDia.Me != null && ZetaDia.Me.IsValid)
            {
                Position = Core.Player.Position;
                RecordedAt = DateTime.UtcNow;
                WorldId = Core.Player.WorldSnoId;
            }
        }

        public static HashSet<PositionCache> Cache = new HashSet<PositionCache>();

        /// <summary>
        /// Adds the current position as needed and maintains the cache
        /// </summary>
        /// <param name="distance"></param>
        public static void AddPosition(float distance = 5f)
        {
            MaintainCache();

            if (Cache.Any(p => DateTime.UtcNow.Subtract(p.RecordedAt).TotalMilliseconds < 100))
                return;

            foreach (PositionCache p in Cache.Where(p => p.Position.Distance(Core.Player.Position) < distance).ToList())
            {
                Cache.Remove(p);
            }
            Cache.Add(new PositionCache());
        }

        /// <summary>
        /// Removes stale objects from the cache
        /// </summary>
        public static void MaintainCache()
        {
            int worldId = ZetaDia.CurrentWorldSnoId;
            Cache.RemoveWhere(p => DateTime.UtcNow.Subtract(p.RecordedAt).TotalMilliseconds > 10000);
            Cache.RemoveWhere(p => p.WorldId != worldId);
        }

        public bool Equals(PositionCache other)
        {
            return this.Position == other.Position && this.WorldId == other.WorldId;
        }
    }
}
