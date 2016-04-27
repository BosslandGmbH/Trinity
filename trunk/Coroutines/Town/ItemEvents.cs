using System;
using System.Linq;
using System.Linq.Expressions;
using Buddy.Coroutines;
using Trinity.DbProvider;
using Trinity.Framework.Actors;
using Zeta.Game.Internals.Actors;

namespace Trinity.Coroutines.Town
{
    public class ItemEvents
    {
        public delegate void ItemEvent(CachedItem item);

        /// <summary>
        /// When an item has been salvaged.
        /// </summary>
        public static event ItemEvent OnItemSalvaged;

        /// <summary>
        /// When an item has been put into the stash from the backpack.
        /// </summary>
        public static event ItemEvent OnItemStashed;

        /// <summary>
        /// When an item has been sold to a vendor.
        /// </summary>
        public static event ItemEvent OnItemSold;

        /// <summary>
        /// When an item is picked up off the ground and ghas arrived in backpack.
        /// </summary>
        public static event ItemEvent OnItemPickedUp;

        /// <summary>
        /// When an item has been identified
        /// </summary>
        public static event ItemEvent OnItemIdentified;

        /// <summary>
        /// When an item moves position in backpack or stash
        /// </summary>
        public static event ItemEvent OnItemMoved;

        /// <summary>
        /// When an  is dropped from the backpack onto the ground.
        /// </summary>
        public static event ItemEvent OnItemDropped;

        public static void FireItemStashed(CachedItem item)
        {
            OnItemStashed?.Invoke(item);
        }

        public static void FireItemSalvaged(CachedItem item)
        {
            OnItemSalvaged?.Invoke(item);
        }

        public static void FireItemDropped(CachedItem item)
        {
            OnItemDropped?.Invoke(item);
        }

        public static void FireItemPickedUp(CachedItem item)
        {
            OnItemPickedUp?.Invoke(item);
        }

        public static void FireItemSold(CachedItem item)
        {
            OnItemSold?.Invoke(item);
        }

        public static void FireItemIdentified(CachedItem item)
        {
            OnItemIdentified?.Invoke(item);
        }

        public static void FireItemMoved(CachedItem item)
        {
            OnItemMoved?.Invoke(item);
        }

        //public delegate bool ItemEvaluation(CachedItem item);

        //public static event ItemEvaluation OnShouldDropItem;

        //public static bool FireShouldDropItem(CachedItem item)
        //{
        //    return FireEvent(OnShouldDropItem, item, true);
        //}

        //private static TResult FireEvent<TResult, TParam>(MulticastDelegate e, TParam param, TResult successResult)
        //{
        //    return e != null && e.GetInvocationList().Any(dlg => Equals((TResult)dlg.Method.Invoke(dlg.Target, new object[] { param }), successResult)) ? successResult : default(TResult);
        //}

    }


}
