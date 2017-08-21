using System;
using Trinity.Framework;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Zeta.Common;
using Zeta.Game;


namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class ClearAreaForNSecondsCoroutine : IBountySubroutine
    {
        private readonly int _questId;
        private readonly int _seconds;
        private readonly int _actorId;
        private readonly int _marker;
        private int _radius;
        private readonly bool _increaseRadius;
        private bool _isDone;
        private States _state;
        private BountyData _bountyData;
        private Vector3 _center;
        private long _startTime;
        private int _worldId = -1;

        private const int OBJECTIVE_SCAN_RANGE = 5000;

        public enum States
        {
            NotStarted,
            Clearing,
            MovingBack,
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
                    Core.Logger.Log("[ClearAreaForNSeconds] " + value);
                    StatusText = "[ClearAreaForNSeconds] " + value;
                }
                _state = value;
            }
        }

        public bool IsDone
        {
            get { return _isDone; }
        }

        public ClearAreaForNSecondsCoroutine(int questId, int seconds, int actorId, int marker, int radius = 30, bool increaseRadius = true)
        {
            _questId = questId;
            _seconds = seconds;
            _actorId = actorId;
            _marker = marker;
            _radius = radius;
            _increaseRadius = increaseRadius;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            if (_worldId > 0 && AdvDia.CurrentWorldId != _worldId)
            {
                State = States.Completed;
                return false;
            }

            if (_startPosition != Vector3.Zero)
            {
                ClearAreaHelper.CheckClearArea(_startPosition, _radius);
            }

            if (IsQuestStepComplete() && State != States.Completed)
            {
                Core.Logger.Log("Ending ClearAreaForNSecondsCoroutine because quest step appears to be completed!");
                State = States.Completed;
                return false;
            }

            switch (State)
            {
                case States.NotStarted:
                    return await NotStarted();

                case States.Clearing:
                    return await Clearing();

                case States.MovingBack:
                    return await MovingBack();

                case States.Completed:
                    return await Completed();

                case States.Failed:
                    return await Failed();
            }
            return false;
        }


        private bool IsQuestStepComplete()
        {
            if (BountyHelpers.QuestNpcExistsNearMe(80f))
            {
                return true;
            }
            return false;
        }

        private async Task<bool> NotStarted()
        {
            SafeZerg.Instance.DisableZerg();
            ScanForObjective();


            if (_center == Vector3.Zero)
            {
                _center = AdvDia.MyPosition;
            }

            // Ask combat routine to make sure everything is killed.
            //PluginCommunicator.RequestClearArea(_seconds);

            State = States.Clearing;
            _startTime = PluginTime.CurrentMillisecond;
            _startPosition = Core.Player.Position;
            return false;
        }

        private WaitCoroutine _waitCoroutine = new WaitCoroutine(1000);
        private Vector3 _startPosition;

        private async Task<bool> Clearing()
        {
            if (PluginTime.CurrentMillisecond - _startTime > _seconds * 1000)
            {
                _isDone = true;
                return false;
            }

            if (_worldId == -1)
                _worldId = AdvDia.CurrentWorldId;

            if (BountyData != null && BountyData.IsAvailable)
            {
                _radius = _increaseRadius ? (_radius < 80 ? _radius + 1 : _radius) : _radius;
                State = States.MovingBack;
                return false;
            }
            _isDone = true;
            State = States.Completed;
            return false;
        }

        private async Task<bool> MovingBack()
        {
            if (!await NavigationCoroutine.MoveTo(_center, 10)) return false;
            State = States.Clearing;
            return false;
        }

        private async Task<bool> Completed()
        {
            _isDone = true;
            return false;
        }

        private async Task<bool> Failed()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            _isDone = false;
            _state = States.NotStarted;
            _startPosition = Vector3.Zero;
        }

        public string StatusText { get; set; }

        public void DisablePulse()
        {
        }

        public BountyData BountyData
        {
            get { return _bountyData ?? (_bountyData = BountyDataFactory.GetBountyData(_questId)); }
        }

        private void ScanForObjective()
        {
            if (_marker != 0)
            {
                _center = BountyHelpers.ScanForMarkerLocation(_marker, OBJECTIVE_SCAN_RANGE);
            }
            if (_center == Vector3.Zero && _actorId != 0)
            {
                _center = BountyHelpers.ScanForActorLocation(_actorId, OBJECTIVE_SCAN_RANGE);
            }
            if (_center == Vector3.Zero)
            {
                _center = AdvDia.MyPosition;
            }
            if (_center != Vector3.Zero)
            {
                Core.Logger.Log("[ClearAreaForNSeconds] Found the objective at distance {0}", AdvDia.MyPosition.Distance(_center));
            }
        }
    }
}