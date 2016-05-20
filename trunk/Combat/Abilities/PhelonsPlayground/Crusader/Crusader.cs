using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Crusader
{
    partial class Crusader : CombatBase
    {
        public static bool IsBombardment = Sets.LegacyOfNightmares.IsEquipped && Skills.Crusader.Bombardment.IsActive;
        public static bool IszDPS
        {
            get
            {
                return (Sets.IstvansPairedBlades.IsEquipped ||
                        Sets.Innas.CurrentBonuses == 2 && Sets.ThousandStorms.CurrentBonuses >= 1) &&
                       Skills.Monk.CycloneStrike.IsActive;
            }
        }

        public static bool IsSteedCharging
        {
            get { return DataDictionary.SteedChargeAnimations.Contains(Player.CurrentAnimation); }
        }

        public static TrinityPower GetPower()
        {
            if (Player.IsInTown)
                return null;
            TrinityPower power = Unconditional.PowerSelector();
            if (power == null && CurrentTarget != null && CurrentTarget.IsUnit)
            {
                if (IsBombardment)
                    return LegacyOfNightmares.PowerSelector();

                if (power == null) power = new TrinityPower(SNOPower.Walk, 0f, PhelonUtils.BestWalkLocation);
            }
            return power;
        }
    }
}