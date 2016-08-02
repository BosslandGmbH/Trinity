using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Util;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;

namespace Trinity.Components.Adventurer.Game.Quests
{
    public class QuestData
    {
        public int QuestId { get; set; }
        public bool IsDataComplete { get; set; }
        public bool IsReadOnly { get; set; }
        public string Name { get; set; }
        public string InternalName { get; set; }
        public Act Act { get; set; }
        public string ActName { get; set; }
        public HashSet<int> LevelAreaIds { get; set; }
        public List<QuestStepData> Steps { get; set; }
        public WaypointData Waypoint { get; set; }

        public static QuestData GetQuestData(int questId)
        {
            var bountyInfo = GetBountyInfo(questId);
            var questData = new QuestData();

            questData.QuestId = questId; //(int)bountyInfo.Quest;

            questData.Steps = new List<QuestStepData>();

            if (bountyInfo == null)
                return questData;

            questData.Name = bountyInfo.Info.DisplayName.Replace("Bounty: ", string.Empty);
            questData.InternalName = bountyInfo.Quest.ToString();
            questData.Act = bountyInfo.Act;
            questData.ActName = bountyInfo.Act.ToString();
            
            questData.LevelAreaIds = new HashSet<int>(bountyInfo.LevelAreas.Select(la => (int)la));
            
            questData.Waypoint = WaypointFactory.GetWaypointByLevelAreaId((int)bountyInfo.StartingLevelArea);


            foreach (var step in bountyInfo.Info.QuestRecord.Steps)
            {
                var questStep = new QuestStepData(questData, step);
                questData.Steps.Add(questStep);
            }

            //if (questData.QuestType == BountyQuestType.Unknown)
            //{
            //    questData.QuestType = BountyQuestTypeFactory.GetQuestType(bountyInfo);
            //}

            //var bountyScripts = new BountyScripts();
            //if (bountyScripts.ContainsKey(questId))
            //{
            //    questData.BountyScript = bountyScripts[questId];
            //    questData.BountyScript.Reset();
            //}

            //Logger.Debug("[QuestData] Saving Quest {0} ({1})", questData.Name, questData.QuestId);
            //questData.Save();
            return questData;
        }    

        public bool IsObjectiveActive(QuestStepObjectiveType objectiveType)
        {
            return Steps.SelectMany(s => s.Objectives).Any(o => o.ObjectiveType == objectiveType && o.IsActive);
        }

        private static BountyInfo GetBountyInfo(int questId)
        {
            return ZetaDia.ActInfo.Bounties.FirstOrDefault(b => (int)b.Quest == questId);
        }
    }

    public class QuestStepData
    {
        public int StepId { get; set; }
        public string Name { get; set; }
        public List<QuestStepObjectiveData> Objectives { get; set; }

        public QuestStepData() { }

        public QuestStepData(QuestData questData, QuestStep questStep)
        {
            StepId = questStep.StepId;
            Name = questStep.Name;
            Objectives = new List<QuestStepObjectiveData>();
            foreach (var stepObjective in questStep.QuestStepObjectiveSet.QuestStepObjectives)
            {
                Objectives.Add(new QuestStepObjectiveData(questData, this, stepObjective));
            }
        }

        private bool _isActive = true;
        //[JsonIgnore]
        public bool IsActive
        {
            get
            {
                if (!_isActive) return false;
                _isActive = ZetaDia.ActInfo.AllQuests.Any(q => q.QuestStep == StepId);
                return _isActive;
            }
        }

    }

    public class QuestStepObjectiveData
    {
        private readonly QuestData _questData;
        private readonly QuestStepData _questStepData;

        public QuestStepObjectiveType ObjectiveType { get; set; }
        public string ObjectiveTypeName { get; set; }
        public string Name { get; set; }

        public QuestStepObjectiveData() { }

        public QuestStepObjectiveData(QuestData questData, QuestStepData questStepData, QuestStepObjective objective)
        {
            _questData = questData;
            _questStepData = questStepData;
            ObjectiveType = objective.ObjectiveType;
            ObjectiveTypeName = objective.ObjectiveType.ToString();
            Name = objective.StepObjectiveName;
        }

        private bool _isActive = true;
        //[JsonIgnore]
        public bool IsActive
        {
            get
            {
                if (!_isActive) return false;
                _isActive = IsQuestObjectiveActive(_questData.QuestId, _questStepData.StepId, ObjectiveType);
                return _isActive;
            }
        }

        private static bool IsQuestObjectiveActive(int questId, int questStepId, QuestStepObjectiveType objectiveType)
        {
            var currentQuest = QuestInfo.FromId(questId);
            if (currentQuest == null)
            {
                Logger.Debug("[Bounty] Failed to determine the step id. Reason: Quest not found.");
                return false;
            }
            var step = currentQuest.QuestRecord.Steps.FirstOrDefault();
            if (step == null)
            {
                Logger.Debug("[Bounty] Failed to determine the step id. Reason: Step not found.");
                return false;
            }
            var objectives = currentQuest.QuestRecord.Steps.First(qs => qs.StepId == questStepId).QuestStepObjectiveSet.QuestStepObjectives;
            int objectiveIndex;
            for (objectiveIndex = 0; objectiveIndex < objectives.Length; objectiveIndex++)
            {
                if (objectives[objectiveIndex].ObjectiveType == objectiveType)
                {
                    break;
                }
            }

            return QuestObjectiveInfo.IsActiveObjective(questId, questStepId, objectiveIndex);
        }
    }

}
