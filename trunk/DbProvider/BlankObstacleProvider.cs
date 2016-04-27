using System.Collections.Generic;
using Zeta.Bot;
using Zeta.Game.Internals.Actors;

namespace Trinity.DbProvider
{
    /// <summary>
    /// Obstacle Targeting Provider 
    /// </summary>
    /// <remarks>
    /// This class is injected to DemonBuddy. 
    /// Leave blank, process is bypassed by plugin
    /// </remarks>
    public class BlankObstacleProvider : ITargetingProvider
    {
        private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();

        /// <summary>
        /// Gets list of obstacle in range by weight.
        /// </summary>
        /// <returns>Blank list of target, Trinity don't use this Db process.</returns>
        public List<DiaObject> GetObjectsByWeight()
        {
            return listEmptyList;
        }
    }
}
