using System;
using Trinity.Coroutines.Town;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Sno;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Trinity.Technicals;

namespace Trinity.Framework.Actors.Properties
{
    public class ItemProperties
    {
        public static void Create(TrinityItem actor)
        {
            if (actor.ActorType != ActorType.Item)
                return;

            if (!actor.IsAcdBased || !actor.IsAcdValid)
                return;

            var commonData = actor.CommonData;
            var attributes = actor.Attributes;
            
            actor.InventoryColumn = commonData.InventoryColumn;
            actor.InventoryRow = commonData.InventoryRow;
            actor.IsUnidentified = attributes.IsUnidentified;
            actor.IsAncient = attributes.IsAncient;
            actor.ItemQualityLevel = attributes.ItemQualityLevel;
            //ItemStackQuantity = Attributes.ItemStackQuantity;
            actor.RequiredLevel = Math.Max(actor.Attributes.RequiredLevel, actor.Attributes.ItemLegendaryItemLevelOverride);
            actor.IsCrafted = attributes.IsCrafted;
            actor.IsVendorBought = attributes.IsVendorBought;
            actor.IsAccountBound = attributes.ItemBoundToAnnId > 0;
            actor.IsTradeable = attributes.IsTradeable;

            var realname = GetName(actor.GameBalanceId);
            actor.Name = string.IsNullOrEmpty(realname) ? actor.InternalName : realname;

            var gbi = SnoManager.GameBalance.GetRecord<SnoGameBalanceItem>(SnoGameBalanceType.Items, actor.GameBalanceId);
            actor.ItemLevel = actor.RequiredLevel; //gbi.ItemLevel;
            actor.InternalName = gbi.InternalName;
            actor.MaxStackCount = gbi.StackSize;
            actor.GemType = gbi.GemType;
            actor.RawItemType = (RawItemType)gbi.ItemTypesGameBalanceId;
            actor.GemQuality = attributes.GemQuality;
            actor.ItemType = TypeConversions.GetItemType(actor.RawItemType);
            actor.ItemBaseType = TypeConversions.GetItemBaseType(actor.ItemType);
            actor.TrinityItemType = TypeConversions.GetTrinityItemType(actor.RawItemType, actor.GemType);
            actor.TrinityItemBaseType = TypeConversions.GetTrinityItemBaseType(actor.TrinityItemType);
            actor.IsGem = actor.RawItemType == RawItemType.Gem || actor.RawItemType == RawItemType.UpgradeableJewel;
            actor.IsCraftingReagent = actor.RawItemType == RawItemType.CraftingReagent || actor.RawItemType == RawItemType.CraftingReagentBound;
            actor.IsGold = actor.RawItemType == RawItemType.Gold;
            actor.IsEquipment = TypeConversions.GetIsEquipment(actor.TrinityItemBaseType);
            actor.IsClassItem = TypeConversions.GetIsClassItem(actor.ItemType);
            actor.IsOffHand = TypeConversions.GetIsOffhand(actor.ItemType);
            actor.IsPotion = actor.RawItemType == RawItemType.HealthPotion;
            actor.IsSalvageable = actor.IsEquipment && !actor.IsVendorBought && actor.RequiredLevel > 1 || actor.IsPotion;
            actor.IsLegendaryGem = actor.RawItemType == RawItemType.UpgradeableJewel;
            actor.IsMiscItem = actor.ItemBaseType == ItemBaseType.Misc;
            actor.IsTwoSquareItem = TypeConversions.GetIsTwoSlot(actor.ItemBaseType, actor.ItemType);
            actor.FollowerType = GetFollowerType(actor.ActorSnoId);
            actor.ItemStackQuantity = attributes.ItemStackQuantity;
            actor.TrinityItemQuality = TypeConversions.GetTrinityItemQuality(actor.ItemQualityLevel);

            actor.ObjectHash = HashGenerator.GenerateItemHash(
                actor.Position,
                actor.ActorSnoId,
                actor.InternalName,
                actor.WorldSnoId,
                actor.ItemQualityLevel,
                actor.ItemLevel);

            if (actor.IsGroundItem)
            {
                actor.IsPickupNoClick = DataDictionary.NoPickupClickItemTypes.Contains(actor.TrinityItemType) || DataDictionary.NoPickupClickTypes.Contains(actor.Type);
                actor.IsMyDroppedItem = DropItems.DroppedItemAnnIds.Contains(actor.AnnId);
                actor.CanPickupItem = CanPickupItem(actor);
                actor.IsItemAssigned = actor.Attributes.ItemBoundToAnnId == 0 && actor.Attributes.ItemAssignedHero == ZetaDia.Service.Hero.HeroId;
            }

            if (actor.Type == TrinityObjectType.Gold)
                actor.GoldAmount = attributes.Gold;
        }

        public static void Update(TrinityItem actor)
        {
            if (actor.ActorType != ActorType.Item)
                return;

            if (!actor.IsAcdBased || !actor.IsAcdValid)
                return;

            var commonData = actor.CommonData;
            actor.AcdId = commonData.AcdId;

            var slot = commonData.InventorySlot;
            var col = commonData.InventoryColumn;
            var row = commonData.InventoryRow;

            var slotChanged = slot != actor.InventorySlot;
            var columnChanged = col != actor.InventoryColumn;
            var rowChanged = row != actor.InventoryRow;

            actor.ItemStackQuantity = actor.Attributes.ItemStackQuantity;

            if (columnChanged || rowChanged || slotChanged)
            {
                if (slotChanged && actor.InventorySlot == InventorySlot.None && slot == InventorySlot.BackpackItems)
                {
                    actor.Attributes = new ItemAttributes(actor.FastAttributeGroupId);
                    Create(actor);
                    actor.OnPickedUp();
                }

                actor.InventoryRow = row;
                actor.InventoryColumn = col;
                actor.InventorySlot = slot;

                actor.OnMoved();
            }

            if (actor.InventorySlot == InventorySlot.BackpackItems && actor.IsUnidentified && !actor.Attributes.IsUnidentified)
            {
                actor.Attributes = new ItemAttributes(actor.FastAttributeGroupId);
                Create(actor);
                actor.OnIdentified();               
            }
        }


        public static FollowerType GetFollowerType(int actorSnoId)
        {
            switch (actorSnoId)
            {
                case 363893:
                case 192942:
                case 4482: return FollowerType.Enchantress;
                case 363891:
                case 192940:
                case 52693: return FollowerType.Templar;
                case 363892:
                case 192941:
                case 52694: return FollowerType.Scoundrel;
            }
            return FollowerType.None;
        }

        public static string GetName(int gameBalanceId) => SnoManager.StringList.GetStringListValue(SnoStringListType.Items, gameBalanceId);

        public static bool CanPickupItem(TrinityItem actor)
        {
            if (actor.InventorySlot != InventorySlot.None)
                return false;

            if (actor.Attributes.ItemBoundToAnnId != 0)
            {
                return actor.Attributes.ItemBoundToAnnId == Core.Actors.Me.AnnId;
            }

            if (actor.Attributes.ItemAssignedHero == ZetaDia.Service.Hero.HeroId)
            {
                return true;
            }

            if (actor.ItemQualityLevel >= ItemQuality.Legendary || actor.IsCraftingReagent)
            {
                return !actor.Attributes.IsTradeable || actor.Attributes.ItemTradePlayerLow.Contains(Core.MemoryModel.Hero.PlayerTradeId);
            }

            if (actor.IsEquipment && actor.ItemQualityLevel <= ItemQuality.Rare6)
            {
                return true;
            }

            return false;
        }

    }
}




