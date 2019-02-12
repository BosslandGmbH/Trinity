using System;
using Trinity.Framework.Helpers;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Common;

namespace Trinity.Framework.Avoidance.Handlers
{
    public class AnimationBeamAvoidanceHandler : NotifyBase, IAvoidanceHandler
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
            var nodes = grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, radius, nonCachedRotation)).SelectMany(n => n.AdjacentNodes);

            grid.FlagAvoidanceNodes(nodes.SelectMany(n => n.AdjacentNodes), AvoidanceFlags.Avoidance, avoidance, 10);
            return true;
        }
    }
}