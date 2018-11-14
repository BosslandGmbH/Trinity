using Buddy.Coroutines;
using GreyMagic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Events;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines.Town
{
    public static partial class TrinityTownRun
    {
        public static HashSet<int> DroppedItemAnnIds = new HashSet<int>();

        public static bool ShouldDrop(ACDItem i)
        {
            if (i.IsProtected())
                return false;

            var action = ItemEvaluationType.Keep;
            if (ShouldStash(i))
                action = ItemEvaluationType.Keep;
            else if (ShouldSell(i))
                action = ItemEvaluationType.Sell;
            else if (ShouldSalvage(i))
                action = ItemEvaluationType.Salvage;

            return Combat.TrinityCombat.Loot.ShouldDrop(i, action) && !i.IsAccountBound;
        }

        public static async Task<CoroutineResult> DropItems()
        {
            if (!ZetaDia.IsInTown)
                return CoroutineResult.NoAction;

            var itemsToDrop = Core.Inventory.Backpack.Where(ShouldDrop).ToList();
            if (!itemsToDrop.Any())
                return CoroutineResult.NoAction;

            var dropCount = 0;
            foreach (var item in itemsToDrop)
            {
                if (await Drop(item) == CoroutineResult.Done)
                    dropCount++;
                // No need to wait here... Drop is doing a Yield when successful.
            }

            s_logger.Info($"[{nameof(DropItems)}] Dropped {dropCount} Items");
            return CoroutineResult.Done;
        }

        /// <summary>
        /// Drop item in town and record it so we can avoid picking it up again.
        /// </summary>
        public static async Task<CoroutineResult> Drop(ACDItem item)
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown || item.IsAccountBound)
                return CoroutineResult.NoAction;

            s_logger.Info($"[{nameof(Drop)}] Dropping {item.Name} ({item.ActorSnoId}) in town. AnnId={item.AnnId} ");

            bool dropResult = false;
            try
            {
                dropResult = item.Drop();
            }
            catch (InjectionSEHException)
            {
                s_logger.Info($"[{nameof(DropItems)}] Failed to Drop {item.Name} ({item.ActorSnoId}) in town. AnnId={item.AnnId} ");
                DroppedItemAnnIds.Add(item.AnnId);
            }

            if (dropResult)
            {
                DroppedItemAnnIds.Add(item.AnnId);
                ItemEvents.FireItemDropped(item);
                await Coroutine.Yield();
                return CoroutineResult.Done;
            }

            return CoroutineResult.Failed;
        }
    }
}
