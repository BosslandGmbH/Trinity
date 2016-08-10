using Trinity.Framework.Actors.ActorTypes;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.WitchDoctor
{
    using System;
    using Technicals;

    partial class WitchDoctor
    {
        partial class Helltooth
        {
            public class AcidCloud
            {
                public static TrinityPower PowerSelector()
                {
                    if (Player.IsIncapacitated) return null;
                    TrinityActor target;

                    if (ShouldSoulHarvest)
                        return SoulHarvest;
                    var bestDpsPosition =
                        PhelonUtils.BestDpsPosition(PhelonTargeting.BestAoeUnit(45, true).Position, 45f, true);
                    if (GetHasBuff(SNOPower.Witchdoctor_SpiritWalk))
                    {
                        if (Player.CurrentHealthPct < Settings.Combat.Misc.HealthGlobeLevel &&
                            PhelonUtils.BestWalkLocation(35f, true).Distance(Player.Position) > 5)
                            return new TrinityPower(SNOPower.Walk, 3f, PhelonUtils.BestWalkLocation(45f, true));

                        if (bestDpsPosition.Distance2D(Player.Position) > 5f)
                            return new TrinityPower(SNOPower.Walk, 3f, bestDpsPosition);
                    }
                    else if (PhelonUtils.UnitsBetweenLocations(Player.Position, bestDpsPosition).Count < 3 && bestDpsPosition.Distance2D(Player.Position) > 5f)
                            return new TrinityPower(SNOPower.Walk, 3f, bestDpsPosition);
                    //if (!PhelonTargeting.BestAoeUnit().HasDebuff(SNOPower.Witchdoctor_Locust_Swarm))
                    //    return new TrinityPower(SNOPower.Walk, 20f, PhelonTargeting.BestAoeUnit().Position);

                    if (ShouldWallOfDeath(out target))
                        return WallOfDeath(target);

                    if (ShouldHaunt(out target))
                        return CastHaunt(target);

                    if (ShouldPiranhas(out target))
                        return Piranhas(target);

                    if (ShouldAcidCloud(out target))
                        return CastAcidCloud(target);

                    if (ShouldGenerate(out target))
                        return CastGenerate(target);
                    
                    if (PhelonUtils.BestDpsPosition(Player.Position, 45f, true).Distance(Player.Position) > 7f)
                        return new TrinityPower(SNOPower.Walk, 3f,
                            PhelonUtils.BestDpsPosition(Player.Position, 45f, true));
                    return new TrinityPower(SNOPower.Walk, 3f, Player.Position);
                }

                #region Conditions

                private static bool ShouldAcidCloud(out TrinityActor target)
                {
                    target = null;

                    if (!Skills.WitchDoctor.AcidCloud.CanCast())
                        return false;

                    target = PhelonTargeting.BestAoeUnit(35, true);

                    return target != null;
                }

                private static bool ShouldGenerate(out TrinityActor target)
                {
                    target = null;
                    if (Skills.WitchDoctor.Firebomb.IsActive && !Skills.WitchDoctor.PlagueOfToads.IsActive &&
                        !Skills.WitchDoctor.CorpseSpiders.IsActive && !Skills.WitchDoctor.PoisonDart.IsActive)
                        return false;

                    target = PhelonTargeting.BestAoeUnit(55f, true);
                    if (target != null)
                        return true;

                    target = TargetUtil.GetClosestUnit(55f);
                    return target != null;
                }

                private static bool ShouldPiranhas(out TrinityActor target)
                {
                    target = null;

                    if (!Skills.WitchDoctor.Piranhas.CanCast())
                        return false;

                    target = Runes.WitchDoctor.WaveOfMutilation.IsActive
                        ? PhelonUtils.GetBestPierceTarget(25, true)
                        : PhelonTargeting.BestAoeUnit(55f, true);

                    return target != null;
                }

                private static bool ShouldWallOfDeath(out TrinityActor target)
                {
                    target = null;

                    if (!Skills.WitchDoctor.WallOfDeath.CanCast())
                        return false;
                    if (Skills.WitchDoctor.WallOfDeath.TimeSinceUse < 2500)
                        return false;
                    target = PhelonTargeting.BestAoeUnit(55f, true);

                    return target != null;
                }

                private static bool ShouldSoulHarvest => CanCast(SNOPower.Witchdoctor_SoulHarvest) &&
                                                         (TargetUtil.AnyMobsInRange(18f, 3) ||
                                                          TargetUtil.AnyBossesInRange(18f) ||
                                                          TargetUtil.AnyElitesInRange(18f));

                private static bool ShouldHaunt(out TrinityActor target)
                {
                    target = null;

                    if (!Skills.WitchDoctor.Haunt.CanCast())
                        return false;

                    if (SpellHistory.LastPowerUsed == Skills.WitchDoctor.Haunt.SNOPower)
                        return false;

                    target = PhelonUtils.BestAuraUnit(SNOPower.Witchdoctor_Haunt, 55f, true);

                    return target != null;
                }

                #endregion

                #region Expressions

                private static TrinityPower SoulHarvest => new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);

                private static TrinityPower Piranhas(TrinityActor target)
                    => new TrinityPower(SNOPower.Witchdoctor_Piranhas, 55f, target.Position);

                private static TrinityPower WallOfDeath(TrinityActor target)
                    => new TrinityPower(SNOPower.Witchdoctor_WallOfZombies, 55f, target.Position);

                private static TrinityPower CastHaunt(TrinityActor target)
                    => new TrinityPower(SNOPower.Witchdoctor_Haunt, 55f, target.AcdId);
                private static TrinityPower CastAcidCloud(TrinityActor target)
                    => new TrinityPower(Skills.WitchDoctor.AcidCloud.SNOPower, 55f, target.AcdId);

                private static TrinityPower CastGenerate(TrinityActor target)
                {
                    if (Skills.WitchDoctor.Firebomb.IsActive)
                        return new TrinityPower(Skills.WitchDoctor.Firebomb.SNOPower, 55f, target.AcdId);
                    if (Skills.WitchDoctor.PlagueOfToads.IsActive)
                        return new TrinityPower(Skills.WitchDoctor.PlagueOfToads.SNOPower, 55f, target.AcdId);
                    if (Skills.WitchDoctor.CorpseSpiders.IsActive)
                        return new TrinityPower(Skills.WitchDoctor.CorpseSpiders.SNOPower, 55f, target.AcdId);
                    if (Skills.WitchDoctor.PoisonDart.IsActive)
                        return new TrinityPower(Skills.WitchDoctor.PoisonDart.SNOPower, 55f, target.AcdId);
                    return null;
                }

                #endregion

            }
        }
    }
}