using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Trinity.Helpers;
using Trinity.Objects.Native;
using MemoryHelper = Trinity.Framework.Helpers.MemoryHelper;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public class SnoStringList : SnoTable
    {
        public const int SizeOf = 0x28; // 40        
        public List<StringTableEntry> StringTableEntries => ReadSerializedObjects<StringTableEntry>(0x10, StringTableSerializeInfo);
        public SerializationInfo StringTableSerializeInfo => ReadOffset<SerializationInfo>(0x18);

        public class StringTableEntry : MemoryWrapper
        {
            public const int SizeOf = 0x28; // 40
            public string Key => ReadString(0x00, KeySerializationData);
            public SerializationInfo KeySerializationData => ReadOffset<SerializationInfo>(0x08);
            public string Value => ReadString(0x10, ValueSerializationData);
            public SerializationInfo ValueSerializationData => ReadOffset<SerializationInfo>(0x18);
            public int GameBalanceNameNormalHashed => ReadOffset<int>(0x20);
        }

        public SortedList<int, string> ToSortedList()
        {
            var dictionary = new SortedList<int,string>();
            foreach (var entry in StringTableEntries)
            {
                dictionary[MemoryHelper.GameBalanceItemHash(entry.Key)] = entry.Value;
            }
            return dictionary;
        }

    }


}



