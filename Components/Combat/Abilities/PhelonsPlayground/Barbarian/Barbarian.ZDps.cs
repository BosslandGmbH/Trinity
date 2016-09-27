//using System.Linq;
//using Trinity.Framework.Actors.ActorTypes;
//using Trinity.Reference;
//using Zeta.Game.Internals.Actors;

//namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.Barbarian
//{
//    partial class Barbarian
//    {
//        internal class ZDps
//        {
//            public static TrinityPower PowerSelector()
//            {
//                TrinityActor target;
                
//                if (ShouldAncientSpear(out target))
//                    return CastAncientSpear(target);
                
//                if (ShouldRend(out target))
//                    return CastRend(target);
                
//                if (ShouldBash(out target))
//                    return CastBash(target);
                
//                if (ShouldWhirlWind(out target))
//                    return CastWhirlWind(target);
                
//                if (ShouldFuriousCharge(out target))
//                    return CastFuriousCharge(target);

//                return null;
//            }

//            public static bool ShouldRend(out TrinityActor target)
//            {
//                target = null;

//                if (!Skills.Barbarian.Rend.CanCast())
//                    return false;

//                target = TargetUtil.BestAuraUnit(Skills.Barbarian.Rend.SNOPower);
//                return target != null && target.Distance <= 12 && Player.PrimaryResourcePct > 0.50;
//            }

//            public static TrinityPower CastRend(TrinityActor target)
//            {
//                return new TrinityPower(Skills.Barbarian.Rend.SNOPower, 12f, target.AcdId);
//            }

//            public static bool ShouldBash(out TrinityActor target)
//            {
//                target = null;

//                if (!Skills.Barbarian.Bash.CanCast() ||
//                    TargetUtil.UnitsToPull(TargetUtil.Monk.Position).FirstOrDefault() != null)
//                    return false;

//                target =
//                    TargetUtil.SafeList()
//                        .OrderBy(y => y.Distance)
//                        .FirstOrDefault(x => x.IsUnit && !x.HasDebuff(SNOPower.Barbarian_Bash));
//                return target != null && target.Distance <= 12;
//            }

//            public static TrinityPower CastBash(TrinityActor target)
//            {
//                return new TrinityPower(SNOPower.Barbarian_Bash, 12f, target.AcdId);
//            }
            
//            public static bool ShouldWhirlWind(out TrinityActor target)
//            {
//                target = null;

//                if (!Skills.Barbarian.Whirlwind.CanCast())
//                    return false;

//                if (CurrentTarget.IsElite)
//                    target = CurrentTarget;
//                else if (TargetUtil.Monk != null && TargetUtil.UnitsToPull(TargetUtil.Monk.Position).FirstOrDefault() != null)
//                    target = TargetUtil.UnitsToPull(TargetUtil.Monk.Position).FirstOrDefault();
//                else if (TargetUtil.Monk != null)
//                    target = TargetUtil.Monk;
//                else if (TargetUtil.ClosestGlobe() != null)
//                    target = TargetUtil.ClosestGlobe();
//                else
//                    TargetUtil.BestAoeUnit(15, true);

//                return target != null && Player.PrimaryResource > 10;
//            }

//            public static TrinityPower CastWhirlWind(TrinityActor target)
//            {
//                var targetPosition = target.Distance < 10 ?
//                TargetUtil.GetZigZagTarget(target.Position, 25f, true) : target.Position;
//                return new TrinityPower(Skills.Barbarian.Whirlwind.SNOPower, 25f, targetPosition,
//                    -1, 0, 1);
//            }

//            public static bool ShouldFuriousCharge(out TrinityActor target)
//            {
//                target = null;

//                if (!Skills.Barbarian.FuriousCharge.CanCast())
//                    return false;

//                target = TargetUtil.BestPierceOrClusterUnit(10, 38);

//                return target != null && Player.PrimaryResourcePct < 0.25;
//            }

//            public static TrinityPower CastFuriousCharge(TrinityActor target)
//            {
//                return new TrinityPower(SNOPower.Barbarian_FuriousCharge, 38,
//                    TargetUtil.PointBehind(target.Position));
//            }

//            public static bool ShouldAncientSpear(out TrinityActor target)
//            {
//                target = null;

//                if (!Skills.Barbarian.AncientSpear.CanCast())
//                    return false;

//                target = TargetUtil.Monk != null
//                    ? TargetUtil.UnitsToPull(TargetUtil.Monk.Position).FirstOrDefault()
//                    : TargetUtil.UnitsToPull(Player.Position).FirstOrDefault();

//                return target != null && target.Distance <= 45 && Player.PrimaryResourcePct > 0.90 &&
//                       TimeSincePowerUse(SNOPower.X1_Barbarian_AncientSpear) > 1500;
//            }

//            public static TrinityPower CastAncientSpear(TrinityActor target)
//            {
//                return new TrinityPower(SNOPower.X1_Barbarian_AncientSpear, 60f,
//                    target.AcdId);
//            }
//        }
//    }
//}