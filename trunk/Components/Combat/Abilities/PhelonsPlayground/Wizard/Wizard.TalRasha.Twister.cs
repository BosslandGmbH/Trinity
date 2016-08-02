using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.Wizard
{
    partial class Wizard
    {
        partial class TalRasha
        {
            public class EnergyTwister
            {
                //public static bool NeedElectricute = false;
                //public static bool NeedExplosiveBlast = false;
                //public static bool NeedElectricute = false;
                //public static bool NeedTwister = false;
                public static TrinityPower PowerSelector()
                {
                    if (Archon.ShouldArchon())
                        return Archon.CastArchon;

                    if (ShouldFrostNova)
                        return CastFrostNova;

                    if (ShouldBlackHole)
                        return CastBlackhole;

                    if (ShouldExplosiveBlast)
                        return CastExplosiveBlast;

                    if (ShouldCastElectrocute)
                        return CastElectrocute;

                    if (ShouldCastEnergyTwister)
                        return CastEnergyTwister;

                    return null;
                }

                #region Vectors

                private static TrinityActor ClosestTwister()
                {
                    return PhelonUtils.GetTwisterDiaObjects(25, true)
                        .OrderBy(x => x.Position.Distance(PhelonTargeting.BestAoeUnit(45, true).Position))
                        .FirstOrDefault(
                            x => x.Position.Distance(PhelonTargeting.BestAoeUnit(45, true).Position) < 7 && x.CanCastTo());
                }

                private static Vector3 ActualLocation
                {
                    get
                    {
                        if (ClosestTwister() != null)
                            return ClosestTwister().Position;
                        if (PhelonTargeting.BestAoeUnit(45, true) == null) return Vector3.Zero;
                        var distance = PhelonTargeting.BestAoeUnit(45, true).Distance - 5;
                        return MathEx.CalculatePointFrom(Player.Position, PhelonTargeting.BestAoeUnit(45, true).Position,
                            distance);
                    }
                }

                #endregion

                #region Conditions

                private static bool ShouldBlackHole
                    => Skills.Wizard.BlackHole.CanCast() && PhelonTargeting.BestAoeUnit(45, true).Distance < 45;

                private static bool ShouldExplosiveBlast
                    =>
                        Skills.Wizard.ExplosiveBlast.CanCast() && TargetUtil.AnyMobsInRange(12f) &&
                        (GetHasBuff(SNOPower.Wizard_ExplosiveBlast) && GetBuffStacks(SNOPower.Wizard_ExplosiveBlast) < 4 ||
                         TimeSincePowerUse(SNOPower.Wizard_ExplosiveBlast) > 5000);

                private static bool ShouldFrostNova
                    => Skills.Wizard.FrostNova.CanCast() && TargetUtil.AnyMobsInRange(12f);

                private static bool ShouldCastEnergyTwister
                    =>
                        Core.Player.PrimaryResource > 50 && //Skills.Wizard.BlackHole.CanCast() && 
                        ActualLocation.Distance(Player.Position) < 45;

                private static bool ShouldCastElectrocute
                    => Skills.Wizard.Electrocute.CanCast() && Player.PrimaryResource < 50;

                #endregion

                #region Expressions

                private static TrinityPower CastBlackhole
                    => new TrinityPower(Skills.Wizard.BlackHole.SNOPower, 45, PhelonTargeting.BestAoeUnit(45, true).Position);

                private static TrinityPower CastExplosiveBlast => new TrinityPower(SNOPower.Wizard_ExplosiveBlast);
                private static TrinityPower CastFrostNova => new TrinityPower(SNOPower.Wizard_FrostNova, 20f);

                private static TrinityPower CastEnergyTwister
                    => new TrinityPower(SNOPower.Wizard_EnergyTwister, 45f, ActualLocation);

                private static TrinityPower CastElectrocute
                    => new TrinityPower(SNOPower.Wizard_Electrocute, 60f, TargetUtil.GetBestPierceTarget(60f).AcdId)
                    ;

                #endregion
            }
        }
    }
}