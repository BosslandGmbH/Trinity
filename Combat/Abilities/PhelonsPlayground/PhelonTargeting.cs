using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Vector3 = Buddy.Auth.Math.Vector3;

namespace Trinity.Combat.Abilities.PhelonsPlayground
{
    class PhelonTargeting : CombatBase
    {
        private static int NonEliteRange = Settings.Combat.Misc.NonEliteRange;
        private static bool ForceKillSummoners = Settings.Combat.Misc.ForceKillSummoners;
        private static bool IgnoreMinions = Settings.Combat.Misc.IgnoreMinions;
        private static bool ExtendedTrashKill = Settings.Combat.Misc.ExtendedTrashKill;
        private static double IgnoreTrashBelowHealthDoT = Settings.Combat.Misc.IgnoreTrashBelowHealthDoT;
        private static int TrashPackSize = Settings.Combat.Misc.TrashPackSize;
        private static int TrashPackSizeMin = Settings.Combat.Misc.TrashPackSizeMin;

        public static TrinityCacheObject BestAoeUnit(float range = 45, bool includeInAoE = false)
        {
            return PhelonUtils.BestEliteInRange(range, includeInAoE) ??
                   PhelonUtils.GetBestClusterUnit(7, range, false, includeInAoE) ?? 
                   CurrentTarget;
        }

        public static TrinityCacheObject BestTarget(bool includeInAoE = false)
        {
            return CurrentTarget.Type == TrinityObjectType.Shrine || CurrentTarget.IsTreasureGoblin ||
                   CurrentTarget.IsTreasureGoblin || CurrentTarget.Type == TrinityObjectType.HealthGlobe || 
                   CurrentTarget.IsBossOrEliteRareUnique
                ? CurrentTarget
                : BestAoeUnit(45, true) ?? CurrentTarget;
        }

        public static void CombatToggling()
        {
            var ArchonExists =
                TrinityPlugin.ObjectCache.FirstOrDefault(x => x.HasDebuff(SNOPower.Wizard_Archon) && x.HasDotDPS);
            if (ArchonExists != null)
            {
                NonEliteRange = Settings.Combat.Misc.NonEliteRange;
                ForceKillSummoners = Settings.Combat.Misc.ForceKillSummoners;
                IgnoreMinions = Settings.Combat.Misc.IgnoreMinions;
                ExtendedTrashKill = Settings.Combat.Misc.ExtendedTrashKill;
                IgnoreTrashBelowHealthDoT = Settings.Combat.Misc.IgnoreTrashBelowHealthDoT;
                TrashPackSize = Settings.Combat.Misc.TrashPackSize;
                TrashPackSizeMin = Settings.Combat.Misc.TrashPackSizeMin;

                Settings.Combat.Misc.NonEliteRange = 0;
                Settings.Combat.Misc.ForceKillSummoners = false;
                Settings.Combat.Misc.IgnoreMinions = true;
                Settings.Combat.Misc.ExtendedTrashKill = false;
                Settings.Combat.Misc.IgnoreTrashBelowHealthDoT = 0.50;
                Settings.Combat.Misc.TrashPackSize = 25;
                Settings.Combat.Misc.TrashPackSizeMin = 25;
                return;
            }

            if (Settings.Combat.Misc.NonEliteRange == NonEliteRange)
                return;

            Settings.Combat.Misc.NonEliteRange = NonEliteRange;
            Settings.Combat.Misc.ForceKillSummoners = ForceKillSummoners;
            Settings.Combat.Misc.IgnoreMinions = IgnoreMinions;
            Settings.Combat.Misc.ExtendedTrashKill = ExtendedTrashKill;
            Settings.Combat.Misc.IgnoreTrashBelowHealthDoT = IgnoreTrashBelowHealthDoT;
            Settings.Combat.Misc.TrashPackSize = TrashPackSize;
            Settings.Combat.Misc.TrashPackSizeMin = TrashPackSizeMin;
        }
    }
}