using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Coroutines;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Modules;
using Trinity.Framework.Objects.Enums;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Framework.Behaviors
{
    public sealed class MoveToInteractBehavior : BaseBehavior
    {
        public TrinityActor Actor { get; set; }

        public HashSet<Vector3> VisitedActorPositions { get; set; } = new HashSet<Vector3>();

        public async Task<bool> While(Predicate<TrinityActor> markerSelector, int timeoutMs = 30000)
        {
            return await Run(async () => await FindActor(markerSelector), Move, timeoutMs);
        }

        private async Task<bool> FindActor(Predicate<TrinityActor> actorSelector)
        {
            var actor = Core.Actors.AllRActors
                .OrderBy(m => m.Distance)
                .FirstOrDefault(m => m.Position != Vector3.Zero 
                    && actorSelector(m) 
                    && !VisitedActorPositions.Contains(m.Position) 
                    && (Actor == null || Actor.Distance > m.AxialRadius));

            if (actor != null && (IsRunning || (!PlayerMover.IsBlocked && actor.Distance < 500)) && !Navigator.StuckHandler.IsStuck)
            {
                Actor = actor;
                return true;
            }
            return false;
        }

        private async Task<bool> Move()
        {
            Logger.LogVerbose($"Moving to Interact: {Actor}");
            await CommonCoroutines.MoveTo(Actor.Position, Actor.Name);
            return true;
        }

        protected override async Task<bool> OnStarted()
        {
            Logger.Warn($"Started moving to Interact: {Actor}");
            return true;
        }

        protected override async Task<bool> OnStopped()
        {
            Logger.Warn($"Arrived at Interact: {Actor}");
            Actor.Interact();
            VisitedActorPositions.Add(Actor.Position);
            Actor = null;
            return true;
        }
    }
}
