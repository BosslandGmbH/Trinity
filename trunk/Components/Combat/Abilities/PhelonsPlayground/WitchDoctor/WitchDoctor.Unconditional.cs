using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.WitchDoctor
{
    partial class WitchDoctor
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
                           (PhelonUtils.ClosestGlobe() != null || Player.CurrentHealthPct < 0.5);
                }
            }

            private static TrinityPower CastSpiritWalk
            {
                get { return new TrinityPower(SNOPower.Witchdoctor_SpiritWalk, 0f, Player.Position); }
            }
        }
    }
}