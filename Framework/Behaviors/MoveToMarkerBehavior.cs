using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Components.Coroutines;
using Trinity.Modules;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;


namespace Trinity.Framework.Behaviors
{
    public sealed class MoveToMarkerBehavior : BaseBehavior
    {
        public TrinityMarker Marker { get; set; }

        public HashSet<Vector3> VisitedMarkerPositions { get; set; } = new HashSet<Vector3>();

        public async Task<bool> While(Predicate<TrinityMarker> markerSelector, int timeoutMs = 20000)
        {
            return await Run(async () => await FindMarker(markerSelector), Move, timeoutMs);
        }

        private async Task<bool> FindMarker(Predicate<TrinityMarker> markerSelector)
        {
            if (Marker != null && Core.Markers.Contains(Marker) && Marker.Distance <= 12f)
                return false;

            var marker = Core.Markers
                .OrderBy(m => m.Distance)
                .FirstOrDefault(m => m.Position != Vector3.Zero && markerSelector(m) && !VisitedMarkerPositions.Contains(m.Position) && m.Distance > 10f);

            if (marker != null && (IsRunning || (!TrinityCombat.IsInCombat && marker.Distance < 500)) && !Navigator.StuckHandler.IsStuck)
            {
                if (VisitedMarkerPositions.Count > 500)
                    VisitedMarkerPositions.Clear();

                Marker = marker;
                return true;
            }

            return false;
        }

        private async Task<bool> Move()
        {
            Core.Logger.Verbose($"Moving to Marker: {Marker}");
            await CommonCoroutines.MoveTo(Marker.Position, "ItemMarker");
            return true;
        }

        protected override async Task<bool> OnStarted()
        {
            Core.Logger.Warn($"Started moving to Marker: {Marker}");
            return true;
        }

        protected override async Task<bool> OnStopped()
        {
            Core.Logger.Warn($"Arrived at Marker: {Marker}");
            await VacuumItems.Execute();
            VisitedMarkerPositions.Add(Marker.Position);
            Marker = null;
            return true;
        }
    }
}