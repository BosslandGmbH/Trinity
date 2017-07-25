using Buddy.Coroutines;
using System;
using System.Collections.Generic;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Exploration.SceneMapping;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;


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
                    Core.Logger.Debug("[Interaction] " + value);
                    StatusText = "[Interaction] " + value;
                }
                _state = value;
            }
        }

        #endregion State

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
        private bool _ignoreSanityChecks;
        private string _endAnimation;
        private string _startAnimation;
        private bool _isQuestGiver;
        private DateTime _castWaitStartTime = DateTime.MinValue;
        private int _markerHash;

        public InteractionCoroutine(int actorId, TimeSpan timeOut, TimeSpan sleepTime, int interactAttempts = 3, 
            bool ignoreSanityChecks = false, string startAnimation ="", string endAnimation="", int markerHash = 0)
        {
            _actorId = actorId;
            _timeOut = timeOut;
            _sleepTime = sleepTime;
            _interactAttempts = interactAttempts;
            _ignoreSanityChecks = ignoreSanityChecks;
            _endAnimation = endAnimation;
            _startAnimation = startAnimation;
            _markerHash = markerHash;

            if (_timeOut != default(TimeSpan))
            {
                _timeoutCheckEnabled = true;
            }
            if (_sleepTime == default(TimeSpan))
            {
                _sleepTime = new TimeSpan(0, 0, 1);
            }
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public bool IsDone => State == States.Failed || State == States.Completed;

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            if (Core.Player.IsCastingOrLoading)
            {
                Core.Logger.Log("Waiting for cast to finish.");
                return false;
            }

            switch (State)
            {
                case States.NotStarted:
                    return NotStarted();

                case States.Checking:
                    return await Checking();

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

        private async Task<bool> Checking()
        {
            _startingWorldId = ZetaDia.Globals.WorldSnoId;

            var actor = GetActor();
            if (actor == null || !actor.IsFullyValid())
            {
                Core.Logger.Debug("Nothing to interact, failing. ");
                State = States.Failed;
                return false;
            }

            Core.Logger.Debug($"Interact Actor Found: {actor.Name} ({actor.ActorSnoId}) Distance={actor.Distance}");

            if (!_ignoreSanityChecks && !actor.IsInteractableQuestObject())
            {
                Core.Logger.Debug("The object is not valid or not interactable, failing.");
                State = States.Failed;
                return false;
            }
            
            Core.PlayerMover.MoveTowards(actor.Position);
            if (!Core.Player.IsTakingDamage)
            {
                await Coroutine.Sleep(500);
                Core.PlayerMover.MoveStop();
            }

            // why not eh?
            if (actor.IsFullyValid() && !ActorFinder.IsDeathGate(actor))
            {
                var gizmo = actor as DiaGizmo;
                if (gizmo != null && gizmo.IsFullyValid() && gizmo.IsPortal)
                {
                    _isPortal = true;
                    GameEvents.FireWorldTransferStart();
                }

                actor.Interact();
            }

            var unit = actor as DiaUnit;
            if (unit != null && unit.IsFullyValid())
            {
                _isQuestGiver = unit.IsQuestGiver;
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

        private DiaObject GetActor()
        {
            var trinActor = ActorFinder.FindActor(_actorId, _markerHash);
            var actor = trinActor != null
                ? ZetaDia.Actors.GetActorByACDId(trinActor.AcdId)
                : ActorFinder.FindObject(_actorId);
            return actor;
        }


        private async Task<bool> Interacting()
        {
            if (ZetaDia.Me.IsFullyValid() && (_castWaitStartTime.Subtract(DateTime.UtcNow).TotalSeconds < 10 || _castWaitStartTime == DateTime.MinValue))
            {
                if (ZetaDia.Me.CommonData.AnimationState == AnimationState.Casting)
                {
                    _castWaitStartTime = DateTime.UtcNow;
                    Core.Logger.Debug("Waiting while AnimationState.Casting");
                    await Coroutine.Sleep(500);
                    return false;
                }
                if (ZetaDia.Me.CommonData.AnimationState == AnimationState.Channeling)
                {
                    _castWaitStartTime = DateTime.UtcNow;
                    Core.Logger.Debug("Waiting while  AnimationState.Channeling");
                    await Coroutine.Sleep(500);
                    return false;
                }
            }

            if (ZetaDia.Globals.IsLoadingWorld)
            {
                Core.Logger.Debug("Waiting for world load");
                await Coroutine.Sleep(500);
                return false;
            }

            if (ZetaDia.Globals.WorldSnoId != _startingWorldId)
            {
                Core.Logger.Debug("World changed, assuming done!");
                await Coroutine.Sleep(2500);
                State = States.Completed;
                return false;
            }

            var actor = GetActor();
            if (actor == null)
            {
                Core.Logger.Debug("Nothing to interact, failing. ");
                State = States.Failed;
                return false;
            }

            if (!string.IsNullOrEmpty(_endAnimation))
            {
                var anim = actor.CommonData?.CurrentAnimation.ToString().ToLowerInvariant();
                if (!string.IsNullOrEmpty(anim) && anim.Contains(_endAnimation.ToLowerInvariant()))
                {
                    Core.Logger.Debug($"Specified end animation was detected {_endAnimation}, done!");
                    State = States.Completed;
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(_startAnimation))
            {
                var anim = actor.CommonData?.CurrentAnimation.ToString().ToLowerInvariant();
                if (!string.IsNullOrEmpty(anim) && !anim.Contains(_startAnimation.ToLowerInvariant()))
                {
                    Core.Logger.Debug($"Specified start animation was no longer detected {_startAnimation}, done!");
                    State = States.Completed;
                    return false;
                }
            }

            var unit = actor as DiaUnit;
            if (_isQuestGiver && unit != null && !unit.IsQuestGiver)
            {
                Core.Logger.Debug($"Unit {actor.Name} is no longer a quest giver, assuming done!");
                State = States.Completed;
                return false;
            }

            //if (actor.Distance > 75f)
            //{
            //    Core.Logger.Debug($"Actor is way too far away. {actor.Distance}");
            //    State = States.Failed;
            //    return false;
            //}

            // Assume done
            if (!_ignoreSanityChecks && !actor.IsInteractableQuestObject())
            {
                State = States.Completed;
                return false;
            }

            if (_currentInteractAttempt > _interactAttempts)
            {
                Core.Logger.Debug($"Max interact attempts reached ({_interactAttempts})");
                State = States.Completed;
                return true;
            }

            if (_currentInteractAttempt > 1 && actor.Position.Distance(ZetaDia.Me.Position) > 5f)
            {
                Navigator.PlayerMover.MoveTowards(actor.Position);
                await Coroutine.Sleep(250 * _currentInteractAttempt);
            }

            if (_isPortal || actor.IsFullyValid() && GameData.PortalTypes.Contains(actor.CommonData.GizmoType))
            {
                var worldId = ZetaDia.Globals.WorldSnoId;
                if (worldId != _startingWorldId)
                {
                    Core.Logger.Debug($"World changed from {_startingWorldId} to {worldId}, assuming done.");
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
                        Core.Logger.Debug("Interaction timed out after {0} seconds", (DateTime.UtcNow - _interactionStartedAt).TotalSeconds);
                        State = States.TimedOut;
                        return false;
                    }
                }
            }

            if (!actor.IsFullyValid())
            {
                Core.Logger.Debug($"Actor is no longer valid, assuming done.");
                State = States.Completed;
                return true;
            }

            Core.Logger.Debug($"Attempting to interact with {((SNOActor)actor.ActorSnoId)} at distance {actor.Distance} #{_currentInteractAttempt}");

            var interactionResult = await Interact(actor);

            await Coroutine.Sleep(300);

            if (ActorFinder.IsDeathGate(actor))
            {
                var nearestGate = ActorFinder.FindNearestDeathGate();
                if (nearestGate.CommonData.AnnId != actor.CommonData.AnnId && actor.Distance > 10f)
                {
                    Core.Logger.Debug("Arrived at Gate Destination (AnnId Check)");
                    State = States.Completed;
                    return true;
                }
            }

            //// Sleep time would have to be set to 0/low for this to be checked during gate travel.
            //if (ZetaDia.Me.IsUsingDeathGate())
            //{
            //    Core.Logger.Debug("Used Death Gate!");
            //    await Coroutine.Wait(5000, () => !ZetaDia.Me.IsUsingDeathGate());
            //    Core.Logger.Debug("Arrived at Gate Destination (Travelling Check)");
            //    State = States.Completed;
            //    return true;
            //}

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

            Core.Logger.Debug($"Interaction Failed");
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
            //var world = ZetaDia.Globals.WorldId;
            //bool retVal = false;
            //switch (actor.ActorType)
            //{
            //    case ActorType.Gizmo:
            //        switch (actor.ActorInfo.GizmoType)
            //        {
            //            case GizmoType.BossPortal:
            //            case GizmoType.Portal:
            //            case GizmoType.ReturnPortal:
            //                retVal = ZetaDia.Me.UsePower(SNOPower.GizmoOperatePortalWithAnimation, actor.Position);
            //                break;
            //            default:
            //                retVal = ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, actor.Position);
            //                break;
            //        }
            //        break;
            //    case ActorType.Monster:
            //        retVal = ZetaDia.Me.UsePower(SNOPower.Axe_Operate_NPC, actor.Position);
            //        break;
            //}

            //if (!ZetaDia.Globals.IsLoadingWorld && world == ZetaDia.Globals.WorldId)
            //{
            //Core.Logger.Debug($"Fallback Interaction Used");
            //actor.Interact();
            //}


            var world = ZetaDia.Globals.WorldId;
            await Coroutine.Sleep(100);

            if (actor == null || !actor.IsFullyValid())
                return false;

            var ret = actor.Interact();
            await Coroutine.Sleep(_sleepTime);
            if (_isPortal)
            {
                await Coroutine.Sleep(1000);
            }
            if (!ZetaDia.Globals.IsLoadingWorld && world == ZetaDia.Globals.WorldId)
            {
                await Coroutine.Sleep(400);
                if (actor.IsFullyValid())
                {
                    ret = actor.Interact();
                }
            }
            return ret;
        }

        public void Reset()
        {
            _interactionStartedAt = default(DateTime);
            _timeoutCheckEnabled = false;
            _currentInteractAttempt = 0;
            State = States.NotStarted;
        }

        public string StatusText { get; set; }

        public void DisablePulse()
        {
        }
    }
}