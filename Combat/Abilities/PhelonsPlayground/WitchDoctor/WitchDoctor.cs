using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.WitchDoctor
{
    partial class WitchDoctor : CombatBase
    {
        public static bool IszDPS
        {
            get { return Legendary.AquilaCuirass.IsEquipped && Legendary.LastBreath.IsEquipped && Legendary.Stormshield.IsEquipped; }
        }

        public static TrinityPower GetPower()
        {
            if (Player.IsInTown)
                return null;
            TrinityPower power = Unconditional.PowerSelector();
            if (power == null && CurrentTarget != null && CurrentTarget.IsUnit)
            {
                if (IszDPS)
                    power = ZDps.PowerSelector() ?? new TrinityPower(SNOPower.Walk, 7f, PhelonUtils.BestWalkLocation);
            }
            return power;
        }
    }
}
