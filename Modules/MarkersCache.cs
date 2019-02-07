using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Modules
{
    public interface IMarkerProvider : IEnumerable<TrinityMarker>
    {
        TrinityMarker FindMarker(int hash);
        TrinityMarker FindMarker(WorldMarkerType type);
    }

    public class TrinityMarker
    {
        public int TextureId { get; set; }
        public float Distance { get; set; }
        public Vector3 Position { get; set; }
        public int NameHash { get; set; }
        public WorldMarkerType MarkerType { get; set; }
        public SNOWorld WorldSnoId { get; set; }
        public int Id { get; set; }

        public override string ToString() => $"{NameHash} at {Position} Distance {Distance} Type={MarkerType} TextureId={TextureId}";
    }


    public class MarkersCache : Module, IMarkerProvider
    {
        public TrinityMarker FindMarker(int hash)
            => CurrentWorldMarkers.FirstOrDefault(m => m.NameHash == hash);

        public TrinityMarker FindMarker(WorldMarkerType type) 
            => CurrentWorldMarkers.Where(m => m.MarkerType == type).OrderBy(m => m.Distance).FirstOrDefault();

        protected override int UpdateIntervalMs => 1000;

        private readonly ConcurrentCache<int, TrinityMarker, MinimapMarker> _cache;

        public IEnumerable<TrinityMarker> CurrentWorldMarkers => _cache.Items.Values.ToList();

        public MarkersCache()
        {
            _cache = new ConcurrentCache<int, TrinityMarker, MinimapMarker>(GetSourceItems, GetKey, Update, Create);
        }

        private IList<MinimapMarker> GetSourceItems() => ZetaDia.Minimap.Markers.CurrentWorldMarkers.ToList();

        private int GetKey(MinimapMarker item) => item.Id;

        private TrinityMarker Create(int key, MinimapMarker newItem, out bool success)
        {
            success = true;
            return new TrinityMarker
            {
                Id = newItem.Id,
                NameHash = newItem.NameHash,
                WorldSnoId = newItem.WorldId,
                Position = newItem.Position,
                Distance = newItem.Distance,
                TextureId = newItem.MinimapTextureSnoId,
                MarkerType = newItem.MarkerType,
            };
        }

        private TrinityMarker Update(int key, TrinityMarker existingItem, MinimapMarker newItem, out bool success)
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

        IEnumerator IEnumerable.GetEnumerator() => CurrentWorldMarkers.GetEnumerator();
        public IEnumerator<TrinityMarker> GetEnumerator() => CurrentWorldMarkers.GetEnumerator();
    }

}

