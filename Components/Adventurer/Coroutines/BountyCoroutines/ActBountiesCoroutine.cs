using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Framework;
using Zeta.Bot;
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
                    Core.Logger.Debug("[ActBounties] " + value);
                }
                _logStateChange = true;
                _state = value;
            }
        }

        #endregion State

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
                    //if (TrinityPluginSettings.Settings.Advanced.BetaPlayground)
                    //{
                    //    try
                    //    {
                    //        if (Core.Player.IsInParty)
                    //        {
                    //            if (BountyCoroutine.currentRandomizedBounty < 0 ||
                    //                BountyCoroutine.currentRandomizedBounty > _bountyCoroutines.Count)
                    //            {
                    //                BountyCoroutine.currentRandomizedBounty =
                    //                    Randomizer.GetRandomNumber(_bountyCoroutines.Count);
                    //                Core.Logger.Log("[ActBounties] Randomized Bounty Complete.", Act);
                    //            }
                    //        }
                    //        else
                    //            BountyCoroutine.currentRandomizedBounty = 0;

                    //        if (BountyCoroutine.currentRandomizedBounty < 0 ||
                    //            BountyCoroutine.currentRandomizedBounty > _bountyCoroutines.Count)
                    //            BountyCoroutine.currentRandomizedBounty = 0;

                    //        _currentBountyCoroutine = _bountyCoroutines[BountyCoroutine.currentRandomizedBounty];
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Core.Logger.Log(BountyCoroutine.currentRandomizedBounty + " | " + _bountyCoroutines.Count, Act);
                    //        BountyCoroutine.currentRandomizedBounty = 0;
                    //        _currentBountyCoroutine = _bountyCoroutines[0];
                    //    }
                    //}
                    //else
                    //{
                    _currentBountyCoroutine = _bountyCoroutines[0];
                    //}
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
            Core.Scenes.Reset();
            if (_currentBountyCoroutine.State == BountyCoroutine.States.Failed)
            {
                //Core.Logger.Log("[ActBounties] Looks like the bounty has failed, skipping the rest of the act.");
                State = States.Failed;
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
            BountyStatistics.CompletedBountyActs++;
            Core.Logger.Log("[章节悬赏] 成功完成 {0} 悬赏.", Act);
            return true;
        }

        private async Task<bool> UnsupportedBountyFound()
        {
            Core.Logger.Log("[章节悬赏] 我们遇到不支持的任务在{0}，跳过此章节.", Act);
            BountyStatistics.RestartsFromUnsupported++;
            return true;
        }

        private async Task<bool> ActIsDisabled()
        {
            Core.Logger.Log("[章节悬赏] {0} 在设置中被禁用, ，跳过此章节.", Act);
            return true;
        }

        private async Task<bool> Failed()
        {
            Core.Logger.Error("[奖励悬赏] {0}不是奖励章节,跳过这个章节.", Act);
            return true;
        }
    }
}