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
            //CurrentQuest = ZetaDia.CurrentQuest;
            //if (CurrentQuest == null)
            //    return;

            //CurrentBounty = ZetaDia.Storage.Quests.ActiveBounty;
            //if (CurrentBounty == null)
            //    return;

            //CurrentBountyData = BountyDataFactory.GetBountyData((int)CurrentBounty.Quest);
            //if (CurrentBountyData == null)
            //    return;

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

        public BountyInfo CurrentBounty { get; set; }

        public string CurrentSceneName { get; set; }

        public SNOLevelArea CurrentLevelAreaInternalName { get; set; }

        public SnoDictionary<string> SceneNames { get; private set; }

        public Quest CurrentQuest { get; private set; }

        public QuestStepData CurrentStepData { get; private set; }

        public QuestData QuestData { get; private set; }

        public QuestStepObjectiveData CurrentObjective { get; private set; }

        public BountyData CurrentBountyData { get; private set; }

        public bool IsKillAllRequired
        {
            get
            {
                return false;

                // Needs work - better detection of when inside the area/level that needs to have things killed.

                //if (CurrentQuest == null || !CurrentQuest.IsValid)
                //    return false;

                //if (CurrentStepData == null || CurrentObjective == null)
                //    return false;

                //if (!CurrentStepData.IsActive || !CurrentObjective.IsActive)
                //    return false;

                //return KillAllTypes.Contains(CurrentObjective.ObjectiveType);
            }
        }

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


        public static void Clear()
        {

        }
    }

}
