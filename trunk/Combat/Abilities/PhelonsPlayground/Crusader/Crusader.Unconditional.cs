using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Config.Combat;
using Trinity.Movement;
using Trinity.Reference;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Combat.Abilities.PhelonsPlayground.Crusader
{
    partial class Crusader
    {
        internal class Unconditional
        {
            public static TrinityPower PowerSelector()
            {
                //Cast Akarats any time to make sure we do not die
                if (ShouldAkaratsChampion)
                    return CastAkaratsChampion;
                if (ShouldLawsOfHope)
                    return CastLawsOfHope;
                return null;
            }

            private static TrinityPower CastLawsOfHope
            {
                get { return new TrinityPower(Skills.Crusader.LawsOfHope.SNOPower); }
            }

            private static bool ShouldLawsOfHope
            {
                get
                {
                    return Skills.Crusader.LawsOfHope.CanCast() &&
                           !CacheData.Buffs.HasBuff(SNOPower.X1_Crusader_LawsOfHope2, 6) &&
                           !IsSteedCharging && (CurrentTarget == null || Settings.Combat.Crusader.SpamLaws);
                    ;
                }
            }

            private static bool ShouldAkaratsChampion
            {
                get
                {
                    //Basic checks
                    if (!Skills.Crusader.AkaratsChampion.CanCast())
                        return false;

                    // Akarat's mode is 'Off Cooldown'
                    if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.WhenReady)
                        return true;
                    //Use on Low Health
                    if (Player.CurrentHealthPct <= 0.25 &&
                        (Settings.Combat.Crusader.AkaratsEmergencyHealth || Runes.Crusader.Prophet.IsActive))
                        return true;
                    //Use if Incapacitated
                    if (Settings.Combat.Crusader.AkaratsOnStatusEffect &&
                        (ZetaDia.Me.IsFrozen || ZetaDia.Me.IsRooted || ZetaDia.Me.IsFeared || ZetaDia.Me.IsStunned))
                        return true;

                    if (!IsSteedCharging || ClassMover.HasInfiniteCasting)
                    {
                        // Let's check for Goblins, Current Health, CDR Pylon, movement impaired
                        if (CurrentTarget != null && CurrentTarget.IsTreasureGoblin)
                            return true;

                        // Akarat's mode is 'Whenever in Combat'
                        if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.WhenInCombat &&
                            TargetUtil.AnyMobsInRange(40f))
                            return true;

                        // Akarat's mode is 'Use when Elites are nearby'
                        if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.Normal &&
                            TargetUtil.AnyElitesInRange(40f))
                            return true;

                        // Akarat's mode is 'Hard Elites Only'
                        if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.HardElitesOnly &&
                            HardElitesPresent)
                            return true;
                    }
                    return false;
                }
            }

            private static TrinityPower CastAkaratsChampion
            {
                get { return new TrinityPower(Skills.Crusader.AkaratsChampion.SNOPower); }
            }
        }
    }
}