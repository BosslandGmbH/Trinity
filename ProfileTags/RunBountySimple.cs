using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.QuestTools;
using Zeta.Game;
using Zeta.XmlEngine;
using System.Threading.Tasks;
using Zeta.Game.Internals;

namespace Trinity.ProfileTags
{
    [XmlElement("RunBountySimple")]
    public class RunBountyMaterialTag : BaseProfileBehavior
    {
        private BountyCoroutine _bounty;
        private List<SNOQuest> _blacklist = new List<SNOQuest>();

        [XmlAttribute("act")]
        public Act Act { get; set; }

        public override async Task<bool> StartTask()
        {
            var bounty = ZetaDia.Storage.Quests.Bounties
                .Where(c => c.Act == Act && c.Info.State != QuestState.Completed && !_blacklist.Contains(c.Quest))
                .OrderByDescending(c => (int)c.Quest)
                .FirstOrDefault();

            // Bounties completed in this Act.
            if (bounty == null)
                return true;

            _bounty = BountyCoroutineFactory.GetBounty(bounty);
            if (_bounty == null)
            {
                Core.Logger.Error($"[RunBountySimple] Bounty is not supported ({QuestId}), ending tag.");
                _blacklist.Add(bounty.Quest);

                // Pick the next...
                return await StartTask();
            }

            _bounty.Reset();
            return false;
        }

        public override async Task<bool> MainTask()
        {
            if (!_bounty.IsDone && !await _bounty.GetCoroutine())
                return false;

            _bounty = null;
            return await StartTask();
        }

    }
}
