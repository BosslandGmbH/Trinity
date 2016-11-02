using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Helpers
{
    public static class ActorExtensions
    {
        public static bool IsUsingDeathGate(this DiaPlayer actor)
        {
            /*
                Line 44:  431: Hidden (-3665) i:1 f:0 Value=1 	
                Line 7:  586: BuffIconStartTick0 (-3510) [ PowerSnoId: ActorLoadingBuff: 212032 ] i:0 f:0 Value=0 
                Line 31:  618: BuffIconEndTick0 (-3478) [ PowerSnoId: ActorLoadingBuff: 212032 ] i:0 f:0 Value=0 
                Line 108:  755: BuffIconCount0 (-3341) [ PowerSnoId: ActorLoadingBuff: 212032 ] i:0 f:0 Value=0 	
                Line 224:  853: PowerBuff0VisualEffectNone (-3243) [ PowerSnoId: x1_Fortress_Portal_Switch: 360496 ] i:1 f:0 Value=1 
                Line 127:  295: PowerImmobilize (-3801) i:1 f:0 Value=1 
            */

            return ZetaDia.Me.CommonData.GetAttribute<bool>(ActorAttributeType.PowerBuff0VisualEffectNone, (int)SNOPower.x1_Fortress_Portal_Switch);
        }

        public static List<ACDItem> ToAcdItems(this IEnumerable<TrinityItem> items)
        {
            return items.Select(ToAcdItem).ToList();
        }

        public static ACDItem ToAcdItem(this TrinityItem item)
        {
            return ZetaDia.Actors.GetACDByAnnId(item.AnnId) as ACDItem;
        }

        public static int StackQuantity(this IEnumerable<ACDItem> items)
        {
            return items.Select(i => (int)i.ItemStackQuantity).Sum();
        }

        public static int StackQuantity(this IEnumerable<TrinityItem> items)
        {
            return items.Select(i => (int)i.ItemStackQuantity).Sum();
        }
    }
}
