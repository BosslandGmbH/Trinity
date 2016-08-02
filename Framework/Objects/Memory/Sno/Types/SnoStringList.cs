using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using Trinity.Framework.Helpers;
using Trinity.Framework.Helpers.Exporter;
using Trinity.Helpers;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public class SnoStringList : SnoTableEntry
    {
        public const int SizeOf = 0x28; // 40        
        public List<StringTableEntry> StringTableEntries => ReadSerializedObjects<StringTableEntry>(0x10, StringTableSerializeInfo);
        public SerializeData StringTableSerializeInfo => ReadOffset<SerializeData>(0x18);

        public class StringTableEntry : MemoryWrapper
        {
            public const int SizeOf = 0x28; // 40
            public string Key => ReadSerializedString(0x00, KeySerializationData);
            public SerializeData KeySerializationData => ReadOffset<SerializeData>(0x08);
            public string Value => ReadSerializedString(0x10, ValueSerializationData);
            public SerializeData ValueSerializationData => ReadOffset<SerializeData>(0x18);
            public int GameBalanceNameNormalHashed => ReadOffset<int>(0x20);
        }


    //public SortedList<int, string> ToSortedList()
    //{
    //    var dictionary = new SortedList<int, string>();
    //    foreach (var entry in StringTableEntries)
    //    {
    //        //dictionary[MemoryHelper.GameBalanceItemHash(entry.Key)] = entry.Value;
    //        dictionary[MemoryHelper.GameBalanceNormalHash(entry.Key)] = entry.Value;
    //    }
    //    return dictionary;
    //}

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



