#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Combat.Abilities;
using Trinity.Helpers;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

#endregion

namespace Trinity.DbProvider
{
    /// <summary>
    /// When a portal spell is detected on player (waypoint map or town portal), 
    /// this task (run in TreeStart hook), enables 'Kill All' combat mode in trinity with a reduced attack radius. 
    /// When all monsters nearby are dead, It will move back to the portal cast position and allow it to cast.
    /// Upon world change it will revert combat settings back to previous.
    /// </summary>
    internal class ClearArea
    {
        private static DateTime MoveToPortalPositionStartTime = DateTime.MinValue;
        private static bool _previousExtendedTrashKill;
        private static int _previousNonEliteRange;
        private static int _previousEliteRange;
        private static int _previousTrashPackSize;
        private static float _previousTrashClusterRadius;
        private static bool _isCasting;
        
        static ClearArea()
        {
            // Make sure the settings are reverted always.
            GameEvents.OnWorldChanged += (s, e) =>
            {
                Logger.Log("World Changed. IsCombatModeOverridden={0}", IsCombatModeOverridden);
                Reset();
                RevertClearAreaMode();
            };

           // GameEvents.OnWorldTransferStart += (s, e) => RevertClearAreaMode();
            BotMain.OnShutdownRequested += (s, e) => RevertClearAreaMode();
            BotMain.OnStop += ibot => RevertClearAreaMode();

            ClearAreaMeleeDistance = 25f;
            ClearAreaRangedDistance = 45f;
            ClearAreaTrashPackSize = 1;
            ClearAreaTrashClusterRadius = 12f;
            LastMoveBackAttemptTime = DateTime.UtcNow;            
            ExtendRangeLimit = 40;
        }

        /// <summary>
        /// Maximum attack range extension that can be added from failed attempts;
        /// </summary>
        public static int ExtendRangeLimit { get; set; }

        public static DateTime LastMoveBackAttemptTime { get; set; }
        public static bool ShouldMoveToPortalPosition { get; set; }
        public static Vector3 PreTownRunPosition { get; set; }
        public static bool IsCombatModeOverridden { get; set; }
        public static float ClearAreaRangedDistance { get; set; }
        public static float ClearAreaMeleeDistance { get; set; }
        public static bool LastClearingResult { get; set; }
        public static int ClearAreaTrashPackSize { get; set; }
        public static float ClearAreaTrashClusterRadius { get; set; }
        public static int CurrentAttempt { get; set; }

        public static List<TrinityCacheObject> MeleeMonsters
        {
            get { return Trinity.ObjectCache.Where(u => u.IsUnit && u.MonsterSize != MonsterSize.Ranged && u.Distance < ClearAreaMeleeDistance).ToList(); }
        }

        public static List<TrinityCacheObject> ImportantLoot
        {
            get { return Trinity.ObjectCache.Where(u => u.TrinityItemType != TrinityItemType.Unknown && u.ItemQuality >= Zeta.Game.Internals.Actors.ItemQuality.Legendary && u.Distance <= 40f).ToList(); }
        }

        public static List<TrinityCacheObject> SummonAndRangedMonsters
        {
            get { return Trinity.ObjectCache.Where(u => u.Distance < ClearAreaRangedDistance && (u.IsSummoner || u.MonsterSize == MonsterSize.Ranged)).ToList(); }
        }

        public static int WeightedMonsterCount
        {
            get { return MeleeMonsters.Count(u => u.Weight > 0) + SummonAndRangedMonsters.Count(u => u.Weight > 0); }
        }

        public static void Enable()
        {
            // Hooks are not run during DBs coroutines like using the waypoint map.
            // So we need to check for the casting of a town portal spell using pulse.
            Pulsator.OnPulse += PulseClearAreaCheck;
            BotMain.OnStart += BotMain_OnStart;
        }

        public static void Disable()
        {
            Pulsator.OnPulse -= PulseClearAreaCheck;
            BotMain.OnStart -= BotMain_OnStart;
        }

        private static bool HookAdded;
        private static void AddHooks()
        {            
            // Just shove it in every hook lol.
            TreeHooks.Instance.InsertHook("VendorRun", 0, new ActionRunCoroutine(ret => MoveToPortalPositionTask()));
            TreeHooks.Instance.InsertHook("BotBehavior", 0, new ActionRunCoroutine(ret => MoveToPortalPositionTask()));
            TreeHooks.Instance.InsertHook("Combat", 0, new ActionRunCoroutine(ret => MoveToPortalPositionTask()));
            HookAdded = true;
        }

