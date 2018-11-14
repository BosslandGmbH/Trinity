using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Helpers;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines.Town
{
    // TODO: Make sure there is only one Salvage Routine. Might need to merge them and finetune it.
    public static partial class TrinityTownRun
    {
        public static bool ShouldSalvage(ACDItem i)
        {
            if (!i.IsValid)
                return false;

            if (BrainBehavior.GreaterRiftInProgress)
                return false;

            if (i.IsProtected())
                return false;

            if (i.IsUnidentified)
                return false;

            if (!i.GetIsSalvageable())
                return false;

            return Combat.TrinityCombat.Loot.ShouldSalvage(i) && !ShouldStash(i);
        }
        public static async Task<CoroutineResult> SalvageItems()
        {
            if (!ZetaDia.IsInTown)
                return CoroutineResult.NoAction;

            var itemsToSalvage = Core.Inventory.Backpack.Where(ShouldSalvage).ToList();
            Core.Logger.Verbose(LogCategory.ItemEvents, $"[SalvageItems] Starting salvage for {itemsToSalvage.Count} items");
            itemsToSalvage.ForEach(i => Core.Logger.Debug(LogCategory.ItemEvents, $"[SalvageItems] Salvaging: {i.Name} ({i.ActorSnoId}) InternalName={i.InternalName} Ann={i.AnnId}"));

            return await BrainBehavior.DoSalvage(itemsToSalvage);
        }
    }
}
