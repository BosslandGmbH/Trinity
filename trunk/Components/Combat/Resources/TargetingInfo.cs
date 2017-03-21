using System;
namespace Trinity.Components.Combat.Resources
{
    public class TargetingInfo
    {
        private bool _isTargetted;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime FirstTargetedTime { get; set; } = DateTime.MinValue;
        public DateTime LastTargetedTime { get; set; } = DateTime.MinValue;
        public DateTime LastUntargetedTime { get; set; } = DateTime.MinValue;
        public TimeSpan TotalTargetedTime { get; set; }
        public int TargetedTimes { get; set; }

        public bool IsTargetted
        {
            get { return _isTargetted; }
            set
            {
                UpdateTargetingInfo(value);
                _isTargetted = value;
            }
        }

        private void UpdateTargetingInfo(bool value)
        {
            if (value)
            {
                TargetedTimes++;
                LastTargetedTime = DateTime.UtcNow;

                if (FirstTargetedTime == DateTime.MinValue)
                    FirstTargetedTime = DateTime.UtcNow;
            }
            else
            {
                LastUntargetedTime = DateTime.UtcNow;

                if (LastTargetedTime != DateTime.MinValue)
                    TotalTargetedTime += DateTime.UtcNow - LastTargetedTime;
            }
        }

        public TimeSpan TimeSinceFirstTargeted
            => FirstTargetedTime != DateTime.MinValue ? DateTime.UtcNow - FirstTargetedTime : TimeSpan.Zero;

        public TimeSpan TimeSinceLastTargeted
            => LastTargetedTime != DateTime.MinValue ? DateTime.UtcNow - LastTargetedTime : TimeSpan.Zero;

        public override string ToString()
            => $"{GetType().Name}: " +
               $"Targeted={TargetedTimes} " +
               $"TotalTime={TotalTargetedTime.TotalSeconds:N2}s " +
               $"SinceFirstTargeted={TimeSinceFirstTargeted.TotalSeconds:N2}s " +
               $"SinceLastTargeted={TimeSinceLastTargeted.TotalSeconds:N2}s";
    }
}