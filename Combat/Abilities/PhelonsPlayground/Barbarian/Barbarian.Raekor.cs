using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Barbarian
{
    partial class Barbarian
    {
        public class Raekor
        {
            public static TrinityPower PowerSelector()
            {
                if (Player.IsIncapacitated) return null;
                if (ShouldAncientSpear)
                    return CastAncientSpear;
                if (ShouldUseFuriousCharge)
                    return CastFuriousCharge;
                return null;
            }

            public static bool ShouldUseFuriousCharge
            {
                get
                {
                    if (!CanCast(SNOPower.Barbarian_FuriousCharge))
                        return false;

                    return TargetUtil.AnyMobsInRange(45f);
                }
            }

            public static TrinityPower CastFuriousCharge
                =>
                    new TrinityPower(SNOPower.Barbarian_FuriousCharge, 45f,
                        PhelonUtils.PointBehind(PhelonTargeting.PhelonCurrentTarget.Position));

            public static bool ShouldAncientSpear
            {
                get
                {
                    if (!CanCast(SNOPower.X1_Barbarian_AncientSpear))
                        return false;

                    return Player.PrimaryResourcePct > 0.95 || Sets.TheLegacyOfRaekor.IsFullyEquipped &&
                           GetBuffStacks(SNOPower.P2_ItemPassive_Unique_Ring_026) >= 5;
                }
            }

            public static TrinityPower CastAncientSpear
                =>
                    new TrinityPower(SNOPower.X1_Barbarian_AncientSpear, 60f,
                        PhelonTargeting.PhelonCurrentTarget.Position);
        }
    }
}