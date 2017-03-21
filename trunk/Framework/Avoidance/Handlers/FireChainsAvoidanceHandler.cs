using System.Linq;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;

namespace Trinity.Framework.Avoidance.Handlers
{
    public class FireChainsAvoidanceHandler : IAvoidanceHandler
    {
        public void UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            var actor = avoidance.Actors.FirstOrDefault();
            if (actor == null)
                return;

            foreach (var otherAvoidance in Core.Avoidance.CurrentAvoidances)
            {
                if (otherAvoidance == avoidance)
                    continue;

                var fireChainFriend = otherAvoidance.Actors.FirstOrDefault(a => a.ActorSnoId == actor.ActorSnoId);
                if (fireChainFriend != null)
                {
                    var nodes = grid.GetRayLineAsNodes(actor.Position, fireChainFriend.Position).SelectMany(n => n.AdjacentNodes);
                    grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance, avoidance, 10);
                }
            }
        }
    }
}