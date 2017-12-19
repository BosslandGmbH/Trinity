using Zeta.Game.Internals;

namespace Trinity.Components.Adventurer.Game.Rift
{
    public class RiftQuest
    {
        private const int RIFT_QUEST_ID = 337492;

        public QuestState State { get; private set; }
        public RiftStep Step { get; private set; }

        public RiftQuest()
        {
            Step = RiftStep.NotStarted;
            State = QuestState.NotStarted;

            var quest = QuestInfo.FromId(RIFT_QUEST_ID);
            if (quest != null)
            {
                State = quest.State;
                var step = quest.QuestStep;
                switch (step)
                {
                    case 1: // Normal rift
                    case 13: // Greater rift
                        Step = RiftStep.KillingMobs;
                        break;

                    case 3: // Normal rift
                    case 16: // Greater rift
                        Step = RiftStep.BossSpawned;
                        break;

                    case 34:
                        Step = RiftStep.UrshiSpawned;
                        break;

                    case 46:
                    case 10:
                        Step = RiftStep.Cleared;
                        break;
                }
            }

            if (State == QuestState.Completed)
            {
                Step = RiftStep.Completed;
            }
        }
    }
}