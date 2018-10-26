using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Trinity.Framework.Helpers;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Components.Coroutines.Town
{
    public static partial class TrinityTownRun
    {
        public static bool ShouldSell(TrinityItem i)
        {
            if (i.IsProtected())
                return false;

            if (BrainBehavior.GreaterRiftInProgress)
                return false;

            if (i.IsUnidentified)
                return false;

            return Combat.TrinityCombat.Loot.ShouldSell(i) && !Combat.TrinityCombat.Loot.ShouldSalvage(i) && !Combat.TrinityCombat.Loot.ShouldStash(i);
        }

        public static async Task<bool> SellItems()
        {
            if (!ZetaDia.IsInTown) return true;

            var sellItem = Core.Inventory.Backpack.FirstOrDefault(i => ShouldSell(i) && InventoryManager.CanSellItem(i.ToAcdItem()));
            if (sellItem == null)
            {
                if (!await RepairItems())
                    return false;
                GameUI.CloseVendorWindow();
                return true;
            }

            if (!UIElements.VendorWindow.IsVisible)
            {
                var merchant = TownInfo.NearestMerchant;
                if (merchant == null)
                {
                    s_logger.Error($"[{nameof(SellItems)}] Unable to find merchant info for this area :(");
                    return true;
                }

                if (!await CommonCoroutines.MoveAndInteract(merchant.GetActor(), () => UIElements.VendorWindow.IsVisible))
                    return false;
            }

            if (!sellItem.IsValid || sellItem.IsUnidentified)
            {
                s_logger.Debug($"[{nameof(SellItems)}] Invalid Items Detected: IsValid={sellItem.IsValid} IsUnidentified={sellItem.IsUnidentified}");
                return false;
            }

            s_logger.Debug($"[{nameof(SellItems)}] Selling: {sellItem.Name} ({sellItem.ActorSnoId}) Quality={sellItem.ItemQualityLevel} IsAncient={sellItem.IsAncient} Name={sellItem.InternalName}");
            ItemEvents.FireItemSold(sellItem);
            InventoryManager.SellItem(sellItem.ToAcdItem());
            Core.Inventory.InvalidAnnIds.Add(sellItem.AnnId);
            return false;
        }
    }
}