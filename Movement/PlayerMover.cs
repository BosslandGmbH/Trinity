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
        public static PlayerMover Instance
        {
            get { return _instance ?? (_instance = new PlayerMover()); }
        }

        #region IsBlocked

        private static readonly Stopwatch BlockedTimer = new Stopwatch();
        private static readonly Stopwatch BlockedCheckTimer = new Stopwatch();

        private const int TimeToBlockMs = 1000;
        private const int TimeToCheckBlockingMs = 25;
        private static bool _isProperBlocked;


        internal static bool GetIsBlocked()
        {
            if (ZetaDia.Me == null || !ZetaDia.Me.IsValid || ZetaDia.Me.IsDead)
                return false;

            if (Legendary.IllusoryBoots.IsEquipped)
                return false;

            if (!BlockedCheckTimer.IsRunning)
                BlockedCheckTimer.Start();

            if (BlockedCheckTimer.ElapsedMilliseconds < TimeToCheckBlockingMs)
                return _isProperBlocked;

            BlockedCheckTimer.Restart();

            var moveSpeed = GetMovementSpeed();
            if (moveSpeed > 2)
            {
                Logger.LogVerbose(LogCategory.Movement, "Not blocked because moving fast! CurrentSpeed={0}", moveSpeed);
                BlockedTimer.Stop();
                BlockedTimer.Reset();
                _isProperBlocked = false;
                return _isProperBlocked;
            }

            //if (ZetaDia.Me.Movement != null && ZetaDia.Me.Movement.IsValid && ZetaDia.Me.Movement.SpeedXY > 0.8)
            //{
            //    _isProperBlocked = false;
            //    BlockedTimer.Stop();
            //    BlockedTimer.Reset();
            //    return false;
            //}                

            var testObjects = Trinity.ObjectCache.Where(o => !o.IsMe && ((o.IsTrashMob || o.IsBossOrEliteRareUnique || o.IsMinion) && o.HitPoints > 0) && o.Distance <= 12f).ToList();

            //testObjects.ForEach(o => Logger.Log("testObject: {0}",o.InternalName));

            //if (testObjects.Count < 3)
            //{
            //    _isProperBlocked = false;
            //    BlockedTimer.Stop();
            //    BlockedTimer.Reset();
            //    return false;
            //}

            //if (testObjects.Count > 10)
            //{
            //    _isProperBlocked = true;
            //    return false;
            //}

            var surrounded = false;
            var testPoints = MathUtil.GetCirclePoints(8, 10f, ZetaDia.Me.Position).Where(p => NavHelper.CanRayCast(p) && Trinity.MainGridProvider.CanStandAt(p)).ToList();
            var halfPoints = Math.Round(testPoints.Count * 0.60, 0, MidpointRounding.AwayFromZero);
            var blockedPoints = testPoints.Count(p => testObjects.Any(o => MathUtil.PositionIsInCircle(p, o.Position, o.Radius / 2)));
            if (blockedPoints > halfPoints)
            {
                Logger.LogVerbose(LogCategory.Movement, "Surrounded BlockedPoints={0} Required={1} TotalPoints={2}", blockedPoints, halfPoints, testPoints.Count);
                surrounded = true;
            }

            //var pointInFacingDirection0 = MathEx.GetPointAt(Trinity.Player.Position, 10f, Trinity.Player.Rotation);

            //var pointInFacingDirectionPlus60 = MathEx.GetPointAt(Trinity.Player.Position, 15f, Trinity.Player.Rotation + (float)(Math.PI / 3));

            //var pointInFacingDirecitonMinus60 = MathEx.GetPointAt(Trinity.Player.Position, 15f, Trinity.Player.Rotation + (float)(Math.PI / 3));

            //RadarDebug.Draw(new List<Vector3>
            //{
            //    pointInFacingDirection0,
            //    pointInFacingDirectionPlus60,
            //    pointInFacingDirecitonMinus60,
            //});

            var blocked = false;
            if (CurrentTarget != null && CurrentTarget.Distance > 10f)
            {
                //testPoints.RemoveAll(p => !MathUtil.PositionIsInsideArc(p, Trinity.Player.Position, 12f, Trinity.Player.Rotation, 65f));
                //testObjects.RemoveAll(obj => !MathUtil.PositionIsInsideArc(obj.Position, Trinity.Player.Position, 12f, Trinity.Player.Rotation, 65f));
                //RadarDebug.Draw(testObjects.Select(o => o.Position), 50);
                //RadarDebug.Draw(testPoints, 50, RadarDebug.DrawType.Elipse, RadarDebug.DrawColor.Green);
                //blocked = testPoints.All(p => testObjects.Any(o => MathUtil.PositionIsInCircle(p, o.Position, o.Radius / 2)));

                var pointInFacingDirection0 = MathEx.GetPointAt(Trinity.Player.Position, 8f, Trinity.Player.Rotation);

                var numMonstersInFront = (from u in Trinity.ObjectCache
                                          where !u.IsMe && u.IsUnit && MathUtil.IntersectsPath(u.Position, u.CollisionRadius, Trinity.Player.Position, pointInFacingDirection0)
                                          select u).Count();

                blocked = numMonstersInFront > 0;

                //blocked = TargetUtil.TargetsInFrontOfMe(10).Count > 2;
            }


            //var pathBlocked = testObjects.

            if (BlockedTimer.IsRunning && (surrounded || blocked) && BlockedTimer.ElapsedMilliseconds > TimeToBlockMs)
            {
                Logger.LogVerbose(LogCategory.Movement, "IsBlocked! Timer={0}ms TestObjects={1} TestPoints={2}", BlockedTimer.ElapsedMilliseconds, testObjects.Count, testPoints.Count());
                _isProperBlocked = true;
                return _isProperBlocked;
            }

            if (BlockedTimer.IsRunning && !(surrounded || blocked))
            {
                Logger.LogVerbose(LogCategory.Movement, "No Longer Blocked!");
                BlockedTimer.Stop();
                BlockedTimer.Reset();
                _isProperBlocked = false;
                return _isProperBlocked;
            }

            if ((surrounded || blocked))
            {
                if (!BlockedTimer.IsRunning)
                    BlockedTimer.Restart();

                Logger.LogVerbose(LogCategory.Movement, "Probably Blocked - Timer={0}ms TestObjects={1}", BlockedTimer.ElapsedMilliseconds, testObjects.Count());
            }

            return _isProperBlocked;
        }


        //public static float Wrap(float value, float lower, float upper)
        //{
        //    float distance = upper - lower;
        //    float times = (float)System.Math.Floor((value - lower) / distance);

        //    return value - (times * distance);
        //}

        public static bool IsBlocked
        {
            get { return _isProperBlocked && Trinity.Settings.Combat.Misc.AttackWhenBlocked; }
        }

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
            return Trinity.ObjectCache.Any(o => BasicMovementOnlyIDs.Contains(o.ActorSNO) && Vector3.Distance(o.Position, targetpos) <= 50f);
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
            if (Trinity.Settings.Advanced.DisableAllMovement)
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
            if (Trinity.Settings.Advanced.DisableAllMovement)
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
                if (TotalAntiStuckAttempts == 1 && Trinity.Settings.Advanced.LogStuckLocation)
                {
                    FileStream LogStream = File.Open(Path.Combine(FileManager.LoggingPath, "Stucks - " + Trinity.Player.ActorClass.ToString() + ".log"), FileMode.Append, FileAccess.Write, FileShare.Read);
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
            if (Trinity.Settings.Advanced.AllowRestartGame && DateTime.UtcNow.Subtract(LastRestartedGame).TotalMinutes >= 5)
            {
                LastRestartedGame = DateTime.UtcNow;
                string sUseProfile = Trinity.FirstProfile;
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Anti-stuck measures exiting current game.");
                // Load the first profile seen last run
                ProfileManager.Load(!string.IsNullOrEmpty(sUseProfile)
                                        ? sUseProfile
                                        : Zeta.Bot.ProfileManager.CurrentProfile.Path);
                Thread.Sleep(1000);
                Trinity.ResetEverythingNewGame();
                ZetaDia.Service.Party.LeaveGame(true);
                // Wait for 10 second log out timer if not in town
                if (!ZetaDia.IsInTown)
                {
                    Thread.Sleep(15000);
                }
            }
            else
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Unstucking measures failed. Now stopping Trinity unstucker for 12 minutes to inactivity timers to kick in or DB to auto-fix.");
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

        private static List<SpeedSensor> SpeedSensors = new List<SpeedSensor>();
        private static int MaxSpeedSensors = 5;

        public static double GetMovementSpeed()
        {
            // Just changed worlds, Clean up the stack
            if (SpeedSensors.Any(s => s.WorldID != Trinity.CurrentWorldDynamicId))
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
                        WorldID = Trinity.CurrentWorldDynamicId
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
                        WorldID = Trinity.CurrentWorldDynamicId
                    });
                }

                lastRecordedPosition = DateTime.UtcNow;
            }

            // If we just used a spell, we "moved"
            if (DateTime.UtcNow.Subtract(Trinity.lastGlobalCooldownUse).TotalMilliseconds <= 1000)
                return 1d;

            if (DateTime.UtcNow.Subtract(Trinity.lastHadUnitInSights).TotalMilliseconds <= 1000)
                return 1d;

            if (DateTime.UtcNow.Subtract(Trinity.lastHadEliteUnitInSights).TotalMilliseconds <= 1000)
                return 1d;

            if (DateTime.UtcNow.Subtract(Trinity.lastHadContainerInSights).TotalMilliseconds <= 1000)
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

        public void MoveTowards(Vector3 destination)
        {
            if (Trinity.Settings.Advanced.DisableAllMovement)
                return;

            Trinity.NavServerReport(true);

            if (!ZetaDia.IsInGame || !ZetaDia.Me.IsValid || ZetaDia.Me.IsDead || ZetaDia.IsLoadingWorld)
            {
                return;
            }

            if (UISafetyCheck())
            {
                return;
            }

            TimeLastUsedPlayerMover = DateTime.UtcNow;
            LastMoveToTarget = destination;

            // Set the public variable

            destination = WarnAndLogLongPath(destination);

            // Store player current position

            // Store distance to current moveto target
            float destinationDistance = MyPosition.Distance(destination);

            //// Do unstuckery things
            //if (Trinity.Settings.Advanced.UnstuckerEnabled)
            //{
            //    // See if we can reset the 10-limit unstuck counter, if >120 seconds since we last generated an unstuck location
            //    // this is used if we're NOT stuck...
            //    if (TotalAntiStuckAttempts > 1 && DateTime.UtcNow.Subtract(LastGeneratedStuckPosition).TotalSeconds >= 120)
            //    {
            //        TotalAntiStuckAttempts = 1;
            //        TimesReachedStuckPoint = 0;
            //        vSafeMovementLocation = Vector3.Zero;
            //        NavHelper.UsedStuckSpots = new List<GridPoint>();
            //        Logger.Log(TrinityLogLevel.Info, LogCategory.Movement, "Resetting unstuck timers", true);
            //    }

            //    // See if we need to, and can, generate unstuck actions
            //    // check if we're stuck
            //    //bool isStuck = UnstuckChecker();

            //    //if (isStuck)
            //    //{
            //    //    // Record the time we last apparently couldn't move for a brief period of time
            //    //    LastRecordedAnyStuck = DateTime.UtcNow;

            //    //    // See if there's any stuck position to try and navigate to generated by random mover
            //    //    //vSafeMovementLocation = UnstuckHandler(MyPosition, vMoveToTarget);

            //    //    if (vSafeMovementLocation == Vector3.Zero)
            //    //    {
            //    //        Logger.Log(TrinityLogLevel.Info, LogCategory.Movement, "Unable to find Unstuck point!", vSafeMovementLocation);
            //    //        return;
            //    //    }
            //    //    Logger.Log(TrinityLogLevel.Verbose, LogCategory.Movement, "SafeMovement Location set to {0}", vSafeMovementLocation);

            //    //}

            //    // See if we can clear the total unstuckattempts if we haven't been stuck in over 6 minutes.
            //    if (DateTime.UtcNow.Subtract(LastRecordedAnyStuck).TotalSeconds >= 360)
            //    {
            //        TimesReachedMaxUnstucks = 0;
            //    }
            //    // Did we have a safe point already generated (eg from last loop through), if so use it as our current location instead
            //    if (vSafeMovementLocation != Vector3.Zero)
            //    {
            //        // Set our current movement target to the safe point we generated last cycle
            //        destination = vSafeMovementLocation;
            //        destinationDistance = MyPosition.Distance(destination);
            //    }
            //    // Get distance to current destination
            //    // Remove the stuck position if it's been reached, this bit of code also creates multiple stuck-patterns in an ever increasing amount
            //    if (vSafeMovementLocation != Vector3.Zero && destinationDistance <= 3f)
            //    {
            //        vSafeMovementLocation = Vector3.Zero;
            //        TimesReachedStuckPoint++;

            //        // Do we want to immediately generate a 2nd waypoint to "chain" anti-stucks in an ever-increasing path-length?
            //        if (TimesReachedStuckPoint <= TotalAntiStuckAttempts)
            //        {
            //            vSafeMovementLocation = NavHelper.FindSafeZone(true, TotalAntiStuckAttempts, MyPosition);
            //            destination = vSafeMovementLocation;
            //        }
            //        else
            //        {
            //            if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
            //                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Clearing old route and trying new path find to: " + LastMoveToTarget.ToString());
            //            // Reset the path and allow a whole "New" unstuck generation next cycle
            //            TimesReachedStuckPoint = 0;
            //            // And cancel unstucking for 9 seconds so DB can try to navigate
            //            CancelUnstuckerForSeconds = (9 * TotalAntiStuckAttempts);
            //            if (CancelUnstuckerForSeconds < 20)
            //                CancelUnstuckerForSeconds = 20;
            //            LastCancelledUnstucker = DateTime.UtcNow;

            //            Navigator.Clear();
            //            PlayerMover.NavigateTo(LastMoveToTarget, "original destination");

            //            return;
            //        }
            //    }
            //}

            if (TargetUtil.AnyMobsInRange(40f))
            {
                // Always move with WW
                if (TryBulKathosWWMovement(destination, destinationDistance))
                    return;
            }

            // don't use special movement within 3 seconds of being stuck
            bool cancelSpecialMovementAfterStuck = DateTime.UtcNow.Subtract(LastGeneratedStuckPosition).TotalMilliseconds > 3000;

            // See if we can use abilities like leap etc. for movement out of combat, but not in town
            if (Trinity.Settings.Combat.Misc.AllowOOCMovement && !Trinity.Player.IsInTown && cancelSpecialMovementAfterStuck && (!CombatBase.IsInCombat || !CombatBase.IsCombatAllowed || IsBlocked))
            {
                if (NavigationProvider == null)
                    NavigationProvider = Navigator.GetNavigationProviderAs<DefaultNavigationProvider>();

                // Whirlwind for a barb, special context only
                if (Trinity.Settings.Combat.Barbarian.SprintMode != BarbarianSprintMode.CombatOnly &&
                    CacheData.Hotbar.ActivePowers.Contains(SNOPower.Barbarian_Whirlwind) && Trinity.ObjectCache.Any(u => u.IsUnit &&
                    MathUtil.IntersectsPath(u.Position, u.Radius + 5f, Trinity.Player.Position, destination)) &&
                    Trinity.Player.PrimaryResource >= 10 && Trinity.Settings.Combat.Barbarian.WWMoveAlways)
                {
                    Skills.Barbarian.Whirlwind.Cast(destination);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Whirlwind for OOC movement, distance={0:0}", destinationDistance);
                    return;
                }

                if (Skills.Crusader.SteedCharge.CanCast() && !Trinity.Player.IsCastingOrLoading &&
                    Trinity.Settings.Combat.Crusader.SteedChargeOOC && ZetaDia.Me.Movement.SpeedXY != 0)
                {
                    Skills.Crusader.SteedCharge.Cast();
                }

                // Furious Charge movement for a barb
                if (Trinity.Settings.Combat.Barbarian.UseChargeOOC &&
                    CacheData.Hotbar.ActivePowers.Contains(SNOPower.Barbarian_FuriousCharge) &&
                    (!Sets.TheLegacyOfRaekor.IsSecondBonusActive && Skills.Barbarian.FuriousCharge.Charges > 0 || Skills.Barbarian.FuriousCharge.Charges > 1) &&
                    PowerManager.CanCast(SNOPower.Barbarian_FuriousCharge) && (destinationDistance >= 20f &&
                    !ShrinesInArea(destination) || Trinity.ObjectCache.Any(u => u.IsUnit &&
                                                                                MathUtil.IntersectsPath(u.Position,
                                                                                    u.Radius + 5f,
                                                                                    Trinity.Player.Position, destination)) ||
                    TargetUtil.TargetsInFrontOfMe(Math.Min(45, destinationDistance)).Count * 2 >
                    Skills.Barbarian.FuriousCharge.CooldownRemaining / 1000))
                {
                    Vector3 vThisTarget = destination;
                    if (destinationDistance > 35f)
                        vThisTarget = MathEx.CalculatePointFrom(destination, MyPosition, 35f);
                    ZetaDia.Me.UsePower(SNOPower.Barbarian_FuriousCharge, vThisTarget, Trinity.CurrentWorldDynamicId, -1);
                    SpellHistory.RecordSpell(SNOPower.Barbarian_FuriousCharge);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement,
                            "Using Furious Charge for OOC movement, distance={0:0}", destinationDistance);
                    return;
                }

                //Furious Charge when blocked (overrides OOC setting)
                if (Skills.Barbarian.FuriousCharge.CanCast() && Skills.Barbarian.FuriousCharge.Charges > 0 && IsBlocked)
                {
                    Vector3 vThisTarget = destination;
                    if (destinationDistance > 35f)
                        vThisTarget = MathEx.CalculatePointFrom(destination, MyPosition, 35f);
                    ZetaDia.Me.UsePower(SNOPower.Barbarian_FuriousCharge, vThisTarget, Trinity.CurrentWorldDynamicId, -1);
                    SpellHistory.RecordSpell(SNOPower.Barbarian_FuriousCharge);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement,
                            "Using Furious Charge for OOC movement, distance={0:0}", destinationDistance);
                    return;
                }


                if (TryBulKathosWWMovement(destination, destinationDistance))
                    return;

                // Whirlwind to keep Taguk's up
                if (CombatBase.CanCast(SNOPower.Barbarian_Whirlwind) && Trinity.Player.PrimaryResource > 10 && !Sets.BulKathossOath.IsFullyEquipped &&
                    !(CurrentTarget != null && CurrentTarget.Type == TrinityObjectType.Item && CurrentTarget.Distance < 10f) &&
                    Gems.Taeguk.IsEquipped && Skills.Barbarian.Whirlwind.TimeSinceUse > 2500 && Skills.Barbarian.Whirlwind.TimeSinceUse < 3000)
                {
                    Skills.Barbarian.Whirlwind.Cast(destination);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Whirlwind for OOC movement, distance={0:0}", destinationDistance);
                    return;
                }

                // Leap movement for a barb
                if (Trinity.Settings.Combat.Barbarian.UseLeapOOC && CacheData.Hotbar.ActivePowers.Contains(SNOPower.Barbarian_Leap) &&
                PowerManager.CanCast(SNOPower.Barbarian_Leap) && !ShrinesInArea(destination))
                {
                    Vector3 vThisTarget = destination;
                    if (destinationDistance > 35f)
                        vThisTarget = MathEx.CalculatePointFrom(destination, MyPosition, 35f);
                    ZetaDia.Me.UsePower(SNOPower.Barbarian_Leap, vThisTarget, Trinity.CurrentWorldDynamicId, -1);
                    SpellHistory.RecordSpell(SNOPower.Barbarian_Leap);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Leap for OOC movement, distance={0:0}", destinationDistance);
                    return;
                }

                // Boulder toss for the new Leapquake barb
                if (PowerManager.CanCast(SNOPower.X1_Barbarian_AncientSpear) && Sets.MightOfTheEarth.IsFullyEquipped &&
                    Legendary.LutSocks.IsEquipped &&
                    Skills.Barbarian.AncientSpear.TimeSinceUse > Trinity.Settings.Combat.Barbarian.AncientSpearWaitDelay &&
                    Skills.Barbarian.Leap.Cooldown > TimeSpan.FromMilliseconds(1000))
                {
                    Vector3 vThisTarget = destination;
                    if (destinationDistance > 35f)
                        vThisTarget = MathEx.CalculatePointFrom(destination, MyPosition, 35f);
                    ZetaDia.Me.UsePower(SNOPower.X1_Barbarian_AncientSpear, vThisTarget, Trinity.CurrentWorldDynamicId, -1);
                    SpellHistory.RecordSpell(SNOPower.X1_Barbarian_AncientSpear);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using AncientSpear to reset Leap cooldown, distance={0:0}", destinationDistance);
                    return;
                }

                if (DemonhunterVault(destination))
                    return;

                // DemonHunter Strafe
                if (Skills.DemonHunter.Strafe.IsActive && Trinity.Player.PrimaryResource > 12 && TargetUtil.AnyMobsInRange(30f, false) &&
                    !(CurrentTarget != null && CurrentTarget.Type == TrinityObjectType.Item && CurrentTarget.Distance < 10f) &&
                    // Don't Strafe into avoidance/monsters if we're kiting
                    (CombatBase.KiteDistance <= 0 || (CombatBase.KiteDistance > 0 &&
                     (!CacheData.TimeBoundAvoidance.Any(a => a.Position.Distance(destination) <= CombatBase.KiteDistance) ||
                     (!CacheData.TimeBoundAvoidance.Any(a => MathEx.IntersectsPath(a.Position, a.Radius, Trinity.Player.Position, destination)))))))
                {
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Strafe for OOC movement, distance={0}", destinationDistance);
                    Skills.DemonHunter.Strafe.Cast(destination);
                }

                // Strafe to keep Taguk's up
                if (CombatBase.CanCast(SNOPower.DemonHunter_Strafe) && Trinity.Player.PrimaryResource > 12 &&
                    Gems.Taeguk.IsEquipped && Skills.DemonHunter.Strafe.TimeSinceUse > 2250 && Skills.DemonHunter.Strafe.TimeSinceUse < 3000)
                {
                    Skills.DemonHunter.Strafe.Cast(destination);
                    return;
                }

                // Tempest rush for a monk
                if (CacheData.Hotbar.ActivePowers.Contains(SNOPower.Monk_TempestRush) &&
                    (Trinity.Settings.Combat.Monk.TROption == TempestRushOption.MovementOnly || Trinity.Settings.Combat.Monk.TROption == TempestRushOption.Always ||
                    (Trinity.Settings.Combat.Monk.TROption == TempestRushOption.TrashOnly && !TargetUtil.AnyElitesInRange(40f))))
                {
                    Vector3 vTargetAimPoint = destination;

                    bool canRayCastTarget = true;

                    vTargetAimPoint = TargetUtil.FindTempestRushTarget();

                    if (!CanChannelTempestRush &&
                        ((Trinity.Player.PrimaryResource >= Trinity.Settings.Combat.Monk.TR_MinSpirit &&
                        destinationDistance >= Trinity.Settings.Combat.Monk.TR_MinDist) ||
                         DateTime.UtcNow.Subtract(CacheData.AbilityLastUsed[SNOPower.Monk_TempestRush]).TotalMilliseconds <= 150) &&
                        canRayCastTarget && PowerManager.CanCast(SNOPower.Monk_TempestRush))
                    {
                        CanChannelTempestRush = true;
                    }
                    else if ((CanChannelTempestRush && (Trinity.Player.PrimaryResource < 10f)) || !canRayCastTarget)
                    {
                        CanChannelTempestRush = false;
                    }

                    double lastUse = DateTime.UtcNow.Subtract(CacheData.AbilityLastUsed[SNOPower.Monk_TempestRush]).TotalMilliseconds;

                    if (CanChannelTempestRush)
                    {
                        if (CombatBase.CanCast(SNOPower.Monk_TempestRush))
                        {
                            LastTempestRushPosition = vTargetAimPoint;

                            ZetaDia.Me.UsePower(SNOPower.Monk_TempestRush, vTargetAimPoint, Trinity.CurrentWorldDynamicId, -1);
                            SpellHistory.RecordSpell(SNOPower.Monk_TempestRush);

                            // simulate movement speed of 30
                            SpeedSensor lastSensor = SpeedSensors.OrderByDescending(s => s.Timestamp).FirstOrDefault();
                            SpeedSensors.Add(new SpeedSensor()
                            {
                                Location = MyPosition,
                                TimeSinceLastMove = new TimeSpan(0, 0, 0, 0, 1000),
                                Distance = 5f,
                                WorldID = Trinity.CurrentWorldDynamicId
                            });

                            if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                                Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Tempest Rush for OOC movement, distance={0:0} spirit={1:0} cd={2} lastUse={3:0} V3={4} vAim={5}",
                                    destinationDistance, Trinity.Player.PrimaryResource, PowerManager.CanCast(SNOPower.Monk_TempestRush), lastUse, destination, vTargetAimPoint);
                            return;
                        }
                        else
                            return;
                    }
                    else
                    {
                        if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                            Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement,
                            "Tempest rush failed!: {0:00.0} / {1} distance: {2:00.0} / {3} Raycast: {4} MS: {5:0.0} lastUse={6:0}",
                            Trinity.Player.PrimaryResource,
                            Trinity.Settings.Combat.Monk.TR_MinSpirit,
                            destinationDistance,
                            Trinity.Settings.Combat.Monk.TR_MinDist,
                            canRayCastTarget,
                            GetMovementSpeed(),
                            lastUse);

                        Trinity.MaintainTempestRush = false;
                    }

                    // Always set this from PlayerMover
                    MonkCombat.LastTempestRushLocation = vTargetAimPoint;

                }

                // Dashing Strike OOC
                if (Trinity.Player.ActorClass == ActorClass.Monk && CombatBase.CanCast(SNOPower.X1_Monk_DashingStrike) && Trinity.Settings.Combat.Monk.UseDashingStrikeOOC && destinationDistance > 15f)
                {
                    var charges = Skills.Monk.DashingStrike.Charges;

                    if (Legendary.Ingeom.IsEquipped && CurrentTarget != null &&
                        CurrentTarget.Type != TrinityObjectType.Item && charges >= 1)
                    {
                        Skills.Monk.DashingStrike.Cast(destination);
                        if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                            Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Dashing Strike for OOC movement, distance={0}", destinationDistance);
                        return;
                    }

                    //Logger.LogVerbose("OOC Dash Charges={0}", charges);
                    if (Sets.ThousandStorms.IsSecondBonusActive && !Trinity.ShouldWaitForLootDrop &&
                        ((charges > 1 && Trinity.Player.PrimaryResource >= 75) || CacheData.BuffsCache.Instance.HasCastingShrine))
                    {
                        if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                            Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Dashing Strike for OOC movement, distance={0} charges={1}", destinationDistance, Skills.Monk.DashingStrike.Charges);
                        Skills.Monk.DashingStrike.Cast(destination);
                        return;
                    }

                    if (charges >= 1 &&
                        (Trinity.Player.PrimaryResource <= 75 || !Sets.ThousandStorms.IsSecondBonusActive) && IsBlocked)
                    {
                        Skills.Monk.DashingStrike.Cast(destination);
                        if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                            Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Dashing Strike for OOC movement, distance={0}", destinationDistance);
                        return;
                    }

                    if (!Sets.ThousandStorms.IsSecondBonusActive && charges > 0 && PowerManager.CanCast(SNOPower.X1_Monk_DashingStrike) && !Trinity.ShouldWaitForLootDrop)
                    {
                        Skills.Monk.DashingStrike.Cast(destination);
                        if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                            Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Dashing Strike for OOC movement, distance={0}", destinationDistance);
                        return;
                    }
                }

                // Teleport for a wizard 
                if (CombatBase.CanCast(SNOPower.Wizard_Teleport, CombatBase.CanCastFlags.NoTimer) &&
                    (!Trinity.Settings.Combat.Wizard.TeleportUseOOCDelay || CombatBase.TimeSincePowerUse(SNOPower.Wizard_Teleport) >= Trinity.Settings.Combat.Wizard.TeleportDelay) &&
                    CombatBase.TimeSincePowerUse(SNOPower.Wizard_Teleport) > 250 && Trinity.Settings.Combat.Wizard.TeleportOOC &&
                    destinationDistance >= 10f && !ShrinesInArea(destination) && !ZetaDia.IsInTown)
                {
                    const float maxTeleportRange = 75f;

                    Vector3 vThisTarget = destination;
                    if (destinationDistance > maxTeleportRange)
                        vThisTarget = MathEx.CalculatePointFrom(destination, MyPosition, maxTeleportRange);
                    ZetaDia.Me.UsePower(SNOPower.Wizard_Teleport, vThisTarget, Trinity.CurrentWorldDynamicId, -1);
                    SpellHistory.RecordSpell(SNOPower.Wizard_Teleport);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Teleport for OOC movement, distance={0}", destinationDistance);
                    return;
                }

                // Teleport when blocked (overrides OOC setting)
                if (CombatBase.CanCast(SNOPower.Wizard_Teleport, CombatBase.CanCastFlags.NoTimer) && IsBlocked &&
                    CombatBase.TimeSincePowerUse(SNOPower.Wizard_Teleport) > 250 && destinationDistance >= 10f &&
                    !ZetaDia.IsInTown)
                {
                    const float maxTeleportRange = 75f;

                    Vector3 vThisTarget = destination;
                    if (destinationDistance > maxTeleportRange)
                        vThisTarget = MathEx.CalculatePointFrom(destination, MyPosition, maxTeleportRange);
                    ZetaDia.Me.UsePower(SNOPower.Wizard_Teleport, vThisTarget, Trinity.CurrentWorldDynamicId, -1);
                    SpellHistory.RecordSpell(SNOPower.Wizard_Teleport);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Teleport for OOC movement, distance={0}", destinationDistance);
                    return;

                }

                // Archon Teleport for a wizard 
                if (CacheData.Hotbar.ActivePowers.Contains(SNOPower.Wizard_Archon_Teleport) && destinationDistance >= 10f &&
                PowerManager.CanCast(SNOPower.Wizard_Archon_Teleport) && !ShrinesInArea(destination))
                {
                    Vector3 vThisTarget = destination;
                    if (destinationDistance > 35f)
                        vThisTarget = MathEx.CalculatePointFrom(destination, MyPosition, 35f);
                    ZetaDia.Me.UsePower(SNOPower.Wizard_Archon_Teleport, vThisTarget, Trinity.CurrentWorldDynamicId, -1);
                    SpellHistory.RecordSpell(SNOPower.Wizard_Archon_Teleport);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Archon Teleport for OOC movement, distance={0}", destinationDistance);
                    return;
                }

                // LoN Firebats WD needs to refresh Taeguk every 2 seconds or so
                if (Gems.Taeguk.IsEquipped && Sets.LegacyOfNightmares.IsFullyEquipped &&
                    Skills.WitchDoctor.Firebats.IsActive &&
                    CombatBase.TimeSincePowerUse(SNOPower.Witchdoctor_Firebats) > 2000)
                {
                    ZetaDia.Me.UsePower(SNOPower.Witchdoctor_Firebats, CombatBase.Player.Position, Trinity.CurrentWorldDynamicId, -1);
                    SpellHistory.RecordSpell(SNOPower.Witchdoctor_Firebats);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Firebats to refresh Taeguk");
                    return;
                }

                // Cold M6 DH needs to refresh Taeguk every 2 seconds or so
                if (Gems.Taeguk.IsEquipped && Sets.EmbodimentOfTheMarauder.IsFullyEquipped &&
                    Skills.DemonHunter.ClusterArrow.IsActive &&
                    CombatBase.TimeSincePowerUse(SNOPower.DemonHunter_ClusterArrow) > 2000)
                {
                    Vector3 vThisTarget = destination;
                    ZetaDia.Me.UsePower(SNOPower.DemonHunter_ClusterArrow, vThisTarget, Trinity.CurrentWorldDynamicId);
                    SpellHistory.RecordSpell(SNOPower.DemonHunter_ClusterArrow);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Cluster Arrow to refresh Taeguk");
                    return;
                }

                // Spirit Walk when blocked
                if (CombatBase.CanCast(SNOPower.Witchdoctor_SpiritWalk) && IsBlocked)
                {
                    Vector3 vThisTarget = destination;
                    if (destinationDistance > 35f)
                        vThisTarget = MathEx.CalculatePointFrom(destination, MyPosition, 35f);
                    ZetaDia.Me.UsePower(SNOPower.Witchdoctor_SpiritWalk, vThisTarget, Trinity.CurrentWorldDynamicId, -1);
                    SpellHistory.RecordSpell(SNOPower.Witchdoctor_SpiritWalk);
                    if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement,
                            "Using SpiritWalk to get unstuck, distance={0:0}", destinationDistance);
                    return;
                }

            }

            if (MyPosition.Distance(destination) > 3f)
            {
                // Default movement
                ZetaDia.Me.UsePower(SNOPower.Walk, destination, ZetaDia.WorldId, -1);

                if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "PlayerMover Moved to:{0} dir:{1} Speed:{2:0.00} Dist:{3:0} ZDiff:{4:0} CanStand:{5} Raycast:{6}",
                        NavHelper.PrettyPrintVector3(destination), MathUtil.GetHeadingToPoint(destination), MovementSpeed, MyPosition.Distance(destination),
                        Math.Abs(MyPosition.Z - destination.Z),
                        Trinity.MainGridProvider.CanStandAt(Trinity.MainGridProvider.WorldToGrid(destination.ToVector2())),
                        !Navigator.Raycast(MyPosition, destination)
                        );

            }
            else
            {
                if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Reached MoveTowards Destination {0} Current Speed: {1:0.0}", destination, MovementSpeed);
            }

            //Trinity.IsMoveRequested = false;
        }

        public static bool DemonhunterVault(Vector3 destination)
        {
            int vaultDelay = Trinity.Settings.Combat.DemonHunter.VaultMovementDelay;
            var destinationDistance = destination.Distance(ZetaDia.Me.Position);

            // DemonHunter Vault
            var vaultBaseCost = 8; //Skills.DemonHunter.Vault.Cost*(1 - Trinity.Player.ResourceCostReductionPct);
            var vaultCost = Runes.DemonHunter.Acrobatics.IsActive
                ? 0
                : Runes.DemonHunter.Tumble.IsActive && Skills.DemonHunter.Vault.TimeSinceUse < 6000
                    ? Math.Round(vaultBaseCost * 0.5)
                    : vaultBaseCost;

            if (DemonHunterCombat.CanAcquireFreeVaultBuff && !CombatBase.IsInCombat && Trinity.Player.PrimaryResourcePct > 0.7f)
            {
                Logger.LogVerbose(LogCategory.Movement, "Casting Impale for free vaults (PlayerMover)");
                Skills.DemonHunter.Impale.Cast(MathEx.GetPointAt(Trinity.Player.Position, 5f, Trinity.Player.Rotation));
            }

            var isfree = DemonHunterCombat.IsVaultFree;
            var timeSinceUse = CombatBase.TimeSincePowerUse(SNOPower.DemonHunter_Vault);
            var currentAnimation = Trinity.Player.CurrentAnimation;

            var movementOnlySettingFailed = Trinity.Settings.Combat.DemonHunter.VaultMode == DemonHunterVaultMode.MovementOnly && (CombatBase.IsInCombat || ZetaDia.Me.IsInCombat);
            var combatOnlySettingFailed = Trinity.Settings.Combat.DemonHunter.VaultMode == DemonHunterVaultMode.CombatOnly && !(CombatBase.IsInCombat || ZetaDia.Me.IsInCombat);

            if (!movementOnlySettingFailed && !combatOnlySettingFailed && CacheData.Hotbar.ActivePowers.Contains(SNOPower.DemonHunter_Vault) &&
                Trinity.Settings.Combat.DemonHunter.VaultMode != DemonHunterVaultMode.CombatOnly &&
                (timeSinceUse > vaultDelay || isfree && timeSinceUse > 250) &&
                //(timeSinceUse > vaultDelay || (isfree && !VaultAnimations.Contains(currentAnimation))) && // Going too fast seemed to upset advneturer exploration.
                (Trinity.Player.SecondaryResource >= vaultCost || isfree) &&
                destinationDistance >= 20f && destination != Vector3.Zero && Trinity.Player.Position != Vector3.Zero &&
                PowerManager.CanCast(SNOPower.DemonHunter_Vault) && !ShrinesInArea(destination) &&
                // Don't Vault into avoidance/monsters if we're kiting
                (CombatBase.KiteDistance <= 0 || (CombatBase.KiteDistance > 0 &&
                                                  (!CacheData.TimeBoundAvoidance.Any(
                                                      a => a.Position.Distance(destination) <= CombatBase.KiteDistance) ||
                                                   (!CacheData.TimeBoundAvoidance.Any(
                                                       a =>
                                                           MathEx.IntersectsPath(a.Position, a.Radius, Trinity.Player.Position,
                                                               destination))) ||
                                                   !CacheData.MonsterObstacles.Any(
                                                       a => a.Position.Distance(destination) <= CombatBase.KiteDistance))))
                )
            {
                Vector3 vThisTarget = destination;

                if ((destinationDistance > 100f || !CombatBase.IsInCombat) && NavigationProvider.CurrentPath.Count > 0)
                {
                    Logger.LogVerbose(LogCategory.Movement, "[PlayerMover] Vault using farthest current path point");
                    vThisTarget = GetCurrentPathFarthestPoint(35f);
                }

                // Double check, sometimes the point is in the opposite direction.
                // todo figure out why sometimes the point from GetCurrentPathFarthestPoint() is in the wrong direction.
                if (vThisTarget.Distance(NavigationProvider.CurrentPathDest) > MyPosition.Distance(NavigationProvider.CurrentPathDest))
                {
                    vThisTarget = destination;
                }

                if (destinationDistance > 50f)
                {
                    Logger.LogVerbose(LogCategory.Movement, "[PlayerMover] Vault using projection towards destination");
                    vThisTarget = MathEx.CalculatePointFrom(destination, MyPosition, 35f);
                }

                if (destinationDistance < 10f)
                {
                    Logger.LogVerbose(LogCategory.Movement, "[PlayerMover] Vault using Projected Position from Player/Rotation");
                    vThisTarget = MathEx.GetPointAt(ZetaDia.Me.Position, 35f, ZetaDia.Me.Movement.Rotation);
                }

                VaultDestination = vThisTarget;

                ZetaDia.Me.UsePower(SNOPower.DemonHunter_Vault, vThisTarget, Trinity.CurrentWorldDynamicId, -1);
                SpellHistory.RecordSpell(SNOPower.DemonHunter_Vault);
                Logger.LogVerbose(LogCategory.Movement, "Cast Vault from HandleTarget.UsedSpecialMovement()");
                SpellHistory.RecordSpell(SNOPower.DemonHunter_Vault);

                // Try to ensure the bot isn't navigating to somewhere behind us.
                Navigator.Clear();
                NavigationProvider.CurrentPath.Clear();
                AbortCurrentNavigation = true;

                if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Vault for OOC movement, distance={0}",
                        destinationDistance);

                return true;
            }
            return false;
        }

        public static HashSet<SNOAnim> VaultAnimations = new HashSet<SNOAnim>
        {
            SNOAnim.Demonhunter_Male_Cast_BackFlip_mid,
            SNOAnim.Demonhunter_Female_Cast_BackFlip_mid,
            SNOAnim.Demonhunter_Male_Cast_BackFlip_out,
            SNOAnim.Demonhunter_Female_Cast_BackFlip_out,
            SNOAnim.Demonhunter_Male_Cast_BackFlip_in,
            SNOAnim.Demonhunter_Female_Cast_BackFlip_in,
        };

        public static Vector3 VaultDestination { get; set; }

        //private static Vector3 _currentPathDestination;
        //public static bool IsPathChanged()
        //{
        //    var currentPathDestination = NavigationProvider.CurrentPath.CurrentOrDefault;
        //    if (currentPathDestination != _currentPathDestination)
        //    {
        //        _currentPathDestination = currentPathDestination;
        //        return true;
        //    }
        //    return false;
        //}

        public static Vector3 GetCurrentPathFarthestPoint(float maxDistance)
        {
            var playerPosition = Trinity.Player.Position;
            var remaining = GetCurrentPathPointsRemaining();
            var points = NavigationProvider.CurrentPath.Count;
            var lastPoint = Vector3.Zero;

            //Logger.LogVerbose(LogCategory.Movement, "GetCurrentPathFarthestPoint - Points: {0} CurrentIndex: {1} PointsRemaining: {2} MaxDistance: {3}",
            //points, NavigationProvider.CurrentPath.Index, remaining.Count, maxDistance);

            for (int i = 0; i < points; i++)
            {
                var point = NavigationProvider.CurrentPath[i];
                var dist = point.Distance(playerPosition);
                if (dist >= maxDistance)
                {
                    //Logger.LogVerbose(LogCategory.Movement, "PathPoint {0}: Distance: {1} > Max: {2}", i, dist, maxDistance);
                    return point;
                }

                if (!NavHelper.CanRayCast(point))
                {
                    //Logger.LogVerbose(LogCategory.Movement, "PathPoint {0}: Distance: {1} failed raycast", i, dist, maxDistance);
                    return i == 0 ? point : lastPoint;
                }

                lastPoint = point;
            }
            return remaining.LastOrDefault();
        }

        public static List<Vector3> GetCurrentPathPointsRemaining()
        {
            var points = new IndexedList<Vector3>();
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

        private static bool TryBulKathosWWMovement(Vector3 destination, float destinationDistance)
        {
            // Whirlwind for Bul-Kathos Sword Set D3 2.2.0
            if (CombatBase.CanCast(SNOPower.Barbarian_Whirlwind) && Trinity.Player.PrimaryResource > 10 && (Sets.BulKathossOath.IsFullyEquipped || Trinity.Settings.Combat.Barbarian.WWMoveAlways)
                && !(CurrentTarget != null && (CurrentTarget.Type == TrinityObjectType.Item || CurrentTarget.IsNPC || CurrentTarget.Type == TrinityObjectType.Shrine) && CurrentTarget.Distance < Trinity.Settings.Combat.Barbarian.WWMoveStopDistance))
            {
                Skills.Barbarian.Whirlwind.Cast(destination);
                if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Using Whirlwind for OOC movement, distance={0:0}", destinationDistance);
                return true;
            }
            return false;
        }


        private static Vector3 WarnAndLogLongPath(Vector3 vMoveToTarget)
        {
            // The below code is to help profile/routine makers avoid waypoints with a long distance between them.
            // Long-distances between waypoints is bad - it increases stucks, and forces the DB nav-server to be called.
            if (Trinity.Settings.Advanced.LogStuckLocation)
            {
                if (vLastMoveTo == Vector3.Zero)
                    vLastMoveTo = vMoveToTarget;
                if (vMoveToTarget != vLastMoveTo)
                {
                    float fDistance = Vector3.Distance(vMoveToTarget, vLastMoveTo);
                    // Log if not in town, last waypoint wasn't FROM town, and the distance is >200 but <2000 (cos 2000+ probably means we changed map zones!)
                    if (!Trinity.Player.IsInTown && !bLastWaypointWasTown && fDistance >= 200 & fDistance <= 2000)
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
                    if (Trinity.Player.IsInTown)
                        bLastWaypointWasTown = true;
                }
            }
            return vMoveToTarget;
        }


        private static TrinityCacheObject CurrentTarget { get { return Trinity.CurrentTarget; } }

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
                if (Trinity.Settings.Advanced.LogCategories.HasFlag(LogCategory.Navigator) && t1.ElapsedMilliseconds > maxTime)
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
                    result, t1.ElapsedMilliseconds, destinationName, destination.Distance(Trinity.Player.Position), pathCheck);
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
                Trinity.NavServerReport();

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

            Trinity.NavServerReport(false, _lastResult);
            return MoveResult.Moved;
        }

        private static DateTime lastRecordedSkipAheadCache = DateTime.MinValue;
        internal static void RecordSkipAheadCachePoint()
        {
            if (DateTime.UtcNow.Subtract(lastRecordedSkipAheadCache).TotalMilliseconds < 100)
                return;

            lastRecordedSkipAheadCache = DateTime.UtcNow;

            if (!Trinity.SkipAheadAreaCache.Any(p => p.Position.Distance(Trinity.Player.Position) <= 5f))
            {
                Trinity.SkipAheadAreaCache.Add(new CacheObstacleObject() { Position = Trinity.Player.Position, Radius = 20f });
            }
        }


    }
}

