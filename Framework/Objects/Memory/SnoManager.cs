using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Objects.Enums;

namespace Trinity.Framework.Objects.Memory
{
    public static class SnoManager
    {
        static SnoManager()
        {
            Core = new SnoCore();
            //GameBalanceHelper = new GameBalanceHelper();
        }

        public static SnoCore Core { get; set; }
        //public static GameBalanceHelper GameBalanceHelper { get; set; }
    }

    public class SnoCore
    {
        public static Dictionary<SnoType, IndexData> LookupTable = new Dictionary<SnoType, IndexData>();

        public class IndexData
        {
            public Dictionary<int, short> Index;
            public int MaxIndex;
            public Dictionary<int, int> Index2;
        }

        public short GetEntityId(SnoType groupId, int snoEntryId)
        {
            IndexData indexData;
            if (LookupTable.TryGetValue(groupId, out indexData))
            {
                short entityId;
                if (indexData.Index.TryGetValue(snoEntryId, out entityId))
                    return entityId;
            }
            return 0;
        }
    }
}