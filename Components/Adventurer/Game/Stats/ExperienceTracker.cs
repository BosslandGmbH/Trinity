using System;
using log4net;
using Trinity.Framework;
using Trinity.Components.Adventurer.Game.Events;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Game.Stats
{
    public class ExperienceTracker : PulsingObject
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();

        private double _currentExperience;
        private double _minExperience;
        private double _maxExperience;
        private double _minExperienceRun;
        private double _maxExperienceRun;
        private double _totalExperience;
        private DateTime _startTime;
        private DateTime _tickStartTime;
        private TimeSpan _bestTime = TimeSpan.Zero;
        private TimeSpan _worseTime;
        private long _lastSeen;
        private int _tick;
        public bool IsStarted { get; private set; }

        public void Start()
        {
            if (_startTime == DateTime.MinValue)
                _startTime = DateTime.UtcNow;
            _tickStartTime = DateTime.UtcNow;
            _currentExperience = 0;
            _tick++;
            _lastSeen = GetExperience();
            EnablePulse();
            IsStarted = true;
            Core.Logger.Log("[XPTracker] Starting a new experience tracking session.");
            Current = this;
        }

        public class ExperienceTrackerResult
        {
            public TimeSpan TimeTaken;
            public double ExperienceGained { get; set; }
        }

        public static ExperienceTrackerResult LastResult = new ExperienceTrackerResult();
        public static ExperienceTracker Current = new ExperienceTracker();

        public void StopAndReport(string reporterName)
        {
            DisablePulse();
            if (IsStarted)
            {
                UpdateExperience();
                ReportExperience(reporterName);
            }
            IsStarted = false;
        }

        private void ReportExperience(string reporterName)
        {
            LastResult = GetResult();

            var timeOfLastRun = DateTime.UtcNow - _tickStartTime;
            var timeOfSession = DateTime.UtcNow - _startTime;
            var averageRunTime = new TimeSpan(timeOfSession.Ticks / _tick);
            var thisExpHour = _currentExperience / timeOfLastRun.TotalHours;

            if (_bestTime == TimeSpan.Zero || _bestTime > timeOfLastRun)
                _bestTime = timeOfLastRun;
            if (_worseTime < timeOfLastRun)
                _worseTime = timeOfLastRun;
            if (_maxExperienceRun < _currentExperience)
                _maxExperienceRun = _currentExperience;
            if (Math.Abs(_minExperienceRun) < double.Epsilon || _minExperienceRun > _currentExperience)
                _minExperienceRun = _currentExperience;
            if (_maxExperience < thisExpHour)
                _maxExperience = thisExpHour;
            if (Math.Abs(_minExperience) < double.Epsilon || _minExperience > thisExpHour)
                _minExperience = thisExpHour;

            s_logger.Info($"[{reporterName}] Runs count: {_tick}");
            s_logger.Info($"[{reporterName}] This run time: {timeOfLastRun}");
            s_logger.Info($"[{reporterName}] Average run time: {averageRunTime}");
            s_logger.Info($"[{reporterName}] Best run time: {_bestTime}");
            s_logger.Info($"[{reporterName}] Worse run time: {_worseTime}");
            s_logger.Info($"[{reporterName}] This run XP Gained: {_currentExperience:0,0}");
            s_logger.Info($"[{reporterName}] This run / Hour: {thisExpHour:0,0}");
            s_logger.Info($"[{reporterName}] Total XP Gained: {_totalExperience:0,0}");
            s_logger.Info($"[{reporterName}] Total XP / Hour: {(_totalExperience / timeOfSession.TotalHours):0,0}");
            s_logger.Info($"[{reporterName}] Best XP / Hour (single run): {_maxExperience:0,0}");
            s_logger.Info($"[{reporterName}] Worse XP / Hour (single run): {_minExperience:0,0}");
            s_logger.Info($"[{reporterName}] Best XP / Single run: {_maxExperienceRun:0,0}");
            s_logger.Info($"[{reporterName}] Worse XP / Single run: {_minExperienceRun:0,0}");
        }

        public static ExperienceTrackerResult GetResult()
        {
            return new ExperienceTrackerResult
            {
                TimeTaken = DateTime.UtcNow - Current._tickStartTime,
                ExperienceGained = Current._currentExperience,
            };
        }

        private void UpdateExperience()
        {
            var currentLastSeen = GetExperience();
            if (_lastSeen < currentLastSeen)
            {
                _currentExperience += (currentLastSeen - _lastSeen);
                _totalExperience += (currentLastSeen - _lastSeen);
            }
            _lastSeen = currentLastSeen;
        }

        private static long GetExperience()
        {
            if (ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                return 0;

            return ZetaDia.Me.Level == 70
                ? ZetaDia.Me.ParagonCurrentExperience
                : ZetaDia.Me.CurrentExperience;
        }

        protected override void OnPulse()
        {
            UpdateExperience();
        }
    }
}