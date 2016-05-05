using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Monk
{
    partial class Monk
    {
        internal class Unconditional
        {
            public static TrinityPower PowerSelector()
            {
                if (ShouldSpiritWalk)
                    return CastSpiritWalk;

                return null;
            }

            private static bool ShouldSpiritWalk
            {
                get
                {
                    return Skills.WitchDoctor.SpiritWalk.CanCast() &&
                           (PhelonUtils.ClosestHealthGlobe() != null || Player.CurrentHealthPct < 0.5);
                }
            }

            private static TrinityPower CastSpiritWalk
            {
                get { return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk, 0f, Player.Position); }
            }
        }
    }
}