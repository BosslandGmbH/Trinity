//using Trinity.Framework.Actors.ActorTypes;
//using Trinity.Reference;
//using Zeta.Game.Internals.Actors;

//namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.WitchDoctor
//{
//    partial class WitchDoctor
//    {
//        public class ZDps
//        {
//            public static TrinityPower PowerSelector()
//            {
//                if (Player.IsIncapacitated) return null;
//                TrinityActor target;

//                if (GetHasBuff(SNOPower.Witchdoctor_SpiritWalk))
//                    return new TrinityPower(SNOPower.Walk, 7f, TargetUtil.BestWalkLocation(35f, true));

//                if (ShouldPiranhas(out target))
//                    return CastPiranhas(target);

//                if (ShouldHex(out target))
//                    return CastHex(target);

//                if (ShouldMassConfusion)
//                    return CastMassConfusion;

//                if (ShouldBigBadVoodoo)
//                    return CastBigBadVoodoo;

//                if (ShouldLocustSwarm(out target))
//                    return CastLocustSwarmt(target);

//                if (ShouldHaunt(out target))
//                    return CastHaunt(target);

//                return TargetUtil.Monk != null
//                    ? new TrinityPower(SNOPower.Walk, 3f, TargetUtil.Monk.Position)
//                    : new TrinityPower(SNOPower.Walk, 3f, TargetUtil.BestAoeUnit(45).Position);
//            }

//            private static bool ShouldPiranhas(out TrinityActor target)
//            {
//                target = null;

//                if (!Skills.WitchDoctor.Piranhas.CanCast())
//                    return false;

//                if (Runes.WitchDoctor.WaveOfMutilation.IsActive)
//                    target = TargetUtil.GetBestPierceTarget(25, true);
//                else
//                    target = TargetUtil.BestAoeUnit(35, true);

//                return target != null;
//            }

//            private static TrinityPower CastPiranhas(TrinityActor target)
//            {
//                return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 45, target.Position);
//            }

//            private static bool ShouldBigBadVoodoo
//            {
//                get
//                {
//                    if (!Skills.WitchDoctor.BigBadVoodoo.CanCast())
//                        return false;
//                    var target = TargetUtil.Monk ?? TargetUtil.BestAoeUnit(45f, true);
//                    return target != null && target.Distance < 15;
//                }
//            }

//            private static TrinityPower CastBigBadVoodoo
//            {
//                get { return new TrinityPower(SNOPower.Witchdoctor_BigBadVoodoo); }
//            }

//            private static bool ShouldHex(out TrinityActor target)
//            {
//                target = null;
//                if (!Skills.WitchDoctor.Hex.CanCast())
//                    return false;
//                target = TargetUtil.Monk ?? TargetUtil.BestAoeUnit(45f, true);
//                return target != null && target.Distance < 35;
//            }

//            private static TrinityPower CastHex(TrinityActor target)
//            {
//                return new TrinityPower(SNOPower.Witchdoctor_Hex, 35f, target.Position);
//            }

//            private static bool ShouldMassConfusion
//            {
//                get
//                {
//                    if (!Skills.WitchDoctor.MassConfusion.CanCast())
//                        return false;
//                    var target = TargetUtil.Monk ?? TargetUtil.BestAoeUnit(45f, true);
//                    return target != null && target.Distance < 15;
//                }
//            }

//            private static TrinityPower CastMassConfusion
//            {
//                get
//                {
//                    var target = TargetUtil.Monk ?? TargetUtil.BestAoeUnit(45f, true);
//                    return new TrinityPower(SNOPower.Witchdoctor_MassConfusion, 35f, target.AcdId); }
//            }

//            private static bool ShouldHaunt(out TrinityActor target)
//            {
//                target = null;
//                if (!Skills.WitchDoctor.Haunt.CanCast())
//                    return false;
//                target = TargetUtil.BestAuraUnit(SNOPower.Witchdoctor_Haunt, 35, true);
//                return target != null;
//            }

//            private static TrinityPower CastHaunt(TrinityActor target)
//            {
//                return new TrinityPower(SNOPower.Witchdoctor_Haunt, 35f, target.AcdId);
//            }

//            private static bool ShouldLocustSwarm(out TrinityActor target)
//            {
//                target = null;
//                if (!Skills.WitchDoctor.LocustSwarm.CanCast() || Legendary.Wormwood.IsEquipped)
//                    return false;
//                target = TargetUtil.BestAuraUnit(SNOPower.Witchdoctor_Locust_Swarm, 35f, true);
//                return target != null;
//            }

//            private static TrinityPower CastLocustSwarmt(TrinityActor target)
//            {
//                return new TrinityPower(SNOPower.Witchdoctor_Locust_Swarm, 35f, target.AcdId);
//            }
//        }
//    }
//}