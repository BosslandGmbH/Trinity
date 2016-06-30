using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Technicals;
using Zeta.Bot;
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
            Markers = ZetaDia.Minimap.Markers.CurrentWorldMarkers.Select(m => (Marker)m);
        }

        public IEnumerable<Marker> Markers { get; set; }
    }
}
