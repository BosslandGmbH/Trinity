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

                if (ShouldArcaneBlast)
                    return CastArcaneBlast;

                if (ShouldArcaneStrike)
                    return CastArcaneStrike;

                if (ShouldDisentegrate)
                    return CastDisentegrate;
                return null;
            }

            private static bool ShouldDisentegrate => Skills.Wizard.ArchonDisintegrationWave.IsActive &&
                                                      PhelonUtils.GetBestPierceTarget(45).Distance < 45;

            private static TrinityPower CastDisentegrate
                => new TrinityPower(Skills.Wizard.ArchonDisintegrationWave.SNOPower, 45f,
                    PhelonUtils.PointBehind(PhelonUtils.GetBestPierceTarget(45).Position));

            private static bool ShouldArcaneStrike
                => Skills.Wizard.ArchonStrike.IsActive && PhelonTargeting.BestAoeUnit(45, true).Distance < 10f;

            private static TrinityPower CastArcaneStrike => new TrinityPower(Skills.Wizard.ArchonStrike.SNOPower, 10f,
                PhelonTargeting.BestAoeUnit(45, true).Position);

            private static bool ShouldArcaneBlast
                => Skills.Wizard.ArchonBlast.IsActive && PhelonTargeting.BestAoeUnit(45, true).Distance < 10f;

            private static TrinityPower CastArcaneBlast => new TrinityPower(Skills.Wizard.ArchonBlast.SNOPower, 10f,
                PhelonTargeting.BestAoeUnit(45, true).Position);

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