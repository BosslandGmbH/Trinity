using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Coroutines.Resources;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Reference;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Coroutines.Town
{
    public static class RepairItems
    {
        public async static Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Logger.LogVerbose("[RepairItems] Need to be in town to Repair items");
                return false;
            }

            if (!EquipmentNeedsRepair())
                return false;

            var coinage = ZetaDia.PlayerData.Coinage;
            var shouldRepairAll = coinage > ZetaDia.Me.Inventory.GetRepairCost(true);
            if (!shouldRepairAll)
            {
                if (coinage < ZetaDia.Me.Inventory.GetRepairCost(false))
                {
                    Logger.LogVerbose("[RepairItems] Can't afford to repair");
                    return false;
                }
            }

            if (UIElements.VendorWindow.IsVisible)
            {
                await Coroutine.Sleep(1000);
                Logger.LogVerbose("[RepairItems] Repairing equipment while at this vendor");
                Repair(shouldRepairAll);
                return true;
            }

            var repairActor = TownInfo.NearestMerchant;
            if (repairActor == null)
            {
                Logger.LogError("[RepairItems] Failed to find somewhere to repair :(");
                return false;
            };

            if (!await MoveToAndInteract.Execute(repairActor))
            {
                Logger.LogError($"[RepairItems] Failed to move to Vendor {repairActor.Name} for repair :( ");
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
                Logger.LogVerbose($"[RepairItems] Repairing Equipment at {repairActor.Name} :)");
                Repair(shouldRepairAll);
                return true;
            }

            repairActor.GetActor().Interact();
            await Coroutine.Sleep(1000);

            if (GameUI.IsBlackSmithWindowOpen || UIElements.VendorWindow.IsVisible)
            {
                await Coroutine.Sleep(1000);
                Logger.LogVerbose($"[RepairItems] Repairing Equipment at {repairActor.Name} :) (Attempt Two)");
                Repair(shouldRepairAll);
                return true;
            }

            Logger.LogError($"[RepairItems] Failed to repair at {repairActor.Name} :(");
            return false;
        }

        private static void Repair(bool shouldRepairAll)
        {
            // todo: figure out why these repair calls don't work while at a blacksmith window.

            if (shouldRepairAll)
            {
                ZetaDia.Me.Inventory.RepairAllItems(); 
            }
            else
            {
                ZetaDia.Me.Inventory.RepairEquippedItems();
            }
        }

        public static bool EquipmentNeedsRepair()
        {
            var equippedItems = ZetaDia.Me.Inventory.Equipped.Where(i => i.DurabilityCurrent < i.DurabilityMax).ToList();
            if (!equippedItems.Any())
                return false;

            var currentHighestDurItem = equippedItems.Max(i => i.DurabilityPercent);
            if (ZetaDia.IsInTown && currentHighestDurItem < 95 && !Core.Player.ParticipatingInTieredLootRun)
            {
                Logger.Log($"Equipment Needs Repair! CurrentMaxDurabillity={currentHighestDurItem} InTownRepairLimit={95}");
                return true;
            }

            var limit = Math.Max(CharacterSettings.Instance.RepairWhenDurabilityBelow, 30);

            if (currentHighestDurItem <= limit)
            {
                Logger.Log($"Equipment Needs Emergency Repair! CurrentMaxDurabillity={currentHighestDurItem} RepairLimit={limit}");
                return true;
            }

            return false;
        }

    }
}
