#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.DbProvider;
using Trinity.Items;
using Trinity.Technicals;
using TrinityCoroutines;
using TrinityCoroutines.Resources;
using Zeta.Bot.Logic;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

#endregion

namespace Trinity.Coroutines.Town
{
    public class Gamble
    {
        public static int TimeoutSeconds = 60;
        public static DateTime LastTimeStarted = DateTime.MinValue;
        private static DateTime _lastGambleTime = DateTime.MinValue;
        private static List<TownInfo.VendorSlot> _gambleRotation = new List<TownInfo.VendorSlot>();
        private static readonly Random Rnd = new Random();
        public static DateTime LastCanRunCheck = DateTime.MinValue;
        public static bool LastCanRunResult;

        public static void CheckShouldTownRunForGambling()
        {
            if (!ZetaDia.IsInTown)
                IsDumpingShards = false;

            if (TrinityPlugin.Settings.Gambling.ShouldTownRun && ZetaDia.PlayerData.BloodshardCount >= Math.Min(TrinityPlugin.Settings.Gambling.SaveShardsThreshold, TrinityPlugin.Player.MaxBloodShards))
            {
                if (CanRun() && !ShouldSaveShards && !TrinityTownRun.IsTryingToTownPortal() && !BrainBehavior.IsVendoring)
                {
                    BrainBehavior.ForceTownrun("Bloodshard Spending Threshold");
                }
            }
        }

