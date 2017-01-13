using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Exploration.SceneMapping;
using Trinity.DbProvider;
using Trinity.Framework.Helpers;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Coroutines
{
    public sealed class InteractionCoroutine : ISubroutine
    {

        #region State

        public enum States
        {
            NotStarted,
            Checking,
            Interacting,
            TimedOut,
            Completed,
            Failed
        }

        private States _state;
        public States State
        {
            get { return _state; }
            private set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Logger.Debug("[Interaction] " + value);
                }
                _state = value;
            }
        }

        #endregion

        private readonly int _actorId;
        private readonly TimeSpan _timeOut;
        private readonly TimeSpan _sleepTime;
        private readonly int _interactAttempts;
        private int _currentInteractAttempt = 1;
        private DateTime _interactionStartedAt;
        private bool _timeoutCheckEnabled;
        private bool _isPortal;
        private bool _isNephalemStone;
        private bool _isOrek;
        private int _startingWorldId;

        public InteractionCoroutine(int actorId, TimeSpan timeOut, TimeSpan sleepTime, int interactAttempts = 3)
        {
            _actorId = actorId;
            _timeOut = timeOut;
            _sleepTime = sleepTime;
            _interactAttempts = interactAttempts;
            _startingWorldId = ZetaDia.CurrentWorldSnoId;

            if (_timeOut != default(TimeSpan))
            {
                _timeoutCheckEnabled = true;
            }
            if (_sleepTime == default(TimeSpan))
            {
                _sleepTime = new TimeSpan(0, 0, 1);
            }
        }

        public bool IsDone => State == States.Failed || State == States.Completed;

        public async Task<bool> GetCoroutine()
        {
            switch (State)
            {
                case States.NotStarted:
                    return NotStarted();
                case States.Checking:
                    return Checking();
                case States.Interacting:
                    return await Interacting();
                case States.TimedOut:
                    return TimedOut();
                case States.Completed:
                    return Completed();
                case States.Failed:
                    return Failed();
            }
            return false;
        }


        private bool NotStarted()
        {
            State = States.Checking;
            return false;
        }

        private bool Checking()
        {
            var actor = ActorFinder.FindObject(_actorId);
            if (actor == null)
            {
                Logger.Debug("Nothing to interact, failing. ");
                State = States.Failed;
                return false;
            }

            Logger.Debug($"Interact Actor Found: {actor.Name} ({actor.ActorSnoId}) Distance={actor.Distance}");

            if (!actor.IsInteractableQuestObject())
            {
                Logger.Debug("The object is not valid or not interactable, failing.");
                State = States.Failed;
                return false;
            }
            if (actor is DiaGizmo)
            {
                var gizmoActor = (DiaGizmo)actor;
                if (gizmoActor.IsDestructibleObject)
                {

                }
            }

            if (actor is DiaGizmo && (actor as DiaGizmo).IsPortal)
            {
                _isPortal = true;
            }
            if (actor.ActorSnoId == 364715)
            {
                _isNephalemStone = true;
            }
            if (actor.ActorSnoId == 363744)
            {
                _isOrek = true;
            }
            if (_isNephalemStone && UIElements.RiftDialog.IsVisible)
            {
                State = States.Completed;
                return false;
            }
            if (_isOrek && AdvDia.RiftQuest.State == QuestState.Completed)
            {
                State = States.Completed;
                return false;
            }
            State = States.Interacting;
            return false;
        }

        private async Task<bool> Interacting()
        {
            if (ZetaDia.Me.IsFullyValid() && (ZetaDia.Me.CommonData.AnimationState == AnimationState.Casting || ZetaDia.Me.CommonData.AnimationState == AnimationState.Channeling))
            {
                Logger.Debug("Waiting for the cast to end");
                await Coroutine.Sleep(500);
                return false;
            }

            var actor = ActorFinder.FindObject(_actorId);
            if (actor == null)
            {
                Logger.Debug("Nothing to interact, failing. ");
                State = States.Failed;
                return false;
            }
            //if (actor.Distance > 75f)
            //{
            //    Logger.Debug($"Actor is way too far away. {actor.Distance}");
            //    State = States.Failed;
            //    return false;
            //}

            // Assume done
            if (!actor.IsInteractableQuestObject())
            {
                State = States.Completed;
                return false;
            }
            if (_currentInteractAttempt > _interactAttempts)
            {
                Logger.Debug($"Max interrupt attempts reached ({_interactAttempts})");
                State = States.Completed;
                return true;
            }
            if (_currentInteractAttempt > 1)
            {
                Navigator.PlayerMover.MoveTowards(actor.Position);
                await Coroutine.Sleep(250);
                Navigator.PlayerMover.MoveStop();
            }

            if (_isPortal)
            {
                var worldId = ZetaDia.CurrentWorldSnoId;
                if (worldId != _startingWorldId)
                {
                    Logger.Debug($"World changed from {_startingWorldId} to {worldId}, assuming done.");
                    State = States.Completed;
                    return true;
                }
            }

            if (_timeoutCheckEnabled)
            {
                if (_interactionStartedAt == default(DateTime))
                {
                    _interactionStartedAt = DateTime.UtcNow;
                }
                else
                {
                    if (DateTime.UtcNow - _interactionStartedAt > _timeOut)
                    {
                        Logger.Debug("Interaction timed out after {0} seconds", (DateTime.UtcNow - _interactionStartedAt).TotalSeconds);
                        State = States.TimedOut;
                        return false;
                    }
                }
            }

            Logger.Debug($"Attempting to interact with {((SNOActor)actor.ActorSnoId)} at distance {actor.Distance} #{_currentInteractAttempt}");

            var interactionResult = await Interact(actor);

            await Coroutine.Sleep(300);

            if (ActorFinder.IsDeathGate(actor))
            {
                var nearestGate = ActorFinder.FindNearestDeathGate();
                if (nearestGate.CommonData.AnnId != actor.CommonData.AnnId && actor.Distance > 10f)
                {
                    Logger.Debug("Arrived at Gate Destination (AnnId Check)");
                    State = States.Completed;
                    return true;
                }
            }

            // Sleep time would have to be set to 0/low for this to be checked during gate travel.  
            if (ZetaDia.Me.IsUsingDeathGate())
            {
                Logger.Debug("Used Death Gate!");
                await Coroutine.Wait(5000, () => !ZetaDia.Me.IsUsingDeathGate());
                Logger.Debug("Arrived at Gate Destination (Travelling Check)");
                State = States.Completed;
                return true;
            }

            if (interactionResult)
            {
                if (_currentInteractAttempt <= _interactAttempts)
                {
                    _currentInteractAttempt++;
                    return false;
                }
                if (!_isPortal && !_isNephalemStone && !_isOrek && actor.IsInteractableQuestObject())
                {
                    return false;
                }
                if (_isNephalemStone && !UIElements.RiftDialog.IsVisible)
                {
                    return false;
                }
                if (_isOrek && AdvDia.RiftQuest.State != QuestState.Completed)
                {
                    return false;
                }
                State = States.Completed;
            }

            Logger.Debug($"Interaction Failed");
            return false;
        }

        private static bool TimedOut()
        {
            return true;
        }

        private static bool Completed()
        {
            return true;
        }

        private static bool Failed()
        {
            return true;
        }

        private async Task<bool> Interact(DiaObject actor)
        {
            bool retVal = false;
            switch (actor.ActorType)
            {
                case ActorType.Gizmo:
                    switch (actor.ActorInfo.GizmoType)
                    {
                        case GizmoType.BossPortal:
                        case GizmoType.Portal:
                        case GizmoType.ReturnPortal:
                            retVal = ZetaDia.Me.UsePower(SNOPower.GizmoOperatePortalWithAnimation, actor.Position);
                            break;
                        default:
                            retVal = ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, actor.Position);
                            break;
                    }
                    break;
                case ActorType.Monster:
                    retVal = ZetaDia.Me.UsePower(SNOPower.Axe_Operate_NPC, actor.Position);
                    break;
            }

            // Doubly-make sure we interact
            actor.Interact();
            await Coroutine.Sleep(_sleepTime);
            return retVal;
        }


        public void Reset()
        {
            _interactionStartedAt = default(DateTime);
            _timeoutCheckEnabled = false;
            _currentInteractAttempt = 0;
            State = States.NotStarted;
        }

        public void DisablePulse()
        {

        }
    }
}
