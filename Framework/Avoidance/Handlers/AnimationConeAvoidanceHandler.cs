using System;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Bot.Navigation;

namespace Trinity.Framework.Avoidance.Handlers
{
    public class AnimationConeAvoidanceHandler : BaseAvoidanceHandler
    {
        public override bool UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            var actor = Core.Actors.RactorByRactorId<TrinityActor>(avoidance.RActorId);
            if (actor == null || !actor.IsValid)
                return false;

            var part = avoidance.Definition.GetPart(actor.Animation);

            var radius = Math.Max(part.Radius, actor.Radius) * avoidance.Settings.DistanceMultiplier;
            var nonCachedRotation = actor.Rotation;
            var arcDegrees = Math.Max(15, part.AngleDegrees);
            var nodes = grid.GetConeAsNodes(actor.Position, arcDegrees, radius, nonCachedRotation);

            HandleNavigationGrid(grid, nodes.SelectMany(n => n.AdjacentNodes), avoidance, actor, radius);
            return true;
        }
    }
}