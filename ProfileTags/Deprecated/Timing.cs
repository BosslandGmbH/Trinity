//using System;
//using Trinity.Framework;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    /// <summary>
//    /// Timing Object, tracks a period of time
//    /// </summary>
//    public class Timing
//    {
//        public string Name = string.Empty;
//        public string Group = string.Empty;
//        public bool IsRunning = false;
//        public bool IsStarted = false;
//        public DateTime StartTime = DateTime.MinValue;
//        public DateTime StopTime = DateTime.MinValue;
//        public int TimesTimed = 0;
//        public int TotalTimeSeconds = 0;
//        public int MaxTimeSeconds = 0;
//        public int MinTimeSeconds = 0;
//        public int FailedCount = 0;
//        public int ObjectiveCount = 0;
//        public bool AllowResetStartTime = false;
//        public bool IsDirty = false;
//        public bool ObjectiveComplete = false;


//        public TimeSpan Elapsed
//        {
//            get { return DateTime.UtcNow.Subtract(StartTime); }
//        }

//        public float TimeAverageSeconds
//        {
//            get
//            {
//                if (TimesTimed > 0)
//                {
//                    if (MinTimeSeconds == MaxTimeSeconds)
//                    {
//                        return MaxTimeSeconds;
//                    }
//                    return (float)TotalTimeSeconds / (float)TimesTimed;
//                }
//                return 0;
//            }
//        }

//        public double ObjectivePercent
//        {
//            get { return (TimesTimed > 0) ? (float) ObjectiveCount/(float) TimesTimed*100 : 0; }
//        }

//        /// <summary>
//        /// Convert a number of seconds into a friendly time format for display
//        /// </summary>
//        public string FormatTime(int seconds)
//        {
//            var t = TimeSpan.FromSeconds(seconds);
//            if (seconds == 0) return "0";
//            var format = t.Hours > 0 ? "{0:0}h " : string.Empty;
//            format += t.Minutes > 0 ? "{1:0}m " : string.Empty;
//            format += t.Seconds > 0 ? "{2:0}s" : string.Empty;
//            return string.Format(format, t.Hours, t.Minutes, t.Seconds);
//        }

//        public string FormatTime(TimeSpan elapsed)
//        {
//            return FormatTime((int)elapsed.TotalSeconds);
//        }

//        /// <summary>
//        /// Start the timer
//        /// </summary>
//        public void Start()
//        {
//            IsRunning = true;
//            if (StartTime == DateTime.MinValue || AllowResetStartTime)
//            {
//                StartTime = DateTime.UtcNow;
//            };
//        }

//        /// <summary>
//        /// Adds this timer to another timer
//        /// </summary>
//        public Timing Add(Timing timer)
//        {
//            timer.TimesTimed += this.TimesTimed;
//            timer.TotalTimeSeconds += this.TotalTimeSeconds;
//            timer.MaxTimeSeconds = this.MaxTimeSeconds > timer.MaxTimeSeconds ? this.MaxTimeSeconds : timer.MaxTimeSeconds;
//            timer.MinTimeSeconds = timer.MinTimeSeconds == 0 || this.MinTimeSeconds < timer.MinTimeSeconds ? this.MinTimeSeconds : timer.MinTimeSeconds;
//            timer.ObjectiveCount = this.ObjectiveCount > timer.ObjectiveCount ? this.ObjectiveCount : timer.ObjectiveCount;
//            return timer;
//        }

//        /// <summary>
//        /// Update statistics for the timer
//        /// </summary>
//        public void Update()
//        {
//            TimesTimed = TimesTimed + 1;
//            TotalTimeSeconds += (int)Elapsed.TotalSeconds;
//            MaxTimeSeconds = (int)Elapsed.TotalSeconds > MaxTimeSeconds ? (int)Elapsed.TotalSeconds : MaxTimeSeconds;
//            MinTimeSeconds = MinTimeSeconds == 0 || (int)Elapsed.TotalSeconds < MinTimeSeconds ? (int)Elapsed.TotalSeconds : MinTimeSeconds;
//            ObjectiveCount = ObjectiveComplete ? ObjectiveCount + 1 : ObjectiveCount;
//        }

//        /// <summary>
//        /// Stop the timer
//        /// </summary>
//        public void Stop()
//        {
//            ObjectiveComplete = false;
//            IsRunning = false;
//            StartTime = DateTime.MinValue;
//            StopTime = DateTime.UtcNow;
//        }

//        /// <summary>
//        /// Write the current state of this timer instance to the console
//        /// </summary>
//        public void DebugPrint(string message = "")
//        {
//            var modified = (IsDirty) ? " >>" : string.Empty;

//            var format = message + modified + " '{0}' Group:{11} Running={4} (Max={5}, Min={6}, Avg={7} Obj={16} Obj={15:0##}% from {9} timings) {13} Objective={14}";

//            var vars = new object[]
//            {
//                Name,
//                string.Empty,
//                string.Empty,
//                string.Empty,
//                IsRunning,
//                FormatTime(MaxTimeSeconds),
//                FormatTime(MinTimeSeconds),
//                FormatTime((int) TimeAverageSeconds),
//                TotalTimeSeconds,
//                TimesTimed,
//                string.Empty,
//                Group,
//                FailedCount,
//                IsRunning ? " Elapsed: " + FormatTime((int)Elapsed.TotalSeconds) : string.Empty,
//                ObjectiveComplete,
//                ObjectivePercent,
//                ObjectiveCount
//            };        
//        }

//        public void PrintSimple(string message = "")
//        {
//            Core.Logger.Warn(message + " {0} ({1}) took {2:0.#}", Name, Group, FormatTime(Elapsed));
//        }



//    }
//}
