using System;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;

namespace Trinity.Framework.Avoidance.Handlers
{
    public class AnimationCircularAvoidanceHandler : IAvoidanceHandler
    {
        public void UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            foreach (var actor in avoidance.Actors)
            {
                if (actor == null || !actor.IsValid)
                    continue;

                var part = avoidance.Definition.GetPart(actor.Animation);
                if (part == null)
                    continue;

                var radius = Math.Max(part.Radius, actor.Radius);
                var finalRadius = radius * avoidance.Settings.DistanceMultiplier;
                var nodes = grid.GetNodesInRadius(actor.Position, finalRadius);

                if (actor.Animation != part.Animation)
                    continue;

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