using System.Linq;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Zeta.Common;


namespace Trinity.Framework.Avoidance.Handlers
{
    internal class FurnaceAvoidanceHandler : IAvoidanceHandler
    {
        public void UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance)
        {
            foreach (var actor in avoidance.Actors)
            {
                if (actor == null)
                    continue;

                var part = avoidance.Definition.GetPart(actor.ActorSnoId);

                if (actor.IsDead || actor.CommonData == null || !actor.CommonData.IsValid || actor.CommonData.IsDisposed)
                {
                    Core.Logger.Verbose("参与者 {0} 常用数据无效 ({1})", actor.InternalName, part.Name);
                    continue;
                }

                if (part.Type == PartType.VisualEffect)
                {
                    if (actor.Attributes.GetAttribute<bool>(part.Attribute, (int)part.Power))
                    {
                        Core.Logger.Log("Power {0} on {1} ({1}) in Attribute {2}", part.Power, actor.InternalName, part.Name, part.Attribute);
                        var nodes = grid.GetRayLineAsNodes(actor.Position, MathEx.GetPointAt(actor.Position, 30f, actor.Rotation)).SelectMany(n => n.AdjacentNodes).Distinct();
                        grid.FlagAvoidanceNodes(nodes, AvoidanceFlags.Avoidance, avoidance, 10);
                    }
                }
                else
                {
                    var obstacleNodes = grid.GetNodesInRadius(actor.Position, part.Radius);
                    grid.FlagAvoidanceNodes(obstacleNodes, AvoidanceFlags.NavigationBlocking, avoidance, 5);
                }
            }
        }
    }
}