using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Trinity.Coroutines.Town;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.Attributes;
using Trinity.Framework.Objects.Memory.Items;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Sno;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Objects.Native;
using Trinity.Reference;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using ActorCommonData = Trinity.Framework.Objects.Memory.Misc.ActorCommonData;
using ActorType = Zeta.Game.Internals.SNO.ActorType;
using FollowerType = Zeta.Game.Internals.Actors.FollowerType;
using GameBalanceType = Zeta.Game.Internals.SNO.GameBalanceType;
using GemType = Zeta.Game.Internals.Actors.GemType;
using ItemQuality = Zeta.Game.Internals.Actors.ItemQuality;
using Logger = Trinity.Technicals.Logger;


namespace Trinity.Framework.Actors
{
    public interface IItem
    {
        int AcdId { get; }
        int AnnId { get; }
        int ActorSnoId { get; }
        long ItemStackQuantity { get; }
        string Name { get; }
        int GameBalanceId { get; }
        string InternalName { get; }
        InventorySlot InventorySlot { get; }
        RawItemType RawItemType { get; }
        ItemBaseType ItemBaseType { get; }
        bool IsUnidentified { get; }
        bool IsAncient { get; }
        int MaxStackCount { get; }
        int InventoryColumn { get; }
        int InventoryRow { get; }
        bool IsTwoSquareItem { get; }
        bool IsTradeable { get; }
    }

    public class CachedItem : IComparable, IItem
    {
        public DateTime CreationTime { get; set; }
        public ItemAttributes Attributes { get; set; }
        public DateTime LastRefreshTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public uint LastUpdatedFrame { get; set; }
        public ActorCommonData Acd { get; set; }
        public int AcdId { get; set; }
        public int AnnId { get; }
        public int ActorSnoId { get; }
        public long ItemStackQuantity => Attributes.ItemStackQuantity;
        public string Name { get; set; }
        public int RequiredLevel { get; set; }
        public int GameBalanceId { get; set; }
        public string InternalName { get; set; }
        public uint CreationFrame { get; set; }
        public int FastAttributeGroupId { get; set; }
        public ItemQuality ItemQualityLevel { get; set; }
        public ItemFlags Flags { get; set; }
        public GameBalanceType GameBalanceType { get; set; }
        public TrinityItemType TrinityItemType { get; set; }
        public TrinityItemBaseType TrinityItemBaseType { get; set; }
        public FollowerType FollowerSpecialType { get; set; }
        public GemQuality GemQuality { get; set; }
        public ItemBaseType ItemBaseType { get; set; }
        public ItemType ItemType { get; set; }
        public InventorySlot InventorySlot { get; set; }
        public RawItemType RawItemType { get; set; }
        public bool IsTradeable => Attributes.IsTradeable;
        public bool IsUnidentified { get; set; }
        public bool IsAncient { get; set; }
        public bool IsTwoSquareItem { get; set; }
        public int ItemLevel { get; set; }
        public int MaxStackCount { get; set; }
        public bool IsGold { get; set; }
        public int InventoryColumn { get; set; }
        public int InventoryRow { get; set; }
        public bool IsGem { get; set; }
        public bool IsCraftingReagent { get; set; }
        public bool IsPotion { get; set; }
        public bool IsMiscItem { get; set; }
        public bool IsSalvageable { get; set; }
        public bool IsOffHand { get; set; }
        public bool IsClassItem { get; set; }
        public bool IsEquipment { get; set; }
        public bool IsVendorBought { get; set; }
        public bool IsCrafted { get; set; }
        public bool IsLegendaryGem { get; set; }
        public bool IsAccountBound { get; set; }
        public GemType GemType { get; set; }
        public ActorType ActorType { get; set; }

        public CachedItem(IntPtr ptr) : this(MemoryWrapper.Create<ActorCommonData>(ptr))
        {

        }

        public CachedItem(ActorCommonData acd)
        {
            CreationTime = DateTime.UtcNow;
            LastRefreshTime = DateTime.UtcNow;
            Acd = acd;
            AnnId = acd.AnnId;
            ActorSnoId = acd.ActorSnoId;
            AcdId = acd.AcdId;
            Refresh(acd);
            OnCreated();
        }

        public void Refresh()
        {
            Refresh(ActorManager.GetAcdByAnnId(AnnId));
        }

