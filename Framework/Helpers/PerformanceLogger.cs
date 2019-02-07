using Serilog;
using System;
using System.Diagnostics;
using Zeta.Common;

namespace Trinity.Framework.Helpers
{
    [DebuggerStepThrough]
    public class PerformanceLogger : IDisposable
    {
        private static readonly ILogger s_logger = Logger.GetLoggerInstanceForType();
        private readonly string _blockName;
        private readonly Stopwatch _stopwatch;
        private bool _isDisposed;
        private readonly bool _forceLog;

        public PerformanceLogger(string blockName, bool forceLog = false)
        {
            _forceLog = forceLog;
            _blockName = blockName;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _stopwatch.Stop();
            if (_stopwatch.Elapsed.TotalMilliseconds > 5 || _forceLog)
            {
                if (Core.Settings.Advanced.LogCategories.HasFlag(LogCategory.Performance) || _forceLog)
                {
                    s_logger.Debug($"[{_blockName}] Execution  took {_stopwatch.Elapsed}.");
                }
                else if (_stopwatch.Elapsed.TotalMilliseconds > 1000)
                {
                    s_logger.Error($"[{_blockName}] Execution  took {_stopwatch.Elapsed}.");
                }
            }
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members

        ~PerformanceLogger()
        {
            Dispose();
        }

        public TimeSpan Elapsed => _stopwatch.Elapsed;

        public override string ToString()
        {
            return _stopwatch.Elapsed.ToString();
        }
    }
}
