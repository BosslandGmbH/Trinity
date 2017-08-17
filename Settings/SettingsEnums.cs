
using System;
using System.ComponentModel;

namespace Trinity.Settings
{
    [Flags]
    public enum GambleSlotTypes
    {
        None = 0,
        OneHanded = 1 << 0,
        TwoHanded = 1 << 1,
        Mojo = 1 << 2,
        Quiver = 1 << 3,
        Orb = 1 << 4,
        Amulet = 1 << 5,
        Ring = 1 << 6,
        Belt = 1 << 7,
        Boots = 1 << 8,
        Bracers = 1 << 9,
        Chest = 1 << 10,
        Gloves = 1 << 11,
        Helm = 1 << 12,
        Pants = 1 << 13,
        Shield = 1 << 14,
        Shoulder = 1 << 15,
        Phylactery = 1 << 16,
    }

    [Flags]
    public enum GameStopReasons
    {
        None = 0,
        GoblinFound = 1 << 0,
        UrshiFound = 1 << 1,
        DeathGateFound = 1 << 2,
        UniqueFound = 1 << 3,
        //Quiver = 1 << 3,
        //Orb = 1 << 4,
        //Amulet = 1 << 5,
        //Ring = 1 << 6,
        //Belt = 1 << 7,
        //Boots = 1 << 8,
        //Bracers = 1 << 9,
        //Chest = 1 << 10,
        //Gloves = 1 << 11,
        //Helm = 1 << 12,
        //Pants = 1 << 13,
        //Shield = 1 << 14,
        //Shoulder = 1 << 15,
    }

    [Flags]
    public enum ShrineTypes
    {
        None = 0,
        Fortune = 1 << 0,
        Frenzied = 1 << 1,
        Reloaded = 1 << 2,
        Enlightened = 1 << 3,
        Glow = 1 << 4,
        RunSpeed = 1 << 5,
        Goblin = 1 << 6,
        Hoarder = 1 << 7,
        Shield = 1 << 8,
        Speed = 1 << 9,
        Casting = 1 << 10,
        Damage = 1 << 11,
        Conduit = 1 << 12,
    }

    [Flags]
    public enum ContainerTypes
    {
        None = 0,
        Corpse = 1 << 0,        
        NormalChest = 1 << 1,
        WeaponRack = 1 << 2,
        GroundClicky = 1 << 3,
        Other = 1 << 4,
        RareChest = 1 << 5,
    }

    [Flags]
    public enum GlobeTypes
    {
        None = 0,
        Health = 1 << 0,
        Power = 1 << 1,
        NephalemRift = 1 << 2,
        GreaterRift = 1 << 3,
    }

    [Flags]
    public enum SpecialTypes
    {
        None = 0,

        [Description("Resurrect Dead Players")]
        PlayerTombstone = 1 << 0,
    }

    [Flags]
    public enum PickupItemQualities
    {
        None = 0,
        Grey = 1 << 0,
        White = 1 << 1,
        Blue = 1 << 2,
        Yellow = 1 << 3,
        Green = 1 << 4,
        Orange = 1 << 5,
    } 

    [Flags]
    public enum SpecialItemTypes
    {
        None = 0,
        CraftingPlans = 1 << 0,
        DeathsBreath = 1 << 1,
        VeiledCrystals = 1 << 2,
        ReusableParts = 1 << 2,
        ArcaneDust = 1 << 3,
        BloodShards = 1 << 4,
        KeywardenIngredients = 1 << 5,
        TransmogWhites = 1 << 6,
        StaffOfHeardingParts = 1 << 7,
        Pets = 1 << 8,
        Wings = 1 << 9,
        TieredLootrunKey = 1 << 10,
        RottenMushroom = 1 << 11,
        Lore = 1 << 12,
        CultistPage = 1 << 13,

        Defaults = CraftingPlans | DeathsBreath | VeiledCrystals | ReusableParts | ArcaneDust | BloodShards | 
            KeywardenIngredients | StaffOfHeardingParts | Pets | Wings | TieredLootrunKey | RottenMushroom,
        
    }