        private void Refresh(ActorCommonData acd)
        {
            if (!acd.IsValid || acd.IsDisposed)
                return;

            AcdId = acd.AcdId;

            ActorType = acd.ActorType;
            GameBalanceId = acd.GameBalanceId;
            GameBalanceType = acd.GameBalanceType;
            InventorySlot = acd.InventorySlot;
            InventoryColumn = acd.InventoryColumn;
            InventoryRow = acd.InventoryRow;
            FastAttributeGroupId = acd.FastAttributeGroupId;
            Attributes = new ItemAttributes(FastAttributeGroupId);
            IsUnidentified = Attributes.IsUnidentified;
            IsAncient = Attributes.IsAncient;
            ItemQualityLevel = Attributes.ItemQualityLevel;
            //ItemStackQuantity = Attributes.ItemStackQuantity;
            RequiredLevel = Math.Max(Attributes.RequiredLevel, Attributes.ItemLegendaryItemLevelOverride);
            IsCrafted = Attributes.IsCrafted;
            IsVendorBought = Attributes.IsVendorBought;
            IsAccountBound = Attributes.ItemBoundToACD > 0;

            var realname = GetName(GameBalanceId);
            Name = string.IsNullOrEmpty(realname) ? InternalName : realname;

            var gbi = SnoManager.GameBalance.GetRecord<SnoGameBalanceItem>(SnoGameBalanceType.Items, GameBalanceId);
            ItemLevel = RequiredLevel; //gbi.ItemLevel;
            InternalName = gbi.InternalName;
            MaxStackCount = gbi.StackSize;
            GemType = gbi.GemType;
            RawItemType = (RawItemType)gbi.ItemTypesGameBalanceId;
            GemQuality = Attributes.GemQuality;
            ItemType = TypeConversions.GetItemType(RawItemType);
            ItemBaseType = TypeConversions.GetItemBaseType(ItemType);
            TrinityItemType = TypeConversions.GetTrinityItemType(RawItemType, GemType);
            TrinityItemBaseType = TypeConversions.GetTrinityItemBaseType(TrinityItemType);
            IsGem = RawItemType == RawItemType.Gem || RawItemType == RawItemType.UpgradeableJewel;
            IsCraftingReagent = RawItemType == RawItemType.CraftingReagent || RawItemType == RawItemType.CraftingReagentBound;
            IsGold = RawItemType == RawItemType.Gold;
            IsEquipment = TypeConversions.GetIsEquipment(TrinityItemBaseType);
            IsClassItem = TypeConversions.GetIsClassItem(ItemType);
            IsOffHand = TypeConversions.GetIsOffhand(ItemType);
            IsPotion = RawItemType == RawItemType.HealthPotion;
            IsSalvageable = IsEquipment && !IsVendorBought && RequiredLevel > 1 || IsPotion;
            IsLegendaryGem = RawItemType == RawItemType.UpgradeableJewel;
            IsMiscItem = ItemBaseType == ItemBaseType.Misc;
            IsTwoSquareItem = TypeConversions.GetIsTwoSlot(ItemBaseType, ItemType, RawItemType);
            LastRefreshTime = DateTime.UtcNow;
        }

        public string GetName(int gameBalanceId)
        {
            return SnoManager.StringList.GetStringListValue(SnoStringListType.Items, gameBalanceId);
        }

        public override string ToString()
        {
            return string.Format($"{ActorSnoId}: {Name}, {ItemType}");
        }

        public bool IsProtected()
        {
            return CharacterSettings.Instance.ProtectedBagSlots.Any(sq => sq.Row == InventoryRow && sq.Column == InventoryColumn);
        }

        public bool IsValid
        {
            get
            {
                if (Acd == null || !Acd.IsValid)
                {
                    return false;
                }
                if (Acd.IsDisposed)
                {
                    Acd.Update(IntPtr.Zero);
                    return false;
                }
                return true;
            }
        }
       
        public Item Reference => Legendary.GetItem(this);

        public override bool Equals(object obj)
        {
            var other = obj as CachedItem;
            return other != null && other.AnnId == AnnId;
        }

