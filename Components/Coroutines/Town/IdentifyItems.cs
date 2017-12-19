using System;
using Trinity.Framework;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Helpers;
using Zeta.Game;
using Trinity.Components.Combat;

namespace Trinity.Components.Coroutines.Town
{
    public static class IdentifyItems
    {
        public static async Task<bool> Execute()
        {
            if (!ZetaDia.IsInTown)
            {
                Core.Logger.Verbose("[鉴定物品] 需要在城里鉴定物品");
                return false;
            }
            //智能包裹整理
            if (Core.Player.IsInventoryLockedForGreaterRift || !Core.Settings.Items.KeepLegendaryUnid && (Core.Player.ParticipatingInTieredLootRun && !DefaultLootProvider.CanVedonInRift))
            {
                Core.Logger.Verbose($"[鉴定物品] 秘境中仓库锁定, IsInventoryLockedForGreaterRift={Core.Player.IsInventoryLockedForGreaterRift}, KeepLegendaryUnid={Core.Settings.Items.KeepLegendaryUnid}, ParticipatingInTieredLootRun={Core.Player.ParticipatingInTieredLootRun}, CanVedonInRift={DefaultLootProvider.CanVedonInRift}");
                return false;
            }
            if (Core.Settings.Items.KeepLegendaryUnid)
            {
                Core.Logger.Verbose("[鉴定物品] 回城设置 '保持未鉴定的传奇' - 跳过 ID");
                return false;
            }

            var timeout = DateTime.UtcNow.Add(TimeSpan.FromSeconds(30));

            var bookActor = TownInfo.BookOfCain;
            if (bookActor == null)
            {
                Core.Logger.Log($"[鉴定物品] 城镇信息.未找到凯恩之书 Act={ZetaDia.CurrentAct} WorldSnoId={ZetaDia.Globals.WorldSnoId}");
                return false;
            }

            while (Core.Inventory.Backpack.Any(i => i.IsUnidentified))
            {
                if (DateTime.UtcNow > timeout)
                    break;
                if (!ZetaDia.IsInTown)
                {
                    Core.Logger.Verbose("[鉴定物品] 不是在城里鉴定物品");
                    break;
                }
                Core.Logger.Log("鉴定物品");

                if (!Core.Grids.CanRayCast(ZetaDia.Me.Position, bookActor.Position))
                {
                    await MoveTo.Execute(TownInfo.NearestSafeSpot);
                }

                await MoveToAndInteract.Execute(bookActor);
                await Coroutine.Wait(8000, () => ZetaDia.Me.LoopingAnimationEndTime <= 0);
            }
            return false;
        }
    }
}