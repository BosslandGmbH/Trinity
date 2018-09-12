using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.Util;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class MoveToSceneCoroutine : IBountySubroutine
    {
        private readonly int _questId;
        private readonly int _worldId;
        private BountyData _bountyData;
        private bool _explore;

        private bool _isDone;
        private long _lastObjectiveFoundTime;

        private long _lastScanTime;

        private Vector3 _objectiveLocation = Vector3.Zero;

        private int _objectiveScanRange = 5000;
        private Vector3 _previouslyFoundLocation = Vector3.Zero;
        private long _returnTimeForPreviousLocation;
        private WorldScene _scene;
        private States _state;
        private readonly bool _zergEnabled;
        private int _failureCount;

        public MoveToSceneCoroutine(int questId, int worldId, int sceneSnoId, bool zergSafe = false, bool explore = true)
        {
            _questId = questId;
            _worldId = worldId;
            SceneName = ZetaDia.SNO.LookupSNOName(SNOGroup.Scene, sceneSnoId);
            _zergEnabled = zergSafe;
            _explore = explore;
            Id = Guid.NewGuid();

        }

        public MoveToSceneCoroutine(int questId, int worldId, string sceneName, bool zergSafe = false, bool explore = true)
        {
            _questId = questId;
            _worldId = worldId;
            SceneName = sceneName;
            _zergEnabled = zergSafe;
            _explore = explore;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public string SceneName { get; }

        public bool IsDone => _isDone || AdvDia.CurrentWorldId != _worldId;

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            CoroutineCoodinator.Current = this;

            if (_scene != null && _scene.IsInScene(AdvDia.MyPosition))
            {
                Core.Logger.Debug($"Currently in Target Scene: {_scene.Name}. IsSubScene={_scene.SubScene}");
                State = States.Completed;
            }

            if (PluginSettings.Current.BountyZerg && BountyData != null)
            {
                SafeZerg.Instance.EnableZerg();
            }
            else
            {
                SafeZerg.Instance.DisableZerg();
            }

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
            _failureCount = 0;
        }

        public string StatusText { get; set; }

        public void DisablePulse()
        {
        }

        public BountyData BountyData => _bountyData ?? (_bountyData = BountyDataFactory.GetBountyData(_questId));

        private async Task<bool> NotStarted()
        {
            SafeZerg.Instance.DisableZerg();
            State = States.Searching;
            return false;
        }

        private async Task<bool> Searching()
        {
            SafeZerg.Instance.DisableZerg();

            if (_objectiveLocation == Vector3.Zero)
                ScanForObjective();
            if (_objectiveLocation != Vector3.Zero)
            {
                State = States.Moving;
                return false;
            }
            var levelArea = BountyData != null ? BountyData.LevelAreaIds : new HashSet<int> {ZetaDia.CurrentLevelAreaSnoId};
            if (!await ExplorationCoroutine.Explore(levelArea)) return false;
            Core.Scenes.Reset();
            return false;
        }

        private async Task<bool> Moving()
        {
            if (AdvDia.CurrentWorldScene.Name.ToLower().Contains(SceneName.ToLower()))
            {
                State = States.Completed;
                return false;
            }

            if (_zergEnabled)
                SafeZerg.Instance.EnableZerg();

            if (await NavigationCoroutine.MoveTo(_objectiveLocation, 10))
            {
                if (AdvDia.MyPosition.Distance(_objectiveLocation) > 30 && NavigationCoroutine.LastResult == CoroutineResult.Failure && _failureCount < 10)
                {
                    _failureCount++;
                    _previouslyFoundLocation = _objectiveLocation;
                    _returnTimeForPreviousLocation = PluginTime.CurrentMillisecond;
                    _objectiveLocation = Vector3.Zero;
                    _objectiveScanRange = Math.Max(ActorFinder.LowerSearchRadius(_objectiveScanRange),250);
                    Core.Logger.Log($"Search Radius changed to  {_objectiveScanRange}");
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
            SafeZerg.Instance.DisableZerg();
            _isDone = true;
            return true;
        }

        private async Task<bool> Failed()
        {
            SafeZerg.Instance.DisableZerg();
            _isDone = true;
            return true;
        }

        private void ScanForObjective()
        {
            if (_previouslyFoundLocation != Vector3.Zero && PluginTime.ReadyToUse(_returnTimeForPreviousLocation, 60000))
            {
                _objectiveLocation = _previouslyFoundLocation;
                _previouslyFoundLocation = Vector3.Zero;
                Core.Logger.Debug("[MoveToScene] Returning previous objective location.");
                return;
            }
            if (PluginTime.ReadyToUse(_lastScanTime, 1000))
            {
                _lastScanTime = PluginTime.CurrentMillisecond;
                if (!string.IsNullOrEmpty(SceneName))
                {
                    _scene = Core.Scenes.CurrentWorldScenes.OrderBy(s => s.Center.DistanceSqr(AdvDia.MyPosition.ToVector2())).FirstOrDefault(s => s.Name.ToLowerInvariant().Contains(SceneName.ToLowerInvariant()) || s.HasChild && s.SubScene.Name.ToLowerInvariant().Contains(SceneName.ToLowerInvariant()));
                    var centerNode =
                        _scene?.Nodes.Where(n => n.HasEnoughNavigableCells)
                            .OrderBy(n => n.Center.DistanceSqr(_scene.Center))
                            .FirstOrDefault();
                    if (centerNode != null)
                        _objectiveLocation = centerNode.NavigableCenter;
                }
                if (_objectiveLocation != Vector3.Zero && PluginTime.ReadyToUse(_lastObjectiveFoundTime, 20000))
                {
                    _lastObjectiveFoundTime = PluginTime.CurrentMillisecond;

                    using (new PerformanceLogger("[MoveToScene] Path to Objective Check", true))
                    {
                        Core.Logger.Log("[MoveToScene] Found the objective at distance {0}",
                            AdvDia.MyPosition.Distance(_objectiveLocation));

                        ExplorationHelpers.SetExplorationPriority(_objectiveLocation);
                    }
                }
                else
                {
                    _objectiveLocation = Vector3.Zero;
                }
            }
        }

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
            get => _state;
            protected set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                    Core.Logger.Log("[MoveToScene] " + value);
                    StatusText = "[MoveToScene] " + value;
                _state = value;
            }
        }

        #endregion State
    }
}