        protected bool Equals(CachedItem other)
        {
            return AnnId == other.AnnId && ActorSnoId == other.ActorSnoId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AnnId * 397) ^ ActorSnoId;
            }
        }

        public int CompareTo(object obj)
        {
            var item = (CachedItem)obj;

            if (InventoryRow < item.InventoryRow)
                return -1;
            if (InventoryColumn < item.InventoryColumn)
                return -1;
            if (InventoryColumn == item.InventoryColumn && InventoryRow == item.InventoryRow)
                return 0;
            return 1;
        }

        public void Update(ActorCommonData newAcd)
        {
            Acd = newAcd;
            AcdId = newAcd.AcdId;

            var inventorySlot = Acd.InventorySlot;
            var inventoryColumn = Acd.InventoryColumn;
            var inventoryRow = Acd.InventoryRow;
            var columnChanged = inventoryColumn != InventoryColumn;
            var rowChanged = inventoryRow != InventoryRow;
            var slotChanged = inventorySlot != InventorySlot;

            if (columnChanged || rowChanged || slotChanged)
            {
                if (slotChanged && InventorySlot == InventorySlot.None && inventorySlot == InventorySlot.BackpackItems)
                {
                    Refresh(Acd);
                    OnPickedUp();
                }

                InventorySlot = inventorySlot;
                InventoryColumn = inventoryColumn;
                InventoryRow = inventoryRow;
                OnMoved();
            }

            if (InventorySlot == InventorySlot.BackpackItems && IsUnidentified)
            {
                if (!Attributes.IsUnidentified)
                {
                    Refresh(Acd);
                    OnIdentified();
                }
            }

        }

        public void OnPickedUp()
        {
            ItemEvents.FireItemPickedUp(this);

            if (ItemQualityLevel >= ItemQuality.Legendary)
                Logger.Warn($"Legendary found: {Name}. (Ann={AnnId} AcdId={AcdId}) Ancient={IsAncient} IsUnidentified={IsUnidentified} RawType={RawItemType}");
        }

        public void OnDestroyed()
        {
            Logger.LogSpecial(() => $"{Name} is no longer valid or is disposed. (Ann={AnnId} ACDid={AcdId})");
        }

        public void OnCreated()
        {
            Logger.LogSpecial(() => $"{Name} was created (Ann={AnnId} AcdId={AcdId}) Ancient={IsAncient} Unidentified={IsUnidentified} RawType={RawItemType} Quality={ItemQualityLevel}");
        }

        public void OnMoved()
        {
            ItemEvents.FireItemMoved(this);
            Logger.LogSpecial(() => $"{Name} was moved to [Col={InventoryColumn}, Row={InventoryRow}, Slot={InventorySlot}] (Ann={AnnId} AcdId={AcdId})");
        }
        public void OnIdentified()
        {
            ItemEvents.FireItemIdentified(this);
            Logger.LogSpecial(() => $"{Name} was identified. (Ann={AnnId} AcdId={AcdId}) Ancient={IsAncient} RawType={RawItemType}");
        }

        public bool Drop()
        {
            var acdItem = GetAcdItem();
            if (IsProperValid(acdItem))
            {
                return acdItem.Drop();
            }
            return false;
        }

        public void Socket(ACDItem gem)
        {
            var acdItem = GetAcdItem();
            if (IsProperValid(acdItem) && IsProperValid(gem))
            {
                acdItem.Socket(gem);
            }
        }

        private static bool IsProperValid(ACDItem acdItem)
        {
            return acdItem != null && acdItem.IsValid && !acdItem.IsDisposed;
        }

        public ACDItem GetAcdItem()
        {
            return ActorManager.GetAcdItemByAnnId(AnnId);
        }

        public bool CanPickupItem()
        {
            if (InventorySlot != InventorySlot.None)
                return false;

            if (Attributes.ItemBoundToACD != 0)
            {
                return Attributes.ItemBoundToACD == ZetaDia.ActivePlayerACDId;
            }

            if (Attributes.ItemAssignedHero == ZetaDia.Service.Hero.HeroId)
            {
                return true;
            }

            if (ItemQualityLevel >= ItemQuality.Legendary || IsCraftingReagent)
            {
                return Attributes.IsTradeable && Attributes.ItemTradePlayerLow.Contains(Core.Hero.PlayerTradeId);
            }

            if (IsEquipment && ItemQualityLevel <= ItemQuality.Rare6)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Items that are not visible to others - like loot from bounty caches
        /// </summary>
        public bool IsItemAssigned => Attributes.ItemBoundToACD == 0 && Attributes.ItemAssignedHero == ZetaDia.Service.Hero.HeroId;
    }
}

