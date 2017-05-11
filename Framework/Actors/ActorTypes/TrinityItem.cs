using System.Linq;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Actors.Properties;
using Trinity.Framework.Events;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Game.Internals.Actors;
using Zeta.Game;

namespace Trinity.Framework.Actors.ActorTypes
{
    public class TrinityItem : TrinityActor
    {
        public bool IsCosmeticItem { get; set; }
        public bool IsLowQuality { get; set; }
        public new AttributesWrapper Attributes { get; set; }
        public bool IsFirstUpdate { get; set; } = true;
        public int InventoryColumn { get; set; }
        public int InventoryRow { get; set; }
        public int LastInventoryColumn { get; set; }
        public int LastInventoryRow { get; set; }
        public InventorySquare InventorySquare { get; set; }
        public int GoldAmount { get; set; }
        public bool IsUnidentified { get; set; }
        public bool IsPrimalAncient { get; set; }
        public bool IsAncient { get; set; }
        public int RequiredLevel { get; set; }
        public bool IsCrafted { get; set; }
        public bool IsVendorBought { get; set; }
        public bool IsAccountBound { get; set; }
        public int ItemLevel { get; set; }
        public int MaxStackCount { get; set; }
        public GemType GemType { get; set; }
        public RawItemType RawItemType { get; set; }
        public GemQuality GemQuality { get; set; }
        public bool IsGem { get; set; }
        public bool IsCraftingReagent { get; set; }
        public bool IsGold { get; set; }
        public bool IsEquipment { get; set; }
        public bool IsClassItem { get; set; }
        public bool IsOffHand { get; set; }
        public bool IsPotion { get; set; }
        public bool IsSalvageable { get; set; }
        public bool IsLegendaryGem { get; set; }
        public bool IsMiscItem { get; set; }
        public bool IsTwoSquareItem { get; set; }
        public bool CanPickupItem { get; set; }

        /// <summary>
        /// If item is assigned - only visible to this player (bounty bag items)
        /// </summary>
        public bool IsItemAssigned { get; set; }

        /// <summary>
        /// If the item can be picked up automatically by walking close by (globes)
        /// </summary>
        public bool IsPickupNoClick { get; set; }

        /// <summary>
        /// If the item was dropped by this player
        /// </summary>
        public bool IsMyDroppedItem { get; set; }
        public GlobeTypes GlobeType { get; set; }
        public FollowerType FollowerType { get; set; }
        public int ItemStackQuantity { get; set; }

        /// <summary>
        /// If item can be picked up by any other players
        /// </summary>
        public bool IsTradeable { get; set; }
        public bool IsWeapon { get; set; }
        public bool IsArmor { get; set; }
        public TrinityItemQuality TrinityItemQuality { get; set; }

        public override void OnCreated()
        {
            Attributes = new AttributesWrapper(CommonData);
            CommonProperties.Populate(this);
            ItemProperties.Create(this);
        }

        public override void OnUpdated()
        {
            if (InventorySlot == InventorySlot.SharedStash && !Core.Player.IsInTown)
                return;

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

    }

}


