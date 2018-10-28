using Buddy.Coroutines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Trinity.Framework.Objects.Enums;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;

namespace Trinity.Components.Coroutines.Town
{
    public static partial class TrinityTownRun
    {
        internal class InventoryMap : Dictionary<Tuple<int, int>, TrinityItem>
        {
            public InventoryMap(Dictionary<Tuple<int, int>, TrinityItem> dictionary) : base(dictionary)
            {
            }

            public TrinityItem this[int indexX, int indexY] => this[new Tuple<int, int>(indexX, indexY)];
        }

        private static readonly HashSet<RawItemType> s_specialCaseNonStackableItems = new HashSet<RawItemType>
        {
            RawItemType.CraftingPlan,
            RawItemType.CraftingPlan_Jeweler,
            RawItemType.CraftingPlanLegendary_Smith,
            RawItemType.CraftingPlan_Mystic,
            RawItemType.CraftingPlan_MysticTransmog,
            RawItemType.CraftingPlan_Smith,
        };

        // TODO: Check if that can be cached in another way.
        private static DateTime _lastTypeMapUpdate = DateTime.MinValue;
        private static Dictionary<RawItemType, int> _itemTypeMap;
        private static Dictionary<RawItemType, int> ItemTypeMap
        {
            get
            {
                if (_itemTypeMap != null &&
                    DateTime.UtcNow.Subtract(_lastTypeMapUpdate) <= TimeSpan.FromMinutes(1))
                {
                    return _itemTypeMap;
                }

                Core.Logger.Verbose("Creating Stashing ItemTypeMap");
                var typeMap = new Dictionary<RawItemType, int>();
                var map = Inventory();
                foreach (var item in map)
                {
                    var type = item.Value.RawItemType;
                    if (typeMap.ContainsKey(type))
                        continue;

                    var x = item.Key.Item1;
                    var y = item.Key.Item2;
                    var page = y / 10;
                    typeMap.Add(type, page);
                    Core.Logger.Verbose($"Type: {type} => Page: {page}");
                }

                _itemTypeMap = typeMap;
                _lastTypeMapUpdate = DateTime.UtcNow;
                return _itemTypeMap;
            }
        }

        public static bool StashPagesAvailableToPurchase =>
            5 - ZetaDia.Me.CommonData.GetAttribute<int>(ActorAttributeType.StashTabsPurchasedWithGold) > 0;

        public static int TotalStashPages => ZetaDia.Me.NumSharedStashSlots / 70;

        private static InventoryMap Inventory
        {
            get
            {
                Core.Actors.Update();
                var stashItems = Core.Actors.Inventory
                    .Where(i => i.InventorySlot == InventorySlot.SharedStash)
                    .ToList();
                var itemDict = new Dictionary<Tuple<int, int>, TrinityItem>();
                foreach (var item in stashItems)
                {
                    var key = new Tuple<int, int>(item.InventoryColumn, item.InventoryRow);
                    if (itemDict.ContainsKey(key))
                    {
                        var dup = itemDict[key];
                        Core.Logger.Debug(
                            $"Duplicate Col/Row [{item.InventoryColumn}, {item.InventoryRow}] found while creating InventoryMap for: {item.Name} ({item.ActorSnoId}) {item.ItemType} IsValid=({item.IsValid}) duplicate is: {dup.Name} ({dup.ActorSnoId}) {dup.ItemType} IsValid=({dup.IsValid})");
                        itemDict[key] = item.IsValid ? item : dup;
                    }
                    else
                        itemDict[key] = item;
                }

                return new InventoryMap(itemDict);
            }
        }

        public static bool ShouldStash(TrinityItem i)
        {
            if (BrainBehavior.GreaterRiftInProgress)
                return false;

            if (i.IsUnidentified)
                return true;

            return !i.IsProtected() && Combat.TrinityCombat.Loot.ShouldStash(i);
        }

