using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Combat.Abilities;
using Trinity.Coroutines;
using Trinity.Framework.Modules;
using Trinity.Framework.Objects.Enums;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Behaviors
{
    public sealed class MoveToMarkerBehavior : BaseBehavior
    {
        public TrinityMarker Marker { get; set; }

        public HashSet<Vector3> VisitedMarkerPositions { get; set; } = new HashSet<Vector3>();

        public async Task<bool> While(Predicate<TrinityMarker> markerSelector, int timeoutMs = 30000)
        {
            return await Run(async () => await FindMarker(markerSelector), Move, timeoutMs);
        }

        private async Task<bool> FindMarker(Predicate<TrinityMarker> markerSelector)
        {
            var marker = Core.Markers.CurrentWorldMarkers.FirstOrDefault(m => m.Position != Vector3.Zero && markerSelector(m) && !VisitedMarkerPositions.Contains(m.Position) && m.Distance > 10f);
            if (marker != null && (IsRunning || (!CombatBase.IsInCombat && marker.Distance < 500)) && !Navigator.StuckHandler.IsStuck)
            {
                Marker = marker;
                return true;
            }
            return false;
        }

        private async Task<bool> Move()
        {
            Logger.LogVerbose($"Moving to Item Marker at {Marker.Position}! Distance {Marker.Distance}!");
            await CommonCoroutines.MoveTo(Marker.Position, "ItemMarker");
            TrinityPlugin.Player.CurrentAction = PlayerAction.Moving;
            return true;
        }

        protected override async Task<bool> OnStarted()
        {
            Logger.Warn($"Started moving to Marker: {Marker.Position}! Distance {Marker.Distance}!");
            return true;
        }

        protected override async Task<bool> OnStopped()
        {
            Logger.Warn($"Arrived at Marker: {Marker.Position}! Distance {Marker.Distance}!");
            VacuumItems.Execute();
            VisitedMarkerPositions.Add(Marker.Position);
            Marker = null;
            return true;
        }
    }
}
