using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


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

        public static void Execute()
        {
            if (Core.Player.IsCasting)
                return;

            var count = 0;

            // Items that shouldn't be picked up are currently excluded from cache.
            // a pickup evaluation should be added here if that changes.

            foreach (var item in Core.Targets.OfType<TrinityItem>())
            {
                if (item.Distance > 8f || VacuumedAcdIds.Contains(item.AcdId))
                    continue;

                if (!ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, item.Position, Core.Player.WorldDynamicId, item.AcdId))
                {
                    Core.Logger.Verbose($"Failed to vacuum item {item.Name} AcdId={item.AcdId}");
                    continue;
                }

                count++;
                Core.Logger.Debug($"Vacuumed: {item.Name} ({item.ActorSnoId}) InternalName={item.InternalName} GbId={item.GameBalanceId}");
                SpellHistory.RecordSpell(SNOPower.Axe_Operate_Gizmo);
                VacuumedAcdIds.Add(item.AcdId);
            }

            if (count > 0)
            {
                Core.Logger.Verbose($"Vacuumed {count} items");
            }

            if (VacuumedAcdIds.Count > 1000)
            {
                VacuumedAcdIds.Clear();
            }
        }

        public static HashSet<int> VacuumedAcdIds { get; } = new HashSet<int>();
    }
}