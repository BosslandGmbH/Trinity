using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Objects.Enums;
using Trinity.Settings.Loot;
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
                return (T)Enum.ToObject(typeof(T), 5);
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
            // One Handed Weapons
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
                    return TrinityItemBaseType.WeaponOneHand;
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
                case RawItemType.Amethyst:
                    return TrinityItemType.Amethyst;
                case RawItemType.Amulet:
                    return TrinityItemType.Amulet;
                case RawItemType.Axe:
                    return TrinityItemType.Axe;
                case RawItemType.BeltBarbarian:
                    return TrinityItemType.MightyBelt;
                case RawItemType.BootsBarbarian:
                case RawItemType.BootsCrusader:
                case RawItemType.BootsDemonHunter:
                case RawItemType.BootsMonk:
                case RawItemType.BootsWitchDoctor:
                case RawItemType.BootsWizard:
                case RawItemType.Boots:
                    return TrinityItemType.Boots;
                case RawItemType.StarterBow:
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
                case RawItemType.CraftingReagentBound:
                case RawItemType.CraftingReagent:
                case RawItemType.HoradricReagent:
                    return TrinityItemType.CraftingMaterial;
                case RawItemType.CraftingPlan:
                case RawItemType.CraftingPlanSmith:
                case RawItemType.CraftingPlanLegendarySmith:
                case RawItemType.CraftingPlanJeweler:
                case RawItemType.CraftingPlanMystic:
                case RawItemType.CraftingPlanMysticTransmog:
                    return TrinityItemType.CraftingPlan;
                case RawItemType.CrusaderShield:
                    return TrinityItemType.CrusaderShield;
                case RawItemType.Dagger:
                    return TrinityItemType.Dagger;
                case RawItemType.Diamond:
                    return TrinityItemType.Diamond;
                case RawItemType.Dye:
                    return TrinityItemType.Dye;
                case RawItemType.Emerald:
                    return TrinityItemType.Emerald;
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
                case RawItemType.GlovesBarbarian:
                case RawItemType.GlovesCrusader:
                case RawItemType.GlovesDemonHunter:
                case RawItemType.GlovesMonk:
                case RawItemType.GlovesWitchDoctor:
                case RawItemType.GlovesWizard:
                case RawItemType.Gloves:
                    return TrinityItemType.Gloves;
                case RawItemType.HandXbow:
                    return TrinityItemType.HandCrossbow;
                case RawItemType.HealthGlyph:
                    return TrinityItemType.HealthPotion;
                case RawItemType.HealthPotion:
                    return TrinityItemType.HealthPotion;
                case RawItemType.LegsBarbarian:
                case RawItemType.LegsCrusader:
                case RawItemType.LegsDemonHunter:
                case RawItemType.LegsMonk:
                case RawItemType.LegsWitchDoctor:
                case RawItemType.LegsWizard:
                case RawItemType.Legs:
                    return TrinityItemType.Legs;
                case RawItemType.Polearm:
                    return TrinityItemType.TwoHandPolearm;
                case RawItemType.Quiver:
                    return TrinityItemType.Quiver;
                case RawItemType.Ring:
                    return TrinityItemType.Ring;
                case RawItemType.Ruby:
                    return TrinityItemType.Ruby;
                case RawItemType.Shield:
                    return TrinityItemType.Shield;
                case RawItemType.ShouldersBarbarian:
                case RawItemType.ShouldersCrusader:
                case RawItemType.ShouldersDemonHunter:
                case RawItemType.ShouldersMonk:
                case RawItemType.ShouldersWitchDoctor:
                case RawItemType.ShouldersWizard:              
                case RawItemType.Shoulders:
                    return TrinityItemType.Shoulder;
                case RawItemType.Spear:
                    return TrinityItemType.Spear;
                case RawItemType.SpiritStoneMonk:
                    return TrinityItemType.SpiritStone;
                case RawItemType.Staff:
                    return TrinityItemType.TwoHandStaff;
                case RawItemType.Sword:
                    return TrinityItemType.Sword;
                case RawItemType.Topaz:
                    return TrinityItemType.Topaz;
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
                case RawItemType.ProgressionOrb:
                    return TrinityItemType.ProgressionGlobe;
                case RawItemType.RamaladnisGift:
                    return TrinityItemType.ConsumableAddSockets;
                case RawItemType.TieredRiftKey:
                    return TrinityItemType.TieredLootrunKey;
                case RawItemType.Mojo:
                    return TrinityItemType.Mojo;
                case RawItemType.GenericChestArmor:
                case RawItemType.ChestArmorBarbarian:
                case RawItemType.ChestArmorCrusader:
                case RawItemType.ChestArmorDemonHunter:
                case RawItemType.ChestArmorMonk:
                case RawItemType.ChestArmorWitchDoctor:
                case RawItemType.ChestArmorWizard:
                case RawItemType.ChestArmor:
                case RawItemType.Armor:
                    return TrinityItemType.Chest;
                case RawItemType.HelmBarbarian:
                case RawItemType.HelmCrusader:
                case RawItemType.HelmDemonHunter:
                case RawItemType.HelmMonk:
                case RawItemType.HelmWitchDoctor:
                case RawItemType.HelmWizard:
                case RawItemType.Helm:
                case RawItemType.GenericHelm:
                    return TrinityItemType.Helm;
                case RawItemType.BeltCrusader:
                case RawItemType.BeltDemonHunter:
                case RawItemType.BeltMonk:
                case RawItemType.BeltWitchDoctor:
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
                case RawItemType.UberReagent:
                    return TrinityItemType.UberReagent;
                case RawItemType.PortalDevice:
                    return TrinityItemType.PortalDevice;
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

        internal static TrinityItemBaseType DetermineBaseType(TrinityItemType itemType)
        {
            var itemBaseType = TrinityItemBaseType.Misc;

            // One Handed Weapons
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
                {

                    itemBaseType = TrinityItemBaseType.WeaponOneHand;
                    break;
                }
                // Two Handed Weapons
                case TrinityItemType.TwoHandDaibo:
                case TrinityItemType.TwoHandMace:
                case TrinityItemType.TwoHandFlail:
                case TrinityItemType.TwoHandMighty:
                case TrinityItemType.TwoHandPolearm:
                case TrinityItemType.TwoHandStaff:
                case TrinityItemType.TwoHandSword:
                case TrinityItemType.TwoHandAxe:
                {
                    itemBaseType = TrinityItemBaseType.WeaponTwoHand;
                    break;
                }
                // Ranged Weapons
                case TrinityItemType.TwoHandCrossbow:
                case TrinityItemType.HandCrossbow:
                case TrinityItemType.TwoHandBow:
                {
                    itemBaseType = TrinityItemBaseType.WeaponRange;
                    break;
                }
                // Off-hands
                case TrinityItemType.Mojo:
                case TrinityItemType.Orb:
                case TrinityItemType.CrusaderShield:
                case TrinityItemType.Quiver:
                case TrinityItemType.Shield:
                {
                    itemBaseType = TrinityItemBaseType.Offhand;
                    break;
                }
                // Armors
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
                {
                    itemBaseType = TrinityItemBaseType.Armor;
                    break;
                }
                // Jewlery
                case TrinityItemType.Amulet:
                case TrinityItemType.Ring:
                {
                    itemBaseType = TrinityItemBaseType.Jewelry;
                    break;
                }
                // Follower Items
                case TrinityItemType.FollowerEnchantress:
                case TrinityItemType.FollowerScoundrel:
                case TrinityItemType.FollowerTemplar:
                {
                    itemBaseType = TrinityItemBaseType.FollowerItem;
                    break;
                }
                // Misc Items
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
                case TrinityItemType.TieredLootrunKey:
                {
                    itemBaseType = TrinityItemBaseType.Misc;
                    break;
                }
                // Gems
                case TrinityItemType.Ruby:
                case TrinityItemType.Emerald:
                case TrinityItemType.Topaz:
                case TrinityItemType.Amethyst:
                case TrinityItemType.Diamond:
                {
                    itemBaseType = TrinityItemBaseType.Gem;
                    break;
                }
                // Globes
                case TrinityItemType.HealthGlobe:
                {
                    itemBaseType = TrinityItemBaseType.HealthGlobe;
                    break;
                }
                case TrinityItemType.PowerGlobe:
                {
                    itemBaseType = TrinityItemBaseType.PowerGlobe;
                    break;
                }
                case TrinityItemType.ProgressionGlobe:
                {
                    itemBaseType = TrinityItemBaseType.ProgressionGlobe;
                    break;
                }
            }
            return itemBaseType;
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
                case RawItemType.StarterBow:
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
                case RawItemType.GlovesBarbarian:
                case RawItemType.GlovesCrusader:
                case RawItemType.GlovesDemonHunter:
                case RawItemType.GlovesMonk:
                case RawItemType.GlovesWitchDoctor:
                case RawItemType.GlovesWizard:
                case RawItemType.Gloves:
                    return ItemType.Gloves;
                case RawItemType.BootsBarbarian:
                case RawItemType.BootsCrusader:
                case RawItemType.BootsDemonHunter:
                case RawItemType.BootsMonk:
                case RawItemType.BootsWitchDoctor:
                case RawItemType.BootsWizard:
                case RawItemType.Boots:
                    return ItemType.Boots;
                case RawItemType.ChestArmorBarbarian:
                case RawItemType.ChestArmorCrusader:
                case RawItemType.ChestArmorDemonHunter:
                case RawItemType.ChestArmorMonk:
                case RawItemType.ChestArmorWitchDoctor:
                case RawItemType.ChestArmorWizard:
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
                case RawItemType.ShouldersBarbarian:
                case RawItemType.ShouldersCrusader:
                case RawItemType.ShouldersDemonHunter:
                case RawItemType.ShouldersMonk:
                case RawItemType.ShouldersWitchDoctor:
                case RawItemType.ShouldersWizard:
                case RawItemType.Shoulders:
                    return ItemType.Shoulder;
                case RawItemType.LegsBarbarian:
                case RawItemType.LegsCrusader:
                case RawItemType.LegsDemonHunter:
                case RawItemType.LegsMonk:
                case RawItemType.LegsWitchDoctor:
                case RawItemType.LegsWizard:
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
                case RawItemType.HelmBarbarian:
                case RawItemType.HelmCrusader:
                case RawItemType.HelmDemonHunter:
                case RawItemType.HelmMonk:
                case RawItemType.HelmWitchDoctor:
                case RawItemType.HelmWizard:
                case RawItemType.Helm:
                case RawItemType.GenericHelm:
                    return ItemType.Helm;
                case RawItemType.BeltCrusader:
                case RawItemType.BeltDemonHunter:
                case RawItemType.BeltMonk:
                case RawItemType.BeltWitchDoctor:
                case RawItemType.GenericBelt:
                    return ItemType.Belt;
                case RawItemType.Bracers:
                    return ItemType.Bracer;
                case RawItemType.Orb:
                    return ItemType.Orb;
                case RawItemType.MightyWeapon2H:
                case RawItemType.MightyWeapon1H:
                    return ItemType.MightyWeapon;
                case RawItemType.BeltBarbarian:
                    return ItemType.MightyBelt;
                case RawItemType.Polearm:
                    return ItemType.Polearm;
                case RawItemType.Cloak:
                    return ItemType.Cloak;
                case RawItemType.Wand:
                    return ItemType.Wand;
                case RawItemType.SpiritStoneMonk:
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
                case RawItemType.Diamond:
                case RawItemType.Emerald:
                case RawItemType.Topaz:
                case RawItemType.Amethyst:
                case RawItemType.Ruby:
                case RawItemType.Gem:
                    return ItemType.Gem;
                case RawItemType.CursedHoradricReagent:
                case RawItemType.CraftingReagentBound:
                case RawItemType.CraftingReagent:
                case RawItemType.HoradricReagent:
                case RawItemType.UberReagent:
                    return ItemType.CraftingReagent;
                case RawItemType.PageOfKnowledge:
                case RawItemType.PageOfRespec:
                case RawItemType.PageOfTrainingJeweler:
                case RawItemType.PageOfTrainingSmith:
                    return ItemType.CraftingPage;
                case RawItemType.CraftingPlan:
                case RawItemType.CraftingPlanSmith:
                case RawItemType.CraftingPlanLegendarySmith:
                case RawItemType.CraftingPlanJeweler:
                case RawItemType.CraftingPlanMystic:
                case RawItemType.CraftingPlanMysticTransmog:
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

                    //return ItemType.SeasonCache; ??

            }
            return ItemType.Unknown;
        }

        internal static SalvageOption GetSalvageOption(ItemQuality qualityLevel)
        {
            if (qualityLevel >= ItemQuality.Inferior && qualityLevel <= ItemQuality.Superior)
            {
                return Trinity.Settings.Loot.TownRun.SalvageWhiteItemOption;
            }

            if (qualityLevel >= ItemQuality.Magic1 && qualityLevel <= ItemQuality.Magic3)
            {
                return Trinity.Settings.Loot.TownRun.SalvageBlueItemOption;
            }

            if (qualityLevel >= ItemQuality.Rare4 && qualityLevel <= ItemQuality.Rare6)
            {
                return Trinity.Settings.Loot.TownRun.SalvageYellowItemOption;
            }

            if (qualityLevel >= ItemQuality.Legendary)
            {
                return Trinity.Settings.Loot.TownRun.SalvageLegendaryItemOption;
            }
            return SalvageOption.Sell;
        }

        public static bool GetIsOffhand(ItemType itemType)
        {
            switch (itemType)
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

        public static bool GetIsTwoSlot(ItemBaseType baseType, ItemType itemType)
        {
            switch (itemType)
            {
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
    }
}

