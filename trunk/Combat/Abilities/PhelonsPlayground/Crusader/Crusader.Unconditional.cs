using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Config.Combat;
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
                return null;
            }

            private static bool ShouldAkaratsChampion
            {
                get
                {
                    //Basic checks
                    if (!Skills.Crusader.AkaratsChampion.CanCast() || Player.IsInTown)
                        return false;

                    // Akarat's mode is 'Off Cooldown'
                    if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.WhenReady)
                        return true;

                    // Let's check for Goblins, Current Health, CDR Pylon, movement impaired
                    if (CurrentTarget != null && CurrentTarget.IsTreasureGoblin &&
                        Settings.Combat.Monk.UseEpiphanyGoblin ||
                        Player.CurrentHealthPct <= 0.39 && Settings.Combat.Crusader.AkaratsEmergencyHealth ||
                        GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting) ||
                        Settings.Combat.Crusader.AkaratsOnStatusEffect &&
                        (ZetaDia.Me.IsFrozen || ZetaDia.Me.IsRooted || ZetaDia.Me.IsFeared || ZetaDia.Me.IsStunned))
                        return true;

                    // Akarat's mode is 'Whenever in Combat'
                    if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.WhenInCombat &&
                        TargetUtil.AnyMobsInRange(80f))
                        return true;

                    // Akarat's mode is 'Use when Elites are nearby'
                    if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.Normal &&
                        TargetUtil.AnyElitesInRange(80f))
                        return true;

                    // Akarat's mode is 'Hard Elites Only'
                    if (Settings.Combat.Crusader.AkaratsMode == CrusaderAkaratsMode.HardElitesOnly && HardElitesPresent)
                        return true;

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