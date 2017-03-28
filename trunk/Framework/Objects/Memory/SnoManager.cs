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
            Groups = new SnoGroups();
            StringListHelper = new StringListHelper();
            //GameBalanceHelper = new GameBalanceHelper();
        }

        public static SnoCore Core { get; set; }
        public static SnoGroups Groups { get; set; }
        public static StringListHelper StringListHelper { get; set; }
        //public static GameBalanceHelper GameBalanceHelper { get; set; }

        public class SnoGroups
        {
            public SnoGroups()
            {
                StringList = Core.CreateGroup<SnoStringList>(SnoType.StringList);
            }

            public SnoGroup<SnoStringList> StringList { get; }
        }
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

        public SnoGroup<T> CreateGroup<T>(SnoType groupId) where T : SnoTableEntry, new()
        {
            var groupNum = (int)groupId;
            if (groupNum < 0 || groupNum > 70)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId));
            }
            var ptr = Zeta.Game.ZetaDia.SNO[(Zeta.Game.Internals.SNOGroup)groupNum].BaseAddress;
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

    public class SnoGroup<T> : MemoryWrapper where T : SnoTableEntry, new()
    {
        public Container<SnoDefinition<T>> Container => ReadPointer<Container<SnoDefinition<T>>>(0x10);
        public IEnumerable<T> Entries => Container.Where(i => i.SnoGroupId == (int)SnoType).Select(v => v.Value);
        public SnoType SnoType => ReadOffset<SnoType>(0x3C);
        public int InvalidSnoId => ReadOffset<int>(0x80);
        public int ItemSize => ReadOffset<int>(0x68);
        public int Limit => ReadOffset<int>(0x64);
        public int Flags => ReadOffset<int>(0x18);
        public string Name => ReadString(0x1C, 32);
    }

    public class SnoDefinition<T> : MemoryWrapper where T : SnoTableEntry, new()
    {
        public const int SizeOf = 16;
        public int Id => ReadOffset<int>(0x00);
        public int LastTouched => ReadOffset<int>(0x04);
        public byte SnoGroupId => ReadOffset<byte>(0x07);
        public int Size => ReadOffset<int>(0x08);
        public T Value => ReadPointer<T>(0x0C);
        public IntPtr ValuePtr => ReadOffset<IntPtr>(0x0C);

        public override string ToString() => $"{GetType().Name}: {Value}";
    }
}