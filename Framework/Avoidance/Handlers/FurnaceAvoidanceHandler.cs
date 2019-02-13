using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Bot.Navigation;
using Zeta.Common;


namespace Trinity.Framework.Avoidance.Handlers
{
    internal class FurnaceAvoidanceHandler : IAvoidanceHandler
    {
        public bool UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            var actor = Core.Actors.RactorByRactorId<TrinityActor>(avoidance.RActorId);
            if (actor == null || !actor.IsValid || actor.IsDead)
                return false;

            var part = avoidance.Definition.GetPart(actor.ActorSnoId);
            if (part == null)
                return false;

            if (part.Type == PartType.VisualEffect)
            {
                if (actor.Attributes.GetAttribute<bool>(part.Attribute, (int)part.Power))
                {
                    Core.Logger.Log("Power {0} on {1} ({1}) in Attribute {2}", part.Power, actor.InternalName, part.Name, part.Attribute);
                    var nodes = grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 30f, actor.Rotation)).SelectMany(n => n.AdjacentNodes).Distinct();

                    if (avoidance.Settings.Prioritize)
                    {
                        grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance | AvoidanceFlags.CriticalAvoidance, avoidance, 50);
                    }
                    else
                    {
                        grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance, avoidance, 10);
                    }

                    Core.DBGridProvider.AddCellWeightingObstacle(actor.RActorId, ObstacleFactory.FromActor(actor));
                    return true;
                }
            }
            else
            {
                var obstacleNodes = grid.GetNodesInRadius(actor.Position, part.Radius);

                if (avoidance.Settings.Prioritize)
                    grid.FlagAvoidanceNodes(obstacleNodes, AvoidanceFlags.NavigationBlocking | AvoidanceFlags.CriticalAvoidance, avoidance, 50);
                else
                    grid.FlagAvoidanceNodes(obstacleNodes, AvoidanceFlags.NavigationBlocking, avoidance, 5);

                Core.DBGridProvider.AddCellWeightingObstacle(actor.RActorId, ObstacleFactory.FromActor(actor));
                return true;
            }

            return false;
        }
    }
}