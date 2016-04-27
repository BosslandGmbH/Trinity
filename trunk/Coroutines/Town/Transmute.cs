using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Actors;
using Trinity.Helpers;
using Trinity.Technicals;
using TrinityCoroutines;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Coroutines.Town
{
    public static class Transmute
    {
        public static async Task<bool> Execute(List<CachedItem> transmuteGroup)
        {
            ZetaDia.Actors.Clear();
            ZetaDia.Actors.Update();
            
            var acds = transmuteGroup.Select(i => i.GetAcdItem()).ToList();
            //var acds = transmuteGroup.Select(i => ZetaDia.Actors.GetACDByAnnId(i.AnnId) as ACDItem).ToList();
            //var acds = transmuteGroup.Select(i => i.GetAcdItem()).ToList();

            return await Execute(acds);
        }

        public static async Task<bool> Execute(List<ACDItem> transmuteGroup)
        {
            if (!ZetaDia.IsInGame)
                return false;

            if (transmuteGroup.Count > 9)
            {
                Logger.Log(" --> Can't convert more than 9 items!");
                return false;
            }

            Logger.Log("Transmuting:");

            foreach (var item in transmuteGroup)
            {
                if (item == null || !item.IsValid || item.IsDisposed)
                {
                    Logger.Log(" --> Invalid Item Found {0}");
                    return false;
                }

                var testItem = ZetaDia.Actors.GetActorsOfType<ACDItem>().FirstOrDefault(a =>
                    a.InventoryColumn == item.InventoryColumn &&
                    a.InventoryRow == item.InventoryRow &&
                    a.InventorySlot == item.InventorySlot);

                if (testItem == null || testItem.InternalName != item.InternalName || testItem.AnnId != item.AnnId)
                {
                    Logger.Log($" --> Error - item mismatch detected in Col={item.InventoryColumn} Row={item.InventoryColumn} item={item.Name}");
                    return false;
                }

                if (!item.IsCraftingReagent && item.Level < 70)
                {
                    Logger.Log(" --> Can't transmute low level items");
                    return false;
                }

                Logger.Log($@" --> {item.Name} StackQuantity={item.ItemStackQuantity} Quality={item.GetItemQuality()} CraftingMaterial={item.IsCraftingReagent} 
                                   InventorySlot={item.InventorySlot} Row={item.InventoryRow} Col={item.InventoryColumn}  (Ann={item.AnnId} AcdId={item.ACDId})");
            }

            if (!UIElements.TransmuteItemsDialog.IsVisible)
            {
                await Coroutine.Sleep(500);

                await MoveToAndInteract.Execute(TownInfo.KanaisCube);

                await Coroutine.Sleep(1000);

                if (!UIElements.TransmuteItemsDialog.IsVisible)
                {
                    Logger.Log("Cube window needs to be open before you can transmute anything.");
                    return false;
                }
            }

            Logger.Log("Zip Zap!");

            ZetaDia.Me.Inventory.TransmuteItems(transmuteGroup);
            //new TransmuteItemsMessage(transmuteGroup.Select(i => i.AnnId).ToArray()).Initialize();

            ZetaDia.Actors.Clear();
            ZetaDia.Actors.Update();

            return true;
        }
    }
}

