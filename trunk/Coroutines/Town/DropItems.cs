using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using Buddy.Coroutines;
using GreyMagic;
using IronPython.Modules;
using Trinity.Components.Combat;
using Trinity.Coroutines.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Items;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Coroutines.Town
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

            var action = ItemEvaluationType.Keep;
            if (StashItems.ShouldStash(i))
                action = ItemEvaluationType.Keep;
            else if (SellItems.ShouldSell(i))
                action = ItemEvaluationType.Sell;
            else if (SalvageItems.ShouldSalvage(i))
                action = ItemEvaluationType.Salvage;

            var decision = Combat.Loot.ShouldDrop(i, action) && !i.IsAccountBound;
            Cache.Add(i.AnnId, decision);
            return decision;
        }

        public async static Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Logger.LogVerbose("[DropItems] Need to be in town to drop items");
                return false;
            }

            var itemsToDrop = Inventory.Backpack.Items.Where(ShouldDrop).ToList();
            if (!itemsToDrop.Any())
            {
                Logger.LogVerbose($"[DropItems] Nothign to Drop");
                return false;
            }

            var dropCount = 0;
            foreach (var item in itemsToDrop)
            { 
                if(await Drop(item))
                    dropCount++;               
            }

            await Coroutine.Sleep(2000);
            Logger.Log($"[DropItems] Dropped {dropCount} Items");
            return false;
        }

        /// <summary>
        /// Drop item in town and record it so we can avoid picking it up again.
        /// </summary>
        public static async Task<bool> Drop(TrinityItem item)
        {
            if (!ZetaDia.IsInGame || !ZetaDia.IsInTown || item.IsAccountBound)
                return false;

            Logger.Log($"[DropItems] --> Dropping {item.Name} ({item.ActorSnoId}) in town. AnnId={item.AnnId} ");

            bool dropResult = false;
            try
            {
                dropResult = item.Drop();
            }
            catch (InjectionSEHException)
            {
                Logger.Log($"[DropItems] --> Failed to Drop {item.Name} ({item.ActorSnoId}) in town. AnnId={item.AnnId} ");
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


