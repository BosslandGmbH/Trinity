using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Framework.Modules
{
    /// <summary>
    /// Minimap markers
    /// </summary>
    public class MarkersCache : Module
    {
        protected override int UpdateIntervalMs => 1000;

        protected override void OnPulse()
        {
            var markers = ZetaDia.Minimap.Markers.CurrentWorldMarkers
                .Select(zetaMarker => (Marker) zetaMarker)
                .Select(nativeMarker => new TrinityMarker
            {
                Name = nativeMarker.Name,
                NameHash = nativeMarker.NameHash,
                Position = nativeMarker.Position,
                Distance = nativeMarker.Distance,
                TextureId = nativeMarker.MinimapTextureId,
                MarkerType = nativeMarker.MarkerType,

            }).ToList();

            CurrentWorldMarkers = markers;
        }

        public IEnumerable<TrinityMarker> CurrentWorldMarkers { get; set; }
    }

    public class TrinityMarker
    {
        public int TextureId { get; set; }
        public float Distance { get; set; }
        public Vector3 Position { get; set; }
        public int NameHash { get; set; }
        public string Name { get; set; }
        public WorldMarkerType MarkerType { get; set; }
    }

}

