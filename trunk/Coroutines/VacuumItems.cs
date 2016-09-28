using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Combat.Abilities;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Items;
using Trinity.Objects;
using Trinity.Reference;
using Zeta.Game;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Coroutines
{
    public class VacuumItems
    {
        static VacuumItems()
        {
            GameEvents.OnWorldChanged += (sender, args) =>  VacuumedAcdIds.Clear();
        }

        public static void Execute()
        {
            if (Core.Player.IsCasting)
                return;

            var count = 0;

            // Items that shouldn't be picked up are currently excluded from cache.
            // a pickup evaluation should be added here if that changes.            

            foreach(var item in TrinityPlugin.Targets.OfType<TrinityItem>())
            {
                if (item.Distance > 8f || VacuumedAcdIds.Contains(item.AcdId))
                    continue;

                if (!ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, item.Position, Core.Player.WorldDynamicId, item.AcdId))
                {
                    Logger.LogVerbose($"Failed to vacuum item {item.Name} AcdId={item.AcdId}");
                    continue;
                }

                count++;       
                SpellHistory.RecordSpell(SNOPower.Axe_Operate_Gizmo);
                VacuumedAcdIds.Add(item.AcdId);            
            }

            if (count > 0)
            {
                Logger.LogVerbose($"Vacuumed {count} items");
            }

            if (VacuumedAcdIds.Count > 1000)
            {
                VacuumedAcdIds.Clear();
            }
        }

        public static HashSet<int> VacuumedAcdIds { get; } = new HashSet<int>();
    }


}




