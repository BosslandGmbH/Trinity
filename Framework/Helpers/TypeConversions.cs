using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Settings;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Helpers
{
    public static class TypeConversions
    {
        public static T To<T>(this IConvertible obj)
        {
            var t = typeof(T);
            if (t.IsEnum)
            {
                return (T)Enum.ToObject(typeof(T), obj);
            }
            var u = Nullable.GetUnderlyingType(t);
            if (u != null)
            {
                return (obj == null) ? default(T) : (T)Convert.ChangeType(obj, u);
            }
            return (T)Convert.ChangeType(obj, t);
        }

        public static T ConvertTo<T>(int integerValue, float floatValue)
        {
            var t = typeof(T);
            if (integerValue is T)
            {
                return (T)Convert.ChangeType(integerValue, t);
            }
            if (floatValue is T)
            {
                var roundedFloat = Math.Round(floatValue, 2, MidpointRounding.AwayFromZero);
                return (T)Convert.ChangeType(roundedFloat, t);
            }
            if (t.IsEnum)
            {
                return (T)Enum.ToObject(typeof(T), integerValue);
            }
            if (typeof(T) == typeof(double)) return (T)Convert.ChangeType(floatValue, t);
            try
            {
                return (T)Convert.ChangeType(integerValue, t);
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        //internal static ItemType GItemTypeToItemType(TrinityItemType itemType)
        //{
        //    switch (itemType)
        //    {
        //        case TrinityItemType.Axe:
        //            return ItemType.Axe;

        //        case TrinityItemType.Dagger:
        //            return ItemType.Dagger;

        //        case TrinityItemType.Flail:
        //            return ItemType.Flail;

        //        case TrinityItemType.FistWeapon:
        //            return ItemType.FistWeapon;

        //        case TrinityItemType.Mace:
        //            return ItemType.Mace;

        //        case TrinityItemType.MightyWeapon:
        //            return ItemType.MightyWeapon;

        //        case TrinityItemType.Spear:
        //            return ItemType.Spear;

        //        case TrinityItemType.Sword:
        //            return ItemType.Sword;

        //        case TrinityItemType.Wand:
        //            return ItemType.Wand;

        //        case TrinityItemType.HandCrossbow:
        //            return ItemType.HandCrossbow;

        //        case TrinityItemType.CeremonialKnife:
        //            return ItemType.CeremonialDagger;

        //        case TrinityItemType.TwoHandDaibo:
        //            return ItemType.Daibo;

        //        case TrinityItemType.TwoHandMace:
        //            return ItemType.Mace;

        //        case TrinityItemType.TwoHandFlail:
        //            return ItemType.Flail;

        //        case TrinityItemType.TwoHandMighty:
        //            return ItemType.MightyWeapon;

        //        case TrinityItemType.TwoHandPolearm:
        //            return ItemType.Polearm;

        //        case TrinityItemType.TwoHandStaff:
        //            return ItemType.Staff;

        //        case TrinityItemType.TwoHandSword:
        //            return ItemType.Sword;

        //        case TrinityItemType.TwoHandAxe:
        //            return ItemType.Axe;

        //        case TrinityItemType.TwoHandCrossbow:
        //            return ItemType.Crossbow;

        //        case TrinityItemType.TwoHandBow:
        //            return ItemType.Bow;

        //        case TrinityItemType.FollowerEnchantress:
        //        case TrinityItemType.FollowerScoundrel:
        //        case TrinityItemType.FollowerTemplar:
        //            return ItemType.FollowerSpecial;

        //        case TrinityItemType.CraftingMaterial:
        //            return ItemType.CraftingReagent;

        //        case TrinityItemType.CraftTome:
        //            return ItemType.CraftingPlan;

        //        case TrinityItemType.HealthPotion:
        //        case TrinityItemType.Dye:
        //        case TrinityItemType.ConsumableAddSockets:
        //        case TrinityItemType.ProgressionGlobe:
        //        case TrinityItemType.PowerGlobe:
        //        case TrinityItemType.HealthGlobe:
        //            return ItemType.Consumable;

        //        case TrinityItemType.Ruby:
        //        case TrinityItemType.Emerald:
        //        case TrinityItemType.Topaz:
        //        case TrinityItemType.Amethyst:
        //        case TrinityItemType.Diamond:
        //            return ItemType.Gem;

        //        case TrinityItemType.LootRunKey:
        //        case TrinityItemType.HoradricRelic:
        //        case TrinityItemType.SpecialItem:
        //        case TrinityItemType.CraftingPlan:
        //        case TrinityItemType.HoradricCache:
        //        case TrinityItemType.StaffOfHerding:
        //        case TrinityItemType.InfernalKey:
        //        case TrinityItemType.TieredLootrunKey:
        //            return ItemType.Unknown;
        //    }

        //    ItemType newType;
        //    if (Enum.TryParse(itemType.ToString(), true, out newType))
        //        return newType;

        //    return ItemType.Unknown;
        //}

        //internal static TrinityItemBaseType DetermineBaseType(TrinityItemType itemType)
        //{
        //    // One Handed Weapons
        //    switch (itemType)
        //    {
        //        case TrinityItemType.Axe:
        //        case TrinityItemType.CeremonialKnife:
        //        case TrinityItemType.Dagger:
        //        case TrinityItemType.Flail:
        //        case TrinityItemType.FistWeapon:
        //        case TrinityItemType.Mace:
        //        case TrinityItemType.MightyWeapon:
        //        case TrinityItemType.Spear:
        //        case TrinityItemType.Sword:
        //        case TrinityItemType.Wand:
        //            return TrinityItemBaseType.WeaponOneHand;

        //        case TrinityItemType.TwoHandDaibo:
        //        case TrinityItemType.TwoHandMace:
        //        case TrinityItemType.TwoHandFlail:
        //        case TrinityItemType.TwoHandMighty:
        //        case TrinityItemType.TwoHandPolearm:
        //        case TrinityItemType.TwoHandStaff:
        //        case TrinityItemType.TwoHandSword:
        //        case TrinityItemType.TwoHandAxe:
        //            return TrinityItemBaseType.WeaponTwoHand;

        //        case TrinityItemType.TwoHandCrossbow:
        //        case TrinityItemType.HandCrossbow:
        //        case TrinityItemType.TwoHandBow:
        //            return TrinityItemBaseType.WeaponRange;

        //        case TrinityItemType.Mojo:
        //        case TrinityItemType.Orb:
        //        case TrinityItemType.CrusaderShield:
        //        case TrinityItemType.Quiver:
        //        case TrinityItemType.Shield:
        //            return TrinityItemBaseType.Offhand;

        //        case TrinityItemType.Boots:
        //        case TrinityItemType.Bracer:
        //        case TrinityItemType.Chest:
        //        case TrinityItemType.Cloak:
        //        case TrinityItemType.Gloves:
        //        case TrinityItemType.Helm:
        //        case TrinityItemType.Legs:
        //        case TrinityItemType.Shoulder:
        //        case TrinityItemType.SpiritStone:
        //        case TrinityItemType.VoodooMask:
        //        case TrinityItemType.WizardHat:
        //        case TrinityItemType.Belt:
        //        case TrinityItemType.MightyBelt:
        //            return TrinityItemBaseType.Armor;

        //        case TrinityItemType.Amulet:
        //        case TrinityItemType.Ring:
        //            return TrinityItemBaseType.Jewelry;

        //        case TrinityItemType.FollowerEnchantress:
        //        case TrinityItemType.FollowerScoundrel:
        //        case TrinityItemType.FollowerTemplar:
        //            return TrinityItemBaseType.FollowerItem;

        //        case TrinityItemType.CraftingMaterial:
        //        case TrinityItemType.CraftTome:
        //        case TrinityItemType.LootRunKey:
        //        case TrinityItemType.HoradricRelic:
        //        case TrinityItemType.SpecialItem:
        //        case TrinityItemType.CraftingPlan:
        //        case TrinityItemType.HealthPotion:
        //        case TrinityItemType.HoradricCache:
        //        case TrinityItemType.Dye:
        //        case TrinityItemType.StaffOfHerding:
        //        case TrinityItemType.InfernalKey:
        //        case TrinityItemType.ConsumableAddSockets:
        //        case TrinityItemType.TieredLootrunKey:
        //            return TrinityItemBaseType.Misc;

        //        case TrinityItemType.Ruby:
        //        case TrinityItemType.Emerald:
        //        case TrinityItemType.Topaz:
        //        case TrinityItemType.Amethyst:
        //        case TrinityItemType.Diamond:
        //            return TrinityItemBaseType.Gem;

        //        case TrinityItemType.HealthGlobe:
        //            return TrinityItemBaseType.HealthGlobe;

        //        case TrinityItemType.PowerGlobe:
        //            return TrinityItemBaseType.PowerGlobe;

        //        case TrinityItemType.ProgressionGlobe:
        //            return TrinityItemBaseType.ProgressionGlobe;
        //    }
        //    return TrinityItemBaseType.Unknown;
        //}

        private static readonly Regex ItemExpansionRegex = new Regex(@"^[xp]\d_", RegexOptions.Compiled);

        //[Obsolete] Not sure why this is obsoleted or whate the right replacement is...
        public static TrinityItemType DetermineItemType(string name, ItemType dbItemType, FollowerType dbFollowerType = FollowerType.None)
        {
            name = name.ToLower();
            if (name.StartsWith("x1_")) name = name.Substring(3, name.Length - 3);
            if (name.StartsWith("p1_")) name = name.Substring(3, name.Length - 3);
            if (name.StartsWith("p2_")) name = name.Substring(3, name.Length - 3);
            if (name.StartsWith("p3_")) name = name.Substring(3, name.Length - 3);
            if (name.StartsWith("p4_")) name = name.Substring(3, name.Length - 3);
            if (name.StartsWith("p5_")) name = name.Substring(3, name.Length - 3);
            if (name.StartsWith("p6_")) name = name.Substring(3, name.Length - 3);
            if (ItemExpansionRegex.IsMatch(name)) name = name.Substring(3, name.Length - 3);

            if (name.StartsWith("demonorgan_")) return TrinityItemType.UberReagent;
            if (name.StartsWith("infernalmachine_")) return TrinityItemType.PortalDevice;
            if (name.StartsWith("a1_")) return TrinityItemType.SpecialItem;
            if (name.StartsWith("amethyst")) return TrinityItemType.Amethyst;
            if (name.StartsWith("amulet_")) return TrinityItemType.Amulet;
            if (name.StartsWith("axe_")) return TrinityItemType.Axe;
            if (name.StartsWith("barbbelt_")) return TrinityItemType.MightyBelt;
            if (name.StartsWith("blacksmithstome")) return TrinityItemType.CraftTome;
            if (name.StartsWith("boots_")) return TrinityItemType.Boots;
            if (name.StartsWith("bow_")) return TrinityItemType.TwoHandBow;
            if (name.StartsWith("bracers_")) return TrinityItemType.Bracer;
            if (name.StartsWith("ceremonialdagger_")) return TrinityItemType.CeremonialKnife;
            if (name.StartsWith("cloak_")) return TrinityItemType.Cloak;
            if (name.StartsWith("combatstaff_")) return TrinityItemType.TwoHandDaibo;
            if (name.StartsWith("crafting_")) return TrinityItemType.CraftingMaterial;
            if (name.StartsWith("craftingmaterials_")) return TrinityItemType.CraftingMaterial;
            if (name.StartsWith("craftingplan_")) return TrinityItemType.CraftingPlan;
            if (name.StartsWith("craftingreagent_legendary_")) return TrinityItemType.CraftingMaterial;
            if (name.StartsWith("crushield_")) return TrinityItemType.CrusaderShield;
            if (name.StartsWith("dagger_")) return TrinityItemType.Dagger;
            if (name.StartsWith("diamond_")) return TrinityItemType.Diamond;
            if (name.StartsWith("dye_")) return TrinityItemType.Dye;
            if (name.StartsWith("emerald_")) return TrinityItemType.Emerald;
            if (name.StartsWith("fistweapon_")) return TrinityItemType.FistWeapon;
            if (name.StartsWith("flail1h_")) return TrinityItemType.Flail;
            if (name.StartsWith("flail2h_")) return TrinityItemType.TwoHandFlail;
            if (name.StartsWith("followeritem_enchantress_") || dbFollowerType == FollowerType.Enchantress) return TrinityItemType.FollowerEnchantress;
            if (name.StartsWith("followeritem_scoundrel_") || dbFollowerType == FollowerType.Scoundrel) return TrinityItemType.FollowerScoundrel;
            if (name.StartsWith("followeritem_templar_") || dbFollowerType == FollowerType.Templar) return TrinityItemType.FollowerTemplar;
            if (name.StartsWith("gloves_")) return TrinityItemType.Gloves;
            if (name.StartsWith("handxbow_")) return TrinityItemType.HandCrossbow;
            if (name.StartsWith("healthglobe")) return TrinityItemType.HealthGlobe;
            if (name.StartsWith("healthpotion")) return TrinityItemType.HealthPotion;
            if (name.StartsWith("horadriccache")) return TrinityItemType.HoradricCache;
            if (name.StartsWith("lore_book_")) return TrinityItemType.CraftTome;
            if (name.StartsWith("lootrunkey")) return TrinityItemType.LootRunKey;
            if (name.StartsWith("mace_")) return TrinityItemType.Mace;
            if (name.StartsWith("mightyweapon_1h_")) return TrinityItemType.MightyWeapon;
            if (name.StartsWith("mightyweapon_2h_")) return TrinityItemType.TwoHandMighty;
            if (name.StartsWith("mojo_")) return TrinityItemType.Mojo;
            if (name.StartsWith("orb_")) return TrinityItemType.Orb;
            if (name.StartsWith("page_of_")) return TrinityItemType.CraftTome;
            if (name.StartsWith("pants_")) return TrinityItemType.Legs;
            if (name.StartsWith("polearm_") || dbItemType == ItemType.Polearm) return TrinityItemType.TwoHandPolearm;
            if (name.StartsWith("quiver_")) return TrinityItemType.Quiver;
            if (name.StartsWith("ring_")) return TrinityItemType.Ring;
            if (name.StartsWith("ruby_")) return TrinityItemType.Ruby;
            if (name.StartsWith("shield_")) return TrinityItemType.Shield;
            if (name.StartsWith("shoulderpads_")) return TrinityItemType.Shoulder;
            if (name.StartsWith("spear_")) return TrinityItemType.Spear;
            if (name.StartsWith("spiritstone_")) return TrinityItemType.SpiritStone;
            if (name.StartsWith("staff_")) return TrinityItemType.TwoHandStaff;
            if (name.StartsWith("staffofcow")) return TrinityItemType.StaffOfHerding;
            if (name.StartsWith("sword_")) return TrinityItemType.Sword;
            if (name.StartsWith("topaz_")) return TrinityItemType.Topaz;
            if (name.StartsWith("twohandedaxe_")) return TrinityItemType.TwoHandAxe;
            if (name.StartsWith("twohandedmace_")) return TrinityItemType.TwoHandMace;
            if (name.StartsWith("twohandedsword_")) return TrinityItemType.TwoHandSword;
            if (name.StartsWith("voodoomask_")) return TrinityItemType.VoodooMask;
            if (name.StartsWith("wand_")) return TrinityItemType.Wand;
            if (name.StartsWith("wizardhat_")) return TrinityItemType.WizardHat;
            if (name.StartsWith("xbow_")) return TrinityItemType.TwoHandCrossbow;
            if (name.StartsWith("console_powerglobe")) return TrinityItemType.PowerGlobe;
            if (name.StartsWith("tiered_rifts_orb")) return TrinityItemType.ProgressionGlobe;
            if (name.StartsWith("normal_rifts_orb")) return TrinityItemType.ProgressionGlobe;
            if (name.StartsWith("consumable_add_sockets")) return TrinityItemType.ConsumableAddSockets; // Ramaladni's Gift
            if (name.StartsWith("tieredlootrunkey_")) return TrinityItemType.TieredLootrunKey;
            if (name.StartsWith("demonkey_") || name.StartsWith("demontrebuchetkey") || name.StartsWith("quest_")) return TrinityItemType.InfernalKey;
            if (name.StartsWith("offhand_")) return TrinityItemType.Mojo;
            if (name.StartsWith("horadricrelic")) return TrinityItemType.HoradricRelic;

            // Follower item types
            if (name.StartsWith("jewelbox_") || dbItemType == ItemType.FollowerSpecial)
            {
                if (dbFollowerType == FollowerType.Scoundrel)
                    return TrinityItemType.FollowerScoundrel;
                if (dbFollowerType == FollowerType.Templar)
                    return TrinityItemType.FollowerTemplar;
                if (dbFollowerType == FollowerType.Enchantress)
                    return TrinityItemType.FollowerEnchantress;
            }

            // Fall back on some partial DB item type checking
            if (name.StartsWith("crafting_"))
            {
                if (dbItemType == ItemType.CraftingPage)
                    return TrinityItemType.CraftTome;
                return TrinityItemType.CraftingMaterial;
            }
            if (name.StartsWith("chestarmor_"))
            {
                if (dbItemType == ItemType.Cloak)
                    return TrinityItemType.Cloak;
                return TrinityItemType.Chest;
            }
            if (name.StartsWith("helm_"))
            {
                if (dbItemType == ItemType.SpiritStone)
                    return TrinityItemType.SpiritStone;
                if (dbItemType == ItemType.VoodooMask)
                    return TrinityItemType.VoodooMask;
                if (dbItemType == ItemType.WizardHat)
                    return TrinityItemType.WizardHat;
                return TrinityItemType.Helm;
            }
            if (name.StartsWith("helmcloth_"))
            {
                if (dbItemType == ItemType.SpiritStone)
                    return TrinityItemType.SpiritStone;
                if (dbItemType == ItemType.VoodooMask)
                    return TrinityItemType.VoodooMask;
                if (dbItemType == ItemType.WizardHat)
                    return TrinityItemType.WizardHat;
                return TrinityItemType.Helm;
            }
            if (name.StartsWith("belt_"))
            {
                if (dbItemType == ItemType.MightyBelt)
                    return TrinityItemType.MightyBelt;
                return TrinityItemType.Belt;
            }
            return TrinityItemType.Unknown;
        }

        public static DamageType GetDamageType(Element element)
        {
            switch (element)
            {
                case Element.Physical: return DamageType.Physical;
                case Element.Fire: return DamageType.Fire;
                case Element.Lightning: return DamageType.Lightning;
                case Element.Cold: return DamageType.Cold;
                case Element.Poison: return DamageType.Poison;
                case Element.Holy: return DamageType.Holy;
                case Element.Arcane: return DamageType.Arcane;
            }
            return DamageType.None;
        }

        public static ItemBaseType GetItemBaseType(ItemType type)
        {
            switch (type)
            {
                case ItemType.Axe:
                case ItemType.Sword:
                case ItemType.Mace:
                case ItemType.Dagger:
                case ItemType.Flail:
                case ItemType.Bow:
                case ItemType.Crossbow:
                case ItemType.Staff:
                case ItemType.Spear:
                case ItemType.FistWeapon:
                case ItemType.CeremonialDagger:
                case ItemType.MightyWeapon:
                case ItemType.Polearm:
                case ItemType.Wand:
                case ItemType.Daibo:
                case ItemType.HandCrossbow:
                case ItemType.Scythe:
                    return ItemBaseType.Weapon;

                case ItemType.Shield:
                case ItemType.CrusaderShield:
                case ItemType.Gloves:
                case ItemType.Boots:
                case ItemType.Chest:
                case ItemType.Quiver:
                case ItemType.Shoulder:
                case ItemType.Legs:
                case ItemType.Mojo:
                case ItemType.WizardHat:
                case ItemType.Helm:
                case ItemType.Belt:
                case ItemType.Bracer:
                case ItemType.Orb:
                case ItemType.MightyBelt:
                case ItemType.Cloak:
                case ItemType.SpiritStone:
                case ItemType.VoodooMask:
                case ItemType.Phylactery:
                    return ItemBaseType.Armor;

                case ItemType.Ring:
                case ItemType.Amulet:
                    return ItemBaseType.Jewelry;

                case ItemType.Gem:
                    return ItemBaseType.Gem;

                default:
                    return ItemBaseType.Misc;
            }
        }

        internal static TrinityItemBaseType GetTrinityItemBaseType(TrinityItemType itemType)
        {
            switch (itemType)
            {
                case TrinityItemType.Axe:
                case TrinityItemType.CeremonialKnife:
                case TrinityItemType.Dagger:
                case TrinityItemType.Flail:
                case TrinityItemType.FistWeapon:
                case TrinityItemType.Mace:
                case TrinityItemType.MightyWeapon:
                case TrinityItemType.Spear:
                case TrinityItemType.Sword:
                case TrinityItemType.Wand:
                case TrinityItemType.Scythe:
                    return TrinityItemBaseType.WeaponOneHand;

                case TrinityItemType.TwoHandScythe:
                case TrinityItemType.TwoHandDaibo:
                case TrinityItemType.TwoHandMace:
                case TrinityItemType.TwoHandFlail:
                case TrinityItemType.TwoHandMighty:
                case TrinityItemType.TwoHandPolearm:
                case TrinityItemType.TwoHandStaff:
                case TrinityItemType.TwoHandSword:
                case TrinityItemType.TwoHandAxe:
                    return TrinityItemBaseType.WeaponTwoHand;

                case TrinityItemType.TwoHandCrossbow:
                case TrinityItemType.HandCrossbow:
                case TrinityItemType.TwoHandBow:
                    return TrinityItemBaseType.WeaponRange;

                case TrinityItemType.Mojo:
                case TrinityItemType.Orb:
                case TrinityItemType.CrusaderShield:
                case TrinityItemType.Quiver:
                case TrinityItemType.Shield:
                case TrinityItemType.Phylactery:
                    return TrinityItemBaseType.Offhand;

                case TrinityItemType.Boots:
                case TrinityItemType.Bracer:
                case TrinityItemType.Chest:
                case TrinityItemType.Cloak:
                case TrinityItemType.Gloves:
                case TrinityItemType.Helm:
                case TrinityItemType.Legs:
                case TrinityItemType.Shoulder:
                case TrinityItemType.SpiritStone:
                case TrinityItemType.VoodooMask:
                case TrinityItemType.WizardHat:
                case TrinityItemType.Belt:
                case TrinityItemType.MightyBelt:
                    return TrinityItemBaseType.Armor;

                case TrinityItemType.Amulet:
                case TrinityItemType.Ring:
                    return TrinityItemBaseType.Jewelry;

                case TrinityItemType.FollowerEnchantress:
                case TrinityItemType.FollowerScoundrel:
                case TrinityItemType.FollowerTemplar:
                    return TrinityItemBaseType.FollowerItem;

                case TrinityItemType.CraftingMaterial:
                case TrinityItemType.CraftTome:
                case TrinityItemType.LootRunKey:
                case TrinityItemType.HoradricRelic:
                case TrinityItemType.SpecialItem:
                case TrinityItemType.CraftingPlan:
                case TrinityItemType.HealthPotion:
                case TrinityItemType.HoradricCache:
                case TrinityItemType.Dye:
                case TrinityItemType.StaffOfHerding:
                case TrinityItemType.InfernalKey:
                case TrinityItemType.ConsumableAddSockets:
                case TrinityItemType.UberReagent:
                case TrinityItemType.PortalDevice:
                case TrinityItemType.TieredLootrunKey:
                    return TrinityItemBaseType.Misc;

                case TrinityItemType.Ruby:
                case TrinityItemType.Emerald:
                case TrinityItemType.Topaz:
                case TrinityItemType.Amethyst:
                case TrinityItemType.Diamond:
                    return TrinityItemBaseType.Gem;

                case TrinityItemType.HealthGlobe:
                    return TrinityItemBaseType.HealthGlobe;

                case TrinityItemType.PowerGlobe:
                    return TrinityItemBaseType.PowerGlobe;

                case TrinityItemType.ProgressionGlobe:
                    return TrinityItemBaseType.ProgressionGlobe;


            }
            return TrinityItemBaseType.Unknown;
        }

        public static TrinityItemType GetTrinityItemType(RawItemType type, GemType gemType)
        {
            switch (type)
            {
                //case RawItemType.Amethyst:
                //    return TrinityItemType.Amethyst;
                case RawItemType.Amulet:
                    return TrinityItemType.Amulet;

                case RawItemType.Axe:
                    return TrinityItemType.Axe;

                case RawItemType.Belt_Barbarian:
                    return TrinityItemType.MightyBelt;

                case RawItemType.Boots_Barbarian:
                case RawItemType.Boots_Crusader:
                case RawItemType.Boots_DemonHunter:
                case RawItemType.Boots_Monk:
                case RawItemType.Boots_WitchDoctor:
                case RawItemType.Boots_Wizard:
                case RawItemType.Boots:
                case RawItemType.NecroBoots:
                    return TrinityItemType.Boots;
                //case RawItemType.StarterBow:
                case RawItemType.GenericBowWeapon:
                case RawItemType.BowClass:
                case RawItemType.Bow:
                    return TrinityItemType.TwoHandBow;

                case RawItemType.Bracers:
                    return TrinityItemType.Bracer;

                case RawItemType.CeremonialDagger:
                    return TrinityItemType.CeremonialKnife;

                case RawItemType.Cloak:
                    return TrinityItemType.Cloak;

                case RawItemType.CombatStaff:
                    return TrinityItemType.TwoHandDaibo;

                case RawItemType.CursedHoradricReagent:
                case RawItemType.CraftingReagent_Bound:
                case RawItemType.CraftingReagent:
                case RawItemType.HoradricReagent:
                    return TrinityItemType.CraftingMaterial;

                case RawItemType.CraftingPlan:
                case RawItemType.CraftingPlan_Smith:
                case RawItemType.CraftingPlanLegendary_Smith:
                case RawItemType.CraftingPlan_Jeweler:
                case RawItemType.CraftingPlan_Mystic:
                case RawItemType.CraftingPlan_MysticTransmog:
                    return TrinityItemType.CraftingPlan;

                case RawItemType.CrusaderShield:
                    return TrinityItemType.CrusaderShield;

                case RawItemType.Dagger:
                    return TrinityItemType.Dagger;
                //case RawItemType.Diamond:
                //    return TrinityItemType.Diamond;
                //case RawItemType.Dye:
                //    return TrinityItemType.Dye;
                //case RawItemType.Emerald:
                //    return TrinityItemType.Emerald;
                case RawItemType.FistWeapon:
                    return TrinityItemType.FistWeapon;

                case RawItemType.Flail1H:
                    return TrinityItemType.Flail;

                case RawItemType.Flail2H:
                    return TrinityItemType.TwoHandFlail;

                case RawItemType.EnchantressSpecial:
                    return TrinityItemType.FollowerEnchantress;

                case RawItemType.ScoundrelSpecial:
                    return TrinityItemType.FollowerScoundrel;

                case RawItemType.TemplarSpecial:
                    return TrinityItemType.FollowerTemplar;

                case RawItemType.Gloves_Barbarian:
                case RawItemType.Gloves_Crusader:
                case RawItemType.Gloves_DemonHunter:
                case RawItemType.Gloves_Monk:
                case RawItemType.Gloves_WitchDoctor:
                case RawItemType.Gloves_Wizard:
                case RawItemType.Gloves:
                case RawItemType.NecroGloves:
                    return TrinityItemType.Gloves;

                case RawItemType.HandXbow:
                    return TrinityItemType.HandCrossbow;

                case RawItemType.HealthGlyph:
                    return TrinityItemType.HealthPotion;

                case RawItemType.HealthPotion:
                    return TrinityItemType.HealthPotion;

                case RawItemType.Legs_Barbarian:
                case RawItemType.Legs_Crusader:
                case RawItemType.Legs_DemonHunter:
                case RawItemType.Legs_Monk:
                case RawItemType.Legs_WitchDoctor:
                case RawItemType.Legs_Wizard:
                case RawItemType.Legs:
                case RawItemType.NecroPants:
                    return TrinityItemType.Legs;

                case RawItemType.Polearm:
                    return TrinityItemType.TwoHandPolearm;

                case RawItemType.Quiver:
                    return TrinityItemType.Quiver;

                case RawItemType.Ring:
                    return TrinityItemType.Ring;
                //case RawItemType.Ruby:
                //    return TrinityItemType.Ruby;
                case RawItemType.Shield:
                    return TrinityItemType.Shield;

                case RawItemType.Shoulders_Barbarian:
                case RawItemType.Shoulders_Crusader:
                case RawItemType.Shoulders_DemonHunter:
                case RawItemType.Shoulders_Monk:
                case RawItemType.Shoulders_WitchDoctor:
                case RawItemType.Shoulders_Wizard:
                case RawItemType.Shoulders:
                case RawItemType.NecroShoulders:
                    return TrinityItemType.Shoulder;

                case RawItemType.Spear:
                    return TrinityItemType.Spear;

                case RawItemType.SpiritStone_Monk:
                    return TrinityItemType.SpiritStone;

                case RawItemType.Staff:
                    return TrinityItemType.TwoHandStaff;

                case RawItemType.Sword:
                    return TrinityItemType.Sword;
                //case RawItemType.Topaz:
                //    return TrinityItemType.Topaz;
                case RawItemType.Axe2H:
                    return TrinityItemType.TwoHandAxe;

                case RawItemType.Mace:
                    return TrinityItemType.Mace;

                case RawItemType.Mace2H:
                    return TrinityItemType.TwoHandMace;

                case RawItemType.Sword2H:
                    return TrinityItemType.TwoHandSword;

                case RawItemType.VoodooMask:
                    return TrinityItemType.VoodooMask;

                case RawItemType.Wand:
                    return TrinityItemType.Wand;

                case RawItemType.WizardHat:
                    return TrinityItemType.WizardHat;

                case RawItemType.Crossbow:
                    return TrinityItemType.TwoHandCrossbow;

                case RawItemType.ShrineGlyph:
                    return TrinityItemType.PowerGlobe;

                case RawItemType.Glyph:
                    return TrinityItemType.ProgressionGlobe;

                case RawItemType.GeneralUtility:
                    return TrinityItemType.ConsumableAddSockets;

                case RawItemType.TieredRiftKey:
                    return TrinityItemType.TieredLootrunKey;

                case RawItemType.Mojo:
                    return TrinityItemType.Mojo;

                case RawItemType.GenericChestArmor:
                case RawItemType.ChestArmor_Barbarian:
                case RawItemType.ChestArmor_Crusader:
                case RawItemType.ChestArmor_DemonHunter:
                case RawItemType.ChestArmor_Monk:
                case RawItemType.ChestArmor_WitchDoctor:
                case RawItemType.ChestArmor_Wizard:
                case RawItemType.ChestArmor:
                case RawItemType.Armor:
                case RawItemType.NecroChest:
                    return TrinityItemType.Chest;

                case RawItemType.Helm_Barbarian:
                case RawItemType.Helm_Crusader:
                case RawItemType.Helm_DemonHunter:
                case RawItemType.Helm_Monk:
                case RawItemType.Helm_WitchDoctor:
                case RawItemType.Helm_Wizard:
                case RawItemType.Helm:
                case RawItemType.GenericHelm:
                case RawItemType.NecroHelm:
                    return TrinityItemType.Helm;

                case RawItemType.Belt_Crusader:
                case RawItemType.Belt_DemonHunter:
                case RawItemType.Belt_Monk:
                case RawItemType.Belt_WitchDoctor:
                case RawItemType.GenericBelt:
                case RawItemType.Belt:
                    return TrinityItemType.Belt;

                case RawItemType.Orb:
                    return TrinityItemType.Orb;

                case RawItemType.MightyWeapon1H:
                    return TrinityItemType.MightyWeapon;

                case RawItemType.MightyWeapon2H:
                    return TrinityItemType.TwoHandMighty;

                case RawItemType.BloodShard:
                    return TrinityItemType.HoradricRelic;

                case RawItemType.DemonicKey:
                    return TrinityItemType.InfernalKey;

                case RawItemType.Gold:
                    return TrinityItemType.Gold;

                case RawItemType.TreasureBag:
                    return TrinityItemType.HoradricCache;

                case RawItemType.DemonicOrgan:
                    return TrinityItemType.UberReagent;

                case RawItemType.PortalDevice:
                    return TrinityItemType.PortalDevice;
               
                case RawItemType.TwoHandedScythe:
                    return TrinityItemType.TwoHandScythe;

                case RawItemType.Scythe:
                    return TrinityItemType.Scythe;

                case RawItemType.Phylactery:
                    return TrinityItemType.Phylactery;

                case RawItemType.Gem:
                    switch (gemType)
                    {
                        case GemType.Amethyst:
                            return TrinityItemType.Amethyst;

                        case GemType.Ruby:
                            return TrinityItemType.Ruby;

                        case GemType.Diamond:
                            return TrinityItemType.Diamond;

                        case GemType.Emerald:
                            return TrinityItemType.Emerald;

                        case GemType.Topaz:
                            return TrinityItemType.Topaz;
                    }
                    break;
            }
            

            return TrinityItemType.Unknown;
        }

        public static PickupItemQualities GetPickupItemQuality(TrinityItemQuality quality)
        {
            switch (quality)
            {
                case TrinityItemQuality.Inferior:
                    return PickupItemQualities.Grey;

                case TrinityItemQuality.Common:
                    return PickupItemQualities.White;

                case TrinityItemQuality.Magic:
                    return PickupItemQualities.Blue;

                case TrinityItemQuality.Rare:
                    return PickupItemQualities.Yellow;

                case TrinityItemQuality.Legendary:
                    return PickupItemQualities.Orange;

                case TrinityItemQuality.Set:
                    return PickupItemQualities.Green;
            }
            return PickupItemQualities.None;
        }

        public static ItemType GetItemType(RawItemType rawItemType)
        {
            switch (rawItemType)
            {
                case RawItemType.Axe:
                case RawItemType.Axe2H:
                    return ItemType.Axe;                               

                case RawItemType.Sword:
                case RawItemType.Sword2H:
                    return ItemType.Sword;

                case RawItemType.Mace:
                case RawItemType.Mace2H:
                    return ItemType.Mace;

                case RawItemType.Dagger:
                    return ItemType.Dagger;

                case RawItemType.Flail1H:
                case RawItemType.Flail2H:
                    return ItemType.Flail;
                //case RawItemType.StarterBow:
                case RawItemType.GenericBowWeapon:
                case RawItemType.BowClass:
                case RawItemType.Bow:
                    return ItemType.Bow;

                case RawItemType.Crossbow:
                    return ItemType.Crossbow;

                case RawItemType.Staff:
                    return ItemType.Staff;

                case RawItemType.Spear:
                    return ItemType.Spear;

                case RawItemType.Shield:
                    return ItemType.Shield;

                case RawItemType.CrusaderShield:
                    return ItemType.CrusaderShield;

                case RawItemType.Gloves_Barbarian:
                case RawItemType.Gloves_Crusader:
                case RawItemType.Gloves_DemonHunter:
                case RawItemType.Gloves_Monk:
                case RawItemType.Gloves_WitchDoctor:
                case RawItemType.Gloves_Wizard:
                case RawItemType.Gloves:
                    return ItemType.Gloves;

                case RawItemType.Boots_Barbarian:
                case RawItemType.Boots_Crusader:
                case RawItemType.Boots_DemonHunter:
                case RawItemType.Boots_Monk:
                case RawItemType.Boots_WitchDoctor:
                case RawItemType.Boots_Wizard:
                case RawItemType.Boots:
                    return ItemType.Boots;

                case RawItemType.ChestArmor_Barbarian:
                case RawItemType.ChestArmor_Crusader:
                case RawItemType.ChestArmor_DemonHunter:
                case RawItemType.ChestArmor_Monk:
                case RawItemType.ChestArmor_WitchDoctor:
                case RawItemType.ChestArmor_Wizard:
                case RawItemType.ChestArmor:
                case RawItemType.GenericChestArmor:
                case RawItemType.Armor:
                    return ItemType.Chest;

                case RawItemType.Ring:
                    return ItemType.Ring;

                case RawItemType.Amulet:
                    return ItemType.Amulet;

                case RawItemType.Quiver:
                    return ItemType.Quiver;

                case RawItemType.Shoulders_Barbarian:
                case RawItemType.Shoulders_Crusader:
                case RawItemType.Shoulders_DemonHunter:
                case RawItemType.Shoulders_Monk:
                case RawItemType.Shoulders_WitchDoctor:
                case RawItemType.Shoulders_Wizard:
                case RawItemType.Shoulders:
                    return ItemType.Shoulder;

                case RawItemType.Legs_Barbarian:
                case RawItemType.Legs_Crusader:
                case RawItemType.Legs_DemonHunter:
                case RawItemType.Legs_Monk:
                case RawItemType.Legs_WitchDoctor:
                case RawItemType.Legs_Wizard:
                case RawItemType.Legs:
                    return ItemType.Legs;

                case RawItemType.FistWeapon:
                    return ItemType.FistWeapon;

                case RawItemType.Mojo:
                    return ItemType.Mojo;

                case RawItemType.CeremonialDagger:
                    return ItemType.CeremonialDagger;

                case RawItemType.WizardHat:
                    return ItemType.WizardHat;

                case RawItemType.Helm_Barbarian:
                case RawItemType.Helm_Crusader:
                case RawItemType.Helm_DemonHunter:
                case RawItemType.Helm_Monk:
                case RawItemType.Helm_WitchDoctor:
                case RawItemType.Helm_Wizard:
                case RawItemType.Helm:
                case RawItemType.GenericHelm:
                    return ItemType.Helm;

                case RawItemType.Belt_Crusader:
                case RawItemType.Belt_DemonHunter:
                case RawItemType.Belt_Monk:
                case RawItemType.Belt_WitchDoctor:
                case RawItemType.GenericBelt:
                    return ItemType.Belt;

                case RawItemType.Bracers:
                    return ItemType.Bracer;

                case RawItemType.Orb:
                    return ItemType.Orb;

                case RawItemType.MightyWeapon2H:
                case RawItemType.MightyWeapon1H:
                    return ItemType.MightyWeapon;

                case RawItemType.Belt_Barbarian:
                    return ItemType.MightyBelt;

                case RawItemType.Polearm:
                    return ItemType.Polearm;

                case RawItemType.Cloak:
                    return ItemType.Cloak;

                case RawItemType.Wand:
                    return ItemType.Wand;

                case RawItemType.SpiritStone_Monk:
                    return ItemType.SpiritStone;

                case RawItemType.CombatStaff:
                    return ItemType.Daibo;

                case RawItemType.HandXbow:
                    return ItemType.HandCrossbow;

                case RawItemType.VoodooMask:
                    return ItemType.VoodooMask;

                case RawItemType.HealthPotion:
                case RawItemType.Potion:
                    return ItemType.Potion;
                //case RawItemType.Diamond:
                //case RawItemType.Emerald:
                //case RawItemType.Topaz:
                //case RawItemType.Amethyst:
                //case RawItemType.Ruby:
                case RawItemType.Gem:
                    return ItemType.Gem;

                case RawItemType.CursedHoradricReagent:
                case RawItemType.CraftingReagent_Bound:
                case RawItemType.CraftingReagent:
                case RawItemType.HoradricReagent:
                case RawItemType.DemonicOrgan:
                    return ItemType.CraftingReagent;
                //case RawItemType.PageOfKnowledge:
                //case RawItemType.PageOfRespec:
                case RawItemType.PageOfTraining_Jeweler:
                case RawItemType.PageOfTraining_Smith:
                    return ItemType.CraftingPage;

                case RawItemType.CraftingPlan:
                case RawItemType.CraftingPlan_Smith:
                case RawItemType.CraftingPlanLegendary_Smith:
                case RawItemType.CraftingPlan_Jeweler:
                case RawItemType.CraftingPlan_Mystic:
                case RawItemType.CraftingPlan_MysticTransmog:
                    return ItemType.CraftingPlan;

                case RawItemType.TemplarSpecial:
                case RawItemType.ScoundrelSpecial:
                case RawItemType.EnchantressSpecial:
                    return ItemType.FollowerSpecial;

                case RawItemType.TreasureBag:
                    return ItemType.HoradricCache;

                case RawItemType.TieredRiftKey:
                    return ItemType.KeystoneFragment;

                case RawItemType.UpgradeableJewel:
                    return ItemType.LegendaryGem;

                case RawItemType.Dye:
                    return ItemType.Consumable;

                case RawItemType.NecroShoulders:
                    return ItemType.Shoulder;
                case RawItemType.NecroBoots:
                    return ItemType.Boots;
                case RawItemType.NecroPants:
                    return ItemType.Legs;
                case RawItemType.NecroChest:
                    return ItemType.Chest;
                case RawItemType.NecroHelm:
                    return ItemType.Helm;
                case RawItemType.NecroGloves:
                    return ItemType.Gloves;
                case RawItemType.TwoHandedScythe:
                    return ItemType.Scythe;
                case RawItemType.Scythe:
                    return ItemType.Scythe;
                case RawItemType.Phylactery:
                    return ItemType.Phylactery;

                    //return ItemType.SeasonCache; ??
            }
            return ItemType.Unknown;
        }

        public static bool GetIsOffhand(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Mojo:
                case ItemType.Quiver:
                case ItemType.CrusaderShield:
                case ItemType.Shield:
                case ItemType.Phylactery:
                case ItemType.Orb:
                    return true;
            }
            return false;
        }

        public static bool GetIsTwoSlot(ItemBaseType baseType, ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.HoradricCache:
                case ItemType.SeasonCache:
                    return true;

                case ItemType.Belt:
                    return false;
            }
            switch (baseType)
            {
                case ItemBaseType.Armor:
                case ItemBaseType.Weapon:
                    return true;
            }
            return false;
        }

        public static bool GetIsClassItem(ItemType itemType)
        {
            switch (itemType)
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
                case ItemType.Phylactery:
                case ItemType.Scythe:                
                    return true;
            }
            return false;
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

        public static bool IsUsableByClass(ActorClass actorClass, TrinityItemType trinityItemType)
        {
            switch (trinityItemType)
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

                case TrinityItemType.Phylactery:
                case TrinityItemType.Scythe:
                case TrinityItemType.TwoHandScythe:
                    if (actorClass != ActorClass.Necromancer)
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

        public static bool IsWeapon(ACDItem item) => WeaponTypes.Contains(item.ItemType);

        public static bool IsArmor(ACDItem item) => ArmorTypes.Contains(item.ItemType);

        internal static HashSet<ItemType> OffHandTypes = new HashSet<ItemType>
        {
            ItemType.Orb,
            ItemType.Mojo,
            ItemType.Quiver,
            ItemType.Shield,
            ItemType.CrusaderShield,
            ItemType.Scythe
        };

        internal static HashSet<ItemType> ShieldTypes = new HashSet<ItemType>
        {
            ItemType.Shield,
            ItemType.CrusaderShield,
        };

        internal static HashSet<ItemBaseType> EquipmentTypes = new HashSet<ItemBaseType>
        {
            ItemBaseType.Armor,
            ItemBaseType.Jewelry,
            ItemBaseType.Weapon,
        };

        internal static HashSet<ItemBaseType> TwoSquareTypes = new HashSet<ItemBaseType>
        {
            ItemBaseType.Armor,
            ItemBaseType.Weapon,
        };

        internal static HashSet<ItemType> WeaponTypes = new HashSet<ItemType>
        {
            ItemType.Axe,
            ItemType.Bow,
            ItemType.CeremonialDagger,
            ItemType.Crossbow,
            ItemType.Dagger,
            ItemType.Daibo,
            ItemType.FistWeapon,
            ItemType.Flail,
            ItemType.HandCrossbow,
            ItemType.Mace,
            ItemType.MightyWeapon,
            ItemType.Polearm,
            ItemType.Spear,
            ItemType.Staff,
            ItemType.Sword,
            ItemType.Wand,
            ItemType.Scythe
        };

        internal static HashSet<ItemType> ArmorTypes = new HashSet<ItemType>
        {
            ItemType.Belt,
            ItemType.Boots,
            ItemType.Bracer,
            ItemType.Chest,
            ItemType.Cloak,
            ItemType.Gloves,
            ItemType.Helm,
            ItemType.Legs,
            ItemType.MightyBelt,
            ItemType.Shoulder,
            ItemType.SpiritStone,
            ItemType.VoodooMask,
            ItemType.WizardHat,
        };

        internal static HashSet<ItemType> JewleryTypes = new HashSet<ItemType>
        {
            ItemType.Amulet,
            ItemType.Ring,
        };

        internal static HashSet<ItemType> MiscTypes = new HashSet<ItemType>
        {
            ItemType.CraftingPage,
            ItemType.CraftingPlan,
            ItemType.CraftingReagent,
            ItemType.FollowerSpecial,
            ItemType.Gem,
            ItemType.Potion,
            ItemType.Unknown,
        };
    }
}