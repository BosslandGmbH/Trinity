using Trinity.Framework;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.Crusader
{
    partial class Crusader
    {
        internal class LegacyOfNightmares
        {
            public static TrinityPower PowerSelector()
            {
                if (ShouldLawsOfJustice)
                    return CastLawsOfJustice;

                if (ShouldCondemn)
                    return CastCondemn;

                //Make sure we cast Bombardment when IronSkin and CoE is Up.
                if (GetHasBuff(SNOPower.X1_Crusader_IronSkin) && ShouldBombardment)
                    return CastBombardment;

                //Wait for CoE to Cast Damage CD's
                if (!Settings.Combat.Misc.UseConventionElementOnly ||
                    !ShouldWaitForConventionofElements(Skills.Crusader.IronSkin, Element.Physical, 1500, 1000))
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
                }
                if (ShouldSteedCharge)
                    return CastSteedCharge;

                if (!IsCurrentlyAvoiding)
                {
                    //Logger.Log("Steed Charge Damage");
                    return PhelonTargeting.BestAoeUnit(45, true).Distance < 15
                        ? new TrinityPower(SNOPower.Walk, 7f,
                            TargetUtil.GetZigZagTarget(PhelonTargeting.BestAoeUnit(45, true).Position, 15f, false),
                            Trinity.TrinityPlugin.CurrentWorldDynamicId,
                            -1, 0, 1)
                        : new TrinityPower(SNOPower.Walk, 3f, PhelonTargeting.BestAoeUnit(45, true).Position);
                }
                return null;
            }

            private static bool ShouldLawsOfJustice
            {
                get
                {
                    //Return false if we have buff or can't cast.
                    if (!Skills.Crusader.LawsOfJustice.CanCast() ||
                        Core.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfJustice_Passive2, 6))
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
                           ((CurrentTarget.IsElite && CurrentTarget.RadiusDistance <= 15f) ||
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
                    return TargetUtil.AnyMobsInRange(20f, Settings.Combat.Crusader.BombardmentAoECount) ||
                            TargetUtil.AnyElitesInRange(20f);
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