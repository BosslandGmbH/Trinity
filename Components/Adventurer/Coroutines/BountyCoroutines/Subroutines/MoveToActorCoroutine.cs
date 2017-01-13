using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class MoveToActorCoroutine : IBountySubroutine
    {
        private readonly int _questId;
        private readonly int _worldId;
        private readonly int _actorId;

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
                    Util.Logger.Info("[MoveToActor] " + value);
                }
                _state = value;
            }
        }

        #endregion

        public bool IsDone
        {
            get { return _isDone || AdvDia.CurrentWorldId != _worldId; }
        }



        public MoveToActorCoroutine(int questId, int worldId, int actorId, int maxRange = 5000)
        {
            _questId = questId;
            _worldId = worldId;
            _actorId = actorId;
            _objectiveScanRange = maxRange;
        }


        public async Task<bool> GetCoroutine()
        {
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

            var areaIds = BountyData != null ? BountyData.LevelAreaIds : new HashSet<int> { AdvDia.CurrentLevelAreaId };
            if (!await ExplorationCoroutine.Explore(areaIds)) return false;
            ScenesStorage.Reset();
            return false;
        }

        private async Task<bool> Moving()
        {
            // todo make this relative to actor's collision radius or bot might get stuck walking into it.
            if (await NavigationCoroutine.MoveTo(_objectiveLocation, 15))
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

        private async Task<bool> ScanForObjective()
        {
            if (PluginTime.ReadyToUse(_lastScanTime, 1000))
            {
                _lastScanTime = PluginTime.CurrentMillisecond;
                if (_actorId != 0)
                {
                    _objectiveLocation = BountyHelpers.ScanForActorLocation(_actorId, _objectiveScanRange);
                }
                if (_objectiveLocation != Vector3.Zero)
                {
                    using (new PerformanceLogger("[MoveToObject] Path to Objective Check", true))
                    {
                        //if (await AdvDia.DefaultNavigationProvider.CanFullyClientPathTo(_objectiveLocation))
                        //{
                        Logger.Info("[MoveToObject] Found the objective at distance {0}",
                        AdvDia.MyPosition.Distance(_objectiveLocation));
                        //}
                        //else
                        //{
                        //    Logger.Debug("[MoveToObject] Found the objective at distance {0}, but cannot get a path to it.",
                        //        AdvDia.MyPosition.Distance(_objectiveLocation));
                        //    _objectiveLocation = Vector3.Zero;                            
                        //}
                    }
                }
            }
            return true;
        }
    }
}
