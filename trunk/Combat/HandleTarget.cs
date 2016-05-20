using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Buddy.Coroutines;
using Trinity.Combat;
using Trinity.Combat.Abilities;
using Trinity.Config.Combat;
using Trinity.Coroutines;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Movement;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Logger = Trinity.Technicals.Logger;
using BotManager = Trinity.BotManager;

namespace Trinity
{
    public partial class TrinityPlugin
    {

        /// <summary>
        /// Returns the current DiaPlayer
        /// </summary>
        public static DiaActivePlayer Me
        {
            get { return ZetaDia.Me; }
        }

        /// <summary>
        /// Returns a RunStatus, if appropriate. Throws an exception if not.
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static RunStatus GetRunStatus(RunStatus status, string location)
        {
            MonkCombat.RunOngoingPowers();
            string extras = "";

            if (CombatBase.CombatMovement.IsQueuedMovement)
            {
                extras = "Aborting to run queued movement";
                Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "HandleTarget returning {0} to tree from " + location + " " + extras, status);
                return RunStatus.Success;
            }
            
            if (_isWaitingForPower)
                extras += " IsWaitingForPower";
            if (_isWaitingAfterPower)
                extras += " IsWaitingAfterPower";
            if (_isWaitingForPotion)
                extras += " IsWaitingForPotion";
            if (TrinityTownRun.IsTryingToTownPortal())
                extras += " IsTryingToTownPortal";
            //if (TownRun.TownRunTimerRunning())
            //    extras += " TownRunTimerRunning";
            //if (TownRun.TownRunTimerFinished())
            //    extras += " TownRunTimerFinished";
            if (_forceTargetUpdate)
                extras += " ForceTargetUpdate";
            if (CurrentTarget == null)
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

        private static int _waitedTicks = 0;

