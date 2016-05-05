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
                    return new TrinityPower(SNOPower.Walk, 7f, PhelonUtils.BestWalkLocation);

                if (ShouldPiranhas(out target))
                    return CastPiranhas(target);

                if (ShouldHex(out target))
                    return CastHex(target);

                if (ShouldMassConfusion(out target))
                    return CastMassConfusion(target);

                if (ShouldBigBadVoodoo(out target))
                    return CastBigBadVoodoo(target);

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
                    target = PhelonGroupSupport.Monk;

                return target != null;
            }

            private static TrinityPower CastPiranhas(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Witchdoctor_Piranhas, 45, target.Position);
            }

            private static bool ShouldBigBadVoodoo(out TrinityCacheObject target)
            {
                target = null;
                if (!Skills.WitchDoctor.BigBadVoodoo.CanCast())
                    return false;
                target = PhelonGroupSupport.Monk;
                return target != null && target.Distance < 45;
            }

            private static TrinityPower CastBigBadVoodoo(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Witchdoctor_BigBadVoodoo, 45, target.Position);
            }

            private static bool ShouldHex(out TrinityCacheObject target)
            {
                target = null;
                if (!Skills.WitchDoctor.Hex.CanCast())
                    return false;
                target = PhelonGroupSupport.Monk;
                return target != null && target.Distance < 45;
            }

            private static TrinityPower CastHex(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Witchdoctor_Hex, 38, target.Position);
            }

            private static bool ShouldMassConfusion(out TrinityCacheObject target)
            {
                target = null;
                if (!Skills.WitchDoctor.MassConfusion.CanCast())
                    return false;
                target = PhelonGroupSupport.Monk;
                return target != null && target.Distance < 45;
            }

            private static TrinityPower CastMassConfusion(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Witchdoctor_MassConfusion, 38, target.Position);
            }

            private static bool ShouldHaunt(out TrinityCacheObject target)
            {
                target = null;
                if (!Skills.WitchDoctor.Haunt.CanCast())
                    return false;
                target = PhelonUtils.BestAuraUnit(SNOPower.Witchdoctor_Haunt, 65, true);
                return target != null;
            }

            private static TrinityPower CastHaunt(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Witchdoctor_Haunt, 65, target.ACDGuid);
            }

            private static bool ShouldLocustSwarm(out TrinityCacheObject target)
            {
                target = null;
                if (!Skills.WitchDoctor.LocustSwarm.CanCast())
                    return false;
                target = PhelonUtils.BestAuraUnit(SNOPower.Witchdoctor_Locust_Swarm, 65, true);
                return target != null;
            }

            private static TrinityPower CastLocustSwarmt(TrinityCacheObject target)
            {
                return new TrinityPower(SNOPower.Witchdoctor_Locust_Swarm, 65, target.ACDGuid);
            }
        }
    }
}