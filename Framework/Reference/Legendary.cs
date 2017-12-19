using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Reference
{
    public class Legendary : FieldCollection<Legendary, Item>
    {
        // Load static version of sets class
        public class Sets : Reference.Sets
        {
        }

        #region Manually Added Items

        /// <summary>
        /// Hellcat Waistguard 
        /// </summary>
        public static Item HellcatWaistguard = new Item
        {
            Id = 193668,
            Name = "女魔护腰",
            Quality = ItemQuality.Legendary,
            Slug = "hellcat-waistguard",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/hellcat-waistguard",
            Url = "https://us.battle.net/d3/en/item/hellcat-waistguard",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_belt_005_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/hellcat-waistguard",
            IsCrafted = false,
            LegendaryAffix = "手雷有一定几率弹跳 (3-5) 次，每次弹跳造成额外的 50% 伤害。该加成效果最后一次弹跳提高至 800%。",
            SetName = "",
        };

        /// <summary>
        /// Talisman of Akkhan
        /// </summary>
        public static Item TalismanOfAkkhan = new Item
        {
            Id = 455735,
            Name = "阿克汉的护符",
            Quality = ItemQuality.Legendary,
            Slug = "talisman-of-akkhan",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/talisman-of-akkhan",
            Url = "https://us.battle.net/d3/en/item/talisman-of-akkhan",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/p43_akkhanset_amulet_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/talisman-of-akkhan",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "阿克汉的战甲",
        };


        #endregion

        #region Special Crafted Items

        public static Item ReapersWraps = new Item
        {
            Id = 298118,
            Name = "夺魂者裹腕",
            Quality = ItemQuality.Legendary,
            Slug = "reapers-wraps",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/recipe/reapers-wraps",
            Url = "https://us.battle.net/d3/en/artisan/blacksmith/recipe/reapers-wraps",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_bracer_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/artisan/blacksmith/recipe/reapers-wraps",
            IsCrafted = true,
            LegendaryAffix = "生命球恢复你主要能量的 (25-30)%。",
            SetName = "",
        };

        public static Item PiroMarella = new Item
        {
            Id = 299411,
            Name = "皮诺·马雷拉",
            Quality = ItemQuality.Legendary,
            Slug = "piro-marella",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "crushield_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/recipe/piro-marella",
            Url = "https://us.battle.net/d3/en/artisan/blacksmith/recipe/piro-marella",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_crushield_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/artisan/blacksmith/recipe/piro-marella",
            IsCrafted = true,
            LegendaryAffix = "使盾牌猛击消耗的愤怒值降低 (40-50)%。",
            SetName = "",
        };

        #endregion

        #region Special Follower Items

        /// <summary>
        /// Ribald Etchings Equip on Follower: Gain access to all skills.
        /// </summary>
        public static Item RibaldEtchings = new Item
        {
            Id = 366971,
            Name = "粗俗的版画",
            Quality = ItemQuality.Legendary,
            Slug = "ribald-etchings",
            ItemType = ItemType.FollowerSpecial,
            TrinityItemType = TrinityItemType.FollowerScoundrel,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ribald-etchings",
            Url = "http://us.battle.net/d3/en/item/ribald-etchings",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/x1_followeritem_scoundrel_legendary_02_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "装备在追随者身上：可使用所有技能。",
            SetName = "",
        };

        /// <summary>
        /// Skeleton Key Equip on Follower: Your follower cannot die.
        /// </summary>
        public static Item SkeletonKey = new Item
        {
            Id = 366970,
            Name = "骷髅钥匙",
            Quality = ItemQuality.Legendary,
            Slug = "skeleton-key",
            ItemType = ItemType.FollowerSpecial,
            TrinityItemType = TrinityItemType.FollowerScoundrel,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/skeleton-key",
            Url = "http://us.battle.net/d3/en/item/skeleton-key",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/x1_followeritem_scoundrel_legendary_01_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "装备在追随者身上：你的追随者不会死。",
            SetName = "",
        };

        /// <summary>
        /// Hand of the Prophet Equip on Follower: Gain access to all skills.
        /// </summary>
        public static Item HandOfTheProphet = new Item
        {
            //[TrinityPlugin 2.13.59] Hand Of The Prophet ActorSnoId=366980 GameBalanceId=-450681607
            Id = -450681607,
            GameBalanceId = true,
            Name = "先知之手",
            Quality = ItemQuality.Legendary,
            Slug = "hand-of-the-prophet",
            ItemType = ItemType.FollowerSpecial,
            TrinityItemType = TrinityItemType.FollowerEnchantress,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/hand-of-the-prophet",
            Url = "http://us.battle.net/d3/en/item/hand-of-the-prophet",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/x1_followeritem_enchantress_legendary_02_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "装备在追随者身上：可使用所有技能。",
            SetName = "",
        };

        /// <summary>
        /// Slipka's Letter Opener Equip on Follower: Reduce the cooldown of all Follower skills by 50%.
        /// </summary>
        public static Item SlipkasLetterOpener = new Item
        {
            Id = 978821514, // ActorSnoId = 366971 // Same as RibaldEtchings
            GameBalanceId = true,
            Name = "斯里普卡的拆信刀",
            Quality = ItemQuality.Legendary,
            Slug = "slipkas-letter-opener",
            ItemType = ItemType.FollowerSpecial,
            TrinityItemType = TrinityItemType.FollowerScoundrel,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/slipkas-letter-opener",
            Url = "http://us.battle.net/d3/en/item/slipkas-letter-opener",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/x1_followeritem_scoundrel_legendary_03_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "装备在追随者身上：使所有追随者技能的冷却时间缩短50%。",
            SetName = "",
        };

        /// <summary>
        /// Vadim's Surge Equip on Follower: Reduce the cooldown of all Follower skills by 50%.
        /// </summary>
        public static Item VadimsSurge = new Item
        {
            //[TrinityPlugin 2.13.59] Vadim's Surge ActorSnoId=366980 GameBalanceId=-450681606
            Id = -450681606,
            GameBalanceId = true,
            Name = "瓦迪姆的电涌器",
            Quality = ItemQuality.Legendary,
            Slug = "vadims-surge",
            ItemType = ItemType.FollowerSpecial,
            TrinityItemType = TrinityItemType.FollowerEnchantress,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/vadims-surge",
            Url = "http://us.battle.net/d3/en/item/vadims-surge",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/x1_followeritem_enchantress_legendary_03_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "装备在追随者身上：使所有追随者技能的冷却时间缩短50%。",
            SetName = "",
        };

        /// <summary>
        /// Smoking Thurible Equip on Follower: Your follower cannot die.
        /// </summary>
        public static Item SmokingThurible = new Item
        {
            Id = 366979,
            Name = "烟熏香炉",
            Quality = ItemQuality.Legendary,
            Slug = "smoking-thurible",
            ItemType = ItemType.FollowerSpecial,
            TrinityItemType = TrinityItemType.FollowerEnchantress,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/smoking-thurible",
            Url = "http://us.battle.net/d3/en/item/smoking-thurible",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/x1_followeritem_enchantress_legendary_01_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "装备在追随者身上：你的追随者不会死。",
            SetName = "",
        };

        /// <summary>
        /// Relic of Akarat Equip on Follower: Gain access to all skills.
        /// </summary>
        public static Item RelicOfAkarat = new Item
        {
            Id = -765770608, // ActorSnoId = 366969 // Same as HillenbrandsTrainingSword
            GameBalanceId = true,
            Name = "阿卡拉特的遗物",
            Quality = ItemQuality.Legendary,
            Slug = "relic-of-akarat",
            ItemType = ItemType.FollowerSpecial,
            TrinityItemType = TrinityItemType.FollowerTemplar,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/relic-of-akarat",
            Url = "http://us.battle.net/d3/en/item/relic-of-akarat",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/x1_followeritem_templar_legendary_02_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "装备在追随者身上：可使用所有技能。",
            SetName = "",
        };

        /// <summary>
        /// Enchanting Favor Equip on Follower: Your follower cannot die.
        /// </summary>
        public static Item EnchantingFavor = new Item
        {
            Id = 366968,
            Name = "情缘",
            Quality = ItemQuality.Legendary,
            Slug = "enchanting-favor",
            ItemType = ItemType.FollowerSpecial,
            TrinityItemType = TrinityItemType.FollowerTemplar,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/enchanting-favor",
            Url = "http://us.battle.net/d3/en/item/enchanting-favor",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/x1_followeritem_templar_legendary_01_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "装备在追随者身上：你的追随者不会死。",
            SetName = "",
        };

        /// <summary>
        /// Hillenbrand's Training Sword Equip on Follower: Reduce the cooldown of all Follower skills by 50%.
        /// </summary>
        public static Item HillenbrandsTrainingSword = new Item
        {
            Id = 366969,
            Name = "希伦布兰的训练剑",
            Quality = ItemQuality.Legendary,
            Slug = "hillenbrands-training-sword",
            ItemType = ItemType.FollowerSpecial,
            TrinityItemType = TrinityItemType.FollowerTemplar,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/hillenbrands-training-sword",
            Url = "http://us.battle.net/d3/en/item/hillenbrands-training-sword",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/x1_followeritem_templar_legendary_03_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "装备在追随者身上：使所有追随者技能的冷却时间缩短50%。",
            SetName = "",
        };

        ///// <summary>
        ///// Deadly Rebirth
        ///// </summary>
        //public static Item DeadlyRebirth = new Item
        //{
        //    Id = 193433,
        //    Name = "还魂",
        //    Quality = ItemQuality.Legendary,
        //    Slug = "deadly-rebirth-1WLDQt",
        //    ItemType = ItemType.CeremonialDagger,
        //    TrinityItemType = TrinityItemType.CeremonialKnife,
        //    IsTwoHanded = false,
        //    BaseType = ItemBaseType.Weapon,
        //    InternalName = "",
        //    DataUrl = "",
        //    Url = "",
        //    IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_ceremonialdagger_003_x1_demonhunter_male.png",
        //    RelativeUrl = "",
        //    IsCrafted = false,
        //    LegendaryAffix = "亡者之握获得天降尸雨符文的效果。",
        //    SetName = "",
        //};

        #endregion

        #region Special Quest Items

        /// <summary>
        /// The Horadric Hamburger 
        /// </summary>
        public static Item TheHoradricHamburger = new Item
        {
            Id = 200476,
            Name = "赫拉迪姆汉堡",
            Quality = ItemQuality.Legendary,
            Slug = "the-horadric-hamburger-2HDjXe",
            ItemType = ItemType.Dagger,
            TrinityItemType = TrinityItemType.Dagger,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-horadric-hamburger-2HDjXe",
            Url = "http://us.battle.net/d3/en/item/the-horadric-hamburger-2HDjXe",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_offhand_001_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Spectrum 
        /// </summary>
        public static Item Spectrum = new Item
        {
            Id = 200558,
            Name = "虹光",
            Quality = ItemQuality.Legendary,
            Slug = "spectrum-Fa7AX",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/spectrum-Fa7AX",
            Url = "http://us.battle.net/d3/en/item/spectrum-Fa7AX",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_sword_1h_021_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "幻化物品，野外哥布林开启的彩虹谷才会掉落。",
            SetName = "",
        };

        #endregion

        #region Special Potions


        // AUTO-GENERATED on Wed, 18 May 2016 21:40:07 GMT

        /// <summary>
        /// Bottomless Potion of Chaos Teleport to another location based on your missing health. Results may vary.
        /// </summary>
        public static Item BottomlessPotionOfChaos = new Item
        {
            Id = 451311, //ActorSnoId=451311 GameBalanceId=-2018707766 
            Name = "混乱之无尽药水",
            Quality = ItemQuality.Legendary,
            Slug = "bottomless-potion-of-chaos",
            ItemType = ItemType.Potion,
            TrinityItemType = TrinityItemType.HealthPotion,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Misc,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bottomless-potion-of-chaos",
            Url = "http://us.battle.net/d3/en/item/bottomless-potion-of-chaos",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/healthpotionlegendary_10_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "根据你损失的生命值闪现到别的位置。结果无法预料。",
            SetName = "",
        };

        /// <summary>
        /// Bottomless Potion of Fear Fears enemies within 12 yards for 3–4 seconds.
        /// </summary>
        public static Item BottomlessPotionOfFear = new Item
        {
            Id = 428805,
            Name = "无尽恐惧药水",
            Quality = ItemQuality.Legendary,
            Slug = "bottomless-potion-of-fear",
            ItemType = ItemType.Potion,
            TrinityItemType = TrinityItemType.HealthPotion,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Misc,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bottomless-potion-of-fear",
            Url = "http://us.battle.net/d3/en/item/bottomless-potion-of-fear",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/healthpotionlegendary_08_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "恐惧 12 码内的敌人，持续 (3-4) 秒。",
            SetName = "",
        };

        /// <summary>
        /// Bottomless Potion of Kulle-Aid Drinking Kulle-Aid allows you to burst through walls summoned by Waller elites for 5 seconds.
        /// </summary>
        public static Item BottomlessPotionOfKulleaid = new Item
        {
            Id = 344093,
            Name = "无尽库帮药水",
            Quality = ItemQuality.Legendary,
            Slug = "bottomless-potion-of-kulleaid",
            ItemType = ItemType.Potion,
            TrinityItemType = TrinityItemType.HealthPotion,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Misc,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bottomless-potion-of-kulleaid",
            Url = "http://us.battle.net/d3/en/item/bottomless-potion-of-kulleaid",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/healthpotionlegendary_06_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "饮用“库帮”药水能使你在5秒内穿过筑墙精英怪所召唤的墙壁。",
            SetName = "",
        };

        ///// <summary>
        ///// Health Potion 
        ///// </summary>
        //public static Item HealthPotion = new Item
        //{
        //    Id = 0,
        //    Name = "Health Potion",
        //    Quality = ItemQuality.Normal,
        //    Slug = "health-potion",
        //    ItemType = ItemType.Unknown,
        //    TrinityItemType = TrinityItemType.Unknown,
        //    IsTwoHanded = false,
        //    BaseType = ItemBaseType.None,
        //    InternalName = "",
        //    DataUrl = "https://us.battle.net/api/d3/data/item/health-potion",
        //    Url = "http://us.battle.net/d3/en/item/health-potion",
        //    IconUrl = "http://media.blizzard.com/d3/icons/items/large/healthpotionbottomless_demonhunter_male.png",
        //    RelativeUrl = "",
        //    IsCrafted = false,
        //    LegendaryAffix = "",
        //    SetName = "",
        //};

        /// <summary>
        /// Bottomless Potion of Mutilation Increases Life per Kill by 40000–50000 for 5 seconds.
        /// </summary>
        public static Item BottomlessPotionOfMutilation = new Item
        {
            Id = 342824,
            Name = "无尽残毁药水",
            Quality = ItemQuality.Legendary,
            Slug = "bottomless-potion-of-mutilation",
            ItemType = ItemType.Potion,
            TrinityItemType = TrinityItemType.HealthPotion,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Misc,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bottomless-potion-of-mutilation",
            Url = "http://us.battle.net/d3/en/item/bottomless-potion-of-mutilation",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/healthpotionlegendary_05_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "消灭回复生命提高 (40000-50000) 点，持续 5 秒。",
            SetName = "",
        };

        /// <summary>
        /// Bottomless Potion of Amplification Increases healing from all sources by 20–25% for 5 seconds.
        /// </summary>
        public static Item BottomlessPotionOfAmplification = new Item
        {
            Id = 434627,
            Name = "无尽增幅药水",
            Quality = ItemQuality.Legendary,
            Slug = "bottomless-potion-of-amplification",
            ItemType = ItemType.Potion,
            TrinityItemType = TrinityItemType.HealthPotion,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Misc,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bottomless-potion-of-amplification",
            Url = "http://us.battle.net/d3/en/item/bottomless-potion-of-amplification",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/healthpotionlegendary_09_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "使受到的所有类型的治疗效果提高 (20-25)%，持续 5 秒。",
            SetName = "",
        };

        /// <summary>
        /// Bottomless Potion of Regeneration Restores an additional 75000–100000 Life over 5 seconds.
        /// </summary>
        public static Item BottomlessPotionOfRegeneration = new Item
        {
            Id = 341343,
            Name = "无尽恢复药水",
            Quality = ItemQuality.Legendary,
            Slug = "bottomless-potion-of-regeneration",
            ItemType = ItemType.Potion,
            TrinityItemType = TrinityItemType.HealthPotion,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Misc,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bottomless-potion-of-regeneration",
            Url = "http://us.battle.net/d3/en/item/bottomless-potion-of-regeneration",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/healthpotionlegendary_03_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "在 5 秒内恢复额外 (75000-100000) 点生命值。",
            SetName = "",
        };

        /// <summary>
        /// Bottomless Potion of Rejuvenation Restores 20–30% resource when used below 50% health.
        /// </summary>
        public static Item BottomlessPotionOfRejuvenation = new Item
        {
            Id = 433027,
            Name = "无尽恢复药水",
            Quality = ItemQuality.Legendary,
            Slug = "bottomless-potion-of-rejuvenation",
            ItemType = ItemType.Potion,
            TrinityItemType = TrinityItemType.HealthPotion,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Misc,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bottomless-potion-of-rejuvenation",
            Url = "http://us.battle.net/d3/en/item/bottomless-potion-of-rejuvenation",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/p2_healthpotionlegendary_07_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "当生命值低于50%时使用，恢复(20-30)%能量。",
            SetName = "",
        };

        /// <summary>
        /// Bottomless Potion of the Leech Increases Life per Hit by 15000–20000 for 5 seconds.
        /// </summary>
        public static Item BottomlessPotionOfTheLeech = new Item
        {
            Id = 342823,
            Name = "无尽吸血药水",
            Quality = ItemQuality.Legendary,
            Slug = "bottomless-potion-of-the-leech",
            ItemType = ItemType.Potion,
            TrinityItemType = TrinityItemType.HealthPotion,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Misc,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bottomless-potion-of-the-leech",
            Url = "http://us.battle.net/d3/en/item/bottomless-potion-of-the-leech",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/healthpotionlegendary_04_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "击中回复生命提高 (15000-20000) 点，持续 5 秒。",
            SetName = "",
        };

        /// <summary>
        /// Bottomless Potion of the Diamond Increases Resist All by 50–100 for 5 seconds.
        /// </summary>
        public static Item BottomlessPotionOfTheDiamond = new Item
        {
            Id = 341342,
            Name = "无尽钻石药水",
            Quality = ItemQuality.Legendary,
            Slug = "bottomless-potion-of-the-diamond",
            ItemType = ItemType.Potion,
            TrinityItemType = TrinityItemType.HealthPotion,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Misc,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bottomless-potion-of-the-diamond",
            Url = "http://us.battle.net/d3/en/item/bottomless-potion-of-the-diamond",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/healthpotionlegendary_02_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "所有抗性提高 (50-100) 点，持续 5 秒。",
            SetName = "",
        };

        /// <summary>
        /// Bottomless Potion of the Tower Increases Armor by 10% for 5 seconds.
        /// </summary>
        public static Item BottomlessPotionOfTheTower = new Item
        {
            Id = 341333,
            Name = "无尽高塔药水",
            Quality = ItemQuality.Legendary,
            Slug = "bottomless-potion-of-the-tower",
            ItemType = ItemType.Potion,
            TrinityItemType = TrinityItemType.HealthPotion,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Misc,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bottomless-potion-of-the-tower",
            Url = "http://us.battle.net/d3/en/item/bottomless-potion-of-the-tower",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/healthpotionlegendary_01_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "使护甲值提高 (10-20)%，持续 5 秒。",
            SetName = "",
        };

        #endregion

        #region Imported Item Data


        // AUTO-GENERATED on Fri, 30 Jun 2017 16:25:13 GMT


        // AUTO-GENERATED on Fri, 30 Jun 2017 16:33:09 GMT


        /// <summary>
        /// Homing Pads Your Town Portal is no longer interrupted by taking damage. While casting Town Portal you gain a protective bubble that reduces damage taken by 50–65 % .
        /// </summary>
        public static Item HomingPads = new Item
        {
            Id = 198573,
            Name = "赋归肩垫",
            Quality = ItemQuality.Legendary,
            Slug = "homing-pads",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "shoulderPads_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/homing-pads",
            Url = "https://us.battle.net/d3/en/item/homing-pads",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/homing-pads",
            IsCrafted = false,
            LegendaryAffix = "受到伤害不会再打断你施放城镇传送门。当施放城镇传送门时，你会获得一个防护气泡，使受到的伤害降低 (50–65)%。",
            SetName = "",
        };

        /// <summary>
        /// Pauldrons of the Skeleton King When receiving fatal damage, there is a chance that you are instead restored to 25% of maximum Life and cause nearby enemies to flee in fear.
        /// </summary>
        public static Item PauldronsOfTheSkeletonKing = new Item
        {
            Id = 298164,
            Name = "骷髅王肩铠",
            Quality = ItemQuality.Legendary,
            Slug = "pauldrons-of-the-skeleton-king",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/pauldrons-of-the-skeleton-king",
            Url = "https://us.battle.net/d3/en/item/pauldrons-of-the-skeleton-king",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pauldrons-of-the-skeleton-king",
            IsCrafted = false,
            LegendaryAffix = "当遭受致命伤害时，有一定几率使你恢复生命值上限25% 的生命值并恐惧附近的敌人。",
            SetName = "",
        };

        /// <summary>
        /// Razeth's Volition Skeletal Mage gains the effect of the Gift of Death rune.
        /// </summary>
        public static Item RazethsVolition = new Item
        {
            Id = 467610,
            Name = "拉泽斯的意志",
            Quality = ItemQuality.Legendary,
            Slug = "razeths-volition",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/razeths-volition",
            Url = "https://us.battle.net/d3/en/item/razeths-volition",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_shoulders_22_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/razeths-volition",
            IsCrafted = false,
            LegendaryAffix = "骷髅法师获得亡魂之赐符文效果。",
            SetName = "",
        };

        /// <summary>
        /// Death Watch Mantle 25–35 % chance to explode in a fan of knives for 750-950% weapon damage when hit.
        /// </summary>
        public static Item DeathWatchMantle = new Item
        {
            Id = 200310,
            Name = "死卫护肩",
            Quality = ItemQuality.Legendary,
            Slug = "death-watch-mantle",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "shoulderPads_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/death-watch-mantle",
            Url = "https://us.battle.net/d3/en/item/death-watch-mantle",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_002_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/death-watch-mantle",
            IsCrafted = false,
            LegendaryAffix = "有 (25-35)% 的几率发生刀扇爆炸，击中时造成 750-950% 的武器伤害。",
            SetName = "",
        };

        /// <summary>
        /// Corpsewhisper Pauldrons Corpse Lance damage is increased by 25–30 % for 3 seconds when you consume a corpse. Max 20 stacks.
        /// </summary>
        public static Item CorpsewhisperPauldrons = new Item
        {
            Id = 467609,
            Name = "拜尸者的护肩",
            Quality = ItemQuality.Legendary,
            Slug = "corpsewhisper-pauldrons",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/corpsewhisper-pauldrons",
            Url = "https://us.battle.net/d3/en/item/corpsewhisper-pauldrons",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_shoulders_21_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/corpsewhisper-pauldrons",
            IsCrafted = false,
            LegendaryAffix = "你消耗一具尸骸的精魂后，尸矛的伤害就会提高(25-30)%,持续3秒。最多叠加20次。",
            SetName = "",
        };

        /// <summary>
        /// Fury of the Ancients Call of the Ancients gains the effect of the Ancients' Fury rune.
        /// </summary>
        public static Item FuryOfTheAncients = new Item
        {
            Id = 426817,
            Name = "先祖的怒火",
            Quality = ItemQuality.Legendary,
            Slug = "fury-of-the-ancients",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/fury-of-the-ancients",
            Url = "https://us.battle.net/d3/en/item/fury-of-the-ancients",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_shoulder_102_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fury-of-the-ancients",
            IsCrafted = false,
            LegendaryAffix = "先祖召唤获得先祖之怒符文的效果。",
            SetName = "",
        };

        /// <summary>
        /// Lefebvre's Soliloquy Cyclone Strike reduces your damage taken by 40–50 % for 5 seconds.
        /// </summary>
        public static Item LefebvresSoliloquy = new Item
        {
            Id = 298158,
            Name = "列斐伏尔的劝诫",
            Quality = ItemQuality.Legendary,
            Slug = "lefebvres-soliloquy",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/lefebvres-soliloquy",
            Url = "https://us.battle.net/d3/en/item/lefebvres-soliloquy",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_shoulder_101_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/lefebvres-soliloquy",
            IsCrafted = false,
            LegendaryAffix = "飓风破使你受到的伤害降低(40-50)%，持续 5 秒。",
            SetName = "",
        };

        /// <summary>
        /// Mantle of Channeling While channeling Whirlwind, Rapid Fire, Strafe, Tempest Rush, Firebats, Arcane Torrent, Disintegrate, or Ray of Frost, you deal 20–25 % increased damage and take 25% reduced damage.
        /// </summary>
        public static Item MantleOfChanneling = new Item
        {
            Id = 429681,
            Name = "导能披肩",
            Quality = ItemQuality.Legendary,
            Slug = "mantle-of-channeling",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/mantle-of-channeling",
            Url = "https://us.battle.net/d3/en/item/mantle-of-channeling",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_shoulder_103_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/mantle-of-channeling",
            IsCrafted = false,
            LegendaryAffix = "当引导旋风斩,急速射击,扫射,风雷冲,火蝠,奥术洪流,瓦解射线或冰霜射线时，你造成的伤害提高(20-25)%，受到的伤害降低25%。",
            SetName = "",
        };

        /// <summary>
        /// Spaulders of Zakara Your items become indestructible.
        /// </summary>
        public static Item SpauldersOfZakara = new Item
        {
            Id = 298163,
            Name = "萨卡拉肩铠",
            Quality = ItemQuality.Legendary,
            Slug = "spaulders-of-zakara",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "shoulderpads_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/spaulders-of-zakara",
            Url = "https://us.battle.net/d3/en/item/spaulders-of-zakara",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/spaulders-of-zakara",
            IsCrafted = false,
            LegendaryAffix = "你的物品变为不可摧毁。",
            SetName = "",
        };

        /// <summary>
        /// Vile Ward Furious Charge deals 30–35 % increased damage for every enemy hit while charging.
        /// </summary>
        public static Item VileWard = new Item
        {
            Id = 201325,
            Name = "辟邪肩甲",
            Quality = ItemQuality.Legendary,
            Slug = "vile-ward",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "shoulderPads_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/vile-ward",
            Url = "https://us.battle.net/d3/en/item/vile-ward",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_003_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vile-ward",
            IsCrafted = false,
            LegendaryAffix = "狂暴冲撞在冲锋时每击中一个敌人，造成的伤害即可提高(30-35)%。",
            SetName = "",
        };

        /// <summary>
        /// Arachyr's Mantle 
        /// </summary>
        public static Item ArachyrsMantle = new Item
        {
            Id = 440420,
            Name = "亚拉基尔的斗篷",
            Quality = ItemQuality.Legendary,
            Slug = "arachyrs-mantle",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/arachyrs-mantle",
            Url = "https://us.battle.net/d3/en/item/arachyrs-mantle",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_02_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/arachyrs-mantle",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "亚拉基尔的灵魂",
        };

        /// <summary>
        /// Burden of the Invoker 
        /// </summary>
        public static Item BurdenOfTheInvoker = new Item
        {
            Id = 335029,
            Name = "唤魔师的重任",
            Quality = ItemQuality.Legendary,
            Slug = "burden-of-the-invoker",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "shoulderpads_norm_set_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/burden-of-the-invoker",
            Url = "https://us.battle.net/d3/en/item/burden-of-the-invoker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_12_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/burden-of-the-invoker",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "唤魔师的荆棘",
        };

        /// <summary>
        /// Dashing Pauldrons of Despair 
        /// </summary>
        public static Item DashingPauldronsOfDespair = new Item
        {
            Id = 414922,
            Name = "绝望肩胄",
            Quality = ItemQuality.Legendary,
            Slug = "dashing-pauldrons-of-despair",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/dashing-pauldrons-of-despair",
            Url = "https://us.battle.net/d3/en/item/dashing-pauldrons-of-despair",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_02_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/dashing-pauldrons-of-despair",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "德尔西尼的杰作",
        };

        /// <summary>
        /// Firebird's Pinions 
        /// </summary>
        public static Item FirebirdsPinions = new Item
        {
            Id = 358792,
            Name = "不死鸟之翼",
            Quality = ItemQuality.Legendary,
            Slug = "firebirds-pinions",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "shoulderpads_norm_set_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/firebirds-pinions",
            Url = "https://us.battle.net/d3/en/item/firebirds-pinions",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_06_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/firebirds-pinions",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不死鸟的华服",
        };

        /// <summary>
        /// Helltooth Mantle 
        /// </summary>
        public static Item HelltoothMantle = new Item
        {
            Id = 340525,
            Name = "魔牙披风",
            Quality = ItemQuality.Legendary,
            Slug = "helltooth-mantle",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "shoulderpads_norm_set_16",
            DataUrl = "https://us.battle.net/api/d3/data/item/helltooth-mantle",
            Url = "https://us.battle.net/d3/en/item/helltooth-mantle",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_16_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/helltooth-mantle",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "魔牙战装",
        };

        /// <summary>
        /// Inarius's Martyrdom 
        /// </summary>
        public static Item InariussMartyrdom = new Item
        {
            Id = 467607,
            Name = "伊纳瑞斯的殉难",
            Quality = ItemQuality.Legendary,
            Slug = "inariuss-martyrdom",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/inariuss-martyrdom",
            Url = "https://us.battle.net/d3/en/item/inariuss-martyrdom",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_3_shoulders_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/inariuss-martyrdom",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "伊纳瑞斯的恩泽",
        };

        /// <summary>
        /// Jade Harvester's Joy 
        /// </summary>
        public static Item JadeHarvestersJoy = new Item
        {
            Id = 338042,
            Name = "玉魂师的喜悦",
            Quality = ItemQuality.Legendary,
            Slug = "jade-harvesters-joy",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "shoulderpads_norm_set_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/jade-harvesters-joy",
            Url = "https://us.battle.net/d3/en/item/jade-harvesters-joy",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_09_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jade-harvesters-joy",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "玉魂师的战甲",
        };

        /// <summary>
        /// Mantle of the Upside-Down Sinners 
        /// </summary>
        public static Item MantleOfTheUpsidedownSinners = new Item
        {
            Id = 338036,
            Name = "颠倒罪人披肩",
            Quality = ItemQuality.Legendary,
            Slug = "mantle-of-the-upsidedown-sinners",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/mantle-of-the-upsidedown-sinners",
            Url = "https://us.battle.net/d3/en/item/mantle-of-the-upsidedown-sinners",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_08_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/mantle-of-the-upsidedown-sinners",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "千风飓",
        };

        /// <summary>
        /// Marauder's Spines 
        /// </summary>
        public static Item MaraudersSpines = new Item
        {
            Id = 336996,
            Name = "掠夺者的脊骨",
            Quality = ItemQuality.Legendary,
            Slug = "marauders-spines",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "shoulderpads_norm_set_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/marauders-spines",
            Url = "https://us.battle.net/d3/en/item/marauders-spines",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_07_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/marauders-spines",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "掠夺者的化身",
        };

        /// <summary>
        /// Mountain of the Light 
        /// </summary>
        public static Item MountainOfTheLight = new Item
        {
            Id = 414925,
            Name = "圣光之山",
            Quality = ItemQuality.Legendary,
            Slug = "mountain-of-the-light",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/mountain-of-the-light",
            Url = "https://us.battle.net/d3/en/item/mountain-of-the-light",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_03_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/mountain-of-the-light",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "圣光追寻者",
        };

        /// <summary>
        /// Pauldrons of Akkhan 
        /// </summary>
        public static Item PauldronsOfAkkhan = new Item
        {
            Id = 358801,
            Name = "阿克汉的肩甲",
            Quality = ItemQuality.Legendary,
            Slug = "pauldrons-of-akkhan",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/pauldrons-of-akkhan",
            Url = "https://us.battle.net/d3/en/item/pauldrons-of-akkhan",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_10_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pauldrons-of-akkhan",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "阿克汉的战甲",
        };

        /// <summary>
        /// Pauldrons of the Wastes 
        /// </summary>
        public static Item PauldronsOfTheWastes = new Item
        {
            Id = 414921,
            Name = "荒原肩甲",
            Quality = ItemQuality.Legendary,
            Slug = "pauldrons-of-the-wastes",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/pauldrons-of-the-wastes",
            Url = "https://us.battle.net/d3/en/item/pauldrons-of-the-wastes",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_01_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pauldrons-of-the-wastes",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "废土之怒",
        };

        /// <summary>
        /// Pestilence Defense 
        /// </summary>
        public static Item PestilenceDefense = new Item
        {
            Id = 467608,
            Name = "死疫护肩",
            Quality = ItemQuality.Legendary,
            Slug = "pestilence-defense",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/pestilence-defense",
            Url = "https://us.battle.net/d3/en/item/pestilence-defense",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_4_shoulders_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pestilence-defense",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "死疫使者的裹布",
        };

        /// <summary>
        /// Raekor's Burden 
        /// </summary>
        public static Item RaekorsBurden = new Item
        {
            Id = 336989,
            Name = "蕾蔻的重任",
            Quality = ItemQuality.Legendary,
            Slug = "raekors-burden",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/raekors-burden",
            Url = "https://us.battle.net/d3/en/item/raekors-burden",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_05_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/raekors-burden",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "蕾蔻的传世铠",
        };

        /// <summary>
        /// Rathma's Spikes 
        /// </summary>
        public static Item RathmasSpikes = new Item
        {
            Id = 460923,
            Name = "拉斯玛的亡骨尖刺",
            Quality = ItemQuality.Legendary,
            Slug = "rathmas-spikes",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/rathmas-spikes",
            Url = "https://us.battle.net/d3/en/item/rathmas-spikes",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_1_shoulders_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rathmas-spikes",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "拉斯玛的骨甲",
        };

        /// <summary>
        /// Roland's Mantle 
        /// </summary>
        public static Item RolandsMantle = new Item
        {
            Id = 404699,
            Name = "罗兰之覆",
            Quality = ItemQuality.Legendary,
            Slug = "rolands-mantle",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "p1_shoulderPads_norm_set_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/rolands-mantle",
            Url = "https://us.battle.net/d3/en/item/rolands-mantle",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_01_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rolands-mantle",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "罗兰的传世甲",
        };

        /// <summary>
        /// Spires of the Earth 
        /// </summary>
        public static Item SpiresOfTheEarth = new Item
        {
            Id = 340526,
            Name = "大地之柱",
            Quality = ItemQuality.Legendary,
            Slug = "spires-of-the-earth",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "shoulderpads_norm_set_15",
            DataUrl = "https://us.battle.net/api/d3/data/item/spires-of-the-earth",
            Url = "https://us.battle.net/d3/en/item/spires-of-the-earth",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_15_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/spires-of-the-earth",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "大地之力",
        };

        /// <summary>
        /// Sunwuko's Balance 
        /// </summary>
        public static Item SunwukosBalance = new Item
        {
            Id = 336175,
            Name = "孙武空的平衡",
            Quality = ItemQuality.Legendary,
            Slug = "sunwukos-balance",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "shoulderpads_norm_set_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/sunwukos-balance",
            Url = "https://us.battle.net/d3/en/item/sunwukos-balance",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_11_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sunwukos-balance",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "猴王战甲",
        };

        /// <summary>
        /// The Shadow's Burden 
        /// </summary>
        public static Item TheShadowsBurden = new Item
        {
            Id = 444527,
            Name = "影缚",
            Quality = ItemQuality.Legendary,
            Slug = "the-shadows-burden",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-shadows-burden",
            Url = "https://us.battle.net/d3/en/item/the-shadows-burden",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_14_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-shadows-burden",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "暗影装束",
        };

        /// <summary>
        /// Trag'Oul's Heart 
        /// </summary>
        public static Item TragoulsHeart = new Item
        {
            Id = 467606,
            Name = "塔格奥之心",
            Quality = ItemQuality.Legendary,
            Slug = "tragouls-heart",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/tragouls-heart",
            Url = "https://us.battle.net/d3/en/item/tragouls-heart",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_2_shoulders_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tragouls-heart",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔格奥的化身",
        };

        /// <summary>
        /// Uliana's Strength 
        /// </summary>
        public static Item UlianasStrength = new Item
        {
            Id = 414923,
            Name = "乌莲娜的力量",
            Quality = ItemQuality.Legendary,
            Slug = "ulianas-strength",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ulianas-strength",
            Url = "https://us.battle.net/d3/en/item/ulianas-strength",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_01_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ulianas-strength",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "乌莲娜的谋略",
        };

        /// <summary>
        /// Unsanctified Shoulders 
        /// </summary>
        public static Item UnsanctifiedShoulders = new Item
        {
            Id = 414760,
            Name = "染邪护肩",
            Quality = ItemQuality.Legendary,
            Slug = "unsanctified-shoulders",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/unsanctified-shoulders",
            Url = "https://us.battle.net/d3/en/item/unsanctified-shoulders",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_03_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/unsanctified-shoulders",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "邪秽之精",
        };

        /// <summary>
        /// Vyr's Proud Pauldrons 
        /// </summary>
        public static Item VyrsProudPauldrons = new Item
        {
            Id = 439186,
            Name = "维尔的傲慢肩甲",
            Quality = ItemQuality.Legendary,
            Slug = "vyrs-proud-pauldrons",
            ItemType = ItemType.Shoulder,
            TrinityItemType = TrinityItemType.Shoulder,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/vyrs-proud-pauldrons",
            Url = "https://us.battle.net/d3/en/item/vyrs-proud-pauldrons",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shoulder_set_13_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vyrs-proud-pauldrons",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "维尔的神装",
        };

        /// <summary>
        /// Funerary Pick Siphon Blood drains blood from 2 additional targets.
        /// </summary>
        public static Item FuneraryPick = new Item
        {
            Id = 467370,
            Name = "墓葬镐",
            Quality = ItemQuality.Legendary,
            Slug = "funerary-pick",
            ItemType = ItemType.Scythe,
            TrinityItemType = TrinityItemType.Scythe,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/funerary-pick",
            Url = "https://us.battle.net/d3/en/item/funerary-pick",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_scythe1h_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/funerary-pick",
            IsCrafted = false,
            LegendaryAffix = "鲜血虹吸现在可以从额外 2 个目标身上窃取生命。",
            SetName = "",
        };

        /// <summary>
        /// Trag'Oul's Corroded Fang The Cursed Scythe rune for Grim Scythe now has a 100% chance to apply a curse and you deal 150–200 % increased damage to cursed enemies.
        /// </summary>
        public static Item TragoulsCorrodedFang = new Item
        {
            Id = 467394,
            Name = "塔格奥的蚀牙",
            Quality = ItemQuality.Legendary,
            Slug = "tragouls-corroded-fang",
            ItemType = ItemType.Scythe,
            TrinityItemType = TrinityItemType.Scythe,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/tragouls-corroded-fang",
            Url = "https://us.battle.net/d3/en/item/tragouls-corroded-fang",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_scythe1h_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tragouls-corroded-fang",
            IsCrafted = false,
            LegendaryAffix = "恐镰的恶咒镰刀符文现在有100%的几率施加一个诅咒，并对受诅咒的敌人造成额外（150-200）%的伤害。",
            SetName = "",
        };

        /// <summary>
        /// Scythe of the Cycle Your Secondary skills deal 250–300 % additional damage while Bone Armor is active but reduce the remaining duration of Bone Armor by 4 seconds.
        /// </summary>
        public static Item ScytheOfTheCycle = new Item
        {
            Id = 467579,
            Name = "轮回镰刀",
            Quality = ItemQuality.Legendary,
            Slug = "scythe-of-the-cycle",
            ItemType = ItemType.Scythe,
            TrinityItemType = TrinityItemType.Scythe,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/scythe-of-the-cycle",
            Url = "https://us.battle.net/d3/en/item/scythe-of-the-cycle",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_scythe1h_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/scythe-of-the-cycle",
            IsCrafted = false,
            LegendaryAffix = "骨甲激活时，你的次要技能造成额外 350-400% 的伤害，但会使骨甲的剩余持续时间缩短4秒。",
            SetName = "",
        };

        /// <summary>
        /// Jesseth Skullscythe 
        /// </summary>
        public static Item JessethSkullscythe = new Item
        {
            Id = 467580,
            Name = "杰西斯骷髅刀",
            Quality = ItemQuality.Legendary,
            Slug = "jesseth-skullscythe",
            ItemType = ItemType.Scythe,
            TrinityItemType = TrinityItemType.Scythe,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/jesseth-skullscythe",
            Url = "https://us.battle.net/d3/en/item/jesseth-skullscythe",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_scythe1h_04_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jesseth-skullscythe",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "杰西斯武装",
        };

        /// <summary>
        /// Fletcher's Pride 
        /// </summary>
        public static Item FletchersPride = new Item
        {
            Id = 197629,
            Name = "造箭师的骄傲",
            Quality = ItemQuality.Legendary,
            Slug = "fletchers-pride",
            ItemType = ItemType.Quiver,
            TrinityItemType = TrinityItemType.Quiver,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Quiver_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/fletchers-pride",
            Url = "https://us.battle.net/d3/en/item/fletchers-pride",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_quiver_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fletchers-pride",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Sin Seekers Rapid Fire no longer has a channel cost.
        /// </summary>
        public static Item SinSeekers = new Item
        {
            Id = 197625,
            Name = "觅罪者",
            Quality = ItemQuality.Legendary,
            Slug = "sin-seekers",
            ItemType = ItemType.Quiver,
            TrinityItemType = TrinityItemType.Quiver,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Quiver_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/sin-seekers",
            Url = "https://us.battle.net/d3/en/item/sin-seekers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p43_unique_quiver_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sin-seekers",
            IsCrafted = false,
            LegendaryAffix = "急速射击不再有引导消耗 (10000%)。",
            SetName = "",
        };

        /// <summary>
        /// Holy Point Shot Impale throws two additional knives.
        /// </summary>
        public static Item HolyPointShot = new Item
        {
            Id = 197627,
            Name = "圣力箭",
            Quality = ItemQuality.Legendary,
            Slug = "holy-point-shot",
            ItemType = ItemType.Quiver,
            TrinityItemType = TrinityItemType.Quiver,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Quiver_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/holy-point-shot",
            Url = "https://us.battle.net/d3/en/item/holy-point-shot",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p5_unique_quiver_004_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/holy-point-shot",
            IsCrafted = false,
            LegendaryAffix = "暗影飞刀投掷 2 把额外的飞刀。",
            SetName = "",
        };

        /// <summary>
        /// Spines of Seething Hatred Chakram now generates 3–4 Hatred.
        /// </summary>
        public static Item SpinesOfSeethingHatred = new Item
        {
            Id = 197628,
            Name = "沸怒脊刺",
            Quality = ItemQuality.Legendary,
            Slug = "spines-of-seething-hatred",
            ItemType = ItemType.Quiver,
            TrinityItemType = TrinityItemType.Quiver,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/spines-of-seething-hatred",
            Url = "https://us.battle.net/d3/en/item/spines-of-seething-hatred",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_quiver_005_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/spines-of-seething-hatred",
            IsCrafted = false,
            LegendaryAffix = "飞轮刃现在可生成 (3-4) 点憎恨值。",
            SetName = "",
        };

        /// <summary>
        /// Bombardier's Rucksack You may have 2 additional Sentries.
        /// </summary>
        public static Item BombardiersRucksack = new Item
        {
            Id = 298171,
            Name = "炮手弹药包",
            Quality = ItemQuality.Legendary,
            Slug = "bombardiers-rucksack",
            ItemType = ItemType.Quiver,
            TrinityItemType = TrinityItemType.Quiver,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bombardiers-rucksack",
            Url = "https://us.battle.net/d3/en/item/bombardiers-rucksack",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_quiver_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bombardiers-rucksack",
            IsCrafted = false,
            LegendaryAffix = "你可以拥有额外2个箭塔。",
            SetName = "",
        };

        /// <summary>
        /// Emimei's Duffel Bolas now explode instantly.
        /// </summary>
        public static Item EmimeisDuffel = new Item
        {
            Id = 298172,
            Name = "艾米梅的粗呢袋",
            Quality = ItemQuality.Legendary,
            Slug = "emimeis-duffel",
            ItemType = ItemType.Quiver,
            TrinityItemType = TrinityItemType.Quiver,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "quiver_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/emimeis-duffel",
            Url = "https://us.battle.net/d3/en/item/emimeis-duffel",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_quiver_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/emimeis-duffel",
            IsCrafted = false,
            LegendaryAffix = "流星索现在会立即爆炸。",
            SetName = "",
        };

        /// <summary>
        /// The Ninth Cirri Satchel Hungering Arrow has 20–25 % additional chance to pierce.
        /// </summary>
        public static Item TheNinthCirriSatchel = new Item
        {
            Id = 298170,
            Name = "希瑞的九号箭袋",
            Quality = ItemQuality.Legendary,
            Slug = "the-ninth-cirri-satchel",
            ItemType = ItemType.Quiver,
            TrinityItemType = TrinityItemType.Quiver,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "quiver_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-ninth-cirri-satchel",
            Url = "https://us.battle.net/d3/en/item/the-ninth-cirri-satchel",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_quiver_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-ninth-cirri-satchel",
            IsCrafted = false,
            LegendaryAffix = "追踪箭的穿透几率提高（20-25）%。",
            SetName = "",
        };

        /// <summary>
        /// Augustine's Panacea Elemental Arrow gains an effect based on the rune: Ball Lightning now travels at 30% speed. Frost Arrow damage and Chilled duration increased by 200–250 % . Immolation Arrow ground damage over time increased by 200–250 % . Lightning Bolts damage and Stun duration increased by 200–250 % . Nether Tentacles damage and healing amount increased by 200–250 % .
        /// </summary>
        public static Item AugustinesPanacea = new Item
        {
            Id = 197624,
            Name = "奥古斯丁的灵药",
            Quality = ItemQuality.Legendary,
            Slug = "augustines-panacea",
            ItemType = ItemType.Quiver,
            TrinityItemType = TrinityItemType.Quiver,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/augustines-panacea",
            Url = "https://us.battle.net/d3/en/item/augustines-panacea",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_quiver_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/augustines-panacea",
            IsCrafted = false,
            LegendaryAffix = "元素箭根据符文获得不同的效果：闪电球现在以 30% 的速度移动。冰霜箭的伤害以及寒冷的持续时间提高 (200-250)%。火祭箭的持续性地面伤害提高 (200-250)%。闪电箭的伤害和昏迷时间提高 (200-250)%。触须箭的伤害和治疗量提高 (200-250)%。",
            SetName = "",
        };

        /// <summary>
        /// Dead Man's Legacy Multishot hits enemies below 50–60 % health twice.
        /// </summary>
        public static Item DeadMansLegacy = new Item
        {
            Id = 197630,
            Name = "亡者遗物",
            Quality = ItemQuality.Legendary,
            Slug = "dead-mans-legacy",
            ItemType = ItemType.Quiver,
            TrinityItemType = TrinityItemType.Quiver,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "quiver_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/dead-mans-legacy",
            Url = "https://us.battle.net/d3/en/item/dead-mans-legacy",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_quiver_007_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/dead-mans-legacy",
            IsCrafted = false,
            LegendaryAffix = "多重射击可击中生命值低于 60% 的敌人两次，并且伤害提高 150-200%。",
            SetName = "",
        };

        /// <summary>
        /// Monster Hunter 
        /// </summary>
        public static Item MonsterHunter = new Item
        {
            Id = 115140,
            Name = "怪物猎手",
            Quality = ItemQuality.Legendary,
            Slug = "monster-hunter",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Sword_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/monster-hunter",
            Url = "https://us.battle.net/d3/en/item/monster-hunter",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_017_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/monster-hunter",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Wildwood 
        /// </summary>
        public static Item Wildwood = new Item
        {
            Id = 270978,
            Name = "野木林",
            Quality = ItemQuality.Legendary,
            Slug = "wildwood",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/wildwood",
            Url = "https://us.battle.net/d3/en/item/wildwood",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/wildwood",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Doombringer 
        /// </summary>
        public static Item Doombringer = new Item
        {
            Id = 185397,
            Name = "末日使者",
            Quality = ItemQuality.Legendary,
            Slug = "doombringer",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "sword_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/doombringer",
            Url = "https://us.battle.net/d3/en/item/doombringer",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_014_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/doombringer",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// The Ancient Bonesaber of Zumakalis 
        /// </summary>
        public static Item TheAncientBonesaberOfZumakalis = new Item
        {
            Id = 194481,
            Name = "祖玛卡里斯的上古骨刃",
            Quality = ItemQuality.Legendary,
            Slug = "the-ancient-bonesaber-of-zumakalis",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Sword_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-ancient-bonesaber-of-zumakalis",
            Url = "https://us.battle.net/d3/en/item/the-ancient-bonesaber-of-zumakalis",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-ancient-bonesaber-of-zumakalis",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Exarian 
        /// </summary>
        public static Item Exarian = new Item
        {
            Id = 271617,
            Name = "王者之剑",
            Quality = ItemQuality.Legendary,
            Slug = "exarian",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "sword_norm_unique_13",
            DataUrl = "https://us.battle.net/api/d3/data/item/exarian",
            Url = "https://us.battle.net/d3/en/item/exarian",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/exarian",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Fulminator Lightning damage has a chance to turn enemies into lightning rods, causing them to pulse 444–555 % weapon damage as Lightning every second to nearby enemies for 6 seconds.
        /// </summary>
        public static Item Fulminator = new Item
        {
            Id = 271631,
            Name = "怒雷",
            Quality = ItemQuality.Legendary,
            Slug = "fulminator",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "sword_norm_unique_15",
            DataUrl = "https://us.battle.net/api/d3/data/item/fulminator",
            Url = "https://us.battle.net/d3/en/item/fulminator",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_sword_1h_104_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fulminator",
            IsCrafted = false,
            LegendaryAffix = "闪电伤害有一定几率将敌人变成引雷针，使他们在 6 秒内每秒对周围的敌人造成 (444-555)% 的武器伤害（作为闪电伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Gift of Silaria 
        /// </summary>
        public static Item GiftOfSilaria = new Item
        {
            Id = 271630,
            Name = "希拉丽雅的礼物",
            Quality = ItemQuality.Legendary,
            Slug = "gift-of-silaria",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "sword_norm_unique_14",
            DataUrl = "https://us.battle.net/api/d3/data/item/gift-of-silaria",
            Url = "https://us.battle.net/d3/en/item/gift-of-silaria",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/gift-of-silaria",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Rimeheart 20% chance on hit to instantly deal 10000 % weapon damage as Cold to enemies that are Frozen.
        /// </summary>
        public static Item Rimeheart = new Item
        {
            Id = 271636,
            Name = "霜心",
            Quality = ItemQuality.Legendary,
            Slug = "rimeheart",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "sword_norm_unique_20",
            DataUrl = "https://us.battle.net/api/d3/data/item/rimeheart",
            Url = "https://us.battle.net/d3/en/item/rimeheart",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_109_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rimeheart",
            IsCrafted = false,
            LegendaryAffix = "击中有 20% 的几率立即对被冰冻的敌人造成 10000% 的武器伤害（作为冰霜伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Thunderfury, Blessed Blade of the Windseeker Chance on hit to blast your enemy with Lightning, dealing 279–372 % weapon damage as Lightning and then jumping to additional nearby enemies. Each enemy hit has their attack speed and movement speed reduced by 30% for 3 seconds. Jumps up to 5 targets.
        /// </summary>
        public static Item ThunderfuryBlessedBladeOfTheWindseeker = new Item
        {
            Id = 229716,
            Name = "雷霆之怒,逐风者的祝福之刃",
            Quality = ItemQuality.Legendary,
            Slug = "thunderfury-blessed-blade-of-the-windseeker",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "sword_norm_unique_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/thunderfury-blessed-blade-of-the-windseeker",
            Url = "https://us.battle.net/d3/en/item/thunderfury-blessed-blade-of-the-windseeker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/thunderfury-blessed-blade-of-the-windseeker",
            IsCrafted = false,
            LegendaryAffix = "击中时有一定几率用闪电冲击敌人，造成 (279-372)% 的武器伤害（作为闪电伤害），然后弹跳到附近的敌人身上。每个被命中的敌人攻击和移动速度降低 30% ，持续 3 秒。最多可弹跳到 5 个目标身上。",
            SetName = "",
        };

        /// <summary>
        /// Sever Slain enemies rest in pieces.
        /// </summary>
        public static Item Sever = new Item
        {
            Id = 115141,
            Name = "断离",
            Quality = ItemQuality.Legendary,
            Slug = "sever",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Sword_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/sever",
            Url = "https://us.battle.net/d3/en/item/sever",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sever",
            IsCrafted = false,
            LegendaryAffix = "让敌人死无全尸。",
            SetName = "",
        };

        /// <summary>
        /// Skycutter Chance to summon angelic assistance when attacking.
        /// </summary>
        public static Item Skycutter = new Item
        {
            Id = 182347,
            Name = "断空",
            Quality = ItemQuality.Legendary,
            Slug = "skycutter",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Sword_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/skycutter",
            Url = "https://us.battle.net/d3/en/item/skycutter",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_004_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/skycutter",
            IsCrafted = false,
            LegendaryAffix = "攻击时有一定几率召唤天使协助作战。 (25%)",
            SetName = "",
        };

        /// <summary>
        /// Azurewrath Undead and Demon enemies within 25 yards take 500–650 % weapon damage as Holy every second and are sometimes knocked into the air.
        /// </summary>
        public static Item Azurewrath = new Item
        {
            Id = 192511,
            Name = "碧蓝怒火",
            Quality = ItemQuality.Legendary,
            Slug = "azurewrath",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "sword_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/azurewrath",
            Url = "https://us.battle.net/d3/en/item/azurewrath",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_sword_1h_012_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/azurewrath",
            IsCrafted = false,
            LegendaryAffix = "25 码内的亡灵和恶魔敌人每秒受到 (500-650)% 的武器伤害（作为神圣伤害），且有时会被击飞。",
            SetName = "",
        };

        /// <summary>
        /// Devil Tongue 
        /// </summary>
        public static Item DevilTongue = new Item
        {
            Id = 189552,
            Name = "恶魔之舌",
            Quality = ItemQuality.Legendary,
            Slug = "devil-tongue",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Sword_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/devil-tongue",
            Url = "https://us.battle.net/d3/en/item/devil-tongue",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/devil-tongue",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Deathwish While channeling Arcane Torrent, Disintegrate, or Ray of Frost, all damage is increased by 30–35 % .
        /// </summary>
        public static Item Deathwish = new Item
        {
            Id = 331908,
            Name = "绝命",
            Quality = ItemQuality.Legendary,
            Slug = "deathwish",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/deathwish",
            Url = "https://us.battle.net/d3/en/item/deathwish",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_112_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/deathwish",
            IsCrafted = false,
            LegendaryAffix = "引导奥术洪流,瓦解射线或冰霜射线时，所有伤害提高 (250-325)%。",
            SetName = "",
        };

        /// <summary>
        /// In-geom Your skill cooldowns are reduced by 8–10 seconds for 15 seconds after killing an elite pack.
        /// </summary>
        public static Item Ingeom = new Item
        {
            Id = 410946,
            Name = "寅剑",
            Quality = ItemQuality.Legendary,
            Slug = "ingeom",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ingeom",
            Url = "https://us.battle.net/d3/en/item/ingeom",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_113_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ingeom",
            IsCrafted = false,
            LegendaryAffix = "消灭一队精英怪群后，你的技能冷却时间缩短(8-10)秒，持续15秒。",
            SetName = "",
        };

        /// <summary>
        /// Shard of Hate Elemental skills have a chance to trigger a powerful attack that deals 200–250 % weapon damage: Cold skills trigger Freezing Skull Poison skills trigger Poison Nova Lightning skills trigger Charged Bolt
        /// </summary>
        public static Item ShardOfHate = new Item
        {
            Id = 376463,
            Name = "憎恨碎片",
            Quality = ItemQuality.Legendary,
            Slug = "shard-of-hate",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "sword_norm_promo_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/shard-of-hate",
            Url = "https://us.battle.net/d3/en/item/shard-of-hate",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_promo_02_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/shard-of-hate",
            IsCrafted = false,
            LegendaryAffix = "元素技能有一定几率触发强力攻击，造成 (200-250)% 的武器伤害：冰霜技能触发冰冻颅骨毒性技能触发剧毒新星闪电技能触发闪电霹雳。",
            SetName = "",
        };

        /// <summary>
        /// Sword of Ill Will Chakram deals 1.0–1.4 % increased damage for every point of Hatred you have.
        /// </summary>
        public static Item SwordOfIllWill = new Item
        {
            Id = 328591,
            Name = "恶意之剑",
            Quality = ItemQuality.Legendary,
            Slug = "sword-of-ill-will",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/sword-of-ill-will",
            Url = "https://us.battle.net/d3/en/item/sword-of-ill-will",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_sword_1h_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sword-of-ill-will",
            IsCrafted = false,
            LegendaryAffix = "你每拥有一点憎恨值，飞轮刃造成的伤害便提高 (1.0-1.4)%。",
            SetName = "",
        };

        /// <summary>
        /// The Twisted Sword Energy Twister damage is increased by 125–150 % for each Energy Twister you have out up to a maximum of 5.
        /// </summary>
        public static Item TheTwistedSword = new Item
        {
            Id = 271634,
            Name = "扭曲之剑",
            Quality = ItemQuality.Legendary,
            Slug = "the-twisted-sword",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-twisted-sword",
            Url = "https://us.battle.net/d3/en/item/the-twisted-sword",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_107_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-twisted-sword",
            IsCrafted = false,
            LegendaryAffix = "你放出的每一个能量气旋（最多5个）都能使你的能量气旋的伤害提高 (125-150)%。",
            SetName = "",
        };

        /// <summary>
        /// Little Rogue 
        /// </summary>
        public static Item LittleRogue = new Item
        {
            Id = 313291,
            Name = "小流氓",
            Quality = ItemQuality.Legendary,
            Slug = "little-rogue",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "sword_norm_set_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/little-rogue",
            Url = "https://us.battle.net/d3/en/item/little-rogue",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_set_03_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/little-rogue",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "伊斯特凡的对剑",
        };

        /// <summary>
        /// The Slanderer 
        /// </summary>
        public static Item TheSlanderer = new Item
        {
            Id = 313290,
            Name = "挑衅者",
            Quality = ItemQuality.Legendary,
            Slug = "the-slanderer",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.Sword,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "sword_norm_set_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-slanderer",
            Url = "https://us.battle.net/d3/en/item/the-slanderer",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_1h_set_02_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-slanderer",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "伊斯特凡的对剑",
        };

        /// <summary>
        /// Lost Time Your cold skills reduce the movement speed of enemies by 30% . In addition, your movement speed is increased by 4–5 % for 5 seconds. Maximum 5 stacks.
        /// </summary>
        public static Item LostTime = new Item
        {
            Id = 467582,
            Name = "失时",
            Quality = ItemQuality.Legendary,
            Slug = "lost-time",
            ItemType = ItemType.Phylactery,
            TrinityItemType = TrinityItemType.Phylactery,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/lost-time",
            Url = "https://us.battle.net/d3/en/item/lost-time",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_phylactery_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/lost-time",
            IsCrafted = false,
            LegendaryAffix = "你的冰霜技能可以使敌人速度降低30%。此外，你的移动速度提高（8-10）%，持续5秒，最多叠加5层。",
            SetName = "",
        };

        /// <summary>
        /// Iron Rose Attacking with Siphon Blood has a 40–50 % chance to cast a free Blood Nova.
        /// </summary>
        public static Item IronRose = new Item
        {
            Id = 467581,
            Name = "铁玫瑰",
            Quality = ItemQuality.Legendary,
            Slug = "iron-rose",
            ItemType = ItemType.Phylactery,
            TrinityItemType = TrinityItemType.Phylactery,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/iron-rose",
            Url = "https://us.battle.net/d3/en/item/iron-rose",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_phylactery_04_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/iron-rose",
            IsCrafted = false,
            LegendaryAffix = "使用鲜血虹吸攻击有（40-50）%的几率施放一次免费的鲜血新星。",
            SetName = "",
        };

        /// <summary>
        /// Leger's Disdain Grim Scythe deals an additional 7–10 % damage for each point of essence it restores.
        /// </summary>
        public static Item LegersDisdain = new Item
        {
            Id = 462250,
            Name = "勒杰的蔑视",
            Quality = ItemQuality.Legendary,
            Slug = "legers-disdain",
            ItemType = ItemType.Phylactery,
            TrinityItemType = TrinityItemType.Phylactery,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/legers-disdain",
            Url = "https://us.battle.net/d3/en/item/legers-disdain",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_phylactery_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/legers-disdain",
            IsCrafted = false,
            LegendaryAffix = "恐镰每为你恢复一点精魂，就会造成额外（65-80）%的伤害。",
            SetName = "",
        };

        /// <summary>
        /// Bone Ringer The damage bonus of Command Skeletons increases by 25–30 % per second they spend attacking the same target.
        /// </summary>
        public static Item BoneRinger = new Item
        {
            Id = 462249,
            Name = "响骨",
            Quality = ItemQuality.Legendary,
            Slug = "bone-ringer",
            ItemType = ItemType.Phylactery,
            TrinityItemType = TrinityItemType.Phylactery,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bone-ringer",
            Url = "https://us.battle.net/d3/en/item/bone-ringer",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_phylactery_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bone-ringer",
            IsCrafted = false,
            LegendaryAffix = "号令骸骨攻击同一目标的时间每持续一秒，其伤害加成就会提高（25-30）%。",
            SetName = "",
        };

        /// <summary>
        /// Reilena's Shadowhook Every point of Maximum Essence increases your damage by 0.5% and Bone Spikes generates 2–5 additional Essence for each enemy hit.
        /// </summary>
        public static Item ReilenasShadowhook = new Item
        {
            Id = 467600,
            Name = "莱勒娜的影镰",
            Quality = ItemQuality.Legendary,
            Slug = "reilenas-shadowhook",
            ItemType = ItemType.Scythe,
            TrinityItemType = TrinityItemType.TwoHandScythe,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/reilenas-shadowhook",
            Url = "https://us.battle.net/d3/en/item/reilenas-shadowhook",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_scythe2h_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/reilenas-shadowhook",
            IsCrafted = false,
            LegendaryAffix = "每一点最大精魂都可以使你的伤害提高 0.5 % 并且骨刺每击中一个敌人就可生成额外2-5点精魂。",
            SetName = "",
        };

        /// <summary>
        /// Maltorius' Petrified Spike Bone Spear now costs 40 Essence and deals 375–450 % increased damage.
        /// </summary>
        public static Item MaltoriusPetrifiedSpike = new Item
        {
            Id = 467598,
            Name = "玛托瑞斯的僵刺",
            Quality = ItemQuality.Legendary,
            Slug = "maltorius-petrified-spike",
            ItemType = ItemType.Scythe,
            TrinityItemType = TrinityItemType.TwoHandScythe,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/maltorius-petrified-spike",
            Url = "https://us.battle.net/d3/en/item/maltorius-petrified-spike",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_scythe2h_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/maltorius-petrified-spike",
            IsCrafted = false,
            LegendaryAffix = "骨矛现在消耗40点精魂并提高（550-700）%的伤害。",
            SetName = "",
        };

        /// <summary>
        /// Bloodtide Blade Death Nova deals 20–30 % increased damage for every enemy within 15 yards.
        /// </summary>
        public static Item BloodtideBlade = new Item
        {
            Id = 467599,
            Name = "血潮利刃",
            Quality = ItemQuality.Legendary,
            Slug = "bloodtide-blade",
            ItemType = ItemType.Scythe,
            TrinityItemType = TrinityItemType.TwoHandScythe,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bloodtide-blade",
            Url = "https://us.battle.net/d3/en/item/bloodtide-blade",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_scythe2h_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bloodtide-blade",
            IsCrafted = false,
            LegendaryAffix = "25 码范围内每个敌人都会使死亡新星造成的伤害提高 80-100%。",
            SetName = "",
        };

        /// <summary>
        /// Nayr's Black Death Each different poison skill you use increases the damage of your poison skills by 50–65 % for 15 seconds.
        /// </summary>
        public static Item NayrsBlackDeath = new Item
        {
            Id = 467594,
            Name = "纳伊尔的黑镰",
            Quality = ItemQuality.Legendary,
            Slug = "nayrs-black-death",
            ItemType = ItemType.Scythe,
            TrinityItemType = TrinityItemType.TwoHandScythe,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/nayrs-black-death",
            Url = "https://us.battle.net/d3/en/item/nayrs-black-death",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_scythe2h_04_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/nayrs-black-death",
            IsCrafted = false,
            LegendaryAffix = "你每使用一个不同的毒性技能都会使你的毒性技能伤害提高（75-100）%，持续15秒。",
            SetName = "",
        };

        // AUTO-GENERATED on Fri, 30 Jun 2017 15:22:55 GMT


        /// <summary>
        /// Crown of the Primus Slow Time gains the effect of every rune.
        /// </summary>
        public static Item CrownOfThePrimus = new Item
        {
            Id = 349951,
            Name = "普莱姆斯之冠",
            Quality = ItemQuality.Legendary,
            Slug = "crown-of-the-primus",
            ItemType = ItemType.WizardHat,
            TrinityItemType = TrinityItemType.WizardHat,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/crown-of-the-primus",
            Url = "https://us.battle.net/d3/en/item/crown-of-the-primus",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_wizardhat_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/crown-of-the-primus",
            IsCrafted = false,
            LegendaryAffix = "时间延缓获得每个符文的效果。",
            SetName = "",
        };

        /// <summary>
        /// The Swami The bonuses from Archon stacks now last for 15–20 seconds after Archon expires.
        /// </summary>
        public static Item TheSwami = new Item
        {
            Id = 218681,
            Name = "法尊",
            Quality = ItemQuality.Legendary,
            Slug = "the-swami",
            ItemType = ItemType.WizardHat,
            TrinityItemType = TrinityItemType.WizardHat,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "wizardhat_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-swami",
            Url = "https://us.battle.net/d3/en/item/the-swami",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_wizardhat_003_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-swami",
            IsCrafted = false,
            LegendaryAffix = "御法者的叠加效果会在御法者形态结束后持续存在 (15-20) 秒。",
            SetName = "",
        };

        /// <summary>
        /// Dark Mage's Shade Automatically cast Diamond Skin when you fall below 35% Life. This effect may occur once every 15–20 seconds.
        /// </summary>
        public static Item DarkMagesShade = new Item
        {
            Id = 224908,
            Name = "黑法师的遮阳帽",
            Quality = ItemQuality.Legendary,
            Slug = "dark-mages-shade",
            ItemType = ItemType.WizardHat,
            TrinityItemType = TrinityItemType.WizardHat,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "wizardhat_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/dark-mages-shade",
            Url = "https://us.battle.net/d3/en/item/dark-mages-shade",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_wizardhat_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/dark-mages-shade",
            IsCrafted = false,
            LegendaryAffix = "当你低于35%生命时自动施放钻石体肤。该效果每 (15-20) 秒只能生效一次。",
            SetName = "",
        };

        /// <summary>
        /// Archmage's Vicalyke Your Mirror Images have a chance to multiply when killed by enemies.
        /// </summary>
        public static Item ArchmagesVicalyke = new Item
        {
            Id = 299471,
            Name = "大法师的回响",
            Quality = ItemQuality.Legendary,
            Slug = "archmages-vicalyke",
            ItemType = ItemType.WizardHat,
            TrinityItemType = TrinityItemType.WizardHat,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "wizardhat_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/archmages-vicalyke",
            Url = "https://us.battle.net/d3/en/item/archmages-vicalyke",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_wizardhat_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/archmages-vicalyke",
            IsCrafted = false,
            LegendaryAffix = "当你的镜像被敌人消灭时，有一定几率复制出更多镜像。",
            SetName = "",
        };

        /// <summary>
        /// The Magistrate Frost Hydra now periodically casts Frost Nova.
        /// </summary>
        public static Item TheMagistrate = new Item
        {
            Id = 325579,
            Name = "执法官",
            Quality = ItemQuality.Legendary,
            Slug = "the-magistrate",
            ItemType = ItemType.WizardHat,
            TrinityItemType = TrinityItemType.WizardHat,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "wizardhat_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-magistrate",
            Url = "https://us.battle.net/d3/en/item/the-magistrate",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_wizardhat_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-magistrate",
            IsCrafted = false,
            LegendaryAffix = "冰霜多头蛇现在会周期性地施放出冰霜新星。",
            SetName = "",
        };

        /// <summary>
        /// Velvet Camaral Double the number of enemies your Electrocute jumps to.
        /// </summary>
        public static Item VelvetCamaral = new Item
        {
            Id = 299472,
            Name = "丝绒羽冠",
            Quality = ItemQuality.Legendary,
            Slug = "velvet-camaral",
            ItemType = ItemType.WizardHat,
            TrinityItemType = TrinityItemType.WizardHat,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "wizardhat_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/velvet-camaral",
            Url = "https://us.battle.net/d3/en/item/velvet-camaral",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_wizardhat_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/velvet-camaral",
            IsCrafted = false,
            LegendaryAffix = "使你电刑弹跳击中的敌人数量翻倍。",
            SetName = "",
        };

        /// <summary>
        /// Storm Crow 20–40 % chance to cast a fiery ball when attacking.
        /// </summary>
        public static Item StormCrow = new Item
        {
            Id = 220694,
            Name = "风暴乌鸦",
            Quality = ItemQuality.Legendary,
            Slug = "storm-crow",
            ItemType = ItemType.WizardHat,
            TrinityItemType = TrinityItemType.WizardHat,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "wizardHat_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/storm-crow",
            Url = "https://us.battle.net/d3/en/item/storm-crow",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_wizardhat_004_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/storm-crow",
            IsCrafted = false,
            LegendaryAffix = "攻击时有 (20-40)% 的几率施放出一枚火球。",
            SetName = "",
        };

        /// <summary>
        /// See No Evil 
        /// </summary>
        public static Item SeeNoEvil = new Item
        {
            Id = 222171,
            Name = "至善之眼",
            Quality = ItemQuality.Legendary,
            Slug = "see-no-evil",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritStone_norm_unique_13",
            DataUrl = "https://us.battle.net/api/d3/data/item/see-no-evil",
            Url = "https://us.battle.net/d3/en/item/see-no-evil",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spiritstone_005_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/see-no-evil",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Gyana Na Kashu Lashing Tail Kick releases a piercing fireball that deals 1050–1400 % weapon damage as Fire to enemies within 10 yards on impact.
        /// </summary>
        public static Item GyanaNaKashu = new Item
        {
            Id = 222169,
            Name = "知识之眼",
            Quality = ItemQuality.Legendary,
            Slug = "gyana-na-kashu",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritStone_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/gyana-na-kashu",
            Url = "https://us.battle.net/d3/en/item/gyana-na-kashu",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spiritstone_004_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/gyana-na-kashu",
            IsCrafted = false,
            LegendaryAffix = "神龙摆尾释放出一发穿刺火球，对击中点附近 10 码内的敌人造成 (1050-1400)% 的武器伤害（作为火焰伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Erlang Shen 
        /// </summary>
        public static Item ErlangShen = new Item
        {
            Id = 222173,
            Name = "二郎神天眼",
            Quality = ItemQuality.Legendary,
            Slug = "erlang-shen",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritStone_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/erlang-shen",
            Url = "https://us.battle.net/d3/en/item/erlang-shen",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spiritstone_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/erlang-shen",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// The Mind's Eye Inner Sanctuary increases Spirit Regeneration per second by 10–15 .
        /// </summary>
        public static Item TheMindsEye = new Item
        {
            Id = 222172,
            Name = "心眼",
            Quality = ItemQuality.Legendary,
            Slug = "the-minds-eye",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritstone_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-minds-eye",
            Url = "https://us.battle.net/d3/en/item/the-minds-eye",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spiritstone_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-minds-eye",
            IsCrafted = false,
            LegendaryAffix = "金轮阵使每秒恢复的内力提高 (10-15) 点。",
            SetName = "",
        };

        /// <summary>
        /// Eye of Peshkov Reduce the cooldown of Breath of Heaven by 38–50 % .
        /// </summary>
        public static Item EyeOfPeshkov = new Item
        {
            Id = 299464,
            Name = "佩什科夫之眼",
            Quality = ItemQuality.Legendary,
            Slug = "eye-of-peshkov",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritstone_norm_unique_16",
            DataUrl = "https://us.battle.net/api/d3/data/item/eye-of-peshkov",
            Url = "https://us.battle.net/d3/en/item/eye-of-peshkov",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spiritstone_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/eye-of-peshkov",
            IsCrafted = false,
            LegendaryAffix = "使回天息的冷却时间缩短 (38-50)%。",
            SetName = "",
        };

        /// <summary>
        /// Kekegi's Unbreakable Spirit Damaging enemies has a chance to grant you an effect that removes the Spirit cost of your abilities for 2–4 seconds.
        /// </summary>
        public static Item KekegisUnbreakableSpirit = new Item
        {
            Id = 299461,
            Name = "柯克吉的不屈精神",
            Quality = ItemQuality.Legendary,
            Slug = "kekegis-unbreakable-spirit",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritstone_norm_unique_15",
            DataUrl = "https://us.battle.net/api/d3/data/item/kekegis-unbreakable-spirit",
            Url = "https://us.battle.net/d3/en/item/kekegis-unbreakable-spirit",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spiritstone_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/kekegis-unbreakable-spirit",
            IsCrafted = false,
            LegendaryAffix = "对敌人造成伤害时有一定几率使你的技能不消耗内力，持续 (2-4) 秒。",
            SetName = "",
        };

        /// <summary>
        /// The Laws of Seph Using Blinding Flash restores 125–165 Spirit.
        /// </summary>
        public static Item TheLawsOfSeph = new Item
        {
            Id = 299454,
            Name = "瑟夫之法",
            Quality = ItemQuality.Legendary,
            Slug = "the-laws-of-seph",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritstone_norm_unique_14",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-laws-of-seph",
            Url = "https://us.battle.net/d3/en/item/the-laws-of-seph",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spiritstone_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-laws-of-seph",
            IsCrafted = false,
            LegendaryAffix = "使用眩目闪恢复 (125-165) 点内力。",
            SetName = "",
        };

        /// <summary>
        /// Bezoar Stone 
        /// </summary>
        public static Item BezoarStone = new Item
        {
            Id = 222306,
            Name = "牛黄石",
            Quality = ItemQuality.Legendary,
            Slug = "bezoar-stone",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritStone_norm_unique_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/bezoar-stone",
            Url = "https://us.battle.net/d3/en/item/bezoar-stone",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spiritstone_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bezoar-stone",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// The Eye of the Storm 
        /// </summary>
        public static Item TheEyeOfTheStorm = new Item
        {
            Id = 222170,
            Name = "风暴之眼",
            Quality = ItemQuality.Legendary,
            Slug = "the-eye-of-the-storm",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritStone_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-eye-of-the-storm",
            Url = "https://us.battle.net/d3/en/item/the-eye-of-the-storm",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spiritstone_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-eye-of-the-storm",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Madstone Your Seven-Sided Strike applies Exploding Palm.
        /// </summary>
        public static Item Madstone = new Item
        {
            Id = 221572,
            Name = "疯狂石",
            Quality = ItemQuality.Legendary,
            Slug = "madstone",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritStone_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/madstone",
            Url = "https://us.battle.net/d3/en/item/madstone",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p1_unique_spiritstone_008_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/madstone",
            IsCrafted = false,
            LegendaryAffix = "你的七相拳具有爆裂掌效果。",
            SetName = "",
        };

        /// <summary>
        /// Tzo Krin's Gaze Wave of Light is now cast at your enemy.
        /// </summary>
        public static Item TzoKrinsGaze = new Item
        {
            Id = 222305,
            Name = "祖科林的凝视",
            Quality = ItemQuality.Legendary,
            Slug = "tzo-krins-gaze",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritStone_norm_unique_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/tzo-krins-gaze",
            Url = "https://us.battle.net/d3/en/item/tzo-krins-gaze",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spiritstone_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tzo-krins-gaze",
            IsCrafted = false,
            LegendaryAffix = "金钟破现在可对你的敌人施放。",
            SetName = "",
        };

        /// <summary>
        /// Inna's Radiance 
        /// </summary>
        public static Item InnasRadiance = new Item
        {
            Id = 222307,
            Name = "尹娜的光华",
            Quality = ItemQuality.Legendary,
            Slug = "innas-radiance",
            ItemType = ItemType.SpiritStone,
            TrinityItemType = TrinityItemType.SpiritStone,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "spiritStone_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/innas-radiance",
            Url = "https://us.battle.net/d3/en/item/innas-radiance",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spiritstone_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/innas-radiance",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "尹娜的真言",
        };

        /// <summary>
        /// Split Tusk 
        /// </summary>
        public static Item SplitTusk = new Item
        {
            Id = 221167,
            Name = "开叉獠牙",
            Quality = ItemQuality.Legendary,
            Slug = "split-tusk",
            ItemType = ItemType.VoodooMask,
            TrinityItemType = TrinityItemType.VoodooMask,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "voodooMask_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/split-tusk",
            Url = "https://us.battle.net/d3/en/item/split-tusk",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_voodoomask_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/split-tusk",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Quetzalcoatl Locust Swarm and Haunt now deal their damage in half of the normal duration.
        /// </summary>
        public static Item Quetzalcoatl = new Item
        {
            Id = 204136,
            Name = "羽蛇神面",
            Quality = ItemQuality.Legendary,
            Slug = "quetzalcoatl",
            ItemType = ItemType.VoodooMask,
            TrinityItemType = TrinityItemType.VoodooMask,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "voodooMask_norm_base_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/quetzalcoatl",
            Url = "https://us.battle.net/d3/en/item/quetzalcoatl",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_voodoomask_005_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/quetzalcoatl",
            IsCrafted = false,
            LegendaryAffix = "瘟疫虫群与蚀魂造成伤害的持续时间减半。",
            SetName = "",
        };

        /// <summary>
        /// Carnevil The 5 Fetishes closest to you will shoot a powerful Poison Dart every time you do.
        /// </summary>
        public static Item Carnevil = new Item
        {
            Id = 299442,
            Name = "邪毒狂欢",
            Quality = ItemQuality.Legendary,
            Slug = "carnevil",
            ItemType = ItemType.VoodooMask,
            TrinityItemType = TrinityItemType.VoodooMask,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "voodoomask_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/carnevil",
            Url = "https://us.battle.net/d3/en/item/carnevil",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_voodoomask_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/carnevil",
            IsCrafted = false,
            LegendaryAffix = "每当你射出一发剧毒飞镖时，离你最近的 5 个鬼娃也会射出一发毒镖。",
            SetName = "",
        };

        /// <summary>
        /// Mask of Jeram Pets deal 75–100 % increased damage.
        /// </summary>
        public static Item MaskOfJeram = new Item
        {
            Id = 299443,
            Name = "杰拉姆的面具",
            Quality = ItemQuality.Legendary,
            Slug = "mask-of-jeram",
            ItemType = ItemType.VoodooMask,
            TrinityItemType = TrinityItemType.VoodooMask,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/mask-of-jeram",
            Url = "https://us.battle.net/d3/en/item/mask-of-jeram",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_voodoomask_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/mask-of-jeram",
            IsCrafted = false,
            LegendaryAffix = "宠物造成的伤害提高 (150-200)%。",
            SetName = "",
        };

        /// <summary>
        /// The Grin Reaper Chance when attacking to summon horrific Mimics that cast some of your equipped skills.
        /// </summary>
        public static Item TheGrinReaper = new Item
        {
            Id = 221166,
            Name = "狞笑死神",
            Quality = ItemQuality.Legendary,
            Slug = "the-grin-reaper",
            ItemType = ItemType.VoodooMask,
            TrinityItemType = TrinityItemType.VoodooMask,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "voodooMask_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-grin-reaper",
            Url = "https://us.battle.net/d3/en/item/the-grin-reaper",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_voodoomask_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-grin-reaper",
            IsCrafted = false,
            LegendaryAffix = "攻击时有一定几率召唤恐怖分身，它们会施放一些你装备的技能。",
            SetName = "",
        };

        /// <summary>
        /// Tiklandian Visage Horrify causes you to Fear and Root enemies around you for 6–8 seconds.
        /// </summary>
        public static Item TiklandianVisage = new Item
        {
            Id = 221382,
            Name = "提克兰凶相",
            Quality = ItemQuality.Legendary,
            Slug = "tiklandian-visage",
            ItemType = ItemType.VoodooMask,
            TrinityItemType = TrinityItemType.VoodooMask,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "voodooMask_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/tiklandian-visage",
            Url = "https://us.battle.net/d3/en/item/tiklandian-visage",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_voodoomask_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tiklandian-visage",
            IsCrafted = false,
            LegendaryAffix = "惧灵会恐惧并定身你周围的敌人，持续 (6-8) 秒。",
            SetName = "",
        };

        /// <summary>
        /// Visage of Giyua Summon a Fetish Army after you kill 2 Elites.
        /// </summary>
        public static Item VisageOfGiyua = new Item
        {
            Id = 221168,
            Name = "姬月之面",
            Quality = ItemQuality.Legendary,
            Slug = "visage-of-giyua",
            ItemType = ItemType.VoodooMask,
            TrinityItemType = TrinityItemType.VoodooMask,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "voodooMask_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/visage-of-giyua",
            Url = "https://us.battle.net/d3/en/item/visage-of-giyua",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_voodoomask_008_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/visage-of-giyua",
            IsCrafted = false,
            LegendaryAffix = "在你消灭2个精英敌人之后，召唤一支鬼娃大军。",
            SetName = "",
        };

        /// <summary>
        /// Zunimassa's Vision 
        /// </summary>
        public static Item ZunimassasVision = new Item
        {
            Id = 221202,
            Name = "祖尼玛萨之视",
            Quality = ItemQuality.Legendary,
            Slug = "zunimassas-vision",
            ItemType = ItemType.VoodooMask,
            TrinityItemType = TrinityItemType.VoodooMask,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "voodooMask_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/zunimassas-vision",
            Url = "https://us.battle.net/d3/en/item/zunimassas-vision",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_voodoomask_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/zunimassas-vision",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "祖尼玛萨之魂",
        };

        /// <summary>
        /// Cloak of Deception Enemy missiles sometimes pass through you harmlessly.
        /// </summary>
        public static Item CloakOfDeception = new Item
        {
            Id = 332208,
            Name = "幻影斗篷",
            Quality = ItemQuality.Legendary,
            Slug = "cloak-of-deception",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/cloak-of-deception",
            Url = "https://us.battle.net/d3/en/item/cloak-of-deception",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_cloak_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/cloak-of-deception",
            IsCrafted = false,
            LegendaryAffix = "敌人的飞弹有时会毫发无伤地穿过你。",
            SetName = "",
        };

        /// <summary>
        /// Beckon Sail When receiving fatal damage, you instead automatically cast Smoke Screen and are healed to 25% Life. This effect may occur once every 120 seconds.
        /// </summary>
        public static Item BeckonSail = new Item
        {
            Id = 223150,
            Name = "远行的召唤",
            Quality = ItemQuality.Legendary,
            Slug = "beckon-sail",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Cloak_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/beckon-sail",
            Url = "https://us.battle.net/d3/en/item/beckon-sail",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_cloak_005_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/beckon-sail",
            IsCrafted = false,
            LegendaryAffix = "当受到致命伤害时，你会自动施放烟雾弹，并恢复至25%的生命值。该效果每 120 秒只能生效一次。",
            SetName = "",
        };

        /// <summary>
        /// Blackfeather Dodging or getting hit by a ranged attack automatically shoots a homing rocket back at the attacker for 600–800 % weapon damage as Physical.
        /// </summary>
        public static Item Blackfeather = new Item
        {
            Id = 332206,
            Name = "黑羽",
            Quality = ItemQuality.Legendary,
            Slug = "blackfeather",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "cloak_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/blackfeather",
            Url = "https://us.battle.net/d3/en/item/blackfeather",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_cloak_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blackfeather",
            IsCrafted = false,
            LegendaryAffix = "成功躲闪或被远程攻击命中时，会朝攻击者自动射出一发追踪导弹，造成 (600-800)% 的武器伤害（作为物理伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Cape of the Dark Night Automatically drop Caltrops when you are hit. This effect may only occur once every 6 seconds.
        /// </summary>
        public static Item CapeOfTheDarkNight = new Item
        {
            Id = 223149,
            Name = "暗夜斗篷",
            Quality = ItemQuality.Legendary,
            Slug = "cape-of-the-dark-night",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Cloak_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/cape-of-the-dark-night",
            Url = "https://us.battle.net/d3/en/item/cape-of-the-dark-night",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_cloak_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/cape-of-the-dark-night",
            IsCrafted = false,
            LegendaryAffix = "当你被击中时自动丢出铁蒺藜。该效果每6秒只能生效一次。",
            SetName = "",
        };

        /// <summary>
        /// The Cloak of the Garwulf Companion - Wolf Companion now summons 3 wolves.
        /// </summary>
        public static Item TheCloakOfTheGarwulf = new Item
        {
            Id = 223151,
            Name = "加沃夫的披风",
            Quality = ItemQuality.Legendary,
            Slug = "the-cloak-of-the-garwulf",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Cloak_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-cloak-of-the-garwulf",
            Url = "https://us.battle.net/d3/en/item/the-cloak-of-the-garwulf",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_cloak_002_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-cloak-of-the-garwulf",
            IsCrafted = false,
            LegendaryAffix = "召唤恶狼战宠时会召唤出3只恶狼。",
            SetName = "",
        };

        /// <summary>
        /// Natalya's Embrace 
        /// </summary>
        public static Item NatalyasEmbrace = new Item
        {
            Id = 208934,
            Name = "娜塔亚的拥抱",
            Quality = ItemQuality.Legendary,
            Slug = "natalyas-embrace",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Cloak_norm_set_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/natalyas-embrace",
            Url = "https://us.battle.net/d3/en/item/natalyas-embrace",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_cloak_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/natalyas-embrace",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "娜塔亚的复仇",
        };

        /// <summary>
        /// Cage of the Hellborn 
        /// </summary>
        public static Item CageOfTheHellborn = new Item
        {
            Id = 408871,
            Name = "困魔笼甲",
            Quality = ItemQuality.Legendary,
            Slug = "cage-of-the-hellborn",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/cage-of-the-hellborn",
            Url = "https://us.battle.net/d3/en/item/cage-of-the-hellborn",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_03_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/cage-of-the-hellborn",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "邪秽之精",
        };

        /// <summary>
        /// Leoric's Crown Increase the effect of any gem socketed into this item by 75–100 % . This effect does not apply to Legendary Gems.
        /// </summary>
        public static Item LeoricsCrown = new Item
        {
            Id = 196024,
            Name = "李奥瑞克的王冠",
            Quality = ItemQuality.Legendary,
            Slug = "leorics-crown",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Helm_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/leorics-crown",
            Url = "https://us.battle.net/d3/en/item/leorics-crown",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_002_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/leorics-crown",
            IsCrafted = false,
            LegendaryAffix = "使镶入这件物品中的任何宝石的作用提高 (75-100)%。该效果对传奇宝石无效。",
            SetName = "",
        };

        /// <summary>
        /// Pride's Fall Your resource costs are reduced by 30% after not taking damage for 5 seconds.
        /// </summary>
        public static Item PridesFall = new Item
        {
            Id = 298147,
            Name = "骄矜必败",
            Quality = ItemQuality.Legendary,
            Slug = "prides-fall",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/prides-fall",
            Url = "https://us.battle.net/d3/en/item/prides-fall",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/prides-fall",
            IsCrafted = false,
            LegendaryAffix = "当你在过去5秒没有受到伤害时，你的能量消耗降低30%。",
            SetName = "",
        };

        /// <summary>
        /// Broken Crown Whenever a gem drops, a gem of the type socketed into your helmet also drops. This effect does not apply to Legendary Gems.
        /// </summary>
        public static Item BrokenCrown = new Item
        {
            Id = 220630,
            Name = "破碎的王冠",
            Quality = ItemQuality.Legendary,
            Slug = "broken-crown",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Helm_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/broken-crown",
            Url = "https://us.battle.net/d3/en/item/broken-crown",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_helm_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/broken-crown",
            IsCrafted = false,
            LegendaryAffix = "每当掉落一枚宝石时，还会掉落一枚已镶嵌入你头盔的同类型宝石。该效果对传奇宝石无效。",
            SetName = "",
        };

        /// <summary>
        /// Blind Faith 
        /// </summary>
        public static Item BlindFaith = new Item
        {
            Id = 197037,
            Name = "盲信面甲",
            Quality = ItemQuality.Legendary,
            Slug = "blind-faith",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/blind-faith",
            Url = "https://us.battle.net/d3/en/item/blind-faith",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blind-faith",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Deathseer's Cowl 15–20 % chance on being hit by an Undead enemy to charm it for 2 seconds.
        /// </summary>
        public static Item DeathseersCowl = new Item
        {
            Id = 298146,
            Name = "死亡先知兜帽",
            Quality = ItemQuality.Legendary,
            Slug = "deathseers-cowl",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_unique_14",
            DataUrl = "https://us.battle.net/api/d3/data/item/deathseers-cowl",
            Url = "https://us.battle.net/d3/en/item/deathseers-cowl",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/deathseers-cowl",
            IsCrafted = false,
            LegendaryAffix = "被亡灵敌人击中时有 (15-20)% 的几率魅惑该敌人2秒。",
            SetName = "",
        };

        /// <summary>
        /// Visage of Gunes Vengeance gains the effect of the Dark Heart rune.
        /// </summary>
        public static Item VisageOfGunes = new Item
        {
            Id = 429266,
            Name = "古内斯之面",
            Quality = ItemQuality.Legendary,
            Slug = "visage-of-gunes",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/visage-of-gunes",
            Url = "https://us.battle.net/d3/en/item/visage-of-gunes",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_helm_103_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/visage-of-gunes",
            IsCrafted = false,
            LegendaryAffix = "复仇获得黑暗之心符文效果。",
            SetName = "",
        };

        /// <summary>
        /// Warhelm of Kassar Reduce the cooldown and increase the damage of Phalanx by 45–60 % .
        /// </summary>
        public static Item WarhelmOfKassar = new Item
        {
            Id = 426784,
            Name = "卡萨的战盔",
            Quality = ItemQuality.Legendary,
            Slug = "warhelm-of-kassar",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/warhelm-of-kassar",
            Url = "https://us.battle.net/d3/en/item/warhelm-of-kassar",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_helm_102_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/warhelm-of-kassar",
            IsCrafted = false,
            LegendaryAffix = "使斗阵的冷却时间和伤害分别减少和提高 (45-60)%。",
            SetName = "",
        };

        /// <summary>
        /// Mask of Scarlet Death Revive now consumes all corpses to raise a minion that deals 125–150 % more damage per corpse.
        /// </summary>
        public static Item MaskOfScarletDeath = new Item
        {
            Id = 467604,
            Name = "血腥面罩",
            Quality = ItemQuality.Legendary,
            Slug = "mask-of-scarlet-death",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/mask-of-scarlet-death",
            Url = "https://us.battle.net/d3/en/item/mask-of-scarlet-death",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_helm_21_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/mask-of-scarlet-death",
            IsCrafted = false,
            LegendaryAffix = "复生现在会消耗所有尸骸的精魂，并召唤出一个仆从。每消耗一具尸骸的精魂可以使该仆从的伤害提高(125-150)%。",
            SetName = "",
        };

        /// <summary>
        /// Skull of Resonance Threatening Shout has a chance to Charm enemies and cause them to join your side.
        /// </summary>
        public static Item SkullOfResonance = new Item
        {
            Id = 220549,
            Name = "共鸣之颅",
            Quality = ItemQuality.Legendary,
            Slug = "skull-of-resonance",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Helm_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/skull-of-resonance",
            Url = "https://us.battle.net/d3/en/item/skull-of-resonance",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_004_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/skull-of-resonance",
            IsCrafted = false,
            LegendaryAffix = "威吓呐喊有一定几率魅惑敌人，并使其为你战斗。",
            SetName = "",
        };

        /// <summary>
        /// Andariel's Visage Attacks release a Poison Nova that deals 350–450 % weapon damage as Poison to enemies within 10 yards.
        /// </summary>
        public static Item AndarielsVisage = new Item
        {
            Id = 198014,
            Name = "安达莉尔的仪容",
            Quality = ItemQuality.Legendary,
            Slug = "andariels-visage",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Helm_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/andariels-visage",
            Url = "https://us.battle.net/d3/en/item/andariels-visage",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_003_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/andariels-visage",
            IsCrafted = false,
            LegendaryAffix = "攻击会释放出剧毒新星，对10码内的敌人造成 (350-450)% 的武器伤害（作为毒性伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Fate's Vow Army of the Dead gains the effect of the Unconventional Warfare rune.
        /// </summary>
        public static Item FatesVow = new Item
        {
            Id = 467605,
            Name = "命运之誓",
            Quality = ItemQuality.Legendary,
            Slug = "fates-vow",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/fates-vow",
            Url = "https://us.battle.net/d3/en/item/fates-vow",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_helm_22_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fates-vow",
            IsCrafted = false,
            LegendaryAffix = "亡者大军获得异界大军符文效果，并且伤害提高 200-250%。",
            SetName = "",
        };

        /// <summary>
        /// Mempo of Twilight 
        /// </summary>
        public static Item MempoOfTwilight = new Item
        {
            Id = 223577,
            Name = "暮光面甲",
            Quality = ItemQuality.Legendary,
            Slug = "mempo-of-twilight",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Helm_norm_unique_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/mempo-of-twilight",
            Url = "https://us.battle.net/d3/en/item/mempo-of-twilight",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/mempo-of-twilight",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Immortal King's Triumph 
        /// </summary>
        public static Item ImmortalKingsTriumph = new Item
        {
            Id = 210265,
            Name = "不朽之王的胜利",
            Quality = ItemQuality.Legendary,
            Slug = "immortal-kings-triumph",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Helm_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/immortal-kings-triumph",
            Url = "https://us.battle.net/d3/en/item/immortal-kings-triumph",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_008_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/immortal-kings-triumph",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不朽之王的呼唤",
        };

        /// <summary>
        /// Natalya's Sight 
        /// </summary>
        public static Item NatalyasSight = new Item
        {
            Id = 210851,
            Name = "娜塔亚的视界",
            Quality = ItemQuality.Legendary,
            Slug = "natalyas-sight",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/natalyas-sight",
            Url = "https://us.battle.net/d3/en/item/natalyas-sight",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/natalyas-sight",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "娜塔亚的复仇",
        };

        /// <summary>
        /// Tal Rasha's Guise of Wisdom 
        /// </summary>
        public static Item TalRashasGuiseOfWisdom = new Item
        {
            Id = 211531,
            Name = "塔·拉夏的智慧面甲",
            Quality = ItemQuality.Legendary,
            Slug = "tal-rashas-guise-of-wisdom",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Helm_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/tal-rashas-guise-of-wisdom",
            Url = "https://us.battle.net/d3/en/item/tal-rashas-guise-of-wisdom",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_010_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tal-rashas-guise-of-wisdom",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔·拉夏的法理",
        };

        /// <summary>
        /// Accursed Visage 
        /// </summary>
        public static Item AccursedVisage = new Item
        {
            Id = 414753,
            Name = "天谴仪容",
            Quality = ItemQuality.Legendary,
            Slug = "accursed-visage",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/accursed-visage",
            Url = "https://us.battle.net/d3/en/item/accursed-visage",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_03_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/accursed-visage",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "邪秽之精",
        };

        /// <summary>
        /// Arachyr's Visage 
        /// </summary>
        public static Item ArachyrsVisage = new Item
        {
            Id = 441178,
            Name = "亚拉基尔的仪容",
            Quality = ItemQuality.Legendary,
            Slug = "arachyrs-visage",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/arachyrs-visage",
            Url = "https://us.battle.net/d3/en/item/arachyrs-visage",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_02_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/arachyrs-visage",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "亚拉基尔的灵魂",
        };

        /// <summary>
        /// Crown of the Invoker 
        /// </summary>
        public static Item CrownOfTheInvoker = new Item
        {
            Id = 335028,
            Name = "唤魔师的法冠",
            Quality = ItemQuality.Legendary,
            Slug = "crown-of-the-invoker",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_set_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/crown-of-the-invoker",
            Url = "https://us.battle.net/d3/en/item/crown-of-the-invoker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_12_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/crown-of-the-invoker",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "唤魔师的荆棘",
        };

        /// <summary>
        /// Crown of the Light 
        /// </summary>
        public static Item CrownOfTheLight = new Item
        {
            Id = 414930,
            Name = "圣光之冠",
            Quality = ItemQuality.Legendary,
            Slug = "crown-of-the-light",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/crown-of-the-light",
            Url = "https://us.battle.net/d3/en/item/crown-of-the-light",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_03_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/crown-of-the-light",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "圣光追寻者",
        };

        /// <summary>
        /// Eyes of the Earth 
        /// </summary>
        public static Item EyesOfTheEarth = new Item
        {
            Id = 340528,
            Name = "大地之眼",
            Quality = ItemQuality.Legendary,
            Slug = "eyes-of-the-earth",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_set_15",
            DataUrl = "https://us.battle.net/api/d3/data/item/eyes-of-the-earth",
            Url = "https://us.battle.net/d3/en/item/eyes-of-the-earth",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_15_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/eyes-of-the-earth",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "大地之力",
        };

        /// <summary>
        /// Firebird's Plume 
        /// </summary>
        public static Item FirebirdsPlume = new Item
        {
            Id = 358791,
            Name = "不死鸟之冠",
            Quality = ItemQuality.Legendary,
            Slug = "firebirds-plume",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_set_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/firebirds-plume",
            Url = "https://us.battle.net/d3/en/item/firebirds-plume",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_06_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/firebirds-plume",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不死鸟的华服",
        };

        /// <summary>
        /// Helltooth Mask 
        /// </summary>
        public static Item HelltoothMask = new Item
        {
            Id = 369016,
            Name = "魔牙面具",
            Quality = ItemQuality.Legendary,
            Slug = "helltooth-mask",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_set_16",
            DataUrl = "https://us.battle.net/api/d3/data/item/helltooth-mask",
            Url = "https://us.battle.net/d3/en/item/helltooth-mask",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_16_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/helltooth-mask",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "魔牙战装",
        };

        /// <summary>
        /// Helm of Akkhan 
        /// </summary>
        public static Item HelmOfAkkhan = new Item
        {
            Id = 358799,
            Name = "阿克汉的头盔",
            Quality = ItemQuality.Legendary,
            Slug = "helm-of-akkhan",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/helm-of-akkhan",
            Url = "https://us.battle.net/d3/en/item/helm-of-akkhan",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_10_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/helm-of-akkhan",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "阿克汉的战甲",
        };

        /// <summary>
        /// Helm of the Wastes 
        /// </summary>
        public static Item HelmOfTheWastes = new Item
        {
            Id = 414926,
            Name = "荒原头盔",
            Quality = ItemQuality.Legendary,
            Slug = "helm-of-the-wastes",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/helm-of-the-wastes",
            Url = "https://us.battle.net/d3/en/item/helm-of-the-wastes",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_01_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/helm-of-the-wastes",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "废土之怒",
        };

        /// <summary>
        /// Inarius's Understanding 
        /// </summary>
        public static Item InariussUnderstanding = new Item
        {
            Id = 467602,
            Name = "伊纳瑞斯的领悟",
            Quality = ItemQuality.Legendary,
            Slug = "inariuss-understanding",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/inariuss-understanding",
            Url = "https://us.battle.net/d3/en/item/inariuss-understanding",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_3_helm_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/inariuss-understanding",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "伊纳瑞斯的恩泽",
        };

        /// <summary>
        /// Jade Harvester's Wisdom 
        /// </summary>
        public static Item JadeHarvestersWisdom = new Item
        {
            Id = 338040,
            Name = "玉魂师的智慧",
            Quality = ItemQuality.Legendary,
            Slug = "jade-harvesters-wisdom",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_set_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/jade-harvesters-wisdom",
            Url = "https://us.battle.net/d3/en/item/jade-harvesters-wisdom",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_09_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jade-harvesters-wisdom",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "玉魂师的战甲",
        };

        /// <summary>
        /// Marauder's Visage 
        /// </summary>
        public static Item MaraudersVisage = new Item
        {
            Id = 336994,
            Name = "掠夺者的仪容",
            Quality = ItemQuality.Legendary,
            Slug = "marauders-visage",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_set_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/marauders-visage",
            Url = "https://us.battle.net/d3/en/item/marauders-visage",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_07_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/marauders-visage",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "掠夺者的化身",
        };

        /// <summary>
        /// Mask of the Searing Sky 
        /// </summary>
        public static Item MaskOfTheSearingSky = new Item
        {
            Id = 338034,
            Name = "灼天之面",
            Quality = ItemQuality.Legendary,
            Slug = "mask-of-the-searing-sky",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_set_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/mask-of-the-searing-sky",
            Url = "https://us.battle.net/d3/en/item/mask-of-the-searing-sky",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_08_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/mask-of-the-searing-sky",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "千风飓",
        };

        /// <summary>
        /// Pestilence Mask 
        /// </summary>
        public static Item PestilenceMask = new Item
        {
            Id = 467603,
            Name = "死疫面具",
            Quality = ItemQuality.Legendary,
            Slug = "pestilence-mask",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/pestilence-mask",
            Url = "https://us.battle.net/d3/en/item/pestilence-mask",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_4_helm_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pestilence-mask",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "死疫死者的裹布",
        };

        /// <summary>
        /// Raekor's Will 
        /// </summary>
        public static Item RaekorsWill = new Item
        {
            Id = 336988,
            Name = "蕾蔻的意志",
            Quality = ItemQuality.Legendary,
            Slug = "raekors-will",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_set_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/raekors-will",
            Url = "https://us.battle.net/d3/en/item/raekors-will",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_05_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/raekors-will",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "蕾蔻的传世铠",
        };

        /// <summary>
        /// Rathma's Skull Helm 
        /// </summary>
        public static Item RathmasSkullHelm = new Item
        {
            Id = 460892,
            Name = "拉斯玛的骸骨战盔",
            Quality = ItemQuality.Legendary,
            Slug = "rathmas-skull-helm",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/rathmas-skull-helm",
            Url = "https://us.battle.net/d3/en/item/rathmas-skull-helm",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_1_helm_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rathmas-skull-helm",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "拉斯玛的骨甲",
        };

        /// <summary>
        /// Roland's Visage 
        /// </summary>
        public static Item RolandsVisage = new Item
        {
            Id = 404700,
            Name = "罗兰之面",
            Quality = ItemQuality.Legendary,
            Slug = "rolands-visage",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Helm_norm_base_flippy",
            DataUrl = "https://us.battle.net/api/d3/data/item/rolands-visage",
            Url = "https://us.battle.net/d3/en/item/rolands-visage",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_01_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rolands-visage",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "罗兰的传世甲",
        };

        /// <summary>
        /// Shrouded Mask 
        /// </summary>
        public static Item ShroudedMask = new Item
        {
            Id = 414927,
            Name = "雾隐面具",
            Quality = ItemQuality.Legendary,
            Slug = "shrouded-mask",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/shrouded-mask",
            Url = "https://us.battle.net/d3/en/item/shrouded-mask",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_02_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/shrouded-mask",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "德尔西尼的杰作",
        };

        /// <summary>
        /// Sunwuko's Crown 
        /// </summary>
        public static Item SunwukosCrown = new Item
        {
            Id = 336173,
            Name = "孙武空的箍冠",
            Quality = ItemQuality.Legendary,
            Slug = "sunwukos-crown",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "helm_norm_set_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/sunwukos-crown",
            Url = "https://us.battle.net/d3/en/item/sunwukos-crown",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_11_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sunwukos-crown",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "猴王战甲",
        };

        /// <summary>
        /// The Shadow's Mask 
        /// </summary>
        public static Item TheShadowsMask = new Item
        {
            Id = 443602,
            Name = "影面",
            Quality = ItemQuality.Legendary,
            Slug = "the-shadows-mask",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-shadows-mask",
            Url = "https://us.battle.net/d3/en/item/the-shadows-mask",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_14_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-shadows-mask",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "暗影装束",
        };

        /// <summary>
        /// Trag'Oul's Guise 
        /// </summary>
        public static Item TragoulsGuise = new Item
        {
            Id = 467601,
            Name = "塔格奥之首",
            Quality = ItemQuality.Legendary,
            Slug = "tragouls-guise",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/tragouls-guise",
            Url = "https://us.battle.net/d3/en/item/tragouls-guise",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_2_helm_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tragouls-guise",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔格奥的化身",
        };

        /// <summary>
        /// Uliana's Spirit 
        /// </summary>
        public static Item UlianasSpirit = new Item
        {
            Id = 414928,
            Name = "乌莲娜的精神",
            Quality = ItemQuality.Legendary,
            Slug = "ulianas-spirit",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ulianas-spirit",
            Url = "https://us.battle.net/d3/en/item/ulianas-spirit",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_01_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ulianas-spirit",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "乌莲娜的谋略",
        };

        /// <summary>
        /// Vyr's Sightless Skull 
        /// </summary>
        public static Item VyrsSightlessSkull = new Item
        {
            Id = 439183,
            Name = "维尔的盲眼颅盔",
            Quality = ItemQuality.Legendary,
            Slug = "vyrs-sightless-skull",
            ItemType = ItemType.Helm,
            TrinityItemType = TrinityItemType.Helm,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/vyrs-sightless-skull",
            Url = "https://us.battle.net/d3/en/item/vyrs-sightless-skull",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_helm_set_13_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vyrs-sightless-skull",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "维尔的神装",
        };

        /// <summary>
        /// Heart of Iron Gain Thorns equal to 250–300 % of your Vitality.
        /// </summary>
        public static Item HeartOfIron = new Item
        {
            Id = 205607,
            Name = "钢铁之心",
            Quality = ItemQuality.Legendary,
            Slug = "heart-of-iron",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestArmor_norm_unique_074",
            DataUrl = "https://us.battle.net/api/d3/data/item/heart-of-iron",
            Url = "https://us.battle.net/d3/en/item/heart-of-iron",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_chest_018_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/heart-of-iron",
            IsCrafted = false,
            LegendaryAffix = "获得相当于你体能值 (250-300)% 的荆棘伤害。",
            SetName = "",
        };

        /// <summary>
        /// Aquila Cuirass While above 90–95 % primary resource, all damage taken is reduced by 50% .
        /// </summary>
        public static Item AquilaCuirass = new Item
        {
            Id = 197203,
            Name = "天鹰斗衣",
            Quality = ItemQuality.Legendary,
            Slug = "aquila-cuirass",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_unique_047",
            DataUrl = "https://us.battle.net/api/d3/data/item/aquila-cuirass",
            Url = "https://us.battle.net/d3/en/item/aquila-cuirass",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_chest_012_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/aquila-cuirass",
            IsCrafted = false,
            LegendaryAffix = "主要能量高于 (90-95)% 时，受到的所有伤害减少 50%。",
            SetName = "",
        };

        /// <summary>
        /// Chaingmail After earning a survival bonus, quickly heal to full Life.
        /// </summary>
        public static Item Chaingmail = new Item
        {
            Id = 197204,
            Name = "锁钢甲",
            Quality = ItemQuality.Legendary,
            Slug = "chaingmail",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestArmor_norm_unique_048",
            DataUrl = "https://us.battle.net/api/d3/data/item/chaingmail",
            Url = "https://us.battle.net/d3/en/item/chaingmail",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_010_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/chaingmail",
            IsCrafted = false,
            LegendaryAffix = "在获得存活奖励后，会快速回满生命值。",
            SetName = "",
        };

        /// <summary>
        /// Cindercoat Reduces the resource cost of Fire skills by 23–30 % .
        /// </summary>
        public static Item Cindercoat = new Item
        {
            Id = 222455,
            Name = "燃火外套",
            Quality = ItemQuality.Legendary,
            Slug = "cindercoat",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/cindercoat",
            Url = "https://us.battle.net/d3/en/item/cindercoat",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/cindercoat",
            IsCrafted = false,
            LegendaryAffix = "使所有火焰技能的能量消耗降低 (23-30)%。",
            SetName = "",
        };

        /// <summary>
        /// Shi Mizu's Haori While below 20–25 % Life, all attacks are guaranteed Critical Hits.
        /// </summary>
        public static Item ShiMizusHaori = new Item
        {
            Id = 332200,
            Name = "清水羽织",
            Quality = ItemQuality.Legendary,
            Slug = "shi-mizus-haori",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/shi-mizus-haori",
            Url = "https://us.battle.net/d3/en/item/shi-mizus-haori",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/shi-mizus-haori",
            IsCrafted = false,
            LegendaryAffix = "当生命值低于 (20-25)% 时，所有攻击必定暴击。",
            SetName = "",
        };

        /// <summary>
        /// Goldskin Chance for enemies to drop gold when you hit them.
        /// </summary>
        public static Item Goldskin = new Item
        {
            Id = 205616,
            Name = "黄金之肤",
            Quality = ItemQuality.Legendary,
            Slug = "goldskin",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Gloves_norm_unique_078",
            DataUrl = "https://us.battle.net/api/d3/data/item/goldskin",
            Url = "https://us.battle.net/d3/en/item/goldskin",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/goldskin",
            IsCrafted = false,
            LegendaryAffix = "击中敌人时有一定几率使其掉落金币 (5%)。",
            SetName = "",
        };

        /// <summary>
        /// Tyrael's Might 
        /// </summary>
        public static Item TyraelsMight = new Item
        {
            Id = 205608,
            Name = "泰瑞尔之力",
            Quality = ItemQuality.Legendary,
            Slug = "tyraels-might",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestArmor_norm_unique_075",
            DataUrl = "https://us.battle.net/api/d3/data/item/tyraels-might",
            Url = "https://us.battle.net/d3/en/item/tyraels-might",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tyraels-might",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Blackthorne's Surcoat 
        /// </summary>
        public static Item BlackthornesSurcoat = new Item
        {
            Id = 222456,
            Name = "黑棘的战袍",
            Quality = ItemQuality.Legendary,
            Slug = "blackthornes-surcoat",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestArmor_norm_unique_050",
            DataUrl = "https://us.battle.net/api/d3/data/item/blackthornes-surcoat",
            Url = "https://us.battle.net/d3/en/item/blackthornes-surcoat",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chestarmor_028_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blackthornes-surcoat",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "黑棘的战铠",
        };

        /// <summary>
        /// Immortal King's Eternal Reign 
        /// </summary>
        public static Item ImmortalKingsEternalReign = new Item
        {
            Id = 205613,
            Name = "不朽之王的永恒统治",
            Quality = ItemQuality.Legendary,
            Slug = "immortal-kings-eternal-reign",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestArmor_norm_unique_086",
            DataUrl = "https://us.battle.net/api/d3/data/item/immortal-kings-eternal-reign",
            Url = "https://us.battle.net/d3/en/item/immortal-kings-eternal-reign",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_013_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/immortal-kings-eternal-reign",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不朽之王的呼唤",
        };

        /// <summary>
        /// Inna's Vast Expanse 
        /// </summary>
        public static Item InnasVastExpanse = new Item
        {
            Id = 205614,
            Name = "尹娜的寰宇胸襟",
            Quality = ItemQuality.Legendary,
            Slug = "innas-vast-expanse",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestArmor_norm_unique_087",
            DataUrl = "https://us.battle.net/api/d3/data/item/innas-vast-expanse",
            Url = "https://us.battle.net/d3/en/item/innas-vast-expanse",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_015_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/innas-vast-expanse",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "尹娜的真言",
        };

        /// <summary>
        /// Tal Rasha's Relentless Pursuit 
        /// </summary>
        public static Item TalRashasRelentlessPursuit = new Item
        {
            Id = 211626,
            Name = "塔·拉夏的无情追捕",
            Quality = ItemQuality.Legendary,
            Slug = "tal-rashas-relentless-pursuit",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestArmor_norm_set_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/tal-rashas-relentless-pursuit",
            Url = "https://us.battle.net/d3/en/item/tal-rashas-relentless-pursuit",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_014_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tal-rashas-relentless-pursuit",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔·拉夏的法理",
        };

        /// <summary>
        /// Zunimassa's Marrow 
        /// </summary>
        public static Item ZunimassasMarrow = new Item
        {
            Id = 205615,
            Name = "祖尼玛萨之髓",
            Quality = ItemQuality.Legendary,
            Slug = "zunimassas-marrow",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/zunimassas-marrow",
            Url = "https://us.battle.net/d3/en/item/zunimassas-marrow",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_016_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/zunimassas-marrow",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "祖尼玛萨之魂",
        };

        /// <summary>
        /// Bloodsong Mail While in the Land of the Dead, Command Skeletons gains the effect of all runes and deals 100–125 % additional damage.
        /// </summary>
        public static Item BloodsongMail = new Item
        {
            Id = 467568,
            Name = "血歌锁甲",
            Quality = ItemQuality.Legendary,
            Slug = "bloodsong-mail",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bloodsong-mail",
            Url = "https://us.battle.net/d3/en/item/bloodsong-mail",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_chest_21_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bloodsong-mail",
            IsCrafted = false,
            LegendaryAffix = "亡者领域期间，号令骸骨获得骷髅打手、黑暗愈合、死寒之握、狂怒者符文效果并造成额外 100-125% 伤害。",
            SetName = "",
        };

        /// <summary>
        /// Armor of the Kind Regent Smite will now also be cast at a second nearby enemy.
        /// </summary>
        public static Item ArmorOfTheKindRegent = new Item
        {
            Id = 332202,
            Name = "摄政仁君之铠",
            Quality = ItemQuality.Legendary,
            Slug = "armor-of-the-kind-regent",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/armor-of-the-kind-regent",
            Url = "https://us.battle.net/d3/en/item/armor-of-the-kind-regent",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/armor-of-the-kind-regent",
            IsCrafted = false,
            LegendaryAffix = "链击现在会对附近的第二个敌人施放。",
            SetName = "",
        };

        /// <summary>
        /// Requiem Cereplate Devour restores an additional 75–100 % Essence and Life. In addition, when Devour restores Essence or Life above your maximum, the excess is granted over 3 seconds.
        /// </summary>
        public static Item RequiemCereplate = new Item
        {
            Id = 467569,
            Name = "安魂胸甲",
            Quality = ItemQuality.Legendary,
            Slug = "requiem-cereplate",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/requiem-cereplate",
            Url = "https://us.battle.net/d3/en/item/requiem-cereplate",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_chest_22_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/requiem-cereplate",
            IsCrafted = false,
            LegendaryAffix = "吞噬可以为你恢复额外（75-100）%的精魂和生命值。此外，当吞噬可以为你恢复过量精魂或生命值时，你在3秒钟内可以保留这些过量的精魂或生命值。",
            SetName = "",
        };

        /// <summary>
        /// Arachyr's Carapace 
        /// </summary>
        public static Item ArachyrsCarapace = new Item
        {
            Id = 441191,
            Name = "亚拉基尔的甲壳",
            Quality = ItemQuality.Legendary,
            Slug = "arachyrs-carapace",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/arachyrs-carapace",
            Url = "https://us.battle.net/d3/en/item/arachyrs-carapace",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_02_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/arachyrs-carapace",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "亚拉基尔的灵魂",
        };

        /// <summary>
        /// Breastplate of Akkhan 
        /// </summary>
        public static Item BreastplateOfAkkhan = new Item
        {
            Id = 358796,
            Name = "阿克汉的胸甲",
            Quality = ItemQuality.Legendary,
            Slug = "breastplate-of-akkhan",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_set_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/breastplate-of-akkhan",
            Url = "https://us.battle.net/d3/en/item/breastplate-of-akkhan",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_10_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/breastplate-of-akkhan",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "阿克汉的战甲",
        };

        /// <summary>
        /// Cuirass of the Wastes 
        /// </summary>
        public static Item CuirassOfTheWastes = new Item
        {
            Id = 408860,
            Name = "荒原胸甲",
            Quality = ItemQuality.Legendary,
            Slug = "cuirass-of-the-wastes",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/cuirass-of-the-wastes",
            Url = "https://us.battle.net/d3/en/item/cuirass-of-the-wastes",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_01_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/cuirass-of-the-wastes",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "废土之怒",
        };

        /// <summary>
        /// Firebird's Breast 
        /// </summary>
        public static Item FirebirdsBreast = new Item
        {
            Id = 358788,
            Name = "不死鸟之胸",
            Quality = ItemQuality.Legendary,
            Slug = "firebirds-breast",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_set_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/firebirds-breast",
            Url = "https://us.battle.net/d3/en/item/firebirds-breast",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_06_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/firebirds-breast",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不死鸟的华服",
        };

        /// <summary>
        /// Harness of Truth 
        /// </summary>
        public static Item HarnessOfTruth = new Item
        {
            Id = 408868,
            Name = "真理甲胄",
            Quality = ItemQuality.Legendary,
            Slug = "harness-of-truth",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/harness-of-truth",
            Url = "https://us.battle.net/d3/en/item/harness-of-truth",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_02_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/harness-of-truth",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "德尔西尼的杰作",
        };

        /// <summary>
        /// Heart of the Crashing Wave 
        /// </summary>
        public static Item HeartOfTheCrashingWave = new Item
        {
            Id = 338032,
            Name = "怒涛之心",
            Quality = ItemQuality.Legendary,
            Slug = "heart-of-the-crashing-wave",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_set_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/heart-of-the-crashing-wave",
            Url = "https://us.battle.net/d3/en/item/heart-of-the-crashing-wave",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_08_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/heart-of-the-crashing-wave",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "千风飓",
        };

        /// <summary>
        /// Heart of the Light 
        /// </summary>
        public static Item HeartOfTheLight = new Item
        {
            Id = 408872,
            Name = "圣光之心",
            Quality = ItemQuality.Legendary,
            Slug = "heart-of-the-light",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/heart-of-the-light",
            Url = "https://us.battle.net/d3/en/item/heart-of-the-light",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_03_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/heart-of-the-light",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "圣光追寻者",
        };

        /// <summary>
        /// Helltooth Tunic 
        /// </summary>
        public static Item HelltoothTunic = new Item
        {
            Id = 363088,
            Name = "魔牙外衣",
            Quality = ItemQuality.Legendary,
            Slug = "helltooth-tunic",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_set_16",
            DataUrl = "https://us.battle.net/api/d3/data/item/helltooth-tunic",
            Url = "https://us.battle.net/d3/en/item/helltooth-tunic",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_16_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/helltooth-tunic",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "魔牙战装",
        };

        /// <summary>
        /// Inarius's Conviction 
        /// </summary>
        public static Item InariussConviction = new Item
        {
            Id = 467567,
            Name = "伊纳瑞斯的信念",
            Quality = ItemQuality.Legendary,
            Slug = "inariuss-conviction",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/inariuss-conviction",
            Url = "https://us.battle.net/d3/en/item/inariuss-conviction",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_3_chest_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/inariuss-conviction",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "伊纳瑞斯的恩泽",
        };

        /// <summary>
        /// Jade Harvester's Peace 
        /// </summary>
        public static Item JadeHarvestersPeace = new Item
        {
            Id = 338038,
            Name = "玉魂师的平和",
            Quality = ItemQuality.Legendary,
            Slug = "jade-harvesters-peace",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_set_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/jade-harvesters-peace",
            Url = "https://us.battle.net/d3/en/item/jade-harvesters-peace",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_09_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jade-harvesters-peace",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "玉魂师的战甲",
        };

        /// <summary>
        /// Marauder's Carapace 
        /// </summary>
        public static Item MaraudersCarapace = new Item
        {
            Id = 363803,
            Name = "掠夺者的甲壳",
            Quality = ItemQuality.Legendary,
            Slug = "marauders-carapace",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_set_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/marauders-carapace",
            Url = "https://us.battle.net/d3/en/item/marauders-carapace",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_07_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/marauders-carapace",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "掠夺者的化身",
        };

        /// <summary>
        /// Pestilence Robe 
        /// </summary>
        public static Item PestilenceRobe = new Item
        {
            Id = 467351,
            Name = "死疫裹袍",
            Quality = ItemQuality.Legendary,
            Slug = "pestilence-robe",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/pestilence-robe",
            Url = "https://us.battle.net/d3/en/item/pestilence-robe",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_4_chest_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pestilence-robe",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "死疫死者的裹布",
        };

        /// <summary>
        /// Raekor's Heart 
        /// </summary>
        public static Item RaekorsHeart = new Item
        {
            Id = 336984,
            Name = "蕾蔻的衷情",
            Quality = ItemQuality.Legendary,
            Slug = "raekors-heart",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_set_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/raekors-heart",
            Url = "https://us.battle.net/d3/en/item/raekors-heart",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_05_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/raekors-heart",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "蕾蔻的传世铠",
        };

        /// <summary>
        /// Rathma's Ribcage Plate 
        /// </summary>
        public static Item RathmasRibcagePlate = new Item
        {
            Id = 460919,
            Name = "拉斯玛的白骨装甲",
            Quality = ItemQuality.Legendary,
            Slug = "rathmas-ribcage-plate",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/rathmas-ribcage-plate",
            Url = "https://us.battle.net/d3/en/item/rathmas-ribcage-plate",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_1_chest_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rathmas-ribcage-plate",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "拉斯玛的骨甲",
        };

        /// <summary>
        /// Roland's Bearing 
        /// </summary>
        public static Item RolandsBearing = new Item
        {
            Id = 404095,
            Name = "罗兰之胸",
            Quality = ItemQuality.Legendary,
            Slug = "rolands-bearing",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestArmor_norm_base_flippy",
            DataUrl = "https://us.battle.net/api/d3/data/item/rolands-bearing",
            Url = "https://us.battle.net/d3/en/item/rolands-bearing",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_01_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rolands-bearing",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "罗兰的传世甲",
        };

        /// <summary>
        /// Spirit of the Earth 
        /// </summary>
        public static Item SpiritOfTheEarth = new Item
        {
            Id = 442474,
            Name = "大地之灵",
            Quality = ItemQuality.Legendary,
            Slug = "spirit-of-the-earth",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/spirit-of-the-earth",
            Url = "https://us.battle.net/d3/en/item/spirit-of-the-earth",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_15_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/spirit-of-the-earth",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "大地之力",
        };

        /// <summary>
        /// Sunwuko's Soul 
        /// </summary>
        public static Item SunwukosSoul = new Item
        {
            Id = 429167,
            Name = "孙武空的灵魂",
            Quality = ItemQuality.Legendary,
            Slug = "sunwukos-soul",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/sunwukos-soul",
            Url = "https://us.battle.net/d3/en/item/sunwukos-soul",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_11_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sunwukos-soul",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "猴王战甲",
        };

        /// <summary>
        /// The Shadow's Bane 
        /// </summary>
        public static Item TheShadowsBane = new Item
        {
            Id = 332359,
            Name = "影劫",
            Quality = ItemQuality.Legendary,
            Slug = "the-shadows-bane",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_set_14",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-shadows-bane",
            Url = "https://us.battle.net/d3/en/item/the-shadows-bane",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_14_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-shadows-bane",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "暗影装束",
        };

        /// <summary>
        /// Trag'Oul's Scales 
        /// </summary>
        public static Item TragoulsScales = new Item
        {
            Id = 467566,
            Name = "塔格奥之鳞",
            Quality = ItemQuality.Legendary,
            Slug = "tragouls-scales",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/tragouls-scales",
            Url = "https://us.battle.net/d3/en/item/tragouls-scales",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_2_chest_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tragouls-scales",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔格奥的化身",
        };

        /// <summary>
        /// Uliana's Heart 
        /// </summary>
        public static Item UlianasHeart = new Item
        {
            Id = 408869,
            Name = "乌莲娜的心境",
            Quality = ItemQuality.Legendary,
            Slug = "ulianas-heart",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ulianas-heart",
            Url = "https://us.battle.net/d3/en/item/ulianas-heart",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_01_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ulianas-heart",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "乌莲娜的谋略",
        };

        /// <summary>
        /// Vyr's Astonishing Aura 
        /// </summary>
        public static Item VyrsAstonishingAura = new Item
        {
            Id = 332357,
            Name = "维尔的惊人气场",
            Quality = ItemQuality.Legendary,
            Slug = "vyrs-astonishing-aura",
            ItemType = ItemType.Chest,
            TrinityItemType = TrinityItemType.Chest,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestarmor_norm_set_13",
            DataUrl = "https://us.battle.net/api/d3/data/item/vyrs-astonishing-aura",
            Url = "https://us.battle.net/d3/en/item/vyrs-astonishing-aura",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_chest_set_13_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vyrs-astonishing-aura",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "维尔的神装",
        };

        /// <summary>
        /// Ashnagarr's Blood Bracer Increases the potency of your shields by 75–100 % .
        /// </summary>
        public static Item AshnagarrsBloodBracer = new Item
        {
            Id = 193686,
            Name = "阿什纳加的血腕",
            Quality = ItemQuality.Legendary,
            Slug = "ashnagarrs-blood-bracer",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ashnagarrs-blood-bracer",
            Url = "https://us.battle.net/d3/en/item/ashnagarrs-blood-bracer",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_bracer_004_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ashnagarrs-blood-bracer",
            IsCrafted = false,
            LegendaryAffix = "使你护盾的效能提高 (75-100)%。",
            SetName = "",
        };

        /// <summary>
        /// Cesar's Memento Enemies take 300–400 % increased damage from your Tempest Rush for 5 seconds after you hit them with a Blind, Freeze, or Stun.
        /// </summary>
        public static Item CesarsMemento = new Item
        {
            Id = 449038,
            Name = "凯撒的回忆",
            Quality = ItemQuality.Legendary,
            Slug = "cesars-memento",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/cesars-memento",
            Url = "https://us.battle.net/d3/en/item/cesars-memento",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_bracer_107_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/cesars-memento",
            IsCrafted = false,
            LegendaryAffix = "当敌人在受到你的致盲、冰冻或昏迷效果后的 5 秒内，你的风雷冲对其造成的伤害提高 (600-800)%。",
            SetName = "",
        };

        /// <summary>
        /// Gungdo Gear Exploding Palm's on-death explosion applies Exploding Palm.
        /// </summary>
        public static Item GungdoGear = new Item
        {
            Id = 193688,
            Name = "箭道护腕",
            Quality = ItemQuality.Legendary,
            Slug = "gungdo-gear",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Bracers_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/gungdo-gear",
            Url = "https://us.battle.net/d3/en/item/gungdo-gear",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_bracer_006_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/gungdo-gear",
            IsCrafted = false,
            LegendaryAffix = "爆裂掌的死亡爆炸效果会产生爆裂掌。",
            SetName = "",
        };

        /// <summary>
        /// Bracers of Destruction Seismic Slam deals 300–400 % increased damage to the first 5 enemies it hits.
        /// </summary>
        public static Item BracersOfDestruction = new Item
        {
            Id = 440429,
            Name = "毁灭护腕",
            Quality = ItemQuality.Legendary,
            Slug = "bracers-of-destruction",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bracers-of-destruction",
            Url = "https://us.battle.net/d3/en/item/bracers-of-destruction",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_bracer_104_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bracers-of-destruction",
            IsCrafted = false,
            LegendaryAffix = "裂地斩对其击中的前 5 名敌人造成的伤害提高 400-500%。",
            SetName = "",
        };

        /// <summary>
        /// Bracers of the First Men Hammer of the Ancients attacks 50% faster and deals 150–200 % increased damage.
        /// </summary>
        public static Item BracersOfTheFirstMen = new Item
        {
            Id = 440430,
            Name = "先民护腕",
            Quality = ItemQuality.Legendary,
            Slug = "bracers-of-the-first-men",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bracers-of-the-first-men",
            Url = "https://us.battle.net/d3/en/item/bracers-of-the-first-men",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_bracer_105_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bracers-of-the-first-men",
            IsCrafted = false,
            LegendaryAffix = "先祖之锤的攻击速度提高 50% ，造成的伤害提高 375-500%。",
            SetName = "",
        };

        /// <summary>
        /// Gabriel's Vambraces When your Blessed Hammer hits 3 or fewer enemies, 75–100 % of its Wrath Cost is refunded.
        /// </summary>
        public static Item GabrielsVambraces = new Item
        {
            Id = 436469,
            Name = "加百利的臂甲",
            Quality = ItemQuality.Legendary,
            Slug = "gabriels-vambraces",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/gabriels-vambraces",
            Url = "https://us.battle.net/d3/en/item/gabriels-vambraces",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_bracer_101_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/gabriels-vambraces",
            IsCrafted = false,
            LegendaryAffix = "当你的祝福之锤击中 3 个或更少的敌人时，返还 (75-100)% 所消耗的愤怒值。",
            SetName = "",
        };

        /// <summary>
        /// Pinto's Pride Wave of Light also Slows enemies by 80% for 3 seconds and deals 125–150 % increased damage.
        /// </summary>
        public static Item PintosPride = new Item
        {
            Id = 447294,
            Name = "平托的骄傲",
            Quality = ItemQuality.Legendary,
            Slug = "pintos-pride",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/pintos-pride",
            Url = "https://us.battle.net/d3/en/item/pintos-pride",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_bracer_105_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pintos-pride",
            IsCrafted = false,
            LegendaryAffix = "金钟破还会使敌人在 3 秒内减速 80%，并且造成的伤害提高 (125-150)%。",
            SetName = "",
        };

        /// <summary>
        /// Sanguinary Vambraces Chance on being hit to deal 1000% of your Thorns damage to nearby enemies.
        /// </summary>
        public static Item SanguinaryVambraces = new Item
        {
            Id = 298120,
            Name = "浴血前臂甲",
            Quality = ItemQuality.Legendary,
            Slug = "sanguinary-vambraces",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/sanguinary-vambraces",
            Url = "https://us.battle.net/d3/en/item/sanguinary-vambraces",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bracer_105_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sanguinary-vambraces",
            IsCrafted = false,
            LegendaryAffix = "被命中时有一定几率对周围的敌人造成相当于你“荆棘”属性 10 倍的伤害。",
            SetName = "",
        };

        /// <summary>
        /// Wraps of Clarity Your Hatred Generators reduce your damage taken by 30–35 % for 5 seconds.
        /// </summary>
        public static Item WrapsOfClarity = new Item
        {
            Id = 440428,
            Name = "明彻裹腕",
            Quality = ItemQuality.Legendary,
            Slug = "wraps-of-clarity",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/wraps-of-clarity",
            Url = "https://us.battle.net/d3/en/item/wraps-of-clarity",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_bracer_103_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/wraps-of-clarity",
            IsCrafted = false,
            LegendaryAffix = "你的憎恨生成技可使你受到的伤害降低 (40-50)% ，持续 5 秒。",
            SetName = "",
        };

        /// <summary>
        /// Bindings of the Lesser Gods Enemies hit by your Cyclone Strike take 150–200 % increased damage from your Mystic Ally for 5 seconds.
        /// </summary>
        public static Item BindingsOfTheLesserGods = new Item
        {
            Id = 440427,
            Name = "蒙尘者绑腕",
            Quality = ItemQuality.Legendary,
            Slug = "bindings-of-the-lesser-gods",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bindings-of-the-lesser-gods",
            Url = "https://us.battle.net/d3/en/item/bindings-of-the-lesser-gods",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_bracer_108_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bindings-of-the-lesser-gods",
            IsCrafted = false,
            LegendaryAffix = "被你飓风破命中的敌人受到你幻身的伤害提高 (150-200)%，持续 5 秒。",
            SetName = "",
        };

        /// <summary>
        /// Akkhan's Manacles Blessed Shield damage is increased by 400–500 % for the first enemy it hits.
        /// </summary>
        public static Item AkkhansManacles = new Item
        {
            Id = 446057,
            Name = "阿克汉的镣铐",
            Quality = ItemQuality.Legendary,
            Slug = "akkhans-manacles",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/akkhans-manacles",
            Url = "https://us.battle.net/d3/en/item/akkhans-manacles",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_bracer_103_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/akkhans-manacles",
            IsCrafted = false,
            LegendaryAffix = "祝福之盾击中第一个敌人的伤害提高 (400-500)%。",
            SetName = "",
        };

        /// <summary>
        /// Jeram's Bracers Wall of Death deals 75–100 % increased damage and can be cast up to three times within 2 seconds before the cooldown begins.
        /// </summary>
        public static Item JeramsBracers = new Item
        {
            Id = 440431,
            Name = "杰拉姆的护腕",
            Quality = ItemQuality.Legendary,
            Slug = "jerams-bracers",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/jerams-bracers",
            Url = "https://us.battle.net/d3/en/item/jerams-bracers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_bracer_106_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jerams-bracers",
            IsCrafted = false,
            LegendaryAffix = "亡者之墙造成的伤害提高 (75-100)%，可在 2 秒内最多施放三次，然后再触发冷却时间。",
            SetName = "",
        };

        /// <summary>
        /// Bracer of Fury Heaven's Fury deals 150–200 % increased damage to enemies that are Blinded, Immobilized, or Stunned.
        /// </summary>
        public static Item BracerOfFury = new Item
        {
            Id = 446161,
            Name = "愤怒护腕",
            Quality = ItemQuality.Legendary,
            Slug = "bracer-of-fury",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bracer-of-fury",
            Url = "https://us.battle.net/d3/en/item/bracer-of-fury",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_bracer_104_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bracer-of-fury",
            IsCrafted = false,
            LegendaryAffix = "天堂之怒对被致盲、被定身或被昏迷的敌人造成的伤害提高 (300-400%)%。",
            SetName = "",
        };

        /// <summary>
        /// Vambraces of Sescheron Your primary skills heal you for 5.0–6.0 % of your missing Life.
        /// </summary>
        public static Item VambracesOfSescheron = new Item
        {
            Id = 447838,
            Name = "赛斯切隆臂甲",
            Quality = ItemQuality.Legendary,
            Slug = "vambraces-of-sescheron",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/vambraces-of-sescheron",
            Url = "https://us.battle.net/d3/en/item/vambraces-of-sescheron",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_bracer_106_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vambraces-of-sescheron",
            IsCrafted = false,
            LegendaryAffix = "你的主要技能治疗你相当于你损失生命值 (5.0-6.0)% 的生命。",
            SetName = "",
        };

        /// <summary>
        /// Ancient Parthan Defenders Each stunned enemy within 25 yards reduces your damage taken by 9–12 % .
        /// </summary>
        public static Item AncientParthanDefenders = new Item
        {
            Id = 298116,
            Name = "古帕萨卫士护腕",
            Quality = ItemQuality.Legendary,
            Slug = "ancient-parthan-defenders",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "bracers_norm_unique_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/ancient-parthan-defenders",
            Url = "https://us.battle.net/d3/en/item/ancient-parthan-defenders",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bracer_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ancient-parthan-defenders",
            IsCrafted = false,
            LegendaryAffix = "25码内每个陷入昏迷的敌人，都会使你受到的伤害降低 (9-12)%。",
            SetName = "",
        };

        /// <summary>
        /// Custerian Wristguards Picking up gold grants experience.
        /// </summary>
        public static Item CusterianWristguards = new Item
        {
            Id = 298122,
            Name = "卡斯特瑞安腕甲",
            Quality = ItemQuality.Legendary,
            Slug = "custerian-wristguards",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "bracers_norm_unique_17",
            DataUrl = "https://us.battle.net/api/d3/data/item/custerian-wristguards",
            Url = "https://us.battle.net/d3/en/item/custerian-wristguards",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bracer_107_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/custerian-wristguards",
            IsCrafted = false,
            LegendaryAffix = "拾取金币可获得经验值。",
            SetName = "",
        };

        /// <summary>
        /// Nemesis Bracers Shrines and Pylons will spawn an enemy champion.
        /// </summary>
        public static Item NemesisBracers = new Item
        {
            Id = 298121,
            Name = "复仇者护腕",
            Quality = ItemQuality.Legendary,
            Slug = "nemesis-bracers",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "chestArmor_norm_unique_088",
            DataUrl = "https://us.battle.net/api/d3/data/item/nemesis-bracers",
            Url = "https://us.battle.net/d3/en/item/nemesis-bracers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bracer_106_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/nemesis-bracers",
            IsCrafted = false,
            LegendaryAffix = "点击圣坛和水晶塔会出现一名敌人勇士。",
            SetName = "",
        };

        /// <summary>
        /// Warzechian Armguards Every time you destroy a wreckable object, you gain a short burst of speed.
        /// </summary>
        public static Item WarzechianArmguards = new Item
        {
            Id = 298115,
            Name = "沃兹克臂甲",
            Quality = ItemQuality.Legendary,
            Slug = "warzechian-armguards",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "bracers_norm_unique_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/warzechian-armguards",
            Url = "https://us.battle.net/d3/en/item/warzechian-armguards",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bracer_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/warzechian-armguards",
            IsCrafted = false,
            LegendaryAffix = "每当你摧毁一个可破坏物时，你就会获得短时间的速度提升。",
            SetName = "",
        };

        /// <summary>
        /// Promise of Glory 4–6 % chance to spawn a Nephalem Glory globe when you Blind an enemy.
        /// </summary>
        public static Item PromiseOfGlory = new Item
        {
            Id = 193684,
            Name = "荣光之诺",
            Quality = ItemQuality.Legendary,
            Slug = "promise-of-glory",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Bracers_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/promise-of-glory",
            Url = "https://us.battle.net/d3/en/item/promise-of-glory",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bracer_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/promise-of-glory",
            IsCrafted = false,
            LegendaryAffix = "当你致盲一名敌人时，有 (4-6)% 的几率生成一个奈非天之耀的能量球。",
            SetName = "",
        };

        /// <summary>
        /// Lacuni Prowlers 
        /// </summary>
        public static Item LacuniProwlers = new Item
        {
            Id = 193687,
            Name = "豹人猎杀者",
            Quality = ItemQuality.Legendary,
            Slug = "lacuni-prowlers",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Bracers_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/lacuni-prowlers",
            Url = "https://us.battle.net/d3/en/item/lacuni-prowlers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bracer_005_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/lacuni-prowlers",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Strongarm Bracers Enemies hit by knockbacks suffer 20–30 % increased damage for 6 seconds.
        /// </summary>
        public static Item StrongarmBracers = new Item
        {
            Id = 193692,
            Name = "力士护腕",
            Quality = ItemQuality.Legendary,
            Slug = "strongarm-bracers",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Bracers_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/strongarm-bracers",
            Url = "https://us.battle.net/d3/en/item/strongarm-bracers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bracer_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/strongarm-bracers",
            IsCrafted = false,
            LegendaryAffix = "被击退的敌人受到的伤害提高 (20-30)%，持续6秒。",
            SetName = "",
        };

        /// <summary>
        /// Coils of the First Spider While channeling Firebats, you gain 30% damage reduction and 60000–80000 Life per Hit.
        /// </summary>
        public static Item CoilsOfTheFirstSpider = new Item
        {
            Id = 440432,
            Name = "蜘蛛始祖的缠绕",
            Quality = ItemQuality.Legendary,
            Slug = "coils-of-the-first-spider",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/coils-of-the-first-spider",
            Url = "https://us.battle.net/d3/en/item/coils-of-the-first-spider",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_bracer_107_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/coils-of-the-first-spider",
            IsCrafted = false,
            LegendaryAffix = "引导火蝠时，你获得 30% 的伤害减免以及 (60000-80000) 点击中回复生命。",
            SetName = "",
        };

        /// <summary>
        /// Drakon's Lesson When your Shield Bash hits 3 or fewer enemies, its damage is increased by 300–400 % and 25% of its Wrath Cost is refunded.
        /// </summary>
        public static Item DrakonsLesson = new Item
        {
            Id = 432833,
            Name = "德拉肯的训导",
            Quality = ItemQuality.Legendary,
            Slug = "drakons-lesson",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/drakons-lesson",
            Url = "https://us.battle.net/d3/en/item/drakons-lesson",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_bracer_110_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/drakons-lesson",
            IsCrafted = false,
            LegendaryAffix = "当你的盾牌猛击击中 3 个或更少的敌人时，其伤害提高 (300-400)% 并且返还 25% 所消耗的愤怒值。",
            SetName = "",
        };

        /// <summary>
        /// Lakumba's Ornament Reduce all damage taken by 6 % for each stack of Soul Harvest you have.
        /// </summary>
        public static Item LakumbasOrnament = new Item
        {
            Id = 445265,
            Name = "拉昆巴的腕饰",
            Quality = ItemQuality.Legendary,
            Slug = "lakumbas-ornament",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/lakumbas-ornament",
            Url = "https://us.battle.net/d3/en/item/lakumbas-ornament",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_bracer_102_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/lakumbas-ornament",
            IsCrafted = false,
            LegendaryAffix = "你拥有的每一层灵魂收割效果都能使你受到的所有伤害降低 6%。",
            SetName = "",
        };

        /// <summary>
        /// Ranslor's Folly Energy Twister periodically pulls in lesser enemies within 30 yards.
        /// </summary>
        public static Item RanslorsFolly = new Item
        {
            Id = 298123,
            Name = "朗斯洛的愚行",
            Quality = ItemQuality.Legendary,
            Slug = "ranslors-folly",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ranslors-folly",
            Url = "https://us.battle.net/d3/en/item/ranslors-folly",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bracer_108_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ranslors-folly",
            IsCrafted = false,
            LegendaryAffix = "能量气旋周期性地将 30 码内的小型敌人卷过来，并且伤害提高 225-300%。",
            SetName = "",
        };

        /// <summary>
        /// Skular's Salvation Increase the damage of Ancient Spear - Boulder Toss by 100% . When your Boulder Toss hits 5 or fewer enemies, the damage is increased by 120–150 % .
        /// </summary>
        public static Item SkularsSalvation = new Item
        {
            Id = 444928,
            Name = "斯古拉的拯救",
            Quality = ItemQuality.Legendary,
            Slug = "skulars-salvation",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/skulars-salvation",
            Url = "https://us.battle.net/d3/en/item/skulars-salvation",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_bracer_101_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/skulars-salvation",
            IsCrafted = false,
            LegendaryAffix = "使上古之矛 - 投掷巨石的伤害提高 100%。当你的投掷巨石击中 5 个或更少的敌人时，伤害提高 (120-150)%。",
            SetName = "",
        };

        /// <summary>
        /// Spirit Guards Your Spirit Generators reduce your damage taken by 30–40 % for 3 seconds.
        /// </summary>
        public static Item SpiritGuards = new Item
        {
            Id = 430290,
            Name = "灵魂守卫",
            Quality = ItemQuality.Legendary,
            Slug = "spirit-guards",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/spirit-guards",
            Url = "https://us.battle.net/d3/en/item/spirit-guards",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_bracer_109_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/spirit-guards",
            IsCrafted = false,
            LegendaryAffix = "你的内力生成技使你受到的伤害降低(45-60)%，持续3秒。",
            SetName = "",
        };

        /// <summary>
        /// Trag'Oul Coils Spike Traps gain the Impaling Spines rune and are deployed twice as fast.
        /// </summary>
        public static Item TragoulCoils = new Item
        {
            Id = 298119,
            Name = "塔格奥腕环",
            Quality = ItemQuality.Legendary,
            Slug = "tragoul-coils",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "bracers_norm_unique_14",
            DataUrl = "https://us.battle.net/api/d3/data/item/tragoul-coils",
            Url = "https://us.battle.net/d3/en/item/tragoul-coils",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p42_unique_bracer_spiketrap_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tragoul-coils",
            IsCrafted = false,
            LegendaryAffix = "尖刺陷阱获得穿骨脊刺符文效果，并且部署的速度提高一倍。",
            SetName = "",
        };

        /// <summary>
        /// Krelm's Buff Bracers You are immune to Knockback and Stun effects.
        /// </summary>
        public static Item KrelmsBuffBracers = new Item
        {
            Id = 336185,
            Name = "克雷姆的强力护腕",
            Quality = ItemQuality.Legendary,
            Slug = "krelms-buff-bracers",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "bracers_norm_set_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/krelms-buff-bracers",
            Url = "https://us.battle.net/d3/en/item/krelms-buff-bracers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bracer_set_02_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/krelms-buff-bracers",
            IsCrafted = false,
            LegendaryAffix = "你对击退和昏迷效果免疫。",
            SetName = "克雷姆的强力壁垒",
        };

        /// <summary>
        /// Shackles of the Invoker 
        /// </summary>
        public static Item ShacklesOfTheInvoker = new Item
        {
            Id = 335030,
            Name = "唤魔师的镣铐",
            Quality = ItemQuality.Legendary,
            Slug = "shackles-of-the-invoker",
            ItemType = ItemType.Bracer,
            TrinityItemType = TrinityItemType.Bracer,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "bracers_norm_set_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/shackles-of-the-invoker",
            Url = "https://us.battle.net/d3/en/item/shackles-of-the-invoker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bracer_set_12_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/shackles-of-the-invoker",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "唤魔师的荆棘",
        };

        /// <summary>
        /// Gloves of Worship Shrine effects last for 10 minutes.
        /// </summary>
        public static Item GlovesOfWorship = new Item
        {
            Id = 332344,
            Name = "礼赞手套",
            Quality = ItemQuality.Legendary,
            Slug = "gloves-of-worship",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/gloves-of-worship",
            Url = "https://us.battle.net/d3/en/item/gloves-of-worship",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/gloves-of-worship",
            IsCrafted = false,
            LegendaryAffix = "圣坛效果持续10分钟。",
            SetName = "",
        };

        /// <summary>
        /// Grasps of Essence When an exploded corpse damages at least one enemy, your Corpse Explosion deals 75–100 % increased damage for 6 seconds, stacking up to 5 times.
        /// </summary>
        public static Item GraspsOfEssence = new Item
        {
            Id = 467573,
            Name = "精魂魔掌",
            Quality = ItemQuality.Legendary,
            Slug = "grasps-of-essence",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/grasps-of-essence",
            Url = "https://us.battle.net/d3/en/item/grasps-of-essence",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_gloves_22_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/grasps-of-essence",
            IsCrafted = false,
            LegendaryAffix = "当尸骸精魂引爆时如果击中至少一个敌人，你的邪爆造成的伤害会提高（75-100）%，持续6秒，最多叠加5层。",
            SetName = "",
        };

        /// <summary>
        /// Stone Gauntlets 
        /// </summary>
        public static Item StoneGauntlets = new Item
        {
            Id = 205640,
            Name = "岩石护手",
            Quality = ItemQuality.Legendary,
            Slug = "stone-gauntlets",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Gloves_norm_unique_076",
            DataUrl = "https://us.battle.net/api/d3/data/item/stone-gauntlets",
            Url = "https://us.battle.net/d3/en/item/stone-gauntlets",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/stone-gauntlets",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Magefist Fire skills deal 15–20 % increased damage.
        /// </summary>
        public static Item Magefist = new Item
        {
            Id = 197206,
            Name = "法师之拳",
            Quality = ItemQuality.Legendary,
            Slug = "magefist",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Gloves_norm_unique_044",
            DataUrl = "https://us.battle.net/api/d3/data/item/magefist",
            Url = "https://us.battle.net/d3/en/item/magefist",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_gloves_014_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/magefist",
            IsCrafted = false,
            LegendaryAffix = "火焰技能造成的伤害提高 (15-20)%。",
            SetName = "",
        };

        /// <summary>
        /// St. Archew's Gage The first time an elite pack damages you, gain an absorb shield equal to 120–150 % of your maximum Life for 10 seconds.
        /// </summary>
        public static Item StArchewsGage = new Item
        {
            Id = 332172,
            Name = "圣·阿契武的战书",
            Quality = ItemQuality.Legendary,
            Slug = "st-archews-gage",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/st-archews-gage",
            Url = "https://us.battle.net/d3/en/item/st-archews-gage",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_101_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/st-archews-gage",
            IsCrafted = false,
            LegendaryAffix = "当精英怪群第一次对你造成伤害时，你获得一道伤害吸收护盾，可吸收你最大生命值 (120-150)% 的伤害，持续10秒。",
            SetName = "",
        };

        /// <summary>
        /// Moribund Gauntlets Your Golem sheds a corpse every second.
        /// </summary>
        public static Item MoribundGauntlets = new Item
        {
            Id = 467572,
            Name = "僵死护手",
            Quality = ItemQuality.Legendary,
            Slug = "moribund-gauntlets",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/moribund-gauntlets",
            Url = "https://us.battle.net/d3/en/item/moribund-gauntlets",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_gloves_21_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/moribund-gauntlets",
            IsCrafted = false,
            LegendaryAffix = "你的傀儡每秒会生成一具尸骸。",
            SetName = "",
        };

        /// <summary>
        /// Gladiator Gauntlets After earning a massacre bonus, gold rains from sky.
        /// </summary>
        public static Item GladiatorGauntlets = new Item
        {
            Id = 205635,
            Name = "角斗士的手套",
            Quality = ItemQuality.Legendary,
            Slug = "gladiator-gauntlets",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Gloves_norm_unique_090",
            DataUrl = "https://us.battle.net/api/d3/data/item/gladiator-gauntlets",
            Url = "https://us.battle.net/d3/en/item/gladiator-gauntlets",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/gladiator-gauntlets",
            IsCrafted = false,
            LegendaryAffix = "当获得一次大歼灭奖励后，金币会如雨般的从天倾降。",
            SetName = "",
        };

        /// <summary>
        /// Frostburn Cold skills deal 15–20 % increased damage and have a 50% chance to Freeze enemies.
        /// </summary>
        public static Item Frostburn = new Item
        {
            Id = 197205,
            Name = "霜燃",
            Quality = ItemQuality.Legendary,
            Slug = "frostburn",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Gloves_norm_unique_043",
            DataUrl = "https://us.battle.net/api/d3/data/item/frostburn",
            Url = "https://us.battle.net/d3/en/item/frostburn",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_gloves_002_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/frostburn",
            IsCrafted = false,
            LegendaryAffix = "冰霜技能造成的伤害提高 (15-20)%，有 50% 的几率冻结敌人。",
            SetName = "",
        };

        /// <summary>
        /// Tasker and Theo Increase attack speed of your pets by 40–50 % .
        /// </summary>
        public static Item TaskerAndTheo = new Item
        {
            Id = 205642,
            Name = "塔斯克与西奥",
            Quality = ItemQuality.Legendary,
            Slug = "tasker-and-theo",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/tasker-and-theo",
            Url = "https://us.battle.net/d3/en/item/tasker-and-theo",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tasker-and-theo",
            IsCrafted = false,
            LegendaryAffix = "使你宠物的攻击速度提高 (40-50)%。",
            SetName = "",
        };

        /// <summary>
        /// Immortal King's Irons 
        /// </summary>
        public static Item ImmortalKingsIrons = new Item
        {
            Id = 205631,
            Name = "不朽之王的铁拳",
            Quality = ItemQuality.Legendary,
            Slug = "immortal-kings-irons",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Gloves_norm_unique_086",
            DataUrl = "https://us.battle.net/api/d3/data/item/immortal-kings-irons",
            Url = "https://us.battle.net/d3/en/item/immortal-kings-irons",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_008_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/immortal-kings-irons",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不朽之王的呼唤",
        };

        /// <summary>
        /// Inna's Hold 
        /// </summary>
        public static Item InnasHold = new Item
        {
            Id = 415197,
            Name = "尹娜的掌控",
            Quality = ItemQuality.Legendary,
            Slug = "innas-hold",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/innas-hold",
            Url = "https://us.battle.net/d3/en/item/innas-hold",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_gloves_04_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/innas-hold",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "尹娜的真言",
        };

        /// <summary>
        /// Natalya's Touch 
        /// </summary>
        public static Item NatalyasTouch = new Item
        {
            Id = 415190,
            Name = "娜塔亚的手感",
            Quality = ItemQuality.Legendary,
            Slug = "natalyas-touch",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/natalyas-touch",
            Url = "https://us.battle.net/d3/en/item/natalyas-touch",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_gloves_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/natalyas-touch",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "娜塔亚的复仇",
        };

        /// <summary>
        /// Tal Rasha's Grasp 
        /// </summary>
        public static Item TalRashasGrasp = new Item
        {
            Id = 415051,
            Name = "塔·拉夏的领悟",
            Quality = ItemQuality.Legendary,
            Slug = "tal-rashas-grasp",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/tal-rashas-grasp",
            Url = "https://us.battle.net/d3/en/item/tal-rashas-grasp",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_gloves_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tal-rashas-grasp",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔·拉夏的法理",
        };

        /// <summary>
        /// Zunimassa's Finger Wraps 
        /// </summary>
        public static Item ZunimassasFingerWraps = new Item
        {
            Id = 205633,
            Name = "祖尼玛萨之手",
            Quality = ItemQuality.Legendary,
            Slug = "zunimassas-finger-wraps",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/zunimassas-finger-wraps",
            Url = "https://us.battle.net/d3/en/item/zunimassas-finger-wraps",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_gloves_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/zunimassas-finger-wraps",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "祖尼玛萨之魂",
        };

        /// <summary>
        /// Arachyr's Claws 
        /// </summary>
        public static Item ArachyrsClaws = new Item
        {
            Id = 441196,
            Name = "亚拉基尔的利爪",
            Quality = ItemQuality.Legendary,
            Slug = "arachyrs-claws",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/arachyrs-claws",
            Url = "https://us.battle.net/d3/en/item/arachyrs-claws",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_02_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/arachyrs-claws",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "亚拉基尔的灵魂",
        };

        /// <summary>
        /// Fiendish Grips 
        /// </summary>
        public static Item FiendishGrips = new Item
        {
            Id = 408876,
            Name = "恶魔之握",
            Quality = ItemQuality.Legendary,
            Slug = "fiendish-grips",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/fiendish-grips",
            Url = "https://us.battle.net/d3/en/item/fiendish-grips",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_03_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fiendish-grips",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "邪秽之精",
        };

        /// <summary>
        /// Fierce Gauntlets 
        /// </summary>
        public static Item FierceGauntlets = new Item
        {
            Id = 408873,
            Name = "凶恶护手",
            Quality = ItemQuality.Legendary,
            Slug = "fierce-gauntlets",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/fierce-gauntlets",
            Url = "https://us.battle.net/d3/en/item/fierce-gauntlets",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_02_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fierce-gauntlets",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "德尔西尼的杰作",
        };

        /// <summary>
        /// Firebird's Talons 
        /// </summary>
        public static Item FirebirdsTalons = new Item
        {
            Id = 358789,
            Name = "不死鸟之爪",
            Quality = ItemQuality.Legendary,
            Slug = "firebirds-talons",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_set_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/firebirds-talons",
            Url = "https://us.battle.net/d3/en/item/firebirds-talons",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_06_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/firebirds-talons",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不死鸟的华服",
        };

        /// <summary>
        /// Fists of Thunder 
        /// </summary>
        public static Item FistsOfThunder = new Item
        {
            Id = 338033,
            Name = "雷光拳套",
            Quality = ItemQuality.Legendary,
            Slug = "fists-of-thunder",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_set_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/fists-of-thunder",
            Url = "https://us.battle.net/d3/en/item/fists-of-thunder",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_08_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fists-of-thunder",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "千风飓",
        };

        /// <summary>
        /// Gauntlet of the Wastes 
        /// </summary>
        public static Item GauntletOfTheWastes = new Item
        {
            Id = 408861,
            Name = "荒原护手",
            Quality = ItemQuality.Legendary,
            Slug = "gauntlet-of-the-wastes",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/gauntlet-of-the-wastes",
            Url = "https://us.battle.net/d3/en/item/gauntlet-of-the-wastes",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_01_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/gauntlet-of-the-wastes",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "废土之怒",
        };

        /// <summary>
        /// Gauntlets of Akkhan 
        /// </summary>
        public static Item GauntletsOfAkkhan = new Item
        {
            Id = 358798,
            Name = "阿克汉的护手",
            Quality = ItemQuality.Legendary,
            Slug = "gauntlets-of-akkhan",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/gauntlets-of-akkhan",
            Url = "https://us.battle.net/d3/en/item/gauntlets-of-akkhan",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_10_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/gauntlets-of-akkhan",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "阿克汉的战甲",
        };

        /// <summary>
        /// Helltooth Gauntlets 
        /// </summary>
        public static Item HelltoothGauntlets = new Item
        {
            Id = 363094,
            Name = "魔牙护手",
            Quality = ItemQuality.Legendary,
            Slug = "helltooth-gauntlets",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_set_16",
            DataUrl = "https://us.battle.net/api/d3/data/item/helltooth-gauntlets",
            Url = "https://us.battle.net/d3/en/item/helltooth-gauntlets",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_16_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/helltooth-gauntlets",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "魔牙战装",
        };

        /// <summary>
        /// Inarius's Will 
        /// </summary>
        public static Item InariussWill = new Item
        {
            Id = 467571,
            Name = "伊纳瑞斯的意志",
            Quality = ItemQuality.Legendary,
            Slug = "inariuss-will",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/inariuss-will",
            Url = "https://us.battle.net/d3/en/item/inariuss-will",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_3_gloves_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/inariuss-will",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "伊纳瑞斯的恩泽",
        };

        /// <summary>
        /// Jade Harvester's Mercy 
        /// </summary>
        public static Item JadeHarvestersMercy = new Item
        {
            Id = 338039,
            Name = "玉魂师的怜悯",
            Quality = ItemQuality.Legendary,
            Slug = "jade-harvesters-mercy",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_set_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/jade-harvesters-mercy",
            Url = "https://us.battle.net/d3/en/item/jade-harvesters-mercy",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_09_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jade-harvesters-mercy",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "玉魂师的战甲",
        };

        /// <summary>
        /// Marauder's Gloves 
        /// </summary>
        public static Item MaraudersGloves = new Item
        {
            Id = 336992,
            Name = "掠夺者的手套",
            Quality = ItemQuality.Legendary,
            Slug = "marauders-gloves",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_set_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/marauders-gloves",
            Url = "https://us.battle.net/d3/en/item/marauders-gloves",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_07_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/marauders-gloves",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "掠夺者的化身",
        };

        /// <summary>
        /// Pestilence Gloves 
        /// </summary>
        public static Item PestilenceGloves = new Item
        {
            Id = 467352,
            Name = "死疫护手",
            Quality = ItemQuality.Legendary,
            Slug = "pestilence-gloves",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/pestilence-gloves",
            Url = "https://us.battle.net/d3/en/item/pestilence-gloves",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_4_gloves_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pestilence-gloves",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "死疫死者的裹布",
        };

        /// <summary>
        /// Pride of the Invoker 
        /// </summary>
        public static Item PrideOfTheInvoker = new Item
        {
            Id = 335027,
            Name = "唤魔师的骄傲",
            Quality = ItemQuality.Legendary,
            Slug = "pride-of-the-invoker",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_set_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/pride-of-the-invoker",
            Url = "https://us.battle.net/d3/en/item/pride-of-the-invoker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_12_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pride-of-the-invoker",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "唤魔师的荆棘",
        };

        /// <summary>
        /// Pull of the Earth 
        /// </summary>
        public static Item PullOfTheEarth = new Item
        {
            Id = 340523,
            Name = "大地之缚",
            Quality = ItemQuality.Legendary,
            Slug = "pull-of-the-earth",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_set_15",
            DataUrl = "https://us.battle.net/api/d3/data/item/pull-of-the-earth",
            Url = "https://us.battle.net/d3/en/item/pull-of-the-earth",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_15_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pull-of-the-earth",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "大地之力",
        };

        /// <summary>
        /// Raekor's Wraps 
        /// </summary>
        public static Item RaekorsWraps = new Item
        {
            Id = 336985,
            Name = "蕾蔻的裹手",
            Quality = ItemQuality.Legendary,
            Slug = "raekors-wraps",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_set_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/raekors-wraps",
            Url = "https://us.battle.net/d3/en/item/raekors-wraps",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_05_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/raekors-wraps",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "蕾蔻的传世铠",
        };

        /// <summary>
        /// Rathma's Macabre Vambraces 
        /// </summary>
        public static Item RathmasMacabreVambraces = new Item
        {
            Id = 460920,
            Name = "拉斯玛的骨魂臂甲",
            Quality = ItemQuality.Legendary,
            Slug = "rathmas-macabre-vambraces",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/rathmas-macabre-vambraces",
            Url = "https://us.battle.net/d3/en/item/rathmas-macabre-vambraces",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_1_gloves_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rathmas-macabre-vambraces",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "拉斯玛的骨甲",
        };

        /// <summary>
        /// Roland's Grasp 
        /// </summary>
        public static Item RolandsGrasp = new Item
        {
            Id = 404096,
            Name = "罗兰之握",
            Quality = ItemQuality.Legendary,
            Slug = "rolands-grasp",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Gloves_norm_base_flippy",
            DataUrl = "https://us.battle.net/api/d3/data/item/rolands-grasp",
            Url = "https://us.battle.net/d3/en/item/rolands-grasp",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_01_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rolands-grasp",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "罗兰的传世甲",
        };

        /// <summary>
        /// Sunwuko's Paws 
        /// </summary>
        public static Item SunwukosPaws = new Item
        {
            Id = 336172,
            Name = "孙武空的灵掌",
            Quality = ItemQuality.Legendary,
            Slug = "sunwukos-paws",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_set_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/sunwukos-paws",
            Url = "https://us.battle.net/d3/en/item/sunwukos-paws",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_11_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sunwukos-paws",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "猴王战甲",
        };

        /// <summary>
        /// The Shadow's Grasp 
        /// </summary>
        public static Item TheShadowsGrasp = new Item
        {
            Id = 332362,
            Name = "影弑",
            Quality = ItemQuality.Legendary,
            Slug = "the-shadows-grasp",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_set_14",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-shadows-grasp",
            Url = "https://us.battle.net/d3/en/item/the-shadows-grasp",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_14_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-shadows-grasp",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "暗影装束",
        };

        /// <summary>
        /// Trag'Oul's Claws 
        /// </summary>
        public static Item TragoulsClaws = new Item
        {
            Id = 467570,
            Name = "塔格奥之爪",
            Quality = ItemQuality.Legendary,
            Slug = "tragouls-claws",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/tragouls-claws",
            Url = "https://us.battle.net/d3/en/item/tragouls-claws",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_2_gloves_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tragouls-claws",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔格奥的化身",
        };

        /// <summary>
        /// Uliana's Fury 
        /// </summary>
        public static Item UlianasFury = new Item
        {
            Id = 408874,
            Name = "乌莲娜的愤怒",
            Quality = ItemQuality.Legendary,
            Slug = "ulianas-fury",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ulianas-fury",
            Url = "https://us.battle.net/d3/en/item/ulianas-fury",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_01_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ulianas-fury",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "乌莲娜的谋略",
        };

        /// <summary>
        /// Vyr's Grasping Gauntlets 
        /// </summary>
        public static Item VyrsGraspingGauntlets = new Item
        {
            Id = 346210,
            Name = "维尔的控能护手",
            Quality = ItemQuality.Legendary,
            Slug = "vyrs-grasping-gauntlets",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "gloves_norm_set_13",
            DataUrl = "https://us.battle.net/api/d3/data/item/vyrs-grasping-gauntlets",
            Url = "https://us.battle.net/d3/en/item/vyrs-grasping-gauntlets",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_13_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vyrs-grasping-gauntlets",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "维尔的神装",
        };

        /// <summary>
        /// Will of the Light 
        /// </summary>
        public static Item WillOfTheLight = new Item
        {
            Id = 408877,
            Name = "圣光之志",
            Quality = ItemQuality.Legendary,
            Slug = "will-of-the-light",
            ItemType = ItemType.Gloves,
            TrinityItemType = TrinityItemType.Gloves,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/will-of-the-light",
            Url = "https://us.battle.net/d3/en/item/will-of-the-light",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_gloves_set_03_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/will-of-the-light",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "圣光追寻者",
        };

        /// <summary>
        /// Girdle of Giants Seismic Slam increases Earthquake damage by 80–100 % for 3 seconds.
        /// </summary>
        public static Item GirdleOfGiants = new Item
        {
            Id = 212232,
            Name = "巨人腰带",
            Quality = ItemQuality.Legendary,
            Slug = "girdle-of-giants",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "BarbBelt_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/girdle-of-giants",
            Url = "https://us.battle.net/d3/en/item/girdle-of-giants",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p42_unique_barbbelt_eq_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/girdle-of-giants",
            IsCrafted = false,
            LegendaryAffix = "裂地斩使地震的伤害提高 200-250%，持续 3 秒。",
            SetName = "",
        };

        /// <summary>
        /// The Undisputed Champion Frenzy gains the effect of every rune.
        /// </summary>
        public static Item TheUndisputedChampion = new Item
        {
            Id = 193676,
            Name = "无可争辩的勇士",
            Quality = ItemQuality.Legendary,
            Slug = "the-undisputed-champion",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "BarbBelt_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-undisputed-champion",
            Url = "https://us.battle.net/d3/en/item/the-undisputed-champion",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_barbbelt_006_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-undisputed-champion",
            IsCrafted = false,
            LegendaryAffix = "狂乱获得每个符文的效果。",
            SetName = "",
        };

        /// <summary>
        /// Pride of Cassius Increases the duration of Ignore Pain by 4–6 seconds.
        /// </summary>
        public static Item PrideOfCassius = new Item
        {
            Id = 193673,
            Name = "卡修斯的骄傲",
            Quality = ItemQuality.Legendary,
            Slug = "pride-of-cassius",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "BarbBelt_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/pride-of-cassius",
            Url = "https://us.battle.net/d3/en/item/pride-of-cassius",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_barbbelt_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pride-of-cassius",
            IsCrafted = false,
            LegendaryAffix = "使无视苦痛的持续时间延长 (4-6)。",
            SetName = "",
        };

        /// <summary>
        /// Chilanik's Chain Using War Cry increases the movement speed for you and all allies affected by 30–40 % for 10 seconds.
        /// </summary>
        public static Item ChilaniksChain = new Item
        {
            Id = 298133,
            Name = "齐拉尼克之链",
            Quality = ItemQuality.Legendary,
            Slug = "chilaniks-chain",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "barbbelt_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/chilaniks-chain",
            Url = "https://us.battle.net/d3/en/item/chilaniks-chain",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_barbbelt_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/chilaniks-chain",
            IsCrafted = false,
            LegendaryAffix = "使用战吼可使你和所有受影响同伴的移动速度提高 (30-40)%，持续10秒。",
            SetName = "",
        };

        /// <summary>
        /// Lamentation Rend can now stack up to 2 times on an enemy.
        /// </summary>
        public static Item Lamentation = new Item
        {
            Id = 212234,
            Name = "悲恸",
            Quality = ItemQuality.Legendary,
            Slug = "lamentation",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "barbbelt_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/lamentation",
            Url = "https://us.battle.net/d3/en/item/lamentation",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_barbbelt_005_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/lamentation",
            IsCrafted = false,
            LegendaryAffix = "痛割在同个敌人身上最多可以叠加至2次。",
            SetName = "",
        };

        /// <summary>
        /// Immortal King's Tribal Binding 
        /// </summary>
        public static Item ImmortalKingsTribalBinding = new Item
        {
            Id = 212235,
            Name = "不朽之王的部族绑腰",
            Quality = ItemQuality.Legendary,
            Slug = "immortal-kings-tribal-binding",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "BarbBelt_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/immortal-kings-tribal-binding",
            Url = "https://us.battle.net/d3/en/item/immortal-kings-tribal-binding",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_barbbelt_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/immortal-kings-tribal-binding",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不朽之王的呼唤",
        };

        /// <summary>
        /// Dread Iron Ground Stomp causes an Avalanche.
        /// </summary>
        public static Item DreadIron = new Item
        {
            Id = 193672,
            Name = "恐惧铸铁",
            Quality = ItemQuality.Legendary,
            Slug = "dread-iron",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "BarbBelt_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/dread-iron",
            Url = "https://us.battle.net/d3/en/item/dread-iron",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_barbbelt_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/dread-iron",
            IsCrafted = false,
            LegendaryAffix = "大地践踏会造成一次山崩地裂。",
            SetName = "",
        };

        /// <summary>
        /// Moonlight Ward Hitting an enemy within 15 yards has a chance to ward you with shards of Arcane energy that explode when enemies get close, dealing 240–320 % weapon damage as Arcane to enemies within 15 yards.
        /// </summary>
        public static Item MoonlightWard = new Item
        {
            Id = 197813,
            Name = "月光护符",
            Quality = ItemQuality.Legendary,
            Slug = "moonlight-ward",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Amulet_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/moonlight-ward",
            Url = "https://us.battle.net/d3/en/item/moonlight-ward",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/moonlight-ward",
            IsCrafted = false,
            LegendaryAffix = "击中 15 码范围内的一名敌人有一定几率使你获得奥能碎片结界，当敌人靠近时爆炸，对 15 码范围内的所有敌人造成 (240-320)% 的武器伤害（作为奥术伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Squirt's Necklace 
        /// </summary>
        public static Item SquirtsNecklace = new Item
        {
            Id = 197819,
            Name = "斯奎特的项链",
            Quality = ItemQuality.Legendary,
            Slug = "squirts-necklace",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "amulet_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/squirts-necklace",
            Url = "https://us.battle.net/d3/en/item/squirts-necklace",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_010_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/squirts-necklace",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Golden Gorget of Leoric After earning a massacre bonus, 4–6 Skeletons are summoned to fight by your side for 10 seconds.
        /// </summary>
        public static Item GoldenGorgetOfLeoric = new Item
        {
            Id = 298052,
            Name = "李奥瑞克的黄金护颈",
            Quality = ItemQuality.Legendary,
            Slug = "golden-gorget-of-leoric",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/golden-gorget-of-leoric",
            Url = "https://us.battle.net/d3/en/item/golden-gorget-of-leoric",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_105_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/golden-gorget-of-leoric",
            IsCrafted = false,
            LegendaryAffix = "当获得一次大歼灭奖励后，会有 (4-6) 具骷髅为你作战10秒。",
            SetName = "",
        };

        /// <summary>
        /// Overwhelming Desire Chance on hit to charm the enemy. While charmed, the enemy takes 35% increased damage.
        /// </summary>
        public static Item OverwhelmingDesire = new Item
        {
            Id = 298053,
            Name = "妄念",
            Quality = ItemQuality.Legendary,
            Slug = "overwhelming-desire",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/overwhelming-desire",
            Url = "https://us.battle.net/d3/en/item/overwhelming-desire",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_106_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/overwhelming-desire",
            IsCrafted = false,
            LegendaryAffix = "击中有一定几率魅惑敌人。当被魅惑时，敌人受到的伤害提高 35% 。",
            SetName = "",
        };

        /// <summary>
        /// Wisdom of Kalan Increases the maximum stacks of Bone Armor by 5 .
        /// </summary>
        public static Item WisdomOfKalan = new Item
        {
            Id = 476718,
            Name = "卡兰的智慧",
            Quality = ItemQuality.Legendary,
            Slug = "wisdom-of-kalan",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/wisdom-of-kalan",
            Url = "https://us.battle.net/d3/en/item/wisdom-of-kalan",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_amulet_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/wisdom-of-kalan",
            IsCrafted = false,
            LegendaryAffix = "骨甲的最大叠加层数提高5层。",
            SetName = "",
        };

        /// <summary>
        /// Eye of Etlich 
        /// </summary>
        public static Item EyeOfEtlich = new Item
        {
            Id = 197823,
            Name = "艾利奇之眼",
            Quality = ItemQuality.Legendary,
            Slug = "eye-of-etlich",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Amulet_norm_unique_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/eye-of-etlich",
            Url = "https://us.battle.net/d3/en/item/eye-of-etlich",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_014_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/eye-of-etlich",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Rondal's Locket 
        /// </summary>
        public static Item RondalsLocket = new Item
        {
            Id = 197818,
            Name = "隆达尔的坠匣",
            Quality = ItemQuality.Legendary,
            Slug = "rondals-locket",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Amulet_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/rondals-locket",
            Url = "https://us.battle.net/d3/en/item/rondals-locket",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rondals-locket",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Talisman of Aranoch Prevent all Cold damage taken and heal yourself for 10–15 % of the amount prevented.
        /// </summary>
        public static Item TalismanOfAranoch = new Item
        {
            Id = 197821,
            Name = "埃拉诺克护身符",
            Quality = ItemQuality.Legendary,
            Slug = "talisman-of-aranoch",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/talisman-of-aranoch",
            Url = "https://us.battle.net/d3/en/item/talisman-of-aranoch",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_012_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/talisman-of-aranoch",
            IsCrafted = false,
            LegendaryAffix = "抵挡所有冰霜伤害，并为你自己恢复相当于抵挡伤害 (10-15)% 的生命值。",
            SetName = "",
        };

        /// <summary>
        /// Ancestors' Grace When receiving fatal damage, you are instead restored to 100% of maximum Life and resources. This item is destroyed in the process.
        /// </summary>
        public static Item AncestorsGrace = new Item
        {
            Id = 298049,
            Name = "先人之佑",
            Quality = ItemQuality.Legendary,
            Slug = "ancestors-grace",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ancestors-grace",
            Url = "https://us.battle.net/d3/en/item/ancestors-grace",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ancestors-grace",
            IsCrafted = false,
            LegendaryAffix = "当你受到致命伤害时，你会恢复生命值上限和能量的100%。该物品同时会被摧毁。",
            SetName = "",
        };

        /// <summary>
        /// Countess Julia's Cameo Prevent all Arcane damage taken and heal yourself for 20–25 % of the amount prevented.
        /// </summary>
        public static Item CountessJuliasCameo = new Item
        {
            Id = 298050,
            Name = "茱莉雅女爵的雕饰项链",
            Quality = ItemQuality.Legendary,
            Slug = "countess-julias-cameo",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Mojo_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/countess-julias-cameo",
            Url = "https://us.battle.net/d3/en/item/countess-julias-cameo",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/countess-julias-cameo",
            IsCrafted = false,
            LegendaryAffix = "阻挡所有奥术伤害，并为你恢复相当于阻挡伤害 (20-25)% 的生命值。",
            SetName = "",
        };

        /// <summary>
        /// Dovu Energy Trap Increases duration of Stun effects by 20–25 % .
        /// </summary>
        public static Item DovuEnergyTrap = new Item
        {
            Id = 298054,
            Name = "多弗的法能陷阱",
            Quality = ItemQuality.Legendary,
            Slug = "dovu-energy-trap",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "amulet_norm_unique_23",
            DataUrl = "https://us.battle.net/api/d3/data/item/dovu-energy-trap",
            Url = "https://us.battle.net/d3/en/item/dovu-energy-trap",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_107_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/dovu-energy-trap",
            IsCrafted = false,
            LegendaryAffix = "使昏迷效果的持续时间延长 (20-25)%。",
            SetName = "",
        };

        /// <summary>
        /// Haunt of Vaxo Summons shadow clones to your aid when you Stun an enemy. This effect may occur once every 30 seconds.
        /// </summary>
        public static Item HauntOfVaxo = new Item
        {
            Id = 297806,
            Name = "瓦索的阴魂",
            Quality = ItemQuality.Legendary,
            Slug = "haunt-of-vaxo",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Boots_norm_unique_051",
            DataUrl = "https://us.battle.net/api/d3/data/item/haunt-of-vaxo",
            Url = "https://us.battle.net/d3/en/item/haunt-of-vaxo",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/haunt-of-vaxo",
            IsCrafted = false,
            LegendaryAffix = "当你使一名敌人陷入昏迷时，召唤暗影分身为你作战。该效果每 30 秒只能生效一次。",
            SetName = "",
        };

        /// <summary>
        /// Rakoff's Glass of Life Enemies you kill have a 3–4 % additional chance to drop a health globe.
        /// </summary>
        public static Item RakoffsGlassOfLife = new Item
        {
            Id = 298055,
            Name = "拉科夫的吸魂镜",
            Quality = ItemQuality.Legendary,
            Slug = "rakoffs-glass-of-life",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "amulet_norm_unique_24",
            DataUrl = "https://us.battle.net/api/d3/data/item/rakoffs-glass-of-life",
            Url = "https://us.battle.net/d3/en/item/rakoffs-glass-of-life",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_108_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rakoffs-glass-of-life",
            IsCrafted = false,
            LegendaryAffix = "你消灭的敌人有额外 (3-4)% 的几率掉落一颗生命球。",
            SetName = "",
        };

        /// <summary>
        /// The Ess of Johan Chance on hit to pull in enemies toward your target and Slow them by 60–80 % .
        /// </summary>
        public static Item TheEssOfJohan = new Item
        {
            Id = 298051,
            Name = "兵要护符",
            Quality = ItemQuality.Legendary,
            Slug = "the-ess-of-johan",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "amulet_norm_unique_20",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-ess-of-johan",
            Url = "https://us.battle.net/d3/en/item/the-ess-of-johan",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-ess-of-johan",
            IsCrafted = false,
            LegendaryAffix = "击中时有一定几率将敌人拉到你的目标身边，并使其减速 (60-80)%。",
            SetName = "",
        };

        /// <summary>
        /// Holy Beacon 
        /// </summary>
        public static Item HolyBeacon = new Item
        {
            Id = 197822,
            Name = "圣灯",
            Quality = ItemQuality.Legendary,
            Slug = "holy-beacon",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "amulet_norm_unique_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/holy-beacon",
            Url = "https://us.battle.net/d3/en/item/holy-beacon",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_013_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/holy-beacon",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Kymbo's Gold Picking up gold heals you for an amount equal to the gold that was picked up.
        /// </summary>
        public static Item KymbosGold = new Item
        {
            Id = 197812,
            Name = "金宝的金牌",
            Quality = ItemQuality.Legendary,
            Slug = "kymbos-gold",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Mojo_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/kymbos-gold",
            Url = "https://us.battle.net/d3/en/item/kymbos-gold",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_002_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/kymbos-gold",
            IsCrafted = false,
            LegendaryAffix = "拾取金币为你恢复生命值，治疗量相当于你拾取的金币数量。",
            SetName = "",
        };

        /// <summary>
        /// The Flavor of Time 
        /// </summary>
        public static Item TheFlavorOfTime = new Item
        {
            Id = 193659,
            Name = "时光流韵",
            Quality = ItemQuality.Legendary,
            Slug = "the-flavor-of-time",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Amulet_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-flavor-of-time",
            Url = "https://us.battle.net/d3/en/item/the-flavor-of-time",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-flavor-of-time",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Mara's Kaleidoscope Prevent all Poison damage taken and heal yourself for 10–15 % of the amount prevented.
        /// </summary>
        public static Item MarasKaleidoscope = new Item
        {
            Id = 197824,
            Name = "玛拉的万花筒",
            Quality = ItemQuality.Legendary,
            Slug = "maras-kaleidoscope",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/maras-kaleidoscope",
            Url = "https://us.battle.net/d3/en/item/maras-kaleidoscope",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_015_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/maras-kaleidoscope",
            IsCrafted = false,
            LegendaryAffix = "抵挡所有毒性伤害，并为你自己恢复相当于抵挡伤害 (10-15)% 的生命值。",
            SetName = "",
        };

        /// <summary>
        /// Ouroboros 
        /// </summary>
        public static Item Ouroboros = new Item
        {
            Id = 197815,
            Name = "轮回之蛇",
            Quality = ItemQuality.Legendary,
            Slug = "ouroboros",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Amulet_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/ouroboros",
            Url = "https://us.battle.net/d3/en/item/ouroboros",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_005_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ouroboros",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// The Star of Azkaranth Prevent all Fire damage taken and heal yourself for 10–15 % of the amount prevented.
        /// </summary>
        public static Item TheStarOfAzkaranth = new Item
        {
            Id = 197817,
            Name = "阿兹卡兰之星",
            Quality = ItemQuality.Legendary,
            Slug = "the-star-of-azkaranth",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "amulet_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-star-of-azkaranth",
            Url = "https://us.battle.net/d3/en/item/the-star-of-azkaranth",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-star-of-azkaranth",
            IsCrafted = false,
            LegendaryAffix = "抵挡所有火焰伤害，并为你自己恢复相当于抵挡伤害 (10-15)% 的生命值。",
            SetName = "",
        };

        /// <summary>
        /// Xephirian Amulet Prevent all Lightning damage taken and heal yourself for 10–15 % of the amount prevented.
        /// </summary>
        public static Item XephirianAmulet = new Item
        {
            Id = 197814,
            Name = "赛飞利安护符",
            Quality = ItemQuality.Legendary,
            Slug = "xephirian-amulet",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Amulet_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/xephirian-amulet",
            Url = "https://us.battle.net/d3/en/item/xephirian-amulet",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_004_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/xephirian-amulet",
            IsCrafted = false,
            LegendaryAffix = "抵挡所有闪电伤害，并为你自己恢复相当于抵挡伤害 (10-15)% 的生命值。",
            SetName = "",
        };

        /// <summary>
        /// Blackthorne's Duncraig Cross 
        /// </summary>
        public static Item BlackthornesDuncraigCross = new Item
        {
            Id = 224189,
            Name = "黑棘的敦克雷十字章",
            Quality = ItemQuality.Legendary,
            Slug = "blackthornes-duncraig-cross",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "amulet_norm_unique_19",
            DataUrl = "https://us.battle.net/api/d3/data/item/blackthornes-duncraig-cross",
            Url = "https://us.battle.net/d3/en/item/blackthornes-duncraig-cross",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_016_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blackthornes-duncraig-cross",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "黑棘的战铠",
        };

        /// <summary>
        /// Tal Rasha's Allegiance 
        /// </summary>
        public static Item TalRashasAllegiance = new Item
        {
            Id = 222486,
            Name = "塔·拉夏的誓言",
            Quality = ItemQuality.Legendary,
            Slug = "tal-rashas-allegiance",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Amulet_norm_unique_14",
            DataUrl = "https://us.battle.net/api/d3/data/item/tal-rashas-allegiance",
            Url = "https://us.battle.net/d3/en/item/tal-rashas-allegiance",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tal-rashas-allegiance",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔·拉夏的法理",
        };

        /// <summary>
        /// The Traveler's Pledge 
        /// </summary>
        public static Item TheTravelersPledge = new Item
        {
            Id = 222490,
            Name = "旅者之誓",
            Quality = ItemQuality.Legendary,
            Slug = "the-travelers-pledge",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Amulet_norm_unique_15",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-travelers-pledge",
            Url = "https://us.battle.net/d3/en/item/the-travelers-pledge",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_008_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-travelers-pledge",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "无尽之途",
        };

        /// <summary>
        /// Halcyon's Ascent 
        /// </summary>
        public static Item HalcyonsAscent = new Item
        {
            Id = 298056,
            Name = "赫西恩之飞升",
            Quality = ItemQuality.Legendary,
            Slug = "halcyons-ascent",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "amulet_norm_unique_25",
            DataUrl = "https://us.battle.net/api/d3/data/item/halcyons-ascent",
            Url = "https://us.battle.net/d3/en/item/halcyons-ascent",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_109_x1_210_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/halcyons-ascent",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// The Johnstone Each corpse consumed in the Land of the Dead grants a stack of Macabre Knowledge. Macabre Knowledge increases the damage of Corpse Lance and Corpse Explosion by 150–200 % while outside Land of the Dead.
        /// </summary>
        public static Item TheJohnstone = new Item
        {
            Id = 476716,
            Name = "约翰之石",
            Quality = ItemQuality.Legendary,
            Slug = "the-johnstone",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-johnstone",
            Url = "https://us.battle.net/d3/en/item/the-johnstone",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_amulet_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-johnstone",
            IsCrafted = false,
            LegendaryAffix = "在亡者领域结束后获得50层禁忌学识，你的尸矛和邪爆会消耗一层禁忌学识并使其伤害提高150-200%。",
            SetName = "",
        };

        /// <summary>
        /// Haunted Visions Simulacrum now drains 5 % of your maximum life every second and lasts twice as long.
        /// </summary>
        public static Item HauntedVisions = new Item
        {
            Id = 476717,
            Name = "鬼灵面容",
            Quality = ItemQuality.Legendary,
            Slug = "haunted-visions",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/haunted-visions",
            Url = "https://us.battle.net/d3/en/item/haunted-visions",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_amulet_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/haunted-visions",
            IsCrafted = false,
            LegendaryAffix = "血魂双分现在每秒会消耗你最大生命值的 5%，且持续时间翻倍。",
            SetName = "",
        };

        /// <summary>
        /// Sunwuko's Shines 
        /// </summary>
        public static Item SunwukosShines = new Item
        {
            Id = 336174,
            Name = "孙武空的戏法",
            Quality = ItemQuality.Legendary,
            Slug = "sunwukos-shines",
            ItemType = ItemType.Amulet,
            TrinityItemType = TrinityItemType.Amulet,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "amulet_norm_set_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/sunwukos-shines",
            Url = "https://us.battle.net/d3/en/item/sunwukos-shines",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_amulet_set_11_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sunwukos-shines",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "猴王战甲",
        };

        ///// <summary>
        ///// Talisman of Akkhan 
        ///// </summary>
        //public static Item TalismanOfAkkhan = new Item
        //{
        //    Id = 455735,
        //    Name = "阿克汉的护符",
        //    Quality = ItemQuality.Legendary,
        //    Slug = "talisman-of-akkhan",
        //    ItemType = ItemType.Amulet,
        //    TrinityItemType = TrinityItemType.Amulet,
        //    IsTwoHanded = false,
        //    BaseType = ItemBaseType.Jewelry,
        //    InternalName = "",
        //    DataUrl = "https://us.battle.net/api/d3/data/item/talisman-of-akkhan",
        //    Url = "https://us.battle.net/d3/en/item/talisman-of-akkhan",
        //    IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p43_akkhanset_amulet_demonhunter_male.png",
        //    RelativeUrl = "/d3/en/item/talisman-of-akkhan",
        //    IsCrafted = false,
        //    LegendaryAffix = "",
        //    SetName = "阿克汉的战甲",
        //};

        /// <summary>
        /// Lut Socks Leap can be cast up to three times within 2 seconds before the cooldown begins.
        /// </summary>
        public static Item LutSocks = new Item
        {
            Id = 205622,
            Name = "掳宝长靴",
            Quality = ItemQuality.Legendary,
            Slug = "lut-socks",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Boots_norm_unique_077",
            DataUrl = "https://us.battle.net/api/d3/data/item/lut-socks",
            Url = "https://us.battle.net/d3/en/item/lut-socks",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/lut-socks",
            IsCrafted = false,
            LegendaryAffix = "跳斩可在 2 秒内最多施放三次，然后才触发冷却时间。",
            SetName = "",
        };

        /// <summary>
        /// Rivera Dancers Lashing Tail Kick attacks 50% faster and deals 250–300 % increased damage.
        /// </summary>
        public static Item RiveraDancers = new Item
        {
            Id = 197224,
            Name = "里维拉的舞鞋",
            Quality = ItemQuality.Legendary,
            Slug = "rivera-dancers",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/rivera-dancers",
            Url = "https://us.battle.net/d3/en/item/rivera-dancers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_boots_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rivera-dancers",
            IsCrafted = false,
            LegendaryAffix = "神龙摆尾攻击速度加快 50%，造成的伤害提高 (250-300)%。",
            SetName = "",
        };

        /// <summary>
        /// The Crudest Boots Mystic Ally summons two Mystic Allies that fight by your side.
        /// </summary>
        public static Item TheCrudestBoots = new Item
        {
            Id = 205620,
            Name = "粗糙至极靴",
            Quality = ItemQuality.Legendary,
            Slug = "the-crudest-boots",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Boots_norm_unique_075",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-crudest-boots",
            Url = "https://us.battle.net/d3/en/item/the-crudest-boots",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p1_unique_boots_010_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-crudest-boots",
            IsCrafted = false,
            LegendaryAffix = "幻身会召唤2个幻身与你并肩战斗。",
            SetName = "",
        };

        /// <summary>
        /// Boots of Disregard Gain 10000 Life regeneration per Second for each second you stand still. This effect stacks up to 4 times.
        /// </summary>
        public static Item BootsOfDisregard = new Item
        {
            Id = 322905,
            Name = "蔑视长靴",
            Quality = ItemQuality.Legendary,
            Slug = "boots-of-disregard",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/boots-of-disregard",
            Url = "https://us.battle.net/d3/en/item/boots-of-disregard",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/boots-of-disregard",
            IsCrafted = false,
            LegendaryAffix = "站定不动时可获得10000点的每秒生命值恢复。该效果最多可叠加4次。",
            SetName = "",
        };

        /// <summary>
        /// Illusory Boots You may move unhindered through enemies.
        /// </summary>
        public static Item IllusoryBoots = new Item
        {
            Id = 332342,
            Name = "虚幻长靴",
            Quality = ItemQuality.Legendary,
            Slug = "illusory-boots",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/illusory-boots",
            Url = "https://us.battle.net/d3/en/item/illusory-boots",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/illusory-boots",
            IsCrafted = false,
            LegendaryAffix = "你可以在敌人当中不受阻碍地移动。",
            SetName = "",
        };

        /// <summary>
        /// Irontoe Mudsputters Gain up to 25–30 % increased movement speed based on amount of Life missing.
        /// </summary>
        public static Item IrontoeMudsputters = new Item
        {
            Id = 339125,
            Name = "铁头防泥靴",
            Quality = ItemQuality.Legendary,
            Slug = "irontoe-mudsputters",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "boots_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/irontoe-mudsputters",
            Url = "https://us.battle.net/d3/en/item/irontoe-mudsputters",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/irontoe-mudsputters",
            IsCrafted = false,
            LegendaryAffix = "基于你损失的生命值，最多提高 (25-30)% 的移动速度。",
            SetName = "",
        };

        /// <summary>
        /// Bryner's Journey Attacking with Bone Spikes has a 20–30 % chance to cast a Bone Nova at the target location.
        /// </summary>
        public static Item BrynersJourney = new Item
        {
            Id = 467565,
            Name = "布莱纳的旅途",
            Quality = ItemQuality.Legendary,
            Slug = "bryners-journey",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bryners-journey",
            Url = "https://us.battle.net/d3/en/item/bryners-journey",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_boots_22_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bryners-journey",
            IsCrafted = false,
            LegendaryAffix = "骨刺有（20-30）%的几率在目标位置施放一次白骨新星。",
            SetName = "",
        };

        /// <summary>
        /// Fire Walkers Burn the ground you walk on, dealing 300–400 % weapon damage each second.
        /// </summary>
        public static Item FireWalkers = new Item
        {
            Id = 205624,
            Name = "踏焰长靴",
            Quality = ItemQuality.Legendary,
            Slug = "fire-walkers",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Boots_norm_unique_085",
            DataUrl = "https://us.battle.net/api/d3/data/item/fire-walkers",
            Url = "https://us.battle.net/d3/en/item/fire-walkers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_007_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fire-walkers",
            IsCrafted = false,
            LegendaryAffix = "引燃你踩踏而过的地面，每秒造成(300-400)%的武器伤害。",
            SetName = "",
        };

        /// <summary>
        /// Steuart's Greaves You gain 40–50 % increased movement speed for 2 seconds after using Blood Rush.
        /// </summary>
        public static Item SteuartsGreaves = new Item
        {
            Id = 467564,
            Name = "斯图亚特的护胫",
            Quality = ItemQuality.Legendary,
            Slug = "steuarts-greaves",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/steuarts-greaves",
            Url = "https://us.battle.net/d3/en/item/steuarts-greaves",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_boots_21_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/steuarts-greaves",
            IsCrafted = false,
            LegendaryAffix = "使用鲜血奔行之后移动速度提高（75-100）%，持续 10 秒。",
            SetName = "",
        };

        /// <summary>
        /// Ice Climbers Gain immunity to Freeze and Immobilize effects.
        /// </summary>
        public static Item IceClimbers = new Item
        {
            Id = 222464,
            Name = "攀冰者",
            Quality = ItemQuality.Legendary,
            Slug = "ice-climbers",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ice-climbers",
            Url = "https://us.battle.net/d3/en/item/ice-climbers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_008_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ice-climbers",
            IsCrafted = false,
            LegendaryAffix = "免疫冰冻和定身效果。",
            SetName = "",
        };

        /// <summary>
        /// Nilfur's Boast Increase the damage of Meteor by 200% . When your Meteor hits 3 or fewer enemies, the damage is increased by 275–350 % .
        /// </summary>
        public static Item NilfursBoast = new Item
        {
            Id = 415050,
            Name = "尼芙尔的夸耀",
            Quality = ItemQuality.Legendary,
            Slug = "nilfurs-boast",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/nilfurs-boast",
            Url = "https://us.battle.net/d3/en/item/nilfurs-boast",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_boots_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/nilfurs-boast",
            IsCrafted = false,
            LegendaryAffix = "陨石术的伤害提高 600%。当你的陨石击中 3 个或更少的敌人时，伤害提高 (675-900)%。",
            SetName = "",
        };

        /// <summary>
        /// Blackthorne's Spurs 
        /// </summary>
        public static Item BlackthornesSpurs = new Item
        {
            Id = 222463,
            Name = "黑棘的马刺靴",
            Quality = ItemQuality.Legendary,
            Slug = "blackthornes-spurs",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Boots_norm_unique_050",
            DataUrl = "https://us.battle.net/api/d3/data/item/blackthornes-spurs",
            Url = "https://us.battle.net/d3/en/item/blackthornes-spurs",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_019_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blackthornes-spurs",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "黑棘的战铠",
        };

        /// <summary>
        /// Immortal King's Stride 
        /// </summary>
        public static Item ImmortalKingsStride = new Item
        {
            Id = 205625,
            Name = "不朽之王的步履",
            Quality = ItemQuality.Legendary,
            Slug = "immortal-kings-stride",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Boots_norm_unique_086",
            DataUrl = "https://us.battle.net/api/d3/data/item/immortal-kings-stride",
            Url = "https://us.battle.net/d3/en/item/immortal-kings-stride",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_012_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/immortal-kings-stride",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不朽之王的呼唤",
        };

        /// <summary>
        /// Inna's Sandals 
        /// </summary>
        public static Item InnasSandals = new Item
        {
            Id = 415264,
            Name = "尹娜的风靴",
            Quality = ItemQuality.Legendary,
            Slug = "innas-sandals",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/innas-sandals",
            Url = "https://us.battle.net/d3/en/item/innas-sandals",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_boots_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/innas-sandals",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "尹娜的真言",
        };

        /// <summary>
        /// Natalya's Bloody Footprints 
        /// </summary>
        public static Item NatalyasBloodyFootprints = new Item
        {
            Id = 197223,
            Name = "娜塔亚的血足",
            Quality = ItemQuality.Legendary,
            Slug = "natalyas-bloody-footprints",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Boots_norm_unique_044",
            DataUrl = "https://us.battle.net/api/d3/data/item/natalyas-bloody-footprints",
            Url = "https://us.battle.net/d3/en/item/natalyas-bloody-footprints",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/natalyas-bloody-footprints",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "娜塔亚的复仇",
        };

        /// <summary>
        /// Zunimassa's Trail 
        /// </summary>
        public static Item ZunimassasTrail = new Item
        {
            Id = 205627,
            Name = "祖尼玛萨之途",
            Quality = ItemQuality.Legendary,
            Slug = "zunimassas-trail",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Boots_norm_unique_088",
            DataUrl = "https://us.battle.net/api/d3/data/item/zunimassas-trail",
            Url = "https://us.battle.net/d3/en/item/zunimassas-trail",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_013_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/zunimassas-trail",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "祖尼玛萨之魂",
        };

        /// <summary>
        /// Arachyr's Stride 
        /// </summary>
        public static Item ArachyrsStride = new Item
        {
            Id = 441195,
            Name = "亚拉基尔的步伐",
            Quality = ItemQuality.Legendary,
            Slug = "arachyrs-stride",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/arachyrs-stride",
            Url = "https://us.battle.net/d3/en/item/arachyrs-stride",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_02_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/arachyrs-stride",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "亚拉基尔的灵魂",
        };

        /// <summary>
        /// Eight-Demon Boots 
        /// </summary>
        public static Item EightdemonBoots = new Item
        {
            Id = 338031,
            Name = "八魔之靴",
            Quality = ItemQuality.Legendary,
            Slug = "eightdemon-boots",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/eightdemon-boots",
            Url = "https://us.battle.net/d3/en/item/eightdemon-boots",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_08_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/eightdemon-boots",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "千风飓",
        };

        /// <summary>
        /// Firebird's Tarsi 
        /// </summary>
        public static Item FirebirdsTarsi = new Item
        {
            Id = 358793,
            Name = "不死鸟之足",
            Quality = ItemQuality.Legendary,
            Slug = "firebirds-tarsi",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "boots_norm_set_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/firebirds-tarsi",
            Url = "https://us.battle.net/d3/en/item/firebirds-tarsi",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_06_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/firebirds-tarsi",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不死鸟的华服",
        };

        /// <summary>
        /// Foundation of the Earth 
        /// </summary>
        public static Item FoundationOfTheEarth = new Item
        {
            Id = 366888,
            Name = "大地之基",
            Quality = ItemQuality.Legendary,
            Slug = "foundation-of-the-earth",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/foundation-of-the-earth",
            Url = "https://us.battle.net/d3/en/item/foundation-of-the-earth",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_15_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/foundation-of-the-earth",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "大地之力",
        };

        /// <summary>
        /// Foundation of the Light 
        /// </summary>
        public static Item FoundationOfTheLight = new Item
        {
            Id = 408867,
            Name = "圣光之源",
            Quality = ItemQuality.Legendary,
            Slug = "foundation-of-the-light",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/foundation-of-the-light",
            Url = "https://us.battle.net/d3/en/item/foundation-of-the-light",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_03_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/foundation-of-the-light",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "圣光追寻者",
        };

        /// <summary>
        /// Hell Walkers 
        /// </summary>
        public static Item HellWalkers = new Item
        {
            Id = 408866,
            Name = "地狱行者",
            Quality = ItemQuality.Legendary,
            Slug = "hell-walkers",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/hell-walkers",
            Url = "https://us.battle.net/d3/en/item/hell-walkers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_03_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/hell-walkers",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "邪秽之精",
        };

        /// <summary>
        /// Helltooth Greaves 
        /// </summary>
        public static Item HelltoothGreaves = new Item
        {
            Id = 340524,
            Name = "魔牙胫甲",
            Quality = ItemQuality.Legendary,
            Slug = "helltooth-greaves",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "boots_norm_set_16",
            DataUrl = "https://us.battle.net/api/d3/data/item/helltooth-greaves",
            Url = "https://us.battle.net/d3/en/item/helltooth-greaves",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_16_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/helltooth-greaves",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "魔牙战装",
        };

        /// <summary>
        /// Inarius's Perseverance 
        /// </summary>
        public static Item InariussPerseverance = new Item
        {
            Id = 467563,
            Name = "伊纳瑞斯的坚毅",
            Quality = ItemQuality.Legendary,
            Slug = "inariuss-perseverance",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/inariuss-perseverance",
            Url = "https://us.battle.net/d3/en/item/inariuss-perseverance",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_3_boots_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/inariuss-perseverance",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "伊纳瑞斯的恩泽",
        };

        /// <summary>
        /// Jade Harvester's Swiftness 
        /// </summary>
        public static Item JadeHarvestersSwiftness = new Item
        {
            Id = 338037,
            Name = "玉魂师的迅捷",
            Quality = ItemQuality.Legendary,
            Slug = "jade-harvesters-swiftness",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "boots_norm_set_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/jade-harvesters-swiftness",
            Url = "https://us.battle.net/d3/en/item/jade-harvesters-swiftness",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_09_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jade-harvesters-swiftness",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "玉魂师的战甲",
        };

        /// <summary>
        /// Marauder's Treads 
        /// </summary>
        public static Item MaraudersTreads = new Item
        {
            Id = 336995,
            Name = "掠夺者的便鞋",
            Quality = ItemQuality.Legendary,
            Slug = "marauders-treads",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "boots_norm_set_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/marauders-treads",
            Url = "https://us.battle.net/d3/en/item/marauders-treads",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_07_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/marauders-treads",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "掠夺者的化身",
        };

        /// <summary>
        /// Pestilence Battle Boots 
        /// </summary>
        public static Item PestilenceBattleBoots = new Item
        {
            Id = 467350,
            Name = "死疫战靴",
            Quality = ItemQuality.Legendary,
            Slug = "pestilence-battle-boots",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/pestilence-battle-boots",
            Url = "https://us.battle.net/d3/en/item/pestilence-battle-boots",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_4_boots_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pestilence-battle-boots",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "死疫死者的裹布",
        };

        /// <summary>
        /// Raekor's Striders 
        /// </summary>
        public static Item RaekorsStriders = new Item
        {
            Id = 336987,
            Name = "蕾蔻的豪迈",
            Quality = ItemQuality.Legendary,
            Slug = "raekors-striders",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "boots_norm_set_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/raekors-striders",
            Url = "https://us.battle.net/d3/en/item/raekors-striders",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_05_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/raekors-striders",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "蕾蔻的传世铠",
        };

        /// <summary>
        /// Rathma's Ossified Sabatons 
        /// </summary>
        public static Item RathmasOssifiedSabatons = new Item
        {
            Id = 460917,
            Name = "拉斯玛的僵骨战靴",
            Quality = ItemQuality.Legendary,
            Slug = "rathmas-ossified-sabatons",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/rathmas-ossified-sabatons",
            Url = "https://us.battle.net/d3/en/item/rathmas-ossified-sabatons",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_1_boots_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rathmas-ossified-sabatons",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "拉斯玛的骨甲",
        };

        /// <summary>
        /// Roland's Stride 
        /// </summary>
        public static Item RolandsStride = new Item
        {
            Id = 404094,
            Name = "罗兰之步",
            Quality = ItemQuality.Legendary,
            Slug = "rolands-stride",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "p1_Boots_norm_set_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/rolands-stride",
            Url = "https://us.battle.net/d3/en/item/rolands-stride",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_01_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rolands-stride",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "罗兰的传世甲",
        };

        /// <summary>
        /// Sabaton of the Wastes 
        /// </summary>
        public static Item SabatonOfTheWastes = new Item
        {
            Id = 408859,
            Name = "荒原长靴",
            Quality = ItemQuality.Legendary,
            Slug = "sabaton-of-the-wastes",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/sabaton-of-the-wastes",
            Url = "https://us.battle.net/d3/en/item/sabaton-of-the-wastes",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_01_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sabaton-of-the-wastes",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "废土之怒",
        };

        /// <summary>
        /// Sabatons of Akkhan 
        /// </summary>
        public static Item SabatonsOfAkkhan = new Item
        {
            Id = 358795,
            Name = "阿克汉的钢靴",
            Quality = ItemQuality.Legendary,
            Slug = "sabatons-of-akkhan",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "boots_norm_set_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/sabatons-of-akkhan",
            Url = "https://us.battle.net/d3/en/item/sabatons-of-akkhan",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_10_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sabatons-of-akkhan",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "阿克汉的战甲",
        };

        /// <summary>
        /// Striders of Destiny 
        /// </summary>
        public static Item StridersOfDestiny = new Item
        {
            Id = 408863,
            Name = "命运阔步靴",
            Quality = ItemQuality.Legendary,
            Slug = "striders-of-destiny",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/striders-of-destiny",
            Url = "https://us.battle.net/d3/en/item/striders-of-destiny",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_02_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/striders-of-destiny",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "德尔西尼的杰作",
        };

        /// <summary>
        /// The Shadow's Heels 
        /// </summary>
        public static Item TheShadowsHeels = new Item
        {
            Id = 332364,
            Name = "影踵",
            Quality = ItemQuality.Legendary,
            Slug = "the-shadows-heels",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "boots_norm_set_14",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-shadows-heels",
            Url = "https://us.battle.net/d3/en/item/the-shadows-heels",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_14_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-shadows-heels",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "暗影装束",
        };

        /// <summary>
        /// Trag'Oul's Stalwart Greaves 
        /// </summary>
        public static Item TragoulsStalwartGreaves = new Item
        {
            Id = 467562,
            Name = "塔格奥之胫",
            Quality = ItemQuality.Legendary,
            Slug = "tragouls-stalwart-greaves",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/tragouls-stalwart-greaves",
            Url = "https://us.battle.net/d3/en/item/tragouls-stalwart-greaves",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_2_boots_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tragouls-stalwart-greaves",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔格奥的化身",
        };

        /// <summary>
        /// Uliana's Destiny 
        /// </summary>
        public static Item UlianasDestiny = new Item
        {
            Id = 408864,
            Name = "乌莲娜的天命",
            Quality = ItemQuality.Legendary,
            Slug = "ulianas-destiny",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ulianas-destiny",
            Url = "https://us.battle.net/d3/en/item/ulianas-destiny",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_01_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ulianas-destiny",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "乌莲娜的谋略",
        };

        /// <summary>
        /// Vyr's Swaggering Stance 
        /// </summary>
        public static Item VyrsSwaggeringStance = new Item
        {
            Id = 332363,
            Name = "维尔的昂扬姿态",
            Quality = ItemQuality.Legendary,
            Slug = "vyrs-swaggering-stance",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "boots_norm_set_13",
            DataUrl = "https://us.battle.net/api/d3/data/item/vyrs-swaggering-stance",
            Url = "https://us.battle.net/d3/en/item/vyrs-swaggering-stance",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_13_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vyrs-swaggering-stance",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "维尔的神装",
        };

        /// <summary>
        /// Zeal of the Invoker 
        /// </summary>
        public static Item ZealOfTheInvoker = new Item
        {
            Id = 442731,
            Name = "唤魔师的热忱",
            Quality = ItemQuality.Legendary,
            Slug = "zeal-of-the-invoker",
            ItemType = ItemType.Boots,
            TrinityItemType = TrinityItemType.Boots,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/zeal-of-the-invoker",
            Url = "https://us.battle.net/d3/en/item/zeal-of-the-invoker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_boots_set_12_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/zeal-of-the-invoker",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "唤魔师的荆棘",
        };

        /// <summary>
        /// Pox Faulds When 3 or more enemies are within 12 yards, you release a vile stench that deals 450–550 % weapon damage as Poison every second for 5 seconds to enemies within 15 yards.
        /// </summary>
        public static Item PoxFaulds = new Item
        {
            Id = 197220,
            Name = "恶疮马裤",
            Quality = ItemQuality.Legendary,
            Slug = "pox-faulds",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_unique_048",
            DataUrl = "https://us.battle.net/api/d3/data/item/pox-faulds",
            Url = "https://us.battle.net/d3/en/item/pox-faulds",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_007_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pox-faulds",
            IsCrafted = false,
            LegendaryAffix = "当12码内有3个或更多敌人时，你会释放出一阵恶臭，每秒对15码内的敌人造成 (450-550)% 的武器伤害（作为毒性伤害），持续5秒。",
            SetName = "",
        };

        /// <summary>
        /// Death's Bargain Gain an aura of death that deals 750–1000 % of your Life per Second as Physical damage to enemies within 16 yards. You no longer regenerate Life.
        /// </summary>
        public static Item DeathsBargain = new Item
        {
            Id = 332205,
            Name = "死神赌注",
            Quality = ItemQuality.Legendary,
            Slug = "deaths-bargain",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/deaths-bargain",
            Url = "https://us.battle.net/d3/en/item/deaths-bargain",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/deaths-bargain",
            IsCrafted = false,
            LegendaryAffix = "获得一道死亡光环，对 16 码内的敌人造成相当于你每秒回复生命 (750-1000)% 的伤害。你不再回复生命值。",
            SetName = "",
        };

        /// <summary>
        /// Golemskin Breeches Your Golem's damage is increased by 100–125 % and you take 30% less damage while it is alive.
        /// </summary>
        public static Item GolemskinBreeches = new Item
        {
            Id = 467576,
            Name = "傀儡大师的马裤",
            Quality = ItemQuality.Legendary,
            Slug = "golemskin-breeches",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/golemskin-breeches",
            Url = "https://us.battle.net/d3/en/item/golemskin-breeches",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_pants_21_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/golemskin-breeches",
            IsCrafted = false,
            LegendaryAffix = "号令傀儡的冷却时间减少 20-25 秒，并且当你的傀儡存活时，你受到的伤害降低30%。",
            SetName = "",
        };

        /// <summary>
        /// Hammer Jammers Enemies take 300–400 % increased damage from your Blessed Hammers for 10 seconds after you hit them with a Blind, Immobilize, or Stun.
        /// </summary>
        public static Item HammerJammers = new Item
        {
            Id = 209059,
            Name = "汉默长裤",
            Quality = ItemQuality.Legendary,
            Slug = "hammer-jammers",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_unique_077",
            DataUrl = "https://us.battle.net/api/d3/data/item/hammer-jammers",
            Url = "https://us.battle.net/d3/en/item/hammer-jammers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_pants_002_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/hammer-jammers",
            IsCrafted = false,
            LegendaryAffix = "当敌人受到你的致盲、定身或昏迷效果时，你的祝福之锤对其造成的伤害提高 (300-400)%，持续 10 秒。",
            SetName = "",
        };

        /// <summary>
        /// Hexing Pants of Mr. Yan Your resource generation and damage is increased by 25% while moving and decreased by 20–25 % while standing still.
        /// </summary>
        public static Item HexingPantsOfMrYan = new Item
        {
            Id = 332204,
            Name = "杨先生的妖法裤",
            Quality = ItemQuality.Legendary,
            Slug = "hexing-pants-of-mr-yan",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/hexing-pants-of-mr-yan",
            Url = "https://us.battle.net/d3/en/item/hexing-pants-of-mr-yan",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/hexing-pants-of-mr-yan",
            IsCrafted = false,
            LegendaryAffix = "移动时你的能量生成速度和造成的伤害提高25%，静止不动时降低 (20-25)% 。",
            SetName = "",
        };

        /// <summary>
        /// Defiler Cuisses Your Bone Spirit's damage is increased by 75–100 % for every second it is active.
        /// </summary>
        public static Item DefilerCuisses = new Item
        {
            Id = 467577,
            Name = "亵渎者的护腿",
            Quality = ItemQuality.Legendary,
            Slug = "defiler-cuisses",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/defiler-cuisses",
            Url = "https://us.battle.net/d3/en/item/defiler-cuisses",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_unique_pants_22_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/defiler-cuisses",
            IsCrafted = false,
            LegendaryAffix = "骨魂在激活状态下每持续一秒，其造成的伤害就会提高（400-500）%。",
            SetName = "",
        };

        /// <summary>
        /// Swamp Land Waders Sacrifice deals 300–400 % additional damage against enemies affected by Locust Swarm or Grasp of the Dead.
        /// </summary>
        public static Item SwampLandWaders = new Item
        {
            Id = 209057,
            Name = "沼地防水裤",
            Quality = ItemQuality.Legendary,
            Slug = "swamp-land-waders",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/swamp-land-waders",
            Url = "https://us.battle.net/d3/en/item/swamp-land-waders",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_pants_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/swamp-land-waders",
            IsCrafted = false,
            LegendaryAffix = "牺牲对受到瘟疫虫群和亡者之握影响的敌人，造成 (300-400)% 的额外伤害。",
            SetName = "",
        };

        /// <summary>
        /// Depth Diggers Primary skills that generate resource deal 80–100 % additional damage.
        /// </summary>
        public static Item DepthDiggers = new Item
        {
            Id = 197216,
            Name = "深渊挖掘裤",
            Quality = ItemQuality.Legendary,
            Slug = "depth-diggers",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_unique_044",
            DataUrl = "https://us.battle.net/api/d3/data/item/depth-diggers",
            Url = "https://us.battle.net/d3/en/item/depth-diggers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_006_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/depth-diggers",
            IsCrafted = false,
            LegendaryAffix = "生成能量的主要技能造成 (80-100)% 的额外伤害。",
            SetName = "",
        };

        /// <summary>
        /// Blackthorne's Jousting Mail 
        /// </summary>
        public static Item BlackthornesJoustingMail = new Item
        {
            Id = 222477,
            Name = "黑棘的决斗锁甲护腿",
            Quality = ItemQuality.Legendary,
            Slug = "blackthornes-jousting-mail",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_unique_050",
            DataUrl = "https://us.battle.net/api/d3/data/item/blackthornes-jousting-mail",
            Url = "https://us.battle.net/d3/en/item/blackthornes-jousting-mail",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_013_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blackthornes-jousting-mail",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "黑棘的战铠",
        };

        /// <summary>
        /// Immortal King's Stature 
        /// </summary>
        public static Item ImmortalKingsStature = new Item
        {
            Id = 205645,
            Name = "不朽之王的威仪",
            Quality = ItemQuality.Legendary,
            Slug = "immortal-kings-stature",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/immortal-kings-stature",
            Url = "https://us.battle.net/d3/en/item/immortal-kings-stature",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_pants_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/immortal-kings-stature",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不朽之王的呼唤",
        };

        /// <summary>
        /// Inna's Temperance 
        /// </summary>
        public static Item InnasTemperance = new Item
        {
            Id = 205646,
            Name = "尹娜的戒律",
            Quality = ItemQuality.Legendary,
            Slug = "innas-temperance",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_unique_087",
            DataUrl = "https://us.battle.net/api/d3/data/item/innas-temperance",
            Url = "https://us.battle.net/d3/en/item/innas-temperance",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_008_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/innas-temperance",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "尹娜的真言",
        };

        /// <summary>
        /// Natalya's Leggings 
        /// </summary>
        public static Item NatalyasLeggings = new Item
        {
            Id = 415282,
            Name = "娜塔亚的长裤",
            Quality = ItemQuality.Legendary,
            Slug = "natalyas-leggings",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/natalyas-leggings",
            Url = "https://us.battle.net/d3/en/item/natalyas-leggings",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_pants_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/natalyas-leggings",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "娜塔亚的复仇",
        };

        /// <summary>
        /// Tal Rasha's Stride 
        /// </summary>
        public static Item TalRashasStride = new Item
        {
            Id = 415049,
            Name = "塔·拉夏的步伐",
            Quality = ItemQuality.Legendary,
            Slug = "tal-rashas-stride",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/tal-rashas-stride",
            Url = "https://us.battle.net/d3/en/item/tal-rashas-stride",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_pants_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tal-rashas-stride",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔·拉夏的法理",
        };

        /// <summary>
        /// Zunimassa's Cloth 
        /// </summary>
        public static Item ZunimassasCloth = new Item
        {
            Id = 205647,
            Name = "祖尼玛萨之裤",
            Quality = ItemQuality.Legendary,
            Slug = "zunimassas-cloth",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/zunimassas-cloth",
            Url = "https://us.battle.net/d3/en/item/zunimassas-cloth",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_pants_04_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/zunimassas-cloth",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "祖尼玛萨之魂",
        };

        /// <summary>
        /// Arachyr's Legs 
        /// </summary>
        public static Item ArachyrsLegs = new Item
        {
            Id = 441194,
            Name = "亚拉基尔的胫腿",
            Quality = ItemQuality.Legendary,
            Slug = "arachyrs-legs",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/arachyrs-legs",
            Url = "https://us.battle.net/d3/en/item/arachyrs-legs",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_02_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/arachyrs-legs",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "亚拉基尔的灵魂",
        };

        /// <summary>
        /// Cuisses of Akkhan 
        /// </summary>
        public static Item CuissesOfAkkhan = new Item
        {
            Id = 358800,
            Name = "阿克汉的腿甲",
            Quality = ItemQuality.Legendary,
            Slug = "cuisses-of-akkhan",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_set_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/cuisses-of-akkhan",
            Url = "https://us.battle.net/d3/en/item/cuisses-of-akkhan",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_10_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/cuisses-of-akkhan",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "阿克汉的战甲",
        };

        /// <summary>
        /// Firebird's Down 
        /// </summary>
        public static Item FirebirdsDown = new Item
        {
            Id = 358790,
            Name = "不死鸟之腹",
            Quality = ItemQuality.Legendary,
            Slug = "firebirds-down",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_set_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/firebirds-down",
            Url = "https://us.battle.net/d3/en/item/firebirds-down",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_06_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/firebirds-down",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不死鸟的华服",
        };

        /// <summary>
        /// Helltooth Leg Guards 
        /// </summary>
        public static Item HelltoothLegGuards = new Item
        {
            Id = 340522,
            Name = "魔牙腿甲",
            Quality = ItemQuality.Legendary,
            Slug = "helltooth-leg-guards",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_set_16",
            DataUrl = "https://us.battle.net/api/d3/data/item/helltooth-leg-guards",
            Url = "https://us.battle.net/d3/en/item/helltooth-leg-guards",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_16_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/helltooth-leg-guards",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "魔牙战装",
        };

        /// <summary>
        /// Inarius's Reticence 
        /// </summary>
        public static Item InariussReticence = new Item
        {
            Id = 467575,
            Name = "伊纳瑞斯的沉默",
            Quality = ItemQuality.Legendary,
            Slug = "inariuss-reticence",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/inariuss-reticence",
            Url = "https://us.battle.net/d3/en/item/inariuss-reticence",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_3_pants_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/inariuss-reticence",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "伊纳瑞斯的恩泽",
        };

        /// <summary>
        /// Jade Harvester's Courage 
        /// </summary>
        public static Item JadeHarvestersCourage = new Item
        {
            Id = 338041,
            Name = "玉魂师的勇气",
            Quality = ItemQuality.Legendary,
            Slug = "jade-harvesters-courage",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_set_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/jade-harvesters-courage",
            Url = "https://us.battle.net/d3/en/item/jade-harvesters-courage",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_09_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jade-harvesters-courage",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "玉魂师的战甲",
        };

        /// <summary>
        /// Leg Guards of Mystery 
        /// </summary>
        public static Item LegGuardsOfMystery = new Item
        {
            Id = 408878,
            Name = "神秘腿甲",
            Quality = ItemQuality.Legendary,
            Slug = "leg-guards-of-mystery",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/leg-guards-of-mystery",
            Url = "https://us.battle.net/d3/en/item/leg-guards-of-mystery",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_02_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/leg-guards-of-mystery",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "德尔西尼的杰作",
        };

        /// <summary>
        /// Marauder's Encasement 
        /// </summary>
        public static Item MaraudersEncasement = new Item
        {
            Id = 336993,
            Name = "掠夺者的护腿",
            Quality = ItemQuality.Legendary,
            Slug = "marauders-encasement",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/marauders-encasement",
            Url = "https://us.battle.net/d3/en/item/marauders-encasement",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_07_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/marauders-encasement",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "掠夺者的化身",
        };

        /// <summary>
        /// Pestilence Incantations 
        /// </summary>
        public static Item PestilenceIncantations = new Item
        {
            Id = 467353,
            Name = "死疫化身",
            Quality = ItemQuality.Legendary,
            Slug = "pestilence-incantations",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/pestilence-incantations",
            Url = "https://us.battle.net/d3/en/item/pestilence-incantations",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_4_pants_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pestilence-incantations",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "死疫死者的裹布",
        };

        /// <summary>
        /// Raekor's Breeches 
        /// </summary>
        public static Item RaekorsBreeches = new Item
        {
            Id = 336986,
            Name = "蕾蔻的马裤",
            Quality = ItemQuality.Legendary,
            Slug = "raekors-breeches",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_set_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/raekors-breeches",
            Url = "https://us.battle.net/d3/en/item/raekors-breeches",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_05_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/raekors-breeches",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "蕾蔻的传世铠",
        };

        /// <summary>
        /// Rathma's Skeletal Legplates 
        /// </summary>
        public static Item RathmasSkeletalLegplates = new Item
        {
            Id = 460918,
            Name = "拉斯玛的骷髅腿甲",
            Quality = ItemQuality.Legendary,
            Slug = "rathmas-skeletal-legplates",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/rathmas-skeletal-legplates",
            Url = "https://us.battle.net/d3/en/item/rathmas-skeletal-legplates",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_1_pants_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rathmas-skeletal-legplates",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "拉斯玛的骨甲",
        };

        /// <summary>
        /// Renewal of the Invoker 
        /// </summary>
        public static Item RenewalOfTheInvoker = new Item
        {
            Id = 442732,
            Name = "唤魔师的新生",
            Quality = ItemQuality.Legendary,
            Slug = "renewal-of-the-invoker",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/renewal-of-the-invoker",
            Url = "https://us.battle.net/d3/en/item/renewal-of-the-invoker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_12_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/renewal-of-the-invoker",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "唤魔师的荆棘",
        };

        /// <summary>
        /// Roland's Determination 
        /// </summary>
        public static Item RolandsDetermination = new Item
        {
            Id = 404097,
            Name = "罗兰之心",
            Quality = ItemQuality.Legendary,
            Slug = "rolands-determination",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "p1_Pants_norm_set_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/rolands-determination",
            Url = "https://us.battle.net/d3/en/item/rolands-determination",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_01_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rolands-determination",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "罗兰的传世甲",
        };

        /// <summary>
        /// Scales of the Dancing Serpent 
        /// </summary>
        public static Item ScalesOfTheDancingSerpent = new Item
        {
            Id = 338035,
            Name = "舞蛇鳞甲",
            Quality = ItemQuality.Legendary,
            Slug = "scales-of-the-dancing-serpent",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_set_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/scales-of-the-dancing-serpent",
            Url = "https://us.battle.net/d3/en/item/scales-of-the-dancing-serpent",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_08_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/scales-of-the-dancing-serpent",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "千风飓",
        };

        /// <summary>
        /// Sunwuko's Leggings 
        /// </summary>
        public static Item SunwukosLeggings = new Item
        {
            Id = 429075,
            Name = "孙武空的护腿",
            Quality = ItemQuality.Legendary,
            Slug = "sunwukos-leggings",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/sunwukos-leggings",
            Url = "https://us.battle.net/d3/en/item/sunwukos-leggings",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_11_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sunwukos-leggings",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "猴王战甲",
        };

        /// <summary>
        /// Tasset of the Wastes 
        /// </summary>
        public static Item TassetOfTheWastes = new Item
        {
            Id = 408862,
            Name = "荒原腿甲",
            Quality = ItemQuality.Legendary,
            Slug = "tasset-of-the-wastes",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/tasset-of-the-wastes",
            Url = "https://us.battle.net/d3/en/item/tasset-of-the-wastes",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_01_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tasset-of-the-wastes",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "废土之怒",
        };

        /// <summary>
        /// The Shadow's Coil 
        /// </summary>
        public static Item TheShadowsCoil = new Item
        {
            Id = 332361,
            Name = "影拥",
            Quality = ItemQuality.Legendary,
            Slug = "the-shadows-coil",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_set_14",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-shadows-coil",
            Url = "https://us.battle.net/d3/en/item/the-shadows-coil",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_14_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-shadows-coil",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "暗影装束",
        };

        /// <summary>
        /// Towers of the Light 
        /// </summary>
        public static Item TowersOfTheLight = new Item
        {
            Id = 408882,
            Name = "圣光之塔",
            Quality = ItemQuality.Legendary,
            Slug = "towers-of-the-light",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/towers-of-the-light",
            Url = "https://us.battle.net/d3/en/item/towers-of-the-light",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_03_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/towers-of-the-light",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "圣光追寻者",
        };

        /// <summary>
        /// Trag'Oul's Hide 
        /// </summary>
        public static Item TragoulsHide = new Item
        {
            Id = 467574,
            Name = "塔格奥之甲",
            Quality = ItemQuality.Legendary,
            Slug = "tragouls-hide",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/tragouls-hide",
            Url = "https://us.battle.net/d3/en/item/tragouls-hide",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_necro_set_2_pants_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tragouls-hide",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔格奥的化身",
        };

        /// <summary>
        /// Uliana's Burden 
        /// </summary>
        public static Item UlianasBurden = new Item
        {
            Id = 408879,
            Name = "乌莲娜的忧虑",
            Quality = ItemQuality.Legendary,
            Slug = "ulianas-burden",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ulianas-burden",
            Url = "https://us.battle.net/d3/en/item/ulianas-burden",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_01_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ulianas-burden",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "乌莲娜的谋略",
        };

        /// <summary>
        /// Unholy Plates 
        /// </summary>
        public static Item UnholyPlates = new Item
        {
            Id = 408881,
            Name = "不洁腿甲",
            Quality = ItemQuality.Legendary,
            Slug = "unholy-plates",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/unholy-plates",
            Url = "https://us.battle.net/d3/en/item/unholy-plates",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_03_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/unholy-plates",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "邪秽之精",
        };

        /// <summary>
        /// Vyr's Fantastic Finery 
        /// </summary>
        public static Item VyrsFantasticFinery = new Item
        {
            Id = 332360,
            Name = "维尔的精美丝裤",
            Quality = ItemQuality.Legendary,
            Slug = "vyrs-fantastic-finery",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_set_13",
            DataUrl = "https://us.battle.net/api/d3/data/item/vyrs-fantastic-finery",
            Url = "https://us.battle.net/d3/en/item/vyrs-fantastic-finery",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_13_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vyrs-fantastic-finery",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "维尔的神装",
        };

        /// <summary>
        /// Weight of the Earth 
        /// </summary>
        public static Item WeightOfTheEarth = new Item
        {
            Id = 340521,
            Name = "大地之重",
            Quality = ItemQuality.Legendary,
            Slug = "weight-of-the-earth",
            ItemType = ItemType.Legs,
            TrinityItemType = TrinityItemType.Legs,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "pants_norm_set_15",
            DataUrl = "https://us.battle.net/api/d3/data/item/weight-of-the-earth",
            Url = "https://us.battle.net/d3/en/item/weight-of-the-earth",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_pants_set_15_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/weight-of-the-earth",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "大地之力",
        };

        /// <summary>
        /// Band of Might After casting Furious Charge, Ground Stomp, or Leap, take 50–60 % reduced damage for 8 seconds.
        /// </summary>
        public static Item BandOfMight = new Item
        {
            Id = 197839,
            Name = "力量指环",
            Quality = ItemQuality.Legendary,
            Slug = "band-of-might",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/band-of-might",
            Url = "https://us.battle.net/d3/en/item/band-of-might",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_ring_05_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/band-of-might",
            IsCrafted = false,
            LegendaryAffix = "施放狂暴冲撞、大地践踏、跳斩后，受到的伤害减少 (60-80%)%，持续 8 秒。",
            SetName = "",
        };

        /// <summary>
        /// Circle of Nailuj's Evol You now raise an additional Skeletal Mage with each cast and they last an additional 2–4 seconds.
        /// </summary>
        public static Item CircleOfNailujsEvol = new Item
        {
            Id = 476592,
            Name = "奈鲁维的轮回",
            Quality = ItemQuality.Legendary,
            Slug = "circle-of-nailujs-evol",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/circle-of-nailujs-evol",
            Url = "https://us.battle.net/d3/en/item/circle-of-nailujs-evol",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_ring_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/circle-of-nailujs-evol",
            IsCrafted = false,
            LegendaryAffix = "现在每次可以复活一个额外的骷髅法师并且其持续时间额外延长（2-4）秒。",
            SetName = "",
        };

        /// <summary>
        /// Avarice Band Each time you pick up gold, increase your Gold and Health Pickup radius by 1 yard for 10 seconds, stacking up to 30 times.
        /// </summary>
        public static Item AvariceBand = new Item
        {
            Id = 298095,
            Name = "贪婪之戒",
            Quality = ItemQuality.Legendary,
            Slug = "avarice-band",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/avarice-band",
            Url = "https://us.battle.net/d3/en/item/avarice-band",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_108_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/avarice-band",
            IsCrafted = false,
            LegendaryAffix = "每当你拾取金币时，会使你的金币和生命球拾取范围扩大1码，持续10秒，最多可叠加30次。",
            SetName = "",
        };

        /// <summary>
        /// Leoric's Signet 
        /// </summary>
        public static Item LeoricsSignet = new Item
        {
            Id = 197835,
            Name = "李奥瑞克的玺戒",
            Quality = ItemQuality.Legendary,
            Slug = "leorics-signet",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_002",
            DataUrl = "https://us.battle.net/api/d3/data/item/leorics-signet",
            Url = "https://us.battle.net/d3/en/item/leorics-signet",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/leorics-signet",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Pandemonium Loop Enemies slain while Feared die in a bloody explosion and cause other nearby enemies to flee in Fear.
        /// </summary>
        public static Item PandemoniumLoop = new Item
        {
            Id = 298096,
            Name = "混沌之环",
            Quality = ItemQuality.Legendary,
            Slug = "pandemonium-loop",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/pandemonium-loop",
            Url = "https://us.battle.net/d3/en/item/pandemonium-loop",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_109_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pandemonium-loop",
            IsCrafted = false,
            LegendaryAffix = "在恐惧持续期间被消灭的敌人会血腥爆炸，造成800%武器伤害并使附近的敌人因恐惧而逃跑。",
            SetName = "",
        };

        /// <summary>
        /// Ring of Royal Grandeur Reduces the number of items needed for set bonuses by 1 (to a minimum of 2).
        /// </summary>
        public static Item RingOfRoyalGrandeur = new Item
        {
            Id = 298094,
            Name = "皇家华戒",
            Quality = ItemQuality.Legendary,
            Slug = "ring-of-royal-grandeur",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ring-of-royal-grandeur",
            Url = "https://us.battle.net/d3/en/item/ring-of-royal-grandeur",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_107_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ring-of-royal-grandeur",
            IsCrafted = false,
            LegendaryAffix = "套装加成效果所需的装备数量降低1件（最少为2件）",
            SetName = "",
        };

        /// <summary>
        /// Manald Heal Enemies stunned with Paralysis also take 13000–14000 % weapon damage as Lightning.
        /// </summary>
        public static Item ManaldHeal = new Item
        {
            Id = 212546,
            Name = "马纳德的治疗",
            Quality = ItemQuality.Legendary,
            Slug = "manald-heal",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_021",
            DataUrl = "https://us.battle.net/api/d3/data/item/manald-heal",
            Url = "https://us.battle.net/d3/en/item/manald-heal",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p43_unique_ring_021_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/manald-heal",
            IsCrafted = false,
            LegendaryAffix = "被瘫痪击晕的敌人还会承受 (13000-14000)% 的武器伤害（作为闪电伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Broken Promises After 5 consecutive non-critical hits, your chance to critically hit is increased to 100% for 3 seconds.
        /// </summary>
        public static Item BrokenPromises = new Item
        {
            Id = 212589,
            Name = "破碎誓言",
            Quality = ItemQuality.Legendary,
            Slug = "broken-promises",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_006",
            DataUrl = "https://us.battle.net/api/d3/data/item/broken-promises",
            Url = "https://us.battle.net/d3/en/item/broken-promises",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_006_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/broken-promises",
            IsCrafted = false,
            LegendaryAffix = "连续5次非暴击攻击后，你的暴击几率提高至100%，持续3秒。",
            SetName = "",
        };

        /// <summary>
        /// Puzzle Ring Summon a treasure goblin who picks up normal-quality items for you. After picking up 12–16 items, he drops a rare item with a chance for a legendary.
        /// </summary>
        public static Item PuzzleRing = new Item
        {
            Id = 197837,
            Name = "机械指环",
            Quality = ItemQuality.Legendary,
            Slug = "puzzle-ring",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_004",
            DataUrl = "https://us.battle.net/api/d3/data/item/puzzle-ring",
            Url = "https://us.battle.net/d3/en/item/puzzle-ring",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_004_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/puzzle-ring",
            IsCrafted = false,
            LegendaryAffix = "召唤一只盗宝地精，为你拾取普通品质的物品。在拾取 (12-16) 件物品后，他会掉落一件稀有物品，且有一定几率掉落传奇物品。",
            SetName = "",
        };

        /// <summary>
        /// Arcstone Lightning pulses periodically between all wearers of this item, dealing 1000–1500 % weapon damage.
        /// </summary>
        public static Item Arcstone = new Item
        {
            Id = 433313,
            Name = "弧光石",
            Quality = ItemQuality.Legendary,
            Slug = "arcstone",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/arcstone",
            Url = "https://us.battle.net/d3/en/item/arcstone",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_ring_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/arcstone",
            IsCrafted = false,
            LegendaryAffix = "在佩戴这件物品的所有角色之间周期性地放射出闪电，造成 (1000-1500)% 的武器伤害。",
            SetName = "",
        };

        /// <summary>
        /// Band of the Rue Chambers Your Spirit Generators generate 40–50 % more Spirit.
        /// </summary>
        public static Item BandOfTheRueChambers = new Item
        {
            Id = 298093,
            Name = "悔恨大厅之戒",
            Quality = ItemQuality.Legendary,
            Slug = "band-of-the-rue-chambers",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "ring_norm_unique_030",
            DataUrl = "https://us.battle.net/api/d3/data/item/band-of-the-rue-chambers",
            Url = "https://us.battle.net/d3/en/item/band-of-the-rue-chambers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_106_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/band-of-the-rue-chambers",
            IsCrafted = false,
            LegendaryAffix = "你的内力生成技能生成的内力提高 (40-50)%。",
            SetName = "",
        };

        /// <summary>
        /// Halo of Karini You take 45–60 % less damage for 3 seconds after your Storm Armor electrocutes an enemy more than 30 yards away.
        /// </summary>
        public static Item HaloOfKarini = new Item
        {
            Id = 449039,
            Name = "卡丽娜之环",
            Quality = ItemQuality.Legendary,
            Slug = "halo-of-karini",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/halo-of-karini",
            Url = "https://us.battle.net/d3/en/item/halo-of-karini",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_ring_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/halo-of-karini",
            IsCrafted = false,
            LegendaryAffix = "当你的风暴护甲电击到 15 码范围之外的一名敌人时，你在 4 秒内受到的伤害降低 (60-80)%。",
            SetName = "",
        };

        /// <summary>
        /// Rechel's Ring of Larceny Gain 45–60 % increased movement speed for 4 seconds after Fearing an enemy.
        /// </summary>
        public static Item RechelsRingOfLarceny = new Item
        {
            Id = 298091,
            Name = "瑞秋的行窃之戒",
            Quality = ItemQuality.Legendary,
            Slug = "rechels-ring-of-larceny",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "ring_norm_unique_028",
            DataUrl = "https://us.battle.net/api/d3/data/item/rechels-ring-of-larceny",
            Url = "https://us.battle.net/d3/en/item/rechels-ring-of-larceny",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rechels-ring-of-larceny",
            IsCrafted = false,
            LegendaryAffix = "在恐惧一名敌人后，移动速度提高 (45-60)%，持续4秒。",
            SetName = "",
        };

        /// <summary>
        /// Rogar's Huge Stone Increase your Life per Second by up to 75–100 % based on your missing Life.
        /// </summary>
        public static Item RogarsHugeStone = new Item
        {
            Id = 298090,
            Name = "罗嘉的巨石",
            Quality = ItemQuality.Legendary,
            Slug = "rogars-huge-stone",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "ring_norm_unique_027",
            DataUrl = "https://us.battle.net/api/d3/data/item/rogars-huge-stone",
            Url = "https://us.battle.net/d3/en/item/rogars-huge-stone",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rogars-huge-stone",
            IsCrafted = false,
            LegendaryAffix = "基于你损失的生命值来提高你的每秒回复生命，最多可提高 (75-100)%。",
            SetName = "",
        };

        /// <summary>
        /// The Short Man's Finger Gargantuan instead summons three smaller gargantuans each more powerful than before.
        /// </summary>
        public static Item TheShortMansFinger = new Item
        {
            Id = 432666,
            Name = "小魔人之指",
            Quality = ItemQuality.Legendary,
            Slug = "the-short-mans-finger",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-short-mans-finger",
            Url = "https://us.battle.net/d3/en/item/the-short-mans-finger",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_ring_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-short-mans-finger",
            IsCrafted = false,
            LegendaryAffix = "僵尸巨兽召唤出3只较小的僵尸巨兽，造成的伤害提高 500-650%。",
            SetName = "",
        };

        /// <summary>
        /// The Tall Man's Finger Zombie Dogs instead summons a single gargantuan dog with more damage and health than all other dogs combined.
        /// </summary>
        public static Item TheTallMansFinger = new Item
        {
            Id = 298088,
            Name = "巨魔人之指",
            Quality = ItemQuality.Legendary,
            Slug = "the-tall-mans-finger",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-tall-mans-finger",
            Url = "https://us.battle.net/d3/en/item/the-tall-mans-finger",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-tall-mans-finger",
            IsCrafted = false,
            LegendaryAffix = "召唤僵尸犬现在会召唤出一只巨犬，造成的伤害和生命值比所有其它僵尸犬加起来还要高。",
            SetName = "",
        };

        /// <summary>
        /// Wyrdward Lightning damage has a 25–35 % chance to Stun for 1.5 seconds.
        /// </summary>
        public static Item Wyrdward = new Item
        {
            Id = 298089,
            Name = "命运守护",
            Quality = ItemQuality.Legendary,
            Slug = "wyrdward",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "ring_norm_unique_026",
            DataUrl = "https://us.battle.net/api/d3/data/item/wyrdward",
            Url = "https://us.battle.net/d3/en/item/wyrdward",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_102_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/wyrdward",
            IsCrafted = false,
            LegendaryAffix = "闪电伤害有 (25-35)% 的几率使敌人昏迷1.5秒。",
            SetName = "",
        };

        /// <summary>
        /// Nagelring Summons a Fallen Lunatic to your side every 10–12 seconds.
        /// </summary>
        public static Item Nagelring = new Item
        {
            Id = 212586,
            Name = "纳格尔之戒",
            Quality = ItemQuality.Legendary,
            Slug = "nagelring",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_018",
            DataUrl = "https://us.battle.net/api/d3/data/item/nagelring",
            Url = "https://us.battle.net/d3/en/item/nagelring",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_018_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/nagelring",
            IsCrafted = false,
            LegendaryAffix = "每 (10-12) 秒召唤一只堕落癫狂者为你而战。",
            SetName = "",
        };

        /// <summary>
        /// Lornelle's Sunstone Your damage reduction is increased by 0.75–0.95 % for every 1% Life you are missing.
        /// </summary>
        public static Item LornellesSunstone = new Item
        {
            Id = 476595,
            Name = "洛奈的太阳石",
            Quality = ItemQuality.Legendary,
            Slug = "lornelles-sunstone",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/lornelles-sunstone",
            Url = "https://us.battle.net/d3/en/item/lornelles-sunstone",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_ring_04_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/lornelles-sunstone",
            IsCrafted = false,
            LegendaryAffix = "你每损失1%的生命值，你受到的伤害就会额外降低（0.75-0.95）%。",
            SetName = "",
        };

        /// <summary>
        /// Bul-Kathos's Wedding Band You drain life from enemies around you.
        /// </summary>
        public static Item BulkathossWeddingBand = new Item
        {
            Id = 212603,
            Name = "布尔凯索的婚戒",
            Quality = ItemQuality.Legendary,
            Slug = "bulkathoss-wedding-band",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bulkathoss-wedding-band",
            Url = "https://us.battle.net/d3/en/item/bulkathoss-wedding-band",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_020_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bulkathoss-wedding-band",
            IsCrafted = false,
            LegendaryAffix = "吸取周围敌人的生命力。",
            SetName = "",
        };

        /// <summary>
        /// Eternal Union Increases the duration of Phalanx avatars by 200% .
        /// </summary>
        public static Item EternalUnion = new Item
        {
            Id = 212601,
            Name = "永恒盟约",
            Quality = ItemQuality.Legendary,
            Slug = "eternal-union",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_007",
            DataUrl = "https://us.battle.net/api/d3/data/item/eternal-union",
            Url = "https://us.battle.net/d3/en/item/eternal-union",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_007_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/eternal-union",
            IsCrafted = false,
            LegendaryAffix = "使斗阵化身的持续时间延长200%。",
            SetName = "",
        };

        /// <summary>
        /// Justice Lantern Gain damage reduction equal to 45–55 % of your Block Chance.
        /// </summary>
        public static Item JusticeLantern = new Item
        {
            Id = 212590,
            Name = "正义灯戒",
            Quality = ItemQuality.Legendary,
            Slug = "justice-lantern",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "ring_norm_unique_008",
            DataUrl = "https://us.battle.net/api/d3/data/item/justice-lantern",
            Url = "https://us.battle.net/d3/en/item/justice-lantern",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_ring_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/justice-lantern",
            IsCrafted = false,
            LegendaryAffix = "获得相当于你格挡几率 (45-55)% 的伤害减免。",
            SetName = "",
        };

        /// <summary>
        /// Obsidian Ring of the Zodiac Reduce the remaining cooldown of one of your skills by 1 seconds when you hit with a resource-spending attack.
        /// </summary>
        public static Item ObsidianRingOfTheZodiac = new Item
        {
            Id = 212588,
            Name = "黄道黑曜石之戒",
            Quality = ItemQuality.Legendary,
            Slug = "obsidian-ring-of-the-zodiac",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "ring_norm_unique_023",
            DataUrl = "https://us.battle.net/api/d3/data/item/obsidian-ring-of-the-zodiac",
            Url = "https://us.battle.net/d3/en/item/obsidian-ring-of-the-zodiac",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_023_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/obsidian-ring-of-the-zodiac",
            IsCrafted = false,
            LegendaryAffix = "当你的能量消耗技击中敌人时，其它一项技能的剩余冷却时间缩短 1 秒。",
            SetName = "",
        };

        /// <summary>
        /// Krysbin's Sentence You deal 75–100 % increased damage against slowed enemies or triple this bonus against enemies afflicted by any other type of control-impairing effect.
        /// </summary>
        public static Item KrysbinsSentence = new Item
        {
            Id = 476594,
            Name = "克里斯宾的审判",
            Quality = ItemQuality.Legendary,
            Slug = "krysbins-sentence",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/krysbins-sentence",
            Url = "https://us.battle.net/d3/en/item/krysbins-sentence",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_ring_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/krysbins-sentence",
            IsCrafted = false,
            LegendaryAffix = "你对被减速的敌人造成额外（75–100）%的伤害。对受到其他任何类型群体控制效果的目标造成伤害时，该加成数值提高三倍。",
            SetName = "",
        };

        /// <summary>
        /// Convention of Elements Gain 150–200 % increased damage to a single element for 4 seconds. This effect rotates through the elements available to your class in the following order: Arcane, Cold, Fire, Holy, Lightning, Physical, Poison.
        /// </summary>
        public static Item ConventionOfElements = new Item
        {
            Id = 433496,
            Name = "全能法戒",
            Quality = ItemQuality.Legendary,
            Slug = "convention-of-elements",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/convention-of-elements",
            Url = "https://us.battle.net/d3/en/item/convention-of-elements",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_ring_04_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/convention-of-elements",
            IsCrafted = false,
            LegendaryAffix = "单一元素的伤害提高(150-200)%，持续4秒。该效果会在你职业可用元素间循环，顺序如下：奥术、冰霜、火焰、神圣、闪电、物理、毒性。",
            SetName = "",
        };

        /// <summary>
        /// Elusive Ring After casting Shadow Power, Smoke Screen, or Vault, take 50–60 % reduced damage for 8 seconds.
        /// </summary>
        public static Item ElusiveRing = new Item
        {
            Id = 446188,
            Name = "残影之戒",
            Quality = ItemQuality.Legendary,
            Slug = "elusive-ring",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/elusive-ring",
            Url = "https://us.battle.net/d3/en/item/elusive-ring",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_ring_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/elusive-ring",
            IsCrafted = false,
            LegendaryAffix = "施放暗影之力、烟雾弹、影轮翻后，受到的伤害降低 (50-60)%，持续 8 秒。",
            SetName = "",
        };

        /// <summary>
        /// Halo of Arlyse Your Ice Armor now reduces damage from melee attacks by 50–60 % and automatically casts Frost Nova whenever you take 10% of your Life in damage.
        /// </summary>
        public static Item HaloOfArlyse = new Item
        {
            Id = 212602,
            Name = "阿莱斯之环",
            Quality = ItemQuality.Legendary,
            Slug = "halo-of-arlyse",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/halo-of-arlyse",
            Url = "https://us.battle.net/d3/en/item/halo-of-arlyse",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_ring_wizard_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/halo-of-arlyse",
            IsCrafted = false,
            LegendaryAffix = "你的寒冰护甲使你从近战攻击中受到的伤害降低(50-60)%，并且每当你承受相当于10%生命值的伤害时，自动施放冰霜新星。",
            SetName = "",
        };

        /// <summary>
        /// Ring of Emptiness You deal 250–300 % increased damage to enemies affected by both your Haunt and Locust Swarm.
        /// </summary>
        public static Item RingOfEmptiness = new Item
        {
            Id = 445697,
            Name = "空虚之戒",
            Quality = ItemQuality.Legendary,
            Slug = "ring-of-emptiness",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/ring-of-emptiness",
            Url = "https://us.battle.net/d3/en/item/ring-of-emptiness",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p42_unique_ring_haunt_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ring-of-emptiness",
            IsCrafted = false,
            LegendaryAffix = "你对同时受到你蚀魂和瘟疫虫群影响的敌人造成的伤害提高 (250-300)%。",
            SetName = "",
        };

        /// <summary>
        /// Band of Hollow Whispers This ring occasionally haunts nearby enemies.
        /// </summary>
        public static Item BandOfHollowWhispers = new Item
        {
            Id = 197834,
            Name = "空灵密语之戒",
            Quality = ItemQuality.Legendary,
            Slug = "band-of-hollow-whispers",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "ring_norm_unique_001",
            DataUrl = "https://us.battle.net/api/d3/data/item/band-of-hollow-whispers",
            Url = "https://us.battle.net/d3/en/item/band-of-hollow-whispers",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/band-of-hollow-whispers",
            IsCrafted = false,
            LegendaryAffix = "这枚戒指会不时地惊吓附近的敌人。",
            SetName = "",
        };

        /// <summary>
        /// Krede's Flame Taking Fire damage restores your primary resource.
        /// </summary>
        public static Item KredesFlame = new Item
        {
            Id = 197836,
            Name = "克雷德之焰",
            Quality = ItemQuality.Legendary,
            Slug = "kredes-flame",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_003",
            DataUrl = "https://us.battle.net/api/d3/data/item/kredes-flame",
            Url = "https://us.battle.net/d3/en/item/kredes-flame",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/kredes-flame",
            IsCrafted = false,
            LegendaryAffix = "受到火焰伤害可恢复你的主要能量。",
            SetName = "",
        };

        /// <summary>
        /// Oculus Ring Chance to create an area of focused power on killing a monster. Damage is increased by 70–85 % while standing in the area.
        /// </summary>
        public static Item OculusRing = new Item
        {
            Id = 212648,
            Name = "神目指环",
            Quality = ItemQuality.Legendary,
            Slug = "oculus-ring",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_017",
            DataUrl = "https://us.battle.net/api/d3/data/item/oculus-ring",
            Url = "https://us.battle.net/d3/en/item/oculus-ring",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_017_p4_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/oculus-ring",
            IsCrafted = false,
            LegendaryAffix = "消灭怪物有一定几率生成一片能量集中区域。站在该区域内，伤害提高(70-85)%。",
            SetName = "",
        };

        /// <summary>
        /// Skull Grasp Increase the damage of Whirlwind by 250–300 %
        /// </summary>
        public static Item SkullGrasp = new Item
        {
            Id = 212618,
            Name = "骷髅扣戒",
            Quality = ItemQuality.Legendary,
            Slug = "skull-grasp",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_022",
            DataUrl = "https://us.battle.net/api/d3/data/item/skull-grasp",
            Url = "https://us.battle.net/d3/en/item/skull-grasp",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_ring_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/skull-grasp",
            IsCrafted = false,
            LegendaryAffix = "使旋风斩的伤害提高 (300-400)% 的武器伤害。",
            SetName = "",
        };

        /// <summary>
        /// Stone of Jordan 
        /// </summary>
        public static Item StoneOfJordan = new Item
        {
            Id = 212582,
            Name = "乔丹之石",
            Quality = ItemQuality.Legendary,
            Slug = "stone-of-jordan",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/stone-of-jordan",
            Url = "https://us.battle.net/d3/en/item/stone-of-jordan",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_019_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/stone-of-jordan",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Unity All damage taken is split between wearers of this item.
        /// </summary>
        public static Item Unity = new Item
        {
            Id = 212581,
            Name = "团结",
            Quality = ItemQuality.Legendary,
            Slug = "unity",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/unity",
            Url = "https://us.battle.net/d3/en/item/unity",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_010_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/unity",
            IsCrafted = false,
            LegendaryAffix = "受到的所有伤害由该物品的佩戴者分担。",
            SetName = "",
        };

        /// <summary>
        /// Litany of the Undaunted 
        /// </summary>
        public static Item LitanyOfTheUndaunted = new Item
        {
            Id = 212651,
            Name = "英豪之愿",
            Quality = ItemQuality.Legendary,
            Slug = "litany-of-the-undaunted",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_015",
            DataUrl = "https://us.battle.net/api/d3/data/item/litany-of-the-undaunted",
            Url = "https://us.battle.net/d3/en/item/litany-of-the-undaunted",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_015_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/litany-of-the-undaunted",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "梦魇者的遗礼",
        };

        /// <summary>
        /// Natalya's Reflection 
        /// </summary>
        public static Item NatalyasReflection = new Item
        {
            Id = 212545,
            Name = "娜塔亚的回忆",
            Quality = ItemQuality.Legendary,
            Slug = "natalyas-reflection",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_011",
            DataUrl = "https://us.battle.net/api/d3/data/item/natalyas-reflection",
            Url = "https://us.battle.net/d3/en/item/natalyas-reflection",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/natalyas-reflection",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "娜塔亚的复仇",
        };

        /// <summary>
        /// The Compass Rose 
        /// </summary>
        public static Item TheCompassRose = new Item
        {
            Id = 212587,
            Name = "罗盘玫瑰",
            Quality = ItemQuality.Legendary,
            Slug = "the-compass-rose",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_013",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-compass-rose",
            Url = "https://us.battle.net/d3/en/item/the-compass-rose",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_013_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-compass-rose",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "无尽之途",
        };

        /// <summary>
        /// The Wailing Host 
        /// </summary>
        public static Item TheWailingHost = new Item
        {
            Id = 212650,
            Name = "千魂狱",
            Quality = ItemQuality.Legendary,
            Slug = "the-wailing-host",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "ring_norm_unique_014",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-wailing-host",
            Url = "https://us.battle.net/d3/en/item/the-wailing-host",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_014_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-wailing-host",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "梦魇者的遗礼",
        };

        /// <summary>
        /// Zunimassa's Pox 
        /// </summary>
        public static Item ZunimassasPox = new Item
        {
            Id = 212579,
            Name = "祖尼玛萨之疾",
            Quality = ItemQuality.Legendary,
            Slug = "zunimassas-pox",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "Ring_norm_unique_012",
            DataUrl = "https://us.battle.net/api/d3/data/item/zunimassas-pox",
            Url = "https://us.battle.net/d3/en/item/zunimassas-pox",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_012_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/zunimassas-pox",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "祖尼玛萨之魂",
        };

        /// <summary>
        /// Briggs' Wrath Uncursed enemies are pulled to the target location when a curse is applied to them.
        /// </summary>
        public static Item BriggsWrath = new Item
        {
            Id = 476593,
            Name = "布里格斯之怒",
            Quality = ItemQuality.Legendary,
            Slug = "briggs-wrath",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/briggs-wrath",
            Url = "https://us.battle.net/d3/en/item/briggs-wrath",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_ring_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/briggs-wrath",
            IsCrafted = false,
            LegendaryAffix = "未被诅咒的敌人会在受到诅咒时被拉向目标位置。",
            SetName = "",
        };

        /// <summary>
        /// Focus 
        /// </summary>
        public static Item Focus = new Item
        {
            Id = 332209,
            Name = "守心",
            Quality = ItemQuality.Legendary,
            Slug = "focus",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "ring_norm_set_001",
            DataUrl = "https://us.battle.net/api/d3/data/item/focus",
            Url = "https://us.battle.net/d3/en/item/focus",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_set_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/focus",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "意志壁垒",
        };

        /// <summary>
        /// Restraint 
        /// </summary>
        public static Item Restraint = new Item
        {
            Id = 332210,
            Name = "克己",
            Quality = ItemQuality.Legendary,
            Slug = "restraint",
            ItemType = ItemType.Ring,
            TrinityItemType = TrinityItemType.Ring,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Jewelry,
            InternalName = "ring_norm_set_002",
            DataUrl = "https://us.battle.net/api/d3/data/item/restraint",
            Url = "https://us.battle.net/d3/en/item/restraint",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ring_set_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/restraint",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "意志壁垒",
        };

        /// <summary>
        /// Coven's Criterion You take 45–60 % less damage from blocked attacks.
        /// </summary>
        public static Item CovensCriterion = new Item
        {
            Id = 298191,
            Name = "巫师会的准则",
            Quality = ItemQuality.Legendary,
            Slug = "covens-criterion",
            ItemType = ItemType.Shield,
            TrinityItemType = TrinityItemType.Shield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/covens-criterion",
            Url = "https://us.battle.net/d3/en/item/covens-criterion",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shield_107_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/covens-criterion",
            IsCrafted = false,
            LegendaryAffix = "你从被格挡的攻击中受到的伤害降低 (45-60)%。",
            SetName = "",
        };

        /// <summary>
        /// Denial Each enemy hit by your Sweep Attack increases the damage of your next Sweep Attack by 30–40 % , stacking up to 5 times.
        /// </summary>
        public static Item Denial = new Item
        {
            Id = 152666,
            Name = "不破之钢",
            Quality = ItemQuality.Legendary,
            Slug = "denial",
            ItemType = ItemType.Shield,
            TrinityItemType = TrinityItemType.Shield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Shield_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/denial",
            Url = "https://us.battle.net/d3/en/item/denial",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_shield_007_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/denial",
            IsCrafted = false,
            LegendaryAffix = "你的横扫每击中一名敌人便会使你的下一个横扫的伤害提高(100-125)%，最多叠加5次。",
            SetName = "",
        };

        /// <summary>
        /// Defender of Westmarch Blocks have a chance of summoning a charging wolf that deals 800–1000 % weapon damage to all enemies it passes through.
        /// </summary>
        public static Item DefenderOfWestmarch = new Item
        {
            Id = 298182,
            Name = "威斯特玛防御者",
            Quality = ItemQuality.Legendary,
            Slug = "defender-of-westmarch",
            ItemType = ItemType.Shield,
            TrinityItemType = TrinityItemType.Shield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "shield_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/defender-of-westmarch",
            Url = "https://us.battle.net/d3/en/item/defender-of-westmarch",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shield_101_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/defender-of-westmarch",
            IsCrafted = false,
            LegendaryAffix = "格挡有一定几率召唤出一只冲锋的战狼，对它沿途遇到的所有敌人造成 (800-1000)% 的武器伤害。",
            SetName = "",
        };

        /// <summary>
        /// Eberli Charo Reduces the cooldown of Heaven's Fury by 45–50 % .
        /// </summary>
        public static Item EberliCharo = new Item
        {
            Id = 298186,
            Name = "艾伯力·卡罗",
            Quality = ItemQuality.Legendary,
            Slug = "eberli-charo",
            ItemType = ItemType.Shield,
            TrinityItemType = TrinityItemType.Shield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "shield_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/eberli-charo",
            Url = "https://us.battle.net/d3/en/item/eberli-charo",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shield_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/eberli-charo",
            IsCrafted = false,
            LegendaryAffix = "使天堂之怒的冷却时间缩短 (45-50)%。",
            SetName = "",
        };

        /// <summary>
        /// Freeze of Deflection Blocking an attack has a chance to Freeze the attacker for 0.5–1.5 seconds.
        /// </summary>
        public static Item FreezeOfDeflection = new Item
        {
            Id = 61550,
            Name = "折射成冰",
            Quality = ItemQuality.Legendary,
            Slug = "freeze-of-deflection",
            ItemType = ItemType.Shield,
            TrinityItemType = TrinityItemType.Shield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Shield_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/freeze-of-deflection",
            Url = "https://us.battle.net/d3/en/item/freeze-of-deflection",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shield_004_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/freeze-of-deflection",
            IsCrafted = false,
            LegendaryAffix = "成功格挡一次攻击有一定几率冰冻攻击者 (0.5-1.5) 秒。",
            SetName = "",
        };

        /// <summary>
        /// Vo'Toyias Spiker Enemies affected by Provoke take double damage from Thorns.
        /// </summary>
        public static Item VotoyiasSpiker = new Item
        {
            Id = 298188,
            Name = "沃托亚的刺盾",
            Quality = ItemQuality.Legendary,
            Slug = "votoyias-spiker",
            ItemType = ItemType.Shield,
            TrinityItemType = TrinityItemType.Shield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "shield_norm_unique_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/votoyias-spiker",
            Url = "https://us.battle.net/d3/en/item/votoyias-spiker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shield_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/votoyias-spiker",
            IsCrafted = false,
            LegendaryAffix = "受到挑衅影响的敌人，从荆棘中受到的伤害翻倍。",
            SetName = "",
        };

        /// <summary>
        /// Lidless Wall 
        /// </summary>
        public static Item LidlessWall = new Item
        {
            Id = 195389,
            Name = "警戒之墙",
            Quality = ItemQuality.Legendary,
            Slug = "lidless-wall",
            ItemType = ItemType.Shield,
            TrinityItemType = TrinityItemType.Shield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Shield_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/lidless-wall",
            Url = "https://us.battle.net/d3/en/item/lidless-wall",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shield_008_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/lidless-wall",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Ivory Tower Blocks release forward a Fires of Heaven.
        /// </summary>
        public static Item IvoryTower = new Item
        {
            Id = 197478,
            Name = "象牙塔",
            Quality = ItemQuality.Legendary,
            Slug = "ivory-tower",
            ItemType = ItemType.Shield,
            TrinityItemType = TrinityItemType.Shield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Shield_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/ivory-tower",
            Url = "https://us.battle.net/d3/en/item/ivory-tower",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_shield_002_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ivory-tower",
            IsCrafted = false,
            LegendaryAffix = "格挡会释放出天堂之火。",
            SetName = "",
        };

        /// <summary>
        /// Stormshield 
        /// </summary>
        public static Item Stormshield = new Item
        {
            Id = 192484,
            Name = "暴风之盾",
            Quality = ItemQuality.Legendary,
            Slug = "stormshield",
            ItemType = ItemType.Shield,
            TrinityItemType = TrinityItemType.Shield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Shield_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/stormshield",
            Url = "https://us.battle.net/d3/en/item/stormshield",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shield_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/stormshield",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Jesseth Skullshield 
        /// </summary>
        public static Item JessethSkullshield = new Item
        {
            Id = 454765,
            Name = "杰西斯的骷髅盾",
            Quality = ItemQuality.Legendary,
            Slug = "jesseth-skullshield",
            ItemType = ItemType.Shield,
            TrinityItemType = TrinityItemType.Shield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/jesseth-skullshield",
            Url = "https://us.battle.net/d3/en/item/jesseth-skullshield",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_shield_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jesseth-skullshield",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "杰西斯的武装",
        };

        /// <summary>
        /// Guard of Johanna Blessed Hammer damage is increased by 200–250 % for the first 3 enemies it hits.
        /// </summary>
        public static Item GuardOfJohanna = new Item
        {
            Id = 298187,
            Name = "乔汉娜的守御",
            Quality = ItemQuality.Legendary,
            Slug = "guard-of-johanna",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/guard-of-johanna",
            Url = "https://us.battle.net/d3/en/item/guard-of-johanna",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shield_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/guard-of-johanna",
            IsCrafted = false,
            LegendaryAffix = "祝福之锤对其击中的前 3 名敌人造成的伤害提高 (200-250)% 。",
            SetName = "",
        };

        /// <summary>
        /// Salvation Blocked attacks heal you and your allies for 20–30 % of the amount blocked.
        /// </summary>
        public static Item Salvation = new Item
        {
            Id = 299418,
            Name = "拯救",
            Quality = ItemQuality.Legendary,
            Slug = "salvation",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/salvation",
            Url = "https://us.battle.net/d3/en/item/salvation",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_crushield_108_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/salvation",
            IsCrafted = false,
            LegendaryAffix = "格挡攻击为你和同伴恢复相当于格挡伤害量 (20-30)% 的生命值。",
            SetName = "",
        };

        /// <summary>
        /// Shield of Fury Each time an enemy takes damage from your Heaven's Fury, it increases the damage they take from your Heaven's Fury by 15–20 % .
        /// </summary>
        public static Item ShieldOfFury = new Item
        {
            Id = 298190,
            Name = "愤怒壁垒",
            Quality = ItemQuality.Legendary,
            Slug = "shield-of-fury",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/shield-of-fury",
            Url = "https://us.battle.net/d3/en/item/shield-of-fury",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_shield_106_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/shield-of-fury",
            IsCrafted = false,
            LegendaryAffix = "每当有敌人受到你的天堂之怒的伤害时，都会使你的天堂之怒对其造成的伤害提高 (25-30)%。",
            SetName = "",
        };

        /// <summary>
        /// Akarat's Awakening Every successful block has a 20–25 % chance to reduce all cooldowns by 1 second.
        /// </summary>
        public static Item AkaratsAwakening = new Item
        {
            Id = 299414,
            Name = "阿卡拉特的顿悟",
            Quality = ItemQuality.Legendary,
            Slug = "akarats-awakening",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "crushield_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/akarats-awakening",
            Url = "https://us.battle.net/d3/en/item/akarats-awakening",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_crushield_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/akarats-awakening",
            IsCrafted = false,
            LegendaryAffix = "每次成功格挡都有 (20-25)% 的几率使所有冷却时间缩短1秒。",
            SetName = "",
        };

        /// <summary>
        /// Hallowed Bulwark Iron Skin also increases your Block Amount by 45–60 % .
        /// </summary>
        public static Item HallowedBulwark = new Item
        {
            Id = 299413,
            Name = "神圣壁垒",
            Quality = ItemQuality.Legendary,
            Slug = "hallowed-bulwark",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "crushield_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/hallowed-bulwark",
            Url = "https://us.battle.net/d3/en/item/hallowed-bulwark",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_crushield_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/hallowed-bulwark",
            IsCrafted = false,
            LegendaryAffix = "钢铁之肤同时使你的格挡量提高 (45-60)%。",
            SetName = "",
        };

        /// <summary>
        /// Hellskull Gain 10% increased damage while wielding a two-handed weapon.
        /// </summary>
        public static Item Hellskull = new Item
        {
            Id = 299415,
            Name = "地狱颅骨",
            Quality = ItemQuality.Legendary,
            Slug = "hellskull",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "crushield_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/hellskull",
            Url = "https://us.battle.net/d3/en/item/hellskull",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_crushield_105_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/hellskull",
            IsCrafted = false,
            LegendaryAffix = "使用双手武器时，造成的伤害提高10% 。",
            SetName = "",
        };

        /// <summary>
        /// Jekangbord Blessed Shield ricochets to 4–6 additional enemies.
        /// </summary>
        public static Item Jekangbord = new Item
        {
            Id = 299412,
            Name = "杰伏坎盾",
            Quality = ItemQuality.Legendary,
            Slug = "jekangbord",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "crushield_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/jekangbord",
            Url = "https://us.battle.net/d3/en/item/jekangbord",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_crushield_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jekangbord",
            IsCrafted = false,
            LegendaryAffix = "祝福之盾会弹射到额外 4-6名敌人身上，并且伤害提高 225-300%。",
            SetName = "",
        };

        /// <summary>
        /// Sublime Conviction When you block, you have up to a 15–20 % chance to Stun the attacker for 1.5 seconds based on your current Wrath.
        /// </summary>
        public static Item SublimeConviction = new Item
        {
            Id = 299416,
            Name = "崇高信念",
            Quality = ItemQuality.Legendary,
            Slug = "sublime-conviction",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "crushield_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/sublime-conviction",
            Url = "https://us.battle.net/d3/en/item/sublime-conviction",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_crushield_106_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sublime-conviction",
            IsCrafted = false,
            LegendaryAffix = "基于你当前的愤怒值，格挡时你有 (15-20)% 的几率使攻击者昏迷1.5秒。",
            SetName = "",
        };

        /// <summary>
        /// The Final Witness Shield Glare now hits all enemies around you.
        /// </summary>
        public static Item TheFinalWitness = new Item
        {
            Id = 299417,
            Name = "最后的见证者",
            Quality = ItemQuality.Legendary,
            Slug = "the-final-witness",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "crushield_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-final-witness",
            Url = "https://us.battle.net/d3/en/item/the-final-witness",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_crushield_107_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-final-witness",
            IsCrafted = false,
            LegendaryAffix = "盾闪现在会击中你身边的所有敌人。",
            SetName = "",
        };

        /// <summary>
        /// Frydehr's Wrath Condemn has no cooldown but instead costs 40 Wrath.
        /// </summary>
        public static Item FrydehrsWrath = new Item
        {
            Id = 405429,
            Name = "弗莱德的怒火",
            Quality = ItemQuality.Legendary,
            Slug = "frydehrs-wrath",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/frydehrs-wrath",
            Url = "https://us.battle.net/d3/en/item/frydehrs-wrath",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p1_crushield_norm_unique_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/frydehrs-wrath",
            IsCrafted = false,
            LegendaryAffix = "天谴没有冷却时间，并且伤害提高 （600-800）%，但会消耗40点愤怒值。",
            SetName = "",
        };

        /// <summary>
        /// Unrelenting Phalanx Phalanx now casts twice.
        /// </summary>
        public static Item UnrelentingPhalanx = new Item
        {
            Id = 405514,
            Name = "无情斗阵",
            Quality = ItemQuality.Legendary,
            Slug = "unrelenting-phalanx",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/unrelenting-phalanx",
            Url = "https://us.battle.net/d3/en/item/unrelenting-phalanx",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p1_crushield_norm_unique_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/unrelenting-phalanx",
            IsCrafted = false,
            LegendaryAffix = "斗阵现在施放两次。",
            SetName = "",
        };

        /// <summary>
        /// Shield of the Steed 
        /// </summary>
        public static Item ShieldOfTheSteed = new Item
        {
            Id = 298189,
            Name = "战马之盾",
            Quality = ItemQuality.Legendary,
            Slug = "shield-of-the-steed",
            ItemType = ItemType.CrusaderShield,
            TrinityItemType = TrinityItemType.CrusaderShield,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/shield-of-the-steed",
            Url = "https://us.battle.net/d3/en/item/shield-of-the-steed",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_shield_set_01_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/shield-of-the-steed",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "诺瓦德的热忱",
        };

        /// <summary>
        /// Gazing Demise Spirit Barrage gains the Phantasm rune. Each active Phantasm increases the damage of Spirit Barrage by 40–50 % .
        /// </summary>
        public static Item GazingDemise = new Item
        {
            Id = 194995,
            Name = "观死",
            Quality = ItemQuality.Legendary,
            Slug = "gazing-demise",
            ItemType = ItemType.Mojo,
            TrinityItemType = TrinityItemType.Mojo,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Mojo_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/gazing-demise",
            Url = "https://us.battle.net/d3/en/item/gazing-demise",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p42_unique_mojo_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/gazing-demise",
            IsCrafted = false,
            LegendaryAffix = "灵魂弹幕获得幽魂鬼影符文效果。每个激活的鬼魂都会使灵魂弹幕的伤害提高 (40-50)%。",
            SetName = "",
        };

        /// <summary>
        /// Homunculus A Zombie Dog is automatically summoned to your side every 2 seconds.
        /// </summary>
        public static Item Homunculus = new Item
        {
            Id = 194991,
            Name = "小鬼符",
            Quality = ItemQuality.Legendary,
            Slug = "homunculus",
            ItemType = ItemType.Mojo,
            TrinityItemType = TrinityItemType.Mojo,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Mojo_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/homunculus",
            Url = "https://us.battle.net/d3/en/item/homunculus",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p42_unique_mojo_004_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/homunculus",
            IsCrafted = false,
            LegendaryAffix = "每隔 2 秒会自动召唤出一只僵尸犬，与你并肩战斗。",
            SetName = "",
        };

        /// <summary>
        /// Shukrani's Triumph Spirit Walk lasts until you attack or until an enemy is within 30 yards of you.
        /// </summary>
        public static Item ShukranisTriumph = new Item
        {
            Id = 272070,
            Name = "舒克拉尼的胜利",
            Quality = ItemQuality.Legendary,
            Slug = "shukranis-triumph",
            ItemType = ItemType.Mojo,
            TrinityItemType = TrinityItemType.Mojo,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "mojo_norm_unique_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/shukranis-triumph",
            Url = "https://us.battle.net/d3/en/item/shukranis-triumph",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mojo_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/shukranis-triumph",
            IsCrafted = false,
            LegendaryAffix = "灵魂行走持续到你发起攻击或在你附近30码内出现敌人为止。",
            SetName = "",
        };

        /// <summary>
        /// Thing of the Deep Increases Gold and Health Pickup by 20 yards.
        /// </summary>
        public static Item ThingOfTheDeep = new Item
        {
            Id = 192468,
            Name = "深渊魔物",
            Quality = ItemQuality.Legendary,
            Slug = "thing-of-the-deep",
            ItemType = ItemType.Mojo,
            TrinityItemType = TrinityItemType.Mojo,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/thing-of-the-deep",
            Url = "https://us.battle.net/d3/en/item/thing-of-the-deep",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_mojo_002_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/thing-of-the-deep",
            IsCrafted = false,
            LegendaryAffix = "金币和生命球的拾取范围扩大 20 码。",
            SetName = "",
        };

        /// <summary>
        /// Uhkapian Serpent 25–30 % of the damage you take is redirected to your Zombie Dogs.
        /// </summary>
        public static Item UhkapianSerpent = new Item
        {
            Id = 191278,
            Name = "尤卡班毒蛇",
            Quality = ItemQuality.Legendary,
            Slug = "uhkapian-serpent",
            ItemType = ItemType.Mojo,
            TrinityItemType = TrinityItemType.Mojo,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Mojo_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/uhkapian-serpent",
            Url = "https://us.battle.net/d3/en/item/uhkapian-serpent",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mojo_008_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/uhkapian-serpent",
            IsCrafted = false,
            LegendaryAffix = "你受到的 (25-30)% 的伤害会转移到你的僵尸犬身上。",
            SetName = "",
        };

        /// <summary>
        /// Manajuma's Gory Fetch 
        /// </summary>
        public static Item ManajumasGoryFetch = new Item
        {
            Id = 210993,
            Name = "马纳祖玛的血祭",
            Quality = ItemQuality.Legendary,
            Slug = "manajumas-gory-fetch",
            ItemType = ItemType.Mojo,
            TrinityItemType = TrinityItemType.Mojo,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "Mojo_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/manajumas-gory-fetch",
            Url = "https://us.battle.net/d3/en/item/manajumas-gory-fetch",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mojo_010_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/manajumas-gory-fetch",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "马纳祖玛之道",
        };

        /// <summary>
        /// Zunimassa's String of Skulls 
        /// </summary>
        public static Item ZunimassasStringOfSkulls = new Item
        {
            Id = 216525,
            Name = "祖尼玛萨的头骨串",
            Quality = ItemQuality.Legendary,
            Slug = "zunimassas-string-of-skulls",
            ItemType = ItemType.Mojo,
            TrinityItemType = TrinityItemType.Mojo,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/zunimassas-string-of-skulls",
            Url = "https://us.battle.net/d3/en/item/zunimassas-string-of-skulls",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mojo_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/zunimassas-string-of-skulls",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "祖尼玛萨之魂",
        };

        /// <summary>
        /// Henri's Perquisition The first time an enemy deals damage to you, reduce that damage by 60–80 % and Charm the enemy for 3 seconds.
        /// </summary>
        public static Item HenrisPerquisition = new Item
        {
            Id = 395199,
            Name = "亨利的永恒追捕",
            Quality = ItemQuality.Legendary,
            Slug = "henris-perquisition",
            ItemType = ItemType.Mojo,
            TrinityItemType = TrinityItemType.Mojo,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/henris-perquisition",
            Url = "https://us.battle.net/d3/en/item/henris-perquisition",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_mojo_norm_unique_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/henris-perquisition",
            IsCrafted = false,
            LegendaryAffix = "敌人第一次对你造成伤害时，该次伤害降低 (60-80)% ，并魅惑这名敌人 3 秒。",
            SetName = "",
        };

        /// <summary>
        /// Vile Hive Locust Swarm gains the effect of the Pestilence rune and deals 45–60 % increased damage.
        /// </summary>
        public static Item VileHive = new Item
        {
            Id = 220326,
            Name = "邪恶虫巢",
            Quality = ItemQuality.Legendary,
            Slug = "vile-hive",
            ItemType = ItemType.Mojo,
            TrinityItemType = TrinityItemType.Mojo,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/vile-hive",
            Url = "https://us.battle.net/d3/en/item/vile-hive",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_mojo_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vile-hive",
            IsCrafted = false,
            LegendaryAffix = "瘟疫虫群获得剧毒虫群符文效果，造成的伤害提高 (45-60)%。",
            SetName = "",
        };

        /// <summary>
        /// Wilken's Reach Grasp of the Dead no longer has a cooldown.
        /// </summary>
        public static Item WilkensReach = new Item
        {
            Id = 395198,
            Name = "维尔根之触",
            Quality = ItemQuality.Legendary,
            Slug = "wilkens-reach",
            ItemType = ItemType.Mojo,
            TrinityItemType = TrinityItemType.Mojo,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/wilkens-reach",
            Url = "https://us.battle.net/d3/en/item/wilkens-reach",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_mojo_003_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/wilkens-reach",
            IsCrafted = false,
            LegendaryAffix = "亡者之握不再有冷却时间。",
            SetName = "",
        };

        /// <summary>
        /// Winter Flurry Enemies killed by Cold damage have a 15–20 % chance to release a Frost Nova.
        /// </summary>
        public static Item WinterFlurry = new Item
        {
            Id = 184199,
            Name = "寒流",
            Quality = ItemQuality.Legendary,
            Slug = "winter-flurry",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "orb_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/winter-flurry",
            Url = "https://us.battle.net/d3/en/item/winter-flurry",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_orb_005_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/winter-flurry",
            IsCrafted = false,
            LegendaryAffix = "被冰霜伤害消灭的敌人有 (15-20)% 的几率释放一次冰霜新星。",
            SetName = "",
        };

        /// <summary>
        /// Etched Sigil Your Arcane Torrent, Disintegrate, and Ray of Frost also cast one of your other damaging Arcane Power Spenders every second.
        /// </summary>
        public static Item EtchedSigil = new Item
        {
            Id = 399318,
            Name = "蚀刻符印",
            Quality = ItemQuality.Legendary,
            Slug = "etched-sigil",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/etched-sigil",
            Url = "https://us.battle.net/d3/en/item/etched-sigil",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_orb_002_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/etched-sigil",
            IsCrafted = false,
            LegendaryAffix = "引导奥术洪流、瓦解射线以及冰霜射线时，奥能消耗技造成的伤害提升 120-150%，每隔一秒还会施放一种你的其它伤害性奥能消耗技。",
            SetName = "",
        };

        /// <summary>
        /// Light of Grace Ray of Frost now pierces.
        /// </summary>
        public static Item LightOfGrace = new Item
        {
            Id = 272038,
            Name = "恩赐之光",
            Quality = ItemQuality.Legendary,
            Slug = "light-of-grace",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "orb_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/light-of-grace",
            Url = "https://us.battle.net/d3/en/item/light-of-grace",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_orb_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/light-of-grace",
            IsCrafted = false,
            LegendaryAffix = "冰霜射线现在会穿透敌人。",
            SetName = "",
        };

        /// <summary>
        /// Mirrorball Magic Missile fires 1–2 extra missiles.
        /// </summary>
        public static Item Mirrorball = new Item
        {
            Id = 272022,
            Name = "镜光魔珠",
            Quality = ItemQuality.Legendary,
            Slug = "mirrorball",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "orb_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/mirrorball",
            Url = "https://us.battle.net/d3/en/item/mirrorball",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_orb_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/mirrorball",
            IsCrafted = false,
            LegendaryAffix = "魔法飞弹发射出额外 (1-2) 发飞弹。",
            SetName = "",
        };

        /// <summary>
        /// Myken's Ball of Hate Electrocute can chain to enemies that have already been hit.
        /// </summary>
        public static Item MykensBallOfHate = new Item
        {
            Id = 272037,
            Name = "麦肯的憎恨宝珠",
            Quality = ItemQuality.Legendary,
            Slug = "mykens-ball-of-hate",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "orb_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/mykens-ball-of-hate",
            Url = "https://us.battle.net/d3/en/item/mykens-ball-of-hate",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_orb_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/mykens-ball-of-hate",
            IsCrafted = false,
            LegendaryAffix = "电刑可跳到已经被击中的敌人身上。",
            SetName = "",
        };

        /// <summary>
        /// The Oculus Taking damage has up to a 15–20 % chance to reset the cooldown on Teleport.
        /// </summary>
        public static Item TheOculus = new Item
        {
            Id = 192320,
            Name = "法瞳",
            Quality = ItemQuality.Legendary,
            Slug = "the-oculus",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "orb_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-oculus",
            Url = "https://us.battle.net/d3/en/item/the-oculus",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_orb_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-oculus",
            IsCrafted = false,
            LegendaryAffix = "受到伤害时最高有 (15-20)% 的几率重置传送的冷却时间。",
            SetName = "",
        };

        /// <summary>
        /// Triumvirate Your Signature Spells increase the damage of Arcane Orb by 150–200 % for 6 seconds, stacking up to 3 times.
        /// </summary>
        public static Item Triumvirate = new Item
        {
            Id = 195325,
            Name = "三元宝珠",
            Quality = ItemQuality.Legendary,
            Slug = "triumvirate",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "orb_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/triumvirate",
            Url = "https://us.battle.net/d3/en/item/triumvirate",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_orb_003_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/triumvirate",
            IsCrafted = false,
            LegendaryAffix = "你的修为法术会使奥术之球的伤害提高 (300-400)%，持续 6 秒，最多叠加 3 次。",
            SetName = "",
        };

        /// <summary>
        /// Chantodo's Force 
        /// </summary>
        public static Item ChantodosForce = new Item
        {
            Id = 212277,
            Name = "迦陀朵的原力之球",
            Quality = ItemQuality.Legendary,
            Slug = "chantodos-force",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "orb_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/chantodos-force",
            Url = "https://us.battle.net/d3/en/item/chantodos-force",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_orb_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/chantodos-force",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "迦陀朵的决心",
        };

        /// <summary>
        /// Tal Rasha's Unwavering Glare 
        /// </summary>
        public static Item TalRashasUnwaveringGlare = new Item
        {
            Id = 212780,
            Name = "塔·拉夏的坚定目光",
            Quality = ItemQuality.Legendary,
            Slug = "tal-rashas-unwavering-glare",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "orb_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/tal-rashas-unwavering-glare",
            Url = "https://us.battle.net/d3/en/item/tal-rashas-unwavering-glare",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_orb_012_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tal-rashas-unwavering-glare",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔·拉夏的法理",
        };

        /// <summary>
        /// Primordial Soul Elemental Exposure's damage bonus per stack is increased to 10 % .
        /// </summary>
        public static Item PrimordialSoul = new Item
        {
            Id = 399317,
            Name = "源生之魂",
            Quality = ItemQuality.Legendary,
            Slug = "primordial-soul",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/primordial-soul",
            Url = "https://us.battle.net/d3/en/item/primordial-soul",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_orb_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/primordial-soul",
            IsCrafted = false,
            LegendaryAffix = "元素易伤的每层伤害加成效果提高至 10%。",
            SetName = "",
        };

        /// <summary>
        /// Orb of Infinite Depth Each time you hit an enemy with Explosive Blast your damage is increased by 4 % and your damage reduction is increased by 15% for 6 seconds. This effect can stack up to 4 times.
        /// </summary>
        public static Item OrbOfInfiniteDepth = new Item
        {
            Id = 399319,
            Name = "无尽深渊法珠",
            Quality = ItemQuality.Legendary,
            Slug = "orb-of-infinite-depth",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/orb-of-infinite-depth",
            Url = "https://us.battle.net/d3/en/item/orb-of-infinite-depth",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_orb_004_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/orb-of-infinite-depth",
            IsCrafted = false,
            LegendaryAffix = "你每次使用聚能爆破击中敌人，你的伤害提高 (8-10)%，伤害减免效果提高 20%，持续 6 秒。该效果最多可叠加 4 次。",
            SetName = "",
        };

        /// <summary>
        /// Firebird's Eye 
        /// </summary>
        public static Item FirebirdsEye = new Item
        {
            Id = 358819,
            Name = "不死鸟之瞳",
            Quality = ItemQuality.Legendary,
            Slug = "firebirds-eye",
            ItemType = ItemType.Orb,
            TrinityItemType = TrinityItemType.Orb,
            IsTwoHanded = false,
            BaseType = ItemBaseType.None,
            InternalName = "orb_norm_set_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/firebirds-eye",
            Url = "https://us.battle.net/d3/en/item/firebirds-eye",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_orb_set_06_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/firebirds-eye",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不死鸟的华服",
        };

        /// <summary>
        /// Envious Blade Gain 100% Critical Hit Chance against enemies at full health.
        /// </summary>
        public static Item EnviousBlade = new Item
        {
            Id = 271732,
            Name = "嫉妒之刃",
            Quality = ItemQuality.Legendary,
            Slug = "envious-blade",
            ItemType = ItemType.Dagger,
            TrinityItemType = TrinityItemType.Dagger,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/envious-blade",
            Url = "https://us.battle.net/d3/en/item/envious-blade",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_dagger_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/envious-blade",
            IsCrafted = false,
            LegendaryAffix = "对满血的敌人有100%的暴击几率。",
            SetName = "",
        };

        /// <summary>
        /// The Barber Instead of dealing direct damage, your Spirit Barrage now accumulates on the target. When you stop casting, it explodes dealing 225–250 % of the accumulated damage to all enemies within 15 yards.
        /// </summary>
        public static Item TheBarber = new Item
        {
            Id = 195400,
            Name = "剃头师",
            Quality = ItemQuality.Legendary,
            Slug = "the-barber-49S3Pa",
            ItemType = ItemType.Dagger,
            TrinityItemType = TrinityItemType.Dagger,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-barber-49S3Pa",
            Url = "https://us.battle.net/d3/en/item/the-barber-49S3Pa",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p43_unique_dagger_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-barber-49S3Pa",
            IsCrafted = false,
            LegendaryAffix = "你的灵魂弹幕不再造成直接伤害，而是在目标身上累积。当你停止施法时，它将爆炸并对 15 码范围内的所有敌人造成 (225-250)% 的累积伤害。",
            SetName = "",
        };

        /// <summary>
        /// Pig Sticker Squeal!
        /// </summary>
        public static Item PigSticker = new Item
        {
            Id = 221313,
            Name = "杀猪刀",
            Quality = ItemQuality.Legendary,
            Slug = "pig-sticker",
            ItemType = ItemType.Dagger,
            TrinityItemType = TrinityItemType.Dagger,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "dagger_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/pig-sticker",
            Url = "https://us.battle.net/d3/en/item/pig-sticker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_dagger_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pig-sticker",
            IsCrafted = false,
            LegendaryAffix = "唧！",
            SetName = "",
        };

        /// <summary>
        /// Wizardspike Performing an attack has a 20–25 % chance to hurl a Frozen Orb.
        /// </summary>
        public static Item Wizardspike = new Item
        {
            Id = 219329,
            Name = "巫师之刺",
            Quality = ItemQuality.Legendary,
            Slug = "wizardspike",
            ItemType = ItemType.Dagger,
            TrinityItemType = TrinityItemType.Dagger,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Dagger_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/wizardspike",
            Url = "https://us.battle.net/d3/en/item/wizardspike",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_dagger_010_x1_210_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/wizardspike",
            IsCrafted = false,
            LegendaryAffix = "进行一次攻击时有 (20-25)% 的几率射出一枚冰冻之球。",
            SetName = "",
        };

        /// <summary>
        /// Eun-jang-do Attacking enemies below 17–20 % Life freezes them for 3 seconds.
        /// </summary>
        public static Item Eunjangdo = new Item
        {
            Id = 410960,
            Name = "银桩刀",
            Quality = ItemQuality.Legendary,
            Slug = "eunjangdo",
            ItemType = ItemType.Dagger,
            TrinityItemType = TrinityItemType.Dagger,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/eunjangdo",
            Url = "https://us.battle.net/d3/en/item/eunjangdo",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_dagger_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/eunjangdo",
            IsCrafted = false,
            LegendaryAffix = "攻击生命值低于(17-20)%的敌人时，将使其被冰冻3秒。",
            SetName = "",
        };

        /// <summary>
        /// Karlei's Point Impale returns 10–15 Hatred if it hits an enemy already Impaled.
        /// </summary>
        public static Item KarleisPoint = new Item
        {
            Id = 271728,
            Name = "家妮的锋芒",
            Quality = ItemQuality.Legendary,
            Slug = "karleis-point",
            ItemType = ItemType.Dagger,
            TrinityItemType = TrinityItemType.Dagger,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/karleis-point",
            Url = "https://us.battle.net/d3/en/item/karleis-point",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_dagger_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/karleis-point",
            IsCrafted = false,
            LegendaryAffix = "击中一名已经被飞刀命中的敌人时，暗影飞刀返还 15 点憎恨值。暗影飞刀的伤害提高 300-375%。",
            SetName = "",
        };

        /// <summary>
        /// Lord Greenstone's Fan Every second, gain 160–200 % increased damage for your next Fan of Knives. Stacks up to 30 times.
        /// </summary>
        public static Item LordGreenstonesFan = new Item
        {
            Id = 271731,
            Name = "格林斯通刀王的匕扇",
            Quality = ItemQuality.Legendary,
            Slug = "lord-greenstones-fan",
            ItemType = ItemType.Dagger,
            TrinityItemType = TrinityItemType.Dagger,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/lord-greenstones-fan",
            Url = "https://us.battle.net/d3/en/item/lord-greenstones-fan",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_dagger_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/lord-greenstones-fan",
            IsCrafted = false,
            LegendaryAffix = "每隔一秒，你的下一个刀扇技能的伤害提高 (300-400)%，最多叠加 30 次。",
            SetName = "",
        };

        /// <summary>
        /// Odyn Son 20–40 % chance to Chain Lightning enemies when you hit them.
        /// </summary>
        public static Item OdynSon = new Item
        {
            Id = 188185,
            Name = "奥丁之子",
            Quality = ItemQuality.Legendary,
            Slug = "odyn-son",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.Mace,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Mace_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/odyn-son",
            Url = "https://us.battle.net/d3/en/item/odyn-son",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_1h_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/odyn-son",
            IsCrafted = false,
            LegendaryAffix = "击中敌人时有 (20-40)% 的几率施放连环闪电。",
            SetName = "",
        };

        /// <summary>
        /// Mad Monarch's Scepter After killing 10 enemies, you release a Poison Nova that deals 1050–1400 % weapon damage as Poison to enemies within 30 yards.
        /// </summary>
        public static Item MadMonarchsScepter = new Item
        {
            Id = 271663,
            Name = "狂君权杖",
            Quality = ItemQuality.Legendary,
            Slug = "mad-monarchs-scepter",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.Mace,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/mad-monarchs-scepter",
            Url = "https://us.battle.net/d3/en/item/mad-monarchs-scepter",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_1h_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/mad-monarchs-scepter",
            IsCrafted = false,
            LegendaryAffix = "在消灭10个敌人后，你会释放出剧毒新星，对30码内的敌人造成 (1050-1400)% 的武器伤害（作为毒性伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Nutcracker 
        /// </summary>
        public static Item Nutcracker = new Item
        {
            Id = 188169,
            Name = "坚果锤",
            Quality = ItemQuality.Legendary,
            Slug = "nutcracker",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.Mace,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Mace_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/nutcracker",
            Url = "https://us.battle.net/d3/en/item/nutcracker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_1h_005_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/nutcracker",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Telranden's Hand 
        /// </summary>
        public static Item TelrandensHand = new Item
        {
            Id = 188189,
            Name = "特尔兰登之手",
            Quality = ItemQuality.Legendary,
            Slug = "telrandens-hand",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.Mace,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Mace_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/telrandens-hand",
            Url = "https://us.battle.net/d3/en/item/telrandens-hand",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_1h_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/telrandens-hand",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Jace's Hammer of Vigilance Increase the size of your Blessed Hammers.
        /// </summary>
        public static Item JacesHammerOfVigilance = new Item
        {
            Id = 271648,
            Name = "杰斯的警戒之锤",
            Quality = ItemQuality.Legendary,
            Slug = "jaces-hammer-of-vigilance",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.Mace,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "mace_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/jaces-hammer-of-vigilance",
            Url = "https://us.battle.net/d3/en/item/jaces-hammer-of-vigilance",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_1h_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jaces-hammer-of-vigilance",
            IsCrafted = false,
            LegendaryAffix = "使祝福之锤的体积增大。",
            SetName = "",
        };

        /// <summary>
        /// Solanium Critical Hits have a 3–4 % chance to spawn a health globe.
        /// </summary>
        public static Item Solanium = new Item
        {
            Id = 271662,
            Name = "阳炎",
            Quality = ItemQuality.Legendary,
            Slug = "solanium",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.Mace,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "mace_norm_unique_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/solanium",
            Url = "https://us.battle.net/d3/en/item/solanium",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_1h_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/solanium",
            IsCrafted = false,
            LegendaryAffix = "暴击有 (3-4)% 的几率生成一个生命球。",
            SetName = "",
        };

        /// <summary>
        /// Nailbiter 
        /// </summary>
        public static Item Nailbiter = new Item
        {
            Id = 188158,
            Name = "钉咬",
            Quality = ItemQuality.Legendary,
            Slug = "nailbiter",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.Mace,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Mace_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/nailbiter",
            Url = "https://us.battle.net/d3/en/item/nailbiter",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_1h_008_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/nailbiter",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Neanderthal 
        /// </summary>
        public static Item Neanderthal = new Item
        {
            Id = 102665,
            Name = "尼安德特之锤",
            Quality = ItemQuality.Legendary,
            Slug = "neanderthal",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.Mace,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Mace_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/neanderthal",
            Url = "https://us.battle.net/d3/en/item/neanderthal",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_1h_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/neanderthal",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Echoing Fury 
        /// </summary>
        public static Item EchoingFury = new Item
        {
            Id = 188181,
            Name = "怒火回荡",
            Quality = ItemQuality.Legendary,
            Slug = "echoing-fury",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.Mace,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Mace_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/echoing-fury",
            Url = "https://us.battle.net/d3/en/item/echoing-fury",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_1h_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/echoing-fury",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Sun Keeper 
        /// </summary>
        public static Item SunKeeper = new Item
        {
            Id = 188173,
            Name = "日光守卫",
            Quality = ItemQuality.Legendary,
            Slug = "sun-keeper",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.Mace,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/sun-keeper",
            Url = "https://us.battle.net/d3/en/item/sun-keeper",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_1h_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sun-keeper",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Saffron Wrap The damage of your next Overpower is increased by 40–50 % for each enemy hit. Max 20 enemies.
        /// </summary>
        public static Item SaffronWrap = new Item
        {
            Id = 193664,
            Name = "藏红裹腰",
            Quality = ItemQuality.Legendary,
            Slug = "saffron-wrap",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Belt_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/saffron-wrap",
            Url = "https://us.battle.net/d3/en/item/saffron-wrap",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p43_unique_belt_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/saffron-wrap",
            IsCrafted = false,
            LegendaryAffix = "压制每击中一名敌人，你的下一次压制的伤害便提高 (40-50)%。最多 20 名敌人。",
            SetName = "",
        };

        /// <summary>
        /// Goldwrap On gold pickup: Gain armor for 5 seconds equal to the amount picked up.
        /// </summary>
        public static Item Goldwrap = new Item
        {
            Id = 193671,
            Name = "金织带",
            Quality = ItemQuality.Legendary,
            Slug = "goldwrap",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "belt_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/goldwrap",
            Url = "https://us.battle.net/d3/en/item/goldwrap",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_010_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/goldwrap",
            IsCrafted = false,
            LegendaryAffix = "拾取金币时：在5秒内获得额外的护甲值，相当于拾取的金币总量。",
            SetName = "",
        };

        /// <summary>
        /// Vigilante Belt 
        /// </summary>
        public static Item VigilanteBelt = new Item
        {
            Id = 193665,
            Name = "行侠腰带",
            Quality = ItemQuality.Legendary,
            Slug = "vigilante-belt",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Belt_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/vigilante-belt",
            Url = "https://us.battle.net/d3/en/item/vigilante-belt",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vigilante-belt",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Insatiable Belt Picking up a Health Globe increases your maximum Life by 5% for 15 seconds, stacking up to 5 times.
        /// </summary>
        public static Item InsatiableBelt = new Item
        {
            Id = 298126,
            Name = "贪婪腰带",
            Quality = ItemQuality.Legendary,
            Slug = "insatiable-belt",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/insatiable-belt",
            Url = "https://us.battle.net/d3/en/item/insatiable-belt",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/insatiable-belt",
            IsCrafted = false,
            LegendaryAffix = "拾取生命球会使你的生命值上限在15秒内提高5%，最多叠加5次。",
            SetName = "",
        };

        /// <summary>
        /// Binding of the Lost Each hit with Seven-Sided Strike grants 3.0–3.5 % damage reduction for 7 seconds.
        /// </summary>
        public static Item BindingOfTheLost = new Item
        {
            Id = 440425,
            Name = "失踪者的绑腰",
            Quality = ItemQuality.Legendary,
            Slug = "binding-of-the-lost",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/binding-of-the-lost",
            Url = "https://us.battle.net/d3/en/item/binding-of-the-lost",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_belt_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/binding-of-the-lost",
            IsCrafted = false,
            LegendaryAffix = "七相拳的每一击使伤害降低 (4-5)% ，持续7秒。",
            SetName = "",
        };

        /// <summary>
        /// The Shame of Delsere Your Signature Spells attack 50% faster and restore 9–12 Arcane Power.
        /// </summary>
        public static Item TheShameOfDelsere = new Item
        {
            Id = 440426,
            Name = "德尔西尼的耻辱",
            Quality = ItemQuality.Legendary,
            Slug = "the-shame-of-delsere",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-shame-of-delsere",
            Url = "https://us.battle.net/d3/en/item/the-shame-of-delsere",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_belt_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-shame-of-delsere",
            IsCrafted = false,
            LegendaryAffix = "你的修为法术攻击速度提高 50%，并且恢复 (9-12) 点奥能。",
            SetName = "",
        };

        /// <summary>
        /// Kyoshiro's Soul Sweeping Wind gains 2 stacks every second it does not deal damage to any enemies.
        /// </summary>
        public static Item KyoshirosSoul = new Item
        {
            Id = 298136,
            Name = "京四郎之魂",
            Quality = ItemQuality.Legendary,
            Slug = "kyoshiros-soul",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/kyoshiros-soul",
            Url = "https://us.battle.net/d3/en/item/kyoshiros-soul",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_belt_05_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/kyoshiros-soul",
            IsCrafted = false,
            LegendaryAffix = "劲风煞未对任何敌人造成伤害时，每秒获得 2 层叠加效果。",
            SetName = "",
        };

        /// <summary>
        /// Dayntee's Binding You gain an additional 40–50 % damage reduction when there is an enemy afflicted by your Decrepify.
        /// </summary>
        public static Item DaynteesBinding = new Item
        {
            Id = 476720,
            Name = "但提的束缚",
            Quality = ItemQuality.Legendary,
            Slug = "dayntees-binding",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/dayntees-binding",
            Url = "https://us.battle.net/d3/en/item/dayntees-binding",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_belt_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/dayntees-binding",
            IsCrafted = false,
            LegendaryAffix = "当有一名敌人受到你的任意诅咒影响时，你额外获得 40-50% 的伤害减免",
            SetName = "",
        };

        /// <summary>
        /// Sacred Harness Judgment gains the effect of the Debilitate rune and is cast at your landing location when casting Falling Sword.
        /// </summary>
        public static Item SacredHarness = new Item
        {
            Id = 440423,
            Name = "圣洁束带",
            Quality = ItemQuality.Legendary,
            Slug = "sacred-harness",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/sacred-harness",
            Url = "https://us.battle.net/d3/en/item/sacred-harness",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_belt_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sacred-harness",
            IsCrafted = false,
            LegendaryAffix = "施放天罚之剑时将在落地位置施放审判，且获得衰弱符文效果。",
            SetName = "",
        };

        /// <summary>
        /// Bakuli Jungle Wraps Firebats deals 150–200 % increased damage to enemies affected by Locust Swarm or Piranhas.
        /// </summary>
        public static Item BakuliJungleWraps = new Item
        {
            Id = 193674,
            Name = "巴库里丛林缠腰",
            Quality = ItemQuality.Legendary,
            Slug = "bakuli-jungle-wraps",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bakuli-jungle-wraps",
            Url = "https://us.battle.net/d3/en/item/bakuli-jungle-wraps",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_belt_007_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bakuli-jungle-wraps",
            IsCrafted = false,
            LegendaryAffix = "火蝠对受到瘟疫虫群或食人鱼影响的敌人造成的伤害提高 (250-300)%。",
            SetName = "",
        };

        /// <summary>
        /// String of Ears Reduces damage from melee attacks by 25–30 % .
        /// </summary>
        public static Item StringOfEars = new Item
        {
            Id = 193669,
            Name = "缠腰耳串",
            Quality = ItemQuality.Legendary,
            Slug = "string-of-ears",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Belt_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/string-of-ears",
            Url = "https://us.battle.net/d3/en/item/string-of-ears",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_belt_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/string-of-ears",
            IsCrafted = false,
            LegendaryAffix = "受到的近战伤害降低 (25-30)%。",
            SetName = "",
        };

        /// <summary>
        /// Fazula's Improbable Chain You automatically start with 40–50 Archon stacks when entering Archon form.
        /// </summary>
        public static Item FazulasImprobableChain = new Item
        {
            Id = 440424,
            Name = "法祖拉的不可信之链",
            Quality = ItemQuality.Legendary,
            Slug = "fazulas-improbable-chain",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/fazulas-improbable-chain",
            Url = "https://us.battle.net/d3/en/item/fazulas-improbable-chain",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_belt_07_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fazulas-improbable-chain",
            IsCrafted = false,
            LegendaryAffix = "进入御法者形态时你将自动获得 (40-50) 层御法者叠加效果。",
            SetName = "",
        };

        /// <summary>
        /// Hergbrash's Binding Reduces the Arcane Power cost of Arcane Torrent, Disintegrate, and Ray of Frost by 50–65 % .
        /// </summary>
        public static Item HergbrashsBinding = new Item
        {
            Id = 449047,
            Name = "赫布拉斯的缠腰",
            Quality = ItemQuality.Legendary,
            Slug = "hergbrashs-binding",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/hergbrashs-binding",
            Url = "https://us.battle.net/d3/en/item/hergbrashs-binding",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_belt_06_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/hergbrashs-binding",
            IsCrafted = false,
            LegendaryAffix = "奥术洪流、瓦解射线以及冰霜射线的奥能消耗减少 (50-65)%。",
            SetName = "",
        };

        /// <summary>
        /// Belt of Transcendence Summon a Fetish Sycophant when you hit with a Mana spender.
        /// </summary>
        public static Item BeltOfTranscendence = new Item
        {
            Id = 423248,
            Name = "通冥腰带",
            Quality = ItemQuality.Legendary,
            Slug = "belt-of-transcendence",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/belt-of-transcendence",
            Url = "https://us.battle.net/d3/en/item/belt-of-transcendence",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_belt_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/belt-of-transcendence",
            IsCrafted = false,
            LegendaryAffix = "当你用法力消耗技击中敌人时，召唤一个鬼娃跟班。",
            SetName = "",
        };

        /// <summary>
        /// Blessed of Haull Justice spawns a Blessed Hammer when it hits an enemy.
        /// </summary>
        public static Item BlessedOfHaull = new Item
        {
            Id = 423251,
            Name = "霍尔的祝福",
            Quality = ItemQuality.Legendary,
            Slug = "blessed-of-haull",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/blessed-of-haull",
            Url = "https://us.battle.net/d3/en/item/blessed-of-haull",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_belt_05_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blessed-of-haull",
            IsCrafted = false,
            LegendaryAffix = "每当正义击中敌人时，就会形成一把祝福之锤。",
            SetName = "",
        };

        /// <summary>
        /// Chain of Shadows After using Impale, Vault costs no resource for 2 seconds.
        /// </summary>
        public static Item ChainOfShadows = new Item
        {
            Id = 445497,
            Name = "暗影之链",
            Quality = ItemQuality.Legendary,
            Slug = "chain-of-shadows",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/chain-of-shadows",
            Url = "https://us.battle.net/d3/en/item/chain-of-shadows",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_belt_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/chain-of-shadows",
            IsCrafted = false,
            LegendaryAffix = "使用暗影飞刀后，影轮翻在 2 秒内不消耗能量。",
            SetName = "",
        };

        /// <summary>
        /// Cord of the Sherma Chance on hit to create a chaos field that Blinds and Slows enemies inside for 3–4 seconds.
        /// </summary>
        public static Item CordOfTheSherma = new Item
        {
            Id = 298127,
            Name = "谢尔曼的缠腰",
            Quality = ItemQuality.Legendary,
            Slug = "cord-of-the-sherma",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "belt_norm_unique_18",
            DataUrl = "https://us.battle.net/api/d3/data/item/cord-of-the-sherma",
            Url = "https://us.battle.net/d3/en/item/cord-of-the-sherma",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_104_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/cord-of-the-sherma",
            IsCrafted = false,
            LegendaryAffix = "击中时有一定几率生成混乱力场，使敌人被致盲并减速，持续 (3-4) 秒。",
            SetName = "",
        };

        /// <summary>
        /// Crashing Rain Rain of Vengeance also summons a crashing beast that deals 3000–4000 % weapon damage.
        /// </summary>
        public static Item CrashingRain = new Item
        {
            Id = 423247,
            Name = "崩天恨雨",
            Quality = ItemQuality.Legendary,
            Slug = "crashing-rain",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/crashing-rain",
            Url = "https://us.battle.net/d3/en/item/crashing-rain",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_belt_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/crashing-rain",
            IsCrafted = false,
            LegendaryAffix = "复仇之雨还会召唤一只撞击野兽，造成 (3000-4000)% 武器伤害。",
            SetName = "",
        };

        /// <summary>
        /// Harrington Waistguard Opening a chest grants 100–135 % increased damage for 10 seconds.
        /// </summary>
        public static Item HarringtonWaistguard = new Item
        {
            Id = 298129,
            Name = "哈林顿的护腰",
            Quality = ItemQuality.Legendary,
            Slug = "harrington-waistguard",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "StaffOfCow",
            DataUrl = "https://us.battle.net/api/d3/data/item/harrington-waistguard",
            Url = "https://us.battle.net/d3/en/item/harrington-waistguard",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_105_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/harrington-waistguard",
            IsCrafted = false,
            LegendaryAffix = "打开箱子可使伤害提高 (100-135)%，持续10秒。",
            SetName = "",
        };

        /// <summary>
        /// Haunting Girdle Haunt releases 1 extra spirit.
        /// </summary>
        public static Item HauntingGirdle = new Item
        {
            Id = 423249,
            Name = "附魂腰带",
            Quality = ItemQuality.Legendary,
            Slug = "haunting-girdle",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/haunting-girdle",
            Url = "https://us.battle.net/d3/en/item/haunting-girdle",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_belt_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/haunting-girdle",
            IsCrafted = false,
            LegendaryAffix = "蚀魂释放出 1 个额外的鬼魂。",
            SetName = "",
        };

        /// <summary>
        /// Hwoj Wrap Locust Swarm also Slows enemies by 60–80 % .
        /// </summary>
        public static Item HwojWrap = new Item
        {
            Id = 298131,
            Name = "沃贾裹腰",
            Quality = ItemQuality.Legendary,
            Slug = "hwoj-wrap",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/hwoj-wrap",
            Url = "https://us.battle.net/d3/en/item/hwoj-wrap",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_107_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/hwoj-wrap",
            IsCrafted = false,
            LegendaryAffix = "瘟疫虫群同时使敌人减速 (60-80)%。",
            SetName = "",
        };

        /// <summary>
        /// Omnislash Slash attacks in all directions.
        /// </summary>
        public static Item Omnislash = new Item
        {
            Id = 423250,
            Name = "万象皆杀",
            Quality = ItemQuality.Legendary,
            Slug = "omnislash",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/omnislash",
            Url = "https://us.battle.net/d3/en/item/omnislash",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_belt_04_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/omnislash",
            IsCrafted = false,
            LegendaryAffix = "朝四面八方发出烈焰斩。",
            SetName = "",
        };

        /// <summary>
        /// Omryn's Chain Drop Caltrops when using Vault.
        /// </summary>
        public static Item OmrynsChain = new Item
        {
            Id = 423261,
            Name = "欧姆瑞的链带",
            Quality = ItemQuality.Legendary,
            Slug = "omryns-chain",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/omryns-chain",
            Url = "https://us.battle.net/d3/en/item/omryns-chain",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_belt_06_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/omryns-chain",
            IsCrafted = false,
            LegendaryAffix = "在使用影轮翻时丢出铁蒺藜。",
            SetName = "",
        };

        /// <summary>
        /// Razor Strop Picking up a Health Globe releases an explosion that deals 300–400 % weapon damage as Fire to enemies within 20 yards.
        /// </summary>
        public static Item RazorStrop = new Item
        {
            Id = 298124,
            Name = "刀锋磨带",
            Quality = ItemQuality.Legendary,
            Slug = "razor-strop",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "belt_norm_unique_15",
            DataUrl = "https://us.battle.net/api/d3/data/item/razor-strop",
            Url = "https://us.battle.net/d3/en/item/razor-strop",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/razor-strop",
            IsCrafted = false,
            LegendaryAffix = "拾取生命球会造成爆破冲击，对20码内的敌人造成 (300-400)% 的武器伤害（作为火焰伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Sash of Knives With every attack, you throw a dagger at a nearby enemy for 500–650 % weapon damage as Physical.
        /// </summary>
        public static Item SashOfKnives = new Item
        {
            Id = 298125,
            Name = "飞刀束带",
            Quality = ItemQuality.Legendary,
            Slug = "sash-of-knives",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "belt_norm_unique_16",
            DataUrl = "https://us.battle.net/api/d3/data/item/sash-of-knives",
            Url = "https://us.battle.net/d3/en/item/sash-of-knives",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_102_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sash-of-knives",
            IsCrafted = false,
            LegendaryAffix = "每次攻击都会朝附近一名敌人掷出一把飞刀，造成 (500-650)% 的武器伤害（作为物理伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Sebor's Nightmare Haunt is cast on all nearby enemies when you open a chest.
        /// </summary>
        public static Item SeborsNightmare = new Item
        {
            Id = 299381,
            Name = "瑟伯的梦魇",
            Quality = ItemQuality.Legendary,
            Slug = "sebors-nightmare",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "belt_norm_unique_22",
            DataUrl = "https://us.battle.net/api/d3/data/item/sebors-nightmare",
            Url = "https://us.battle.net/d3/en/item/sebors-nightmare",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_108_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sebors-nightmare",
            IsCrafted = false,
            LegendaryAffix = "每当你打开一个箱子时，你就会对附近所有敌人施放蚀魂。",
            SetName = "",
        };

        /// <summary>
        /// Angel Hair Braid Punish gains the effect of every rune.
        /// </summary>
        public static Item AngelHairBraid = new Item
        {
            Id = 193666,
            Name = "天使发带",
            Quality = ItemQuality.Legendary,
            Slug = "angel-hair-braid",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "belt_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/angel-hair-braid",
            Url = "https://us.battle.net/d3/en/item/angel-hair-braid",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_003_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/angel-hair-braid",
            IsCrafted = false,
            LegendaryAffix = "惩罚获得每个符文的效果。",
            SetName = "",
        };

        /// <summary>
        /// Thundergod's Vigor Blocking, dodging or being hit causes you to discharge bolts of electricity that deal 100–130 % weapon damage as Lightning.
        /// </summary>
        public static Item ThundergodsVigor = new Item
        {
            Id = 212230,
            Name = "雷神之力",
            Quality = ItemQuality.Legendary,
            Slug = "thundergods-vigor",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "BarbBelt_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/thundergods-vigor",
            Url = "https://us.battle.net/d3/en/item/thundergods-vigor",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_barbbelt_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/thundergods-vigor",
            IsCrafted = false,
            LegendaryAffix = "格挡、躲闪或被命中都会使你释放闪电箭，造成 (100-130)% 的武器伤害（作为闪电伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Belt of the Trove Every 6–8 seconds, call down Bombardment on a random nearby enemy.
        /// </summary>
        public static Item BeltOfTheTrove = new Item
        {
            Id = 193675,
            Name = "宝藏腰带",
            Quality = ItemQuality.Legendary,
            Slug = "belt-of-the-trove",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/belt-of-the-trove",
            Url = "https://us.battle.net/d3/en/item/belt-of-the-trove",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_belt_008_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/belt-of-the-trove",
            IsCrafted = false,
            LegendaryAffix = "每隔 (6-8) 秒，对一名附近的敌人发起轰击。",
            SetName = "",
        };

        /// <summary>
        /// The Witching Hour 
        /// </summary>
        public static Item TheWitchingHour = new Item
        {
            Id = 193670,
            Name = "行巫时刻",
            Quality = ItemQuality.Legendary,
            Slug = "the-witching-hour",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-witching-hour",
            Url = "https://us.battle.net/d3/en/item/the-witching-hour",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-witching-hour",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Blackthorne's Notched Belt 
        /// </summary>
        public static Item BlackthornesNotchedBelt = new Item
        {
            Id = 224191,
            Name = "黑棘的刻痕腰带",
            Quality = ItemQuality.Legendary,
            Slug = "blackthornes-notched-belt",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Belt_norm_unique_14",
            DataUrl = "https://us.battle.net/api/d3/data/item/blackthornes-notched-belt",
            Url = "https://us.battle.net/d3/en/item/blackthornes-notched-belt",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_015_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blackthornes-notched-belt",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "黑棘的战铠",
        };

        /// <summary>
        /// Inna's Favor 
        /// </summary>
        public static Item InnasFavor = new Item
        {
            Id = 222487,
            Name = "尹娜的眷顾",
            Quality = ItemQuality.Legendary,
            Slug = "innas-favor",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Belt_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/innas-favor",
            Url = "https://us.battle.net/d3/en/item/innas-favor",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/innas-favor",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "尹娜的真言",
        };

        /// <summary>
        /// Tal Rasha's Brace 
        /// </summary>
        public static Item TalRashasBrace = new Item
        {
            Id = 212657,
            Name = "塔·拉夏的束带",
            Quality = ItemQuality.Legendary,
            Slug = "tal-rashas-brace",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "Belt_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/tal-rashas-brace",
            Url = "https://us.battle.net/d3/en/item/tal-rashas-brace",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/tal-rashas-brace",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "塔·拉夏的法理",
        };

        /// <summary>
        /// Jang's Envelopment Enemies damaged by Black Hole are also slowed by 60–80 % for 3 seconds.
        /// </summary>
        public static Item JangsEnvelopment = new Item
        {
            Id = 298130,
            Name = "张的腰封",
            Quality = ItemQuality.Legendary,
            Slug = "jangs-envelopment",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/jangs-envelopment",
            Url = "https://us.battle.net/d3/en/item/jangs-envelopment",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_106_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jangs-envelopment",
            IsCrafted = false,
            LegendaryAffix = "被黑洞伤害的敌人其速度降低 (60-80)%，持续3秒。",
            SetName = "",
        };

        /// <summary>
        /// Hunter's Wrath Your primary skills attack 30% faster and deal 45–60 % increased damage.
        /// </summary>
        public static Item HuntersWrath = new Item
        {
            Id = 440742,
            Name = "猎手之怒",
            Quality = ItemQuality.Legendary,
            Slug = "hunters-wrath",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/hunters-wrath",
            Url = "https://us.battle.net/d3/en/item/hunters-wrath",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_belt_005_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/hunters-wrath",
            IsCrafted = false,
            LegendaryAffix = "你的主要技能的攻击速度提高 30% ，造成的伤害提高 (45-60)% 。",
            SetName = "",
        };

        /// <summary>
        /// Khassett's Cord of Righteousness Fist of the Heavens costs 40% less Wrath and deals 130–170 % more damage.
        /// </summary>
        public static Item KhassettsCordOfRighteousness = new Item
        {
            Id = 0,
            Name = "哈塞特的正义束带",
            Quality = ItemQuality.Legendary,
            Slug = "khassetts-cord-of-righteousness",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/khassetts-cord-of-righteousness",
            Url = "https://us.battle.net/d3/en/item/khassetts-cord-of-righteousness",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p42_crusader_foh_belt_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/khassetts-cord-of-righteousness",
            IsCrafted = false,
            LegendaryAffix = "天堂之拳消耗的愤怒值降低 40%，造成的伤害提高 (130-170)%。",
            SetName = "",
        };

        /// <summary>
        /// Zoey's Secret You take 8.0–9.0 % less damage for every Companion you have active.
        /// </summary>
        public static Item ZoeysSecret = new Item
        {
            Id = 298137,
            Name = "佐伊的秘密",
            Quality = ItemQuality.Legendary,
            Slug = "zoeys-secret",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/zoeys-secret",
            Url = "https://us.battle.net/d3/en/item/zoeys-secret",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_belt_04_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/zoeys-secret",
            IsCrafted = false,
            LegendaryAffix = "你每拥有一只激活的战宠，你受到的伤害便降低 (8.0-9.0)%。",
            SetName = "",
        };

        /// <summary>
        /// Krelm's Buff Belt Gain 25% run speed. This effect is lost for 5 seconds after taking damage.
        /// </summary>
        public static Item KrelmsBuffBelt = new Item
        {
            Id = 336184,
            Name = "克雷姆的强力腰带",
            Quality = ItemQuality.Legendary,
            Slug = "krelms-buff-belt",
            ItemType = ItemType.Belt,
            TrinityItemType = TrinityItemType.Belt,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Armor,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/krelms-buff-belt",
            Url = "https://us.battle.net/d3/en/item/krelms-buff-belt",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_belt_set_02_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/krelms-buff-belt",
            IsCrafted = false,
            LegendaryAffix = "奔跑速度提高25% 。该效果会在受到伤害5秒后消失。",
            SetName = "克雷姆的强力壁垒",
        };

        /// <summary>
        /// Spear of Jairo Your Thorns is increased by 10–15 % for every enemy afflicted by one of your curses.
        /// </summary>
        public static Item SpearOfJairo = new Item
        {
            Id = 470273,
            Name = "罗嘉的长矛",
            Quality = ItemQuality.Legendary,
            Slug = "spear-of-jairo",
            ItemType = ItemType.Spear,
            TrinityItemType = TrinityItemType.Spear,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/spear-of-jairo",
            Url = "https://us.battle.net/d3/en/item/spear-of-jairo",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p6_unique_spear_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/spear-of-jairo",
            IsCrafted = false,
            LegendaryAffix = "你的任意诅咒每影响一个敌人，你的荆棘伤害就会提高（10-15）%。",
            SetName = "",
        };

        /// <summary>
        /// Arreat's Law Weapon Throw generates up to 15–20 additional Fury based on how far away the enemy hit is. Maximum benefit when the enemy hit is 20 or more yards away.
        /// </summary>
        public static Item ArreatsLaw = new Item
        {
            Id = 191446,
            Name = "亚瑞特之律",
            Quality = ItemQuality.Legendary,
            Slug = "arreats-law",
            ItemType = ItemType.Spear,
            TrinityItemType = TrinityItemType.Spear,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Spear_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/arreats-law",
            Url = "https://us.battle.net/d3/en/item/arreats-law",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_spear_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/arreats-law",
            IsCrafted = false,
            LegendaryAffix = "武器飞掷最多生成 (15-20) 点额外怒气（基于被命中的敌人的距离），当被命中的敌人距离你 20 码或更远时取最大值。",
            SetName = "",
        };

        /// <summary>
        /// Scrimshaw Reduces the Mana cost of Zombie Charger by 40–50 % .
        /// </summary>
        public static Item Scrimshaw = new Item
        {
            Id = 197095,
            Name = "鲸骨利矛",
            Quality = ItemQuality.Legendary,
            Slug = "scrimshaw",
            ItemType = ItemType.Spear,
            TrinityItemType = TrinityItemType.Spear,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Spear_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/scrimshaw",
            Url = "https://us.battle.net/d3/en/item/scrimshaw",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spear_004_p3_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/scrimshaw",
            IsCrafted = false,
            LegendaryAffix = "使僵尸死士的法力消耗降低 (40-50)%。",
            SetName = "",
        };

        /// <summary>
        /// The Three Hundredth Spear Increase the damage of Weapon Throw and Ancient Spear by 45–60 % .
        /// </summary>
        public static Item TheThreeHundredthSpear = new Item
        {
            Id = 196638,
            Name = "三百壮矛",
            Quality = ItemQuality.Legendary,
            Slug = "the-three-hundredth-spear",
            ItemType = ItemType.Spear,
            TrinityItemType = TrinityItemType.Spear,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Spear_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-three-hundredth-spear",
            Url = "https://us.battle.net/d3/en/item/the-three-hundredth-spear",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_spear_002_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-three-hundredth-spear",
            IsCrafted = false,
            LegendaryAffix = "武器飞掷和上古之矛的伤害提高 (45-60)%。",
            SetName = "",
        };

        /// <summary>
        /// Empyrean Messenger 
        /// </summary>
        public static Item EmpyreanMessenger = new Item
        {
            Id = 194241,
            Name = "天穹信使",
            Quality = ItemQuality.Legendary,
            Slug = "empyrean-messenger",
            ItemType = ItemType.Spear,
            TrinityItemType = TrinityItemType.Spear,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "spear_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/empyrean-messenger",
            Url = "https://us.battle.net/d3/en/item/empyrean-messenger",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spear_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/empyrean-messenger",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Akanesh, the Herald of Righteousness 
        /// </summary>
        public static Item AkaneshTheHeraldOfRighteousness = new Item
        {
            Id = 272043,
            Name = "阿坎内什,正义使者",
            Quality = ItemQuality.Legendary,
            Slug = "akanesh-the-herald-of-righteousness",
            ItemType = ItemType.Spear,
            TrinityItemType = TrinityItemType.Spear,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "spear_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/akanesh-the-herald-of-righteousness",
            Url = "https://us.battle.net/d3/en/item/akanesh-the-herald-of-righteousness",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_spear_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/akanesh-the-herald-of-righteousness",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Deadly Rebirth Grasp of the Dead gains the effect of the Rain of Corpses rune.
        /// </summary>
        public static Item DeadlyRebirth = new Item
        {
            Id = 193433,
            Name = "还魂",
            Quality = ItemQuality.Legendary,
            Slug = "deadly-rebirth",
            ItemType = ItemType.CeremonialDagger,
            TrinityItemType = TrinityItemType.CeremonialKnife,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/deadly-rebirth",
            Url = "https://us.battle.net/d3/en/item/deadly-rebirth",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ceremonialdagger_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/deadly-rebirth",
            IsCrafted = false,
            LegendaryAffix = "亡者之握获得天降尸雨符文的效果。",
            SetName = "",
        };

        /// <summary>
        /// Rhen'ho Flayer Plague of Toads now seek out enemies and can explode twice.
        /// </summary>
        public static Item RhenhoFlayer = new Item
        {
            Id = 271745,
            Name = "任霍的剥皮刀",
            Quality = ItemQuality.Legendary,
            Slug = "rhenho-flayer",
            ItemType = ItemType.CeremonialDagger,
            TrinityItemType = TrinityItemType.CeremonialKnife,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/rhenho-flayer",
            Url = "https://us.battle.net/d3/en/item/rhenho-flayer",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ceremonialdagger_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rhenho-flayer",
            IsCrafted = false,
            LegendaryAffix = "蟾蜍之疫现在会追踪敌人，并爆炸两次。",
            SetName = "",
        };

        /// <summary>
        /// Sacred Harvester Soul Harvest now stacks up to 10 times.
        /// </summary>
        public static Item SacredHarvester = new Item
        {
            Id = 403748,
            Name = "神圣收割者",
            Quality = ItemQuality.Legendary,
            Slug = "sacred-harvester",
            ItemType = ItemType.CeremonialDagger,
            TrinityItemType = TrinityItemType.CeremonialKnife,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "p1_ceremonialDagger_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/sacred-harvester",
            Url = "https://us.battle.net/d3/en/item/sacred-harvester",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p1_ceremonialdagger_norm_unique_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sacred-harvester",
            IsCrafted = false,
            LegendaryAffix = "灵魂收割现在最多可叠加至10层。",
            SetName = "",
        };

        /// <summary>
        /// The Dagger of Darts Your Poison Darts and your Fetishes' Poison Darts now pierce.
        /// </summary>
        public static Item TheDaggerOfDarts = new Item
        {
            Id = 403767,
            Name = "箭镖匕刃",
            Quality = ItemQuality.Legendary,
            Slug = "the-dagger-of-darts",
            ItemType = ItemType.CeremonialDagger,
            TrinityItemType = TrinityItemType.CeremonialKnife,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "p1_ceremonialDagger_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-dagger-of-darts",
            Url = "https://us.battle.net/d3/en/item/the-dagger-of-darts",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p1_ceremonialdagger_norm_unique_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-dagger-of-darts",
            IsCrafted = false,
            LegendaryAffix = "你和鬼娃的剧毒飞镖现在可穿透敌人。",
            SetName = "",
        };

        ///// <summary>
        ///// The Barber Instead of dealing direct damage, your Spirit Barrage now accumulates on the target. When you stop casting, it explodes dealing 225–250 % of the accumulated damage to all enemies within 15 yards.
        ///// </summary>
        //public static Item TheBarber = new Item
        //{
        //    Id = 0,
        //    Name = "剃头师",
        //    Quality = ItemQuality.Legendary,
        //    Slug = "the-barber-ReRBQ",
        //    ItemType = ItemType.CeremonialDagger,
        //    TrinityItemType = TrinityItemType.CeremonialKnife,
        //    IsTwoHanded = false,
        //    BaseType = ItemBaseType.Weapon,
        //    InternalName = "",
        //    DataUrl = "https://us.battle.net/api/d3/data/item/the-barber-ReRBQ",
        //    Url = "https://us.battle.net/d3/en/item/the-barber-ReRBQ",
        //    IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p5_unique_dagger_003_x1_demonhunter_male.png",
        //    RelativeUrl = "/d3/en/item/the-barber-ReRBQ",
        //    IsCrafted = false,
        //    LegendaryAffix = "你的灵魂弹幕不再造成直接伤害，而是在目标身上累积。当你停止施法时，它将爆炸并对 15 码范围内的所有敌人造成 (225-250)% 的累积伤害。",
        //    SetName = "",
        //};

        /// <summary>
        /// Last Breath Reduces cooldown of Mass Confusion by 15–20 seconds.
        /// </summary>
        public static Item LastBreath = new Item
        {
            Id = 195370,
            Name = "临终之息",
            Quality = ItemQuality.Legendary,
            Slug = "last-breath",
            ItemType = ItemType.CeremonialDagger,
            TrinityItemType = TrinityItemType.CeremonialKnife,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "ceremonialDagger_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/last-breath",
            Url = "https://us.battle.net/d3/en/item/last-breath",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_ceremonialdagger_008_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/last-breath",
            IsCrafted = false,
            LegendaryAffix = "使群体混乱的冷却时间缩短 (15-20) 秒。",
            SetName = "",
        };

        /// <summary>
        /// The Spider Queen's Grasp Corpse Spiders releases a web on impact that Slows enemies by 60–80 % .
        /// </summary>
        public static Item TheSpiderQueensGrasp = new Item
        {
            Id = 222978,
            Name = "蛛后之缚",
            Quality = ItemQuality.Legendary,
            Slug = "the-spider-queens-grasp",
            ItemType = ItemType.CeremonialDagger,
            TrinityItemType = TrinityItemType.CeremonialKnife,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "ceremonialDagger_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-spider-queens-grasp",
            Url = "https://us.battle.net/d3/en/item/the-spider-queens-grasp",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ceremonialdagger_004_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-spider-queens-grasp",
            IsCrafted = false,
            LegendaryAffix = "尸蛛击中后会放出蜘蛛网，使敌人减速 (60-80)%。",
            SetName = "",
        };

        /// <summary>
        /// Starmetal Kukri Reduce the cooldown of Fetish Army and Big Bad Voodoo by 1 second each time your fetishes deal damage.
        /// </summary>
        public static Item StarmetalKukri = new Item
        {
            Id = 271738,
            Name = "星铁反曲刀",
            Quality = ItemQuality.Legendary,
            Slug = "starmetal-kukri",
            ItemType = ItemType.CeremonialDagger,
            TrinityItemType = TrinityItemType.CeremonialKnife,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "ceremonialdagger_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/starmetal-kukri",
            Url = "https://us.battle.net/d3/en/item/starmetal-kukri",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ceremonialdagger_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/starmetal-kukri",
            IsCrafted = false,
            LegendaryAffix = "每当你的鬼娃造成伤害时，即可使你的鬼娃大军和巫毒狂舞的冷却时间缩短1秒。",
            SetName = "",
        };

        /// <summary>
        /// Anessazi Edge Zombie Dogs stuns enemies around them for 1.5 seconds when summoned.
        /// </summary>
        public static Item AnessaziEdge = new Item
        {
            Id = 196250,
            Name = "阿内萨兹之锋",
            Quality = ItemQuality.Legendary,
            Slug = "anessazi-edge",
            ItemType = ItemType.CeremonialDagger,
            TrinityItemType = TrinityItemType.CeremonialKnife,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "ceremonialDagger_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/anessazi-edge",
            Url = "https://us.battle.net/d3/en/item/anessazi-edge",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ceremonialdagger_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/anessazi-edge",
            IsCrafted = false,
            LegendaryAffix = "僵尸犬被召唤时，可使附近的敌人昏迷1.5秒。",
            SetName = "",
        };

        /// <summary>
        /// Voo's Juicer Spirit Barrage gains the effects of the Phlebotomize and The Spirit is Willing runes.
        /// </summary>
        public static Item VoosJuicer = new Item
        {
            Id = 192579,
            Name = "老巫榨汁器",
            Quality = ItemQuality.Legendary,
            Slug = "voos-juicer",
            ItemType = ItemType.CeremonialDagger,
            TrinityItemType = TrinityItemType.CeremonialKnife,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/voos-juicer",
            Url = "https://us.battle.net/d3/en/item/voos-juicer",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_dagger_002_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/voos-juicer",
            IsCrafted = false,
            LegendaryAffix = "灵魂弹幕获得魅魂飞弹和万灵之愿符文效果。",
            SetName = "",
        };

        /// <summary>
        /// The Gidbinn Chance to summon a Fetish when attacking.
        /// </summary>
        public static Item TheGidbinn = new Item
        {
            Id = 209246,
            Name = "吉德宾",
            Quality = ItemQuality.Legendary,
            Slug = "the-gidbinn",
            ItemType = ItemType.CeremonialDagger,
            TrinityItemType = TrinityItemType.CeremonialKnife,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "ceremonialDagger_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-gidbinn",
            Url = "https://us.battle.net/d3/en/item/the-gidbinn",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ceremonialdagger_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-gidbinn",
            IsCrafted = false,
            LegendaryAffix = "攻击时有一定几率召唤一名鬼娃 (40%)。",
            SetName = "",
        };

        /// <summary>
        /// Manajuma's Carving Knife 
        /// </summary>
        public static Item ManajumasCarvingKnife = new Item
        {
            Id = 223365,
            Name = "马纳祖玛的雕骨刀",
            Quality = ItemQuality.Legendary,
            Slug = "manajumas-carving-knife",
            ItemType = ItemType.CeremonialDagger,
            TrinityItemType = TrinityItemType.CeremonialKnife,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "ceremonialDagger_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/manajumas-carving-knife",
            Url = "https://us.battle.net/d3/en/item/manajumas-carving-knife",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_ceremonialdagger_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/manajumas-carving-knife",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "马纳祖玛之道",
        };

        /// <summary>
        /// Crystal Fist Dashing Strike reduces your damage taken by 40–50 % for 6 seconds.
        /// </summary>
        public static Item CrystalFist = new Item
        {
            Id = 175939,
            Name = "水晶拳",
            Quality = ItemQuality.Legendary,
            Slug = "crystal-fist",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "fistWeapon_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/crystal-fist",
            Url = "https://us.battle.net/d3/en/item/crystal-fist",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_fist_008_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/crystal-fist",
            IsCrafted = false,
            LegendaryAffix = "疾风击使你受到的伤害降低 (40-50)%，持续 6 秒。",
            SetName = "",
        };

        /// <summary>
        /// Fleshrake Dashing Strike increases the damage of Dashing Strike by 75–100 % for 1 second, stacking up to 5 times.
        /// </summary>
        public static Item Fleshrake = new Item
        {
            Id = 145850,
            Name = "肉耙",
            Quality = ItemQuality.Legendary,
            Slug = "fleshrake",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "fistWeapon_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/fleshrake",
            Url = "https://us.battle.net/d3/en/item/fleshrake",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_fist_007_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fleshrake",
            IsCrafted = false,
            LegendaryAffix = "疾风击使疾风击的伤害提高 (75-100)%，持续 1 秒，最多叠加 5 次。",
            SetName = "",
        };

        /// <summary>
        /// Scarbringer The damage of Lashing Tail Kick is increased by 300% to the first 5–7 enemies hit.
        /// </summary>
        public static Item Scarbringer = new Item
        {
            Id = 130557,
            Name = "伤痕使者",
            Quality = ItemQuality.Legendary,
            Slug = "scarbringer",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "fistWeapon_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/scarbringer",
            Url = "https://us.battle.net/d3/en/item/scarbringer",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p42_unique_fist_013_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/scarbringer",
            IsCrafted = false,
            LegendaryAffix = "神龙摆尾对击中的前7名敌人造成的伤害提高 450-600%。",
            SetName = "",
        };

        /// <summary>
        /// Sledge Fist 
        /// </summary>
        public static Item SledgeFist = new Item
        {
            Id = 175938,
            Name = "锤拳",
            Quality = ItemQuality.Legendary,
            Slug = "sledge-fist",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "fistWeapon_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/sledge-fist",
            Url = "https://us.battle.net/d3/en/item/sledge-fist",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_fist_012_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sledge-fist",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Jawbreaker When Dashing Strike hits an enemy more than 30–35 yards away, its Charge cost is refunded.
        /// </summary>
        public static Item Jawbreaker = new Item
        {
            Id = 271957,
            Name = "断颚",
            Quality = ItemQuality.Legendary,
            Slug = "jawbreaker",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "fistweapon_norm_unique_14",
            DataUrl = "https://us.battle.net/api/d3/data/item/jawbreaker",
            Url = "https://us.battle.net/d3/en/item/jawbreaker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_fist_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/jawbreaker",
            IsCrafted = false,
            LegendaryAffix = "当疾风击击中 (30-35) 码之外的敌人时，会返还使用次数。",
            SetName = "",
        };

        /// <summary>
        /// Logan's Claw 
        /// </summary>
        public static Item LogansClaw = new Item
        {
            Id = 145849,
            Name = "金刚狼爪",
            Quality = ItemQuality.Legendary,
            Slug = "logans-claw",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "fistweapon_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/logans-claw",
            Url = "https://us.battle.net/d3/en/item/logans-claw",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_fist_005_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/logans-claw",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// The Fist of Az'Turrasq Exploding Palm's on-death explosion damage is increased by 250–300 % .
        /// </summary>
        public static Item TheFistOfAzturrasq = new Item
        {
            Id = 175937,
            Name = "阿兹·图拉斯克之拳",
            Quality = ItemQuality.Legendary,
            Slug = "the-fist-of-azturrasq",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "fistWeapon_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-fist-of-azturrasq",
            Url = "https://us.battle.net/d3/en/item/the-fist-of-azturrasq",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_fist_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-fist-of-azturrasq",
            IsCrafted = false,
            LegendaryAffix = "爆裂掌的死亡爆破伤害提高 (400-500)%。",
            SetName = "",
        };

        /// <summary>
        /// Won Khim Lau 
        /// </summary>
        public static Item WonKhimLau = new Item
        {
            Id = 145851,
            Name = "流云桥",
            Quality = ItemQuality.Legendary,
            Slug = "won-khim-lau",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "fistWeapon_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/won-khim-lau",
            Url = "https://us.battle.net/d3/en/item/won-khim-lau",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_fist_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/won-khim-lau",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Shenlong's Fist of Legend 
        /// </summary>
        public static Item ShenlongsFistOfLegend = new Item
        {
            Id = 208996,
            Name = "神龙的传说之拳",
            Quality = ItemQuality.Legendary,
            Slug = "shenlongs-fist-of-legend",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "fistWeapon_norm_unique_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/shenlongs-fist-of-legend",
            Url = "https://us.battle.net/d3/en/item/shenlongs-fist-of-legend",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_fist_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/shenlongs-fist-of-legend",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "神龙之魂",
        };

        /// <summary>
        /// Shenlong's Relentless Assault 
        /// </summary>
        public static Item ShenlongsRelentlessAssault = new Item
        {
            Id = 208898,
            Name = "神龙的无情猛袭",
            Quality = ItemQuality.Legendary,
            Slug = "shenlongs-relentless-assault",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "fistWeapon_norm_unique_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/shenlongs-relentless-assault",
            Url = "https://us.battle.net/d3/en/item/shenlongs-relentless-assault",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_fist_010_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/shenlongs-relentless-assault",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "神龙之魂",
        };

        /// <summary>
        /// Rabid Strike Spirit spenders that teleport you while Epiphany is active are also mimicked on a nearby target for free.
        /// </summary>
        public static Item RabidStrike = new Item
        {
            Id = 196472,
            Name = "狂击",
            Quality = ItemQuality.Legendary,
            Slug = "rabid-strike",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "fistweapon_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/rabid-strike",
            Url = "https://us.battle.net/d3/en/item/rabid-strike",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p43_unique_fist_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/rabid-strike",
            IsCrafted = false,
            LegendaryAffix = "在灵光悟状态下打出使你传送的内力消耗技，还会免费模仿攻击远处的一名目标。",
            SetName = "",
        };

        /// <summary>
        /// Kyoshiro's Blade Increase the damage of Wave of Light by 150% . When the initial impact of your Wave of Light hits 3 or fewer enemies, the damage is increased by 200–250 % .
        /// </summary>
        public static Item KyoshirosBlade = new Item
        {
            Id = 271963,
            Name = "京四郎之刃",
            Quality = ItemQuality.Legendary,
            Slug = "kyoshiros-blade",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/kyoshiros-blade",
            Url = "https://us.battle.net/d3/en/item/kyoshiros-blade",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_fist_102_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/kyoshiros-blade",
            IsCrafted = false,
            LegendaryAffix = "金钟破的伤害提高 150%。当你的金钟破初始轰击击中 3 名或更少的敌人时，其伤害提高 (200-250)%。",
            SetName = "",
        };

        /// <summary>
        /// Lion's Claw Seven-Sided Strike performs an additional 7 strikes.
        /// </summary>
        public static Item LionsClaw = new Item
        {
            Id = 403772,
            Name = "狮爪",
            Quality = ItemQuality.Legendary,
            Slug = "lions-claw",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/lions-claw",
            Url = "https://us.battle.net/d3/en/item/lions-claw",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p1_fistweapon_norm_unique_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/lions-claw",
            IsCrafted = false,
            LegendaryAffix = "七相拳会额外施展 7 次攻击。",
            SetName = "",
        };

        /// <summary>
        /// Vengeful Wind Increases the maximum stack count of Sweeping Wind by 6–7 .
        /// </summary>
        public static Item VengefulWind = new Item
        {
            Id = 403775,
            Name = "复仇之风",
            Quality = ItemQuality.Legendary,
            Slug = "vengeful-wind",
            ItemType = ItemType.FistWeapon,
            TrinityItemType = TrinityItemType.FistWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "p1_fistWeapon_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/vengeful-wind",
            Url = "https://us.battle.net/d3/en/item/vengeful-wind",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_fistweapon_norm_unique_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vengeful-wind",
            IsCrafted = false,
            LegendaryAffix = "使劲风煞的层数上限提高 (6-7)。",
            SetName = "",
        };

        /// <summary>
        /// Johanna's Argument Increase the attack speed and damage of Blessed Hammer by 100 % .
        /// </summary>
        public static Item JohannasArgument = new Item
        {
            Id = 403812,
            Name = "乔汉娜的辩护",
            Quality = ItemQuality.Legendary,
            Slug = "johannas-argument",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.Flail,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/johannas-argument",
            Url = "https://us.battle.net/d3/en/item/johannas-argument",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p1_flail1h_norm_unique_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/johannas-argument",
            IsCrafted = false,
            LegendaryAffix = "祝福之锤的攻击速度和伤害提高 100% 。",
            SetName = "",
        };

        /// <summary>
        /// Darklight Fist of the Heavens has a 45–60 % chance to be cast twice.
        /// </summary>
        public static Item Darklight = new Item
        {
            Id = 299428,
            Name = "黑暗之光",
            Quality = ItemQuality.Legendary,
            Slug = "darklight",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.Flail,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "flail1h_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/darklight",
            Url = "https://us.battle.net/d3/en/item/darklight",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p42_unique_flail_1h_106_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/darklight",
            IsCrafted = false,
            LegendaryAffix = "天堂之拳有 (45-60)% 的几率施放两次。",
            SetName = "",
        };

        /// <summary>
        /// Gyrfalcon's Foote Removes the resource cost of Blessed Shield.
        /// </summary>
        public static Item GyrfalconsFoote = new Item
        {
            Id = 299427,
            Name = "矛隼之爪",
            Quality = ItemQuality.Legendary,
            Slug = "gyrfalcons-foote",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.Flail,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "flail1h_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/gyrfalcons-foote",
            Url = "https://us.battle.net/d3/en/item/gyrfalcons-foote",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_flail_1h_105_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/gyrfalcons-foote",
            IsCrafted = false,
            LegendaryAffix = "移除祝福之盾的能量消耗，并且伤害提高 275-350%。",
            SetName = "",
        };

        /// <summary>
        /// Inviolable Faith Casting Consecration also casts Consecration beneath all of your allies.
        /// </summary>
        public static Item InviolableFaith = new Item
        {
            Id = 299429,
            Name = "至高信仰",
            Quality = ItemQuality.Legendary,
            Slug = "inviolable-faith",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.Flail,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "flail1h_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/inviolable-faith",
            Url = "https://us.battle.net/d3/en/item/inviolable-faith",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_flail_1h_107_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/inviolable-faith",
            IsCrafted = false,
            LegendaryAffix = "奉献同时会施放在你所有同伴脚下。",
            SetName = "",
        };

        /// <summary>
        /// Justinian's Mercy Blessed Hammer gains the effect of the Dominion rune.
        /// </summary>
        public static Item JustiniansMercy = new Item
        {
            Id = 299424,
            Name = "加斯迪安的仁慈",
            Quality = ItemQuality.Legendary,
            Slug = "justinians-mercy",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.Flail,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "flail1h_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/justinians-mercy",
            Url = "https://us.battle.net/d3/en/item/justinians-mercy",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_flail_1h_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/justinians-mercy",
            IsCrafted = false,
            LegendaryAffix = "祝福之锤获得统御之锤符文的效果。",
            SetName = "",
        };

        /// <summary>
        /// Kassar's Retribution Casting Justice increases your movement speed by 15–20 % for 2 seconds.
        /// </summary>
        public static Item KassarsRetribution = new Item
        {
            Id = 299426,
            Name = "卡萨的惩戒",
            Quality = ItemQuality.Legendary,
            Slug = "kassars-retribution",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.Flail,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "flail1h_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/kassars-retribution",
            Url = "https://us.battle.net/d3/en/item/kassars-retribution",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_flail_1h_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/kassars-retribution",
            IsCrafted = false,
            LegendaryAffix = "施放正义使你的移动速度提高 (15-20)%，持续2秒。",
            SetName = "",
        };

        /// <summary>
        /// Swiftmount Doubles the duration of Steed Charge.
        /// </summary>
        public static Item Swiftmount = new Item
        {
            Id = 299425,
            Name = "天马连枷",
            Quality = ItemQuality.Legendary,
            Slug = "swiftmount",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.Flail,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "flail1h_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/swiftmount",
            Url = "https://us.battle.net/d3/en/item/swiftmount",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_flail_1h_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/swiftmount",
            IsCrafted = false,
            LegendaryAffix = "使战马冲锋的持续时间翻倍。",
            SetName = "",
        };

        /// <summary>
        /// Genzaniku Chance to summon a ghostly Fallen Champion when attacking.
        /// </summary>
        public static Item Genzaniku = new Item
        {
            Id = 116386,
            Name = "根扎尼库",
            Quality = ItemQuality.Legendary,
            Slug = "genzaniku",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.Axe,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Axe_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/genzaniku",
            Url = "https://us.battle.net/d3/en/item/genzaniku",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_axe_1h_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/genzaniku",
            IsCrafted = false,
            LegendaryAffix = "攻击时有一定几率召唤出一名堕落者勇士的鬼魂 (30%)。",
            SetName = "",
        };

        /// <summary>
        /// Flesh Tearer 
        /// </summary>
        public static Item FleshTearer = new Item
        {
            Id = 116388,
            Name = "裂肉者",
            Quality = ItemQuality.Legendary,
            Slug = "flesh-tearer",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.Axe,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "shoulderpads_norm_set_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/flesh-tearer",
            Url = "https://us.battle.net/d3/en/item/flesh-tearer",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_axe_1h_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/flesh-tearer",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Hack 75–100 % of your Thorns damage is applied on every attack.
        /// </summary>
        public static Item Hack = new Item
        {
            Id = 271598,
            Name = "劈肉斧",
            Quality = ItemQuality.Legendary,
            Slug = "hack",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.Axe,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "axe_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/hack",
            Url = "https://us.battle.net/d3/en/item/hack",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_axe_1h_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/hack",
            IsCrafted = false,
            LegendaryAffix = "每次攻击附加你荆棘伤害的 (75-100)%。",
            SetName = "",
        };

        /// <summary>
        /// The Butcher's Sickle 20–25 % chance to drag enemies to you when attacking.
        /// </summary>
        public static Item TheButchersSickle = new Item
        {
            Id = 189973,
            Name = "屠夫钩镰",
            Quality = ItemQuality.Legendary,
            Slug = "the-butchers-sickle",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.Axe,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Axe_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-butchers-sickle",
            Url = "https://us.battle.net/d3/en/item/the-butchers-sickle",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_axe_1h_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-butchers-sickle",
            IsCrafted = false,
            LegendaryAffix = "攻击时有 (20-25)% 的几率将敌人拖到你的身前。",
            SetName = "",
        };

        /// <summary>
        /// Sky Splitter 15–20 % chance to Smite enemies for 600-750% weapon damage as Lightning when you hit them.
        /// </summary>
        public static Item SkySplitter = new Item
        {
            Id = 116389,
            Name = "破天",
            Quality = ItemQuality.Legendary,
            Slug = "sky-splitter",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.Axe,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/sky-splitter",
            Url = "https://us.battle.net/d3/en/item/sky-splitter",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_axe_1h_005_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sky-splitter",
            IsCrafted = false,
            LegendaryAffix = "每当你击中敌人时，有 (15-20)% 的几率对其重击，造成 600-750% 的武器伤害（作为闪电伤害）。",
            SetName = "",
        };

        /// <summary>
        /// The Burning Axe of Sankis Chance to fight through the pain when enemies hit you.
        /// </summary>
        public static Item TheBurningAxeOfSankis = new Item
        {
            Id = 181484,
            Name = "桑基斯的烈焰斧",
            Quality = ItemQuality.Legendary,
            Slug = "the-burning-axe-of-sankis",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.Axe,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Axe_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-burning-axe-of-sankis",
            Url = "https://us.battle.net/d3/en/item/the-burning-axe-of-sankis",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_axe_1h_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-burning-axe-of-sankis",
            IsCrafted = false,
            LegendaryAffix = "被击中时有一定几率忘却伤痛继续战斗 (8%)。",
            SetName = "",
        };

        /// <summary>
        /// Mordullu's Promise Firebomb generates 100–125 Mana.
        /// </summary>
        public static Item MordullusPromise = new Item
        {
            Id = 271597,
            Name = "摩杜鲁的承诺",
            Quality = ItemQuality.Legendary,
            Slug = "mordullus-promise",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.Axe,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/mordullus-promise",
            Url = "https://us.battle.net/d3/en/item/mordullus-promise",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_axe_1h_102_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/mordullus-promise",
            IsCrafted = false,
            LegendaryAffix = "烈焰炸弹生成 (100-125) 点法力。",
            SetName = "",
        };

        /// <summary>
        /// Fjord Cutter You are surrounded by a Chilling Aura when attacking.
        /// </summary>
        public static Item FjordCutter = new Item
        {
            Id = 192105,
            Name = "劈山巨斧",
            Quality = ItemQuality.Legendary,
            Slug = "fjord-cutter",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.MightyWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "mightyWeapon_1H_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/fjord-cutter",
            Url = "https://us.battle.net/d3/en/item/fjord-cutter",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p3_unique_mighty_1h_006_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fjord-cutter",
            IsCrafted = false,
            LegendaryAffix = "攻击时你将被寒冷光环所围绕。",
            SetName = "",
        };

        /// <summary>
        /// Ambo's Pride 
        /// </summary>
        public static Item AmbosPride = new Item
        {
            Id = 193486,
            Name = "安铂的骄傲",
            Quality = ItemQuality.Legendary,
            Slug = "ambos-pride",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.MightyWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "mightyweapon_1h_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/ambos-pride",
            Url = "https://us.battle.net/d3/en/item/ambos-pride",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mighty_1h_012_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ambos-pride",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Blade of the Warlord Bash consumes up to 40 Fury to deal up to 400–500 % increased damage.
        /// </summary>
        public static Item BladeOfTheWarlord = new Item
        {
            Id = 193611,
            Name = "战神之刃",
            Quality = ItemQuality.Legendary,
            Slug = "blade-of-the-warlord",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.MightyWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "mightyweapon_1h_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/blade-of-the-warlord",
            Url = "https://us.battle.net/d3/en/item/blade-of-the-warlord",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_mighty_1h_005_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blade-of-the-warlord",
            IsCrafted = false,
            LegendaryAffix = "猛击最多可消耗 40 点怒气，造成最高可达 (400-500)% 的伤害。",
            SetName = "",
        };

        /// <summary>
        /// Bul-Kathos's Solemn Vow 
        /// </summary>
        public static Item BulkathossSolemnVow = new Item
        {
            Id = 208771,
            Name = "布尔凯索的庄严之誓",
            Quality = ItemQuality.Legendary,
            Slug = "bulkathoss-solemn-vow",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.MightyWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bulkathoss-solemn-vow",
            Url = "https://us.battle.net/d3/en/item/bulkathoss-solemn-vow",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mighty_1h_010_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bulkathoss-solemn-vow",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "布尔凯索之誓",
        };

        /// <summary>
        /// Bul-Kathos's Warrior Blood 
        /// </summary>
        public static Item BulkathossWarriorBlood = new Item
        {
            Id = 208775,
            Name = "布尔凯索的勇士之血",
            Quality = ItemQuality.Legendary,
            Slug = "bulkathoss-warrior-blood",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.MightyWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/bulkathoss-warrior-blood",
            Url = "https://us.battle.net/d3/en/item/bulkathoss-warrior-blood",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mighty_1h_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bulkathoss-warrior-blood",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "布尔凯索之誓",
        };

        /// <summary>
        /// Dishonored Legacy Cleave deals up to 300–400 % increased damage based on percentage of missing Fury.
        /// </summary>
        public static Item DishonoredLegacy = new Item
        {
            Id = 272008,
            Name = "耻辱之证",
            Quality = ItemQuality.Legendary,
            Slug = "dishonored-legacy",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.MightyWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/dishonored-legacy",
            Url = "https://us.battle.net/d3/en/item/dishonored-legacy",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mighty_1h_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/dishonored-legacy",
            IsCrafted = false,
            LegendaryAffix = "顺劈斩基于损失的怒气百分比最多可造成 (300-400)% 的额外伤害。",
            SetName = "",
        };

        /// <summary>
        /// Oathkeeper Your primary skills attack 50% faster and deal 150–200 % increased damage.
        /// </summary>
        public static Item Oathkeeper = new Item
        {
            Id = 272009,
            Name = "守誓者",
            Quality = ItemQuality.Legendary,
            Slug = "oathkeeper",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.MightyWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/oathkeeper",
            Url = "https://us.battle.net/d3/en/item/oathkeeper",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_mighty_1h_104_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/oathkeeper",
            IsCrafted = false,
            LegendaryAffix = "你的主要技能攻击速度加快 50%，造成的伤害提高 (150-200)%。",
            SetName = "",
        };

        /// <summary>
        /// Remorseless Hammer of the Ancients has a 25–30 % chance to summon an Ancient for 20 seconds.
        /// </summary>
        public static Item Remorseless = new Item
        {
            Id = 271979,
            Name = "无悯",
            Quality = ItemQuality.Legendary,
            Slug = "remorseless",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.MightyWeapon,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/remorseless",
            Url = "https://us.battle.net/d3/en/item/remorseless",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mighty_1h_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/remorseless",
            IsCrafted = false,
            LegendaryAffix = "先祖之锤有 (25-30)% 的几率召唤出一位先祖，持续存在20秒。",
            SetName = "",
        };

        /// <summary>
        /// Arthef's Spark of Life Heal for 3–4 % of your missing Life when you kill an Undead enemy.
        /// </summary>
        public static Item ArthefsSparkOfLife = new Item
        {
            Id = 59633,
            Name = "阿瑟夫的生命之光",
            Quality = ItemQuality.Legendary,
            Slug = "arthefs-spark-of-life",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.TwoHandMace,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedMace_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/arthefs-spark-of-life",
            Url = "https://us.battle.net/d3/en/item/arthefs-spark-of-life",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_2h_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/arthefs-spark-of-life",
            IsCrafted = false,
            LegendaryAffix = "当你消灭一名亡灵敌人时，为你恢复你损失生命值的 (3-4)%。",
            SetName = "",
        };

        /// <summary>
        /// Crushbane 
        /// </summary>
        public static Item Crushbane = new Item
        {
            Id = 99227,
            Name = "破劫",
            Quality = ItemQuality.Legendary,
            Slug = "crushbane",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.TwoHandMace,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedMace_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/crushbane",
            Url = "https://us.battle.net/d3/en/item/crushbane",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_2h_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/crushbane",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Soulsmasher When you kill an enemy, it explodes for 450–600 % of your Life per Kill as damage to all enemies within 20 yards. You no longer benefit from your Life per Kill.
        /// </summary>
        public static Item Soulsmasher = new Item
        {
            Id = 271671,
            Name = "碎魂",
            Quality = ItemQuality.Legendary,
            Slug = "soulsmasher",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.TwoHandMace,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/soulsmasher",
            Url = "https://us.battle.net/d3/en/item/soulsmasher",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_2h_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/soulsmasher",
            IsCrafted = false,
            LegendaryAffix = "每当你消灭一名敌人时，敌人的尸体会爆炸，对 20 码内的所有敌人造成相当于你消灭回复生命 (450-600)% 的伤害。你从每次消灭中不再获得生命值。",
            SetName = "",
        };

        /// <summary>
        /// Skywarden Every 60 seconds, gain a random Law for 60 seconds.
        /// </summary>
        public static Item Skywarden = new Item
        {
            Id = 190840,
            Name = "天界卫士",
            Quality = ItemQuality.Legendary,
            Slug = "skywarden",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.TwoHandMace,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twohandedmace_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/skywarden",
            Url = "https://us.battle.net/d3/en/item/skywarden",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_2h_010_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/skywarden",
            IsCrafted = false,
            LegendaryAffix = "每隔60秒，获得一种随机律法，持续60秒。",
            SetName = "",
        };

        /// <summary>
        /// Wrath of the Bone King 
        /// </summary>
        public static Item WrathOfTheBoneKing = new Item
        {
            Id = 191584,
            Name = "骷髅王之怒",
            Quality = ItemQuality.Legendary,
            Slug = "wrath-of-the-bone-king",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.TwoHandMace,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedMace_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/wrath-of-the-bone-king",
            Url = "https://us.battle.net/d3/en/item/wrath-of-the-bone-king",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_2h_012_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/wrath-of-the-bone-king",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// The Furnace Increases damage against elites by 40–50 % .
        /// </summary>
        public static Item TheFurnace = new Item
        {
            Id = 271666,
            Name = "焚炉",
            Quality = ItemQuality.Legendary,
            Slug = "the-furnace",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.TwoHandMace,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twohandedmace_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-furnace",
            Url = "https://us.battle.net/d3/en/item/the-furnace",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_2h_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-furnace",
            IsCrafted = false,
            LegendaryAffix = "对精英敌人造成的伤害提高 (40-50)%。",
            SetName = "",
        };

        /// <summary>
        /// Schaefer's Hammer Casting a Lightning skill charges you with Lightning, causing you to deal 650–850 % weapon damage as Lightning every second for 5 seconds to nearby enemies.
        /// </summary>
        public static Item SchaefersHammer = new Item
        {
            Id = 197717,
            Name = "舍费尔之锤",
            Quality = ItemQuality.Legendary,
            Slug = "schaefers-hammer",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.TwoHandMace,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedMace_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/schaefers-hammer",
            Url = "https://us.battle.net/d3/en/item/schaefers-hammer",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_2h_009_p2_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/schaefers-hammer",
            IsCrafted = false,
            LegendaryAffix = "施放闪电法术会使自身充满闪电之力，每秒对附近的敌人造成 (650-850)% 的武器伤害（作为闪电伤害），持续5秒。",
            SetName = "",
        };

        /// <summary>
        /// Sledge of Athskeleng 
        /// </summary>
        public static Item SledgeOfAthskeleng = new Item
        {
            Id = 190866,
            Name = "阿斯格伦巨锤",
            Quality = ItemQuality.Legendary,
            Slug = "sledge-of-athskeleng",
            ItemType = ItemType.Mace,
            TrinityItemType = TrinityItemType.TwoHandMace,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedMace_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/sledge-of-athskeleng",
            Url = "https://us.battle.net/d3/en/item/sledge-of-athskeleng",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mace_2h_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sledge-of-athskeleng",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Pledge of Caldeum 
        /// </summary>
        public static Item PledgeOfCaldeum = new Item
        {
            Id = 196570,
            Name = "卡尔蒂姆之誓",
            Quality = ItemQuality.Legendary,
            Slug = "pledge-of-caldeum",
            ItemType = ItemType.Polearm,
            TrinityItemType = TrinityItemType.TwoHandPolearm,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Polearm_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/pledge-of-caldeum",
            Url = "https://us.battle.net/d3/en/item/pledge-of-caldeum",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_polearm_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pledge-of-caldeum",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Standoff Furious Charge deals increased damage equal to 200–250 % of your bonus movement speed.
        /// </summary>
        public static Item Standoff = new Item
        {
            Id = 191570,
            Name = "对峙",
            Quality = ItemQuality.Legendary,
            Slug = "standoff",
            ItemType = ItemType.Polearm,
            TrinityItemType = TrinityItemType.TwoHandPolearm,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Polearm_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/standoff",
            Url = "https://us.battle.net/d3/en/item/standoff",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_polearm_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/standoff",
            IsCrafted = false,
            LegendaryAffix = "狂暴冲撞造成的伤害提高，数值相当于你移动速度加成的 (400-500)%。",
            SetName = "",
        };

        /// <summary>
        /// Bovine Bardiche Chance on hit to summon a herd of murderous cows.
        /// </summary>
        public static Item BovineBardiche = new Item
        {
            Id = 272056,
            Name = "牛王戟斧",
            Quality = ItemQuality.Legendary,
            Slug = "bovine-bardiche",
            ItemType = ItemType.Polearm,
            TrinityItemType = TrinityItemType.TwoHandPolearm,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "polearm_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/bovine-bardiche",
            Url = "https://us.battle.net/d3/en/item/bovine-bardiche",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_polearm_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bovine-bardiche",
            IsCrafted = false,
            LegendaryAffix = "击中时有一定几率召唤出一群凶残的奶牛。",
            SetName = "",
        };

        /// <summary>
        /// Heart Slaughter 
        /// </summary>
        public static Item HeartSlaughter = new Item
        {
            Id = 192569,
            Name = "诛心",
            Quality = ItemQuality.Legendary,
            Slug = "heart-slaughter",
            ItemType = ItemType.Polearm,
            TrinityItemType = TrinityItemType.TwoHandPolearm,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "polearm_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/heart-slaughter",
            Url = "https://us.battle.net/d3/en/item/heart-slaughter",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_polearm_003_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/heart-slaughter",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Vigilance Getting hit has a chance to automatically cast Inner Sanctuary.
        /// </summary>
        public static Item Vigilance = new Item
        {
            Id = 195491,
            Name = "警戒者",
            Quality = ItemQuality.Legendary,
            Slug = "vigilance",
            ItemType = ItemType.Polearm,
            TrinityItemType = TrinityItemType.TwoHandPolearm,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Polearm_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/vigilance",
            Url = "https://us.battle.net/d3/en/item/vigilance",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_polearm_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vigilance",
            IsCrafted = false,
            LegendaryAffix = "被击中时有一定几率自动施放金轮阵 (5%)。",
            SetName = "",
        };

        /// <summary>
        /// Staff of Chiroptera Firebats attacks 100% faster and costs 70–75 % less Mana.
        /// </summary>
        public static Item StaffOfChiroptera = new Item
        {
            Id = 184228,
            Name = "蝙蝠法杖",
            Quality = ItemQuality.Legendary,
            Slug = "staff-of-chiroptera",
            ItemType = ItemType.Staff,
            TrinityItemType = TrinityItemType.TwoHandStaff,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/staff-of-chiroptera",
            Url = "https://us.battle.net/d3/en/item/staff-of-chiroptera",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_staff_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/staff-of-chiroptera",
            IsCrafted = false,
            LegendaryAffix = "火蝠攻击速度加快 100%，消耗的法力值降低75%，伤害提高 125-150%。",
            SetName = "",
        };

        /// <summary>
        /// The Broken Staff 
        /// </summary>
        public static Item TheBrokenStaff = new Item
        {
            Id = 59601,
            Name = "断杖",
            Quality = ItemQuality.Legendary,
            Slug = "the-broken-staff",
            ItemType = ItemType.Staff,
            TrinityItemType = TrinityItemType.TwoHandStaff,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "staff_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-broken-staff",
            Url = "https://us.battle.net/d3/en/item/the-broken-staff",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_staff_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-broken-staff",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Ahavarion, Spear of Lycander Chance on killing a demon to gain a random Shrine effect.
        /// </summary>
        public static Item AhavarionSpearOfLycander = new Item
        {
            Id = 271768,
            Name = "阿瓦里昂,莱姗德之矛",
            Quality = ItemQuality.Legendary,
            Slug = "ahavarion-spear-of-lycander",
            ItemType = ItemType.Staff,
            TrinityItemType = TrinityItemType.TwoHandStaff,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "staff_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/ahavarion-spear-of-lycander",
            Url = "https://us.battle.net/d3/en/item/ahavarion-spear-of-lycander",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_staff_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/ahavarion-spear-of-lycander",
            IsCrafted = false,
            LegendaryAffix = "消灭恶魔有一定几率获得一个随机圣坛效果。",
            SetName = "",
        };

        /// <summary>
        /// SuWong Diviner Acid Cloud gains the effect of the Lob Blob Bomb rune.
        /// </summary>
        public static Item SuwongDiviner = new Item
        {
            Id = 271775,
            Name = "苏旺卜杖",
            Quality = ItemQuality.Legendary,
            Slug = "suwong-diviner",
            ItemType = ItemType.Staff,
            TrinityItemType = TrinityItemType.TwoHandStaff,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/suwong-diviner",
            Url = "https://us.battle.net/d3/en/item/suwong-diviner",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_staff_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/suwong-diviner",
            IsCrafted = false,
            LegendaryAffix = "酸蚀之云获得酸蚀软泥符文的效果。",
            SetName = "",
        };

        /// <summary>
        /// The Smoldering Core Lesser enemies are now lured to your Meteor impact areas.
        /// </summary>
        public static Item TheSmolderingCore = new Item
        {
            Id = 271774,
            Name = "蕴火之心",
            Quality = ItemQuality.Legendary,
            Slug = "the-smoldering-core",
            ItemType = ItemType.Staff,
            TrinityItemType = TrinityItemType.TwoHandStaff,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "staff_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-smoldering-core",
            Url = "https://us.battle.net/d3/en/item/the-smoldering-core",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_staff_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-smoldering-core",
            IsCrafted = false,
            LegendaryAffix = "次级敌人现在将被吸到你的陨石术的轰击区域。",
            SetName = "",
        };

        /// <summary>
        /// Valthek's Rebuke Energy Twister now travels in a straight path.
        /// </summary>
        public static Item ValtheksRebuke = new Item
        {
            Id = 271773,
            Name = "瓦泰克的训斥",
            Quality = ItemQuality.Legendary,
            Slug = "valtheks-rebuke",
            ItemType = ItemType.Staff,
            TrinityItemType = TrinityItemType.TwoHandStaff,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "staff_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/valtheks-rebuke",
            Url = "https://us.battle.net/d3/en/item/valtheks-rebuke",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_staff_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/valtheks-rebuke",
            IsCrafted = false,
            LegendaryAffix = "能量气旋现在沿着直线前进。",
            SetName = "",
        };

        /// <summary>
        /// Maloth's Focus Enemies occasionally flee at the sight of this staff.
        /// </summary>
        public static Item MalothsFocus = new Item
        {
            Id = 193832,
            Name = "马洛斯的聚能法杖",
            Quality = ItemQuality.Legendary,
            Slug = "maloths-focus",
            ItemType = ItemType.Staff,
            TrinityItemType = TrinityItemType.TwoHandStaff,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Staff_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/maloths-focus",
            Url = "https://us.battle.net/d3/en/item/maloths-focus",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_staff_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/maloths-focus",
            IsCrafted = false,
            LegendaryAffix = "敌人偶尔会见此法杖就逃。",
            SetName = "",
        };

        /// <summary>
        /// Wormwood Locust Swarm continuously plagues enemies around you.
        /// </summary>
        public static Item Wormwood = new Item
        {
            Id = 195407,
            Name = "苦艾之杖",
            Quality = ItemQuality.Legendary,
            Slug = "wormwood",
            ItemType = ItemType.Staff,
            TrinityItemType = TrinityItemType.TwoHandStaff,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Staff_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/wormwood",
            Url = "https://us.battle.net/d3/en/item/wormwood",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_staff_003_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/wormwood",
            IsCrafted = false,
            LegendaryAffix = "瘟疫虫群会持续影响你身边的敌人。",
            SetName = "",
        };

        /// <summary>
        /// The Grand Vizier Reduces the Arcane Power cost of Meteor by 40–50 % .
        /// </summary>
        public static Item TheGrandVizier = new Item
        {
            Id = 192167,
            Name = "大维兹尔之杖",
            Quality = ItemQuality.Legendary,
            Slug = "the-grand-vizier",
            ItemType = ItemType.Staff,
            TrinityItemType = TrinityItemType.TwoHandStaff,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Staff_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-grand-vizier",
            Url = "https://us.battle.net/d3/en/item/the-grand-vizier",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_staff_009_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-grand-vizier",
            IsCrafted = false,
            LegendaryAffix = "陨石术的奥能消耗降低 50%，并且伤害提高300-400%。",
            SetName = "",
        };

        /// <summary>
        /// The Tormentor Chance to charm enemies when you hit them.
        /// </summary>
        public static Item TheTormentor = new Item
        {
            Id = 193066,
            Name = "折磨者",
            Quality = ItemQuality.Legendary,
            Slug = "the-tormentor",
            ItemType = ItemType.Staff,
            TrinityItemType = TrinityItemType.TwoHandStaff,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "staff_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-tormentor",
            Url = "https://us.battle.net/d3/en/item/the-tormentor",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_staff_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-tormentor",
            IsCrafted = false,
            LegendaryAffix = "击中敌人时有一定几率魅惑敌人 (3%)。",
            SetName = "",
        };

        /// <summary>
        /// The Zweihander 
        /// </summary>
        public static Item TheZweihander = new Item
        {
            Id = 59665,
            Name = "北地巨神剑",
            Quality = ItemQuality.Legendary,
            Slug = "the-zweihander",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twohandedsword_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-zweihander",
            Url = "https://us.battle.net/d3/en/item/the-zweihander",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-zweihander",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Faithful Memory Each enemy hit by Falling Sword increases the damage of Blessed Hammer by 50–60 % for 10 seconds. Max 10 stacks.
        /// </summary>
        public static Item FaithfulMemory = new Item
        {
            Id = 198960,
            Name = "忠贞回忆",
            Quality = ItemQuality.Legendary,
            Slug = "faithful-memory",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedSword_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/faithful-memory",
            Url = "https://us.battle.net/d3/en/item/faithful-memory",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p43_unique_sword_2h_012_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/faithful-memory",
            IsCrafted = false,
            LegendaryAffix = "被天罚之剑击中的每一名敌人都会使祝福之锤的伤害提高 (60-80)%，持续 10 秒。最多10层。",
            SetName = "",
        };

        /// <summary>
        /// Blackguard 
        /// </summary>
        public static Item Blackguard = new Item
        {
            Id = 270979,
            Name = "黑卫士",
            Quality = ItemQuality.Legendary,
            Slug = "blackguard",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedSword_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/blackguard",
            Url = "https://us.battle.net/d3/en/item/blackguard",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blackguard",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Scourge 20–45 % chance when attacking to explode with demonic fury for 1800-2000% weapon damage as Fire.
        /// </summary>
        public static Item Scourge = new Item
        {
            Id = 181511,
            Name = "灾星",
            Quality = ItemQuality.Legendary,
            Slug = "scourge",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedSword_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/scourge",
            Url = "https://us.battle.net/d3/en/item/scourge",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_004_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/scourge",
            IsCrafted = false,
            LegendaryAffix = "攻击时有 (20-45)% 的几率爆炸产生恶魔之怒，造成1800-2000%的武器伤害（作为火焰伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Stalgard's Decimator Your melee attacks throw a piercing axe at a nearby enemy, dealing 550–700 % weapon damage as Physical.
        /// </summary>
        public static Item StalgardsDecimator = new Item
        {
            Id = 271639,
            Name = "斯塔加德的屠戮者",
            Quality = ItemQuality.Legendary,
            Slug = "stalgards-decimator",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twohandedsword_norm_unique_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/stalgards-decimator",
            Url = "https://us.battle.net/d3/en/item/stalgards-decimator",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/stalgards-decimator",
            IsCrafted = false,
            LegendaryAffix = "你的近战攻击会朝一名附近的敌人掷出一柄穿刺战斧，造成 (550-700)% 的武器伤害（作为物理伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Blade of Prophecy Two Condemned enemies also trigger Condemn's explosion.
        /// </summary>
        public static Item BladeOfProphecy = new Item
        {
            Id = 184184,
            Name = "预言之刃",
            Quality = ItemQuality.Legendary,
            Slug = "blade-of-prophecy",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedSword_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/blade-of-prophecy",
            Url = "https://us.battle.net/d3/en/item/blade-of-prophecy",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blade-of-prophecy",
            IsCrafted = false,
            LegendaryAffix = "两个被天谴的敌人同时会触发天谴爆炸。天谴的伤害提高 600-800%。",
            SetName = "",
        };

        /// <summary>
        /// The Sultan of Blinding Sand 
        /// </summary>
        public static Item TheSultanOfBlindingSand = new Item
        {
            Id = 184190,
            Name = "沙漠之王",
            Quality = ItemQuality.Legendary,
            Slug = "the-sultan-of-blinding-sand",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedSword_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-sultan-of-blinding-sand",
            Url = "https://us.battle.net/d3/en/item/the-sultan-of-blinding-sand",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_008_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-sultan-of-blinding-sand",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Maximus Chance on hit to summon a Demonic Slave.
        /// </summary>
        public static Item Maximus = new Item
        {
            Id = 184187,
            Name = "马西姆斯",
            Quality = ItemQuality.Legendary,
            Slug = "maximus",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedSword_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/maximus",
            Url = "https://us.battle.net/d3/en/item/maximus",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_010_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/maximus",
            IsCrafted = false,
            LegendaryAffix = "击中时有一定几率召唤一名恶魔奴仆。 (30%)",
            SetName = "",
        };

        /// <summary>
        /// The Grandfather 
        /// </summary>
        public static Item TheGrandfather = new Item
        {
            Id = 190360,
            Name = "祖父",
            Quality = ItemQuality.Legendary,
            Slug = "the-grandfather",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twohandedsword_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-grandfather",
            Url = "https://us.battle.net/d3/en/item/the-grandfather",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-grandfather",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Warmonger 
        /// </summary>
        public static Item Warmonger = new Item
        {
            Id = 181495,
            Name = "好战者",
            Quality = ItemQuality.Legendary,
            Slug = "warmonger",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedSword_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/warmonger",
            Url = "https://us.battle.net/d3/en/item/warmonger",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/warmonger",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Cam's Rebuttal Falling Sword can be used again within 4 seconds before the cooldown is triggered.
        /// </summary>
        public static Item CamsRebuttal = new Item
        {
            Id = 271644,
            Name = "坎姆的驳斥",
            Quality = ItemQuality.Legendary,
            Slug = "cams-rebuttal",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twohandedsword_norm_unique_12",
            DataUrl = "https://us.battle.net/api/d3/data/item/cams-rebuttal",
            Url = "https://us.battle.net/d3/en/item/cams-rebuttal",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/cams-rebuttal",
            IsCrafted = false,
            LegendaryAffix = "可在4秒内再次使用天罚之剑，然后再触发冷却时间。",
            SetName = "",
        };

        /// <summary>
        /// Blood Brother Grants a 15–20 % chance to block attacks. Blocked attacks inflict 30% less damage. After blocking an attack, your next attack inflicts 30% additional damage.
        /// </summary>
        public static Item BloodBrother = new Item
        {
            Id = 271645,
            Name = "鲜血兄弟",
            Quality = ItemQuality.Legendary,
            Slug = "blood-brother",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/blood-brother",
            Url = "https://us.battle.net/d3/en/item/blood-brother",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blood-brother",
            IsCrafted = false,
            LegendaryAffix = "获得(15-20)%的几率格挡攻击。被格挡的攻击其伤害降低30%。格挡一次攻击后，你的下一次攻击造成30%的额外伤害。",
            SetName = "",
        };

        /// <summary>
        /// Corrupted Ashbringer Chance on kill to raise a skeleton to fight for you. Upon accumulating 5 skeletons, they each explode for 1000% weapon damage and the sword transforms into Ashbringer for a short time. Attacking with Ashbringer burns your enemy for 5000–6000 % weapon damage as Holy.
        /// </summary>
        public static Item CorruptedAshbringer = new Item
        {
            Id = 430567,
            Name = "堕落的灰烬使者",
            Quality = ItemQuality.Legendary,
            Slug = "corrupted-ashbringer",
            ItemType = ItemType.Sword,
            TrinityItemType = TrinityItemType.TwoHandSword,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/corrupted-ashbringer",
            Url = "https://us.battle.net/d3/en/item/corrupted-ashbringer",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_sword_2h_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/corrupted-ashbringer",
            IsCrafted = false,
            LegendaryAffix = "消灭敌人后有一定几率复活一具骷髅为你作战。当累积5具骷髅时，它们每一只爆炸造成1000%武器伤害，并且剑会变形成灰烬使者，持续一小段时间。使用灰烬使者攻击会使目标燃烧，对其造成(5000-6000)%的武器伤害（作为神圣伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Balance When your Tempest Rush hits 3 or fewer enemies, it gains 100% Critical Hit Chance.
        /// </summary>
        public static Item Balance = new Item
        {
            Id = 195145,
            Name = "平衡",
            Quality = ItemQuality.Legendary,
            Slug = "balance",
            ItemType = ItemType.Daibo,
            TrinityItemType = TrinityItemType.TwoHandDaibo,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "combatstaff_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/balance",
            Url = "https://us.battle.net/d3/en/item/balance",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_combatstaff_2h_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/balance",
            IsCrafted = false,
            LegendaryAffix = "当你的风雷冲击中 3 名或更少的敌人时，获得 100% 的暴击几率。风雷冲的伤害提高 450-600%。",
            SetName = "",
        };

        /// <summary>
        /// The Flow of Eternity Increase the damage of Seven-Sided Strike by 100% and reduce the cooldown of Seven-Sided Strike by 45–60 % .
        /// </summary>
        public static Item TheFlowOfEternity = new Item
        {
            Id = 197072,
            Name = "永恒之悟",
            Quality = ItemQuality.Legendary,
            Slug = "the-flow-of-eternity",
            ItemType = ItemType.Daibo,
            TrinityItemType = TrinityItemType.TwoHandDaibo,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "combatStaff_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-flow-of-eternity",
            Url = "https://us.battle.net/d3/en/item/the-flow-of-eternity",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_combatstaff_2h_005_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-flow-of-eternity",
            IsCrafted = false,
            LegendaryAffix = "使七相拳的冷却时间缩短 (45-60)%。",
            SetName = "",
        };

        /// <summary>
        /// The Paddle Slap!
        /// </summary>
        public static Item ThePaddle = new Item
        {
            Id = 197068,
            Name = "战桨",
            Quality = ItemQuality.Legendary,
            Slug = "the-paddle",
            ItemType = ItemType.Daibo,
            TrinityItemType = TrinityItemType.TwoHandDaibo,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "combatStaff_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-paddle",
            Url = "https://us.battle.net/d3/en/item/the-paddle",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_combatstaff_2h_007_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-paddle",
            IsCrafted = false,
            LegendaryAffix = "啪！",
            SetName = "",
        };

        /// <summary>
        /// Staff of Kyro 
        /// </summary>
        public static Item StaffOfKyro = new Item
        {
            Id = 271749,
            Name = "凯洛之杖",
            Quality = ItemQuality.Legendary,
            Slug = "staff-of-kyro",
            ItemType = ItemType.Daibo,
            TrinityItemType = TrinityItemType.TwoHandDaibo,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "combatstaff_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/staff-of-kyro",
            Url = "https://us.battle.net/d3/en/item/staff-of-kyro",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_combatstaff_2h_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/staff-of-kyro",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Warstaff of General Quang Tempest Rush gains the effect of the Tailwind rune.
        /// </summary>
        public static Item WarstaffOfGeneralQuang = new Item
        {
            Id = 271765,
            Name = "广将军的战杖",
            Quality = ItemQuality.Legendary,
            Slug = "warstaff-of-general-quang",
            ItemType = ItemType.Daibo,
            TrinityItemType = TrinityItemType.TwoHandDaibo,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "combatstaff_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/warstaff-of-general-quang",
            Url = "https://us.battle.net/d3/en/item/warstaff-of-general-quang",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_combatstaff_2h_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/warstaff-of-general-quang",
            IsCrafted = false,
            LegendaryAffix = "风雷冲获得顺风而行符文的效果。",
            SetName = "",
        };

        /// <summary>
        /// Incense Torch of the Grand Temple Reduces the Spirit cost of Wave of Light by 40–50 % .
        /// </summary>
        public static Item IncenseTorchOfTheGrandTemple = new Item
        {
            Id = 192342,
            Name = "大殿香烛",
            Quality = ItemQuality.Legendary,
            Slug = "incense-torch-of-the-grand-temple",
            ItemType = ItemType.Daibo,
            TrinityItemType = TrinityItemType.TwoHandDaibo,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "combatStaff_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/incense-torch-of-the-grand-temple",
            Url = "https://us.battle.net/d3/en/item/incense-torch-of-the-grand-temple",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_combatstaff_2h_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/incense-torch-of-the-grand-temple",
            IsCrafted = false,
            LegendaryAffix = "使金钟破的内力消耗降低 50%，伤害提高 450-550%。",
            SetName = "",
        };

        /// <summary>
        /// Flying Dragon Chance to double your attack speed when attacking.
        /// </summary>
        public static Item FlyingDragon = new Item
        {
            Id = 197065,
            Name = "翔龙",
            Quality = ItemQuality.Legendary,
            Slug = "flying-dragon",
            ItemType = ItemType.Daibo,
            TrinityItemType = TrinityItemType.TwoHandDaibo,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "combatStaff_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/flying-dragon",
            Url = "https://us.battle.net/d3/en/item/flying-dragon",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_combatstaff_2h_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/flying-dragon",
            IsCrafted = false,
            LegendaryAffix = "攻击时有一定几率使你的攻击速度加倍提高 (5%)。",
            SetName = "",
        };

        /// <summary>
        /// Inna's Reach 
        /// </summary>
        public static Item InnasReach = new Item
        {
            Id = 212208,
            Name = "尹娜的审判",
            Quality = ItemQuality.Legendary,
            Slug = "innas-reach",
            ItemType = ItemType.Daibo,
            TrinityItemType = TrinityItemType.TwoHandDaibo,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "combatStaff_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/innas-reach",
            Url = "https://us.battle.net/d3/en/item/innas-reach",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_combatstaff_2h_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/innas-reach",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "尹娜的真言",
        };

        /// <summary>
        /// Flail of the Ascended Your Shield Glare deals damage equal to up to your last 5 Shield Bash casts.
        /// </summary>
        public static Item FlailOfTheAscended = new Item
        {
            Id = 403860,
            Name = "晋升者的连枷",
            Quality = ItemQuality.Legendary,
            Slug = "flail-of-the-ascended",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.TwoHandFlail,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/flail-of-the-ascended",
            Url = "https://us.battle.net/d3/en/item/flail-of-the-ascended",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_flail_2h_002_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/flail-of-the-ascended",
            IsCrafted = false,
            LegendaryAffix = "你的金光冲盾造成的伤害，最多相当于你前 5 次施放的金光冲盾所造成的总伤害。",
            SetName = "",
        };

        /// <summary>
        /// Akkhan's Addendum Akarat's Champion gains the effects of the Prophet and Embodiment of Power runes.
        /// </summary>
        public static Item AkkhansAddendum = new Item
        {
            Id = 395228,
            Name = "阿克汉的补件",
            Quality = ItemQuality.Legendary,
            Slug = "akkhans-addendum",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.TwoHandFlail,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/akkhans-addendum",
            Url = "https://us.battle.net/d3/en/item/akkhans-addendum",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_flail_2h_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/akkhans-addendum",
            IsCrafted = false,
            LegendaryAffix = "阿卡拉特勇士获得先知化身以及圣力代言符文效果。",
            SetName = "",
        };

        /// <summary>
        /// Baleful Remnant Enemies killed while Akarat's Champion is active turn into Phalanx Avatars for 10 seconds.
        /// </summary>
        public static Item BalefulRemnant = new Item
        {
            Id = 299435,
            Name = "凶星破片",
            Quality = ItemQuality.Legendary,
            Slug = "baleful-remnant",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.TwoHandFlail,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "flail2h_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/baleful-remnant",
            Url = "https://us.battle.net/d3/en/item/baleful-remnant",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_flail_2h_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/baleful-remnant",
            IsCrafted = false,
            LegendaryAffix = "在阿卡拉特勇士持续期间被消灭的敌人会被转化为斗阵化身，持续10秒。",
            SetName = "",
        };

        /// <summary>
        /// Fate of the Fell Gain two additional rays of Heaven’s Fury.
        /// </summary>
        public static Item FateOfTheFell = new Item
        {
            Id = 299436,
            Name = "妖邪必败",
            Quality = ItemQuality.Legendary,
            Slug = "fate-of-the-fell",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.TwoHandFlail,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "flail2h_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/fate-of-the-fell",
            Url = "https://us.battle.net/d3/en/item/fate-of-the-fell",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_flail_2h_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fate-of-the-fell",
            IsCrafted = false,
            LegendaryAffix = "天堂之怒获得额外2道光束，并且伤害提高（375-500）%。",
            SetName = "",
        };

        /// <summary>
        /// Golden Flense Sweep Attack restores 4–6 Wrath for each enemy hit.
        /// </summary>
        public static Item GoldenFlense = new Item
        {
            Id = 299437,
            Name = "刨肉金枷",
            Quality = ItemQuality.Legendary,
            Slug = "golden-flense",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.TwoHandFlail,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "flail2h_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/golden-flense",
            Url = "https://us.battle.net/d3/en/item/golden-flense",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_flail_2h_104_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/golden-flense",
            IsCrafted = false,
            LegendaryAffix = "横扫每击中一名敌人会恢复6 点愤怒值。横扫的伤害提高 225-300%。",
            SetName = "",
        };

        /// <summary>
        /// The Mortal Drama Double the number of Bombardment impacts.
        /// </summary>
        public static Item TheMortalDrama = new Item
        {
            Id = 299431,
            Name = "人世无常",
            Quality = ItemQuality.Legendary,
            Slug = "the-mortal-drama",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.TwoHandFlail,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "flail2h_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-mortal-drama",
            Url = "https://us.battle.net/d3/en/item/the-mortal-drama",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_flail_2h_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-mortal-drama",
            IsCrafted = false,
            LegendaryAffix = "轰击的数量翻倍。",
            SetName = "",
        };

        /// <summary>
        /// Akkhan's Leniency Each enemy hit by your Blessed Shield increases the damage of your Blessed Shield by 15–20 % for 3 seconds.
        /// </summary>
        public static Item AkkhansLeniency = new Item
        {
            Id = 403846,
            Name = "阿克汉的仁慈",
            Quality = ItemQuality.Legendary,
            Slug = "akkhans-leniency",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.TwoHandFlail,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/akkhans-leniency",
            Url = "https://us.battle.net/d3/en/item/akkhans-leniency",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_flail2h_norm_unique_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/akkhans-leniency",
            IsCrafted = false,
            LegendaryAffix = "你的祝福之盾击中每一名敌人都会使你祝福之盾的伤害提高 (15-20)%，持续 3 秒。",
            SetName = "",
        };

        /// <summary>
        /// Flail of the Charge 
        /// </summary>
        public static Item FlailOfTheCharge = new Item
        {
            Id = 395227,
            Name = "冲锋连枷",
            Quality = ItemQuality.Legendary,
            Slug = "flail-of-the-charge",
            ItemType = ItemType.Flail,
            TrinityItemType = TrinityItemType.TwoHandFlail,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/flail-of-the-charge",
            Url = "https://us.battle.net/d3/en/item/flail-of-the-charge",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_flail_2h_set_01_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/flail-of-the-charge",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "诺瓦德的热忱",
        };

        /// <summary>
        /// Bastion's Revered Frenzy now stacks up to 10 times.
        /// </summary>
        public static Item BastionsRevered = new Item
        {
            Id = 195690,
            Name = "巴斯廷之力",
            Quality = ItemQuality.Legendary,
            Slug = "bastions-revered",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.TwoHandMighty,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "mightyWeapon_2H_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/bastions-revered",
            Url = "https://us.battle.net/d3/en/item/bastions-revered",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mighty_2h_004_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bastions-revered",
            IsCrafted = false,
            LegendaryAffix = "狂乱现在最多可叠加至 10 次。",
            SetName = "",
        };

        /// <summary>
        /// Fury of the Vanished Peak Reduces the Fury cost of Seismic Slam by 40–50 % .
        /// </summary>
        public static Item FuryOfTheVanishedPeak = new Item
        {
            Id = 195138,
            Name = "隐峰之怒",
            Quality = ItemQuality.Legendary,
            Slug = "fury-of-the-vanished-peak",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.TwoHandMighty,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/fury-of-the-vanished-peak",
            Url = "https://us.battle.net/d3/en/item/fury-of-the-vanished-peak",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_mighty_2h_006_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fury-of-the-vanished-peak",
            IsCrafted = false,
            LegendaryAffix = "使裂地斩的怒气消耗降低50%，并且造成的伤害提高 400-500%。",
            SetName = "",
        };

        /// <summary>
        /// Madawc's Sorrow Stun enemies for 2 seconds the first time you hit them.
        /// </summary>
        public static Item MadawcsSorrow = new Item
        {
            Id = 272012,
            Name = "马道克的悲伤",
            Quality = ItemQuality.Legendary,
            Slug = "madawcs-sorrow",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.TwoHandMighty,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "mightyweapon_2h_norm_unique_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/madawcs-sorrow",
            Url = "https://us.battle.net/d3/en/item/madawcs-sorrow",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mighty_2h_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/madawcs-sorrow",
            IsCrafted = false,
            LegendaryAffix = "当你首次击中敌人时，使其昏迷 2 秒。",
            SetName = "",
        };

        /// <summary>
        /// The Gavel of Judgment Hammer of the Ancients returns 20–25 Fury if it hits 3 or fewer enemies.
        /// </summary>
        public static Item TheGavelOfJudgment = new Item
        {
            Id = 193657,
            Name = "审判之锤",
            Quality = ItemQuality.Legendary,
            Slug = "the-gavel-of-judgment",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.TwoHandMighty,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "mightyWeapon_2H_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-gavel-of-judgment",
            Url = "https://us.battle.net/d3/en/item/the-gavel-of-judgment",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_mighty_2h_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-gavel-of-judgment",
            IsCrafted = false,
            LegendaryAffix = "先祖之锤若击中3个或更少的敌人，返还 25 点怒气。造成的伤害提高600-800%。",
            SetName = "",
        };

        /// <summary>
        /// Immortal King's Boulder Breaker 
        /// </summary>
        public static Item ImmortalKingsBoulderBreaker = new Item
        {
            Id = 210678,
            Name = "不朽之王的碎石锤",
            Quality = ItemQuality.Legendary,
            Slug = "immortal-kings-boulder-breaker",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.TwoHandMighty,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "mightyWeapon_2H_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/immortal-kings-boulder-breaker",
            Url = "https://us.battle.net/d3/en/item/immortal-kings-boulder-breaker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_mighty_2h_010_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/immortal-kings-boulder-breaker",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "不朽之王的呼唤",
        };

        /// <summary>
        /// Blade of the Tribes War Cry and Threatening Shout cause an Avalanche and Earthquake.
        /// </summary>
        public static Item BladeOfTheTribes = new Item
        {
            Id = 322776,
            Name = "部族之刃",
            Quality = ItemQuality.Legendary,
            Slug = "blade-of-the-tribes",
            ItemType = ItemType.MightyWeapon,
            TrinityItemType = TrinityItemType.TwoHandMighty,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/blade-of-the-tribes",
            Url = "https://us.battle.net/d3/en/item/blade-of-the-tribes",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_mighty_2h_101_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blade-of-the-tribes",
            IsCrafted = false,
            LegendaryAffix = "战吼和威吓呐喊会造成山崩地裂与地震。",
            SetName = "",
        };

        /// <summary>
        /// Demon Machine 35–65 % chance to shoot explosive bolts when attacking.
        /// </summary>
        public static Item DemonMachine = new Item
        {
            Id = 222286,
            Name = "恶魔机弩",
            Quality = ItemQuality.Legendary,
            Slug = "demon-machine",
            ItemType = ItemType.Crossbow,
            TrinityItemType = TrinityItemType.TwoHandCrossbow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "xbow_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/demon-machine",
            Url = "https://us.battle.net/d3/en/item/demon-machine",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_xbow_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/demon-machine",
            IsCrafted = false,
            LegendaryAffix = "攻击时有 (35-65)% 的几率射出若干支爆炸箭。",
            SetName = "",
        };

        /// <summary>
        /// Buriza-Do Kyanon Your projectiles pierce 1–2 additional times.
        /// </summary>
        public static Item BurizadoKyanon = new Item
        {
            Id = 194219,
            Name = "暴雪重炮",
            Quality = ItemQuality.Legendary,
            Slug = "burizado-kyanon",
            ItemType = ItemType.Crossbow,
            TrinityItemType = TrinityItemType.TwoHandCrossbow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/burizado-kyanon",
            Url = "https://us.battle.net/d3/en/item/burizado-kyanon",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_xbow_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/burizado-kyanon",
            IsCrafted = false,
            LegendaryAffix = "你的远程攻击额外穿透 (1-2) 次。",
            SetName = "",
        };

        /// <summary>
        /// Bakkan Caster 
        /// </summary>
        public static Item BakkanCaster = new Item
        {
            Id = 98163,
            Name = "巴坎弩枪",
            Quality = ItemQuality.Legendary,
            Slug = "bakkan-caster",
            ItemType = ItemType.Crossbow,
            TrinityItemType = TrinityItemType.TwoHandCrossbow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "xbow_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/bakkan-caster",
            Url = "https://us.battle.net/d3/en/item/bakkan-caster",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_xbow_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/bakkan-caster",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Pus Spitter 25–50 % chance to lob an acid blob when attacking.
        /// </summary>
        public static Item PusSpitter = new Item
        {
            Id = 204874,
            Name = "吐脓毒弩",
            Quality = ItemQuality.Legendary,
            Slug = "pus-spitter",
            ItemType = ItemType.Crossbow,
            TrinityItemType = TrinityItemType.TwoHandCrossbow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "XBow_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/pus-spitter",
            Url = "https://us.battle.net/d3/en/item/pus-spitter",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_xbow_012_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/pus-spitter",
            IsCrafted = false,
            LegendaryAffix = "攻击时有 (25-50)% 的几率抛出一颗酸液弹。",
            SetName = "",
        };

        /// <summary>
        /// Hellrack Chance to root enemies to the ground when you hit them.
        /// </summary>
        public static Item Hellrack = new Item
        {
            Id = 192836,
            Name = "地狱刑器",
            Quality = ItemQuality.Legendary,
            Slug = "hellrack",
            ItemType = ItemType.Crossbow,
            TrinityItemType = TrinityItemType.TwoHandCrossbow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "XBow_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/hellrack",
            Url = "https://us.battle.net/d3/en/item/hellrack",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_xbow_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/hellrack",
            IsCrafted = false,
            LegendaryAffix = "击中敌人时有一定几率使敌人定身 (20%)。",
            SetName = "",
        };

        /// <summary>
        /// Manticore Reduces the Hatred cost of Cluster Arrow by 40–50 % .
        /// </summary>
        public static Item Manticore = new Item
        {
            Id = 221760,
            Name = "蝎尾狮",
            Quality = ItemQuality.Legendary,
            Slug = "manticore",
            ItemType = ItemType.Crossbow,
            TrinityItemType = TrinityItemType.TwoHandCrossbow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "XBow_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/manticore",
            Url = "https://us.battle.net/d3/en/item/manticore",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_xbow_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/manticore",
            IsCrafted = false,
            LegendaryAffix = "使集束箭的憎恨值消耗降低 50%，并且伤害提高 250-300%。",
            SetName = "",
        };

        /// <summary>
        /// Chanon Bolter Your Spike Traps lure enemies to them. Enemies may be taunted once every 12–16 seconds.
        /// </summary>
        public static Item ChanonBolter = new Item
        {
            Id = 271884,
            Name = "重炮机弩",
            Quality = ItemQuality.Legendary,
            Slug = "chanon-bolter",
            ItemType = ItemType.Crossbow,
            TrinityItemType = TrinityItemType.TwoHandCrossbow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "xbow_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/chanon-bolter",
            Url = "https://us.battle.net/d3/en/item/chanon-bolter",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_xbow_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/chanon-bolter",
            IsCrafted = false,
            LegendaryAffix = "你的尖刺陷阱将把敌人吸引至其跟前。敌人每 (12-16) 秒可能会被嘲讽一次。",
            SetName = "",
        };

        /// <summary>
        /// Wojahnni Assaulter Rapid Fire deals 60–75 % increased damage for every half second that you channel. Stacks up to 4 times.
        /// </summary>
        public static Item WojahnniAssaulter = new Item
        {
            Id = 271889,
            Name = "沃杨强击弩",
            Quality = ItemQuality.Legendary,
            Slug = "wojahnni-assaulter",
            ItemType = ItemType.Crossbow,
            TrinityItemType = TrinityItemType.TwoHandCrossbow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "xbow_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/wojahnni-assaulter",
            Url = "https://us.battle.net/d3/en/item/wojahnni-assaulter",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p41_unique_xbow_102_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/wojahnni-assaulter",
            IsCrafted = false,
            LegendaryAffix = "你每引导半秒的时间，急速射击造成的伤害便提高 (60-75)%。最多叠加 4 次。",
            SetName = "",
        };

        /// <summary>
        /// Uskang 
        /// </summary>
        public static Item Uskang = new Item
        {
            Id = 175580,
            Name = "乌斯刚",
            Quality = ItemQuality.Legendary,
            Slug = "uskang",
            ItemType = ItemType.Bow,
            TrinityItemType = TrinityItemType.TwoHandBow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "bow_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/uskang",
            Url = "https://us.battle.net/d3/en/item/uskang",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bow_005_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/uskang",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Etrayu 
        /// </summary>
        public static Item Etrayu = new Item
        {
            Id = 175581,
            Name = "伊崔羽",
            Quality = ItemQuality.Legendary,
            Slug = "etrayu",
            ItemType = ItemType.Bow,
            TrinityItemType = TrinityItemType.TwoHandBow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Bow_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/etrayu",
            Url = "https://us.battle.net/d3/en/item/etrayu",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bow_001_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/etrayu",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// The Raven's Wing A raven flies to your side.
        /// </summary>
        public static Item TheRavensWing = new Item
        {
            Id = 221938,
            Name = "渡鸦之翼",
            Quality = ItemQuality.Legendary,
            Slug = "the-ravens-wing",
            ItemType = ItemType.Bow,
            TrinityItemType = TrinityItemType.TwoHandBow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Bow_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-ravens-wing",
            Url = "https://us.battle.net/d3/en/item/the-ravens-wing",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bow_008_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-ravens-wing",
            IsCrafted = false,
            LegendaryAffix = "一只渡鸦飞到你的身边。",
            SetName = "",
        };

        /// <summary>
        /// Kridershot Elemental Arrow now generates 3–4 Hatred.
        /// </summary>
        public static Item Kridershot = new Item
        {
            Id = 271875,
            Name = "莱德之击",
            Quality = ItemQuality.Legendary,
            Slug = "kridershot",
            ItemType = ItemType.Bow,
            TrinityItemType = TrinityItemType.TwoHandBow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "bow_norm_unique_09",
            DataUrl = "https://us.battle.net/api/d3/data/item/kridershot",
            Url = "https://us.battle.net/d3/en/item/kridershot",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bow_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/kridershot",
            IsCrafted = false,
            LegendaryAffix = "元素箭现在可生成 (3-4) 点憎恨值。",
            SetName = "",
        };

        /// <summary>
        /// Cluckeye 25–50 % chance to cluck when attacking.
        /// </summary>
        public static Item Cluckeye = new Item
        {
            Id = 175582,
            Name = "啄目弓",
            Quality = ItemQuality.Legendary,
            Slug = "cluckeye",
            ItemType = ItemType.Bow,
            TrinityItemType = TrinityItemType.TwoHandBow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Bow_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/cluckeye",
            Url = "https://us.battle.net/d3/en/item/cluckeye",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bow_015_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/cluckeye",
            IsCrafted = false,
            LegendaryAffix = "攻击时有 (25-50)% 的几率射出咯咯叫的飞禽。",
            SetName = "",
        };

        /// <summary>
        /// Windforce 
        /// </summary>
        public static Item Windforce = new Item
        {
            Id = 192602,
            Name = "风之力",
            Quality = ItemQuality.Legendary,
            Slug = "windforce",
            ItemType = ItemType.Bow,
            TrinityItemType = TrinityItemType.TwoHandBow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "bow_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/windforce",
            Url = "https://us.battle.net/d3/en/item/windforce",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bow_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/windforce",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Leonine Bow of Hashir Bolas have a 15–20 % chance on explosion to pull in all enemies within 24 yards.
        /// </summary>
        public static Item LeonineBowOfHashir = new Item
        {
            Id = 271882,
            Name = "哈希尔的狮弓",
            Quality = ItemQuality.Legendary,
            Slug = "leonine-bow-of-hashir",
            ItemType = ItemType.Bow,
            TrinityItemType = TrinityItemType.TwoHandBow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "bow_norm_unique_11",
            DataUrl = "https://us.battle.net/api/d3/data/item/leonine-bow-of-hashir",
            Url = "https://us.battle.net/d3/en/item/leonine-bow-of-hashir",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bow_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/leonine-bow-of-hashir",
            IsCrafted = false,
            LegendaryAffix = "流星索在爆炸时有 (15-20)% 的几率将 24 码内的所有敌人拉过来。",
            SetName = "",
        };

        /// <summary>
        /// Odyssey's End Enemies snared by your Entangling Shot take 20–25 % increased damage from all sources.
        /// </summary>
        public static Item OdysseysEnd = new Item
        {
            Id = 271880,
            Name = "旅途终点",
            Quality = ItemQuality.Legendary,
            Slug = "odysseys-end",
            ItemType = ItemType.Bow,
            TrinityItemType = TrinityItemType.TwoHandBow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/odysseys-end",
            Url = "https://us.battle.net/d3/en/item/odysseys-end",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bow_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/odysseys-end",
            IsCrafted = false,
            LegendaryAffix = "被你的缠绕射击限制移动的敌人，受到的所有伤害提高 (20-25)%。",
            SetName = "",
        };

        /// <summary>
        /// Yang's Recurve Multishot attacks 50 % faster.
        /// </summary>
        public static Item YangsRecurve = new Item
        {
            Id = 319407,
            Name = "杨的反曲弓",
            Quality = ItemQuality.Legendary,
            Slug = "yangs-recurve",
            ItemType = ItemType.Bow,
            TrinityItemType = TrinityItemType.TwoHandBow,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/yangs-recurve",
            Url = "https://us.battle.net/d3/en/item/yangs-recurve",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_bow_104_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/yangs-recurve",
            IsCrafted = false,
            LegendaryAffix = "多重射击的攻击速度提高 50% ，并且伤害提高 150-200%。",
            SetName = "",
        };

        /// <summary>
        /// Helltrapper 7–10 % chance on hit to summon a Spike Trap, Caltrops or Sentry.
        /// </summary>
        public static Item Helltrapper = new Item
        {
            Id = 271914,
            Name = "地狱猎手",
            Quality = ItemQuality.Legendary,
            Slug = "helltrapper",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/helltrapper",
            Url = "https://us.battle.net/d3/en/item/helltrapper",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_handxbow_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/helltrapper",
            IsCrafted = false,
            LegendaryAffix = "击中有 (7-10)% 的几率召唤出尖刺陷阱、铁蒺藜或箭塔。",
            SetName = "",
        };

        /// <summary>
        /// Valla's Bequest Strafe projectiles pierce.
        /// </summary>
        public static Item VallasBequest = new Item
        {
            Id = 192467,
            Name = "维拉的遗赠",
            Quality = ItemQuality.Legendary,
            Slug = "vallas-bequest",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/vallas-bequest",
            Url = "https://us.battle.net/d3/en/item/vallas-bequest",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p43_unique_handxbow_005_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/vallas-bequest",
            IsCrafted = false,
            LegendaryAffix = "扫射的弹药可以穿透敌人。",
            SetName = "",
        };

        /// <summary>
        /// Balefire Caster 
        /// </summary>
        public static Item BalefireCaster = new Item
        {
            Id = 192528,
            Name = "焚火投手",
            Quality = ItemQuality.Legendary,
            Slug = "balefire-caster",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "handxbow_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/balefire-caster",
            Url = "https://us.battle.net/d3/en/item/balefire-caster",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_handxbow_004_p1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/balefire-caster",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// K'mar Tenclip Strafe gains the effect of the Drifting Shadow rune.
        /// </summary>
        public static Item KmarTenclip = new Item
        {
            Id = 271892,
            Name = "凯马十击",
            Quality = ItemQuality.Legendary,
            Slug = "kmar-tenclip",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "handxbow_norm_unique_10",
            DataUrl = "https://us.battle.net/api/d3/data/item/kmar-tenclip",
            Url = "https://us.battle.net/d3/en/item/kmar-tenclip",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_handxbow_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/kmar-tenclip",
            IsCrafted = false,
            LegendaryAffix = "扫射获得暗影游移符文的效果。",
            SetName = "",
        };

        /// <summary>
        /// Calamity Automatically cast Marked for Death when you damage an enemy.
        /// </summary>
        public static Item Calamity = new Item
        {
            Id = 225181,
            Name = "灾劫",
            Quality = ItemQuality.Legendary,
            Slug = "calamity",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "handXbow_norm_unique_08",
            DataUrl = "https://us.battle.net/api/d3/data/item/calamity",
            Url = "https://us.battle.net/d3/en/item/calamity",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_handxbow_012_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/calamity",
            IsCrafted = false,
            LegendaryAffix = "受到你伤害的敌人会被自动打上死亡印记。",
            SetName = "",
        };

        /// <summary>
        /// Danetta's Revenge Vault gains the effect of the Rattling Roll rune.
        /// </summary>
        public static Item DanettasRevenge = new Item
        {
            Id = 211749,
            Name = "达内塔之仇",
            Quality = ItemQuality.Legendary,
            Slug = "danettas-revenge",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "handXbow_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/danettas-revenge",
            Url = "https://us.battle.net/d3/en/item/danettas-revenge",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_handxbow_002_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/danettas-revenge",
            IsCrafted = false,
            LegendaryAffix = "影轮翻获得霹雳翻滚符文的效果。",
            SetName = "达内塔之憎",
        };

        /// <summary>
        /// Danetta's Spite Leave a clone of yourself behind after using Vault.
        /// </summary>
        public static Item DanettasSpite = new Item
        {
            Id = 211745,
            Name = "达内塔之恨",
            Quality = ItemQuality.Legendary,
            Slug = "danettas-spite",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "handXbow_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/danettas-spite",
            Url = "https://us.battle.net/d3/en/item/danettas-spite",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_handxbow_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/danettas-spite",
            IsCrafted = false,
            LegendaryAffix = "在使用影轮翻之后，留下一个你自身的克隆体。",
            SetName = "达内塔之憎",
        };

        /// <summary>
        /// Natalya's Slayer 
        /// </summary>
        public static Item NatalyasSlayer = new Item
        {
            Id = 210874,
            Name = "娜塔亚的杀戮",
            Quality = ItemQuality.Legendary,
            Slug = "natalyas-slayer",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "handXbow_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/natalyas-slayer",
            Url = "https://us.battle.net/d3/en/item/natalyas-slayer",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_handxbow_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/natalyas-slayer",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "娜塔亚的复仇",
        };

        /// <summary>
        /// Dawn Reduce the cooldown of Vengeance by 50–65 % .
        /// </summary>
        public static Item Dawn = new Item
        {
            Id = 196409,
            Name = "黎明",
            Quality = ItemQuality.Legendary,
            Slug = "dawn",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "handXbow_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/dawn",
            Url = "https://us.battle.net/d3/en/item/dawn",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_handxbow_001_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/dawn",
            IsCrafted = false,
            LegendaryAffix = "使复仇的冷却时间减少 (50-65)%。",
            SetName = "",
        };

        /// <summary>
        /// Fortress Ballista Attacks grant you an absorb shield for 2.0–3.0 % of your maximum Life. Stacks up to 10 times.
        /// </summary>
        public static Item FortressBallista = new Item
        {
            Id = 395304,
            Name = "要塞弩机",
            Quality = ItemQuality.Legendary,
            Slug = "fortress-ballista",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/fortress-ballista",
            Url = "https://us.battle.net/d3/en/item/fortress-ballista",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_handxbow_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fortress-ballista",
            IsCrafted = false,
            LegendaryAffix = "攻击可使你获得一道相当于你生命值上限 (2.0-3.0)% 的吸收护盾。最多可叠加 10 次。",
            SetName = "",
        };

        /// <summary>
        /// Lianna's Wings Shadow Power also triggers Smoke Screen.
        /// </summary>
        public static Item LiannasWings = new Item
        {
            Id = 395303,
            Name = "莲娜的飞羽",
            Quality = ItemQuality.Legendary,
            Slug = "liannas-wings",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/liannas-wings",
            Url = "https://us.battle.net/d3/en/item/liannas-wings",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_handxbow_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/liannas-wings",
            IsCrafted = false,
            LegendaryAffix = "暗影之力还会触发烟雾弹。",
            SetName = "",
        };

        /// <summary>
        /// The Demon's Demise The blast from Spike Trap will damage all enemies again after 1 second.
        /// </summary>
        public static Item TheDemonsDemise = new Item
        {
            Id = 395305,
            Name = "恶魔之灾",
            Quality = ItemQuality.Legendary,
            Slug = "the-demons-demise",
            ItemType = ItemType.HandCrossbow,
            TrinityItemType = TrinityItemType.HandCrossbow,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-demons-demise",
            Url = "https://us.battle.net/d3/en/item/the-demons-demise",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p42_handxbow_norm_unique_03_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-demons-demise",
            IsCrafted = false,
            LegendaryAffix = "尖刺陷阱产生的爆炸会在 1 秒后再次对所有敌人造成伤害。",
            SetName = "",
        };

        /// <summary>
        /// Starfire Lightning damage is increased by 10–15 % for every 10 yards you are from the target up to a maximum of 40 yards.
        /// </summary>
        public static Item Starfire = new Item
        {
            Id = 182074,
            Name = "星火",
            Quality = ItemQuality.Legendary,
            Slug = "starfire",
            ItemType = ItemType.Wand,
            TrinityItemType = TrinityItemType.Wand,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Wand_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/starfire",
            Url = "https://us.battle.net/d3/en/item/starfire",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p42_unique_wand_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/starfire",
            IsCrafted = false,
            LegendaryAffix = "距离目标每 10 码就会使闪电伤害提高 (10-15)%，最多可提高相当于 40 码的伤害。",
            SetName = "",
        };

        /// <summary>
        /// Unstable Scepter Arcane Orb's explosion triggers an additional time.
        /// </summary>
        public static Item UnstableScepter = new Item
        {
            Id = 380733,
            Name = "不稳节杖",
            Quality = ItemQuality.Legendary,
            Slug = "unstable-scepter",
            ItemType = ItemType.Wand,
            TrinityItemType = TrinityItemType.Wand,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/unstable-scepter",
            Url = "https://us.battle.net/d3/en/item/unstable-scepter",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p1_wand_norm_unique_02_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/unstable-scepter",
            IsCrafted = false,
            LegendaryAffix = "奥术之球的爆炸将额外触发一次。奥术之球的伤害提高 350-450%。",
            SetName = "",
        };

        /// <summary>
        /// Blackhand Key 
        /// </summary>
        public static Item BlackhandKey = new Item
        {
            Id = 193355,
            Name = "黑手钥匙",
            Quality = ItemQuality.Legendary,
            Slug = "blackhand-key",
            ItemType = ItemType.Wand,
            TrinityItemType = TrinityItemType.Wand,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "wand_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/blackhand-key",
            Url = "https://us.battle.net/d3/en/item/blackhand-key",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_wand_006_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/blackhand-key",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Serpent's Sparker You may have one extra Hydra active at a time.
        /// </summary>
        public static Item SerpentsSparker = new Item
        {
            Id = 272084,
            Name = "妖蛇之牙",
            Quality = ItemQuality.Legendary,
            Slug = "serpents-sparker",
            ItemType = ItemType.Wand,
            TrinityItemType = TrinityItemType.Wand,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "wand_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/serpents-sparker",
            Url = "https://us.battle.net/d3/en/item/serpents-sparker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_wand_102_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/serpents-sparker",
            IsCrafted = false,
            LegendaryAffix = "你同时可拥有一只额外的多头蛇。",
            SetName = "",
        };

        /// <summary>
        /// Wand of Woh 3 additional Explosive Blasts are triggered after casting Explosive Blast.
        /// </summary>
        public static Item WandOfWoh = new Item
        {
            Id = 272086,
            Name = "沃尔魔杖",
            Quality = ItemQuality.Legendary,
            Slug = "wand-of-woh",
            ItemType = ItemType.Wand,
            TrinityItemType = TrinityItemType.Wand,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "wand_norm_unique_06",
            DataUrl = "https://us.battle.net/api/d3/data/item/wand-of-woh",
            Url = "https://us.battle.net/d3/en/item/wand-of-woh",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_wand_101_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/wand-of-woh",
            IsCrafted = false,
            LegendaryAffix = "施放聚能爆破后将触发3次额外的爆炸。聚能爆破的伤害提高 300-400%。",
            SetName = "",
        };

        /// <summary>
        /// Fragment of Destiny Spectral Blade attacks 50% faster and deals 150–200 % increased damage.
        /// </summary>
        public static Item FragmentOfDestiny = new Item
        {
            Id = 181995,
            Name = "命运碎片",
            Quality = ItemQuality.Legendary,
            Slug = "fragment-of-destiny",
            ItemType = ItemType.Wand,
            TrinityItemType = TrinityItemType.Wand,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Wand_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/fragment-of-destiny",
            Url = "https://us.battle.net/d3/en/item/fragment-of-destiny",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p4_unique_wand_010_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/fragment-of-destiny",
            IsCrafted = false,
            LegendaryAffix = "幽魂之刃的攻击速度加快 50%，造成的伤害提高 (150-200)%。",
            SetName = "",
        };

        /// <summary>
        /// Gesture of Orpheus Reduces the cooldown of Slow Time by 30–40 % .
        /// </summary>
        public static Item GestureOfOrpheus = new Item
        {
            Id = 182071,
            Name = "乐神的指挥棒",
            Quality = ItemQuality.Legendary,
            Slug = "gesture-of-orpheus",
            ItemType = ItemType.Wand,
            TrinityItemType = TrinityItemType.Wand,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Wand_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/gesture-of-orpheus",
            Url = "https://us.battle.net/d3/en/item/gesture-of-orpheus",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p2_unique_wand_002_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/gesture-of-orpheus",
            IsCrafted = false,
            LegendaryAffix = "使时间延缓的冷却时间缩短 (30-40)%。",
            SetName = "",
        };

        /// <summary>
        /// Slorak's Madness This wand finds your death humorous.
        /// </summary>
        public static Item SloraksMadness = new Item
        {
            Id = 181982,
            Name = "斯洛拉克的疯狂",
            Quality = ItemQuality.Legendary,
            Slug = "sloraks-madness",
            ItemType = ItemType.Wand,
            TrinityItemType = TrinityItemType.Wand,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Wand_norm_unique_01",
            DataUrl = "https://us.battle.net/api/d3/data/item/sloraks-madness",
            Url = "https://us.battle.net/d3/en/item/sloraks-madness",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_wand_013_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/sloraks-madness",
            IsCrafted = false,
            LegendaryAffix = "这把魔杖觉得你的死亡很有趣。",
            SetName = "",
        };

        /// <summary>
        /// Chantodo's Will 
        /// </summary>
        public static Item ChantodosWill = new Item
        {
            Id = 210479,
            Name = "迦陀朵的意志",
            Quality = ItemQuality.Legendary,
            Slug = "chantodos-will",
            ItemType = ItemType.Wand,
            TrinityItemType = TrinityItemType.Wand,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "Wand_norm_unique_07",
            DataUrl = "https://us.battle.net/api/d3/data/item/chantodos-will",
            Url = "https://us.battle.net/d3/en/item/chantodos-will",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_wand_012_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/chantodos-will",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "迦陀朵的决心",
        };

        /// <summary>
        /// Aether Walker Teleport no longer has a cooldown but costs 25 Arcane Power.
        /// </summary>
        public static Item AetherWalker = new Item
        {
            Id = 403781,
            Name = "以太行者",
            Quality = ItemQuality.Legendary,
            Slug = "aether-walker",
            ItemType = ItemType.Wand,
            TrinityItemType = TrinityItemType.Wand,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/aether-walker",
            Url = "https://us.battle.net/d3/en/item/aether-walker",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/p1_wand_norm_unique_01_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/aether-walker",
            IsCrafted = false,
            LegendaryAffix = "传送不再有冷却时间但要消耗25点奥能。",
            SetName = "",
        };

        /// <summary>
        /// The Executioner 
        /// </summary>
        public static Item TheExecutioner = new Item
        {
            Id = 186560,
            Name = "处决者",
            Quality = ItemQuality.Legendary,
            Slug = "the-executioner",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.TwoHandAxe,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedAxe_norm_unique_02",
            DataUrl = "https://us.battle.net/api/d3/data/item/the-executioner",
            Url = "https://us.battle.net/d3/en/item/the-executioner",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_axe_2h_003_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/the-executioner",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Burst of Wrath Killing enemies and destroying objects has a chance to grant 20% of your maximum primary resource.
        /// </summary>
        public static Item BurstOfWrath = new Item
        {
            Id = 271601,
            Name = "怒涌",
            Quality = ItemQuality.Legendary,
            Slug = "burst-of-wrath",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.TwoHandAxe,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/burst-of-wrath",
            Url = "https://us.battle.net/d3/en/item/burst-of-wrath",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_axe_2h_103_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/burst-of-wrath",
            IsCrafted = false,
            LegendaryAffix = "消灭敌人和摧毁物品有一定几率使你获得相当于主能量上限 20% 的能量。",
            SetName = "",
        };

        /// <summary>
        /// Butcher's Carver The Butcher still inhabits his carver.
        /// </summary>
        public static Item ButchersCarver = new Item
        {
            Id = 186494,
            Name = "屠夫的切肉刀",
            Quality = ItemQuality.Legendary,
            Slug = "butchers-carver",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.TwoHandAxe,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedAxe_norm_unique_03",
            DataUrl = "https://us.battle.net/api/d3/data/item/butchers-carver",
            Url = "https://us.battle.net/d3/en/item/butchers-carver",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_axe_2h_001_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/butchers-carver",
            IsCrafted = false,
            LegendaryAffix = "屠夫的怨魂依然寄宿在他的剁肉刀里。",
            SetName = "",
        };

        /// <summary>
        /// Messerschmidt's Reaver 
        /// </summary>
        public static Item MesserschmidtsReaver = new Item
        {
            Id = 191065,
            Name = "梅塞施密特的劫掠者",
            Quality = ItemQuality.Legendary,
            Slug = "messerschmidts-reaver",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.TwoHandAxe,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedAxe_norm_unique_04",
            DataUrl = "https://us.battle.net/api/d3/data/item/messerschmidts-reaver",
            Url = "https://us.battle.net/d3/en/item/messerschmidts-reaver",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_axe_2h_011_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/messerschmidts-reaver",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        /// <summary>
        /// Skorn 
        /// </summary>
        public static Item Skorn = new Item
        {
            Id = 192887,
            Name = "斯科恩巨斧",
            Quality = ItemQuality.Legendary,
            Slug = "skorn",
            ItemType = ItemType.Axe,
            TrinityItemType = TrinityItemType.TwoHandAxe,
            IsTwoHanded = true,
            BaseType = ItemBaseType.Weapon,
            InternalName = "twoHandedAxe_norm_unique_05",
            DataUrl = "https://us.battle.net/api/d3/data/item/skorn",
            Url = "https://us.battle.net/d3/en/item/skorn",
            IconUrl = "https://blzmedia-a.akamaihd.net/d3/icons/items/large/unique_axe_2h_009_x1_demonhunter_male.png",
            RelativeUrl = "/d3/en/item/skorn",
            IsCrafted = false,
            LegendaryAffix = "",
            SetName = "",
        };

        #endregion

        #region Methods

        public static Item GetItemByACD(ACDItem acdItem)
        {
            if (Items == null || acdItem == null || !acdItem.IsValid || acdItem.IsDisposed)
                return null;

            Item item;

            if (TryGetItemByGameBalanceId(acdItem.GameBalanceId, out item))
                return item;

            if (TryGetItemByActorSnoId(acdItem.ActorSnoId, out item))
                return item;

            return new Item();
        }

        public static Item GetItem(TrinityItem TrinityItem)
        {
            Item item;

            if (TryGetItemByGameBalanceId(TrinityItem.GameBalanceId, out item))
                return item;

            if (TryGetItemByActorSnoId(TrinityItem.ActorSnoId, out item))
                return item;

            return null;
        }

        public static bool TryGetItemByGameBalanceId(int gbid, out Item item)
        {
            if (Items.ContainsKey(gbid))
            {
                item = Items[gbid];
                return true;
            }
            item = default(Item);
            return false;
        }

        public static bool TryGetItemByActorSnoId(int sno, out Item item)
        {
            if (Items.ContainsKey(sno))
            {
                item = Items[sno];
                return true;
            }
            item = default(Item);
            return false;
        }

        /// <summary>
        /// Hashset of all Legendaries ActorSnoId
        /// </summary>
        public static HashSet<int> ItemIds
        {
            get { return _itemIds ?? (_itemIds = new HashSet<int>(ToList().Where(i => i.Id != 0).Select(i => i.Id))); }
        }
        private static HashSet<int> _itemIds;

        /// <summary>
        /// Dictionary of all Legendaries Items
        /// </summary>
        public static new Dictionary<int, Item> Items
        {
            get { return _items ?? (_items = ToList().Where(i => i.Id != 0).DistinctBy(i => i.Id).ToDictionary(k => k.Id, v => v)); }
        }
        private static Dictionary<int, Item> _items;

        /// <summary>
        /// Gets equipped legendaries
        /// </summary>
        public static List<Item> Equipped
        {
            get { return ToList().Where(i => i.IsEquipped).ToList(); }
        }

        /// <summary>
        /// Gets equipped legendaries
        /// </summary>
        public static List<TrinityItem> EquippedTrinityItems
        {
            get { return Core.Inventory.Equipped.Where(i => ItemIds.Contains(i.ActorSnoId)).ToList(); }
        }

        #endregion

    }
}

