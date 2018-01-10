using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Coroutines.Town;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;


namespace Trinity.Components.Coroutines
{
    public static class ItemSort
    {
        private const int ItemMovementDelay = 75;
        const string HookName = "VendorRun";

        public static bool IsSorting { get; private set; }

        /// <summary>
        /// Compares items for sorting. Returns -1 if item1 comes before item2. Returns 0 if items are of equal sort. Returns 1 if item1 should come after item2.
        /// </summary>
        /// <param name="thisItem">The item1.</param>
        /// <param name="thatItem">The item2.</param>
        /// <returns>System.Int32.</returns>
        public static int Compare(this ItemWrapper thisItem, ItemWrapper thatItem)
        {
            Core.Logger.Verbose("Comparing item {0} ({1}) to {2} ({3})", thisItem.Name, thisItem.InternalName, thatItem.Name, thatItem.InternalName);
            if (thisItem.DynamicId == thatItem.DynamicId)
                return 0;

            string thisInternalName = thisItem.InternalName.ToLower().Replace("x1_", "").Replace("p1_", "");
            string thatInternalName = thatItem.InternalName.ToLower().Replace("x1_", "").Replace("p1_", "");
            string thisSortName = thisItem.Name;
            string thatSortName = thatItem.Name;

            // Compare front to back, or back to front
            if (!thisItem.IsEquipment)
            {
                /*
                 *  Non-Equipent (Potions, Gems, Keys, Consumables, Follower Items)
                 */

                // Potions
                if (thisItem.ItemType == ItemType.Potion)
                {
                    if (thatItem.ItemType == ItemType.Potion)
                    {
                        return String.Compare(thisSortName, thatSortName, StringComparison.InvariantCulture);
                    }
                    return 1;
                }
                if (thisItem.ItemType != ItemType.Potion && thatItem.ItemType == ItemType.Potion)
                    return -1;

                // Gems
                if (thisItem.IsGem)
                {
                    if (thatItem.IsGem)
                    {
                        // Amethyst
                        // Diamond
                        // Emerald
                        // Ruby
                        // Topaz
                        var gemType = new[] { thisInternalName.Substring(0, 4), thatInternalName.Substring(0, 4) };
                        if (gemType[0] != gemType[1])
                            return String.Compare(gemType[0], gemType[1], StringComparison.InvariantCulture);

                        if (thisItem.GemQuality == thatItem.GemQuality)
                            return thisItem.ItemStackQuantity.CompareTo(thatItem.ItemStackQuantity) * -1;
                        return thisItem.GemQuality.CompareTo(thatItem.GemQuality);
                    }
                    return 1;
                }
                if (thatItem.IsGem && !thisItem.IsGem)
                    return -1;

                // Rift Keys
                if (thisItem.ItemType == ItemType.KeystoneFragment)
                {
                    // Greater
                    // Trial
                    // Normal
                    if (thatItem.ItemType == ItemType.KeystoneFragment)
                    {
                        return thisItem.TieredLootRunKeyLevel.CompareTo(thatItem.TieredLootRunKeyLevel);
                    }
                    return 1;
                }
                if (thatItem.ItemType == ItemType.KeystoneFragment && thisItem.ItemType != ItemType.KeystoneFragment)
                    return -1;

                // Ramadalini's Gift
                const string ramadalinisGiftInternalname = "Consumable_Add_Sockets";
                if (thisItem.Name.StartsWith(ramadalinisGiftInternalname))
                {
                    if (thatItem.Name.StartsWith(ramadalinisGiftInternalname))
                        return 0;
                    return 1;
                }
                if (thatItem.Name.StartsWith(ramadalinisGiftInternalname) && !thatItem.Name.StartsWith(ramadalinisGiftInternalname))
                    return -1;

                // Item Quality
                if (thisItem.ItemQualityLevel != thatItem.ItemQualityLevel)
                {
                    return thisItem.ItemQualityLevel.CompareTo(thatItem.ItemQualityLevel);
                }

                // Item Base Type
                if (thisItem.ItemBaseType != thatItem.ItemBaseType)
                {
                    return thisItem.ItemBaseType.CompareTo(thatItem.ItemBaseType);
                }

                // Item Type
                if (thisItem.ItemType != thatItem.ItemType)
                {
                    return thisItem.ItemType.CompareTo(thatItem.ItemType);
                }

                return String.Compare(thisSortName, thatSortName, StringComparison.InvariantCulture);
            }
            if (thisItem.IsEquipment)
            {
                /*
                 *  Equipment (Weapons, Armor, Jewlery)
                 */

                // Two slots before one slots
                if (!thisItem.IsTwoSquareItem && thatItem.IsTwoSquareItem)
                    return -1;
                if (thisItem.IsTwoSquareItem && !thatItem.IsTwoSquareItem)
                    return 1;

                // Sort Sets
                if (thisItem.IsSetItem && thatItem.IsSetItem && thisItem.IsTwoSquareItem)
                {
                    bool isSameSet = thisItem.ItemSetName == thatItem.ItemSetName;
                    if (isSameSet)
                    {
                        return String.Compare(thisSortName, thatSortName, StringComparison.InvariantCulture);
                    }

                    return String.Compare(thisItem.ItemSetName, thatItem.ItemSetName, StringComparison.InvariantCulture);
                }

                // Compare ItemQualityLevel - Legendaries come before other junk
                if (thisItem.ItemQualityLevel != thatItem.ItemQualityLevel)
                {
                    return thisItem.ItemQualityLevel.CompareTo(thatItem.ItemQualityLevel);
                }

                // Compare DBItemBaseType order
                if (thisItem.ItemBaseType == ItemBaseType.Weapon && thatItem.ItemBaseType != ItemBaseType.Weapon)
                    return 1;
                if (thatItem.ItemBaseType == ItemBaseType.Weapon && thisItem.ItemBaseType != ItemBaseType.Weapon)
                    return -1;

                // Compare Armor
                if (thisItem.ItemBaseType == ItemBaseType.Armor)
                {
                    if (thatItem.ItemBaseType == ItemBaseType.Weapon)
                        return -1;
                    if (thatItem.ItemBaseType != ItemBaseType.Armor)
                        return 1;

                    if (thisItem.ItemType != thatItem.ItemType)
                    {
                        if (thisItem.IsTwoSquareItem && !thatItem.IsTwoSquareItem)
                            return 1;

                        if (!thisItem.IsTwoSquareItem && thatItem.IsTwoSquareItem)
                            return -1;

                        return thisItem.ItemType.CompareTo(thatItem.ItemType);
                    }

                    return String.Compare(thisSortName, thatSortName, StringComparison.InvariantCulture);
                }

                // Compare Jewlery
                if (thisItem.ItemBaseType == ItemBaseType.Jewelry)
                {
                    if (thatItem.ItemBaseType == ItemBaseType.Weapon)
                        return -1;

                    if (thatItem.ItemBaseType == ItemBaseType.Armor)
                        return -1;

                    if (thisItem.ItemType != thatItem.ItemType)
                    {
                        return thisItem.ItemType.CompareTo(thatItem.ItemType);
                    }

                    return String.Compare(thisSortName, thatSortName, StringComparison.InvariantCulture);
                }

                // Weapons, sort by Has Sockets, Average Damage
                if (thisItem.ItemBaseType == ItemBaseType.Weapon)
                {
                    if (thisItem.StatsData.Sockets > 0 && thatItem.StatsData.Sockets == 0)
                        return 1;
                    if (thatItem.StatsData.Sockets > 0 && thisItem.StatsData.Sockets == 0)
                        return -1;

                    if (((thisItem.StatsData.MinDamage + thisItem.Stats.MaxDamage) / 2) > ((thatItem.StatsData.MinDamage + thatItem.StatsData.MaxDamage) / 2))
                        return 1;

                    if (((thisItem.StatsData.MinDamage + thisItem.Stats.MaxDamage) / 2) < ((thatItem.StatsData.MinDamage + thatItem.StatsData.MaxDamage) / 2))
                        return -1;

                    if (thisItem.ItemType != thatItem.ItemType)
                        return thisItem.ItemType.CompareTo(thatItem.ItemType);

                    return thisItem.StatsData.WeaponDamagePerSecond.CompareTo(thatItem.StatsData.WeaponDamagePerSecond);
                }
                return 1;

            }
            return -1;
        }

