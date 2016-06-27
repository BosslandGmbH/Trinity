using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using log4net.Core;
using Trinity.Framework.Modules;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Game;

namespace Trinity
{
    /// <summary>
    /// Item Stats Class and Variables - for the detailed item drop/pickup etc. stats
    /// </summary>
    public class ItemDropStats
    {
        public double Total { get; set; }
        public double[] TotalPerQuality { get; set; }
        public double[] TotalPerLevel { get; set; }
        [XmlIgnore] public double[,] TotalPerQPerL { get; set; }
        public double TotalPotions { get; set; }
        public double[] PotionsPerLevel { get; set; }
        public double TotalGems { get; set; }
        public double[] GemsPerType { get; set; }
        public double[] GemsPerLevel { get; set; }
        [XmlIgnore] public double[,] GemsPerTPerL { get; set; }
        public int TotalInfernalKeys { get; set; }
        // How many follower items were ignored, purely for item stat tracking
        internal static int TotalFollowerItemsIgnored = 0;


        // Constants and variables used by the item-stats stuff
        internal const int QUALITYWHITE = 0;
        internal const int QUALITYBLUE = 1;
        internal const int QUALITYYELLOW = 2;
        internal const int QUALITYORANGE = 3;
        internal static readonly string[] ItemQualityTypeStrings = new string[4] { "White", "Magic", "Rare", "Legendary" };
        internal const int GEMRUBY = 0;
        internal const int GEMTOPAZ = 1;
        internal const int GEMAMETHYST = 2;
        internal const int GEMEMERALD = 3;
        internal const int GEMDIAMOND = 4;
        internal static readonly string[] GemTypeStrings = new string[5] { "Ruby", "Topaz", "Amethyst", "Emerald", "Diamond" };
        internal static DateTime ItemStatsLastPostedReport = DateTime.MinValue;
        internal static DateTime ItemStatsWhenStartedBot = DateTime.MinValue;
        internal static bool MaintainStatTracking = false;

        // Store items already logged by item-stats, to make sure no stats get doubled up by accident
        internal static HashSet<string> _hashsetItemStatsLookedAt = new HashSet<string>();
        internal static HashSet<string> _hashsetItemPicksLookedAt = new HashSet<string>();
        internal static HashSet<string> _hashsetItemFollowersIgnored = new HashSet<string>();

        // These objects are instances of my stats class above, holding identical types of data for two different things - one holds item DROP stats, one holds item PICKUP stats
        internal static ItemDropStats ItemsDroppedStats = new ItemDropStats(0, new double[4], new double[74], new double[4, 74], 0, new double[74], 0, new double[5], new double[74], new double[5, 74], 0);
        internal static ItemDropStats ItemsPickedStats = new ItemDropStats(0, new double[4], new double[74], new double[4, 74], 0, new double[74], 0, new double[5], new double[74], new double[5, 74], 0);
        public static long TotalXP = 0;
        public static long LastXP = 0;
        public static long NextLevelXP = 0;
        // Gold counter
        public static long TotalGold = 0;
        public static long LastGold = 0;
        // Level and ParagonLevel
        public static int Level = 0;
        public static int ParagonLevel = 0;

        // For serialization
        public class Serializable2DimArray
        {
            public int rows;
            public int cols;
            public double[] data;

            public Serializable2DimArray() 
            { 
                rows = 0;
                cols = 0;
                data = null; 
            }

            public Serializable2DimArray(double[,] a)
            {
                rows = a.GetLength(0);
                cols = a.GetLength(1);
                data = new double[a.Length];

                int idx = 0;
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++, idx++)
                        data[idx] = a[r, c];
            }