        public static async Task<bool> StashItems()
        {
            if (!ZetaDia.IsInTown)
                return true;

            var stashItems = Core.Inventory.Backpack.Where(ShouldStash).ToList();
            if (!stashItems.Any())
                return true;

            Core.Logger.Verbose($"[StashItems] Now to stash {stashItems.Count} items");
            stashItems.ForEach(i => Core.Logger.Debug($"[StashItems] Stashing: {i.Name} ({i.ActorSnoId}) InternalName={i.InternalName} Ancient={i.IsAncient} Ann={i.AnnId}"));

            if (!UIElements.StashWindow.IsVisible)
            {
                var stash = ZetaDia.Actors.GetActorsOfType<GizmoPlayerSharedStash>().FirstOrDefault();
                if (stash == null)
                    return false;

                if (!await CommonCoroutines.MoveAndInteract(stash, () => UIElements.StashWindow.IsVisible))
                    return false;
            }

            if (Core.Settings.Items.BuyStashTabs && StashPagesAvailableToPurchase)
            {
                Core.Logger.Error("[StashItems] Attempting to buy stash pages");
                InventoryManager.BuySharedStashSlots();
            }

            // TODO: Figure out if those 2 calls are still relevant.
            await StackRamaladnisGift();
            await StackCraftingMaterials();

            // Get the first item to stash...
            var item = Core.Inventory.Backpack.FirstOrDefault(ShouldStash);
            if (item == null)
                return true;

            try
            {
                item.OnUpdated(); // make sure wrong col/row/location is not cached after a move.
                var page = GetBestStashLocation(item, out var col, out var row);
                if (page == -1)
                {
                    Core.Logger.Verbose($"[StashItems] No place to put item, stash is probably full ({item.Name} [{col},{row}] Page={page})");
                    HandleFullStash();
                    return true;
                }

                if (page != InventoryManager.CurrentStashPage)
                {
                    Core.Logger.Verbose($"[StashItems] Changing to stash page: {page}");
                    InventoryManager.SwitchStashPage(page);
                    return false;
                }

                Core.Logger.Debug($"[StashItems] Stashing: {item.Name} ({item.ActorSnoId}) [{item.InventoryColumn},{item.InventoryRow} {item.InventorySlot}] Quality={item.ItemQualityLevel} IsAncient={item.IsAncient} InternalName={item.InternalName} StashPage={page}");

                ItemEvents.FireItemStashed(item);
                InventoryManager.MoveItem(
                    item.AnnId,
                    Core.Player.MyDynamicID,
                    InventorySlot.SharedStash,
                    col,
                    row);
            }
            catch (Exception ex)
            {
                Core.Logger.Log($"Exception Stashing Item: {ex}");
            }

            return false;
        }

        private static void UpdateAfterItemMove(TrinityItem item)
        {
            if (item.IsValid &&
                item.CommonData.IsValid &&
                !item.CommonData.IsDisposed)
            {
                item.OnCreated();
            }

            Core.Actors.Update();
        }

        public static async Task<bool> StackRamaladnisGift()
        {
            var items = Core.Inventory.Stash.Where(i => i.RawItemType == RawItemType.GeneralUtility && !i.IsTradeable).ToList();
            if (!items.Any())
                return false;

            var targetStack = items.OrderBy(i => i.ItemStackQuantity).First();
            foreach (var item in items)
            {
                if (Equals(item, targetStack))
                    continue;

                InventoryManager.MoveItem(item.AnnId, Core.Player.MyDynamicID, InventorySlot.SharedStash, targetStack.InventoryColumn, targetStack.InventoryRow);
                await Coroutine.Yield();
            }

            return true;
        }

        public static int GetStashPage(TrinityItem item)
        {
            if (item.InventorySlot != InventorySlot.SharedStash)
                return -1;

            return (int)Math.Floor(item.InventoryRow / 10d);
        }

        public static TrinityItem GetItemAtLocation(int col, int row)
        {
            return Core.Inventory.Stash.FirstOrDefault(i => i.InventoryRow == row && i.InventoryColumn == col);
        }

        public static TrinityItem GetNextStashItem(int currentCol, int currentRow, int actorSnoId = -1)
        {
            if (actorSnoId > 0)
            {
                var nextItemOfType = Core.Inventory.Stash.FirstOrDefault(i =>
                    i.ActorSnoId == actorSnoId &&
                    i.InventoryRow > currentRow || i.InventoryRow == currentRow && i.InventoryColumn > currentCol);

                if (nextItemOfType != null)
                    return nextItemOfType;
            }
            return Core.Inventory.Stash.FirstOrDefault(i => !i.IsTwoSquareItem &&
                                                            (i.InventoryRow > currentRow ||
                                                             i.InventoryRow == currentRow) &&
                                                            i.InventoryColumn > currentCol);
        }

