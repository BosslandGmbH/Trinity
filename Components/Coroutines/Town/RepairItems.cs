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
using Trinity.Components.Combat;

namespace Trinity.Components.Coroutines.Town
{
    public static class RepairItems
    {
        public async static Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Core.Logger.Verbose("[修理装备] 需要在城里修理装备");
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
                    Core.Logger.Verbose("[修理装备] 不能修理");
                    return false;
                }
            }

            if (UIElements.VendorWindow.IsVisible)
            {
                await Coroutine.Sleep(1000);
                Core.Logger.Verbose("[修理装备] 在这个NPC修理装备");
                Repair(shouldRepairAll);
                return true;
            }

            var smith = TownInfo.Blacksmith;
            var merchant = TownInfo.NearestMerchant;
            var repairActor = (Randomizer.Boolean ? merchant : smith) ?? (smith ?? merchant);

            if (repairActor == null)
            {
                Core.Logger.Error("[修理装备] 找不到地方修理 :(");
                return false;
            };

            if (!await MoveToAndInteract.Execute(repairActor))
            {
                Core.Logger.Error($"[修理装备] 无法移动到NPC{repairActor.Name} 进行修复 :( ");
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
                Core.Logger.Verbose($"[修理装备] 	在 {repairActor.Name} 修理装备:)");
                Repair(shouldRepairAll);
                return true;
            }

            repairActor.GetActor().Interact();
            await Coroutine.Sleep(1000);

            if (GameUI.IsBlackSmithWindowOpen || UIElements.VendorWindow.IsVisible)
            {
                await Coroutine.Sleep(1000);
                Core.Logger.Verbose($"[修理装备] 在 {repairActor.Name}修理装备 :) (尝试两个)");
                Repair(shouldRepairAll);
                return true;
            }

            Core.Logger.Error($"[修理装备] 在 {repairActor.Name}无法修复 :(");
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
            var equippedItems = InventoryManager.Equipped.Where(i => i.IsValid && !i.IsDisposed && i.DurabilityCurrent < i.DurabilityMax).ToList();
            if (!equippedItems.Any())
                return false;

            // Fix for Campaign quest start of ACT1
            if (ZetaDia.CurrentQuest.QuestSnoId == 87700)
                return false;
			
			// 最高耐久
            var currentHighestDurItem = equippedItems.Max(i => i.DurabilityPercent);
			// 最低耐久
            var currentLowestDurItem = equippedItems.Min(i => i.DurabilityPercent);
			// 低于一半
            var LowThanHalf = equippedItems.Where(i => i.DurabilityPercent < 60).ToList().Count >= equippedItems.Count / 2;
            // 智能包裹整理
            bool cantoTwnrun = ZetaDia.Service.Party.NumPartyMembers > 1 && !ZetaDia.Service.Party.IsPartyLeader
                ? !Core.Player.ParticipatingInTieredLootRun
                : !Core.Player.ParticipatingInTieredLootRun || DefaultLootProvider.CanVedonInRift;

            bool NeedsRepairInTown = !Core.Settings.SenExtend.EnableIntelligentFinishing
                ? ZetaDia.IsInTown && currentHighestDurItem < 95 && cantoTwnrun
                : ZetaDia.IsInTown && (LowThanHalf || currentLowestDurItem < 30) && cantoTwnrun;
            //Core.Logger.Log($"条件1 = 1 = {ZetaDia.IsInTown},2= {currentHighestDurItem < 98},3= {!Core.Player.ParticipatingInTieredLootRun},4 = {ZetaDia.Me.CommonData.ParticipatingInTieredLootRun} 5 ={ZetaDia.Me.IsParticipatingInTieredLootRun}");
            //Core.Logger.Log($"条件2 = 1 = {ZetaDia.IsInTown},2 = {(LowThanHalf || currentLowestDurItem < 30)},3 = { !Core.Player.ParticipatingInTieredLootRun}");
            //Core.Logger.Log($"NeedsRepairInTown = {NeedsRepairInTown},currentHighestDurItem = {currentHighestDurItem},currentLowestDurItem={currentLowestDurItem},LowThanHalf={LowThanHalf}");
            if (NeedsRepairInTown)
            {
                Core.Logger.Log($"装备需要紧急维修! 当前耐久度={currentHighestDurItem} 修理限度={95}");
                return true;
            }

            var limit = Math.Max(CharacterSettings.Instance.RepairWhenDurabilityBelow, 30);

            if (currentHighestDurItem <= limit)
            {
                Core.Logger.Log($"装备需要紧急维修! 当前耐久度={currentHighestDurItem} 修理限度={limit}");
                return true;
            }

            return false;
        }
    }
}