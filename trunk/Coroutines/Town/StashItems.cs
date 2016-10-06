using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using Buddy.Coroutines;
using IronPython.Modules;
using Trinity.Components.Combat;
using Trinity.Coroutines.Resources;
using Trinity.Framework;
using Trinity.Framework.Objects.Enums;
using Trinity.Items;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Trinity.Framework.Actors;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Trinity.Framework.Helpers;
using Trinity.Reference;
using Extensions = Zeta.Common.Extensions;

namespace Trinity.Coroutines.Town
{
    public static class StashItems
    {
        static StashItems()
        {
            GameEvents.OnWorldChanged += (sender, args) => Cache.Clear();
            BotMain.OnStop += bot => Cache.Clear();
        }

        private static readonly Dictionary<int, bool> Cache = new Dictionary<int, bool>();

        public static bool ShouldStash(TrinityItem i)
        {
            if (i.IsUnidentified)
                return true;

            if (Cache.ContainsKey(i.AnnId))
                return Cache[i.AnnId];

            var decision = Combat.Loot.ShouldStash(i);
            Cache.Add(i.AnnId, decision);
            return decision;
        }

        public async static Task<bool> Execute(bool dontStashCraftingMaterials = false)
        {
            if (!ZetaDia.IsInTown)
            {
                Logger.LogVerbose("[StashItems] Need to be in town to stash items");
                return false;
            }

            var stashItems = Inventory.Backpack.Items.Where(ShouldStash).Where(i => AllowedToStash(dontStashCraftingMaterials, i)).ToList();
            if (!stashItems.Any())
            {
                Logger.LogVerbose($"[StashItems] Nothing to stash");
                return false;
            }

            Logger.LogVerbose($"[StashItems] Now to stash {stashItems.Count} items");
            stashItems.ForEach(i => Logger.LogDebug($"[StashItems] Stashing: {i.Name} ({i.ActorSnoId}) InternalName={i.InternalName} Ancient={i.IsAncient} Ann={i.AnnId}"));

            GameUI.CloseVendorWindow();

            if (StashPagesAvailableToPurchase && ZetaDia.PlayerData.Coinage > 100000000)
            {
                // todo: get the actual cost of buying pages
                Logger.LogError("[StashItems] Buying stash page");
                ZetaDia.Me.Inventory.BuySharedStashSlots();
            }

            await MoveToStash();

            if (!UIElements.StashWindow.IsVisible)
            {
                var stash = ZetaDia.Actors.GetActorsOfType<GizmoPlayerSharedStash>().FirstOrDefault();
                if (stash == null)
                    return false;

                if (!await MoveTo.Execute(stash.Position))
                {
                    Logger.LogError($"[SalvageItems] Failed to move to stash interact position ({stash.Name}) to stash items :(");
                    return false;
                };
                Navigator.PlayerMover.MoveTowards(stash.Position);
                if (!await MoveToAndInteract.Execute(stash, 5f))
                {
                    Logger.LogError($"[SalvageItems] Failed to move to blacksmith ({stash.Name}) to stash items :(");
                    return false;
                };
                await Coroutine.Sleep(750);
                stash.Interact();
            }

            if (UIElements.StashWindow.IsVisible)
            {
                await StackRamaladnisGift();
                await StackCraftingMaterials();

                var isStashFull = false;

                // Get items again to make sure they are valid and current this tick           
                var freshItems = Inventory.Backpack.Items.Where(ShouldStash).Where(i => AllowedToStash(dontStashCraftingMaterials, i)).ToList();
                if (!freshItems.Any())
                {
                    Logger.LogVerbose($"[StashItems] No items to stash");
                }
                else
                {
                    foreach (var item in freshItems)
                    {
                        try
                        {
                            int col;
                            int row;

                            var page = GetBestStashLocation(item, out col, out row);
                            if (page == -1)
                            {
                                Logger.LogVerbose($"[StashItems] No place to put item, stash is probably full ({item.Name} [{col},{row}] Page={page})");
                                HandleFullStash();
                                isStashFull = true;
                                continue;
                            }

                            if (page != ZetaDia.Me.Inventory.CurrentStashPage)
                            {
                                Logger.LogVerbose($"[StashItems] Changing to stash page: {page}");
                                ZetaDia.Me.Inventory.SwitchStashPage(page);
                                await Coroutine.Sleep(500);
                            }

                            Logger.LogVerbose($"[StashItems] Stashing: {item.Name} ({item.ActorSnoId}) Quality={item.ItemQualityLevel} IsAncient={item.IsAncient} InternalName={item.InternalName} StashPage={page}");
                            ZetaDia.Me.Inventory.MoveItem(item.AnnId, Core.Player.MyDynamicID, InventorySlot.SharedStash, col, row);
                            item.OnUpdated();
                            ItemEvents.FireItemStashed(item);
                            await Coroutine.Sleep(250);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"Exception Stashing Item: {ex}");
                        }
                    }
                }

                await Coroutine.Sleep(1000);
                await RepairItems.Execute();

                if (isStashFull)
                    return false;

                return true;
            }