        /// <summary>
        /// Handles all aspects of moving to and attacking the current target
        /// </summary>
        /// <returns></returns>
        internal static RunStatus HandleTarget()
        {
            using (new PerformanceLogger("HandleTarget"))
            {
                try
                {
                    if (!CombatTargeting.Instance.AllowedToKillMonsters && (CurrentTarget == null || CurrentTarget.IsUnit))
                    {
                        Logger.LogVerbose("Aborting HandleTarget() AllowCombat={0} ShouldAvoid={1}", CombatTargeting.Instance.AllowedToKillMonsters, Core.Avoidance.Avoider.ShouldAvoid);
                        return RunStatus.Failure;
                    }

                    if (!Player.IsValid)
                    {
                        Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "No longer in game world", true);
                        return GetRunStatus(RunStatus.Failure, "PlayerInvalid");
                    }

                    if (Player.IsDead)
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
                        if (Core.Avoidance.Avoider.TryGetSafeSpot(out safespot))
                        {
                            Logger.Log(LogCategory.Avoidance, $"Safespot found: {safespot}");

                            // Don't avoid when we need to goblin kamakazi or interact with a door etc
                            var highPriorityObject = ObjectCache.OrderBy(o => o.Weight).FirstOrDefault();
                            if ((highPriorityObject == null || highPriorityObject.Type != TrinityObjectType.Barricade && highPriorityObject.Type != TrinityObjectType.Door) && !CombatBase.IsDoingGoblinKamakazi)
                            {                                
                                if (Core.Avoidance.Grid.IsStandingInFlags(AvoidanceFlags.CriticalAvoidance) || Player.CurrentHealthPct < 0.9 || highPriorityObject == null || !highPriorityObject.IsUnit || CombatBase.CurrentPower.MinimumRange > highPriorityObject.Distance)
                                {                                    
                                    var distance = safespot.Distance(Player.Position);
                                    Logger.Log(LogCategory.Avoidance, $"Targetted SafeSpot Distance={distance}");
                                    CurrentTarget = new TrinityCacheObject()
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
                    }
                    else if (CurrentTarget != null)
                    {
                        if (CurrentTarget.IsSafeSpot)
                        {
                            TargetUtil.ClearCurrentTarget("Avoidance finished moving to safe spot.");
                            return GetRunStatus(RunStatus.Failure, "Finished Avoiding");
                        }

                        if (ObjectCache.All(a => a.AnnId != CurrentTarget.AnnId))
                        {
                            TargetUtil.ClearCurrentTarget("Target no longer exists.");
                            return GetRunStatus(RunStatus.Failure, "Target Missing");
                        }
                    }

                    if (Player.IsCasting && CurrentTarget != null && CurrentTarget.GizmoType == GizmoType.Headstone)
                    {
                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Player is casting revive ({0})", Player.CurrentAnimation);
                        return GetRunStatus(RunStatus.Success, "RevivingPlayer");
                    }

                    if (!Core.Avoidance.Avoider.IsAvoiding)
                    {
                        RunStatus runStatus;
                        if (ThrottleActionPerSecond(out runStatus)) //Settings.Advanced.ThrottleAPS && 
                            return runStatus;

                        VacuumItems.Execute();

                        // Make sure we reset unstucker stuff here
                        PlayerMover.TimesReachedStuckPoint = 0;
                        PlayerMover.vSafeMovementLocation = Vector3.Zero;
                        PlayerMover.TimeLastRecordedPosition = DateTime.UtcNow;

                        // Time based wait delay for certain powers with animations
                        if (CombatBase.CurrentPower != null)
                        {
                            if (CombatBase.CurrentPower.ShouldWaitAfterUse && _isWaitingAfterPower || _isWaitingBeforePower && CombatBase.CurrentPower.ShouldWaitBeforeUse)
                            {
                                var type = _isWaitingAfterPower ? "IsWaitingAfterPower" : "IsWaitingBeforePower";
                                _waitedTicks++;
                                Logger.LogVerbose($"Waiting... {type} Power={CombatBase.CurrentPower.SNOPower} TicksWaited={_waitedTicks}");
                                return GetRunStatus(RunStatus.Running, type);
                            }
                        }
                    }

                    if (ShouldWaitForLootDrop)
                    {
                        Logger.LogVerbose("Wait for loot drop");
                    }

                    if (_isWaitingForAttackToFinish)
                    {
                        Logger.LogVerbose("Wait for Attack to finish");
                    }

                    if (_isWaitingBeforePower)
                    {
                        Logger.LogVerbose("Wait Before Power Finished");
                    }

                    if (_isWaitingAfterPower)
                    {
                        Logger.LogVerbose("Wait After Power Finished");
                    }

                    if (_isWaitingForPotion)
                    {
                        Logger.LogVerbose("Wait for Potion");
                    }

                    if (CurrentTarget == null)
                    {
                        Logger.LogVerbose("CurrentTarget == null");
                    }

                    //if (ClearArea.ShouldMoveToPortalPosition)
                    //    return RunStatus.Success;

                    _waitedTicks = 0;
                    _isWaitingAfterPower = false;
                    _isWaitingBeforePower = false;

                    if (!Core.Avoidance.Avoider.IsAvoiding)
                    {

                        if (!_isWaitingForPower && !_isWaitingBeforePower && (CombatBase.CurrentPower == null || CombatBase.CurrentPower.SNOPower == SNOPower.None) && CurrentTarget != null)
                        {
                            CombatBase.CurrentPower = AbilitySelector();

                            if (CombatBase.CurrentPower.SNOPower == SNOPower.None)
                            {
                                Logger.LogVerbose(LogCategory.Behavior, "SNOPower.None selected from combat routine :S");
                                _shouldPickNewAbilities = true;
                            }

                        }
                        else
                        {
                            Logger.LogVerbose(LogCategory.Behavior, "Not Selecting Ability WaitingForPower={0} WaitingBeforePower={1} CurrentPower={2} CurrentTarget={3}",
                                _isWaitingForPower, _isWaitingBeforePower, CombatBase.CurrentPower, CurrentTarget);
                        }
                    }

                    // Prevent running away after progression globes spawn if they're in aoe
                    if (Player.IsInRift && !Core.Avoidance.Avoider.IsAvoiding)
                    {                       
                        var globes = ObjectCache.Where(o => o.Type == TrinityObjectType.ProgressionGlobe && o.Distance < AvoidanceManager.MaxDistance).ToList();
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
                    if (Player.IsRooted && !wasRootedLastTick)
                    {
                        wasRootedLastTick = true;
                        _forceTargetUpdate = true;
                    }
                    if (!Player.IsRooted)
                        wasRootedLastTick = false;

                    if (CurrentTarget == null)
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "CurrentTarget was passed as null! Continuing...");
                    }

                    MonkCombat.RunOngoingPowers();

                    // Refresh the object Cache every time
                    //RefreshDiaObjectCache();

                    if (!CombatBase.IsCombatAllowed && (CurrentTarget == null || CurrentTarget.IsUnit))
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "Ending: Combat is Disabled!");
                        return RunStatus.Failure;
                    }

                    if (CurrentTarget == null && TrinityTownRun.IsWantingTownRun)
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


                    if (CurrentTarget == null)
                    {
                        Logger.Log(TrinityLogLevel.Info, LogCategory.Behavior, "CurrentTarget set as null in refresh! Error 2, Returning Failure");
                        return GetRunStatus(RunStatus.Failure, "CurrentTargetNull2");
                    }

                    // Handle Target stuck / timeout
                    var targetName = CurrentTarget.InternalName;
                    if (HandleTargetTimeoutTask())
                    {
                        Logger.LogVerbose(LogCategory.Behavior, "Blacklisted Target: {0}, Returning Failure", targetName);
                        return GetRunStatus(RunStatus.Running, "BlackListTarget");
                    }

                    if (CurrentTarget != null)
                        AssignPower();

                    // Pop a potion when necessary



                    using (new PerformanceLogger("HandleTarget.CheckAvoidanceBuffs"))
                    {
                        // See if we can use any special buffs etc. while in avoidance
                        if ((CurrentTarget.Type == TrinityObjectType.Avoidance || CurrentTarget.IsSafeSpot) && !CurrentTarget.IsWaitSpot )
                        {
                            powerBuff = AbilitySelector(true);
                            if (powerBuff != null && powerBuff.SNOPower != SNOPower.None)
                            {
                                Logger.LogVerbose(LogCategory.Behavior, "HandleTarget: Casting {0} for Avoidance", powerBuff.SNOPower);

                                if (ZetaDia.Me.UsePower(powerBuff.SNOPower, powerBuff.TargetPosition, powerBuff.TargetDynamicWorldId, powerBuff.TargetACDGUID))
                                {
                                    LastPowerUsed = powerBuff.SNOPower;
                                    CacheData.AbilityLastUsed[powerBuff.SNOPower] = DateTime.UtcNow;
                                    SpellHistory.RecordSpell(powerBuff.SNOPower);
                                    return GetRunStatus(RunStatus.Running, "Cast Avoidance Spell");
                                }
                                else
                                {
                                    TrinityPlugin.LastActionTimes.Add(DateTime.UtcNow);
                                }

                                //return GetRunStatus(RunStatus.Running, "UsePowerBuff");
                            }
                        }
                    }

                    // Pick the destination point and range of target
                    /*
                     * Set the range required for attacking/interacting/using
                     */

                    SetRangeRequiredForTarget();

                    using (new PerformanceLogger("HandleTarget.SpecialNavigation"))
                    {
                        PositionCache.AddPosition();

                        // Maintain an area list of all zones we pass through/near while moving, for our custom navigation handler
                        if (DateTime.UtcNow.Subtract(LastAddedLocationCache).TotalMilliseconds >= 100)
                        {
                            LastAddedLocationCache = DateTime.UtcNow;
                            if (Vector3.Distance(Player.Position, LastRecordedPosition) >= 5f)
                            {
                                SkipAheadAreaCache.Add(new CacheObstacleObject(Player.Position, 20f, 0));
                                LastRecordedPosition = Player.Position;

                            }
                        }
                    }


                    using (new PerformanceLogger("HandleTarget.LoSCheck"))
                    {                        
                        TargetCurrentDistance = CurrentTarget.RadiusDistance;
                        if (DataDictionary.AlwaysRaycastWorlds.Contains(Player.WorldID) && CurrentTarget.Distance > CurrentTarget.Radius + 2f)
                        {
                            CurrentTargetIsInLoS = NavHelper.CanRayCast(Player.Position, CurrentDestination);
                        }
                        else if (TargetCurrentDistance <= 2f)
                        {
                            CurrentTargetIsInLoS = true;
                        }
                        else if (Settings.Combat.Misc.UseNavMeshTargeting && CurrentTarget.Type != TrinityObjectType.Barricade && CurrentTarget.Type != TrinityObjectType.Destructible)
                        {
                            CurrentTargetIsInLoS = (NavHelper.CanRayCast(Player.Position, CurrentDestination) || DataDictionary.LineOfSightWhitelist.Contains(CurrentTarget.ActorSNO));
                        }
                        else
                        {
                            CurrentTargetIsInLoS = true;
                        }
                    }

                    using (new PerformanceLogger("HandleTarget.InRange"))
                    {
                        bool stuckOnTarget =
                            ((CurrentTarget.Type == TrinityObjectType.Barricade ||
                            CurrentTarget.Type == TrinityObjectType.Item ||
                             CurrentTarget.Type == TrinityObjectType.Interactable ||
                             CurrentTarget.Type == TrinityObjectType.CursedChest ||
                             CurrentTarget.Type == TrinityObjectType.CursedShrine ||
                             CurrentTarget.Type == TrinityObjectType.Destructible) &&
                             !ZetaDia.Me.Movement.IsMoving && DateTime.UtcNow.Subtract(PlayerMover.TimeLastUsedPlayerMover).TotalMilliseconds < 250);

                        bool npcInRange = CurrentTarget.IsQuestGiver && CurrentTarget.RadiusDistance <= 3f;

                        bool noRangeRequired = TargetRangeRequired <= 1f;
                        switch (CurrentTarget.Type)
                        {
                            // These always have TargetRangeRequired=1f, but, we need to run directly to their center until we stop moving, then destroy them
                            case TrinityObjectType.Door:
                            case TrinityObjectType.Barricade:
                            case TrinityObjectType.Destructible:
                                noRangeRequired = false;
                                break;
                        }

                        if (CurrentTarget.IsBoss && Player.IsInRift && CurrentTarget.IsSpawning && !TargetUtil.AnyTrashInRange(20f) &&
                            !Gems.Taeguk.IsEquipped && Skills.Barbarian.FuriousCharge.IsActive)
                        {
                            Logger.LogVerbose(LogCategory.Avoidance, "Waiting for Rift Boss to Spawn");
                            return RunStatus.Running;
                        }

                        // Interact/use power on target if already in range
                        if (!CurrentTarget.IsSafeSpot && (noRangeRequired || (TargetCurrentDistance <= TargetRangeRequired && CurrentTargetIsInLoS) || stuckOnTarget || npcInRange))
                        {
                            Logger.LogDebug(LogCategory.Behavior, "Object in Range: noRangeRequired={0} Target In Range={1} stuckOnTarget={2} npcInRange={3} power={4} target={5}", noRangeRequired, (TargetCurrentDistance <= TargetRangeRequired && CurrentTargetIsInLoS), stuckOnTarget, npcInRange, CombatBase.CurrentPower.SNOPower, CurrentTarget);

                            UpdateStatusTextTarget(true);

                            HandleObjectInRange();
                            return GetRunStatus(RunStatus.Running, "HandleObjectInRange");
                        }
                    }


                    using (new PerformanceLogger("HandleTarget.UpdateStatusText"))
                    {
                        // Out-of-range, so move towards the target
                        UpdateStatusTextTarget(false);
                    }

                    // Are we currently incapacitated? If so then wait...
                    if (Player.IsIncapacitated || Player.IsRooted)
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
                    LastDistanceFromTarget = TargetCurrentDistance;

                    if (TimeSinceUse(SNOPower.Monk_TempestRush) < 250)
                    {
                        ForceNewMovement = true;
                    }

                    // Only position-shift when not avoiding
                    // See if we want to ACTUALLY move, or are just waiting for the last move command...
                    if (!ForceNewMovement && IsAlreadyMoving && CurrentDestination == LastMoveToTarget && DateTime.UtcNow.Subtract(lastMovementCommand).TotalMilliseconds <= 100)
                    {
                        // return GetTaskResult(true);
                    }

                    if (!Player.IsInTown)
                    {
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

                HandleTargetBasicMovement(ForceNewMovement);

                Logger.LogDebug(LogCategory.Behavior, "End of HandleTarget");
                return GetRunStatus(RunStatus.Running, "End");
            }
        }

        public static List<DateTime> LastActionTimes
        {
            get { return _lastActionTimes ?? (_lastActionTimes = new List<DateTime>()); }
            set { _lastActionTimes = value; }
        }

