using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Helpers
{
    public static class CacheUtils
    {
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