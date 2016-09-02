using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("RunActBounties")]
    public class RunActBountiesTag : ProfileBehavior
    {
        private static readonly HashSet<Act> ValidActs = new HashSet<Act> { Act.A1, Act.A2, Act.A3, Act.A4, Act.A5 };


        [XmlAttribute("act")]
        [DefaultValue(Act.OpenWorld)]
        public Act Act { get; set; }

        private BountyCoroutine _currentBounty;
        private CompleteActBountiesCoroutine _completeActBountiesCoroutine;
        private List<BountyCoroutine> _bounties;

        private bool _isDone;

        public override bool IsDone
        {
            get
            {
                return _isDone;
            }
        }

        public override void OnStart()
        {
            //AdvDia.Update(true);
            if (!ValidActs.Contains(Act))
            {
                Logger.Error("[RunActBounties] Invalid act, valid acts are {0}", string.Join(", ", ValidActs));
                _isDone = true;
            }
            _bounties = BountyCoroutineFactory.GetActBounties(Act).OrderBy(a => a.QuestData.Act).ThenBy(a => a.QuestData.InternalName).ToList();
            _completeActBountiesCoroutine = new CompleteActBountiesCoroutine(Act);
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => GetCoroutine());
        }

        private async Task<bool> GetCoroutine()
        {
            if (_isDone)
            {
                return true;
            }

            if (PluginEvents.TimeSinceWorldChange < 1000)
            {
                Logger.Debug("[RunActBountiesTag] Sleeping 1 second due to world change");
                await Coroutine.Sleep(1000);
            }

            if (_bounties == null || _bounties.Count == 0)
            {
                if (BountyHelpers.AreAllActBountiesCompleted(Act))
                {
                    if (await _completeActBountiesCoroutine.GetCoroutine())
                    {
                        _isDone = true;
                        return true;
                    }
                    return true;
                }
                _isDone = true;
                return true;
            }
            _currentBounty = _bounties.FirstOrDefault();
            if (_currentBounty != null)
            {
                if (_currentBounty.State != BountyCoroutine.States.Completed && _currentBounty.State != BountyCoroutine.States.Failed)
                {
                    return await _currentBounty.GetCoroutine();
                }
                ScenesStorage.Reset();

                _bounties.Remove(_currentBounty);
                BountyStatistics.Report();
            }
            return true;

        }

        //public override void OnDone()
        //{
        //    if (_bounty != null)
        //    {
        //        _bounty.ResetState();
        //    }
        //}

        //public override void ResetCachedDone(bool force = false)
        //{
        //    _currentBounty = null;
        //    _bounties = null;
        //    _isDone = false;
        //}
    }
}