        private static void BotMain_OnStart(IBot bot)
        {
            HookAdded = false;
        }

        public static void Reset()
        {
            MoveToPortalPositionStartTime = DateTime.MinValue;
            ShouldMoveToPortalPosition = false;
            PreTownRunPosition = Vector3.Zero;
            AttemptOfLastCombatSettingsChange = 0;
        }

        /// <summary>
        /// Checks if player is trying to cast a town portal spell
        /// And if there are monsters nearby, triggers a clearing of the area.
        /// </summary>
        public static void PulseClearAreaCheck(object sender, EventArgs e)
        {
            //// How this should interact with new town run is TBD
            //if (Trinity.Settings.Advanced.UseExperimentalTownRun)
            //{
            //    Reset();
            //    RevertClearAreaMode();
            //    return;
            //}

            if (!HookAdded)
                AddHooks();            

            var secondsSinceWorldChange = DateTime.UtcNow.Subtract(Trinity.LastWorldChangeTime).TotalSeconds;
            var secondsSinceLevelAreaChange = DateTime.UtcNow.Subtract(Trinity.Player.LastChangedLevelAreaId).TotalSeconds;
            if (secondsSinceWorldChange < 10 || secondsSinceLevelAreaChange < 10 || ZetaDia.IsInTown || ZetaDia.Me == null || ZetaDia.Me.IsDead)
            {
                Reset();
                RevertClearAreaMode();
                CurrentAttempt = 0;
                return;
            }

            var lastClearingResult = LastClearingResult;
            var shouldClearArea = ShouldClearArea();

            // Attempt to run back to position every X seconds if we get too far away, will take a while for monsters to follow us.
            if (PreTownRunPosition != Vector3.Zero)
            {
                if (Trinity.Player.IsLoadingWorld || ZetaDia.IsInTown || ZetaDia.Me.IsDead)
                {
                    Reset();
                    return;
                }

                var lastReturnAttemptSecs = DateTime.UtcNow.Subtract(LastMoveBackAttemptTime).TotalSeconds;
                var distanceToPortalPosition = PreTownRunPosition.Distance(ZetaDia.Me.Position);
                var isMonstersReallyClose = MeleeMonsters.Any(m => m.Distance < 10f);
                var shouldTownRun = TownRun.TownRunCanRun();
                if (lastReturnAttemptSecs > 10 && distanceToPortalPosition > 60f && !isMonstersReallyClose && !ImportantLoot.Any() && shouldTownRun)
                {
                    Logger.Log("Too far away from  PreTownRunPosition, attempting to return. LastReturnAttempt={0}s Distance={1}",
                        lastReturnAttemptSecs, distanceToPortalPosition);

                    ShouldMoveToPortalPosition = true;
                }
            }

            if (shouldClearArea)
            {
                SetClearAreaMode();    
                
                if (PreTownRunPosition == Vector3.Zero && !ZetaDia.IsInTown)
                {
                    Logger.Log("Setting Portal Position to {0} (ClearAreaCheck)", Trinity.Player.Position, Trinity.Player.WorldID);
                    PreTownRunPosition = ZetaDia.Me.Position;
                }      
            }
            else if (lastClearingResult)
            {
                ShouldMoveToPortalPosition = true;
            }
            else
            {
                CheckIsCasting();                    
            }
        }

        private static void CheckIsCasting()
        {
            if (Trinity.Player.IsCasting)
            {
                if (_isCasting) return;                
                _isCasting = true;
                CurrentAttempt++;
                Logger.Log("Attempting a portal spell ... Attempt={0}", CurrentAttempt);
                UpdateKillRanges();
            }
            else
            {
                _isCasting = false;
            }
        }

