using System.Collections.Generic;

namespace Trinity.Framework.Helpers.Exporter
{
    public interface IStructureExporter
    {
        string Create(IDictionary<string, string> input, ExportOptions options);
    }
}
