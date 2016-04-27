using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;

namespace Trinity.Cache
{
    public class TrinityQuestInfo
    {
        public int BonusCount { get; set; }
        public int KillCount { get; set; }
        public int LevelArea { get; set; }
        public SNOQuest Quest { get; set; }
        public int QuestSNO { get; set; }
        public int QuestStep { get; set; }
        public QuestType QuestType { get; set; }
        public QuestState State { get; set; }
        public QuestInfo QuestInfo { get; set; }

        public TrinityQuestInfo()
        {

        }
    }
}