        /// <summary>
        /// Moves back to the position when town run was initiated.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> MoveToPortalPositionTask()
        {
            try
            {
                if (!ShouldMoveToPortalPosition || !Trinity.Player.IsInGame)
                {
                    return false;
                }

                LastMoveBackAttemptTime = DateTime.UtcNow;

                if (ZetaDia.IsInTown)
                {
                    Logger.LogVerbose(LogCategory.Behavior, "Arrived in Town");
                    Reset();
                    return false;
                }

                if (PreTownRunPosition == Vector3.Zero)
                {
                    Logger.LogVerbose(LogCategory.Behavior, "Bad pre-town run position / Vector3.Zero");
                    Reset();
                    return false;
                }


                if (PreTownRunPosition.Distance(ZetaDia.Me.Position) > 600)
                {
                    Logger.LogVerbose(LogCategory.Behavior, "Portal position is now too far away.");
                    Reset();
                    return false;
                }

                Logger.Log("Moving back to to portal position. Distance={0}", PreTownRunPosition.Distance(ZetaDia.Me.Position));                

                if (MoveToPortalPositionStartTime == DateTime.MinValue)
                    MoveToPortalPositionStartTime = DateTime.UtcNow;

                float distance;

                var lastMovedTime = DateTime.UtcNow;

                while ((distance = PreTownRunPosition.Distance(ZetaDia.Me.Position)) >= 6f)
                {
                    if (PlayerMover.MovementSpeed < 0.5)
                    {
                        if (DateTime.UtcNow.Subtract(lastMovedTime).TotalSeconds > 2 && CombatBase.CurrentTarget == null)
                        {
                            Logger.Log("Moving Stopped due to Stuck");
                            Reset();
                            return false;
                        }
                    }
                    else
                    {
                        lastMovedTime = DateTime.UtcNow;
                    }

                    if (PlayerMover.IsBlocked || DateTime.UtcNow.Subtract(MoveToPortalPositionStartTime).TotalSeconds > 45 || ZetaDia.IsInTown || Navigator.StuckHandler.IsStuck || Trinity.Player.IsDead)
                    {
                        Logger.Log("Moving Stopped due to Timeout or Blocked or InTown");
                        Reset();
                        return false;
                    }

                    if (!ZetaDia.Me.IsDead)
                    {
                        Logger.Log("Moving to {0} at {1} Distance={2}", "Portal Position", PreTownRunPosition, distance);
                        PlayerMover.NavigateTo(PreTownRunPosition, "Portal Position");
                    }

                    await Coroutine.Yield();
                }

                if (PreTownRunPosition.Distance(ZetaDia.Me.Position) <= 8f)
                {
                    Logger.Log("Arrived at PreTownRunPosition");

                    if (!BrainBehavior.IsVendoring)
                    {
                        // Waypoint/Random Portal, revert everything in case bot doesnt want ant to portal anymore.
                        RevertClearAreaMode();
                        ShouldMoveToPortalPosition = false;
                        PreTownRunPosition = Vector3.Zero;
                    }
                    else
                    {
                        // Town Run, wait until we arrive in town to revert everything.
                        //MoveToPortalPositionStartTime = DateTime.MinValue;
                        ShouldMoveToPortalPosition = false;
                    }

                }

                Logger.Log("MoveToPortalPositionTask Finished!");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("MoveToPortalPositionTask Exception {0}", ex);

                if (ex is CoroutineStoppedException)
                    throw;
            }

            return false;
        }

        /// <summary>
        /// Determines if we should be clearing the area around us
        /// </summary>
        public static bool ShouldClearArea()
        {
            if (Legendary.HomingPads.IsEquipped)
            {
                return false;
            }

            if (Trinity.Player.IsInTown || Legendary.HomingPads.IsEquipped || CacheData.BuffsCache.Instance.HasInvulnerableShrine)
            {
                LastClearingResult = false;
                return false;
            }

            if ((Trinity.Player.IsCastingPortal || BrainBehavior.IsVendoring) && (MeleeMonsters.Count > 0 || SummonAndRangedMonsters.Count > 0))
            {
                //Logger.LogVerbose("Can't use portal with monsters nearby. Clearing Area! Within{3}={0} Ranged/SummonWithin{4}={1} WeightedWithin{4}={2}",
                //    MeleeMonsters.Count, SummonAndRangedMonsters.Count, WeightedMonsterCount, ClearAreaMeleeDistance, ClearAreaRangedDistance);

                LastClearingResult = true;
                return true;
            }

            if (IsCombatModeOverridden && (MeleeMonsters.Count > 0 || SummonAndRangedMonsters.Count > 0))
            {
                //Logger.LogVerbose("Currently Clearing Area! Within{3}={0} Ranged/SummonWithin{4}={1} WeightedWithin{4}={2}",
                //    MeleeMonsters.Count, SummonAndRangedMonsters.Count, WeightedMonsterCount, ClearAreaMeleeDistance, ClearAreaRangedDistance);

                LastClearingResult = true;
                return true;
            }

            LastClearingResult = false;
            return false;
        }

