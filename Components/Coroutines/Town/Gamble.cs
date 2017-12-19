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
                    Core.Logger.Error("执行赌博 异常, {0}", ex);

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
                        Core.Logger.Log("[赌博] 无法移动到卡达拉, 很不幸.");
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

                    if (TrinityCombat.Loot.IsBackpackFull)
                    {
                        BrainBehavior.ForceTownrun();
                    }

                    await Coroutine.Sleep(100);
                    await Coroutine.Yield();
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error("执行赌博 异常, {0}", ex);

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
                    Core.Logger.Error("[赌博]没有选择需要赌博的道具，现在随机购买项目来花费碎片", slot);
                    var randomItem = ZetaDia.Actors.GetActorsOfType<ACDItem>().FirstOrDefault(a => a.InternalName.StartsWith("PH_"));
                    if (randomItem == null)
                        return true;

                    item = randomItem;
                }

                _gambleRotation.Remove(slot);
                InventoryManager.BuyItem(item.AnnId);
                Core.Logger.Log("[赌博] 购买中: {0}", slot);
                _lastGambleTime = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                Core.Logger.Error("赌博购买异常, {0}", ex);

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

                if (!DefaultLootProvider.IsAnyTwoSlotBackpackLocation || InventoryManager.NumFreeBackpackSlots < 5)
                {
                    LogVerbose("空间不足,无法赌博!");
                    return false;
                }
                // 智能包裹整理
                if (Core.Player.IsInventoryLockedForGreaterRift || !Core.Settings.Items.KeepLegendaryUnid && (Core.Player.ParticipatingInTieredLootRun && !DefaultLootProvider.CanVedonInRift))
                {
                    LogVerbose("处于大秘境中, 背包物品被禁用而无法赌博 ");
                    return false;
                }

                if (Core.Settings.Items.GamblingMode == SettingMode.Disabled)
                    return false;

                if (Core.Settings.Items.GamblingMode == SettingMode.Selective)
                {
                    if (Core.Settings.Items.GamblingTypes == GambleSlotTypes.None)
                    {
                        LogVerbose("在设置中选择至少一件要赌博的物品");
                        return false;
                    }
                }

                if (BelowMinimumShards)
                {
                    if (IsDumpingShards)
                    {
                        IsDumpingShards = false;
                    }
                    LogVerbose("没有足够的碎片!");
                    return false;
                }

                if (!CanAffordMostExpensiveItem)
                {
                    if (IsDumpingShards)
                    {
                        IsDumpingShards = false;
                    }
                    LogVerbose("赌不起想要的东西!");
                    return false;
                }
             
            }
            catch (Exception ex)
            {
                Core.Logger.Error("赌博购买异常, {0}", ex);

                if (ex is CoroutineStoppedException)
                    throw;

                return false;
            }

            LogVerbose("开始赌博!");
            return true;
        }

        private static void LogVerbose(string msg, params object[] args)
        {
            var debugInfo = $" 碎片={ZetaDia.Storage.PlayerDataManager.ActivePlayerData.BloodshardCount} 赌博模式={Core.Settings.Items.GamblingMode} 最小碎片={Core.Settings.Items.GamblingMinShards}";
            Core.Logger.Verbose("[赌博]" + msg + debugInfo, args);
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