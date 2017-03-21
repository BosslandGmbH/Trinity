namespace Trinity.Framework.Objects.Memory
{
    public enum ShaderAType // 2.5.0.44030 @27712528 index:0
    {
        _32bitARGB = 0,
        _16bitARGB = 3,
        _16bit1bitalpha = 4,
        _16bitRGB = 6,
        _15bitRGB = 5,
        _8bitalpha = 23,
        _8bitgrayscale = 7,
        Compressednoalpha = 9,
        Compressed1bitalpha = 10,
        Compressed4bitalpha = 11,
        Compressed8bitinterpalpha = 12,
        CompressedNormalMap = 43,
    }

    public enum ShaderBType // 2.5.0.44030 @27712632 index:1
    {
        _32bitARGB = 0,
        _32bitRGBA = 1,
        _32bitRGB = 2,
        _16bitARGB = 3,
        _16bit1bitalpha = 4,
        _16bitRGB = 6,
        _15bitRGB = 5,
        _8bitalpha = 23,
        _8bitgrayscale = 7,
        _Compressednoalpha = 9,
        _Compressed1bitalpha = 10,
        _Compressed4bitalpha = 11,
        _Compressed8bitinterpalpha = 12,
        _CompressedNormalMap = 43,
        _32bitlinearARGB = 13,
        _32bitlinearRGBA = 14,
        _32bitlinearRGB = 15,
        _16bitlinearARGB = 16,
        _16bitlinear1bitalpha = 17,
        _15bitlinearRGB = 18,
        _16bitlinearRGB = 19,
        _8bitlineargrayscale = 20,
        _32bitABGR = 21,
        _24bitRGB = 22,
        _32bitfloatingpointR32F = 24,
        _16bitfloatingpoint = 25,
        _32bitfloatingpoint = 26,
        _16bitgrayscale = 27,
        _R16G16B16 = 28,
        _A16R16G16B16 = 29,
        _24bitdepthbuffer = 30,
        _16bitdepthbuffer = 31,
        _24bitintegerdepthATIonly = 32,
        _16bitintegerdepthATIonly = 33,
        _24bitdepthbuffer2 = 34,
        _RawZ = 35,
        _IntZ = 36,
        _DepthOpaque = 37,
        _16bitfloatingpointR16F = 38,
        _G32FR32F = 39,
        _G16FR16F = 40,
        _A16B16G16R16 = 41,
        _A2B10G10R10 = 42,
        _CompressedNormalMap2 = 43,
    }


    public enum VisualEffectType // 2.5.0.44030 @27712992 index:2
    {
        ProjectedShadowMap = 9,
        Floors = 12,
        Opaque = 5,
        Sky = 0,
        TransparentGround = 2,
        GroundFX = 14,
        Auras = 13,
        ProjectedAdditive = 15,
        Reflection = 11,
        Transparent = 6,
        Bloom = 7,
        MotionBlur = 26,
        PostFXDistortion = 3,
        WaterSimulation = 4,
        InteractiveFog = 16,
        Highlights = 17,
        PostFX = 8,
        DominoDebug = 20,
        UI = 10,
        UIOverlay = 19,
        BattleNetUI = 18,
        DepthPrepass = 21,
        OcclusionQuery = 22,
        TransparentMasked = 23,
        ShadowMapStencilBounds = 24,
        HighlightStencilBounds = 25,
    }

    public enum BonusType // 2.5.0.44030 @27714728 index:6
    {
        None = 0,
        Lightning = 1,
        Cold = 2,
        Fire = 3,
        Poison = 4,
        Arcane = 5,
        WitchdoctorDamage = 6,
        LifeSteal = 7,
        ManaSteal = 8,
        MagicFind = 9,
        GoldFind = 10,
        AttackSpeedBonus = 11,
        CastSpeedBonus = 12,
        Holy = 13,
        WizardDamage = 14,
    }

    //public enum GameBalanceType // 2.5.0.44030 @27714856 index:7
    //{
    //    ItemTypes = 1,
    //    Items = 2,
    //    ExperienceTable = 3,
    //    HelpCodes = 24,
    //    MonsterLevels = 5,
    //    Heros = 7,
    //    AffixList = 8,
    //    MovementStyles = 10,
    //    Labels = 11,
    //    LootDistribution = 12,
    //    RareItemNames = 16,
    //    MonsterAffixes = 18,
    //    MonsterNames = 19,
    //    SocketedEffects = 21,
    //    ItemDropTable = 25,
    //    ItemLevelModifiers = 26,
    //    QualityClasses = 27,
    //    Handicaps = 28,
    //    ItemSalvageLevels = 29,
    //    Scenery = 17,
    //    Hirelings = 4,
    //    SetItemBonuses = 33,
    //    EliteModifiers = 34,
    //    ItemTiers = 35,
    //    PowerFormulaTables = 36,
    //    Recipes = 32,
    //    ScriptedAchievementEvents = 37,
    //    LootRunQuestTiers = 39,
    //    ParagonBonuses = 40,
    //    LegacyItemConversions = 45,
    //    EnchantItemAffixUseCountCostScalars = 46,
    //    TieredLootRunLevels = 49,
    //    TransmuteRecipes = 50,
    //    CurrencyConversions = 51,
    //}

    public enum CombinationType // 2.5.0.44030 @27715136 index:8
    {
        None = -1,
        Total = 0,
        Agg = 1,
        Base = 2,
    }

    public enum WeaponsComboType // 2.5.0.44030 @27715176 index:9
    {
        None = -1,
        DefaultActor = 0,
        TargetActor = 1,
        Game = 2,
    }

    public enum ArmorType // 2.5.0.44030 @27715216 index:10
    {
        None = 0,
        LightA = 1,
        LightB = 2,
        LightC = 7,
        MediumA = 3,
        MediumB = 4,
        MediumC = 8,
        HeavyA = 5,
        HeavyB = 6,
        HeavyC = 9,
        ClassLightA = 10,
        ClassLightB = 11,
        ClassLightC = 12,
        ClassMediumA = 13,
        ClassMediumB = 14,
        ClassMediumC = 15,
        ClassHeavyA = 16,
        ClassHeavyB = 17,
        ClassHeavyC = 18,
    }

    //public enum MonsterType // 2.5.0.44030 @27715376 index:11
    //{
    //    Unknown = -1,
    //    Undead = 0,
    //    Demon = 1,
    //    Beast = 2,
    //    Human = 3,
    //    Breakable = 4,
    //    Scenery = 5,
    //    Ally = 6,
    //    Team = 7,
    //    Helper = 8,
    //    CorruptedAngel = 9,
    //    Pandemonium = 10,
    //    Adria = 11,
    //    BloodGolem = 12,
    //}

    //public enum MonsterRace // 2.5.0.44030 @27715496 index:12
    //{
    //    Unknown = 0,
    //    Fallen = 1,
    //    GoatMen = 2,
    //    Rogue = 3,
    //    Skeleton = 4,
    //    Zombie = 5,
    //    Spider = 6,
    //    Triune = 7,
    //    WoodWraith = 8,
    //    Human = 9,
    //    Animal = 10,
    //    TreasureGoblin = 11,
    //    CrazedAngel = 12,
    //}

    public enum DamageType // 2.5.0.44030 @27715680 index:14
    {
        Unknown = 0,
        Melee = 1,
        Ranged = 2,
        Magic = 3,
        DOT = 4,
        Thorns = 5,
    }

    public enum HealingType // 2.5.0.44030 @27715736 index:15
    {
        Direct = 0,
        DirectOverTime = 1,
        LifeSteal = 2,
        HealthGlobe = 3,
        Potion = 4,
        Shrine = 5,
        Resurrection = 6,
        Absorb = 7,
        Regen = 8,
        OnKill = 9,
        OnHit = 10,
    }

    //public enum MonsterSize // 2.5.0.44030 @27715832 index:16
    //{
    //    Unknown = -1,
    //    Big = 3,
    //    Standard = 4,
    //    Ranged = 5,
    //    Swarm = 6,
    //    Boss = 7,
    //}

    //public enum MonsterQuality // 2.5.0.44030 @27715888 index:17
    //{
    //    Unknown = 0,
    //    Champion = 1,
    //    Rare = 2,
    //    Minion = 3,
    //    Unique = 4,
    //    Hireling = 5,
    //    Boss = 7,
    //}

    public enum ActType // 2.5.0.44030 @27715952 index:18
    {
        Invalid = -1,
        A1 = 0,
        A2 = 100,
        A3 = 200,
        A4 = 300,
        A5 = 400,
        OpenWorld = 3000,
        Test = 1000,
    }

    public enum VendorSlot // 2.5.0.44030 @27716024 index:19
    {
        PlayerBackpack = 0,
        PlayerHead = 1,
        PlayerTorso = 2,
        PlayerRightHand = 3,
        PlayerLeftHand = 4,
        PlayerHands = 5,
        PlayerWaist = 6,
        PlayerFeet = 7,
        PlayerShoulders = 8,
        PlayerLegs = 9,
        PlayerBracers = 10,
        PlayerLeftFinger = 12,
        PlayerRightFinger = 11,
        PlayerNeck = 13,
        Merchant = 18,
        PetRightHand = 20,
        PetLeftHand = 21,
        PetSpecial = 22,
        PetLeftFinger = 25,
        PetRightFinger = 24,
        PetNeck = 23,
    }

    //public enum ItemQuality // 2.5.0.44030 @27716200 index:20
    //{
    //    Invalid = -1,
    //    Inferior = 0,
    //    Normal = 1,
    //    Superior = 2,
    //    Magic1 = 3,
    //    Magic2 = 4,
    //    Magic3 = 5,
    //    Rare4 = 6,
    //    Rare5 = 7,
    //    Rare6 = 8,
    //    Legendary = 9,
    //    Special = 10,
    //    Set = 11,
    //}

    //public enum GemType // 2.5.0.44030 @27716312 index:21
    //{
    //    Amethyst = 1,
    //    Emerald = 2,
    //    Ruby = 3,
    //    Topaz = 4,
    //    Diamond = 5,
    //}

    public enum CosmeticType // 2.5.0.44030 @27716360 index:22
    {
        CosmeticsWings = 1,
        CosmeticsPets = 3,
        CosmeticsPennants = 2,
        CosmeticsPortraitFrames = 4,
    }

    public enum UnknownType // 2.5.0.44030 @27716400 index:23
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
    }

    public enum MonsterGroup // 2.5.0.44030 @27716440 index:24
    {
        All = 0,
        Rares = 1,
        Shooters = 2,
        Champions = 3,
    }

    public enum ToolTipType // 2.5.0.44030 @27716480 index:25
    {
        Items = 0,
        Minimap = 1,
        UI = 2,
    }

    public enum SoundAdjustmentType // 2.5.0.44030 @27716760 index:30
{
        Volume = 0,
        Pitch = 1,
    }

    public enum CurveType // 2.5.0.44030 @27716784 index:31
    {
        Linear = 0,
        Logarithmic = 1,
        SCurve = 2,
    }

    public enum ShapeType // 2.5.0.44030 @27716816 index:32
    {
        Line = 0,
        Cross = 1,
        Circle = 2,
    }

    public enum StuckFlags // 2.5.0.44030 @27716848 index:33
    {
        IgnoreCollision = 0,
        NoPathfinding = 1,
        NoSteering = 2,
        ArrivingAtDestination = 3,
        AutoUpdateYaw = 4,
        RemoteMovement = 5,
        NoSnapToGround = 6,
        CheckVertical = 7,
        Snap = 8,
        FromPower = 9,
        WasStuck = 10,
    }

    public enum DifficultyType // 2.5.0.44030 @27716944 index:34
    {
        Normal = 0,
        Nightmare = 1,
        Hell = 2,
        Inferno = 3,
        X1 = 4,
        Dynamic = 5,
    }

    public enum Difficulty // 2.5.0.44030 @27717000 index:35
    {
        None = 0,
        Normal = 1,
        Hard = 2,
        Expert = 3,
        Master = 4,
        Torment1 = 5,
        Torment2 = 6,
        Torment3 = 7,
        Torment4 = 8,
        Torment5 = 9,
        Torment6 = 10,
        Torment7 = 11,
        Torment8 = 12,
        Torment9 = 13,
        Torment10 = 14,
        Torment11 = 15,
        Torment12 = 16,
        Torment13 = 17,
        Torment14 = 18,
        Torment15 = 19,
    }


    public enum NpcType // 2.5.0.44030 @27717168 index:36
{
        Blacksmith = 0,
        Jeweler = 1,
        Mystic = 2,
        JewelUpgrade = 4,
        Horadrim = 3,
        None = -1,
    }

    public enum CurrencyType // 2.5.0.44030 @27717224 index:37
    {
        None = -1,
        Gold = 0,
        BloodShards = 1,
        Platinum = 2,
        ReusableParts = 3,
        ArcaneDust = 4,
        VeiledCrystal = 5,
        DeathsBreath = 6,
        ForgottenSoul = 7,
        KhanduranRune = 8,
        CaldeumNightshade = 9,
        ArreatWarTapestry = 10,
        CorruptedAngelFlesh = 11,
        WestmarchHolyWater = 12,
        DemonOrganDiablo = 13,
        DemonOrganGhom = 14,
        DemonOrganSiegeBreaker = 15,
        DemonOrganSkeletonKing = 16,
        DemonOrganEye = 17,
        DemonOrganSpineCord = 18,
        DemonOrganTooth = 19,
    }

    public enum SocketResult // 2.5.0.44030 @27717400 index:38
    {
        Invalid = -1,
        ItemNotSocketable = 0,
        NoFreeSockets = 1,
        LevelRestrictionNotMet = 2,
        ItemTypeMismatch = 3,
        Unidentified = 4,
        Success = 5,
        UniqueEquipped = 6,
    }

    public enum SpecialInventoryType // 2.5.0.44030 @27717472 index:39
    {
        Invalid = -1,
        X1Templar = 0,
        X1Scoundrel = 1,
        X1Enchantress = 2,
        X1DeadKing = 3,
        X1Jeweler = 4,
        X1Blacksmith = 5,
        P1GemNPC = 6,
        P1CrazedHermit = 7,
    }

    public enum CollisionResult // 2.5.0.44030 @27717552 index:40
    {
        None = 0,
        Walking = 1,
        WalkingInPlace = 2,
        WalkFinishing = 3,
        FixedProjectile = 4,
        FixedProjectileColliding = 5,
        Arc = 6,
        ClientAuth = 7,
        ClientAuthFacing = 8,
        DeterministicPath = 9,
        StoppedProjectile = 10,
    }

