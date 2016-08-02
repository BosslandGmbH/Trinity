using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines
{
    public class ActBountiesCoroutine
    {
        public readonly Act Act;
        private States _state;
        private CompleteActBountiesCoroutine _completeActBountiesCoroutine;
        private List<BountyCoroutine> _bountyCoroutines;
        private BountyCoroutine _currentBountyCoroutine;


        #region State

        public enum States
        {
            NotStarted,
            RunningBounties,
            TurningInTheActQuest,
            Completed,
            UnsupportedBountyFound,
            ActIsDisabled,
            Failed
        }

        protected bool _logStateChange;
        protected bool LogStateChange
        {
            get
            {
                if (!_logStateChange) return false;
                _logStateChange = false;
                return true;
            }
        }

        public States State
        {
            get { return _state; }
            protected set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Logger.Debug("[ActBounties] " + value);
                }
                _logStateChange = true;
                _state = value;
            }
        }

        #endregion

        public ActBountiesCoroutine(Act act)
        {
            Act = act;
            _completeActBountiesCoroutine = new CompleteActBountiesCoroutine(act);
            _bountyCoroutines = BountyCoroutineFactory.GetActBounties(act);
        }

        public virtual async Task<bool> GetCoroutine()
        {
            switch (State)
            {
                case States.NotStarted:
                    return await NotStarted();

                case States.RunningBounties:
                    return await RunningBounties();

                case States.TurningInTheActQuest:
                    return await TurningInTheActQuest();

                case States.Completed:
                    return await Completed();

                case States.UnsupportedBountyFound:
                    return await UnsupportedBountyFound();

                case States.ActIsDisabled:
                    return await ActIsDisabled();

                case States.Failed:
                    return await Failed();
            }
            return false;
        }


        private async Task<bool> NotStarted()
        {
            //if (!BountyHelpers.IsActEnabledOnSettings(Act))
            //{
            //    State = States.ActIsDisabled;
            //    return false;
            //}
            if (BountyHelpers.IsActTurninCompleted(Act))
            {
                State = States.Completed;
                return false;
            }
            if (BountyHelpers.IsActTurninInProgress(Act))
            {
                State = States.TurningInTheActQuest;
                return false;
            }
            if (!BountyHelpers.AreAllActBountiesSupported(Act))
            {
                State = States.UnsupportedBountyFound;
                return false;
            }
            State = States.RunningBounties;
            return false;

        }

        private async Task<bool> RunningBounties()
        {
            if (_currentBountyCoroutine == null)
            {
                if (_bountyCoroutines.Count != 0)
                {
                    _currentBountyCoroutine = _bountyCoroutines[0];
                }
                else
                {
                    if (BountyHelpers.IsActTurninInProgress(Act))
                    {
                        State = States.TurningInTheActQuest;
                        return false;
                    }
                    State = States.Failed;
                    return false;
                }
            }
            if (!await _currentBountyCoroutine.GetCoroutine()) return false;
            BountyStatistics.Report();
            ScenesStorage.Reset();
            if (_currentBountyCoroutine.State == BountyCoroutine.States.Failed)
            {
                //Logger.Info("[ActBounties] Looks like the bounty has failed, skipping the rest of the act.");
                State=States.Failed;
                return false;
            }
            _bountyCoroutines.Remove(_currentBountyCoroutine);
            _currentBountyCoroutine = null;
            return false;
        }

        private async Task<bool> TurningInTheActQuest()
        {
            if (!await _completeActBountiesCoroutine.GetCoroutine()) return false;
            if (BountyHelpers.IsActTurninCompleted(Act))
            {
                State = States.Completed;
            }
            else
            {
                _completeActBountiesCoroutine = new CompleteActBountiesCoroutine(Act);
            }
            return false;
        }

        private async Task<bool> Completed()
        {
            Logger.Info("[ActBounties] Successfully completed {0} bounties.", Act);
            return true;
        }


        private async Task<bool> UnsupportedBountyFound()
        {
            Logger.Info("[ActBounties] It seems like we have an unsupported bounty in {0}, skipping the act.", Act);
            return true;

        }

        private async Task<bool> ActIsDisabled()
        {
            Logger.Info("[ActBounties] {0} is disabled on settings, skipping the act.", Act);
            return true;

        }


        private async Task<bool> Failed()
        {
            Logger.Error("[ActBounties] Act bounties failed for {0}, skipping the act.", Act);
            return true;
        }
    }
}
