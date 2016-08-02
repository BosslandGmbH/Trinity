using System.Linq;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.Components.Adventurer.Tags
{
    [XmlElement("RunBounty")]
    public class RunBountyTag : ProfileBehavior
    {
        private BountyCoroutine _bounty;

        private bool _isDone;

        public override bool IsDone
        {
            get
            {
                return _isDone || (_bounty != null && _bounty.IsDone);
            }
        }

        public override void OnStart()
        {
            var bountyInfo = ZetaDia.ActInfo.Bounties.FirstOrDefault(b => (int)b.Quest == QuestId);

            _bounty = BountyCoroutineFactory.GetBounty(bountyInfo);
            if (_bounty == null)
            {
                Logger.Error("[RunBountyTag] Unsupported QuestId ({0}), ending tag.", QuestId);
                _isDone = true;
            }
        }

        protected override Composite CreateBehavior()
        {
            if (_bounty == null)
            {
                return null;
            }
            return new ActionRunCoroutine(ctx => _bounty.GetCoroutine());
        }

        //public override void OnDone()
        //{
        //    if (_bounty != null)
        //    {
        //        _bounty.ResetState();
        //    }
        //}

        public override void ResetCachedDone(bool force = false)
        {
            _bounty = null;
            _isDone = false;
        }
    }
}
