using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Framework.Avoidance;
using Zeta.Common;

namespace Trinity.Framework.Helpers
{
    public class GridHelper
    {
        protected NavigationGrid Navigation => NavigationGrid.Instance;

        public ExplorationGrid Exploration => ExplorationGrid.Instance;

        public AvoidanceGrid Avoidance => AvoidanceGrid.Instance;

        public bool CanRayCast(Vector3 @from, Vector3 to)
        {
            return Navigation.CanRayCast(@from, to);
        }

        public bool CanRayWalk(Vector3 @from, Vector3 to)
        {
            return Navigation.CanRayCast(@from, to);
        }
    }

}


