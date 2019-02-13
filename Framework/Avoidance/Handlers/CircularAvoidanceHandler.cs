using System;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Bot.Navigation;

namespace Trinity.Framework.Avoidance.Handlers
{
    public class CircularAvoidanceHandler : IAvoidanceHandler
    {
        public bool UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            var actor = Core.Actors.RactorByRactorId<TrinityActor>(avoidance.RActorId);
            if (actor == null || !actor.IsValid)
                return false;

            var part = avoidance.Definition.GetPart(actor.ActorSnoId);
            if (part == null)
                return false;

            var radius = Math.Max(part.Radius, actor.Radius);
            var finalRadius = radius * avoidance.Settings.DistanceMultiplier;
            var nodes = grid.GetNodesInRadius(actor.Position, finalRadius);

            if (avoidance.Settings.Prioritize)
            {
                //Core.Logger.Log(LogCategory.Avoidance, $"<CircularAvoidanceHandler> marking {nodes.Count} nodes critical for actor {actor}, def={avoidance.Definition.Name}");
                grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance | AvoidanceFlags.CriticalAvoidance,
                    avoidance, 50);
            }
            else
            {
                //Core.Logger.Log(LogCategory.Avoidance, $"<CircularAvoidanceHandler> marking {nodes.Count} nodes for actor {actor}, def={avoidance.Definition.Name}");
                grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance, avoidance, 10);
            }

            Core.DBGridProvider.AddCellWeightingObstacle(actor.RActorId, ObstacleFactory.FromActor(actor, finalRadius));
            return true;
        }
    }
}