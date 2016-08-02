using Trinity.Reference;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.WitchDoctor
{
    partial class WitchDoctor : CombatBase
    {
        public static bool IszDPS
        {
            get { return Legendary.AquilaCuirass.IsEquipped && Legendary.LastBreath.IsEquipped; }
        }

        public static TrinityPower GetPower()
        {
            if (Player.IsInTown)
                return null;
            TrinityPower power = Unconditional.PowerSelector();
            if (power == null && CurrentTarget != null)
            {
                if (IszDPS)
                    power = ZDps.PowerSelector();
            }
            return power;
        }
    }
}
