using System;
using Zeta.Common;

namespace Trinity.Cache
{
    internal static class CacheUtils
    {
        internal static bool IsBossSNO(int actorSNO)
        {
            return DataDictionary.BossIds.Contains(actorSNO);
        }

        internal static bool IsAvoidanceSNO(int actorSNO)
        {
            return DataDictionary.Avoidances.Contains(actorSNO) || DataDictionary.ButcherFloorPanels.Contains(actorSNO) || DataDictionary.AvoidanceProjectiles.Contains(actorSNO);
        }

        internal static float GetZDiff(Vector3 Position)
        {
            if (Position != Vector3.Zero)
                return Math.Abs(Trinity.Player.Position.Z - Position.Z);
            else
                return 0f;
        }
    }
}
