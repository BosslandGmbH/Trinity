using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Events;
using Trinity.Framework.Reference;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines.Town
{
    public static partial class TrinityTownRun
    {
        public static bool ShouldSell(ACDItem i)
        {
            if (i.IsProtected())
                return false;

            if (BrainBehavior.GreaterRiftInProgress)
                return false;

            if (i.IsUnidentified)
                return false;

            return Combat.TrinityCombat.Loot.ShouldSell(i) &&
                   !Combat.TrinityCombat.Loot.ShouldSalvage(i) &&
                   !Combat.TrinityCombat.Loot.ShouldStash(i);
        }

        public static async Task<CoroutineResult> SellItems()
        {
            if (!ZetaDia.IsInTown)
                return CoroutineResult.NoAction;

            var sellItem = InventoryManager.Backpack
                .FirstOrDefault(i => ShouldSell(i) &&
                                     InventoryManager.CanSellItem(i));
            if (sellItem == null)
            {
                if (await RepairItems() == CoroutineResult.Running)
                    return CoroutineResult.Running;

                GameUI.CloseVendorWindow();
                return CoroutineResult.Done;
            }

            if (!UIElements.VendorWindow.IsVisible)
            {
                var merchant = TownInfo.NearestMerchant;
                if (merchant == null)
                {
                    s_logger.Error($"[{nameof(SellItems)}] Unable to find merchant info for this area :(");
                    return CoroutineResult.Failed;
                }

                if (await CommonCoroutines.MoveAndInteract(
                        merchant.GetActor(),
                        () => UIElements.VendorWindow.IsVisible) == CoroutineResult.Running)
                {
                    return CoroutineResult.Running;
                }
            }

            if (!sellItem.IsValid || sellItem.IsUnidentified)
            {
                s_logger.Debug($"[{nameof(SellItems)}] Invalid Items Detected: IsValid={sellItem.IsValid} IsUnidentified={sellItem.IsUnidentified}");
                return CoroutineResult.Failed;
            }

            s_logger.Debug($"[{nameof(SellItems)}] Selling: {sellItem.Name} ({sellItem.ActorSnoId}) Quality={sellItem.ItemQualityLevel} IsAncient={sellItem.Stats.IsAncient} Name={sellItem.InternalName}");
            ItemEvents.FireItemSold(sellItem);
            InventoryManager.SellItem(sellItem);
            Core.Inventory.InvalidAnnIds.Add(sellItem.AnnId);
            return CoroutineResult.Running;
        }
    }
}