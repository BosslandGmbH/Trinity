using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.WitchDoctor
{
    partial class WitchDoctor
    {
        public class ZDps
        {
            public static TrinityPower PowerSelector()
            {
                if (Player.IsIncapacitated) return null;
                TrinityCacheObject target;

                if (GetHasBuff(SNOPower.Witchdoctor_SpiritWalk))
                    return new TrinityPower(SNOPower.Walk, 7f, PhelonUtils.BestWalkLocation(35f, true));

                if (ShouldPiranhas(out target))
                    return CastPiranhas(target);

                if (ShouldHex(out target))
                    return CastHex(target);

                if (ShouldMassConfusion)
                    return CastMassConfusion;

                if (ShouldBigBadVoodoo)
                    return CastBigBadVoodoo;

                if (ShouldLocustSwarm(out target))
                    return CastLocustSwarmt(target);

                if (ShouldHaunt(out target))
                    return CastHaunt(target);

                return null;
            }

            private static bool ShouldPiranhas(out TrinityCacheObject target)
            {
                target = null;

                if (!Skills.WitchDoctor.Piranhas.CanCast())
                    return false;

                if (Runes.WitchDoctor.WaveOfMutilation.IsActive)
                    target = PhelonUtils.GetBestPierceTarget(25, true);
                else
                    target = PhelonTargeting.BestAoeUnit(35, true);

                return target != null;
            }

            private static TrinityPower CastPiranhas(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 45, target.Position);
            }

            private static bool ShouldBigBadVoodoo
            {
                get
                {
                    if (!Skills.WitchDoctor.BigBadVoodoo.CanCast())
                        return false;
                    var target = PhelonGroupSupport.Monk ?? PhelonTargeting.BestAoeUnit(45f, true);
                    return target != null && target.Distance < 45;
                }
            }

            private static TrinityPower CastBigBadVoodoo
            {
                get { return new TrinityPower(SNOPower.Witchdoctor_BigBadVoodoo); }
            }

            private static bool ShouldHex(out TrinityCacheObject target)
            {
                target = null;
                if (!Skills.WitchDoctor.Hex.CanCast())
                    return false;
                target = PhelonGroupSupport.Monk ?? PhelonTargeting.BestAoeUnit(45f, true);
                return target != null && target.Distance < 35;
            }

            private static TrinityPower CastHex(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Witchdoctor_Hex, 35f, target.Position);
            }

            private static bool ShouldMassConfusion
            {
                get
                {
                    if (!Skills.WitchDoctor.MassConfusion.CanCast())
                        return false;
                    var target = PhelonGroupSupport.Monk ?? PhelonTargeting.BestAoeUnit(45f, true);
                    return target != null && target.Distance < 45;
                }
            }

            private static TrinityPower CastMassConfusion
            {
                get { return new TrinityPower(SNOPower.Witchdoctor_MassConfusion); }
            }

            private static bool ShouldHaunt(out TrinityCacheObject target)
            {
                target = null;
                if (!Skills.WitchDoctor.Haunt.CanCast())
                    return false;
                target = PhelonUtils.BestAuraUnit(SNOPower.Witchdoctor_Haunt, 35, true);
                return target != null;
            }

            private static TrinityPower CastHaunt(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Witchdoctor_Haunt, 35f, target.ACDGuid);
            }

            private static bool ShouldLocustSwarm(out TrinityCacheObject target)
            {
                target = null;
                if (!Skills.WitchDoctor.LocustSwarm.CanCast() || Legendary.Wormwood.IsEquipped)
                    return false;
                target = PhelonUtils.BestAuraUnit(SNOPower.Witchdoctor_Locust_Swarm, 35f, true);
                return target != null;
            }

            private static TrinityPower CastLocustSwarmt(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Witchdoctor_Locust_Swarm, 35f, target.ACDGuid);
            }
        }
    }
}