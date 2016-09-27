//using Trinity.Reference;
//using Trinity.Technicals;
//using Zeta.Game.Internals.Actors;

//namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.WitchDoctor
//{
//    using Framework;

//    partial class WitchDoctor
//    {
//        internal class Unconditional
//        {
//            private static readonly int SoulStacks = Legendary.SacredHarvester.IsEquipped ? 10 : 5;

//            public static TrinityPower PowerSelector()
//            {

//                if (ShouldSoulHarvest)
//                    return SoulHarvest;

//                if (ShouldSummonDogs)
//                    return SummonDogs;

//                if (ShouldSummonGargs)
//                    return SummonGargs;

//                if (Player.IsInTown)
//                    return null;

//                if (ShouldSpiritWalk)
//                    return CastSpiritWalk;

//                return null;
//            }

//            private static bool ShouldSoulHarvest
//                =>
//                    CanCast(SNOPower.Witchdoctor_SoulHarvest) &&
//                    GetBuffStacks(SNOPower.Witchdoctor_SoulHarvest) < SoulStacks ||
//                    Skills.WitchDoctor.SoulHarvest.TimeSinceUse > 8000 &&
//                    (TargetUtil.AnyMobsInRange(18f, 3) ||
//                     TargetUtil.AnyBossesInRange(18f) ||
//                     TargetUtil.AnyElitesInRange(18f));

//            private static bool ShouldSpiritWalk => Skills.WitchDoctor.SpiritWalk.CanCast() &&
//                                                    (TargetUtil.ClosestGlobe() != null ||
//                                                     TargetUtil.BestAoeUnit(45, true) != null &&
//                                                     TargetUtil.BestDpsPosition(
//                                                         TargetUtil.BestAoeUnit(45, true).Position, 45f, true)
//                                                         .Distance2D(Player.Position) > 5f && !IszDPS ||
//                                                     Player.CurrentHealthPct < 0.5 ||
//                                                     Core.Avoidance.InAvoidance(Player.Position));

//            private static bool ShouldSummonGargs => CanCast(SNOPower.Witchdoctor_Gargantuan) &&
//                                                     Player.Summons.GargantuanCount < GargCount;

//            private static bool ShouldSummonDogs => CanCast(SNOPower.Witchdoctor_SummonZombieDog) &&
//                                                    Player.Summons.ZombieDogCount < DogCount;

//            private static TrinityPower CastSpiritWalk
//                => new TrinityPower(SNOPower.Witchdoctor_SpiritWalk, 0f, Player.Position);

//            private static TrinityPower SummonGargs => new TrinityPower(SNOPower.Witchdoctor_Gargantuan);

//            private static TrinityPower SummonDogs => new TrinityPower(SNOPower.Witchdoctor_SummonZombieDog);
//            private static TrinityPower SoulHarvest => new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);
//        }
//    }
//}