using Trinity.Framework;
using Trinity.Reference;

namespace Trinity.Components.Combat.Abilities.PhelonsPlayground.Wizard
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
                TargetUtil.GetClosestUnit(14).Position);

            private static bool ShouldSlowTime => Skills.Wizard.ArchonSlowTime.IsActive && NeedSlowTime;

            private static TrinityPower CastSlowTime
            {
                get
                {
                    NeedSlowTime = false;
                    return new TrinityPower(Skills.Wizard.ArchonSlowTime.SNOPower);
                }
            }

            public static bool ShouldArchon()
            {
                if (!Skills.Wizard.Archon.CanCast() || Skills.Wizard.ArchonBlast.CanCast())
                    return false;

                if (Core.Buffs.HasArchon&& Legendary.TheTwistedSword.IsEquipped)
                    return false;

                //if (Legendary.ConventionOfElements.IsEquipped && TimeToElementStart(Element.Cold) < 3000 && TimeToElementStart(Element.Cold) > 0)
                //    return false;

                //if (Core.Buffs.HasArchon && Core.Buffs.GetBuffTimeRemainingMilliseconds(SNOPower.Wizard_Archon) < 1500)
                //    return true;

                //if (Sets.ChantodosResolve.IsFullyEquipped)
                //    return GetHasBuff(SNOPower.P3_ItemPassive_Unique_Ring_021) && GetBuffStacks(SNOPower.P3_ItemPassive_Unique_Ring_021) > 19;

                return true;
            }

            public static TrinityPower CastArchon
            {
                get
                {
                    Archon.NeedSlowTime = true;
                    return new TrinityPower(Skills.Wizard.Archon.SNOPower);
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