//using Trinity.Reference;

//namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.Barbarian
//{
//    partial class Barbarian : CombatBase
//    {
//        private static int RaekorCount = Sets.TheLegacyOfRaekor.CurrentBonuses;
//        private static int ImmortalKingsCount = Sets.ImmortalKingsCall.CurrentBonuses;
//        private static int EarthCount = Sets.MightOfTheEarth.CurrentBonuses;
//        private static int WastesCount = Sets.WrathOfTheWastes.CurrentBonuses;

//        public static bool zDPSEquipped = (WastesCount <= 2 || RaekorCount <= 2 || Sets.LegacyOfNightmares.IsEquipped) &&
//                                  (Sets.BulKathossOath.IsEquipped ||
//                                   Sets.IstvansPairedBlades.IsEquipped ||
//                                   Legendary.IllusoryBoots.IsEquipped);

//        public static TrinityPower GetPower()
//        {
//            if (Player.IsInTown)
//                return null;
//            TrinityPower power = Unconditional.PowerSelector();
//            if (power == null && CurrentTarget != null && CurrentTarget.IsUnit)
//            {
//                //Logger.Log($"{zDPSEquipped} | {Sets.LegacyOfNightmares.IsEquipped} | {Legendary.IllusoryBoots.IsEquipped}");
//                if (zDPSEquipped)
//                    power = ZDps.PowerSelector();

//                if (RaekorCount == 3)
//                    power = Raekor.PowerSelector();

//                if (ImmortalKingsCount == 3)
//                    power = ImmortalKingsCall.PowerSelector();

//                if (EarthCount == 3)
//                    power = MightOfTheEarth.PowerSelector();

//                if (WastesCount == 3)
//                    power = WrathOfTheWastes.PowerSelector();

//                if (!zDPSEquipped && RaekorCount < 1 && ImmortalKingsCount < 1 && EarthCount < 1 && WastesCount < 1)
//                    power = LegacyOfNightmares.PowerSelector();

//                //if (power == null) power = new TrinityPower(SNOPower.Walk, 7f, TargetUtil.BestWalkLocation);
//            }
//            //if (power != null)
//            //    Logger.Log($"Using: {power}");
//            return power;
//        }
//    }
//}