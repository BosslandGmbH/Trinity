#region

using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Combat;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Bot.Logic;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

#endregion

namespace Trinity.Components.Coroutines.Town
{
    public class Gamble
    {
        public static int TimeoutSeconds = 60;
        public static DateTime LastTimeStarted = DateTime.MinValue;
        private static DateTime _lastGambleTime = DateTime.MinValue;
        private static List<GambleSlotTypes> _gambleRotation = new List<GambleSlotTypes>();
        private static readonly Random Rnd = new Random();
        public static DateTime LastCanRunCheck = DateTime.MinValue;
        public static bool LastCanRunResult;

        /// <summary>
        /// If bot can actually purchase something from vendor right now.
        /// </summary>
        private static bool CanBuyItems
        {
            get
            {
                try
                {
                    if (!ZetaDia.IsInTown || ZetaDia.Storage.CurrentWorldType != Act.OpenWorld || Core.Player.IsCastingOrLoading)
                        return false;

                    if (Core.Settings.Items.GamblingMode == SettingMode.Disabled)
                        return false;

                    var kadala = TownInfo.Kadala?.GetActor();
                    if (!UIElements.VendorWindow.IsVisible || kadala == null || kadala.Distance > 12f)
                    {
                        LogVerbose("Vendor window is not open or can't find kadala or she's not close enough");
                        return false;
                    }

                    if (!DefaultLootProvider.IsAnyTwoSlotBackpackLocation)
                    {
                        LogVerbose("No bag space");
                        return false;
                    }

                    if (ZetaDia.Storage.PlayerDataManager.ActivePlayerData.BloodshardCount < GambleMinimumShards || !CanAffordMostExpensiveItem)
                    {
                        LogVerbose("Not enough shards!");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Core.Logger.Error("Exception in Gamble.Execute, {0}", ex);

                    if (ex is CoroutineStoppedException)
                        throw;

                    return false;
                }

                LogVerbose("Can buy items!");
                return true;
            }
        }

        public static async Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
                IsDumpingShards = false;

            try
            {
                while (CanRun() && (!StillSavingShards || IsDumpingShards))
                {
                    IsDumpingShards = true;
                    if ((TownInfo.Kadala.Distance > 8f || !UIElements.VendorWindow.IsVisible) && !await MoveToAndInteract.Execute(TownInfo.Kadala))
                    {
                        Core.Logger.Log("[Gamble] Failed to move to Kadala, quite unfortunate.");
                        break;
                    }

                    if (CanBuyItems)
                    {
                        await BuyItem();
                    }
                    else
                    {
                        GameUI.CloseVendorWindow();
                    }

                    if (!DefaultLootProvider.IsAnyTwoSlotBackpackLocation)
                    {
                        BrainBehavior.ForceTownrun();
                    }

                    await Coroutine.Sleep(100);
                    await Coroutine.Yield();
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Exception in Gamble.Execute, {0}", ex);

                if (ex is CoroutineStoppedException)
                    throw;
            }

            GameUI.CloseVendorWindow();

            return true;
        }

        private static int GambleMinimumShards => Core.Settings.Items.GamblingMode == SettingMode.Enabled ? 0 : Core.Settings.Items.GamblingMinShards;
        private static int GambleMinimumSpendingShards => Core.Settings.Items.GamblingMode == SettingMode.Enabled ? 0 : Core.Settings.Items.GamblingMinSpendingShards;

        private static async Task<bool> BuyItem()
        {
            try
            {
                if (!PurchaseDelayPassed)
                    return false;

                if (!UIElements.VendorWindow.IsVisible)
                    return false;

                if (!_gambleRotation.Any())
                {
                    _gambleRotation = NewGambleRotation();
                }

                var slot = _gambleRotation[Rnd.Next(_gambleRotation.Count)];
                var itemId = TownInfo.MysterySlotTypeAndId[slot];
                var item = ZetaDia.Actors.GetActorsOfType<ACDItem>(true).FirstOrDefault(a => a.ActorSnoId == itemId);

                if (item == null)
                {
                    Core.Logger.Error("[Gamble] DB Error ACDItem == null Slot={0} Now buying random item to spend shards", slot);
                    var randomItem = ZetaDia.Actors.GetActorsOfType<ACDItem>().FirstOrDefault(a => a.InternalName.StartsWith("PH_"));
                    if (randomItem == null)
                        return true;

                    item = randomItem;
                }

                _gambleRotation.Remove(slot);
                InventoryManager.BuyItem(item.AnnId);
                Core.Logger.Log("[Gamble] Buying: {0}", slot);
                _lastGambleTime = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Exception in Gamble.BuyItems, {0}", ex);

                if (ex is CoroutineStoppedException)
                    throw;
            }

            return false;
        }

        public static IEnumerable<GambleSlotTypes> Types
            => Core.Settings.Items.GamblingTypes.ToList<GambleSlotTypes>();

        public static List<GambleSlotTypes> NewGambleRotation()
            => Core.Settings.Items.GamblingMode != SettingMode.Enabled
            ? Types.Where(t => Core.Settings.Items.GamblingTypes.HasFlag(t)).ToList()
            : Types.ToList();

        public static bool CanRun(bool ignoreSaveThreshold = false)
        {
            if (!ZetaDia.IsInGame)
                return false;

            try
            {
                if (ZetaDia.Storage.CurrentWorldType != Act.OpenWorld || Core.Player.IsCastingOrLoading)
                {
                    return false;
                }

                if (Core.Player.IsInventoryLockedForGreaterRift || !Core.Settings.Items.KeepLegendaryUnid && Core.Player.ParticipatingInTieredLootRun)
                {
                    LogVerbose("No gambling during greater rift due to backpack items being disabled ");
                    return false;
                }

                if (Core.Settings.Items.GamblingMode == SettingMode.Disabled)
                    return false;

                if (Core.Settings.Items.GamblingMode == SettingMode.Selective)
                {
                    if (Core.Settings.Items.GamblingTypes == GambleSlotTypes.None)
                    {
                        LogVerbose("Select at least one thing to buy in settings");
                        return false;
                    }
                }

                if (BelowMinimumShards)
                {
                    if (IsDumpingShards)
                    {
                        IsDumpingShards = false;
                    }
                    LogVerbose("Not enough shards!");
                    return false;
                }

                if (!CanAffordMostExpensiveItem)
                {
                    if (IsDumpingShards)
                    {
                        IsDumpingShards = false;
                    }
                    LogVerbose("Can't afford desired items!");
                    return false;
                }

                if (!DefaultLootProvider.IsAnyTwoSlotBackpackLocation || InventoryManager.NumFreeBackpackSlots < 5)
                {
                    LogVerbose("No Backpack space!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Exception in Gamble.BuyItems, {0}", ex);

                if (ex is CoroutineStoppedException)
                    throw;

                return false;
            }

            LogVerbose("Should Gamble!");
            return true;
        }

        private static void LogVerbose(string msg, params object[] args)
        {
            var debugInfo = $" Shards={ZetaDia.Storage.PlayerDataManager.ActivePlayerData.BloodshardCount} GambleMode={Core.Settings.Items.GamblingMode} ShardMinimum={Core.Settings.Items.GamblingMinShards}";
            Core.Logger.Verbose("[Gamble]" + msg + debugInfo, args);
        }

        public static bool PurchaseDelayPassed
        {
            get
            {
                var timeSinceGamble = DateTime.UtcNow.Subtract(_lastGambleTime).TotalMilliseconds;
                return (_lastGambleTime == DateTime.MinValue || timeSinceGamble > Rnd.Next(50, 350));
            }
        }

        public static bool CanAffordMostExpensiveItem
        {
            get
            {
                if (Core.Settings.Items.GamblingMode == SettingMode.Enabled)
                    return TownInfo.MysterySlotTypeAndPrice.Max(pair => pair.Value) <= ZetaDia.Storage.PlayerDataManager.ActivePlayerData.BloodshardCount;

                var slotAndPrice = TownInfo.MysterySlotTypeAndPrice.Where(pair => Core.Settings.Items.GamblingTypes.HasFlag(pair.Key)).ToList();
                return slotAndPrice.Any() && slotAndPrice.Max(pair => pair.Value) <= ZetaDia.Storage.PlayerDataManager.ActivePlayerData.BloodshardCount;
            }
        }

        private static bool BelowMinimumShards => ZetaDia.Storage.PlayerDataManager.ActivePlayerData.BloodshardCount < GambleMinimumShards;
        private static bool StillSavingShards => ZetaDia.Storage.PlayerDataManager.ActivePlayerData.BloodshardCount < GambleMinimumSpendingShards;
        private static bool IsDumpingShards { get; set; }
    }
}