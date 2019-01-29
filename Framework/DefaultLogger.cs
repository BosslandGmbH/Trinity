using Serilog;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Zeta.Common;

namespace Trinity.Framework
{
    public interface IFrameworkLogger
    {
        string Prefix { get; set; }
        void Debug(string s, params object[] args);
        void Debug(LogCategory category, string s, params object[] args);
        void Log(string s, params object[] args);
        void Log(LogCategory category, string s, params object[] args);
        void Raw(string s, params object[] args);
        void Raw(LogCategory category, string s, params object[] args);
        void Error(string s, params object[] args);
        void Error(LogCategory category, string s, params object[] args);
        void Verbose(string s, params object[] args);
        void Verbose(LogCategory category, string s, params object[] args);
        void Warn(string s, params object[] args);
        void Warn(LogCategory category, string s, params object[] args);
    }

    public class DefaultLogger : IFrameworkLogger
    {
        private static readonly ILogger s_logger = Logger.GetLoggerInstanceForType();

        public string Prefix { get; set; } = $"{TrinityPlugin.Instance.Name} {TrinityPlugin.Instance.Version}";

        public void Log(string s, params object[] args)
            => LogToCategory(LogCategory.None, TrinityLogLevel.Info, s, args);

        public void Log(LogCategory category, string s, params object[] args)
            => LogToCategory(category, TrinityLogLevel.Info, s, args);

        public void Debug(string s, params object[] args)
            => LogToCategory(LogCategory.None, TrinityLogLevel.Debug, s, args);

        public void Debug(LogCategory category, string s, params object[] args)
            => LogToCategory(category, TrinityLogLevel.Debug, s, args);

        public void Warn(string s, params object[] args)
            => LogToCategory(LogCategory.None, TrinityLogLevel.Warn, s, args);

        public void Warn(LogCategory category, string s, params object[] args)
            => LogToCategory(category, TrinityLogLevel.Warn, s, args);

        public void Verbose(string s, params object[] args)
            => LogToCategory(LogCategory.None, TrinityLogLevel.Verbose, s, args);

        public void Verbose(LogCategory category, string s, params object[] args)
            => LogToCategory(category, TrinityLogLevel.Verbose, s, args);

        public void Raw(string s, params object[] args)
            => LogToCategory(LogCategory.None, TrinityLogLevel.Raw, s, args);

        public void Raw(LogCategory category, string s, params object[] args)
            => LogToCategory(category, TrinityLogLevel.Raw, s, args);

        public void Error(string s, params object[] args)
            => LogToCategory(LogCategory.None, TrinityLogLevel.Error, s, args);

        public void Error(LogCategory category, string s, params object[] args)
            => LogToCategory(category, TrinityLogLevel.Error, s, args);

        private string _lastMessage;
        private void LogToCategory(LogCategory category, TrinityLogLevel level, string s, object[] args)
        {
            if (args.Length > 0)
                s = string.Format(s, args);
            if (!(Core.Settings?.Advanced?.LogCategories.HasFlag(category) ?? true))
                return;

            var cat = category != LogCategory.None ? $" [{category}] " : string.Empty;
            var msg = $"[{Prefix}]{cat} {s}";

            if (_lastMessage == msg)
                return;
            _lastMessage = msg;
            switch (level)
            {
                case TrinityLogLevel.Warn:
                    s_logger.Warning(msg);
                    break;
                case TrinityLogLevel.Verbose:
                    s_logger.Verbose(msg);
                    break;
                case TrinityLogLevel.Debug:
                    s_logger.Debug(msg);
                    break;
                case TrinityLogLevel.Error:
                    s_logger.Error(msg);
                    break;
                case TrinityLogLevel.Info:
                    s_logger.Information(msg);
                    break;
                case TrinityLogLevel.Raw:
                    s_logger.Information(s);
                    break;
            }

        }
    }
}
