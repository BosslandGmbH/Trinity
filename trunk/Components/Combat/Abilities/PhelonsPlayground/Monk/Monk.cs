using Trinity.Reference;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.Monk
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

        public static bool IsRainment
        {
            get { return Sets.ThousandStorms.IsFullyEquipped; }
        }

        public static TrinityPower GetPower()
        {
            if (Player.IsInTown)
                return null;

            TrinityPower power = Unconditional.PowerSelector();

            if (power == null && CurrentTarget != null && CurrentTarget.IsUnit)
            {
                if (IsRainment)
                {
                    if (Sets.ShenlongsSpirit.IsFullyEquipped)
                    {
                        power = Rainment.PowerSelector();
                    }
                }
                if (IszDPS)
                {
                    power = ZDps.PowerSelector();
                }
            }
            return power;
        }

        
    }
}