using System;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Reference;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects
{
    [DataContract(Namespace = "")]
    public class Item : IUnique, IEquatable<Item>
    {
        [DataMember]
        public int Id { get; set; }

        public string Name { get; set; }

        public ItemType ItemType { get; set; }

        public ItemQuality Quality { get; set; }
        public ItemBaseType BaseType { get; set; }

        public string Slug { get; set; }
        public string InternalName { get; set; }
        public string RelativeUrl { get; set; }
        public string DataUrl { get; set; }
        public string Url { get; set; }
        public string LegendaryAffix { get; set; }
        public string SetName { get; set; }
        public bool IsCrafted { get; set; }

        public Item()
        {
        }

        public Item(int actorId, string name = "", ItemType itemType = ItemType.Unknown)
        {
            Id = actorId;
            Name = name;
            ItemType = itemType;
        }

        public Item(ACDItem acdItem)
        {
            Id = acdItem.ActorSnoId;
            Name = acdItem.Name;
            ItemType = acdItem.ItemType;
        }

        public TrinityItem GetEquippedItem() => Core.Inventory.Equipped.FirstOrDefault(u => u.ActorSnoId == Id);

        /// <summary>
        /// If this item is currently equipped
        /// </summary>
        public bool IsEquipped
        {
            get { return Core.Inventory.PlayerEquippedIds.Contains(Id) || IsEquippedInCube; }
        }

        /// <summary>
        /// Item is one of the three selected in Kanais cube
        /// </summary>
        public bool IsEquippedInCube
        {
            get { return Core.Inventory.KanaisCubeIds.Contains(Id); }
        }

        /// <summary>
        /// If this item belongs to a set
        /// </summary>
        public bool IsSetItem
        {
            get { return Sets.SetItemIds.Contains(Id); }
        }

        public bool IsLegendaryAffixed
        {
            get { return !string.IsNullOrEmpty(LegendaryAffix); }
        }

        /// <summary>
        /// The set this item belongs to, if applicable.
        /// </summary>
        public Set Set
        {
            get
            {
                var set = Sets.ToList().FirstOrDefault(s => s.ItemIds.Contains(Id));
                return set ?? new Set();
            }
        }

        /// <summary>
        /// If the associated buff or minion is currently active
        /// </summary>
        public bool IsBuffActive
        {
            get
            {
                if (!IsEquipped) return false;

                // Item Creates Buff
                SNOPower power;
                if (GameData.PowerByItem.TryGetValue(this, out power))
                    return Core.Player.HasBuff(power);

                // Item Spawns Minions
                string internalNameToken;
                if (GameData.MinionInternalNameTokenByItem.TryGetValue(this, out internalNameToken))
                    return ZetaDia.Actors.GetActorsOfType<DiaUnit>().Any(u => u.PetType > 0 && u.Name.Contains(internalNameToken));

                return false;
            }
        }

        public ActorClass ClassRestriction
        {
            get { return GetClassRestriction(TrinityItemType); }
        }

        public static ActorClass GetClassRestriction(TrinityItemType type)
        {
            switch (type)
            {
                case TrinityItemType.Flail:
                case TrinityItemType.CrusaderShield:
                case TrinityItemType.TwoHandFlail:
                    return ActorClass.Crusader;

                case TrinityItemType.FistWeapon:
                case TrinityItemType.SpiritStone:
                case TrinityItemType.TwoHandDaibo:
                    return ActorClass.Monk;

                case TrinityItemType.VoodooMask:
                case TrinityItemType.Mojo:
                case TrinityItemType.CeremonialKnife:
                    return ActorClass.Witchdoctor;

                case TrinityItemType.MightyBelt:
                case TrinityItemType.MightyWeapon:
                    return ActorClass.Barbarian;

                case TrinityItemType.WizardHat:
                case TrinityItemType.Orb:
                    return ActorClass.Wizard;

                case TrinityItemType.Scythe:
                case TrinityItemType.TwoHandScythe:
                case TrinityItemType.Phylactery:
                    return ActorClass.Necromancer;

                case TrinityItemType.HandCrossbow:
                case TrinityItemType.Cloak:
                case TrinityItemType.Quiver:
                case TrinityItemType.TwoHandBow:
                case TrinityItemType.TwoHandCrossbow:
                    return ActorClass.DemonHunter;
            }

            return ActorClass.Invalid;
        }

        public bool Equals(Item other)
        {
            return GetHashCode().Equals(other.GetHashCode());
        }

        /// <summary>
        /// Unique Identifier so that dictionarys can compare this object properly.
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Name.GetHashCode();
        }

        public bool IsTwoHanded { get; set; }

        public TrinityItemType TrinityItemType { get; set; }

        public string IconUrl { get; set; }

        public bool GameBalanceId { get; set; }
        public int Importance { get; set; }
        public int MaxRank { get; set; }
    }
}