            public double[,] To2DArray()
            {
                double[,] res = new double[rows, cols];
                int idx = 0;
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++, idx++)
                        res[r, c] = data[idx];
                return res;
            }
        }

        public Serializable2DimArray SerializableTotalPerQPerL
        {
            get { return new Serializable2DimArray(TotalPerQPerL); }
            set { TotalPerQPerL = value.To2DArray();  }
        }

        public Serializable2DimArray SerializableGemsPerTPerL
        {
            get { return new Serializable2DimArray(GemsPerTPerL); }
            set { GemsPerTPerL = value.To2DArray(); }
        }



        public ItemDropStats()
            : this(0, new double[4], new double[74], new double[4, 74], 0, new double[74], 0, new double[5], new double[74], new double[5, 74], 0)
        {
            // Creates the default values used by most
        }

        public ItemDropStats(
            double total, 
            double[] totalPerQuality,
            double[] totalPerLevel,
            double[,] totalPerQPerL,
            double totalPotions,
            double[] potionsPerLevel, 
            double totalGems,
            double[] gemsPerType, 
            double[] gemsPerLevel, 
            double[,] gemsPerTPerL, 
            int totalKeys)
        {
            Total = total;
            TotalPerQuality = totalPerQuality;
            TotalPerLevel = totalPerLevel;
            TotalPerQPerL = totalPerQPerL;
            TotalPotions = totalPotions;
            PotionsPerLevel = potionsPerLevel;
            TotalGems = totalGems;
            GemsPerType = gemsPerType;
            GemsPerLevel = gemsPerLevel;
            GemsPerTPerL = gemsPerTPerL;
            TotalInfernalKeys = totalKeys;
        }

        private static PlayerCache Player
        {
            get
            {
                return CacheData.Player;
            }
        }

        /// <summary>
        ///     Full Output Of Item Stats
        /// </summary>
        internal static void OutputReport()
        {
            using (new PerformanceLogger("OutputReport"))
            {
                if (!ZetaDia.Service.IsValid)
                    return;

                if (!ZetaDia.Service.Platform.IsConnected)
                    return;

                if (!ZetaDia.IsInGame)
                    return;

                if (ZetaDia.Me.IsFullyValid())
                    return;

                if (!TrinityPlugin.Settings.Advanced.OutputReports)
                    return;

                if (Player.WorldSnoId <= 0 || TrinityPlugin.Player.ActorClass == ActorClass.Invalid)
                    return;

                /*
                  Check is Lv 60 or not
                 * If lv 60 use Paragon
                 * If not lv 60 use normal xp/hr
                 */
                try
                {
                    Level = TrinityPlugin.Player.Level;

                    if (Player.Level < 70)
                    {
                        if (!(TotalXP == 0 && LastXP == 0 && NextLevelXP == 0))
                        {
                            if (LastXP > Player.CurrentExperience)
                            {
                                TotalXP += NextLevelXP;
                            }
                            else
                            {
                                TotalXP += ZetaDia.Me.CurrentExperience - LastXP;
                            }
                        }
                        LastXP = Player.CurrentExperience;
                        NextLevelXP = Player.ExperienceNextLevel;
                    }
                    else
                    {
                        if (!(TotalXP == 0 && LastXP == 0 && NextLevelXP == 0))
                        {
                            // We have leveled up
                            if (NextLevelXP < Player.ParagonExperienceNextLevel)
                            {
                                TotalXP += NextLevelXP + Player.ParagonCurrentExperience;
                            }
                            else // We have not leveled up
                            {
                                TotalXP += NextLevelXP - Player.ParagonExperienceNextLevel;
                            }
                        }
                        LastXP = Player.ParagonCurrentExperience;
                        NextLevelXP = Player.ParagonExperienceNextLevel;
                    }


                    PersistentStats.PersistentOutputReport();
                    TimeSpan totalRunningTime = DateTime.UtcNow.Subtract(ItemStatsWhenStartedBot);

                    string runStatsPath = Path.Combine(FileManager.LoggingPath, String.Format("RunStats - {0}.log", Player.ActorClass));

                    // Create whole new file
                    using (FileStream logStream =
                        File.Open(runStatsPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        using (var logWriter = new StreamWriter(logStream))
                        {
                            logWriter.WriteLine("===== Misc Statistics =====");
                            logWriter.WriteLine("Total tracking time: " + ((int)totalRunningTime.TotalHours) + "h " + totalRunningTime.Minutes +
                                                "m " + totalRunningTime.Seconds + "s");
                            logWriter.WriteLine("Total deaths: " + TrinityPlugin.TotalDeaths + " [" + Math.Round(TrinityPlugin.TotalDeaths / totalRunningTime.TotalHours, 2) + " per hour]");
                            logWriter.WriteLine("Total games (approx): " + TrinityPlugin.TotalLeaveGames + " [" + Math.Round(TrinityPlugin.TotalLeaveGames / totalRunningTime.TotalHours, 2) + " per hour]");
                            logWriter.WriteLine("Total Caches Opened:" + TrinityPlugin.TotalBountyCachesOpened);
                            if (TrinityPlugin.TotalLeaveGames == 0 && TrinityPlugin.TotalGamesJoined > 0)
                            {
                                if (TrinityPlugin.TotalGamesJoined == 1 && TrinityPlugin.TotalProfileRecycles > 1)
                                {
                                    logWriter.WriteLine("(a profile manager/death handler is interfering with join/leave game events, attempting to guess total runs based on profile-loops)");
                                    logWriter.WriteLine("Total full profile cycles: " + TrinityPlugin.TotalProfileRecycles + " [" + Math.Round(TrinityPlugin.TotalProfileRecycles / totalRunningTime.TotalHours, 2) + " per hour]");
                                }
                                else
                                {
                                    logWriter.WriteLine("(your games left value may be bugged @ 0 due to profile managers/routines etc., now showing games joined instead:)");
                                    logWriter.WriteLine("Total games joined: " + TrinityPlugin.TotalGamesJoined + " [" + Math.Round(TrinityPlugin.TotalGamesJoined / totalRunningTime.TotalHours, 2) + " per hour]");
                                }
                            }

                            logWriter.WriteLine("Total XP gained: " + Math.Round(TotalXP / (float)1000000, 2) + " million [" + Math.Round(TotalXP / totalRunningTime.TotalHours / 1000000, 2) + " million per hour]");
                            if (LastGold == 0)
                            {
                                LastGold = Player.Coinage;
                            }
                            if (Player.Coinage - LastGold >= 500000)
                            {
                                LastGold = Player.Coinage;
                            }
                            else
                            {
                                TotalGold += Player.Coinage - LastGold;
                                LastGold = Player.Coinage;
                            }
                            logWriter.WriteLine("Total Gold gained: " + Math.Round(TotalGold / (float)1000, 2) + " Thousand [" + Math.Round(TotalGold / totalRunningTime.TotalHours / 1000, 2) + " Thousand per hour]");
                            logWriter.WriteLine("");
                            logWriter.WriteLine("===== Item DROP Statistics =====");

                            // Item stats
                            if (ItemsDroppedStats.Total > 0)
                            {
                                logWriter.WriteLine("Items:");
                                logWriter.WriteLine("Total items dropped: " + ItemsDroppedStats.Total + " [" +
                                                    Math.Round(ItemsDroppedStats.Total / totalRunningTime.TotalHours, 2) + " per hour]");
                                logWriter.WriteLine("Items dropped by ilvl: ");
                                for (int itemLevel = 1; itemLevel <= 63; itemLevel++)
                                    if (ItemsDroppedStats.TotalPerLevel[itemLevel] > 0)
                                        logWriter.WriteLine("- ilvl" + itemLevel + ": " + ItemsDroppedStats.TotalPerLevel[itemLevel] + " [" +
                                                            Math.Round(ItemsDroppedStats.TotalPerLevel[itemLevel] / totalRunningTime.TotalHours, 2) + " per hour] {" +
                                                            Math.Round((ItemsDroppedStats.TotalPerLevel[itemLevel] / ItemsDroppedStats.Total) * 100, 2) + " %}");
                                logWriter.WriteLine("");
                                logWriter.WriteLine("Items dropped by quality: ");
                                for (int iThisQuality = 0; iThisQuality <= 3; iThisQuality++)
                                {
                                    if (ItemsDroppedStats.TotalPerQuality[iThisQuality] > 0)
                                    {
                                        logWriter.WriteLine("- " + ItemQualityTypeStrings[iThisQuality] + ": " + ItemsDroppedStats.TotalPerQuality[iThisQuality] + " [" + Math.Round(ItemsDroppedStats.TotalPerQuality[iThisQuality] / totalRunningTime.TotalHours, 2) + " per hour] {" + Math.Round((ItemsDroppedStats.TotalPerQuality[iThisQuality] / ItemsDroppedStats.Total) * 100, 2) + " %}");
                                        for (int itemLevel = 1; itemLevel <= 63; itemLevel++)
                                            if (ItemsDroppedStats.TotalPerQPerL[iThisQuality, itemLevel] > 0)
                                                logWriter.WriteLine("--- ilvl " + itemLevel + " " + ItemQualityTypeStrings[iThisQuality] + ": " + ItemsDroppedStats.TotalPerQPerL[iThisQuality, itemLevel] + " [" + Math.Round(ItemsDroppedStats.TotalPerQPerL[iThisQuality, itemLevel] / totalRunningTime.TotalHours, 2) + " per hour] {" + Math.Round((ItemsDroppedStats.TotalPerQPerL[iThisQuality, itemLevel] / ItemsDroppedStats.Total) * 100, 2) + " %}");
                                    }

                                    // Any at all this quality?
                                }

                                // For loop on quality
                                logWriter.WriteLine("");
                            }

                            // End of item stats

                            // Gem stats
                            if (ItemsDroppedStats.TotalGems > 0)
                            {
                                logWriter.WriteLine("Gem Drops:");
                                logWriter.WriteLine("Total gems: " + ItemsDroppedStats.TotalGems + " [" + Math.Round(ItemsDroppedStats.TotalGems / totalRunningTime.TotalHours, 2) + " per hour]");
                                for (int iThisGemType = 0; iThisGemType <= 3; iThisGemType++)
                                {
                                    if (ItemsDroppedStats.GemsPerType[iThisGemType] > 0)
                                    {
                                        logWriter.WriteLine("- " + GemTypeStrings[iThisGemType] + ": " + ItemsDroppedStats.GemsPerType[iThisGemType] + " [" + Math.Round(ItemsDroppedStats.GemsPerType[iThisGemType] / totalRunningTime.TotalHours, 2) + " per hour] {" + Math.Round((ItemsDroppedStats.GemsPerType[iThisGemType] / ItemsDroppedStats.TotalGems) * 100, 2) + " %}");
                                        for (int itemLevel = 1; itemLevel <= 63; itemLevel++)
                                            if (ItemsDroppedStats.GemsPerTPerL[iThisGemType, itemLevel] > 0)
                                                logWriter.WriteLine("--- ilvl " + itemLevel + " " + GemTypeStrings[iThisGemType] + ": " + ItemsDroppedStats.GemsPerTPerL[iThisGemType, itemLevel] + " [" + Math.Round(ItemsDroppedStats.GemsPerTPerL[iThisGemType, itemLevel] / totalRunningTime.TotalHours, 2) + " per hour] {" + Math.Round((ItemsDroppedStats.GemsPerTPerL[iThisGemType, itemLevel] / ItemsDroppedStats.TotalGems) * 100, 2) + " %}");
                                    }

                                    // Any at all this quality?
                                }

                                // For loop on quality
                            }

                            // End of gem stats

                            // Key stats
                            if (ItemsDroppedStats.TotalInfernalKeys > 0)
                            {
                                logWriter.WriteLine("Infernal Key Drops:");
                                logWriter.WriteLine("Total Keys: " + ItemsDroppedStats.TotalInfernalKeys + " [" + Math.Round(ItemsDroppedStats.TotalInfernalKeys / totalRunningTime.TotalHours, 2) + " per hour]");
                            }

                            // End of key stats
                            logWriter.WriteLine("");
                            logWriter.WriteLine("");
                            logWriter.WriteLine("===== Item PICKUP Statistics =====");

                            // Item stats
                            if (ItemsPickedStats.Total > 0)
                            {
                                logWriter.WriteLine("Items:");
                                logWriter.WriteLine("Total items picked up: " + ItemsPickedStats.Total + " [" + Math.Round(ItemsPickedStats.Total / totalRunningTime.TotalHours, 2) + " per hour]");
                                logWriter.WriteLine("Item picked up by ilvl: ");
                                for (int itemLevel = 1; itemLevel <= 63; itemLevel++)
                                    if (ItemsPickedStats.TotalPerLevel[itemLevel] > 0)
                                        logWriter.WriteLine("- ilvl" + itemLevel + ": " + ItemsPickedStats.TotalPerLevel[itemLevel] + " [" + Math.Round(ItemsPickedStats.TotalPerLevel[itemLevel] / totalRunningTime.TotalHours, 2) + " per hour] {" + Math.Round((ItemsPickedStats.TotalPerLevel[itemLevel] / ItemsPickedStats.Total) * 100, 2) + " %}");
                                logWriter.WriteLine("");
                                logWriter.WriteLine("Items picked up by quality: ");
                                for (int iThisQuality = 0; iThisQuality <= 3; iThisQuality++)
                                {
                                    if (ItemsPickedStats.TotalPerQuality[iThisQuality] > 0)
                                    {
                                        logWriter.WriteLine("- " + ItemQualityTypeStrings[iThisQuality] + ": " + ItemsPickedStats.TotalPerQuality[iThisQuality] + " [" + Math.Round(ItemsPickedStats.TotalPerQuality[iThisQuality] / totalRunningTime.TotalHours, 2) + " per hour] {" + Math.Round((ItemsPickedStats.TotalPerQuality[iThisQuality] / ItemsPickedStats.Total) * 100, 2) + " %}");
                                        for (int itemLevel = 1; itemLevel <= 63; itemLevel++)
                                            if (ItemsPickedStats.TotalPerQPerL[iThisQuality, itemLevel] > 0)
                                                logWriter.WriteLine("--- ilvl " + itemLevel + " " + ItemQualityTypeStrings[iThisQuality] + ": " + ItemsPickedStats.TotalPerQPerL[iThisQuality, itemLevel] + " [" + Math.Round(ItemsPickedStats.TotalPerQPerL[iThisQuality, itemLevel] / totalRunningTime.TotalHours, 2) + " per hour] {" + Math.Round((ItemsPickedStats.TotalPerQPerL[iThisQuality, itemLevel] / ItemsPickedStats.Total) * 100, 2) + " %}");
                                    }

                                    // Any at all this quality?
                                }

                                // For loop on quality
                                logWriter.WriteLine("");
                                if (TotalFollowerItemsIgnored > 0)
                                {
                                    logWriter.WriteLine("  (note: " + TotalFollowerItemsIgnored + " follower items ignored for being ilvl <60 or blue)");
                                }
                            }

                            // End of item stats
                            // Gem stats
                            if (ItemsPickedStats.TotalGems > 0)
                            {
                                logWriter.WriteLine("Gem Pickups:");
                                logWriter.WriteLine("Total gems: " + ItemsPickedStats.TotalGems + " [" + Math.Round(ItemsPickedStats.TotalGems / totalRunningTime.TotalHours, 2) + " per hour]");
                                for (int iThisGemType = 0; iThisGemType <= 3; iThisGemType++)
                                {
                                    if (ItemsPickedStats.GemsPerType[iThisGemType] > 0)
                                    {
                                        logWriter.WriteLine("- " + GemTypeStrings[iThisGemType] + ": " + ItemsPickedStats.GemsPerType[iThisGemType] + " [" + Math.Round(ItemsPickedStats.GemsPerType[iThisGemType] / totalRunningTime.TotalHours, 2) + " per hour] {" + Math.Round((ItemsPickedStats.GemsPerType[iThisGemType] / ItemsPickedStats.TotalGems) * 100, 2) + " %}");
                                        for (int itemLevel = 1; itemLevel <= 63; itemLevel++)
                                            if (ItemsPickedStats.GemsPerTPerL[iThisGemType, itemLevel] > 0)
                                                logWriter.WriteLine("--- ilvl " + itemLevel + " " + GemTypeStrings[iThisGemType] + ": " + ItemsPickedStats.GemsPerTPerL[iThisGemType, itemLevel] + " [" + Math.Round(ItemsPickedStats.GemsPerTPerL[iThisGemType, itemLevel] / totalRunningTime.TotalHours, 2) + " per hour] {" + Math.Round((ItemsPickedStats.GemsPerTPerL[iThisGemType, itemLevel] / ItemsPickedStats.TotalGems) * 100, 2) + " %}");
                                    }

                                    // Any at all this quality?
                                }

                                // For loop on quality
                            }

                            // End of gem stats

                            // Key stats
                            if (ItemsPickedStats.TotalInfernalKeys > 0)
                            {
                                logWriter.WriteLine("Infernal Key Pickups:");
                                logWriter.WriteLine("Total Keys: " + ItemsPickedStats.TotalInfernalKeys + " [" + Math.Round(ItemsPickedStats.TotalInfernalKeys / totalRunningTime.TotalHours, 2) + " per hour]");
                            }

                            // End of key stats
                            logWriter.WriteLine("===== End Of Report =====");

                            logWriter.Flush();
                            logStream.Flush();
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    // do nothing... db read error
                }
                catch (AccessViolationException)
                {
                    // do nothing... db read error. 
                }
                catch (Exception ex)
                {
                    Logger.Log(LogCategory.UserInformation, "Error generating item report! Try deleting TrinityLogs directory to fix.");
                    Logger.Log(LogCategory.UserInformation, "{0}", ex.ToString());
                }
            }
        }
    }
}
