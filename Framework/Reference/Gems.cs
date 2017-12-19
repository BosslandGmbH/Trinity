using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Reference
{
    public class Gems : FieldCollection<Gems, Item>
    {

        /// <summary>
        /// Simplicity's Strength Increase the damage of primary skills by 25.00%.
        /// </summary>
        public static Item SimplicitysStrength = new Item
        {
            Id = 405802,
            Name = "至简之力",
            Quality = ItemQuality.Legendary,
            Slug = "simplicitys-strength",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "",
            DataUrl = "https://cn.battle.net/api/d3/data/item/simplicitys-strength",
            Url = "http://us.battle.net/d3/en/item/simplicitys-strength",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_013_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "使主要技能造成的伤害提高 25.00%（+0.50%/等级）。",
            SetName = "",
            Importance = 5,
        };

        /// <summary>
        /// Bane of the Stricken Each attack you make against an enemy increases the damage it takes form your attacks by 0.80%.
        /// </summary>
        public static Item BaneOfTheStricken = new Item
        {
            Id = 428345,
            Name = "受罚者之灾",
            Quality = ItemQuality.Legendary,
            Slug = "bane-of-the-stricken",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_018_x1-182",
            DataUrl = "https://cn.battle.net/api/d3/data/item/bane-of-the-stricken",
            Url = "http://us.battle.net/d3/en/item/bane-of-the-stricken",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_018_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "你对敌人造成的每次攻击都会使敌人从你攻击中受到的伤害提高 0.80%（+0.01%/等级） 。",
            SetName = "",
            Importance = 15,
        };

        /// <summary>
        /// Bane of the Powerful Gain 20% increased damage for 30.0 seconds after killing an elite pack.
        /// </summary>
        public static Item BaneOfThePowerful = new Item
        {
            Id = 405775,
            Name = "强者之灾",
            Quality = ItemQuality.Legendary,
            Slug = "bane-of-the-powerful",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_001_x1-304",
            DataUrl = "https://cn.battle.net/api/d3/data/item/bane-of-the-powerful",
            Url = "http://us.battle.net/d3/en/item/bane-of-the-powerful",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_001_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "消灭一队精英怪后，伤害提高20%，持续 30.0（+1.0/等级） 秒。",
            SetName = "",
            Importance = 10,
        };


        /// <summary>
        /// Gem of Ease Monster kills grant +500 experience.
        /// </summary>
        public static Item GemOfEase = new Item
        {
            Id = 405783,
            Name = "自在宝石",
            Quality = ItemQuality.Legendary,
            Slug = "gem-of-ease",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_003_x1-371",
            DataUrl = "https://cn.battle.net/api/d3/data/item/gem-of-ease",
            Url = "http://us.battle.net/d3/en/item/gem-of-ease",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_003_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "消灭敌人获得的经验值 +500（++50/等级）。",
            SetName = "",
        };

        /// <summary>
        /// Esoteric Alteration Gain 10.0% non-Physical damage reduction.
        /// </summary>
        public static Item EsotericAlteration = new Item
        {
            Id = 428033,
            Name = "转煞秘石",
            Quality = ItemQuality.Legendary,
            Slug = "esoteric-alteration",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_016_x1-306",
            DataUrl = "https://cn.battle.net/api/d3/data/item/esoteric-alteration",
            Url = "http://us.battle.net/d3/en/item/esoteric-alteration",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_016_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "受到的非物理伤害降低 10.0%（+0.5%/等级）。",
            SetName = "",
            Importance = 5,
            MaxRank = 100,
        };

        /// <summary>
        /// Boon of the Hoarder 25.0% chance on killing an enemy to cause an explosion of gold.
        /// </summary>
        public static Item BoonOfTheHoarder = new Item
        {
            Id = 405803,
            Name = "囤宝者的恩惠",
            Quality = ItemQuality.Legendary,
            Slug = "boon-of-the-hoarder",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_014_x1-251",
            DataUrl = "https://cn.battle.net/api/d3/data/item/boon-of-the-hoarder",
            Url = "http://us.battle.net/d3/en/item/boon-of-the-hoarder",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_014_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "消灭敌人时有 25.0%（+1.5%/等级） 的几率爆出大量金币。",
            SetName = "",
            MaxRank = 50,
        };

        /// <summary>
        /// Enforcer Increase the damage of your pets by 15.00%.
        /// </summary>
        public static Item Enforcer = new Item
        {
            Id = 405798, //Enforcer ActorSnoId=405798 GameBalanceId=-1045305945 
            Name = "侍从宝石",
            Quality = ItemQuality.Legendary,
            Slug = "enforcer",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_010_x1-219",
            DataUrl = "https://cn.battle.net/api/d3/data/item/enforcer",
            Url = "http://us.battle.net/d3/en/item/enforcer",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_010_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "使你宠物造成的伤害提高 15.00%（+0.30%/等级）。",
            SetName = "",
        };

        /// <summary>
        /// Bane of the Trapped Increase damage against enemies under the effects of control-impairing effects by 15.00%.
        /// </summary>
        public static Item BaneOfTheTrapped = new Item
        {
            Id = 405781,
            Name = "困者之灾",
            Quality = ItemQuality.Legendary,
            Slug = "bane-of-the-trapped",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_002_x1-608",
            DataUrl = "https://cn.battle.net/api/d3/data/item/bane-of-the-trapped",
            Url = "http://us.battle.net/d3/en/item/bane-of-the-trapped",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_002_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "对受到控制类限制效果影响的敌人造成的伤害提高  15.00%（+0.30%/等级）。",
            SetName = "",
            Importance = 20,
        };

        /// <summary>
        /// Gem of Efficacious Toxin Poison all enemies hit for 2000% weapon damage over 10 seconds.
        /// </summary>
        public static Item GemOfEfficaciousToxin = new Item
        {
            Id = 405793,
            Name = "剧毒宝石",
            Quality = ItemQuality.Legendary,
            Slug = "gem-of-efficacious-toxin",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_005_x1-352",
            DataUrl = "https://cn.battle.net/api/d3/data/item/gem-of-efficacious-toxin",
            Url = "http://us.battle.net/d3/en/item/gem-of-efficacious-toxin",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_005_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "使命中的所有敌人中毒，在 10 秒内造成 2000%（+50%/等级） 的武器伤害。",
            SetName = "",
        };

        /// <summary>
        /// Invigorating Gemstone While under any control-impairing effects, reduce all damage taken by 30.0%.
        /// </summary>
        public static Item InvigoratingGemstone = new Item
        {
            Id = 405797,
            Name = "活力宝石",
            Quality = ItemQuality.Legendary,
            Slug = "invigorating-gemstone",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_009_x1-370",
            DataUrl = "https://cn.battle.net/api/d3/data/item/invigorating-gemstone",
            Url = "http://us.battle.net/d3/en/item/invigorating-gemstone",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_009_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "在任何控制类限制效果的影响下，受到的所有伤害降低 30.0%（+0.4%/等级）。",
            SetName = "",
        };

        /// <summary>
        /// Gogok of Swiftness 50.0% chance on hit to gain Swiftness, increasing your Attack Speed by 1% for 4 seconds. This effect stacks up to 15 times.
        /// </summary>
        public static Item GogokOfSwiftness = new Item
        {
            Id = 405796,
            Name = "迅捷勾玉",
            Quality = ItemQuality.Legendary,
            Slug = "gogok-of-swiftness",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_008_x1-252",
            DataUrl = "https://cn.battle.net/api/d3/data/item/gogok-of-swiftness",
            Url = "http://us.battle.net/d3/en/item/gogok-of-swiftness",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_008_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "每次攻击获得迅捷效果，使你的攻击速度提高 1%，躲闪几率提高 0.50%（+0.01%/等级），持续 4 秒。该效果最多可叠加 15 次。",
            SetName = "",
            Importance = 2,
            MaxRank = 150,
        };

        /// <summary>
        /// Mirinae, Teardrop of the Starweaver 15% chance on hit to smite a nearby enemy for 2000% weapon damage as Holy.
        /// </summary>
        public static Item MirinaeTeardropOfTheStarweaver = new Item
        {
            Id = 405795,
            Name = "银河，织星者之泪",
            Quality = ItemQuality.Legendary,
            Slug = "mirinae-teardrop-of-the-starweaver",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_007_x1-351",
            DataUrl = "https://cn.battle.net/api/d3/data/item/mirinae-teardrop-of-the-starweaver",
            Url = "http://us.battle.net/d3/en/item/mirinae-teardrop-of-the-starweaver",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_007_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "击中时有 15% 的几率重击附近的一名敌人，对其造成 2000%（+40%/等级） 的武器伤害（作为神圣伤害）。",
            SetName = "",
        };

        /// <summary>
        /// Iceblink Your Cold skills now apply Chill effects and your Chill effects now Slow enemy movement by an additional 5.0%.
        /// </summary>
        public static Item Iceblink = new Item
        {
            Id = 428355,
            Name = "闪耀冰晶",
            Quality = ItemQuality.Legendary,
            Slug = "iceblink",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "",
            DataUrl = "https://cn.battle.net/api/d3/data/item/iceblink",
            Url = "http://us.battle.net/d3/en/item/iceblink",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_021_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "你的冰霜技能现在会造成寒冷效果，且你的寒冷效果可使敌人的移动速度额外降低5.0%（+0.4%/等级）。",
            SetName = "",
            MaxRank = 50,
        };

        /// <summary>
        /// Molten Wildebeest's Gizzard Regenerates 10000 Life per Second.
        /// </summary>
        public static Item MoltenWildebeestsGizzard = new Item
        {
            Id = 428034,
            Name = "火牛羚砂囊",
            Quality = ItemQuality.Legendary,
            Slug = "molten-wildebeests-gizzard",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_017_x1-349",
            DataUrl = "https://cn.battle.net/api/d3/data/item/molten-wildebeests-gizzard",
            Url = "http://us.battle.net/d3/en/item/molten-wildebeests-gizzard",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_017_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "回复 10000（+1000/等级） 点每秒回复生命。",
            SetName = "",
        };

        /// <summary>
        /// Pain Enhancer Critical hits cause the enemy to bleed for 1200.0% weapon damage as Physical over 3 seconds.
        /// </summary>
        public static Item PainEnhancer = new Item
        {
            Id = 405794,
            Name = "增痛宝石",
            Quality = ItemQuality.Legendary,
            Slug = "pain-enhancer",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_006_x1-616",
            DataUrl = "https://cn.battle.net/api/d3/data/item/pain-enhancer",
            Url = "http://us.battle.net/d3/en/item/pain-enhancer",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_006_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "暴击使敌人流血，在3秒内受到 1200.0%（+30.0%/等级） 的武器伤害（作为物理伤害）。",
            SetName = "",
            Importance = 2,
        };

        /// <summary>
        /// Moratorium 25% of all damage taken is instead staggered and dealt to you over 3.00 seconds.
        /// </summary>
        public static Item Moratorium = new Item
        {
            Id = 405800,
            Name = "免死宝石",
            Quality = ItemQuality.Legendary,
            Slug = "moratorium",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_011_x1-346",
            DataUrl = "https://cn.battle.net/api/d3/data/item/moratorium",
            Url = "http://us.battle.net/d3/en/item/moratorium",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_011_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "受到所有伤害的25%会延迟生效，在 3.00（+0.10/等级） 秒内作用到你身上。",
            SetName = "",
        };

        /// <summary>
        /// Taeguk Gain 0.5% increased damage for 3 seconds after spending primary resource. This effect stacks up to 20 times.
        /// </summary>
        public static Item Taeguk = new Item
        {
            Id = 405804,
            Name = "太极石",
            Quality = ItemQuality.Legendary,
            Slug = "taeguk",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_015_x1-611",
            DataUrl = "https://cn.battle.net/api/d3/data/item/taeguk",
            Url = "http://us.battle.net/d3/en/item/taeguk",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_015_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "在消耗主要能量之后的3秒内，伤害提高0.5%。该效果最多可叠加 20（+1/等级） 次。",
            SetName = "",
            Importance = 5,
        };

        /// <summary>
        /// Zei's Stone of Vengeance Damage you deal is increased by 4.00% for every 10 yards between you and the enemy hit. Maximum 20.00% increase at 50 yards.
        /// </summary>
        public static Item ZeisStoneOfVengeance = new Item
        {
            Id = 405801,
            Name = "贼神的复仇之石",
            Quality = ItemQuality.Legendary,
            Slug = "zeis-stone-of-vengeance",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_012_x1-305",
            DataUrl = "https://cn.battle.net/api/d3/data/item/zeis-stone-of-vengeance",
            Url = "http://us.battle.net/d3/en/item/zeis-stone-of-vengeance",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_012_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "你和命中的敌人每间隔 10 码距离，你造成的伤害即可提高 4.00%（+0.25%/等级）。上限为 50 码距离，伤害提高 20.00%。",
            SetName = "",
            Importance = 10,
        };

        /// <summary>
        /// Wreath of Lightning 15% chance on hit to gain a Wreath of Lightning, dealing 600.0% weapon damage as Lightning every second to nearby enemies for 3 seconds.
        /// </summary>
        public static Item WreathOfLightning = new Item
        {
            Id = 405792,
            Name = "闪电华冠",
            Quality = ItemQuality.Legendary,
            Slug = "wreath-of-lightning",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_004_x1-350",
            DataUrl = "https://cn.battle.net/api/d3/data/item/wreath-of-lightning",
            Url = "http://us.battle.net/d3/en/item/wreath-of-lightning",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_004_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "击中时有 15% 的几率获得一个闪电华冠，每秒对附近的敌人造成 600.0%（+10.0%/等级） 的武器伤害（作为闪电伤害），持续 3 秒。",
            SetName = "",
        };

        /// <summary>
        /// Mutilation Guard Gain 10.0% melee damage reduction.
        /// </summary>
        public static Item MutilationGuard = new Item
        {
            Id = 428346,
            Name = "毁伤之御",
            Quality = ItemQuality.Legendary,
            Slug = "mutilation-guard",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "",
            DataUrl = "https://cn.battle.net/api/d3/data/item/mutilation-guard",
            Url = "http://us.battle.net/d3/en/item/mutilation-guard",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_019_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "受到近战伤害降低 10.0%（+0.5%/等级）。",
            SetName = "",
            MaxRank = 100,
        };

        /// <summary>
        /// Red Soul Shard Periodically struggle for control, unleashing a ring of Fire that inflicts 12500% weapon damage to enemies it passes through.
        /// </summary>
        public static Item RedSoulShard = new Item
        {
            Id = 454794,
            Name = "猩红灵魂碎片",
            Quality = ItemQuality.Legendary,
            Slug = "red-soul-shard",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "",
            DataUrl = "https://cn.battle.net/api/d3/data/item/red-soul-shard",
            Url = "http://us.battle.net/d3/en/item/red-soul-shard",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_022_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "周期性的争夺控制权并释放一个火环，对穿行其中的敌人造成10000%的武器伤害。",
            SetName = "",
        };

        /// <summary>
        /// Boyarsky's Chip Adds 16000 Thorns.
        /// </summary>
        public static Item BoyarskysChip = new Item
        {
            Id = 428347,
            Name = "波亚斯基的芯片",
            Quality = ItemQuality.Legendary,
            Slug = "boyarskys-chip",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "",
            DataUrl = "https://cn.battle.net/api/d3/data/item/boyarskys-chip",
            Url = "http://us.battle.net/d3/en/item/boyarskys-chip",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_020_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "增加 16000（+800/等级） 点荆棘伤害。",
            SetName = "",
            Importance = 15,
        };

        #region Methods

        /// <summary>
        /// Hashset of all gem ActorSnoId
        /// </summary>
        public static HashSet<int> ItemIds
        {
            get { return _itemIds ?? (_itemIds = new HashSet<int>(ToList().Where(i => i.Id != 0).Select(i => i.Id))); }
        }
        private static HashSet<int> _itemIds;

        /// <summary>
        /// Dictionary of all gems
        /// </summary>
        public static new Dictionary<int, Item> Items
        {
            get { return _items ?? (_items = ToList().Where(i => i.Id != 0).DistinctBy(i => i.Id).ToDictionary(k => k.Id, v => v)); }
        }
        private static Dictionary<int, Item> _items;

        /// <summary>
        /// Gets equipped gems
        /// </summary>
        public static List<Item> Equipped
        {
            get { return ToList().Where(i => i.IsEquipped && i.Id != 0).ToList(); }
        }

        #endregion
    }
}