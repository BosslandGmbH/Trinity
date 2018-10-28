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
    public static partial class TrinityTownRun
    {
        public static bool ShouldSalvage(TrinityItem i)
        {
            if (!i.IsValid)
                return false;

            if (BrainBehavior.GreaterRiftInProgress)
                return false;
            
            if (i.IsProtected())
                return false;

            if (i.IsUnidentified)
                return false;

            if (!i.IsSalvageable)
                return false;

            return Combat.TrinityCombat.Loot.ShouldSalvage(i) && !ShouldStash(i);
        }
        public static async Task<bool> SalvageItems()
        {
            if (!ZetaDia.IsInTown)
                return true;

            var itemsToSalvage = Core.Inventory.Backpack.Where(ShouldSalvage).Select(item => item.ToAcdItem()).ToList();
            Core.Logger.Verbose(LogCategory.ItemEvents, $"[SalvageItems] Starting salvage for {itemsToSalvage.Count} items");
            itemsToSalvage.ForEach(i => Core.Logger.Debug(LogCategory.ItemEvents, $"[SalvageItems] Salvaging: {i.Name} ({i.ActorSnoId}) InternalName={i.InternalName} Ann={i.AnnId}"));

            if (!await BrainBehavior.DoSalvage(itemsToSalvage))
                return false;

            return true;
        }
    }
}