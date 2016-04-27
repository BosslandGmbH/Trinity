using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Technicals;
using TrinityCoroutines;
using TrinityCoroutines.Resources;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Coroutines.Town
{
    public static class IdentifyItems
    {
        public static async Task<bool> Execute()        
        {
            if (!ZetaDia.IsInTown)
            {
                Logger.LogVerbose("[IdentifyItems] Need to be in town to identify items");
                return false;
            }

            if (TrinityPlugin.Settings.Loot.TownRun.KeepLegendaryUnid)
            {
                Logger.LogVerbose("[IdentifyItems] Town run setting 'Keep Legendary Unidentified' - Skipping ID");
                return false;
            }

            var timeout = DateTime.UtcNow.Add(TimeSpan.FromSeconds(30));

            while(Inventory.Backpack.Items.Any(i => i.IsUnidentified))
            {
                if (DateTime.UtcNow > timeout)
                    break;

                Logger.Log("Identifying Items");

                if (!NavHelper.CanRayCast(ZetaDia.Me.Position, TownInfo.BookOfCain.Position))
                {
                    await MoveTo.Execute(TownInfo.NearestSafeSpot);
                }

                await MoveToAndInteract.Execute(TownInfo.BookOfCain);
                await Coroutine.Wait(8000, () => ZetaDia.Me.LoopingAnimationEndTime <= 0);
            }
            return false;
        }
    }
}
