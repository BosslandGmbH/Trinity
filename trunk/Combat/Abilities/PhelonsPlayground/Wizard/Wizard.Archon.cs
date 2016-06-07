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
            public static bool NeedSlowTime = true;

            public static TrinityPower PowerSelector()
            {
                if (!Skills.Wizard.ArchonStrike.IsActive)
                    return null;

                if (ShouldSlowTime)
                    return CastSlowTime;
                //todo Arcane Strike is broken.
                //if (PhelonUtils.GetBestPierceTarget(45).NearbyUnitsWithinDistance() < TargetUtil.NumMobsInRange(14))
                //{
                //    //if (ShouldArcaneStrike)
                //        return CastArcaneStrike;
                //}
                if (ShouldDisentegrate)
                    return CastDisentegrate;
                return null;
            }

            private static bool ShouldDisentegrate => Skills.Wizard.ArchonDisintegrationWave.IsActive &&
                                                      PhelonUtils.GetBestPierceTarget(45) != null;

            private static TrinityPower CastDisentegrate
                => new TrinityPower(Skills.Wizard.ArchonDisintegrationWave.SNOPower, 45f,
                    PhelonUtils.PointBehind(PhelonUtils.GetBestPierceTarget(45).Position));

            private static bool ShouldArcaneStrike
                => Skills.Wizard.ArchonStrike.IsActive && TargetUtil.GetClosestUnit(14) != null;

            private static TrinityPower CastArcaneStrike => new TrinityPower(Skills.Wizard.ArchonStrike.SNOPower, 14f,
                PhelonUtils.SlightlyForwardPosition());

            private static bool ShouldSlowTime => Skills.Wizard.ArchonSlowTime.IsActive && NeedSlowTime;

            private static TrinityPower CastSlowTime
            {
                get
                {
                    NeedSlowTime = false;
                    return new TrinityPower(Skills.Wizard.ArchonSlowTime.SNOPower);
                }
            }

            //private static bool ShouldCancelArchon
            //    => 
            //        CanCast(SNOPower.Wizard_Archon_Cancel) && VyrsCount >= 1 && TalRashasCount >= 3 &&
            //        Skills.Wizard.Archon.TimeSinceUse > 9500;

            //private static TrinityPower CastCancelArchon => new TrinityPower(SNOPower.Wizard_Archon_Cancel);
        }
    }
}