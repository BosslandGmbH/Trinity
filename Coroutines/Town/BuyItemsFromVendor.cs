using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework.Objects;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Coroutines.Town
{
    public class BuyItemsFromVendor
    {
        /// <summary>
        /// Work in progress
        /// </summary>
        public static async Task<bool> Execute(ItemQualityColor qualityColor, List<ItemType> types = null, int totalAmount = -1, int vendorId = -1)
        {
            Logger.Log("BuyItemsFromVendor Started!");

            //if (ZetaDia.Me.Inventory.NumFreeBackpackSlots < totalAmount * 2)
            //{
            //    Logger.Log("Not enough bag space to buy {0} items", totalAmount);
            //    await BrainBehavior.CreateVendorBehavior().ExecuteCoroutine();
            //}

            foreach (var item in ZetaDia.Me.Inventory.MerchantItems)
            {
                item.PrintEFlags();
            }

            var items = ZetaDia.Me.Inventory.MerchantItems.ToList();

            if (!await MoveToAndInteract.Execute(TownInfo.NearestMerchant))
                return false;

            Logger.Log("BuyItemsFromVendor Finished!");
            return true;
        }

    }
}
