using System;
using System.Collections.Generic;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework.Objects.Memory.Attributes;
using Trinity.Items;
using Zeta.Bot;
using Zeta.Common;

namespace Trinity.Framework.Helpers
{
    public class TrinityEventHandlers
    {
        public static int TotalGamesJoined = 0;
        public static int TotalLeaveGames = 0;
        public static int TotalProfileRecycles = 0;
        public static DateTime BotStartTime = DateTime.MaxValue;
        public static DateTime BotStopTime = DateTime.MaxValue;

        /// <summary>
        /// This is wired up by Plugin.OnEnabled, and called when the bot is started
        /// </summary>
        /// <param name="bot"></param>
        public static void TrinityBotStart(IBot bot)
        {
            Logger.Log("Bot Starting");
            BotStartTime = DateTime.UtcNow;
            Logger.Log("Checking System Information");
            DebugUtil.LogSystemInformation();
            DefaultLootProvider.ResetBackPackCheck();
            Logger.Log("Replacing Tree Hooks");
            HookManager.ReplaceTreeHooks();
            TreeHooks.Instance.OnHooksCleared += HookManager.InstanceOnOnHooksCleared;
            GoldInactivity.Instance.ResetCheckGold();
            XpInactivity.Instance.ResetCheckXp();
        }

        public static void GameEvents_OnGameChanged(object sender, EventArgs e)
        {
            Clear();
            ResetEverythingNewGame();
        }

        private static void Clear()
        {
            Core.Actors.Clear();
            Core.Hotbar.Clear();
            Core.Inventory.Clear();
            Core.Buffs.Clear();
            Core.Targets.Clear();
        }

        public static void GameEvents_OnWorldChanged(object sender, EventArgs e)
        {
            SnapShot.Record();
            DefaultLootProvider.ResetBackPackCheck();
        }

        public static void TrinityBotStop(IBot bot)
        {
            BotStopTime = DateTime.UtcNow;
            GoldInactivity.Instance.ResetCheckGold();
            XpInactivity.Instance.ResetCheckXp();
            Clear();
        }

        public static void TrinityOnDeath(object src, EventArgs mea)
        {
            if (DateTime.UtcNow.Subtract(DeathHandler.LastDeathTime).TotalSeconds > 10)
            {
                Clear();
                SpellHistory.History.Clear();
                DefaultLootProvider.ResetBackPackCheck();
            }

            SnapShot.Record();
        }


        public static void TrinityOnJoinGame(object src, EventArgs mea)
        {
            TotalGamesJoined++;
            ResetEverythingNewGame();            
        }

        public static void TrinityOnLeaveGame(object src, EventArgs mea)
        {
            TotalLeaveGames++;
            ResetEverythingNewGame();
        }

        public static void ResetEverythingNewGame()
        {
            Trinity.TrinityPlugin.BeginInvoke(() =>
            {
                Logger.Log("New Game - resetting everything");
                DefaultLootProvider.ResetBackPackCheck();
                SpellHistory.History.Clear();            
                GoldInactivity.Instance.ResetCheckGold();
                Clear();
                AttributeManager.Reset();
                GenericBlacklist.ClearBlacklist();
            });
        }
    }
}