using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Barbarian
{
    partial class Barbarian : CombatBase
    {
        private static int RaekorCount = Sets.TheLegacyOfRaekor.CurrentBonuses;
        private static int ImmortalKingsCount = Sets.ImmortalKingsCall.CurrentBonuses;
        private static int EarthCount = Sets.MightOfTheEarth.CurrentBonuses;
        private static int WastesCount = Sets.WrathOfTheWastes.CurrentBonuses;
        public static bool zDPS = RaekorCount == 2 && Sets.BulKathossOath.IsEquipped ||
                                  Sets.IstvansPairedBlades.IsEquipped && Legendary.IllusoryBoots.IsEquipped;

        public static TrinityPower GetPower()
        {
            if (Player.IsInTown)
                return null;
            TrinityPower power = Unconditional.PowerSelector();
            if (power == null && CurrentTarget != null && CurrentTarget.IsUnit)
            {
                if (RaekorCount == 3)
                    power = Raekor.PowerSelector();

                if (zDPS)
                    power = ZDps.PowerSelector();

                if (ImmortalKingsCount == 3)
                    power = ImmortalKingsCall.PowerSelector();
                if (EarthCount == 3)
                    power = MightOfTheEarth.PowerSelector();
                if (WastesCount == 3)
                    power = WrathOfTheWastes.PowerSelector();
                if (RaekorCount < 1 && ImmortalKingsCount < 1 && EarthCount < 1 && WastesCount < 1)
                    power = LegacyOfNightmares.PowerSelector();
            }
            return power ?? new TrinityPower(SNOPower.Walk, 7f, PhelonUtils.BestWalkLocation);
        }
    }
}