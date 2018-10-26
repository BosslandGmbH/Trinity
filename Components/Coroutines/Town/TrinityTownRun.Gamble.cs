using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines.Town
{
    public static partial class TrinityTownRun
    {
        private static List<GambleSlotTypes> _gambleRotation = new List<GambleSlotTypes>();
        private static readonly Random s_rnd = new Random();

        private static int GambleMinimumShards => Core.Settings.Items.GamblingMode == SettingMode.Enabled ? 0 : Core.Settings.Items.GamblingMinShards;
        private static int GambleMinimumSpendingShards => Core.Settings.Items.GamblingMode == SettingMode.Enabled ? 0 : Core.Settings.Items.GamblingMinSpendingShards;
        private static bool BelowMinimumShards => ZetaDia.Storage.PlayerDataManager.ActivePlayerData.BloodshardCount < GambleMinimumShards;
        private static bool StillSavingShards => ZetaDia.Storage.PlayerDataManager.ActivePlayerData.BloodshardCount < GambleMinimumSpendingShards;
        private static IEnumerable<GambleSlotTypes> Types => Core.Settings.Items.GamblingTypes.ToList<GambleSlotTypes>();
        private static List<GambleSlotTypes> NewGambleRotation => Core.Settings.Items.GamblingMode != SettingMode.Enabled ? Types.Where(t => Core.Settings.Items.GamblingTypes.HasFlag(t)).ToList() : Types.ToList();
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

        public static async Task<bool> EnsureKadalaWindow()
        {
            if (!await CommonCoroutines.MoveAndInteract(TownInfo.Kadala.GetActor(), () => UIElements.VendorWindow.IsVisible))
                return false;
            return UIElements.VendorWindow.IsVisible;
        }

        public static bool IsGamblePossible
        {
            get
            {
                if (!ZetaDia.IsInGame || !ZetaDia.IsInTown || ZetaDia.Storage.CurrentWorldType != Act.OpenWorld || BrainBehavior.GreaterRiftInProgress)
                    return false;

                if (Core.Settings.Items.GamblingMode == SettingMode.Disabled)
                    return false;

                if (InventoryManager.NumFreeBackpackSlots < 5)
                {
                    s_logger.Warn($"[{nameof(IsGamblePossible)}] Not enought Backpack space, can't gamble!");
                    return false;
                }

                if (Core.Settings.Items.GamblingMode == SettingMode.Selective)
                {
                    if (Core.Settings.Items.GamblingTypes == GambleSlotTypes.None)
                    {
                        s_logger.Warn($"[{nameof(IsGamblePossible)}] Select at least one thing to buy in settings");
                        return false;
                    }
                }

                if (StillSavingShards)
                {
                    s_logger.Info($"[{nameof(IsGamblePossible)}] Saving up some shards for later...");
                    return false;
                }

                if (BelowMinimumShards)
                {
                    s_logger.Info($"[{nameof(IsGamblePossible)}] Let's wait till we have more shards...");
                    return false;
                }

                if (!CanAffordMostExpensiveItem)
                {
                    s_logger.Info($"[{nameof(IsGamblePossible)}] Can't afford desired items...");
                    return false;
                }

                return true;
            }
        }

        public static async Task<bool> Gamble()
        {
            if (!IsGamblePossible) return true;

            if (!await EnsureKadalaWindow())
                return false;

            if (!await BuyItem())
                return false;

            return false;
        }

        private static async Task<bool> BuyItem()
        {
            if (!_gambleRotation.Any())
            {
                _gambleRotation = NewGambleRotation;
            }

            var slot = _gambleRotation[s_rnd.Next(_gambleRotation.Count)];
            var itemId = TownInfo.MysterySlotTypeAndId[slot];
            var item = ZetaDia.Actors.GetActorsOfType<ACDItem>(true).FirstOrDefault(a => a.ActorSnoId == itemId);

            if (item == null)
            {
                s_logger.Warn($"[{nameof(BuyItem)}] Could not find the item requested by rotation in slot {slot}. Going to buy a random item.");
                item = ZetaDia.Actors.GetActorsOfType<ACDItem>().FirstOrDefault(a => a.InternalName.StartsWith("PH_"));
            }
            if (item == null)
                return true;
            slot = TownInfo.MysterySlotTypeAndId.FirstOrDefault(o => item.ActorSnoId == o.Value).Key;
            InventoryManager.BuyItem(item.AnnId);
            s_logger.Warn($"[{nameof(BuyItem)}] Buying: '{slot}'");
            _gambleRotation.Remove(slot);
            return true;
        }
    }
}