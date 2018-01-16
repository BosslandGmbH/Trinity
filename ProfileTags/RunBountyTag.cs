using System.Linq;
using Trinity.Framework;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.QuestTools;
using Zeta.Game;
using Zeta.XmlEngine;
using System.Threading.Tasks;

namespace Trinity.ProfileTags
{
    [XmlElement("RunBounty")]
    public class RunBountyTag : BaseProfileBehavior
    {
        private BountyCoroutine _bounty;

        public override async Task<bool> StartTask()
        {
            var bountyInfo = ZetaDia.Storage.Quests.Bounties.FirstOrDefault(b => (int)b.Quest == QuestId);
            if (bountyInfo == null)
            {
                Core.Logger.Error($"[RunBountyTag] Bounty is not available in this game.");
                return true;
            }

            _bounty = BountyCoroutineFactory.GetBounty(bountyInfo);
            if (_bounty == null)
            {
                Core.Logger.Error($"[RunBountyTag] Bounty is not supported ({QuestId}), ending tag.");
                return true;
            }

            _bounty.Reset();
            return false;
        }

        public override async Task<bool> MainTask()
        {
            if (!_bounty.IsDone && !await _bounty.GetCoroutine())
                return false;

            return true;
        }

    }
}
