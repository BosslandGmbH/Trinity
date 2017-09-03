using System;
using System.Collections.Generic;
using IronPython.Modules;
using System.Linq;
using Buddy.Coroutines;
using Trinity.Framework;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using static Core;
using Trinity.Framework.Avoidance.Structures;
using System.Threading.Tasks;
namespace Trinity.Components.Coroutines
{
    /// <summary>
    /// Pickup items within range.
    /// </summary>
    public class VacuumItems
    {
        static VacuumItems()
        {
            GameEvents.OnWorldChanged += (sender, args) => VacuumedAcdIds.Clear();
        }

        /* Should be able to wait for finishing this task While doing something.
         * Otherwise we see the bot locks in dilemma between going to Quest/Rift
         * or Vacuuming nearby items -Seq */
        public static async Task<bool> Execute(bool inTown = false)
        {

            bool isVacuuming = false;
            if (Player.IsCasting)
                return isVacuuming = false;

            var oldItems = VacuumedAcdIds.Where(x => (DateTime.Now - x.Value).TotalSeconds > 30).Select(x => x.Key).ToList();
            if (oldItems.Any())
            {
                foreach (var item in oldItems)
                {
                    VacuumedAcdIds.Remove(item);
                }
            }

            var count = 0;

            // Items that shouldn't be picked up are currently excluded from cache.
            // a pickup evaluation should be added here if that changes.

            foreach (var item in Targets.OfType<TrinityItem>().OrderBy(x => !x.IsPickupNoClick).ThenBy(x => x.Distance))
            {
                var validApproach = Grids.Avoidance.IsIntersectedByFlags(Player.Position, item.Position, AvoidanceFlags.NavigationBlocking, AvoidanceFlags.NavigationImpairing) && !Player.IsFacing(item.Position, 90);
                Inventory.Backpack.Update();
                if (Player.FreeBackpackSlots < 4)
                    break;
                if (inTown)
                {
                    if (VacuumedAcdIds.Keys.Contains(item.AcdId))
                        continue;
                    Logger.Warn($"Moving to vacuum town item {item.Name} AcdId={item.AcdId} Distance={item.Distance}");
                    if (!await MoveToAndInteract.Execute(item.Position, item.AcdId, 3))
                    {
                        Logger.Debug($"[TownLoot] Failed to move to item ({item.Name}) to pick up items :(");
                        await Coroutine.Sleep((int)(item.Position.Distance2D(Player.Position) + 1) * 150);
                    }
                    await Coroutine.Sleep(500);
                    if (!ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, item.Position, Player.WorldDynamicId,
                            item.AcdId))
                    {
                        Logger.Warn($"[TownLoot] Failed to vacuum town item {item.Name} AcdId={item.AcdId} Distance={item.Position.Distance2D(Player.Position)}");
                        VacuumedAcdIds.Add(item.AcdId, DateTime.Now);
                        continue;
                    }
                }
                else
                {
                    /* Added checkpoints to avoid approach stuck -Seq */
                    if (item.Distance > 8f || !validApproach)
                        //Core.Logger.Debug("Vacuuming is valid");
                        continue;
                    if (!ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, item.Position, Player.WorldDynamicId, item.AcdId))
                    {
                        Logger.Warn($"Failed to vacuum item {item.Name} AcdId={item.AcdId}");
                        continue;
                    }
                    VacuumedAcdIds.Add(item.AcdId, DateTime.Now);
                }
                count++;
                Logger.Debug($"Vacuumed: {item.Name} ({item.ActorSnoId}) InternalName={item.InternalName} GbId={item.GameBalanceId}");
                SpellHistory.RecordSpell(SNOPower.Axe_Operate_Gizmo);
                isVacuuming = true;
            }
            Logger.Verbose($"Vacuumed {count} items");
            return isVacuuming;
        }

        public static Dictionary<int, DateTime> VacuumedAcdIds { get; } = new Dictionary<int, DateTime>();
    }
}