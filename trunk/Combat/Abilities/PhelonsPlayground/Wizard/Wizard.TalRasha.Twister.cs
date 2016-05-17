using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Wizard
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
                    if (ShouldFrostNova)
                        return CastFrostNova;
                    if (ShouldExplosiveBlast)
                        return CastExplosiveBlast;
                    if (ShouldCastElectrocute)
                        return CastElectrocute;
                    if (ShouldCastEnergyTwister)
                        return CastEnergyTwister;
                    return null;
                }

                #region Vectors

                static readonly TrinityCacheObject ClosestTwister = PhelonUtils.GetTwisterDiaObjects(35)
                    .OrderBy(x => x.Position.Distance(PhelonTargeting.BestAoeUnit().Position))
                    .FirstOrDefault(x => x.Position.Distance(PhelonTargeting.BestAoeUnit().Position) < 7);

                static readonly Vector3 ActualLocation = ClosestTwister?.Position ?? PhelonTargeting.BestAoeUnit().Position;

                #endregion

                #region Conditions

                private static bool ShouldExplosiveBlast
                    => Skills.Wizard.ExplosiveBlast.CanCast() && TargetUtil.AnyMobsInRange(12f) &&
                       (GetHasBuff(SNOPower.Wizard_ExplosiveBlast) && GetBuffStacks(SNOPower.Wizard_ExplosiveBlast) < 4 ||
                        TimeSincePowerUse(SNOPower.Wizard_ExplosiveBlast) > 5);

                private static bool ShouldFrostNova
                    => CanCast(SNOPower.Wizard_FrostNova) && TargetUtil.AnyMobsInRange(12f);

                private static bool ShouldCastEnergyTwister => Skills.Wizard.EnergyTwister.CanCast();

                private static bool ShouldCastElectrocute
                    => Skills.Wizard.Electrocute.CanCast() && Player.PrimaryResource < 50;

                #endregion

                #region Expressions

                private static TrinityPower CastExplosiveBlast => new TrinityPower(SNOPower.Wizard_ExplosiveBlast);
                private static TrinityPower CastFrostNova => new TrinityPower(SNOPower.Wizard_FrostNova, 20f);

                private static TrinityPower CastEnergyTwister
                    => new TrinityPower(SNOPower.Wizard_EnergyTwister, 0f, ActualLocation);

                private static TrinityPower CastElectrocute
                    => new TrinityPower(SNOPower.Wizard_Electrocute, 45f, PhelonTargeting.BestAoeUnit(45, true).ACDGuid);

                #endregion
            }
        }
    }
}