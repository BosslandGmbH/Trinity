using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Trinity.Technicals;
using Zeta.Common.Plugins;
using Zeta.Game;

namespace Trinity
{
    public class PersistentStats
    {
        public bool IsReset;            // true between reset and next report output
        public DateTime WhenStartedSession; // when did current session start, helps identify item increments
        [XmlIgnore]
        public TimeSpan TotalRunningTime;
        public int TotalDeaths;
        public int TotalLeaveGames;
        public int TotalJoinGames;
        public int TotalProfileRecycles;
        public long TotalXp;
        public long LastXp;
        public long NextLvXp;
        public long TotalGold;
        public long LastGold;
        public int Level;
        public int ParagonLevel;
        public ItemDropStats ItemsDropped;
        public ItemDropStats ItemsPicked;
        public long TotalRunningTimeTicks
        {
            get { return TotalRunningTime.Ticks; }
            set { TotalRunningTime = new TimeSpan(value); }
        }
        public PersistentStats()
        {
            ItemsDropped = new ItemDropStats(0, new double[4], new double[74], new double[4, 74], 0, new double[74], 0, new double[5], new double[74], new double[5, 74], 0);
            ItemsPicked = new ItemDropStats(0, new double[4], new double[74], new double[4, 74], 0, new double[74], 0, new double[5], new double[74], new double[5, 74], 0);

            Reset();
        }
        public void Reset()
        {
            IsReset = true;
            TotalRunningTime = TimeSpan.Zero;
            TotalDeaths = 0;
            TotalLeaveGames = 0;
            TotalJoinGames = 0;
            TotalProfileRecycles = 0;
            TotalXp = 0;
            LastXp = 0;
            NextLvXp = 0;
            TotalGold = 0;
            LastGold = 0;
            Level = 0;
            ParagonLevel = 0;
            ItemsDropped.Total = 0;
            Array.Clear(ItemsDropped.TotalPerQuality, 0, ItemsDropped.TotalPerQuality.Length);
            Array.Clear(ItemsDropped.TotalPerLevel, 0, ItemsDropped.TotalPerLevel.Length);
            Array.Clear(ItemsDropped.TotalPerQPerL, 0, ItemsDropped.TotalPerQPerL.Length);
            ItemsDropped.TotalPotions = 0;
            Array.Clear(ItemsDropped.PotionsPerLevel, 0, ItemsDropped.PotionsPerLevel.Length);
            ItemsDropped.TotalGems = 0;
            Array.Clear(ItemsDropped.GemsPerType, 0, ItemsDropped.GemsPerType.Length);
            Array.Clear(ItemsDropped.GemsPerLevel, 0, ItemsDropped.GemsPerLevel.Length);
            Array.Clear(ItemsDropped.GemsPerTPerL, 0, ItemsDropped.GemsPerTPerL.Length);
            ItemsDropped.TotalInfernalKeys = 0;

            ItemsPicked.Total = 0;
            Array.Clear(ItemsPicked.TotalPerQuality, 0, ItemsPicked.TotalPerQuality.Length);
            Array.Clear(ItemsPicked.TotalPerLevel, 0, ItemsPicked.TotalPerLevel.Length);
            Array.Clear(ItemsPicked.TotalPerQPerL, 0, ItemsPicked.TotalPerQPerL.Length);
            ItemsPicked.TotalPotions = 0;
            Array.Clear(ItemsPicked.PotionsPerLevel, 0, ItemsPicked.PotionsPerLevel.Length);
            ItemsPicked.TotalGems = 0;
            Array.Clear(ItemsPicked.GemsPerType, 0, ItemsPicked.GemsPerType.Length);
            Array.Clear(ItemsPicked.GemsPerLevel, 0, ItemsPicked.GemsPerLevel.Length);
            Array.Clear(ItemsPicked.GemsPerTPerL, 0, ItemsPicked.GemsPerTPerL.Length);
            ItemsPicked.TotalInfernalKeys = 0;
        }

