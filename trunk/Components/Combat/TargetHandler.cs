#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Buddy.Coroutines;
using Trinity.Components.Combat.Abilities;
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

#endregion

namespace Trinity.Components.Combat
{
    public class TargetHandler
    {
        public static float TargetRangeRequired { get; set; }
        public static float TargetCurrentRadiusDistance { get; set; }

        /// <summary>
        /// Flag used to indicate if we are simply waiting for a power to go off - so don't do any new target checking or anything
        /// </summary>
        public static bool IsWaitingForPower;

        /// <summary>
        /// A special post power use pause, causes targetHandler to wait on any new decisions
        /// </summary>
        public static bool IsWaitingAfterPower;

        /// <summary>
        /// A special post power use pause, causes targetHandler to wait on any new decisions
        /// </summary>
        public static bool IsWaitingBeforePower;

        /// <summary>
        /// If TargetHandle is waiting waiting before popping a potion - we won't refresh cache/change targets/unstuck/etc
        /// </summary>
        public static bool IsWaitingForPotion;

        public static DateTime LastPickedTargetTime { get; set; } = DateTime.MinValue;

        private static DateTime _lastMaintenanceCheck = DateTime.UtcNow;
        public List<DateTime> _lastActionTimes;
        public int WaitedTicks;

        public HashSet<SNOPower> NonCombatPowers = new HashSet<SNOPower>
        {
            SNOPower.None,
            SNOPower.Walk
        };

        public List<DateTime> LastActionTimes
        {
            get { return _lastActionTimes ?? (_lastActionTimes = new List<DateTime>()); }
            set { _lastActionTimes = value; }
        }

        public Dictionary<int, DateTime> InteractionCooldowns { get; set; } = new Dictionary<int, DateTime>();
        public DateTime InteractionWaitUntilTime { get; set; } = DateTime.MinValue;

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

        public bool IsWaitingForAttackToFinish { get; set; }

        private static bool TargetCheckResult(bool result, string source)
        {
            Logger.LogDebug(LogCategory.GlobalHandler, "TargetCheck returning {0}, {1}", result, source);
            return result;
        }

