using Buddy.Coroutines;
using System;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Reference;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Components.Coroutines.Town
{
    public static partial class TrinityTownRun
    {
        public static async Task<bool> RepairItems()
        {
            if (!ZetaDia.IsInTown)
                return true;

            if (!EquipmentNeedsRepair())
                return true;

            var coinage = ZetaDia.Storage.PlayerDataManager.ActivePlayerData.Coinage;
            var shouldRepairAll = coinage > InventoryManager.GetRepairCost(true);
            if (!shouldRepairAll)
            {
                if (coinage < InventoryManager.GetRepairCost(false))
                {
                    s_logger.Error($"[{nameof(RepairItems)}] Can't afford to repair");
                    return true;
                }
            }

            if (!(GameUI.IsBlackSmithWindowOpen || UIElements.VendorWindow.IsVisible))
            {
                var smith = TownInfo.Blacksmith;
                var merchant = TownInfo.NearestMerchant;
                var repairActor = (Randomizer.Boolean ? merchant : smith) ?? (smith ?? merchant);

                if (repairActor == null)
                {
                    s_logger.Error($"[{nameof(RepairItems)}] Failed to find somewhere to repair :(");
                    return true;
                };

                if (!await CommonCoroutines.MoveAndInteract(
                        repairActor.GetActor(),
                        () => GameUI.IsBlackSmithWindowOpen ||
                              UIElements.VendorWindow.IsVisible))
                    return false;
            }

            await Coroutine.Yield();
            s_logger.Info($"[{nameof(RepairItems)}] Repairing equipment while at this vendor");
            Repair(shouldRepairAll);
            return true;
        }

        private static void Repair(bool shouldRepairAll)
        {
            // TODO: figure out why these repair calls don't work while at a blacksmith window.
            if (shouldRepairAll)
                InventoryManager.RepairAllItems();
            else
                InventoryManager.RepairEquippedItems();
        }

        public static bool EquipmentNeedsRepair()
        {
            var equippedItems = InventoryManager.Equipped
                .Where(i => i.IsValid &&
                            !i.IsDisposed &&
                            i.DurabilityCurrent < i.DurabilityMax)
                .ToList();
            if (!equippedItems.Any())
                return false;

            // Fix for Campaign quest start of ACT1
            if (ZetaDia.CurrentQuest.QuestSnoId == 87700)
                return false;

            var currentHighestDurItem = equippedItems.Max(i => i.DurabilityPercent);
            if (ZetaDia.IsInTown &&
                currentHighestDurItem < 95 &&
                !Core.Player.ParticipatingInTieredLootRun)
            {
                s_logger.Debug($"[{nameof(EquipmentNeedsRepair)}] Equipment Needs Repair! CurrentMaxDurabillity={currentHighestDurItem} InTownRepairLimit={95}");
                return true;
            }

            var limit = Math.Max(CharacterSettings.Instance.RepairWhenDurabilityBelow, 30);

            if (currentHighestDurItem <= limit)
            {
                s_logger.Debug($"[{nameof(EquipmentNeedsRepair)}] Equipment Needs Emergency Repair! CurrentMaxDurabillity={currentHighestDurItem} RepairLimit={limit}");
                return true;
            }

            return false;
        }
    }
}