            Logger.LogError($"[StashItems] Failed to stash items");
            return false;
        }

        public static async Task<bool> MoveToStash()
        {
            var stash = TownInfo.Stash;
            if (stash == null)
            {
                Logger.LogError("[StashItems] Unable to find a stash info for this area :(");
                return false;
            }

            if (!UIElements.StashWindow.IsVisible)
            {
                if (!await MoveToAndInteract.Execute(stash))
                {
                    Logger.LogError($"[StashItems] Failed to move to stash ({stash.Name}) to salvage items :(");
                    return false;
                }
                await Coroutine.Sleep(1000);
            }
            return true;
        }

        public static async Task<bool> StackRamaladnisGift()
        {
            var items = Inventory.Stash.Items.Where(i => i.RawItemType == RawItemType.RamaladnisGift && !i.IsTradeable).ToList();
            if (!items.Any())
                return false;

            var targetStack = items.OrderBy(i => i.ItemStackQuantity).First();
            foreach (var item in items)
            {
                if (Equals(item, targetStack))
                    continue;

                ZetaDia.Me.Inventory.MoveItem(item.AnnId, Core.Player.MyDynamicID, InventorySlot.SharedStash, targetStack.InventoryColumn, targetStack.InventoryRow);
                await Coroutine.Sleep(100);
            }

            return true;
        }

        public static int GetStashPage(TrinityItem item)
        {
            if (item.InventorySlot != InventorySlot.SharedStash) return -1;
            return (int)Math.Floor(item.InventoryRow / 10d);
        }

        public static TrinityItem GetItemAtLocation(int col, int row)
        {
            return Inventory.Stash.Items.FirstOrDefault(i => i.InventoryRow == row && i.InventoryColumn == col);
        }

        public static TrinityItem GetNextStashItem(int currentCol, int currentRow, int actorSnoId = -1)
        {
            if (actorSnoId > 0)
            {
                var nextItemOfType = Inventory.Stash.Items.FirstOrDefault(i =>
                    i.ActorSnoId == actorSnoId &&
                    i.InventoryRow > currentRow || i.InventoryRow == currentRow && i.InventoryColumn > currentCol);

                if (nextItemOfType != null)
                {
                    return nextItemOfType;
                }
            }
            return Inventory.Stash.Items.FirstOrDefault(i => !i.IsTwoSquareItem && i.InventoryRow > currentRow || i.InventoryRow == currentRow && i.InventoryColumn > currentCol);
        }

