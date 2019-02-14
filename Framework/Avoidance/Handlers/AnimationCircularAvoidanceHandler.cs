using System;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Bot.Navigation;
using Zeta.Game.Internals.Actors.Gizmos;

namespace Trinity.Framework.Avoidance.Handlers
{
    public class AnimationCircularAvoidanceHandler : BaseAvoidanceHandler
    {
        public override bool UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            var actor = Core.Actors.RactorByRactorId<TrinityActor>(avoidance.RActorId);
            if (actor == null || !actor.IsValid)
                return false;

            var part = avoidance.Definition.GetPart(actor.Animation);
            if (part == null)
                return false;

            var radius = Math.Max(part.Radius, actor.Radius);
            var finalRadius = radius * avoidance.Settings.DistanceMultiplier;
            var nodes = grid.GetNodesInRadius(actor.Position, finalRadius);

            HandleNavigationGrid(grid, nodes, avoidance, actor, finalRadius);
            return true;
        }
    }
}