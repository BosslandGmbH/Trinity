using System.Collections.Generic;
using Trinity.Framework.Objects.Enums;

namespace Trinity.Framework.Objects.Memory
{
    public class StringListHelper
    {
        private readonly Dictionary<int, SnoDictionary<string>> _cachedStringLists = new Dictionary<int, SnoDictionary<string>>();

        public SnoDictionary<string> GetStringList(SnoStringListType type)
        {
            return GetStringList((int)type);
        }

        public SnoDictionary<string> GetStringList(int gameBalanceId)
        {
            SnoDictionary<string> dict;

            if (_cachedStringLists.TryGetValue(gameBalanceId, out dict))
            {
                return dict;
            }

            var entityId = SnoManager.Core.GetEntityId(SnoType.StringList, gameBalanceId);
            if (entityId == 0)
            {
                return new SnoDictionary<string>();
            }

            var stringList = SnoManager.Groups.StringList.Container[entityId].Value;
            dict = stringList.ToSnoDictionary();
            _cachedStringLists.Add(gameBalanceId, dict);

            return dict;
        }

        public string GetStringListValue(SnoStringListType type, int stringListId)
        {
            return GetStringListValue((int)type, stringListId);
        }

        public string GetStringListValue(int listGameBalanceId, int stringListId)
        {
            var dict = GetStringList(listGameBalanceId);
            string value;
            if (dict.TryGetValue(stringListId, out value))
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

    public class NativeStringList : SnoTableEntry
    {
        public const int SizeOf = 40; // 0x28
        public List<NativeStringTableEntry> _1_0x10_VariableArray => ReadSerializedObjects<NativeStringTableEntry>(0x10, 0x18); //    VarArrSerializeOffsetDiff=8   Flags=33
        public NativeSerializeData _2_0x18_SerializeData => ReadObject<NativeSerializeData>(0x18);
    }

    public class NativeStringTableEntry : MemoryWrapper
    {
        public const int SizeOf = 40; // 0x28
        public string _1_0x0_SerializedString => ReadSerializedString(0x0, 0x8); //    VarArrSerializeOffsetDiff=8   Flags=2081
        public NativeSerializeData _2_0x8_SerializeData => ReadObject<NativeSerializeData>(0x8); //      Flags=2048
        public string _3_0x10_SerializedString => ReadSerializedString(0x10, 0x18); //    VarArrSerializeOffsetDiff=8   Flags=33
        public NativeSerializeData _4_0x18_SerializeData => ReadObject<NativeSerializeData>(0x18);
        public int _5_0x20_int => ReadOffset<int>(0x20);
        public int _6_0x24_int => ReadOffset<int>(0x24);
    }

    public class NativeSerializeData : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
        public int Offset => ReadOffset<int>(0x0); //      Flags=1  _1_0x0_int
        public int Length => ReadOffset<int>(0x4); //      Flags=1   _2_0x4_int
    }

    public class SnoTableEntry : MemoryWrapper
    {
        public SnoHeader Header => ReadObject<SnoHeader>(0x00);
    }

    public class SnoHeader : MemoryWrapper
    {
        public const int SizeOf = 0x0C; // 12
        public int SnoId => ReadOffset<int>(0x00);
        public int LockCount => ReadOffset<int>(0x04);
        public int Flags => ReadOffset<int>(0x08); // 1 = DoNotPurge
    }

    public class SnoStringList : SnoTableEntry
    {
        public const int SizeOf = 0x28; // 40
        public List<StringTableEntry> StringTableEntries => ReadSerializedObjects<StringTableEntry>(0x10, StringTableSerializeInfo);
        public NativeSerializeData StringTableSerializeInfo => ReadObject<NativeSerializeData>(0x18);
        public SnoStringListType Type => (SnoStringListType)Header.SnoId;

        public override string ToString() => $"{GetType().Name}: {Type}";

        public class StringTableEntry : MemoryWrapper
        {
            public const int SizeOf = 0x28; // 40
            public string Key => ReadSerializedString(0x00, KeySerializationData);
            public NativeSerializeData KeySerializationData => ReadObject<NativeSerializeData>(0x08);
            public string Value => ReadSerializedString(0x10, ValueSerializationData);
            public NativeSerializeData ValueSerializationData => ReadObject<NativeSerializeData>(0x18);
            public int GameBalanceNameNormalHashed => ReadOffset<int>(0x20);
        }

        public SnoDictionary<string> ToSnoDictionary()
        {
            var dictionary = new SnoDictionary<string>();
            foreach (var entry in StringTableEntries)
            {
                dictionary.Add(entry.Key, entry.Value);
            }
            return dictionary;
        }
    }
}