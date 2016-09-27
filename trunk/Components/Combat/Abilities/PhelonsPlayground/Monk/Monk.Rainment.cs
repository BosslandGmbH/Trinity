//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Trinity.Config.Combat;
//using Trinity.Framework;
//using Trinity.Framework.Actors.ActorTypes;
//using Trinity.Reference;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;

//namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.Monk
//{
//    partial class Monk
//    {
//        class Rainment
//        {
//            public static TrinityPower PowerSelector()
//            {
//                Vector3 dashLocation;

//                if (ShouldEpiphany)
//                    return CastEpiphany;

//                if (ShouldCastSerenity())
//                    return CastSerenity;

//                if (ShouldDashingStrike(out dashLocation))
//                    return CastDashingStrike(dashLocation);

//                if (ShouldCycloneStrike)
//                    return CastCycloneStrike;

//                if (ShouldBreathOfHeaven)
//                    return CastBreathOfHeaven;

//                TrinityPower buffPower;
//                if (CastMantra(out buffPower))
//                    return buffPower;

//                if (ShouldInnerSanctuary)
//                    return CastInnerSanctuary;

//                if (ShouldBlindingFlash)
//                    return CastBlindingFlash;

//                TrinityActor target = TargetUtil.BestTarget(25f, Player.IsInParty);
//                if (ShouldGenerate(out target))
//                    return CastGenerator(target);

//                if (target == null)
//                    return new TrinityPower(SNOPower.Walk, 3f, Player.Position);
//                return new TrinityPower(SNOPower.Walk, 3f, target.Position);
//            }

//            private static bool ShouldDashingStrike(out Vector3 bestDpsPos)
//            {
//                var target = TargetUtil.BestTarget(25f, Player.IsInParty);

//                var charges = Skills.Monk.DashingStrike.Charges;

//                bestDpsPos = target.Position;

//                if (!Skills.Monk.DashingStrike.CanCast() || 
//                    charges < 1 || SpellHistory.LastPowerUsed == Skills.Monk.DashingStrike.SNOPower ||
//                    charges < 2 && Player.PrimaryResource < 75)
//                    return false;

//                // Ports to Closest HealthGlobe
//                if (Core.Player.CurrentHealthPct < Settings.Combat.Monk.HealthGlobeLevel)
//                {
//                    var safePoint = TargetUtil.GetBestHealthGlobeClusterPoint(5, 45);
//                    if (safePoint != Vector3.Zero)
//                    {
//                        bestDpsPos = safePoint;
//                        return true;
//                    }
//                }

//                //Ports to best DPS location
//                //if (Core.Avoidance.InAvoidance(Player.Position) && Skills.Monk.DashingStrike.TimeSinceUse > 2000)
//                //{
//                //    var distance = Skills.Monk.CycloneStrike.CanCast() ? 20f : 12f;
//                //    bestDpsPos = TargetUtil.GetLoiterPosition(target, distance);
//                //    return true;
//                //}

//                var pullRange = Runes.Monk.Implosion.IsActive ? 30f : 20f;
//                if (!Player.IsInParty && Skills.Monk.DashingStrike.TimeSinceUse > 1500 &&
//                    Skills.Monk.CycloneStrike.CanCast() &&
//                    TargetUtil.BestBuffPosition(pullRange, target.Position, Player.IsInParty, out bestDpsPos) &&
//                    bestDpsPos.Distance(Player.Position) < 45 && bestDpsPos.Distance(Player.Position) > 6f &&
//                    !target.IsBoss)
//                    return true;
//                var useTimer = target.Distance <= 10f ? 3500 : 1500;
//                if (Skills.Monk.DashingStrike.TimeSinceUse < useTimer)
//                    return false;

//                target = TargetUtil.GetClosestUnit(25f);

//                if (target == null) return false;

//                bestDpsPos = target.Position;
//                return true;
//            }

//            private static bool ShouldCastSerenity()
//            {
//                return (Player.CurrentHealthPct <= Settings.Combat.Monk.SerenityHealthPct || Player.IsIncapacitated && Player.CurrentHealthPct <= 0.90) && Skills.Monk.Serenity.CanCast();
//            }

//            private static TrinityPower CastDashingStrike(Vector3 location)
//            {
//                return new TrinityPower(SNOPower.X1_Monk_DashingStrike, 50, location);
//            }

//            private static bool ShouldBreathOfHeaven
//            {
//                get { return Skills.Monk.BreathOfHeaven.CanCast(); }
//            }

//            private static TrinityPower CastBreathOfHeaven
//            {
//                get { return new TrinityPower(Skills.Monk.BreathOfHeaven.SNOPower); }

//            }

//            private static bool ShouldInnerSanctuary
//            {
//                get
//                {
//                    return Skills.Monk.InnerSanctuary.CanCast() &&
//                           TargetUtil.BestAoeUnit(25, Player.IsInParty).Distance < 20;
//                }
//            }

//            private static TrinityPower CastInnerSanctuary
//            {
//                get { return new TrinityPower(SNOPower.X1_Monk_InnerSanctuary); }

//            }

//            private static bool ShouldBlindingFlash
//            {
//                get
//                {
//                    return Skills.Monk.BlindingFlash.CanCast() && Skills.Monk.BlindingFlash.TimeSinceUse > 2750 &&
//                           TargetUtil.BestAoeUnit(25, Player.IsInParty).Distance < 20;
//                }
//            }

