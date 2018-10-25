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
using Zeta.Bot.Logic;
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

            if (BrainBehavior.GreaterRiftInProgress)
                return false;

            if (i.IsUnidentified)
                return false;

            return Combat.TrinityCombat.Loot.ShouldSell(i) && !Combat.TrinityCombat.Loot.ShouldSalvage(i) && !Combat.TrinityCombat.Loot.ShouldStash(i);
        }

        public static async Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Core.Logger.Verbose("[SellItems] Need to be in town to sell items");
                return false;
            }

            var sellItems = Core.Inventory.Backpack.Where(ShouldSell).ToList();
            if (!sellItems.Any())
            {
                await RepairItems.Execute();

                Core.Logger.Verbose("[SellItems] Nothing to sell");
                return true;
            }

            Core.Logger.Verbose("[SellItems] Now to sell {0} items", sellItems.Count);
            sellItems.ForEach(i => Core.Logger.Debug($"[SellItems] Selling: {i.Name} ({i.ActorSnoId}) InternalName={i.InternalName} Ancient={i.IsAncient} Ann={i.AnnId}"));

            await Coroutine.Sleep(Randomizer.Fudge(150));
            GameUI.CloseVendorWindow();

            var merchant = TownInfo.NearestMerchant;
            if (merchant == null)
            {
                Core.Logger.Error("[SellItems] Unable to find merchant info for this area :(");
                return false;
            }

            if (!UIElements.VendorWindow.IsVisible)
            {
                if (!await MoveToAndInteract.Execute(merchant))
                {
                    Core.Logger.Error($"[SellItems] Failed to move to merchant ({merchant.Name}) to sell items :(");
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
                            Core.Logger.Verbose($"[SellItems] Invalid Items Detected: IsValid={item.IsValid} IsUnidentified={item.IsUnidentified}");
                            continue;
                        }

                        await Coroutine.Sleep(Randomizer.Fudge(100));
                        Core.Logger.Verbose($"[SellItems] Selling: {item.Name} ({item.ActorSnoId}) Quality={item.ItemQualityLevel} IsAncient={item.IsAncient} Name={item.InternalName}");
                        InventoryManager.SellItem(item.ToAcdItem());
                        ItemEvents.FireItemSold(item);
                        Core.Inventory.InvalidAnnIds.Add(item.AnnId);
                    }
                }

                await Coroutine.Sleep(Randomizer.Fudge(1000));
                await RepairItems.Execute();
                return true;
            }

            Core.Logger.Error($"[SellItems] Failed to sell items");
            return false;
        }
    }
}