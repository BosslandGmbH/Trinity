using System;
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
        TrinityMarker FindMarker(string name);
        TrinityMarker FindMarker(WorldMarkerType type);
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
        public int Id { get; set; }

        public override string ToString() => $"{Name} at {Position} Distance {Distance} Type={MarkerType} TextureId={TextureId}";
    }


    public class MarkersCache : Module, IMarkerProvider
    {
        public TrinityMarker FindMarker(int hash)
            => CurrentWorldMarkers.FirstOrDefault(m => m.NameHash == hash);

        public TrinityMarker FindMarker(string name)
            => CurrentWorldMarkers.Where(m => m.Name.ToLowerInvariant().Contains(name.ToLowerInvariant())).OrderBy(m => m.Distance).FirstOrDefault();

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
            var nativeMarker = (Marker)newItem;
            success = true;
            return new TrinityMarker
            {
                Name = nativeMarker.Name,
                Id = nativeMarker.Id,
                NameHash = nativeMarker.NameHash,
                WorldSnoId = nativeMarker.WorldId,
                Position = nativeMarker.Position,
                Distance = nativeMarker.Distance,
                TextureId = nativeMarker.MinimapTextureId,
                MarkerType = nativeMarker.MarkerType,
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

