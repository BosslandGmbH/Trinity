using System.Collections.Generic;
using System.Linq; using Trinity.Framework;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Trinity.UI.Visualizer;
using Zeta.Common;
using Zeta.Game;


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
                    Core.Logger.Log("[MoveToScenePosition] " + value);
                }
                _state = value;
            }
        }

        #endregion State

        public bool IsDone
        {
            get { return _isDone || _worldId != 0 && AdvDia.CurrentWorldId != _worldId; }
        }

        public MoveToScenePositionCoroutine(int questId, int worldId, string sceneName, Vector3 position, bool straightLinePath = false)
        {
            _questId = questId;
            _worldId = worldId;
            _sceneName = sceneName;
            _position = position;
            _straightLinePath = straightLinePath;
        }

        public MoveToScenePositionCoroutine(int sceneSnoId, Vector3 position)
        {
            _sceneSnoId = sceneSnoId;
            _position = position;
        }

        public MoveToScenePositionCoroutine(string sceneName, Vector3 position)
        {
            _sceneName = sceneName;
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
            if (AdvDia.CurrentWorldScene == null)
            {
                Core.Logger.Debug("waiting patiently for world scene data");
                return false;
            }

            SafeZerg.Instance.DisableZerg();
            if (_sceneName == null)
            {
                _tempSceneName = AdvDia.CurrentWorldScene.Name;
            }
            else
            {
                _tempSceneName = null;
            }

            Core.Logger.Debug($"Started MoveToScenePositionCoroutine SceneName='{_sceneName}' SceneSnoId={_sceneSnoId}");
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

            Core.Logger.Debug("Unable to find scene, exploring...");

            var bountyData = BountyData;

            var levelAreaIds = bountyData?.LevelAreaIds != null && bountyData.LevelAreaIds.Any()
                ? bountyData.LevelAreaIds : new HashSet<int> { ZetaDia.CurrentLevelAreaSnoId };

            if (!await ExplorationCoroutine.Explore(levelAreaIds))
                return false;

            Core.Scenes.Reset();
            return false;
        }

        private async Task<bool> Moving()
        {
            if (await NavigationCoroutine.MoveTo(_objectiveLocation, 10, _straightLinePath))
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
            return true;
        }

        private async Task<bool> Failed()
        {
            _isDone = true;
            return true;
        }

        private Vector3 _objectiveLocation = Vector3.Zero;
        private Vector3 _previouslyFoundLocation = Vector3.Zero;
        private long _returnTimeForPreviousLocation;

        private long _lastScanTime;
        private BountyData _bountyData;
        private int _sceneSnoId;
        private bool _straightLinePath;

        private void ScanForObjective()
        {
            //if (_previouslyFoundLocation != Vector3.Zero && PluginTime.ReadyToUse(_returnTimeForPreviousLocation, 60000))
            //{
            //    _objectiveLocation = _previouslyFoundLocation;
            //    _previouslyFoundLocation = Vector3.Zero;
            //    Core.Logger.Debug("[MoveToScenePosition] Returning previous objective location.");
            //    return;
            //}
            if (PluginTime.ReadyToUse(_lastScanTime, 1000))
            {
                _lastScanTime = PluginTime.CurrentMillisecond;
                if (_sceneSnoId > 0)
                {
                    var scene = Core.Scenes.CurrentWorldScenes.OrderBy(s => s.Center.DistanceSqr(AdvDia.MyPosition.ToVector2())).FirstOrDefault(s => s.SnoId == _sceneSnoId);
                    if (scene != null)
                    {
                        _worldScene = scene;
                        _objectiveLocation = _worldScene.GetWorldPosition(_position);
                        Core.Logger.Debug($"Scan found target scene by SnoId {_worldScene.Name} ({_worldScene.SnoId}). Pos={_objectiveLocation} Dist={_objectiveLocation.Distance(AdvDia.MyPosition)} Relative={_position}");
                    }
                }
                else if (!string.IsNullOrEmpty(_sceneName))
                {
                    var scene = Core.Scenes.CurrentWorldScenes.OrderBy(
                        s => s.Center.DistanceSqr(AdvDia.MyPosition.ToVector2()))
                            .FirstOrDefault(
                                s => s.Name.ToLowerInvariant().Contains(_sceneName.ToLowerInvariant()) ||
                                s.SubScene != null && s.SubScene.Name.ToLowerInvariant().Contains(_sceneName.ToLowerInvariant())
                            );

                    if (scene != null)
                    {
                        _worldScene = scene;
                        _objectiveLocation = _worldScene.GetWorldPosition(_position);
                        VisualizerViewModel.DebugPosition = _objectiveLocation;
                        Core.Logger.Debug($"Scan found target scene {_worldScene.Name} ({_worldScene.SnoId}). Pos={_objectiveLocation} Dist={_objectiveLocation.Distance(AdvDia.MyPosition)} Relative={_position}");
                    }
                }
                //else if (!string.IsNullOrEmpty(_tempSceneName))
                //{
                //    var scene = Core.Scenes.CurrentWorldScenes.OrderBy(s => s.Center.DistanceSqr(AdvDia.MyPosition.ToVector2())).FirstOrDefault(s => s.Name == _tempSceneName);
                //    if (scene != null)
                //    {
                //        _worldScene = scene;
                //        _objectiveLocation = _worldScene.GetWorldPosition(_position);
                //    }
                //}
                //if (_objectiveLocation == Vector3.Zero && _actorId != 0)
                //{
                //    _objectiveLocation = BountyHelpers.ScanForActorLocation(_actorId, _objectiveScanRange);
                //}
                //if (_objectiveLocation != Vector3.Zero)
                //{
                //    using (new PerformanceLogger("[MoveToScenePosition] Path to Objective Check", true))
                //    {
                //        //if ((Navigator.GetNavigationProviderAs<Navigator>().CanFullyClientPathTo(_objectiveLocation)))
                //        //{
                //        Core.Logger.Log("[MoveToScenePosition] Found the objective at distance {0}",
                //            AdvDia.MyPosition.Distance(_objectiveLocation));
                //        //}
                //        //else
                //        //{
                //        //    Core.Logger.Debug("[MoveToMapMarker] Found the objective at distance {0}, but cannot get a path to it.",
                //        //        AdvDia.MyPosition.Distance(_objectiveLocation));
                //        //    _objectiveLocation = Vector3.Zero;
                //        //}
                //    }

                //}
            }
        }
    }
}