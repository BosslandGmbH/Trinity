using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Framework.Helpers;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory.Symbols
{
    public class SymbolManager
    {
        public static Dictionary<int, SymbolTable> Tables => _tables ?? (_tables = ReadTables());
        private static Dictionary<int, SymbolTable> _tables;

        protected static Dictionary<int, SymbolTable> ReadTables()
        {
            var tables = new Dictionary<int, SymbolTable>();
            var address = Internals.Addresses.SymbolManager;
            var table = MemoryWrapper.Create<SymbolTable>(address);
            var i = 0;

            while (table != null)
            {
                if (!table.Symbols.Any())
                    break;

                if (SymbolTableNames.SymbolTableNamesByIndex.ContainsKey(i))
                {
                    table.Name = SymbolTableNames.SymbolTableNamesByIndex[i];
                }

                table.Index = i;
                tables.Add((int)table.BaseAddress, table);
                table = MemoryWrapper.Create<SymbolTable>(table.NextTableAddress);
                i++;
            }
            return tables;
        }

        public static void GenerateEnums()
        {
            var sb = new List<string>();
            Action<string> add = str => sb.Add(str);
            var build = ZetaDia.Memory.Process.MainModule.FileVersionInfo.FileVersion.Replace(", ",".");
            var indent = "    ";
            var sortedTables = Tables.OrderByDescending(t => !string.IsNullOrEmpty(t.Value.Name)).ThenBy(t => t.Value.Index);
            foreach (var pair in sortedTables)
            {
                WriteTable(pair, add, build, indent);
            }
            WriteTableNamesDictionary(add, indent);
            DebugUtil.WriteLinesToLog("Symbols.txt", sb, true);
        }

        private static void WriteTable(KeyValuePair<int, SymbolTable> pair, Action<string> add, string build, string indent)
        {
            var table = pair.Value;
            add(string.Empty);
            add($"public enum {table.Name} // {build} @{pair.Key} index:{table.Index}");
            add("{");
            foreach (var symbol in table.Symbols)
            {
                add($"{indent} {GetEnumValueName(symbol.Name)} = {symbol.Id},");
            }
            add("}");
            add(string.Empty);
        }

        private static void WriteTableNamesDictionary(Action<string> add, string indent)
        {
            add(string.Empty);
            add($"public static Dictionary<int, string> SymbolTableNamesByIndex = new Dictionary<int, string>");
            add("{");
            foreach (var table in Tables.Values)
            {
                add($"{indent} {{ {table.Index}, \"{(string.IsNullOrEmpty(table.Name)? "_Enum_" + table.BaseAddress : table.Name)}\"}},");
            }
            add("};");
            add(string.Empty);
        }

        private static string GetEnumValueName(string name)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in name)
            {                           
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    builder.Append(c);
            }
            var result = builder.ToString();
            return string.IsNullOrEmpty(result) ? "Unknown" : result;
        }
    }
}

