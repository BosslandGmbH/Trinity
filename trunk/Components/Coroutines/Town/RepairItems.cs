using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Reference;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Components.Coroutines.Town
{
    public static class RepairItems
    {
        public async static Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Core.Logger.Verbose("[RepairItems] Need to be in town to Repair items");
                return false;
            }

            if (!EquipmentNeedsRepair())
                return false;

            var coinage = ZetaDia.Storage.PlayerDataManager.ActivePlayerData.Coinage;
            var shouldRepairAll = coinage > InventoryManager.GetRepairCost(true);
            if (!shouldRepairAll)
            {
                if (coinage < InventoryManager.GetRepairCost(false))
                {
                    Core.Logger.Verbose("[RepairItems] Can't afford to repair");
                    return false;
                }
            }

            if (UIElements.VendorWindow.IsVisible)
            {
                await Coroutine.Sleep(1000);
                Core.Logger.Verbose("[RepairItems] Repairing equipment while at this vendor");
                Repair(shouldRepairAll);
                return true;
            }

            var repairActor = Randomizer.Boolean ? TownInfo.NearestMerchant : TownInfo.Blacksmith;
            //var repairActor = TownInfo.NearestMerchant;
            if (repairActor == null)
            {
                Core.Logger.Error("[RepairItems] Failed to find somewhere to repair :(");
                return false;
            };

            if (!await MoveToAndInteract.Execute(repairActor))
            {
                Core.Logger.Error($"[RepairItems] Failed to move to Vendor {repairActor.Name} for repair :( ");
                return false;
            };

            if (repairActor.Distance <= 10f)
            {
                Navigator.PlayerMover.MoveStop();
                await Coroutine.Sleep(1500);
            }

            if (GameUI.IsBlackSmithWindowOpen || UIElements.VendorWindow.IsVisible)
            {
                await Coroutine.Sleep(1500);
                Core.Logger.Verbose($"[RepairItems] Repairing Equipment at {repairActor.Name} :)");
                Repair(shouldRepairAll);
                return true;
            }

            repairActor.GetActor().Interact();
            await Coroutine.Sleep(1000);

            if (GameUI.IsBlackSmithWindowOpen || UIElements.VendorWindow.IsVisible)
            {
                await Coroutine.Sleep(1000);
                Core.Logger.Verbose($"[RepairItems] Repairing Equipment at {repairActor.Name} :) (Attempt Two)");
                Repair(shouldRepairAll);
                return true;
            }

            Core.Logger.Error($"[RepairItems] Failed to repair at {repairActor.Name} :(");
            return false;
        }

        private static void Repair(bool shouldRepairAll)
        {
            // todo: figure out why these repair calls don't work while at a blacksmith window.

            if (shouldRepairAll)
            {
                InventoryManager.RepairAllItems();
            }
            else
            {
                InventoryManager.RepairEquippedItems();
            }
        }

        public static bool EquipmentNeedsRepair()
        {
            var equippedItems = InventoryManager.Equipped.Where(i => i.DurabilityCurrent < i.DurabilityMax).ToList();
            if (!equippedItems.Any())
                return false;

            var currentHighestDurItem = equippedItems.Max(i => i.DurabilityPercent);
            if (ZetaDia.IsInTown && currentHighestDurItem < 95 && !Core.Player.ParticipatingInTieredLootRun)
            {
                Core.Logger.Log($"Equipment Needs Repair! CurrentMaxDurabillity={currentHighestDurItem} InTownRepairLimit={95}");
                return true;
            }

            var limit = Math.Max(CharacterSettings.Instance.RepairWhenDurabilityBelow, 30);

            if (currentHighestDurItem <= limit)
            {
                Core.Logger.Log($"Equipment Needs Emergency Repair! CurrentMaxDurabillity={currentHighestDurItem} RepairLimit={limit}");
                return true;
            }

            return false;
        }
    }
}