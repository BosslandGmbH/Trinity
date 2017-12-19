using System;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;

namespace Trinity.Framework.Avoidance.Handlers
{
    public class CircularAvoidanceHandler : IAvoidanceHandler
    {
        public void UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            foreach (var actor in avoidance.Actors)
            {
                if (actor == null || !actor.IsValid)
                {
                    //Core.Logger.Log(LogCategory.Avoidance, $"<CircularAvoidanceHandler> actor is null or invalid for {avoidance.Definition.Name}");
                    continue;
                }

                var part = avoidance.Definition.GetPart(actor.ActorSnoId);
                var radius = Math.Max(part.Radius, actor.Radius);
                var finalRadius = radius * avoidance.Settings.DistanceMultiplier;
                var nodes = grid.GetNodesInRadius(actor.Position, finalRadius);

                if (avoidance.Settings.Prioritize)
                {
                    //Core.Logger.Log(LogCategory.Avoidance, $"<CircularAvoidanceHandler> marking {nodes.Count} nodes critical for actor {actor}, def={avoidance.Definition.Name}");
                    Core.DBGridProvider.AddCellWeightingObstacle(actor.ActorSnoId, finalRadius);
                    grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance | AvoidanceFlags.CriticalAvoidance, avoidance, 50);
                }
                else
                {
                    //Core.Logger.Log(LogCategory.Avoidance, $"<CircularAvoidanceHandler> marking {nodes.Count} nodes for actor {actor}, def={avoidance.Definition.Name}");
                    grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance, avoidance, 10);
                }
            }
        }
    }
}