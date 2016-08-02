using System.Linq;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;

namespace Trinity.Components.Adventurer.Game.Quests
{
    public enum BountyQuestType
    {
        Unknown,
        GuardedGizmo,
        ClearZone,
        KillMonster,
        ClearCurse,
        SpecialEvent,
        KillBossBounty
    }

    public static class BountyQuestTypeFactory
    {
        public static BountyQuestType GetQuestType(BountyInfo bountyInfo)
        {
            var questType = BountyQuestType.Unknown;
            var objectives = bountyInfo.Info.QuestRecord.Steps.First().QuestStepObjectiveSet.QuestStepObjectives;
            if (objectives.Any(qo => qo.ObjectiveType == QuestStepObjectiveType.EnterLevelArea) &&
                objectives.Any(qo => qo.ObjectiveType == QuestStepObjectiveType.KillAllMonstersInWorld))
            {
                questType = BountyQuestType.ClearZone;
            }
            else if (objectives.Any(qo => qo.ObjectiveType == QuestStepObjectiveType.KillMonster) && bountyInfo.Info.DisplayName.Replace("Bounty: ", string.Empty).StartsWith("Kill "))
            {
                questType = BountyQuestType.KillMonster;
            }
            else if (objectives.Any(qo => qo.ObjectiveType == QuestStepObjectiveType.KillGroup) &&
                     objectives.Any(qo => qo.ObjectiveType == QuestStepObjectiveType.InteractWithActor) &&
                     objectives.Any(qo => qo.ObjectiveType == QuestStepObjectiveType.EventReceived)
                )
            {
                questType = BountyQuestType.GuardedGizmo;
            }
            else if (bountyInfo.Info.DisplayName.Replace("Bounty: ", string.Empty).StartsWith("The Cursed") && bountyInfo.Quest != SNOQuest.X1_Bounty_A5_PandExt_Event_CursedCrystals)
            {
                questType = BountyQuestType.ClearCurse;
            }
            else if (bountyInfo.Quest.ToString().Contains("_Event_") && objectives.Any(qo => qo.ObjectiveType == QuestStepObjectiveType.CompleteQuest))
            {
                questType = BountyQuestType.SpecialEvent;
            }
            return questType;
        }
    }
}