        public static async Task<bool> StackCraftingMaterials()
        {
            var items = GetInventoryMap();

            foreach (var itemGroup in Inventory.Stash.Items.Where(i => i.MaxStackCount > 0 && i.ItemStackQuantity < i.MaxStackCount && !i.IsTradeable).GroupBy(i => i.Name))
            {
                if (itemGroup.Count() <= 1)
                    continue;

                Logger.LogVerbose($">> Stacking: {itemGroup.Key}");

                foreach (var item in itemGroup.OrderBy(i => i.InventoryRow))
                {
                    int col = 0, row = 0;
                    var page = GetIdealStashPage(item);
                    if (page == -1)
                        page = GetStashPage(item);

                    if (CanStackOnPage(item, page, ref col, ref row, items))
                    {
                        Logger.LogVerbose($"[StashItems] Stashing: {item.Name} ({item.ActorSnoId}) Quality={item.ItemQualityLevel} IsAncient={item.IsAncient} InternalName={item.InternalName} StashPage={page}");
                        ZetaDia.Me.Inventory.MoveItem(item.AnnId, Core.Player.MyDynamicID, InventorySlot.SharedStash, col, row);
                        await Coroutine.Sleep(100);
                    }
                }
            }
            return true;
        }

        public static IEnumerable<TrinityItem> GetItemsOnStashPage(int page)
        {
            return Inventory.Stash.Items.Where(i => i.InventoryRow >= page * 10 && i.InventoryRow < page * 10 + 10);
        }

        //public static async Task<bool> SortStashPages()
        //{
        //    if (!Core.Settings.Loot.TownRun.SortStashPages)
        //        return false;

        //    if (Core.Settings.Loot.TownRun.StashGemsOnSecondToLastPage)
        //    {
        //        var secondLastPageIndex = TotalStashPages - 2;
        //        ZetaDia.Me.Inventory.SwitchStashPage(secondLastPageIndex);
        //        await SortStashPage(secondLastPageIndex);
        //    }

        //    var lastPageIndex = TotalStashPages - 1;
        //    ZetaDia.Me.Inventory.SwitchStashPage(lastPageIndex);
        //    await SortStashPage(lastPageIndex);
        //    return true;
        //}

        //public static async Task<bool> SortStashPage(int page)
        //{
        //    if (!await MoveToStash())
        //        return false;

        //    var targetCol = 0;
        //    var rowOnPage = 0;
        //    var map = GetInventoryMap();

        //    var itemsMovedAway = new List<TrinityItem>();
        //    var items = GetItemsOnStashPage(page).ToList();
        //    var equipment = items.Where(i => i.IsEquipment).OrderBy(i => i.RawItemType).ThenBy(i => i.ActorSnoId);
        //    var other = items.Where(i => !i.IsEquipment).OrderBy(i => i.RawItemType).ThenBy(i => i.ActorSnoId);
        //    var orderedItems = new Queue<TrinityItem>(other).AddRange(equipment);

        //    // Step through each square on the stash page.
        //    while (orderedItems.Any())
        //    {
        //        var itemToMove = orderedItems.Dequeue();
        //        var targetRow = page * 10 + rowOnPage;

        //        TrinityItem itemInTargetLocation, itemInLocationBelow, itemInLocationAbove;

        //        if (targetCol > 6 || rowOnPage > 9)
        //            break;

