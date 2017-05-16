using System;
using Trinity.Framework;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Quests;
using Zeta.Bot.Navigation;
using Zeta.Common;

namespace Trinity.Components.Adventurer.Coroutines.CommonSubroutines
{
    public class MoveToPositionCoroutine : ISubroutine
    {
        private readonly int _worldId;
        private readonly int _distance;
        private readonly Vector3 _position;
        private bool _isDone;
        private States _state;
        private DateTime _startTime;
        private bool _straightLinePathing;

        #region State

        public enum States
        {
            NotStarted,
            Moving,
            Completed,
            Failed
        }

        public States State
        {
            get { return _state; }
            protected set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Core.Logger.Debug("[MoveToPosition] " + value);
                }
                _state = value;
            }
        }

        #endregion State

        public bool IsDone
        {
            get { return _isDone || AdvDia.CurrentWorldId != _worldId; }
        }

        public MoveToPositionCoroutine(int worldId, Vector3 position, int distance = 1, bool forceStraightLinePathing = false)
        {
            _startTime = DateTime.UtcNow;
            _distance = distance;
            _worldId = worldId;
            _position = position;
            _straightLinePathing = forceStraightLinePathing;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            switch (State)
            {
                case States.NotStarted:
                    return NotStarted();

                case States.Moving:
                    return await Moving();

                case States.Completed:
                    return Completed();

                case States.Failed:
                    return Failed();
            }
            return false;
        }

        public void Reset()
        {
            _isDone = false;
            _startTime = DateTime.MinValue;
            _state = States.NotStarted;
        }

        public void DisablePulse()
        {
        }

        public BountyData BountyData
        {
            get { return null; }
        }

        private bool NotStarted()
        {
            NavigationCoroutine.Reset();
            State = States.Moving;
            return false;
        }

        private async Task<bool> Moving()
        {
            if (!await NavigationCoroutine.MoveTo(_position, _distance, _straightLinePathing))
            {
                return false;
            }

            if (NavigationCoroutine.LastResult == CoroutineResult.Failure)
            {
                Core.Logger.Debug("[MoveToPosition] CoroutineResult.Failure");

                var canFullyPath = await AdvDia.Navigator.CanFullyClientPathTo(_position);
                var closeRayCastFail = AdvDia.MyPosition.Distance(_position) < 15f && !Core.Grids.CanRayWalk(AdvDia.MyPosition, _position);//!NavigationGrid.Instance.CanRayWalk(AdvDia.MyPosition, _position);
                var failedMoveResult = NavigationCoroutine.LastMoveResult == MoveResult.Failed || NavigationCoroutine.LastMoveResult == MoveResult.PathGenerationFailed;
                if (!canFullyPath || closeRayCastFail || failedMoveResult)
                {
                    Core.Logger.Debug("[MoveToPosition] Failed to reach position");
                    State = States.Failed;
                    return false;
                }
            }

            if (AdvDia.MyPosition.Distance(_position) > 20)
            {
                return false;
            }

            State = States.Completed;
            return false;
        }

        private bool Completed()
        {
            _isDone = true;
            return true;
        }

        private bool Failed()
        {
            Core.PlayerMover.MoveTowards(_position);
            _isDone = true;
            return true;
        }
    }
}