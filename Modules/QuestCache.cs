using System.Collections.Generic;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Memory;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;

namespace Trinity.Modules
{
    public class QuestCache : Module
    {
        protected override int UpdateIntervalMs => 2000;

        protected override void OnPulse()
        {
            //if (!ZetaDia.IsInGame || ZetaDia.Service.Party.CurrentPartyLockReasonFlags != PartyLockReasonFlag.None)
            //    return;

            //CurrentQuest = ZetaDia.CurrentQuest;
            //if (CurrentQuest == null)
            //    return;

            ////ZetaDia.ActRecord.
            ////Act = ZetaDia.ActInfo;
            ////if (Act == null || !Act.IsValid)
            ////    return;

            ////ZetaDia.

            ////CurrentBountyInfo = Act.ActiveBounty;
            ////if (CurrentBountyInfo == null)
            ////    return;


            //CurrentBountyData = BountyDataFactory.GetBountyData((int)CurrentBountyInfo.Quest);
            //if (CurrentBountyData == null)
            //    return;

            ////if (SceneNames == null)
            ////{
            ////    SceneNames = SnoManager.StringListHelper.GetStringList(SnoStringListType.LevelAreaNames);
            ////}

            //QuestData = CurrentBountyData.QuestData;

            //foreach (var step in QuestData.Steps)
            //{
            //    if (!step.IsActive) continue;

            //    CurrentStepData = step;

            //    foreach (var objective in step.Objectives)
            //    {
            //        if (!objective.IsActive) continue;

            //        CurrentObjective = objective;
            //    }
            //}

        }

        public string CurrentSceneName { get; set; }

        public SNOLevelArea CurrentLevelAreaInternalName { get; set; }

        public SnoDictionary<string> SceneNames { get; private set; }

        public BountyInfo CurrentBountyInfo { get; private set; }

        public Quest CurrentQuest { get; private set; }

        //public ActInfo Act { get; private set; }

        public QuestStepData CurrentStepData { get; private set; }

        public QuestData QuestData { get; private set; }

        public QuestStepObjectiveData CurrentObjective { get; private set; }

        public BountyData CurrentBountyData { get; private set; }

        public bool IsKillAllRequired => CurrentQuest != null && CurrentQuest.IsValid && CurrentStepData != null && CurrentObjective != null &&
            CurrentStepData.IsActive && CurrentObjective.IsActive && KillAllTypes.Contains(CurrentObjective.ObjectiveType);

        public HashSet<QuestStepObjectiveType> KillAllTypes { get; } = new HashSet<QuestStepObjectiveType>
        {
            QuestStepObjectiveType.KillAllMonstersInWorld,
            QuestStepObjectiveType.KillAnyMonsterInLevelArea,
            QuestStepObjectiveType.KillElitePackInLevelArea,
            //QuestStepObjectiveType.KillMonster,
            QuestStepObjectiveType.KillMonsterFromFamily,
            QuestStepObjectiveType.KillMonsterFromGroup,
            QuestStepObjectiveType.KillGroup,
        };
    }

}
