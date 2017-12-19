using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Combat;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;


namespace Trinity.Components.Coroutines.Town
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

            if (TownInfo.Stash.Distance > 20f)
            {
                await MoveTo.Execute(TownInfo.Stash.InteractPosition);
            }

            var stash = TownInfo.Stash?.GetActor();
            if (stash == null)
            {
                Core.Logger.Log("[从仓库拿取道具] 无法找到仓库");
                return false;
            }

            if (!UIElements.StashWindow.IsVisible && TownInfo.Stash.Distance <= 10f)
            {
                await MoveToAndInteract.Execute(TownInfo.Stash);
            }

            if (!UIElements.StashWindow.IsVisible && TownInfo.Stash.Distance <= 10f)
            {
                Core.Logger.Log("[从仓库拿取道具] 仓库界面无法打开, 互动");
                stash.Interact();
            }

            var itemIdsHashSet = new HashSet<int>(itemIds);
            var amountWithdrawn = itemIdsHashSet.ToDictionary(k => k, v => (long)0);
            var overageTaken = itemIdsHashSet.ToDictionary(k => k, v => false);
            var lastStackTaken = itemIdsHashSet.ToDictionary(k => k, v => default(ACDItem));

            foreach (var item in InventoryManager.Backpack.Where(i => i.ACDId != 0 && i.IsValid && itemIdsHashSet.Contains(i.ActorSnoId)).ToList())
            {
                amountWithdrawn[item.ActorSnoId] += item.ItemStackQuantity;
                lastStackTaken[item.ActorSnoId] = item;
            }

            foreach (var item in InventoryManager.StashItems.Where(i => i.ACDId != 0 && i.IsValid && itemIdsHashSet.Contains(i.ActorSnoId)).ToList())
            {
                try
                {
                    if (!item.IsValid || item.IsDisposed)
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
                    if (!willBeOverMax || !overageTaken[item.ActorSnoId])
                    {
                        var lastItem = lastStackTaken[item.ActorSnoId];
                        var amountRequiredToMax = maxAmount - numTakenAlready;

                        if (willBeOverMax && lastItem != null && lastItem.IsValid && !lastItem.IsDisposed && stackSize > amountRequiredToMax)
                        {
                            // Tried InventoryManager.SplitStack but it didnt work, reverting to moving onto existing stacks.

                            var amountToSplit = stackSize - lastItem.ItemStackQuantity;
                            Core.Logger.Log($"[从仓库拿取道具] 合并仓库内 {item.Name} ({item.ActorSnoId}) 到背包. 堆叠道具大小={amountToSplit} 已撤销={numTakenAlready} 内部名称={item.InternalName} Id={item.ActorSnoId} 品质={item.ItemQualityLevel} 远古等级={item.AncientRank}");

                            InventoryManager.MoveItem(item.AnnId, ZetaDia.Me.CommonData.AnnId, InventorySlot.BackpackItems, lastItem.InventoryColumn, lastItem.InventoryRow);

                            amountWithdrawn[item.ActorSnoId] += amountToSplit;
                            overageTaken[item.ActorSnoId] = true;
                        }
                        else
                        {
                            if (item.IsValid && !item.IsDisposed)
                            {
                                Core.Logger.Log($"[从仓库拿取道具] 丢弃 {item.Name} ({item.ActorSnoId}) 从仓库中. 堆叠道具大小={stackSize} 已撤销={numTakenAlready} 内部名称={item.InternalName} Id={item.ActorSnoId} AnnId={item.AnnId} 品质={item.ItemQualityLevel} 远古等级={item.AncientRank}");

                                // Quick withdraw broken?
                                //InventoryManager.QuickWithdraw(item);

                                var newPosition = TrinityCombat.Loot.FindBackpackSlot(item.IsTwoSquareItem);
                                InventoryManager.MoveItem(item.AnnId, ZetaDia.Me.CommonData.AnnId, InventorySlot.BackpackItems, (int)newPosition.X, (int)newPosition.Y);
                                
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
                    Core.Logger.Error(ex.ToString());
                }
            }

            await Coroutine.Sleep(1000);
            return true;
        }

        public static async Task<bool> Execute(List<TrinityItem> stashCandidates)
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
                Core.Logger.Log("[从仓库拿取道具] 无法找到仓库");
                return false;
            }

            if (!UIElements.StashWindow.IsVisible && TownInfo.Stash.Distance <= 10f)
            {
                Core.Logger.Log("[从仓库拿取道具] 仓库界面无法打开, 互动");
                stash.Interact();
            }

            foreach (var item in stashCandidates)
            {
                try
                {
                    if (!item.IsValid)
                    {
                        Core.Logger.Verbose("[从仓库拿取道具] An ACDItem 是无效的, 无法将其从仓库取出.");
                        continue;
                    }

                    Core.Logger.Verbose($"[从仓库拿取道具] 快速提取: {item.InternalName} Id={item.ActorSnoId} AnnId={item.AnnId} 名称={item.Name} 品质={item.ItemQualityLevel} 远古={item.IsAncient}");
                    InventoryManager.QuickWithdraw(item.ToAcdItem());
                }
                catch (Exception ex)
                {
                    Core.Logger.Error(ex.ToString());
                }
            }

            await Coroutine.Sleep(1000);
            return true;
        }
    }
}