        /// <summary>
        /// If bot can actually purchase something from vendor right now.
        /// </summary>
        private static bool CanBuyItems
        {
            get
            {
                try
                {                                  
                    if (!ZetaDia.IsInTown || ZetaDia.WorldType != Act.OpenWorld || TrinityPlugin.Player.IsCastingOrLoading)
                        return false;

                    var kadala = TownInfo.Kadala?.GetActor();
                    if (!UIElements.VendorWindow.IsVisible || kadala == null || kadala.Distance > 12f)
                    {
                        LogVerbose("Vendor window is not open or can't find kadala or shes not close enough");
                        return false;
                    }

                    if (!TrinityItemManager.IsAnyTwoSlotBackpackLocation)
                    {
                        LogVerbose("No bag space");
                        return false;
                    }

                    if (ZetaDia.PlayerData.BloodshardCount < TrinityPlugin.Settings.Gambling.MinimumBloodShards || !CanAffordMostExpensiveItem)
                    {
                        LogVerbose("Not enough shards!");
                        return false;
                    }

                }
                catch(Exception ex)
                {
                    Logger.LogError("Exception in Gamble.Execute, {0}", ex);

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
                while (CanRun() && (!ShouldSaveShards || IsDumpingShards))
                {
                    IsDumpingShards = true;

                    if ((TownInfo.Kadala.Distance > 8f || !UIElements.VendorWindow.IsVisible) && !await MoveToAndInteract.Execute(TownInfo.Kadala))
                    {
                        Logger.Log("[Gamble] Failed to move to Kadala, quite unfortunate.");
                        break;
                    }

                    if (CanBuyItems)
                        await BuyItem();
                    else
                        GameUI.CloseVendorWindow();

                    if (!TrinityItemManager.IsAnyTwoSlotBackpackLocation)
                    {
                        BrainBehavior.ForceTownrun();
                    }

                    await Coroutine.Sleep(100);
                    await Coroutine.Yield();
                }
            }
            catch(Exception ex)
            {
                Logger.LogError("Exception in Gamble.Execute, {0}", ex);

                if (ex is CoroutineStoppedException)
                    throw;
            }

            GameUI.CloseVendorWindow();

            return true;           
        }

        private static async Task<bool> BuyItem()
        {
            try
            {

                if (!PurchaseDelayPassed)
                    return false;

                if (!UIElements.VendorWindow.IsVisible)
                    return false;



                if (!_gambleRotation.Any())
                    _gambleRotation = TrinityPlugin.Settings.Gambling.SelectedGambleSlots;

                var slot = _gambleRotation[Rnd.Next(_gambleRotation.Count)];
                var itemId = TownInfo.MysterySlotTypeAndId[slot];
                var item = ZetaDia.Actors.GetActorsOfType<ACDItem>(true).FirstOrDefault(a => a.ActorSnoId == itemId);

                if (item == null)
                {
                    Logger.LogError("[Gamble] DB Error ACDItem == null Slot={0} Now buying random item to spend shards", slot);
                    var randomItem = ZetaDia.Actors.GetActorsOfType<ACDItem>().FirstOrDefault(a => a.InternalName.StartsWith("PH_"));
                    if (randomItem == null)
                        return true;

                    item = randomItem;
                }

                _gambleRotation.Remove(slot);
                ZetaDia.Me.Inventory.BuyItem(item.AnnId);
                Logger.Log("[Gamble] Buying: {0}", slot);
                _lastGambleTime = DateTime.UtcNow;

            }
            catch(Exception ex)
            {
                Logger.LogError("Exception in Gamble.BuyItems, {0}", ex);

                if (ex is CoroutineStoppedException)
                    throw;
            }

            return false;
        }

        public static bool CanRun(bool ignoreSaveThreshold = false)
        {
            if (!ZetaDia.IsInGame)
                return false;

            try
            {
                if (ZetaDia.WorldType != Act.OpenWorld || TrinityPlugin.Player.IsCastingOrLoading)
                {
                    return false;
                }

                if (TrinityPlugin.Player.IsInventoryLockedForGreaterRift || !TrinityPlugin.Settings.Loot.TownRun.KeepLegendaryUnid && TrinityPlugin.Player.ParticipatingInTieredLootRun)
                {
                    LogVerbose("No gambling during greater rift due to backpack items being disabled ");
                    return false;
                }

                if (TrinityPlugin.Settings.Gambling.SelectedGambleSlots.Count <= 0)
                {
                    LogVerbose("Select at least one thing to buy in settings");
                    return false;
                }

                if (BelowMinimumShards)
                {
                    LogVerbose("Not enough shards!");
                    return false;
                }

                if (!CanAffordMostExpensiveItem)
                {
                    LogVerbose("Can't afford desired items!");
                    return false;
                }

                if (!TrinityItemManager.IsAnyTwoSlotBackpackLocation || ZetaDia.Me.Inventory.NumFreeBackpackSlots < 5)
                {
                    LogVerbose("No Backpack space!");
                    return false;
                }


            }
            catch(Exception ex)
            {
                Logger.LogError("Exception in Gamble.BuyItems, {0}", ex);

                if (ex is CoroutineStoppedException)
                    throw;

                return false;
            } 

            LogVerbose("Should Gamble!");
            return true;
        }

        private static void LogVerbose(string msg, params object[] args)
        {
            var debugInfo = string.Format(" Shards={0} SaveShards={1} SaveThreshold={2} CanAffordItem={3} SelectedSlots={4}",
                ZetaDia.PlayerData.BloodshardCount,
                TrinityPlugin.Settings.Gambling.ShouldSaveShards,
                Math.Min(TrinityPlugin.Settings.Gambling.SaveShardsThreshold, TrinityPlugin.Player.MaxBloodShards),
                CanAffordMostExpensiveItem,
                TrinityPlugin.Settings.Gambling.SelectedGambleSlots.Count);

            Logger.LogVerbose("[Gamble]" + msg + debugInfo, args);
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
                var slotAndPrice = TownInfo.MysterySlotTypeAndPrice.Where(pair => TrinityPlugin.Settings.Gambling.SelectedGambleSlots.Contains(pair.Key)).ToList();
                return slotAndPrice.Any() && slotAndPrice.Max(pair => pair.Value) <= ZetaDia.PlayerData.BloodshardCount;
            }
        }

        private static bool IsDumpingShards { get; set; }

        private static bool BelowMinimumShards
        {
            get { return ZetaDia.PlayerData.BloodshardCount < TrinityPlugin.Settings.Gambling.MinimumBloodShards; }
        }

        private static bool ShouldSaveShards
        {
            get
            {
                if (TrinityPlugin.Settings.Gambling.ShouldSaveShards && ZetaDia.PlayerData.BloodshardCount < Math.Min(TrinityPlugin.Settings.Gambling.SaveShardsThreshold, TrinityPlugin.Player.MaxBloodShards))
                {
                    LogVerbose("Should Save Shards!");
                    return true;
                }
                return false;
            }
        }


    }
}