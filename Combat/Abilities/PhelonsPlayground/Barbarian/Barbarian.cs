using Trinity.Reference;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Barbarian
{
    partial class Barbarian : CombatBase
    {
        private static int RaekorCount = Sets.TheLegacyOfRaekor.CurrentBonuses;
        private static int ImmortalKingsCount = Sets.ImmortalKingsCall.CurrentBonuses;
        private static int EarthCount = Sets.MightOfTheEarth.CurrentBonuses;
        private static int WastesCount = Sets.WrathOfTheWastes.CurrentBonuses;

        public static TrinityPower GetPower()
        {
            if (Player.IsInTown)
                return null;
            TrinityPower power = UnconditionalPower();
            if (power == null && CurrentTarget != null)
            {
                if (RaekorCount > 3)
                    return Raekor.PowerSelector();
                if (ImmortalKingsCount > 3)
                    return ImmortalKingsCall.PowerSelector();
                if (EarthCount > 3)
                    return MightOfTheEarth.PowerSelector();
                if (WastesCount > 3)
                    return WrathOfTheWastes.PowerSelector();
                if (RaekorCount < 1 && ImmortalKingsCount < 1 && EarthCount < 1 && WastesCount < 1)
                    return LegacyOfNightmares.PowerSelector();
            }
            return power;
        }
    }
}