        public static async Task<bool> StackCraftingMaterials()
        {
            foreach (var itemGroup in Core.Inventory.Stash
                .Where(i => i.MaxStackCount > 0 &&
                            i.ItemStackQuantity < i.MaxStackCount &&
                            !i.IsTradeable)
                .GroupBy(i => i.Name))
            {
                if (itemGroup.Count() <= 1)
                    continue;

                Core.Logger.Debug($">> Started Stacking: {itemGroup.Key}");
                var usedIds = new HashSet<int>();
                var stacks = itemGroup.OrderBy(i => i.ItemStackQuantity).ToList();
                foreach (var item in stacks)
                {
                    if (usedIds.Contains(item.AnnId))
                        continue;

                    var map = Inventory();
                    int col = 0, row = 0;
                    var page = GetIdealStashPage(item);
                    if (page == -1)
                        page = GetStashPage(item);

                    if (!item.IsValid)
                        continue;

                    if (!CanStackOnPage(item, page, ref col, ref row, map))
                        continue;

                    var targetItem = map[col, row];
                    if (!targetItem.IsValid)
                        continue;

                    Core.Logger.Log($"[StashItems] Stacking: {item.Name} ({item.ActorSnoId}) [{item.InventoryColumn},{item.InventoryRow}] ({item.ItemStackQuantity}) onto [{col},{row}] ({targetItem.ItemStackQuantity})");
                    InventoryManager.MoveItem(
                        item.AnnId,
                        Core.Player.MyDynamicID,
                        InventorySlot.SharedStash,
                        col,
                        row);

                    usedIds.Add(item.AnnId);

                    await Coroutine.Yield();
                    // TODO: Check if that yield update wait sequence is needed at all. I'm not even sure if crafting materials still go into the stash...
                    Core.Actors.Update();
                    await Coroutine.Wait(5000, () => !item.IsValid ||
                                                     targetItem.ItemStackQuantity == targetItem.MaxStackCount);

                    Core.Logger.Verbose($"Source [{item.InventoryColumn},{item.InventoryRow}] IsValid={item.IsValid} Stack={item.ItemStackQuantity}");
                    Core.Logger.Verbose($"Target [{targetItem.InventoryColumn},{targetItem.InventoryRow}] IsValid={targetItem.IsValid} Stack={targetItem.ItemStackQuantity}");
                }
                Core.Logger.Debug($">> Finished Stacking: {itemGroup.Key}");
            }
            return true;
        }

        public static IEnumerable<TrinityItem> GetItemsOnStashPage(int page)
        {
            return Core.Inventory.Stash
                .Where(i => i.InventoryRow >= page * 10 &&
                            i.InventoryRow < page * 10 + 10);
        }

        /// <summary>
        /// Get the stash page where items should ideally be placed, ignoring if it can actually be placed there.
        /// </summary>
        public static int GetIdealStashPage(TrinityItem item)
        {
            if (item.IsEquipment &&
                Core.Settings.Items.UseTypeStashingEquipment &&
                ItemTypeMap.ContainsKey(item.RawItemType))
                return ItemTypeMap[item.RawItemType];

            if (Core.Settings.Items.UseTypeStashingOther &&
                ItemTypeMap.ContainsKey(item.RawItemType))
                return ItemTypeMap[item.RawItemType];

            if (item.ItemBaseType >= ItemBaseType.Misc)
                return TotalStashPages - 1;

            return -1;
        }

