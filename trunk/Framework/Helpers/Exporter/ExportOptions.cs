namespace Trinity.Framework.Helpers.Exporter
{
    public class ExportOptions
    {
        public bool QuoteValue { get; set; } = true;
        public string KeyPrefix { get; set; } = string.Empty;
        public string ValuePrefix { get; set; } = string.Empty;
        public string Name { get; set; } = "Name";
    }
}