    [Flags]
    public enum EliteTypes
    {
        None = 0,

        [Description("Yellow Elites")]
        Rare = 1 << 0,

        [Description("Subordinates of Rare Elites")]
        Minion = 1 << 1,

        [Description("Blue Elites")]
        Champion = 1 << 2,
    }

    public enum SalvageOption
    {
        Sell,
        Salvage,
        None,
        InfernoOnly,
        All
    }

    public enum DropInTownOption
    {
        None = 0,
        Vendor,
        Keep,
        All
    }

    public enum LegendaryMode
    {
        None = 0,
        AlwaysStash,
        Ignore,
        StashAncients,
        ItemList
    }

    public enum LootHandlingMode
    {
        None = 0,
        Stash,
        Ignore,
        Salvage,
        Sell,
        Drop
    }

    public enum CubeExtractOption
    {
        None = 0,
        OnlyTrashed,
        OnlyNonAncient,
        All
    }

    public enum ItemRuleType
    {
        Config,
        Soft,
        Hard,
        Custom
    }

    public enum ItemFilterMode
    {
        None = 0,
        TrinityOnly,
        TrinityWithItemRules,
        DemonBuddy,
        ItemRanks,
        ItemList
    }

    public enum ItemRankMode
    {
        AnyClass,
        HeroOnly,
    }

    public enum RoutineMode
    {
        None = 0,
        Automatic,
        Manual,
    }

    [Flags]
    public enum TrinityGemType
    {
        None = 1,
        Emerald = 2,
        Topaz = 4,
        Amethyst = 8,
        Ruby = 16,
        Diamond = 32,
        All = Emerald | Topaz | Amethyst | Ruby | Diamond
    }

    public enum ItemRuleLogLevel
    {
        None = 0,
        Normal = 1,
        Magic = 2,
        Rare = 3,
        Legendary = 4
    }

    public enum FollowerBossFightMode
    {
        None = 0,
        AlwaysAccept,
        DeclineInBounty,
        AlwaysDecline,
    }

    public enum TargetPriority
    {
        Ignore = 0,
        Normal = 1,
        Prioritize = 2,
        Kamikaze = 3
    }
    public enum TempestRushOption
    {
        MovementOnly = 0,
        ElitesGroupsOnly = 1,
        CombatOnly = 2,
        Always = 3,
        TrashOnly = 4,
    }
    public enum WizardKiteOption
    {
        Anytime,
        ArchonOnly,
        NormalOnly
    }

    public enum WizardArchonCancelOption
    {
        Never,
        Timer,
        RebuffArmor,
        RebuffMagicWeaponFamiliar,
    }

    public enum DestructibleIgnoreOption
    {
        ForceIgnore,
        OnlyIfStuck,
        DestroyAll
    }

    public enum CrusaderAkaratsMode
    {
        HardElitesOnly,
        Normal,
        WhenReady,
        WhenInCombat
    }

    public enum MonkEpiphanyMode
    {
        HardElitesOnly,
        Normal,
        WhenReady,
        WhenInCombat
    }

    public enum BarbarianWOTBMode
    {
        HardElitesOnly,
        Normal,
        WhenReady,
        WhenInCombat
    }

    public enum BarbarianSprintMode
    {
        Always,
        CombatOnly,
        MovementOnly
    }

    public enum DemonHunterVaultMode
    {
        Always,
        CombatOnly,
        MovementOnly
    }

    public enum KiteMode
    {
        Never,
        Always,
        Bosses,
        Elites,
    }

    public enum KiteVariation
    {
        None = 0,
        NearTargetCluster,
        DistantEmptySpace,
    }

    public enum TrinityItemQuality
    {
        Invalid = -1,
        None = 0,
        Inferior,
        Common,
        Magic,
        Rare,
        Legendary,
        Set
    }

    public enum SettingMode
    {
        None = 0,
        Enabled,
        Disabled,
        Selective,
        Auto
    }
}

