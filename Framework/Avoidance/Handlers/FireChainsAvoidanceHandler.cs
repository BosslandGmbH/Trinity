using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Bot.Navigation;

namespace Trinity.Framework.Avoidance.Handlers
{
    public class FireChainsAvoidanceHandler : IAvoidanceHandler
    {
        public bool UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            var actor = Core.Actors.RactorByRactorId<TrinityActor>(avoidance.RActorId);
            if (actor == null || !actor.IsValid)
                return false;

            var fireChainFriendList = Core.Avoidance.CurrentAvoidances.Where(c => c != avoidance && c.ActorSno == avoidance.ActorSno)
                .ToList();

            var appliedAvoidanceNode = false;
            foreach (var fireChainFriend in fireChainFriendList)
            {
                var friend = Core.Actors.RactorByRactorId<TrinityActor>(fireChainFriend.RActorId);
                if (friend == null || !friend.IsValid)
                    continue;

                var nodes = grid.GetRayLineAsNodes(actor.Position, friend.Position).SelectMany(n => n.AdjacentNodes);

                AvoidanceFlags flags = AvoidanceFlags.Avoidance;
                int weightModification = BaseAvoidanceHandler.DefaultWeightModification;
                if (avoidance.Settings.Prioritize)
                {
                    flags |= AvoidanceFlags.CriticalAvoidance;
                    weightModification = BaseAvoidanceHandler.CriticalWeightModification;
                }

                grid.FlagAvoidanceNodes(nodes, flags, avoidance, weightModification);
                appliedAvoidanceNode = true;
            }

            if (appliedAvoidanceNode)
            {
                Core.DBGridProvider.AddCellWeightingObstacle(actor.RActorId, ObstacleFactory.FromActor(actor));
                return true;
            }

            return false;
        }
    }
}