using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeta.Common;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Combat.Abilities.PhelonsPlayground
{
    class PhelonGroupSupport
    {
        internal static TrinityCacheObject Monk
        {
            get
            {
                try
                {
                    var monk = TrinityPlugin.ObjectCache.FirstOrDefault(x => x.InternalName.ToLower().Contains("monk"));
                    if (monk == null)
                        Logger.Log("Unable to find Monk.  Where did he go?");
                    return monk;
                }
                catch (Exception)
                {
                    Logger.Log("Unable to find Monk.  Error?");
                    //return CombatBase.IsInParty ? TargetUtil.GetClosestUnit(25) : null;
                    return null;
                }
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
            int groupCount = 1, float searchRange = 45, bool includeUnitsInAoe = true)
        {

            return
                (from u in PhelonUtils.SafeList(includeUnitsInAoe)
                    where u.IsUnit && u.CanCastTo() && u.HasBeenInLoS &&
                          !UnitsAroundPuller(pullLocation, 20, includeUnitsInAoe)
                              .Select(x => x.ACDGuid)
                              .Contains(u.ACDGuid) &&
                          !UnitsAroundPlayer(10, includeUnitsInAoe)
                              .Select(x => x.ACDGuid)
                              .Contains(u.ACDGuid) &&
                          u.Position.Distance(pullLocation) <= searchRange //&&
                          //u.NearbyUnitsWithinDistance(groupRadius) >= groupCount
                    orderby //u.NearbyUnitsWithinDistance(groupRadius),
                        u.Distance descending 
                    select u).ToList();
        }
    }
}