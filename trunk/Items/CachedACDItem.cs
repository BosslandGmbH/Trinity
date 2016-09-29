using System;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Items
{
    // CachedACDItem - Special caching class to help with backpack-item handling
    // So we can make an object, read all item stats from a backpack item *ONCE*, then store it here while my behavior trees process everything
    // Preventing any need for calling D3 memory again after the initial read (every D3 memory read is a chance for a DB crash/item mis-read/stuck!)
    public class CachedACDItem : IComparable
    {
        public string InternalName { get; set; }
        public string RealName { get; set; }
        public int Level { get; set; }
        public ItemQuality Quality { get; set; }
        public int GoldAmount { get; set; }
        public int BalanceID { get; set; }
        public int DynamicId { get; set; }
        public int ActorSnoId { get; set; }
        public bool OneHanded { get; set; }
        public bool TwoHanded { get; set; }
        public DyeType DyeType { get; set; }
        public ItemType ItemType { get; set; }
        public ItemBaseType BaseType { get; set; }
        public TrinityItemBaseType TrinityItemBaseType { get; set; }
        public TrinityItemType TrinityItemType { get; set; }
        public FollowerType FollowerType { get; set; }
        public bool IsUnidentified { get; set; }
        public long ItemStackQuantity { get; set; }
        public float Dexterity { get; set; }
        public float Intelligence { get; set; }
        public float Strength { get; set; }
        public float Vitality { get; set; }
        public float LifePercent { get; set; }
        public float LifeOnHit { get; set; }
        public float LifeSteal { get; set; }
        public float HealthPerSecond { get; set; }
        public float MagicFind { get; set; }
        public float GoldFind { get; set; }
        public float MovementSpeed { get; set; }
        public float PickUpRadius { get; set; }
        public float Sockets { get; set; }
        public float CritPercent { get; set; }
        public float CritDamagePercent { get; set; }
        public float AttackSpeedPercent { get; set; }
        public float MinDamage { get; set; }
        public float MaxDamage { get; set; }
        public float BlockChance { get; set; }
        public float Thorns { get; set; }
        public float ResistAll { get; set; }
        public float ResistArcane { get; set; }
        public float ResistCold { get; set; }
        public float ResistFire { get; set; }
        public float ResistHoly { get; set; }
        public float ResistLightning { get; set; }
        public float ResistPhysical { get; set; }
        public float ResistPoison { get; set; }
        public float WeaponDamagePerSecond { get; set; }
        public float ArmorBonus { get; set; }
        public float MaxDiscipline { get; set; }
        public float MaxMana { get; set; }
        public float ArcaneOnCrit { get; set; }
        public float ManaRegen { get; set; }
        public float GlobeBonus { get; set; }
        public float HatredRegen { get; set; }
        public float MaxFury { get; set; }
        public float SpiritRegen { get; set; }
        public float MaxSpirit { get; set; }
        public float HealthPerSpiritSpent { get; set; }
        public float MaxArcanePower { get; set; }
        public float DamageReductionPhysicalPercent { get; set; }
        public float ArmorTotal { get; set; }
        public float Armor { get; set; }
        public float FireDamagePercent { get; set; }
        public float LightningDamagePercent { get; set; }
        public float ColdDamagePercent { get; set; }
        public float PoisonDamagePercent { get; set; }
        public float ArcaneDamagePercent { get; set; }
        public float HolyDamagePercent { get; set; }
        public float HealthGlobeBonus { get; set; }
        public float WeaponAttacksPerSecond { get; set; }
        public float WeaponMaxDamage { get; set; }
        public float WeaponMinDamage { get; set; }
        public ACDItem AcdItem { get; set; }
        public int InventoryRow { get; set; }
        public int InventoryColumn { get; set; }
        public string ItemLink { get; set; }
        public bool IsAncient { get; set; }
        public bool IsEquipment { get; set; }
        public bool IsSalvageable { get; set; }
        public int GameBalanceId { get; set; }
        public InventorySlot InventorySlot { get; set; }
        public double HighestPrimary { get; set; }
        public bool IsClassItem { get; set; }
        public bool IsOffHand { get; set; }

        public CachedACDItem(ItemStats stats)
        {
            CacheStats(stats);
        }

        private void CacheStats(ItemStats stats)
        {
            WeaponDamagePerSecond = stats.WeaponDamagePerSecond;
            Dexterity = stats.Dexterity;
            Intelligence = stats.Intelligence;
            Strength = stats.Strength;
            Vitality = stats.Vitality;
            LifePercent = stats.LifePercent;
            LifeOnHit = stats.LifeOnHit;
            LifeSteal = stats.LifeSteal;
            HealthPerSecond = stats.HealthPerSecond;
            MagicFind = stats.MagicFind;
            GoldFind = stats.GoldFind;
            MovementSpeed = stats.MovementSpeed;
            PickUpRadius = stats.PickUpRadius;
            Sockets = stats.Sockets;
            CritPercent = stats.CritPercent;
            CritDamagePercent = stats.CritDamagePercent;
            AttackSpeedPercent = stats.AttackSpeedPercent;
            MinDamage = stats.MinDamage;
            MaxDamage = stats.MaxDamage;
            BlockChance = stats.BlockChance;
            Thorns = stats.Thorns;
            ResistAll = stats.ResistAll;
            ResistArcane = stats.ResistArcane;
            ResistCold = stats.ResistCold;
            ResistFire = stats.ResistFire;
            ResistHoly = stats.ResistHoly;
            ResistLightning = stats.ResistLightning;
            ResistPhysical = stats.ResistPhysical;
            ResistPoison = stats.ResistPoison;
            WeaponDamagePerSecond = stats.WeaponDamagePerSecond;
            ArmorBonus = stats.ArmorBonus;
            MaxDiscipline = stats.MaxDiscipline;
            MaxMana = stats.MaxMana;
            ArcaneOnCrit = stats.ArcaneOnCrit;
            ManaRegen = stats.ManaRegen;
            GlobeBonus = stats.HealthGlobeBonus;
            HatredRegen = stats.HatredRegen;
            MaxFury = stats.MaxFury;
            SpiritRegen = stats.SpiritRegen;
            MaxSpirit = stats.MaxSpirit;
            HealthPerSpiritSpent = stats.HealthPerSpiritSpent;
            MaxArcanePower = stats.MaxArcanePower;
            DamageReductionPhysicalPercent = stats.DamageReductionPhysicalPercent;
            ArmorTotal = stats.ArmorTotal;
            Armor = stats.Armor;
            //FireDamagePercent = stats.FireDamagePercent;
            //LightningDamagePercent = stats.LightningDamagePercent;
            //ColdDamagePercent = stats.ColdDamagePercent;
            //PoisonDamagePercent = stats.PoisonDamagePercent;
            //ArcaneDamagePercent = stats.ArcaneDamagePercent;
            //HolyDamagePercent = stats.HolyDamagePercent;
            HealthGlobeBonus = stats.HealthGlobeBonus;
            WeaponAttacksPerSecond = stats.WeaponAttacksPerSecond;
            WeaponMaxDamage = stats.WeaponMaxDamage;
            WeaponMinDamage = stats.WeaponMinDamage;
            HighestPrimary = stats.HighestPrimaryAttribute;            
        }

        public CachedACDItem()
        {

        }

        public int CompareTo(object obj)
        {
            CachedACDItem item = (CachedACDItem)obj;

            if (InventoryRow < item.InventoryRow)
                return -1;
            if (InventoryColumn < item.InventoryColumn)
                return -1;
            if (InventoryColumn == item.InventoryColumn && InventoryRow == item.InventoryRow)
                return 0;
            return 1;
        }

        public static CachedACDItem GetTrinityItem(ACDItem item)
        {
            try
            {
                if (!item.IsValid)
                    return default(CachedACDItem);

                CachedACDItem cItem = new CachedACDItem(item.Stats)
                {
                    AcdItem = item,
                    InternalName = item.InternalName,
                    RealName = item.Name,
                    Level = item.Level,
                    Quality = item.GetItemQuality(),
                    GoldAmount = item.Gold,
                    BalanceID = item.GameBalanceId,
                    DynamicId = item.AnnId,
                    ActorSnoId = item.ActorSnoId,
                    OneHanded = item.IsOneHand,
                    TwoHanded = item.IsTwoHand,
                    DyeType = item.DyeType,
                    ItemType = item.ItemType,
                    BaseType = item.ItemBaseType,
                    FollowerType = item.FollowerSpecialType,
                    IsUnidentified = item.IsUnidentified,
                    ItemStackQuantity = item.ItemStackQuantity,
                    InventoryRow = item.InventoryRow,
                    InventoryColumn = item.InventoryColumn,
                    ItemLink = item.ItemLink,
                    GameBalanceId = item.GameBalanceId,
                    TrinityItemType = TrinityItemManager.DetermineItemType(item.InternalName, item.ItemType, item.FollowerSpecialType),
                    IsAncient = item.GetAttribute<int>(ActorAttributeType.AncientRank) > 0,
                    InventorySlot = item.InventorySlot,
                };

                TrinityItemBaseType trinityItemBaseType = TrinityItemManager.DetermineBaseType(TrinityItemManager.DetermineItemType(item.InternalName, item.ItemType, item.FollowerSpecialType));
                cItem.TrinityItemBaseType = trinityItemBaseType;
                cItem.IsEquipment = GetIsEquipment(trinityItemBaseType);
                cItem.IsSalvageable = GetIsSalvageable(cItem);
                cItem.IsClassItem = GetIsClassItem(cItem);
                cItem.IsOffHand = GetIsOffhand(cItem);
                return cItem;
            }
            catch (Exception ex)
            {
                Logger.LogError("Error getting TrinityItem {0}", ex.Message);
                return default(CachedACDItem);
            }

        }

        private static bool GetIsOffhand(CachedACDItem cItem)
        {
            switch (cItem.ItemType)
            {
                case ItemType.Mojo:
                case ItemType.Quiver:
                case ItemType.CrusaderShield:
                case ItemType.Shield:
                case ItemType.Orb:
                    return true;
            }
            return false;
        }

        public static bool GetIsClassItem(CachedACDItem cItem)
        {
            switch (cItem.ItemType)
            {
                case ItemType.Mojo:
                case ItemType.Quiver:
                case ItemType.Orb:
                case ItemType.CrusaderShield:
                case ItemType.MightyWeapon:
                case ItemType.MightyBelt:
                case ItemType.SpiritStone:
                case ItemType.Daibo:
                case ItemType.Flail:
                case ItemType.Cloak:
                case ItemType.WizardHat:
                case ItemType.CeremonialDagger:
                case ItemType.VoodooMask:
                case ItemType.FistWeapon:
                case ItemType.Crossbow:
                case ItemType.HandCrossbow:
                case ItemType.Bow:
                    return true;
            }
            return false;
        }

        public static bool GetIsSalvageable(CachedACDItem cItem)
        {
            if (!cItem.IsEquipment)
                return false;

            if (cItem.AcdItem.IsVendorBought)
                return false;

            return true;
        }

        public static bool GetIsEquipment(TrinityItemBaseType baseType)
        {
            switch (baseType)
            {
                case TrinityItemBaseType.Armor:
                case TrinityItemBaseType.Jewelry:
                case TrinityItemBaseType.Offhand:
                case TrinityItemBaseType.WeaponOneHand:
                case TrinityItemBaseType.WeaponRange:
                case TrinityItemBaseType.WeaponTwoHand:
                case TrinityItemBaseType.FollowerItem:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsUsableByClass(ActorClass actorClass)
        {
            switch (TrinityItemType)
            {
                case TrinityItemType.Mojo:
                case TrinityItemType.CeremonialKnife:
                case TrinityItemType.VoodooMask:
                    if (actorClass != ActorClass.Witchdoctor)
                        return false;
                    break;

                case TrinityItemType.SpiritStone:
                case TrinityItemType.FistWeapon:
                case TrinityItemType.TwoHandDaibo:
                    if (actorClass != ActorClass.Monk)
                        return false;
                    break;

                case TrinityItemType.MightyBelt:
                case TrinityItemType.MightyWeapon:
                case TrinityItemType.TwoHandMighty:
                    if (actorClass != ActorClass.Barbarian)
                        return false;
                    break;

                case TrinityItemType.Orb:
                case TrinityItemType.WizardHat:
                case TrinityItemType.Wand:
                    if (actorClass != ActorClass.Wizard)
                        return false;
                    break;

                case TrinityItemType.Flail:
                case TrinityItemType.TwoHandFlail:
                case TrinityItemType.CrusaderShield:
                    if (actorClass != ActorClass.Crusader)
                        return false;
                    break;

                case TrinityItemType.Cloak:
                case TrinityItemType.Quiver:
                case TrinityItemType.TwoHandBow:
                case TrinityItemType.HandCrossbow:
                case TrinityItemType.TwoHandCrossbow:
                    if (actorClass != ActorClass.DemonHunter)
                        return false;
                    break;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            var other = obj as CachedACDItem;
            return other != null && other.DynamicId == DynamicId;
        }

        protected bool Equals(CachedACDItem other)
        {
            return DynamicId == other.DynamicId && ActorSnoId == other.ActorSnoId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DynamicId * 397) ^ ActorSnoId;
            }
        }
    }
}
