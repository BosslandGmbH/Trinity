using System.Collections.Generic;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Cache
{
    public class TrinityBountyInfo
    {
        public Act Act { get; set; }
        public TrinityQuestInfo Info { get; set; }
        public List<SNOLevelArea> LevelAreas { get; set; }
        public SNOLevelArea StartingLevelArea { get; set; }
        public SNOQuest Quest { get; set; }
        public QuestState State { get; set; }
        public BountyInfo BountyInfo { get; set; }

        public TrinityBountyInfo()
        {

        }
    }
}
