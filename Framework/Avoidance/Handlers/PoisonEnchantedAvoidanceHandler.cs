using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Common;


namespace Trinity.Framework.Avoidance.Handlers
{
    internal class PoisonEnchantedAvoidanceHandler : IAvoidanceHandler
    {
        public void UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            foreach (var actor in avoidance.Actors)
            {
                if (actor == null)
                    continue;

                var part = avoidance.Definition.GetPart(actor.ActorSnoId);
                if (part == null) continue;

                try
                {
                    if (part.Type == PartType.Telegraph)
                    {
                        var nodes = new List<AvoidanceNode>();
                        nodes.AddRange(grid.GetRayLineAsNodes(actor.Position,
                            MathEx.GetPointAt(actor.Position, 60f, (float)(Math.PI / 2))));
                        nodes.AddRange(grid.GetRayLineAsNodes(actor.Position,
                            MathEx.GetPointAt(actor.Position, 60f, (float)(2 * Math.PI))));
                        nodes.AddRange(grid.GetRayLineAsNodes(actor.Position,
                            MathEx.GetPointAt(actor.Position, 60f, (float)(3 * Math.PI / 2))));
                        nodes.AddRange(grid.GetRayLineAsNodes(actor.Position,
                            MathEx.GetPointAt(actor.Position, 60f, (float)(Math.PI))));
                        grid.FlagAvoidanceNodes(nodes.SelectMany(n => n.AdjacentNodes), AvoidanceFlags.Avoidance,
                            avoidance, 10);
                    }
                    else
                    {
                        var nodes = grid.GetRayLineAsNodes(actor.Position, avoidance.StartPosition)
                            .SelectMany(n => n.AdjacentNodes);
                        grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance, avoidance, 10);
                    }
                }
                catch (Exception ex)
                {
                    Core.Logger.Error("Exception {0}", ex);
                }
            }
        }
    }
}