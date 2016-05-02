using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Actors;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Technicals;
using TrinityCoroutines;
using TrinityCoroutines.Resources;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using ActorManager = Trinity.Framework.Actors.ActorManager;

namespace Trinity.Coroutines.Town
{
    public static class SalvageItems
    {
        static SalvageItems()
        {
            GameEvents.OnWorldChanged += (sender, args) => Cache.Clear();
            BotMain.OnStop += bot => Cache.Clear();
        }

        private static readonly Dictionary<int, bool> Cache = new Dictionary<int, bool>();
        private static readonly Random Rnd = new Random();

        public static bool ShouldSalvage(CachedItem i)
        {
            if (!i.IsValid)
                return false;

            if (Cache.ContainsKey(i.AnnId))
                return Cache[i.AnnId];

            var decision = TrinityItemManager.TrinitySalvage(i) && !StashItems.ShouldStash(i);
            Cache.Add(i.AnnId, decision);
            return decision;
        }

        public async static Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Logger.LogVerbose("[SalvageItems] Need to be in town to salvage items");
                return false;
            }

            //if (Inventory.Backpack.Items.Any(Inventory.IsBadData))
            //{
            //    Logger.Log("[SalvageItems] Bad Data Detected; Clearing Actor Manager.");
            //    ZetaDia.Actors.Clear();
            //    ZetaDia.Actors.Update();
            //    await Coroutine.Sleep(500);
            //    Cache.Clear();
            //}

            var salvageItems = Inventory.Backpack.Items.Where(ShouldSalvage).ToList();
            if (!salvageItems.Any())
            {
                Logger.LogVerbose("[SalvageItems] Nothing to salvage");
                return false;
            }

            Logger.LogVerbose("[SalvageItems] Starting salvage for {0} items", salvageItems.Count);

            GameUI.CloseVendorWindow();

            var blacksmith = TownInfo.BlacksmithSalvage;
            if (blacksmith == null)
            {
                Logger.LogError("[SalvageItems] Unable to find a blacksmith info for this area :(");
                return false;
            }

            if (!UIElements.SalvageWindow.IsVisible)
            {
                var blacksmithNpc = TownInfo.Blacksmith;
                if (blacksmithNpc != null)
                {
                    TrinityPlugin.MainGridProvider.AddCellWeightingObstacle(blacksmithNpc.ActorId, 4);
                }

                if (!await MoveTo.Execute(blacksmith.InteractPosition))
                {
                    Logger.LogError($"[SalvageItems] Failed to move to blacksmith interact position ({blacksmith.Name}) to salvage items :(");
                    return false;
                };

                if (!await MoveToAndInteract.Execute(blacksmith, 10f))
                {
                    Logger.LogError($"[SalvageItems] Failed to move to blacksmith ({blacksmith.Name}) to salvage items :(");
                    return false;
                };
                await Coroutine.Sleep(Rnd.Next(750, 1250));
            }

            if (UIElements.SalvageWindow.IsVisible)
            {
                if (ZetaDia.Me.Level >= 70 && UIElements.SalvageAllWrapper.IsVisible)
                {
                    var items = Inventory.Backpack.Items.Where(i => TrinityItemManager.TrinitySalvage(i)).ToList();

                    var normalItemCount = items.Count(i => NormalQualityLevels.Contains(i.ItemQualityLevel));
                    if (normalItemCount > 0)
                    {
                        Logger.LogVerbose($"[SalvageItems] Bulk Salvaging {normalItemCount} Normal");
                        ZetaDia.Me.Inventory.SalvageItemsOfRarity(SalvageRarity.Normal);
                        //await Coroutine.Sleep(Rnd.Next(125, 350));
                    }

                    var magicItemCount = items.Count(i => MagicQualityLevels.Contains(i.ItemQualityLevel));
                    if (magicItemCount > 0)
                    {
                        Logger.LogVerbose($"[SalvageItems] Bulk Salvaging {magicItemCount} Magic");
                        ZetaDia.Me.Inventory.SalvageItemsOfRarity(SalvageRarity.Magic);
                        //await Coroutine.Sleep(Rnd.Next(125, 350));
                    }

                    var rareItemCount = items.Count(i => RareQualityLevels.Contains(i.ItemQualityLevel));
                    if (rareItemCount > 0)
                    {
                        Logger.LogVerbose($"[SalvageItems] Bulk Salvaging {rareItemCount} Rare");
                        ZetaDia.Me.Inventory.SalvageItemsOfRarity(SalvageRarity.Rare);
                        //await Coroutine.Sleep(Rnd.Next(125, 350));
                    }
                }

                await Coroutine.Sleep(500);
                await Coroutine.Yield();

                var timeout = DateTime.UtcNow.Add(TimeSpan.FromSeconds(30));
                while (DateTime.UtcNow < timeout)
                {
                    if (!UIElements.SalvageWindow.IsVisible)
                        break;

                    await Coroutine.Sleep(Rnd.Next(500, 750));
                    ActorManager.Update();
                    //await ActorManager.WaitForUpdate();

                    var freshItems = Inventory.Backpack.Items.Where(i => ShouldSalvage(i) && !Inventory.InvalidItemDynamicIds.Contains(i.AnnId)).ToList();
                    if (!freshItems.Any())
                        break;

                    var item = freshItems.First();

                    if (ZetaDia.Actors.GetACDByAnnId(item.AnnId) == null)
                    {
                        Logger.Log("AnnId doesn't exist, skipping salvage");
                        Inventory.InvalidItemDynamicIds.Add(item.AnnId);
                        continue;
                    }

                    if (!ActorManager.IsAnnIdValid(item.AnnId))
                    {
                        Logger.Log("AnnId test failed, skipping salvage to prevent disconnect");
                        Inventory.InvalidItemDynamicIds.Add(item.AnnId);
                        continue;
                    }

                    Logger.Log("Salvaging");
                    ZetaDia.Me.Inventory.SalvageItem(item.AnnId);
                    Inventory.InvalidItemDynamicIds.Add(item.AnnId);
                    ItemEvents.FireItemSalvaged(item);                    
                }

                await Coroutine.Sleep(Rnd.Next(750, 1250));
                await RepairItems.Execute();
                return true;
            }

            Logger.LogError($"[SalvageItems] Failed to salvage items");
            return false;
        }

        private static readonly HashSet<ItemQuality> RareQualityLevels = new HashSet<ItemQuality>
        {
            ItemQuality.Rare4,
            ItemQuality.Rare5,
            ItemQuality.Rare6
        };

        private static readonly HashSet<ItemQuality> MagicQualityLevels = new HashSet<ItemQuality>
        {
            ItemQuality.Magic1,
            ItemQuality.Magic2,
            ItemQuality.Magic3
        };

        private static readonly HashSet<ItemQuality> NormalQualityLevels = new HashSet<ItemQuality>
        {
            ItemQuality.Superior,
            ItemQuality.Inferior,
        };

        private static readonly HashSet<ItemQuality> LegendaryQualityLevels = new HashSet<ItemQuality>
        {
            ItemQuality.Legendary,
            ItemQuality.Special,
        };



    }

}



