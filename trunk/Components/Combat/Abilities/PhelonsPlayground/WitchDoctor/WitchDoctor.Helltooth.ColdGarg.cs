using Trinity.Framework.Actors.ActorTypes;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.WitchDoctor
{
    using System;
    using Zeta.Common;
    using Logger = Technicals.Logger;

    partial class WitchDoctor
    {
        partial class Helltooth
        {
            public class ColdGarg
            {
                public static TrinityPower PowerSelector()
                {
                    //if (Player.IsIncapacitated) return null;
                    TrinityActor target;

                    if (ShouldSoulHarvest)
                        return SoulHarvest;

                    var bestDpsTarget = PhelonTargeting.BestAoeUnit(25f, true);
                    Vector3 bestDpsPosition;

                    if (GetHasBuff(SNOPower.Witchdoctor_SpiritWalk))
                    {
                        if (Player.CurrentHealthPct < Settings.Combat.Misc.HealthGlobeLevel &&
                            PhelonUtils.ClosestGlobe(35f, true) != null)
                            return new TrinityPower(SNOPower.Walk, 3f, PhelonUtils.ClosestGlobe(35f, true).Position);

                        if (PhelonUtils.BestBuffPosition(35f, bestDpsTarget.Position, false, out bestDpsPosition) && bestDpsPosition.Distance2D(Player.Position) > 6f)
                            return new TrinityPower(SNOPower.Walk, 3f, bestDpsPosition);
                    }

                    if (PhelonUtils.BestBuffPosition(35f, bestDpsTarget.Position, false, out bestDpsPosition) &&
                        PhelonUtils.UnitsBetweenLocations(Player.Position, bestDpsPosition).Count < 3 &&
                        bestDpsPosition.Distance2D(Player.Position) > 6f)
                        return new TrinityPower(SNOPower.Walk, 3f, bestDpsPosition);

                    if (ShouldPiranhas(out target))
                        return Piranhas(target);

                    if (ShouldWallOfDeath(out target))
                        return WallOfDeath(target);

                    if (ShouldHaunt(out target))
                        return CastHaunt(target);
                    
                    return new TrinityPower(SNOPower.Walk, 3f, TargetUtil.GetLoiterPosition(bestDpsTarget, 25f));
                }

                #region Conditions

                private static bool ShouldPiranhas(out TrinityActor target)
                {
                    target = null;

                    if (!Skills.WitchDoctor.Piranhas.CanCast())
                        return false;

                    target = Runes.WitchDoctor.WaveOfMutilation.IsActive
                        ? PhelonUtils.GetBestPierceTarget(25, true)
                        : PhelonTargeting.BestAoeUnit(35, true);

                    return target != null;
                }

                private static bool ShouldWallOfDeath(out TrinityActor target)
                {
                    target = null;

                    if (!Skills.WitchDoctor.WallOfDeath.CanCast())
                        return false;

                    target = PhelonTargeting.BestAoeUnit(35, true);

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

                    target = PhelonUtils.BestAuraUnit(SNOPower.Witchdoctor_Haunt, 35, true) ?? CurrentTarget;

                    return target != null;
                }

                #endregion

                #region Expressions

                private static TrinityPower Walk => PhelonUtils.GetBestClusterUnit(18f, 30f) != null
                    ? new TrinityPower(SNOPower.Walk, 0f, PhelonUtils.GetBestClusterUnit(18f, 30f).Position)
                    : new TrinityPower(SNOPower.Walk, 5f, CurrentTarget.Position);

                private static TrinityPower SoulHarvest => new TrinityPower(SNOPower.Witchdoctor_SoulHarvest);

                private static TrinityPower Piranhas(TrinityActor target)
                    => new TrinityPower(SNOPower.Witchdoctor_Piranhas, 45, target.Position);

                private static TrinityPower WallOfDeath(TrinityActor target)
                    => new TrinityPower(SNOPower.Witchdoctor_WallOfZombies, 45, target.Position);

                private static TrinityPower CastHaunt(TrinityActor target)
                    => new TrinityPower(SNOPower.Witchdoctor_Haunt, 35f, target.AcdId);

                #endregion

            }
        }
    }
}