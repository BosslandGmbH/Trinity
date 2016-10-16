using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Trinity.Framework.Objects.Memory.Misc;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class Map<TValue> : MemoryWrapper where TValue : MemoryWrapper, new()
    {
        public const int SizeOf = 0x70; // 112
        public int x00_Mask => ReadOffset<int>(0x00);
        public int x04_Count => ReadOffset<int>(0x04);
        public Allocator<Pair> x08_PtrEntryAllocator => ReadPointer<Allocator<Pair>>(0x08);
        public int _x0C => ReadOffset<int>(0x0C);
        public PtrTable<Pair> _0x10_Buckets => ReadObject<PtrTable<Pair>>(0x10);

        //public Allocator<Pair> x48_EntryAllocator => ReadObject<Allocator<Pair>>(0x48);

        public int _x64 => ReadOffset<int>(0x00);
        public int _x68 => ReadOffset<int>(0x00);
        public int _x6C => ReadOffset<int>(0x00);

        public bool TryGetItemByKey<TKey>(TKey modKey, out TValue value, Func<TKey, uint> hasher)
        {
            var hash = hasher(modKey);
            var bucketIndex = unchecked((int)(hash & x00_Mask));
            var bucketEntry = _0x10_Buckets[bucketIndex];
            while (bucketEntry != null)
            {
                if (bucketEntry.ModKey.Equals(modKey))
                {
                    value = Create<TValue>(bucketEntry.BaseAddress);
                    return true;
                }
                bucketEntry = bucketEntry.x00_Next;
            }
            value = default(TValue);
            return false;
        }

        public List<TValue> Entries
        {
            get
            {
                var entries = new List<TValue>();
                foreach (var firstItem in _0x10_Buckets.Items)
                {
                    var pair = firstItem;
                    while (pair != null)
                    //while (pair?.x08_Value != null && pair.IsValid)
                    {
                        entries.Add(pair.x08_Value);
                        pair = pair.x00_Next;
                    }
                }
                return entries;
            }
        }

        public Dictionary<int, TValue> ToDictionary()
        {
            var count = (short)x04_Count;
            var dic = new Dictionary<int, TValue>(count);

            foreach (var block in x08_PtrEntryAllocator.x08_Blocks)
            {
                var elements = block.x00_Elements;
                var isSlotFree = block.x24_FreeSpaceBitmap;
                for (int i = 0; i < elements.Count; i++)
                {
                    if (isSlotFree[i])
                        continue;
                    var element = elements[i];
                    dic[element.x04_Key] = element.x08_Value;
                }
            }
            return dic;
        }

        public string ToEnum(Func<TValue, Tuple<string, string, string>> enumPartSelector, string name = "")
        {
            var indent = "    ";
            var sb = new StringBuilder();
            sb.AppendLine($"public enum {(string.IsNullOrEmpty(name) ? "Name" : name)} // Count={Entries.Count}");
            sb.AppendLine("{");
            foreach (var entry in Entries)
            {
                var def = enumPartSelector(entry);
                sb.AppendLine(indent + def.Item1 + " = " + def.Item2 + ", // " + def.Item3);
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        //public string ToReference<T>(Func<T enumPartSelector, string name = "") where T : TValue
        //{ 
        //    var indent = "    ";
        //    var type = typeof (T);
        //    var sb = new StringBuilder();
        //    sb.AppendLine($"public class {(string.IsNullOrEmpty(name) ? "Name" : name)} // Count={Entries.Count}");
        //    sb.AppendLine("{{");

        //    var propInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    foreach (var propInfo in propInfos.Where(p => !p.Name.StartsWith("_")))
        //    {                
        //        sb.AppendLine($"{indent}{propInfo.Name} = {propInfo.GetValue()} ");
        //    }

        //    sb.AppendLine("}}");
        //    return sb.ToString();
        //}

        public class Pair : MemoryWrapper
        {
            public static int SizeOf = 12;
            public Pair x00_Next => ReadPointer<Pair>(0x00);
            public int x04_Key => ReadOffset<int>(0x04);
            public TValue x08_Value => ReadPointer<TValue>(0x08);
            public int ModKey => x04_Key;
            public override string ToString() => $"{typeof(TValue).Name}: {x04_Key}, {x08_Value}";
        }

        public override string ToString() => $"{GetType().Name}: {typeof(TValue)}: Count={x04_Count} {x08_PtrEntryAllocator}";
    }


}