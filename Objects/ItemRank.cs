using System.Collections.Generic;

namespace Trinity.Objects
{
    public class ItemRank
    {
        public ItemRank()
        {
            HardcoreRank = new List<ItemRankData>();
            SoftcoreRank = new List<ItemRankData>();
        }
        public Item Item;
        public List<ItemRankData> HardcoreRank;
        public List<ItemRankData> SoftcoreRank;
    }
}
