using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Cache;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Barbarian
{
    partial class Barbarian
    {
        internal class ZDps
        {
            public static TrinityPower PowerSelector()
            {
                TrinityCacheObject target;
                
                if (ShouldAncientSpear(out target))
                    return CastAncientSpear(target);
                
                if (ShouldRend(out target))
                    return CastRend(target);
                
                if (ShouldBash(out target))
                    return CastBash(target);
                
                if (ShouldWhirlWind(out target))
                    return CastWhirlWind(target);
                
                if (ShouldFuriousCharge(out target))
                    return CastFuriousCharge(target);

                return null;
            }

            public static bool ShouldRend(out TrinityCacheObject target)
            {
                target = null;

                if (!Skills.Barbarian.Rend.CanCast())
                    return false;

                target = PhelonUtils.BestAuraUnit(Skills.Barbarian.Rend.SNOPower);
                return target != null && target.Distance <= 12 && Player.PrimaryResourcePct > 0.50;
            }

            public static TrinityPower CastRend(TrinityCacheObject target)
            {
                return new TrinityPower(Skills.Barbarian.Rend.SNOPower, 12f, target.ACDGuid);
            }

            public static bool ShouldBash(out TrinityCacheObject target)
            {
                target = null;

                if (!Skills.Barbarian.Bash.CanCast() ||
                    PhelonGroupSupport.UnitsToPull(PhelonGroupSupport.Monk.Position).FirstOrDefault() != null)
                    return false;

                target =
                    PhelonUtils.SafeList()
                        .OrderBy(y => y.Distance)
                        .FirstOrDefault(x => x.IsUnit && !x.HasDebuff(SNOPower.Barbarian_Bash));
                return target != null && target.Distance <= 12;
            }

            public static TrinityPower CastBash(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Barbarian_Bash, 12f, target.ACDGuid);
            }
            
            public static bool ShouldWhirlWind(out TrinityCacheObject target)
            {
                target = null;

                if (!Skills.Barbarian.Whirlwind.CanCast())
                    return false;

                if (CurrentTarget.IsElite)
                    target = CurrentTarget;
                else if (PhelonGroupSupport.Monk != null && PhelonGroupSupport.UnitsToPull(PhelonGroupSupport.Monk.Position).FirstOrDefault() != null)
                    target = PhelonGroupSupport.UnitsToPull(PhelonGroupSupport.Monk.Position).FirstOrDefault();
                else if (PhelonGroupSupport.Monk != null)
                    target = PhelonGroupSupport.Monk;
                else if (PhelonUtils.ClosestGlobe() != null)
                    target = PhelonUtils.ClosestGlobe();
                else
                    PhelonTargeting.BestAoeUnit(15, true);

                return target != null && Player.PrimaryResource > 10;
            }

            public static TrinityPower CastWhirlWind(TrinityCacheObject target)
            {
                var targetPosition = target.Distance < 10 ?
                TargetUtil.GetZigZagTarget(target.Position, 25f, true) : target.Position;
                return new TrinityPower(Skills.Barbarian.Whirlwind.SNOPower, 25f, targetPosition,
                    TrinityPlugin.CurrentWorldDynamicId, -1, 0, 1);
            }

            public static bool ShouldFuriousCharge(out TrinityCacheObject target)
            {
                target = null;

                if (!Skills.Barbarian.FuriousCharge.CanCast())
                    return false;

                target = PhelonUtils.BestPierceOrClusterUnit(10, 38);

                return target != null && Player.PrimaryResourcePct < 0.25;
            }

            public static TrinityPower CastFuriousCharge(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Barbarian_FuriousCharge, 38,
                    PhelonUtils.PointBehind(target.Position));
            }

            public static bool ShouldAncientSpear(out TrinityCacheObject target)
            {
                target = null;

                if (!Skills.Barbarian.AncientSpear.CanCast())
                    return false;

                target = PhelonGroupSupport.Monk != null
                    ? PhelonGroupSupport.UnitsToPull(PhelonGroupSupport.Monk.Position).FirstOrDefault()
                    : PhelonGroupSupport.UnitsToPull(Player.Position).FirstOrDefault();

                return target != null && target.Distance <= 45 && Player.PrimaryResourcePct > 0.90 &&
                       TimeSincePowerUse(SNOPower.X1_Barbarian_AncientSpear) > 1500;
            }

            public static TrinityPower CastAncientSpear(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.X1_Barbarian_AncientSpear, 60f,
                    target.ACDGuid);
            }
        }
    }
}