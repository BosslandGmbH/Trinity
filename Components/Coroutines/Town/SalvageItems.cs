using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
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

        public static async Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Core.Logger.Verbose("[分解物品] 需要在城里分解物品");
                return false;
            }

            var salvageItems = Core.Inventory.Backpack.Where(ShouldSalvage).ToList();
            if (!salvageItems.Any())
            {
                Core.Logger.Verbose("[分解物品] 没有可分解物品");
                return false;
            }

            Core.Logger.Verbose("[分解物品] 开始分解 {0} 个物品.", salvageItems.Count);
            salvageItems.ForEach(i => Core.Logger.Debug($"[分解物品] 分解: {i.Name} ({i.ActorSnoId}) InternalName={i.InternalName} Ancient={i.IsAncient} Ann={i.AnnId}"));

            GameUI.CloseVendorWindow();

            var blacksmith = TownInfo.BlacksmithSalvage;
            if (blacksmith == null)
            {
                Core.Logger.Error("[分解物品] 无法找到此区域的铁匠 :(");
                return false;
            }

            if (!UIElements.SalvageWindow.IsVisible)
            {
                var blacksmithNpc = TownInfo.Blacksmith;
                if (blacksmithNpc != null)
                {
                    Core.DBGridProvider.AddCellWeightingObstacle(blacksmithNpc.ActorId, 4);
                }

                if (!await MoveTo.Execute(blacksmith.InteractPosition))
                {
                    Core.Logger.Error($"[分解物品] 未能移动到与铁匠 ({blacksmith.Name}) 位置交互分解物品 :(");
                    return false;
                };

                if (!await MoveToAndInteract.Execute(blacksmith, 10f))
                {
                    Core.Logger.Error($"[分解物品] 未能移动到与铁匠 ({blacksmith.Name}) 位置交互分解物品 :(");
                    return false;
                };
                await Coroutine.Sleep(Rnd.Next(750, 1250));
            }

            if (UIElements.SalvageWindow.IsVisible)
            {
                if (ZetaDia.Me.Level >= 70 && UIElements.SalvageAllWrapper.IsVisible)
                {
                    var items = Core.Inventory.Backpack.Where(i => Combat.TrinityCombat.Loot.ShouldSalvage(i)).ToList();

                    var normals = items.Where(i => NormalQualityLevels.Contains(i.ItemQualityLevel)).ToList();
                    if (normals.Count > 0)
                    {
                        Core.Logger.Verbose($"[分解物品] 批量分解 {normals.Count} 个一般物品");
                        if (InventoryManager.SalvageItemsOfRarity(SalvageRarity.Normal))
                        {
                            normals.ForEach(ItemEvents.FireItemSalvaged);
                        }
                    }

                    var magic = items.Where(i => MagicQualityLevels.Contains(i.ItemQualityLevel)).ToList();
                    if (magic.Count > 0)
                    {
                        Core.Logger.Verbose($"[分解物品] 批量分解 {magic.Count} 个蓝色物品");
                        if (InventoryManager.SalvageItemsOfRarity(SalvageRarity.Magic))
                        {
                            magic.ForEach(ItemEvents.FireItemSalvaged);
                        }
                    }

                    var rares = items.Where(i => RareQualityLevels.Contains(i.ItemQualityLevel)).ToList();
                    if (rares.Count > 0)
                    {
                        Core.Logger.Verbose($"[分解物品] 批量分解 {rares.Count} 个稀有物品");
                        if (InventoryManager.SalvageItemsOfRarity(SalvageRarity.Rare))
                        {
                            rares.ForEach(ItemEvents.FireItemSalvaged);
                        }
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
                    Core.Actors.Update();

                    var freshItems = Core.Inventory.Backpack.Where(i => ShouldSalvage(i) && !Core.Inventory.InvalidAnnIds.Contains(i.AnnId)).ToList();
                    if (!freshItems.Any())
                        break;

                    var item = freshItems.First();

                    if (ZetaDia.Actors.GetACDByAnnId(item.AnnId) == null)
                    {
                        Core.Logger.Log("AnnId 不存在，不分解");
                        Core.Inventory.InvalidAnnIds.Add(item.AnnId);
                        continue;
                    }

                    if (!Core.Actors.IsAnnIdValid(item.AnnId))
                    {
                        Core.Logger.Log("AnnId 测试失败，分解中止!");
                        Core.Inventory.InvalidAnnIds.Add(item.AnnId);
                        continue;
                    }

                    Core.Logger.Log($"分解: {item.Name} ({item.ActorSnoId}) Ancient={item.IsAncient}");
                    InventoryManager.SalvageItem(item.AnnId);
                    Core.Inventory.InvalidAnnIds.Add(item.AnnId);
                    ItemEvents.FireItemSalvaged(item);
                }

                await Coroutine.Sleep(Rnd.Next(750, 1250));
                await RepairItems.Execute();
                return true;
            }

            Core.Logger.Error($"[分解物品] 未能分解物品");
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