        public void AddItemsDroppedStats(ItemDropStats Last, ItemDropStats New)
        {
            ItemsDropped.Total += New.Total - Last.Total;
            ItemsDropped.TotalPotions += New.TotalPotions - Last.TotalPotions;
            ItemsDropped.TotalGems += New.TotalGems - Last.TotalGems;
            ItemsDropped.TotalInfernalKeys += New.TotalInfernalKeys - Last.TotalInfernalKeys;

            AddArray(ItemsDropped.TotalPerQuality, Last.TotalPerQuality, New.TotalPerQuality);
            AddArray(ItemsDropped.TotalPerLevel, Last.TotalPerLevel, New.TotalPerLevel);
            AddArray(ItemsDropped.TotalPerQPerL, Last.TotalPerQPerL, New.TotalPerQPerL);
            AddArray(ItemsDropped.PotionsPerLevel, Last.PotionsPerLevel, New.PotionsPerLevel);
            AddArray(ItemsDropped.GemsPerType, Last.GemsPerType, New.GemsPerType);
            AddArray(ItemsDropped.GemsPerLevel, Last.GemsPerLevel, New.GemsPerLevel);
            AddArray(ItemsDropped.GemsPerTPerL, Last.GemsPerTPerL, New.GemsPerTPerL);
        }

        public void AddItemsPickedStats(
            ItemDropStats Last, ItemDropStats New)
        {
            ItemsPicked.Total += New.Total - Last.Total;
            ItemsPicked.TotalPotions += New.TotalPotions - Last.TotalPotions;
            ItemsPicked.TotalGems += New.TotalGems - Last.TotalGems;
            ItemsPicked.TotalInfernalKeys += New.TotalInfernalKeys - Last.TotalInfernalKeys;

            AddArray(ItemsPicked.TotalPerQuality, Last.TotalPerQuality, New.TotalPerQuality);
            AddArray(ItemsPicked.TotalPerLevel, Last.TotalPerLevel, New.TotalPerLevel);
            AddArray(ItemsPicked.TotalPerQPerL, Last.TotalPerQPerL, New.TotalPerQPerL);
            AddArray(ItemsPicked.PotionsPerLevel, Last.PotionsPerLevel, New.PotionsPerLevel);
            AddArray(ItemsPicked.GemsPerType, Last.GemsPerType, New.GemsPerType);
            AddArray(ItemsPicked.GemsPerLevel, Last.GemsPerLevel, New.GemsPerLevel);
            AddArray(ItemsPicked.GemsPerTPerL, Last.GemsPerTPerL, New.GemsPerTPerL);
        }
        static public void UpdateItemsDroppedStats(
            ItemDropStats Last, ItemDropStats New)
        {
            Last.Total = New.Total;
            Last.TotalPotions = New.TotalPotions;
            Last.TotalGems = New.TotalGems;
            Last.TotalInfernalKeys = New.TotalInfernalKeys;

            CopyArray(Last.TotalPerQuality, New.TotalPerQuality);
            CopyArray(Last.TotalPerLevel, New.TotalPerLevel);
            CopyArray(Last.TotalPerQPerL, New.TotalPerQPerL);
            CopyArray(Last.PotionsPerLevel, New.PotionsPerLevel);
            CopyArray(Last.GemsPerType, New.GemsPerType);
            CopyArray(Last.GemsPerLevel, New.GemsPerLevel);
            CopyArray(Last.GemsPerTPerL, New.GemsPerTPerL);
        }

        static public void UpdateItemsPickedStats(
            ItemDropStats Last, ItemDropStats New)
        {
            Last.Total = New.Total;
            Last.TotalPotions = New.TotalPotions;
            Last.TotalGems = New.TotalGems;
            Last.TotalInfernalKeys = New.TotalInfernalKeys;

           CopyArray(Last.TotalPerQuality, New.TotalPerQuality);
           CopyArray(Last.TotalPerLevel, New.TotalPerLevel);
           CopyArray(Last.TotalPerQPerL, New.TotalPerQPerL);
           CopyArray(Last.PotionsPerLevel, New.PotionsPerLevel);
           CopyArray(Last.GemsPerType, New.GemsPerType);
           CopyArray(Last.GemsPerLevel, New.GemsPerLevel);
           CopyArray(Last.GemsPerTPerL, New.GemsPerTPerL);
        }


