using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Coroutines.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Items;
using Trinity.Reference;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Coroutines.Town
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
            //if (Cache.ContainsKey(i.AnnId))
            //    return Cache[i.AnnId];
            
            var decision = TrinityItemManager.TrinitySell(i) && !TrinityItemManager.TrinitySalvage(i) && !TrinityItemManager.TrinityStash(i);
            //Cache.Add(i.AnnId, decision);
            return decision;
        }

        public async static Task<bool> Execute()
        {
   
            if (!ZetaDia.IsInTown)
            {
                Logger.LogVerbose("[SellItems] Need to be in town to sell items");
                return false;
            }

            var sellItems = Inventory.Backpack.Items.Where(ShouldSell).ToList();
            if (!sellItems.Any())
            {
                await RepairItems.Execute();

                Logger.LogVerbose("[SellItems] Nothing to sell");
                return false;
            }

            Logger.LogVerbose("[SellItems] Now to sell {0} items", sellItems.Count);
            sellItems.ForEach(i => Logger.LogDebug($"[SellItems] Selling: {i.Name} ({i.ActorSnoId}) InternalName={i.InternalName} Ancient={i.IsAncient} Ann={i.AnnId}"));

            GameUI.CloseVendorWindow();

            var merchant = TownInfo.NearestMerchant;
            if (merchant == null)
            {
                Logger.LogError("[SellItems] Unable to find merchant info for this area :(");
                return false;
            }


            if (!UIElements.VendorWindow.IsVisible)
            {
                if (!await MoveToAndInteract.Execute(merchant))
                {
                    Logger.LogError($"[SellItems] Failed to move to merchant ({merchant.Name}) to sell items :(");
                    return false;
                }
                await Coroutine.Sleep(1000);
            }

            if (UIElements.VendorWindow.IsVisible)
            {
                await Coroutine.Sleep(1000);
                var freshItems = Inventory.Backpack.Items.Where(ShouldSell);
                foreach (var item in freshItems)
                {
                    if (ZetaDia.Me.Inventory.CanSellItem(item.ToAcdItem()))
                    {
                        if (!item.IsValid || item.IsUnidentified)
                        {
                            Logger.LogVerbose($"[SellItems] Invalid Items Detected: IsValid={item.IsValid} IsUnidentified={item.IsUnidentified}");
                            continue;
                        }

                        Logger.LogVerbose($"[SellItems] Selling: {item.Name} ({item.ActorSnoId}) Quality={item.ItemQualityLevel} IsAncient={item.IsAncient} Name={item.InternalName}");
                        ZetaDia.Me.Inventory.SellItem(item.ToAcdItem());
                        ItemEvents.FireItemSold(item);
                        Inventory.InvalidItemDynamicIds.Add(item.AnnId);
                    }
                }

                await Coroutine.Sleep(1000);
                await RepairItems.Execute();
                return true;
            }

            Logger.LogError($"[SellItems] Failed to sell items");
            return false;
        }

    }

}

