using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Combat.Abilities;
using Trinity.Config.Combat;
using Trinity.Framework;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Utilities;
using Trinity.Movement;
using Trinity.Reference;
using Trinity.Technicals;
using Trinity.UI.UIComponents.RadarCanvas;
using Zeta.Bot;
using Zeta.Bot.Dungeons;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Common;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.DbProvider
{
    // Player Mover Class
    public class PlayerMover : IPlayerMover
    {
        static PlayerMover()
        {
            Pulsator.OnPulse += Pulsator_OnPulse; ;
        }

        public bool MovementRequested { get; set; }

        private static void Pulsator_OnPulse(object sender, EventArgs e)
        {
            //if (BotMain.IsRunning && Navigator.SearchGridProvider != null && Navigator.SearchGridProvider.Width == 0 || Navigator.SearchGridProvider.SearchArea.Count == 0)
            //    Logger.Log("Waiting for Navigation Server...");

            GetIsBlocked();

            try
            {
                if (ZetaDia.Me == null || !ZetaDia.Me.IsValid || ZetaDia.Me.CommonData == null || !ZetaDia.Me.CommonData.IsValid)
                    return;

                if (ZetaDia.IsLoadingWorld || ZetaDia.Me.Movement == null || !ZetaDia.Me.Movement.IsValid)
                    return;

                if (ZetaDia.Me.Movement.IsMoving)
                    _timeLastMoved = DateTime.UtcNow;
            }
            catch (Exception)
            {

            }

        }

        public PlayerMover()
        {
            _instance = this;
        }

        private static PlayerMover _instance;
        public static PlayerMover Instance => _instance ?? (_instance = new PlayerMover());

        #region IsBlocked

        private static readonly Stopwatch BlockedTimer = new Stopwatch();
        private static readonly Stopwatch BlockedCheckTimer = new Stopwatch();

        private const int TimeToBlockMs = 1000;
        private const int TimeToCheckBlockingMs = 25;
        public static bool IsBlocked = false;

        internal static bool GetIsBlocked()
        {
            if (ZetaDia.Me == null || !ZetaDia.Me.IsValid || ZetaDia.Me.IsDead)
                return false;

            if (Legendary.IllusoryBoots.IsEquipped)
                return false;

            if (!BlockedCheckTimer.IsRunning)
                BlockedCheckTimer.Start();

            if (BlockedCheckTimer.ElapsedMilliseconds < TimeToCheckBlockingMs)
                return IsBlocked;

            BlockedCheckTimer.Restart();

            var moveSpeed = GetMovementSpeed();
            if (moveSpeed > 2)
            {
                Logger.LogVerbose(LogCategory.Movement, "Not blocked because moving fast! CurrentSpeed={0}", moveSpeed);
                BlockedTimer.Stop();
                BlockedTimer.Reset();
                IsBlocked = false;
                return IsBlocked;
            }

            //if (ZetaDia.Me.Movement != null && ZetaDia.Me.Movement.IsValid && ZetaDia.Me.Movement.SpeedXY > 0.8)
            //{
            //    IsBlocked = false;
            //    BlockedTimer.Stop();
            //    BlockedTimer.Reset();
            //    return false;
            //}                

            var testObjects = TrinityPlugin.ObjectCache.Where(o => !o.IsMe && ((o.IsTrashMob || o.IsBossOrEliteRareUnique || o.IsMinion) && o.HitPoints > 0) && o.Distance <= 12f).ToList();

            //testObjects.ForEach(o => Logger.Log("testObject: {0}",o.InternalName));

            //if (testObjects.Count < 3)
            //{
            //    IsBlocked = false;
            //    BlockedTimer.Stop();
            //    BlockedTimer.Reset();
            //    return false;
            //}

            //if (testObjects.Count > 10)
            //{
            //    IsBlocked = true;
            //    return false;
            //}

            var surrounded = false;
            var testPoints = MathUtil.GetCirclePoints(8, 10f, ZetaDia.Me.Position).Where(p => NavHelper.CanRayCast(p) && TrinityPlugin.MainGridProvider.CanStandAt(p)).ToList();
            var halfPoints = Math.Round(testPoints.Count * 0.60, 0, MidpointRounding.AwayFromZero);
            var blockedPoints = testPoints.Count(p => testObjects.Any(o => MathUtil.PositionIsInCircle(p, o.Position, o.Radius / 2)));
            if (blockedPoints > halfPoints)
            {
                Logger.LogVerbose(LogCategory.Movement, "Surrounded BlockedPoints={0} Required={1} TotalPoints={2}", blockedPoints, halfPoints, testPoints.Count);
                surrounded = true;
            }

            //var pointInFacingDirection0 = MathEx.GetPointAt(TrinityPlugin.Player.Position, 10f, TrinityPlugin.Player.Rotation);

            //var pointInFacingDirectionPlus60 = MathEx.GetPointAt(TrinityPlugin.Player.Position, 15f, TrinityPlugin.Player.Rotation + (float)(Math.PI / 3));

            //var pointInFacingDirecitonMinus60 = MathEx.GetPointAt(TrinityPlugin.Player.Position, 15f, TrinityPlugin.Player.Rotation + (float)(Math.PI / 3));

            //RadarDebug.Draw(new List<Vector3>
            //{
            //    pointInFacingDirection0,
            //    pointInFacingDirectionPlus60,
            //    pointInFacingDirecitonMinus60,
            //});

            var blocked = false;
            if (CurrentTarget != null && CurrentTarget.Distance > 10f)
            {
                //testPoints.RemoveAll(p => !MathUtil.PositionIsInsideArc(p, TrinityPlugin.Player.Position, 12f, TrinityPlugin.Player.Rotation, 65f));
                //testObjects.RemoveAll(obj => !MathUtil.PositionIsInsideArc(obj.Position, TrinityPlugin.Player.Position, 12f, TrinityPlugin.Player.Rotation, 65f));
                //RadarDebug.Draw(testObjects.Select(o => o.Position), 50);
                //RadarDebug.Draw(testPoints, 50, RadarDebug.DrawType.Elipse, RadarDebug.DrawColor.Green);
                //blocked = testPoints.All(p => testObjects.Any(o => MathUtil.PositionIsInCircle(p, o.Position, o.Radius / 2)));

                var pointInFacingDirection0 = MathEx.GetPointAt(TrinityPlugin.Player.Position, 8f, TrinityPlugin.Player.Rotation);

                var numMonstersInFront = (from u in TrinityPlugin.ObjectCache
                                          where !u.IsMe && u.IsUnit && MathUtil.IntersectsPath(u.Position, u.CollisionRadius, TrinityPlugin.Player.Position, pointInFacingDirection0)
                                          select u).Count();

                blocked = numMonstersInFront > 0;

                //blocked = TargetUtil.TargetsInFrontOfMe(10).Count > 2;
            }


            //var pathBlocked = testObjects.

            if (BlockedTimer.IsRunning && (surrounded || blocked) && BlockedTimer.ElapsedMilliseconds > TimeToBlockMs)
            {
                Logger.LogVerbose(LogCategory.Movement, "IsBlocked! Timer={0}ms TestObjects={1} TestPoints={2}", BlockedTimer.ElapsedMilliseconds, testObjects.Count, testPoints.Count());
                IsBlocked = true;
                return IsBlocked;
            }

            if (BlockedTimer.IsRunning && !(surrounded || blocked))
            {
                Logger.LogVerbose(LogCategory.Movement, "No Longer Blocked!");
                BlockedTimer.Stop();
                BlockedTimer.Reset();
                IsBlocked = false;
                return IsBlocked;
            }

            if ((surrounded || blocked))
            {
                if (!BlockedTimer.IsRunning)
                    BlockedTimer.Restart();

                Logger.LogVerbose(LogCategory.Movement, "Probably Blocked - Timer={0}ms TestObjects={1}", BlockedTimer.ElapsedMilliseconds, testObjects.Count());
            }

            return IsBlocked;
        }

        //public static float Wrap(float value, float lower, float upper)
        //{
        //    float distance = upper - lower;
        //    float times = (float)System.Math.Floor((value - lower) / distance);

        //    return value - (times * distance);
        //}

        public static long BlockedTimeMs
        {
            get { return BlockedTimer.ElapsedMilliseconds; }
        }

        #endregion

        private static readonly HashSet<int> BasicMovementOnlyIDs = new HashSet<int> { 138989, 176074, 176076, 176077, 176536, 260330, 330695, 330696, 330697, 330698, 330699 };
        // 138989 = health pool, 176074 = protection, 176076 = fortune, 176077 = frenzied, 176536 = portal in leorics, 260330 = cooldown shrine, 330695 to 330699 = pylons
        // Exp shrines = ???? Other shrines ????


        private static bool ShrinesInArea(Vector3 targetpos)
        {
            return TrinityPlugin.ObjectCache.Any(o => BasicMovementOnlyIDs.Contains(o.ActorSNO) && Vector3.Distance(o.Position, targetpos) <= 50f);
        }

        private static readonly DateTime LastUsedMoveStop = DateTime.MinValue;
        public void MoveStop()
        {
            if (DateTime.UtcNow.Subtract(LastUsedMoveStop).TotalMilliseconds < 250)
                return;

            ZetaDia.Me.UsePower(SNOPower.Walk, ZetaDia.Me.Position, ZetaDia.WorldId);
        }

        // Anti-stuck variables
        internal static Vector3 LastMoveToTarget = Vector3.Zero;
        internal static int TimesReachedStuckPoint = 0;
        internal static int TotalAntiStuckAttempts = 1;
        internal static Vector3 vSafeMovementLocation = Vector3.Zero;
        internal static DateTime TimeLastRecordedPosition = DateTime.MinValue;
        internal static Vector3 LastPosition = Vector3.Zero;
        internal static DateTime LastGeneratedStuckPosition = DateTime.MinValue;
        internal static int TimesReachedMaxUnstucks = 0;
        internal static DateTime LastCancelledUnstucker = DateTime.MinValue;
        internal static DateTime LastRecordedAnyStuck = DateTime.MinValue;
        internal static int CancelUnstuckerForSeconds = 60;
        internal static DateTime LastRestartedGame = DateTime.MinValue;
        internal static bool UnStuckCheckerLastResult = false;
        internal static DateTime TimeLastUsedPlayerMover = DateTime.MinValue;

        internal static Vector3 LastTempestRushPosition = Vector3.Zero;

        // Store player current position
        public static Vector3 MyPosition { get { return ZetaDia.Me.Position; } }

        //For Tempest Rush Monks
        private static bool CanChannelTempestRush;


        private const int UnstuckCheckDelay = 5000;

        /// <summary>
        /// Check if we are stuck or not by simply checking for position changing max once every 3 seconds
        /// </summary>
        /// <returns>True if we are stuck</returns>
        public static bool UnstuckChecker()
        {
            var myPosition = ZetaDia.Me.Position;

            // Never stuck if movement disabled
            if (TrinityPlugin.Settings.Advanced.DisableAllMovement)
            {
                return false;
            }

            // Keep checking distance changes every 3 seconds
            if (DateTime.UtcNow.Subtract(TimeLastRecordedPosition).TotalMilliseconds < UnstuckCheckDelay)
                return UnStuckCheckerLastResult;

            ProfileBehavior currentProfileBehavior = null;
            try
            {
                if (ProfileManager.CurrentProfileBehavior != null)
                    currentProfileBehavior = ProfileManager.CurrentProfileBehavior;
            }
            catch (Exception ex)
            {
                Logger.Log(LogCategory.UserInformation, "Exception while checking for current profile behavior!");
                Logger.Log(LogCategory.GlobalHandler, ex.ToString());
            }
            if (currentProfileBehavior != null)
            {
                Type profileBehaviortype = currentProfileBehavior.GetType();
                string behaviorName = profileBehaviortype.Name;
                if (profileBehaviortype == typeof(UseTownPortalTag) ||
                     profileBehaviortype == typeof(WaitTimerTag) ||
                     behaviorName.ToLower().Contains("townrun") ||
                     behaviorName.ToLower().Contains("townportal") ||
                     behaviorName.ToLower().Contains("leave") ||
                     behaviorName.ToLower().Contains("wait"))
                {
                    TimeLastRecordedPosition = DateTime.UtcNow;
                    UnStuckCheckerLastResult = false;
                    SpeedSensors.Clear();
                    return UnStuckCheckerLastResult;
                }
            }

            if (ZetaDia.IsInTown && (UIElements.VendorWindow.IsVisible || UIElements.SalvageWindow.IsVisible))
            {
                TimeLastRecordedPosition = DateTime.UtcNow;
                UnStuckCheckerLastResult = false;
                SpeedSensors.Clear();
                return UnStuckCheckerLastResult;
            }

            // We're not stuck if we're doing stuff!
            var animationEndTime = 0;
            try
            {
                animationEndTime = ZetaDia.Me.LoopingAnimationEndTime;
            }
            catch (Exception ex)
            {
                if (ex is CoroutineStoppedException)
                    throw;
            }

            if (animationEndTime > 0 || ZetaDia.Me.IsInConversation || ZetaDia.IsPlayingCutscene || ZetaDia.IsLoadingWorld)
            {
                LastPosition = Vector3.Zero;
                TimeLastRecordedPosition = DateTime.UtcNow;
                UnStuckCheckerLastResult = false;
                SpeedSensors.Clear();
                return UnStuckCheckerLastResult;
            }

            if (DateTime.UtcNow.Subtract(_timeLastMoved).TotalSeconds < 10)
                return false;

            if (LastPosition != Vector3.Zero && LastPosition.Distance(myPosition) <= 2f && GetMovementSpeed() < 1)
            {
                TimeLastRecordedPosition = DateTime.MinValue;
                UnStuckCheckerLastResult = true;
                return UnStuckCheckerLastResult;
            }

            TimeLastRecordedPosition = DateTime.UtcNow;
            LastPosition = myPosition;

            UnStuckCheckerLastResult = false;
            return UnStuckCheckerLastResult;
        }

        private static DateTime _timeLastMoved = DateTime.MinValue;

        public static Vector3 UnstuckHandler()
        {
            return UnstuckHandler(MyPosition, LastMoveToTarget);
        }
        // Actually deal with a stuck - find an unstuck point etc.
        public static Vector3 UnstuckHandler(Vector3 vMyCurrentPosition, Vector3 vOriginalDestination)
        {
            if (TrinityPlugin.Settings.Advanced.DisableAllMovement)
                return Vector3.Zero;

            // Update the last time we generated a path
            LastGeneratedStuckPosition = DateTime.UtcNow;
            Navigator.Clear();

            // If we got stuck on a 2nd/3rd/4th "chained" anti-stuck route, then return the old move to target to keep movement of some kind going
            if (TimesReachedStuckPoint > 0)
            {
                vSafeMovementLocation = Vector3.Zero;

                // Reset the path and allow a whole "New" unstuck generation next cycle
                TimesReachedStuckPoint = 0;
                // And cancel unstucking for 9 seconds so DB can try to navigate
                CancelUnstuckerForSeconds = (9 * TotalAntiStuckAttempts);
                if (CancelUnstuckerForSeconds < 20)
                    CancelUnstuckerForSeconds = 20;
                LastCancelledUnstucker = DateTime.UtcNow;
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "Clearing old route and trying new path find to: " + LastMoveToTarget.ToString());
                NavigateTo(LastMoveToTarget, "original destination");
                return vSafeMovementLocation;
            }
            // Only try an unstuck 10 times maximum in XXX period of time
            if (Vector3.Distance(vOriginalDestination, vMyCurrentPosition) >= 1500)
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "You are " + Vector3.Distance(vOriginalDestination, vMyCurrentPosition).ToString() + " distance away from your destination.");
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "This is too far for the unstucker, and is likely a sign of ending up in the wrong map zone.");
                TotalAntiStuckAttempts = 20;
            }

            if (TotalAntiStuckAttempts <= 10)
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Your bot got stuck! Trying to unstuck (attempt #{0} of 10 attempts) {1} {2} {3} {4}",
                    TotalAntiStuckAttempts.ToString(),
                    "Act=\"" + ZetaDia.CurrentAct + "\"",
                    "questId=\"" + ZetaDia.CurrentQuest.QuestSnoId + "\"",
                    "stepId=\"" + ZetaDia.CurrentQuest.StepId + "\"",
                    "worldId=\"" + ZetaDia.CurrentWorldSnoId + "\""
                );

                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "(destination=" + vOriginalDestination.ToString() + ", which is " + Vector3.Distance(vOriginalDestination, vMyCurrentPosition).ToString() + " distance away)");

                /*
                 * Unstucker position
                 */
                //vSafeMovementLocation = NavHelper.FindSafeZone(true, TotalAntiStuckAttempts, vMyCurrentPosition);
                vSafeMovementLocation = NavHelper.SimpleUnstucker();

                // Temporarily log stuff
                if (TotalAntiStuckAttempts == 1 && TrinityPlugin.Settings.Advanced.LogStuckLocation)
                {
                    FileStream LogStream = File.Open(Path.Combine(FileManager.LoggingPath, "Stucks - " + TrinityPlugin.Player.ActorClass.ToString() + ".log"), FileMode.Append, FileAccess.Write, FileShare.Read);
                    using (StreamWriter LogWriter = new StreamWriter(LogStream))
                    {
                        LogWriter.WriteLine(DateTime.UtcNow.ToString() + ": Original Destination=" + LastMoveToTarget.ToString() + ". Current player position when stuck=" + vMyCurrentPosition.ToString());
                        LogWriter.WriteLine("Profile Name=" + ProfileManager.CurrentProfile.Name);
                    }
                    LogStream.Close();
                }
                // Now count up our stuck attempt generations
                TotalAntiStuckAttempts++;
                return vSafeMovementLocation;
            }

            TimesReachedMaxUnstucks++;
            TotalAntiStuckAttempts = 1;
            vSafeMovementLocation = Vector3.Zero;
            LastPosition = Vector3.Zero;
            TimesReachedStuckPoint = 0;
            TimeLastRecordedPosition = DateTime.MinValue;
            LastGeneratedStuckPosition = DateTime.MinValue;
            // int iSafetyLoops = 0;
            if (TimesReachedMaxUnstucks == 1)
            {
                Navigator.Clear();
                GridSegmentation.Reset();
                Logger.Log(TrinityLogLevel.Info, LogCategory.Movement, "Anti-stuck measures now attempting to kickstart DB's path-finder into action.");
                var result = NavigateTo(vOriginalDestination, "original destination");
                //Navigator.MoveTo(vOriginalDestination, "original destination");
                CancelUnstuckerForSeconds = 40;
                LastCancelledUnstucker = DateTime.UtcNow;
                return vSafeMovementLocation;
            }
            if (TimesReachedMaxUnstucks == 2)
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.Movement, "Anti-stuck measures failed. Now attempting to reload current profile.");

                Navigator.Clear();

                ProfileManager.Load(Zeta.Bot.ProfileManager.CurrentProfile.Path);
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Anti-stuck successfully reloaded current profile, DemonBuddy now navigating again.");
                return vSafeMovementLocation;

                // Didn't make it to town, so skip instantly to the exit game system
                //iTimesReachedMaxUnstucks = 3;
            }
            // Exit the game and reload the profile
            if (TrinityPlugin.Settings.Advanced.AllowRestartGame && DateTime.UtcNow.Subtract(LastRestartedGame).TotalMinutes >= 5)
            {
                LastRestartedGame = DateTime.UtcNow;
                string sUseProfile = TrinityPlugin.FirstProfile;
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Anti-stuck measures exiting current game.");
                // Load the first profile seen last run
                ProfileManager.Load(!string.IsNullOrEmpty(sUseProfile)
                                        ? sUseProfile
                                        : Zeta.Bot.ProfileManager.CurrentProfile.Path);
                Thread.Sleep(1000);
                TrinityPlugin.ResetEverythingNewGame();
                ZetaDia.Service.Party.LeaveGame(true);
                // Wait for 10 second log out timer if not in town
                if (!ZetaDia.IsInTown)
                {
                    Thread.Sleep(15000);
                }
            }
            else
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Unstucking measures failed. Now stopping TrinityPlugin unstucker for 12 minutes to inactivity timers to kick in or DB to auto-fix.");
                CancelUnstuckerForSeconds = 720;
                LastCancelledUnstucker = DateTime.UtcNow;
                return vSafeMovementLocation;
            }
            return vSafeMovementLocation;
        }
        // Handle moveto requests from the current routine/profile
        // This replaces DemonBuddy's own built-in "Basic movement handler" with a custom one
        private static Vector3 vLastMoveTo = Vector3.Zero;
        private static bool bLastWaypointWasTown = false;
        private static HashSet<Vector3> hashDoneThisVector = new HashSet<Vector3>();
        private static Vector3 vShiftedPosition = Vector3.Zero;
        private static DateTime lastShiftedPosition = DateTime.MinValue;

        private static Vector3 lastMovementPosition = Vector3.Zero;
        private static DateTime lastRecordedPosition = DateTime.UtcNow;

        internal static double MovementSpeed { get { return GetMovementSpeed(); } }

        internal static List<SpeedSensor> SpeedSensors = new List<SpeedSensor>();
        private static int MaxSpeedSensors = 5;

        public static double GetMovementSpeed()
        {
            // Just changed worlds, Clean up the stack
            if (SpeedSensors.Any(s => s.WorldID != TrinityPlugin.CurrentWorldDynamicId))
            {
                SpeedSensors.Clear();
                return 1d;
            }

            // record speed once per second
            if (DateTime.UtcNow.Subtract(lastRecordedPosition).TotalMilliseconds >= 1000)
            {
                // Record our current location and time
                if (!SpeedSensors.Any())
                {
                    SpeedSensors.Add(new SpeedSensor()
                    {
                        Location = MyPosition,
                        TimeSinceLastMove = new TimeSpan(0),
                        Distance = 0f,
                        WorldID = TrinityPlugin.CurrentWorldDynamicId
                    });
                }
                else
                {
                    SpeedSensor lastSensor = SpeedSensors.OrderByDescending(s => s.Timestamp).FirstOrDefault();
                    SpeedSensors.Add(new SpeedSensor()
                    {
                        Location = MyPosition,
                        TimeSinceLastMove = new TimeSpan(DateTime.UtcNow.Subtract(lastSensor.TimeSinceLastMove).Ticks),
                        Distance = Vector3.Distance(MyPosition, lastSensor.Location),
                        WorldID = TrinityPlugin.CurrentWorldDynamicId
                    });
                }

                lastRecordedPosition = DateTime.UtcNow;
            }

            // If we just used a spell, we "moved"
            if (DateTime.UtcNow.Subtract(TrinityPlugin.lastGlobalCooldownUse).TotalMilliseconds <= 1000)
                return 1d;

            if (DateTime.UtcNow.Subtract(TrinityPlugin.lastHadUnitInSights).TotalMilliseconds <= 1000)
                return 1d;

            if (DateTime.UtcNow.Subtract(TrinityPlugin.lastHadEliteUnitInSights).TotalMilliseconds <= 1000)
                return 1d;

            if (DateTime.UtcNow.Subtract(TrinityPlugin.lastHadContainerInSights).TotalMilliseconds <= 1000)
                return 1d;

            // Minimum of 2 records to calculate speed
            if (!SpeedSensors.Any() || SpeedSensors.Count <= 1)
                return 0d;

            // If we haven't "moved" in over a second, then we're standing still
            if (DateTime.UtcNow.Subtract(TimeLastUsedPlayerMover).TotalMilliseconds > 1000)
                return 0d;

            // Check if we have enough recorded positions, remove one if so
            while (SpeedSensors.Count > MaxSpeedSensors - 1)
            {
                // first sensors
                SpeedSensors.Remove(SpeedSensors.OrderBy(s => s.Timestamp).FirstOrDefault());
            }

            double AverageRecordingTime = SpeedSensors.Average(s => s.TimeSinceLastMove.TotalHours); ;
            double averageMovementSpeed = SpeedSensors.Average(s => Vector3.Distance(s.Location, MyPosition) * 1000000);

            return averageMovementSpeed / AverageRecordingTime;
        }

        /// <summary>
        /// Returns true if there's a blocking UIElement that we should NOT be moving!
        /// </summary>
        /// <returns></returns>
        public bool UISafetyCheck()
        {
            if (ElementIsVisible(UIElements.ConfirmationDialog))
                return true;
            if (ElementIsVisible(UIElements.ConfirmationDialogCancelButton))
                return true;
            if (ElementIsVisible(UIElements.ConfirmationDialogOkButton))
                return true;
            if (ElementIsVisible(UIElements.ReviveAtLastCheckpointButton))
                return true;

            return false;
        }

        private bool ElementIsVisible(UIElement element)
        {
            if (element == null)
                return false;
            if (!UIElement.IsValidElement(element.Hash))
                return false;
            if (!element.IsValid)
                return false;
            if (!element.IsVisible)
                return false;

            return true;
        }

        /// <summary>
        /// Returns true if there's a blocking UIElement that we should NOT be moving!
        /// </summary>
        /// <returns></returns>
        public bool UiSafetyCheck()
        {
            return ElementIsVisible(UIElements.ConfirmationDialog) ||
                   ElementIsVisible(UIElements.ConfirmationDialogCancelButton) ||
                   ElementIsVisible(UIElements.ConfirmationDialogOkButton) ||
                   ElementIsVisible(UIElements.ReviveAtLastCheckpointButton);
        }

        public void MoveTowards(Vector3 destination)
        {
            if (TrinityPlugin.Settings.Advanced.DisableAllMovement)
                return;

            TrinityPlugin.NavServerReport(true);

            if (!ZetaDia.IsInGame || !ZetaDia.Me.IsValid || ZetaDia.Me.IsDead || ZetaDia.IsLoadingWorld)
            {
                return;
            }

            if (UiSafetyCheck())
            {
                return;
            }

            TimeLastUsedPlayerMover = DateTime.UtcNow;
            LastMoveToTarget = destination;
            destination = WarnAndLogLongPath(destination);

            // Store distance to current moveto target
            float destinationDistance = MyPosition.Distance(destination);

            if (!ZetaDia.IsInTown && ClassMover.IsSpecialMovementReady && !TrinityPlugin.ShouldWaitForLootDrop &&
                (IsBlocked && TrinityPlugin.Settings.Combat.Misc.AllowOOCMovement ||
                CombatBase.IsCurrentlyAvoiding || ClassMover.OutOfCombatMovementAllowed))
            {
                if (NavigationProvider == null)
                    NavigationProvider = Navigator.GetNavigationProviderAs<DefaultNavigationProvider>();

                if (ClassMover.SpecialMovement(destination) && destinationDistance > 7)
                {
                    Navigator.Clear();
                    NavigationProvider.CurrentPath.Clear();
                    AbortCurrentNavigation = true;
                    return;
                }
            }
            if (destinationDistance > 5f)
            {
                // Default movement
                ZetaDia.Me.UsePower(SNOPower.Walk, destination, TrinityPlugin.CurrentWorldDynamicId, -1);

                if (TrinityPlugin.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "PlayerMover Moved to:{0} dir:{1} Speed:{2:0.00} Dist:{3:0} ZDiff:{4:0} CanStand:{5} Raycast:{6}",
                        NavHelper.PrettyPrintVector3(destination), MathUtil.GetHeadingToPoint(destination), MovementSpeed, MyPosition.Distance(destination),
                        Math.Abs(MyPosition.Z - destination.Z),
                        TrinityPlugin.MainGridProvider.CanStandAt(TrinityPlugin.MainGridProvider.WorldToGrid(destination.ToVector2())),
                        !Navigator.Raycast(MyPosition, destination));

            }
            else
            {
                if (TrinityPlugin.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Reached MoveTowards Destination {0} Current Speed: {1:0.0}", destination, MovementSpeed);
            }

            //Trinity.IsMoveRequested = false;
        }

        public static Vector3 GetCurrentPathFarthestPoint(float minDistance, float maxDistance)
        {

            var remaining = GetCurrentPathPointsRemaining();

            if (!remaining.Any())
                return Vector3.Zero;

            var points =
                NavigationProvider.CurrentPath.Where(
                    x =>
                        remaining.Contains(x) && NavHelper.CanRayCast(x) &&
                        x.Distance(CacheData.Player.Position) <= maxDistance &&
                        x.Distance(CacheData.Player.Position) >= minDistance && 
                        !Core.Avoidance.Grid.IsLocationInFlags(x, AvoidanceFlags.CriticalAvoidance))
                    .OrderByDescending(y => y.Distance(CacheData.Player.Position))
                    .ToList();
            //Add some redundancy to find a spot that isn't ray cast
            if (!points.Any())
            {
                points =
                NavigationProvider.CurrentPath.Where(
                    x =>
                        remaining.Contains(x) && //NavHelper.CanRayCast(x) &&
                        x.Distance(CacheData.Player.Position) <= maxDistance &&
                        x.Distance(CacheData.Player.Position) >= minDistance)
                    .OrderByDescending(y => y.Distance(CacheData.Player.Position))
                    .ToList();
            }

            return points.Any() ? points.FirstOrDefault() : Vector3.Zero;
        }

        public static List<Vector3> GetCurrentPathPointsRemaining()
        {
            var points = new IndexedList<Vector3>();

            if (!NavigationProvider.CurrentPath.Any())
                return points;

            var current = NavigationProvider.CurrentPath.CurrentOrDefault;

            if (current == Vector3.Zero)
                return points;

            var found = false;
            foreach (var point in NavigationProvider.CurrentPath)
            {
                if (!found)
                {
                    if (point != current)
                        continue;

                    found = true;
                    points.Add(point);
                    continue;
                }
                points.Add(point);
            }
            return points;
        }

        public static bool AbortCurrentNavigation { get; set; }


        private static Vector3 WarnAndLogLongPath(Vector3 vMoveToTarget)
        {
            // The below code is to help profile/routine makers avoid waypoints with a long distance between them.
            // Long-distances between waypoints is bad - it increases stucks, and forces the DB nav-server to be called.
            if (TrinityPlugin.Settings.Advanced.LogStuckLocation)
            {
                if (vLastMoveTo == Vector3.Zero)
                    vLastMoveTo = vMoveToTarget;
                if (vMoveToTarget != vLastMoveTo)
                {
                    float fDistance = Vector3.Distance(vMoveToTarget, vLastMoveTo);
                    // Log if not in town, last waypoint wasn't FROM town, and the distance is >200 but <2000 (cos 2000+ probably means we changed map zones!)
                    if (!TrinityPlugin.Player.IsInTown && !bLastWaypointWasTown && fDistance >= 200 & fDistance <= 2000)
                    {
                        if (!hashDoneThisVector.Contains(vMoveToTarget))
                        {
                            // Log it
                            FileStream LogStream = File.Open(Path.Combine(FileManager.LoggingPath, "LongPaths - " + ZetaDia.Me.ActorClass.ToString() + ".log"), FileMode.Append, FileAccess.Write, FileShare.Read);
                            using (StreamWriter LogWriter = new StreamWriter(LogStream))
                            {
                                LogWriter.WriteLine(DateTime.UtcNow.ToString() + ":");
                                LogWriter.WriteLine("Profile Name=" + ProfileManager.CurrentProfile.Name);
                                LogWriter.WriteLine("'From' Waypoint=" + vLastMoveTo.ToString() + ". 'To' Waypoint=" + vMoveToTarget.ToString() + ". Distance=" + fDistance.ToString());
                            }
                            LogStream.Close();
                            hashDoneThisVector.Add(vMoveToTarget);
                        }
                    }
                    vLastMoveTo = vMoveToTarget;
                    bLastWaypointWasTown = false;
                    if (TrinityPlugin.Player.IsInTown)
                        bLastWaypointWasTown = true;
                }
            }
            return vMoveToTarget;
        }


        private static TrinityCacheObject CurrentTarget { get { return TrinityPlugin.CurrentTarget; } }

        internal static async Task<MoveResult> NavigateToTask(Vector3 destination, string destinationName = "")
        {
            PositionCache.AddPosition();
            MoveResult result;
            try
            {
                Stopwatch t1 = new Stopwatch();
                t1.Start();

                using (new PerformanceLogger("Navigator.MoveTo"))
                {
                    result = await Navigator.MoveTo(destination, destinationName);
                }
                t1.Stop();

                const float maxTime = 750;

                // Shit was slow, make it slower but tell us why :)
                string pathCheck = "";
                if (TrinityPlugin.Settings.Advanced.LogCategories.HasFlag(LogCategory.Navigator) && t1.ElapsedMilliseconds > maxTime)
                {
                    //if (Navigator.GetNavigationProviderAs<DefaultNavigationProvider>().CanFullyClientPathTo(destination))
                    //    pathCheck = "CanFullyPath";
                    //else                   
                    pathCheck = "CannotFullyPath";
                }

                LogCategory lc;
                TrinityLogLevel ll;
                if (t1.ElapsedMilliseconds > maxTime)
                {
                    lc = LogCategory.UserInformation;
                    ll = TrinityLogLevel.Info;
                }
                else
                {
                    lc = LogCategory.Navigator;
                    ll = TrinityLogLevel.Debug;
                }
                Logger.Log(ll, lc, "{0} in {1:0}ms {2} dist={3:0} {4}",
                    result, t1.ElapsedMilliseconds, destinationName, destination.Distance(TrinityPlugin.Player.Position), pathCheck);
            }
            catch (OutOfMemoryException)
            {
                Logger.LogDebug("Navigator ran out of memory!");
                return MoveResult.Failed;
            }
            catch (Exception ex)
            {
                Logger.Log("{0}", ex);
                return MoveResult.Failed;
            }
            return result;
        }

        private static Vector3 LastDestination { get; set; }
        public static DefaultNavigationProvider NavigationProvider { get; set; }

        private static Coroutine _navigateToCoroutine;
        private static MoveResult _lastResult;
        public static MoveResult NavigateTo(Vector3 destination, string destinationName = "")
        {
            if (_navigateToCoroutine == null || _navigateToCoroutine.IsFinished)
            {
                TrinityPlugin.NavServerReport();

                if (!ZetaDia.Me.Movement.IsMoving && LastDestination != destination && ZetaDia.IsInGame)
                {
                    Logger.LogVerbose(LogCategory.Movement, "NavigateTo: Starting Movement Towards {0} ({1})", destination, destinationName);
                    Instance.MoveTowards(destination);
                }
                _navigateToCoroutine = new Coroutine(async () =>
                {
                    _lastResult = await NavigateToTask(destination, destinationName);
                    return _lastResult;
                });
            }

            LastDestination = destination;
            _navigateToCoroutine.Resume();


            if (_navigateToCoroutine.Status == CoroutineStatus.RanToCompletion)
            {
                return (MoveResult)_navigateToCoroutine.Result;
            }

            TrinityPlugin.NavServerReport(false, _lastResult);
            return MoveResult.Moved;
        }

        //private static Coroutine _navigateToCoroutine;
        //private static MoveResult _lastResult;
        //public static MoveResult NavigateTo(Vector3 destination, string destinationName = "")
        //{
        //    if (_navigateToCoroutine == null || _navigateToCoroutine.IsFinished)
        //    {
        //        TrinityPlugin.NavServerReport();

        //        if (!ZetaDia.Me.Movement.IsMoving && LastDestination != destination && ZetaDia.IsInGame)
        //        {
        //            Logger.LogVerbose(LogCategory.Movement, "NavigateTo: Starting Movement Towards {0} ({1})", destination, destinationName);
        //            Instance.MoveTowards(destination);
        //        }

        //        _navigateToCoroutine = new Coroutine(async () =>
        //        {
        //            _lastResult = await NavigateToTask(destination, destinationName);
        //            return _lastResult;
        //        });
        //    }

        //    LastDestination = destination;
        //    _navigateToCoroutine.Resume();


        //    if (_navigateToCoroutine.Status == CoroutineStatus.RanToCompletion)
        //    {
        //        return (MoveResult)_navigateToCoroutine.Result;
        //    }

        //    TrinityPlugin.NavServerReport(false, _lastResult);
        //    return MoveResult.Moved;
        //}


        private static DateTime lastRecordedSkipAheadCache = DateTime.MinValue;
        internal static void RecordSkipAheadCachePoint()
        {
            if (DateTime.UtcNow.Subtract(lastRecordedSkipAheadCache).TotalMilliseconds < 100)
                return;

            lastRecordedSkipAheadCache = DateTime.UtcNow;

            if (!TrinityPlugin.SkipAheadAreaCache.Any(p => p.Position.Distance(TrinityPlugin.Player.Position) <= 5f))
            {
                TrinityPlugin.SkipAheadAreaCache.Add(new CacheObstacleObject() { Position = TrinityPlugin.Player.Position, Radius = 20f });
            }
        }


    }
}

