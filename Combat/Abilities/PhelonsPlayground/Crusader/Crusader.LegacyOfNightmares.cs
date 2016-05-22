using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Config.Combat;
using Trinity.Movement;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Crusader
{
    partial class Crusader
    {
        internal class LegacyOfNightmares
        {
            public static TrinityPower PowerSelector()
            {
                if (ShouldAkaratsChampion)
                    return CastAkaratsChampion;

                if (ShouldLawsOfJustice)
                    return CastLawsOfJustice;

                if (ShouldCondemn)
                    return CastCondemn;

                //Wait for CoE to Cast Damage CD's
                if (!Settings.Combat.Misc.UseConventionElementOnly ||
                    !ShouldWaitForConventionofElements(Skills.Crusader.Bombardment, Element.Physical, 1500, 1000))
                {

                    if (ShouldIronSkin)
                        return CastIronSkin;

                    //Only Cast if not Steed Charging
                    if (GetHasBuff(SNOPower.X1_Crusader_IronSkin))
                    {
                        if (ShouldShieldGlare)
                            return CastShieldGlare;

                        if (ShouldConsecration)
                            return CastConsecration;
                    }

                    //Make sure we cast Bombardment when IronSkin and CoE is Up.
                    if (GetHasBuff(SNOPower.X1_Crusader_IronSkin) && ShouldBombardment)
                        return CastBombardment;
                }
                if (ShouldSteedCharge)
                    return CastSteedCharge;

                if (!IsCurrentlyAvoiding)
                {
                    //Logger.Log("Steed Charge Damage");
                    return PhelonTargeting.BestAoeUnit(45, true).Distance < 10
                        ? new TrinityPower(SNOPower.Walk, 20f,
                            TargetUtil.GetZigZagTarget(PhelonTargeting.BestAoeUnit(45, true).Position, 15f, false), TrinityPlugin.CurrentWorldDynamicId,
                            -1, 0, 1)
                        : new TrinityPower(SNOPower.Walk, 3f, PhelonTargeting.BestAoeUnit(45, true).Position);
                }
                return null;
            }


            private static bool ShouldAkaratsChampion
            {
                get
                {
                    //Basic checks
                    if (!Skills.Crusader.AkaratsChampion.CanCast())
                        return false;

                    // Akarat's mode is 'Off Cooldown'
                    if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.WhenReady)
                        return true;
                    //Use on Low Health
                    if (Player.CurrentHealthPct <= 0.25 &&
                        (Settings.Combat.Crusader.AkaratsEmergencyHealth || Runes.Crusader.Prophet.IsActive))
                        return true;
                    //Use if Incapacitated
                    if (Settings.Combat.Crusader.AkaratsOnStatusEffect &&
                        (ZetaDia.Me.IsFrozen || ZetaDia.Me.IsRooted || ZetaDia.Me.IsFeared || ZetaDia.Me.IsStunned))
                        return true;

                    if (!IsSteedCharging || ClassMover.HasInfiniteCasting)
                    {
                        // Let's check for Goblins, Current Health, CDR Pylon, movement impaired
                        if (CurrentTarget != null && CurrentTarget.IsTreasureGoblin)
                            return true;

                        // Akarat's mode is 'Whenever in Combat'
                        if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.WhenInCombat &&
                            TargetUtil.AnyMobsInRange(40f))
                            return true;

                        // Akarat's mode is 'Use when Elites are nearby'
                        if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.Normal &&
                            TargetUtil.AnyElitesInRange(40f))
                            return true;

                        // Akarat's mode is 'Hard Elites Only'
                        if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.HardElitesOnly &&
                            HardElitesPresent)
                            return true;
                    }
                    return false;
                }
            }

            private static TrinityPower CastAkaratsChampion
            {
                get { return new TrinityPower(Skills.Crusader.AkaratsChampion.SNOPower); }
            }

            private static bool ShouldLawsOfJustice
            {
                get
                {
                    //Return false if we have buff or can't cast.
                    if (!Skills.Crusader.LawsOfJustice.CanCast() ||
                        CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfJustice_Passive2, 6))
                        return false;
                    //Use if Low Health
                    if (Player.CurrentHealthPct <= Settings.Combat.Crusader.LawsOfJusticeHpPct)
                        return true;
                    //Use if not steed charging.
                    if (!IsSteedCharging)
                    {
                        return TargetUtil.EliteOrTrashInRange(16f) ||
                                TargetUtil.AnyMobsInRange(15f, 5) ||
                                Settings.Combat.Crusader.SpamLaws;
                    }
                    return false;
                }
            }

            private static TrinityPower CastLawsOfJustice
            {
                get { return new TrinityPower(Skills.Crusader.LawsOfJustice.SNOPower); }
            }
            private static bool ShouldCondemn
            {
                get { return Skills.Crusader.Condemn.CanCast() && !IsSteedCharging; }
            }

            private static TrinityPower CastCondemn
            {
                get { return new TrinityPower(Skills.Crusader.Condemn.SNOPower); }
            }

            private static bool ShouldIronSkin
            {
                get { return Skills.Crusader.IronSkin.CanCast(); }
            }

            private static TrinityPower CastIronSkin
            {
                get { return new TrinityPower(Skills.Crusader.IronSkin.SNOPower); }
            }

            private static bool ShouldShieldGlare
            {
                get
                {
                    return Skills.Crusader.ShieldGlare.CanCast() &&
                           ((CurrentTarget.IsBossOrEliteRareUnique && CurrentTarget.RadiusDistance <= 15f) ||
                            TargetUtil.UnitsPlayerFacing(16f) >= Settings.Combat.Crusader.ShieldGlareAoECount);
                }
            }

            private static TrinityPower CastShieldGlare
            {
                get { return new TrinityPower(Skills.Crusader.ShieldGlare.SNOPower); }
            }

            private static bool ShouldConsecration
            {
                get { return Skills.Crusader.Consecration.CanCast(); }
            }

            private static TrinityPower CastConsecration
            {
                get { return new TrinityPower(Skills.Crusader.Consecration.SNOPower); }
            }

            private static bool ShouldBombardment
            {
                get
                {
                    if (!Skills.Crusader.Bombardment.CanCast())
                        return false;

                    if (ShouldWaitForConventionofElements(Skills.Crusader.Bombardment, Element.Physical, 1500, 1000))
                    {
                        Logger.LogVerbose("Bombardment Waiting for convention element");
                        return false;
                    }

                    if (CanCast(SNOPower.X1_Crusader_IronSkin) && !GetHasBuff(SNOPower.X1_Crusader_IronSkin))
                    {
                        Logger.LogVerbose("Bombardment Waiting for iron skin");
                        Skills.Crusader.IronSkin.Cast();
                        return false;
                    }

                    if (Math.Abs(ZetaDia.Me.Movement.SpeedXY) < Single.Epsilon)
                    {
                        Logger.LogVerbose("Waiting to move for bombard with hexing pants.");
                        return false;
                    }
                    return TargetUtil.AnyMobsInRange(60f, Settings.Combat.Crusader.BombardmentAoECount) ||
                            TargetUtil.AnyElitesInRange(60f);
                }
            }

            private static TrinityPower CastBombardment
            {
                get { return new TrinityPower(Skills.Crusader.Bombardment.SNOPower); }
            }

            private static bool ShouldSteedCharge
            {
                get { return Skills.Crusader.SteedCharge.CanCast() && !IsSteedCharging; }
            }

            private static TrinityPower CastSteedCharge
            {
                get { return new TrinityPower(Skills.Crusader.SteedCharge.SNOPower); }
            }
        }
    }
}