        //        // ignore target spots if the current item is already there.
        //        if (!map.TryGetValue(new Tuple<int, int>(targetCol, targetRow), out itemInTargetLocation) || itemInTargetLocation.AnnId != itemToMove.AnnId)
        //        {
        //            if (CanSwapOrPlaceAtLocation(targetCol, targetRow, itemToMove.IsTwoSquareItem, map))
        //            {
        //                Logger.LogVerbose($"[StashItems] Sorting: {itemToMove.Name} ({itemToMove.ActorSnoId}) StashPage={page} from [{itemToMove.InventoryColumn},{itemToMove.InventoryRow}] to [{targetCol},{targetRow}]");
        //                ZetaDia.Me.Inventory.MoveItem(itemToMove.AnnId, Core.Player.MyDynamicID, InventorySlot.SharedStash, targetCol, targetRow);
        //                await Coroutine.Sleep(25);
        //                await ActorManager.WaitForUpdate();
        //                itemInTargetLocation?.Refresh();
        //                itemToMove.Refresh();
        //                map = GetInventoryMap();
        //            }
        //            else if (itemInTargetLocation != null)
        //            {
        //                // Move an item away from the target square.
        //                var moveToLocation = GetNextEmptySquare(targetCol, targetRow, itemInTargetLocation.IsTwoSquareItem);
        //                Logger.LogVerbose($"[StashItems] >> Moving Item out of the way: {itemInTargetLocation.Name} ({itemInTargetLocation.ActorSnoId}) StashPage={Math.Floor(moveToLocation.Row / 10d)} from [{itemInTargetLocation.InventoryColumn},{itemInTargetLocation.InventoryRow}] to [{moveToLocation.Column},{moveToLocation.Row}]");
        //                ZetaDia.Me.Inventory.MoveItem(itemInTargetLocation.AnnId, Core.Player.MyDynamicID, InventorySlot.SharedStash, moveToLocation.Column, moveToLocation.Row);
        //                Logger.LogVerbose($"[StashItems] Sorting: {itemToMove.Name} ({itemToMove.ActorSnoId}) StashPage={page} from [{itemToMove.InventoryColumn},{itemToMove.InventoryRow}] to [{targetCol},{targetRow}]");
        //                ZetaDia.Me.Inventory.MoveItem(itemToMove.AnnId, Core.Player.MyDynamicID, InventorySlot.SharedStash, targetCol, targetRow);
        //                await Coroutine.Sleep(25);
        //                await ActorManager.WaitForUpdate();
        //                itemsMovedAway.Add(itemInTargetLocation);
        //                itemInTargetLocation.Refresh();
        //                itemToMove.Refresh();
        //                map = GetInventoryMap();
        //            }
        //            else if (itemToMove.IsTwoSquareItem && map.TryGetValue(new Tuple<int, int>(targetCol, targetRow + 1), out itemInLocationBelow))
        //            {
        //                // Move item in lower half of the needed 2-square space.
        //                var moveToLocation = GetNextEmptySquare(targetCol, targetRow, itemInLocationBelow.IsTwoSquareItem);
        //                Logger.LogVerbose($"[StashItems] >> Moving Item out of the way (below): {itemInLocationBelow.Name} ({itemInLocationBelow.ActorSnoId}) StashPage={Math.Floor(moveToLocation.Row / 10d)} from [{itemInLocationBelow.InventoryColumn},{itemInLocationBelow.InventoryRow}] to [{moveToLocation.Column},{moveToLocation.Row}]");
        //                ZetaDia.Me.Inventory.MoveItem(itemInLocationBelow.AnnId, Core.Player.MyDynamicID, InventorySlot.SharedStash, moveToLocation.Column, moveToLocation.Row);
        //                Logger.LogVerbose($"[StashItems] Sorting: {itemToMove.Name} ({itemToMove.ActorSnoId}) StashPage={page} from [{itemToMove.InventoryColumn},{itemToMove.InventoryRow}] to [{targetCol},{targetRow}]");
        //                ZetaDia.Me.Inventory.MoveItem(itemToMove.AnnId, Core.Player.MyDynamicID, InventorySlot.SharedStash, targetCol, targetRow);
        //                await Coroutine.Sleep(25);
        //                await ActorManager.WaitForUpdate();
        //                itemsMovedAway.Add(itemInLocationBelow);
        //                itemInLocationBelow.Refresh();
        //                itemToMove.Refresh();
        //                map = GetInventoryMap();
        //            }
        //            else
        //            {
        //                if (rowOnPage > 0 && map.TryGetValue(new Tuple<int, int>(targetCol, targetRow - 1), out itemInLocationAbove) && itemInLocationAbove.IsTwoSquareItem)
        //                {
        //                    // Target location was blocked by a two-square item above.
        //                    orderedItems.InsertItem(itemToMove);
        //                    Logger.LogVerbose($"[StashItems] {itemToMove.Name}'s Target [{targetCol},{targetRow}] is blocked by {itemInLocationAbove.Name} ({itemInLocationAbove.ActorSnoId})");
        //                }
        //                else
        //                {
        //                    Logger.Log($"[StashItems] Unknown: {itemToMove.Name} ({itemToMove.ActorSnoId}) StashPage={page} this=[{itemToMove.InventoryColumn},{itemToMove.InventoryRow}] Target=[{targetCol},{targetRow}]");
        //                }
        //            }
        //        }
        //        if (targetCol >= 6)
        //        {
        //            targetCol = 0;
        //            rowOnPage++;
        //        }
        //        else targetCol = targetCol + 1;
        //    }

