using System;
namespace Trinity.Components.Combat.Resources
{
    public class TargetingInfo
    {
        private bool _isTargeted;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime FirstTargetedTime { get; set; } = DateTime.MinValue;
        public DateTime LastWasTargetTime { get; set; } = DateTime.MinValue;
        public DateTime LastNotTargetedTime { get; set; } = DateTime.MinValue;
        public DateTime LastTargetedTime { get; set; } = DateTime.MinValue;
        public DateTime LastUntargetedTime { get; set; } = DateTime.MinValue;
        public TimeSpan PreviousTargetedTime { get; set; }
        public int TargetedTimes { get; set; }


        public bool IsTargeted => _isTargeted;

        /// <summary>
        /// Update time and duration tracking.
        /// </summary>
        public void UpdateTargetInfo(bool isTargetted)
        {
            if (!_isTargeted && isTargetted)
            {
                OnGainedTargeting();
            }
            else if (_isTargeted && !isTargetted)
            {
                OnLostTargeting();
            }
            UpdateTargetingInfo(isTargetted);
            _isTargeted = isTargetted;
        }

        private void OnLostTargeting()
        {
            LastUntargetedTime = DateTime.UtcNow;
            PreviousTargetedTime += LastDurationAsTarget;
        }

        private void OnGainedTargeting()
        {
            LastTargetedTime = DateTime.UtcNow;
            TargetedTimes++;
        }

        private void UpdateTargetingInfo(bool isTargeted)
        {
            if (isTargeted)
            {                
                LastWasTargetTime = DateTime.UtcNow;

                if (FirstTargetedTime == DateTime.MinValue)
                    FirstTargetedTime = DateTime.UtcNow;
            }
            else
            {                
                LastNotTargetedTime = DateTime.UtcNow;
            }
        }

        public TimeSpan TimeSinceFirstTargeted
            => FirstTargetedTime != DateTime.MinValue ? DateTime.UtcNow - FirstTargetedTime : TimeSpan.Zero;

        public TimeSpan TimeSinceLastTargeted
            => FirstTargetedTime != DateTime.MinValue ? DateTime.UtcNow - LastTargetedTime : TimeSpan.Zero;

        public TimeSpan TimeSinceLastCurrentTarget
            => LastWasTargetTime != DateTime.MinValue ? DateTime.UtcNow - LastWasTargetTime : TimeSpan.Zero;        

        public TimeSpan LastDurationAsTarget
            => LastWasTargetTime != DateTime.MinValue ? LastWasTargetTime - LastTargetedTime : TimeSpan.Zero;

        public TimeSpan CurrentDurationAsTarget 
            => _isTargeted ? TimeSinceLastCurrentTarget : TimeSpan.Zero;

        public TimeSpan TotalTargetedTime 
            => PreviousTargetedTime + CurrentDurationAsTarget;


        public override string ToString()
            => $"{GetType().Name}: " +
               $"Targeted={TargetedTimes} " +
               $"TotalTime={TotalTargetedTime.TotalSeconds:N2}s " +
               $"SinceFirstTargeted={TimeSinceFirstTargeted.TotalSeconds:N2}s " +
               $"SinceLastTargeted={TimeSinceLastTargeted.TotalSeconds:N2}s";
    }
}