using System;
using System.Collections.Generic;
using Trinity.Coroutines.Town;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Sno;
using Trinity.Reference;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors.Properties
{
    public class ItemProperties
    {
        static ItemProperties()
        {
            ChangeEvents.WorldId.Changed += WorldId_Changed;
        }

        private static void WorldId_Changed(ChangeDetectorEventArgs<int> args)
        {
            if(!GameData.TownLevelAreaIds.Contains(args.OldValue) && !ZetaDia.IsInTown)
            {
                Logger.LogDebug($"Clearing ItemProperties Seen AnnIds");
                _seenActorAnnIds.Clear();
            }
        }

        private static HashSet<string> _seenActorAnnIds = new HashSet<string>();

        public static void Create(TrinityItem actor)
        {
            if (actor.ActorType != ActorType.Item)
                return;

            if (!actor.IsAcdBased || !actor.IsAcdValid)
                return;

            var commonData = actor.CommonData;
            var attributes = actor.Attributes;

            actor.InventorySlot = commonData.InventorySlot;
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

            var gbi = SnoManager.GameBalanceHelper.GetRecord<SnoGameBalanceItem>(SnoGameBalanceType.Items, actor.GameBalanceId);
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
            actor.IsCraftingReagent = actor.RawItemType == RawItemType.CraftingReagent || actor.RawItemType == RawItemType.CraftingReagent_Bound;
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
            actor.IsCosmeticItem= actor.RawItemType == RawItemType.CosmeticPet || actor.RawItemType == RawItemType.CosmeticPennant || actor.RawItemType == RawItemType.CosmeticPortraitFrame || actor.RawItemType == RawItemType.CosmeticWings;

            actor.IsLowQuality = actor.TrinityItemQuality == TrinityItemQuality.Common ||
                                 actor.TrinityItemQuality == TrinityItemQuality.Inferior ||
                                 actor.TrinityItemQuality == TrinityItemQuality.Magic ||
                                 actor.TrinityItemQuality == TrinityItemQuality.Rare;

            actor.GlobeType = GetGlobeType(actor);            
            actor.IsWeapon = TypeConversions.IsWeapon(actor);
            actor.IsArmor = TypeConversions.IsArmor(actor);

            actor.ObjectHash = HashGenerator.GenerateItemHash(
                actor.Position,
                actor.ActorSnoId,
                actor.InternalName,
                actor.WorldDynamicId,
                actor.ItemQualityLevel,
                actor.ItemLevel);

            if (actor.IsGroundItem)
            {
                actor.IsPickupNoClick = GameData.NoPickupClickItemTypes.Contains(actor.TrinityItemType) || GameData.NoPickupClickTypes.Contains(actor.Type);
                actor.IsMyDroppedItem = DropItems.DroppedItemAnnIds.Contains(actor.AnnId);
                actor.CanPickupItem = CanPickupItem(actor);
                actor.IsItemAssigned = actor.Attributes.ItemBoundToAnnId == 0 && actor.Attributes.ItemAssignedHero == ZetaDia.Service.Hero.HeroId;
            }

            if (actor.Type == TrinityObjectType.Gold)
                actor.GoldAmount = attributes.Gold;

            if (!_seenActorAnnIds.Contains(actor.PositionHash))
            {
                _seenActorAnnIds.Add(actor.PositionHash);

                if (actor.InventorySlot == InventorySlot.None)
                {
                    actor.OnDropped();
                }
            }
        }

        public static void Update(TrinityItem actor)
        {

            if (actor.ActorType != ActorType.Item)
                return;

            if (!actor.IsAcdBased || !actor.IsAcdValid)
                return;

            var commonData = actor.CommonData;
            actor.AcdId = commonData.AcdId;

            var col = commonData.InventoryColumn;
            var row = commonData.InventoryRow;
            var slot = commonData.InventorySlot;

            var columnChanged = col != actor.InventoryColumn;
            var rowChanged = row != actor.InventoryRow;
            var slotChanged = slot != actor.InventorySlot;

            actor.LastInventorySlot = actor.InventorySlot;
            actor.LastInventoryRow = actor.InventoryRow;
            actor.LastInventoryColumn = actor.InventoryColumn;

            actor.InventorySlot = slot;
            actor.InventoryRow = row;
            actor.InventoryColumn = col;

            if (!actor.IsEquipment)
            {
                actor.ItemStackQuantity = actor.Attributes.ItemStackQuantity;
            }

            if (actor.LastInventorySlot == InventorySlot.None && actor.InventorySlot == InventorySlot.BackpackItems)
            {
                UpdateBasicProperties(actor);
                actor.Attributes = new ItemAttributes(actor.FastAttributeGroupId);
                Create(actor);
                actor.OnPickedUp();
            }

            if (columnChanged || rowChanged || slotChanged)
            {
                actor.OnMoved();
            }

            if (actor.InventorySlot == InventorySlot.BackpackItems && actor.IsUnidentified && !actor.Attributes.IsUnidentified)
            {
                UpdateBasicProperties(actor);
                actor.Attributes = new ItemAttributes(actor.FastAttributeGroupId);
                Create(actor);
                actor.OnIdentified();               
            }
        }

        public static void UpdateBasicProperties(TrinityActor cacheObject)
        {
            cacheObject.ActorSnoId = cacheObject.CommonData.ActorSnoId;
            cacheObject.GameBalanceId = cacheObject.CommonData.GameBalanceId;
            cacheObject.FastAttributeGroupId = cacheObject.CommonData.FastAttributeGroupId;
            cacheObject.AnnId = cacheObject.CommonData.AnnId;
            cacheObject.AcdId = cacheObject.CommonData.AcdId;
        }


        public static GlobeTypes GetGlobeType(TrinityActor cacheObject)
        {
            switch (cacheObject.Type)
            {
                case TrinityObjectType.ProgressionGlobe:
                    if (GameData.GreaterProgressionGlobeSNO.Contains(cacheObject.ActorSnoId))
                        return GlobeTypes.GreaterRift;
                    return GlobeTypes.NephalemRift;

                case TrinityObjectType.PowerGlobe:
                    return GlobeTypes.Power;

                case TrinityObjectType.HealthGlobe:
                    return GlobeTypes.Health;
            }
            return GlobeTypes.None;
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

        public static string GetName(int gameBalanceId) => SnoManager.StringListHelper.GetStringListValue(SnoStringListType.Items, gameBalanceId);

        public static bool CanPickupItem(TrinityItem actor)
        {
            if (actor.InventorySlot != InventorySlot.None)
                return false;

            if (actor.Attributes != null)
            {
                if (actor.Attributes.ItemBoundToAnnId != 0)
                {
                    return actor.Attributes.ItemBoundToAnnId == Core.Actors.Me?.AnnId;
                }

                if (actor.Attributes.ItemAssignedHero == ZetaDia.Service.Hero?.HeroId)
                {
                    return true;
                }
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




