using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Coroutines.Town;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Actors.Properties;
using Trinity.Framework.Events;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors.ActorTypes
{
    public class TrinityItem : TrinityActor
    {
        // Lootable Actors
        public TrinityItem(DiaObject seed) : base(seed)
        {
        }

        public TrinityItem(ACD seed) : base(seed, ActorType.Item)
        {
            Attributes = new AttributesWrapper(seed);
        }

        public bool IsCosmeticItem => RawItemType == RawItemType.CosmeticPet ||
                                      RawItemType == RawItemType.CosmeticPennant ||
                                      RawItemType == RawItemType.CosmeticPortraitFrame ||
                                      RawItemType == RawItemType.CosmeticWings;

        public bool IsLowQuality => TrinityItemQuality == TrinityItemQuality.Common ||
                                    TrinityItemQuality == TrinityItemQuality.Inferior ||
                                    TrinityItemQuality == TrinityItemQuality.Magic ||
                                    TrinityItemQuality == TrinityItemQuality.Rare;

        public new AttributesWrapper Attributes { get; private set; }
        public bool IsFirstUpdate { get; set; } = true;

        // ItemProperties.cs
        public InventorySlot InventorySlot { get; internal set; }
        public int InventoryColumn { get; internal set; }
        public int InventoryRow { get; internal set; }
        public bool IsGroundItem => IsItem &&
                                    InventorySlot == InventorySlot.None &&
                                    Position != Vector3.Zero;

        public InventorySlot LastInventorySlot { get; set; }
        public int LastInventoryColumn { get; set; }
        public int LastInventoryRow { get; set; }
        public InventorySquare InventorySquare { get; set; }
        public int GoldAmount => Type == TrinityObjectType.Gold ? Attributes?.Gold ?? 0 : 0;
        public bool IsUnidentified => Attributes?.IsUnidentified ?? false;
        public bool IsPrimalAncient => Attributes?.IsPrimalAncient ?? false;
        public bool IsAncient => Attributes?.IsAncient ?? false;
        public int RequiredLevel => Math.Max(Attributes?.RequiredLevel ?? 0, Attributes?.ItemLegendaryItemLevelOverride ?? 0);
        public bool IsCrafted => Attributes?.IsCrafted ?? false;
        public bool IsVendorBought => Attributes?.IsVendorBought ?? false;
        public bool IsAccountBound => (Attributes?.ItemBoundToACDId ?? 0) > 0;

        // TODO: This is still accessed to often...
        private PerFrameCachedValue<ACDItem.ItemRecord?> _gbiCache;
        public ACDItem.ItemRecord? Gbi => (_gbiCache ?? (
                                               _gbiCache = new PerFrameCachedValue<ACDItem.ItemRecord?>(
                                                   () => SNORecordGameBalance.GetGameBalanceRecord<ACDItem.ItemRecord>(
                                                       GameBalanceId,
                                                       GameBalanceType.Items))
                                               )
                                           ).Value;


        public int MaxStackCount => Gbi?.StackSize ?? 0;
        public GemType GemType => Gbi?.GemType ?? 0;
        public RawItemType RawItemType => (RawItemType)(Gbi?.ItemTypeGBId ?? 0);
        public GemQuality GemQuality => Attributes?.GemQuality ?? GemQuality.Normal;
        public ItemQuality ItemQualityLevel => Attributes?.ItemQualityLevel ?? ItemQuality.Invalid;
        public ItemType ItemType => TypeConversions.GetItemType(RawItemType);
        public ItemBaseType ItemBaseType => TypeConversions.GetItemBaseType(ItemType);
        public TrinityItemType TrinityItemType => TypeConversions.GetTrinityItemType(RawItemType, GemType);
        public TrinityItemBaseType TrinityItemBaseType => TypeConversions.GetTrinityItemBaseType(TrinityItemType);
        public bool IsGem => RawItemType == RawItemType.Gem ||
                             RawItemType == RawItemType.UpgradeableJewel;

        public bool IsCraftingReagent => RawItemType == RawItemType.CraftingReagent ||
                                         RawItemType == RawItemType.CraftingReagent_Bound;

        public bool IsGold => RawItemType == RawItemType.Gold;
        public bool IsEquipment => TypeConversions.GetIsEquipment(TrinityItemBaseType);
        public bool IsClassItem => TypeConversions.GetIsClassItem(ItemType);
        public bool IsOffHand => TypeConversions.GetIsOffhand(ItemType);
        public bool IsPotion => RawItemType == RawItemType.HealthPotion;
        public bool IsSalvageable => IsEquipment &&
                                     !IsVendorBought && RequiredLevel > 1 ||
                                     IsPotion;

        public bool IsLegendaryGem => RawItemType == RawItemType.UpgradeableJewel;
        public bool IsMiscItem => ItemBaseType == ItemBaseType.Misc;
        public bool IsTwoSquareItem => TypeConversions.GetIsTwoSlot(ItemBaseType, ItemType);
        public bool CanPickupItem => ItemProperties.CanPickupItem(this);
        public TimeSpan ItemTradeEndTime { get; set; }
        public List<int> TradablePlayers { get; set; }

        /// <summary>
        /// If item is assigned - only visible to this player (bounty bag items)
        /// </summary>
        public bool IsItemAssigned => !IsAccountBound &&
                                      (Attributes?.ItemAssignedHero ?? 0) == ZetaDia.Service.Hero.HeroId;

        /// <summary>
        /// If the item can be picked up automatically by walking close by (globes)
        /// </summary>
        public bool IsPickupNoClick => GameData.NoPickupClickItemTypes.Contains(TrinityItemType) ||
                                       GameData.NoPickupClickTypes.Contains(Type);

        /// <summary>
        /// If the item was dropped by this player
        /// </summary>
        public bool IsMyDroppedItem => TrinityTownRun.DroppedItemAnnIds.Contains(AnnId);

        public GlobeTypes GlobeType => ItemProperties.GetGlobeType(this);
        public FollowerType FollowerType => ItemProperties.GetFollowerType(ActorSnoId);
        public int ItemStackQuantity => Attributes?.ItemStackQuantity ?? 0;

        /// <summary>
        /// If item can be picked up by any other players
        /// </summary>
        public bool IsTradeable { get; set; }

        public bool IsWeapon => TypeConversions.IsWeapon(this);
        public bool IsArmor => TypeConversions.IsArmor(this);
        public TrinityItemQuality TrinityItemQuality => TypeConversions.GetTrinityItemQuality(ItemQualityLevel);

        public override void OnCreated()
        {
            Attributes = new AttributesWrapper(CommonData);
            CommonProperties.Populate(this);
            ItemProperties.Create(this);
        }

        public override void OnUpdated()
        {
            if (InventorySlot == InventorySlot.SharedStash &&
                !Core.Player.IsInTown)
            {
                return;
            }

            Attributes.Update(CommonData);
            ItemProperties.Update(this);
            CommonProperties.Update(this);
        }

        public void OnMoved()
        {
            ItemEvents.FireItemMoved(this);
            Core.Logger.Log($"{Name} was moved from [{LastInventoryColumn},{LastInventoryRow} {LastInventorySlot}] => [{InventoryColumn},{InventoryRow} {InventorySlot}] (Ann={AnnId} AcdId={AcdId})");
        }

        public void OnIdentified()
        {
            ItemEvents.FireItemIdentified(this);
            Core.Logger.Log($"{Name} was identified. (Ann={AnnId} AcdId={AcdId} GbId={GameBalanceId}) Ancient={IsAncient} RawType={RawItemType}");
        }

        public void OnPickedUp()
        {
            Core.Logger.Log($"{Name} was picked up. (SnoId={ActorSnoId} Ann={AnnId} AcdId={AcdId} GbId={GameBalanceId}) InternalName={InternalName} Quality={ItemQualityLevel} Ancient={IsAncient} RawType={RawItemType}");
            ItemEvents.FireItemPickedUp(this);
        }

        public void OnDropped()
        {
            if (IsPickupNoClick) return;

            if (IsPrimalAncient)
                Core.Logger.Warn($"Primal {Name} dropped. (SnoId={ActorSnoId} Ann={AnnId} AcdId={AcdId} GbId={GameBalanceId}) InternalName={InternalName} Quality={ItemQualityLevel} Ancient={IsAncient} Primal={IsPrimalAncient} RawType={RawItemType}");
            else
                Core.Logger.Log($"{Name} dropped. (SnoId={ActorSnoId} Ann={AnnId} AcdId={AcdId} GbId={GameBalanceId}) InternalName={InternalName} Quality={ItemQualityLevel} Ancient={IsAncient} Primal={IsPrimalAncient} RawType={RawItemType}");

            ItemEvents.FireItemDropped(this);
        }

        public bool Drop()
        {
            var acdItem = ToAcdItem();
            if (IsAcdItemValid(acdItem))
            {
                return acdItem.Drop();
            }
            return false;
        }

        public void Socket(ACDItem gem)
        {
            var acdItem = ToAcdItem();
            if (IsAcdItemValid(acdItem) && IsAcdItemValid(gem))
            {
                acdItem.Socket(gem);
            }
        }

        public Item Reference => Legendary.GetItem(this);

        public ACDItem ToAcdItem() => Core.Actors.GetAcdItemByAnnId(AnnId);

        private bool IsAcdItemValid(ACDItem acdItem) => acdItem != null && acdItem.IsValid && !acdItem.IsDisposed;

        public bool IsProtected() => CharacterSettings.Instance.ProtectedBagSlots.Any(sq => sq.Row == InventoryRow && sq.Column == InventoryColumn);

        public override string ToString() => $"{GetType().Name}: AcdId={AcdId}, {InventorySlot}, [{InventoryColumn},{InventoryRow}], {Name}";

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + AnnId.GetHashCode();
            hash = (hash * 7) + ActorSnoId.GetHashCode();
            return hash;
        }

        public override ItemQuality GetItemQualityLevel()
        {
            return ItemQualityLevel;
        }
    }

}


