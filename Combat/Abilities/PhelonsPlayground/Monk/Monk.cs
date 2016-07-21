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
        private static bool IszDPS => (Sets.IstvansPairedBlades.IsEquipped ||
                                      Sets.Innas.CurrentBonuses == 2 && Sets.ThousandStorms.CurrentBonuses >= 1) &&
                                      Skills.Monk.CycloneStrike.IsActive;

        private static bool IsInnas => Sets.ShenlongsSpirit.IsFullyEquipped &&
                                       Sets.Innas.CurrentBonuses == 3 && 
                                       Sets.ThousandStorms.CurrentBonuses == 1;

        public static TrinityPower GetPower()
        {
            if (Player.IsInTown)
                return null;

            TrinityPower power = Unconditional.PowerSelector();

            if (power == null && CurrentTarget != null && CurrentTarget.IsUnit)
            {
                if (IszDPS)
                    power = ZDps.PowerSelector();
                if (IsInnas)
                    power = Innas.PowerSelector();
            }
            return power;
        }

        
    }
}