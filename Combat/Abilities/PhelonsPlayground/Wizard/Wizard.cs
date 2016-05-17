using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Wizard
{
    partial class Wizard : CombatBase
    {
        private static int FirebirdsCount = Sets.FirebirdsFinery.CurrentBonuses;
        private static int VyrsCount = Sets.VyrsAmazingArcana.CurrentBonuses;
        private static int TalRashasCount = Sets.TalRashasElements.CurrentBonuses;
        private static int DMOCount = Sets.DelseresMagnumOpus.CurrentBonuses;

        public static TrinityPower GetPower()
        {
            if (Player.IsInTown)
                return null;
            TrinityPower power = Unconditional.PowerSelector();
            if (power == null && CurrentTarget != null)
            {
                if (FirebirdsCount == 3 || VyrsCount == 3)
                {
                    power = Firebirds.PowerSelector();
                    //if (power == null) power = new TrinityPower(SNOPower.Walk, 7f, PhelonUtils.BestWalkLocation);
                    if (power == null) power = new TrinityPower(SNOPower.Walk, 7f, Player.Position);
                }
                if (TalRashasCount == 3)
                {
                    if (Legendary.TheTwistedSword.IsEquipped)
                    {
                        power = TalRasha.EnergyTwister.PowerSelector();
                    }
                }
            }
            return power;
        }
    }
}