using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Movement;
using Trinity.Reference;

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
                    if (ShouldSpectralBlade)
                        return CastSpectralBlade;
                    if (ShouldFrostNova)
                        return CastFrostNova;
                    if (ShouldExplosiveBlast)
                        return CastExplosiveBlast;
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
                    get { return Skills.Wizard.FrostNova.CanCast() && TargetUtil.AnyMobsInRange(12); }
                }

                private static TrinityPower CastFrostNova
                {
                    get { return new TrinityPower(Skills.Wizard.FrostNova.SNOPower); }
                }

                private static bool ShouldSpectralBlade
                {
                    get
                    {
                        return Skills.Wizard.SpectralBlade.CanCast() && (Player.PrimaryResourcePct < 0.20 ||
                               Skills.Wizard.SpectralBlade.TimeSinceUse > 3500) && CurrentTarget != null;
                    }
                }

                private static TrinityPower CastSpectralBlade
                {
                    get { return new TrinityPower(Skills.Wizard.SpectralBlade.SNOPower, 12f, CurrentTarget.ACDGuid); }
                }
            }
        }
    }
}