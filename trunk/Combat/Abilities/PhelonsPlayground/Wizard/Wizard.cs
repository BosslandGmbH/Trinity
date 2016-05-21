using Trinity.Reference;
using Trinity.Technicals;
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
            if (power == null && CurrentTarget != null && CurrentTarget.IsUnit)
            {
                if (FirebirdsCount == 3 || VyrsCount == 3)
                {
                    power = Firebirds.PowerSelector();
                    //if (power == null) power = new TrinityPower(SNOPower.Walk, 7f, PhelonUtils.BestWalkLocation);
                }
                if (TalRashasCount == 3)
                {
                    if (Legendary.TheTwistedSword.IsEquipped)
                    {
                        power = TalRasha.EnergyTwister.PowerSelector();
                    }
                    //if (Legendary.WandOfWoh.IsEquipped)
                    //{
                    //    power = TalRasha.Flashfire.PowerSelector();
                    //}
                }
                if (power == null) power = new TrinityPower(SNOPower.Walk, 0f, Player.Position);
            }
            return power;
        }
    }
}