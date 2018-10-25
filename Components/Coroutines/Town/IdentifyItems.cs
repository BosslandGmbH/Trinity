using System;
using Trinity.Framework;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Game;


namespace Trinity.Components.Coroutines.Town
{
    public static class IdentifyItems
    {
        public static async Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Core.Logger.Verbose("[IdentifyItems] Need to be in town to identify items");
                return false;
            }

            if (Core.Settings.Items.KeepLegendaryUnid)
            {
                Core.Logger.Verbose("[IdentifyItems] Town run setting 'Keep Legendary Unidentified' - Skipping ID");
                return true;
            }
            if (Core.Inventory.Backpack.Any(i => i.IsUnidentified))
            {
                var bookActor = TownInfo.BookOfCain;
                if (bookActor == null)
                {
                    Core.Logger.Log($"[IdentifyItems] TownInfo.BookOfCain not found Act={ZetaDia.CurrentAct} WorldSnoId={ZetaDia.Globals.WorldSnoId}");
                    return true;
                }

                Core.Logger.Log("Identifying Items");
                if (!await CommonCoroutines.MoveAndInteract(bookActor.GetActor(), () => CommonCoroutines.IsInteracting))
                    return false;

                await Coroutine.Wait(TimeSpan.FromSeconds(10), () => !CommonCoroutines.IsInteracting);
                return false;
            }
            return true;
        }
    }
}