using System;
using System.Diagnostics;

namespace Trinity.Framework.Helpers
{
    //public static class PerformanceTracker
    //{
    //    public static ConcurrentDictionary<DateTime, TrackedPerformance> Performances = new ConcurrentDictionary<DateTime, TrackedPerformance>();

    //    public class TrackedPerformance
    //    {
    //        public TrackedPerformance(DateTime startTime, DateTime endTime)
    //        {
    //            StartTime = startTime;
    //            EndTime = endTime;
    //            Duration = endTime - startTime;
    //            Milliseconds = (float)Duration.TotalMilliseconds;
    //        }

    //        public DateTime StartTime { get; set; }
    //        public DateTime EndTime { get; set; }
    //        public TimeSpan Duration { get; set; }
    //        public float Milliseconds { get; set; }
    //    }
    //}

    [DebuggerStepThrough]
    public class PerformanceLogger : IDisposable
    {
        

        private static readonly log4net.ILog Logging = Zeta.Common.Logger.GetLoggerInstanceForType();
        private readonly string _BlockName;
        private readonly Stopwatch _Stopwatch;
        private bool _IsDisposed;
        private bool _ForceLog;

        public PerformanceLogger(string blockName, bool forceLog = false)
        {
            _ForceLog = forceLog;
            _BlockName = blockName;
            _Stopwatch = new Stopwatch();
            _Stopwatch.Start();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!_IsDisposed)
            {
                _IsDisposed = true;
                _Stopwatch.Stop();
                if (_Stopwatch.Elapsed.TotalMilliseconds > 5 || _ForceLog)
                {
                    if (Core.Settings.Advanced.LogCategories.HasFlag(LogCategory.Performance) || _ForceLog)
                    {
                        Logging.DebugFormat("[TrinityPlugin][Performance] Execution of {0} took {1:00.00000}ms.", _BlockName,
                                            _Stopwatch.Elapsed.TotalMilliseconds);
                    }
                    else if (_Stopwatch.Elapsed.TotalMilliseconds > 1000)
                    {
                        Logging.ErrorFormat("[TrinityPlugin][Performance] Execution of {0} took {1:00.00000}ms.", _BlockName,
                                            _Stopwatch.Elapsed.TotalMilliseconds);
                    }
                }
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        ~PerformanceLogger()
        {
            Dispose();
        }
    }
}
