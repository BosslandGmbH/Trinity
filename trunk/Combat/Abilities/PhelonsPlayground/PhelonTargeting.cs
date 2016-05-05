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
        public static TrinityCacheObject BestAoeUnit(bool includeInAoE = false)
            => CurrentTarget.Type == TrinityObjectType.Shrine || CurrentTarget.IsTreasureGoblin ||
               CurrentTarget.Type == TrinityObjectType.HealthGlobe || CurrentTarget.IsBoss
                ? CurrentTarget
                : PhelonUtils.BestEliteInRange(35, includeInAoE) ?? (PhelonUtils.GetBestClusterUnit(10, 45, false, includeInAoE) ?? CurrentTarget);
    }
}