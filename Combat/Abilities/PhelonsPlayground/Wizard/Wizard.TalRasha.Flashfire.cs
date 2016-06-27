using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Movement;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Wizard
{
    partial class Wizard
    {
        partial class TalRasha
        {
            public class Flashfire
            {
                //public static bool NeedElectricute = false;
                //public static bool NeedExplosiveBlast = false;
                //public static bool NeedElectricute = false;
                //public static bool NeedTwister = false;
                public static TrinityPower PowerSelector()
                {
                    if (CurrentTarget != null && CurrentTarget.IsUnit &&
                        (TimeSincePowerUse(SNOPower.Wizard_Electrocute) > 3500 ||
                         !Skills.Wizard.ExplosiveBlast.CanCast()))
                    {
                        if (ShouldElectrocute)
                            return CastElectrocute;
                    }
                    if (ShouldExplosiveBlast)
                        return CastExplosiveBlast;
                    if (ShouldFrostNova)
                        return CastFrostNova;
                    return null;
                }

                private static bool ShouldExplosiveBlast
                {
                    get { return Skills.Wizard.ExplosiveBlast.CanCast(); }
                }

                private static TrinityPower CastExplosiveBlast
                {
                    get { return new TrinityPower(Skills.Wizard.ExplosiveBlast.SNOPower); }
                }

                private static bool ShouldFrostNova
                {
                    get { return Skills.Wizard.FrostNova.CanCast(); }
                }

                private static TrinityPower CastFrostNova
                {
                    get { return new TrinityPower(Skills.Wizard.FrostNova.SNOPower); }
                }

                private static bool ShouldSpectralBlade
                {
                    get
                    {
                        return Skills.Wizard.SpectralBlade.CanCast() && CurrentTarget != null &&
                               (TalRashasCount < 4 || Player.PrimaryResourcePct < 0.25 ||
                                Skills.Wizard.ExplosiveBlast.CooldownRemaining > 0);
                    }
                }

                private static TrinityPower CastSpectralBlade
                {
                    get { return new TrinityPower(Skills.Wizard.SpectralBlade.SNOPower, 12f, CurrentTarget.AcdId); }
                }

                private static bool ShouldElectrocute
                {
                    get
                    {
                        return Skills.Wizard.Electrocute.CanCast() && CurrentTarget != null &&
                               (TalRashasCount < 4 || Player.PrimaryResourcePct < 0.25 ||
                                Skills.Wizard.ExplosiveBlast.CooldownRemaining > 0);
                    }
                }

                private static TrinityPower CastElectrocute
                {
                    get { return new TrinityPower(Skills.Wizard.Electrocute.SNOPower, 35f, CurrentTarget.AcdId); }
                }
            }
        }
    }
}