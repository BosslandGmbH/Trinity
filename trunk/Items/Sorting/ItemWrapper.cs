using System;
using System.Collections.Generic;
using Trinity.Framework.Helpers;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Items
{
    public class ItemWrapper : IComparable<ItemWrapper>, IEquatable<ItemWrapper>
    {
        public int ActorSNO { get; set; }
        public int GameBalanceId { get; set; }
        public int DynamicId { get; set; }
        public int ACDGuid { get; set; }
        public int RequiredLevel { get; set; }
        public InventorySlot InventorySlot { get; set; }
        public InventorySlot[] ValidInventorySlots { get; set; }
        public bool IsUnidentified { get; set; }
        public bool IsTwoHand { get; set; }
        public bool IsOneHand { get; set; }
        public string Name { get; set; }
        public string InternalName { get; set; }
        public ItemType ItemType { get; set; }
        public ItemBaseType ItemBaseType { get; set; }
        public bool HasSingleUseSlot { get; set; }
        public bool IsShield { get; set; }
        public bool IsOffHand { get; set; }
        public bool IsWeapon { get; set; }
        public bool IsJewelry { get; set; }
        public bool IsArmor { get; set; }
        public bool IsEquipment { get; set; }
        public bool IsMisc { get; set; }
        public bool IsGem { get; set; }
        public bool IsTwoSquareItem { get; set; }
        public bool IsPotion { get; set; }
        public ItemQuality ItemQualityLevel { get; set; }
        public GemQuality GemQuality { get; set; }
        public int TieredLootRunKeyLevel { get; set; }
        public long ItemStackQuantity { get; set; }
        public bool IsSetItem { get; set; }
        public string ItemSetName { get; set; }

        public ACDItem Item { get; private set; }
        public ItemStats Stats { get; private set; }
        public ItemStatsData StatsData { get; private set; }
        public Item ReferenceItem { get; private set; }

        public ItemWrapper(ACDItem item)
        {
            try
            {
                ActorSNO = item.ActorSnoId;
                GameBalanceId = item.GameBalanceId;
                DynamicId = item.AnnId;
                ACDGuid = item.ACDId;
                InventorySlot = item.InventorySlot;
                ValidInventorySlots = item.ValidInventorySlots;
                RequiredLevel = item.RequiredLevel;
            }
            catch (Exception ex)
            {
                Logger.LogError("Error wrapping non-attribute properties on item {0}: " + ex);
            }
            try
            {
                Name = item.Name;
                IsUnidentified = item.IsUnidentified;
                IsTwoHand = item.IsTwoHand;
                IsOneHand = item.IsOneHand;
                InternalName = item.InternalName;
                ItemType = item.ItemType;
                ItemBaseType = item.ItemBaseType;
                IsShield = TypeConversions.ShieldTypes.Contains(ItemType);
                IsOffHand = TypeConversions.OffHandTypes.Contains(ItemType);
                IsArmor = TypeConversions.ArmorTypes.Contains(ItemType);
                IsJewelry = TypeConversions.JewleryTypes.Contains(ItemType);
                IsWeapon = TypeConversions.WeaponTypes.Contains(ItemType);
                IsEquipment = item.ItemBaseType == ItemBaseType.Armor || item.ItemBaseType == ItemBaseType.Jewelry || item.ItemBaseType == ItemBaseType.Weapon;
                IsMisc = TypeConversions.MiscTypes.Contains(ItemType);
                IsGem = item.ItemBaseType == ItemBaseType.Gem;
                IsTwoSquareItem = (item.ItemBaseType == ItemBaseType.Armor || item.ItemBaseType == ItemBaseType.Weapon) && item.IsTwoSquareItem;
                IsPotion = item.IsPotion;
                ItemQualityLevel = item.ItemQualityLevel;
                GemQuality = item.GemQuality;
                TieredLootRunKeyLevel = item.TieredLootRunKeyLevel;
                ItemStackQuantity = item.ItemStackQuantity;
                IsSetItem = item.IsSetItem();
                ItemSetName = item.ItemSetName();
                HasSingleUseSlot = IsSingleSlotItem();
                Item = item;
                Stats = item.Stats;
                StatsData = ItemStatsDataFactory.GetItemStatsDataFromStats(Stats);
                ReferenceItem = new Item(Item);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error wrapping item {0}: " + ex);
            }
        }

        public bool IsSingleSlotItem()
        {
            if (IsOneHand)
                return false;

            if (ItemType == ItemType.Ring)
                return false;

            return true;
        }

        public int CompareTo(ItemWrapper other)
        {
            return this.Compare(other);
        }

        public bool Equals(ItemWrapper other)
        {
            if (other == null)
                return false;

            if (DynamicId == other.DynamicId)
                return true;

            return ItemType == other.ItemType &&
                ItemBaseType == other.ItemBaseType &&
                Item.ItemQualityLevel == other.Item.ItemQualityLevel &&
                Item.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Item.GetHashCode();
        }

        public static bool operator ==(ItemWrapper a, ItemWrapper b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.ItemType == b.ItemType &&
                a.ItemBaseType == b.ItemBaseType &&
                a.Item.ItemQualityLevel == b.Item.ItemQualityLevel &&
                a.Item.Name == b.Name;
        }

        public static bool operator !=(ItemWrapper item, ItemWrapper other)
        {
            return !(item == other);
        }

        public static bool operator <(ItemWrapper item, ItemWrapper other)
        {
            return item.Compare(other) < 0;
        }

        public static bool operator >(ItemWrapper item, ItemWrapper other)
        {
            return item.CompareTo(other) > 0;
        }
        public static bool operator >=(ItemWrapper item, ItemWrapper other)
        {
            return item.CompareTo(other) >= 0;
        }

        public static bool operator <=(ItemWrapper item, ItemWrapper other)
        {
            return item.CompareTo(other) <= 0;
        }
    }
}

