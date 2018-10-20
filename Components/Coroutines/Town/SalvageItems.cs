using System;
using Trinity.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines.Town
{
    // TODO: Make sure there is only one Salvage Routine. Might need to merge them and finetune it.
    public static class SalvageItems
    {
        static SalvageItems()
        {
            GameEvents.OnWorldChanged += (sender, args) => Cache.Clear();
            BotMain.OnStop += bot => Cache.Clear();
        }

        private static readonly Dictionary<int, bool> Cache = new Dictionary<int, bool>();
        private static readonly Random Rnd = new Random();

        public static bool ShouldSalvage(TrinityItem i)
        {
            if (!i.IsValid)
                return false;

            if (Core.Player.IsInventoryLockedForGreaterRift)
                return false;

            if (Cache.ContainsKey(i.AnnId))
                return Cache[i.AnnId];

            if (i.IsProtected())
                return false;

            if (i.IsUnidentified)
                return false;

            if (!i.IsSalvageable)
                return false;

            var decision = Combat.TrinityCombat.Loot.ShouldSalvage(i) && !StashItems.ShouldStash(i);
            Cache.Add(i.AnnId, decision);
            return decision;
        }

        public static async Task<bool> EnsureSalvageWindow()
        {
            var blacksmith = TownInfo.BlacksmithSalvage;
            if (blacksmith == null)
            {
                Core.Logger.Error("[SalvageItems] Unable to find a blacksmith info for this area :(");
                return false;
            }

            while (!UIElements.SalvageWindow.IsVisible)
            {
                var blacksmithNpc = TownInfo.Blacksmith;
                if (blacksmithNpc != null)
                {
                    Core.DBGridProvider.AddCellWeightingObstacle(blacksmithNpc.ActorId, 4);
                }

                if (!await MoveTo.Execute(blacksmith.InteractPosition))
                {
                    Core.Logger.Error($"[SalvageItems] Failed to move to blacksmith interact position ({blacksmith.Name}) to salvage items :(");
                    return false;
                };

                if (!await MoveToAndInteract.Execute(blacksmith, 10f))
                {
                    Core.Logger.Error($"[SalvageItems] Failed to move to blacksmith ({blacksmith.Name}) to salvage items :(");
                    return false;
                };
                await Coroutine.Wait(1000, () => UIElements.SalvageWindow.IsVisible);
            }
            return true;
        }

        public static async Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Core.Logger.Verbose("[SalvageItems] Need to be in town to salvage items");
                return false;
            }

            var salvageItems = Core.Inventory.Backpack.Where(ShouldSalvage).ToList();
            if (!salvageItems.Any())
            {
                Core.Logger.Verbose("[SalvageItems] Nothing to salvage");
                return false;
            }

            Core.Logger.Verbose("[SalvageItems] Starting salvage for {0} items", salvageItems.Count);
            salvageItems.ForEach(i => Core.Logger.Debug($"[SalvageItems] Salvaging: {i.Name} ({i.ActorSnoId}) InternalName={i.InternalName} Ancient={i.IsAncient} Ann={i.AnnId}"));

            GameUI.CloseVendorWindow();

            if (!await EnsureSalvageWindow())
            {
                Core.Logger.Error($"[SalvageItems] Failed to salvage items");
                return false;
            }

            if (ZetaDia.Me.Level >= 70 && UIElements.SalvageAllWrapper.IsVisible)
            {
                var items = Core.Inventory.Backpack.Where(i => Combat.TrinityCombat.Loot.ShouldSalvage(i)).ToList();

                var normals = items.Where(i => NormalQualityLevels.Contains(i.ItemQualityLevel)).ToList();
                if (normals.Count > 0)
                {
                    Core.Logger.Verbose($"[SalvageItems] Bulk Salvaging {normals.Count} Normal");
                    if (InventoryManager.SalvageItemsOfRarity(SalvageRarity.Normal))
                    {
                        normals.ForEach(ItemEvents.FireItemSalvaged);
                    }
                }

                var magic = items.Where(i => MagicQualityLevels.Contains(i.ItemQualityLevel)).ToList();
                if (magic.Count > 0)
                {
                    Core.Logger.Verbose($"[SalvageItems] Bulk Salvaging {magic.Count} Magic");
                    if (InventoryManager.SalvageItemsOfRarity(SalvageRarity.Magic))
                    {
                        magic.ForEach(ItemEvents.FireItemSalvaged);
                    }
                }

                var rares = items.Where(i => RareQualityLevels.Contains(i.ItemQualityLevel)).ToList();
                if (rares.Count > 0)
                {
                    Core.Logger.Verbose($"[SalvageItems] Bulk Salvaging {rares.Count} Rare");
                    if (InventoryManager.SalvageItemsOfRarity(SalvageRarity.Rare))
                    {
                        rares.ForEach(ItemEvents.FireItemSalvaged);
                    }
                }
            }
            else
            {
                var timeout = DateTime.UtcNow.Add(TimeSpan.FromSeconds(30));
                while (DateTime.UtcNow < timeout)
                {
                    if (!UIElements.SalvageWindow.IsVisible)
                        break;

                    await Coroutine.Yield();
                    Core.Actors.Update();

                    var freshItems = Core.Inventory.Backpack.Where(i => ShouldSalvage(i) && !Core.Inventory.InvalidAnnIds.Contains(i.AnnId)).ToList();
                    if (!freshItems.Any())
                        break;

                    var item = freshItems.First();

                    if (ZetaDia.Actors.GetACDByAnnId(item.AnnId) == null)
                    {
                        Core.Logger.Log("AnnId doesn't exist, skipping salvage");
                        Core.Inventory.InvalidAnnIds.Add(item.AnnId);
                        continue;
                    }

                    if (!Core.Actors.IsAnnIdValid(item.AnnId))
                    {
                        Core.Logger.Log("AnnId test failed, skipping salvage to prevent disconnect");
                        Core.Inventory.InvalidAnnIds.Add(item.AnnId);
                        continue;
                    }

                    Core.Logger.Log($"Salvaging: {item.Name} ({item.ActorSnoId}) Ancient={item.IsAncient}");
                    InventoryManager.SalvageItem(item.AnnId);
                    Core.Inventory.InvalidAnnIds.Add(item.AnnId);
                    ItemEvents.FireItemSalvaged(item);
                }
            }

            await Coroutine.Yield();

            await RepairItems.Execute();
            return true;
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