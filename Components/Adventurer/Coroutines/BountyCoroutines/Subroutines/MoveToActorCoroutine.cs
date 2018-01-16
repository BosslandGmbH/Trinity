using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Common;


namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class MoveToActorCoroutine : IBountySubroutine
    {
        private readonly int _questId;
        private readonly int _worldId;
        private readonly int _actorId;
        private readonly bool _isExploreAllowed;

        private bool _isDone;
        private States _state;

        private int _objectiveScanRange;

        #region State

        public enum States
        {
            NotStarted,
            Searching,
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
                    Core.Logger.Log("[MoveToActor] " + value);
                    StatusText = "[MoveToActor] " + value;
                }
                _state = value;
            }
        }

        #endregion State

        public bool IsDone
        {
            get { return _isDone || AdvDia.CurrentWorldId != _worldId; }
        }

        public MoveToActorCoroutine(int questId, int worldId, int actorId, int maxRange = 5000, 
            bool isExploreAllowed = true, Func<TrinityActor,bool> actorSelector = null, float stopDistance = -1, int markerId = 0)
        {
            _questId = questId;
            _worldId = worldId;
            _actorId = actorId;
            _objectiveScanRange = maxRange;
            _isExploreAllowed = isExploreAllowed;
            _actorSelector = actorSelector;
            _stopDistance = stopDistance;
            _markerId = markerId;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            switch (State)
            {
                case States.NotStarted:
                    return await NotStarted();

                case States.Searching:
                    return await Searching();

                case States.Moving:
                    return await Moving();

                case States.Completed:
                    return await Completed();

                case States.Failed:
                    return await Failed();
            }
            return false;
        }

        public void Reset()
        {
            _isDone = false;
            _state = States.NotStarted;
            _objectiveScanRange = 5000;
            _objectiveLocation = Vector3.Zero;
        }

        public string StatusText { get; set; }

        public void DisablePulse()
        {
        }

        public BountyData BountyData
        {
            get { return _bountyData ?? (_bountyData = BountyDataFactory.GetBountyData(_questId)); }
        }

        private async Task<bool> NotStarted()
        {
            SafeZerg.Instance.DisableZerg();
            State = States.Searching;
            return false;
        }

        private async Task<bool> Searching()
        {
            if (_objectiveLocation == Vector3.Zero)
            {
                await ScanForObjective();
            }

            if (_objectiveLocation != Vector3.Zero)
            {
                State = States.Moving;
                return false;
            }

            if (!_isExploreAllowed)
            {
                Core.Logger.Log("Unable to find actor and exploration is disabled");
                State = States.Failed;
                return false;
            }

            var areaIds = BountyData != null ? BountyData.LevelAreaIds : new HashSet<int> { AdvDia.CurrentLevelAreaId };

            if (!await ExplorationCoroutine.Explore(areaIds, useIgnoreRegions:false)) return false;

            Core.Scenes.Reset();
            return false;
        }

        private async Task<bool> Moving()
        {
            if (await NavigationCoroutine.MoveTo(_objectiveLocation, Math.Max(5,(int)_stopDistance)))
            {
                if (AdvDia.MyPosition.Distance(_objectiveLocation) > 30 && NavigationCoroutine.LastResult == CoroutineResult.Failure)
                {
                    _objectiveLocation = Vector3.Zero;
                    _objectiveScanRange = ActorFinder.LowerSearchRadius(_objectiveScanRange);
                    if (_objectiveScanRange <= 0)
                    {
                        _objectiveScanRange = 50;
                    }
                    State = States.Searching;
                    return false;
                }
                State = States.Completed;
                return false;
            }
            return false;
        }

        private async Task<bool> Completed()
        {
            _isDone = true;
            return false;
        }

        private async Task<bool> Failed()
        {
            _isDone = true;
            return false;
        }

        private Vector3 _objectiveLocation = Vector3.Zero;

        private long _lastScanTime;
        private BountyData _bountyData;
        private Func<TrinityActor, bool> _actorSelector;
        private float _stopDistance;
        private int _markerId;

        private async Task<bool> ScanForObjective()
        {
            if (PluginTime.ReadyToUse(_lastScanTime, 1000))
            {
                _lastScanTime = PluginTime.CurrentMillisecond;
                if (_actorId != 0)
                {
                    //var objectiveActor = BountyHelpers.ScanForActor(_actorId, _objectiveScanRange, _actorSelector);
                    var objectiveActor = ActorFinder.FindActor(_actorId, _markerId, 500, "", _actorSelector);
                    if (objectiveActor != null)
                    {
                        _objectiveLocation = objectiveActor.Position;
                        if (_stopDistance == -1)
                            _stopDistance = objectiveActor.Radius;
                    }
                    else
                    {
                        _objectiveLocation = Vector3.Zero;
                    }
                }
                if (_objectiveLocation != Vector3.Zero)
                {
                    Core.Logger.Log($"[MoveToObject] Found the objective at distance {AdvDia.MyPosition.Distance(_objectiveLocation)}");
                }
            }
            return true;
        }
    }
}