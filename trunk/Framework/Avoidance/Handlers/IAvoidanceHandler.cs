using System.Collections.Generic;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;

namespace Trinity.Framework.Avoidance.Handlers
{
    public interface IAvoidanceHandler
    {
        void UpdateNodes(AvoidanceGrid grid, Avoidance avoidance);
        bool IsAllowed { get; }
    }
}


