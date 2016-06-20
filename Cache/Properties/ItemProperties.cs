using System;
using Trinity.Coroutines.Town;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Items;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Cache.Properties
{
    /// <summary>
    /// Properties that are specific to items (on the ground only)
    /// This class should update all values that are possible/reasonable/useful.
    /// DO NOT put settings or situational based exclusions in here, do that in weighting etc.
    /// </summary>
    public class ItemProperties : IPropertyCollection
    {
        private DateTime _lastUpdated = DateTime.MinValue;
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(250);

        public bool IsValid { get; set; } = true;

        public DateTime CreationTime { get; } = DateTime.UtcNow;

        public void ApplyTo(TrinityCacheObject target)
        {
            if (!target.IsFrozen && DateTime.UtcNow.Subtract(_lastUpdated) > UpdateInterval)
                Update(target);

            target.IsMyDroppedItem = this.IsMyDroppedItem;
            target.GoldAmount = this.GoldAmount;
            target.DBItemType = this.DBItemType;
            target.DBItemBaseType = this.DBItemBaseType;
            target.ItemType = this.TrinityItemType;
            target.BaseType = this.TrinityItemBaseType;
            target.FollowerType = this.FollowerType;
            target.IsEquipment = this.IsEquipment;
            target.IsTwoSlotItem = this.IsTwoHanded;
            target.ItemLevel = this.ItemLevel;
            target.ObjectHash = this.ItemHash;
            target.ItemQuality = this.ItemQuality;
            target.IsPickupNoClick = this.IsPickupNoClick;
        }

        public void OnCreate(TrinityCacheObject source)
        {
            if (source.ActorType != ActorType.Item || source.CommonData == null)
                return;

            var item = source.Item;
            if (item == null || !item.IsValid)
                return;

            var acdItem = item.CommonData;
            if (acdItem == null || !acdItem.IsValid)
                return;

            //todo use faster methods from CachedItem
            this.DBItemType = acdItem.ItemType;
            this.DBItemBaseType = acdItem.ItemBaseType;
            this.FollowerType = acdItem.FollowerSpecialType;
            this.TrinityItemType = TrinityItemManager.DetermineItemType(source.InternalName, DBItemType, FollowerType);
            this.TrinityItemBaseType = TypeConversions.GetTrinityItemBaseType(this.TrinityItemType);
            this.IsEquipment = TypeConversions.GetIsEquipment(this.TrinityItemBaseType);
            this.IsTwoHanded = TypeConversions.GetIsTwoSlot(DBItemBaseType, DBItemType);
            this.ItemLevel = this.TrinityItemBaseType == TrinityItemBaseType.Gem ? acdItem.Level : (int)acdItem.GemQuality;
            this.ItemQuality = source.ActorAttributes.GetCachedAttribute<ItemQuality>(ActorAttributeType.ItemQualityLevel);
            this.IsPickupNoClick = DataDictionary.NoPickupClickItemTypes.Contains(this.TrinityItemType) || DataDictionary.NoPickupClickTypes.Contains(source.Type);
            this.IsMyDroppedItem = DropItems.DroppedItemAnnIds.Contains(source.AnnId);

            this.ItemHash = HashGenerator.GenerateItemHash(
                source.Position, 
                source.ActorSNO, 
                source.InternalName, 
                source.WorldSnoId, 
                this.ItemQuality, 
                this.ItemLevel);

            if (source.Type == TrinityObjectType.Gold)
                this.GoldAmount = acdItem.Gold;

            Update(source);
        }

        public void Update(TrinityCacheObject source)
        {
            _lastUpdated = DateTime.UtcNow;

            if (source.ActorType != ActorType.Item || source.CommonData == null)
                return;

            var item = source.Item;            
            if (item == null || !item.IsValid)
                return;

            var acdItem = item.CommonData;
            if(acdItem == null || !acdItem.IsValid)
                return;


        }

        public bool IsPickupNoClick { get; set; }
        public string ItemHash { get; set; }
        public int ItemLevel { get; set; }
        public ItemQuality ItemQuality { get; set; }
        public bool IsTwoHanded { get; set; }
        public bool IsEquipment { get; set; }
        public int GoldAmount { get; set; }
        public ItemType DBItemType { get; set; }
        public ItemBaseType DBItemBaseType { get; set; }
        public TrinityItemType TrinityItemType { get; set; }
        public TrinityItemBaseType TrinityItemBaseType { get; set; }
        public FollowerType FollowerType { get; set; }
        public bool IsMyDroppedItem { get; set; }
    }

    public class ItemPropertyUtils
    {

    }
}




