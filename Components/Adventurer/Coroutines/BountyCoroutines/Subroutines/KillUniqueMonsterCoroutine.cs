using System;
using System.Linq; using Trinity.Framework;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Trinity.Framework.Helpers;
using Trinity.UI.Visualizer;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;


namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class KillUniqueMonsterCoroutine : IBountySubroutine
    {
        private readonly int _questId;
        private readonly int _worldId;
        private readonly int _actorId;
        private readonly int _marker;
        private bool _isDone;
        private States _state;

        private int _objectiveScanRange = 5000;
        private Vector3 _objectiveLocation = Vector3.Zero;
        private Vector3 _previouslyFoundLocation = Vector3.Zero;
        private long _lastScanTime;
        private long _returnTimeForPreviousLocation;
        private BountyData _bountyData;

        #region State

        public enum States
        {
            NotStarted,
            Searching,
            Moving,
            Found,
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
                    Core.Logger.Log("[KillUniqueMonster] " + value);
                    StatusText = "[KillUniqueMonster] " + value;
                }
                _state = value;
            }
        }

        #endregion State

        public bool IsDone
        {
            get { return _isDone || AdvDia.CurrentWorldId != _worldId || !BountyData.QuestData.IsObjectiveActive(QuestStepObjectiveType.KillMonster); }
        }

        public KillUniqueMonsterCoroutine(int questId, int worldId, int actorId, int marker)
        {
            _questId = questId;
            _worldId = worldId;
            _actorId = actorId;
            _marker = marker;
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

                case States.Found:
                    return await Found();

                case States.Completed:
                    return await Completed();

                case States.Failed:
                    return await Failed();
            }
            return false;
        }

        public void Reset()
        {
            State = States.NotStarted;
            _isDone = false;
            _objectiveScanRange = 5000;
            _objectiveLocation = Vector3.Zero;
            _previouslyFoundLocation = Vector3.Zero;
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
                ScanForObjective();
            }
            if (_objectiveLocation != Vector3.Zero)
            {
                State = States.Moving;
                return false;
            }
            if (!await ExplorationCoroutine.Explore(BountyData.LevelAreaIds)) return false;
            Core.Scenes.Reset();
            return false;
        }

        private async Task<bool> Moving()
        {
            if (!await NavigationCoroutine.MoveTo(_objectiveLocation, 1)) return false;
            if (NavigationCoroutine.LastResult == CoroutineResult.Failure)
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
            State = States.Found;
            _clearAreaForNSecondsCoroutine = new ClearAreaForNSecondsCoroutine(_questId, 10, _actorId, _marker);
            return false;
        }

        private ClearAreaForNSecondsCoroutine _clearAreaForNSecondsCoroutine;

        private async Task<bool> Found()
        {
            if (await _clearAreaForNSecondsCoroutine.GetCoroutine())
            {
                State = States.Completed;
                return false;
            }
            return false;
        }

        private async Task<bool> Completed()
        {
            if (CurrentStepObjective().IsActive)
            {
                State = States.Found;
                _clearAreaForNSecondsCoroutine = new ClearAreaForNSecondsCoroutine(_questId, 10, ZetaDia.Me.ActorSnoId, 0, 100);
                return false;
            }
            _isDone = true;
            return false;
        }

        private async Task<bool> Failed()
        {
            _isDone = true;
            return false;
        }

        private void ScanForObjective()
        {
            if (_previouslyFoundLocation != Vector3.Zero && PluginTime.ReadyToUse(_returnTimeForPreviousLocation, 60000))
            {
                _objectiveLocation = _previouslyFoundLocation;
                _previouslyFoundLocation = Vector3.Zero;
                Core.Logger.Debug("[KillUniqueMonster] Returning previous objective location.");
                return;
            }

            if (PluginTime.ReadyToUse(_lastScanTime, 1000))
            {
                _lastScanTime = PluginTime.CurrentMillisecond;
                if (_marker != 0)
                {
                    _objectiveLocation = BountyHelpers.ScanForMarkerLocation(_marker, _objectiveScanRange);
                }
                if (_objectiveLocation == Vector3.Zero && _actorId != 0)
                {
                    _objectiveLocation = BountyHelpers.ScanForActorLocation(_actorId, _objectiveScanRange);
                }
                if (_objectiveLocation != Vector3.Zero)
                {
                    using (new PerformanceLogger("[KillUniqueMonster] Path to Objective Check", true))
                    {
                        Core.Logger.Log("[KillUniqueMonster] Found the objective at distance {0}",
                            AdvDia.MyPosition.Distance(_objectiveLocation));

                        ExplorationHelpers.SetExplorationPriority(_objectiveLocation);
                    }
                }
            }
        }

        private QuestStepObjectiveData CurrentStepObjective()
        {
            var steps = BountyData.QuestData.Steps.First().Objectives.Where(s => s.ObjectiveType == QuestStepObjectiveType.KillMonster).ToList();
            var bountySteps = BountyData.Coroutines.Where(c => c.GetType() == typeof(KillUniqueMonsterCoroutine)).ToList();
            var objectiveId = bountySteps.IndexOf(this);
            return steps[objectiveId];
        }
    }
}