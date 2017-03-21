using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;


namespace Trinity.Framework.Behaviors
{
    public sealed class MoveToInteractBehavior : BaseBehavior
    {
        public TrinityActor Actor { get; set; }

        public HashSet<Vector3> VisitedActorPositions { get; set; } = new HashSet<Vector3>();

        public async Task<bool> While(Predicate<TrinityActor> actorSelector, int timeoutMs = 30000)
        {
            return await Run(async () => await FindActor(actorSelector), Move, timeoutMs);
        }

        private async Task<bool> FindActor(Predicate<TrinityActor> actorSelector)
        {
            var actor = Core.Actors.AllRActors
                .OrderBy(m => m.Distance)
                .FirstOrDefault(m => m.Position != Vector3.Zero
                    && actorSelector(m)
                    && !m.IsExcludedId && !m.IsExcludedType
                    && !VisitedActorPositions.Contains(m.Position)
                    && (Actor == null || Actor.Distance > m.AxialRadius));

            if (actor != null && (IsRunning || (!PlayerMover.IsBlocked && actor.Distance < 500)) && !Navigator.StuckHandler.IsStuck)
            {
                if (VisitedActorPositions.Count > 500)
                    VisitedActorPositions.Clear();

                Actor = actor;
                return true;
            }
            return false;
        }

        private async Task<bool> Move()
        {
            Core.Logger.Verbose($"Moving to Interact: {Actor}");
            await CommonCoroutines.MoveTo(Actor.Position, Actor.Name);
            return true;
        }

        protected override async Task<bool> OnStarted()
        {
            Core.Logger.Warn($"Started moving to Interact: {Actor}");
            return true;
        }

        protected override async Task<bool> OnStopped()
        {
            Core.Logger.Warn($"Arrived at Interact: {Actor}");
            Actor.Interact();
            VisitedActorPositions.Add(Actor.Position);
            Actor = null;
            return true;
        }
    }
}