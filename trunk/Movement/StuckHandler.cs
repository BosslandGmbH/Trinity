using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.DbProvider;
using Trinity.Technicals;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using System.Threading.Tasks;
using System.Windows.Forms;
using Buddy.Coroutines;
using Trinity.Combat.Abilities;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Common;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Action = Zeta.TreeSharp.Action;
using Logger = Trinity.Technicals.Logger;



namespace Trinity
{
    public class StuckHandler : IStuckHandler
    {
        private DateTime _lastStuckCheck = DateTime.MinValue;
        private Vector3 _lastPosition = Vector3.Zero;
        private bool _isStuck;
        private int _checkIntervalMs = 2500;
        private bool _isSuspectedStuck;
        private int _stuckValidationTime = 10000;
        private DateTime _suspectedStuckStartTime = DateTime.MaxValue;
        private Vector3 _stuckPosition;
        private Vector3 _suspectedStuckPosition;

        public StuckHandler()
        {
            GameEvents.OnWorldChanged += (sender, args) => Reset();
        }

        public bool IsStuck
        {
            get
            {
                if (DateTime.UtcNow.Subtract(_lastStuckCheck).TotalMilliseconds <= _checkIntervalMs)
                    return _isStuck;

                CheckForStuck();
                return _isStuck;
            }
        }

        protected bool CheckForStuck()
        {
            _lastPosition = ZetaDia.Me.Position;
            _lastStuckCheck = DateTime.UtcNow;

            if (IsNotStuck())
            {
                if (_isSuspectedStuck)
                    Logger.LogVerbose(LogCategory.Movement, "No longer suspected of stuck");

                if (_isStuck)
                    Logger.LogVerbose(LogCategory.Movement, "No longer stuck!");

                Reset();
                return _isStuck;
            }

            if (!_isStuck && !_isSuspectedStuck)
            {
                SetSuspectedStuck();
                return _isStuck;
            }

            if (_isSuspectedStuck)
            {
                var millisecondsSuspected = DateTime.UtcNow.Subtract(_suspectedStuckStartTime).TotalMilliseconds;
                if (millisecondsSuspected >= _stuckValidationTime)
                {
                    SetStuck();
                    return _isStuck;
                }

                Logger.Log(LogCategory.Movement, "Suspected Stuck for {0}ms", millisecondsSuspected);
                return _isStuck;
            }

            if (_isStuck)
                return true;

            Reset();
            return _isStuck;
        }

        private void SetStuck()
        {
            Logger.LogVerbose(LogCategory.Movement, "Definately Stuck!");
            _isStuck = true;
            _stuckPosition = ZetaDia.Me.Position;
            _isSuspectedStuck = false;
            _suspectedStuckStartTime = DateTime.MaxValue;
        }

        private void SetSuspectedStuck()
        {
            _isStuck = false;
            _isSuspectedStuck = true;
            _suspectedStuckStartTime = DateTime.UtcNow;
            _suspectedStuckPosition = ZetaDia.Me.Position;
        }

        private void Reset()
        {
            _isStuck = false;
            _isSuspectedStuck = false;
            _stuckPosition = Vector3.Zero;
            _suspectedStuckStartTime = DateTime.MaxValue;
            _suspectedStuckPosition = Vector3.Zero;
        }

