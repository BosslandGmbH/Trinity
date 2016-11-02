using System;
using System.Collections.Generic;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Containers;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public class SnoCore
    {
        public static Dictionary<SnoType, IndexData> LookupTable = new Dictionary<SnoType, IndexData>();
        public static IntPtr[] SnoManagerPtrs => MemoryWrapper.ReadArray<IntPtr>(Internals.Addresses.SnoGroups, 70);

        public class IndexData
        {
            public Dictionary<int, short> Index;
            public int MaxIndex;
            public Dictionary<int, int> Index2;
        }

        public SnoGroup<T> CreateGroup<T>(SnoType groupId) where T : SnoTableEntry, new()
        {
            var groupNum = (int)groupId;
            if (groupNum < 0 || groupNum > 70)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId));
            }
            var ptr = SnoManagerPtrs[groupNum];
            var manager = MemoryWrapper.Create<SnoGroup<T>>(ptr);
            if (!LookupTable.ContainsKey(groupId))
            {
                CreateIndex(groupId, manager, groupNum);
            }
            return manager;
        }

        private void CreateIndex<T>(SnoType groupType, SnoGroup<T> snoGroup, int groupNum) where T : SnoTableEntry, new()
        {
            var container = snoGroup.Container;
            var maxIndex = container.MaxIndex;
            var index = new Dictionary<int, short>();
            for (var i = 0; i < maxIndex; i++)
            {
                var entry = container[(short)i];
                var snoEntryId = entry.SnoGroupId;
                if (snoEntryId != groupNum)
                    continue;

                var entityId = (short)entry.Id;
                if (entityId == -1)
                    continue;

                var identifier = entry.Value.Header.SnoId;
                index[identifier] = entityId;
            }
            LookupTable.Add(groupType, new IndexData
            {
                Index = index,
                MaxIndex = maxIndex,
            });
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