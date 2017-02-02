using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Zeta.Common;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Coroutines
{
    public sealed class ClearAreaCoroutine
    {
        private static ClearAreaCoroutine _clearAreaCoroutine;
        private static Vector3 _clearCenter;
        private static int _clearRadius;
        private static bool _clearForce;
        private static bool _clearReturnToCenter = true;
        private readonly Vector3 _center;
        private readonly bool _forceMoveAround;
        private readonly int _radius;
        private readonly bool _returnToCenter;
        private Vector3 _currentDestination;
        private ConcurrentBag<Vector3> _forceClearDestinations;
        private States _state;

        private ClearAreaCoroutine(Vector3 center, int radius, bool forceMoveAround, bool returnToCenter)
        {
            _center = center;
            _radius = radius;
            _forceMoveAround = forceMoveAround;
            _returnToCenter = returnToCenter;
        }

        private States State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Util.Logger.Debug("[ClearArea] " + value);
                }
                _state = value;
            }
        }

        public static async Task<bool> Clear(Vector3 center, int radius, bool forceMoveAround = false, bool returnToCenter = true)
        {
            if (_clearAreaCoroutine == null || radius != _clearRadius || forceMoveAround != _clearForce ||
                returnToCenter != _clearReturnToCenter)
            {
                _clearCenter = center;
                _clearRadius = radius;
                _clearForce = forceMoveAround;
                _clearReturnToCenter = returnToCenter;
                _clearAreaCoroutine = new ClearAreaCoroutine(center, radius, forceMoveAround, returnToCenter);
            }
            if (await _clearAreaCoroutine.GetCoroutine())
            {
                _clearAreaCoroutine = null;
                return true;
            }
            return false;
        }

        private async Task<bool> GetCoroutine()
        {
            switch (State)
            {
                case States.NotStarted:
                    return NotStarted();
                case States.Clearing:
                    return await Clearing();
                case States.ForceClearing:
                    return await ForceClearing();
                case States.Cleared:
                    return await Cleared();
                case States.Completed:
                    return Completed();
            }
            return false;
        }

        private bool NotStarted()
        {
            State = States.Clearing;

            _currentDestination = ActorFinder.FindNearestHostileUnitInRadius(_center, _radius);
            if (_currentDestination == Vector3.Zero && !_forceMoveAround)
            {
                State = States.Completed;
                return false;
            }

            if (_forceMoveAround)
            {
                // Desperate Measures
                _forceClearDestinations =
                    new ConcurrentBag<Vector3>(
                        ExplorationHelpers.GetFourPointsInEachDirection(_center, _radius).Where(d => d != Vector3.Zero));

                Util.Logger.Debug($"[ClearArea] No actors found in the area, using the desperate measures. Center={_center} Radius={_radius}");
                State = States.ForceClearing;
                if (_forceClearDestinations.TryTake(out _currentDestination))
                {
                    return false;
                }
                Util.Logger.Error($"[ClearArea] Couldn't get force clear destinations, ending tag. Center={_center} Radius={_radius}");
                State = States.Completed;
                return true;
            }


            return false;
        }

        private async Task<bool> Clearing()
        {
            ClearAreaHelper.CheckClearArea(_center, _radius);

            if (!await NavigationCoroutine.MoveTo(_currentDestination, 10)) return false;
            _currentDestination = _currentDestination = ActorFinder.FindNearestHostileUnitInRadius(_center, _radius);
            if (_currentDestination == Vector3.Zero)
            {
                State = States.Cleared;
            }
            return false;
        }

        private async Task<bool> ForceClearing()
        {
            if (!await NavigationCoroutine.MoveTo(_currentDestination, 10)) return false;
            if (!_forceClearDestinations.TryTake(out _currentDestination))
            {
                State = States.Cleared;
            }
            return false;
        }

        private async Task<bool> Cleared()
        {
            if (!_returnToCenter)
            {
                State = States.Completed;
                return false;
            }

            if (!await NavigationCoroutine.MoveTo(_center, 10)) return false;
            State = States.Completed;
            return true;
        }

        private bool Completed()
        {
            return true;
        }

        private enum States
        {
            NotStarted,
            Clearing,
            ForceClearing,
            Cleared,
            Completed
        }
    }
}