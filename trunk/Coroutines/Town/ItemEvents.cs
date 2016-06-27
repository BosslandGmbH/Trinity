using System;
using System.Linq;
using System.Linq.Expressions;
using Buddy.Coroutines;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Game.Internals.Actors;

namespace Trinity.Coroutines.Town
{
    public class ItemEvents
    {
        public delegate void ItemEvent(TrinityItem item);

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

        public static void FireItemStashed(TrinityItem item)
        {
            OnItemStashed?.Invoke(item);
        }

        public static void FireItemSalvaged(TrinityItem item)
        {
            OnItemSalvaged?.Invoke(item);
        }

        public static void FireItemDropped(TrinityItem item)
        {
            OnItemDropped?.Invoke(item);
        }

        public static void FireItemPickedUp(TrinityItem item)
        {
            OnItemPickedUp?.Invoke(item);
        }

        public static void FireItemSold(TrinityItem item)
        {
            OnItemSold?.Invoke(item);
        }

        public static void FireItemIdentified(TrinityItem item)
        {
            OnItemIdentified?.Invoke(item);
        }

        public static void FireItemMoved(TrinityItem item)
        {
            OnItemMoved?.Invoke(item);
        }

        //public delegate bool ItemEvaluation(TrinityItem item);

        //public static event ItemEvaluation OnShouldDropItem;

        //public static bool FireShouldDropItem(TrinityItem item)
        //{
        //    return FireEvent(OnShouldDropItem, item, true);
        //}

        //private static TResult FireEvent<TResult, TParam>(MulticastDelegate e, TParam param, TResult successResult)
        //{
        //    return e != null && e.GetInvocationList().Any(dlg => Equals((TResult)dlg.Method.Invoke(dlg.Target, new object[] { param }), successResult)) ? successResult : default(TResult);
        //}

    }


}
