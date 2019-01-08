using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Game;
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
            Id = SNOActor.Unique_Gem_013_x1,
            Name = "Simplicity's Strength",
            Quality = ItemQuality.Legendary,
            Slug = "simplicitys-strength",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/simplicitys-strength",
            Url = "http://us.battle.net/d3/en/item/simplicitys-strength",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_013_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Increase the damage of primary skills by 25.00%.",
            SetName = "",
            Importance = 5,
        };

        /// <summary>
        /// Bane of the Stricken Each attack you make against an enemy increases the damage it takes form your attacks by 0.80%.
        /// </summary>
        public static Item BaneOfTheStricken = new Item
        {
            Id = SNOActor.Unique_Gem_018_x1,
            Name = "Bane of the Stricken",
            Quality = ItemQuality.Legendary,
            Slug = "bane-of-the-stricken",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_018_x1-182",
            DataUrl = "https://us.battle.net/api/d3/data/item/bane-of-the-stricken",
            Url = "http://us.battle.net/d3/en/item/bane-of-the-stricken",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_018_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Each attack you make against an enemy increases the damage it takes form your attacks by 0.80%.",
            SetName = "",
            Importance = 15,
        };

        /// <summary>
        /// Bane of the Powerful Gain 20% increased damage for 30.0 seconds after killing an elite pack.
        /// </summary>
        public static Item BaneOfThePowerful = new Item
        {
            Id = SNOActor.Unique_Gem_001_x1,
            Name = "Bane of the Powerful",
            Quality = ItemQuality.Legendary,
            Slug = "bane-of-the-powerful",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_001_x1-304",
            DataUrl = "https://us.battle.net/api/d3/data/item/bane-of-the-powerful",
            Url = "http://us.battle.net/d3/en/item/bane-of-the-powerful",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_001_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Gain 20% increased damage for 30.0 seconds after killing an elite pack.",
            SetName = "",
            Importance = 10,
        };


        /// <summary>
        /// Gem of Ease Monster kills grant +500 experience.
        /// </summary>
        public static Item GemOfEase = new Item
        {
            Id = SNOActor.Unique_Gem_003_x1,
            Name = "Gem of Ease",
            Quality = ItemQuality.Legendary,
            Slug = "gem-of-ease",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_003_x1-371",
            DataUrl = "https://us.battle.net/api/d3/data/item/gem-of-ease",
            Url = "http://us.battle.net/d3/en/item/gem-of-ease",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_003_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Monster kills grant +500 experience.",
            SetName = "",
        };

        /// <summary>
        /// Esoteric Alteration Gain 10.0% non-Physical damage reduction.
        /// </summary>
        public static Item EsotericAlteration = new Item
        {
            Id = SNOActor.Unique_Gem_016_x1,
            Name = "Esoteric Alteration",
            Quality = ItemQuality.Legendary,
            Slug = "esoteric-alteration",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_016_x1-306",
            DataUrl = "https://us.battle.net/api/d3/data/item/esoteric-alteration",
            Url = "http://us.battle.net/d3/en/item/esoteric-alteration",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_016_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Gain 10.0% non-Physical damage reduction.",
            SetName = "",
            Importance = 5,
            MaxRank = 100,
        };

        /// <summary>
        /// Boon of the Hoarder 25.0% chance on killing an enemy to cause an explosion of gold.
        /// </summary>
        public static Item BoonOfTheHoarder = new Item
        {
            Id = SNOActor.Unique_Gem_014_x1,
            Name = "Boon of the Hoarder",
            Quality = ItemQuality.Legendary,
            Slug = "boon-of-the-hoarder",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_014_x1-251",
            DataUrl = "https://us.battle.net/api/d3/data/item/boon-of-the-hoarder",
            Url = "http://us.battle.net/d3/en/item/boon-of-the-hoarder",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_014_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "25.0% chance on killing an enemy to cause an explosion of gold.",
            SetName = "",
            MaxRank = 50,
        };

        /// <summary>
        /// Enforcer Increase the damage of your pets by 15.00%.
        /// </summary>
        public static Item Enforcer = new Item
        {
            Id = SNOActor.Unique_Gem_010_x1, //Enforcer ActorSnoId=405798 GameBalanceId=-1045305945 
            Name = "Enforcer",
            Quality = ItemQuality.Legendary,
            Slug = "enforcer",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_010_x1-219",
            DataUrl = "https://us.battle.net/api/d3/data/item/enforcer",
            Url = "http://us.battle.net/d3/en/item/enforcer",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_010_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Increase the damage of your pets by 15.00%.",
            SetName = "",
        };

        /// <summary>
        /// Bane of the Trapped Increase damage against enemies under the effects of control-impairing effects by 15.00%.
        /// </summary>
        public static Item BaneOfTheTrapped = new Item
        {
            Id = SNOActor.Unique_Gem_002_x1,
            Name = "Bane of the Trapped",
            Quality = ItemQuality.Legendary,
            Slug = "bane-of-the-trapped",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_002_x1-608",
            DataUrl = "https://us.battle.net/api/d3/data/item/bane-of-the-trapped",
            Url = "http://us.battle.net/d3/en/item/bane-of-the-trapped",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_002_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Increase damage against enemies under the effects of control-impairing effects by 15.00%.",
            SetName = "",
            Importance = 20,
        };

        /// <summary>
        /// Gem of Efficacious Toxin Poison all enemies hit for 2000% weapon damage over 10 seconds.
        /// </summary>
        public static Item GemOfEfficaciousToxin = new Item
        {
            Id = SNOActor.Unique_Gem_005_x1,
            Name = "Gem of Efficacious Toxin",
            Quality = ItemQuality.Legendary,
            Slug = "gem-of-efficacious-toxin",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_005_x1-352",
            DataUrl = "https://us.battle.net/api/d3/data/item/gem-of-efficacious-toxin",
            Url = "http://us.battle.net/d3/en/item/gem-of-efficacious-toxin",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_005_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Poison all enemies hit for 2000% weapon damage over 10 seconds.",
            SetName = "",
        };

        /// <summary>
        /// Invigorating Gemstone While under any control-impairing effects, reduce all damage taken by 30.0%.
        /// </summary>
        public static Item InvigoratingGemstone = new Item
        {
            Id = SNOActor.Unique_Gem_009_x1,
            Name = "Invigorating Gemstone",
            Quality = ItemQuality.Legendary,
            Slug = "invigorating-gemstone",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_009_x1-370",
            DataUrl = "https://us.battle.net/api/d3/data/item/invigorating-gemstone",
            Url = "http://us.battle.net/d3/en/item/invigorating-gemstone",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_009_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "While under any control-impairing effects, reduce all damage taken by 30.0%.",
            SetName = "",
        };

        /// <summary>
        /// Gogok of Swiftness 50.0% chance on hit to gain Swiftness, increasing your Attack Speed by 1% for 4 seconds. This effect stacks up to 15 times.
        /// </summary>
        public static Item GogokOfSwiftness = new Item
        {
            Id = SNOActor.Unique_Gem_008_x1,
            Name = "Gogok of Swiftness",
            Quality = ItemQuality.Legendary,
            Slug = "gogok-of-swiftness",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_008_x1-252",
            DataUrl = "https://us.battle.net/api/d3/data/item/gogok-of-swiftness",
            Url = "http://us.battle.net/d3/en/item/gogok-of-swiftness",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_008_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "50.0% chance on hit to gain Swiftness, increasing your Attack Speed by 1% for 4 seconds. This effect stacks up to 15 times.",
            SetName = "",
            Importance = 2,
            MaxRank = 150,
        };

        /// <summary>
        /// Mirinae, Teardrop of the Starweaver 15% chance on hit to smite a nearby enemy for 2000% weapon damage as Holy.
        /// </summary>
        public static Item MirinaeTeardropOfTheStarweaver = new Item
        {
            Id = SNOActor.Unique_Gem_007_x1,
            Name = "Mirinae, Teardrop of the Starweaver",
            Quality = ItemQuality.Legendary,
            Slug = "mirinae-teardrop-of-the-starweaver",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_007_x1-351",
            DataUrl = "https://us.battle.net/api/d3/data/item/mirinae-teardrop-of-the-starweaver",
            Url = "http://us.battle.net/d3/en/item/mirinae-teardrop-of-the-starweaver",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_007_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "15% chance on hit to smite a nearby enemy for 2000% weapon damage as Holy.",
            SetName = "",
        };

        /// <summary>
        /// Iceblink Your Cold skills now apply Chill effects and your Chill effects now Slow enemy movement by an additional 5.0%.
        /// </summary>
        public static Item Iceblink = new Item
        {
            Id = SNOActor.Unique_Gem_021_x1,
            Name = "Iceblink",
            Quality = ItemQuality.Legendary,
            Slug = "iceblink",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/iceblink",
            Url = "http://us.battle.net/d3/en/item/iceblink",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_021_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Your Cold skills now apply Chill effects and your Chill effects now Slow enemy movement by an additional 5.0%.",
            SetName = "",
            MaxRank = 50,
        };

        /// <summary>
        /// Molten Wildebeest's Gizzard Regenerates 10000 Life per Second.
        /// </summary>
        public static Item MoltenWildebeestsGizzard = new Item
        {
            Id = SNOActor.Unique_Gem_017_x1,
            Name = "Molten Wildebeest's Gizzard",
            Quality = ItemQuality.Legendary,
            Slug = "molten-wildebeests-gizzard",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_017_x1-349",
            DataUrl = "https://us.battle.net/api/d3/data/item/molten-wildebeests-gizzard",
            Url = "http://us.battle.net/d3/en/item/molten-wildebeests-gizzard",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_017_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Regenerates 10000 Life per Second.",
            SetName = "",
        };

        /// <summary>
        /// Pain Enhancer Critical hits cause the enemy to bleed for 1200.0% weapon damage as Physical over 3 seconds.
        /// </summary>
        public static Item PainEnhancer = new Item
        {
            Id = SNOActor.Unique_Gem_006_x1,
            Name = "Pain Enhancer",
            Quality = ItemQuality.Legendary,
            Slug = "pain-enhancer",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_006_x1-616",
            DataUrl = "https://us.battle.net/api/d3/data/item/pain-enhancer",
            Url = "http://us.battle.net/d3/en/item/pain-enhancer",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_006_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Critical hits cause the enemy to bleed for 1200.0% weapon damage as Physical over 3 seconds.",
            SetName = "",
            Importance = 2,
        };

        /// <summary>
        /// Moratorium 25% of all damage taken is instead staggered and dealt to you over 3.00 seconds.
        /// </summary>
        public static Item Moratorium = new Item
        {
            Id = SNOActor.Unique_Gem_011_x1,
            Name = "Moratorium",
            Quality = ItemQuality.Legendary,
            Slug = "moratorium",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_011_x1-346",
            DataUrl = "https://us.battle.net/api/d3/data/item/moratorium",
            Url = "http://us.battle.net/d3/en/item/moratorium",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_011_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "25% of all damage taken is instead staggered and dealt to you over 3.00 seconds.",
            SetName = "",
        };

        /// <summary>
        /// Taeguk Gain 0.5% increased damage for 3 seconds after spending primary resource. This effect stacks up to 20 times.
        /// </summary>
        public static Item Taeguk = new Item
        {
            Id = SNOActor.Unique_Gem_015_x1,
            Name = "Taeguk",
            Quality = ItemQuality.Legendary,
            Slug = "taeguk",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_015_x1-611",
            DataUrl = "https://us.battle.net/api/d3/data/item/taeguk",
            Url = "http://us.battle.net/d3/en/item/taeguk",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_015_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Gain 0.5% increased damage for 3 seconds after spending primary resource. This effect stacks up to 20 times.",
            SetName = "",
            Importance = 5,
        };

        /// <summary>
        /// Zei's Stone of Vengeance Damage you deal is increased by 4.00% for every 10 yards between you and the enemy hit. Maximum 20.00% increase at 50 yards.
        /// </summary>
        public static Item ZeisStoneOfVengeance = new Item
        {
            Id = SNOActor.Unique_Gem_012_x1,
            Name = "Zei's Stone of Vengeance",
            Quality = ItemQuality.Legendary,
            Slug = "zeis-stone-of-vengeance",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_012_x1-305",
            DataUrl = "https://us.battle.net/api/d3/data/item/zeis-stone-of-vengeance",
            Url = "http://us.battle.net/d3/en/item/zeis-stone-of-vengeance",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_012_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Damage you deal is increased by 4.00% for every 10 yards between you and the enemy hit. Maximum 20.00% increase at 50 yards.",
            SetName = "",
            Importance = 10,
        };

        /// <summary>
        /// Wreath of Lightning 15% chance on hit to gain a Wreath of Lightning, dealing 600.0% weapon damage as Lightning every second to nearby enemies for 3 seconds.
        /// </summary>
        public static Item WreathOfLightning = new Item
        {
            Id = SNOActor.Unique_Gem_004_x1,
            Name = "Wreath of Lightning",
            Quality = ItemQuality.Legendary,
            Slug = "wreath-of-lightning",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "Unique_Gem_004_x1-350",
            DataUrl = "https://us.battle.net/api/d3/data/item/wreath-of-lightning",
            Url = "http://us.battle.net/d3/en/item/wreath-of-lightning",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_004_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "15% chance on hit to gain a Wreath of Lightning, dealing 600.0% weapon damage as Lightning every second to nearby enemies for 3 seconds.",
            SetName = "",
        };

        /// <summary>
        /// Mutilation Guard Gain 10.0% melee damage reduction.
        /// </summary>
        public static Item MutilationGuard = new Item
        {
            Id = SNOActor.Unique_Gem_019_x1,
            Name = "Mutilation Guard",
            Quality = ItemQuality.Legendary,
            Slug = "mutilation-guard",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/mutilation-guard",
            Url = "http://us.battle.net/d3/en/item/mutilation-guard",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_019_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Gain 10.0% melee damage reduction.",
            SetName = "",
            MaxRank = 100,
        };

        /// <summary>
        /// Red Soul Shard Periodically struggle for control, unleashing a ring of Fire that inflicts 12500% weapon damage to enemies it passes through.
        /// </summary>
        public static Item RedSoulShard = new Item
        {
            Id = SNOActor.Unique_Gem_022_x1,
            Name = "Red Soul Shard",
            Quality = ItemQuality.Legendary,
            Slug = "red-soul-shard",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/red-soul-shard",
            Url = "http://us.battle.net/d3/en/item/red-soul-shard",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_022_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Periodically struggle for control, unleashing a ring of Fire that inflicts 12500% weapon damage to enemies it passes through.",
            SetName = "",
        };

        /// <summary>
        /// Boyarsky's Chip Adds 16000 Thorns.
        /// </summary>
        public static Item BoyarskysChip = new Item
        {
            Id = SNOActor.Unique_Gem_020_x1,
            Name = "Boyarsky's Chip",
            Quality = ItemQuality.Legendary,
            Slug = "boyarskys-chip",
            ItemType = ItemType.LegendaryGem,
            IsTwoHanded = false,
            BaseType = ItemBaseType.Gem,
            InternalName = "",
            DataUrl = "https://us.battle.net/api/d3/data/item/boyarskys-chip",
            Url = "http://us.battle.net/d3/en/item/boyarskys-chip",
            IconUrl = "http://media.blizzard.com/d3/icons/items/large/unique_gem_020_x1_demonhunter_male.png",
            RelativeUrl = "",
            IsCrafted = false,
            LegendaryAffix = "Adds 16000 Thorns.",
            SetName = "",
            Importance = 15,
        };

        #region Methods

        /// <summary>
        /// Hashset of all gem ActorSnoId
        /// </summary>
        public static HashSet<SNOActor> ItemIds
        {
            get { return _itemIds ?? (_itemIds = new HashSet<SNOActor>(ToList().Where(i => i.Id != 0).Select(i => i.Id))); }
        }
        private static HashSet<SNOActor> _itemIds;

        /// <summary>
        /// Dictionary of all gems
        /// </summary>
        public static new Dictionary<SNOActor, Item> Items
        {
            get { return _items ?? (_items = ToList().Where(i => i.Id != 0).DistinctBy(i => i.Id).ToDictionary(k => k.Id, v => v)); }
        }
        private static Dictionary<SNOActor, Item> _items;

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