        /// <summary>
        /// Adjusts Trinity combat settings to kill everything in a small radius around player.
        /// </summary>
        public static void SetClearAreaMode()
        {
            if (CombatBase.CombatMode != CombatMode.KillAll)
            {
                var baseRange = SummonAndRangedMonsters.Count > 0 ? ClearAreaRangedDistance : ClearAreaMeleeDistance;
                var finalRange = baseRange + Math.Min(CurrentAttempt*5, ExtendRangeLimit);

                _previousTrashPackSize = Trinity.Settings.Combat.Misc.TrashPackSize;
                _previousTrashClusterRadius = Trinity.Settings.Combat.Misc.TrashPackClusterRadius;
                _previousExtendedTrashKill = Trinity.Settings.Combat.Misc.ExtendedTrashKill;
                _previousEliteRange = Trinity.Settings.Combat.Misc.EliteRange;
                _previousNonEliteRange = Trinity.Settings.Combat.Misc.NonEliteRange;

                Logger.Log("Reducing combat range settings to: Elites={0} Trash={1} ClusterSize={2} TrashCount={3} Attempt={4}",
                    finalRange, finalRange, ClearAreaTrashClusterRadius, ClearAreaTrashPackSize, CurrentAttempt);

                Trinity.Settings.Combat.Misc.TrashPackSize = ClearAreaTrashPackSize;
                Trinity.Settings.Combat.Misc.TrashPackClusterRadius = ClearAreaTrashClusterRadius;
                Trinity.Settings.Combat.Misc.ExtendedTrashKill = false;
                Trinity.Settings.Combat.Misc.EliteRange = (int) finalRange;
                Trinity.Settings.Combat.Misc.NonEliteRange = (int) finalRange;

                AttemptOfLastCombatSettingsChange = CurrentAttempt;
                CombatBase.CombatMode = CombatMode.KillAll;
                IsCombatModeOverridden = true;
            }
        }

        private static void UpdateKillRanges()
        {
            // Need to update the current ranges each time we fail to cast portal.
            if (AttemptOfLastCombatSettingsChange < CurrentAttempt && IsCombatModeOverridden)
            {
                var baseRange = SummonAndRangedMonsters.Count > 0 ? ClearAreaRangedDistance : ClearAreaMeleeDistance;
                var finalRange = baseRange + Math.Min(CurrentAttempt*5, ExtendRangeLimit);

                Trinity.Settings.Combat.Misc.EliteRange = (int) finalRange;
                Trinity.Settings.Combat.Misc.NonEliteRange = (int) finalRange;

                Logger.Log("Changing Kill Ranges to Elites={0} Trash={1} Attempt={2}",
                    finalRange, finalRange, CurrentAttempt);

                AttemptOfLastCombatSettingsChange = CurrentAttempt;
            }
        }

        public static int AttemptOfLastCombatSettingsChange { get; set; }

        /// <summary>
        /// Reverts Trinity combat settings to what they were previously.
        /// </summary>
        public static void RevertClearAreaMode()
        {
            if (IsCombatModeOverridden && CombatBase.CombatMode != CombatBase.LastCombatMode)
            {
                Trinity.Settings.Combat.Misc.TrashPackSize = _previousTrashPackSize;
                Trinity.Settings.Combat.Misc.TrashPackClusterRadius = _previousTrashClusterRadius;
                Trinity.Settings.Combat.Misc.ExtendedTrashKill = _previousExtendedTrashKill;
                Trinity.Settings.Combat.Misc.EliteRange = _previousEliteRange;
                Trinity.Settings.Combat.Misc.NonEliteRange = _previousNonEliteRange;

                Logger.Log("Restoring previous combat range settings to: Elites={0} Trash={1} ClusterSize={2} TrashCount={3}", 
                    _previousEliteRange, _previousNonEliteRange, _previousTrashClusterRadius, _previousTrashPackSize);

                CombatBase.CombatMode = CombatBase.LastCombatMode;
                IsCombatModeOverridden = false;                
            }
            Reset();
        }

    }
}