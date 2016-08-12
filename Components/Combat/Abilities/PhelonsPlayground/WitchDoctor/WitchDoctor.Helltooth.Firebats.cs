using Trinity.Framework.Actors.ActorTypes;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.WitchDoctor
{
    using System;
    using System.Linq;
    using Technicals;
    using Zeta.Common;
    using Logger = Technicals.Logger;

    partial class WitchDoctor
    {
        partial class Helltooth
        {
            public class Firebats
            {
                public static TrinityPower PowerSelector()
                {
                    //if (Player.IsIncapacitated) return null;
                    TrinityActor target;

                    var bestDpsTarget = PhelonTargeting.BestAoeUnit(25f, true);
                    Vector3 bestDpsPosition;

                    if (GetHasBuff(SNOPower.Witchdoctor_SpiritWalk))
                    {
                        if (Player.CurrentHealthPct < Settings.Combat.Misc.HealthGlobeLevel &&
                            PhelonUtils.ClosestGlobe(35f, true) != null)
                        {
                            Logger.LogNormal("Moving to GLOBE Location - Spirit Walk " + PhelonUtils.ClosestGlobe(35f, true).Position.Distance2D(Player.Position));
                            return new TrinityPower(SNOPower.Walk, 3f, PhelonUtils.ClosestGlobe(35f, true).Position);
                        }

                        if (PhelonUtils.BestBuffPosition(12f, bestDpsTarget.Position, true, out bestDpsPosition) && bestDpsPosition.Distance2D(Player.Position) > 6f)
                        {
                            Logger.LogNormal("Moving to DPS Location - Spirit Walk " + bestDpsPosition.Distance2D(Player.Position));
                            return new TrinityPower(SNOPower.Walk, 3f, bestDpsPosition);
                        }
                    }

                    if (PhelonUtils.BestBuffPosition(12f, bestDpsTarget.Position, false, out bestDpsPosition) &&
                        (PhelonUtils.UnitsBetweenLocations(Player.Position, bestDpsPosition).Count < 6 || Legendary.IllusoryBoots.IsEquipped) &&
                        bestDpsPosition.Distance2D(Player.Position) > 6f)
                    {
                        Logger.LogNormal("Moving to DPS Location - No Spirit Walk " + bestDpsPosition.Distance2D(Player.Position));
                        return new TrinityPower(SNOPower.Walk, 3f, bestDpsPosition);
                    }

                    if (ShouldWallOfDeath(out target))
                        return WallOfDeath(target);

                    if (ShouldPiranhas(out target))
                        return Piranhas(target);

                    if (Player.CurrentHealthPct > Math.Max(Settings.Combat.Misc.HealthGlobeLevel, 0.45))
                    {
                        if (ShouldHaunt(out target))
                            return CastHaunt(target);

                        if (ShouldLocustSwarm(out target))
                            return CastLocustSwarm(target);
                    }

                    if (ShouldFirebats(out target))
                        return CastFirebats;

                    if (ShouldGenerate(out target))
                        return CastGenerate(target);
                    
                    return new TrinityPower(SNOPower.Walk, 3f, target.Position);
                }

                #region Conditions

                private static bool ShouldFirebats(out TrinityActor target)
                {
                    target = null;

                    if (!Skills.WitchDoctor.Firebats.CanCast())
                        return false;
                    
                    return PhelonTargeting.BestAoeUnit(25f, true).Distance < 10f;
                }

                private static bool ShouldLocustSwarm(out TrinityActor target)
                {
                    target = null;

                    if (!Skills.WitchDoctor.LocustSwarm.CanCast())
                        return false;

                    if ((double)PhelonUtils.AuraUnits(Skills.WitchDoctor.LocustSwarm.SNOPower).Count /
                        PhelonUtils.SafeList(true).Count(x => x.Distance <= 12f) > 0.80)
                        return false;

                    target = PhelonUtils.BestAuraUnit(SNOPower.Witchdoctor_Haunt, 15f, true);

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
                    if (Skills.WitchDoctor.WallOfDeath.TimeSinceUse < 12000)
                        return false;
                    target = PhelonTargeting.BestAoeUnit(55f, true);

                    return target != null;
                }

                private static bool ShouldHaunt(out TrinityActor target)
                {
                    target = null;

                    if (!Skills.WitchDoctor.Haunt.CanCast())
                        return false;
                    if ((double) PhelonUtils.AuraUnits(Skills.WitchDoctor.Haunt.SNOPower).Count/
                        PhelonUtils.SafeList(true).Count(x => x.Distance <= 12f) > 0.80)
                        return false;
                    target = PhelonUtils.BestAuraUnit(SNOPower.Witchdoctor_Haunt, 15f, true);

                    return target != null;
                }

                #endregion

                #region Expressions


                private static TrinityPower Piranhas(TrinityActor target)
                    => new TrinityPower(SNOPower.Witchdoctor_Piranhas, 55f, target.Position);

                private static TrinityPower WallOfDeath(TrinityActor target)
                    => new TrinityPower(SNOPower.Witchdoctor_WallOfZombies, 55f, target.Position);

                private static TrinityPower CastHaunt(TrinityActor target)
                    => new TrinityPower(SNOPower.Witchdoctor_Haunt, 55f, target.AcdId);
                private static TrinityPower CastLocustSwarm(TrinityActor target)
                    => new TrinityPower(Skills.WitchDoctor.LocustSwarm.SNOPower, 55f, target.AcdId);

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
                private static TrinityPower CastFirebats => new TrinityPower(Skills.WitchDoctor.Firebats.SNOPower);

                #endregion

            }
        }
    }
}