        private static Composite _sortBehavior;
        private static Queue<ItemWrapper> _sortedItemsQueue;
        private static Queue<ItemWrapper> _reverseSortedItemsQueue;
        private static bool[,] _usedGrid = new bool[10, 6];
        private static bool _hookInserted;

        public static void SortBackpack()
        {
            if (!BotMain.IsRunning)
            {
                TaskDispatcher.Start(ret => SortTask(InventorySlot.BackpackItems), ret => !IsSorting);
                return;
            }

            try
            {
                //GoldInactivity.Instance.ResetCheckGold();
                //XpInactivity.Instance.ResetCheckXp();

                if (!_hookInserted)
                {
                    _sortBehavior = CreateSortBehavior(inventorySlot: InventorySlot.BackpackItems);
                    TreeHooks.Instance.InsertHook(HookName, 0, _sortBehavior);
                    _hookInserted = true;
                    BotMain.OnStop += bot => RemoveBehavior();
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Error running Sort backpack: " + ex);
                RemoveBehavior();
            }

        }

        public static void SortStash()
        {
            if (!BotMain.IsRunning)
            {
                TaskDispatcher.Start(ret => SortTask(InventorySlot.SharedStash), ret => !IsSorting);
                return;
            }

            try
            {
                if (!_hookInserted)
                {
                    _sortBehavior = CreateSortBehavior(inventorySlot: InventorySlot.SharedStash);
                    TreeHooks.Instance.InsertHook(HookName, 0, _sortBehavior);
                    _hookInserted = true;
                    BotMain.OnStop += bot => RemoveBehavior();
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Error running Sort stash: " + ex);
                RemoveBehavior();
            }

        }

        private static void RemoveBehavior()
        {
            IsSorting = false;

            if (_sortBehavior != null)
            {
                try
                {
                    if (_hookInserted)
                    {
                        TreeHooks.Instance.RemoveHook(HookName, _sortBehavior);
                        _hookInserted = false;
                    }
                }
                catch (Exception ex)
                {
                    Core.Logger.Verbose("Sort behavior not inserted? " + ex);
                }
            }
        }

        /// <summary>
        /// Creates the sort behavior.
        /// </summary>
        /// <returns>Composite.</returns>
        public static Composite CreateSortBehavior(InventorySlot inventorySlot)
        {
            return new ActionRunCoroutine(ret => SortTask(inventorySlot));
        }

        internal static async Task<bool> SortTask(InventorySlot inventorySlot)
        {

            IsSorting = true;

            if (!ZetaDia.IsInGame)
                return false;
            if (ZetaDia.Globals.IsLoadingWorld)
                return false;
            if (!ZetaDia.Me.IsFullyValid())
                return false;

            if (ZetaDia.Me.IsParticipatingInTieredLootRun)
            {
                Core.Logger.Log("Cannot sort while in trial/greater rift");
                RemoveBehavior();
                return false;
            }

            if (inventorySlot == InventorySlot.SharedStash && !await ReturnToStash.Execute())
            {
                return true;
            }
            Core.Logger.Log("Starting sort task for {0}", inventorySlot);

            List<ItemWrapper> wrappedItems;

            // Setup grid
            if (inventorySlot == InventorySlot.BackpackItems)
            {
                wrappedItems = InventoryManager.Backpack.Where(i => i.IsValid).Select(i => new ItemWrapper(i)).ToList();

                _usedGrid = new bool[10, 6];
                // Block off the entire of any "protected bag slots"
                foreach (InventorySquare square in CharacterSettings.Instance.ProtectedBagSlots)
                {
                    _usedGrid[square.Column, square.Row] = true;
                    Core.Logger.Verbose("Slot {0},{1} is protected", square.Column, square.Row);
                }
            }
            else if (inventorySlot == InventorySlot.SharedStash)
            {
                wrappedItems = InventoryManager.StashItems.Where(i => i.IsValid).Select(i => new ItemWrapper(i)).ToList();

                int maxStashRow = InventoryManager.NumSharedStashSlots / 7;
                // 7 columns, 10 rows x 5 pages
                _usedGrid = new bool[7, maxStashRow];
            }
            else
            {
                Core.Logger.Error("Unsupported Inventory Slot {0}", inventorySlot);
                return false;
            }


            var equipment = wrappedItems.Where(i => i.IsEquipment).OrderByDescending(i => i);
            _sortedItemsQueue = new Queue<ItemWrapper>(equipment);
            Core.Logger.Verbose("Queued {0} items for forward sort", _sortedItemsQueue.Count());

            foreach (var item in equipment)
            {
                Core.Logger.Verbose("{0}", item.Name);
            }

            var misc = wrappedItems.Where(i => !i.IsEquipment).OrderByDescending(i => i);
            _reverseSortedItemsQueue = new Queue<ItemWrapper>(misc);
            Core.Logger.Verbose("Queued {0} items for reverse sort", _reverseSortedItemsQueue.Count());

            foreach (var item in misc)
            {
                Core.Logger.Verbose("{0}", item.Name);
            }

            if (!_reverseSortedItemsQueue.Any() && !_sortedItemsQueue.Any())
            {
                _reverseSortedItemsQueue = null;
                _sortedItemsQueue = null;
                Core.Logger.Log("No items found to sort?");
                RemoveBehavior();
                return false;
            }

            if (!UIElements.InventoryWindow.IsVisible && inventorySlot == InventorySlot.BackpackItems)
            {
                Core.Logger.Log("Opening inventory window");
                var inventoryButton = UIElement.FromName("Root.NormalLayer.game_dialog_backgroundScreenPC.button_inventory");
                if (inventoryButton != null && inventoryButton.IsEnabled && inventoryButton.IsVisible)
                {
                    inventoryButton.Click();
                    await Coroutine.Sleep(100);
                    await Coroutine.Yield();
                }
                else
                {
                    Core.Logger.Error("Derp - couldn't find inventory Button!");
                }
            }

            Core.Logger.Log("Executing sort task");
            if (GameUI.IsElementVisible(GameUI.StashDialogMainPage) && inventorySlot == InventorySlot.SharedStash)
            {
                await SortItems(inventorySlot);
                await ReverseItems(inventorySlot);

                Core.Logger.Log("Waiting 5 seconds...");
                BotMain.StatusText = "Waiting 5 seconds...";
                await Coroutine.Sleep(5000);

                if (ReturnToStash.StartedOutOfTown && ZetaDia.IsInTown)
                    await CommonBehaviors.TakeTownPortalBack().ExecuteCoroutine();
            }
            else if (inventorySlot == InventorySlot.BackpackItems)
            {
                await SortItems(inventorySlot);
                await ReverseItems(inventorySlot);
            }

            RemoveBehavior();
            return true;

        }

        public static async Task<bool> SortItems(InventorySlot inventorySlot)
        {
            try
            {
                Core.Logger.Verbose("Initiating sort task 1");

                var myDynamicId = ZetaDia.Me.CommonData.AnnId;
                int currentRow = 0;
                int currentCol = 0;
                int maxCol = 0, maxRow = 0;
                switch (inventorySlot)
                {
                    case InventorySlot.BackpackItems:
                        maxCol = 9;
                        maxRow = 5;
                        break;
                    case InventorySlot.SharedStash:
                        maxCol = 6;
                        maxRow = (InventoryManager.NumSharedStashSlots / 7) - 1;
                        break;
                }

                Core.Logger.Log("Using max columns of {0}, max rows of {1}", maxCol, maxRow);

                while (_sortedItemsQueue.Any())
                {
                    var i = _sortedItemsQueue.Dequeue();

                    if (inventorySlot == InventorySlot.BackpackItems && CharacterSettings.Instance.ProtectedBagSlots.Any(pbs => pbs.IsItemInSquare(i.Item)))
                    {
                        Core.Logger.Verbose("Item {0} is protected!", i.Name);
                        continue;
                    }

                    if (currentCol > maxCol)
                    {
                        currentCol = 0;
                        currentRow++;
                    }

                    while (_usedGrid[currentCol, currentRow])
                    {
                        Core.Logger.Verbose("Grid location {0},{1} is already used", currentCol, currentRow);
                        currentCol++;
                        if (currentCol > maxCol)
                        {
                            currentCol = 0;
                            currentRow++;
                        }
                    }

                    if (i.Item.InventoryColumn == currentCol && i.Item.InventoryRow == currentRow)
                    {
                        Core.Logger.Verbose("Item {0} is already sorted at {1},{2}", i.Name, i.Item.InventoryColumn, i.Item.InventoryRow);
                        MarkCellAsUsed(currentRow, currentCol, i);

                        currentCol++;
                        continue;
                    }

                    await SetStashpage(currentRow);

                    await ClearSpot(inventorySlot, currentCol, currentRow, i.IsTwoSquareItem, isForward: true);

                    string msg = String.Format("Moving item {0} from {1},{2} to {3},{4}", i.Name, i.Item.InventoryColumn, i.Item.InventoryRow, currentCol, currentRow);
                    BotMain.StatusText = msg;
                    Core.Logger.Verbose(msg);
                    InventoryManager.MoveItem(i.DynamicId, myDynamicId, inventorySlot, currentCol, currentRow);

                    MarkCellAsUsed(currentRow, currentCol, i);
                    currentCol++;

                    await Coroutine.Sleep(ItemMovementDelay);
                    await Coroutine.Yield();
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Error sorting " + inventorySlot + " " + ex);
                RemoveBehavior();
            }
            return false;
        }

        public static async Task<bool> ReverseItems(InventorySlot inventorySlot)
        {
            try
            {
                Core.Logger.Verbose("Initiating sort task 2");

                var myDynamicId = ZetaDia.Me.CommonData.AnnId;
                int currentRow = 5;
                int currentCol = 9;
                int maxCol = 0, maxRow = 0;
                switch (inventorySlot)
                {
                    case InventorySlot.BackpackItems:
                        currentCol = maxCol = 9;
                        currentRow = maxRow = 5;
                        break;
                    case InventorySlot.SharedStash:
                        currentCol = maxCol = 6;
                        currentRow = maxRow = (InventoryManager.NumSharedStashSlots / 7) - 1;
                        break;
                }

                Core.Logger.Log("Using max columns of {0}, max rows of {1}", maxCol, maxRow);

                while (_reverseSortedItemsQueue.Any())
                {
                    var i = _reverseSortedItemsQueue.Dequeue();
                    if (inventorySlot == InventorySlot.BackpackItems && CharacterSettings.Instance.ProtectedBagSlots.Any(pbs => pbs.IsItemInSquare(i.Item)))
                    {
                        Core.Logger.Verbose("Item {0} is protected!", i.Name);
                        continue;
                    }

                    if (currentCol < 0)
                    {
                        currentCol = maxCol;
                        currentRow--;
                    }

                    while (_usedGrid[currentCol, currentRow])
                    {
                        Core.Logger.Verbose("Grid location {0},{1} is used already", currentCol, currentRow);
                        currentCol--;
                        if (currentCol < 0)
                        {
                            currentCol = maxCol;
                            currentRow--;
                        }
                    }

                    if (i.Item.InventoryColumn == currentCol && i.Item.InventoryRow == currentRow)
                    {
                        Core.Logger.Verbose("Item {0} is already sorted at {1},{2}", i.Name, i.Item.InventoryColumn, i.Item.InventoryRow);
                        MarkCellAsUsed(currentRow, currentCol, i);
                        currentCol--;
                        continue;
                    }

                    int desiredStashPage = await SetStashpage(currentRow);

                    if (i.Item.MaxStackCount > 1 && InventoryManager.CanStackItemInStashPage(i.Item, desiredStashPage) && GetNumberOfStacks(i.Item, inventorySlot) > 1)
                    {
                        InventoryManager.QuickWithdraw(i.Item);
                        await Coroutine.Sleep(100);
                        await Coroutine.Yield();

                        var sameItem = InventoryManager.Backpack.FirstOrDefault(item => item.ActorSnoId == i.ActorSnoId && item.Name.StartsWith(i.Name.Substring(0, 4)));
                        if (sameItem != null)
                        {
                            InventoryManager.QuickStash(sameItem);
                            await Coroutine.Sleep(100);
                        }
                        continue;
                    }

                    await ClearSpot(inventorySlot, currentCol, currentRow, i.IsTwoSquareItem, isForward: false);

                    string msg = String.Format("Moving item {0} from {1},{2} to {3},{4}", i.Name, i.Item.InventoryColumn, i.Item.InventoryRow, currentCol, currentRow);
                    BotMain.StatusText = msg;
                    Core.Logger.Verbose(msg);
                    InventoryManager.MoveItem(i.DynamicId, myDynamicId, inventorySlot, currentCol, currentRow);

                    MarkCellAsUsed(currentRow, currentCol, i);
                    currentCol--;

                    await Coroutine.Sleep(ItemMovementDelay);
                    await Coroutine.Yield();

                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Error sorting " + inventorySlot + " " + ex);
                RemoveBehavior();
            }
            return false;
        }

        private static int GetNumberOfStacks(ACDItem item, InventorySlot inventorySlot)
        {
            List<ACDItem> items;
            switch (inventorySlot)
            {
                case InventorySlot.BackpackItems:
                    items = InventoryManager.Backpack.Where(i => i.IsValid && i.ActorSnoId == item.ActorSnoId && i.ItemType == item.ItemType).ToList();
                    break;
                case InventorySlot.SharedStash:
                    items = InventoryManager.StashItems.Where(i => i.IsValid && i.ActorSnoId == item.ActorSnoId && i.ItemType == item.ItemType).ToList();
                    break;
                default:
                    return 0;
            }
            return items.Count();
        }

        private static async Task<int> SetStashpage(int currentRow)
        {
            int desiredStashPage = (int)Math.Floor((double)currentRow / 10);
            if (InventoryManager.CurrentStashPage != desiredStashPage)
            {
                InventoryManager.SwitchStashPage(desiredStashPage);
                await Coroutine.Sleep(100);
                await Coroutine.Yield();
            }
            return desiredStashPage;
        }

        private static void MarkCellAsUsed(int currentRow, int currentCol, ItemWrapper i)
        {
            _usedGrid[currentCol, currentRow] = true;
            if (i.IsTwoSquareItem)
            {
                Core.Logger.Verbose("{0} is two squares, marking {1},{2} as used", i.Name, currentCol, currentRow + 1);
                _usedGrid[currentCol, currentRow + 1] = true;
            }
        }


        public static async Task<bool> ClearSpot(InventorySlot location, int col, int row, bool isTwoSquare, bool isForward)
        {
            var myDynamicId = ZetaDia.Me.CommonData.AnnId;
            int lastRow = isTwoSquare ? row + 1 : row;

            for (; row <= lastRow; row++)
            {
                if (InventoryManager.ItemInLocation(location, col, row))
                {
                    var item = GetItemInLocation(location, col, row);

                    if (item != null)
                    {
                        var newSpot = FindEmptySquare(location, col, row, isTwoSquare, isForward: isForward);

                        if (newSpot.Item1 != -1 && newSpot.Item2 != -1)
                        {
                            string msg = $"Clearing location {col},{row} - Moving item {item.Name} to {newSpot.Item1},{newSpot.Item2}";
                            BotMain.StatusText = msg;
                            Core.Logger.Verbose(msg);
                            InventoryManager.MoveItem(item.AnnId, myDynamicId, location, newSpot.Item1, newSpot.Item2);

                            await Coroutine.Sleep(ItemMovementDelay);
                            await Coroutine.Yield();
                        }
                        else
                        {
                            Core.Logger.Verbose("Couldn't find a new location for {0} at {1},{2}", item.Name, col, row);
                        }
                    }
                    else
                    {
                        Core.Logger.Error("Item in location {0},{1} was not found!", col, row);
                    }
                }
            }

            return true;
        }

        private static ACDItem GetItemInLocation(InventorySlot location, int col, int row)
        {
            Func<ACDItem, bool> twoSlot1 = i => i.InventorySlot == location && i.InventoryRow == row && i.InventoryColumn == col;
            Func<ACDItem, bool> twoSlot2 = i => i.InventorySlot == location && i.InventoryRow == row - 1 && i.InventoryColumn == col && i.IsTwoSquareItem;

            return ZetaDia.Actors.ACDList
                .Where(i => i is ACDItem && i.IsValid)
                .Cast<ACDItem>()
                .FirstOrDefault(i => twoSlot1(i) || twoSlot2(i));
        }

        internal static Tuple<int, int> FindEmptySquare(InventorySlot inventorySlot, int targetCol, int targetRow, bool isTwoSquare, bool isForward)
        {
            switch (inventorySlot)
            {
                case InventorySlot.BackpackItems:
                    return FindEmptyBackpackSquare(targetCol, targetRow, isTwoSquare, isForward);
                case InventorySlot.SharedStash:
                    return FindEmptyStashSquare(targetCol, targetRow, isTwoSquare, isForward);
                default:
                    Core.Logger.Error("Invalid location to find empty slot: {0}", inventorySlot);
                    return new Tuple<int, int>(-1, -1);
            }
        }

        private static Tuple<int, int> FindEmptyStashSquare(int targetCol, int targetRow, bool isTwoSquare, bool isForward)
        {
            try
            {
                int stashRows = InventoryManager.NumSharedStashSlots / 7;
                bool[,] stashSlotBlocked = new bool[7, stashRows];

                if (targetCol >= 0 && targetRow >= 0)
                {
                    stashSlotBlocked[targetCol, targetRow] = true;
                    if (isTwoSquare)
                        stashSlotBlocked[targetCol, targetRow + 1] = true;
                }
                // Map out all the items already in the backpack
                foreach (ACDItem item in InventoryManager.StashItems)
                {
                    if (!item.IsValid)
                    {
                        Core.Logger.Verbose("Found invalid item while trying to find backback slot!");
                        continue;
                    }

                    int col = item.InventoryColumn;
                    int row = item.InventoryRow;

                    stashSlotBlocked[col, row] = true;

                    if (!item.IsTwoSquareItem)
                        continue;

                    stashSlotBlocked[col, row + 1] = true;
                }

                if (isForward)
                {
                    // 50 rows
                    for (int row = 0; row <= stashRows - 1; row++)
                    {
                        // 7 columns
                        for (int col = 0; col <= 6; col++)
                        {
                            if (InventoryManager.ItemInLocation(InventorySlot.SharedStash, col, row))
                                continue;

                            // Slot is blocked, skip
                            if (stashSlotBlocked[col, row])
                                continue;

                            // Slot is used for sorting!
                            if (_usedGrid[col, row])
                                continue;

                            if (isTwoSquare)
                            {
                                // Is a Two Slot, Can't check for 2 slot items on last row
                                if (row == 5)
                                    continue;

                                // Is a Two Slot, check row below
                                if (stashSlotBlocked[col, row + 1])
                                    continue;

                                // Slot is used for sorting!
                                if (_usedGrid[col, row + 1])
                                    continue;
                            }
                            return new Tuple<int, int>(col, row);
                        }
                    }
                }
                else
                {
                    // 10 columns
                    for (int col = 6; col >= 0; col--)
                    {
                        // 6 rows
                        for (int row = stashRows - 1; row >= 0; row--)
                        {
                            if (InventoryManager.ItemInLocation(InventorySlot.SharedStash, col, row))
                                continue;

                            // Slot is blocked, skip
                            if (stashSlotBlocked[col, row])
                                continue;

                            // Slot is used for sorting!
                            if (_usedGrid[col, row])
                                continue;

                            if (isTwoSquare)
                            {
                                // Is a Two Slot, Can't check for 2 slot items on last row
                                if (row == 5)
                                    continue;

                                // Is a Two Slot, check row below
                                if (stashSlotBlocked[col, row + 1])
                                    continue;

                                // Slot is used for sorting!
                                if (_usedGrid[col, row + 1])
                                    continue;
                            }

                            return new Tuple<int, int>(col, row);
                        }
                    }
                }

                return new Tuple<int, int>(-1, -1);
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Error finding stash square: " + ex);
                return new Tuple<int, int>(-1, -1);
            }

        }

        private static Tuple<int, int> _lastBackPackLocation = new Tuple<int, int>(-2, -2);

        /// <summary>
        /// Search backpack to see if we have room for a 2-slot item anywhere
        /// </summary>
        /// <param name="targetCol">The target col.</param>
        /// <param name="targetRow">The target row.</param>
        /// <param name="isTwoSquare">if set to <c>true</c> [is two square].</param>
        /// <param name="isForward">if set to <c>true</c> [is forward].</param>
        /// <returns>Tuple&lt;System.Int32, System.Int32&gt;.</returns>
        internal static Tuple<int, int> FindEmptyBackpackSquare(int targetCol, int targetRow, bool isTwoSquare, bool isForward)
        {
            try
            {
                bool[,] backpackSlotBlocked = new bool[10, 6];

                // Block off the entire of any "protected bag slots"
                foreach (InventorySquare square in CharacterSettings.Instance.ProtectedBagSlots)
                {
                    backpackSlotBlocked[square.Column, square.Row] = true;
                    Core.Logger.Verbose("Slot {0},{1} is protected", square.Column, square.Row);
                }

                if (targetRow != -1 && targetCol != -1)
                {
                    backpackSlotBlocked[targetCol, targetRow] = true;
                    if (isTwoSquare)
                        backpackSlotBlocked[targetCol, targetRow + 1] = true;
                }

                // Map out all the items already in the backpack
                foreach (ACDItem item in InventoryManager.Backpack)
                {
                    if (!item.IsValid)
                    {
                        Core.Logger.Verbose("Found invalid item while trying to find backback slot!");
                        continue;
                    }

                    int col = item.InventoryColumn;
                    int row = item.InventoryRow;

                    if (row < 0 || row > 5)
                    {
                        Core.Logger.Error("Item {0} ({1}) is reporting invalid backpack row of {2}!",
                            item.Name, item.InternalName, item.InventoryRow);
                        continue;
                    }

                    if (col < 0 || col > 9)
                    {
                        Core.Logger.Error("Item {0} ({1}) is reporting invalid backpack column of {2}!",
                            item.Name, item.InternalName, item.InventoryColumn);
                        continue;
                    }

                    backpackSlotBlocked[col, row] = true;

                    if (!item.IsTwoSquareItem)
                        continue;

                    backpackSlotBlocked[col, row + 1] = true;
                }

                if (isForward)
                {
                    // 10 columns
                    for (int col = 0; col <= 9; col++)
                    {
                        // 6 rows
                        for (int row = 0; row <= 5; row++)
                        {
                            if (InventoryManager.ItemInLocation(InventorySlot.BackpackItems, col, row))
                                continue;

                            // Slot is blocked, skip
                            if (backpackSlotBlocked[col, row])
                                continue;

                            // Slot is used for sorting!
                            if (_usedGrid[col, row])
                                continue;

                            if (isTwoSquare)
                            {
                                // Is a Two Slot, Can't check for 2 slot items on last row
                                if (row == 5)
                                    continue;

                                // Is a Two Slot, check row below
                                if (backpackSlotBlocked[col, row + 1])
                                    continue;

                                // Slot is used for sorting!
                                if (_usedGrid[col, row + 1])
                                    continue;
                            }
                            _lastBackPackLocation = new Tuple<int, int>(col, row);
                            return _lastBackPackLocation;
                        }
                    }
                }
                else
                {
                    // 10 columns
                    for (int col = 9; col >= 0; col--)
                    {
                        // 6 rows
                        for (int row = 5; row >= 0; row--)
                        {
                            if (InventoryManager.ItemInLocation(InventorySlot.BackpackItems, col, row))
                                continue;

                            // Slot is blocked, skip
                            if (backpackSlotBlocked[col, row])
                                continue;

                            // Slot is used for sorting!
                            if (_usedGrid[col, row])
                                continue;

                            if (isTwoSquare)
                            {
                                // Is a Two Slot, Can't check for 2 slot items on last row
                                if (row == 5)
                                    continue;

                                // Is a Two Slot, check row below
                                if (backpackSlotBlocked[col, row + 1])
                                    continue;

                                // Slot is used for sorting!
                                if (_usedGrid[col, row + 1])
                                    continue;
                            }

                            _lastBackPackLocation = new Tuple<int, int>(col, row);
                            return _lastBackPackLocation;
                        }
                    }
                }

                // no free slot
                Core.Logger.Verbose("No Free slots!");
                _lastBackPackLocation = new Tuple<int, int>(-1, -1);
                return _lastBackPackLocation;
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Exception while finding backpack slot: " + ex);
                return new Tuple<int, int>(-1, -1);
            }
        }

    }

    public class ItemWrapper : IComparable<ItemWrapper>, IEquatable<ItemWrapper>
    {
        public class ItemStatsData
        {
            public float WeaponMinDamage { get; set; }
            public float WeaponMaxDamage { get; set; }
            public float WeaponDamagePerSecond { get; set; }
            public float MinDamage { get; set; }
            public float MaxDamage { get; set; }
            public int Sockets { get; set; }
        }

        public class ItemStatsDataFactory
        {
            internal static ItemStatsData GetItemStatsDataFromStats(ItemStats stats)
            {
                if (!stats.Item.IsValid)
                    return default(ItemStatsData);

                ItemStatsData itemStatsData = new ItemStatsData()
                {
                    Sockets = stats.Sockets,
                    WeaponMinDamage = stats.WeaponMinDamage,
                    WeaponMaxDamage = stats.WeaponMaxDamage,
                    WeaponDamagePerSecond = stats.WeaponDamagePerSecond,
                };
                return itemStatsData;
            }
        }

        public int ActorSnoId { get; set; }
        public int GameBalanceId { get; set; }
        public int DynamicId { get; set; }
        public int AcdId { get; set; }
        public int RequiredLevel { get; set; }
        public InventorySlot InventorySlot { get; set; }
        public InventorySlot[] ValidInventorySlots { get; set; }
        public bool IsUnidentified { get; set; }
        public bool IsTwoHand { get; set; }
        public bool IsOneHand { get; set; }
        public string Name { get; set; }
        public string InternalName { get; set; }
        public ItemType ItemType { get; set; }
        public ItemBaseType ItemBaseType { get; set; }
        public bool HasSingleUseSlot { get; set; }
        public bool IsShield { get; set; }
        public bool IsOffHand { get; set; }
        public bool IsWeapon { get; set; }
        public bool IsJewelry { get; set; }
        public bool IsArmor { get; set; }
        public bool IsEquipment { get; set; }
        public bool IsMisc { get; set; }
        public bool IsGem { get; set; }
        public bool IsTwoSquareItem { get; set; }
        public bool IsPotion { get; set; }
        public ItemQuality ItemQualityLevel { get; set; }
        public GemQuality GemQuality { get; set; }
        public int TieredLootRunKeyLevel { get; set; }
        public long ItemStackQuantity { get; set; }
        public bool IsSetItem { get; set; }
        public string ItemSetName { get; set; }

        public ACDItem Item { get; private set; }
        public ItemStats Stats { get; private set; }
        public ItemStatsData StatsData { get; private set; }

        public ItemWrapper(ACDItem item)
        {
            try
            {
                ActorSnoId = item.ActorSnoId;
                GameBalanceId = item.GameBalanceId;
                DynamicId = item.AnnId;
                AcdId = item.ACDId;
                InventorySlot = item.InventorySlot;
                ValidInventorySlots = item.ValidInventorySlots;
                RequiredLevel = item.RequiredLevel;
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Error wrapping non-attribute properties on item {0}: " + ex);
            }
            try
            {
                Name = item.Name;
                IsUnidentified = item.IsUnidentified;
                IsTwoHand = item.IsTwoHand;
                IsOneHand = item.IsOneHand;
                InternalName = item.InternalName;
                ItemType = item.ItemType;
                ItemBaseType = item.ItemBaseType;
                IsShield = TypeConversions.ShieldTypes.Contains(ItemType);
                IsOffHand = TypeConversions.OffHandTypes.Contains(ItemType);
                IsArmor = TypeConversions.ArmorTypes.Contains(ItemType);
                IsJewelry = TypeConversions.JewleryTypes.Contains(ItemType);
                IsWeapon = TypeConversions.WeaponTypes.Contains(ItemType);
                IsEquipment = TypeConversions.EquipmentTypes.Contains(item.ItemBaseType);
                IsTwoSquareItem = TypeConversions.GetIsTwoSlot(item.ItemBaseType, item.ItemType); /*Adding itemType exception into consideration -Seq*/
                IsMisc = TypeConversions.MiscTypes.Contains(ItemType);
                IsGem = item.ItemBaseType == ItemBaseType.Gem;
                IsPotion = item.IsPotion;
                ItemQualityLevel = item.ItemQualityLevel;
                GemQuality = item.GemQuality;
                TieredLootRunKeyLevel = item.TieredLootRunKeyLevel;
                ItemStackQuantity = item.ItemStackQuantity;
                IsSetItem = item.IsSetItem();
                ItemSetName = item.ItemSetName();
                HasSingleUseSlot = IsSingleSlotItem();
                Item = item;
                Stats = item.Stats;
                StatsData = ItemStatsDataFactory.GetItemStatsDataFromStats(Stats);

            }
            catch (Exception ex)
            {
                Core.Logger.Error("Error wrapping item {0}: " + ex);
            }
        }

        public bool IsSingleSlotItem()
        {
            if (IsOneHand)
                return false;

            if (ItemType == ItemType.Ring)
                return false;

            return true;
        }

        public int CompareTo(ItemWrapper other)
        {
            return this.Compare(other);
        }

        public bool Equals(ItemWrapper other)
        {
            if (other == null)
                return false;

            if (DynamicId == other.DynamicId)
                return true;

            return ItemType == other.ItemType &&
                ItemBaseType == other.ItemBaseType &&
                Item.ItemQualityLevel == other.Item.ItemQualityLevel &&
                Item.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Item.GetHashCode();
        }

        public static bool operator ==(ItemWrapper a, ItemWrapper b)
        {
            if (ReferenceEquals(a, null))
                return false;
            if (ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            var b = obj as ItemWrapper;
            if (b == null)
            {
                return false;
            }
            if (ReferenceEquals(this, b))
            {
                return true;
            }
            return ItemType == b.ItemType && ItemBaseType == b.ItemBaseType &&
                Item.ItemQualityLevel == b.Item.ItemQualityLevel &&
                Item.Name == b.Name;
        }

        public static bool operator !=(ItemWrapper item, ItemWrapper other)
        {
            return !(item == other);
        }

        public static bool operator <(ItemWrapper item, ItemWrapper other)
        {
            return item.Compare(other) < 0;
        }

        public static bool operator >(ItemWrapper item, ItemWrapper other)
        {
            return item.CompareTo(other) > 0;
        }
        public static bool operator >=(ItemWrapper item, ItemWrapper other)
        {
            return item.CompareTo(other) >= 0;
        }

        public static bool operator <=(ItemWrapper item, ItemWrapper other)
        {
            return item.CompareTo(other) <= 0;
        }
    }
}