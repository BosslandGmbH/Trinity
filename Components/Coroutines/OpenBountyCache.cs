using System.Linq;
using Trinity.Framework;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Coroutines.Town;
using Trinity.Framework.Objects.Enums;
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
                foreach (var item in Core.Inventory.Backpack.ToList())
                {
                    if (item.RawItemType == RawItemType.TreasureBag)
                    {
                        Core.Logger.Log($"打开背包 {bagsOpened + 1}, Id={item.AnnId}");
                        InventoryManager.UseItem(item.AnnId);
                        bagsOpened++;
                        await Coroutine.Sleep(500);
                    }
                }
                if (bagsOpened > 0)
                {
                    Core.Logger.Log($"等待背包战利品");
                    await Coroutine.Sleep(2500);
                    TrinityTownRun.IsWantingTownRun = true;
                    return true;
                }
            }
            return false;
        }
    }
}