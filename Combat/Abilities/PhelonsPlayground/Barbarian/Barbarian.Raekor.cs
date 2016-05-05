using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Barbarian
{
    partial class Barbarian
    {
        private class Raekor
        {
            public static TrinityPower PowerSelector()
            {
                if (Player.IsIncapacitated) return null;
                TrinityCacheObject target = null;
                if (ShouldAncientSpear(out target))
                    return CastAncientSpear(target);
                if (ShouldUseFuriousCharge(out target))
                    return CastFuriousCharge(target);
                return null;
            }

            private static bool ShouldUseFuriousCharge(out TrinityCacheObject target)
            {
                target = null;

                if (!CanCast(SNOPower.Barbarian_FuriousCharge))
                        return false;

                target = PhelonUtils.BestPierceOrClusterUnit(10, 38);

                return target != null;
            }

            private static TrinityPower CastFuriousCharge(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Barbarian_FuriousCharge, 38,
                        PhelonUtils.PointBehind(target.Position));
            }

            private static bool ShouldAncientSpear(out TrinityCacheObject target)
            {
                target = null;

                if (!CanCast(SNOPower.X1_Barbarian_AncientSpear))
                    return false;

                target = PhelonTargeting.BestAoeUnit(true).IsInLineOfSight()
                    ? PhelonTargeting.BestAoeUnit(true)
                    : PhelonUtils.GetBestClusterUnit(10, 60, false, true, false, true);

                if (target == null)
                    return false;

                return target.Distance <= 60 &&
                       (Player.PrimaryResourcePct > 0.95 || Sets.TheLegacyOfRaekor.IsFullyEquipped &&
                        GetBuffStacks(SNOPower.P2_ItemPassive_Unique_Ring_026) >= 5);
            }

            private static TrinityPower CastAncientSpear(TrinityCacheObject target)
            {
                    return new TrinityPower(SNOPower.X1_Barbarian_AncientSpear, 60f,
                        target.Position);
            }
        }
    }
}