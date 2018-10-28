using System;
using Trinity.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Combat;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;


namespace Trinity.Components.Coroutines.Town
{
    // TODO: Clean this mess up!
    public static partial class TrinityTownRun
    {
        /// <summary>
        /// Moves items from the Stash to the Backpack
        /// </summary>
        /// <param name="itemIds">list of items to withdraw</param>
        /// <param name="maxAmount">amount to withdraw up to (including counts already in backpack)</param>
        /// <returns></returns>
        public static async Task<bool> TakeItemsFromStash(IEnumerable<int> itemIds, int maxAmount)
        {
            if (!ZetaDia.IsInGame ||
                !ZetaDia.IsInTown)
                return true;

            while (await CommonCoroutines.MoveAndStop(
                       TownInfo.Stash.InteractPosition,
                       TownInfo.Stash.GetActor().InteractDistance,
                       "Stash") != MoveResult.ReachedDestination)
            {
                await Coroutine.Yield();
            }

            var stash = TownInfo.Stash?.GetActor();
            if (stash == null)
            {
                Core.Logger.Log("[TakeItemsFromStash] Unable to find Stash");
                return false;
            }

            if (!UIElements.StashWindow.IsVisible &&
                TownInfo.Stash.Distance <= 10f)
                await MoveToAndInteract.Execute(TownInfo.Stash);

            if (!UIElements.StashWindow.IsVisible && TownInfo.Stash.Distance <= 10f)
            {
                Core.Logger.Log("[TakeItemsFromStash] Stash window not open, interacting");
                stash.Interact();
            }

            var itemIdsHashSet = new HashSet<int>(itemIds);
            var amountWithdrawn = itemIdsHashSet.ToDictionary(k => k, v => (long)0);
            var overageTaken = itemIdsHashSet.ToDictionary(k => k, v => false);
            var lastStackTaken = itemIdsHashSet.ToDictionary(k => k, v => default(ACDItem));

            foreach (var item in InventoryManager.Backpack
                .Where(i => i.ACDId != 0 &&
                            i.IsValid &&
                            itemIdsHashSet.Contains(i.ActorSnoId))
                .ToList())
            {
                amountWithdrawn[item.ActorSnoId] += item.ItemStackQuantity;
                lastStackTaken[item.ActorSnoId] = item;
            }

            foreach (var item in InventoryManager.StashItems
                .Where(i => i.ACDId != 0 &&
                            i.IsValid &&
                            itemIdsHashSet.Contains(i.ActorSnoId))
                .ToList())
            {
                try
                {
                    if (!item.IsValid ||
                        item.IsDisposed)
                        continue;

                    var stackSize = Math.Max(1, item.ItemStackQuantity);
                    var numTakenAlready = amountWithdrawn[item.ActorSnoId];

                    // We have enough of this material already
                    var alreadyTakenEnough = numTakenAlready >= maxAmount;
                    if (alreadyTakenEnough)
                        continue;

                    // We have enough of everything already.
                    if (amountWithdrawn.All(i => i.Value >= maxAmount))
                        break;

                    // Only take up to the required amount.
                    var willBeOverMax = numTakenAlready + stackSize > maxAmount;
                    if (willBeOverMax &&
                        overageTaken[item.ActorSnoId])
                        continue;

                    var lastItem = lastStackTaken[item.ActorSnoId];
                    var amountRequiredToMax = maxAmount - numTakenAlready;

                    if (willBeOverMax &&
                        lastItem != null &&
                        lastItem.IsValid &&
                        !lastItem.IsDisposed &&
                        stackSize > amountRequiredToMax)
                    {
                        // Tried InventoryManager.SplitStack but it didnt work, reverting to moving onto existing stacks.

                        var amountToSplit = stackSize - lastItem.ItemStackQuantity;
                        Core.Logger.Log($"[TakeItemsFromStash] Merging Stash Stack {item.Name} ({item.ActorSnoId}) onto Backpack Stack. StackSize={amountToSplit} WithdrawnAlready={numTakenAlready} InternalName={item.InternalName} Id={item.ActorSnoId} Quality={item.ItemQualityLevel} AncientRank={item.AncientRank}");

                        InventoryManager.MoveItem(
                            item.AnnId,
                            ZetaDia.Me.CommonData.AnnId,
                            InventorySlot.BackpackItems,
                            lastItem.InventoryColumn,
                            lastItem.InventoryRow);

                        amountWithdrawn[item.ActorSnoId] += amountToSplit;
                        overageTaken[item.ActorSnoId] = true;
                    }
                    else
                    {
                        if (item.IsValid &&
                            !item.IsDisposed)
                        {
                            Core.Logger.Log($"[TakeItemsFromStash] Removing {item.Name} ({item.ActorSnoId}) from stash. StackSize={stackSize} WithdrawnAlready={numTakenAlready} InternalName={item.InternalName} Id={item.ActorSnoId} AnnId={item.AnnId} Quality={item.ItemQualityLevel} AncientRank={item.AncientRank}");

                            // Quick withdraw broken?
                            //InventoryManager.QuickWithdraw(item);

                            var newPosition = TrinityCombat.Loot.FindBackpackSlot(item.IsTwoSquareItem);
                            InventoryManager.MoveItem(
                                item.AnnId,
                                ZetaDia.Me.CommonData.AnnId,
                                InventorySlot.BackpackItems,
                                (int)newPosition.X,
                                (int)newPosition.Y);

                            amountWithdrawn[item.ActorSnoId] += stackSize;
                            lastStackTaken[item.ActorSnoId] = item;
                        }
                    }
                    await Coroutine.Yield();
                }
                catch (Exception ex)
                {
                    Core.Logger.Error(ex.ToString());
                }
            }

            await Coroutine.Yield();
            return true;
        }

        public static async Task<bool> TakeItemsFromStash(List<TrinityItem> stashCandidates)
        {
            if (!ZetaDia.IsInGame ||
                !ZetaDia.IsInTown)
                return false;

            if (TownInfo.Stash.Distance > 3f)
            {
                await MoveToAndInteract.Execute(TownInfo.Stash);
            }

            var stash = TownInfo.Stash?.GetActor();
            if (stash == null)
            {
                Core.Logger.Log("[TakeItemsFromStash] Unable to find Stash");
                return false;
            }

            if (!UIElements.StashWindow.IsVisible &&
                TownInfo.Stash.Distance <= 10f)
            {
                Core.Logger.Log("[TakeItemsFromStash] Stash window not open, interacting");
                stash.Interact();
            }

            foreach (var item in stashCandidates)
            {
                try
                {
                    if (!item.IsValid)
                    {
                        Core.Logger.Verbose("[TakeItemsFromStash] An ACDItem was invalid, unable to remove it from stash.");
                        continue;
                    }

                    Core.Logger.Verbose($"[TakeItemsFromStash] QuickWithdrawing: {item.InternalName} Id={item.ActorSnoId} AnnId={item.AnnId} Name={item.Name} Quality={item.ItemQualityLevel} IsAncient={item.IsAncient}");
                    InventoryManager.QuickWithdraw(item.ToAcdItem());
                }
                catch (Exception ex)
                {
                    Core.Logger.Error(ex.ToString());
                }
            }

            await Coroutine.Yield();
            return true;
        }
    }
}