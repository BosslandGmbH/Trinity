//using Trinity.Framework.Actors.ActorTypes;
//using Trinity.Reference;
//using Zeta.Game.Internals.Actors;

//namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.WitchDoctor
//{
//    partial class WitchDoctor
//    {
//        public class JadeHarvester
//        {
//            public class Plaguebearer
//            {
//                public static TrinityPower PowerSelector()
//                {
//                    if (Player.IsIncapacitated) return null;
//                    TrinityActor target;

//                    if (ShouldSoulHarvest)
//                        return SoulHarvest;

//                    if (GetHasBuff(SNOPower.Witchdoctor_SpiritWalk))
//                    {
//                        if (Player.CurrentHealthPct < Settings.Combat.Misc.HealthGlobeLevel &&
//                            TargetUtil.BestWalkLocation(35f, true).Distance(Player.Position) > 5)
//                            return new TrinityPower(SNOPower.Walk, 7f, TargetUtil.BestWalkLocation(45f, true));

//                        if (TargetUtil.BestDpsPosition(TargetUtil.BestAoeUnit(45, true).Position, 45f, true).Distance(Player.Position) > 5)
//                            return new TrinityPower(SNOPower.Walk, 3f,
//                                TargetUtil.BestDpsPosition(TargetUtil.BestAoeUnit(45, true).Position, 45f, true));
//                    }

//                    if (ShouldWallOfDeath(out target))
//                        return WallOfDeath(target);

//                    if (ShouldHaunt(out target))
//                        return CastHaunt(target);

//                    if (ShouldPiranhas(out target))
//                        return Piranhas(target);

//                    if (ShouldAcidCloud(out target))
//                        return CastAcidCloud(target);

//                    if (ShouldFirebomb(out target))
//                        return CastFirebomb(target);

//                    return new TrinityPower(SNOPower.Walk, 3f,
//                        TargetUtil.BestDpsPosition(TargetUtil.BestAoeUnit(45, true).Position, 45f, true));

//                    return null;
//                }

//                #region Conditions

//                private static bool ShouldAcidCloud(out TrinityActor target)
//                {
//                    target = null;

//                    if (!Skills.WitchDoctor.AcidCloud.CanCast())
//                        return false;

//                    target = TargetUtil.BestAoeUnit(35, true);

//                    return target != null;
//                }

//                private static bool ShouldFirebomb(out TrinityActor target)
//                {
//                    target = null;

//                    if (Skills.WitchDoctor.Firebomb.IsActive && !Skills.WitchDoctor.PlagueOfToads.IsActive &&
//                        !Skills.WitchDoctor.CorpseSpiders.IsActive && !Skills.WitchDoctor.PoisonDart.IsActive)
//                        return false;

//                    target = TargetUtil.BestAoeUnit(45, true);
//                    if (target != null)
//                        return true;

//                    target = TargetUtil.GetClosestUnit(45f);
//                    return target != null;
//                }

//                private static bool ShouldPiranhas(out TrinityActor target)
//                {
//                    target = null;

//                    if (!Skills.WitchDoctor.Piranhas.CanCast())
//                        return false;

//                    target = Runes.WitchDoctor.WaveOfMutilation.IsActive
//                        ? TargetUtil.GetBestPierceTarget(25, true)
//                        : TargetUtil.BestAoeUnit(35, true);

//                    return target != null;
//                }

//                private static bool ShouldWallOfDeath(out TrinityActor target)
//                {
//                    target = null;

//                    if (!Skills.WitchDoctor.WallOfDeath.CanCast())
//                        return false;
//                    if (Skills.WitchDoctor.WallOfDeath.TimeSinceUse < 2500)
//                        return false;
//                    target = TargetUtil.BestAoeUnit(35, true);

//                    return target != null;
//                }

//                private static bool ShouldSoulHarvest => CanCast(SNOPower.Witchdoctor_SoulHarvest) &&
//                                                         (TargetUtil.AnyMobsInRange(18f, 3) ||
//                                                          TargetUtil.AnyBossesInRange(18f) ||
//                                                          TargetUtil.AnyElitesInRange(18f));

//                private static bool ShouldHaunt(out TrinityActor target)
//                {
//                    target = null;

//                    if (!Skills.WitchDoctor.Haunt.CanCast())
//                        return false;

//                    if (SpellHistory.LastPowerUsed == Skills.WitchDoctor.Haunt.SNOPower)
//                        return false;

//                    target = TargetUtil.BestAuraUnit(SNOPower.Witchdoctor_Haunt, 45, true);

//                    return target != null;
//                }

//                #endregion

//                #region Expressions

//                private static TrinityPower SoulHarvest => new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);

//                private static TrinityPower Piranhas(TrinityActor target)
//                    => new TrinityPower(SNOPower.Witchdoctor_Piranhas, 45, target.Position);

//                private static TrinityPower WallOfDeath(TrinityActor target)
//                    => new TrinityPower(SNOPower.Witchdoctor_WallOfZombies, 45, target.Position);

//                private static TrinityPower CastHaunt(TrinityActor target)
//                    => new TrinityPower(SNOPower.Witchdoctor_Haunt, 35f, target.AcdId);
//                private static TrinityPower CastAcidCloud(TrinityActor target)
//                    => new TrinityPower(Skills.WitchDoctor.AcidCloud.SNOPower, 45f, target.AcdId);

//                private static TrinityPower CastFirebomb(TrinityActor target)
//                    => new TrinityPower(Skills.WitchDoctor.Firebomb.SNOPower, 45f, target.AcdId);

//                #endregion

//            }
//        }
//    }
//}