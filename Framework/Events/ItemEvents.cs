using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;

namespace Trinity.Framework.Events
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
        /// When an item is dropped from the backpack onto the ground.
        /// </summary>
        public static event ItemEvent OnItemDropped;

        /// <summary>
        /// When an item is created by the cube
        /// </summary>
        public static event ItemEvent OnItemCubed;

        /// <summary>
        /// When an item is created by gambling
        /// </summary>
        public static event ItemEvent OnItemGambled;

        public static void FireItemStashed(TrinityItem item)
        {
            Core.Logger.Log(LogCategory.ItemEvents, $"储存物品 {item.Name}");
            OnItemStashed?.Invoke(item);
        }

        public static void FireItemSalvaged(TrinityItem item)
        {
            Core.Logger.Log(LogCategory.ItemEvents, $"分解物品 {item.Name}");
            OnItemSalvaged?.Invoke(item);
        }

        public static void FireItemDropped(TrinityItem item)
        {
            Core.Logger.Log(LogCategory.ItemEvents, $"丢弃物品 {item.Name}");
            OnItemDropped?.Invoke(item);
        }

        public static void FireItemPickedUp(TrinityItem item)
        {
            Core.Logger.Log(LogCategory.ItemEvents, $"拾取物品 {item.Name}");
            OnItemPickedUp?.Invoke(item);
        }

        public static void FireItemSold(TrinityItem item)
        {
            Core.Logger.Log(LogCategory.ItemEvents, $"出售物品 {item.Name}");
            OnItemSold?.Invoke(item);
        }

        public static void FireItemIdentified(TrinityItem item)
        {
            Core.Logger.Log(LogCategory.ItemEvents, $"鉴定物品 {item.Name}");
            OnItemIdentified?.Invoke(item);
        }

        public static void FireItemMoved(TrinityItem item)
        {
            Core.Logger.Log(LogCategory.ItemEvents, $"移动物品 {item.Name}");
            OnItemMoved?.Invoke(item);
        }

        public static void FireItemCubed(TrinityItem item)
        {
            Core.Logger.Log(LogCategory.ItemEvents, $"提取物品传奇威能 {item.Name}");
            OnItemCubed?.Invoke(item);
        }

        public static void FireItemGambled(TrinityItem item)
        {
            Core.Logger.Log(LogCategory.ItemEvents, $"赌博物品 {item.Name}");
            OnItemGambled?.Invoke(item);
        }
    }
}