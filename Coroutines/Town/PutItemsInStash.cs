using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using TrinityCoroutines;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Coroutines.Town
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
        //        Logger.Log("[PutItemsInStash] Unable to find Stash");
        //        return false;
        //    }

        //    if (!UIElements.StashWindow.IsVisible && TownInfo.Stash.Distance <= 10f)
        //    {
        //        Logger.Log("[PutItemsInStash] Stash window not open, interacting");
        //        stash.Interact();
        //    }

        //    foreach (var item in stashCandidates)
        //    {
        //        try
        //        {
        //            if (!item.IsValid || item.IsDisposed)
        //            {
        //                Logger.LogVerbose("[PutItemsInStash] An ACDItem was invalid, unable to put it in stash.");
        //                continue;
        //            }

        //            Logger.Log($"[PutItemsInStash] Adding {item.Name} ({item.ActorSnoId}) to stash. StackSize={item.ItemStackQuantity} AnnId={item.AnnId} InternalName={item.InternalName} Id={item.ActorSnoId} Quality={item.ItemQualityLevel} AncientRank={item.AncientRank}");
        //            ZetaDia.Me.Inventory.QuickStash(item);                    
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.LogError(ex.ToString());
        //        }
        //    }

        //    await Coroutine.Sleep(1000);
        //    Logger.Log("[PutItemsInStash] Finished!");
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
                Logger.Log("[PutItemsInStash] Unable to find Stash");
                return false;
            }

            if (!UIElements.StashWindow.IsVisible && TownInfo.Stash.Distance <= 10f)
            {
                Logger.Log("[PutItemsInStash] Stash window not open, interacting");
                stash.Interact();
            }

            foreach (var item in ZetaDia.Me.Inventory.Backpack.Where(i => annIds.Contains(i.AnnId)))
            {
                try
                {
                    if (!item.IsValid || item.IsDisposed)
                    {
                        Logger.LogVerbose("[PutItemsInStash] An ACDItem was invalid, unable to put it in stash.");
                        continue;
                    }

                    Logger.Log($"[PutItemsInStash] Adding {item.Name} ({item.ActorSnoId}) to stash. StackSize={item.ItemStackQuantity} AnnId={item.AnnId} InternalName={item.InternalName} Id={item.ActorSnoId} Quality={item.ItemQualityLevel} AncientRank={item.AncientRank}");
                    ZetaDia.Me.Inventory.QuickStash(item);
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
