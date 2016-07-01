namespace Trinity.Framework.Helpers.Exporter
{
    public static class Exporter
    {
        public static DictionaryExporter Dictionary => _dictionaryExporter ?? (_dictionaryExporter = new DictionaryExporter());
        private static DictionaryExporter _dictionaryExporter;

        public static EnumExporter Enum => _enumExporter ?? (_enumExporter = new EnumExporter());
        private static EnumExporter _enumExporter;
    }
}


