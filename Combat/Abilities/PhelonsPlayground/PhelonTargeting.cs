using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeta.Common;
using Vector3 = Buddy.Auth.Math.Vector3;

namespace Trinity.Combat.Abilities.PhelonsPlayground
{
    class PhelonTargeting : CombatBase
    {
        public static TrinityCacheObject PhelonCurrentTarget = BestAoeUnit;
        public static TrinityCacheObject BestAoeUnit
            => CurrentTarget.Type == TrinityObjectType.Shrine || CurrentTarget.IsTreasureGoblin ||
               CurrentTarget.Type == TrinityObjectType.HealthGlobe || CurrentTarget.IsBoss
                ? CurrentTarget
                : TargetUtil.BestEliteInRange(35)
                  ??
                  TargetUtil.GetBestClusterUnit(10,
                      Math.Max(Settings.Combat.Misc.EliteRange, Settings.Combat.Misc.NonEliteRange), 5, false)
                      //?? TargetUtil.GetClosestUnit()
                  ?? CurrentTarget;
    }
}