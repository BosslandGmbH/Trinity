using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Monk
{
    partial class Monk : CombatBase
    {
        public static bool IszDPS
        {
            get
            {
                return (Sets.IstvansPairedBlades.IsEquipped ||
                        Sets.Innas.CurrentBonuses == 2 && Sets.ThousandStorms.CurrentBonuses >= 1) &&
                       Skills.Monk.CycloneStrike.IsActive;
            }
        }

        public static TrinityPower GetPower()
        {
            if (Player.IsInTown)
                return null;
            TrinityPower power = Unconditional.PowerSelector();

            if (power == null && CurrentTarget != null && CurrentTarget.IsUnit)
            {
                if (IszDPS)
                {
                    var bestdps = PhelonUtils.BestDpsPosition(12f, 20f, true);
                    power = bestdps.Distance(Player.Position) < 10f &&
                            bestdps.Distance(Player.Position) > 5f
                        ? new TrinityPower(SNOPower.Walk, 3f, bestdps)
                        : ZDps.PowerSelector();
                }
                //power = ZDps.PowerSelector() ?? new TrinityPower(SNOPower.Walk, 0f, PhelonUtils.BestDpsPosition(35f, true));
            }
            return power;
        }
    }
}