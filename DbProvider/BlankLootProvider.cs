using System.Collections.Generic;
using Zeta.Bot;
using Zeta.Game.Internals.Actors;

namespace Trinity.DbProvider
{
    public class BlankLootProvider : ITargetingProvider
    {
        private static readonly List<DiaObject> listEmptyList = new List<DiaObject>();

        public List<DiaObject> GetObjectsByWeight()
        {
            return listEmptyList;
        }
    }
}