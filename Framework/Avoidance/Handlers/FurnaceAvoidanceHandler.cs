using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Bot.Navigation;
using Zeta.Common;


namespace Trinity.Framework.Avoidance.Handlers
{
    internal class FurnaceAvoidanceHandler : BaseAvoidanceHandler
    {
        public override bool UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
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

                    HandleNavigationGrid(grid, nodes, avoidance, actor, 0f);
                    return true;
                }
            }
            else
            {
                var obstacleNodes = grid.GetNodesInRadius(actor.Position, part.Radius);

                const int weightOverride = 5;
                HandleNavigationGrid(AvoidanceFlags.NavigationBlocking, grid, obstacleNodes, avoidance, actor, 0f, weightOverride);
                return true;
            }

            return false;
        }
    }
}