        internal static PersistentStats PersistentTotalStats = new PersistentStats();
        internal static PersistentStats PersistentLastSaved = new PersistentStats();
        internal static Dictionary<int, PersistentStats> WorldStatsDictionary = new Dictionary<int, PersistentStats>();
        internal static void AddArray(double[] target, double[] last, double[] src)
        {
            for (int i = 0; i < target.GetLength(0); i++)
            {
                target[i] += src[i] - last[i];
            }
        }
        internal static PersistentStats PersistentUpdateOne(String aFilename)
        {
            PersistentStats updated = new PersistentStats();
            // Load from file
            var xml = new XmlSerializer(updated.GetType());

            if (File.Exists(aFilename))
            {
                try
                {
                    using (var reader = new StreamReader(aFilename))
                    {
                        updated = xml.Deserialize(reader) as PersistentStats;
                        if (updated.IsReset)
                            updated.Reset();
                    }
                }
                catch
                {
                    File.Delete(aFilename);
                }
            }
            else
            {
                updated.Reset();
            }

            // If we are in new session (started times don't match) we clear PersistentLastSaved,
            // because we want to add all we now have.
            // However, after reset, we don't want to do that, because we are (hopefully) curious
            // only about new stuff
            if (updated.WhenStartedSession != ItemDropStats.ItemStatsWhenStartedBot && !updated.IsReset)
            {
                PersistentLastSaved.Reset();
            }

            // Add the differences
            TimeSpan TotalRunningTime = DateTime.UtcNow.Subtract(ItemDropStats.ItemStatsWhenStartedBot);

            updated.IsReset = false;
            updated.WhenStartedSession = ItemDropStats.ItemStatsWhenStartedBot;
            updated.TotalRunningTime += TotalRunningTime - PersistentLastSaved.TotalRunningTime;
            updated.TotalDeaths += Trinity.TotalDeaths - PersistentLastSaved.TotalDeaths;
            updated.TotalLeaveGames += Trinity.TotalLeaveGames - PersistentLastSaved.TotalLeaveGames;
            updated.TotalJoinGames += Trinity.TotalGamesJoined - PersistentLastSaved.TotalJoinGames;
            updated.TotalProfileRecycles += Trinity.TotalProfileRecycles - PersistentLastSaved.TotalProfileRecycles;
            updated.TotalXp += ItemDropStats.TotalXP - PersistentLastSaved.TotalXp;
            updated.LastXp += ItemDropStats.LastXP - PersistentLastSaved.LastXp;
            updated.NextLvXp += ItemDropStats.NextLevelXP - PersistentLastSaved.NextLvXp;

            updated.TotalGold = ItemDropStats.TotalGold;
            updated.LastGold = ItemDropStats.LastGold;

            updated.Level = ItemDropStats.Level;
            updated.ParagonLevel = ItemDropStats.ParagonLevel;

            // Adds difference between now and LastSaved, and set LastSaved to now
            updated.AddItemsDroppedStats(PersistentLastSaved.ItemsDropped, ItemDropStats.ItemsDroppedStats);
            updated.AddItemsPickedStats(PersistentLastSaved.ItemsPicked, ItemDropStats.ItemsPickedStats);

            // Write the PersistentTotalStats
            using (var writer = new StreamWriter(aFilename))
            {
                xml.Serialize(writer, updated);
                writer.Flush();
            }

            return updated;
        }

