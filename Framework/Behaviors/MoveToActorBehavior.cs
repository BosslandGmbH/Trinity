using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Combat;
using Trinity.Components.Coroutines;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Trinity.Framework.Objects;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;


namespace Trinity.Framework.Behaviors
{
    public sealed class MoveToActorBehavior : BaseBehavior
    {
        public MoveToActorBehavior()
        {
            ChangeEvents.WorldId.Changed += id => VisitedActorPositions.Clear();
        }

        public TrinityActor Actor { get; set; }

        public HashSet<Vector3> VisitedActorPositions { get; set; } = new HashSet<Vector3>();

        public async Task<bool> While(Predicate<TrinityActor> actorSelector, int timeoutMs = 30000)
        {
            return await Run(async () => FindActor(actorSelector), MoveProducer, timeoutMs);
        }

        private bool FindActor(Predicate<TrinityActor> actorSelector)
        {
            var actor = Core.Actors.Actors
                .OrderBy(m => m.Distance) // TODO: This is a TrinityActor... They don't have DistanceSqr...
                .FirstOrDefault(m => m.IsValid &&
                                     m.Position != Vector3.Zero &&
                                     actorSelector(m) &&
                                     !VisitedActorPositions.Contains(m.Position) &&
                                     m.Distance > 8f);

            if (actor != null &&
                (IsRunning ||
                 !PlayerMover.IsBlocked &&
                 actor.Distance < 500) &&
                !Navigator.StuckHandler.IsStuck)
            {
                if (VisitedActorPositions.Count > 500)
                    VisitedActorPositions.Clear();

                Actor = actor;
                return true;
            }

            return false;
        }

        private async Task<bool> MoveProducer()
        {
            Core.Logger.Verbose($"Moving to Actor: {Actor} {Actor.Position}");
            while (await CommonCoroutines.MoveAndStop(Actor.Position, 5f, Actor.Name) != MoveResult.ReachedDestination)
            {
                await Coroutine.Yield();
            }
            return true;
        }

        protected override async Task<bool> OnStarted()
        {
            Core.Logger.Warn($"Started moving to Actor: {Actor}");
            return true;
        }

        protected override async Task<bool> OnStopped()
        {
            Core.Logger.Warn($"Arrived at Actor: {Actor}");
            VisitedActorPositions.Add(Actor.Position);
            Actor = null;
            return true;
        }
    }
}