using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration.SceneMapping;
using Trinity.Components.Adventurer.Game.Quests;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class MoveThroughDeathGates : IBountySubroutine
    {
        private readonly int _questId;
        private readonly int _worldId;
        private BountyData _bountyData;
        private readonly int _gatesToUse;
        private int _gatesUsed;
        private bool _isDone;
        private States _state;
        private readonly HashSet<Vector3> _usedGatePositions = new HashSet<Vector3>();
        private MoveToSceneCoroutine _moveToSceneCoroutine;
        private InteractionCoroutine _interactionCoroutine;
        private int _failedMoveToGateAttempts;
        private int _maxFailedMoveToGateAttempts = 5;
        private DateTime _sceneDataBufferStartTime = DateTime.MinValue;

        public Vector3 TargetGatePosition { get; set; }

        public MoveThroughDeathGates(int questId, int worldId, int numberOfGatesToUse = -1)
        {
            _questId = questId;
            _worldId = worldId;
            _gatesToUse = numberOfGatesToUse;
        }

        public DeathGateScene CurrentGateScene { get; set; }

        public DeathGateScene TargetGateScene { get; set; }

        public bool IsDone => _isDone || AdvDia.CurrentWorldId != _worldId;

        public async Task<bool> GetCoroutine()
        {
            switch (State)
            {
                case States.NotStarted:
                    return await NotStarted();
                case States.Searching:
                    return await Searching();
                case States.MovingToScene:
                    return await MovingToScene();
                case States.MovingToGate:
                    return await MovingToGate();
                case States.Interacting:
                    return await Interacting();
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
            _usedGatePositions.Clear();
            _gatesUsed = 0;
            _failedMoveToGateAttempts = 0;
            TargetGatePosition = Vector3.Zero;
            TargetGateScene = null;
            CurrentGateScene = null;
            _moveToSceneCoroutine = null;
            _interactionCoroutine = null;
            _sceneDataBufferStartTime = DateTime.MinValue;
        }

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
            CurrentGateScene = DeathGates.CurrentGateScene;

            if (CurrentGateScene == null)
            {
                if (_sceneDataBufferStartTime == DateTime.MinValue)
                {
                    _sceneDataBufferStartTime = DateTime.UtcNow;
                }
                if (DateTime.UtcNow.Subtract(_sceneDataBufferStartTime).TotalSeconds > 15)
                {
                    Logger.Debug("Unable to find a gate scene");
                    State = States.Failed;
                }
                return false;
            }

            if (CurrentGateScene.SnoId != ZetaDia.Me.CurrentScene.SceneInfo.SNOId)
            {
                Logger.Debug($"Moving to CurrentGateScene {CurrentGateScene} Distance={CurrentGateScene.Distance}");
                State = States.MovingToScene;
                return false;
            }

            TargetGateScene = GetClosestSceneWithUnvisitedGate();
            if (TargetGateScene == null)
            {
                Logger.Debug("A TargetGateScene wasn't found, using gate in current scene");
                TargetGateScene = CurrentGateScene;
            }

            TargetGatePosition = DeathGates.SelectGate(CurrentGateScene, TargetGateScene);
            if (TargetGatePosition == Vector3.Zero)
            {
                Logger.Debug($"TargetGatePosition was not found from {CurrentGateScene?.Name} to {TargetGateScene?.Name}");
                State = States.Failed;
                return false;
            }

            Logger.Debug($"TargetGatePosition found at distance {TargetGatePosition.Distance(AdvDia.MyPosition)} in scene {TargetGateScene.Name}");
            State = States.MovingToGate;
            return false;
        }

        private async Task<bool> MovingToScene()
        {
            if (CurrentGateScene == null)
            {
                Logger.Debug("Can't move to unspecified scene");
                State = States.Failed;
                return false;
            }

            Logger.Debug("Not in the nearest gate scene, moving...");
            if (_moveToSceneCoroutine == null || _moveToSceneCoroutine.SceneName != CurrentGateScene.Name)
            {
                _moveToSceneCoroutine = new MoveToSceneCoroutine(_questId, _worldId, CurrentGateScene.Name);
            }

            if (!await _moveToSceneCoroutine.GetCoroutine())
                return false;

            if (CurrentGateScene.SnoId != ZetaDia.Me.CurrentScene.SceneInfo.SNOId)
            {
                Logger.Debug("Failed to move to gate scene, finished.");
                State = States.Failed;
                return false;
            }

            Logger.Debug($"Inside gate scene {CurrentGateScene.Name}.");
            State = States.Searching;
            return false;
        }

        private async Task<bool> MovingToGate()
        {
            if (TargetGatePosition == Vector3.Zero)
            {
                Logger.Debug("Can't move to unspecified TargetGatePosition");
                State = States.Failed;
                return false;
            }
            if (TargetGatePosition.Distance(AdvDia.MyPosition) > 5f)
            {
                Logger.Debug("Moving to TargetGatePosition...");

                if (!await NavigationCoroutine.MoveTo(TargetGatePosition, 4))
                    return false;

                if (TargetGatePosition.Distance(AdvDia.MyPosition) > 5f)
                {
                    if (NavigationCoroutine.LastResult == CoroutineResult.Failure)
                    {
                        _failedMoveToGateAttempts++;
                        if (_failedMoveToGateAttempts <= _maxFailedMoveToGateAttempts)
                        {
                            Logger.Debug($"Failed attempt #{_failedMoveToGateAttempts} to move to TargetGatePosition...");
                            return false;
                        }
                    }

                    Logger.Debug("Failed to move to TargetGatePosition, finished.");
                    State = States.Failed;
                    return false;
                }
            }
            Logger.Debug($"Inside gate scene {CurrentGateScene.Name}.");
            State = States.Interacting;
            return false;
        }

        private async Task<bool> Interacting()
        {
            var startGate = ActorFinder.FindNearestDeathGate();
            if (startGate == null)
            {
                Logger.Debug("Can't find a gate nearby");
                State = States.Failed;
                return false;
            }
            if (startGate.Distance > startGate.CollisionSphere.Radius)
            {
                Logger.Debug("Gate is too far away to interact");
                State = States.MovingToGate;
                return false;
            }

            if (_interactionCoroutine == null || _interactionCoroutine.IsDone)
            {
                _interactionCoroutine = new InteractionCoroutine(startGate.ActorSnoId, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1), 5);
            }

            if (!await _interactionCoroutine.GetCoroutine())
                return false;

            if (_interactionCoroutine.State == InteractionCoroutine.States.Failed)
            {
                Logger.Debug("InteractionCoroutine reports interaction with gate failed");
                State = States.Failed;
                return false;
            }

            var endGate = ActorFinder.FindNearestDeathGate();
            if (TargetGatePosition.Distance(AdvDia.MyPosition) <= 10f)
            {
                Logger.Debug("Interaction with gate failed, we didnt move anywhere.");
                State = States.Failed;
                return false;
            }

            _gatesUsed++;
            var endGateInteractPos = DeathGates.NearestGateToPosition(endGate.Position);
            _usedGatePositions.Add(endGateInteractPos);
            _usedGatePositions.Add(TargetGatePosition);

            Logger.Debug($"Gate #{_gatesUsed} was used successfully");
            if (_gatesToUse >= 0 && _gatesUsed >= _gatesToUse)
            {
                Logger.Debug($"We've used all the gates we're supposed to ({_gatesUsed})");
                State = States.Completed;
                return false;
            }
            State = States.Searching;
            return false;
        }

        private DeathGateScene GetClosestSceneWithUnvisitedGate()
        {
            return
                DeathGates.Scenes.Where(
                        s => s != CurrentGateScene && s.PortalPositions.Any(p => !_usedGatePositions.Contains(p)))
                    .OrderBy(s => s.Distance)
                    .FirstOrDefault();
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

        #region State

        public enum States
        {
            NotStarted,
            Searching,
            Moving,
            Completed,
            Failed,
            MovingToScene,
            MovingToGate,
            Interacting
        }

        public States State
        {
            get { return _state; }
            protected set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                    Logger.Info("[MoveThroughDeathGates] " + value);
                _state = value;
            }
        }

        #endregion
    }
}