        internal static void PersistentUpdateStats()
        {
            int worldId = Trinity.Player.WorldID;
            if (worldId <= 0 || Trinity.Player.ActorClass == ActorClass.Invalid)
                return;

            // Total stats
            string filename = Path.Combine(FileManager.LoggingPath, String.Format("FullStats - {0}.xml", Trinity.Player.ActorClass));
            PersistentTotalStats = PersistentUpdateOne(filename);

            // World ID stats
            filename = Path.Combine(FileManager.LoggingPath, String.Format("WorldStats {1} - {0}.xml", Trinity.Player.ActorClass, worldId));
            if (!WorldStatsDictionary.ContainsKey(worldId))
                WorldStatsDictionary.Add(worldId, new PersistentStats());
            WorldStatsDictionary[worldId] = PersistentUpdateOne(filename);

            // Sets LastSaved to now for the rest of the things
            TimeSpan TotalRunningTime = DateTime.UtcNow.Subtract(ItemDropStats.ItemStatsWhenStartedBot);
            PersistentLastSaved.TotalRunningTime = TotalRunningTime;
            PersistentLastSaved.TotalDeaths = Trinity.TotalDeaths;
            PersistentLastSaved.TotalLeaveGames = Trinity.TotalLeaveGames;
            PersistentLastSaved.TotalJoinGames = Trinity.TotalGamesJoined;
            PersistentLastSaved.TotalProfileRecycles = Trinity.TotalProfileRecycles;
            PersistentLastSaved.TotalXp = ItemDropStats.TotalXP;
            PersistentLastSaved.LastXp = ItemDropStats.LastXP;
            PersistentLastSaved.NextLvXp = ItemDropStats.NextLevelXP;
            PersistentLastSaved.TotalGold = ItemDropStats.TotalGold;
            PersistentLastSaved.LastGold = ItemDropStats.LastGold;
            PersistentLastSaved.Level = ItemDropStats.Level;
            PersistentLastSaved.ParagonLevel = ItemDropStats.ParagonLevel;

            UpdateItemsDroppedStats(PersistentLastSaved.ItemsDropped, ItemDropStats.ItemsDroppedStats);
            UpdateItemsPickedStats(PersistentLastSaved.ItemsPicked, ItemDropStats.ItemsPickedStats);
        }

        /// <summary>
        /// Full Output Of Item Stats
        /// </summary>
        internal static void PersistentOutputReport()
        {
            try
            {
                PersistentUpdateStats();
            }
            catch (Exception ex)
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "PersistentOutputReport exception: {0}", ex.ToString());
            }

