using System.Linq;
using Trinity.Framework;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Coroutines.Town;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Objects.Enums;
using Zeta.Bot.Logic;
using Zeta.Game;


namespace Trinity.Components.Coroutines
{
    public class OpenTreasureBags
    {
        public static async Task<bool> Execute()
        {
            if (Core.Settings.Items.StashTreasureBags)
                return false;

            var bagsOpened = 0;
            if (Core.Player.IsInTown)
            {
                foreach (var item in InventoryManager.Backpack.ToList())
                {
                    if (item.GetRawItemType() == RawItemType.TreasureBag)
                    {
                        Core.Logger.Log($"Opening Treasure Bag {bagsOpened + 1}, Id={item.AnnId}");
                        InventoryManager.UseItem(item.AnnId);
                        bagsOpened++;
                        await Coroutine.Yield();
                    }
                }
                if (bagsOpened > 0)
                {
                    Core.Logger.Log($"Waiting for Treasure Bag loot");
                    await Coroutine.Yield();
                    BrainBehavior.ForceTownrun(nameof(OpenTreasureBags));
                    return true;
                }
            }
            return false;
        }
    }
}
