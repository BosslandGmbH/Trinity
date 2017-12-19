using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
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
        public async static Task<bool> Execute()
        {

            bool isVacuuming = false;
            if (Core.Player.IsCasting)
                return isVacuuming = false;

            var count = 0;

            // Items that shouldn't be picked up are currently excluded from cache.
            // a pickup evaluation should be added here if that changes.

            foreach (var item in Core.Targets.OfType<TrinityItem>())
            {
                bool validApproach = Core.Grids.Avoidance.IsIntersectedByFlags(Core.Player.Position, item.Position, AvoidanceFlags.NavigationBlocking, AvoidanceFlags.NavigationImpairing) && !Core.Player.IsFacing(item.Position, 90);

                /* Added checkpoints to avoid approach stuck -Seq */
                if (item.Distance > 8f || VacuumedAcdIds.Contains(item.AcdId) && !validApproach)
                    //Core.Logger.Debug("Vacuuming is valid");
                    continue;

                if (!ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, item.Position, Core.Player.WorldDynamicId, item.AcdId))
                {
                    Core.Logger.Verbose($"无效的战利品 {item.Name} AcdId={item.AcdId}");
                    continue;
                }

                count++;
                Core.Logger.Debug($"已拾取: {item.Name} ({item.ActorSnoId}) InternalName={item.InternalName} GbId={item.GameBalanceId}");
                SpellHistory.RecordSpell(SNOPower.Axe_Operate_Gizmo);
                VacuumedAcdIds.Add(item.AcdId);
                isVacuuming = true;
            }

            if (count > 0)
            {
                Core.Logger.Verbose($"拾取 {count} 个物品");
            }

            if (VacuumedAcdIds.Count > 1000)
            {
                VacuumedAcdIds.Clear();
            }
            return isVacuuming;
        }

        public static HashSet<int> VacuumedAcdIds { get; } = new HashSet<int>();
    }
}