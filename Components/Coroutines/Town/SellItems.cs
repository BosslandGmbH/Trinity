using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Trinity.Framework.Helpers;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals;


namespace Trinity.Components.Coroutines.Town
{
    public static class SellItems
    {
        static SellItems()
        {
            GameEvents.OnWorldChanged += (sender, args) => Cache.Clear();
            BotMain.OnStop += bot => Cache.Clear();
        }

        private static readonly Dictionary<int, bool> Cache = new Dictionary<int, bool>();

        public static bool ShouldSell(TrinityItem i)
        {
            if (i.IsProtected())
                return false;

            if (Core.Player.IsInventoryLockedForGreaterRift)
                return false;

            if (i.IsUnidentified)
                return false;

            return Combat.TrinityCombat.Loot.ShouldSell(i) && !Combat.TrinityCombat.Loot.ShouldSalvage(i) && !Combat.TrinityCombat.Loot.ShouldStash(i);
        }

        public async static Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Core.Logger.Verbose("[卖物品] 需要在城里卖物品");
                return false;
            }

            var sellItems = Core.Inventory.Backpack.Where(ShouldSell).ToList();
            if (!sellItems.Any())
            {
                await RepairItems.Execute();

                Core.Logger.Verbose("[卖物品] 没有东西可卖");
                return false;
            }

            Core.Logger.Verbose("[卖物品] 现在出售物品 {0} ", sellItems.Count);
            sellItems.ForEach(i => Core.Logger.Debug($"[卖物品] 卖: {i.Name} ({i.ActorSnoId}) 内部名={i.InternalName} 远古={i.IsAncient} Ann={i.AnnId}"));

            await Coroutine.Sleep(Randomizer.Fudge(150));
            GameUI.CloseVendorWindow();

            var merchant = TownInfo.NearestMerchant;
            if (merchant == null)
            {
                Core.Logger.Error("[卖物品] 无法找到此区域的小贩信息 :(");
                return false;
            }

            if (!UIElements.VendorWindow.IsVisible)
            {
                if (!await MoveToAndInteract.Execute(merchant,10f))
                {
                    Core.Logger.Error($"[卖物品] 未能移动到小贩 ({merchant.Name}) 出售 :(");
                    return false;
                }
                await Coroutine.Sleep(Randomizer.Fudge(1000));
            }

            if (UIElements.VendorWindow.IsVisible)
            {
                await Coroutine.Sleep(Randomizer.Fudge(1500));
                var freshItems = Core.Inventory.Backpack.Where(ShouldSell);
                foreach (var item in freshItems)
                {
                    if (InventoryManager.CanSellItem(item.ToAcdItem()))
                    {
                        if (!item.IsValid || item.IsUnidentified)
                        {
                            Core.Logger.Verbose($"[卖物品] 检测到无效的物品: 有效的={item.IsValid} 是未知的={item.IsUnidentified}");
                            continue;
                        }

                        await Coroutine.Sleep(Randomizer.Fudge(100));
                        Core.Logger.Verbose($"[卖物品] 卖: {item.Name} ({item.ActorSnoId}) 品质={item.ItemQualityLevel} 远古={item.IsAncient} 名称={item.InternalName}");
                        InventoryManager.SellItem(item.ToAcdItem());
                        ItemEvents.FireItemSold(item);
                        Core.Inventory.InvalidAnnIds.Add(item.AnnId);
                    }
                }

                await Coroutine.Sleep(Randomizer.Fudge(1000));
                await RepairItems.Execute();
                return true;
            }

            Core.Logger.Error($"[卖物品] 未能卖掉物品");
            return false;
        }
    }
}