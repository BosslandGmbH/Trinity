using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Common;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Combat.Abilities.PhelonsPlayground
{
    class PhelonGroupSupport
    {
        internal static TrinityActor Monk
        {
            get
            {
                try
                {
                    var monk = TrinityPlugin.Targets.FirstOrDefault(x => x.InternalName.ToLower().Contains("monk"));
                    //if (monk == null)
                    //    Logger.Log("Unable to find Monk.  Where did he go?");
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

        internal static TrinityActor PullCharacter
        {
            get
            {
                return
                    TrinityPlugin.Targets.OrderBy(y => y.NearbyUnitsWithinDistance(20))
                        .FirstOrDefault(x => x.IsPlayer);
            }
        }

        internal static List<TrinityActor> UnitsAroundPuller(Vector3 pullLocation, float groupRadius = 20,
            bool includeUnitsInAoe = true)
        {
            return
                (from u in PhelonUtils.SafeList(includeUnitsInAoe)
                    where u.IsUnit && u.Position.Distance(pullLocation) <= groupRadius
                    select u).ToList();
        }

        internal static List<TrinityActor> UnitsAroundPlayer(float groupRadius = 20, bool includeUnitsInAoe = true)
        {
            return
                (from u in PhelonUtils.SafeList(includeUnitsInAoe)
                    where u.IsUnit && u.Position.Distance(TrinityPlugin.Player.Position) <= groupRadius
                    select u).ToList();
        }

        internal static List<TrinityActor> UnitsToPull(Vector3 pullLocation, float groupRadius = 15,
            int groupCount = 1, float searchRange = 45, bool includeUnitsInAoe = true)
        {

            return
                (from u in PhelonUtils.SafeList(includeUnitsInAoe)
                    where u.IsUnit && u.CanCastTo() && u.HasBeenInLoS &&
                          !UnitsAroundPuller(pullLocation, 20, includeUnitsInAoe)
                              .Select(x => x.AcdId)
                              .Contains(u.AcdId) &&
                          !UnitsAroundPlayer(10, includeUnitsInAoe)
                              .Select(x => x.AcdId)
                              .Contains(u.AcdId) &&
                          u.Position.Distance(pullLocation) <= searchRange
                    orderby u.Distance
                    select u).ToList();
        }
    }
}