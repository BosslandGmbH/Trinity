using System;
using System.Collections.Generic;

using Trinity.Cache;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Abilities;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers;
using Trinity.Framework.Modules;
using Trinity.Framework.Objects.Memory.Attributes;
using Trinity.Helpers;
using Trinity.ItemRules;
using Trinity.Items;
using Trinity.Settings.Loot;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;
using BotManager = Trinity.BotManager;

namespace Trinity
{
    public partial class TrinityPlugin
    {
        // How many total leave games, for stat-tracking?
        public static int TotalGamesJoined = 0;
        // How many total leave games, for stat-tracking?
        public static int TotalLeaveGames = 0;
        public static int TotalProfileRecycles = 0;

        public static DateTime BotStartTime = DateTime.MaxValue;
        public static DateTime BotStopTime = DateTime.MaxValue;

        /// <summary>
        /// This is wired up by Plugin.OnEnabled, and called when the bot is started
        /// </summary>
        /// <param name="bot"></param>
        private static void TrinityBotStart(IBot bot)
        {
            Logger.Log("Bot Starting");
            BotStartTime = DateTime.UtcNow;

            // Recording of all the XML's in use this run
            try
            {
                string sThisProfile = GlobalSettings.Instance.LastProfile;
                if (sThisProfile != CurrentProfile)
                {
                    ProfileHistory.Add(sThisProfile);
                    CurrentProfile = sThisProfile;
                    if (FirstProfile == "")
                        FirstProfile = sThisProfile;
                }

                DebugUtil.LogSystemInformation();
            }
            catch
            {
            }

            if (!ItemDropStats.MaintainStatTracking)
            {
                ItemDropStats.ItemStatsWhenStartedBot = DateTime.UtcNow;
                ItemDropStats.ItemStatsLastPostedReport = DateTime.UtcNow;
                ItemDropStats.MaintainStatTracking = true;
            }
            else
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation,
                    "Note: Maintaining item stats from previous run. To reset stats fully, please restart DB.");
            }

            TrinityItemManager.ResetBackPackCheck();

            BeginInvoke(UsedProfileManager.RefreshProfileBlacklists);
            UsedProfileManager.SetProfileInWindowTitle();

            BotManager.ReplaceTreeHooks();
            TreeHooks.Instance.OnHooksCleared += BotManager.InstanceOnOnHooksCleared;


            PlayerMover.TimeLastRecordedPosition = DateTime.UtcNow;
            PlayerMover.LastRestartedGame = DateTime.UtcNow;
            Logger.Log("Bot Starting, Resetting Gold Inactivity Timer");
            GoldInactivity.Instance.ResetCheckGold();
            XpInactivity.Instance.ResetCheckXp();

            if (CharacterSettings.Instance.KillRadius < 20)
            {
                Logger.Log("WARNING: Low Kill Radius detected, currently set to: {0} (you can change this through Demonbuddy bot settings)",
                    CharacterSettings.Instance.KillRadius);
            }

            if (CharacterSettings.Instance.LootRadius < 50)
            {
               Logger.Log("WARNING: Low Gold Loot Radius detected, currently set to: {0} (you can change this through Demonbuddy bot settings)",
                    CharacterSettings.Instance.LootRadius);
            }

            if (Settings.Loot.Pickup.ItemFilterMode == ItemFilterMode.TrinityWithItemRules)
            {
                try
                {
                    if (StashRule == null)
                        StashRule = new Interpreter();
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error configuring ItemRules Interpreter: " + ex);
                }
            }

