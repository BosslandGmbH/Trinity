using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;
using Trinity.Components.Adventurer.Settings;
using Zeta.Common;

namespace Trinity.Components.Adventurer.Util
{
    public static class Logger
    {
        private static readonly ILog DbLog = Zeta.Common.Logger.GetLoggerInstanceForType();

        public static void Log(LogLevel logLevel, string message, params object[] args)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Debug(message, args);
                    break;
                case LogLevel.Error:
                    Error(message, args);
                    break;
                case LogLevel.Info:
                    Info(message, args);
                    break;
                case LogLevel.Verbose:
                    Verbose(message, args);
                    break;
                case LogLevel.Overlay:
                    Info(message, args);
                    Overlay(message, args);
                    break;
            }
        }

        public static void Error(string message, params object[] args)
        {
            if (!message.StartsWith("["))
            {
                message = " " + message;
            }
            message = ClassTag + message;
            DbLog.ErrorFormat(message, args);
        }

        public static void Info(string message, params object[] args)
        {
            if (!message.StartsWith("["))
            {
                message = " " + message;
            }
            message = ClassTag + message;
            DbLog.InfoFormat(message, args);
        }

        public static void DebugSetting(string message, params object[] args)
        {
            if (PluginSettings.Current.DebugLogging)
            {
                if (!message.StartsWith("["))
                {
                    message = " " + message;
                }
                message = ClassTag + message;
                DbLog.InfoFormat(message, args);
            }
        }

        public static void Debug(string message, params object[] args)
        {
            if (!message.StartsWith("["))
            {
                message = " " + message;
            }
            message = ClassTag + message;
            DbLog.DebugFormat(message, args);
        }
        
        public static void Warn(string message, params object[] args)
        {
            if (!message.StartsWith("["))
            {
                message = " " + message;
            }
            message = ClassTag + message;
            DbLog.WarnFormat(message, args);
        }

        public static void Verbose(string message, params object[] args)
        {
            if (!message.StartsWith("["))
            {
                message = " " + message;
            }
            message = ClassTag + message;
            if (args.Any())
            {
                message = string.Format(message, args);
            }
            DbLog.Verbose(message);
        }

        private static void Overlay(string message, params object[] args)
        {
            if (args.Any())
            {
                message = string.Format(message, args);
            }
            //OverlayUI.AddLineToLog(message);
        }

        public static void Raw(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                DbLog.InfoFormat(message, args);
            }
            else
            {
                DbLog.Info(message);
            }
        }

        public static void RawWarning(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                DbLog.WarnFormat(message, args);
            }
            else
            {
                DbLog.Warn(message);
            }
        }
        public static void RawError(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                DbLog.ErrorFormat(message, args);
            }
            else
            {
                DbLog.Error(message);
            }
        }

        private static string ClassTag
        {
            get
            {
                var frame = new StackFrame(2);
                var method = frame.GetMethod();
                var type = method.DeclaringType;

                if (type == null)
                    return "[Adventurer] ";

                if ((type.Namespace == type.Name || type.Name.ToLowerInvariant().Contains("displayclass")) && !string.IsNullOrEmpty(type.Namespace))
                    return "[" + TaskNameRegex.Replace(type.Namespace, match => string.Empty) + "] ";

                return "[" + type.Namespace?.Split('.').LastOrDefault() + "][" + TaskNameRegex.Replace(type.Name, match => string.Empty) + "] ";
            }
        }

        private static Regex TaskNameRegex { get; } = new Regex(@"d__\d+", RegexOptions.Compiled);

    }

    public enum LogLevel
    {
        Info,
        Error,
        Debug,
        Verbose,
        Overlay
    }

}