        //    foreach (var item in itemsMovedAway)
        //    {
        //        if (GetStashPage(item) == page)
        //        {
        //            await ActorManager.WaitForUpdate();
        //            continue;
        //        }

        //        int col, row;
        //        if (!CanPutItemInStashPage(item, page, out col, out row))
        //        {
        //            GetBestStashLocation(item, out col, out row);
        //        }

        //        Logger.LogVerbose($"[StashItems] << Restoring: {item.Name} ({item.ActorSnoId}) StashPage={page} from [{item.InventoryColumn},{item.InventoryRow}] to [{col},{row}]");
        //        ZetaDia.Me.Inventory.MoveItem(item.AnnId, Core.Player.MyDynamicID, InventorySlot.SharedStash, col, row);
        //    }

        //    return true;
        //}

        /// <summary>
        /// Get the stash page where items should ideally be placed, ignoring if it can actually be placed there.
        /// </summary>
        public static int GetIdealStashPage(TrinityItem item)
        {
            //if (Core.Settings.Items.StashGemsOnSecondToLastPage && ItemLocationMap.ContainsKey(item.RawItemType))
            //{
            //    return ItemLocationMap[item.RawItemType];
            //}
            if (item.ItemBaseType >= ItemBaseType.Misc)
            {
                return TotalStashPages - 1;
            }
            return -1;
        }

        private static bool AllowedToStash(bool dontStashCraftingMaterials, TrinityItem i)
        {
            return !dontStashCraftingMaterials || !i.IsCraftingReagent;
        }

        static Dictionary<RawItemType, int> ItemLocationMap = new Dictionary<RawItemType, int>
        {
            { RawItemType.Gem, -1 },
            { RawItemType.UpgradeableJewel, -1 },
            //{ RawItemType.PortalDevice, -1 },
            //{ RawItemType.Potion, -1 },
        };

