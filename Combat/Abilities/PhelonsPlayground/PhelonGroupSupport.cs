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
            get
            {
                return TrinityPlugin.ObjectCache.FirstOrDefault(x => x.IsPlayer && x.InternalName.Contains("monk_"));
            }
        }

        internal static TrinityCacheObject PullCharacter
        {
            get
            {
                return
                    TrinityPlugin.ObjectCache.OrderBy(y => y.NearbyUnitsWithinDistance(20))
                        .FirstOrDefault(x => x.IsPlayer);
            }
        }

        internal static List<TrinityCacheObject> UnitsAroundPuller(Vector3 pullLocation, float groupRadius = 20,
            bool includeUnitsInAoe = true)
        {
            return
                (from u in PhelonUtils.SafeList(includeUnitsInAoe)
                    where u.IsUnit && u.Position.Distance(pullLocation) <= groupRadius
                    select u).ToList();
        }

        internal static List<TrinityCacheObject> UnitsAroundPlayer(float groupRadius = 20, bool includeUnitsInAoe = true)
        {
            return
                (from u in PhelonUtils.SafeList(includeUnitsInAoe)
                    where u.IsUnit && u.Position.Distance(TrinityPlugin.Player.Position) <= groupRadius
                    select u).ToList();
        }

        internal static List<TrinityCacheObject> UnitsToPull(Vector3 pullLocation, float groupRadius = 15,
            int groupCount = 1,
            float searchRange = 90, bool includeUnitsInAoe = true)
        {

            return
                (from u in PhelonUtils.SafeList(includeUnitsInAoe)
                    where u.IsUnit && 
                          !UnitsAroundPuller(pullLocation, 20, includeUnitsInAoe)
                              .Select(x => x.ACDGuid)
                              .Contains(u.ACDGuid) &&
                          !UnitsAroundPlayer(10, includeUnitsInAoe)
                              .Select(x => x.ACDGuid)
                              .Contains(u.ACDGuid) &&
                          u.Position.Distance(pullLocation) <= searchRange //&&
                          //u.NearbyUnitsWithinDistance(groupRadius) >= groupCount
                    orderby u.NearbyUnitsWithinDistance(groupRadius),
                        u.Distance
                    select u).ToList();
        }
    }
}