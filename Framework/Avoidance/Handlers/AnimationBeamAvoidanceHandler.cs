using System;
using Trinity.Framework.Helpers;
using System.Linq;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Common;


namespace Trinity.Framework.Avoidance.Handlers
{
    public class AnimationBeamAvoidanceHandler : NotifyBase, IAvoidanceHandler
    {
        public void UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            var checkActors = avoidance.Actors.ToArray();
            foreach (var actor in checkActors)
            {
                try
                {
                    var part = avoidance.Definition.GetPart(actor.Animation);
                    if (actor.Animation != part.Animation)
                        continue;

                    var radius = Math.Max(part.Radius, actor.Radius) * avoidance.Settings.DistanceMultiplier;
                    var nonCachedRotation = actor.Rotation;
                    var nodes = grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, radius, nonCachedRotation)).SelectMany(n => n.AdjacentNodes);

                    grid.FlagAvoidanceNodes(nodes.SelectMany(n => n.AdjacentNodes), AvoidanceFlags.Avoidance, avoidance, 10);
                }
                catch (Exception)
                {
                    Core.Logger.Debug($"AnimationBeamAvoidanceHandler Exception reading Animation/Rotation for actor: {actor.InternalName}");
                    avoidance.Actors.Remove(actor);
                }
            }
        }
    }
}