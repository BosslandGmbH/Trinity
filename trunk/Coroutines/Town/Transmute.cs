using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Coroutines.Town
{
    public static class Transmute
    {
        public static async Task<bool> Execute(List<TrinityItem> transmuteGroup)
        {
            if (!UIElements.TransmuteItemsDialog.IsVisible)
            {
                await MoveToAndInteract.Execute(TownInfo.KanaisCube);
                await Coroutine.Sleep(1000);
            }

            //ZetaDia.Actors.Clear();
            //ZetaDia.Actors.Update();

            var acds = transmuteGroup.Select(i => Core.Actors.GetAcdItemByAcdId(i.AcdId)).ToList();
            //var acds = transmuteGroup.Select(i => ZetaDia.Actors.GetACDByAnnId(i.AnnId) as ACDItem).ToList();
            //var acds = transmuteGroup.Select(i => i.ToAcdItem()).ToList();

            return await Execute(acds);
        }

        public static async Task<bool> Execute(List<ACDItem> transmuteGroup)
        {
            if (!ZetaDia.IsInGame)
                return false;

            if (transmuteGroup.Count > 9)
            {
                Logger.Log(" --> Can't convert with more than 9 items!");
                return false;
            }

            Logger.Log("Transmuting:");

            //var testItems = ZetaDia.Actors.GetActorsOfType<ACDItem>();

            foreach (var item in transmuteGroup)
            {
                if (item == null || !item.IsValid || item.IsDisposed)
                {
                    Logger.Log(" --> Invalid Item Found {0}");
                    return false;
                }

                //var itemCountAtLocation = testItems.Count(a =>
                //    a.InventoryColumn == item.InventoryColumn &&
                //    a.InventoryRow == item.InventoryRow &&
                //    a.InventorySlot == item.InventorySlot);

                //if (itemCountAtLocation > 1)
                //{
                //    Logger.Log($" --> Error - item mismatch detected in Col={item.InventoryColumn} Row={item.InventoryColumn} item={item.Name}");
                //    return false;
                //}

                //var isVendorWhite = item.IsVendorBought && item.ItemQualityLevel <= ItemQuality.Superior;
                if (!item.IsCraftingReagent && item.Level < 70)
                {
                    Logger.Log($" --> The internal item level for {item.Name} is {item.Level}; most items less than 70 level will cause a failed transmute");
                    return false;
                }

                Logger.Log($@" --> {item.Name} StackQuantity={item.ItemStackQuantity} Quality={item.GetItemQuality()} CraftingMaterial={item.IsCraftingReagent} 
                                   InventorySlot={item.InventorySlot} Row={item.InventoryRow} Col={item.InventoryColumn}  (Ann={item.AnnId} AcdId={item.ACDId})");
            }

            if (!UIElements.TransmuteItemsDialog.IsVisible)
            {
                Logger.Log("Cube window needs to be open before you can transmute anything.");
                return false;
            }


            //if (item == null)
            //    return false;

            Logger.Log("Zip Zap!");
            //transmuteGroup.Add(item);
            //Inventory.InvalidItemDynamicIds.Add(item.AnnId);
            ZetaDia.Me.Inventory.TransmuteItems(transmuteGroup);

            //await Coroutine.Sleep(1500);

            var first = transmuteGroup.FirstOrDefault();

            var zetaItems = ZetaDia.Actors.GetActorsOfType<ACDItem>().Where(a => a.InventorySlot == InventorySlot.BackpackItems && a.ItemQualityLevel >= ItemQuality.Rare4 && a.ItemQualityLevel <= ItemQuality.Rare6);
            var trinItems = Core.Actors._commonDataContainer.Where(a => a.InventorySlot == InventorySlot.BackpackItems && a.ActorType == ActorType.Item);

            var firstNewByAcd = Core.Actors.GetCommonDataById(first.ACDId);
            var firstNewByAnn = Core.Actors.GetAcdByAnnId(first.AnnId);
            var firstCommonByAnn = Core.Actors.GetCommonDataByAnnId(first.AnnId);
            var itemByAnn = Core.Actors.GetItemByAnnId(first.AnnId);
            var valid = Core.Actors.IsAnnIdValid(first.AnnId);

            //new TransmuteItemsMessage(transmuteGroup.Select(i => i.AnnId).ToArray()).Initialize();

            //ZetaDia.Actors.Clear();
            //ZetaDia.Actors.Update();
            return true;
        }
    }
}