//            private static TrinityPower CastBlindingFlash
//            {
//                get { return new TrinityPower(SNOPower.Monk_BlindingFlash); }

//            }

//            private static bool CastMantra(out TrinityPower buffPower)
//            {
//                buffPower = null;
//                if (Player.PrimaryResource >= 0.99)
//                {
//                    if (Skills.Monk.MantraOfConviction.CanCast() &&
//                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfConviction_v2) > 2750)
//                    {
//                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfConviction_v2, 3);
//                        return true;
//                    }
//                    if (Skills.Monk.MantraOfRetribution.CanCast() &&
//                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfRetribution_v2) > 2750)
//                    {
//                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfRetribution_v2, 3);
//                        return true;
//                    }
//                    if (Skills.Monk.MantraOfHealing.CanCast() &&
//                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfHealing_v2) > 2750)
//                    {
//                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfHealing_v2, 3);
//                        return true;
//                    }
//                    if (Skills.Monk.MantraOfSalvation.CanCast() &&
//                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfEvasion_v2) > 2750)
//                    {
//                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfEvasion_v2, 3);
//                        return true;
//                    }
//                }
//                return false;
//            }

//            private static bool ShouldEpiphany
//            {
//                get
//                {
//                    if (!Skills.Monk.Epiphany.CanCast() || GetHasBuff(SNOPower.X1_Monk_Epiphany))
//                        return false;

//                    // Epiphany mode is 'Off Cooldown'
//                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.WhenReady)
//                        return true;

//                    // Let's check for Goblins, Current Health, CDR Pylon, movement impaired
//                    if (CurrentTarget.IsTreasureGoblin && Settings.Combat.Monk.UseEpiphanyGoblin ||
//                        Player.CurrentHealthPct <= 0.39 &&
//                        Settings.Combat.Monk.EpiphanyEmergencyHealth || GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting) ||
//                        ZetaDia.Me.IsFrozen || ZetaDia.Me.IsRooted || ZetaDia.Me.IsFeared || ZetaDia.Me.IsStunned)
//                        return true;

//                    // Epiphany mode is 'Whenever in Combat'
//                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.WhenInCombat &&
//                        TargetUtil.AnyMobsInRange(12f))
//                        return true;

//                    // Epiphany mode is 'Use when Elites are nearby'
//                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.Normal && TargetUtil.AnyElitesInRange(12f))
//                        return true;

//                    // Epiphany mode is 'Hard Elites Only'
//                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.HardElitesOnly && HardElitesPresent && (
//                        Player.CurrentHealthPct < Settings.Combat.Misc.HealthGlobeLevel || Player.IsIncapacitated))
//                        return true;

//                    return false;
//                }
//            }

//            private static TrinityPower CastEpiphany
//            {
//                get { return new TrinityPower(SNOPower.X1_Monk_Epiphany); }
//            }

//            private static TrinityPower CastSerenity
//            {
//                get { return new TrinityPower(SNOPower.Monk_Serenity); }
//            }

//            private static bool ShouldCycloneStrike
//            {
//                get
//                {
//                    var target = TargetUtil.BestTarget(25f, Player.IsInParty);
//                    var pullRange = Runes.Monk.Implosion.IsActive ? 30f : 20f;
//                    return Skills.Monk.CycloneStrike.CanCast() &&
//                           (Skills.Monk.CycloneStrike.TimeSinceUse > 3750 ||
//                            target.Distance > 12f && target.Distance < pullRange &&
//                            Skills.Monk.CycloneStrike.TimeSinceUse > 1500 ||
//                            SpellHistory.LastPowerUsed == SNOPower.X1_Monk_DashingStrike);
//                }
//            }

//            private static TrinityPower CastCycloneStrike
//            {
//                get { return new TrinityPower(SNOPower.Monk_CycloneStrike); }

//            }

//            private static bool ShouldGenerate(out TrinityActor target)
//            {
//                target = null;

//                if (!Skills.Monk.CripplingWave.CanCast() && !Skills.Monk.WayOfTheHundredFists.CanCast() &&
//                    !Skills.Monk.DeadlyReach.CanCast() && !Skills.Monk.FistsOfThunder.CanCast())
//                    return false;

//                target = TargetUtil.BestAoeUnit(25f, Player.IsInParty);
//                if (target != null && target.Distance < 12f)
//                    return true;

//                target = TargetUtil.GetClosestUnit(12f);
//                return target != null;
//            }

//            private static TrinityPower CastGenerator(TrinityActor target)
//            {
//                //Legendary.ConventionOfElements.IsEquipped
//                if (Skills.Monk.WayOfTheHundredFists.IsActive && (IsInsideCoeTimeSpan(Element.Physical, 250, 0) ||
//                    !GetHasBuff(Skills.Monk.WayOfTheHundredFists.SNOPower) ||
//                    //!GetHasBuff(SNOPower.P2_ItemPassive_Unique_Ring_033) ||
//                    Skills.Monk.WayOfTheHundredFists.TimeSinceUse > 4250))
//                    return new TrinityPower(Skills.Monk.WayOfTheHundredFists.SNOPower, 12f, target.AcdId);
//                return new TrinityPower(Skills.Monk.CripplingWave.SNOPower, 12f, target.AcdId);
//            }
//        }
//    }
//}