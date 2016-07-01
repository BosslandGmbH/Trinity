using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Trinity.Framework.Helpers.Exporter
{
    public class EnumExporter : ExporterBase, IStructureExporter
    {
        public ExportOptions DefaultExportOptions = new ExportOptions();

        /// <summary>
        /// Converts data into c# enum code.
        /// </summary>
        /// <param name="input">Output format "[Key] = [Value]"</param>
        /// <param name="options">Options to be applied</param>
        /// <returns></returns>
        public string Create(IDictionary<string, string> input, ExportOptions options = null)
        {
            options = options ?? DefaultExportOptions;

            var sb = new StringBuilder();
            var indent = 0;

            sb.AppendLine($"public enum {options.Name} ");

            sb.AppendLine(OpenBracketLine(indent));

            WriteIndentedLoop(input, indent, sb, (i, k, v) =>
            {
                var value = options.QuoteValue ? $"\"{v}\"" : v.ToString();
                sb.AppendLine(IndentedLine(i, " {0}{1} = {2}{3},", options.KeyPrefix, k.Replace(" ","_"), options.ValuePrefix, value));
            });

            sb.AppendLine(CloseBracketLine(indent));

            return sb.ToString();            
        }

        private void WriteIndentedLoop(IDictionary<string, string> input, int indent, StringBuilder sb, Action<int, string,string> writer)
        {
            indent++;
            foreach (var pair in input)
            {
                if (writer != null)
                    writer(indent, pair.Key, pair.Value);
            }
        }
    }
}





