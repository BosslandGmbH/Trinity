using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Zeta.Bot;
using Zeta.Bot.Logic;
using Zeta.Game;

namespace Trinity.Components.Coroutines.Town
{
    // TODO: Make sure there is only one Salvage Routine. Might need to merge them and finetune it.
    public static class SalvageItems
    {
        static SalvageItems()
        {
            GameEvents.OnWorldChanged += (sender, args) => s_cache.Clear();
            BotMain.OnStop += bot => s_cache.Clear();
        }

        private static readonly Dictionary<int, bool> s_cache = new Dictionary<int, bool>();

        public static bool ShouldSalvage(TrinityItem i)
        {
            if (!i.IsValid)
                return false;

            if (BrainBehavior.GreaterRiftInProgress)
                return false;

            if (s_cache.ContainsKey(i.AnnId))
                return s_cache[i.AnnId];

            if (i.IsProtected())
                return false;

            if (i.IsUnidentified)
                return false;

            if (!i.IsSalvageable)
                return false;

            var decision = Combat.TrinityCombat.Loot.ShouldSalvage(i) && !StashItems.ShouldStash(i);
            s_cache.Add(i.AnnId, decision);
            return decision;
        }

        public static async Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
                return false;

            var itemsToSalvage = Core.Inventory.Backpack.Where(ShouldSalvage).Select(item => item.ToAcdItem()).ToList();
            Core.Logger.Verbose(LogCategory.ItemEvents, $"[SalvageItems] Starting salvage for {itemsToSalvage.Count} items");
            itemsToSalvage.ForEach(i => Core.Logger.Debug(LogCategory.ItemEvents, $"[SalvageItems] Salvaging: {i.Name} ({i.ActorSnoId}) InternalName={i.InternalName} Ann={i.AnnId}"));

            if (!await BrainBehavior.DoSalvage(itemsToSalvage))
                return false;

            Core.Actors.Update();

            return true;
        }
    }
}