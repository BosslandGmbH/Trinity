using System;
using System.Diagnostics;

namespace Trinity.Components.Adventurer.Util
{
    public class PerformanceLogger : IDisposable
    {
        private Stopwatch Stopwatch { get; set; }
        private readonly string _message;
        private readonly bool _enabled;
        private readonly bool _ignoreMinimumElapsed;

        public PerformanceLogger(string message = null, bool enabled = false, bool ignoreMinimumElapsed = false)
        {
            Stopwatch = new Stopwatch();
            _enabled = enabled;
            _ignoreMinimumElapsed = ignoreMinimumElapsed;
            if (!_enabled) return;
            if (message == null)
            {
                var frame = new StackFrame(1);
                var method = frame.GetMethod();
                var type = method.DeclaringType;
                message = (type == null ? string.Empty : type.FullName + ".") + method.Name;
            }
            _message = message;
            Stopwatch.Start();
        }

        public void Dispose()
        {
            var elapsed = Stopwatch.Elapsed.TotalMilliseconds;
            if (!_enabled) return;
            elapsed = Math.Round(elapsed * 10000) / 10000;
            if (!_ignoreMinimumElapsed && elapsed < 2) return;
            Logger.Debug(string.Format("{0} took {1} ms.", _message, elapsed));
            Stopwatch.Stop();
            Stopwatch = null;
        }
    }
}
