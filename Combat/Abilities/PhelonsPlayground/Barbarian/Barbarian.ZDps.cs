using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Barbarian
{
    partial  class Barbarian
    {
        internal class ZDps
        {
            public static TrinityPower PowerSelector()
            {
                TrinityCacheObject target;

                if (ShouldThreateningShout)
                    return CastThreateningShout;

                if (ShouldFuriousCharge(out target))
                    return CastFuriousCharge(target);

                if (ShouldAncientSpear(out target))
                    return CastAncientSpear(target);

                if (ShouldWhirlWind(out target))
                    CastWhirlWind(target);

                if (ShouldRend(out target))
                    CastRend(target);

                if (ShouldBash(out target))
                    return CastBash(target);

                return null;
            }

            public static bool ShouldRend(out TrinityCacheObject target)
            {
                target = null;

                if (!CanCast(SNOPower.Barbarian_Bash))
                    return false;

                target =
                    PhelonUtils.SafeList()
                        .OrderBy(y => y.Distance)
                        .FirstOrDefault(x => x.IsUnit && !GetHasBuff(SNOPower.Barbarian_Rend));
                return target != null && target.Distance <= 7;
            }

            public static TrinityPower CastRend(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Barbarian_Rend, 7f, target.ACDGuid);
            }

            public static bool ShouldBash(out TrinityCacheObject target)
            {
                target = null;

                if (!CanCast(SNOPower.Barbarian_Bash))
                    return false;

                target =
                    PhelonUtils.SafeList()
                        .OrderBy(y => y.Distance)
                        .FirstOrDefault(x => x.IsUnit && !GetHasBuff(SNOPower.Barbarian_Bash));
                return target != null && target.Distance <= 7;
            }

            public static TrinityPower CastBash(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Barbarian_Bash, 7f, target.ACDGuid);
            }


            public static bool ShouldThreateningShout
            {
                get { return CanCast(SNOPower.Barbarian_ThreateningShout) && PhelonTargeting.BestAoeUnit().Distance < 25; }
            }

            public static TrinityPower CastThreateningShout
            {
                get { return new TrinityPower(SNOPower.Barbarian_ThreateningShout, 75, Player.Position); }
            }

            public static bool ShouldWhirlWind(out TrinityCacheObject target)
            {
                target = null;

                if (!CanCast(SNOPower.Barbarian_Whirlwind))
                    return false;

                target = PhelonGroupSupport.UnitsToPull(PhelonGroupSupport.Monk.Position).FirstOrDefault() ??
                         PhelonTargeting.BestAoeUnit();

                return target != null && target.Distance <= 75 && Player.PrimaryResource > 25;
            }

            public static TrinityPower CastWhirlWind(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Barbarian_Whirlwind, 75, target.Position);
            }

            public static bool ShouldFuriousCharge(out TrinityCacheObject target)
            {
                target = null;

                if (!CanCast(SNOPower.Barbarian_FuriousCharge))
                    return false;

                target = PhelonUtils.BestPierceOrClusterUnit(10, 38);

                return target != null && Player.PrimaryResourcePct < 0.60;
            }

            public static TrinityPower CastFuriousCharge(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Barbarian_FuriousCharge, 38,
                        PhelonUtils.PointBehind(target.Position));
            }

            public static bool ShouldAncientSpear(out TrinityCacheObject target)
            {
                target = null;

                if (!CanCast(SNOPower.X1_Barbarian_AncientSpear))
                    return false;

                target = PhelonGroupSupport.UnitsToPull(PhelonGroupSupport.Monk.Position).FirstOrDefault();

                return target != null && target.Distance <= 45 && Player.PrimaryResource > 25;
            }

            public static TrinityPower CastAncientSpear(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.X1_Barbarian_AncientSpear, 45f,
                    target.Position);
            }
        }
    }
}
