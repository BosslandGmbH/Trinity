//using System;
//using Trinity.Framework;
//using Trinity.Framework.Helpers;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading;
//using Trinity.Components.QuestTools.Helpers;
//using Zeta.Bot;


//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    /// <summary>
//    /// Keeps track of multiple timers and persists their stats accross bot sessions
//    /// </summary>
//    public static class TimeTracker
//    {
//        public static List<Timing> Timings = new List<Timing>();
//        private static readonly string FilePath = Path.Combine(FileManager.LoggingPath, String.Format("TimeTracker.csv"));
//        internal static DateTime LastLoad { get; set; }
//        internal static DateTime LastSave { get; set; }
//        private static bool Initialized { get; set; }
//        private static bool _loadFailed;

//        static TimeTracker()
//        {
//            if (Initialized)
//                return;
            
//            WireUp();
//            Initialized = true;            
//        }

//        /// <summary>
//        /// Start listening to demonbuddy events.
//        /// </summary>
//        internal static void WireUp()
//        {
//            BotMain.OnStart += PersistentTiming_OnStart;
//            BotMain.OnStop += PersistentTiming_OnStop;
//            Pulsator.OnPulse += PulsatorOnOnPulse;
//            GameEvents.OnGameChanged += PersistentTiming_OnGameChanged;
//        }

//        private static DateTime _lastPulse = DateTime.MinValue;
//        private static void PulsatorOnOnPulse(object sender, EventArgs eventArgs)
//        {
//            if (DateTime.UtcNow.Subtract(_lastPulse).TotalMilliseconds < 15000 || !Timings.Any(t => t.IsRunning))
//                return;

//            _lastPulse = DateTime.UtcNow;
//            Core.Logger.Debug("----- Timings Total={0} Running={1} ChangedSinceLoad={2} -----", Timings.Count, Timings.Count(t => t.IsRunning), Timings.Count(t => t.IsDirty));
//            Timings.ForEach(t => t.DebugPrint());
//        }

//        /// <summary>
//        /// Stop listening to demonbuddy events.
//        /// </summary>
//        internal static void UnWire()
//        {
//            BotMain.OnStart -= PersistentTiming_OnStart;
//            BotMain.OnStop -= PersistentTiming_OnStop;
//            GameEvents.OnGameChanged -= PersistentTiming_OnGameChanged;
//        }

//        /// <summary>
//        /// When the game is stopped using the START button on DemonBuddy
//        /// </summary>
//        private static void PersistentTiming_OnStart(IBot bot)
//        {

//        }

//        /// <summary>
//        /// When the game is started via the STOP button on DemonBuddy
//        /// </summary>
//        private static void PersistentTiming_OnStop(IBot bot)
//        {
//            LoadUpdateAndSaveTimings();
//        }        

//        /// <summary>
//        /// Handle when the game is 'reset' - (leaving game and starting a new one)
//        /// </summary>
//        private static void PersistentTiming_OnGameChanged(object sender, EventArgs e)
//        {
//            LoadUpdateAndSaveTimings();
//        }

//        public static void LoadUpdateAndSaveTimings()
//        {
//            if (!Timings.Any())
//                return;

//            var persistedTimings = Load();

//            Core.Logger.Debug("Loaded");

//            if (!persistedTimings.Any())
//                Core.Logger.Debug("No existing timings were loaded");

//            // Mark any unfinished timers from last game as failed
//            Timings.ForEach(t =>
//            {
//                if (t.IsRunning)
//                {
//                    t.FailedCount++;
//                    t.Stop();
//                }
//            });

//            // Update the loaded timings with this sessions data.
//            Timings.ForEach(t =>
//            {
//                t.DebugPrint("Merging: ");

//                var pTiming = persistedTimings.FirstOrDefault(t1 => t1.Name == t.Name && (string.IsNullOrEmpty(t1.Group) || t1.Group == t.Group));

