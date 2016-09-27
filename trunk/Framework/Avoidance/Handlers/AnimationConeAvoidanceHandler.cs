
using System;
using System.Linq;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Technicals;

namespace Trinity.Framework.Avoidance.Handlers
{
    public class AnimationConeAvoidanceHandler : IAvoidanceHandler
    {
        public void UpdateNodes(AvoidanceGrid grid, Structures.Avoidance avoidance)
        {
            foreach (var actor in avoidance.Actors)
            {
                try
                {
                    var part = avoidance.Definition.GetPart(actor.Animation);
                    if (actor.Animation != part.Animation)
                        continue;
                    
                    var radius = Math.Max(part.Radius, actor.Radius) * avoidance.Settings.DistanceMultiplier;
                    var nonCachedRotation = actor.Rotation;
                    var arcDegrees = Math.Max(15, part.AngleDegrees);
                    var nodes = grid.GetConeAsNodes(actor.Position, arcDegrees, radius, nonCachedRotation);
                    grid.FlagAvoidanceNodes(nodes.SelectMany(n => n.AdjacentNodes), AvoidanceFlags.Avoidance, avoidance, 10);
                }
                catch (Exception ex)
                {
                    Logger.LogDebug($"AnimationConeAvoidanceHandler Exception for Actor: {actor.InternalName}. {ex}");
                }
            }
        }
    }
}




