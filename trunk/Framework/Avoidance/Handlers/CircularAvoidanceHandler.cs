using System;
using System.Linq;
using Trinity.Framework.Avoidance.Structures;

namespace Trinity.Framework.Avoidance.Handlers
{
    public class CircularAvoidanceHandler : IAvoidanceHandler
    {
        public void UpdateNodes(AvoidanceGrid grid, Structures.Avoidance avoidance)
        {            
            foreach (var actor in avoidance.Actors)
            {
                if (actor == null || !actor.IsValid)
                    continue;

                var part = avoidance.Definition.GetPart(actor.ActorSnoId);
                var radius = Math.Max(part.Radius, actor.Radius);
                var finalRadius = radius * avoidance.Settings.DistanceMultiplier;
                var nodes = grid.GetNodesInRadius(actor.Position, finalRadius);

                if (avoidance.Settings.Prioritize)
                {
                    Core.DBGridProvider.AddCellWeightingObstacle(actor.ActorSnoId, finalRadius);
                    grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance | AvoidanceFlags.CriticalAvoidance, avoidance, 50);
                }
                else
                {
                    grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance, avoidance, 10);
                }

            }
        }

    }
}




