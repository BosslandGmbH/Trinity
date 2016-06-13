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
    /// PropertyLoader that are specific to items (on the ground only)
    /// </summary>
    public class ItemProperties : PropertyLoader.IPropertyCollection
    {
        private DateTime _lastUpdated = DateTime.MinValue;
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(100);

        public void ApplyTo(TrinityCacheObject target)
        {
            if (DateTime.UtcNow.Subtract(_lastUpdated).TotalMilliseconds > UpdateInterval.TotalMilliseconds)
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

            this.ItemHash = HashGenerator.GenerateItemHash(
                source.Position, 
                source.ActorSNO, 
                source.InternalName, 
                source.WorldSnoId, 
                this.ItemQuality, 
                this.ItemLevel);

            if (source.Type == TrinityObjectType.Gold)
                this.GoldAmount = acdItem.Gold;
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