//    public enum QuestStepObjectiveType // 2.5.0.44030 @27717648 index:41
//{
//        HadConversation = 0,
//        PossessItem = 1,
//        KillMonster = 2,
//        InteractWithActor = 3,
//        EnterLevelArea = 4,
//        EnterScene = 5,
//        EnterWorld = 6,
//        EnterTrigger = 7,
//        CompleteQuest = 8,
//        PlayerFlagSet = 9,
//        TimedEventExpired = 10,
//        KillGroup = 11,
//        GameFlagSet = 12,
//        EventReceived = 13,
//        KillMonsterFromGroup = 14,
//        KillMonsterFromFamily = 15,
//        KillElitePackInLevelArea = 16,
//        KillAnyMonsterInLevelArea = 17,
//        KillAllMonstersInWorld = 18,
//        HQActReagentsCollected = 19,
//    }

//public enum QuestType // 2.5.0.44030 @27717888 index:43
//{
//    MainQuest = 0,
//    Event = 2,
//    Challenge = 4,
//    Bounty = 5,
//    HoradricQuest = 6,
//    SetDungeon = 7,
//    SetDungeonBonus = 8,
//    SetDungeonMastery = 9,
//    SetDungeonTracker = 10,
//}

    public enum BountyQuestType // 2.5.0.44030 @27717968 index:44
    {
        None = -1,
        KillUnique = 0,
        KillBoss = 1,
        CompleteEvent = 2,
        ClearDungeon = 3,
        Camps = 4,
        HQColorOrbs = 5,
        HQHotCold = 6,
    }

    public enum BountyEventType // 2.5.0.44030 @27718040 index:45
    {
        None = -1,
        TimedDungeon = 0,
        WaveFight = 1,
        Horde = 2,
        Zapper = 3,
        GoblinHunt = 4,
    }

    public enum QuestType // 2.5.0.44030 @27718096 index:46
    {
        Invalid = -1,
        Idle = 0,
        Bounty = 1,
        DungeonFinder = 2,
        ResetPending = 3,
    }

    public enum RecipeType // 2.5.0.44030 @27718144 index:47
{
        NoItem = 0,
        SharedRecipe = 1,
        ClassRecipe = 2,
        TreasureClass = 3,
    }

    public enum PathType // 2.5.0.44030 @27718184 index:48
{
        PathA = 0,
        PathB = 1,
        PathC = 2,
        PathD = 3,
        PathE = 4,
        PathF = 5,
        PathG = 6,
        PathH = 7,
        PathI = 8,
        PathJ = 9,
        PathK = 10,
        PathL = 11,
        PathM = 12,
        PathN = 13,
        PathO = 14,
    }

    public enum AmbientConversationType // 2.5.0.44030 @27719688 index:50
    {
        QuestEvent = 13,
        QuestFloat = 12,
        QuestStandard = 11,
        TalkMenuGossip = 10,
        AmbientGossip = 9,
        LoreBook = 8,
        GlobalFloat = 7,
        GlobalChatter = 6,
        PlayerCallout = 5,
        FollowerCallout = 4,
        FollowerBanter = 3,
        AmbientFloat = 2,
        PlayerEmote = 1,
        FollowerSoundset = 0,
    }

    public enum ConversationType // 2.5.0.44030 @27719808 index:51
    {
        QuestEventGlobalSkippableDialog = 13,
        QuestFloatGlobalSkippable = 12,
        QuestStandardLocalSkippableDialog = 11,
        TalkMenuGossipLocalSkippableDialog = 10,
        AmbientGossipLocalNotskippable = 9,
        LoreBook = 8,
        GlobalFloatGlobalNotskippable = 7,
        GlobalChatterGlobalNotskippableOverlaps = 6,
        PlayerCalloutLocalNotskipppable = 5,
        FollowerCalloutLocalNotskippable = 4,
        FollowerBanterLocalNotskippable = 3,
        AmbientFloatLocalNotskippable = 2,
        PlayerEmoteGlobalNotskippable = 1,
        FollowerSoundsetLocalNotskippable = 0,
    }

    //public enum FollowerType // 2.5.0.44030 @27719928 index:52
    //{
    //    None = -1,
    //    Player = 0,
    //    PrimaryNPC = 1,
    //    AltNPC1 = 2,
    //    AltNPC2 = 3,
    //    AltNPC3 = 4,
    //    AltNPC4 = 5,
    //    TemplarFollower = 6,
    //    ScoundrelFollower = 7,
    //    EnchantressFollower = 8,
    //}

    public enum ComparisonType // 2.5.0.44030 @27720016 index:53
    {
        None = -1,
        EqualTo = 0,
        LessThan = 1,
        GreaterThan = 2,
        LessThanOrEqualTo = 3,
        GreaterThanOrEqualTo = 4,
        NotEqualTo = 5,
    }

    public enum AdjustmentType // 2.5.0.44030 @27720080 index:54
    {
        None = -1,
        IncrementedBy = 0,
        DecrementedBy = 1,
        SetTo = 2,
    }

    public enum UnknownType2 // 2.5.0.44030 @27720120 index:55
    {
        None = -1,
        Unknown = 0,
        Unknown1 = 1,
        Unknown2 = 2,
        Unknown3 = 3,
        Unknown4 = 4,
        Unknown5 = 5,
    }

    public enum UnknownType3 // 2.5.0.44030 @27720184 index:56
    {
        None = -1,
        Unknown1 = 0,
        Unknown2 = 1,
        Unknown3 = 2,
    }

    public enum UnknownType4 // 2.5.0.44030 @27720224 index:57
    {
        Disabled = 1,
        Native = 0,
        Auto = 2,
    }

    public enum ClothingDisplaySlot // 2.5.0.44030 @27720536 index:63
    {
        None = 0,
        HPclothplane1 = 1,
        HPclothplane2 = 2,
        HPclothplane3 = 3,
        HPclothplane4 = 4,
        HPclothplane5 = 5,
        HPclothplane6 = 6,
        HPclothplane7 = 7,
        HPclothplane8 = 8,
    }

    public enum ParagonType // 2.5.0.44030 @27720744 index:65
{
        PrimaryAttributes = 0,
        Attack = 1,
        Defense = 2,
        Utility = 3,
    }

    public enum NodeFlags // 2.5.0.44030 @27720928 index:67
    {
        SCRIPTREFERENCEPCANDCONSOLE = 0,
        SCRIPTREFERENCEPCONLY = 1,
        SCRIPTREFERENCECONSOLEONLY = 2,
        SCRIPTSOFTREFERENCEPCANDCONSOLE = 3,
        SCRIPTSOFTREFERENCEPCONLY = 4,
        SCRIPTSOFTREFERENCECONSOLEONLY = 5,
    }

    public enum SequenceState // 2.5.0.44030 @27720984 index:68
    {
        Invalid = -1,
        Precursor = 0,
        Active = 1,
        Ended = 2,
    }

    public enum RuneSlot // 2.5.0.44030 @27721024 index:69
    {
        None = -1,
        RuneA = 0,
        RuneB = 1,
        RuneC = 2,
        RuneD = 3,
        RuneE = 4,
    }

    public enum GizmoType // 2.5.0.44030 @27721144 index:71
    {
        Unknown = -1,
        Door = 0,
        BreakableDoor = 56,
        Chest = 1,
        Sign = 8,
        Portal = 2,
        PortalDestination = 20,
        Waypoint = 4,
        HealingWell = 9,
        PowerUp = 10,
        TownPortal = 11,
        HearthPortal = 12,
        Headstone = 18,
        BreakableChest = 23,
        SharedStash = 25,
        Spawner = 28,
        Trigger = 44,
        SecretPortal = 47,
        DestroyableObject = 48,
        Checkpoint = 7,
        Switch = 57,
        LootRunSwitch = 78,
        PressurePlate = 58,
        Gate = 59,
        DestroySelfWhenNear = 60,
        ActTransitionObject = 62,
        ReformingDestroyableObject = 63,
        Banner = 64,
        LoreChest = 65,
        BossPortal = 66,
        PlacedLoot = 67,
        SavePoint = 68,
        ReturnPointPortal = 69,
        DungeonPortal = 70,
        ReturnPortal = 75,
        PageOfFatePortal = 33,
        IdentifyAll = 71,
        RecreateGameWithParty = 76,
        Mailbox = 77,
        PoolOfReflection = 79,
        MultiClick = 80,
        ObjectiveFinder = 82,
    }

    //public enum InventorySlot // 2.5.0.44030 @27721488 index:72
    //{
    //    BackpackItems = 0,
    //    Head = 1,
    //    Torso = 2,
    //    RightHand = 3,
    //    LeftHand = 4,
    //    Hands = 5,
    //    Waist = 6,
    //    Feet = 7,
    //    Shoulders = 8,
    //    Legs = 9,
    //    Bracers = 10,
    //    LeftFinger = 12,
    //    RightFinger = 11,
    //    Neck = 13,
    //    Buyback = 14,
    //    SharedStash = 15,
    //    BaseHealthPotion = 16,
    //    HirelingRightHand = 20,
    //    HirelingLeftHand = 21,
    //    HirelingSpecial = 22,
    //    HirelingLeftFinger = 25,
    //    HirelingRightFinger = 24,
    //    HirelingNeck = 23,
    //}

    public enum ConsumableType // 2.5.0.44030 @27721680 index:73
    {
        None = 0,
        HealthPotion = 1,
        TownPortalStone = 3,
        TownPortalUnlock = 54,
        HearthPortalScroll = 24,
        CalldownGrenade = 6,
        PageofFate = 21,
        HealthGlyph = 25,
        Scroll = 28,
        ManaGlyph = 31,
        ArcaneGlyph = 50,
        Dye = 32,
        DyeReusable = 57,
        LearnEnchant = 38,
        LearnRecipe = 39,
        SummonMonster = 46,
        SummonCompanion = 48,
        PageofTraining = 53,
        PowerUpGlyph = 55,
        CollectorsEditionBuff = 56,
        PortalScroll = 19,
        PowerScroll = 20,
        EnchantScroll = 58,
        ShrineGlyph = 59,
        UnlockTransmogs = 60,
        TreasureBag = 61,
        PlayerGift = 62,
        AddSockets = 63,
        LootRunProgressGlyph = 64,
        LearnWings = 65,
        LearnPet = 67,
        LearnPortraitFrame = 68,
        LearnPennant = 69,
        HoradricReagent = 70,
    }

    public enum DyeColor // 2.5.0.44030 @27721960 index:74
    {
        None = 0,
        DyeVanishing = 1,
        Dye01 = 2,
        Dye02 = 3,
        Dye03 = 4,
        Dye04 = 5,
        Dye05 = 6,
        Dye06 = 7,
        Dye07 = 8,
        Dye08 = 9,
        Dye09 = 10,
        Dye10 = 11,
        Dye11 = 12,
        Dye12 = 13,
        Dye13 = 14,
        Dye14 = 15,
        Dye15 = 16,
        Dye16 = 17,
        Dye17 = 18,
        Dye18 = 19,
        Dye19 = 20,
        Dye20 = 21,
        Dye21 = 22,
    }

    public enum DyeType // 2.5.0.44030 @27722152 index:75
    {
        None = 0,
        DyeVanishing = 21,
        Dye01 = 1,
        Dye02 = 2,
        Dye03 = 3,
        Dye04 = 4,
        Dye05 = 5,
        Dye06 = 6,
        Dye07 = 7,
        Dye08 = 8,
        Dye09 = 9,
        Dye10 = 10,
        Dye11 = 11,
        Dye12 = 12,
        Dye13 = 13,
        Dye14 = 14,
        Dye15 = 15,
        Dye16 = 16,
        Dye17 = 17,
        Dye18 = 18,
        Dye19 = 19,
        Dye20 = 20,
        Dye21 = 22,
    }

    public enum PetType // 2.5.0.44030 @27723408 index:79
    {
        None = -1,
        Hireling = 0,
        WizardMirrorImage = 1,
        WdWickerman = 2,
        WdDeathTotem = 3,
        WdFireTotem = 4,
        WdZombie = 5,
        Helper = 6,
        RangerDustdevil = 7,
        Pet0 = 8,
        Pet1 = 9,
        Gargantuan = 10,
        Pet3 = 11,
        Pet4 = 12,
        Pet5 = 13,
        Pet6 = 14,
        Pet0Static = 15,
        Pet1Static = 16,
        Pet2Static = 17,
        Pet3Static = 18,
        Pet4Static = 19,
        Pet5Static = 20,
        Pet6Static = 21,
        Follower = 22,
        Companion = 23,
        HirelingProxy = 24,
        FollowerProxy = 25,
        ItemProcPet0 = 26,
        ItemProcPet1 = 27,
        ItemProcPetSlotPlayerHead = 28,
        ItemProcPetSlotPlayerTorso = 29,
        ItemProcPetSlotPlayerRightHand = 30,
        ItemProcPetSlotPlayerLeftHand = 31,
        ItemProcPetSlotPlayerHands = 32,
        ItemProcPetSlotPlayerWaist = 33,
        ItemProcPetSlotPlayerFeet = 34,
        ItemProcPetSlotPlayerShoulders = 35,
        ItemProcPetSlotPlayerLegs = 36,
        ItemProcPetSlotPlayerBracers = 37,
        ItemProcPetSlotPlayerRightFinger = 38,
        ItemProcPetSlotPlayerLeftFinger = 39,
        ItemProcPetSlotPlayerNeck = 40,
        ItemProcPetSlotPlayerRightHand2 = 41,
        ItemProcPetSlotPlayerLeftHand2 = 42,
        ItemProcPetSlotPlayerSpecial = 43,
        ItemProcPetSlotPlayerNeck2 = 44,
        ItemProcPetSlotPlayerRightFinger2 = 45,
        ItemProcPetSlotPlayerLeftFinger2 = 46,
        ItemProcPetSet00 = 47,
        ItemProcPetSet01 = 48,
        ItemProcPetSet02 = 49,
        ItemProcPetSet03 = 50,
        ItemProcPetSet04 = 51,
        ItemProcPetSet05 = 52,
        ItemProcPetSet06 = 53,
        ItemProcPetSet07 = 54,
        ItemProcPetSet08 = 55,
        ItemProcPetSet09 = 56,
        ItemProcPetSet10 = 57,
        ItemProcPetSet11 = 58,
        ItemProcPetSet12 = 59,
        ItemProcPetSet13 = 60,
        ItemProcPetSet14 = 61,
        ItemProcPetSet15 = 62,
        ItemProcPetSet16 = 63,
        ItemProcPetSet17 = 64,
        ItemProcPetSet18 = 65,
        ItemProcPetSet19 = 66,
    }

    public enum SpecialBonusType // 2.5.0.44030 @27723952 index:80
    {
        Massacre = 0,
        Destruction = 1,
        MightyBlow = 2,
        Pulverized = 3,
    }

    public enum LeaveGameResult // 2.5.0.44030 @27723992 index:81
{
        Normal = 0,
        Removed = 1,
        MismatchingProtocolVersion = 2,
        JoinFailed = 3,
        AssertMismatch = 4,
        AccountCreateFailed = 5,
        UsingNDBStorage = 6,
        ShutdownGame = 7,
        HardcoreDeath = 8,
        KickedbyPlayers = 9,
        GameCompleted = 10,
        NotRestrictedContentPlayerLegal = 11,
    }

    public enum SpecialBonus // 2.5.0.44030 @27724096 index:82
    {
        D3CollectorsEdition = 169,
        WebpromoDemonHunter = 198,
        WebpromoBarbarian = 199,
        WebpromoMonk = 200,
        WebpromoWitchDoctor = 201,
        WebpromoWizard = 202,
        D3BetaParticipant = 227,
        WOWMistsofPandariaCollectorsEdition = 229,
        WOWLegionCollectorsEdition = 16061,
        SC2HeartoftheSwarmCollectorsEdition = 233,
        D3X1Beta = 262,
        D3X1Lifetime = 263,
        D3X1CollectorsEdition = 264,
        Blizzcon2013 = 277,
        Blizzcon2014 = 442,
        SC2LegacyoftheVoidCollectorsEdition = 11153,
        HeroesoftheStormCrossPromotion = 11366,
        D3DigitalDeluxePremiumPassExtras = 11391,
        D3Blizzcon2015 = 11492,
        D3Blizzcon2016 = 16980,
        OverwatchCollectorsEdition = 16407,
    }

    public enum SpecialHeroFlags // 2.5.0.44030 @27724272 index:83
    {
        Seasonallevel70 = 0,
    }

    public enum AnimationTransitionType // 2.5.0.44030 @27724568 index:86
    {
        Animation = 0,
        PiecewiseLinearBlend = 1,
        BoneWeightedBlend = 2,
        SwitchBlend = 3,
        Pose = 4,
        AdditiveBlend = 5,
        BoneWeightsLinearBlend = 6,
        BoneWeightsMultiplyBlend = 7,
    }

    public enum MovementType // 2.5.0.44030 @27724640 index:87
{
        ForwardSpeed = 0,
        TurnSpeed = 1,
        IsIdle = 2,
        WeaponClass = 3,
        WalkSlowSpeed = 4,
        WalkSpeed = 5,
        RunSpeed = 6,
        SprintSpeed = 7,
        InTown = 8,
        IsSpecialMove = 9,
        AbsTurnSpeed = 10,
        AimYaw = 11,
        AimBlend = 12,
        IsAlive = 13,
        IsTurning = 14,
        ForceWalk = 15,
        None = 17,
    }

    public enum ContinuationType // 2.5.0.44030 @27724784 index:88
    {
        Continuous = 0,
        InactiveReset = 1,
    }

    public enum TransitionType // 2.5.0.44030 @27724912 index:91
    {
        Unknown = 0,
        TownPortal = 1,
        Waypoint = 2,
        ChangingActs = 3,
        HearthPortal = 4,
        Checkpoint = 5,
        Companion = 6,
        BossFight = 7,
        Elevator = 8,
        Portal = 9,
        Script = 10,
        Headstone = 11,
        ConsolePortal = 12,
        Challenge = 13,
        Cancelled = 14,
        ConsolePVPSS = 15,
        TieredRift = 16,
        SetDungeon = 17,
    }

    public enum IdentifierType // 2.5.0.44030 @27725064 index:92
{
        ConversationSNOs = 1,
        TutorialSNOs = 2,
        ItemActorSNOs = 3,
        LoreSNOs = 4,
        RecipeGBIDs = 5,
        TransmogGBIDs = 6,
        CosmeticItemGBIDs = 7,
    }

    public enum AffixType // 2.5.0.44030 @27725128 index:93
    {
        Prefix = 0,
        Suffix = 1,
        Inherent = 2,
        Title = 5,
        Quality = 6,
        Immunity = 7,
        Random = 9,
        Enchantment = 10,
        SocketEnhancement = 11,
    }

    public enum LightingType // 2.5.0.44030 @27713208 index:3
{
        Directional = 0,
        Point = 1,
        Pointlinearfalloff = 4,
        Spotlight = 2,
        Cylindrical = 3,
    }

    public enum KeyBoardKey  // 2.5.0.44030 @27713256 index:4
{
        Unknown = -1,
        ESCAPE = 0,
         N1 = 1,
        N2 = 2,
        N3 = 3,
        N4 = 4,
        N5 = 5,
        N6 = 6,
        N7 = 7,
        N8 = 8,
        N9 = 9,
        N0 = 10,
        MINUS = 11,
        EQUALS = 12,
        BACK = 13,
        TAB = 14,
        Q = 15,
        W = 16,
        E = 17,
        R = 18,
        T = 19,
        Y = 20,
        U = 21,
        I = 22,
        O = 23,
        P = 24,
        LBRACKET = 25,
        RBRACKET = 26,
        RETURN = 27,
        LCONTROL = 28,
        A = 29,
        S = 30,
        D = 31,
        F = 32,
        G = 33,
        H = 34,
        J = 35,
        K = 36,
        L = 37,
        SEMICOLON = 38,
        APOSTROPHE = 39,
        GRAVE = 40,
        LSHIFT = 41,
        BACKSLASH = 42,
        Z = 43,
        X = 44,
        C = 45,
        V = 46,
        B = 47,
        N = 48,
        M = 49,
        COMMA = 50,
        PERIOD = 51,
        SLASH = 52,
        RSHIFT = 53,
        MULTIPLY = 54,
        LMENU = 55,
        SPACE = 56,
        CAPITAL = 57,
        F1 = 58,
        F2 = 59,
        F3 = 60,
        F4 = 61,
        F5 = 62,
        F6 = 63,
        F7 = 64,
        F8 = 65,
        F9 = 66,
        F10 = 67,
        NUMLOCK = 68,
        SCROLL = 69,
        NUMPAD7 = 70,
        NUMPAD8 = 71,
        NUMPAD9 = 72,
        SUBTRACT = 73,
        NUMPAD4 = 74,
        NUMPAD5 = 75,
        NUMPAD6 = 76,
        ADD = 77,
        NUMPAD1 = 78,
        NUMPAD2 = 79,
        NUMPAD3 = 80,
        NUMPAD0 = 81,
        DECIMAL = 82,
        OEM102 = 83,
        F11 = 84,
        F12 = 85,
        F13 = 86,
        F14 = 87,
        F15 = 88,
        KANA = 89,
        ABNTC1 = 90,
        CONVERT = 91,
        NOCONVERT = 92,
        YEN = 93,
        ABNTC2 = 94,
        NUMPADEQUALS = 95,
        PREVTRACK = 96,
        AT = 97,
        COLON = 98,
        UNDERLINE = 99,
        KANJI = 100,
        STOP = 101,
        AX = 102,
        UNLABELED = 103,
        NEXTTRACK = 104,
        NUMPADENTER = 105,
        RCONTROL = 106,
        MUTE = 107,
        CALCULATOR = 108,
        PLAYPAUSE = 109,
        MEDIASTOP = 110,
        VOLUMEDOWN = 111,
        VOLUMEUP = 112,
        WEBHOME = 113,
        NUMPADCOMMA = 114,
        DIVIDE = 115,
        SYSRQ = 116,
        RMENU = 117,
        PAUSE = 118,
        HOME = 119,
        UP = 120,
        PRIOR = 121,
        LEFT = 122,
        RIGHT = 123,
        END = 124,
        DOWN = 125,
        NEXT = 126,
        INSERT = 127,
        DELETE = 128,
        LWIN = 129,
        RWIN = 130,
        APPS = 131,
        Power = 132,
        SLEEP = 133,
        WAKE = 134,
        WEBSEARCH = 135,
        WEBFAVORITES = 136,
        WEBREFRESH = 137,
        WEBSTOP = 138,
        WEBFORWARD = 139,
        WEBBACK = 140,
        MYCOMPUTER = 141,
        MAIL = 142,
        MEDIASELECT = 143,
        Mouse1 = 144,
        Mouse2 = 145,
        Mouse3 = 146,
        Mouse4 = 147,
        Mouse5 = 148,
        MWheelUp = 149,
        MWheelDown = 150,
        OEM8 = 155,
    }

    public enum WeaponCombinationType // 2.5.0.44030 @27714488 index:5
    {
        None = -1,
        Handtohand = 0,
        _1HSwing = 1,
        _1HThrust = 2,
        _1HFist = 15,
        _2HSwing = 3,
        _2HAxeMace = 16,
        _2HFlail = 25,
        _2HThrust = 4,
        Staff = 5,
        Bow = 6,
        Crossbow = 7,
        Wand = 8,
        DualWield = 9,
        DualWieldSF = 13,
        DualWieldFF = 14,
        HtHwOrb = 10,
        _1HSwOrb = 11,
        _1HTwOrb = 12,
        WandwOrb = 18,
        HandXBow = 17,
        _1HSwingwShield = 19,
        _1HThrustwShield = 20,
        HthwShield = 21,
        _2HSwingwShield = 22,
        _2HThrustwShield = 23,
        _2HFlailwShield = 26,
        StaffwShield = 24,
        OnHorse = 27,
    }

    public enum ElementType // 2.5.0.44030 @27715608 index:13
    {
        None = -1,
        Physical = 0,
        Fire = 1,
        Lightning = 2,
        Cold = 3,
        Poison = 4,
        Arcane = 5,
        Holy = 6,
    }

    public enum VisualLayer  // 2.5.0.44030 @27716512 index:26
{
        Other = 5,
        Background = 0,
        UI = 1,
        PostFX = 2,
        Actors = 3,
        Particles = 4,
    }

    //public enum ResourceType // 2.5.0.44030 @27716568 index:27
    //{
    //    Mana = 0,
    //    Arcanum = 1,
    //    Fury = 2,
    //    Spirit = 3,
    //    Power = 4,
    //    Hatred = 5,
    //    Discipline = 6,
    //    Faith = 7,
    //}

    public enum DensityType // 2.5.0.44030 @27716640 index:28
{
        Countper100sqft = 0,
        Exactly = 1,
        XPDensityWeight = 2,
    }

    public enum SoundEffectType // 2.5.0.44030 @27716672 index:29
{
        Chorus = 0,
        Compressor = 1,
        Distortion = 2,
        Echo = 3,
        Flange = 4,
        HighPassFilter = 5,
        LowPassFilter = 6,
        LowPassFilterSimple = 7,
        ParametricEQ = 8,
        SFXReverb = 9,
    }

    public enum EventResult // 2.5.0.44030 @27717816 index:42
{
        MonsterDied = 0,
        PlayerDied = 1,
        ActorDied = 2,
        TimedEventExpired = 3,
        ItemUsed = 4,
        GameFlagSet = 5,
        PlayerFlagSet = 6,
        EventReceived = 7,
    }

    public enum ErrorResultType // 2.5.0.44030 @27718312 index:49
{
        PowerUnusableInArea = 0,
        ItemNotReady = 1,
        PowerInvalidTarget = 2,
        PowerNotEnoughHealth = 3,
        PowerNotEnoughCharges = 4,
        PowerNotEnoughMana = 5,
        PowerNotEnoughArcanum = 6,
        PowerNotEnoughFury = 7,
        PowerNotEnoughSpirit = 8,
        PowerNotEnoughHatred = 9,
        PowerNotEnoughDiscipline = 10,
        PowerNotEnoughFaith = 11,
        PowerWrongWeapon = 12,
        PowerInTown = 13,
        PowerTargetTooFar = 14,
        PowerInCooldown = 15,
        PowerNeedsLineOfSight = 16,
        PowerUnusableInCombat = 17,
        PowerUnusableGeneric = 18,
        PowerUnusableWhileMoving = 19,
        PowerMissingItem = 20,
        PowerUnusableDuringBossEncounter = 21,
        IARNotAllowedHere = 22,
        IARCantTradeQuestItems = 23,
        IARCantEquipUnidentifiedItem = 24,
        IARPlayerDoesntMeetItemRequirements = 25,
        IARPlayerDoesntMeetItemLevel = 26,
        IARPlayerIsWrongClassForItem = 27,
        IARTemplarCannotUseWeapon = 28,
        IARScoundrelCannotUseWeapon = 29,
        IAREnchantressCannotUseWeapon = 30,
        IARTemplarCannotEquipSpecialItem = 31,
        IARScoundrelCannotEquipSpecialItem = 32,
        IAREnchantressCannotEquipSpecialItem = 33,
        IARHirelingCantUseItem = 34,
        IARHirelingDoesntMeetItemLevel = 35,
        IARItemTypeDoesntMatchSlot = 36,
        IARNotEnoughRoom = 37,
        IARContainerRemoveOnly = 38,
        IARNoFreeSockets = 39,
        IARItemIsSeasonal = 40,
        IARItemIsNonSeasonal = 41,
        IARItemRequiresSkill = 42,
        IARItemRequiresXBow = 43,
        ItemCraftingNoCombineDesign = 44,
        ItemCraftingMissingReagents = 45,
        IARCannotSalvage = 46,
        IARCannotSalvageGemmedItems = 47,
        IARCannotSalvageVendorItems = 48,
        SkillsSkillbarGeneric = 49,
        SkillsNotAssignableToThatSlot = 50,
        SkillsCantChangeWhileInCooldown = 51,
        SkillsCantChangeWhileInCombat = 52,
        SkillsCantChangeWhileInBossEncounter = 53,
        SkillsCantChangeAtThisTime = 54,
        BuffHigherLevelActive = 55,
        AlreadyIdentified = 56,
        CannotSocket = 57,
        AlreadySocketed = 58,
        CannotUnSocket = 59,
        CannotSocketWithThisItem = 60,
        CannotAcceptTrade = 61,
        CannotTrade = 62,
        CannotTradeBecauseOfGems = 63,
        ItemUseUnknown = 64,
        ItemUseNotOnUnidentifiedItems = 65,
        ItemUseAlreadyAtMaxHealth = 66,
        ItemUseRecipeAlreadyKnown = 67,
        ItemUseOnlyAtBlacksmith = 68,
        ItemUseOnlyAtJeweler = 69,
        ItemUseOnlyAtMystic = 70,
        ItemUseDoesntMeetArtisanLevel = 71,
        ItemUseNotEnoughPagesToConvert = 72,
        CannotDoWhileDead = 73,
        SomeoneDeclined = 74,
        PickupFailed = 75,
        PickupNoSuitableSlot = 76,
        PickupOnlyOneAllowed = 77,
        PickupBelongsToOther = 78,
        InsufficientFunds = 79,
        InsufficientFundsSpecialCurrency = 80,
        InsufficientFundsPlatinum = 81,
        InsufficientParagonPoints = 82,
        ParagonCategoryMaxed = 83,
        ItemCannotBeRepaired = 84,
        ItemCannotBeEnchanted = 85,
        ItemCannotBeEnchantedByType = 86,
        ItemCannotBeEnchantedBelowItemLevel = 87,
        ItemCannotBeEnchantedAboveItemLevel = 88,
        ItemCannotBeEnchantedUnidentified = 89,
        ItemCannotBeEnchantedSocketable = 90,
        ItemCannotBeEnchantedInvalidQuality = 91,
        ItemCannotBeEnchantedFilledSockets = 92,
        ItemCannotBeEnchantedCrafted = 93,
        ItemCannotBeEnchantedNoRandomAffixes = 94,
        ItemCannotBeEnchantedLegacy = 95,
        ItemCannotBeEnchantedMysticLevelRequired = 96,
        ItemCannotBeImproved = 97,
        ItemCannotBeDyed = 98,
        ItemCannotBeReforged = 99,
        ItemCannotBeReforgedUnidentified = 100,
        ItemCannotBeReforgedNotLegendary = 101,
        ItemCannotBeReforgedCannotEquip = 102,
        ItemCannotBeReforgedHasGems = 103,
        ItemCannotBeUsedForTransmog = 104,
        ItemCannotBeUsedForTransmogUnidentified = 105,
        ItemTransmogAlreadyUnlocked = 106,
        TradeOtherBusy = 107,
        TradeSplitStacks = 108,
        TradeDisabled = 109,
        TradeOfGoldDisabled = 110,
        NoOtherWaypoints = 111,
        BnetPartyLeaderCancelLeaveGame = 112,
        BnetPartyInviteRemovedLeaveGameCanceled = 113,
        BnetMultiplayerGameNotPaused = 114,
        BnetNoSeasonJourneyAvailableInNoSeasonalGame = 115,
        CannotDoDuringBossEncounter = 116,
        CannotDoDuringQueuedBossEncounter = 117,
        BannerPlayerTooClose = 118,
        BannerPlayerIsInBossEncounter = 119,
        BannerPlayerIsDead = 120,
        PlayerIsDueling = 121,
        CannotKickNow = 122,
        ItemSoldNonError = 123,
        ItemCannotBeSold = 124,
        ItemIsUniqueEquipped = 125,
        CannotDoThatRightNow = 126,
        ItemSocketTypeMismatch = 127,
        ItemSocketUniqueEquipped = 128,
        EquipItemSocketUniqueEquipped = 129,
        IdentifyAllNoItems = 130,
        ItemCannotBeDropped = 131,
        WaypointTooClose = 132,
        NephalemRiftWarningPartyInside = 133,
        BannerPlayerIsInTieredLootRun = 135,
        NephalemRiftWarningPlayerNeedsKey = 136,
        NephalemRiftWarningPlayerNeedsTieredKey = 137,
        NephalemRiftWarningPlayerDeclined = 138,
        NephalemRiftWarningTieredRiftAlreadyStarted = 139,
        NephalemRiftWarningTieredRiftNotMaxLevel = 140,
        NephalemRiftWarningTieredRiftGameNotMaxLevel = 141,
        NephalemRiftWarningCantDoThatDuringTieredRift = 142,
        NephalemRiftWarningCantChangeSkillsOrEquipmentOutsideGame = 143,
        NephalemRiftWarningTieredRiftCantBeInSetDungeon = 144,
        DifficultyTooLow = 145,
        ItemsCannotBeMovedAtThisTime = 146,
        PlayerNotAllowedInWorld = 147,
        TransmuteCannotUseItemWithFilledSockets = 148,
        TransmuteCannotUseUnidentifiedItem = 149,
        TransmuteMissingCoreIngredients = 150,
        TransmuteCannotUseThisItem = 151,
        TransmuteFailed = 152,
        TransmuteFailedLegendaryPowerAlreadyExtracted = 153,
        TransmuteFailedLegendaryPowerUnavailable = 154,
        TransmuteFailedCannotExtractFromThisItem = 155,
        TransmuteFailedCannotUseCraftedItem = 156,
        TransmuteFailedCannotEquipItem = 157,
        TransmuteFailedCannotFindItem = 158,
        TransmuteFailedItemLevelTooLow = 159,
        TransmuteFailedAlreadyRemovedLevelReq = 160,
        TransmuteFailedNotEnoughItemsInSet = 161,
        EmpoweredRiftCannotAfford = 162,
        BannerPlayerIsInSetDungeon = 163,
        TransmuteFailedLowJewelRank = 164,
        SetDungeonCancelled = 165,
        SetDungeonCannotEnterNotWorthy = 166,
        SetDungeonCannotEnterClosing = 167,
        EquipmentManagerCannotChangeLoadout = 168,
        EquipmentManagerCannotChangeLoadoutIsInGRift = 169,
        EquipmentManagerCannotChangeLoadoutSkillOnCooldown = 170,
        EquipmentManagerCannotChangeLoadoutCurrentlyEquipped = 171,
    }

    public enum DisplayType // 2.5.0.44030 @27720256 index:58
    {
        Normal = 0,
        Light = 1,
        LCD = 2,
        LCDVertical = 3,
    }

    ////public enum  FollowerType // 2.5.0.44030 @27720296 index:59
    ////{
    ////    None = 0,
    ////    Templar = 1,
    ////    Scoundrel = 2,
    ////    Enchantress = 3,
    ////}

    //public enum ActorClass // 2.5.0.44030 @27720336 index:60
    //{
    //    None = -1,
    //    DemonHunter = 0,
    //    Barbarian = 1,
    //    Wizard = 2,
    //    Witchdoctor = 3,
    //    Monk = 4,
    //    Crusader = 5,
    //}

    public enum VisibilityType // 2.5.0.44030 @27720400 index:61
    {
        Always = 0,
        LabelAlreadySet = 2,
        GameIsOpenWorld = 3,
    }

    //public enum ActorType // 2.5.0.44030 @27720432 index:62
    //{
    //    Invalid = 0,
    //    Monster = 1,
    //    Gizmo = 2,
    //    ClientEffect = 3,
    //    ServerProp = 4,
    //    Environment = 5,
    //    Critter = 6,
    //    Player = 7,
    //    Item = 8,
    //    AxeSymbol = 9,
    //    Projectile = 10,
    //    CustomBrain = 11,
    //}

    //public enum NodeFlags // 2.5.0.44030 @27720616 index:64
    //{
    //    ALLOWWALK = 0,
    //    ALLOWFLIER = 1,
    //    ALLOWSPIDER = 2,
    //    NOSPAWN = 6,
    //    ALLOWPROJECTILE = 10,
    //    ALLOWGHOST = 11,
    //    LEVELAREABIT0 = 3,
    //    LEVELAREABIT1 = 4,
    //    SUBSCENE = 5,
    //    SPECIAL0 = 7,
    //    SPECIAL1 = 8,
    //    RoundedCorner0 = 12,
    //    RoundedCorner1 = 13,
    //    RoundedCorner2 = 14,
    //    RoundedCorner3 = 15,
    //}

    public enum ActorState // 2.5.0.44030 @27720784 index:66
    {
        Idle = 0,
        Job = 1,
        Wandering = 2,
        Attacking = 3,
        Circling = 4,
        Charging = 5,
        Fleeing = 6,
        Feared = 7,
        Dead = 8,
        Walking = 9,
        Walking3State = 10,
        Animating = 11,
        SpecialPower = 12,
        Frozen = 13,
        Blinded = 14,
        Stunned = 15,
        Taunted = 16,
    }

