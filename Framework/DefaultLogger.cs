using log4net;
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
        private readonly ILog _Logger = Zeta.Common.Logger.GetLoggerInstanceForType();

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

        private void LogToCategory(LogCategory category, TrinityLogLevel level, string s, object[] args)
        {
            if (args.Length > 0)
                s = string.Format(s, args);

            if (Core.Settings?.Advanced?.LogCategories.HasFlag(category) ?? true)
            {
                var cat = category != LogCategory.None ? $" [{category}] " : string.Empty;
                var msg = $"[{Prefix}]{cat} {s}";

                switch (level)
                {
                    case TrinityLogLevel.Warn:
                        _Logger.Warn(msg);
                        break;
                    case TrinityLogLevel.Verbose:
                        _Logger.Debug(msg);
                        break;
                    case TrinityLogLevel.Debug:
                        _Logger.Debug(msg);
                        break;
                    case TrinityLogLevel.Error:
                        _Logger.Error(msg);
                        break;
                    case TrinityLogLevel.Info:
                        _Logger.Info(msg);
                        break;
                    case TrinityLogLevel.Raw:
                        _Logger.Info(s);
                        break;
                }
            }

        }
    }
}