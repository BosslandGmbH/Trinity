using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Actors;
using Trinity.Helpers;
using TrinityCoroutines;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Coroutines.Town
{
    public class TakeItemsFromStash
    {
        /// <summary>
        /// Moves items from the Stash to the Backpack
        /// </summary>
        /// <param name="itemIds">list of items to withdraw</param>
        /// <param name="maxAmount">amount to withdraw up to (including counts already in backpack)</param>
        /// <returns></returns>
        public static async Task<bool> Execute(IEnumerable<int> itemIds, int maxAmount)
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown)
                return true;

            if (TownInfo.Stash.Distance > 3f)
            {
                await MoveToAndInteract.Execute(TownInfo.Stash);
            }

            var stash = TownInfo.Stash?.GetActor();
            if (stash == null)
            {
                Logger.Log("[TakeItemsFromStash] Unable to find Stash");
                return false;
            }

            if (!UIElements.StashWindow.IsVisible && TownInfo.Stash.Distance <= 10f)
            {                
                Logger.Log("[TakeItemsFromStash] Stash window not open, interacting");
                stash.Interact();
            }
                
            var itemIdsHashSet = new HashSet<int>(itemIds);
            var amountWithdrawn = itemIdsHashSet.ToDictionary(k => k, v => (long)0);
            var overageTaken = itemIdsHashSet.ToDictionary(k => k, v => false);
            var lastStackTaken = itemIdsHashSet.ToDictionary(k => k, v => default(ACDItem));

            foreach (var item in ZetaDia.Me.Inventory.Backpack.Where(i => i.ACDId != 0 && i.IsValid && itemIdsHashSet.Contains(i.ActorSnoId)).ToList())
            {
                amountWithdrawn[item.ActorSnoId] += item.ItemStackQuantity;
                lastStackTaken[item.ActorSnoId] = item;
            }
            
            foreach (var item in ZetaDia.Me.Inventory.StashItems.Where(i => i.ACDId != 0 && i.IsValid && itemIdsHashSet.Contains(i.ActorSnoId)).ToList())
            {
                try
                {
                    if (!item.IsValid || item.IsDisposed)
                        continue;

                    var stackSize = item.ItemStackQuantity;
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
                    if (!willBeOverMax || !overageTaken[item.ActorSnoId])
                    {
                        var lastItem = lastStackTaken[item.ActorSnoId];
                        var amountRequiredToMax = maxAmount - numTakenAlready;

                        if (willBeOverMax && lastItem != null && lastItem.IsValid && !lastItem.IsDisposed && stackSize > amountRequiredToMax)
                        {
                            // Tried InventoryManager.SplitStack but it didnt work, reverting to moving onto existing stacks.

                            var amountToSplit = stackSize - lastItem.ItemStackQuantity;                        
                            Logger.Log($"[TakeItemsFromStash] Merging Stash Stack {item.Name} ({item.ActorSnoId}) onto Backpack Stack. StackSize={amountToSplit} WithdrawnAlready={numTakenAlready} InternalName={item.InternalName} Id={item.ActorSnoId} Quality={item.ItemQualityLevel} AncientRank={item.AncientRank}");

                            ZetaDia.Me.Inventory.MoveItem(item.AnnId, ZetaDia.Me.CommonData.AnnId, InventorySlot.BackpackItems, lastItem.InventoryColumn, lastItem.InventoryRow);

                            amountWithdrawn[item.ActorSnoId] += amountToSplit;
                            overageTaken[item.ActorSnoId] = true;
                        }
                        else
                        {
                            if (item.IsValid && !item.IsDisposed)
                            {
                                Logger.Log($"[TakeItemsFromStash] Removing {item.Name} ({item.ActorSnoId}) from stash. StackSize={stackSize} WithdrawnAlready={numTakenAlready} InternalName={item.InternalName} Id={item.ActorSnoId} AnnId={item.AnnId} Quality={item.ItemQualityLevel} AncientRank={item.AncientRank}");
                                ZetaDia.Me.Inventory.QuickWithdraw(item);
                                amountWithdrawn[item.ActorSnoId] += stackSize;
                                lastStackTaken[item.ActorSnoId] = item;
                            }
                        }
                                                    
                        await Coroutine.Sleep(25);
                        await Coroutine.Yield();                        
                    }


                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }

            await Coroutine.Sleep(1000);
            return true;
        }

        public static async Task<bool> Execute(List<CachedItem> stashCandidates)
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown)
                return false;

            if (TownInfo.Stash.Distance > 3f)
            {
                await MoveToAndInteract.Execute(TownInfo.Stash);
            }

            var stash = TownInfo.Stash?.GetActor();
            if (stash == null)
            {
                Logger.Log("[TakeItemsFromStash] Unable to find Stash");
                return false;
            }

            if (!UIElements.StashWindow.IsVisible && TownInfo.Stash.Distance <= 10f)
            {
                Logger.Log("[TakeItemsFromStash] Stash window not open, interacting");
                stash.Interact();
            }

            foreach (var item in stashCandidates)
            {
                try
                {
                    if (!item.IsValid)
                    {
                        Logger.LogVerbose("[TakeItemsFromStash] An ACDItem was invalid, unable to remove it from stash.");
                        continue;
                    }

                    Logger.LogVerbose($"[TakeItemsFromStash] QuickWithdrawing: {item.InternalName} Id={item.ActorSnoId} AnnId={item.AnnId} Name={item.Name} Quality={item.ItemQualityLevel} IsAncient={item.IsAncient}");
                    ZetaDia.Me.Inventory.QuickWithdraw(item.GetAcdItem());                    
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }

            await Coroutine.Sleep(1000);
            return true;
        }
    }
}
