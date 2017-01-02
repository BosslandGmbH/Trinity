using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Misc;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Modules
{
    /// <summary>
    /// Minimap markers
    /// </summary>
    public class MarkersCache : Module
    {
        protected override int UpdateIntervalMs => 1000;

        private readonly ConcurrentCache<Vector3, TrinityMarker, MinimapMarker> _cache;

        public IEnumerable<TrinityMarker> CurrentWorldMarkers => _cache.Items.Values.ToList();

        public MarkersCache()
        {
            _cache = new ConcurrentCache<Vector3, TrinityMarker, MinimapMarker>(GetSourceItems, GetKey, Update, Create);
        }

        private IList<MinimapMarker> GetSourceItems() => ZetaDia.Minimap.Markers.CurrentWorldMarkers.ToList();

        private Vector3 GetKey(MinimapMarker item) => item.Position;

        private TrinityMarker Create(Vector3 key, MinimapMarker newItem, out bool success)
        {
            var nativeMarker = (Marker)newItem;
            success = true;
            return new TrinityMarker
            {
                Name = nativeMarker.Name,
                NameHash = nativeMarker.NameHash,
                WorldSnoId = nativeMarker.WorldId,
                Position = key,
                Distance = nativeMarker.Distance,
                TextureId = nativeMarker.MinimapTextureId,
                MarkerType = nativeMarker.MarkerType,
            };
        }

        private TrinityMarker Update(Vector3 key, TrinityMarker existingItem, MinimapMarker newItem, out bool success)
        {
            success = existingItem.WorldSnoId == Core.Player.WorldDynamicId;
            existingItem.Distance = Core.Player.Position.Distance(existingItem.Position);         
            return existingItem;
        }

        protected override void OnPulse()
        {
            if (ZetaDia.Me == null)
                return;

            _cache.Update();
        }
    }

    public class TrinityMarker
    {
        public int TextureId { get; set; }
        public float Distance { get; set; }
        public Vector3 Position { get; set; }
        public int NameHash { get; set; }
        public string Name { get; set; }
        public WorldMarkerType MarkerType { get; set; }
        public int WorldSnoId { get; set; }
        public override string ToString() => $"{Name} at {Position} Distance {Distance} Type={MarkerType} TextureId={TextureId}";
    }
}

