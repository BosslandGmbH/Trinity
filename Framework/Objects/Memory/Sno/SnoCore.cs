using System;
using System.Collections.Generic;
using Trinity.Framework.Objects.Enums;
using Trinity.Helpers;

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
        }

        /// <summary>
        /// Creates a SnoGroup for accessing a SnoType's data.
        /// </summary>
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

        /// <summary>
        /// Creates a dictionary to maps SnoEntryId (values given elsewhere to find sno groups) and EntityId (array index in sno group)        
        /// </summary>
        private void CreateIndex<T>(SnoType groupType, SnoGroup<T> snoGroup, int groupNum) where T : SnoTableEntry, new()
        {
            var container = snoGroup.Container;
            var maxIndex = container.MaxIndex;
            var index = new Dictionary<int, short>();

            for (int i = 0; i < maxIndex; i++)
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
                MaxIndex = maxIndex
            });
        }

        /// <summary>
        /// Get the Entry id (array index to snoGroup) for a SnoId
        /// </summary>
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