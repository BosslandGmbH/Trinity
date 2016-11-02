using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.Scripting.Interpreter;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory.Misc;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class Map<TValue> : MemoryWrapper, IEnumerable<TValue> where TValue : MemoryWrapper,  new()
    {
        public const int SizeOf = 0x70; // 112
        public int x00_Mask => ReadOffset<int>(0x00);
        public int x04_Count => ReadOffset<int>(0x04);
        public Allocator<Pair> x08_PtrEntryAllocator => ReadPointer<Allocator<Pair>>(0x08);
        public int _x0C => ReadOffset<int>(0x0C);
        public PtrTable<Pair> _0x10_Buckets => ReadObject<PtrTable<Pair>>(0x10);
        public int _x64 => ReadOffset<int>(0x00);
        public int _x68 => ReadOffset<int>(0x00);
        public int _x6C => ReadOffset<int>(0x00);

        private IEnumerable<Pair> Data
        {
            get
            {      
                foreach (var firstItem in _0x10_Buckets.Items)
                {
                    var pair = firstItem;
                    while (pair != null)
                    {
                        yield return pair;
                        pair = pair.x00_Next;
                    }
                }  
            }
        }

        public List<TValue> Entries => Data.Select(d => d.x08_Value).ToList();
        public Dictionary<int, TValue> ToDictionary() => Data.ToDictionary(k => k.x04_Key, v => v.x08_Value);


        public string ToEnum(Func<int, TValue, Tuple<string, string, string>> enumPartSelector, string name = "")
        {
            var indent = "    ";
            var sb = new StringBuilder();
            sb.AppendLine($"public enum {(string.IsNullOrEmpty(name) ? "Name" : name)} // Count={Entries.Count}, Build={ZetaDia.Memory.Process.MainModule.FileVersionInfo.FileVersion.Replace(", ",".")}");
            sb.AppendLine("{");
            var seenKeys = new HashSet<string>();
            foreach (var entry in Data)
            {
                var def = enumPartSelector(entry.x04_Key, entry.x08_Value);
                var key = def.Item1.Trim();

                var dupeToken = string.Empty;
                if (seenKeys.Contains(key))
                {
                    dupeToken = "// DUPLICATE ";
                }
                else
                {
                    seenKeys.Add(key);
                }

                var firstChar = key.FirstOrDefault();
                var safeKey = !char.IsLetter(firstChar) && !'_'.Equals(firstChar) ? "_" + key : key;
                safeKey = safeKey.Replace(" ", "");
                safeKey = safeKey.Replace("'", "");
                safeKey = safeKey.Replace("-", "");
                safeKey = safeKey.Replace(")", "");
                safeKey = safeKey.Replace("(", "_");


                var line = $"{indent}{dupeToken}{safeKey} = {def.Item2}, // {def.Item3} KeyHash={MemoryHelper.GameBalanceNormalHash(key)} {(key != safeKey ? $"Original={key}" : string.Empty)}";
                sb.AppendLine(line);
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        public class Pair : MemoryWrapper
        {
            public static int SizeOf = 12;
            public Pair x00_Next => ReadPointer<Pair>(0x00);
            public int x04_Key => ReadOffset<int>(0x04);
            public TValue x08_Value => ReadPointer<TValue>(0x08);
            public int ModKey => x04_Key;
            public override string ToString() => $"{typeof(TValue).Name}: {x04_Key}, {x08_Value}";
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<TValue> GetEnumerator() => Data.Select(d => d.x08_Value).GetEnumerator();
        public override string ToString() => $"{GetType().Name}: {typeof(TValue)}: Count={x04_Count} {x08_PtrEntryAllocator}";

    }


}