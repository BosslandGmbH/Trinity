using Trinity.Components.Adventurer.Game.Exploration;
using Zeta.Common;

namespace Trinity.Framework.Grid
{
    public class GridHelper
    {
        public ExplorationGrid Exploration => ExplorationGrid.Instance;

        public TrinityGrid Avoidance => TrinityGrid.Instance;

        public bool CanRayCast(Vector3 @from, Vector3 to)
        {
            return Avoidance.CanRayCast(@from, to);
        }

        public bool CanRayCast(Vector3 to)
        {
            if (!Avoidance.IsPopulated) return false;
            return Avoidance.CanRayCast(Core.Player.Position, to);
        }

        public bool CanRayWalk(Vector3 from, Vector3 to)
        {
            return Avoidance.CanRayWalk(from, to);
        }

    }
}