//                if (pTiming != null)
//                    t.Add(pTiming);
//                else
//                    persistedTimings.Add(t);
//            });

//            if(Save(persistedTimings))
//                Reset();   
         
//        }

//        /// <summary>
//        /// Clear the timings collection
//        /// </summary>
//        public static void Reset()
//        {
//            Timings = new List<Timing>();
//        }

//        /// <summary>
//        /// Start a timer
//        /// </summary>
//        public static void Start(Timing timing)
//        {
//            var existingTimer = Timings.Find(t => t.Name == timing.Name);
//            if (existingTimer == null)
//            {
//                timing.Start();
//                timing.IsDirty = true;
//                Timings.Add(timing);
//            }
//            else
//            {
//                existingTimer.Start();
//                existingTimer.IsDirty = true;
//            }
//        }

//        /// <summary>
//        /// Stop a timer
//        /// </summary>
//        public static bool StopTimer(string timerName, bool objectiveFound)
//        {
//            var found = false;
//            Timings.ForEach(t =>
//            {
//                if (t.Name != timerName || !t.IsRunning) 
//                    return;

//                found = true;
//                t.ObjectiveComplete = objectiveFound;
//                t.DebugPrint("Pre-Update");
//                t.Update();
//                t.DebugPrint("Post-Update");
//                t.PrintSimple();
//                t.Stop();
//            });
//            return found;
//        }

//        /// <summary>
//        /// Stop all timers that are part of a group
//        /// </summary>
//        public static bool StopGroup(string groupName, bool objectiveFound)
//        {
//            var found = false;
//            Timings.ForEach(t =>
//            {
//                if (t.Group != groupName || !t.IsRunning) 
//                    return;

//                found = true;
//                t.ObjectiveComplete = objectiveFound;
//                t.DebugPrint("Pre-Update");
//                t.Update();
//                t.DebugPrint("Post-Update");
//                t.PrintSimple();
//                t.Stop();
//            });
//            return found;
//        }

//        /// <summary>
//        /// Stop all timers
//        /// </summary>
//        public static bool StopAll(bool objectiveFound)
//        {
//            var found = false;

//            Timings.ForEach(t =>
//            {
//                if (!t.IsRunning) return;

//                found = true;
//                t.ObjectiveComplete = objectiveFound;
//                t.DebugPrint("Pre-Update");
//                t.Update();
//                t.DebugPrint("Post-Update");
//                t.PrintSimple();
//                t.Stop();
//            });
//            return found;
//        }

//        private const string FileFieldFormat = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}\r\n";

//        private static readonly string FileHeaderLabels = String.Format(FileFieldFormat,
//            "Name",
//            "Group",
//            "TimeAverageSeconds",
//            "MinTimeSeconds",
//            "MaxTimeSeconds",
//            "TimesTimed",
//            "TotalTimeSeconds",
//            "FailedCount",
//            "ObjectiveCount",
//            "ObjectivePercent"
//        );

//        /// <summary>
//        /// Load timing data from file
//        /// </summary>
//        private static List<Timing> Load()
//        {
//            Core.Logger.Debug(">> Loading Timings");
//            var output = new List<Timing>();
//            try
//            {
//                if (File.Exists(FilePath) && LockManager.GetLock())
//                {
//                    var lines = File.ReadAllLines(FilePath);

//                    // Check file format; .Contains avoids having to filter out BOM
//                    if (lines.First().Contains(FileHeaderLabels))
//                    {
//                        Core.Logger.Log("{0}", lines.First());
//                        Core.Logger.Log("{0}", FileHeaderLabels);
//                        Core.Logger.Warn("TimeTracker.csv data format doesn't match, all old timing data will be lost on save");
//                        return output;
//                    }

