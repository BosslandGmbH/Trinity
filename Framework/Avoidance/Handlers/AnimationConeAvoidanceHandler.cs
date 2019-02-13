using System;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Bot.Navigation;

namespace Trinity.Framework.Avoidance.Handlers
{
    public class AnimationConeAvoidanceHandler : IAvoidanceHandler
    {
        public bool UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            var actor = Core.Actors.RactorByRactorId<TrinityActor>(avoidance.RActorId);
            if (actor == null || !actor.IsValid)
                return false;

            var part = avoidance.Definition.GetPart(actor.Animation);
            if (actor.Animation != part?.Animation)
                return false;

            var radius = Math.Max(part.Radius, actor.Radius) * avoidance.Settings.DistanceMultiplier;
            var nonCachedRotation = actor.Rotation;
            var arcDegrees = Math.Max(15, part.AngleDegrees);
            var nodes = grid.GetConeAsNodes(actor.Position, arcDegrees, radius, nonCachedRotation);

            if (avoidance.Settings.Prioritize)
            {
                grid.FlagAvoidanceNodes(nodes.SelectMany(n => n.AdjacentNodes), AvoidanceFlags.Avoidance | AvoidanceFlags.CriticalAvoidance, avoidance, 50);
            }
            else
            {
                grid.FlagAvoidanceNodes(nodes.SelectMany(n => n.AdjacentNodes), AvoidanceFlags.Avoidance, avoidance, 10);
            }

            Core.DBGridProvider.AddCellWeightingObstacle(actor.RActorId, ObstacleFactory.FromActor(actor, radius));
            return true;
        }
    }
}