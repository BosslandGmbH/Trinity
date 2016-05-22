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
                    power = Firebirds.PowerSelector() ?? new TrinityPower(SNOPower.Walk, 3f, Player.Position);
                    //if (power == null) power = new TrinityPower(SNOPower.Walk, 7f, PhelonUtils.BestWalkLocation);
                }
                if (TalRashasCount == 3)
                {
                    if (VyrsCount > 1)
                        power = TalRasha.VyrArchon.PowerSelector() ?? new TrinityPower(SNOPower.Walk, 3f, Player.Position);

                    if (Legendary.TheTwistedSword.IsEquipped)
                        power = TalRasha.EnergyTwister.PowerSelector() ?? new TrinityPower(SNOPower.Walk, 3f, Player.Position);

                    if (Legendary.WandOfWoh.IsEquipped)
                        power = new TrinityPower(SNOPower.Walk, 3f, CurrentTarget.Position);
                }
            }
            return power;
        }
    }
}