//                    foreach (var line in lines.Skip(1))
//                    {
//                        var tokens = line.Split(',');
//                        var t = new Timing
//                        {
//                            Name = tokens[0],
//                            Group = tokens[1],
//                            MinTimeSeconds = tokens[3].ChangeType<int>(),
//                            MaxTimeSeconds = tokens[4].ChangeType<int>(),
//                            TimesTimed = tokens[5].ChangeType<int>(),
//                            TotalTimeSeconds = tokens[6].ChangeType<int>(),
//                            FailedCount = tokens[7].ChangeType<int>(),
//                            ObjectiveCount = tokens[8].ChangeType<int>()
//                        };
//                        t.DebugPrint("Loaded: ");
//                        output.Add(t);
//                    }
//                    LastLoad = DateTime.UtcNow;
//                    _loadFailed = false;
//                }
//                else
//                {
//                    LockManager.ReleaseLock();
//                }
//            }
//            catch (Exception ex)
//            {
//                Core.Logger.Log("Load Exception, data will not be saved this game: {0}", ex);
//                _loadFailed = true;
//                LockManager.ReleaseLock();
//            }            
//            return output;
//        }

//        /// <summary>
//        /// Save timing data to a file
//        /// </summary>
//        public static bool Save(List<Timing> timings)
//        {
//            Core.Logger.Debug(">> Saving Timings");
//            var saved = false;

//            try
//            {
//                if (timings.Any() && LockManager.GetLock())
//                {                        
//                    if (File.Exists(FilePath))
//                    {
//                        Core.Logger.Debug("Timings File Exists at {0}, Overwriting!", FilePath);
//                        File.Delete(FilePath);
//                    }

//                    using (var w = new StreamWriter(FilePath, true))
//                    {
//                        w.Write(FileHeaderLabels);
//                        timings.ForEach(t =>
//                        {
//                            var line = String.Format(FileFieldFormat,
//                                t.Name,
//                                t.Group,
//                                t.TimeAverageSeconds,
//                                t.MinTimeSeconds,
//                                t.MaxTimeSeconds,
//                                t.TimesTimed,
//                                t.TotalTimeSeconds,
//                                t.FailedCount,
//                                t.ObjectiveCount,
//                                t.ObjectivePercent
//                                );
//                            t.DebugPrint("Saving: ");
//                            t.IsDirty = false;
//                            w.Write(line);
//                        });
//                    }
//                    saved = true;
//                    LastSave = DateTime.UtcNow;
//                }               
//            }
//            catch (Exception ex)
//            {
//                Core.Logger.Log("Exception Saving Timer File: {0}", ex);
//            }
//            finally
//            {
//                LockManager.ReleaseLock();             
//            }
//            return saved;
//        }

//        /// <summary>
//        /// Ensures multiple bots dont mess each other up or the stats when trying to access the same file.
//        /// </summary>
//        internal class LockManager
//        {
//            internal static bool HasLock { get; private set; }

//            private static readonly string FileLockPath = Path.Combine(FileManager.LoggingPath, String.Format("TimeTracker.lock"));

//            internal static bool GetLock()
//            {
//                if (HasLock && File.Exists(FileLockPath))
//                    return true;

//                if (!WaitForLock())
//                    return false;

//                File.Create(FileLockPath).Close();

//                if (!File.Exists(FileLockPath))
//                    return false;

//                HasLock = true;
//                return true;
//            }

//            internal static bool WaitForLock()
//            {
//                var startTime = DateTime.UtcNow;
//                while (File.Exists(FileLockPath))
//                {
//                    if (DateTime.UtcNow.Subtract(startTime).TotalSeconds > 10)
//                    {
//                        Core.Logger.Error("Timed out waiting for file lock on {0}", FilePath);
//                        return false;
//                    }
//                    Core.Logger.Debug("Waiting for {0} to become available", FilePath);
//                    Thread.Sleep(50);
//                }
//                return true;
//            }

//            internal static void ReleaseLock()
//            {
//                if (File.Exists(FileLockPath) && HasLock)
//                    File.Delete(FileLockPath);

//                HasLock = false;
//            }
//        }

//    }

//}
