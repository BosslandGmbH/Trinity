using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Wizard
{
    partial class Wizard
    {
        public class Archon
        {
            public static TrinityPower PowerSelector()
            {
                if (!Skills.Wizard.ArchonStrike.IsActive)
                    return null;

                if (ShouldSlowTime)
                    return CastSlowTime;

                if (ShouldArcaneBlast)
                    return CastArcaneBlast;

                if (ShouldArcaneStrike)
                    return CastArcaneStrike;

                if (ShouldDisentegrate)
                    return CastDisentegrate;
                return null;
            }

            private static bool ShouldDisentegrate
            {
                get
                {
                    return Skills.Wizard.ArchonDisintegrationWave.CanCast() &&
                           PhelonUtils.GetBestPierceTarget(45).Distance < 45;
                }
            }

            private static TrinityPower CastDisentegrate
            {
                get
                {
                    return new TrinityPower(Skills.Wizard.ArchonDisintegrationWave.SNOPower, 45f,
                        PhelonUtils.PointBehind(PhelonUtils.GetBestPierceTarget(45).Position));
                }
            }

            private static bool ShouldArcaneStrike
            {
                get { return Skills.Wizard.ArchonStrike.CanCast() && PhelonTargeting.BestAoeUnit().Distance < 10f; }
            }

            private static TrinityPower CastArcaneStrike
            {
                get
                {
                    return new TrinityPower(Skills.Wizard.ArchonStrike.SNOPower, 10f,
                        PhelonTargeting.BestAoeUnit().Position);
                }
            }

            private static bool ShouldArcaneBlast
            {
                get { return Skills.Wizard.ArchonBlast.CanCast() && PhelonTargeting.BestAoeUnit().Distance < 10f; }
            }

            private static TrinityPower CastArcaneBlast
            {
                get
                {
                    return new TrinityPower(Skills.Wizard.ArchonBlast.SNOPower, 10f,
                        PhelonTargeting.BestAoeUnit().Position);
                }
            }

            private static bool ShouldSlowTime
            {
                get
                {
                    if (!Skills.Wizard.ArchonSlowTime.CanCast())
                        return false;
                    var bubbles =
                        SpellHistory.History.Where(
                            s =>
                                s.Power.SNOPower == Skills.Wizard.ArchonSlowTime.SNOPower &&
                                s.TimeSinceUse.TotalSeconds < 12)
                            .ToList();
                    var bubblePositions = new HashSet<Vector3>(bubbles.Select(b => b.TargetPosition));
                    Func<Vector3, bool> isValidBubblePosition =
                        pos => bubblePositions.Any(b => b.Distance2D(pos) <= 8f);

                    if (!isValidBubblePosition(Player.Position) || !bubbles.Any())
                    {
                        return true;
                    }
                    return false;
                }
            }

            private static TrinityPower CastSlowTime
            {
                get { return new TrinityPower(Skills.Wizard.ArchonSlowTime.SNOPower); }
            }
        }
    }
}

//private static SNOPower GetDisentegratePower
//{
//    get
//    {
//        var skillPower = CacheData.HotbarCache.Instance.ActiveSkills
//            .FirstOrDefault(p => p.Power == SNOPower.Wizard_Archon_DisintegrationWave ||
//                                 p.Power == SNOPower.Wizard_Archon_DisintegrationWave_Fire ||
//                                 p.Power == SNOPower.Wizard_Archon_DisintegrationWave_Lightning ||
//                                 p.Power == SNOPower.Wizard_Archon_DisintegrationWave_Cold);
//        if (skillPower == null)
//        {
//            Logger.LogNormal("************** THERE IS NO DISENTEGRATE POWER **************");
//            return SNOPower.None;
//        }
//        return skillPower.Power;
//    }
//}


//private static SNOPower GetStrikePower
//{
//    get
//    {
//        var skillPower = CacheData.HotbarCache.Instance.ActiveSkills
//            .FirstOrDefault(p => p.Power == SNOPower.Wizard_Archon_ArcaneStrike ||
//                                 p.Power == SNOPower.Wizard_Archon_ArcaneStrike_Fire ||
//                                 p.Power == SNOPower.Wizard_Archon_ArcaneStrike_Lightning ||
//                                 p.Power == SNOPower.Wizard_Archon_ArcaneStrike_Cold);
//        if (skillPower == null)
//        {
//            Logger.LogNormal("************** THERE IS NO STRIKE POWER **************");
//            return SNOPower.None;
//        }
//        return skillPower.Power;
//    }
//}

//private static SNOPower GetBlastPower
//{
//    get
//    {
//        var skillPower = CacheData.HotbarCache.Instance.ActiveSkills
//            .FirstOrDefault(p => p.Power == SNOPower.Wizard_Archon_ArcaneBlast ||
//                                 p.Power == SNOPower.Wizard_Archon_ArcaneBlast_Fire ||
//                                 p.Power == SNOPower.Wizard_Archon_ArcaneBlast_Lightning ||
//                                 p.Power == SNOPower.Wizard_Archon_ArcaneBlast_Cold);
//        if (skillPower == null)
//        {
//            Logger.LogNormal("************** THERE IS NO BLAST POWER **************");
//            return SNOPower.None;
//        }
//        return skillPower.Power;
//    }
//}