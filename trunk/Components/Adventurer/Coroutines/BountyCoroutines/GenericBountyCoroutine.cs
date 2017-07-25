using Buddy.Coroutines;
using System;
using Trinity.Framework;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Quests;
using Zeta.Bot;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines
{
    public class GenericBountyCoroutine : BountyCoroutine
    {
        public new enum States
        {
            NotStarted,
            InProgress,
            Completed,
            Failed
        }

        private States _state;

        public new States State
        {
            get { return _state; }
            protected set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Core.Logger.Debug("[GenericBounty] " + value);
                }
                _logStateChange = true;
                _state = value;
            }
        }

        public readonly BountyData Bounty;
        private ISubroutine _currentStep;
        private static ISubroutine _lastGenericBountySubroutine;

        public GenericBountyCoroutine(int questId)
            : base(questId)
        {
            Bounty = BountyDataFactory.GetBountyData(questId);
            ObjectiveSearchRadius = 1000;
            AutoSetNearbyNodesExplored = true;
            AutoSetNearbyNodesRadius = 30;
        }

        public override async Task<bool> GetCoroutine()
        {
            CheckBountyStatus();
            if (base.State == BountyCoroutine.States.BountyMain)
            {
                switch (State)
                {
                    case States.NotStarted:
                        return NotStarted();

                    case States.InProgress:
                        return await InProgress();

                    case States.Completed:
                        return Completed();

                    case States.Failed:
                        return Failed();

                    default:
                        Core.Logger.Error("[GenericBounty] If you see this, it's not good!");
                        await Coroutine.Sleep(TimeSpan.MaxValue);
                        return true;
                }
            }
            return await base.GetCoroutine();
        }

        private bool NotStarted()
        {
            State = States.InProgress;
            BountyData.Reset();
            return false;
        }

        private async Task<bool> InProgress()
        {
            _currentStep = Bounty.Coroutines.FirstOrDefault(b => !b.IsDone);
            if (_currentStep == null)
            {
                //TODO if boss bounty and completed run this
                //new MoveToObjectCoroutine(347558,AdvDia.CurrentWorldSnoId,433670),
                //new InteractWithGizmoCoroutine(347558,AdvDia.CurrentWorldSnoId,433670,0,5),
                _lastGenericBountySubroutine = null;
                State = BountyData.IsAvailable ? States.Failed : States.Completed;
                return false;
            }
            //if (step is EnterLevelAreaCoroutine)
            //{
            //    var enterLevelAreaStep = step as EnterLevelAreaCoroutine;
            //    if (enterLevelAreaStep.SourceWorldId != AdvDia.CurrentWorldSnoId ||
            //        enterLevelAreaStep.DestinationWorldId != AdvDia.CurrentWorldSnoId)
            //    {
            //        Bounty.Reset();
            //        Reset();
            //        base.State = BountyCoroutine.States.TakingWaypoint;
            //        return false;
            //    }
            //}

            _lastGenericBountySubroutine = _currentStep;
            BotMain.SetCurrentStatusTextProvider(() => _lastGenericBountySubroutine.StatusText);
            await _currentStep.GetCoroutine();
            return false;
        }

        public ISubroutine CurrentBountySubroutine => State == States.InProgress ? _currentStep : null;
        public static ISubroutine LastBountySubroutine => _lastGenericBountySubroutine;

        public override void Reset()
        {
            _lastGenericBountySubroutine = null;
            _currentStep = null;
            base.Reset();
        }

        private bool Completed()
        {
            Reset();
            State = States.NotStarted;
            base.State = BountyCoroutine.States.Completed;
            return true;
        }

        private bool Failed()
        {
            Reset();
            State = States.NotStarted;
            base.State = BountyCoroutine.States.Failed;
            return true;
        }
    }
}