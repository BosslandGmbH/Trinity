using System.Linq;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("HaveBounty")]
    public class HaveBountyTag : BaseContainerProfileBehavior
    {
        public override bool StartMethod()
        {
            var bounty = ZetaDia.Storage.Quests.Bounties.FirstOrDefault(b => b.Info.QuestSNO == QuestId && b.Info.State != QuestState.Completed);
            if (bounty != null)
            {
                Core.Logger.Log($"Bounty with QuestId {QuestId} was found. {bounty.Info.DisplayName}.");
                return false;
            }

            Core.Logger.Log($"Bounty with QuestId {QuestId} was not found");
            return true;
        }
    }
}
