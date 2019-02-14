using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;

namespace Trinity.Framework.Avoidance.Handlers
{
    public abstract class BaseAvoidanceHandler : IAvoidanceHandler
    {
        public const int DefaultWeightModification = 10;
        public const int CriticalWeightModification = 50;

        public abstract bool UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance);

        protected void HandleNavigationGrid(AvoidanceFlags defaultFlags, TrinityGrid grid, IEnumerable<AvoidanceNode> nodes, Structures.Avoidance avoidance, TrinityActor actor, float radius, int normalWeightModificationOverride = 0)
        {
            AvoidanceFlags flags = defaultFlags;
            int weightModification = DefaultWeightModification;

            if (normalWeightModificationOverride != 0)
                weightModification = normalWeightModificationOverride;

            if (avoidance.Settings.Prioritize)
            {
                weightModification = CriticalWeightModification;
                flags |= AvoidanceFlags.CriticalAvoidance;
            }

            grid.FlagAvoidanceNodes(nodes, flags, avoidance, weightModification);
            Core.DBGridProvider.AddCellWeightingObstacle(actor.RActorId, ObstacleFactory.FromActor(actor, radius));
        }

        protected void HandleNavigationGrid(TrinityGrid grid, IEnumerable<AvoidanceNode> nodes, Structures.Avoidance avoidance, TrinityActor actor, float radius, int normalWeightModificationOverride = 0)
        {
            HandleNavigationGrid(AvoidanceFlags.Avoidance, grid, nodes, avoidance, radius, normalWeightModificationOverride);
        }
    }
}
