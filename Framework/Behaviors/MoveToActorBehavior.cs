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
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Logger = Trinity.Framework.Helpers.Logger;

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
            return await Run(async () => await FindActor(actorSelector), Move, timeoutMs);
        }

        private async Task<bool> FindActor(Predicate<TrinityActor> actorSelector)
        {
            var actor = Core.Actors.AllRActors
                .OrderBy(m => m.Distance)
                .FirstOrDefault(m => m.Position != Vector3.Zero && actorSelector(m) && !VisitedActorPositions.Contains(m.Position) && m.Distance > 5f);

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
            Logger.LogVerbose($"Moving to Actor: {Actor} {Actor.Position}");
            PlayerMover.MoveTo(Actor.Position);
            return true;
        }        

        protected override async Task<bool> OnStarted()
        {
            Logger.Warn($"Started moving to Actor: {Actor}");
            return true;
        }

        protected override async Task<bool> OnStopped()
        {
            Logger.Warn($"Arrived at Actor: {Actor}");         
            VisitedActorPositions.Add(Actor.Position);
            Actor = null;
            return true;
        }
    }
}
