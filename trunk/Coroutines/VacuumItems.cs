using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Combat.Abilities;
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

            foreach(var item in Trinity.ObjectCache)
            {
                var guid = item.ACDGuid;

                if (item.Item == null || item.Distance > 8f || VacuumedGuids.Contains(guid))
                    continue;

                if (!ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, guid))
                    continue;

                count++;       
                SpellHistory.RecordSpell(SNOPower.Axe_Operate_Gizmo);
                VacuumedGuids.Add(guid);            
            }

            if (count > 0)
            {
                Logger.LogVerbose($"Vacuumed {count} items");
            }
        }

        public static HashSet<int> VacuumedGuids { get; } = new HashSet<int>();
    }


}




