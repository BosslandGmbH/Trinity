using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using System.Threading.Tasks;
using Buddy.Coroutines;
using GreyMagic;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Trinity.Framework.Helpers;
using Zeta.Bot;
using Zeta.Game;

namespace Trinity.Components.Coroutines.Town
{
    public static class DropItems
    {
        private static readonly Dictionary<int, bool> Cache = new Dictionary<int, bool>();
        public static HashSet<int> DroppedItemAnnIds = new HashSet<int>();

        static DropItems()
        {
            GameEvents.OnWorldChanged += (sender, args) => Cache.Clear();
            BotMain.OnStop += bot => Cache.Clear();
        }

        public static bool ShouldDrop(TrinityItem i)
        {
            if (Cache.ContainsKey(i.AnnId))
                return Cache[i.AnnId];

            if (i.IsProtected())
                return false;

            var action = ItemEvaluationType.Keep;
            if (StashItems.ShouldStash(i))
                action = ItemEvaluationType.Keep;
            else if (SellItems.ShouldSell(i))
                action = ItemEvaluationType.Sell;
            else if (SalvageItems.ShouldSalvage(i))
                action = ItemEvaluationType.Salvage;

            var decision = Combat.TrinityCombat.Loot.ShouldDrop(i, action) && !i.IsAccountBound;
            Cache.Add(i.AnnId, decision);
            return decision;
        }

        public static async Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Core.Logger.Verbose("[丢弃物品] 需要在城里丢弃物品");
                return false;
            }

            var itemsToDrop = Core.Inventory.Backpack.Where(ShouldDrop).ToList();
            if (!itemsToDrop.Any())
            {
                Core.Logger.Verbose($"[丢弃物品] 没有什么可以丢弃");
                return false;
            }

            var dropCount = 0;
            foreach (var item in itemsToDrop)
            {
                if (await Drop(item))
                    dropCount++;
            }

            await Coroutine.Sleep(2000);
            Core.Logger.Log($"[丢弃物品] 扔下物品 {dropCount} ");
            return false;
        }

        /// <summary>
        /// Drop item in town and record it so we can avoid picking it up again.
        /// </summary>
        public static async Task<bool> Drop(TrinityItem item)
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown || item.IsAccountBound)
                return false;

            Core.Logger.Log($"[丢弃物品] --> 丢弃 {item.Name} ({item.ActorSnoId}) 在城里. AnnId={item.AnnId} ");

            bool dropResult = false;
            try
            {
                dropResult = item.Drop();
            }
            catch (InjectionSEHException)
            {
                Core.Logger.Log($"[丢弃物品] --> 未能丢弃 {item.Name} ({item.ActorSnoId}) 在城里. AnnId={item.AnnId} ");
                DroppedItemAnnIds.Add(item.AnnId);
            }

            if (dropResult)
            {
                DroppedItemAnnIds.Add(item.AnnId);
                ItemEvents.FireItemDropped(item);
                await Coroutine.Sleep(500);
                return true;
            }

            return false;
        }
    }
}