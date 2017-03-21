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

//namespace Trinity.Framework.Helpers
//{
//    public interface IFrameworkLogger
//    {
//        /// <summary>
//        /// Prefix to be added to all log messages.
//        /// </summary>
//        string Prefix { get; set; }

//        /// <summary>
//        /// Logs a message without decoration or prefix.
//        /// </summary>
//        void LogRaw(string formatMessage);

//        /// <summary>
//        /// Logs a message for a specified category.
//        /// </summary>
//        void Log(LogCategory category, string formatMessage, params object[] args);

//        /// <summary>
//        /// Logs the message for a specified category.
//        /// </summary>
//        void Log(string formatMessage, params object[] args);

//        /// <summary>
//        /// Logs a verbose (excessive information, requires verbose logging to be enabled) message.
//        /// </summary>
//        void LogVerbose(string formatMessage, params object[] args);

//        /// <summary>
//        /// Logs a message as verbose (excessive information, requires verbose logging to be enabled) for a specified category.
//        /// </summary>
//        void LogVerbose(LogCategory category, string formatMessage, params object[] args);

//        /// <summary>
//        /// Logs a message to the log file without displaying it.
//        /// </summary>
//        void LogDebug(string formatMessage, params object[] args);

//        /// <summary>
//        /// Logs a message to the log file without displaying it for a specified category.
//        /// </summary>
//        void LogDebug(LogCategory logCategory, string formatMessage, params object[] args);

//        /// <summary>
//        /// Logs a message with Error/None
//        /// </summary>
//        /// <param name="formatMessage"></param>
//        /// <param name="args"></param>
//        void LogError(string formatMessage, params object[] args);

//        /// <summary>
//        /// Logs an error message for a specific category.
//        /// </summary>
//        void LogError(LogCategory logCategory, string formatMessage, params object[] args);

//        /// <summary>
//        /// Logs a warning message.
//        /// </summary>
//        void Warn(string s, params object[] args);

//        /// <summary>
//        /// Logs a warning message for a specific category.
//        /// </summary>
//        void Warn(LogCategory category, string s, params object[] args);
//    }

//    /// <summary>
//    /// Utilities help developer interact with DemonBuddy
//    /// </summary>
//    [DebuggerStepThrough]
//    public class Logger : IFrameworkLogger
//    {
//        private readonly ILog _Logger = Zeta.Common.Logger.GetLoggerInstanceForType();
//        private string prefix = string.Empty;

//        public string Prefix
//        {
//            get { return prefix; }
//            set { prefix = value; }
//        }

//        private readonly Dictionary<Tuple<LogCategory, TrinityLogLevel>, string> LastLogMessages = new Dictionary<Tuple<LogCategory, TrinityLogLevel>, string>();

//        /// <summary>Logs the specified level.</summary>
//        /// <param name="level">The logging level.</param>
//        /// <param name="category">The category.</param>
//        /// <param name="formatMessage">The format message.</param>
//        /// <param name="args">The parameters used when format message.</param>
//        public void Log(TrinityLogLevel level, LogCategory category, string formatMessage, params object[] args)
//        {
//            if (string.IsNullOrEmpty(prefix))
//            {
//                prefix = $"[Trinity {Trinity.TrinityPlugin.Instance.Version}]";
//            }

//            var isCategory = Trinity.TrinityPlugin.IsEnabled && Core.Settings != null && Core.Settings.Advanced.LogCategories.HasFlag(category);

//            if (category == LogToCategory.UserInformation || level >= TrinityLogLevel.Error || isCategory)
//            {
//                string msg = string.Format(prefix + "{0} {1}", category != LogToCategory.UserInformation ? "[" + category + "]" : string.Empty, formatMessage);

//                try
//                {
//                    if (args.Length > 0)
//                        msg = string.Format(msg, args);
//                }
//                catch
//                {
//                    msg = msg + " || " + args;
//                }
//                switch (level)
//                {
//                    case TrinityLogLevel.Error:
//                        _Core.Logger.Error(msg);
//                        break;

//                    case TrinityLogLevel.Info:
//                        _Core.Logger.Log(msg);
//                        break;

//                    case TrinityLogLevel.Verbose:
//                        _Core.Logger.Debug(msg);
//                        break;

