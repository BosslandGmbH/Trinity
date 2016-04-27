using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Objects.Enums;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public static class Sno
    {
        static Sno()
        {
            Internals = new SnoInternals();
            Managers = new SnoManagers();
            StringList = new StringListHelper();
            GameBalance = new GameBalanceHelper();
        }

        public static SnoInternals Internals { get; set; }

        public static SnoManagers Managers { get; set; }

        public static StringListHelper StringList { get; set; }

        public static GameBalanceHelper GameBalance { get; set; }

        public class SnoManagers
        {
            public SnoManagers()
            {
                StringList = Internals.CreateManager<SnoStringList>(SnoType.StringList);
            }

            public SnoGroup<SnoStringList> StringList { get; }
        }

        public class SnoInternals
        {
            public static Dictionary<SnoType, IndexData> LookupTable = new Dictionary<SnoType, IndexData>();

            public static IntPtr[] SnoManagerPtrs => MemoryWrapper.ReadArray<IntPtr>((IntPtr)0x01E9B510, 70);

            public class IndexData
            {
                public Dictionary<int, short> Index;
                public int MaxIndex;
            }

            public SnoGroup<T> CreateManager<T>(SnoType groupId) where T : SnoTable, new()
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

            private void CreateIndex<T>(SnoType groupId, SnoGroup<T> manager, int groupNum) where T : SnoTable, new()
            {
                var container = manager.Container;
                var maxIndex = container.MaxIndex;
                var index = new Dictionary<int, short>();

                for (int i = 0; i < maxIndex; i++)
                {
                    var entry = container[(short)i];
                    var entrySnoGroupId = entry.SnoGroupId;
                    if (entrySnoGroupId != groupNum)
                        continue;

                    var entityId = (short)entry.Id;
                    if (entityId == -1)
                        continue;

                    var identifier = entry.Value.Header.SnoId;
                    index[identifier] = entityId;
                }

                LookupTable.Add(groupId, new IndexData
                {
                    Index = index,
                    MaxIndex = maxIndex
                });
            }

            public short GetEntityId(SnoType groupId, int gameBalanceId)
            {
                IndexData indexData;
                if (LookupTable.TryGetValue(groupId, out indexData))
                {
                    return indexData.Index[gameBalanceId];
                }
                return 0;
            }

        }

        public class StringListHelper
        {
            private readonly Dictionary<int, SortedList<int, string>> _cachedStringLists = new Dictionary<int, SortedList<int, string>>();

            public SortedList<int, string> GetStringList(SnoType gameBalanceId)
            {
                return GetStringList((int)gameBalanceId);
            }

            public SortedList<int, string> GetStringList(int gameBalanceId)
            {
                SortedList<int, string> dict;

                if (_cachedStringLists.TryGetValue(gameBalanceId, out dict))
                {
                    return dict;
                }

                var entityId = Internals.GetEntityId(SnoType.StringList, gameBalanceId);
                if (entityId == 0)
                {
                    return new SortedList<int, string>();
                }

                var stringList = Managers.StringList.Container[entityId].Value;
                dict = stringList.ToSortedList();
                _cachedStringLists.Add(gameBalanceId, dict);
                return dict;
            }

            public string GetStringListValue(SnoStringListType type, int entryGameBalanceId)
            {
                return GetStringListValue((int)type, entryGameBalanceId);
            }

            public string GetStringListValue(int listGameBalanceId, int entryGameBalanceId)
            {
                var dict = GetStringList(listGameBalanceId);
                string value;
                if (dict.TryGetValue(entryGameBalanceId, out value))
                {
                    return value;
                }
                return string.Empty;
            }

            public string GetStringListValueByIndex(int listGameBalanceId, int index)
            {
                var dict = GetStringList(listGameBalanceId);
                if (index >= 0 && index < dict.Count)
                {
                    return dict[index];
                }
                return string.Empty;
            }
        }

        public class GameBalanceHelper
        {
            public Dictionary<int, object> Cache = new Dictionary<int, object>();

            static GameBalanceHelper()
            {

            }

            public HashSet<SnoGameBalanceType> ValidGameBalanceTypes { get; set; }

            public IntPtr GetRecordPtr(SnoGameBalanceType gbType, int gbId)
            {
                if ((int)gbType != -1)
                {
                    return SNORecordGameBalance.GetGameBalanceRecord(gbId, (Zeta.Game.Internals.SNO.GameBalanceType)(int)gbType);
                }
                return IntPtr.Zero;
            }

            public T GetRecord<T>(SnoGameBalanceType gbType, int gbId) where T : struct
            {
                if ((int)gbType != -1)
                {
                    if (Cache.ContainsKey(gbId))
                        return (T)Cache[gbId];

                    var record = SNORecordGameBalance.GetGameBalanceRecord<T>(gbId, (Zeta.Game.Internals.SNO.GameBalanceType)(int)gbType);
                    if (record.HasValue)
                    {
                        Cache.Add(gbId, record);
                        return (T) record;
                    }
                    else
                    {
                        Logger.Log($"SnoRecord not found GbId={gbId} GbType={gbType}");
                    }

                }
                return default(T);
            }
        }

    }
}