//    public enum MonsterQuality  // 2.5.0.44030 @27721080 index:70
//{
//        Normal = 0,
//        Champion = 1,
//        Rare = 2,
//        Minion = 3,
//        Unique = 4,
//        Hireling = 5,
//        Boss = 7,
//    }

    public enum DyeColor2 // 2.5.0.44030 @27722344 index:76
{
        None = 0,
        DyeVanishing = 1,
        Dye20 = 2,
        Dye18 = 3,
        Dye19 = 4,
        Dye10 = 5,
        Dye17 = 6,
        Dye13 = 7,
        Dye11 = 8,
        Dye16 = 9,
        Dye09 = 10,
        Dye15 = 11,
        Dye04 = 12,
        Dye03 = 13,
        DyeCE02 = 14,
        Dye14 = 15,
        Dye05 = 16,
        Dye12 = 17,
        Dye06 = 18,
        Dye01 = 19,
        Dye08 = 20,
        DyeCE01 = 21,
        Dye21 = 22,
    }

    //public enum ItemType // 2.5.0.44030 @27722536 index:77
    //{
    //    None = -1,
    //    Weapon = 0,
    //    Melee = 1,
    //    Swing = 2,
    //    Axe = 3,
    //    Sword = 4,
    //    Mace = 5,
    //    Thrust = 6,
    //    Dagger = 7,
    //    Polearm = 8,
    //    Onehand = 9,
    //    Twohand = 10,
    //    Ranged = 11,
    //    BowClass = 12,
    //    Bow = 13,
    //    Crossbow = 14,
    //    Wand = 16,
    //    Staff = 17,
    //    Thrown = 22,
    //    ThrowingAxe = 23,
    //    Spear = 24,
    //    Armor = 25,
    //    Shield = 26,
    //    Helm = 27,
    //    Gloves = 28,
    //    Boots = 29,
    //    ChestArmor = 30,
    //    Belt = 31,
    //    Jewelry = 32,
    //    Ring = 33,
    //    Amulet = 34,
    //    Socketable = 36,
    //    Gem = 37,
    //    Utility = 38,
    //    GeneralUtility = 39,
    //    Potion = 40,
    //    HealthPotion = 41,
    //    Quest = 44,
    //    QuestUsable = 45,
    //    Gold = 46,
    //    Quiver = 47,
    //    Shoulders = 48,
    //    Glyph = 49,
    //    Legs = 50,
    //    Offhand = 51,
    //    Orb = 52,
    //    Scroll = 53,
    //    TownPortalStone = 54,
    //    IdentifyScroll = 56,
    //    Junk = 58,
    //    Book = 73,
    //    Ornament = 76,
    //    FistWeapon = 77,
    //    CombatStaff = 78,
    //    Bracers = 79,
    //    CraftingReagent = 80,
    //    Dye = 81,
    //    Calldown = 82,
    //    HealthGlyph = 83,
    //    ShrineGlyph = 117,
    //    KnowledgeUtility = 84,
    //    HandXbow = 86,
    //    Tome = 87,
    //    FollowerSpecial = 88,
    //    TemplarSpecial = 89,
    //    ScoundrelSpecial = 90,
    //    EnchantressSpecial = 91,
    //    Mojo = 92,
    //    MightyWeapon = 93,
    //    CeremonialDagger = 94,
    //    WizardHat = 95,
    //    VoodooMask = 96,
    //    Cloak = 97,
    //    BarbBelt = 98,
    //    SpiritStone = 99,
    //    GenericSwingWeapon = 100,
    //    GenericThrustWeapon = 101,
    //    GenericBowWeapon = 102,
    //    GenericRangedWeapon = 103,
    //    GenericHelm = 104,
    //    GenericChestArmor = 105,
    //    GenericBelt = 106,
    //    GenericOffHand = 107,
    //    UseOnPickup = 113,
    //    CraftingPlan = 114,
    //    CraftingPlanLegendary = 115,
    //    CraftingPlanGeneric = 116,
    //    Flail = 118,
    //    CrusaderShield = 119,
    //    Jewel = 120,
    //    Collectible = 121,
    //    CollectibleDevilsHand = 122,
    //    Shard = 123,
    //    GreaterShard = 124,
    //    UpgradeableJewel = 125,
    //    BloodShard = 126,
    //    CosmeticWings = 127,
    //    CosmeticPet = 128,
    //    CosmeticPennant = 129,
    //    CosmeticPortraitFrame = 130,
    //    Platinum = 131,
    //    HoradricReagent = 132,
    //    TieredRiftKey = 75,
    //}

    public enum PlayerAttribute // 2.5.0.44030 @27723368 index:78
    {
        None = -1,
        Strength = 0,
        Dexterity = 1,
        Intelligence = 2,
    }

    //public enum AnimationState // 2.5.0.44030 @27724288 index:84
    //{
    //    NotAnimating = 0,
    //    Idle = 1,
    //    Walking = 2,
    //    Power = 3,
    //    Spawn = 4,
    //    Gizmo = 5,
    //    Stance = 6,
    //    InterruptableAnimation = 7,
    //    PreplayPower = 8,
    //    Cutscene = 9,
    //    PreplayChanneled = 10,
    //    Death = 11,
    //    FloatConversation = 12,
    //    InterruptableAnimationNoLookswitch = 13,
    //}

    public enum LightingBlend // 2.5.0.44030 @27724408 index:85
{
        None = 0,
        AO = 1,
        AOShadowMap = 2,
        Graybox = 3,
        Grayscale = 4,
        Mipmaps = 5,
        LightmapReplacewithWhite = 7,
        MipmapsLightmaps = 6,
        Envmap = 8,
        Gloss = 9,
        LightingDiffuseOnly = 10,
        LightingSpecularOnly = 11,
        LightingIrradianceOnly = 12,
        Overdraw = 13,
        OverdrawPMA = 14,
        ActorBatches = 15,
        FlatShaded = 16,
        Wireframe = 17,
        Retro = 18,
    }

    public enum ProjectileEffect // 2.5.0.44030 @27724808 index:89
{
        Action = 0,
        GetHit = 1,
        None = 16,
    }

    public enum ValueDataType // 2.5.0.44030 @27724840 index:90
{
        Int = 0,
        Float = 1,
        SNO = 2,
        GBID = 3,
        Formula = 4,
        ActorGroup = 5,
        ActorAttribute = 6,
        StartLocationName = 7,
    }
}