        /// <summary>
        /// Doing too many things in too short of at time will disconnect you from the Diablo3 client
        /// This includes dropping items, casting spells, probably anything injection based.
        /// This checks a collection of recorded actions and waits if too much has happened too fast.
        /// </summary>
        private static bool ThrottleActionPerSecond(out RunStatus runStatus)
        {            
            const int measureTimeMs = 500;
            const int actionLimit = 4;
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
            //    var target = ZetaDia.Actors.GetActorByACDId(CurrentTarget.ACDGuid) as DiaUnit;

            //    Logger.Log($"CurrentTarget={CurrentTarget?.InternalName} SNO={CurrentTarget?.ActorSNO} Distance={CurrentTarget?.Distance} " +
            //               $"CurrentPower={CombatBase.CurrentPower} TargetAcd={CurrentTarget?.ACDGuid} TargetValid={target.IsFullyValid()} " +
            //               $"TargetAlive={target?.IsAlive} TargetHealth={CurrentTarget.HitPoints} " +
            //               $"PowerTargetDistance={CombatBase.CurrentPower.TargetPosition.Distance(ZetaDia.Me.Position)} " +
            //               $"TargetDistance={target?.Distance} TimeSinceLastUse={SpellHistory.TimeSinceUse(CombatBase.CurrentPower.SNOPower)}");
            //}

            // Wait until NTh action happend more than than half the measure time ago          
            var timeSince = DateTime.UtcNow.Subtract(actionLimitTime).TotalMilliseconds;
            if (timeSince < measureTimeMs / 2) 
            {
                Logger.Log(LogCategory.Behavior, "Throttling - Actions Per Second Limit Reached! {0} actions were taken within {1}ms", actionLimit, timeSince);               
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
        private static bool TrySpecialMovement(out RunStatus statusResult)
        {
            if (!CombatBase.IsInCombat && !Settings.Combat.Misc.AllowOOCMovement)
            {
                statusResult = default(RunStatus);
                return false;
            }

            using (new PerformanceLogger("HandleTarget.TrySpecialMovement"))
            { 
                if (ClassMover.SpecialMovement(CurrentDestination))
                {
                    // Try to ensure the bot isn't navigating to somewhere behind us.
                    Navigator.Clear();
                    LastMoveToTarget = CurrentDestination;
                    statusResult = GetRunStatus(RunStatus.Running, "SpecialMovement");
                    return true;
                }
            }
            statusResult = RunStatus.Failure;
            return false;
        }


        private static void HandleObjectInRange()
        {
            Logger.LogVerbose(LogCategory.Behavior, "CurrentTarget is {0}", CurrentTarget);

            switch (CurrentTarget.Type)
            {
                case TrinityObjectType.Avoidance:
                    _forceTargetUpdate = true;
                    break;
                case TrinityObjectType.Player:
                    break;

                // Unit, use our primary power to attack
                case TrinityObjectType.Unit:
                    {
                        if (CombatBase.CurrentPower.SNOPower != SNOPower.None)
                        {
                            if (_isWaitingForPower && CombatBase.CurrentPower.ShouldWaitBeforeUse)
                            {
                            }
                            else if (_isWaitingForPower && !CombatBase.CurrentPower.ShouldWaitBeforeUse)
                            {
                                _isWaitingForPower = false;
                            }
                            else
                            {
                                _isWaitingForPower = false;
                                HandleUnitInRange();
                            }
                        }
                        break;
                    }
                // Item, interact with it and log item stats
                case TrinityObjectType.Item:
                    {
                        // Check if we actually have room for this item first

                        bool isTwoSlot = true;
                        if (CurrentTarget.Item != null && CurrentTarget.Item.CommonData != null)
                        {
                            isTwoSlot = CurrentTarget.Item.CommonData.IsTwoSquareItem;
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
                case TrinityObjectType.Gold:
                case TrinityObjectType.HealthGlobe:
                case TrinityObjectType.PowerGlobe:
                case TrinityObjectType.ProgressionGlobe:
                    {
                        int interactAttempts;
                        // Count how many times we've tried interacting
                        if (!CacheData.InteractAttempts.TryGetValue(CurrentTarget.RActorGuid, out interactAttempts))
                        {
                            CacheData.InteractAttempts.Add(CurrentTarget.RActorGuid, 1);
                        }
                        else
                        {
                            CacheData.InteractAttempts[CurrentTarget.RActorGuid]++;
                        }
                        // If we've tried interacting too many times, blacklist this for a while
                        if (interactAttempts > 3)
                        {
                            Blacklist3Seconds.Add(CurrentTarget.AnnId);
                        }
                        _ignoreRactorGuid = CurrentTarget.RActorGuid;
                        _ignoreTargetForLoops = 3;

                        // Now tell TrinityPlugin to get a new target!
                        _forceTargetUpdate = true;
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
                        _forceTargetUpdate = true;

                        if (ZetaDia.Me.Movement.SpeedXY > 0.5 && CurrentTarget.Distance < 8f)
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
                            CacheData.InteractAttempts.TryGetValue(CurrentTarget.RActorGuid, out attemptCount);

                            Logger.LogDebug(LogCategory.UserInformation, "Interacting with {1} Distance {2:0} Radius {3:0.0} Attempt {4}",
                                     SNOPower.Axe_Operate_Gizmo, CurrentTarget.InternalName, CurrentTarget.Distance, CurrentTarget.Radius, attemptCount);

                            if (CurrentTarget.ActorType == ActorType.Monster)
                            {
                                if (ZetaDia.Me.UsePower(SNOPower.Axe_Operate_NPC, Vector3.Zero, CurrentWorldDynamicId, CurrentTarget.ACDGuid))
                                {
                                    SpellHistory.RecordSpell(new TrinityPower()
                                    {
                                        SNOPower = SNOPower.Axe_Operate_Gizmo,
                                        TargetACDGUID = CurrentTarget.ACDGuid,
                                        MinimumRange = TargetRangeRequired,
                                        TargetPosition = CurrentTarget.Position,
                                    });
                                }
                                else
                                {
                                    TrinityPlugin.LastActionTimes.Add(DateTime.UtcNow);
                                }
                            }
                            else
                            {
                                //Navigator.PlayerMover.MoveTowards(CurrentCacheObject.Position);
                                //CurrentTarget.Object.Interact();

             
                                Logger.LogNormal("Interacting with {0}", CurrentTarget.InternalName);
                                //if (ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, CurrentTarget.ACDId))
                                //ZetaDia.Me.UsePower(SNOPower.Interact_Crouching, Vector3.Zero, 0, CurrentTarget.ACDId)
                                var hasBeenOperated = c_diaObject is DiaGizmo && (c_diaObject as DiaGizmo).HasBeenOperated;
                                if (!hasBeenOperated && ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, CurrentTarget.ACDGuid))
                                {
                                    SpellHistory.RecordSpell(new TrinityPower()
                                    {
                                        SNOPower = SNOPower.Axe_Operate_Gizmo,
                                        TargetACDGUID = CurrentTarget.ACDGuid,
                                        MinimumRange = TargetRangeRequired,
                                        TargetPosition = CurrentTarget.Position,
                                    });
                                }
                                else
                                {
                                    CurrentTarget.Object.Interact();

                                    //{
                                    //    Logger.LogNormal("Fallback Interact on {0}", CurrentCacheObject.InternalName);
                                    //    SpellHistory.RecordSpell(new TrinityPower()
                                    //    {
                                    //        SNOPower = SNOPower.Axe_Operate_Gizmo,
                                    //        TargetACDGUID = CurrentTarget.ACDId,
                                    //        MinimumRange = TargetRangeRequired,
                                    //        TargetPosition = CurrentTarget.Position,
                                    //    });
                                    //}
                                    //else
                                    //{
                                    //    TrinityPlugin.LastActionTimes.Add(DateTime.UtcNow);
                                    //}

                                }
                            }

                            // Count how many times we've tried interacting
                            if (!CacheData.InteractAttempts.TryGetValue(CurrentTarget.RActorGuid, out attemptCount))
                            {
                                CacheData.InteractAttempts.Add(CurrentTarget.RActorGuid, 1);
                            }
                            else
                            {
                                CacheData.InteractAttempts[CurrentTarget.RActorGuid] += 1;
                            }

                            var attempts = CurrentTarget.Type == TrinityObjectType.Shrine ? 5 : 15;

                            // If we've tried interacting too many times, blacklist this for a while
                            if (CacheData.InteractAttempts[CurrentTarget.RActorGuid] > attempts && CurrentTarget.Type != TrinityObjectType.HealthWell)
                            {
                                Logger.LogVerbose("Blacklisting {0} ({1}) for 60 seconds after {2} interactions",
                                    CurrentTarget.InternalName, CurrentTarget.ActorSNO, attemptCount);

                                CacheData.InteractAttempts[CurrentTarget.RActorGuid] = 0;
                                Blacklist60Seconds.Add(CurrentTarget.AnnId);
                                Blacklist60LastClear = DateTime.UtcNow;
                            }
                        }
                        break;
                    }
                // * Destructible - need to pick an ability and attack it
                case TrinityObjectType.Destructible:
                case TrinityObjectType.Barricade:
                    {
                        if (CombatBase.CurrentPower.SNOPower != SNOPower.None)
                        {
                            if (CurrentTarget.Type == TrinityObjectType.Barricade)
                            {
                                Logger.Log(TrinityLogLevel.Verbose, LogCategory.Behavior,
                                    "Barricade: Name={0}. SNO={1}, Range={2}. Needed range={3}. Radius={4}. Type={5}. Using power={6}",
                                    CurrentTarget.InternalName,     // 0
                                    CurrentTarget.ActorSNO,         // 1
                                    CurrentTarget.Distance,         // 2
                                    TargetRangeRequired,            // 3
                                    CurrentTarget.Radius,           // 4
                                    CurrentTarget.Type,             // 5
                                    CombatBase.CurrentPower.SNOPower// 6 
                                    );
                            }
                            else
                            {
                                Logger.Log(TrinityLogLevel.Verbose, LogCategory.Behavior,
                                    "Destructible: Name={0}. SNO={1}, Range={2}. Needed range={3}. Radius={4}. Type={5}. Using power={6}",
                                    CurrentTarget.InternalName,       // 0
                                    CurrentTarget.ActorSNO,           // 1
                                    TargetCurrentDistance,            // 2
                                    TargetRangeRequired,              // 3 
                                    CurrentTarget.Radius,             // 4
                                    CurrentTarget.Type,               // 5
                                    CombatBase.CurrentPower.SNOPower  // 6
                                    );
                            }

                            //if (CurrentTarget.RActorId == _ignoreRactorGuid || DataDictionary.DestroyAtLocationIds.Contains(CurrentTarget.ActorSnoId))
                            if (DataDictionary.DestroyAtLocationIds.Contains(CurrentTarget.ActorSNO))
                            {
                                // Location attack - attack the Vector3/map-area (equivalent of holding shift and left-clicking the object in-game to "force-attack")
                                Vector3 vAttackPoint;
                                if (CurrentTarget.Distance >= 6f)
                                    vAttackPoint = MathEx.CalculatePointFrom(CurrentTarget.Position, Player.Position, 6f);
                                else
                                    vAttackPoint = CurrentTarget.Position;

                                vAttackPoint.Z += 1.5f;

                                Logger.LogVerbose(LogCategory.Behavior, "Attacking  destructable at location");

                                if (ZetaDia.Me.UsePower(CombatBase.CurrentPower.SNOPower, vAttackPoint, CurrentWorldDynamicId, -1))
                                {
                                    SpellHistory.RecordSpell(CombatBase.CurrentPower.SNOPower);
                                }
                                else
                                {
                                    Navigator.PlayerMover.MoveTowards(vAttackPoint);

                                    if (ZetaDia.Me.UsePower(CombatBase.DefaultWeaponPower, vAttackPoint, CurrentWorldDynamicId, -1))
                                    {
                                        SpellHistory.RecordSpell(CombatBase.CurrentPower.SNOPower);
                                    }

                                    TrinityPlugin.LastActionTimes.Add(DateTime.UtcNow);
                                }
                               
                                if (CombatBase.CurrentPower.SNOPower == SNOPower.Monk_TempestRush)
                                    MonkCombat.LastTempestRushLocation = vAttackPoint;
                            }
                            else
                            {
                                Logger.LogVerbose(LogCategory.Behavior, "Attacking  destructable");

                                // Standard attack - attack the ACDGUID (equivalent of left-clicking the object in-game)
                                if (ZetaDia.Me.UsePower(CombatBase.CurrentPower.SNOPower, CurrentTarget.Position, -1, CurrentTarget.ACDGuid))
                                {
                                    SpellHistory.RecordSpell(CombatBase.CurrentPower.SNOPower);
                                }
                                else
                                {
                                    CombatBase.CurrentPower = CombatBase.DefaultPower;
                                    Navigator.PlayerMover.MoveTowards(CurrentTarget.Position);

                                    if (ZetaDia.Me.UsePower(CombatBase.CurrentPower.SNOPower, CurrentTarget.Position, -1, CurrentTarget.ACDGuid))
                                    {
                                        SpellHistory.RecordSpell(CombatBase.CurrentPower.SNOPower);
                                    }
                                    else
                                    {
                                        if (ZetaDia.Me.UsePower(CombatBase.CurrentPower.SNOPower, CurrentTarget.Position))
                                        {
                                            SpellHistory.RecordSpell(CombatBase.CurrentPower.SNOPower);
                                        }
                                        else
                                        {
                                            TrinityPlugin.LastActionTimes.Add(DateTime.UtcNow);
                                        }
                                    }
                                }


                                if (CombatBase.CurrentPower.SNOPower == SNOPower.Monk_TempestRush)
                                    MonkCombat.LastTempestRushLocation = CurrentTarget.Position;
                            }

                            int interactAttempts;
                            // Count how many times we've tried interacting
                            if (!CacheData.InteractAttempts.TryGetValue(CurrentTarget.RActorGuid, out interactAttempts))
                            {
                                CacheData.InteractAttempts.Add(CurrentTarget.RActorGuid, 1);
                            }
                            else
                            {
                                CacheData.InteractAttempts[CurrentTarget.RActorGuid]++;
                            }

                            CacheData.AbilityLastUsed[CombatBase.CurrentPower.SNOPower] = DateTime.UtcNow;

                            // Prevent this EXACT object being targetted again for a short while, just incase
                            _ignoreRactorGuid = CurrentTarget.RActorGuid;
                            _ignoreTargetForLoops = 3;
                            // Add this destructible/barricade to our very short-term ignore list
                            //Destructible3SecBlacklist.Add(CurrentTarget.RActorId);
                            Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "Blacklisting {0} {1} {2} for 3 seconds for Destrucable attack", CurrentTarget.Type, CurrentTarget.InternalName, CurrentTarget.ActorSNO);
                            _lastDestroyedDestructible = DateTime.UtcNow;
                            _needClearDestructibles = true;
                        }
                        // Now tell TrinityPlugin to get a new target!
                        _forceTargetUpdate = true;
                    }
                    break;
                default:
                    {
                        _forceTargetUpdate = true;
                        Logger.LogError("Default handle target in range encountered for {0} Type: {1}", CurrentTarget.InternalName, CurrentTarget.Type);
                        break;
                    }
            }
        }

        private static bool HandleTargetDistanceCheck()
        {
            using (new PerformanceLogger("HandleTarget.DistanceEqualCheck"))
            {
                // Count how long we have failed to move - body block stuff etc.
                if (Math.Abs(TargetCurrentDistance - LastDistanceFromTarget) < 5f && PlayerMover.GetMovementSpeed() < 1)
                {
                    ForceNewMovement = true;
                    if (DateTime.UtcNow.Subtract(_lastMovedDuringCombat).TotalMilliseconds >= 250)
                    {
                        _lastMovedDuringCombat = DateTime.UtcNow;
                        // We've been stuck at least 250 ms, let's go and pick new targets etc.
                        _timesBlockedMoving++;
                        _forceCloseRangeTarget = true;
                        _lastForcedKeepCloseRange = DateTime.UtcNow;
                        // And tell TrinityPlugin to get a new target
                        _forceTargetUpdate = true;

                        // Reset the emergency loop counter and return success
                        return true;
                    }
                }
                else
                {
                    // Movement has been made, so count the time last moved!
                    _lastMovedDuringCombat = DateTime.UtcNow;
                }
            }
            return false;
        }

        /// <summary>
        /// Handles target blacklist assignment if necessary, used for all targets (units/gold/items/interactables)
        /// </summary>
        /// <param name="runStatus"></param>
        /// <returns></returns>
        private static bool HandleTargetTimeoutTask()
        {
            using (new PerformanceLogger("HandleTarget.TargetTimeout"))
            {

                bool shouldTryBlacklist = false;

                // don't timeout on avoidance
                if (CurrentTarget.Type == TrinityObjectType.Avoidance)
                    return false;

                // don't timeout on legendary items
                if (CurrentTarget.Type == TrinityObjectType.Item && CurrentTarget.ItemQuality >= ItemQuality.Legendary)
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

                if ((CurrentTargetIsUnit() && CurrentTarget.IsBoss && GetSecondsSinceTargetUpdate() > 45))
                    shouldTryBlacklist = true;

                if ((CurrentTargetIsUnit() && !CurrentTarget.IsBoss && GetSecondsSinceTargetUpdate() > 30))
                    shouldTryBlacklist = true;

                if (CurrentTarget.Type == TrinityObjectType.HotSpot)
                    shouldTryBlacklist = false;

                if (shouldTryBlacklist)
                {

                    bool isNavigable = CurrentDestination.Distance(Player.Position) < 3f || NavHelper.CanRayCast(Player.Position, CurrentDestination);
                    bool addTargetToBlacklist = true;

                    var isKamakaziOnGoblin = CurrentTarget.IsUnit && isNavigable && CurrentTarget.IsTreasureGoblin && Settings.Combat.Misc.GoblinPriority >= GoblinPriority.Kamikaze;
                    if (isKamakaziOnGoblin)
                    {
                        addTargetToBlacklist = false;
                    }

                    int interactAttempts;
                    CacheData.InteractAttempts.TryGetValue(CurrentTarget.RActorGuid, out interactAttempts);

                    if ((CurrentTarget.Type == TrinityObjectType.Door || CurrentTarget.Type == TrinityObjectType.Interactable || CurrentTarget.Type == TrinityObjectType.Container) &&
                        interactAttempts < 45 && DateTime.UtcNow.Subtract(PlayerMover.LastRecordedAnyStuck).TotalSeconds > 15)
                    {
                        addTargetToBlacklist = false;
                    }

                    if (addTargetToBlacklist)
                    {
                        if (CurrentTarget.IsBoss)
                        {
                            Blacklist3Seconds.Add(CurrentTarget.AnnId);
                            Blacklist3LastClear = DateTime.UtcNow;
                            TargetUtil.ClearCurrentTarget("HandleTargetTimeoutTask: Blacklisted - CurrentTarget.IsBoss");
                            return true;
                        }

                        if (CurrentTarget.Type == TrinityObjectType.Item && CurrentTarget.ItemQuality >= ItemQuality.Legendary)
                        {
                            return false;
                        }

                        if (CurrentTarget.Type == TrinityObjectType.ProgressionGlobe)
                        {
                            return false;
                        }

                        if (CurrentTarget.IsUnit)
                        {
                            Logger.LogDebug(
                                "Blacklisting a monster because of possible stuck issues. Monster={0} [{1}] Range={2:0} health %={3:0} RActorGUID={4}",
                                CurrentTarget.InternalName,         // 0
                                CurrentTarget.ActorSNO,             // 1
                                CurrentTarget.Distance,       // 2
                                CurrentTarget.HitPointsPct,            // 3
                                CurrentTarget.RActorGuid            // 4
                                );
                        }
                        else
                        {
                            Logger.LogDebug(
                                "Blacklisting an object because of possible stuck issues. Object={0} [{1}]. Range={2:0} RActorGUID={3}",
                                CurrentTarget.InternalName,         // 0
                                CurrentTarget.ActorSNO,             // 1 
                                CurrentTarget.Distance,       // 2
                                CurrentTarget.RActorGuid            // 3
                                );
                        }

                        Blacklist15Seconds.Add(CurrentTarget.AnnId);
                        Blacklist15LastClear = DateTime.UtcNow;
                        TargetUtil.ClearCurrentTarget($"HandleTargetTimeoutTask: Blacklisted: {CurrentTarget.Type} - {CurrentTarget.InternalName} - {CurrentTarget.Distance}");
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Checks to see if we need a new monster power and will assign it to <see cref="CurrentPower"/>, distinguishes destructables/barricades from units
        /// </summary>
        private static void AssignPower()
        {
            using (new PerformanceLogger("HandleTarget.AssignMonsterTargetPower"))
            {
                if (CombatBase.CurrentPower.TimeSinceAssignedMs > 500)
                {
                    _shouldPickNewAbilities = true;
                }

                // Find a valid ability if the target is a monster
                if (_shouldPickNewAbilities && !_isWaitingForPower && !_isWaitingForPotion && !_isWaitingBeforePower)
                {
                    _shouldPickNewAbilities = false;
                    if (CurrentTarget.IsUnit)
                    {
                        // Pick a suitable ability
                        CombatBase.CurrentPower = AbilitySelector();

                        if (Player.IsInCombat && CombatBase.CurrentPower.SNOPower == SNOPower.None && !Player.IsIncapacitated)
                        {
                            NoAbilitiesAvailableInARow++;
                            if (DateTime.UtcNow.Subtract(lastRemindedAboutAbilities).TotalSeconds > 60 && NoAbilitiesAvailableInARow >= 4)
                            {
                                lastRemindedAboutAbilities = DateTime.UtcNow;
                                Logger.Log(TrinityLogLevel.Info, LogCategory.Behavior, "Error: Couldn't find a valid attack ability. Not enough resource for any abilities or all on cooldown");
                                Logger.Log(TrinityLogLevel.Info, LogCategory.Behavior, "If you get this message frequently, you should consider changing your build");
                            }
                        }
                        else
                        {
                            NoAbilitiesAvailableInARow = 0;
                        }
                    }

                    // Select an ability for destroying a destructible with in advance
                    if (CurrentTarget.Type == TrinityObjectType.Destructible || CurrentTarget.Type == TrinityObjectType.Barricade)
                        CombatBase.CurrentPower = AbilitySelector(UseDestructiblePower: true);
                    
                    if (CombatBase.CurrentPower == null || CombatBase.CurrentPower.SNOPower == SNOPower.None)
                    {
                        _shouldPickNewAbilities = true;
                        _isWaitingForPower = false;
                        _isWaitingBeforePower = false;     
                    }
                    
                    return;                                      
                }

                if (!_isWaitingForPower && CombatBase.CurrentPower == null)
                {
                    CombatBase.CurrentPower = AbilitySelector(UseOOCBuff: true);
                }
            }
        }

        /// <summary>
        /// Will check <see cref=" _isWaitingForPotion"/> and Use a Potion if needed
        /// </summary>
        private static bool UsePotionIfNeededTask()
        {
            using (new PerformanceLogger("HandleTarget.UseHealthPotionIfNeeded"))
            {
                if (!Player.IsIncapacitated && Player.CurrentHealthPct > 0 && !Player.IsInTown &&
                    SpellHistory.TimeSinceUse(SNOPower.DrinkHealthPotion) > TimeSpan.FromSeconds(30) &&
                    (Player.CurrentHealthPct <= CombatBase.EmergencyHealthPotionLimit || ShouldSnapshot()))
                {
                    if (UsePotion())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static bool UsePotion()
        {
            var logEntry = ShouldSnapshot() ? "Using Potion to Snapshot Bane of the Stricken!" : "Using Potion";

            var legendaryPotions = CacheData.Inventory.Backpack.Where(i => i.InternalName.ToLower().Contains("healthpotion_legendary_")).ToList();
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
        private static bool ShouldSnapshot()
        {
            if (!Settings.Combat.Misc.TryToSnapshot || !Gems.BaneOfTheStricken.IsEquipped ||
                ZetaDia.Me.AttacksPerSecond < Settings.Combat.Misc.SnapshotAttackSpeed || Player.CurrentHealthPct >= 1)
                return false;

            // Check the last snapshotted attack speed
            if (SnapShot.Last.AttacksPerSecond >= Settings.Combat.Misc.SnapshotAttackSpeed)
                return false;

            return true;
        }

        private static bool CurrentTargetIsNonUnit()
        {
            return CurrentTarget.Type != TrinityObjectType.Unit;
        }

        private static bool CurrentTargetIsUnit()
        {
            return CurrentTarget.IsUnit;
        }

        /// <summary>
        /// Returns the number of seconds since our current target was updated
        /// </summary>
        /// <returns></returns>
        private static double GetSecondsSinceTargetUpdate()
        {
            return DateTime.UtcNow.Subtract(_lastPickedTargetTime).TotalSeconds;
        }

        private static string lastStatusText = "";
        private static List<DateTime> _lastActionTimes;

        /// <summary>
        /// Updates bot status text with appropriate information if we are moving into range of our <see cref="CurrentTarget"/>
        /// </summary>
        private static void UpdateStatusTextTarget(bool targetIsInRange)
        {
            string action = "";

            StringBuilder statusText = new StringBuilder();
            if (!targetIsInRange)
                action = "Moveto ";
            else
                switch (CurrentTarget.Type)
                {
                    case TrinityObjectType.Avoidance:
                        action = "Avoid ";
                        break;
                    case TrinityObjectType.Unit:
                        action = "Attack ";
                        break;
                    case TrinityObjectType.Item:
                    case TrinityObjectType.Gold:
                    case TrinityObjectType.PowerGlobe:
                    case TrinityObjectType.HealthGlobe:
                    case TrinityObjectType.ProgressionGlobe:
                        action = "Pickup ";
                        break;
                    case TrinityObjectType.Interactable:
                        action = "Interact ";
                        break;
                    case TrinityObjectType.Door:
                    case TrinityObjectType.Container:
                        action = "Open ";
                        break;
                    case TrinityObjectType.Destructible:
                    case TrinityObjectType.Barricade:
                        action = "Destroy ";
                        break;
                    case TrinityObjectType.Shrine:
                        action = "Click ";
                        break;
                }
            statusText.Append(action);

            statusText.Append("Target=");
            statusText.Append(CurrentTarget.InternalName);
            if (CurrentTarget.IsUnit && CombatBase.CurrentPower.SNOPower != SNOPower.None)
            {
                statusText.Append(" Power=");
                statusText.Append(CombatBase.CurrentPower.SNOPower);
            }
            //statusText.Append(" Speed=");
            //statusText.Append(ZetaDia.Me.Movement.SpeedXY.ToString("0.00"));
            statusText.Append(" SNO=");
            statusText.Append(CurrentTarget.ActorSNO.ToString(CultureInfo.InvariantCulture));
            statusText.Append(" Elite=");
            statusText.Append(CurrentTarget.IsBossOrEliteRareUnique.ToString());
            statusText.Append(" Weight=");
            statusText.Append(CurrentTarget.Weight.ToString("0"));
            statusText.Append(" Type=");
            statusText.Append(CurrentTarget.Type.ToString());
            statusText.Append(" C-Dist=");
            statusText.Append(CurrentTarget.Distance.ToString("0.0"));
            statusText.Append(" R-Dist=");
            statusText.Append(CurrentTarget.RadiusDistance.ToString("0.0"));
            statusText.Append(" RangeReq'd=");
            statusText.Append(TargetRangeRequired.ToString("0.0"));
            statusText.Append(" DistfromTrgt=");
            statusText.Append(TargetCurrentDistance.ToString("0"));
            statusText.Append(" tHP=");
            statusText.Append((CurrentTarget.HitPointsPct * 100).ToString("0"));
            statusText.Append(" MyHP=");
            statusText.Append((Player.CurrentHealthPct * 100).ToString("0"));
            statusText.Append(" MyMana=");
            statusText.Append((Player.PrimaryResource).ToString("0"));
            statusText.Append(" InLoS=");
            statusText.Append(CurrentTargetIsInLoS.ToString());

            statusText.Append(String.Format(" Duration={0:0}", DateTime.UtcNow.Subtract(_lastPickedTargetTime).TotalSeconds));

            if (Settings.Advanced.DebugInStatusBar)
            {
                _statusText = statusText.ToString();
                BotMain.StatusText = _statusText;
            }
            if (lastStatusText != statusText.ToString())
            {
                // prevent spam
                lastStatusText = statusText.ToString();
                Logger.Log(TrinityLogLevel.Debug, LogCategory.Targetting, "{0}", statusText.ToString());
                _resetStatusText = true;
            }
        }

        /// <summary>
        /// Moves our player if no special ability is available
        /// </summary>
        /// <param name="bForceNewMovement"></param>
        private static void HandleTargetBasicMovement(bool bForceNewMovement)
        {
            using (new PerformanceLogger("HandleTarget.HandleBasicMovement"))
            {
                // Now for the actual movement request stuff
                IsAlreadyMoving = true;
                lastMovementCommand = DateTime.UtcNow;
                
                if (DateTime.UtcNow.Subtract(lastSentMovePower).TotalMilliseconds >= 250 || Vector3.Distance(LastMoveToTarget, CurrentDestination) >= 2f || bForceNewMovement)
                {

                    if(CurrentTarget.IsSafeSpot)
                        Logger.Log(LogCategory.Avoidance, $"Moving to SafeSpot Distance={CurrentTarget.Distance}");

                    var distance = CurrentDestination.Distance(Player.Position);
                    var straightLinePathing = !DataDictionary.StraightLinePathingLevelAreaIds.Contains(Player.LevelAreaId) && distance <= 35f && !PlayerMover.IsBlocked && !Navigator.StuckHandler.IsStuck && NavHelper.CanRayCast(CurrentDestination);

                    string destname = String.Format("{0} {1:0} yds Elite={2} LoS={3} HP={4:0.00} Dir={5}",
                        CurrentTarget.InternalName,
                        CurrentTarget.Distance,
                        CurrentTarget.IsBossOrEliteRareUnique,
                        CurrentTarget.HasBeenInLoS,
                        CurrentTarget.HitPointsPct,
                        MathUtil.GetHeadingToPoint(CurrentTarget.Position));

                    MoveResult lastMoveResult;
                    if (straightLinePathing || distance < 10f)
                    {
                        lastMoveResult = MoveResult.Moved;
                        // just "Click" 
                        Navigator.PlayerMover.MoveTowards(CurrentDestination);
                        Logger.LogVerbose(LogCategory.Movement, "Straight line pathing to {0}", destname);
                    }                    
                    else
                    {
                        lastMoveResult = PlayerMover.NavigateTo(CurrentDestination, destname);
                    }

                    lastSentMovePower = DateTime.UtcNow;

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
                    //            + (CurrentTarget.ItemQuality >= ItemQuality.Legendary ? "IsLegendaryItem " : ""),
                    //            CurrentTarget.InternalName, CurrentTarget.ActorSnoId, CurrentTarget.RActorId, CurrentTarget.Distance);
                    //    }
                    //}

                    // Store the current destination for comparison incase of changes next loop
                    LastMoveToTarget = CurrentDestination;
                    // Reset total body-block count, since we should have moved
                    if (DateTime.UtcNow.Subtract(_lastForcedKeepCloseRange).TotalMilliseconds >= 2000)
                        _timesBlockedMoving = 0;
                }
            }
        }

        private static void SetRangeRequiredForTarget()
        {
            using (new PerformanceLogger("HandleTarget.SetRequiredRange"))
            {
                TargetRangeRequired = 2f;
                TargetCurrentDistance = CurrentTarget.RadiusDistance;
                CurrentTargetIsInLoS = false;
                // Set current destination to our current target's destination
                CurrentDestination = CurrentTarget.Position;

                switch (CurrentTarget.Type)
                {
                    // * Unit, we need to pick an ability to use and get within range
                    case TrinityObjectType.Unit:
                        {
                            // Pick a range to try to reach
                            TargetRangeRequired = CombatBase.CurrentPower.MinimumRange;
                            break;
                        }
                    // * Item - need to get within 6 feet and then interact with it
                    case TrinityObjectType.Item:
                        {
                            TargetRangeRequired = 2f;
                            TargetCurrentDistance = CurrentTarget.Distance;
                            break;
                        }
                    // * Gold - need to get within pickup radius only
                    case TrinityObjectType.Gold:
                        {
                            TargetRangeRequired = 2f;
                            TargetCurrentDistance = CurrentTarget.Distance;
                            CurrentDestination = MathEx.CalculatePointFrom(Player.Position, CurrentTarget.Position, -2f);
                            break;
                        }
                    // * Globes - need to get within pickup radius only
                    case TrinityObjectType.PowerGlobe:
                    case TrinityObjectType.HealthGlobe:
                    case TrinityObjectType.ProgressionGlobe:
                        {
                            TargetRangeRequired = 2f;
                            TargetCurrentDistance = CurrentTarget.Distance;
                            break;
                        }
                    // * Shrine & Container - need to get within 8 feet and interact
                    case TrinityObjectType.HealthWell:
                        {
                            TargetRangeRequired = 4f;

                            float range;
                            if (DataDictionary.CustomObjectRadius.TryGetValue(CurrentTarget.ActorSNO, out range))
                            {
                                TargetRangeRequired = range;
                            }
                            break;
                        }
                    case TrinityObjectType.Shrine:
                    case TrinityObjectType.Container:
                        {
                            TargetRangeRequired = 6f;

                            float range;
                            if (DataDictionary.CustomObjectRadius.TryGetValue(CurrentTarget.ActorSNO, out range))
                            {
                                TargetRangeRequired = range;
                            }
                            break;
                        }
                    case TrinityObjectType.Interactable:
                        {
                            if (CurrentTarget.IsQuestGiver)
                            {
                                CurrentDestination = MathEx.CalculatePointFrom(CurrentTarget.Position, Player.Position, CurrentTarget.Radius + 2f);
                                TargetRangeRequired = 5f;
                            }
                            else
                            {
                                TargetRangeRequired = 5f;
                            }
                            // Check if it's in our interactable range dictionary or not
                            float range;

                            if (DataDictionary.CustomObjectRadius.TryGetValue(CurrentTarget.ActorSNO, out range))
                            {
                                TargetRangeRequired = range;
                            }
                            if (TargetRangeRequired <= 0)
                                TargetRangeRequired = CurrentTarget.Radius;

                            break;
                        }
                    // * Destructible - need to pick an ability and attack it
                    case TrinityObjectType.Destructible:
                        {
                            // Pick a range to try to reach + (tmp_fThisRadius * 0.70);
                            //TargetRangeRequired = CombatBase.CurrentPower.SNOPower == SNOPower.None ? 9f : CombatBase.CurrentPower.MinimumRange;
                            TargetRangeRequired = CombatBase.CurrentPower.MinimumRange;
                            CurrentTarget.Radius = 1f;
                            TargetCurrentDistance = CurrentTarget.Distance;
                            break;
                        }
                    case TrinityObjectType.Barricade:
                        {
                            // Pick a range to try to reach + (tmp_fThisRadius * 0.70);
                            TargetRangeRequired = CombatBase.CurrentPower.MinimumRange;
                            CurrentTarget.Radius = 1f;
                            TargetCurrentDistance = CurrentTarget.Distance;
                            break;
                        }
                    // * Avoidance - need to pick an avoid location and move there
                    case TrinityObjectType.Avoidance:
                        {
                            TargetRangeRequired = 2f;
                            break;
                        }
                    case TrinityObjectType.Door:
                        TargetRangeRequired = 2f;
                        break;
                    default:
                        TargetRangeRequired = CurrentTarget.Radius;
                        break;
                }
            }
        }

        private static void HandleUnitInRange()
        {
            using (new PerformanceLogger("HandleTarget.HandleUnitInRange"))
            {
                bool usePowerResult;

                if (CombatBase.CurrentPower == null)
                {
                    Logger.LogVerbose(LogCategory.Targetting, "Current Power is null, Target={0}", CurrentTarget.InternalName);
                }

                if (CombatBase.CurrentPower == null)
                {
                    Logger.LogVerbose(LogCategory.Targetting, "Current Power is SNOPower.None, Target={0}", CurrentTarget.InternalName);
                }

                if (CurrentTarget.HitPoints <= 0)
                {
                    Logger.LogVerbose(LogCategory.Targetting, "Target is Dead ({0})", CurrentTarget.InternalName);
                    return;
                }

                float distance;
                Vector3 targetPosition = Vector3.Zero;
                int targetACDGuid = -1;                

                if (CombatBase.CurrentPower.IsCastOnSelf)
                {
                    distance = 0;
                    targetACDGuid = -1;
                    targetPosition = ZetaDia.Me.Position;
                }
                else
                {
                    var isTargetPosition = CombatBase.CurrentPower.TargetPosition != Vector3.Zero;
                    var isACDGuid = CombatBase.CurrentPower.TargetACDGUID != -1;
                    var isValidTargetData = isACDGuid && isTargetPosition;

                    // TrinityPower gave us everything we need, lets rely on its accuracy
                    if (isValidTargetData)
                    {
                        Logger.LogVerbose(LogCategory.Behavior, "{0} Using power targetACD and targetPosition", CombatBase.CurrentPower.SNOPower);

                        targetPosition = CombatBase.CurrentPower.TargetPosition;
                        targetACDGuid = CombatBase.CurrentPower.TargetACDGUID;
                    }

                    // We were given only an ACDId, find the actors position.
                    if (!isTargetPosition && isACDGuid)
                    {
                        var powerTarget = ObjectCache.FirstOrDefault(o => o.ACDGuid == CombatBase.CurrentPower.TargetACDGUID);
                        if (powerTarget != null)
                        {
                            Logger.LogVerbose(LogCategory.Behavior, "{0} Found target by ACDId", CombatBase.CurrentPower.SNOPower);

                            targetPosition = powerTarget.Position;
                            targetACDGuid = powerTarget.ACDGuid;
                        }                        
                    }

                    distance = ZetaDia.Me.Position.Distance(targetPosition); 

                    // We don't have valid casting data, use current target
                    if ((!isTargetPosition && !isACDGuid || targetPosition == Vector3.Zero || targetACDGuid == -1 || distance > 150f) && !CurrentTarget.IsSafeSpot)
                    {
                        Logger.LogVerbose(LogCategory.Behavior,"{0} Using  CurrentTarget Position/ACD", CombatBase.CurrentPower.SNOPower);
                        targetPosition = CurrentTarget.Position;
                        targetACDGuid = CurrentTarget.ACDGuid; 
                    }

                    if (isTargetPosition && !isACDGuid)
                    {
                        Logger.LogVerbose(LogCategory.Behavior,"{0} Using target position only.", CombatBase.CurrentPower.SNOPower);
                        targetPosition = CombatBase.CurrentPower.TargetPosition;
                        targetACDGuid = CombatBase.CurrentPower.TargetACDGUID;
                    }
                }

                if (targetPosition == Vector3.Zero)
                {
                    Logger.LogVerbose(LogCategory.Targetting, "Can't cast on Vector3.Zero!");
                    return;
                }

                if (targetACDGuid > 0)
                {
                    var targetAcd = ZetaDia.Actors.GetActorByACDId(targetACDGuid);
                    if (!targetAcd.IsFullyValid())
                    {
                        Logger.LogVerbose("Invalid target Acd, probably dead");
                        return;
                    }
                    var unit = targetAcd as DiaUnit;
                    if (unit == null || unit.HitpointsCurrentPct <= 0 || unit.HitpointsCurrentPct > 1)
                    {
                        Logger.LogVerbose("Invalid target hitpoints, probably dead");
                        return;
                    }
                }


                var d = targetPosition.Distance(Player.Position);
                if (d > 120)
                {
                    Logger.LogVerbose(LogCategory.Targetting, $"Target position is too far away! {d}");
                    return;
                }

                // See if we should force a long wait BEFORE casting
                _isWaitingBeforePower = CombatBase.CurrentPower.ShouldWaitBeforeUse;
                if (_isWaitingBeforePower)
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
                        //    targetACDGuid, CombatBase.CurrentPower.IsCastOnSelf, flags, ZetaDia.Me.IsDead, ZetaDia.Actors.GetActorsOfType<DiaPlayer>().Any(x => x.IsDead));

                        if (powerBuff != null && powerBuff.ShouldWaitForAttackToFinish)
                            _isWaitingForAttackToFinish = true;

                        usePowerResult = ZetaDia.Me.UsePower(CombatBase.CurrentPower.SNOPower, targetPosition, CombatBase.CurrentPower.TargetDynamicWorldId, targetACDGuid);

                        if (!usePowerResult)
                        {
                            usePowerResult = ZetaDia.Me.UsePower(CombatBase.CurrentPower.SNOPower, targetPosition, CombatBase.CurrentPower.TargetDynamicWorldId);
                        }
                    }
                    else
                    {
                        Logger.Log(LogCategory.Targetting, "Unable to Cast {0} at {1} WorldId={2} ACDId={3} CastOnSelf={4} Flags={5}",
                            CombatBase.CurrentPower.SNOPower, targetPosition, CombatBase.CurrentPower.TargetDynamicWorldId, targetACDGuid, CombatBase.CurrentPower.IsCastOnSelf, flags);

                        usePowerResult = false;
                    }
                }

                var skill = SkillUtils.ById(CombatBase.CurrentPower.SNOPower);
                string target = GetTargetName();


                if (usePowerResult)
                {
                    // Monk Stuffs get special attention
                    {
                        if (CombatBase.CurrentPower.SNOPower == SNOPower.Monk_TempestRush)
                            MonkCombat.LastTempestRushLocation = CombatBase.CurrentPower.TargetPosition;

                        MonkCombat.RunOngoingPowers();
                    }

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
                            targetPosition.Distance(Player.Position),
                            CurrentTarget != null ? CurrentTarget.InternalName : "Null",
                            skill.Charges
                            );

                    }
                    else
                    {
                        Logger.LogVerbose("Used Power {0} " + target, CombatBase.CurrentPower.SNOPower);
                    }

                    SpellTracker.TrackSpellOnUnit(CombatBase.CurrentPower.TargetACDGUID, CombatBase.CurrentPower.SNOPower);
                    SpellHistory.RecordSpell(CombatBase.CurrentPower);

                    CacheData.AbilityLastUsed[CombatBase.CurrentPower.SNOPower] = DateTime.UtcNow;
                    lastGlobalCooldownUse = DateTime.UtcNow;
                    LastPowerUsed = CombatBase.CurrentPower.SNOPower;

                    // See if we should force a long wait AFTERWARDS, too
                    // Force waiting AFTER power use for certain abilities
                    _isWaitingAfterPower = CombatBase.CurrentPower.ShouldWaitAfterUse;

                    _shouldPickNewAbilities = true;

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
                                           Player.IsFacing(CombatBase.CurrentPower.TargetPosition),
                                           CurrentTarget != null && Player.IsFacing(CurrentTarget.Position),
                                           CurrentTarget != null ? CurrentTarget.InternalName : "Null",
                                           skill.Meta.AfterUseDelay, 
                                           ZetaDia.Me.CommonData.CurrentAnimation
                                           );
                    }
                    else
                    {
                        Logger.LogVerbose(LogCategory.Behavior, "Failed to use power {0} (CurrentAnimation={1})" + target, CombatBase.CurrentPower.SNOPower, ZetaDia.Me.CommonData.CurrentAnimation);
                    }

                    CombatBase.CurrentPower.CastAttempts++;
                    _shouldPickNewAbilities = CombatBase.CurrentPower.CastAttempts >= CombatBase.CurrentPower.MaxFailedCastReTryAttempts;
                }

                

                // Keep looking for monsters at "normal kill range" a few moments after we successfully attack a monster incase we can pull them into range
                _keepKillRadiusExtendedForSeconds = 8;
                _timeKeepKillRadiusExtendedUntil = DateTime.UtcNow.AddSeconds(_keepKillRadiusExtendedForSeconds);
                _keepLootRadiusExtendedForSeconds = 8;
                // if at full or nearly full health, see if we can raycast to it, if not, ignore it for 2000 ms
                if (CurrentTarget.HitPointsPct >= 0.9d &&
                    !NavHelper.CanRayCast(Player.Position, CurrentTarget.Position) &&
                    !CurrentTarget.IsBoss &&
                    !(DataDictionary.StraightLinePathingLevelAreaIds.Contains(Player.LevelAreaId) || DataDictionary.LineOfSightWhitelist.Contains(CurrentTarget.ActorSNO)))
                {
                    _ignoreRactorGuid = CurrentTarget.RActorGuid;
                    _ignoreTargetForLoops = 6;
                    // Add this monster to our very short-term ignore list
                    Blacklist3Seconds.Add(CurrentTarget.AnnId);
                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.Behavior, "Blacklisting {0} {1} {2} for 3 seconds due to Raycast failure", CurrentTarget.Type, CurrentTarget.InternalName, CurrentTarget.ActorSNO);
                    Blacklist3LastClear = DateTime.UtcNow;
                    NeedToClearBlacklist3 = true;
                }

            }
        }



        private static string GetTargetName()
        {
            float dist = 0;
            if (CombatBase.CurrentPower.TargetPosition != Vector3.Zero)
                dist = CombatBase.CurrentPower.TargetPosition.Distance(Player.Position);
            else if (CurrentTarget != null)
                dist = CurrentTarget.Position.Distance(Player.Position);

            var name = CurrentTarget != null && CurrentTarget.ACDGuid == CombatBase.CurrentPower.TargetACDGUID ? CurrentTarget.InternalName : string.Empty;

            string target = CombatBase.CurrentPower.TargetPosition != Vector3.Zero ? "at " + NavHelper.PrettyPrintVector3(CombatBase.CurrentPower.TargetPosition) + " dist=" + (int)dist : "";
            target += CombatBase.CurrentPower.TargetACDGUID != -1 ? " on " + name + " (" + CombatBase.CurrentPower.TargetACDGUID : ")";
            return target;
        }

        private static int HandleItemInRange()
        {
            using (new PerformanceLogger("HandleTarget.HandleItemInRange"))
            {
                int iInteractAttempts;
                // Pick the item up the usepower way, and "blacklist" for a couple of loops
                if (ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, CurrentTarget.ACDGuid))
                {
                    SpellHistory.RecordSpell(SNOPower.Axe_Operate_Gizmo);
                }
                else
                {
                    TrinityPlugin.LastActionTimes.Add(DateTime.UtcNow);
                }
                _ignoreRactorGuid = CurrentTarget.RActorGuid;
                _ignoreTargetForLoops = 3;
                // Store item pickup stats

 

                string itemSha1Hash = HashGenerator.GenerateItemHash(CurrentTarget.Position, CurrentTarget.ActorSNO, CurrentTarget.InternalName, CurrentWorldDynamicId, CurrentTarget.ItemQuality, CurrentTarget.ItemLevel);
                if (!ItemDropStats._hashsetItemPicksLookedAt.Contains(itemSha1Hash))
                {
                    ItemDropStats._hashsetItemPicksLookedAt.Add(itemSha1Hash);
                    TrinityItemType itemType = TrinityItemManager.DetermineItemType(CurrentTarget.InternalName, CurrentTarget.DBItemType, CurrentTarget.FollowerType);
                    TrinityItemBaseType itemBaseType = TrinityItemManager.DetermineBaseType(itemType);
                    if (itemBaseType == TrinityItemBaseType.Armor || itemBaseType == TrinityItemBaseType.WeaponOneHand || itemBaseType == TrinityItemBaseType.WeaponTwoHand ||
                        itemBaseType == TrinityItemBaseType.WeaponRange || itemBaseType == TrinityItemBaseType.Jewelry || itemBaseType == TrinityItemBaseType.FollowerItem ||
                        itemBaseType == TrinityItemBaseType.Offhand)
                    {
                        int iQuality;
                        ItemDropStats.ItemsPickedStats.Total++;
                        if (CurrentTarget.ItemQuality >= ItemQuality.Legendary)
                            iQuality = ItemDropStats.QUALITYORANGE;
                        else if (CurrentTarget.ItemQuality >= ItemQuality.Rare4)
                            iQuality = ItemDropStats.QUALITYYELLOW;
                        else if (CurrentTarget.ItemQuality >= ItemQuality.Magic1)
                            iQuality = ItemDropStats.QUALITYBLUE;
                        else
                            iQuality = ItemDropStats.QUALITYWHITE;
                        //asserts	
                        if (iQuality > ItemDropStats.QUALITYORANGE)
                        {
                            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "ERROR: Item type (" + iQuality + ") out of range");
                        }
                        if ((CurrentTarget.ItemLevel < 0) || (CurrentTarget.ItemLevel >= 74))
                        {
                            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "ERROR: Item level (" + CurrentTarget.ItemLevel + ") out of range");
                        }
                        ItemDropStats.ItemsPickedStats.TotalPerQuality[iQuality]++;
                        ItemDropStats.ItemsPickedStats.TotalPerLevel[CurrentTarget.ItemLevel]++;
                        ItemDropStats.ItemsPickedStats.TotalPerQPerL[iQuality, CurrentTarget.ItemLevel]++;
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
                        ItemDropStats.ItemsPickedStats.GemsPerLevel[CurrentTarget.ItemLevel]++;
                        ItemDropStats.ItemsPickedStats.GemsPerTPerL[iGemType, CurrentTarget.ItemLevel]++;
                    }
                    else if (itemType == TrinityItemType.HealthPotion)
                    {
                        ItemDropStats.ItemsPickedStats.TotalPotions++;
                        if ((CurrentTarget.ItemLevel < 0) || (CurrentTarget.ItemLevel > 63))
                        {
                            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "ERROR: Potion level ({0}) out of range", CurrentTarget.ItemLevel);
                        }
                        ItemDropStats.ItemsPickedStats.PotionsPerLevel[CurrentTarget.ItemLevel]++;
                    }
                    else if (_cItemTinityItemType == TrinityItemType.InfernalKey)
                    {
                        ItemDropStats.ItemsPickedStats.TotalInfernalKeys++;
                    }
                }

                // Count how many times we've tried interacting
                if (!CacheData.InteractAttempts.TryGetValue(CurrentTarget.RActorGuid, out iInteractAttempts))
                {
                    CacheData.InteractAttempts.Add(CurrentTarget.RActorGuid, 1);

                    // Fire item looted for Demonbuddy Item stats
                    GameEvents.FireItemLooted(CurrentTarget.ACDGuid);
                }
                else
                {
                    CacheData.InteractAttempts[CurrentTarget.RActorGuid]++;
                }
                // If we've tried interacting too many times, blacklist this for a while
                if (iInteractAttempts > 20 && CurrentTarget.ItemQuality < ItemQuality.Legendary)
                {
                    Blacklist90Seconds.Add(CurrentTarget.AnnId);
                }
                // Now tell TrinityPlugin to get a new target!
                _forceTargetUpdate = true;
                return iInteractAttempts;
            }
        }

        public static bool _isWaitingForAttackToFinish { get; set; }
    }
}