            Logger.LogDebug("TrinityPlugin BotStart took {0:0}ms", DateTime.UtcNow.Subtract(BotStartTime).TotalMilliseconds);
        }

        private static void GameEvents_OnGameChanged(object sender, EventArgs e)
        {
            Clear();

            // reload the profile juuuuuuuuuuuust in case Demonbuddy missed it... which it is known to do on disconnects
            //string currentProfilePath = ProfileManager.CurrentProfile.Path;
            //ProfileManager.Load(currentProfilePath);
            //Navigator.SearchGridProvider.Update();
            ResetEverythingNewGame();
            UsedProfileManager.SetProfileInWindowTitle();
        }

        private static void Clear()
        {
            Core.Actors.Clear();
            Core.Hotbar.Clear();
            Core.Inventory.Clear();
            Core.Buffs.Clear();
            Core.Targets.Clear();
        }

        static void GameEvents_OnWorldChanged(object sender, EventArgs e)
        {
            SnapShot.Record();
            //CacheData.WorldChangedClear();
            TrinityItemManager.ResetBackPackCheck();
            LastWorldChangeTime = DateTime.UtcNow;
        }

        // When the bot stops, output a final item-stats report so it is as up-to-date as can be
        private static void TrinityBotStop(IBot bot)
        {
            BotStopTime = DateTime.UtcNow;

            GoldInactivity.Instance.ResetCheckGold();
            XpInactivity.Instance.ResetCheckXp();
            // Issue final reports
            ItemDropStats.OutputReport();
            PlayerMover.TotalAntiStuckAttempts = 1;
            PlayerMover.vSafeMovementLocation = Vector3.Zero;
            PlayerMover.LastPosition = Vector3.Zero;
            PlayerMover.TimesReachedStuckPoint = 0;
            PlayerMover.TimeLastRecordedPosition = DateTime.MinValue;
            PlayerMover.LastGeneratedStuckPosition = DateTime.MinValue;
            DeathsThisRun = 0;
            Clear();
        }

        private static void TrinityOnDeath(object src, EventArgs mea)
        {
            if (DateTime.UtcNow.Subtract(LastDeathTime).TotalSeconds > 10)
            {
                LastDeathTime = DateTime.UtcNow;
                TotalDeaths++;
                DeathsThisRun++;
                Clear();
                PlayerMover.TotalAntiStuckAttempts = 1;
                PlayerMover.vSafeMovementLocation = Vector3.Zero;

                // Reset pre-townrun position if we die
                //TownRun.PreTownRunPosition = Vector3.Zero;
                //TownRun.PreTownRunWorldId = -1;
                //TownRun.LastCheckBackpackDurability = DateTime.MinValue;
                SpellHistory.History.Clear();
                TrinityItemManager.ResetBackPackCheck();
            }

            SnapShot.Record();
        }


        // Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
        private static void TrinityOnJoinGame(object src, EventArgs mea)
        {
            TotalGamesJoined++;
            ResetEverythingNewGame();            
        }

        // Each time we join & leave a game, might as well clear the hashset of looked-at dropped items - just to keep it smaller
        private static void TrinityOnLeaveGame(object src, EventArgs mea)
        {
            TotalLeaveGames++;
            ResetEverythingNewGame();
        }

        public static void ResetEverythingNewGame()
        {
            // Out of thread Async stuff
            BeginInvoke(() =>
            {
                Logger.Log("New Game - resetting everything");

                TrinityItemManager.ResetBackPackCheck();
                WantToTownRun = false;
                ForceVendorRunASAP = false;
                //TownRun.TownRunCheckTimer.Reset();
                Helpers.Notifications.SendEmailNotification();
                //TownRun.PreTownRunPosition = Vector3.Zero;
                //TownRun.PreTownRunWorldId = -1;
                //TownRun.WasVendoring = false;

                //CacheData.AbilityLastUsed.Clear();
                SpellHistory.History.Clear();

                DeathsThisRun = 0;
                LastDeathTime = DateTime.UtcNow;
                ItemDropStats._hashsetItemStatsLookedAt = new HashSet<string>();
                ItemDropStats._hashsetItemPicksLookedAt = new HashSet<string>();
                ItemDropStats._hashsetItemFollowersIgnored = new HashSet<string>();

                Blacklist60Seconds = new HashSet<int>();
                Blacklist90Seconds = new HashSet<int>();
                Blacklist15Seconds = new HashSet<int>();
           
                PlayerMover.TotalAntiStuckAttempts = 1;
                PlayerMover.vSafeMovementLocation = Vector3.Zero;
                PlayerMover.LastPosition = Vector3.Zero;
                PlayerMover.TimesReachedStuckPoint = 0;
                PlayerMover.TimeLastRecordedPosition = DateTime.MinValue;
                PlayerMover.LastGeneratedStuckPosition = DateTime.MinValue;
                PlayerMover.TimesReachedMaxUnstucks = 0;
                PlayerMover.CancelUnstuckerForSeconds = 0;
                PlayerMover.LastCancelledUnstucker = DateTime.MinValue;
                //NavHelper.UsedStuckSpots = new List<GridPoint3>();

                //CacheData.WorldChangedClear();

                // Reset all the caches
                ProfileHistory = new List<string>();
                CurrentProfile = "";
                FirstProfile = "";

                Logger.Log("New Game, resetting Gold Inactivity Timer");
                GoldInactivity.Instance.ResetCheckGold();

                CombatBase.IsQuestingMode = false;

                Clear();
                AttributeManager.Reset();

                //GenericCache.ClearCache();
                GenericBlacklist.ClearBlacklist();
            });
        }
    }
}