        public static int GetBestStashLocation(TrinityItem item, out int col, out int row)
        {
            col = 0;
            row = 0;

            //if (Core.Settings.Loot.TownRun.StashGemsOnSecondToLastPage && ItemLocationMap.ContainsKey(item.RawItemType))
            //{
            //    var stashPageOffset = ItemLocationMap[item.RawItemType];

            //    int stashpage;
            //    if (stashPageOffset < 0)
            //    {
            //        stashpage = (-1 + TotalStashPages) + stashPageOffset;
            //    }
            //    else if (stashPageOffset > TotalStashPages - 1)
            //    {
            //        stashpage = TotalStashPages - 1;
            //    }
            //    else
            //    {
            //        stashpage = stashPageOffset;
            //    }

            //    if (CanPutItemInStashPage(item, stashpage, out col, out row))
            //    {
            //        return stashpage;
            //    }
            //}
            if (item.ItemBaseType >= ItemBaseType.Misc)
            {
                for (int i = TotalStashPages - 1; i >= 0; i--)
                {
                    if (CanPutItemInStashPage(item, i, out col, out row))
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (int j = 0; j < TotalStashPages - 1; j++)
                {
                    if (CanPutItemInStashPage(item, j, out col, out row))
                    {
                        return j;
                    }
                }
            }
            return -1;
        }

        private static HashSet<RawItemType> _specialCaseNonStackableItems = new HashSet<RawItemType>
        {
            RawItemType.CraftingPlan,
            RawItemType.CraftingPlanJeweler,
            RawItemType.CraftingPlanLegendarySmith,
            RawItemType.CraftingPlanMystic,
            RawItemType.CraftingPlanMysticTransmog,
            RawItemType.CraftingPlanSmith,
        };

        public static bool CanPutItemInStashPage(TrinityItem item, int stashPageNumber, out int col, out int row)
        {
            var itemsOnStashPage = GetInventoryMap();

            col = 0;
            row = 0;

            if (stashPageNumber == 0 && !UIElements.StashDialogMainPageTab1.IsEnabled)
            {
                return false;
            }
            if (stashPageNumber == 1 && !UIElements.StashDialogMainPageTab2.IsEnabled)
            {
                return false;
            }
            if (stashPageNumber == 2 && !UIElements.StashDialogMainPageTab3.IsEnabled)
            {
                return false;
            }
            if (stashPageNumber == 3 && !UIElements.StashDialogMainPageTab4.IsEnabled)
            {
                return false;
            }
            if (stashPageNumber == 4 && !UIElements.StashDialogMainPageTab5.IsEnabled)
            {
                return false;
            }
            if (CharacterSettings.Instance.ProtectedStashPages.Contains(stashPageNumber))
            {
                return false;
            }
            if (CanStackOnPage(item, stashPageNumber, ref col, ref row, itemsOnStashPage))
            {
                return true;
            }
            if (CanPlaceOnPage(item, stashPageNumber, ref col, ref row, itemsOnStashPage))
            {
                return true;
            }
            return false;
        }

        private static bool CanPlaceOnPage(TrinityItem item, int stashPageNumber, ref int col, ref int row, InventoryMap itemsOnStashPage)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int c = 0; c < 7; c++)
                {
                    var r = stashPageNumber * 10 + i;

                    if (TryGetStashingLocation(item, stashPageNumber, c, r, itemsOnStashPage, ref col, ref row))
                        return true;
                }
            }
            return false;
        }

        private static bool CanStackOnPage(TrinityItem item, int stashPageNumber, ref int col, ref int row, InventoryMap itemsOnStashPage)
        {

            if (item.IsUnidentified)
                return false;

            if (item.MaxStackCount > 0 && !item.IsTradeable && !_specialCaseNonStackableItems.Contains(item.RawItemType))
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int c = 0; c < 7; c++)
                    {
                        var r = stashPageNumber * 10 + i;

                        if (TryGetStackLocation(item, stashPageNumber, c, r, itemsOnStashPage, ref col, ref row))
                            return true;
                    }
                }
            }
            return false;
        }

        private static InventoryMap GetInventoryMap()
        {
            var items = Inventory.Stash.Items.ToList();
            //items.ForEach(i => i.Refresh());
            return new InventoryMap(items.ToDictionary(k => new Tuple<int, int>(k.InventoryColumn, k.InventoryRow), v => v));
        }

        //public static InventorySquare GetNextEmptySquare(int col, int row, bool twoSquare)
        //{
        //    for (int r = row; r < TotalStashPages - 1 * 10; r++)
        //    {
        //        for (int c = col; c < 7; c++)
        //        {
        //            if (CanPlaceAtLocation(c, r, twoSquare))
        //            {
        //                return new InventorySquare((byte)c, (byte)r);
        //            }
        //        }
        //    }
        //    for (int r = 0; r <= row; r++)
        //    {
        //        for (int c = 0; c < 7; c++)
        //        {
        //            if (CanPlaceAtLocation(c, r, twoSquare))
        //            {
        //                return new InventorySquare((byte)c, (byte)r);
        //            }
        //        }
        //    }
        //    return new InventorySquare(0, 0);
        //}

        private static bool TryGetStackLocation(TrinityItem item, int stashPageNumber, int col, int row, InventoryMap map, ref int placeAtCol, ref int placeAtRow)
        {
            var loc = new Tuple<int, int>(col, row);

            if (col == placeAtCol && row == placeAtRow)
                return false;

            var isSquareEmpty = !map.ContainsKey(loc);
            if (isSquareEmpty)
                return false;

            if (item.IsTwoSquareItem)
                return false;

            if (item.MaxStackCount <= 0)
                return false;

            var existingItem = map[loc];
            var existingStackQuantity = existingItem.ItemStackQuantity;
            var itemStackQuantity = item.ItemStackQuantity;
            var newStackSize = existingStackQuantity + itemStackQuantity;

            if (item.ActorSnoId == existingItem.ActorSnoId && newStackSize <= item.MaxStackCount && item.AnnId != existingItem.AnnId)
            {
                Logger.LogVerbose($"Can stash item {item.Name} on page {stashPageNumber} at [col={col},row={row}] (stack on existing {existingStackQuantity} + {itemStackQuantity} ({newStackSize}) / {item.MaxStackCount})");
                placeAtCol = col;
                placeAtRow = row;
                return true;
            }

            return false;
        }

        private static bool TryGetStashingLocation(TrinityItem item, int stashPageNumber, int col, int row, InventoryMap map, ref int placeAtCol, ref int placeAtRow)
        {
            var loc = new Tuple<int, int>(col, row);
            var isSquareEmpty = !map.ContainsKey(loc);

            var isLastRow = (row + 1) % 10 == 0;
            if (isLastRow && item.IsTwoSquareItem)
                return false;

            if (item.IsTwoSquareItem)
            {
                var locBelow = new Tuple<int, int>(col, row + 1);
                if (map.ContainsKey(locBelow))
                    return false;
            }

            var locAbove = new Tuple<int, int>(col, row - 1);
            var isItemAbove = map.ContainsKey(locAbove);
            if (isItemAbove && map[locAbove].IsTwoSquareItem)
                return false;

            if (isSquareEmpty)
            {
                Logger.LogVerbose($"Can stash item {item.Name} on page {stashPageNumber} at [col={col},row={row}]");
                placeAtCol = col;
                placeAtRow = row;
                return true;
            }
            return false;
        }


        public class InventoryMap : Dictionary<Tuple<int, int>, TrinityItem>
        {
            public InventoryMap(Dictionary<Tuple<int, int>, TrinityItem> dictionary) : base(dictionary)
            {
            }
        }

        //private static bool CanPlaceAtLocation(int col, int row, bool isTwoSquare, InventoryMap map = null)
        //{
        //    if (map == null)
        //        map = GetInventoryMap();

        //    var loc = new Tuple<int, int>(col, row);
        //    var isSquareEmpty = !map.ContainsKey(loc);

        //    var isLastRow = (9 + 1) % 10 == 0;
        //    if (isLastRow && isTwoSquare)
        //    {
        //        Logger.Log("LastRow CanPlaceAtLocation");
        //        return false;
        //    }

        //    if (isTwoSquare)
        //    {
        //        var locBelow = new Tuple<int, int>(col, row + 1);
        //        if (map.ContainsKey(locBelow))
        //            return false;
        //    }

        //    var locAbove = new Tuple<int, int>(col, row - 1);
        //    var isItemAbove = map.ContainsKey(locAbove);
        //    if (isItemAbove && map[locAbove].IsTwoSquareItem)
        //        return false;

        //    return isSquareEmpty;
        //}

        //private static bool CanSwapOrPlaceAtLocation(int col, int row, bool isTwoSquare, InventoryMap map = null)
        //{
        //    if (map == null)
        //        map = GetInventoryMap();

        //    var key = new Tuple<int, int>(col, row);

        //    return CanPlaceAtLocation(col, row, isTwoSquare, map) || (map.ContainsKey(key) && isTwoSquare == map[key].IsTwoSquareItem);
        //}

        private static void HandleFullStash()
        {
            Logger.LogError($"[StashItems] There is no space in the stash. Woops!");

            if (GlobalSettings.Instance.FullInventoryHandling == FullInventoryOption.Logout)
            {
                Logger.LogError($"[StashItems] Shutting down DB and D3 and requesting no restarts (DemonbuddyExitCode.DoNotRestart: 12) because of DB Setting 'FullInventoryOption.Logout'!");
                ZetaDia.Service.Party.LeaveGame(false);
                Thread.Sleep(15000);
                BotMain.Stop(false, "");
                BotMain.Shutdown(DemonbuddyExitCode.DoNotRestart, true);
            }
        }

        public static bool StashPagesAvailableToPurchase => 5 - ZetaDia.Me.CommonData.GetAttribute<int>(ActorAttributeType.StashTabsPurchasedWithGold) > 0;

        public static int TotalStashPages => ZetaDia.Me.NumSharedStashSlots / 70;
    }

}


