using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Config.Combat;
using Trinity.Reference;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Monk
{
    partial class Monk
    {
        internal class ZDps
        {
            public static TrinityPower PowerSelector()
            {
                if (Player.IsIncapacitated) return null;
                
                TrinityCacheObject target;
                if (ShouldDashingStrike(out target))
                    return CastDashingStrike(target);

                if (ShouldEpiphany)
                    return CastEpiphany;

                TrinityPower buffPower;
                if (CastMantra(out buffPower))
                    return buffPower;

                if (ShouldInnerSanctuary)
                    return CastInnerSanctuary;

                if (ShouldBlindingFlash)
                    return CastBlindingFlash;
                if (ShouldCripplingWave(out target))
                    return CastCripplingWave(target);

                if (ShouldCycloneStrike)
                    return CastCycloneStrike;

                return null;
            }

            private static bool ShouldDashingStrike(out TrinityCacheObject target)
            {
                target = null;

                var charges = Skills.Monk.DashingStrike.Charges;

                if (!Skills.Monk.DashingStrike.CanCast() ||
                    TrinityPlugin.ShouldWaitForLootDrop || charges < 1)
                    return false;

                target = PhelonTargeting.BestAoeUnit(false);
                if ((target == null || target.Distance > 50 || target.Distance < 10) &&
                    TimeSincePowerUse(SNOPower.X1_Monk_DashingStrike) < Settings.Combat.Monk.DashingStrikeDelay)
                    return false;

                if (Sets.ThousandStorms.IsSecondBonusActive &&
                    (charges > 1 && TrinityPlugin.Player.PrimaryResource >= 75 ||
                     CacheData.BuffsCache.Instance.HasCastingShrine))
                    return true;

                if (!Sets.ThousandStorms.IsSecondBonusActive &&
                    (TargetUtil.AnyMobsInRange(60f) || CacheData.BuffsCache.Instance.HasCastingShrine))
                    return true;
                return false;
            }

            private static TrinityPower CastDashingStrike(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.X1_Monk_DashingStrike, 65, target.ACDGuid);
            }

            private static bool ShouldInnerSanctuary
            {
                get
                {
                    return Skills.Monk.InnerSanctuary.CanCast() &&
                           PhelonTargeting.BestAoeUnit(false).Distance < 20 && Player.CurrentHealthPct <= 0.4;
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
                    return Skills.Monk.BlindingFlash.CanCast() &&
                           PhelonTargeting.BestAoeUnit(false).Distance < 20 && Player.CurrentHealthPct <= 0.4;
                }
            }

            private static TrinityPower CastBlindingFlash
            {
                get { return new TrinityPower(SNOPower.Monk_BlindingFlash); }

            }

            private static bool CastMantra(out TrinityPower buffPower)
            {
                buffPower = null;
                if (Player.PrimaryResource >= 40)
                {
                    if (Skills.Monk.MantraOfConviction.CanCast() &&
                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfConviction_v2) > 3500)
                    {
                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfConviction_v2, 3);
                        return true;
                    }
                    if (Skills.Monk.MantraOfRetribution.CanCast() &&
                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfRetribution_v2) > 3500)
                    {
                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfRetribution_v2, 3);
                        return true;
                    }
                    if (Skills.Monk.MantraOfHealing.CanCast() &&
                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfHealing_v2) > 3500)
                    {
                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfHealing_v2, 3);
                        return true;
                    }
                    if (Skills.Monk.MantraOfSalvation.CanCast() &&
                        TimeSincePowerUse(SNOPower.X1_Monk_MantraOfEvasion_v2) > 3500)
                    {
                        buffPower = new TrinityPower(SNOPower.X1_Monk_MantraOfEvasion_v2, 3);
                        return true;
                    }
                }
                return false;
            }

            private static
                bool ShouldEpiphany
            {
                get
                {
                    if (!Skills.Monk.Epiphany.CanCast() ||  GetHasBuff(SNOPower.X1_Monk_Epiphany))
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
                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.WhenInCombat && TargetUtil.AnyMobsInRange(80f))
                        return true;

                    // Epiphany mode is 'Use when Elites are nearby'
                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.Normal && TargetUtil.AnyElitesInRange(80f))
                        return true;

                    // Epiphany mode is 'Hard Elites Only'
                    if (Settings.Combat.Monk.EpiphanyMode == MonkEpiphanyMode.HardElitesOnly && HardElitesPresent)
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
                    var cycloneStrikeRange = Runes.Monk.Implosion.IsActive ? 34f : 24f;
                    var cycloneStrikeSpirit = Runes.Monk.EyeOfTheStorm.IsActive ? 80 : 100;
                    return Skills.Monk.CycloneStrike.CanCast() && Player.PrimaryResource > cycloneStrikeSpirit &&
                            (PhelonUtils.MobsBetweenRange(13, cycloneStrikeRange).Count > 3 ||
                             TimeSincePowerUse(SNOPower.Monk_CycloneStrike) > Settings.Combat.Monk.CycloneStrikeDelay);
                }
            }

            private static TrinityPower CastCycloneStrike
            {
                get { return new TrinityPower(SNOPower.Monk_CycloneStrike); }

            }

            private static bool ShouldCripplingWave(out TrinityCacheObject target)
            {
                target = null;
                if (!Skills.Monk.CripplingWave.CanCast())
                    return false;
                target = PhelonUtils.BestAuraUnit(SNOPower.Monk_CripplingWave, 12, true);
                return target != null;
            }

            private static TrinityPower CastCripplingWave(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Monk_CripplingWave, 65, target.ACDGuid);
            }
        }
    }
}