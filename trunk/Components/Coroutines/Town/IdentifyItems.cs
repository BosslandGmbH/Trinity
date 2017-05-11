using System;
using Trinity.Framework;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Helpers;
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
                return false;
            }

            var timeout = DateTime.UtcNow.Add(TimeSpan.FromSeconds(30));

            var bookActor = TownInfo.BookOfCain;
            if (bookActor == null)
            {
                Core.Logger.Log($"[IdentifyItems] TownInfo.BookOfCain not found Act={ZetaDia.CurrentAct} WorldSnoId={ZetaDia.Globals.WorldSnoId}");
                return false;
            }

            while (Core.Inventory.Backpack.Any(i => i.IsUnidentified))
            {
                if (DateTime.UtcNow > timeout)
                    break;

                Core.Logger.Log("Identifying Items");

                if (!Core.Grids.CanRayCast(ZetaDia.Me.Position, bookActor.Position))
                {
                    await MoveTo.Execute(TownInfo.NearestSafeSpot);
                }

                await MoveToAndInteract.Execute(bookActor);
                await Coroutine.Wait(8000, () => ZetaDia.Me.LoopingAnimationEndTime <= 0);
            }
            return false;
        }
    }
}