using System.Collections.Generic;
using System.Linq; using Trinity.Framework;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Framework.Helpers;
using Zeta.Game;
using Zeta.Game.Internals;


namespace Trinity.Components.Adventurer.Game.Quests
{
    public static class BountyCoroutineFactory
    {
        public static List<BountyCoroutine> GetActBounties(Act act)
        {
            IEnumerable<BountyInfo> gameActBounties;
            if (act == Act.OpenWorld)
            {
                gameActBounties = ZetaDia.Storage.Quests.Bounties.Where(b => b.State != QuestState.Completed).OrderBy(b => b.Act);
            }
            else
            {
                gameActBounties = ZetaDia.Storage.Quests.Bounties.Where(b => b.Act == act && b.State != QuestState.Completed);
            }

            var actBounties = new List<BountyCoroutine>();

            actBounties.AddRange(gameActBounties.Select(GetBounty).Where(bounty => bounty != null));

            return Randomizer.RandomShuffle(actBounties).ToList();
        }

        public static BountyCoroutine GetBounty(BountyInfo bountyInfo)
        {
            if (bountyInfo == null)
            {
                return null;
            }

            var bountyData = BountyDataFactory.GetBountyData(bountyInfo.Quest);
            if (bountyData == null)
            {
                Core.Logger.Log("Unsupported bounty: {0} - {1}", (int)bountyInfo.Quest, bountyInfo.Info.DisplayName);
                return null;
            }
            //if (bountyData.QuestType != BountyQuestType.SpecialEvent)
            //{
            //    Core.Logger.Debug("Skipping bounty: {0} - {1}", (int)bountyInfo.Quest, bountyInfo.Info.DisplayName);
            //    return null;
            //}

            //if (QuestDataFactory.QuestDatas.ContainsKey((int)bountyInfo.Quest)) return null;

            BountyCoroutine bounty = null;
            //var bountyType = BountyHelpers.GetQuestType(bountyInfo);
            //if (bountyType != BountyQuestType.SpecialEvent)
            //{
            //    return null;
            //}

            //if (Bounties.TryGetValue((int)bountyInfo.Quest, out bounty))
            //{
            //    Core.Logger.Debug("[BountyFactory] Returning hardcoded bounty:" + bounty.QuestData.Name);
            //}

            // Try to get the kill bounty

            //// Try to get the kill bounty
            //if (bounty == null && bountyInfo.Info.DisplayName.Replace("Bounty: ", string.Empty).StartsWith("Kill ") && TryGetKillBounty((int)bountyInfo.Quest, out bounty))
            //{
            //    Core.Logger.Debug("[BountyFactory] Returning generated bounty:" + bounty.QuestData.Name);
            //}

            // Try to get the clear bounty
            //if (bounty == null && bountyInfo.Info.DisplayName.Replace("Bounty: ", string.Empty).StartsWith("Clear ") && TryGetClearBounty((int)bountyInfo.Quest, out bounty))
            //{
            //    Core.Logger.Debug("[BountyFactory] Returning generated bounty:" + bounty.QuestData.Name);
            //}

            //// Try to get the curse bounty
            //if (bounty == null && bountyInfo.Info.DisplayName.Replace("Bounty: ", string.Empty).StartsWith("The Cursed") && bountyInfo.Quest != SNOQuest.X1_Bounty_A5_PandExt_Event_CursedCrystals && TryGetCursedBounty((int)bountyInfo.Quest, out bounty))
            //{
            //    Core.Logger.Debug("[BountyFactory] Returning generated bounty:" + bounty.QuestData.Name);
            //}

            if (bounty == null)
            {
                bounty = new GenericBountyCoroutine(bountyInfo.Quest);
                //if (bounty.QuestData.Steps.First().Objectives.Any(o => o.ObjectiveType == QuestStepObjectiveType.EnterLevelArea))
                //    return null;
                //if (bounty.QuestData.QuestType != BountyQuestType.KillMonster && bounty.QuestData.QuestType != BountyQuestType.ClearZone)
                //    return null;
                Core.Logger.Debug("[BountyFactory] Returning generic bounty:" + bounty.QuestData.Name);
            }

            if (bounty != null)
            {
                bounty.Reset();
                //if (bounty.QuestData.IsDataComplete)
                //{
                //    return null;
                //}
            }
            return bounty;
        }
    }
}