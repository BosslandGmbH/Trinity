using System;
namespace Trinity.Framework.Objects
{

    public enum TrinityItemType
    {
        Unknown = 0,
        Amethyst,
        Amulet,
        Axe,
        Belt,
        Boots,
        Bracer,
        CeremonialKnife,
        Chest,
        Cloak,
        ConsumableAddSockets,
        CraftTome,
        CraftingMaterial,
        CraftingPlan,
        CrusaderShield,
        Dagger,
        Diamond,
        Dye,
        Emerald,
        FistWeapon,
        FollowerEnchantress,
        FollowerScoundrel,
        FollowerTemplar,
        Flail,
        Gloves,
        HandCrossbow,
        HealthGlobe,
        HealthPotion,
        Helm,
        HoradricRelic,
        HoradricCache,
        InfernalKey,
        Legs,
        LootRunKey,
        Mace,
        MightyBelt,
        MightyWeapon,
        Mojo,
        Orb,
        Phylactery,
        PowerGlobe,
        ProgressionGlobe,
        Quiver,
        Ring,
        Ruby,
        Scythe,
        Shield,
        Shoulder,
        Spear,
        SpecialItem,
        SpiritStone,
        StaffOfHerding,
        Sword,
        TieredLootrunKey,
        Topaz,
        TwoHandAxe,
        TwoHandBow,
        TwoHandCrossbow,
        TwoHandDaibo,
        TwoHandFlail,
        TwoHandMace,
        TwoHandMighty,
        TwoHandPolearm,
        TwoHandStaff,
        TwoHandSword,
        TwoHandScythe,
        VoodooMask,
        Wand,
        WizardHat,
        Gold,
        PortalDevice,
        UberReagent,
    }

    [Flags]
    public enum ItemSelectionType : ulong
    {
        Unknown = 0,
        Amulet = 0x00000001,
        Axe = 0x00000002,
        Belt = 0x00000004,
        Boots = 0x00000008,
        Bracer = 0x00000010,
        CeremonialKnife = 0x00000020,
        Chest = 0x00000040,
        Cloak = 0x00000080,
        CrusaderShield = 0x00000100,
        Dagger = 0x00000200,
        FistWeapon = 0x00000400,
        Flail = 0x00000800,
        Gloves = 0x00001000,
        HandCrossbow = 0x00002000,
        Helm = 0x00004000,
        Legs = 0x00008000,
        Mace = 0x00010000,
        MightyBelt = 0x00020000,
        MightyWeapon = 0x00040000,
        Mojo = 0x00080000,
        Orb = 0x00100000,
        Quiver = 0x00200000,
        Ring = 0x00400000,
        Shield = 0x00800000,
        Shoulder = 0x01000000,
        Spear = 0x02000000,
        SpiritStone = 0x04000000,
        Sword = 0x08000000,
        TwoHandAxe = 0x10000000,
        TwoHandBow = 0x20000000,
        TwoHandCrossbow = 0x40000000,
        TwoHandDaibo = 0x80000000,
        TwoHandFlail = 0x100000000,
        TwoHandMace = 0x200000000,
        TwoHandMighty = 0x400000000,
        TwoHandPolearm = 0x800000000,
        TwoHandStaff = 0x1000000000,
        TwoHandSword = 0x2000000000,
        VoodooMask = 0x4000000000,
        Wand = 0x8000000000,
        WizardHat = 0x10000000000,
        TwoHandScythe = 0x20000000000,
        Scythe = 0x40000000000,
        Phylactery = 0x80000000000
    }

    public enum ItemQualityColor
    {
        Unknown = 0,
        Grey,
        White,
        Blue,
        Yellow,
        Orange,
        Green,
        Other
    }

    public enum TrinityItemBaseType
    {
        Unknown,
        WeaponOneHand,
        WeaponTwoHand,
        WeaponRange,
        Offhand,
        Armor,
        Jewelry,
        FollowerItem,
        Misc,
        Gem,
        HealthGlobe,
        PowerGlobe,
        ProgressionGlobe,
    }

    public enum TrinityObjectType
    {
        Unknown = 0,
        Avoidance,
        Backtrack,
        Barricade,
        Checkpoint,
        Container,
        Destructible,
        Door,
        HealthGlobe,
        Gold,
        HealthWell,
        HotSpot,
        Interactable,
        Item,
        MarkerLocation,
        Player,
        PowerGlobe,
        ProgressionGlobe,
        Proxy,
        SavePoint,
        ServerProp,
        Shrine,
        StartLocation,
        Trigger,
        Unit,
        CursedChest,
        CursedShrine,
        Portal,
        Banner,
        Waypoint,
        ClientEffect,
        Projectile,
        Environment,
        BloodShard,
        Invalid,
        BuffedRegion,
        Gate
    }

    [Flags]
    public enum Element
    {
        Unknown = 0,
        Arcane = 1 << 1,
        Cold = 1 << 2,
        Fire = 1 << 3,
        Holy = 1 << 4,
        Lightning = 1 << 5,
        Physical = 1 << 6,
        Poison = 1 << 7,
        Any = Arcane | Cold | Fire | Holy | Lightning | Physical | Poison
    }

    public enum SpellCategory
    {
        Unknown = 0,
        Primary,
        Secondary,
        Defensive,
        Might,
        Tactics,
        Rage,
        Techniques,
        Focus,
        Mantras,
        Utility,
        Laws,
        Conviction,
        Voodoo,
        Decay,
        Terror,
        Hunting,
        Archery,
        Devices,
        Conjuration,
        Mastery,
        Force,
        Archon,
        Curses,
        Reanimation,
        Corpses,
        BloodAndBone
    }

    public enum Resource
    {
        Unknown = 0,
        None,
        Fury,
        Arcane,
        Wrath,
        Mana,
        Discipline,
        Hatred,
        Spirit,
        Essense,
    }

    public enum ItemQualityName
    {
        Inferior,
        Normal,
        Magic,
        Rare,
        Legendary,
        Set,
        Special
    }

    public enum ResourceEffectType
    {
        None = 0,
        Generator,
        Spender
    }


    [Flags]
    public enum EffectTypeFlags
    {
        None = 0,
        Stun = 1 << 0,
        Knockback = 1 << 1,
        Immobilize = 1 << 2,
        Chill = 1 << 3,
        Blind = 1 << 4,
        Charm = 1 << 5,
        Slow = 1 << 6,
        Freeze = 1 << 7,

        Any = Freeze | Stun | Knockback | Immobilize | Chill | Blind | Charm | Slow
    }


    public enum TrinityPetType
    {
        Unknown = 0,
        Sentry,
        Hydra,
        ZombieDog,
        MysticAlly,
        Gargantuan,
        Companion,
        Hireling,
    }

}