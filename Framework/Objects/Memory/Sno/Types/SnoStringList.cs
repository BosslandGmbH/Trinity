using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Helpers;
using Trinity.Framework.Helpers.Exporter;
using Trinity.Framework.Objects.Enums;

namespace Trinity.Framework.Objects.Memory.Sno.Types
{
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

        public string WriteToEnumFile()
        {
            var options = new ExportOptions
            {
                QuoteValue = false
            };

            var exportData = StringTableEntries.ToDictionary(k => k.Key, v => v.GameBalanceNameNormalHashed.ToString());

            var enumString = Exporter.Enum.Create(exportData, options);    
                    
            DebugUtil.WriteLinesToLog($"enum_{Header.SnoId}.txt", enumString, true);
            return enumString;
        }

    }
}



