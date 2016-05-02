using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Trinity.Framework;
using Zeta.Common;

namespace Trinity.Combat.Abilities.PhelonsPlayground
{
    class PhelonUtils
    {
        internal static List<TrinityCacheObject> SafeList(bool objectsInAoe = false)
        {
            return
                TrinityPlugin.ObjectCache.Where(x => objectsInAoe || !Core.Avoidance.InAvoidance(x.Position)).ToList();
        }

        internal static TrinityCacheObject ClosestHealthGlobe(float distance = 45, bool objectsInAoe = false)
        {
            return (from u in SafeList(objectsInAoe)
                where u.Type == TrinityObjectType.HealthGlobe && u.RadiusDistance <= distance
                select u).FirstOrDefault();
        }

        internal static bool WithInDistance(TrinityCacheObject actor, TrinityCacheObject actor2, float distance, bool objectsInAoe = false)
        {
            return
                SafeList(objectsInAoe).Any(
                    m => m.ActorSNO == actor.ActorSNO && m.Position.Distance(actor2.Position) <= distance);
        }

        internal static bool WithInDistance(TrinityCacheObject actor, Vector3 unitLocation, float distance, bool objectsInAoe = false)
        {
            return
                SafeList(objectsInAoe).Any(
                    m => m.ActorSNO == actor.ActorSNO && m.Position.Distance(unitLocation) <= distance);
        }

        internal static List<TrinityCacheObject> GetDiaObjects(uint actorSNO, float range = 25f, bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.ActorSNO == actorSNO
                    orderby u.Distance
                    select u).ToList();
        }

        internal static Vector3 GetDiaObjectBestClusterPoint(uint actorSNO, float radius = 15f, float maxRange = 45f,
            bool useWeights = true, bool includeUnitsInAoe = true, bool objectsInAoe = false)
        {
            var clusterUnits =
                (from u in SafeList(objectsInAoe)
                    where u.ActorSNO == actorSNO &&
                          u.RadiusDistance <= maxRange
                    orderby u.NearbyUnitsWithinDistance(radius),
                        u.Distance descending
                    select u.Position).ToList();

            return clusterUnits.Any() ? clusterUnits.FirstOrDefault() : Vector3.Zero;
        }

        internal static List<TrinityCacheObject> GetTwisterDiaObjects(float range = 25f, bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.ActorSNO == 322236
                    orderby u.Distance
                    select u).ToList();
        }

        internal static Vector3 GetBestTwsiterClusterPoint(float radius = 15f, float maxRange = 45f,
            bool useWeights = true, bool includeUnitsInAoe = true, bool objectsInAoe = false)
        {
            if (radius < 5f)
                radius = 5f;
            if (maxRange > 75f)
                maxRange = 75f;

            var clusterUnits =
                (from u in SafeList(objectsInAoe)
                    where u.ActorSNO == 322236 &&
                          u.RadiusDistance <= maxRange
                    orderby u.NearbyUnitsWithinDistance(radius),
                        u.Distance descending
                    select u.Position).ToList();

            return clusterUnits.Any() ? clusterUnits.FirstOrDefault() : Vector3.Zero;
        }

        internal static List<TrinityCacheObject> GetOculusBuffDiaObjects(float range = 25f, bool objectsInAoe = false)
        {
            return
                (from u in SafeList(objectsInAoe)
                    where u.RadiusDistance <= range &&
                          u.ActorSNO == 433966
                    orderby u.Distance
                    select u).ToList();
        }

        public static Vector3 PointBehind(Vector3 point, bool objectsInAoe = false)
        {
            return MathEx.GetPointAt(point, -5f, TrinityPlugin.Player.Rotation);
        }
    }
}