//                    case TrinityLogLevel.Debug:
//                        LogToTrinityDebug(msg);
//                        break;
//                }
//            }
//        }

//        public void Log(TrinityLogLevel level, string formatMessage, params object[] args)
//        {
//            Log(level, LogToCategory.UserInformation, formatMessage, args);
//        }

//        /// <summary>Logs the message to Normal log level</summary>
//        /// <param name="category">The category.</param>
//        /// <param name="formatMessage">The format message.</param>
//        /// <param name="args">The parameters used when format message.</param>
//        public void Log(LogCategory category, string formatMessage, params object[] args)
//        {
//            Log(TrinityLogLevel.Info, category, formatMessage, args);
//        }

//        /// <summary>Logs the message to Normal log level</summary>
//        /// <param name="category">The category.</param>
//        /// <param name="formatMessage">The format message.</param>
//        /// <param name="args">The parameters used when format message.</param>
//        public void Log(string formatMessage)
//        {
//            LogNormal(formatMessage, 0);
//        }

//        public void LogRaw(string formatMessage)
//        {
//            _Core.Logger.Log(formatMessage);
//        }

//        public void Log(string formatMessage, params object[] args)
//        {
//            LogNormal(formatMessage, args);
//        }

//        /// <summary>
//        /// Logs a message with Normal/None
//        /// </summary>
//        /// <param name="formatMessage"></param>
//        /// <param name="args"></param>
//        public void LogNormal(string formatMessage, params object[] args)
//        {
//            Log(TrinityLogLevel.Info, LogToCategory.UserInformation, formatMessage, args);
//        }

//        /// <summary>
//        /// Logs a message with Normal/None
//        /// </summary>
//        /// <param name="formatMessage"></param>
//        /// <param name="args"></param>
//        public void LogVerbose(string formatMessage, params object[] args)
//        {
//            Log(TrinityLogLevel.Verbose, LogToCategory.UserInformation, formatMessage, args);
//        }

//        /// <summary>
//        /// Logs a message as verbose with the specified category
//        /// </summary>
//        /// <param name="category"></param>
//        /// <param name="formatMessage"></param>
//        /// <param name="args"></param>
//        public void LogVerbose(LogCategory category, string formatMessage, params object[] args)
//        {
//            Log(TrinityLogLevel.Verbose, category, formatMessage, args);
//        }

//        /// <summary>
//        /// Logs a message with Debug/None
//        /// </summary>
//        /// <param name="formatMessage"></param>
//        /// <param name="args"></param>
//        public void LogDebug(string formatMessage, params object[] args)
//        {
//            Log(TrinityLogLevel.Debug, LogToCategory.UserInformation, formatMessage, args);
//        }

//        /// <summary>
//        /// Logs a message with Debug/None
//        /// </summary>
//        /// <param name="formatMessage"></param>
//        /// <param name="args"></param>
//        public void LogDebug(LogCategory logCategory, string formatMessage, params object[] args)
//        {
//            Log(TrinityLogLevel.Debug, logCategory, formatMessage, args);
//        }

//        /// <summary>
//        /// Logs a message with Error/None
//        /// </summary>
//        /// <param name="formatMessage"></param>
//        /// <param name="args"></param>
//        public void LogError(string formatMessage, params object[] args)
//        {
//            Log(TrinityLogLevel.Error, LogToCategory.UserInformation, formatMessage, args);
//        }

//        /// <summary>
//        /// Logs a message with Error/None
//        /// </summary>
//        /// <param name="formatMessage"></param>
//        /// <param name="args"></param>
//        public void LogError(LogCategory logCategory, string formatMessage, params object[] args)
//        {
//            Log(TrinityLogLevel.Error, logCategory, formatMessage, args);
//        }

//        /// <summary>Converts <see cref="TrinityLogLevel"/> to <see cref="LogLevel"/>.</summary>
//        /// <param name="level">The trinity logging level.</param>
//        /// <returns>DemonBuddy logging level.</returns>
//        private LogLevel ConvertToLogLevel(TrinityLogLevel level)
//        {
//            LogLevel logLevel = LogLevel.Debug;
//            switch (level)
//            {
//                case TrinityLogLevel.Error:
//                    logLevel = LogLevel.Error;
//                    break;