        internal bool TargetCheck(object ret)
        {
            using (new PerformanceLogger("TargetCheck"))
            {
                if (Core.Player.IsDead)
                {
                    return TargetCheckResult(false, "Is Dead");
                }

                Trinity.TrinityPlugin._timesBlockedMoving = 0;
                Trinity.TrinityPlugin.IsAlreadyMoving = false;
                Trinity.TrinityPlugin.lastMovementCommand = DateTime.MinValue;
                IsWaitingForPower = false;
                IsWaitingAfterPower = false;
                IsWaitingBeforePower = false;
                IsWaitingForPotion = false;
                Trinity.TrinityPlugin.wasRootedLastTick = false;

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
                            Trinity.TrinityPlugin.CurrentTarget = new TrinityActor
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

                if (ShouldUsePotion())
                {
                    UsePotion();
                }
         
                // Nothing to do... do we have some maintenance we can do instead, like out of combat buffing?

                if (DateTime.UtcNow.Subtract(_lastMaintenanceCheck).TotalMilliseconds > 150)
                {
                    using (new PerformanceLogger("TargetCheck.OOCBuff"))
                    {
                        _lastMaintenanceCheck = DateTime.UtcNow;

                        var isLoopingAnimation = ZetaDia.Me.LoopingAnimationEndTime > 0;

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
                return TargetCheckResult(false, "End of TargetCheck");
            }
        }

        internal async Task<bool> HandleTarget()
        {
            Core.Player.CurrentAction = default(PlayerAction);
            RunStatus status;

            var currentTarget = Trinity.TrinityPlugin.CurrentTarget;
            if (currentTarget == null)
                return false;

            using (new PerformanceLogger("HandleTarget"))
            {
                try
                {
                    if (!CombatTargeting.Instance.AllowedToKillMonsters && (currentTarget == null || currentTarget.IsUnit) && CombatBase.CombatMode != CombatMode.KillAll)
                    {
                        Logger.LogVerbose("Aborting HandleTarget() AllowCombat={0} ShouldAvoid={1}", CombatTargeting.Instance.AllowedToKillMonsters, Core.Avoidance.Avoider.ShouldAvoid);
                        return false;
                    }

                    if (!Core.Player.IsValid)
                    {
                        Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "No longer in game world", true);
                        return false;
                    }

                    if (Core.Player.IsDead)
                    {
                        Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "Player is dead", true);
                        return false;
                    }

                    if (ShouldUsePotion())
                    {
                        UsePotion();
                    }

                    if (Core.Avoidance.Avoider.ShouldAvoid)
                    {
                        Logger.Log(LogCategory.Avoidance, $"Avoid now!");
                        Vector3 safespot;
                        if (Core.Avoidance.Avoider.TryGetSafeSpot(out safespot) && safespot.Distance(ZetaDia.Me.Position) > 3f)
                        {
                            Logger.Log(LogCategory.Avoidance, $"Safespot found: {safespot}");

                            if (currentTarget == null || currentTarget.Type != TrinityObjectType.Barricade && currentTarget.Type != TrinityObjectType.Door || Core.Avoidance.Grid.IsStandingInFlags(AvoidanceFlags.CriticalAvoidance))
                            {
                                var distance = safespot.Distance(Core.Player.Position);
                                Logger.Log(LogCategory.Avoidance, $"Targetted SafeSpot Distance={distance}");

                                currentTarget = new TrinityActor
                                {
                                    Position = safespot,
                                    Type = TrinityObjectType.Avoidance,
                                    Distance = distance,
                                    Radius = 3.5f,
                                    InternalName = "Avoidance Safespot",
                                    IsSafeSpot = true,
                                    Weight = Weighting.MaxWeight
                                };
                                Trinity.TrinityPlugin.CurrentTarget = currentTarget;

                                TryCastAvoidancePower();
                                PlayerMover.NavigateTo(safespot, "Avoidance SafeSpot");
                                Core.Player.CurrentAction = PlayerAction.Avoiding;
                                Logger.LogDebug(LogCategory.Avoidance, "Movement for Avoidance");
                                return true;
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
                            currentTarget = new TrinityActor
                            {
                                Position = safespot,
                                Type = TrinityObjectType.Avoidance,
                                Distance = distance,
                                Radius = 3.5f,
                                InternalName = "Avoidance Safespot",
                                IsSafeSpot = true,
                                Weight = Weighting.MaxWeight
                            };
                            Trinity.TrinityPlugin.CurrentTarget = currentTarget;

                            // Prevent spells like from firing in the wrong direction becasue we turned while casting it.
                            if ((DateTime.UtcNow - SpellHistory.LastSpellUseTime).TotalMilliseconds < 500)
                                await Coroutine.Wait(500, () => ZetaDia.Me.CommonData.AnimationState != AnimationState.Casting
                                                                && ZetaDia.Me.CommonData.AnimationState != AnimationState.Attacking
                                                                && ZetaDia.Me.CommonData.AnimationState != AnimationState.Transform);

                            Logger.Log(LogCategory.Avoidance, $"Movement for Kiting to {currentTarget}");
                            PlayerMover.NavigateTo(safespot, "Kiting SafeSpot");

                            Core.Player.CurrentAction = PlayerAction.Kiting;
                            Logger.LogDebug(LogCategory.Avoidance, "Movement for Kiting");
                            return true;
                        }
                    }

                    if (Core.Player.IsCasting && currentTarget != null && currentTarget.GizmoType == GizmoType.Headstone)
                    {
                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Player is casting revive ({0})", Core.Player.CurrentAnimation);
                        return true;
                    }

                    if (!Core.Avoidance.Avoider.ShouldAvoid)
                    {
                        if (ThrottleActionPerSecond(out status))
                            return true;

                        VacuumItems.Execute();

                        // Make sure we reset unstucker stuff here
                        PlayerMover.TimesReachedStuckPoint = 0;
                        PlayerMover.vSafeMovementLocation = Vector3.Zero;
                        PlayerMover.TimeLastRecordedPosition = DateTime.UtcNow;

                        // Time based wait delay for certain powers with animations
                        if (CombatBase.CurrentPower != null)
                        {
                            if (CombatBase.CurrentPower.ShouldWaitAfterUse && IsWaitingAfterPower || IsWaitingBeforePower && CombatBase.CurrentPower.ShouldWaitBeforeUse)
                            {
                                var type = IsWaitingAfterPower ? "IsWaitingAfterPower" : "IsWaitingBeforePower";
                                WaitedTicks++;
                                Logger.LogVerbose($"Waiting... {type} Power={CombatBase.CurrentPower.SNOPower} TicksWaited={WaitedTicks}");
                                return true;
                            }
                        }
                    }

                    if (CombatBase.IsDoingGoblinKamakazi && currentTarget != null && currentTarget.Type != TrinityObjectType.Door && currentTarget.Type != TrinityObjectType.Barricade && !currentTarget.InternalName.ToLower().Contains("corrupt") && currentTarget.Weight >= Weighting.MaxWeight)
                    {
                        Logger.Log("Forcing Target to Goblin '{0} ({1})' Distance={2}", CombatBase.KamakaziGoblin.InternalName, CombatBase.KamakaziGoblin.ActorSnoId, CombatBase.KamakaziGoblin.Distance);
                        currentTarget = CombatBase.KamakaziGoblin;
                    }

                    if (CombatBase.IsDoingGoblinKamakazi && currentTarget == null)
                    {
                        Logger.Log("No Target, Switching to Goblin '{0} ({1})' Distance={2}", CombatBase.KamakaziGoblin.InternalName, CombatBase.KamakaziGoblin.ActorSnoId, CombatBase.KamakaziGoblin.Distance);
                        currentTarget = CombatBase.KamakaziGoblin;
                    }

                    //if (ShouldWaitForLootDrop)
                    //{
                    //    Logger.LogVerbose("Wait for loot drop");
                    //}

                    if (IsWaitingForAttackToFinish)
                    {
                        Logger.LogVerbose("Wait for Attack to finish");
                    }

                    if (IsWaitingBeforePower)
                    {
                        Logger.LogVerbose("Wait Before Power Finished");
                    }

                    if (IsWaitingAfterPower)
                    {
                        Logger.LogVerbose("Wait After Power Finished");
                    }

                    if (IsWaitingForPotion)
                    {
                        Logger.LogVerbose("Wait for Potion");
                    }

                    if (currentTarget == null)
                    {
                        Logger.LogVerbose("CurrentTarget == null");
                    }

                    if (InteractionWaitUntilTime > DateTime.UtcNow)
                    {
                        Logger.LogDebug("Waiting after interaction");
                        return true;
                    }

                    WaitedTicks = 0;
                    IsWaitingAfterPower = false;
                    IsWaitingBeforePower = false;

                    // Select new power
                    if (!Core.Avoidance.Avoider.ShouldAvoid)
                    {
                        if (!IsWaitingForPower && !IsWaitingBeforePower && (CombatBase.CurrentPower == null || CombatBase.CurrentPower.SNOPower == SNOPower.None) && currentTarget != null)
                        {
                            CombatBase.CurrentPower = CombatManager.AbilitySelector.SelectAbility();

                            Logger.LogVerbose(LogCategory.Behavior, $"Selected Power {CombatBase.CurrentPower}");

                            if (CombatBase.CurrentPower.SNOPower == SNOPower.None)
                            {
                                Logger.LogVerbose(LogCategory.Behavior, "SNOPower.None selected from combat routine :S");
                                Trinity.TrinityPlugin._shouldPickNewAbilities = true;
                            }
                        }
                    }

                    // Prevent running away after progression globes spawn if they're in aoe
                    if (Core.Player.IsInRift && !Core.Avoidance.Avoider.ShouldAvoid)
                    {
                        var globes = Trinity.TrinityPlugin.Targets.Where(o => o.Type == TrinityObjectType.ProgressionGlobe && o.Distance < AvoidanceManager.MaxDistance).ToList();

                        //Logger.Warn($"Globes found: {globes.Count}");

                        var shouldWaitForGlobes = globes.Any(o => Core.Avoidance.Grid.IsIntersectedByFlags(ZetaDia.Me.Position, o.Position, AvoidanceFlags.CriticalAvoidance));
                        if (shouldWaitForGlobes)
                        {
                            Logger.Log($"Waiting for progression globe GlobeCount={globes.Count}");
                            var globe = globes.FirstOrDefault();
                            if (globe != null)
                            {
                                Navigator.PlayerMover.MoveTowards(globe.Position);
                            }
                            return true;
                        }
                    }

                    // Some skills we need to wait to finish (like cyclone strike while epiphany is active)
                    if (IsWaitingForAttackToFinish)
                    {
                        if (ZetaDia.Me.LoopingAnimationEndTime > 0 || ZetaDia.Me.CommonData.AnimationState == AnimationState.Attacking || ZetaDia.Me.CommonData.AnimationState == AnimationState.Casting || ZetaDia.Me.CommonData.AnimationState == AnimationState.Transform)
                        {
                            Logger.LogVerbose(LogCategory.Behavior, $"Waiting for Attack to Finish CurrentPower={CombatBase.CurrentPower}");
                            return true;
                        }
                        IsWaitingForAttackToFinish = false;
                    }

                    if (currentTarget.IsSpawningBoss && Core.Player.IsInRift)
                    {
                        Logger.LogVerbose("Rift Boss is Spawning!");
                        if (!TargetUtil.AnyTrashInRange(20f) && !Gems.Taeguk.IsEquipped)
                        {
                            Logger.LogVerbose(LogCategory.Avoidance, "Waiting for Rift Boss to Spawn");
                            Core.Player.CurrentAction = PlayerAction.Waiting;
                            return true;
                        }
                    }

                    if (!CombatBase.IsCombatAllowed && (currentTarget == null || currentTarget.IsUnit))
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Behavior, "Ending: Combat is Disabled!");
                        return false;
                    }

                    AssignPower();                    

                    if (CombatBase.CurrentPower == null)
                        return false;

                    if (TryCastAvoidancePower())
                    {
                        Logger.Log("Cast Avoidance power");
                        return true;
                    }

                    var defaultDistance = Math.Max(2f, CombatBase.CurrentTarget.RequiredRadiusDistance);
                    var combatDistance = Math.Max(2f, Math.Max(CombatBase.CurrentPower.MinimumRange, CombatBase.CurrentTarget.RequiredRadiusDistance));
                    TargetRangeRequired = CombatBase.CurrentTarget.IsHostile || CombatBase.CurrentTarget.IsDestroyable ? combatDistance : defaultDistance;


                    TargetCurrentRadiusDistance = currentTarget.RadiusDistance;

                    Logger.LogVerbose(LogCategory.Behavior, $">> CurrentPower={CombatBase.CurrentPower} CurrentTarget={CombatBase.CurrentTarget} RangeReq:{TargetRangeRequired} RadDist:{TargetCurrentRadiusDistance}");

                    if (!currentTarget.IsSafeSpot)
                    {
                        if (TargetCurrentRadiusDistance <= TargetRangeRequired && IsInLineOfSight(currentTarget))
                        {
                            Logger.LogDebug(LogCategory.Behavior, $"Target is in Range: Target In Range={(TargetCurrentRadiusDistance <= TargetRangeRequired)} power={CombatBase.CurrentPower.SNOPower} target={currentTarget}");
                            HandleTargetInRange();
                            return true;
                        }
                    }

                    Trinity.TrinityPlugin.LastDistanceFromTarget = TargetCurrentRadiusDistance;

                    if (!Core.Player.IsInTown)
                    {
                        if (TrySpecialMovement())
                            return true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error in HandleTarget: {0}", ex);
                    return false;
                }

                HandleTargetBasicMovement(CombatBase.CurrentTarget.Position);
                return true;
            }
        }

