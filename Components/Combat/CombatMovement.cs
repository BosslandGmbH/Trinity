using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Trinity.DbProvider;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Components.Combat
{
    public delegate bool CombatMovementCondition(CombatMovement movement);
    public delegate void CombatMovementUpdateDelegate(CombatMovement movement);

    /// <summary>
    /// Executed by HandleTarget when added to CombatMovement.Queue();
    /// </summary>
    public class CombatMovement
    {
        /// <summary>
        /// A friendly name to identify this SpecialMovement
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Destination where this SpecialMovement will move to
        /// </summary>
        public Vector3 Destination { get; set; }

        /// <summary>
        /// Executed directly after moving, will stop SpecialMovement if true is returned
        /// </summary>
        public CombatMovementCondition StopCondition { get; set; }

        /// <summary>
        /// Executed directly before moving, may be used to update destination
        /// </summary>
        public CombatMovementUpdateDelegate OnUpdate { get; set; }

        /// <summary>
        /// Executed after success or failure of specialmovement
        /// </summary>
        public CombatMovementUpdateDelegate OnFinished { get; set; }

        /// <summary>
        /// Optional Options to further customize the movement
        /// </summary>
        public CombatMovementOptions Options = new CombatMovementOptions();

        /// <summary>
        /// The position of player when movement started
        /// </summary>
        public Vector3 StartPosition { get; set; }

        /// <summary>
        /// Status is updated every tick and passed to OnUpdate() and OnFinished() events
        /// </summary>
        public CombatMovementStatus Status { get; set; }

        public DateTime LastFinishedTime { get; set; }
        public DateTime LastStartedTime { get; set; }
        public string Caller { get; set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString() => $"{Status?.LastStatus} Distance={Status?.DistanceToObjective} Speed={Status?.ChangeInDistance}";
    }

    public class CombatMovementStatus
    {
        public MoveResult LastStatus { get; set; }
        public Vector3 LastPosition { get; set; }
        public double DistanceToObjective { get; set; }
        public double ChangeInDistance { get; set; }
    }

    public class CombatMovementOptions
    {
        public CombatMovementOptions ()
        {
            FailureBlacklistSeconds = 1.5;
            SuccessBlacklistSeconds = 0;
            ChangeInDistanceLimit = 2f;
            TimeBeforeBlocked = 500;
            Logging = LogLevel.Info;
            AcceptableDistance = 8f;
            MaxDistance = 150f;
        }

        /// <summary>
        /// Change in distance since last move tick
        /// </summary>
        public float ChangeInDistanceLimit { get; set; }

        /// <summary>
        /// Time in Milliseconds below the ChangeInDistance setting to be 'blocked'
        /// </summary>
        public double TimeBeforeBlocked { get; set; }

        /// <summary>
        /// Duration movements are blacklisted from re-queue after Blocked or failed MoveResult
        /// </summary>
        public double FailureBlacklistSeconds { get; set; }
        public int SuccessBlacklistSeconds { get; set; }

        /// <summary>
        /// How detailed the logging will be
        /// </summary>
        public LogLevel Logging { get; set; }

        /// <summary>
        /// How close it should get to the destination before considering the destination reached
        /// </summary>
        public float AcceptableDistance { get; set; }

        /// <summary>
        /// How far away the destination is allowed to be
        /// </summary>
        public double MaxDistance { get; set; }


    }
    
    public class CombatMovementManager
    {
        private CombatMovement CurrentMovement { get; set; }
        private Queue<CombatMovement> _internalQueue = new Queue<CombatMovement>();
        private readonly List<CombatMovement> _blacklist = new List<CombatMovement>();
        private readonly CombatMovementStatus _status = new CombatMovementStatus();
        private CombatMovementOptions _options = new CombatMovementOptions();

        public void Queue(CombatMovement movement, [CallerMemberName] string caller = "")
        {
            movement.Caller = caller;

            if (movement.Destination == Vector3.Zero)
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.Movement, "Movement to {0} from {1} was rejected (Vector3.Zero Destination)", movement.Name, movement.Caller);
                return;
            }                

            if (movement != null && !IsBlacklisted(movement) && _internalQueue.All(m => m.Destination != movement.Destination))
            {

                _internalQueue = new Queue<CombatMovement>(_internalQueue.Where(m => m.Caller != caller));
                _internalQueue.Enqueue(movement);

                if (movement.Options.Logging >= LogLevel.Info)
                    LogLocation(string.Format("Queueing ({0})", _internalQueue.Count), movement);
            }
        }

        public RunStatus Execute()
        {           
            if (!IsQueuedMovement) 
                return RunStatus.Failure;

            if (CurrentMovement == null)
            {                
                CurrentMovement = _internalQueue.Dequeue();
                CurrentMovement.StartPosition = ZetaDia.Me.Position;
                CurrentMovement.LastStartedTime = DateTime.UtcNow;
                Stuck.Reset();
            }

            _options = CurrentMovement.Options;

            if (CurrentMovement.OnUpdate != null)
                CurrentMovement.OnUpdate.Invoke(CurrentMovement);

            _status.LastStatus = PlayerMover.NavigateTo(CurrentMovement.Destination, CurrentMovement.Name);
            _status.DistanceToObjective = ZetaDia.Me.Position.Distance(CurrentMovement.Destination);
            _status.ChangeInDistance = _status.LastPosition.Distance(CurrentMovement.Destination) - _status.DistanceToObjective;
            _status.LastPosition = ZetaDia.Me.Position;

            CurrentMovement.Status = _status;

            if (CurrentMovement.StopCondition != null &&
                CurrentMovement.StopCondition.Invoke(CurrentMovement))
            {
                FailedHandler("StopWhen");
                return RunStatus.Failure;
            }

            if (Stuck.IsStuck(_options.ChangeInDistanceLimit,_options.TimeBeforeBlocked))
            {
                FailedHandler("Blocked " + Stuck.LastLogMessage);
                return RunStatus.Failure;
            }

            if (_status.DistanceToObjective < _options.AcceptableDistance)
            {
                SuccessHandler(string.Format("AcceptableDistance: {0}", _options.AcceptableDistance));
                return RunStatus.Success;
            }

            if (IsBlacklisted(CurrentMovement))
            {
                FailedHandler("RecentlyFailed");
                return RunStatus.Success;
            }

            if (_status.DistanceToObjective > _options.MaxDistance)
            {
                FailedHandler(string.Format("MaxDistance: {0}", _options.MaxDistance));
                return RunStatus.Success;
            }

            switch (_status.LastStatus)
            {
                case MoveResult.ReachedDestination:
                    SuccessHandler();
                    return RunStatus.Success;
                case MoveResult.PathGenerationFailed:
                case MoveResult.Moved:
                    MovedHandler();
                    return RunStatus.Running;
                case MoveResult.Failed:
                    FailedHandler("Navigation");
                    return RunStatus.Failure;
                default:
                    return RunStatus.Success;
            }

        }

        /// <summary>
        /// CombatMovement is finished successfully - arrived at destination.
        /// </summary>
        /// <param name="reason"></param>
        public void SuccessHandler(string reason = "")
        {
            if (CurrentMovement.Options.Logging >= LogLevel.Info)
            {
                var location = (!string.IsNullOrEmpty(reason) ? "(" + reason + ")" : reason);
                LogLocation("Arrived at " + location, CurrentMovement);
            }

            if (CurrentMovement.Options.SuccessBlacklistSeconds > 0 && !_blacklist.Contains(CurrentMovement))
                _blacklist.Add(CurrentMovement);

            FinishedHandler();
        }

        /// <summary>
        /// CombatMovement is in progress
        /// </summary>
        public void MovedHandler()
        {
            //if (CurrentMovement.Options.Logging >= LogLevel.Debug)
            LogLocation("Moving to", CurrentMovement, Stuck.LastLogMessage, TrinityLogLevel.Verbose);
        }

        /// <summary>
        /// CombatMovement was a dismal failure.
        /// </summary>
        public void FailedHandler(string reason = "")
        {
            if (!_blacklist.Contains(CurrentMovement))
                _blacklist.Add(CurrentMovement);

            if (CurrentMovement.Options.Logging >= LogLevel.Debug)
            {
                var location = (!string.IsNullOrEmpty(reason) ? "(" + reason + ") " : reason);
                LogLocation("Failed " + location + "moving to ", CurrentMovement);
            }
            FinishedHandler();
        }

        /// <summary>
        /// Common tidy-up after finishing
        /// </summary>
        public void FinishedHandler()
        {
            CurrentMovement.LastFinishedTime = DateTime.UtcNow;

            if (CurrentMovement.OnFinished != null)
                CurrentMovement.OnFinished.Invoke(CurrentMovement);

            CurrentMovement = null;
        }

        public void LogLocation(string pre, CombatMovement movement, string post = "", TrinityLogLevel level = TrinityLogLevel.Info)
        {
            Logger.Log(level, LogCategory.Movement, pre + " {0} Distance={4:0.#} ({1:0.#},{2:0.#},{3:0.#}) {5}",
                movement.Name,
                movement.Destination.X,
                movement.Destination.Y,
                movement.Destination.Z,
                ZetaDia.Me.Position.Distance(movement.Destination),
                post);
        }

        public bool IsQueuedMovement
        {
            get { return _internalQueue.Count > 0 || CurrentMovement != null; }
        }

        public bool IsBlacklisted(CombatMovement movement)
        {
            _blacklist.RemoveAll(m => DateTime.UtcNow.Subtract(m.LastFinishedTime).TotalSeconds > m.Options.FailureBlacklistSeconds);
            return _blacklist.Any(m => m.Name == movement.Name);
        }

        public static class Stuck
        {
            static Stuck()
            {
                Pulsator.OnPulse += (sender, args) => Pulse();
            }

            private static bool _isMoving;
            private static Vector3 _lastPosition = Vector3.Zero;
            public static float ChangeInDistance { get; set; }
            private const int MaxPossibleDistanceTravelled = 200;
            static readonly Stopwatch StuckTime = new Stopwatch();
            private static string _log;

            private static void Pulse()
            {
                ChangeInDistance = _lastPosition.Distance(ZetaDia.Me.Position);
                _lastPosition = ZetaDia.Me.Position;
                IsStuck();
            }

            public static double StuckElapsedMilliseconds
            {
                get { return StuckTime.ElapsedMilliseconds; }
            }

            public static string LastLogMessage
            {
                get { return _log; }
            }
            
            public static bool IsStuck(float changeInDistanceLimit = 2.5f, double stuckTimeLimit = 500)
            {
                if (ChangeInDistance < MaxPossibleDistanceTravelled && ChangeInDistance < changeInDistanceLimit * ZetaDia.Me.MovementScalar)
                {
                    if (_isMoving)
                    {
                        Reset();
                        StuckTime.Start();
                    }
                    _isMoving = false;
                }
                else
                {
                    Reset();
                }

                _log = string.Format("Speed={0:0.#}/{1:0.#} StuckTime={2:0.#}/{3:0.#}", ChangeInDistance, changeInDistanceLimit * ZetaDia.Me.MovementScalar, StuckTime.ElapsedMilliseconds, stuckTimeLimit);
                
                var stuck = !_isMoving && StuckTime.ElapsedMilliseconds >= stuckTimeLimit;

                return stuck;
            }

            public static void Reset()
            {
                StuckTime.Stop();
                StuckTime.Reset();
                _isMoving = true;
            }
        }
    }



}
