using System;
using Trinity.Framework;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Trinity.Components.Coroutines;
using Zeta.Common;


namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class InteractWithGizmoCoroutine : IBountySubroutine
    {
        private readonly int _questId;
        private readonly int _worldId;
        private readonly int _actorId;
        private readonly int _marker;
        private readonly int _interactAttemps;
        private readonly int _secondsToSleepAfterInteraction;
        private readonly int _secondsToTimeout;

        private bool _isDone;
        private States _state;
        private InteractionCoroutine _interactionCoroutine;
        private int _objectiveScanRange = 5000;

        #region State

        public enum States
        {
            NotStarted,
            Searching,
            Moving,
            Interacting,
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
                    Core.Logger.Log("[互动] " + value);
                    StatusText = "[互动] " + value;
                }
                _state = value;
            }
        }

        #endregion State

        public bool IsDone
        {
            get { return _isDone || AdvDia.CurrentWorldId != _worldId && _worldId != -1; }
        }

        public InteractWithGizmoCoroutine(int questId, int worldId, int actorId, int marker, int interactAttemps = 1, int secondsToSleepAfterInteraction = 1, int secondsToTimeout = 10, bool useAll = false)
        {
            _questId = questId;
            _worldId = worldId;
            _actorId = actorId;
            _marker = marker;
            _interactAttemps = interactAttemps;
            _secondsToSleepAfterInteraction = secondsToSleepAfterInteraction;
            _secondsToTimeout = secondsToTimeout;
            _useAll = useAll;
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
                ScanForObjective();
            }
            if (_objectiveLocation != Vector3.Zero)
            {
                // Special case for cursed chest events.
                if (_objectiveLocation.Distance(AdvDia.MyPosition) < 16f && _actorId == 365097 && ActorFinder.FindGizmo(364559) != null)
                {
                    Core.Logger.Log("Target gizmo has transformed into invulnerable event gizmo. Ending.");
                    State = States.Failed;
                    return false;
                }

                State = States.Moving;
                return false;
            }
            if (!await ExplorationCoroutine.Explore(BountyData.LevelAreaIds)) return false;
            Core.Scenes.Reset();
            return false;
        }

        private async Task<bool> Moving()
        {
            if (!await NavigationCoroutine.MoveTo(_objectiveLocation, 7)) return false;
            if (NavigationCoroutine.LastResult == CoroutineResult.Failure)
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
            var actor = ActorFinder.FindGizmo(_actorId);
            if (actor == null)
            {
                Core.Logger.Debug("没有发现互动目标.");
                State = States.Searching;
                return false;
            }
            State = States.Interacting;
            _interactionCoroutine = new InteractionCoroutine(actor.ActorSnoId, new TimeSpan(0, 0, _secondsToTimeout),
                new TimeSpan(0, 0, _secondsToSleepAfterInteraction), _interactAttemps);
            if (!actor.IsInteractableQuestObject())
            {
                Core.Logger.Error($"无法互动: {actor.Name} | 距离: {actor.Distance}.");
                ActorFinder.InteractWhitelist.Add(actor.ActorSnoId);
            }
            return false;
        }

        private async Task<bool> Interacting()
        {
            //if (_interactionCoroutine.State == InteractionCoroutine.States.NotStarted)
            //{
            //    var portalGizmo = BountyHelpers.GetPortalNearMarkerPosition(_markerPosition);
            //    if (portalGizmo == null)
            //    {
            //        Core.Logger.Debug("[Bounty] No portal nearby, keep exploring .");
            //        State = States.SearchingForDestinationWorld;
            //        return false;
            //    }
            //    _interactionCoroutine.DiaObject = portalGizmo;
            //}

            Core.Logger.Debug("[InteractWithGizmo] Starting interaction subroutine.");

            if (await _interactionCoroutine.GetCoroutine())
            {
                ActorFinder.InteractWhitelist.Remove(_actorId);
                if (_interactionCoroutine.State == InteractionCoroutine.States.TimedOut)
                {
                    Core.Logger.Debug("[互动] 互动超时.");
                    State = States.Failed;
                    return false;
                }

                if (_useAll)
                {
                    var nextGizmo = ActorFinder.FindGizmo(_actorId, gizmo => gizmo.IsInteractableQuestObject());
                    if (nextGizmo != null)
                    {
                        Core.Logger.Warn("发现另一个互动目标. 距离={0}", nextGizmo.Distance);
                        State = States.Searching;
                        return false;
                    }
                }

                var actor = ActorFinder.FindGizmo(_actorId);
                if (actor == null)
                {
                    Core.Logger.Log($"没有发现互动目标.");
                    State = States.Failed;
                    return false;
                }

                if (await (MoveToAndInteract.Execute(actor)))
                {
                    State = States.Completed;
                    _interactionCoroutine = null;
                    return false;
                }
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
        private Vector3 _lastSeenLocation = Vector3.Zero;
        private long _lastScanTime;
        private BountyData _bountyData;
        private bool _useAll;

        private void ScanForObjective()
        {
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
                    Core.Logger.Log("[InteractWithGizmo] Found the objective at distance {0}", AdvDia.MyPosition.Distance(_objectiveLocation));
                }
            }
        }
    }
}