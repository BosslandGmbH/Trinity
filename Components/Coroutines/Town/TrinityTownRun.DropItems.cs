using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using System.Threading.Tasks;
using Buddy.Coroutines;
using GreyMagic;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Zeta.Bot;
using Zeta.Game;

namespace Trinity.Components.Coroutines.Town
{
    public static partial class TrinityTownRun
    {
        public static HashSet<int> DroppedItemAnnIds = new HashSet<int>();
        
        public static bool ShouldDrop(TrinityItem i)
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

        public static async Task<bool> DropItems()
        {
            if (!ZetaDia.IsInTown) return true;

            var itemsToDrop = Core.Inventory.Backpack.Where(ShouldDrop).ToList();
            if (!itemsToDrop.Any()) return true;

            var dropCount = 0;
            foreach (var item in itemsToDrop)
            {
                if (await Drop(item))
                    dropCount++;
            }

            s_logger.Info($"[{nameof(DropItems)}] Dropped {dropCount} Items");
            return true;
        }

        /// <summary>
        /// Drop item in town and record it so we can avoid picking it up again.
        /// </summary>
        public static async Task<bool> Drop(TrinityItem item)
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown || item.IsAccountBound)
                return false;

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
                return true;
            }

            return false;
        }
    }
}