        private bool IsNotStuck()
        {
            if (Trinity.Settings.Advanced.DisableAllMovement)
                return true;

            if (!ZetaDia.IsInGame || ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                return true;

            if (ZetaDia.IsInTown && (UIElements.VendorWindow.IsVisible || UIElements.SalvageWindow.IsVisible) && !ZetaDia.Me.Movement.IsMoving)
                return true;

            if (ZetaDia.Me.IsInConversation || ZetaDia.IsPlayingCutscene || ZetaDia.IsLoadingWorld)
                return true;

            if (ZetaDia.Me.LoopingAnimationEndTime > 0)
                return true;

            if (_stuckPosition != Vector3.Zero && _stuckPosition.Distance(ZetaDia.Me.Position) > 20f)
            {
                Logger.Log(LogCategory.Movement, $"Not Stuck: Moved {_stuckPosition.Distance(ZetaDia.Me.Position)} from stuck position");
                return true;
            }

            if (_suspectedStuckPosition != Vector3.Zero && _suspectedStuckPosition.Distance(ZetaDia.Me.Position) > 15f)
            {
                Logger.Log(LogCategory.Movement, $"Not Stuck: Moved {_suspectedStuckPosition.Distance(ZetaDia.Me.Position)} from suspected stuck position");
                return true;
            }

            if (DateTime.UtcNow.Subtract(SpellHistory.LastSpellUseTime).TotalSeconds < 4)
                return true;

            if (DateTime.UtcNow.Subtract(Trinity.BotStartTime).TotalSeconds < 10)
                return true;

            if (_busyAnimationStates.Contains(ZetaDia.Me.CommonData.AnimationState))
                return true;

            if (PlayerMover.GetMovementSpeed() > 2)
            {
                Logger.Log(LogCategory.Movement, $"Not Stuck: Moving (Speed: {PlayerMover.GetMovementSpeed()})");
                return true;
            }

            if (ZetaDia.Me.IsDead || UIElements.WaypointMap.IsVisible)
                return true;

            if (IsProfileBusy())
                return true;

            return false;
        }

        private readonly HashSet<AnimationState> _busyAnimationStates = new HashSet<AnimationState>
        {
            AnimationState.Attacking,
            AnimationState.Channeling,
            AnimationState.Casting,
            AnimationState.Dead,
            AnimationState.Invalid,
        };

        private bool IsProfileBusy()
        {
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
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DoUnstick()
        {
            Logger.LogNormal("Trying to get Unstuck...");
            Navigator.Clear();
            Navigator.NavigationProvider.Clear();
            await Navigator.SearchGridProvider.Update();
            var startPosition = ZetaDia.Me.Position;
            Logger.LogVerbose("Starting Segment 1...");
            await MoveAwayFrom(startPosition);
            Logger.LogVerbose("Starting Segment 2 ...");
            await MoveAwayFrom(startPosition);
            return true;
        }

        private static async Task<bool> MoveAwayFrom(Vector3 position)
        {
            var startTime = DateTime.UtcNow;
            var playerDistance = position.Distance(ZetaDia.Me.Position);

            var positions = GenerateUnstickPositions().Where(p => p.Distance(position) > playerDistance).ToList();

            if (!positions.Any())
                return false;

            var targetPosition = positions.First();
            var segmentStartTime = DateTime.UtcNow;        

            while (DateTime.UtcNow.Subtract(startTime).TotalSeconds < 10)
            {
                if (targetPosition == Vector3.Zero)
                    break;

                var distance = targetPosition.Distance(ZetaDia.Me.Position);

                Logger.LogVerbose("Moving to {0} Dist={1}", targetPosition, distance);
                ZetaDia.Me.UsePower(SNOPower.Walk, targetPosition, ZetaDia.WorldId);
                await Coroutine.Sleep(50);

                if (distance < 4f || position.Distance(ZetaDia.Me.Position) > 15f)
                    break;

                if (!ZetaDia.Me.Movement.IsMoving || DateTime.UtcNow.Subtract(segmentStartTime).TotalMilliseconds > 2500)
                {
                    positions.Remove(targetPosition);
                    segmentStartTime = DateTime.UtcNow;
                    targetPosition = positions.FirstOrDefault();
                }
            }
            return true;
        }


        private static List<Vector3> GenerateUnstickPositions()
        {
            var circlePoints = GetCirclePoints(10, 30, ZetaDia.Me.Position);
            //var validatedPoints = circlePoints.Where(p => Navigator.Raycast(ZetaDia.Me.Position, p)).ToList();
            var validatedPoints = circlePoints.Where(p => Trinity.MainGridProvider.CanStandAt(p)).ToList();
            return RandomShuffle(validatedPoints);
        }

        public static List<T> RandomShuffle<T>(List<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public static List<Vector3> GetCirclePoints(int points, double radius, Vector3 center)
        {
            var result = new List<Vector3>();
            var slice = 2 * Math.PI / points;
            for (var i = 0; i < points; i++)
            {
                var angle = slice * i;
                var newX = (int)(center.X + radius * Math.Cos(angle));
                var newY = (int)(center.Y + radius * Math.Sin(angle));

                var newpoint = new Vector3(newX, newY, center.Z);
                result.Add(newpoint);

                //Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement, "Calculated point {0}: {1}", i, newpoint.ToString());
            }
            return result;
        }

    }
}