            // Full Stats
            try
            {
                if (Trinity.Player.ActorClass.ToString() != "Invalid")
                {
                    var fullStatsPath = Path.Combine(FileManager.LoggingPath, String.Format("FullStats - {0}.log", Trinity.Player.ActorClass));

                    using (FileStream LogStream =
                        File.Open(fullStatsPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        LogStats(LogStream, PersistentTotalStats);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "FullStats exception: {0}", ex.ToString());
            }

            // Current World Stats

            try
            {
                if (Trinity.Player.WorldID > 0 && Trinity.Player.ActorClass != ActorClass.Invalid)
                {
                    var worldStatsPath = Path.Combine(FileManager.LoggingPath, String.Format("WorldStats {1} - {0}.log", Trinity.Player.ActorClass, Trinity.Player.WorldID));

                    using (FileStream LogStream = File.Open(worldStatsPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        LogStats(LogStream, WorldStatsDictionary[Trinity.Player.WorldID]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "WorldStats exception: {0}", ex.ToString());
            }

            // AggregateWorldStats
            try
            {
                if (Trinity.Player.ActorClass != ActorClass.Invalid)
                {
                    var aggregateWorldStatsPath = Path.Combine(FileManager.LoggingPath, String.Format("AgregateWorldStats - {0}.log", Trinity.Player.ActorClass));

                    using (FileStream LogStream = File.Open(aggregateWorldStatsPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        LogWorldStats(LogStream, WorldStatsDictionary);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "AggregateWorldStats exception: {0}", ex.ToString());
            }

        }

        internal static void LogWorldStats(FileStream LogStream, Dictionary<int, PersistentStats> aWorldStats)
        {
            using (StreamWriter LogWriter = new StreamWriter(LogStream))
            {
                LogWriter.WriteLine("=== Per World agregate stats ===");
                LogWriter.WriteLine("The format is: worldid <tab> stat");
                LogWriter.WriteLine();
                LogWriter.WriteLine("=== Time spent in WorldID ===");
                foreach (var v in aWorldStats)
                    LogWriter.WriteLine(v.Key + "\t" + v.Value.TotalRunningTime.TotalHours);
                LogWriter.WriteLine();
                LogWriter.WriteLine("=== Total items + iph dropped per WorldID ===");
                foreach (var v in aWorldStats)
                    LogWriter.WriteLine(v.Key + "\t" + v.Value.ItemsDropped.Total + "\t" + Math.Round(v.Value.ItemsDropped.Total / v.Value.TotalRunningTime.TotalHours, 2).ToString("0.00"));
                LogWriter.WriteLine();
                LogWriter.WriteLine("=== Total rares + iph dropped per WorldID ===");
                foreach (var v in aWorldStats)
                    LogWriter.WriteLine(v.Key + "\t" + v.Value.ItemsDropped.TotalPerQuality[2] + "\t" + Math.Round(v.Value.ItemsDropped.TotalPerQuality[2] / v.Value.TotalRunningTime.TotalHours, 2).ToString("0.00"));
                LogWriter.WriteLine();
                LogWriter.WriteLine("=== ilvl63 rares + iph dropped per WorldID ===");
                foreach (var v in aWorldStats)
                    LogWriter.WriteLine(v.Key + "\t" + v.Value.ItemsDropped.TotalPerQPerL[2, 63] + "\t" + Math.Round(v.Value.ItemsDropped.TotalPerQPerL[2, 63] / v.Value.TotalRunningTime.TotalHours, 2).ToString("0.00"));
                LogWriter.WriteLine();
                LogWriter.WriteLine("=== Total legendaries + iph dropped per WorldID ===");
                foreach (var v in aWorldStats)
                    LogWriter.WriteLine(v.Key + "\t" + v.Value.ItemsDropped.TotalPerQuality[3] + "\t" + Math.Round(v.Value.ItemsDropped.TotalPerQuality[3] / v.Value.TotalRunningTime.TotalHours, 2).ToString("0.00"));
                LogWriter.WriteLine();
                LogWriter.WriteLine("=== Total items + iph picked per WorldID ===");
                foreach (var v in aWorldStats)
                    LogWriter.WriteLine(v.Key + "\t" + v.Value.ItemsPicked.Total + "\t" + Math.Round(v.Value.ItemsPicked.Total / v.Value.TotalRunningTime.TotalHours, 2).ToString("0.00"));

                LogWriter.Flush();
                LogStream.Flush();
            }
        }

        internal static void LogStats(FileStream LogStream, PersistentStats aPersistentStats)
        {
            var ts = aPersistentStats;
            // Create whole new file
            using (StreamWriter LogWriter = new StreamWriter(LogStream))
            {
                LogWriter.WriteLine("===== Misc Statistics =====");
                LogWriter.WriteLine("Total tracking time: " + ((int)ts.TotalRunningTime.TotalHours).ToString() + "h " + ts.TotalRunningTime.Minutes.ToString() +
                                    "m " + ts.TotalRunningTime.Seconds.ToString() + "s");
                LogWriter.WriteLine("Total deaths: " + ts.TotalDeaths.ToString() + " [" + Math.Round(ts.TotalDeaths / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour]");
                LogWriter.WriteLine("Total games (approx): " + ts.TotalLeaveGames.ToString() + " [" + Math.Round(ts.TotalLeaveGames / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour]");
                if (ts.TotalLeaveGames == 0 && ts.TotalJoinGames > 0)
                {
                    if (ts.TotalJoinGames == 1 && ts.TotalProfileRecycles > 1)
                    {
                        LogWriter.WriteLine("(a profile manager/death handler is interfering with join/leave game events, attempting to guess total runs based on profile-loops)");
                        LogWriter.WriteLine("Total full profile cycles: " + ts.TotalProfileRecycles.ToString() + " [" + Math.Round(ts.TotalProfileRecycles / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour]");
                    }
                    else
                    {
                        LogWriter.WriteLine("Total games joined: " + ts.TotalJoinGames.ToString() + " [" + Math.Round(ts.TotalJoinGames / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour]");
                    }
                }
                LogWriter.WriteLine("Total XP gained: " + Math.Round(ts.TotalXp / (float)1000000, 2).ToString("0.00") + " million [" + Math.Round(ts.TotalXp / ts.TotalRunningTime.TotalHours / 1000000, 2).ToString("0.00") + " million per hour]");
                LogWriter.WriteLine("Total Gold gained: " + Math.Round(ts.TotalGold / (float)1000, 2).ToString("0.00") + " Thousand [" + Math.Round(ts.TotalGold / ts.TotalRunningTime.TotalHours / 1000, 2).ToString("0.00") + " Thousand per hour]");
                LogWriter.WriteLine("");
                LogWriter.WriteLine("===== Item DROP Statistics =====");

                // Item stats
                if (ts.ItemsDropped.Total > 0)
                {
                    LogWriter.WriteLine("Items:");
                    LogWriter.WriteLine("Total items dropped: " + ts.ItemsDropped.Total.ToString() + " [" +
                                        Math.Round(ts.ItemsDropped.Total / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour]");

                    LogWriter.WriteLine("");
                    LogWriter.WriteLine("Items dropped by quality: ");
                    for (int iThisQuality = 0; iThisQuality <= 3; iThisQuality++)
                    {
                        if (ts.ItemsDropped.TotalPerQuality[iThisQuality] > 0)
                        {
                            LogWriter.WriteLine("- " + ItemDropStats.ItemQualityTypeStrings[iThisQuality] + ": " + ts.ItemsDropped.TotalPerQuality[iThisQuality].ToString() + " [" + Math.Round(ts.ItemsDropped.TotalPerQuality[iThisQuality] / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour] {" + Math.Round((ts.ItemsDropped.TotalPerQuality[iThisQuality] / ts.ItemsDropped.Total) * 100, 2).ToString("0.00") + " %}");
                            //intell
                            if (iThisQuality == 3)
                            {
                                for (int itemLevel = 1; itemLevel <= 63; itemLevel++)
                                    if (ts.ItemsDropped.TotalPerQPerL[iThisQuality, itemLevel] > 0)
                                        LogWriter.WriteLine("--- ilvl " + itemLevel.ToString() + " " + ItemDropStats.ItemQualityTypeStrings[iThisQuality] + ": " + ts.ItemsDropped.TotalPerQPerL[iThisQuality, itemLevel].ToString() + " [" + Math.Round(ts.ItemsDropped.TotalPerQPerL[iThisQuality, itemLevel] / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour] {" + Math.Round((ts.ItemsDropped.TotalPerQPerL[iThisQuality, itemLevel] / ts.ItemsDropped.Total) * 100, 2).ToString("0.00") + " %}");
                            }
                        }

                        // Any at all this quality?
                    }

                    // For loop on quality
                    LogWriter.WriteLine("");
                }

                // End of item stats


                // Key stats
                if (ts.ItemsDropped.TotalInfernalKeys > 0)
                {
                    LogWriter.WriteLine("Infernal Key Drops:");
                    LogWriter.WriteLine("Total Keys: " + ts.ItemsDropped.TotalInfernalKeys.ToString() + " [" + Math.Round(ts.ItemsDropped.TotalInfernalKeys / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour]");
                }

                // End of key stats
                //LogWriter.WriteLine("");
                LogWriter.WriteLine("");
                LogWriter.WriteLine("===== Item PICKUP Statistics =====");

                // Item stats
                if (ts.ItemsPicked.Total > 0)
                {
                    LogWriter.WriteLine("Items:");
                    LogWriter.WriteLine("Total items picked up: " + ts.ItemsPicked.Total.ToString() + " [" + Math.Round(ts.ItemsPicked.Total / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour]");

                    LogWriter.WriteLine("");
                    LogWriter.WriteLine("Items picked up by quality: ");
                    for (int iThisQuality = 0; iThisQuality <= 3; iThisQuality++)
                    {
                        if (ts.ItemsPicked.TotalPerQuality[iThisQuality] > 0)
                        {
                            LogWriter.WriteLine("- " + ItemDropStats.ItemQualityTypeStrings[iThisQuality] + ": " + ts.ItemsPicked.TotalPerQuality[iThisQuality].ToString() + " [" + Math.Round(ts.ItemsPicked.TotalPerQuality[iThisQuality] / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour] {" + Math.Round((ts.ItemsPicked.TotalPerQuality[iThisQuality] / ts.ItemsPicked.Total) * 100, 2).ToString("0.00") + " %}");
                            //intell
                            if (iThisQuality == 3)
                            {
                                for (int itemLevel = 1; itemLevel <= 63; itemLevel++)
                                    if (ts.ItemsPicked.TotalPerQPerL[iThisQuality, itemLevel] > 0)
                                        LogWriter.WriteLine("--- ilvl " + itemLevel.ToString() + " " + ItemDropStats.ItemQualityTypeStrings[iThisQuality] + ": " + ts.ItemsPicked.TotalPerQPerL[iThisQuality, itemLevel].ToString() + " [" + Math.Round(ts.ItemsPicked.TotalPerQPerL[iThisQuality, itemLevel] / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour] {" + Math.Round((ts.ItemsPicked.TotalPerQPerL[iThisQuality, itemLevel] / ts.ItemsPicked.Total) * 100, 2).ToString("0.00") + " %}");
                            }
                        }

                        // Any at all this quality?
                    }

                    // For loop on quality
                    LogWriter.WriteLine("");
                    if (ItemDropStats.TotalFollowerItemsIgnored > 0)
                    {
                        LogWriter.WriteLine("  (note: " + ItemDropStats.TotalFollowerItemsIgnored.ToString() + " follower items ignored for being ilvl <60 or blue)");
                    }
                }

                // End of item stats

                // Potion stats
                if (ts.ItemsPicked.TotalPotions > 0)
                {
                    LogWriter.WriteLine("Potion Pickups:");
                    LogWriter.WriteLine("Total potions: " + ts.ItemsPicked.TotalPotions.ToString() + " [" + Math.Round(ts.ItemsPicked.TotalPotions / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour]");
                    LogWriter.WriteLine("");
                }

                // End of potion stats

                // Gem stats
                if (ts.ItemsPicked.TotalGems > 0)
                {
                    LogWriter.WriteLine("Gem Pickups:");
                    LogWriter.WriteLine("Total gems: " + ts.ItemsPicked.TotalGems.ToString() + " [" + Math.Round(ts.ItemsPicked.TotalGems / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour]");
                    for (int iThisGemType = 0; iThisGemType <= 3; iThisGemType++)
                    {
                        if (ts.ItemsPicked.GemsPerType[iThisGemType] > 0)
                        {
                            LogWriter.WriteLine("- " + ItemDropStats.GemTypeStrings[iThisGemType] + ": " + ts.ItemsPicked.GemsPerType[iThisGemType].ToString() + " [" + Math.Round(ts.ItemsPicked.GemsPerType[iThisGemType] / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour] {" + Math.Round((ts.ItemsPicked.GemsPerType[iThisGemType] / ts.ItemsPicked.TotalGems) * 100, 2).ToString("0.00") + " %}");
                            for (int itemLevel = 1; itemLevel <= 63; itemLevel++)
                                if (ts.ItemsPicked.GemsPerTPerL[iThisGemType, itemLevel] > 0)
                                    LogWriter.WriteLine("--- ilvl " + itemLevel.ToString() + " " + ItemDropStats.GemTypeStrings[iThisGemType] + ": " + ts.ItemsPicked.GemsPerTPerL[iThisGemType, itemLevel].ToString() + " [" + Math.Round(ts.ItemsPicked.GemsPerTPerL[iThisGemType, itemLevel] / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour] {" + Math.Round((ts.ItemsPicked.GemsPerTPerL[iThisGemType, itemLevel] / ts.ItemsPicked.TotalGems) * 100, 2).ToString("0.00") + " %}");
                        }

                        // Any at all this quality?
                    }

                    // For loop on quality
                }

                // End of gem stats

                // Key stats
                if (ts.ItemsPicked.TotalInfernalKeys > 0)
                {
                    LogWriter.WriteLine("Infernal Key Pickups:");
                    LogWriter.WriteLine("Total Keys: " + ts.ItemsPicked.TotalInfernalKeys.ToString() + " [" + Math.Round(ts.ItemsPicked.TotalInfernalKeys / ts.TotalRunningTime.TotalHours, 2).ToString("0.00") + " per hour]");
                }

                // End of key stats
                LogWriter.WriteLine("===== End Of Report =====");
                LogWriter.Flush();
                LogStream.Flush();
            }
        }

        internal static void AddArray(double[,] target, double[,] last, double[,] src)
        {
            for (int i = 0; i < target.GetLength(0); i++)
                for (int j = 0; j < target.GetLength(1); j++)
                {
                    target[i, j] += src[i, j] - last[i, j];
                }
        }

        internal static void CopyArray(double[] last, double[] src)
        {
            for (int i = 0; i < last.GetLength(0); i++)
            {
                last[i] = src[i];
            }
        }

        internal static void CopyArray(double[,] last, double[,] src)
        {
            for (int i = 0; i < last.GetLength(0); i++)
                for (int j = 0; j < last.GetLength(1); j++)
                {
                    last[i, j] = src[i, j];
                }
        }
    
    }
}
