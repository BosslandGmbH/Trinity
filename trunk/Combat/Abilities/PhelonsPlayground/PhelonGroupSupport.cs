using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeta.Common;

namespace Trinity.Combat.Abilities.PhelonsPlayground
{
    class PhelonGroupSupport
    {
        internal static TrinityCacheObject Monk
        {
            get { return TrinityPlugin.ObjectCache.FirstOrDefault(x => x.InternalNameLowerCase.Contains("monk")); }
        }
        internal static List<TrinityCacheObject> UnitsAroundPuller(Vector3 pullLocation, float groupRadius = 20)
        {
            return
                (from u in PhelonUtils.SafeList()
                    where u.IsUnit && u.Position.Distance(pullLocation) <= groupRadius
                    select u).ToList();
        }

        internal static List<TrinityCacheObject> UnitsToPull(Vector3 pullLocation, float groupRadius = 10, int groupCount = 3,
            float searchRange = 75, bool includeUnitsInAoe = true)
        {

            return
                (from u in PhelonUtils.SafeList(includeUnitsInAoe)
                 where u.IsUnit && !UnitsAroundPuller(pullLocation).Contains(u) &&
                       u.Position.Distance(pullLocation) <= searchRange &&
                       u.NearbyUnitsWithinDistance(groupRadius) >= groupCount
                 orderby u.NearbyUnitsWithinDistance(groupRadius),
                     u.Distance descending
                 select u).ToList();
        }
    }
}
