using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Config.Combat;
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
                //Wait for CoE to Cast Damage CD's
                if (!Settings.Combat.Misc.UseConventionElementOnly ||
                    !ShouldWaitForConventionofElements(Skills.Crusader.Bombardment, Element.Physical, 1500, 1000))
                {
                    //Make sure we cast Bombardment when IronSkin and CoE is Up.
                    if (GetHasBuff(SNOPower.X1_Crusader_IronSkin) && ShouldBombardment)
                        return CastBombardment;

                    //Only Cast if not Steed Charging
                    if (!IsSteedCharging)
                    {
                        if (ShouldCondemn)
                            return CastCondemn;

                        if (ShouldShieldGlare)
                            return CastShieldGlare;

                        if (ShouldIronSkin)
                            return CastIronSkin;

                        if (ShouldConsecration)
                            return CastConsecration;
                    }
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

            private static bool ShouldCondemn
            {
                get { return Skills.Crusader.Condemn.CanCast(); }
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