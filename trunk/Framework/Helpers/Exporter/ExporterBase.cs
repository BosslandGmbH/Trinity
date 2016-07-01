namespace Trinity.Framework.Helpers.Exporter
{
    public class ExporterBase
    {
        public const string Indent = "    ";

        public string OpenBracketLine(int indent = 0)
        {
            return GetIndent(indent) + "{";
        }

        public string CloseBracketLine(int indent = 0)
        {
            return GetIndent(indent) + "}";
        }

        public string CommentLine(int indent, string msg, params object[] args)
        {
            return @"//" + GetIndent(indent) + string.Format(msg, args);
        }

        public string IndentedLine(int indent, string msg, params object[] args)
        {
            return GetIndent(indent) + string.Format(msg, args);
        }

        private static string GetIndent(int indentCount)
        {
            var indent = string.Empty;
            for (var i = 0; i < indentCount; i++)
            {
                indent += Indent;
            }
            return indent;
        }
    }
}

