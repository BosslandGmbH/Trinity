using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Coroutines.Town;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects.Enums;
using Trinity.Items;
using Trinity.Reference;
using Zeta.Game;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Coroutines
{
    public class OpenTreasureBags
    {
        public static async Task<bool> Execute()
        {
            var bagsOpened = 0;
            if (Core.Player.IsInTown)
            {
                foreach (var item in Core.Inventory.Backpack.ToList())
                {
                    if (item.RawItemType == RawItemType.TreasureBag)
                    {
                        Logger.Log($"Opening Treasure Bag {bagsOpened+1}, Id={item.AnnId}");
                        ZetaDia.Me.Inventory.UseItem(item.AnnId);
                        bagsOpened++;
                        await Coroutine.Sleep(500);
                    }
                }
                if (bagsOpened > 0)
                {
                    Logger.Log($"Waiting for Treasure Bag loot");
                    await Coroutine.Sleep(2500);
                    TrinityTownRun.IsWantingTownRun = true;
                    return true;
                }
            }
            return false;
        }
    }


}




