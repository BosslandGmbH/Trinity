using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trinity.Combat.Abilities;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Logger = Trinity.Technicals.Logger;

namespace Trinity
{
    public partial class TrinityPlugin
    {
        internal static int LastSceneId = -1;

        internal static Stopwatch HotbarRefreshTimer = new Stopwatch();

        private static bool TargetCheckResult(bool result, string source)
        {
            Logger.LogDebug(LogCategory.GlobalHandler, "TargetCheck returning {0}, {1}", result, source);
            return result;
        }

        /// <summary>
        /// Find fresh targets, start main BehaviorTree if needed, cast any buffs needed etc.        
        /// </summary>
        /// <param name="ret"></param>
        /// <returns></returns>
        internal static bool TargetCheck(object ret)
        {
            //Logger.LogVerbose("TargetCheck Tick");

            using (new PerformanceLogger("TargetCheck"))
            {
                if (Player.IsDead)
                {
                    return TargetCheckResult(false, "Is Dead");
                }

                _timesBlockedMoving = 0;
                IsAlreadyMoving = false;
                lastMovementCommand = DateTime.MinValue;
                _isWaitingForPower = false;
                _isWaitingAfterPower = false;
                _isWaitingBeforePower = false;
                _isWaitingForPotion = false;
                wasRootedLastTick = false;

                ClearBlacklists();

                // We have a target, start the target handler!
                if (CurrentTarget != null)
                {
                    _shouldPickNewAbilities = true;
                    return TargetCheckResult(true, "Current Target is not null");
                }

                if (Core.Avoidance.Avoider.ShouldAvoid && (Settings.Avoidance.AvoidOutsideCombat || Core.Avoidance.Grid.IsPathingOverFlags(AvoidanceFlags.CriticalAvoidance)))
                {
                    Vector3 safespot;
                    if (Core.Avoidance.Avoider.TryGetSafeSpot(out safespot))
                    {
                        CurrentTarget = new TrinityCacheObject()
                        {
                            Position = safespot,
                            Type = TrinityObjectType.Avoidance,
                            Distance = safespot.Distance(Player.Position),
                            Radius = 3.5f,
                            InternalName = "Avoidance Safespot",
                            IsSafeSpot = true
                        };
                    }
                }

                MonkCombat.RunOngoingPowers();

                // if we just opened a horadric cache, wait around to open it
                if (DateTime.UtcNow.Subtract(Composites.LastFoundHoradricCache).TotalSeconds < 5)
                    return TargetCheckResult(true, "Recently opened Horadric Cache");

                using (new PerformanceLogger("TargetCheck.OOCPotion"))
                {
                    // Pop a potion when necessary
                    if (Player.CurrentHealthPct <= CombatBase.EmergencyHealthPotionLimit)
                    {
                        UsePotionIfNeededTask();
                    }
                }
                _statusText = "[TrinityPlugin] No more targets - DemonBuddy/profile management is now in control";

                if (Settings.Advanced.DebugInStatusBar && _resetStatusText)
                {
                    _resetStatusText = false;
                    BotMain.StatusText = _statusText;
                }

                // Nothing to do... do we have some maintenance we can do instead, like out of combat buffing?

                if (DateTime.UtcNow.Subtract(_lastMaintenanceCheck).TotalMilliseconds > 150)
                {
                    using (new PerformanceLogger("TargetCheck.OOCBuff"))
                    {
                        _lastMaintenanceCheck = DateTime.UtcNow;

                        bool isLoopingAnimation = ZetaDia.Me.LoopingAnimationEndTime > 0;

                        if (!isLoopingAnimation && !WantToTownRun && !ForceVendorRunASAP)
                        {
                            BarbarianCombat.AllowSprintOOC = true;
                            DisableOutofCombatSprint = false;

                            powerBuff = AbilitySelector(UseOOCBuff: true);                            

                            if (powerBuff != null && powerBuff.SNOPower != SNOPower.None)
                            {
                                Logger.LogVerbose(LogCategory.Behavior, "Using OOC Buff: {0}", powerBuff.SNOPower.ToString());
                                if (ZetaDia.Me.UsePower(powerBuff.SNOPower, powerBuff.TargetPosition, powerBuff.TargetDynamicWorldId, powerBuff.TargetACDGUID))
                                {
                                    SpellHistory.RecordSpell(powerBuff);
                                }
                                LastPowerUsed = powerBuff.SNOPower;
                                CacheData.AbilityLastUsed[powerBuff.SNOPower] = DateTime.UtcNow;

                                // Monk Stuffs get special attention
                                {
                                    if (powerBuff.SNOPower == SNOPower.Monk_TempestRush)
                                        MonkCombat.LastTempestRushLocation = CombatBase.CurrentPower.TargetPosition;
                                }

                            }
                        }
                        else if (isLoopingAnimation)
                        {
                            _keepKillRadiusExtendedForSeconds = 20;
                            _timeKeepKillRadiusExtendedUntil = DateTime.UtcNow.AddSeconds(_keepKillRadiusExtendedForSeconds);
                        }
                    }
                }
                TargetUtil.ClearCurrentTarget("Target Check Failed to have Valid Target.");

                if ((ForceVendorRunASAP || WantToTownRun) && TownRun.TownRunTimerRunning())
                {
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Waiting for town run timer (Target Check)", true);
                    return TargetCheckResult(true, "Waiting for TownRunTimer");
                }

                return TargetCheckResult(false, "End of TargetCheck");
            }
        }
        private static DateTime _lastMaintenanceCheck = DateTime.UtcNow;

        public static void ClearBlacklists()
        {
            // Clear the temporary blacklist every 90 seconds (default was 90)
            if (DateTime.UtcNow.Subtract(Blacklist90LastClear).TotalSeconds > 90)
            {
                Blacklist90LastClear = DateTime.UtcNow;
                Blacklist90Seconds = new HashSet<int>();

                // Refresh profile blacklists now, just in case
                UsedProfileManager.RefreshProfileBlacklists();
            }
            // Clear the full blacklist every 60 seconds (default was 60)
            if (DateTime.UtcNow.Subtract(Blacklist60LastClear).TotalSeconds > 60)
            {
                Blacklist60LastClear = DateTime.UtcNow;
                Blacklist60Seconds = new HashSet<int>();
            }
            // Clear the temporary blacklist every 15 seconds (default was 15)
            if (DateTime.UtcNow.Subtract(Blacklist15LastClear).TotalSeconds > 15)
            {
                Blacklist15LastClear = DateTime.UtcNow;
                Blacklist15Seconds = new HashSet<int>();
            }
            // Clear our very short-term ignore-monster blacklist (from not being able to raycast on them or already dead units)
            if (DateTime.UtcNow.Subtract(Blacklist3LastClear).TotalMilliseconds > 3000)
            {
                Blacklist3LastClear = DateTime.UtcNow;
                NeedToClearBlacklist3 = false;
                Blacklist3Seconds = new HashSet<int>();
            }

            if (DateTime.UtcNow.Subtract(Blacklist1LastClear).TotalMilliseconds > 1000)
            {
                Blacklist1LastClear = DateTime.UtcNow;
                Blacklist1Second = new HashSet<int>();
            }


        }
    }
}
