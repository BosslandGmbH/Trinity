//using Trinity.Framework.Actors.ActorTypes;
//using Trinity.Reference;
//using Zeta.Game.Internals.Actors;

//namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.WitchDoctor
//{
//    using System;
//    using Zeta.Common;
//    using Logger = Technicals.Logger;

//    partial class WitchDoctor
//    {
//        partial class Helltooth
//        {
//            public class ColdGarg
//            {
//                public static TrinityPower PowerSelector()
//                {
//                    //if (Player.IsIncapacitated) return null;
//                    TrinityActor target;
                    
//                    var bestDpsTarget = TargetUtil.BestAoeUnit(35f, true);
//                    Vector3 bestDpsPosition;

//                    if (GetHasBuff(SNOPower.Witchdoctor_SpiritWalk))
//                    {
//                        if (Player.CurrentHealthPct < Settings.Combat.Misc.HealthGlobeLevel &&
//                            TargetUtil.ClosestGlobe(35f, true) != null)
//                            return new TrinityPower(SNOPower.Walk, 3f, TargetUtil.ClosestGlobe(35f, true).Position);

//                        if (TargetUtil.BestBuffPosition(35f, bestDpsTarget.Position, false, out bestDpsPosition) && bestDpsPosition.Distance2D(Player.Position) > 6f)
//                            return new TrinityPower(SNOPower.Walk, 3f, bestDpsPosition);
//                    }

//                    if (TargetUtil.BestBuffPosition(12f, bestDpsTarget.Position, false, out bestDpsPosition) &&
//                        (TargetUtil.UnitsBetweenLocations(Player.Position, bestDpsPosition).Count < 6 || Legendary.IllusoryBoots.IsEquipped) &&
//                        bestDpsPosition.Distance2D(Player.Position) > 6f)
//                        return new TrinityPower(SNOPower.Walk, 3f, bestDpsPosition);

//                    if (ShouldPiranhas(out target))
//                        return Piranhas(target);

//                    if (ShouldWallOfDeath(out target))
//                        return WallOfDeath(target);

//                    if (ShouldHaunt(out target))
//                        return CastHaunt(target);
                    
//                    return new TrinityPower(SNOPower.Walk, 3f, TargetUtil.GetLoiterPosition(bestDpsTarget, 25f));
//                }

//                #region Conditions

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

//                    target = TargetUtil.BestAoeUnit(35, true);

//                    return target != null;
//                }
                

//                private static bool ShouldHaunt(out TrinityActor target)
//                {
//                    target = null;

//                    if (!Skills.WitchDoctor.Haunt.CanCast())
//                        return false;

//                    if (SpellHistory.LastPowerUsed == Skills.WitchDoctor.Haunt.SNOPower)
//                        return false;

//                    target = TargetUtil.BestAuraUnit(SNOPower.Witchdoctor_Haunt, 35, true) ?? CurrentTarget;

//                    return target != null;
//                }

//                #endregion

//                #region Expressions

//                private static TrinityPower Piranhas(TrinityActor target)
//                    => new TrinityPower(SNOPower.Witchdoctor_Piranhas, 45, target.Position);

//                private static TrinityPower WallOfDeath(TrinityActor target)
//                    => new TrinityPower(SNOPower.Witchdoctor_WallOfZombies, 45, target.Position);

//                private static TrinityPower CastHaunt(TrinityActor target)
//                    => new TrinityPower(SNOPower.Witchdoctor_Haunt, 35f, target.AcdId);

//                #endregion

//            }
//        }
//    }
//}