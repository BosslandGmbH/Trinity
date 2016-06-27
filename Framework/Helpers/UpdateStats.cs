using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Technicals;

namespace Trinity.Framework.Helpers
{
    public class UpdateStats
    {
        private int _updateCount;
        private int _currentSecond;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private double _totalTimeMs;

        public string Name { get; set; }
        public double AverageUpdateTimeMilliseconds { get; set; }
        public double TotalTimeMilliseconds { get; set; }
        public int UpdateCount { get; set; }

        public void Start()
        {
            _stopwatch.Restart();
        }

        public void Stop()
        {
            _stopwatch.Stop();

            var currentTime = DateTime.UtcNow;
            var currentSecond = currentTime.Second;

            if (_currentSecond != currentSecond)
            {
                _currentSecond = currentSecond;
                OnSecondChanged();
                Reset();
                return;
            }
      
            _updateCount++;
            _totalTimeMs += _stopwatch.Elapsed.TotalMilliseconds;          
        }

        private void OnSecondChanged()
        {
            if (_updateCount == 0 || Math.Abs(_totalTimeMs) < double.Epsilon) return;

            UpdateCount = _updateCount;
            TotalTimeMilliseconds = _totalTimeMs;
            AverageUpdateTimeMilliseconds = _totalTimeMs / _updateCount;
            Logger.Log(LogCategory.Performance, ToString());
        }

        public override string ToString()
        {
            return $"{Name} PerSecondStat: Updates={UpdateCount} Total={TotalTimeMilliseconds:N4}ms Average={AverageUpdateTimeMilliseconds:N4}ms";
        }
    
        private void Reset()
        {
            _updateCount = 0;
            _totalTimeMs = 0;
        }
    }
}


