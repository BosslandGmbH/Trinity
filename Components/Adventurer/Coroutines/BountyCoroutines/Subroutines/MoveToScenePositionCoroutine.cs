using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Zeta.Common;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class MoveToScenePositionCoroutine : IBountySubroutine
    {
        private readonly int _questId;
        private readonly int _worldId;
        private readonly string _sceneName;
        private string _tempSceneName;
        private readonly Vector3 _position;
        private WorldScene _worldScene;


        private bool _isDone;
        private States _state;

        private int _objectiveScanRange = 5000;

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
                    Util.Logger.Info("[MoveToScenePosition] " + value);
                }
                _state = value;
            }
        }

        #endregion

        public bool IsDone
        {
            get { return _isDone || AdvDia.CurrentWorldId != _worldId; }
        }



        public MoveToScenePositionCoroutine(int questId, int worldId, string sceneName, Vector3 position)
        {
            _questId = questId;
            _worldId = worldId;
            _sceneName = sceneName;
            _position = position;
        }

        public MoveToScenePositionCoroutine(int sceneSnoId, Vector3 position)
        {
            _sceneSnoId = sceneSnoId;
            _position = position;
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
            if (_sceneName == null)
            {
                _tempSceneName = AdvDia.CurrentWorldScene.Name;
            }
            else
            {
                _tempSceneName = null;
            }
            State = States.Searching;

            return false;
        }

        private async Task<bool> Searching()
        {
            if (_objectiveLocation == Vector3.Zero)
            {
                ScanForObjective();
            }
            if (_objectiveLocation != Vector3.Zero)
            {
                State = States.Moving;
                return false;
            }
            if (!await ExplorationCoroutine.Explore(BountyData.LevelAreaIds)) return false;
            ScenesStorage.Reset();
            return false;
        }

        private async Task<bool> Moving()
        {
            if (await NavigationCoroutine.MoveTo(_objectiveLocation, 10))
            {
                if (AdvDia.MyPosition.Distance(_objectiveLocation) > 30 && NavigationCoroutine.LastResult == CoroutineResult.Failure)
                {
                    _previouslyFoundLocation = _objectiveLocation;
                    _returnTimeForPreviousLocation = PluginTime.CurrentMillisecond;
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
        private Vector3 _previouslyFoundLocation = Vector3.Zero;
        private long _returnTimeForPreviousLocation;

        private long _lastScanTime;
        private BountyData _bountyData;
        private int _sceneSnoId;

        private void ScanForObjective()
        {
            if (_previouslyFoundLocation != Vector3.Zero && PluginTime.ReadyToUse(_returnTimeForPreviousLocation, 60000))
            {
                _objectiveLocation = _previouslyFoundLocation;
                _previouslyFoundLocation = Vector3.Zero;
                Logger.Debug("[MoveToScenePosition] Returning previous objective location.");
                return;
            }
            if (PluginTime.ReadyToUse(_lastScanTime, 1000))
            {
                _lastScanTime = PluginTime.CurrentMillisecond;
                if (_sceneSnoId > 0)
                {
                    var scene = ScenesStorage.CurrentWorldScenes.OrderBy(s => s.Center.DistanceSqr(AdvDia.MyPosition.ToVector2())).FirstOrDefault(s => s.SnoId == _sceneSnoId);
                    if (scene != null)
                    {
                        _worldScene = scene;
                        _objectiveLocation = _worldScene.GetWorldPosition(_position);
                    }

                }
                else if (!string.IsNullOrEmpty(_sceneName))
                {
                    var scene = ScenesStorage.CurrentWorldScenes.OrderBy(s => s.Center.DistanceSqr(AdvDia.MyPosition.ToVector2())).FirstOrDefault(s => s.Name.Contains(_sceneName));
                    if (scene != null)
                    {
                        _worldScene = scene;
                        _objectiveLocation = _worldScene.GetWorldPosition(_position);
                    }
                }
                else if (!string.IsNullOrEmpty(_tempSceneName))
                {
                    var scene = ScenesStorage.CurrentWorldScenes.OrderBy(s => s.Center.DistanceSqr(AdvDia.MyPosition.ToVector2())).FirstOrDefault(s => s.Name==_tempSceneName);
                    if (scene != null)
                    {
                        _worldScene = scene;
                        _objectiveLocation = _worldScene.GetWorldPosition(_position);
                    }
                }
                //if (_objectiveLocation == Vector3.Zero && _actorId != 0)
                //{
                //    _objectiveLocation = BountyHelpers.ScanForActorLocation(_actorId, _objectiveScanRange);
                //}
                if (_objectiveLocation != Vector3.Zero)
                {
                    using (new PerformanceLogger("[MoveToScenePosition] Path to Objective Check", true))
                    {
                        //if ((Navigator.GetNavigationProviderAs<DefaultNavigationProvider>().CanFullyClientPathTo(_objectiveLocation)))
                        //{
                        Logger.Info("[MoveToScenePosition] Found the objective at distance {0}",
                            AdvDia.MyPosition.Distance(_objectiveLocation));
                        //}
                        //else
                        //{
                        //    Logger.Debug("[MoveToMapMarker] Found the objective at distance {0}, but cannot get a path to it.",
                        //        AdvDia.MyPosition.Distance(_objectiveLocation));
                        //    _objectiveLocation = Vector3.Zero;
                        //}
                    }

                }
            }
        }
    }
}
