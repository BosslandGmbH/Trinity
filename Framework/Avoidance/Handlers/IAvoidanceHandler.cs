using Trinity.Framework.Grid;

namespace Trinity.Framework.Avoidance.Handlers
{
    public interface IAvoidanceHandler
    {
        bool UpdateNodes(TrinityGrid grid, Structures.Avoidance avoidance);
    }
}