        public static int GetBestStashLocation(TrinityItem item, out int col, out int row)
        {
            col = 0;
            row = 0;

            var stashPageOffset = GetIdealStashPage(item);
            if (stashPageOffset != -1)
            {
                int stashpage;
                if (stashPageOffset < 0)
                    stashpage = (-1 + TotalStashPages) + stashPageOffset;
                else if (stashPageOffset > TotalStashPages - 1)
                    stashpage = TotalStashPages - 1;
                else
                    stashpage = stashPageOffset;

                if (CanPutItemInStashPage(item, stashpage, out col, out row))
                    return stashpage;
            }
            if (item.ItemBaseType >= ItemBaseType.Misc)
            {
                for (var i = TotalStashPages - 1; i >= 0; i--)
                {
                    if (CanPutItemInStashPage(item, i, out col, out row))
                        return i;
                }
            }
            else
            {
                for (var j = 0; j < TotalStashPages - 1; j++)
                {
                    if (CanPutItemInStashPage(item, j, out col, out row))
                        return j;
                }
            }
            return -1;
        }

        public static bool CanPutItemInStashPage(TrinityItem item, int stashPageNumber, out int col, out int row)
        {
            var itemsOnStashPage = Inventory();

            col = 0;
            row = 0;

            switch (stashPageNumber)
            {
                case 0 when !UIElements.StashDialogMainPageTab1.IsEnabled:
                case 1 when !UIElements.StashDialogMainPageTab2.IsEnabled:
                case 2 when !UIElements.StashDialogMainPageTab3.IsEnabled:
                case 3 when !UIElements.StashDialogMainPageTab4.IsEnabled:
                case 4 when !UIElements.StashDialogMainPageTab5.IsEnabled:
                    return false;
            }

            if (CharacterSettings.Instance.ProtectedStashPages.Contains(stashPageNumber))
                return false;

            return CanStackOnPage(item, stashPageNumber, ref col, ref row, itemsOnStashPage) ||
                   CanPlaceOnPage(item, stashPageNumber, ref col, ref row, itemsOnStashPage);
        }

        private static bool CanPlaceOnPage(TrinityItem item, int stashPageNumber, ref int col, ref int row, InventoryMap itemsOnStashPage)
        {
            for (var i = 0; i < 10; i++)
            {
                for (var c = 0; c < 7; c++)
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

            if (item.MaxStackCount <= 0 ||
                item.IsTradeable ||
                s_specialCaseNonStackableItems.Contains(item.RawItemType))
            {
                return false;
            }

            for (var i = 0; i < 10; i++)
            {
                for (var c = 0; c < 7; c++)
                {
                    var r = stashPageNumber * 10 + i;

                    if (TryGetStackLocation(item, stashPageNumber, c, r, itemsOnStashPage, ref col, ref row))
                        return true;
                }
            }
            return false;
        }
        
        private static bool TryGetStackLocation(TrinityItem item, int stashPageNumber, int col, int row, InventoryMap map, ref int placeAtCol, ref int placeAtRow)
        {
            var loc = new Tuple<int, int>(col, row);

            if (col == placeAtCol &&
                row == placeAtRow)
            {
                return false;
            }

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

            if (item.ActorSnoId != existingItem.ActorSnoId ||
                newStackSize > item.MaxStackCount ||
                item.AnnId == existingItem.AnnId)
            {
                return false;
            }

            Core.Logger.Debug($"Can stash item {item.Name} on page {stashPageNumber} at [col={col},row={row}] (stack on existing {existingStackQuantity} + {itemStackQuantity} ({newStackSize}) / {item.MaxStackCount})");
            placeAtCol = col;
            placeAtRow = row;
            return true;

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
            if (isItemAbove &&
                map[locAbove].IsTwoSquareItem)
            {
                return false;
            }

            if (!isSquareEmpty)
                return false;

            Core.Logger.Verbose($"Can stash item {item.Name} on page {stashPageNumber} at [col={col},row={row}]");
            placeAtCol = col;
            placeAtRow = row;
            return true;
        }

        private static void HandleFullStash()
        {
            Core.Logger.Error($"[StashItems] There is no space in the stash. Woops!");

            if (GlobalSettings.Instance.FullInventoryHandling != FullInventoryOption.Logout)
                return;

            Core.Logger.Error($"[StashItems] Shutting down DB and D3 and requesting no restarts (DemonbuddyExitCode.DoNotRestart: 12) because of DB Setting 'FullInventoryOption.Logout'!");
            ZetaDia.Service.Party.LeaveGame();
            Thread.Sleep(15000);
            BotMain.Stop();
            BotMain.Shutdown(DemonbuddyExitCode.DoNotRestart, true);
        }
    }
}