        private static bool IsInLineOfSight(TrinityActor currentTarget)
        {
            if (DataDictionary.LineOfSightWhitelist.Contains(currentTarget.ActorSnoId))
                return true;

            if (currentTarget.RadiusDistance <= 2f)
                return true;

            return currentTarget.IsInLineOfSight || currentTarget.IsWalkable;
        }

        public bool TryCastAvoidancePower()
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
                            return true;
                        }
                    }
                    LastActionTimes.Add(DateTime.UtcNow);
                }
            }
            return false;
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

            // Wait until NTh action happend more than than half the measure time ago          
            var timeSince = DateTime.UtcNow.Subtract(actionLimitTime).TotalMilliseconds;
            if (timeSince < measureTimeMs/2)
            {
                Logger.LogDebug(LogCategory.Behavior, "Throttling - Actions Per Second Limit Reached! {0} actions were taken within {1}ms", actionLimit, timeSince);
                //Logger.Warn($"Throttling - Actions Per Second Limit Reached! {actionLimit} actions were taken within {timeSince}ms");               
                runStatus = RunStatus.Running;
                return true;
            }

            runStatus = default(RunStatus);
            return false;
        }

        public bool TrySpecialMovement()
        {
            if (!CombatBase.IsInCombat && !Core.Settings.Combat.Misc.AllowOOCMovement)
            {
                return false;
            }

            using (new PerformanceLogger("HandleTarget.TrySpecialMovement"))
            {
                if (ClassMover.SpecialMovement(Trinity.TrinityPlugin.CurrentDestination))
                {
                    // Try to ensure the bot isn't navigating to somewhere behind us.
                    Navigator.Clear();
                    Trinity.TrinityPlugin.LastMoveToTarget = Trinity.TrinityPlugin.CurrentDestination;
                    return true;
                }
            }
            return false;
        }

        public void HandleTargetInRange()
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

                        //if (!ZetaDia.Me.IsFacing(CombatBase.CurrentTarget.Position))
                        //{
                        //    Logger.LogVerbose($"Turning to look at target {CombatBase.CurrentTarget.Name}");
                        //    ZetaDia.Me.UsePower(SNOPower.Walk, MathEx.CalculatePointFrom(ZetaDia.Me.Position, CombatBase.CurrentTarget.Position, 1f), ZetaDia.WorldId);
                        //    break;
                        //}

                        if (IsWaitingForPower && CombatBase.CurrentPower.ShouldWaitBeforeUse)
                        {
                        }
                        else if (IsWaitingForPower && !CombatBase.CurrentPower.ShouldWaitBeforeUse)
                        {
                            IsWaitingForPower = false;
                        }
                        else
                        {
                            IsWaitingForPower = false;
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
                    var isTwoSlot = true;
                    if (item != null && item.IsValid)
                    {
                        isTwoSlot = item.IsTwoSquareItem;
                    }

                    var validLocation = TrinityItemManager.FindValidBackpackLocation(isTwoSlot);
                    if (validLocation.X < 0 || validLocation.Y < 0)
                    {
                        Logger.Log("No more space to pickup item, town-run requested at next free moment. (HandleTarget)");
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
                                SpellHistory.RecordSpell(new TrinityPower
                                {
                                    SNOPower = SNOPower.Axe_Operate_Gizmo,
                                    TargetAcdId = Trinity.TrinityPlugin.CurrentTarget.AcdId,
                                    MinimumRange = TargetRangeRequired,
                                    TargetPosition = Trinity.TrinityPlugin.CurrentTarget.Position
                                });
                            }
                            else
                            {
                                CombatManager.TargetHandler.LastActionTimes.Add(DateTime.UtcNow);
                            }
                        }
                        else
                        {
                            Logger.LogNormal("Interacting with {0}", Trinity.TrinityPlugin.CurrentTarget.InternalName);

                            if (!Trinity.TrinityPlugin.CurrentTarget.IsUsed && ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, Trinity.TrinityPlugin.CurrentTarget.AcdId))
                            {
                                SpellHistory.RecordSpell(new TrinityPower
                                {
                                    SNOPower = SNOPower.Axe_Operate_Gizmo,
                                    TargetAcdId = Trinity.TrinityPlugin.CurrentTarget.AcdId,
                                    MinimumRange = TargetRangeRequired,
                                    TargetPosition = Trinity.TrinityPlugin.CurrentTarget.Position
                                });
                            }
                            else
                            {
                                Trinity.TrinityPlugin.CurrentTarget.Interact();
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
                                Trinity.TrinityPlugin.CurrentTarget.InternalName, // 0
                                Trinity.TrinityPlugin.CurrentTarget.ActorSnoId, // 1
                                Trinity.TrinityPlugin.CurrentTarget.Distance, // 2
                                TargetRangeRequired, // 3
                                Trinity.TrinityPlugin.CurrentTarget.Radius, // 4
                                Trinity.TrinityPlugin.CurrentTarget.Type, // 5
                                CombatBase.CurrentPower.SNOPower // 6 
                                );
                        }
                        else
                        {
                            Logger.Log(TrinityLogLevel.Verbose, LogCategory.Behavior,
                                "Destructible: Name={0}. SNO={1}, Range={2}. Needed range={3}. Radius={4}. Type={5}. Using power={6}",
                                Trinity.TrinityPlugin.CurrentTarget.InternalName, // 0
                                Trinity.TrinityPlugin.CurrentTarget.ActorSnoId, // 1
                                TargetCurrentRadiusDistance, // 2
                                TargetRangeRequired, // 3 
                                Trinity.TrinityPlugin.CurrentTarget.Radius, // 4
                                Trinity.TrinityPlugin.CurrentTarget.Type, // 5
                                CombatBase.CurrentPower.SNOPower // 6
                                );
                        }

                        var vAttackPoint = Trinity.TrinityPlugin.CurrentTarget.Position;
                        vAttackPoint = MathEx.CalculatePointFrom(Core.Player.Position, Trinity.TrinityPlugin.CurrentTarget.Position, Trinity.TrinityPlugin.CurrentTarget.CollisionRadius);
                        vAttackPoint.Z += 1.5f;

                        var destructiblePower = CombatBase.CurrentPower;

                        Logger.LogVerbose($"Attacking Destructable Power={destructiblePower}");                            
                        Core.PlayerMover.MoveTowards(CombatBase.CurrentTarget.Position);

                        // Note: UsePower result cannot be trusted - it will sometimes return true when the power wasn't actually cast.

                        // try with acdId
                        if (ZetaDia.Me.UsePower(destructiblePower.SNOPower, vAttackPoint, ZetaDia.WorldId, Trinity.TrinityPlugin.CurrentTarget.AcdId))
                        {
                            SpellHistory.RecordSpell(destructiblePower.SNOPower);
                            LastActionTimes.Add(DateTime.UtcNow);
                        }

                        // try position
                        if (ZetaDia.Me.UsePower(destructiblePower.SNOPower, vAttackPoint, ZetaDia.WorldId))
                        {
                            SpellHistory.RecordSpell(destructiblePower.SNOPower);
                            LastActionTimes.Add(DateTime.UtcNow);
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

                        // Prevent this EXACT object being targetted again for a short while, just incase
                        Trinity.TrinityPlugin._ignoreRactorGuid = Trinity.TrinityPlugin.CurrentTarget.RActorId;
                        Trinity.TrinityPlugin._ignoreTargetForLoops = 3;
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

        public void AssignPower()
        {
            // todo, refactor this completely.

            using (new PerformanceLogger("HandleTarget.AssignMonsterTargetPower"))
            {
                if (CombatBase.CurrentTarget.IsDestroyable)
                {
                    var power = CombatManager.AbilitySelector.SelectAbility(UseDestructiblePower: true);
                    if (power == null || NonCombatPowers.Contains(power.SNOPower))
                        power = CombatBase.DefaultPower;

                    CombatBase.CurrentPower = power;
                    Logger.LogDebug($"Current Target is Destroyable, selected destructible power : {power}");
                    return;
                }

                if (CombatBase.CurrentPower.TimeSinceAssignedMs > 500)
                {
                    Trinity.TrinityPlugin._shouldPickNewAbilities = true;
                }

                // Find a valid ability if the target is a monster
                if (Trinity.TrinityPlugin._shouldPickNewAbilities && !IsWaitingForPower && !IsWaitingForPotion && !IsWaitingBeforePower)
                {
                    Trinity.TrinityPlugin._shouldPickNewAbilities = false;
                    if (Trinity.TrinityPlugin.CurrentTarget.IsUnit || Trinity.TrinityPlugin.CurrentTarget.IsDestroyable)
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

                    if (CombatBase.CurrentPower == null || CombatBase.CurrentPower.SNOPower == SNOPower.None)
                    {
                        Trinity.TrinityPlugin._shouldPickNewAbilities = true;
                        IsWaitingForPower = false;
                        IsWaitingBeforePower = false;
                    }

                    return;
                }

                if (!IsWaitingForPower && CombatBase.CurrentPower == null)
                {
                    CombatBase.CurrentPower = CombatManager.AbilitySelector.SelectAbility(UseOOCBuff: true);
                }
            }
        }

        public bool ShouldUsePotion()
        {
            if (Core.Player.CurrentHealthPct > CombatBase.EmergencyHealthPotionLimit)
                return false;

            if (Core.Player.IsIncapacitated || !(Core.Player.CurrentHealthPct > 0) || Core.Player.IsInTown)
                return false;

            if (SpellHistory.TimeSinceUse(SNOPower.DrinkHealthPotion) <= TimeSpan.FromSeconds(30))
                return false;

            return Core.Player.CurrentHealthPct <= CombatBase.EmergencyHealthPotionLimit || ShouldSnapshot();
        }

        public bool UsePotion()
        {
            var logEntry = ShouldSnapshot() ? "Using Potion to Snapshot Bane of the Stricken!" : "Using Potion";

            var legendaryPotions = Core.Inventory.Backpack.Where(i => i.InternalName.ToLower().Contains("healthpotion_legendary_")).ToList();
            if (legendaryPotions.Any())
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, logEntry, 0);
                var dynamicId = legendaryPotions.FirstOrDefault().AnnId;
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
        ///     Checks if we should be snapshotting Bane of the Stricken
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

        public double GetSecondsSinceTargetUpdate()
        {
            return DateTime.UtcNow.Subtract(LastPickedTargetTime).TotalSeconds;
        }

        public void HandleTargetBasicMovement(Vector3 destination)
        {
            PlayerMover.NavigateTo(destination, CombatBase.CurrentTarget.Name);

            //using (new PerformanceLogger("HandleTarget.HandleBasicMovement"))
            //{
            //    // Now for the actual movement request stuff
            //    Trinity.TrinityPlugin.IsAlreadyMoving = true;
            //    Trinity.TrinityPlugin.lastMovementCommand = DateTime.UtcNow;

            //    Core.Player.CurrentAction = PlayerAction.Moving;

            //    if (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.lastSentMovePower).TotalMilliseconds >= 250 || Vector3.Distance(Trinity.TrinityPlugin.LastMoveToTarget, destination) >= 2f || bForceNewMovement)
            //    {
            //        if (Trinity.TrinityPlugin.CurrentTarget.IsSafeSpot)
            //            Logger.Log(LogCategory.Avoidance, $"Moving to SafeSpot Distance={Trinity.TrinityPlugin.CurrentTarget.Distance}");

            //        var distance = destination.Distance(Core.Player.Position);
            //        var straightLinePathing = distance <= 10f && !DataDictionary.StraightLinePathingLevelAreaIds.Contains(Core.Player.LevelAreaId) && !PlayerMover.IsBlocked && !Navigator.StuckHandler.IsStuck && Core.Grids.CanRayWalk(ZetaDia.Me.Position, destination); //&& NavHelper.CanRayCast(CurrentDestination)

            //        var destname = string.Format("{0} {1:0} yds Elite={2} LoS={3} HP={4:0.00} Dir={5}",
            //            Trinity.TrinityPlugin.CurrentTarget.InternalName,
            //            Trinity.TrinityPlugin.CurrentTarget.Distance,
            //            Trinity.TrinityPlugin.CurrentTarget.IsElite,
            //            Trinity.TrinityPlugin.CurrentTarget.HasBeenInLoS,
            //            Trinity.TrinityPlugin.CurrentTarget.HitPointsPct,
            //            MathUtil.GetHeadingToPoint(Trinity.TrinityPlugin.CurrentTarget.Position));

            //        MoveResult lastMoveResult;
            //        if (straightLinePathing)
            //        {
            //            lastMoveResult = MoveResult.Moved;
            //            // just "Click" 
            //            Navigator.PlayerMover.MoveTowards(destination);
            //            Logger.LogVerbose(LogCategory.Movement, "MoveTowards Straight line pathing to {0}", destname);
            //        }
            //        else
            //        {
            //            Logger.LogVerbose(LogCategory.Movement, "NavigateTo Straight line pathing to {0}", destname);
            //            lastMoveResult = PlayerMover.NavigateTo(destination, destname);
            //        }

            //        Trinity.TrinityPlugin.lastSentMovePower = DateTime.UtcNow;

            //        // Store the current destination for comparison incase of changes next loop
            //        Trinity.TrinityPlugin.LastMoveToTarget = destination;

            //        // Reset total body-block count, since we should have moved
            //        if (DateTime.UtcNow.Subtract(Trinity.TrinityPlugin._lastForcedKeepCloseRange).TotalMilliseconds >= 2000)
            //            Trinity.TrinityPlugin._timesBlockedMoving = 0;
            //    }
            //}
        }

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
                            HandleTargetBasicMovement(questTarget.Position);
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
                var targetPosition = Vector3.Zero;
                var targetAcdId = -1;

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
                        Logger.LogVerbose(LogCategory.Behavior, "{0} Using  CurrentTarget Position/ACD", CombatBase.CurrentPower.SNOPower);
                        targetPosition = Trinity.TrinityPlugin.CurrentTarget.Position;
                        targetAcdId = Trinity.TrinityPlugin.CurrentTarget.AcdId;
                    }

                    if (isTargetPosition && !isAcdId)
                    {
                        Logger.LogVerbose(LogCategory.Behavior, "{0} Using target position only.", CombatBase.CurrentPower.SNOPower);
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

                //// See if we should force a long wait BEFORE casting
                //Trinity.TrinityPlugin._isWaitingBeforePower = CombatBase.CurrentPower.ShouldWaitBeforeUse;
                //if (Trinity.TrinityPlugin._isWaitingBeforePower)
                //{
                //    Logger.LogVerbose("Starting wait before use {0} ms", CombatBase.CurrentPower.WaitBeforeUseDelay);
                //    return;
                //}


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
                            IsWaitingForAttackToFinish = true;

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
                var target = GetTargetName();


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
                        Logger.Log("Used Power {0} ({1}) {2} Range={3} ({4} {5}) Delay={6}/{7} TargetDist={8} CurrentTarget={9} charges={10}",
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
                    IsWaitingAfterPower = CombatBase.CurrentPower.ShouldWaitAfterUse;

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

        public string GetTargetName()
        {
            float dist = 0;
            if (CombatBase.CurrentPower.TargetPosition != Vector3.Zero)
                dist = CombatBase.CurrentPower.TargetPosition.Distance(Core.Player.Position);
            else if (Trinity.TrinityPlugin.CurrentTarget != null)
                dist = Trinity.TrinityPlugin.CurrentTarget.Position.Distance(Core.Player.Position);

            var name = Trinity.TrinityPlugin.CurrentTarget != null && Trinity.TrinityPlugin.CurrentTarget.AcdId == CombatBase.CurrentPower.TargetAcdId ? Trinity.TrinityPlugin.CurrentTarget.InternalName : String.Empty;

            var target = CombatBase.CurrentPower.TargetPosition != Vector3.Zero ? "at " + NavHelper.PrettyPrintVector3(CombatBase.CurrentPower.TargetPosition) + " dist=" + (int) dist : "";
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

                var itemSha1Hash = HashGenerator.GenerateItemHash(item.Position, item.ActorSnoId, item.InternalName, Trinity.TrinityPlugin.CurrentWorldDynamicId, item.ItemQualityLevel, item.ItemLevel);
                if (!ItemDropStats._hashsetItemPicksLookedAt.Contains(itemSha1Hash))
                {
                    ItemDropStats._hashsetItemPicksLookedAt.Add(itemSha1Hash);
                    var itemType = TrinityItemManager.DetermineItemType(item.InternalName, item.ItemType, item.FollowerType);
                    var itemBaseType = TrinityItemManager.DetermineBaseType(itemType);
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
                        var iGemType = 0;
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
    }
}