using System.Collections.Generic;
using Trinity.Framework.Avoidance.Structures;

namespace Trinity.Framework.Avoidance.Handlers
{
    public interface IAvoidanceHandler
    {
        /// <summary>
        /// Updates the grid nodes that should be avoided.
        /// </summary>
        /// <param name="grid">grid to be updated</param>
        /// <param name="avoidance">avoidance to be processed</param>
        void UpdateNodes(AvoidanceGrid grid, Structures.Avoidance avoidance);

        /// <summary>
        /// If the avoidance should be avoided or not.
        /// </summary>
        bool IsAllowed { get; }

        /// <summary>
        /// Reset avoidance settings to handler defaults.
        /// </summary>
        void LoadDefaults();
    }
}


