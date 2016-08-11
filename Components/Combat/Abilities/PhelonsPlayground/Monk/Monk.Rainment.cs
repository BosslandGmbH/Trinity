using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Config.Combat;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.Monk
{
    partial class Monk
    {
        class Rainment
        {
            public static TrinityPower PowerSelector()
            {
                if (Player.IsIncapacitated) return null;

                Vector3 dashLocation;
                if (ShouldDashingStrike(out dashLocation))
                    return CastDashingStrike(dashLocation);

                if (ShouldEpiphany)
                    return CastEpiphany;

                if (ShouldBreathOfHeaven)
                    return CastBreathOfHeaven;

                TrinityPower buffPower;
                if (CastMantra(out buffPower))
                    return buffPower;

                if (ShouldInnerSanctuary)
                    return CastInnerSanctuary;

                if (ShouldBlindingFlash)
                    return CastBlindingFlash;

                if (ShouldCycloneStrike)
                    return CastCycloneStrike;

                TrinityActor target;
                if (ShouldCripplingWave(out target))
                    return CastCripplingWave(target);

                return null;
            }

            private static bool ShouldDashingStrike(out Vector3 bestDpsPos)
            {
                var target = PhelonTargeting.BestTarget(45f, true);

                bestDpsPos = PhelonUtils.BestDpsPosition(target.Position);
                var charges = Skills.Monk.DashingStrike.Charges;

                if (!Skills.Monk.DashingStrike.CanCast() ||
                    CombatManager.TargetHandler.ShouldWaitForLootDrop || charges < 1)
                    return false;

                // Ports to Closest HealthGlobe
                if (Core.Player.CurrentHealthPct < Settings.Combat.Monk.HealthGlobeLevel)
                {
                    var safePoint = TargetUtil.GetBestHealthGlobeClusterPoint(5, 40);
                    if (safePoint != Vector3.Zero)
                    {
                        bestDpsPos = safePoint;
                        return true;
                    }
                }

                //Ports to best DPS location
                //if (Core.Avoidance.InAvoidance(Player.Position) && Skills.Monk.DashingStrike.TimeSinceUse > 2000)
                //{
                //    var distance = Skills.Monk.CycloneStrike.CanCast() ? 20f : 12f;
                //    bestDpsPos = TargetUtil.GetLoiterPosition(target, distance);
                //    return true;
                //}

                if (Skills.Monk.DashingStrike.TimeSinceUse < 3500)
                    return false;

                if (bestDpsPos != Vector3.Zero && bestDpsPos.Distance(Player.Position) < 50)
                {
                    return true;
                }

                target = TargetUtil.GetClosestUnit(50f);

                if (target == null) return false;

                bestDpsPos = target.Position;
                return true;
            }

            private static TrinityPower CastDashingStrike(Vector3 location)
            {
                return new TrinityPower(SNOPower.X1_Monk_DashingStrike, 50, location);
            }

            private static bool ShouldBreathOfHeaven
            {
                get
                {
                    return Skills.Monk.BreathOfHeaven.CanCast();
                }
            }

            private static TrinityPower CastBreathOfHeaven
            {
                get { return new TrinityPower(Skills.Monk.BreathOfHeaven.SNOPower); }

            }

            private static bool ShouldInnerSanctuary
            {
                get
                {
                    return Skills.Monk.InnerSanctuary.CanCast() &&
                           PhelonTargeting.BestAoeUnit(45, true).Distance < 20;
                }
            }

            private static TrinityPower CastInnerSanctuary
            {
                get { return new TrinityPower(SNOPower.X1_Monk_InnerSanctuary); }

            }

            private static bool ShouldBlindingFlash
            {
                get
                {
                    return Skills.Monk.BlindingFlash.CanCast() && Skills.Monk.BlindingFlash.TimeSinceUse > 2750 &&
                           PhelonTargeting.BestAoeUnit(45, true).Distance < 20;
                }
            }

            private static TrinityPower CastBlindingFlash
            {
                get { return new TrinityPower(SNOPower.Monk_BlindingFlash); }

            }

            private static bool CastMantra(out TrinityPower buffPower)
            {
                buffPower = null;
                if (Player.PrimaryResource >= 0.99)
                {
                    if (Skills.Monk.MantraOfConviction.CanCast() &&
                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfConviction_v2) > 2750)
                    {
                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfConviction_v2, 3);
                        return true;
                    }
                    if (Skills.Monk.MantraOfRetribution.CanCast() &&
                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfRetribution_v2) > 2750)
                    {
                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfRetribution_v2, 3);
                        return true;
                    }
                    if (Skills.Monk.MantraOfHealing.CanCast() &&
                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfHealing_v2) > 2750)
                    {
                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfHealing_v2, 3);
                        return true;
                    }
                    if (Skills.Monk.MantraOfSalvation.CanCast() &&
                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfEvasion_v2) > 2750)
                    {
                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfEvasion_v2, 3);
                        return true;
                    }
                }
                return false;
            }

            private static bool ShouldEpiphany
            {
                get
                {
                    if (!Skills.Monk.Epiphany.CanCast() || GetHasBuff(SNOPower.X1_Monk_Epiphany))
                        return false;

                    // Epiphany mode is 'Off Cooldown'
                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.WhenReady)
                        return true;

                    // Let's check for Goblins, Current Health, CDR Pylon, movement impaired
                    if (CurrentTarget.IsTreasureGoblin && Settings.Combat.Monk.UseEpiphanyGoblin ||
                        Player.CurrentHealthPct <= 0.39 &&
                        Settings.Combat.Monk.EpiphanyEmergencyHealth || GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting) ||
                        ZetaDia.Me.IsFrozen || ZetaDia.Me.IsRooted || ZetaDia.Me.IsFeared || ZetaDia.Me.IsStunned)
                        return true;

                    // Epiphany mode is 'Whenever in Combat'
                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.WhenInCombat && TargetUtil.AnyMobsInRange(12f))
                        return true;

                    // Epiphany mode is 'Use when Elites are nearby'
                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.Normal && TargetUtil.AnyElitesInRange(12f))
                        return true;

                    // Epiphany mode is 'Hard Elites Only'
                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.HardElitesOnly && HardElitesPresent && (
                        Player.CurrentHealthPct < Settings.Combat.Misc.HealthGlobeLevel || Player.IsIncapacitated))
                        return true;

                    return false;
                }
            }

            private static TrinityPower CastEpiphany
            {
                get { return new TrinityPower(SNOPower.X1_Monk_Epiphany); }

            }

            private static bool ShouldCycloneStrike
            {
                get
                {
                    return Skills.Monk.CycloneStrike.CanCast() && Player.PrimaryResource > cycloneStrikeSpirit && (Skills.Monk.CycloneStrike.TimeSinceUse > 3750 || SpellHistory.LastPowerUsed == SNOPower.X1_Monk_DashingStrike);
                }
            }

            private static TrinityPower CastCycloneStrike
            {
                get { return new TrinityPower(SNOPower.Monk_CycloneStrike); }

            }

            private static bool ShouldCripplingWave(out TrinityActor target)
            {
                target = null;

                if (!Skills.Monk.CripplingWave.CanCast())
                    return false;

                target = PhelonTargeting.BestAoeUnit(45, true);
                if (target != null && target.Distance < 12)
                    return true;

                target = TargetUtil.GetClosestUnit(12f);
                return target != null;
            }

            private static TrinityPower CastCripplingWave(TrinityActor target)
            {
                return new TrinityPower(SNOPower.Monk_CripplingWave, 12f, target.AcdId);
            }
        }
    }
}