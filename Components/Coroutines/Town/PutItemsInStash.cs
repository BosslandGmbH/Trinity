using System;
using Trinity.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Zeta.Game;
using Zeta.Game.Internals;


namespace Trinity.Components.Coroutines.Town
{
    public class PutItemsInStash
    {
        //public static async Task<bool> Execute(List<ACDItem> stashCandidates)
        //{
        //    if (!ZetaDia.IsInGame || !ZetaDia.IsInTown)
        //        return false;

        //    if (TownInfo.Stash.Distance > 3f)
        //    {
        //        await MoveToAndInteract.Execute(TownInfo.Stash);
        //    }

        //    var stash = TownInfo.Stash?.GetActor();
        //    if (stash == null)
        //    {
        //        Core.Logger.Log("[把物品放入仓库] 无法找到仓库");
        //        return false;
        //    }

        //    if (!UIElements.StashWindow.IsVisible && TownInfo.Stash.Distance <= 10f)
        //    {
        //        Core.Logger.Log("[把物品放入仓库] 仓库界面无法打开, 互动");
        //        stash.Interact();
        //    }

        //    foreach (var item in stashCandidates)
        //    {
        //        try
        //        {
        //            if (!item.IsValid || item.IsDisposed)
        //            {
        //                Core.Logger.Verbose("[PutItemsInStash] An ACDItem was invalid, unable to put it in stash.");
        //                continue;
        //            }

        //            Core.Logger.Log($"[把物品放入仓库] 添加 {item.Name} ({item.ActorSnoId}) 要存储的. 堆栈大小={item.ItemStackQuantity} AnnId={item.AnnId} 内部名={item.InternalName} Id={item.ActorSnoId} 品质={item.ItemQualityLevel} 远古等级={item.AncientRank}");
        //            InventoryManager.QuickStash(item);
        //        }
        //        catch (Exception ex)
        //        {
        //            Core.Logger.Error(ex.ToString());
        //        }
        //    }

        //    await Coroutine.Sleep(1000);
        //    Core.Logger.Log("[把物品放入仓库] 完成!");
        //    return true;
        //}

        public static async Task<bool> Execute(List<int> annIds)
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
                Core.Logger.Log("[把物品放入仓库] 无法找到仓库");
                return false;
            }

            if (!UIElements.StashWindow.IsVisible && TownInfo.Stash.Distance <= 10f)
            {
                Core.Logger.Log("[把物品放入仓库] 仓库界面无法打开, 互动");
                stash.Interact();
            }

            foreach (var item in InventoryManager.Backpack.Where(i => annIds.Contains(i.AnnId)))
            {
                try
                {
                    if (!item.IsValid || item.IsDisposed)
                    {
                        Core.Logger.Verbose("[PutItemsInStash] An ACDItem was invalid, unable to put it in stash.");
                        continue;
                    }

                    Core.Logger.Log($"[把物品放入仓库] 添加 {item.Name} ({item.ActorSnoId}) 要存储的. 堆栈大小={item.ItemStackQuantity} AnnId={item.AnnId} 内部名={item.InternalName} Id={item.ActorSnoId} 品质={item.ItemQualityLevel} 远古等级={item.AncientRank}");
                    InventoryManager.QuickStash(item);
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