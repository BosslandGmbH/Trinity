using System;
using Trinity.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration.SceneMapping;
using Trinity.Components.Adventurer.Game.Quests;
using Zeta.Common;
using Zeta.Game;


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
        private int _moveAttempts;
        private readonly int _maxFailedMoveToGateAttempts = 8;
        private DateTime _sceneDataBufferStartTime = DateTime.MinValue;

        public Vector3 TargetGatePosition { get; set; }

        public MoveThroughDeathGates(int questId, int worldId, int numberOfGatesToUse = -1)
        {
            _questId = questId;
            _worldId = worldId;
            _gatesToUse = numberOfGatesToUse;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public DeathGateScene CurrentGateScene { get; set; }

        public DeathGateScene TargetGateScene { get; set; }

        public bool IsDone => _isDone || AdvDia.CurrentWorldId != _worldId;

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

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
            _moveAttempts = 0;
            TargetGatePosition = Vector3.Zero;
            TargetGateScene = null;
            CurrentGateScene = null;
            _moveToSceneCoroutine = null;
            _interactionCoroutine = null;
            _sceneDataBufferStartTime = DateTime.MinValue;
        }

        public string StatusText { get; set; }

        public void DisablePulse()
        {
        }

        public BountyData BountyData => _bountyData ?? (_bountyData = BountyDataFactory.GetBountyData(_questId));

        private async Task<bool> NotStarted()
        {
            Core.Logger.Debug($"Started MoveThroughDeathGates ({_gatesToUse})");
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
                    Core.Logger.Debug("Unable to find a gate scene");
                    State = States.Failed;
                }
                return false;
            }

            if (CurrentGateScene.SnoId != ZetaDia.Me.CurrentScene.SceneInfo.SNOId)
            {
                Core.Logger.Debug($"Moving to CurrentGateScene {CurrentGateScene} Distance={CurrentGateScene.Distance}");
                State = States.MovingToScene;
                return false;
            }

            Core.Logger.Debug($"Current Gate Scene is {CurrentGateScene?.Name}.");

            if (!CurrentGateScene.PortalPositions.All(p => _usedGatePositions.Contains(p)))
            {
                TargetGateScene = CurrentGateScene;
            }
            else
            {
                var connectedDeathGateScenes = CurrentGateScene.WorldScene.ConnectedScenes()
                        .Select(cws => DeathGates.Scenes.FirstOrDefault(s => s.WorldScene == cws.Scene)).ToList();

                TargetGateScene = connectedDeathGateScenes.FirstOrDefault(s => s != null && !s.PortalPositions.Any(p => _usedGatePositions.Contains(p)));
            }

            if (TargetGateScene == null)
            {
                Core.Logger.Debug("A TargetGateScene wasn't found, using gate in current scene");
                TargetGateScene = GetClosestSceneWithUnvisitedGate();
            }

            Core.Logger.Debug($"Target Gate Scene is {TargetGateScene?.Name}");

            TargetGatePosition = DeathGates.SelectGate(CurrentGateScene, TargetGateScene);
            if (TargetGatePosition == Vector3.Zero)
            {
                Core.Logger.Debug($"TargetGatePosition was not found from {CurrentGateScene?.Name} to {TargetGateScene?.Name}");
                State = States.Failed;
                return false;
            }

            Core.Logger.Debug($"TargetGatePosition found at distance {TargetGatePosition.Distance(AdvDia.MyPosition)} in scene {TargetGateScene?.Name}");
            State = States.MovingToGate;
            return false;
        }

        private async Task<bool> MovingToScene()
        {
            if (CurrentGateScene == null)
            {
                Core.Logger.Debug("Can't move to unspecified scene");
                State = States.Failed;
                return false;
            }

            Core.Logger.Debug($"Not in the nearest gate scene ({CurrentGateScene.Name}), moving...");
            if (_moveToSceneCoroutine == null || _moveToSceneCoroutine.SceneName != CurrentGateScene.Name)
            {
                _moveToSceneCoroutine = new MoveToSceneCoroutine(_questId, _worldId, CurrentGateScene.Name);
            }

            if (!await _moveToSceneCoroutine.GetCoroutine())
                return false;

            if (CurrentGateScene.SnoId != ZetaDia.Me.CurrentScene.SceneInfo.SNOId)
            {
                Core.Logger.Debug("Failed to move to gate scene, finished.");
                State = States.Failed;
                return false;
            }

            Core.Logger.Debug($"Successfully moved to gate scene {CurrentGateScene.Name}.");
            State = States.Searching;
            return false;
        }

        private NavigationCoroutine _navigationCoroutine;

        private async Task<bool> MovingToGate()
        {
            if (TargetGatePosition == Vector3.Zero)
            {
                Core.Logger.Debug("Can't move to unspecified TargetGatePosition");
                State = States.Failed;
                return false;
            }

            var distance = TargetGatePosition.Distance(AdvDia.MyPosition);
            if (distance > 5f)
            {
                Core.Logger.Debug($"Moving to TargetGatePosition: {TargetGatePosition} in {TargetGateScene.Name} Distance={distance}");

                if (_navigationCoroutine == null || _navigationCoroutine.Destination != TargetGatePosition)
                    _navigationCoroutine = new NavigationCoroutine(TargetGatePosition, 4);

                if (!await _navigationCoroutine.GetCoroutine())
                    return false;

                distance = TargetGatePosition.Distance(AdvDia.MyPosition);
                if (distance > 8f)
                {
                    //if (NavigationCoroutine.LastResult == CoroutineResult.Failure)
                    //{
                    _moveAttempts++;
                    if (_moveAttempts < _maxFailedMoveToGateAttempts)
                    {
                        Core.Logger.Debug($"Failed attempt #{_moveAttempts} moving to {TargetGatePosition} Distance={distance}.");
                        _navigationCoroutine = null;
                        return false;
                    }
                    Core.Logger.Debug($"Failed to move to TargetGatePosition, Max Attempts Reached ({_maxFailedMoveToGateAttempts}), finished.");
                    State = States.Failed;
                    //}
                    //Core.Logger.Debug($"Failed to move to TargetGatePosition, too far away from gate ({distance}). finished.");
                    //State = States.Failed;
                    return false;
                }
            }

            Core.Logger.Debug($"Successfully moved to gate {TargetGatePosition} within {TargetGateScene.Name}.");

            CurrentGateScene = DeathGates.CurrentGateScene;

            if (CurrentGateScene == null)
            {
                Core.Logger.Debug("Unable to find a gate scene");
                State = States.Failed;
                return false;
            }

            Core.Logger.Debug($"Updated current scene to {CurrentGateScene.Name}");
            State = States.Interacting;
            _moveAttempts = 0;
            return false;
        }

        private async Task<bool> Interacting()
        {
            var startSide = DeathGates.MySide;
            var startGate = ActorFinder.FindNearestDeathGate();
            if (startGate == null)
            {
                Core.Logger.Debug("Can't find a gate nearby");
                State = States.Failed;
                return false;
            }
            if (startGate.Distance > startGate.CollisionSphere.Radius)
            {
                Core.Logger.Debug("Gate is too far away to interact");
                State = States.MovingToGate;
                return false;
            }

            if (_interactionCoroutine == null || _interactionCoroutine.IsDone)
            {
                Core.Logger.Debug($"Interacting with {startGate.Name} at {startGate.Position} Distance={startGate.Distance} CurrentScene={CurrentGateScene}");
                _interactionCoroutine = new InteractionCoroutine(startGate.ActorSnoId, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1), 5);
            }

            if (!await _interactionCoroutine.GetCoroutine())
                return false;

            if (_interactionCoroutine.State == InteractionCoroutine.States.Failed)
            {
                Core.Logger.Debug("InteractionCoroutine reports interaction with gate failed");
                State = States.Failed;
                return false;
            }

            if (TargetGatePosition.Distance(AdvDia.MyPosition) <= 10f)
            {
                Core.Logger.Debug("Interaction with gate failed, we didnt move anywhere.");
                State = States.Failed;
                return false;
            }

            _gatesUsed++;

            //var endGateInteractPos = DeathGates.NearestGateToPosition(endGate.Position);
            var endGateInteractPos = CurrentGateScene.PortalPositions.FirstOrDefault(p => p != TargetGatePosition);

            Core.Logger.Debug($"Ignoring Gate at {endGateInteractPos} within {CurrentGateScene.Name} {DeathGates.MySide}");
            _usedGatePositions.Add(endGateInteractPos);

            Core.Logger.Debug($"Ignoring Gate at {TargetGatePosition} within {CurrentGateScene.Name} {startSide}");
            _usedGatePositions.Add(TargetGatePosition);

            Core.Logger.Debug($"Gate #{_gatesUsed} at {TargetGatePosition} within {CurrentGateScene.Name} ({startSide}=>{DeathGates.MySide}) was used successfully");
            if (_gatesToUse >= 0 && _gatesUsed >= _gatesToUse)
            {
                Core.Logger.Debug($"We've used all the gates we're supposed to ({_gatesUsed})");
                State = States.Completed;
                return false;
            }
            State = States.Searching;
            return false;
        }

        private DeathGateScene GetClosestSceneWithUnvisitedGate()
        {
            return DeathGates.Scenes.Where(s => s != CurrentGateScene && s.PortalPositions.Any(p => !_usedGatePositions.Contains(p)))
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
            get => _state;
            protected set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                    Core.Logger.Log("[MoveThroughDeathGates] " + value);
                    StatusText = "[MoveThroughDeathGates] " + value;
                _state = value;
            }
        }

        #endregion State
    }
}