using Trinity.Framework.Grid;

namespace Trinity.Framework.Avoidance.Handlers
{
    public interface IAvoidanceHandler
    {
        void UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance);
    }
}