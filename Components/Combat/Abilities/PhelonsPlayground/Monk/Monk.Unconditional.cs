using Trinity.Reference;
using Zeta.Common;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.Monk
{
    partial class Monk
    {
        internal static float cycloneStrikeRange = Runes.Monk.Implosion.IsActive ? 34f : 24f;
        internal static float cycloneStrikeSpirit = Runes.Monk.EyeOfTheStorm.IsActive ? 80f : 100f;
        internal class Unconditional
        {
            public static TrinityPower PowerSelector()
            {
                if (ShouldCycloneStrike)
                    return CastCycloneStrike;

                return null;
            }
            private static Vector3 LastPullLocation = Vector3.Zero;

            private static bool ShouldCycloneStrike
            {
                get
                {
                    return IszDPS && CurrentTarget == null && Skills.Monk.CycloneStrike.CanCast() &&
                           Player.PrimaryResource > cycloneStrikeSpirit && LastPullLocation.Distance(Player.Position) > 20;
                }
            }

            private static TrinityPower CastCycloneStrike
            {
                get
                {
                    LastPullLocation = Player.Position;
                    return new TrinityPower(SNOPower.Monk_CycloneStrike);
                }

            }
        }
    }
}