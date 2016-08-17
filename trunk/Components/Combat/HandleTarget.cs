using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trinity.Cache;
using Trinity.Components.Combat.Abilities;
using Trinity.Config.Combat;
using Trinity.Coroutines;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Movement;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Components.Combat
{


    public class TargetHandler
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
        internal bool TargetCheck(object ret)
        {
            //Logger.LogVerbose("TargetCheck Tick");

            using (new PerformanceLogger("TargetCheck"))
            {
                if (Core.Player.IsDead)
                {
                    return TargetCheckResult(false, "Is Dead");
                }

                Trinity.TrinityPlugin._timesBlockedMoving = 0;
                Trinity.TrinityPlugin.IsAlreadyMoving = false;
                Trinity.TrinityPlugin.lastMovementCommand = DateTime.MinValue;
                Trinity.TrinityPlugin._isWaitingForPower = false;
                Trinity.TrinityPlugin._isWaitingAfterPower = false;
                Trinity.TrinityPlugin._isWaitingBeforePower = false;
                Trinity.TrinityPlugin._isWaitingForPotion = false;
                Trinity.TrinityPlugin.wasRootedLastTick = false;

                ClearBlacklists();

                if (Core.Avoidance.Avoider.ShouldAvoid && (Core.Settings.Avoidance.AvoidOutsideCombat || Core.Avoidance.Grid.IsPathingOverFlags(AvoidanceFlags.CriticalAvoidance)))
                {
                    Logger.Log(LogCategory.Avoidance, $"Avoid now (Out of Combat)!");
                    Vector3 safespot;
                    if (Core.Avoidance.Avoider.TryGetSafeSpot(out safespot) && safespot.Distance(ZetaDia.Me.Position) > 3f)
                    {
                        Logger.Log(LogCategory.Avoidance, $"Safespot found: {safespot}");

                        if (Trinity.TrinityPlugin.CurrentTarget == null || Trinity.TrinityPlugin.CurrentTarget.Type != TrinityObjectType.Barricade && Trinity.TrinityPlugin.CurrentTarget.Type != TrinityObjectType.Door || Core.Avoidance.Grid.IsStandingInFlags(AvoidanceFlags.CriticalAvoidance))
                        {
                            var distance = safespot.Distance(Core.Player.Position);
                            Logger.Log(LogCategory.Avoidance, $"Targetted SafeSpot Distance={distance}");
                            Trinity.TrinityPlugin.CurrentTarget = new TrinityActor()
                            {
                                Position = safespot,
                                Type = TrinityObjectType.Avoidance,
                                Distance = distance,
                                Radius = 3.5f,
                                InternalName = "Avoidance Safespot",
                                IsSafeSpot = true,
                                Weight = Weighting.MaxWeight
                            };
                        }
                    }
                }

                // We have a target, start the target handler!
                if (Trinity.TrinityPlugin.CurrentTarget != null)
                {
                    Trinity.TrinityPlugin._shouldPickNewAbilities = true;
                    return TargetCheckResult(true, "Current Target is not null");
                }

                // if we just opened a horadric cache, wait around to open it
                if (DateTime.UtcNow.Subtract(Composites.LastFoundHoradricCache).TotalSeconds < 5)
                    return TargetCheckResult(true, "Recently opened Horadric Cache");

                using (new PerformanceLogger("TargetCheck.OOCPotion"))
                {
                    // Pop a potion when necessary
                    if (Core.Player.CurrentHealthPct <= CombatBase.EmergencyHealthPotionLimit)
                    {
                        CombatManager.TargetHandler.UsePotionIfNeededTask();
                    }
                }

                // Nothing to do... do we have some maintenance we can do instead, like out of combat buffing?

                if (DateTime.UtcNow.Subtract(_lastMaintenanceCheck).TotalMilliseconds > 150)
                {
                    using (new PerformanceLogger("TargetCheck.OOCBuff"))
                    {
                        _lastMaintenanceCheck = DateTime.UtcNow;

                        bool isLoopingAnimation = ZetaDia.Me.LoopingAnimationEndTime > 0;

                        if (!isLoopingAnimation && !Trinity.TrinityPlugin.WantToTownRun && !Trinity.TrinityPlugin.ForceVendorRunASAP)
                        {
                            BarbarianCombat.AllowSprintOOC = true;
                            Trinity.TrinityPlugin.DisableOutofCombatSprint = false;

                            Trinity.TrinityPlugin.powerBuff = CombatManager.AbilitySelector.SelectAbility(UseOOCBuff: true);

                            if (Trinity.TrinityPlugin.powerBuff != null && Trinity.TrinityPlugin.powerBuff.SNOPower != SNOPower.None)
                            {
                                Logger.LogVerbose(LogCategory.Behavior, "Using OOC Buff: {0}", Trinity.TrinityPlugin.powerBuff.SNOPower.ToString());
                                if (ZetaDia.Me.UsePower(Trinity.TrinityPlugin.powerBuff.SNOPower, Trinity.TrinityPlugin.powerBuff.TargetPosition, Trinity.TrinityPlugin.powerBuff.TargetDynamicWorldId, Trinity.TrinityPlugin.powerBuff.TargetAcdId))
                                {
                                    SpellHistory.RecordSpell(Trinity.TrinityPlugin.powerBuff);
                                }
                                Trinity.TrinityPlugin.LastPowerUsed = Trinity.TrinityPlugin.powerBuff.SNOPower;

                                // Monk Stuffs get special attention
                                {
                                    if (Trinity.TrinityPlugin.powerBuff.SNOPower == SNOPower.Monk_TempestRush)
                                        MonkCombat.LastTempestRushLocation = CombatBase.CurrentPower.TargetPosition;
                                }

                            }
                        }
                        else if (isLoopingAnimation)
                        {
                            Trinity.TrinityPlugin._keepKillRadiusExtendedForSeconds = 20;
                            Trinity.TrinityPlugin._timeKeepKillRadiusExtendedUntil = DateTime.UtcNow.AddSeconds(Trinity.TrinityPlugin._keepKillRadiusExtendedForSeconds);
                        }
                    }
                }
                TargetUtil.ClearCurrentTarget("Target Check Failed to have Valid Target.");

                //if ((ForceVendorRunASAP || WantToTownRun) && TownRun.TownRunTimerRunning())
                //{
                //    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Waiting for town run timer (Target Check)", true);
                //    return TargetCheckResult(true, "Waiting for TownRunTimer");
                //}

                return TargetCheckResult(false, "End of TargetCheck");
            }
        }
        private static DateTime _lastMaintenanceCheck = DateTime.UtcNow;

        public static void ClearBlacklists()
        {
            // Clear the temporary blacklist every 90 seconds (default was 90)
            if (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.Blacklist90LastClear).TotalSeconds > 90)
            {
                Trinity.TrinityPlugin.Blacklist90LastClear = DateTime.UtcNow;
                Trinity.TrinityPlugin.Blacklist90Seconds = new HashSet<int>();

                // Refresh profile blacklists now, just in case
                UsedProfileManager.RefreshProfileBlacklists();
            }
            // Clear the full blacklist every 60 seconds (default was 60)
            if (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.Blacklist60LastClear).TotalSeconds > 60)
            {
                Trinity.TrinityPlugin.Blacklist60LastClear = DateTime.UtcNow;
                Trinity.TrinityPlugin.Blacklist60Seconds = new HashSet<int>();
            }
            // Clear the temporary blacklist every 15 seconds (default was 15)
            if (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.Blacklist15LastClear).TotalSeconds > 15)
            {
                Trinity.TrinityPlugin.Blacklist15LastClear = DateTime.UtcNow;
                Trinity.TrinityPlugin.Blacklist15Seconds = new HashSet<int>();
            }
            // Clear our very short-term ignore-monster blacklist (from not being able to raycast on them or already dead units)
            if (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.Blacklist3LastClear).TotalMilliseconds > 3000)
            {
                Trinity.TrinityPlugin.Blacklist3LastClear = DateTime.UtcNow;
                Trinity.TrinityPlugin.NeedToClearBlacklist3 = false;
                Trinity.TrinityPlugin.Blacklist3Seconds = new HashSet<int>();
            }

            if (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.Blacklist1LastClear).TotalMilliseconds > 1000)
            {
                Trinity.TrinityPlugin.Blacklist1LastClear = DateTime.UtcNow;
                Trinity.TrinityPlugin.Blacklist1Second = new HashSet<int>();
            }


        }

        /// <summary>
        /// Returns the current DiaPlayer
        /// </summary>
        public DiaActivePlayer Me
        {
            get { return ZetaDia.Me; }
        }

        /// <summary>
        /// Returns a RunStatus, if appropriate. Throws an exception if not.
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public RunStatus GetRunStatus(RunStatus status, string location)
        {
            string extras = "";

            if (CombatBase.CombatMovement.IsQueuedMovement)
            {
                extras = "Aborting to run queued movement";
                Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "HandleTarget returning {0} to tree from " + location + " " + extras, status);
                return RunStatus.Success;
            }
            
            if (Trinity.TrinityPlugin._isWaitingForPower)
                extras += " IsWaitingForPower";
            if (Trinity.TrinityPlugin._isWaitingAfterPower)
                extras += " IsWaitingAfterPower";
            if (Trinity.TrinityPlugin._isWaitingForPotion)
                extras += " IsWaitingForPotion";
            if (TrinityTownRun.IsTryingToTownPortal())
                extras += " IsTryingToTownPortal";
            //if (TownRun.TownRunTimerRunning())
            //    extras += " TownRunTimerRunning";
            //if (TownRun.TownRunTimerFinished())
            //    extras += " TownRunTimerFinished";
            if (Trinity.TrinityPlugin._forceTargetUpdate)
                extras += " ForceTargetUpdate";
            if (Trinity.TrinityPlugin.CurrentTarget == null)
                extras += " CurrentTargetIsNull";
            if (CombatBase.CurrentPower != null && CombatBase.CurrentPower.ShouldWaitBeforeUse)
                extras += " CPowerShouldWaitBefore=" + (CombatBase.CurrentPower.WaitBeforeUseDelay - CombatBase.CurrentPower.TimeSinceAssignedMs);
            if (CombatBase.CurrentPower != null && CombatBase.CurrentPower.ShouldWaitAfterUse)
                extras += " CPowerShouldWaitAfter=" + (CombatBase.CurrentPower.WaitAfterUseDelay - CombatBase.CurrentPower.TimeSinceUse);
            if (CombatBase.CurrentPower != null && (CombatBase.CurrentPower.ShouldWaitBeforeUse || CombatBase.CurrentPower.ShouldWaitAfterUse))
                extras += " " + CombatBase.CurrentPower;

            Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "HandleTarget returning {0} to tree from " + location + " " + extras, status);
            return status;

        }

        public int _waitedTicks = 0;

        /// <summary>
        /// Handles all aspects of moving to and attacking the current target
        /// </summary>
        /// <returns></returns>
        internal RunStatus HandleTarget()
        {
            RunStatus status;

            Core.Player.CurrentAction = default(PlayerAction);

            using (new PerformanceLogger("HandleTarget"))
            {
                try
                {
                    if (!CombatTargeting.Instance.AllowedToKillMonsters && (Trinity.TrinityPlugin.CurrentTarget == null || Trinity.TrinityPlugin.CurrentTarget.IsUnit) && CombatBase.CombatMode != CombatMode.KillAll)
                    {
                        Logger.LogVerbose("Aborting HandleTarget() AllowCombat={0} ShouldAvoid={1}", CombatTargeting.Instance.AllowedToKillMonsters, Core.Avoidance.Avoider.ShouldAvoid);
                        return RunStatus.Failure;
                    }

                    if (!Core.Player.IsValid)
                    {
                        Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "No longer in game world", true);
                        return GetRunStatus(RunStatus.Failure, "PlayerInvalid");
                    }

                    if (Core.Player.IsDead)
                    {
                        Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "Player is dead", true);
                        return GetRunStatus(RunStatus.Failure, "PlayerDead");
                    }

                    if (UsePotionIfNeededTask())
                    {
                        return GetRunStatus(RunStatus.Running, "UsePotionTask");
                    }

                    if (Core.Avoidance.Avoider.ShouldAvoid)
                    {
                        Logger.Log(LogCategory.Avoidance, $"Avoid now!");
                        Vector3 safespot;
                        if (Core.Avoidance.Avoider.TryGetSafeSpot(out safespot) && safespot.Distance(ZetaDia.Me.Position) > 3f)
                        {
                            Logger.Log(LogCategory.Avoidance, $"Safespot found: {safespot}");

                            if(Trinity.TrinityPlugin.CurrentTarget == null || Trinity.TrinityPlugin.CurrentTarget.Type != TrinityObjectType.Barricade && Trinity.TrinityPlugin.CurrentTarget.Type != TrinityObjectType.Door || Core.Avoidance.Grid.IsStandingInFlags(AvoidanceFlags.CriticalAvoidance))
                            {                                
                                var distance = safespot.Distance(Core.Player.Position);
                                Logger.Log(LogCategory.Avoidance, $"Targetted SafeSpot Distance={distance}");
                                Trinity.TrinityPlugin.CurrentTarget = new TrinityActor()
                                {
                                    Position = safespot,
                                    Type = TrinityObjectType.Avoidance,
                                    Distance = distance,
                                    Radius = 3.5f,
                                    InternalName = "Avoidance Safespot",
                                    IsSafeSpot = true,
                                    Weight = Weighting.MaxWeight
                                };

                                TryCastAvoidancePower(out status);
                                PlayerMover.NavigateTo(safespot, "SafeSpot");
                                Core.Player.CurrentAction = PlayerAction.Avoiding;
                                return GetRunStatus(RunStatus.Running, "Movement for Avoidance");
                            }
                        }
                    }

                    if (Core.Avoidance.Avoider.ShouldKite)
                    {
                        Logger.Log(LogCategory.Avoidance, $"Kite now!");
                        Vector3 safespot;
                        if (Core.Avoidance.Avoider.TryGetSafeSpot(out safespot) && safespot.Distance(ZetaDia.Me.Position) > 3f)
                        {
                            Logger.Log(LogCategory.Avoidance, $"KiteSpot found: {safespot}");

                            var distance = safespot.Distance(Core.Player.Position);
                            Logger.Log(LogCategory.Avoidance, $"Targetted KiteSpot Distance={distance}");
                            Trinity.TrinityPlugin.CurrentTarget = new TrinityActor()
                            {
                                Position = safespot,
                                Type = TrinityObjectType.Avoidance,
                                Distance = distance,
                                Radius = 3.5f,
                                InternalName = "Avoidance Safespot",
                                IsSafeSpot = true,
                                Weight = Weighting.MaxWeight
                            };

                            PlayerMover.NavigateTo(safespot, "KiteSpot");
                            Core.Player.CurrentAction = PlayerAction.Kiting;
                            return GetRunStatus(RunStatus.Running, "Movement for Kiting");                          
                        }
                    }

                    //else if (CurrentTarget != null)
                    //{
                    //    if (CurrentTarget.IsSafeSpot)
                    //    {
                    //        TargetUtil.ClearCurrentTarget("Avoidance finished moving to safe spot.");
                    //        return GetRunStatus(RunStatus.Failure, "Finished Avoiding");
                    //    }

                    //    if (ObjectCache.All(a => a.AnnId != CurrentTarget.AnnId))
                    //    {
                    //        TargetUtil.ClearCurrentTarget("Target no longer exists.");
                    //        return GetRunStatus(RunStatus.Failure, "Target Missing");
                    //    }
                    //}

                    if (Core.Player.IsCasting && Trinity.TrinityPlugin.CurrentTarget != null && Trinity.TrinityPlugin.CurrentTarget.GizmoType == GizmoType.Headstone)
                    {
                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Player is casting revive ({0})", Core.Player.CurrentAnimation);
                        return GetRunStatus(RunStatus.Success, "RevivingPlayer");
                    }

                    if (!Core.Avoidance.Avoider.ShouldAvoid)
                    {
                        if (ThrottleActionPerSecond(out status)) //Settings.Advanced.ThrottleAPS && 
                            return status;

                        VacuumItems.Execute();

                        // Make sure we reset unstucker stuff here
                        PlayerMover.TimesReachedStuckPoint = 0;
                        PlayerMover.vSafeMovementLocation = Vector3.Zero;
                        PlayerMover.TimeLastRecordedPosition = DateTime.UtcNow;

                        // Time based wait delay for certain powers with animations
                        if (CombatBase.CurrentPower != null)
                        {
                            if (CombatBase.CurrentPower.ShouldWaitAfterUse && Trinity.TrinityPlugin._isWaitingAfterPower || Trinity.TrinityPlugin._isWaitingBeforePower && CombatBase.CurrentPower.ShouldWaitBeforeUse)
                            {
                                var type = Trinity.TrinityPlugin._isWaitingAfterPower ? "IsWaitingAfterPower" : "IsWaitingBeforePower";
                                _waitedTicks++;
                                Logger.LogVerbose($"Waiting... {type} Power={CombatBase.CurrentPower.SNOPower} TicksWaited={_waitedTicks}");
                                return GetRunStatus(RunStatus.Running, type);
                            }
                        }
                    }

                    if (CombatBase.IsDoingGoblinKamakazi && Trinity.TrinityPlugin.CurrentTarget != null && Trinity.TrinityPlugin.CurrentTarget.Type != TrinityObjectType.Door && Trinity.TrinityPlugin.CurrentTarget.Type != TrinityObjectType.Barricade && !Trinity.TrinityPlugin.CurrentTarget.InternalName.ToLower().Contains("corrupt") && Trinity.TrinityPlugin.CurrentTarget.Weight >= Weighting.MaxWeight)
                    {
                        Logger.Log("Forcing Target to Goblin '{0} ({1})' Distance={2}", CombatBase.KamakaziGoblin.InternalName, CombatBase.KamakaziGoblin.ActorSnoId, CombatBase.KamakaziGoblin.Distance);
                        Trinity.TrinityPlugin.CurrentTarget = CombatBase.KamakaziGoblin;
                    }

                    if (CombatBase.IsDoingGoblinKamakazi && Trinity.TrinityPlugin.CurrentTarget == null)
                    {
                        Logger.Log("No Target, Switching to Goblin '{0} ({1})' Distance={2}", CombatBase.KamakaziGoblin.InternalName, CombatBase.KamakaziGoblin.ActorSnoId, CombatBase.KamakaziGoblin.Distance);
                        Trinity.TrinityPlugin.CurrentTarget = CombatBase.KamakaziGoblin;
                    }

                    //if (ShouldWaitForLootDrop)
                    //{
                    //    Logger.LogVerbose("Wait for loot drop");
                    //}

                    if (_isWaitingForAttackToFinish)
                    {
                        Logger.LogVerbose("Wait for Attack to finish");
                    }

                    if (Trinity.TrinityPlugin._isWaitingBeforePower)
                    {
                        Logger.LogVerbose("Wait Before Power Finished");
                    }

                    if (Trinity.TrinityPlugin._isWaitingAfterPower)
                    {
                        Logger.LogVerbose("Wait After Power Finished");
                    }

                    if (Trinity.TrinityPlugin._isWaitingForPotion)
                    {
                        Logger.LogVerbose("Wait for Potion");
                    }

                    if (Trinity.TrinityPlugin.CurrentTarget == null)
                    {
                        Logger.LogVerbose("CurrentTarget == null");
                    }


                    if (InteractionWaitUntilTime > DateTime.UtcNow)
                    {
                        Logger.Log("Waiting after interaction");
                        return RunStatus.Running;
                    }

                        //if (ClearArea.ShouldMoveToPortalPosition)
                        //    return RunStatus.Success;

                        _waitedTicks = 0;
                    Trinity.TrinityPlugin._isWaitingAfterPower = false;
                    Trinity.TrinityPlugin._isWaitingBeforePower = false;

                    if (!Core.Avoidance.Avoider.ShouldAvoid)
                    {
                        if (!Trinity.TrinityPlugin._isWaitingForPower && !Trinity.TrinityPlugin._isWaitingBeforePower && (CombatBase.CurrentPower == null || CombatBase.CurrentPower.SNOPower == SNOPower.None) && Trinity.TrinityPlugin.CurrentTarget != null)
                        {
                            CombatBase.CurrentPower = CombatManager.AbilitySelector.SelectAbility();

                            Logger.LogVerbose(LogCategory.Behavior, $"Selected Power {CombatBase.CurrentPower}");

                            if (CombatBase.CurrentPower.SNOPower == SNOPower.None)
                            {
                                Logger.LogVerbose(LogCategory.Behavior, "SNOPower.None selected from combat routine :S");
                                Trinity.TrinityPlugin._shouldPickNewAbilities = true;
                            }

                        }
                        else
                        {
                            Logger.LogVerbose(LogCategory.Behavior, "Not Avoiding WaitingForPower={0} WaitingBeforePower={1} CurrentPower={2} CurrentTarget={3}",
                                Trinity.TrinityPlugin._isWaitingForPower, Trinity.TrinityPlugin._isWaitingBeforePower, CombatBase.CurrentPower, Trinity.TrinityPlugin.CurrentTarget);
                        }
                    }

                    // Prevent running away after progression globes spawn if they're in aoe
                    if (Core.Player.IsInRift && !Core.Avoidance.Avoider.ShouldAvoid)
                    {                       
                        var globes = Trinity.TrinityPlugin.Targets.Where(o => o.Type == TrinityObjectType.ProgressionGlobe && o.Distance < AvoidanceManager.MaxDistance).ToList();
                        var shouldWaitForGlobes = globes.Any(o => Core.Avoidance.Grid.IsIntersectedByFlags(ZetaDia.Me.Position, o.Position, AvoidanceFlags.CriticalAvoidance));
                        if (shouldWaitForGlobes)
                        {                          
                            Logger.Log($"Waiting for progression globe GlobeCount={globes.Count}");
                            var globe = globes.FirstOrDefault();
                            if (globe != null)
                            {
                                Navigator.PlayerMover.MoveTowards(globe.Position);
                            }
                            return RunStatus.Running;
                        }
                    }

                    // Some skills we need to wait to finish (like cyclone strike while epiphany is active)
                    if (_isWaitingForAttackToFinish)
                    {
                        if (ZetaDia.Me.LoopingAnimationEndTime > 0 || ZetaDia.Me.CommonData.AnimationState == AnimationState.Attacking || ZetaDia.Me.CommonData.AnimationState == AnimationState.Casting || ZetaDia.Me.CommonData.AnimationState == AnimationState.Transform)
                        {
                            Logger.LogVerbose(LogCategory.Behavior, $"Waiting for Attack to Finish CurrentPower={CombatBase.CurrentPower}");
                            return GetRunStatus(RunStatus.Running, "WaitForAttackToFinish");
                        }
                        _isWaitingForAttackToFinish = false;
                    }

                    // See if we have been "newly rooted", to force target updates
                    if (Core.Player.IsRooted && !Trinity.TrinityPlugin.wasRootedLastTick)
                    {
                        Trinity.TrinityPlugin.wasRootedLastTick = true;
                        Trinity.TrinityPlugin._forceTargetUpdate = true;
                    }
                    if (!Core.Player.IsRooted)
                        Trinity.TrinityPlugin.wasRootedLastTick = false;

                    if (Trinity.TrinityPlugin.CurrentTarget == null)
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "CurrentTarget was passed as null! Continuing...");
                    }

                    // Refresh the object Cache every time
                    //RefreshDiaObjectCache();

                    if (!CombatBase.IsCombatAllowed && (Trinity.TrinityPlugin.CurrentTarget == null || Trinity.TrinityPlugin.CurrentTarget.IsUnit))
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "Ending: Combat is Disabled!");
                        return RunStatus.Failure;
                    }

                    if (Trinity.TrinityPlugin.CurrentTarget == null && TrinityTownRun.IsWantingTownRun)
                    {
                        Logger.Log(TrinityLogLevel.Info, LogCategory.Behavior, "CurrentTarget is null, need to town run, returning ");
                        return GetRunStatus(RunStatus.Success, "CurrentTargetNull");
                    }

                    //while (CurrentTarget == null && TownRun.IsTryingToTownPortal() && TownRun.TownRunTimerRunning())
                    //{
                    //    Logger.Log(TrinityLogLevel.Info, LogCategory.Behavior, "Waiting for town run... ");
                    //    return GetRunStatus(RunStatus.Running, "TownRunWait");
                    //}

                    //if (CurrentTarget == null && TownRun.IsTryingToTownPortal() && TownRun.TownRunTimerFinished())
                    //{
                    //    Logger.Log(TrinityLogLevel.Info, LogCategory.Behavior, "Town Run Ready!");
                    //    return GetRunStatus(RunStatus.Success, "TownRunRead");
                    //}


                    if (Trinity.TrinityPlugin.CurrentTarget == null)
                    {
                        Logger.Log(TrinityLogLevel.Info, LogCategory.Behavior, "CurrentTarget set as null in refresh! Error 2, Returning Failure");
                        return GetRunStatus(RunStatus.Failure, "CurrentTargetNull2");
                    }

                    //// Handle Target stuck / timeout // blacklist should be handled already in weighting 
                    //var targetName = CurrentTarget.InternalName;
                    //if (HandleTargetTimeoutTask())
                    //{
                    //    Logger.LogVerbose(LogCategory.Behavior, "Blacklisted Target: {0}, Returning Failure", targetName);
                    //    return GetRunStatus(RunStatus.Running, "BlackListTarget");
                    //}

                    if (Trinity.TrinityPlugin.CurrentTarget != null)
                        AssignPower();

                    using (new PerformanceLogger("HandleTarget.CheckAvoidanceBuffs"))
                    {
                        if (TryCastAvoidancePower(out status))
                            return status;
                    }


                    Trinity.TrinityPlugin.TargetRangeRequired = Math.Max(2f, Trinity.TrinityPlugin.CurrentTarget.RequiredRange);
                    Trinity.TrinityPlugin.TargetCurrentDistance = Trinity.TrinityPlugin.CurrentTarget.RadiusDistance;

                    switch (Trinity.TrinityPlugin.CurrentTarget.Type)
                    {
                        case TrinityObjectType.Gold:
                            Trinity.TrinityPlugin.CurrentDestination = MathEx.CalculatePointFrom(Core.Player.Position, Trinity.TrinityPlugin.CurrentTarget.Position, -2f);
                            break;
                        case TrinityObjectType.Interactable:
                            Trinity.TrinityPlugin.CurrentDestination = MathEx.CalculatePointFrom(Trinity.TrinityPlugin.CurrentTarget.Position, Core.Player.Position, Trinity.TrinityPlugin.CurrentTarget.Radius + 2f);
                            break;
                        default:
                            Trinity.TrinityPlugin.CurrentDestination = Trinity.TrinityPlugin.CurrentTarget.Position;
                            break;
                    }

                    // SetRangeRequiredForTarget();

                    //using (new PerformanceLogger("HandleTarget.SpecialNavigation"))
                    //{
                    //    PositionCache.AddPosition();

                    //    // Maintain an area list of all zones we pass through/near while moving, for our custom navigation handler
                    //    if (DateTime.UtcNow.Subtract(LastAddedLocationCache).TotalMilliseconds >= 100)
                    //    {
                    //        LastAddedLocationCache = DateTime.UtcNow;
                    //        if (Vector3.Distance(Player.Position, LastRecordedPosition) >= 5f)
                    //        {
                    //            SkipAheadAreaCache.Add(new CacheObstacleObject(Player.Position, 20f, 0));
                    //            LastRecordedPosition = Player.Position;

                    //        }
                    //    }
                    //}

                    Trinity.TrinityPlugin.TargetCurrentDistance = Trinity.TrinityPlugin.CurrentTarget.RadiusDistance;

                    Trinity.TrinityPlugin.CurrentTargetWithinRange = IsWithinRange(Trinity.TrinityPlugin.CurrentTarget);

                   


                    //using (new PerformanceLogger("HandleTarget.LoSCheck"))
                    //{                        

                    //    if (DataDictionary.AlwaysRaycastWorlds.Contains(Player.WorldSnoId) && CurrentTarget.Distance > CurrentTarget.Radius + 2f)
                    //    {
                    //        CurrentTargetIsInLoS = NavHelper.CanRayCast(Player.Position, CurrentDestination);
                    //    }
                    //    else if (TargetCurrentDistance <= 2f)
                    //    {
                    //        CurrentTargetIsInLoS = true;
                    //    }
                    //    else if (CurrentTarget.IsUnit && CurrentTarget.Unit.IsHidden)
                    //    {
                    //        CurrentTargetIsInLoS = false;
                    //    }
                    //    else if (Settings.Combat.Misc.UseNavMeshTargeting && CurrentTarget.Type != TrinityObjectType.Barricade && CurrentTarget.Type != TrinityObjectType.Destructible)
                    //    {
                    //        CurrentTargetIsInLoS = (NavHelper.CanRayCast(Player.Position, CurrentDestination) || DataDictionary.LineOfSightWhitelist.Contains(CurrentTarget.ActorSnoId));
                    //    }
                    //    else
                    //    {
                    //        CurrentTargetIsInLoS = true;
                    //    }
                    //}

                    using (new PerformanceLogger("HandleTarget.InRange"))
                    {
                        bool stuckOnTarget =
                            ((Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Barricade ||
                            Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Item ||
                             Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Interactable ||
                             Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.CursedChest ||
                             Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.CursedShrine ||
                             Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Destructible) &&
                             !ZetaDia.Me.Movement.IsMoving && DateTime.UtcNow.Subtract(PlayerMover.TimeLastUsedPlayerMover).TotalMilliseconds < 250);

                        bool noRangeRequired = Trinity.TrinityPlugin.TargetRangeRequired <= 1f;
                        switch (Trinity.TrinityPlugin.CurrentTarget.Type)
                        {
                            // These always have TargetRangeRequired=1f, but, we need to run directly to their center until we stop moving, then destroy them
                            case TrinityObjectType.Door:
                            case TrinityObjectType.Barricade:
                            case TrinityObjectType.Destructible:
                                noRangeRequired = false;
                                break;
                        }

                        if (Trinity.TrinityPlugin.CurrentTarget.IsSpawningBoss && Core.Player.IsInRift)
                        {
                            Logger.LogVerbose("Rift Boss is Spawning!");

                            if (!TargetUtil.AnyTrashInRange(20f) && !Gems.Taeguk.IsEquipped) {
                                Logger.LogVerbose(LogCategory.Avoidance, "Waiting for Rift Boss to Spawn");
                                Core.Player.CurrentAction = PlayerAction.Waiting;
                                return RunStatus.Running;
                            }
                        }

                        // Interact/use power on target if already in range
                        if (!Trinity.TrinityPlugin.CurrentTarget.IsSafeSpot && (noRangeRequired || (Trinity.TrinityPlugin.TargetCurrentDistance <= Trinity.TrinityPlugin.TargetRangeRequired && Trinity.TrinityPlugin.CurrentTargetWithinRange) || stuckOnTarget))
                        {
                            Logger.LogDebug(LogCategory.Behavior, "Object in Range: noRangeRequired={0} Target In Range={1} stuckOnTarget={2} npcInRange={3} power={4} target={5}", noRangeRequired, (Trinity.TrinityPlugin.TargetCurrentDistance <= Trinity.TrinityPlugin.TargetRangeRequired && Trinity.TrinityPlugin.CurrentTargetWithinRange), stuckOnTarget, string.Empty, CombatBase.CurrentPower.SNOPower, Trinity.TrinityPlugin.CurrentTarget);
                            Core.Player.CurrentAction = PlayerAction.Moving;

                            HandleObjectInRange();
                            return GetRunStatus(RunStatus.Running, "HandleObjectInRange");
                        }
                    }                    

                    // Are we currently incapacitated? If so then wait...
                    if (Core.Player.IsIncapacitated || Core.Player.IsRooted)
                    {
                        Logger.Log(LogCategory.Behavior, "Player is rooted or incapacitated!");
                        return GetRunStatus(RunStatus.Running, "PlayerRooted");
                    }

                    // Check to see if we're stuck in moving to the target
                    if (HandleTargetDistanceCheck())
                    {
                        return GetRunStatus(RunStatus.Running, "DistanceCheck");
                    }
                    // Update the last distance stored
                    Trinity.TrinityPlugin.LastDistanceFromTarget = Trinity.TrinityPlugin.TargetCurrentDistance;

                    // Only position-shift when not avoiding
                    // See if we want to ACTUALLY move, or are just waiting for the last move command...
                    if (!Trinity.TrinityPlugin.ForceNewMovement && Trinity.TrinityPlugin.IsAlreadyMoving && Trinity.TrinityPlugin.CurrentDestination == Trinity.TrinityPlugin.LastMoveToTarget && DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.lastMovementCommand).TotalMilliseconds <= 100)
                    {
                        // return GetTaskResult(true);
                    }

                    if (!Core.Player.IsInTown)
                    {
                        Logger.Log(LogCategory.Behavior, "Player is Attempting Special Movement!");
                        RunStatus specialMovementResult;
                        if (TrySpecialMovement(out specialMovementResult)) 
                            return specialMovementResult;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error in HandleTarget: {0}", ex);
                    return GetRunStatus(RunStatus.Failure, "Exception");
                }

                HandleTargetBasicMovement(Trinity.TrinityPlugin.CurrentDestination, Trinity.TrinityPlugin.ForceNewMovement);

                Logger.LogDebug(LogCategory.Behavior, "End of HandleTarget");
                return GetRunStatus(RunStatus.Running, "End");
            }
        }

        /// <summary>
        /// Determine if the target is attackable/interactable, false will cause the bot to move towards target.
        /// </summary>
        public bool IsWithinRange(TrinityActor actor)
        {
            if (actor.Distance <= 2f)
                return true;

            if (Core.Actors.AllRActors.Any(r => r.ActorSnoId == (int)SNOActor.x1_Fortress_Portal_Switch && r.Distance <= 40f))
                return actor.IsWalkable;
  
            if (DataDictionary.LineOfSightWhitelist.Contains(actor.ActorSnoId))
                return true;

            return actor.IsUnit && actor.Distance < 50f && !actor.IsGizmo ? actor.IsInLineOfSight : actor.IsWalkable;
        }

        public bool TryCastAvoidancePower(out RunStatus status)
        {
            if ((Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Avoidance || Trinity.TrinityPlugin.CurrentTarget.IsSafeSpot) && !Trinity.TrinityPlugin.CurrentTarget.IsWaitSpot)
            {
                Trinity.TrinityPlugin.powerBuff = CombatManager.AbilitySelector.SelectAbility(true);
                if (Trinity.TrinityPlugin.powerBuff != null && Trinity.TrinityPlugin.powerBuff.SNOPower != SNOPower.None)
                {
                    Logger.LogVerbose(LogCategory.Behavior, "HandleTarget: Casting {0} for Avoidance", Trinity.TrinityPlugin.powerBuff.SNOPower);

                    if (ZetaDia.Me.UsePower(Trinity.TrinityPlugin.powerBuff.SNOPower, Trinity.TrinityPlugin.powerBuff.TargetPosition, Trinity.TrinityPlugin.powerBuff.TargetDynamicWorldId, Trinity.TrinityPlugin.powerBuff.TargetAcdId))
                    {
                        Trinity.TrinityPlugin.LastPowerUsed = Trinity.TrinityPlugin.powerBuff.SNOPower;
                        SpellHistory.RecordSpell(Trinity.TrinityPlugin.powerBuff.SNOPower);
                        {
                            status = GetRunStatus(RunStatus.Running, "Cast Avoidance Spell");
                            return true;
                        }
                    }
                    LastActionTimes.Add(DateTime.UtcNow);
                }
            }
            status = default(RunStatus);
            return false;
        }

        public List<DateTime> LastActionTimes
        {
            get { return _lastActionTimes ?? (_lastActionTimes = new List<DateTime>()); }
            set { _lastActionTimes = value; }
        }

        /// <summary>
        /// Doing too many things in too short of at time will disconnect you from the Diablo3 client
        /// This includes dropping items, casting spells, probably anything injection based.
        /// This checks a collection of recorded actions and waits if too much has happened too fast.
        /// </summary>
        public bool ThrottleActionPerSecond(out RunStatus runStatus)
        {            
            const int measureTimeMs = 500;
            const int actionLimit = 6;
            DateTime actionLimitTime;

            if (LastActionTimes.Count >= actionLimit)
            {
                actionLimitTime = LastActionTimes.ElementAt(LastActionTimes.Count - actionLimit);
            }
            else
            {
                actionLimitTime = LastActionTimes.LastOrDefault();
            }

            while (LastActionTimes.Count > 25)
            {
                LastActionTimes.RemoveAt(0);
            }

            //if (CurrentTarget != null)
            //{
            //    var target = ZetaDia.Actors.GetActorByACDId(CurrentTarget.AcdId) as DiaUnit;

            //    Logger.Log($"CurrentTarget={CurrentTarget?.InternalName} SNO={CurrentTarget?.ActorSnoId} Distance={CurrentTarget?.Distance} " +
            //               $"CurrentPower={CombatBase.CurrentPower} TargetAcd={CurrentTarget?.AcdId} TargetValid={target.IsFullyValid()} " +
            //               $"TargetAlive={target?.IsAlive} TargetHealth={CurrentTarget.HitPoints} " +
            //               $"PowerTargetDistance={CombatBase.CurrentPower.TargetPosition.Distance(ZetaDia.Me.Position)} " +
            //               $"TargetDistance={target?.Distance} TimeSinceLastUse={SpellHistory.TimeSinceUse(CombatBase.CurrentPower.SNOPower)}");
            //}

            // Wait until NTh action happend more than than half the measure time ago          
            var timeSince = DateTime.UtcNow.Subtract(actionLimitTime).TotalMilliseconds;
            if (timeSince < measureTimeMs / 2) 
            {
                Logger.LogDebug(LogCategory.Behavior, "Throttling - Actions Per Second Limit Reached! {0} actions were taken within {1}ms", actionLimit, timeSince);               
                //Logger.Warn($"Throttling - Actions Per Second Limit Reached! {actionLimit} actions were taken within {timeSince}ms");               
                runStatus = RunStatus.Running;
                return true;                
            }

            runStatus = default(RunStatus);
            return false;
        }

        /// <summary>
        /// Try to use a special movement skill like Monk Dashing Strike or Wizard Teleport
        /// </summary>
        public bool TrySpecialMovement(out RunStatus statusResult)
        {
            if (!CombatBase.IsInCombat && !Core.Settings.Combat.Misc.AllowOOCMovement)
            {
                statusResult = default(RunStatus);
                return false;
            }

            using (new PerformanceLogger("HandleTarget.TrySpecialMovement"))
            { 
                if (ClassMover.SpecialMovement(Trinity.TrinityPlugin.CurrentDestination))
                {
                    // Try to ensure the bot isn't navigating to somewhere behind us.
                    Navigator.Clear();
                    Trinity.TrinityPlugin.LastMoveToTarget = Trinity.TrinityPlugin.CurrentDestination;
                    statusResult = GetRunStatus(RunStatus.Running, "SpecialMovement");
                    return true;
                }
            }
            statusResult = RunStatus.Failure;
            return false;
        }


        public void HandleObjectInRange()
        {
            Logger.LogVerbose(LogCategory.Behavior, "CurrentTarget is {0}", Trinity.TrinityPlugin.CurrentTarget);

            switch (Trinity.TrinityPlugin.CurrentTarget.Type)
            {
                case TrinityObjectType.Avoidance:
                    Trinity.TrinityPlugin._forceTargetUpdate = true;
                    break;
                case TrinityObjectType.Player:
                    break;

                // Unit, use our primary power to attack
                case TrinityObjectType.Unit:
                    {
                        if (CombatBase.CurrentPower.SNOPower != SNOPower.None)
                        {
                            if (Trinity.TrinityPlugin._isWaitingForPower && CombatBase.CurrentPower.ShouldWaitBeforeUse)
                            {
                            }
                            else if (Trinity.TrinityPlugin._isWaitingForPower && !CombatBase.CurrentPower.ShouldWaitBeforeUse)
                            {
                                Trinity.TrinityPlugin._isWaitingForPower = false;
                            }
                            else
                            {
                                Trinity.TrinityPlugin._isWaitingForPower = false;
                                HandleUnitInRange();
                            }
                        }
                        break;
                    }
                // Item, interact with it and log item stats
                case TrinityObjectType.Item:
                    {
                        // Check if we actually have room for this item first
                        var item = Trinity.TrinityPlugin.CurrentTarget as TrinityItem;
                        bool isTwoSlot = true;
                        if (item != null && item.IsValid)
                        {
                            isTwoSlot = item.IsTwoSquareItem;
                        }

                        Vector2 validLocation = TrinityItemManager.FindValidBackpackLocation(isTwoSlot);
                        if (validLocation.X < 0 || validLocation.Y < 0)
                        {
                            Logger.Log("No more space to pickup item, town-run requested at next free moment. (HandleTarget)");
                            //ForceVendorRunASAP = true;

                            //// Record the first position when we run out of bag space, so we can return later
                            //TownRun.SetPreTownRunPosition();
                        }
                        else
                        {
                            HandleItemInRange();
                        }
                        break;
                    }
                // * Gold & Globe - need to get within pickup radius only
                case TrinityObjectType.BloodShard:
                case TrinityObjectType.Gold:
                case TrinityObjectType.HealthGlobe:
                case TrinityObjectType.PowerGlobe:
                case TrinityObjectType.ProgressionGlobe:
                    {
                        int interactAttempts;
                        // Count how many times we've tried interacting
                        if (!Core.Targets.InteractAttempts.TryGetValue(Trinity.TrinityPlugin.CurrentTarget.RActorId, out interactAttempts))
                        {
                            Core.Targets.InteractAttempts.Add(Trinity.TrinityPlugin.CurrentTarget.RActorId, 1);
                        }
                        else
                        {
                            Core.Targets.InteractAttempts[Trinity.TrinityPlugin.CurrentTarget.RActorId]++;
                        }
                        // If we've tried interacting too many times, blacklist this for a while
                        if (interactAttempts > 3)
                        {
                            Trinity.TrinityPlugin.Blacklist3Seconds.Add(Trinity.TrinityPlugin.CurrentTarget.AnnId);
                        }
                        Trinity.TrinityPlugin._ignoreRactorGuid = Trinity.TrinityPlugin.CurrentTarget.RActorId;
                        Trinity.TrinityPlugin._ignoreTargetForLoops = 3;

                        // Now tell TrinityPlugin to get a new target!
                        Trinity.TrinityPlugin._forceTargetUpdate = true;
                        break;
                    }

                case TrinityObjectType.Door:
                case TrinityObjectType.HealthWell:
                case TrinityObjectType.Shrine:
                case TrinityObjectType.Container:
                case TrinityObjectType.Interactable:
                case TrinityObjectType.CursedChest:
                case TrinityObjectType.CursedShrine:
                    {
                        Trinity.TrinityPlugin._forceTargetUpdate = true;

                        Core.Player.CurrentAction = PlayerAction.Interacting;

                        if (ZetaDia.Me.Movement.SpeedXY > 0.5 && Trinity.TrinityPlugin.CurrentTarget.Distance < 8f)
                        {
                            Logger.LogVerbose(LogCategory.Behavior, "Trying to stop, Speeds:{0:0.00}/{1:0.00}", ZetaDia.Me.Movement.SpeedXY, PlayerMover.GetMovementSpeed());
                            Navigator.PlayerMover.MoveStop();
                        }
                        else
                        {
                            if (SpellHistory.TimeSinceUse(SNOPower.Axe_Operate_Gizmo) < TimeSpan.FromMilliseconds(150))
                            {
                                break;
                            }

                            int attemptCount;
                            Core.Targets.InteractAttempts.TryGetValue(Trinity.TrinityPlugin.CurrentTarget.RActorId, out attemptCount);

                            Logger.LogDebug(LogCategory.UserInformation, "Interacting with {1} Distance {2:0} Radius {3:0.0} Attempt {4}",
                                     SNOPower.Axe_Operate_Gizmo, Trinity.TrinityPlugin.CurrentTarget.InternalName, Trinity.TrinityPlugin.CurrentTarget.Distance, Trinity.TrinityPlugin.CurrentTarget.Radius, attemptCount);

                            if (Trinity.TrinityPlugin.CurrentTarget.ActorType == ActorType.Monster)
                            {
                                if (ZetaDia.Me.UsePower(SNOPower.Axe_Operate_NPC, Vector3.Zero, Trinity.TrinityPlugin.CurrentWorldDynamicId, Trinity.TrinityPlugin.CurrentTarget.AcdId))
                                {
                                    SpellHistory.RecordSpell(new TrinityPower()
                                    {
                                        SNOPower = SNOPower.Axe_Operate_Gizmo,
                                        TargetAcdId = Trinity.TrinityPlugin.CurrentTarget.AcdId,
                                        MinimumRange = Trinity.TrinityPlugin.TargetRangeRequired,
                                        TargetPosition = Trinity.TrinityPlugin.CurrentTarget.Position,
                                    });
                                }
                                else
                                {
                                    CombatManager.TargetHandler.LastActionTimes.Add(DateTime.UtcNow);
                                }
                            }
                            else
                            {
                                //Navigator.PlayerMover.MoveTowards(CurrentCacheObject.Position);
                                //CurrentTarget.Object.Interact();

             
                                Logger.LogNormal("Interacting with {0}", Trinity.TrinityPlugin.CurrentTarget.InternalName);
                                //if (ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, CurrentTarget.AcdId))
                                //ZetaDia.Me.UsePower(SNOPower.Interact_Crouching, Vector3.Zero, 0, CurrentTarget.AcdId)
                                //var hasBeenOperated = c_diaObject is DiaGizmo && (c_diaObject as DiaGizmo).HasBeenOperated;
                                
                                if (!Trinity.TrinityPlugin.CurrentTarget.IsUsed && ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, Trinity.TrinityPlugin.CurrentTarget.AcdId))
                                {
                                    SpellHistory.RecordSpell(new TrinityPower()
                                    {
                                        SNOPower = SNOPower.Axe_Operate_Gizmo,
                                        TargetAcdId = Trinity.TrinityPlugin.CurrentTarget.AcdId,
                                        MinimumRange = Trinity.TrinityPlugin.TargetRangeRequired,
                                        TargetPosition = Trinity.TrinityPlugin.CurrentTarget.Position,
                                    });
                                }
                                else
                                {
                                    Trinity.TrinityPlugin.CurrentTarget.Interact();

                                    //{
                                    //    Logger.LogNormal("Fallback Interact on {0}", CurrentCacheObject.InternalName);
                                    //    SpellHistory.RecordSpell(new TrinityPower()
                                    //    {
                                    //        SNOPower = SNOPower.Axe_Operate_Gizmo,
                                    //        TargetAcdId = CurrentTarget.AcdId,
                                    //        MinimumRange = TargetRangeRequired,
                                    //        TargetPosition = CurrentTarget.Position,
                                    //    });
                                    //}
                                    //else
                                    //{
                                    //    CombatManager.TargetHandler.LastActionTimes.Add(DateTime.UtcNow);
                                    //}

                                }
                            }

                            // Count how many times we've tried interacting
                            if (!Core.Targets.InteractAttempts.TryGetValue(Trinity.TrinityPlugin.CurrentTarget.RActorId, out attemptCount))
                            {
                                Core.Targets.InteractAttempts.Add(Trinity.TrinityPlugin.CurrentTarget.RActorId, 1);
                            }
                            else
                            {
                                Core.Targets.InteractAttempts[Trinity.TrinityPlugin.CurrentTarget.RActorId] += 1;
                            }

                            var attempts = Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Shrine ? 8 : 20;

                            // If we've tried interacting too many times, blacklist this for a while
                            if (Core.Targets.InteractAttempts[Trinity.TrinityPlugin.CurrentTarget.RActorId] > attempts && Trinity.TrinityPlugin.CurrentTarget.Type != TrinityObjectType.HealthWell)
                            {
                                Logger.LogVerbose("Blacklisting {0} ({1}) for 60 seconds after {2} interactions",
                                    Trinity.TrinityPlugin.CurrentTarget.InternalName, Trinity.TrinityPlugin.CurrentTarget.ActorSnoId, attemptCount);

                                Core.Targets.InteractAttempts[Trinity.TrinityPlugin.CurrentTarget.RActorId] = 0;
                                GenericBlacklist.Blacklist(Trinity.TrinityPlugin.CurrentTarget, TimeSpan.FromSeconds(15), "Too Many Interaction Attempts");
                                //Blacklist60Seconds.Add(CurrentTarget.AnnId);
                                //Blacklist60LastClear = DateTime.UtcNow;
                            }
                        }
                        break;
                    }
                // * Destructible - need to pick an ability and attack it
                case TrinityObjectType.Destructible:
                case TrinityObjectType.Barricade:
                    {

                        Core.Player.CurrentAction = PlayerAction.Attacking;

                        if (CombatBase.CurrentPower.SNOPower != SNOPower.None)
                        {
                            if (Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Barricade)
                            {
                                Logger.Log(TrinityLogLevel.Verbose, LogCategory.Behavior,
                                    "Barricade: Name={0}. SNO={1}, Range={2}. Needed range={3}. Radius={4}. Type={5}. Using power={6}",
                                    Trinity.TrinityPlugin.CurrentTarget.InternalName,     // 0
                                    Trinity.TrinityPlugin.CurrentTarget.ActorSnoId,         // 1
                                    Trinity.TrinityPlugin.CurrentTarget.Distance,         // 2
                                    Trinity.TrinityPlugin.TargetRangeRequired,            // 3
                                    Trinity.TrinityPlugin.CurrentTarget.Radius,           // 4
                                    Trinity.TrinityPlugin.CurrentTarget.Type,             // 5
                                    CombatBase.CurrentPower.SNOPower// 6 
                                    );
                            }
                            else
                            {
                                Logger.Log(TrinityLogLevel.Verbose, LogCategory.Behavior,
                                    "Destructible: Name={0}. SNO={1}, Range={2}. Needed range={3}. Radius={4}. Type={5}. Using power={6}",
                                    Trinity.TrinityPlugin.CurrentTarget.InternalName,       // 0
                                    Trinity.TrinityPlugin.CurrentTarget.ActorSnoId,           // 1
                                    Trinity.TrinityPlugin.TargetCurrentDistance,            // 2
                                    Trinity.TrinityPlugin.TargetRangeRequired,              // 3 
                                    Trinity.TrinityPlugin.CurrentTarget.Radius,             // 4
                                    Trinity.TrinityPlugin.CurrentTarget.Type,               // 5
                                    CombatBase.CurrentPower.SNOPower  // 6
                                    );
                            }

                            var vAttackPoint = Trinity.TrinityPlugin.CurrentTarget.Position;
                            if (DataDictionary.DestroyAtLocationIds.Contains(Trinity.TrinityPlugin.CurrentTarget.ActorSnoId))
                            {
                                // Location attack - attack the Vector3/map-area (equivalent of holding shift and left-clicking the object in-game to "force-attack")                                
                                if (Trinity.TrinityPlugin.CurrentTarget.Distance >= 6f)
                                    vAttackPoint = MathEx.CalculatePointFrom(Trinity.TrinityPlugin.CurrentTarget.Position, Core.Player.Position, 6f);                                     
                            }
                            vAttackPoint.Z += 1.5f;

                            // try with routine selected a destructible power
                            var destructiblePower = CombatManager.AbilitySelector.SelectAbility();
                            if (destructiblePower == null || destructiblePower.SNOPower == SNOPower.Walk || destructiblePower.SNOPower == SNOPower.None)
                            {
                                destructiblePower = CombatBase.DefaultPower;
                                Navigator.PlayerMover.MoveTowards(Trinity.TrinityPlugin.CurrentTarget.Position);
                            }

                            Logger.LogVerbose($"Attacking Destructable Power={destructiblePower}");

                            // try with acdId
                            if (ZetaDia.Me.UsePower(destructiblePower.SNOPower, vAttackPoint, -1, Trinity.TrinityPlugin.CurrentTarget.AcdId))
                            {
                                SpellHistory.RecordSpell(destructiblePower.SNOPower);
                                LastActionTimes.Add(DateTime.UtcNow);
                            }
                            else
                            {
                                // try position
                                if (ZetaDia.Me.UsePower(destructiblePower.SNOPower, vAttackPoint))
                                {
                                    SpellHistory.RecordSpell(destructiblePower.SNOPower);
                                    LastActionTimes.Add(DateTime.UtcNow);
                                }
                            }

                            if (CombatBase.CurrentPower.SNOPower == SNOPower.Monk_TempestRush)
                                MonkCombat.LastTempestRushLocation = Trinity.TrinityPlugin.CurrentTarget.Position;
                          

                            int interactAttempts;
                            // Count how many times we've tried interacting
                            if (!Core.Targets.InteractAttempts.TryGetValue(Trinity.TrinityPlugin.CurrentTarget.RActorId, out interactAttempts))
                            {
                                Core.Targets.InteractAttempts.Add(Trinity.TrinityPlugin.CurrentTarget.RActorId, 1);
                            }
                            else
                            {
                                Core.Targets.InteractAttempts[Trinity.TrinityPlugin.CurrentTarget.RActorId]++;
                            }

                            //CacheData.AbilityLastUsed[CombatBase.CurrentPower.SNOPower] = DateTime.UtcNow;

                            // Prevent this EXACT object being targetted again for a short while, just incase
                            Trinity.TrinityPlugin._ignoreRactorGuid = Trinity.TrinityPlugin.CurrentTarget.RActorId;
                            Trinity.TrinityPlugin._ignoreTargetForLoops = 3;
                            // Add this destructible/barricade to our very short-term ignore list
                            //Destructible3SecBlacklist.Add(CurrentTarget.RActorId);
                            Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "Blacklisting {0} {1} {2} for 3 seconds for Destrucable attack", Trinity.TrinityPlugin.CurrentTarget.Type, Trinity.TrinityPlugin.CurrentTarget.InternalName, Trinity.TrinityPlugin.CurrentTarget.ActorSnoId);
                            Trinity.TrinityPlugin._lastDestroyedDestructible = DateTime.UtcNow;
                            Trinity.TrinityPlugin._needClearDestructibles = true;
                        }
                        // Now tell TrinityPlugin to get a new target!
                        Trinity.TrinityPlugin._forceTargetUpdate = true;
                    }
                    break;
                default:
                    {
                        Trinity.TrinityPlugin._forceTargetUpdate = true;
                        Logger.LogError("Default handle target in range encountered for {0} Type: {1}", Trinity.TrinityPlugin.CurrentTarget.InternalName, Trinity.TrinityPlugin.CurrentTarget.Type);
                        break;
                    }
            }
        }

        public bool HandleTargetDistanceCheck()
        {
            using (new PerformanceLogger("HandleTarget.DistanceEqualCheck"))
            {
                // Count how long we have failed to move - body block stuff etc.
                if (Math.Abs(Trinity.TrinityPlugin.TargetCurrentDistance - Trinity.TrinityPlugin.LastDistanceFromTarget) < 5f && PlayerMover.GetMovementSpeed() < 1)
                {
                    Trinity.TrinityPlugin.ForceNewMovement = true;
                    if (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin._lastMovedDuringCombat).TotalMilliseconds >= 250)
                    {
                        Trinity.TrinityPlugin._lastMovedDuringCombat = DateTime.UtcNow;
                        // We've been stuck at least 250 ms, let's go and pick new targets etc.
                        Trinity.TrinityPlugin._timesBlockedMoving++;
                        Trinity.TrinityPlugin._forceCloseRangeTarget = true;
                        Trinity.TrinityPlugin._lastForcedKeepCloseRange = DateTime.UtcNow;
                        // And tell TrinityPlugin to get a new target
                        Trinity.TrinityPlugin._forceTargetUpdate = true;

                        // Reset the emergency loop counter and return success
                        return true;
                    }
                }
                else
                {
                    // Movement has been made, so count the time last moved!
                    Trinity.TrinityPlugin._lastMovedDuringCombat = DateTime.UtcNow;
                }
            }
            return false;
        }

        /// <summary>
        /// Handles target blacklist assignment if necessary, used for all targets (units/gold/items/interactables)
        /// </summary>
        /// <param name="runStatus"></param>
        /// <returns></returns>
        public bool HandleTargetTimeoutTask()
        {
            using (new PerformanceLogger("HandleTarget.TargetTimeout"))
            {

                bool shouldTryBlacklist = false;

                // don't timeout on avoidance
                if (Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Avoidance)
                    return false;

                // don't timeout on legendary items
                if (Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Item && Trinity.TrinityPlugin.CurrentTarget.ItemQualityLevel >= ItemQuality.Legendary)
                    return false;

                // don't timeout if we're actively moving
                if (PlayerMover.GetMovementSpeed() > 1)
                    return false;

                if (ZetaDia.IsPlayingCutscene)
                    return false;

                // GetSecondsSinceTargetUpdate() time changes whenever the current target changes to a different monster
                // or when the current target monster changes in health. 

                if (CurrentTargetIsNonUnit() && GetSecondsSinceTargetUpdate() > 6)
                    shouldTryBlacklist = true;

                if ((CurrentTargetIsUnit() && Trinity.TrinityPlugin.CurrentTarget.IsBoss && GetSecondsSinceTargetUpdate() > 45))
                    shouldTryBlacklist = true;

                if ((CurrentTargetIsUnit() && !Trinity.TrinityPlugin.CurrentTarget.IsBoss && GetSecondsSinceTargetUpdate() > 30))
                    shouldTryBlacklist = true;


                if (shouldTryBlacklist)
                {

                    bool isNavigable = Trinity.TrinityPlugin.CurrentDestination.Distance(Core.Player.Position) < 3f || NavHelper.CanRayCast(Core.Player.Position, Trinity.TrinityPlugin.CurrentDestination);
                    bool addTargetToBlacklist = true;

                    var isKamakaziOnGoblin = Trinity.TrinityPlugin.CurrentTarget.IsUnit && isNavigable && Trinity.TrinityPlugin.CurrentTarget.IsTreasureGoblin && Core.Settings.Combat.Misc.GoblinPriority >= GoblinPriority.Kamikaze;
                    if (isKamakaziOnGoblin)
                    {
                        addTargetToBlacklist = false;
                    }

                    int interactAttempts;
                    Core.Targets.InteractAttempts.TryGetValue(Trinity.TrinityPlugin.CurrentTarget.RActorId, out interactAttempts);

                    if ((Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Door || Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Interactable || Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Container) &&
                        interactAttempts < 45 && DateTime.UtcNow.Subtract(PlayerMover.LastRecordedAnyStuck).TotalSeconds > 15)
                    {
                        addTargetToBlacklist = false;
                    }

                    if (addTargetToBlacklist)
                    {
                        if (Trinity.TrinityPlugin.CurrentTarget.IsBoss)
                        {
                            Trinity.TrinityPlugin.Blacklist3Seconds.Add(Trinity.TrinityPlugin.CurrentTarget.AnnId);
                            Trinity.TrinityPlugin.Blacklist3LastClear = DateTime.UtcNow;
                            TargetUtil.ClearCurrentTarget("HandleTargetTimeoutTask: Blacklisted - CurrentTarget.IsBoss");
                            return true;
                        }

                        if (Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Item && Trinity.TrinityPlugin.CurrentTarget.ItemQualityLevel >= ItemQuality.Legendary)
                        {
                            return false;
                        }

                        if (Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.ProgressionGlobe)
                        {
                            return false;
                        }

                        if (Trinity.TrinityPlugin.CurrentTarget.IsUnit)
                        {
                            Logger.LogDebug(
                                "Blacklisting a monster because of possible stuck issues. Monster={0} [{1}] Range={2:0} health %={3:0} RActorGUID={4}",
                                Trinity.TrinityPlugin.CurrentTarget.InternalName,         // 0
                                Trinity.TrinityPlugin.CurrentTarget.ActorSnoId,             // 1
                                Trinity.TrinityPlugin.CurrentTarget.Distance,       // 2
                                Trinity.TrinityPlugin.CurrentTarget.HitPointsPct,            // 3
                                Trinity.TrinityPlugin.CurrentTarget.RActorId            // 4
                                );
                        }
                        else
                        {
                            Logger.LogDebug(
                                "Blacklisting an object because of possible stuck issues. Object={0} [{1}]. Range={2:0} RActorGUID={3}",
                                Trinity.TrinityPlugin.CurrentTarget.InternalName,         // 0
                                Trinity.TrinityPlugin.CurrentTarget.ActorSnoId,             // 1 
                                Trinity.TrinityPlugin.CurrentTarget.Distance,       // 2
                                Trinity.TrinityPlugin.CurrentTarget.RActorId            // 3
                                );
                        }

                        Trinity.TrinityPlugin.Blacklist15Seconds.Add(Trinity.TrinityPlugin.CurrentTarget.AnnId);
                        Trinity.TrinityPlugin.Blacklist15LastClear = DateTime.UtcNow;
                        TargetUtil.ClearCurrentTarget($"HandleTargetTimeoutTask: Blacklisted: {Trinity.TrinityPlugin.CurrentTarget.Type} - {Trinity.TrinityPlugin.CurrentTarget.InternalName} - {Trinity.TrinityPlugin.CurrentTarget.Distance}");
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Checks to see if we need a new monster power and will assign it to <see cref="CurrentPower"/>, distinguishes destructables/barricades from units
        /// </summary>
        public void AssignPower()
        {
            using (new PerformanceLogger("HandleTarget.AssignMonsterTargetPower"))
            {
                if (CombatBase.CurrentPower.TimeSinceAssignedMs > 500)
                {
                    Trinity.TrinityPlugin._shouldPickNewAbilities = true;
                }

                // Find a valid ability if the target is a monster
                if (Trinity.TrinityPlugin._shouldPickNewAbilities && !Trinity.TrinityPlugin._isWaitingForPower && !Trinity.TrinityPlugin._isWaitingForPotion && !Trinity.TrinityPlugin._isWaitingBeforePower)
                {
                    Trinity.TrinityPlugin._shouldPickNewAbilities = false;
                    if (Trinity.TrinityPlugin.CurrentTarget.IsUnit)
                    {
                        // Pick a suitable ability
                        CombatBase.CurrentPower = CombatManager.AbilitySelector.SelectAbility();

                        if (Core.Player.IsInCombat && CombatBase.CurrentPower.SNOPower == SNOPower.None && !Core.Player.IsIncapacitated)
                        {
                            Trinity.TrinityPlugin.NoAbilitiesAvailableInARow++;
                            if (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.lastRemindedAboutAbilities).TotalSeconds > 60 && Trinity.TrinityPlugin.NoAbilitiesAvailableInARow >= 4)
                            {
                                Trinity.TrinityPlugin.lastRemindedAboutAbilities = DateTime.UtcNow;
                                Logger.Log(TrinityLogLevel.Info, LogCategory.Behavior, "Error: Couldn't find a valid attack ability. Not enough resource for any abilities or all on cooldown");
                                Logger.Log(TrinityLogLevel.Info, LogCategory.Behavior, "If you get this message frequently, you should consider changing your build");
                            }
                        }
                        else
                        {
                            Trinity.TrinityPlugin.NoAbilitiesAvailableInARow = 0;
                        }
                    }

                    if (CombatBase.CurrentPower?.SNOPower == SNOPower.None && (Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Destructible || Trinity.TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Barricade))
                    {
                        Logger.LogDebug("Selecting default destructible power");
                        CombatBase.CurrentPower = CombatBase.DefaultPower;
                    }
                    
                    if (CombatBase.CurrentPower == null || CombatBase.CurrentPower.SNOPower == SNOPower.None)
                    {
                        Trinity.TrinityPlugin._shouldPickNewAbilities = true;
                        Trinity.TrinityPlugin._isWaitingForPower = false;
                        Trinity.TrinityPlugin._isWaitingBeforePower = false;     
                    }
                    
                    return;                                      
                }

                if (!Trinity.TrinityPlugin._isWaitingForPower && CombatBase.CurrentPower == null)
                {
                    CombatBase.CurrentPower = CombatManager.AbilitySelector.SelectAbility(UseOOCBuff: true);
                }
            }
        }

        /// <summary>
        /// Will check <see cref=" Trinity.TrinityPlugin._isWaitingForPotion"/> and Use a Potion if needed
        /// </summary>
        public bool UsePotionIfNeededTask()
        {
            using (new PerformanceLogger("HandleTarget.UseHealthPotionIfNeeded"))
            {
                if (!Core.Player.IsIncapacitated && Core.Player.CurrentHealthPct > 0 && !Core.Player.IsInTown &&
                    SpellHistory.TimeSinceUse(SNOPower.DrinkHealthPotion) > TimeSpan.FromSeconds(30) &&
                    (Core.Player.CurrentHealthPct <= CombatBase.EmergencyHealthPotionLimit || ShouldSnapshot()))
                {
                    if (UsePotion())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool UsePotion()
        {
            var logEntry = ShouldSnapshot() ? "Using Potion to Snapshot Bane of the Stricken!" : "Using Potion";

            var legendaryPotions = Core.Inventory.Backpack.Where(i => i.InternalName.ToLower().Contains("healthpotion_legendary_")).ToList();
            if (legendaryPotions.Any())
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, logEntry, 0);
                int dynamicId = legendaryPotions.FirstOrDefault().AnnId;
                ZetaDia.Me.Inventory.UseItem(dynamicId);
                SpellHistory.RecordSpell(new TrinityPower(SNOPower.DrinkHealthPotion));
                SnapShot.Record();
                return true;
            }

            var potion = ZetaDia.Me.Inventory.BaseHealthPotion;
            if (potion != null)
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, logEntry, 0);
                ZetaDia.Me.Inventory.UseItem(potion.AnnId);
                SpellHistory.RecordSpell(new TrinityPower(SNOPower.DrinkHealthPotion));
                SnapShot.Record();
                return true;
            }
                        
            Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "No Available potions!", 0);
            return false;
        }

        /// <summary>
        /// Checks if we should be snapshotting Bane of the Stricken
        /// </summary>
        public bool ShouldSnapshot()
        {
            if (!Core.Settings.Combat.Misc.TryToSnapshot || !Gems.BaneOfTheStricken.IsEquipped ||
                ZetaDia.Me.AttacksPerSecond < Core.Settings.Combat.Misc.SnapshotAttackSpeed || Core.Player.CurrentHealthPct >= 1)
                return false;

            // Check the last snapshotted attack speed
            if (SnapShot.Last.AttacksPerSecond >= Core.Settings.Combat.Misc.SnapshotAttackSpeed)
                return false;

            return true;
        }

        public bool CurrentTargetIsNonUnit()
        {
            return Trinity.TrinityPlugin.CurrentTarget.Type != TrinityObjectType.Unit;
        }

        public bool CurrentTargetIsUnit()
        {
            return Trinity.TrinityPlugin.CurrentTarget.IsUnit;
        }

        /// <summary>
        /// Returns the number of seconds since our current target was updated
        /// </summary>
        /// <returns></returns>
        public double GetSecondsSinceTargetUpdate()
        {
            return DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.LastPickedTargetTime).TotalSeconds;
        }

        public List<DateTime> _lastActionTimes;

        ///// <summary>
        ///// Updates bot status text with appropriate information if we are moving into range of our <see cref="CurrentTarget"/>
        ///// </summary>
        //public void UpdateStatusTextTarget(bool targetIsInRange)
        //{
        //    if (!Settings.Advanced.DebugInStatusBar)
        //        return;

        //    string action = "";

        //    StringBuilder statusText = new StringBuilder();
        //    if (!targetIsInRange)
        //        action = "Moveto ";
        //    else
        //        switch (CurrentTarget.Type)
        //        {
        //            case TrinityObjectType.Avoidance:
        //                action = "Avoid ";
        //                break;
        //            case TrinityObjectType.Unit:
        //                action = "Attack ";
        //                break;
        //            case TrinityObjectType.Item:
        //            case TrinityObjectType.Gold:
        //            case TrinityObjectType.PowerGlobe:
        //            case TrinityObjectType.HealthGlobe:
        //            case TrinityObjectType.ProgressionGlobe:
        //                action = "Pickup ";
        //                break;
        //            case TrinityObjectType.Interactable:
        //                action = "Interact ";
        //                break;
        //            case TrinityObjectType.Door:
        //            case TrinityObjectType.Container:
        //                action = "Open ";
        //                break;
        //            case TrinityObjectType.Destructible:
        //            case TrinityObjectType.Barricade:
        //                action = "Destroy ";
        //                break;
        //            case TrinityObjectType.Shrine:
        //                action = "Click ";
        //                break;
        //        }
        //    statusText.Append(action);

        //    statusText.Append("Target=");
        //    statusText.Append(CurrentTarget.InternalName);
        //    if (CurrentTarget.IsUnit && CombatBase.CurrentPower.SNOPower != SNOPower.None)
        //    {
        //        statusText.Append(" Power=");
        //        statusText.Append(CombatBase.CurrentPower.SNOPower);
        //    }
        //    //statusText.Append(" Speed=");
        //    //statusText.Append(ZetaDia.Me.Movement.SpeedXY.ToString("0.00"));
        //    statusText.Append(" SNO=");
        //    statusText.Append(CurrentTarget.ActorSnoId.ToString(CultureInfo.InvariantCulture));
        //    statusText.Append(" Elite=");
        //    statusText.Append(CurrentTarget.IsElite.ToString());
        //    statusText.Append(" Weight=");
        //    statusText.Append(CurrentTarget.Weight.ToString("0"));
        //    statusText.Append(" Type=");
        //    statusText.Append(CurrentTarget.Type.ToString());
        //    statusText.Append(" C-Dist=");
        //    statusText.Append(CurrentTarget.Distance.ToString("0.0"));
        //    statusText.Append(" R-Dist=");
        //    statusText.Append(CurrentTarget.RadiusDistance.ToString("0.0"));
        //    statusText.Append(" RangeReq'd=");
        //    statusText.Append(TargetRangeRequired.ToString("0.0"));
        //    statusText.Append(" DistfromTrgt=");
        //    statusText.Append(TargetCurrentDistance.ToString("0"));
        //    statusText.Append(" tHP=");
        //    statusText.Append((CurrentTarget.HitPointsPct * 100).ToString("0"));
        //    statusText.Append(" MyHP=");
        //    statusText.Append((Player.CurrentHealthPct * 100).ToString("0"));
        //    statusText.Append(" MyMana=");
        //    statusText.Append((Player.PrimaryResource).ToString("0"));
        //    statusText.Append(" InLoS=");
        //    statusText.Append(CurrentTargetIsInLoS.ToString());

        //    statusText.Append(String.Format(" Duration={0:0}", DateTime.UtcNow.Subtract(LastPickedTargetTime).TotalSeconds));

        //    if (Settings.Advanced.DebugInStatusBar)
        //    {
        //        _statusText = statusText.ToString();
        //        BotMain.StatusText = _statusText;
        //    }
        //    if (lastStatusText != statusText.ToString())
        //    {
        //        // prevent spam
        //        lastStatusText = statusText.ToString();
        //        Logger.Log(TrinityLogLevel.Debug, LogCategory.Targetting, "{0}", statusText.ToString());
        //        _resetStatusText = true;
        //    }
        //}

        /// <summary>
        /// Moves our player if no special ability is available
        /// </summary>
        /// <param name="bForceNewMovement"></param>
        public void HandleTargetBasicMovement(Vector3 destination, bool bForceNewMovement)
        {
            using (new PerformanceLogger("HandleTarget.HandleBasicMovement"))
            {
                // Now for the actual movement request stuff
                Trinity.TrinityPlugin.IsAlreadyMoving = true;
                Trinity.TrinityPlugin.lastMovementCommand = DateTime.UtcNow;

                Core.Player.CurrentAction = PlayerAction.Moving;

                if (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.lastSentMovePower).TotalMilliseconds >= 250 || Vector3.Distance(Trinity.TrinityPlugin.LastMoveToTarget, destination) >= 2f || bForceNewMovement)
                {

                    if(Trinity.TrinityPlugin.CurrentTarget.IsSafeSpot)
                        Logger.Log(LogCategory.Avoidance, $"Moving to SafeSpot Distance={Trinity.TrinityPlugin.CurrentTarget.Distance}");

                    var distance = destination.Distance(Core.Player.Position);
                    var straightLinePathing = distance <= 10f && !DataDictionary.StraightLinePathingLevelAreaIds.Contains(Core.Player.LevelAreaId) &&  !PlayerMover.IsBlocked && !Navigator.StuckHandler.IsStuck && Core.Grids.CanRayWalk(ZetaDia.Me.Position, destination); //&& NavHelper.CanRayCast(CurrentDestination)

                    string destname = String.Format("{0} {1:0} yds Elite={2} LoS={3} HP={4:0.00} Dir={5}",
                        Trinity.TrinityPlugin.CurrentTarget.InternalName,
                        Trinity.TrinityPlugin.CurrentTarget.Distance,
                        Trinity.TrinityPlugin.CurrentTarget.IsElite,
                        Trinity.TrinityPlugin.CurrentTarget.HasBeenInLoS,
                        Trinity.TrinityPlugin.CurrentTarget.HitPointsPct,
                        MathUtil.GetHeadingToPoint(Trinity.TrinityPlugin.CurrentTarget.Position));

                    MoveResult lastMoveResult;
                    if (straightLinePathing)
                    {
                        lastMoveResult = MoveResult.Moved;
                        // just "Click" 
                        Navigator.PlayerMover.MoveTowards(destination);
                        Logger.LogVerbose(LogCategory.Movement, "MoveTowards Straight line pathing to {0}", destname);
                    }                    
                    else
                    {
                        Logger.LogVerbose(LogCategory.Movement, "NavigateTo Straight line pathing to {0}", destname);
                        lastMoveResult = PlayerMover.NavigateTo(destination, destname);
                    }

                    Trinity.TrinityPlugin.lastSentMovePower = DateTime.UtcNow;

                    //bool inRange = TargetCurrentDistance <= TargetRangeRequired || CurrentTarget.Distance < 10f;
                    //if (lastMoveResult == MoveResult.ReachedDestination && !inRange &&
                    //    CurrentTarget.Type != TrinityObjectType.Item &&
                    //    CurrentTarget.Type != TrinityObjectType.Destructible &&
                    //    CurrentTarget.Type != TrinityObjectType.Barricade)
                    //{
                    //    bool pathFindresult = ((DefaultNavigationProvider)Navigator.NavigationProvider).CanPathWithinDistance(CurrentTarget.Position, CurrentTarget.Radius);
                    //    if (!pathFindresult)
                    //    {
                    //        Blacklist60Seconds.Add(CurrentTarget.RActorId);
                    //        Logger.Log("Unable to navigate to target! Blacklisting {0} SNO={1} RAGuid={2} dist={3:0} "
                    //            + (CurrentTarget.IsElite ? " IsElite " : "")
                    //            + (CurrentTarget.ItemQualityLevel >= ItemQuality.Legendary ? "IsLegendaryItem " : ""),
                    //            CurrentTarget.InternalName, CurrentTarget.ActorSnoId, CurrentTarget.RActorId, CurrentTarget.Distance);
                    //    }
                    //}

                    // Store the current destination for comparison incase of changes next loop
                    Trinity.TrinityPlugin.LastMoveToTarget = destination;
                    // Reset total body-block count, since we should have moved
                    if (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin._lastForcedKeepCloseRange).TotalMilliseconds >= 2000)
                        Trinity.TrinityPlugin._timesBlockedMoving = 0;
                }
            }
        }

        //public void SetRangeRequiredForTarget()
        //{
        //    using (new PerformanceLogger("HandleTarget.SetRequiredRange"))
        //    {


        //        switch (CurrentTarget.Type)
        //        {
        //            // * Unit, we need to pick an ability to use and get within range
        //            case TrinityObjectType.Unit:
        //            {
        //                if (CurrentTarget.IsHidden)
        //                {
        //                    TargetRangeRequired = CurrentTarget.CollisionRadius;
        //                }
        //                else
        //                {
        //                    TargetRangeRequired = CombatBase.CurrentPower.MinimumRange;
        //                }
        //                break;
        //            }
        //            // * Item - need to get within 6 feet and then interact with it
        //            case TrinityObjectType.Item:
        //                {
        //                    TargetRangeRequired = 2f;
        //                    TargetCurrentDistance = CurrentTarget.Distance;
        //                    break;
        //                }
        //            // * Gold - need to get within pickup radius only
        //            case TrinityObjectType.Gold:
        //                {
        //                    TargetRangeRequired = 2f;
        //                    TargetCurrentDistance = CurrentTarget.Distance;
        //                    CurrentDestination = MathEx.CalculatePointFrom(Player.Position, CurrentTarget.Position, -2f);
        //                    break;
        //                }
        //            // * Globes - need to get within pickup radius only
        //            case TrinityObjectType.PowerGlobe:
        //            case TrinityObjectType.HealthGlobe:
        //            case TrinityObjectType.ProgressionGlobe:
        //                {
        //                    TargetRangeRequired = 2f;
        //                    TargetCurrentDistance = CurrentTarget.Distance;
        //                    break;
        //                }
        //            // * Shrine & Container - need to get within 8 feet and interact
        //            case TrinityObjectType.HealthWell:
        //                {
        //                    TargetRangeRequired = 4f;

        //                    float range;
        //                    if (DataDictionary.CustomObjectRadius.TryGetValue(CurrentTarget.ActorSnoId, out range))
        //                    {
        //                        TargetRangeRequired = range;
        //                    }
        //                    break;
        //                }
        //            case TrinityObjectType.Shrine:
        //            case TrinityObjectType.Container:
        //                {
        //                    TargetRangeRequired = 6f;

        //                    float range;
        //                    if (DataDictionary.CustomObjectRadius.TryGetValue(CurrentTarget.ActorSnoId, out range))
        //                    {
        //                        TargetRangeRequired = range;
        //                    }
        //                    break;
        //                }
        //            case TrinityObjectType.Interactable:
        //                {
        //                    if (CurrentTarget.IsQuestGiver)
        //                    {
        //                        CurrentDestination = MathEx.CalculatePointFrom(CurrentTarget.Position, Player.Position, CurrentTarget.Radius + 2f);
        //                        TargetRangeRequired = 5f;
        //                    }
        //                    else
        //                    {
        //                        TargetRangeRequired = 5f;
        //                    }
        //                    // Check if it's in our interactable range dictionary or not
        //                    float range;

        //                    if (DataDictionary.CustomObjectRadius.TryGetValue(CurrentTarget.ActorSnoId, out range))
        //                    {
        //                        TargetRangeRequired = range;
        //                    }
        //                    if (TargetRangeRequired <= 0)
        //                        TargetRangeRequired = CurrentTarget.Radius;

        //                    break;
        //                }
        //            // * Destructible - need to pick an ability and attack it
        //            case TrinityObjectType.Destructible:
        //                {
        //                    // Pick a range to try to reach + (tmp_fThisRadius * 0.70);
        //                    //TargetRangeRequired = CombatBase.CurrentPower.SNOPower == SNOPower.None ? 9f : CombatBase.CurrentPower.MinimumRange;
        //                    TargetRangeRequired = CombatBase.CurrentPower.MinimumRange;
        //                    CurrentTarget.Radius = 1f;
        //                    TargetCurrentDistance = CurrentTarget.Distance;
        //                    break;
        //                }
        //            case TrinityObjectType.Barricade:
        //                {
        //                    // Pick a range to try to reach + (tmp_fThisRadius * 0.70);
        //                    TargetRangeRequired = CombatBase.CurrentPower.MinimumRange;
        //                    CurrentTarget.Radius = 1f;
        //                    TargetCurrentDistance = CurrentTarget.Distance;
        //                    break;
        //                }
        //            // * Avoidance - need to pick an avoid location and move there
        //            case TrinityObjectType.Avoidance:
        //                {
        //                    TargetRangeRequired = 2f;
        //                    break;
        //                }
        //            case TrinityObjectType.Door:
        //                TargetRangeRequired = 2f;
        //                break;
        //            default:
        //                TargetRangeRequired = CurrentTarget.Radius;
        //                break;
        //        }
        //    }
        //}

        public void HandleUnitInRange()
        {
            
            using (new PerformanceLogger("HandleTarget.HandleUnitInRange"))
            {
                bool usePowerResult;

                if (CombatBase.CurrentPower == null)
                {
                    Logger.LogVerbose(LogCategory.Targetting, "Current Power is null, Target={0}", Trinity.TrinityPlugin.CurrentTarget.InternalName);
                }

                if (CombatBase.CurrentPower == null)
                {
                    Logger.LogVerbose(LogCategory.Targetting, "Current Power is SNOPower.None, Target={0}", Trinity.TrinityPlugin.CurrentTarget.InternalName);
                }

                if (Trinity.TrinityPlugin.CurrentTarget.HitPoints <= 0)
                {
                    Logger.LogVerbose(LogCategory.Targetting, "Target is Dead ({0})", Trinity.TrinityPlugin.CurrentTarget.InternalName);
                    return;
                }

                var questTarget = Trinity.TrinityPlugin.CurrentTarget;
                if (questTarget.IsQuestGiver)
                {                    
                    InteractionCooldowns.RemoveAll(t => t < DateTime.UtcNow);
                    var annId = questTarget.AnnId;

                    if (!InteractionCooldowns.ContainsKey(annId))
                    {
                        if (questTarget.RadiusDistance > 1)
                        {
                            HandleTargetBasicMovement(questTarget.Position, true);
                            return;
                        }
                        InteractionCooldowns.Add(annId, DateTime.UtcNow.AddSeconds(30));
                        InteractionWaitUntilTime = DateTime.UtcNow.AddMilliseconds(500);
                        Logger.LogVerbose(LogCategory.Targetting, $"Interacting with quest giver {questTarget}");
                        questTarget.Interact();
                        return;
                    }
                }

                float distance;
                Vector3 targetPosition = Vector3.Zero;
                int targetAcdId = -1;                

                if (CombatBase.CurrentPower.IsCastOnSelf)
                {
                    distance = 0;
                    targetAcdId = -1;
                    targetPosition = ZetaDia.Me.Position;
                }
                else
                {
                    var isTargetPosition = CombatBase.CurrentPower.TargetPosition != Vector3.Zero;
                    var isAcdId = CombatBase.CurrentPower.TargetAcdId != -1;
                    var isValidTargetData = isAcdId && isTargetPosition;

                    // TrinityPower gave us everything we need, lets rely on its accuracy
                    if (isValidTargetData)
                    {
                        Logger.LogVerbose(LogCategory.Behavior, "{0} Using power targetACD and targetPosition", CombatBase.CurrentPower.SNOPower);

                        targetPosition = CombatBase.CurrentPower.TargetPosition;
                        targetAcdId = CombatBase.CurrentPower.TargetAcdId;
                    }

                    // We were given only an ACDId, find the actors position.
                    if (!isTargetPosition && isAcdId)
                    {
                        var powerTarget = Trinity.TrinityPlugin.Targets.FirstOrDefault(o => o.AcdId == CombatBase.CurrentPower.TargetAcdId);
                        if (powerTarget != null)
                        {
                            Logger.LogVerbose(LogCategory.Behavior, "{0} Found target by ACDId", CombatBase.CurrentPower.SNOPower);

                            targetPosition = powerTarget.Position;
                            targetAcdId = powerTarget.AcdId;
                        }                        
                    }

                    distance = ZetaDia.Me.Position.Distance(targetPosition); 

                    // We don't have valid casting data, use current target
                    if ((!isTargetPosition && !isAcdId || targetPosition == Vector3.Zero || targetAcdId == -1 || distance > 150f) && !Trinity.TrinityPlugin.CurrentTarget.IsSafeSpot)
                    {
                        Logger.LogVerbose(LogCategory.Behavior,"{0} Using  CurrentTarget Position/ACD", CombatBase.CurrentPower.SNOPower);
                        targetPosition = Trinity.TrinityPlugin.CurrentTarget.Position;
                        targetAcdId = Trinity.TrinityPlugin.CurrentTarget.AcdId; 
                    }

                    if (isTargetPosition && !isAcdId)
                    {
                        Logger.LogVerbose(LogCategory.Behavior,"{0} Using target position only.", CombatBase.CurrentPower.SNOPower);
                        targetPosition = CombatBase.CurrentPower.TargetPosition;
                        targetAcdId = CombatBase.CurrentPower.TargetAcdId;
                    }
                }

                if (targetPosition == Vector3.Zero)
                {
                    Logger.LogVerbose(LogCategory.Targetting, "Can't cast on Vector3.Zero!");
                    return;
                }

                if (targetAcdId > 0)
                {
                    var targetAcd = ZetaDia.Actors.GetActorByACDId(targetAcdId);
                    if (!targetAcd.IsFullyValid())
                    {
                        Logger.LogVerbose("Invalid target Acd, probably dead");
                        return;
                    }
                    var unit = targetAcd as DiaUnit;
                    if (unit == null || unit.HitpointsCurrentPct <= 0 || !unit.IsFullyValid()) // || unit.HitpointsCurrentPct > 1)
                    {
                        Logger.LogVerbose("Invalid target hitpoints, probably dead");
                        return;
                    }
                }

                Core.Player.CurrentAction = PlayerAction.Attacking;


                var d = targetPosition.Distance(Core.Player.Position);
                if (d > 120)
                {
                    Logger.LogVerbose(LogCategory.Targetting, $"Target position is too far away! {d}");
                    return;
                }

                // See if we should force a long wait BEFORE casting
                Trinity.TrinityPlugin._isWaitingBeforePower = CombatBase.CurrentPower.ShouldWaitBeforeUse;
                if (Trinity.TrinityPlugin._isWaitingBeforePower)
                {
                    Logger.LogVerbose("Starting wait before use {0} ms", CombatBase.CurrentPower.WaitBeforeUseDelay);
                    return;
                }
                    

                // For "no-attack" logic
                if (CombatBase.CurrentPower.SNOPower == SNOPower.Walk && (CombatBase.CurrentPower.TargetPosition == Vector3.Zero || CombatBase.CurrentPower.TargetPosition.Distance2D(ZetaDia.Me.Position) < 3f))
                {
                    Logger.LogVerbose(LogCategory.Behavior, "Using no-attack logic");
                    //Navigator.PlayerMover.MoveStop();
                    usePowerResult = true;
                }
                else
                {
                    PowerManager.CanCastFlags flags;

                    if (PowerManager.CanCast(CombatBase.CurrentPower.SNOPower, out flags))
                    {                       
                        //Logger.Log(LogCategory.Targetting, "Casting {0} at {1} WorldId={2} ACDId={3} CastOnSelf={4} Flags={5} IsDeadZeta={6} DeadPlayers={7}",
                        //    CombatBase.CurrentPower.SNOPower, targetPosition, CombatBase.CurrentPower.TargetDynamicWorldId, 
                        //    targetAcdId, CombatBase.CurrentPower.IsCastOnSelf, flags, ZetaDia.Me.IsDead, ZetaDia.Actors.GetActorsOfType<DiaPlayer>().Any(x => x.IsDead));

                        if (Trinity.TrinityPlugin.powerBuff != null && Trinity.TrinityPlugin.powerBuff.ShouldWaitForAttackToFinish)
                            _isWaitingForAttackToFinish = true;

                        usePowerResult = ZetaDia.Me.UsePower(CombatBase.CurrentPower.SNOPower, targetPosition, CombatBase.CurrentPower.TargetDynamicWorldId, targetAcdId);

                        if (!usePowerResult)
                        {
                            usePowerResult = ZetaDia.Me.UsePower(CombatBase.CurrentPower.SNOPower, targetPosition, CombatBase.CurrentPower.TargetDynamicWorldId);
                        }
                    }
                    else
                    {
                        Logger.Log(LogCategory.Targetting, "Unable to Cast {0} at {1} WorldId={2} ACDId={3} CastOnSelf={4} Flags={5}",
                            CombatBase.CurrentPower.SNOPower, targetPosition, CombatBase.CurrentPower.TargetDynamicWorldId, targetAcdId, CombatBase.CurrentPower.IsCastOnSelf, flags);

                        usePowerResult = false;
                    }
                }

                var skill = SkillUtils.ById(CombatBase.CurrentPower.SNOPower);
                string target = GetTargetName();


                if (usePowerResult)
                {
                    // Monk Stuffs get special attention
                    //{
                    //    if (CombatBase.CurrentPower.SNOPower == SNOPower.Monk_TempestRush)
                    //        MonkCombat.LastTempestRushLocation = CombatBase.CurrentPower.TargetPosition;

                    //    MonkCombat.RunOngoingPowers();
                    //}

                    if (skill != null && skill.Meta != null)
                    {
                        Logger.LogVerbose("Used Power {0} ({1}) {2} Range={3} ({4} {5}) Delay={6}/{7} TargetDist={8} CurrentTarget={9} charges={10}",
                            skill.Name,
                            CombatBase.CurrentPower.SNOPower,
                            target,
                            skill.Meta.CastRange,
                            skill.Meta.DebugResourceEffect,
                            skill.Meta.DebugType,
                            skill.Meta.BeforeUseDelay,
                            skill.Meta.AfterUseDelay,
                            targetPosition.Distance(Core.Player.Position),
                            Trinity.TrinityPlugin.CurrentTarget != null ? Trinity.TrinityPlugin.CurrentTarget.InternalName : "Null",
                            skill.Charges
                            );

                    }
                    else
                    {
                        Logger.LogVerbose("Used Power {0} " + target, CombatBase.CurrentPower.SNOPower);
                    }

                    SpellTracker.TrackSpellOnUnit(CombatBase.CurrentPower.TargetAcdId, CombatBase.CurrentPower.SNOPower);
                    SpellHistory.RecordSpell(CombatBase.CurrentPower);

                    Trinity.TrinityPlugin.lastGlobalCooldownUse = DateTime.UtcNow;
                    Trinity.TrinityPlugin.LastPowerUsed = CombatBase.CurrentPower.SNOPower;

                    // See if we should force a long wait AFTERWARDS, too
                    // Force waiting AFTER power use for certain abilities
                    Trinity.TrinityPlugin._isWaitingAfterPower = CombatBase.CurrentPower.ShouldWaitAfterUse;

                    Trinity.TrinityPlugin._shouldPickNewAbilities = true;

                    if (CombatBase.CurrentPower.SNOPower == SNOPower.DemonHunter_Vault)
                    {
                        Logger.LogVerbose(LogCategory.Movement, "Cast Vault from HandleTarget.HandleUnitInRange()");
                        Navigator.Clear();
                        if (PlayerMover.NavigationProvider != null && PlayerMover.NavigationProvider.CurrentPath != null)
                        {
                            PlayerMover.NavigationProvider.CurrentPath.Clear();                            
                        }
                        PlayerMover.AbortCurrentNavigation = true;
                    }

                }
                else
                {
                    LastActionTimes.Add(DateTime.UtcNow);

                    if (skill != null && skill.Meta != null)
                    {


                        Logger.LogVerbose(LogCategory.Behavior, "Failed to use Power {0} ({1}) {2} Range={3} ({4} {5}) Delay={6}/{11} TargetDist={7} CurrentTarget={10} CurrentAnimation={12}",
                                           skill.Name,
                                           CombatBase.CurrentPower.SNOPower,
                                           target,
                                           skill.Meta.CastRange,
                                           skill.Meta.DebugResourceEffect,
                                           skill.Meta.DebugType,
                                           skill.Meta.BeforeUseDelay,
                                           distance,
                                           Core.Player.IsFacing(CombatBase.CurrentPower.TargetPosition),
                                           Trinity.TrinityPlugin.CurrentTarget != null && Core.Player.IsFacing(Trinity.TrinityPlugin.CurrentTarget.Position),
                                           Trinity.TrinityPlugin.CurrentTarget != null ? Trinity.TrinityPlugin.CurrentTarget.InternalName : "Null",
                                           skill.Meta.AfterUseDelay, 
                                           ZetaDia.Me.CommonData.CurrentAnimation
                                           );
                    }
                    else
                    {
                        Logger.LogVerbose(LogCategory.Behavior, "Failed to use power {0} (CurrentAnimation={1})" + target, CombatBase.CurrentPower.SNOPower, ZetaDia.Me.CommonData.CurrentAnimation);
                    }

                    CombatBase.CurrentPower.CastAttempts++;
                    Trinity.TrinityPlugin._shouldPickNewAbilities = CombatBase.CurrentPower.CastAttempts >= CombatBase.CurrentPower.MaxFailedCastReTryAttempts;
                }

                

                // Keep looking for monsters at "normal kill range" a few moments after we successfully attack a monster incase we can pull them into range
                Trinity.TrinityPlugin._keepKillRadiusExtendedForSeconds = 8;
                Trinity.TrinityPlugin._timeKeepKillRadiusExtendedUntil = DateTime.UtcNow.AddSeconds(Trinity.TrinityPlugin._keepKillRadiusExtendedForSeconds);
                Trinity.TrinityPlugin._keepLootRadiusExtendedForSeconds = 8;
                // if at full or nearly full health, see if we can raycast to it, if not, ignore it for 2000 ms
                if (Trinity.TrinityPlugin.CurrentTarget.HitPointsPct >= 0.9d &&
                    !NavHelper.CanRayCast(Core.Player.Position, Trinity.TrinityPlugin.CurrentTarget.Position) &&
                    !Trinity.TrinityPlugin.CurrentTarget.IsBoss &&
                    !(DataDictionary.StraightLinePathingLevelAreaIds.Contains(Core.Player.LevelAreaId) || DataDictionary.LineOfSightWhitelist.Contains(Trinity.TrinityPlugin.CurrentTarget.ActorSnoId)))
                {
                    Trinity.TrinityPlugin._ignoreRactorGuid = Trinity.TrinityPlugin.CurrentTarget.RActorId;
                    Trinity.TrinityPlugin._ignoreTargetForLoops = 6;
                    // Add this monster to our very short-term ignore list
                    Trinity.TrinityPlugin.Blacklist3Seconds.Add(Trinity.TrinityPlugin.CurrentTarget.AnnId);
                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.Behavior, "Blacklisting {0} {1} {2} for 3 seconds due to Raycast failure", Trinity.TrinityPlugin.CurrentTarget.Type, Trinity.TrinityPlugin.CurrentTarget.InternalName, Trinity.TrinityPlugin.CurrentTarget.ActorSnoId);
                    Trinity.TrinityPlugin.Blacklist3LastClear = DateTime.UtcNow;
                    Trinity.TrinityPlugin.NeedToClearBlacklist3 = true;
                }

            }
        }

        public Dictionary<int,DateTime> InteractionCooldowns { get; set; } = new Dictionary<int, DateTime>();
        public DateTime InteractionWaitUntilTime { get; set; } = DateTime.MinValue;



        public string GetTargetName()
        {
            float dist = 0;
            if (CombatBase.CurrentPower.TargetPosition != Vector3.Zero)
                dist = CombatBase.CurrentPower.TargetPosition.Distance(Core.Player.Position);
            else if (Trinity.TrinityPlugin.CurrentTarget != null)
                dist = Trinity.TrinityPlugin.CurrentTarget.Position.Distance(Core.Player.Position);

            var name = Trinity.TrinityPlugin.CurrentTarget != null && Trinity.TrinityPlugin.CurrentTarget.AcdId == CombatBase.CurrentPower.TargetAcdId ? Trinity.TrinityPlugin.CurrentTarget.InternalName : string.Empty;

            string target = CombatBase.CurrentPower.TargetPosition != Vector3.Zero ? "at " + NavHelper.PrettyPrintVector3(CombatBase.CurrentPower.TargetPosition) + " dist=" + (int)dist : "";
            target += CombatBase.CurrentPower.TargetAcdId != -1 ? " on " + name + " (" + CombatBase.CurrentPower.TargetAcdId : ")";
            return target;
        }

        public void HandleItemInRange()
        {
            var item = Trinity.TrinityPlugin.CurrentTarget as TrinityItem;
            if (item == null)
                return;

            using (new PerformanceLogger("HandleTarget.HandleItemInRange"))
            {
                int iInteractAttempts;
                // Pick the item up the usepower way, and "blacklist" for a couple of loops
                if (ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, item.AcdId))
                {
                    SpellHistory.RecordSpell(SNOPower.Axe_Operate_Gizmo);
                }
                else
                {
                    LastActionTimes.Add(DateTime.UtcNow);
                }
                Trinity.TrinityPlugin._ignoreRactorGuid = item.RActorId;
                Trinity.TrinityPlugin._ignoreTargetForLoops = 3;
                // Store item pickup stats

                Core.Player.CurrentAction = PlayerAction.Interacting;

                string itemSha1Hash = HashGenerator.GenerateItemHash(item.Position, item.ActorSnoId, item.InternalName, Trinity.TrinityPlugin.CurrentWorldDynamicId, item.ItemQualityLevel, item.ItemLevel);
                if (!ItemDropStats._hashsetItemPicksLookedAt.Contains(itemSha1Hash))
                {
                    ItemDropStats._hashsetItemPicksLookedAt.Add(itemSha1Hash);
                    TrinityItemType itemType = TrinityItemManager.DetermineItemType(item.InternalName, item.ItemType, item.FollowerType);
                    TrinityItemBaseType itemBaseType = TrinityItemManager.DetermineBaseType(itemType);
                    if (itemBaseType == TrinityItemBaseType.Armor || itemBaseType == TrinityItemBaseType.WeaponOneHand || itemBaseType == TrinityItemBaseType.WeaponTwoHand ||
                        itemBaseType == TrinityItemBaseType.WeaponRange || itemBaseType == TrinityItemBaseType.Jewelry || itemBaseType == TrinityItemBaseType.FollowerItem ||
                        itemBaseType == TrinityItemBaseType.Offhand)
                    {
                        int iQuality;
                        ItemDropStats.ItemsPickedStats.Total++;
                        if (item.ItemQualityLevel >= ItemQuality.Legendary)
                            iQuality = ItemDropStats.QUALITYORANGE;
                        else if (item.ItemQualityLevel >= ItemQuality.Rare4)
                            iQuality = ItemDropStats.QUALITYYELLOW;
                        else if (item.ItemQualityLevel >= ItemQuality.Magic1)
                            iQuality = ItemDropStats.QUALITYBLUE;
                        else
                            iQuality = ItemDropStats.QUALITYWHITE;
                        //asserts	
                        if (iQuality > ItemDropStats.QUALITYORANGE)
                        {
                            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "ERROR: Item type (" + iQuality + ") out of range");
                        }
                        if ((item.ItemLevel < 0) || (item.ItemLevel >= 74))
                        {
                            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "ERROR: Item level (" + item.ItemLevel + ") out of range");
                        }
                        ItemDropStats.ItemsPickedStats.TotalPerQuality[iQuality]++;
                        ItemDropStats.ItemsPickedStats.TotalPerLevel[item.ItemLevel]++;
                        ItemDropStats.ItemsPickedStats.TotalPerQPerL[iQuality, item.ItemLevel]++;
                    }
                    else if (itemBaseType == TrinityItemBaseType.Gem)
                    {
                        int iGemType = 0;
                        ItemDropStats.ItemsPickedStats.TotalGems++;
                        if (itemType == TrinityItemType.Topaz)
                            iGemType = ItemDropStats.GEMTOPAZ;
                        if (itemType == TrinityItemType.Ruby)
                            iGemType = ItemDropStats.GEMRUBY;
                        if (itemType == TrinityItemType.Emerald)
                            iGemType = ItemDropStats.GEMEMERALD;
                        if (itemType == TrinityItemType.Amethyst)
                            iGemType = ItemDropStats.GEMAMETHYST;
                        if (itemType == TrinityItemType.Diamond)
                            iGemType = ItemDropStats.GEMDIAMOND;

                        ItemDropStats.ItemsPickedStats.GemsPerType[iGemType]++;
                        ItemDropStats.ItemsPickedStats.GemsPerLevel[item.ItemLevel]++;
                        ItemDropStats.ItemsPickedStats.GemsPerTPerL[iGemType, item.ItemLevel]++;
                    }
                    else if (itemType == TrinityItemType.HealthPotion)
                    {
                        ItemDropStats.ItemsPickedStats.TotalPotions++;
                        if ((item.ItemLevel < 0) || (item.ItemLevel > 63))
                        {
                            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "ERROR: Potion level ({0}) out of range", item.ItemLevel);
                        }
                        ItemDropStats.ItemsPickedStats.PotionsPerLevel[item.ItemLevel]++;
                    }
                    else if (itemType == TrinityItemType.InfernalKey)
                    {
                        ItemDropStats.ItemsPickedStats.TotalInfernalKeys++;
                    }
                }

                // Count how many times we've tried interacting
                if (!Core.Targets.InteractAttempts.TryGetValue(item.RActorId, out iInteractAttempts))
                {
                    Core.Targets.InteractAttempts.Add(item.RActorId, 1);

                    // Fire item looted for Demonbuddy Item stats
                    GameEvents.FireItemLooted(item.AcdId);
                }
                else
                {
                    Core.Targets.InteractAttempts[item.RActorId]++;
                }
                // If we've tried interacting too many times, blacklist this for a while
                if (iInteractAttempts > 20 && item.ItemQualityLevel < ItemQuality.Legendary)
                {
                    Trinity.TrinityPlugin.Blacklist90Seconds.Add(item.AnnId);
                }
                // Now tell TrinityPlugin to get a new target!
                Trinity.TrinityPlugin._forceTargetUpdate = true;

            }
        }

        public bool ShouldWaitForLootDrop
        {
            get
            {
                if (Core.Player.ParticipatingInTieredLootRun)
                {
                    return Trinity.TrinityPlugin.CurrentTarget == null &&
                           (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.lastHadEliteUnitInSights).TotalMilliseconds <= Core.Settings.Combat.Misc.DelayAfterKill ||
                            DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.lastHadBossUnitInSights).TotalMilliseconds <= 3000);
                }

                return Trinity.TrinityPlugin.CurrentTarget == null &&
                           (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.lastHadUnitInSights).TotalMilliseconds <= Core.Settings.Combat.Misc.DelayAfterKill ||
                            DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.lastHadEliteUnitInSights).TotalMilliseconds <= Core.Settings.Combat.Misc.DelayAfterKill ||
                            DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.lastHadBossUnitInSights).TotalMilliseconds <= 3000 ||
                            DateTime.UtcNow.Subtract(Composites.LastFoundHoradricCache).TotalMilliseconds <= 5000) ||
                           DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.lastHadContainerInSights).TotalMilliseconds <= Core.Settings.WorldObject.OpenContainerDelay;
            }
        }

        public bool _isWaitingForAttackToFinish { get; set; }
    }

}
