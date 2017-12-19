using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
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
            return await Run(async () => await FindActor(actorSelector), Move, timeoutMs);
        }

        private async Task<bool> FindActor(Predicate<TrinityActor> actorSelector)
        {
            var actor = Core.Actors.Actors
                .OrderBy(m => m.Distance)
                .FirstOrDefault(m => m.IsValid && m.Position != Vector3.Zero && actorSelector(m) && !VisitedActorPositions.Contains(m.Position) && m.Distance > 8f);

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
            Core.Logger.Verbose($"移动到: {Actor} {Actor.Position}");
            PlayerMover.MoveTo(Actor.Position);
            return true;
        }

        protected override async Task<bool> OnStarted()
        {
            Core.Logger.Warn($"开始移动到: {Actor}");
            return true;
        }

        protected override async Task<bool> OnStopped()
        {
            Core.Logger.Warn($"到达: {Actor}");
            VisitedActorPositions.Add(Actor.Position);
            Actor = null;
            return true;
        }
    }
}