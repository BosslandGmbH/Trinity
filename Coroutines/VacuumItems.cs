using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Combat.Abilities;
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
            GameEvents.OnWorldChanged += (sender, args) =>  VacuumedGuids.Clear();
        }

        public static void Execute()
        {
            var count = 0;

            // Items that shouldn't be picked up are currently excluded from cache.
            // a pickup evaluation should be added if that changes.            

            foreach(var item in TrinityPlugin.Targets)
            {
                if (item == null || !item.IsValid)
                    continue;

                if (item.Distance > 8f || VacuumedGuids.Contains(item.AcdId))
                    continue;

                if (!ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, item.AcdId))
                    continue;

                count++;       
                SpellHistory.RecordSpell(SNOPower.Axe_Operate_Gizmo);
                VacuumedGuids.Add(item.AcdId);            
            }

            if (count > 0)
            {
                Logger.LogVerbose($"Vacuumed {count} items");
            }

            if(VacuumedGuids.Count > 1000)
                VacuumedGuids.Clear();
        }

        public static HashSet<int> VacuumedGuids { get; } = new HashSet<int>();
    }


}




