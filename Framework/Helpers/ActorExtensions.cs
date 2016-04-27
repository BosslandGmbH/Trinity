using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Actors;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Helpers
{
    public static class CacheUtils
    {
        public static List<ACDItem> ToAcdItems(this IEnumerable<CachedItem> items)
        {
            return items.Select(i => ToAcdItem((CachedItem) i)).ToList();
        }

        public static ACDItem ToAcdItem(this CachedItem item)
        {
            return ZetaDia.Actors.GetACDByAnnId(item.AnnId) as ACDItem;
        }

        public static int StackQuantity(this IEnumerable<ACDItem> items)
        {
            return items.Select(i => (int)i.ItemStackQuantity).Sum();
        }

        public static int StackQuantity(this IEnumerable<CachedItem> items)
        {
            return items.Select(i => (int)i.ItemStackQuantity).Sum();
        }
    }
}