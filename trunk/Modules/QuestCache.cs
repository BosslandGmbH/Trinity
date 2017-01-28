using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Sno;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Modules
{
    public class QuestCache : Module
    {
        protected override int UpdateIntervalMs => 1000;

        protected override void OnPulse()
        {
            CurrentQuest = ZetaDia.CurrentQuest;
            if (CurrentQuest == null)
                return;

            var act = ZetaDia.ActInfo;
            if (act == null)
                return;

            CurrentBountyInfo = act.ActiveBounty;
            if (CurrentBountyInfo == null)
                return;

            CurrentBountyData = BountyDataFactory.GetBountyData((int)CurrentBountyInfo.Quest);
            if (CurrentBountyData == null)
                return;

            //if (SceneNames == null)
            //{
            //    SceneNames = SnoManager.StringListHelper.GetStringList(SnoStringListType.LevelAreaNames);
            //}

            foreach (var step in CurrentBountyData.QuestData.Steps)
            {
                if (!step.IsActive) continue;

                CurrentStepData = step;

                foreach (var objective in step.Objectives)
                {
                    if (!objective.IsActive) continue;

                    CurrentObjective = objective;
                }
            }

            //var stringstringlistslsits = Core.MemoryModel.SnoGroups.StringList.Entries.ToList();
            //foreach (var list in stringstringlistslsits)
            //{
            //    foreach (var item in list.StringTableEntries)
            //    {
            //        if (item.Value.Contains("Tunnels"))
            //        {
            //            Debugger.Break();
            //        }
            //    }
            //}

            //if (CurrentObjective != null)
            //{
            //    CurrentLevelAreaInternalName = (SNOLevelArea)ZetaDia.CurrentLevelAreaSnoId;

            //    string localizedName;
            //    if (SceneNames.TryGetValue(CurrentLevelAreaInternalName.ToString(), out localizedName))
            //        CurrentSceneName = localizedName;
            //}

        }

        public string CurrentSceneName { get; set; }

        public SNOLevelArea CurrentLevelAreaInternalName { get; set; }

        public SnoDictionary<string> SceneNames { get; private set; }

        public BountyInfo CurrentBountyInfo { get; private set; }

        public Quest CurrentQuest { get; private set; }

        public QuestStepData CurrentStepData { get; private set; }

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
            //QuestStepObjectiveType.KillGroup,
        };
    }

}