//                case TrinityLogLevel.Info:
//                    logLevel = LogLevel.Info;
//                    break;

//                case TrinityLogLevel.Verbose:
//                    logLevel = LogLevel.Verbose;
//                    break;

//                case TrinityLogLevel.Debug:
//                    logLevel = LogLevel.Debug;
//                    break;
//            }
//            return logLevel;
//        }

//        public string ListToString(List<object> list)
//        {
//            string result = "";
//            for (int i = 0; i < list.Count; i++)
//            {
//                result += list[i];
//                if (i < list.Count - 1)
//                    result += ", ";
//            }
//            return result;
//        }

//        /*
//         * Log4Net logging
//         */

//        private ILog trinityLog;
//        private log4net.Filter.LoggerMatchFilter trinityFilter;
//        private PatternLayout trinityLayout;
//        private log4net.Repository.Hierarchy.Logger trinityLogger;
//        private FileAppender trinityAppender;
//        private Object _loglock = 0;

//        private void SetupLogger()
//        {
//            try
//            {
//                if (trinityLog == null)
//                {
//                    lock (_loglock)
//                    {
//                        int myPid = Process.GetCurrentProcess().Id;
//                        DateTime startTime = Process.GetCurrentProcess().StartTime;

//                        trinityLayout = new PatternLayout("%date{HH:mm:ss.fff} %-5level %logger{1} %m%n");
//                        trinityLayout.ActivateOptions();

//                        trinityFilter = new log4net.Filter.LoggerMatchFilter();
//                        trinityFilter.LoggerToMatch = "TrinityPlugin";
//                        trinityFilter.AcceptOnMatch = true;
//                        trinityFilter.ActivateOptions();

//                        Hierarchy h = (Hierarchy)LogManager.GetRepository();
//                        var appenders = h.GetAppenders();

//                        foreach (var appender in appenders)
//                        {
//                            if (appender is FileAppender)
//                            {
//                                trinityAppender = appender as FileAppender;
//                            }
//                        }

//                        trinityAppender.Layout = trinityLayout;
//                        //trinityAppender.AddFilter(trinityFilter);
//                        trinityAppender.LockingModel = new FileAppender.ExclusiveLock();
//                        //trinityAppender.LockingModel = new FileAppender.InterProcessLock();

//                        trinityAppender.ActivateOptions();

//                        trinityLog = LogManager.GetLogger("TrinityDebug");
//                        trinityLogger = ((log4net.Repository.Hierarchy.Logger)trinityLog.Logger);
//                        trinityLogger.Additivity = false;
//                        trinityLogger.AddAppender(trinityAppender);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                _Core.Logger.Error("Error setting up TrinityPlugin Logger:\n" + ex.ToString());
//            }
//        }

//        public void LogToTrinityDebug(string message, params object[] args)
//        {
//            try
//            {
//                SetupLogger();

//                string data = "";

//                if (message.Contains("{c:"))
//                    data = message + " args: " + args;
//                else if (args != null && args.Length > 0)
//                    data = string.Format(message, args);
//                else
//                    data = message;

//                trinityLog.Debug(data);
//            }
//            catch (Exception ex)
//            {
//                _Core.Logger.Error("Error in LogToTrinityDebug: " + ex.ToString());
//            }
//        }

//        internal string CallingClassName
//        {
//            get
//            {
//                var result = string.Empty;
//                try
//                {
//                    var frame = new StackFrame(2);
//                    var method = frame.GetMethod();

//                    if (method.DeclaringType != null)
//                        result = method.DeclaringType.Name;
//                }
//                catch (Exception ex)
//                {
//                    LogDebug("Exception in Logger.ClassTag method. {0} {1}", ex.Message, ex.InnerException);
//                }
//                return result;
//            }
//        }

//        internal string CallingMethodName
//        {
//            get
//            {
//                var result = string.Empty;
//                try
//                {
//                    var frame = new StackFrame(2);
//                    result = frame.GetMethod().Name;
//                }
//                catch (Exception ex)
//                {
//                    LogDebug("Exception in Logger.ClassTag method. {0} {1}", ex.Message, ex.InnerException);
//                }
//                return result;
//            }
//        }

//    }
//}
}