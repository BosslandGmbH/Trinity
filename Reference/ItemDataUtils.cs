using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Objects;
using Trinity.Technicals;
using Trinity.UIComponents;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Trinity.Framework.Objects;
using Trinity.Framework.Actors;

namespace Trinity.Reference
{
    class ItemDataUtils
    {

        public static StatType GetMainStatType(ACDItem item)
        {
            if (item.Stats.Strength > 0) return StatType.Strength;
            if (item.Stats.Intelligence > 0) return StatType.Intelligence;
            if (item.Stats.Dexterity > 0) return StatType.Dexterity;
            return StatType.Unknown;
        }

        public static int GetMainStatValue(ACDItem item)
        {
            if (item.Stats.Strength > 0) return (int)item.Stats.Strength;
            if (item.Stats.Intelligence > 0) return (int)item.Stats.Intelligence;
            if (item.Stats.Dexterity > 0) return (int)item.Stats.Dexterity;
            return 0;
        }

        public static int GetMinBaseDamage(ACDItem item)
        {
            var min = Math.Min(item.Stats.MinDamageElemental, item.Stats.MaxDamageElemental);
            return (min != 0) ? (int)min : (int)item.WeaponBaseMinPhysicalDamage();
        }


        public static float GetMaxDamageProperty(ACDItem item)
        {
            var arcaneDmg = item.GetAttribute<float>(ActorAttributeType.DamageWeaponMaxArcane);
            if (arcaneDmg > 0)
                return arcaneDmg;

            var coldDmg = item.GetAttribute<float>(ActorAttributeType.DamageWeaponMaxCold);
            if (coldDmg > 0)
                return coldDmg;

            var fireDmg = item.GetAttribute<float>(ActorAttributeType.DamageWeaponMaxFire);
            if (fireDmg > 0)
                return fireDmg;

            var holyDmg = item.GetAttribute<float>(ActorAttributeType.DamageWeaponMaxHoly);
            if (holyDmg > 0)
                return holyDmg;

            var lightningDmg = item.GetAttribute<float>(ActorAttributeType.DamageWeaponMaxLightning);
            if (lightningDmg > 0)
                return lightningDmg;

            var poisonDmg = item.GetAttribute<float>(ActorAttributeType.DamageWeaponMaxPoison);
            if (poisonDmg > 0)
                return poisonDmg;

            var totalPhysicalDmg = item.GetAttribute<float>(ActorAttributeType.DamageWeaponMaxTotalPhysical);
            var basePhysicalDmg = item.GetAttribute<float>(ActorAttributeType.DamageWeaponMaxPhysical);
            return totalPhysicalDmg - basePhysicalDmg;
        }


        internal static double GetAttackSpeed(ACDItem acdItem)
        {
            return Math.Round(Math.Max(acdItem.Stats.AttackSpeedPercent, acdItem.Stats.AttackSpeedPercentBonus), MidpointRounding.AwayFromZero);
        }

        public static double GetElementalDamage(ACDItem item, Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return item.Stats.FireSkillDamagePercentBonus;
                case Element.Cold:
                    return item.Stats.ColdSkillDamagePercentBonus;
                case Element.Lightning:
                    return item.Stats.LightningSkillDamagePercentBonus;
                case Element.Poison:
                    return item.Stats.PosionSkillDamagePercentBonus;
                case Element.Arcane:
                    return item.Stats.ArcaneSkillDamagePercentBonus;
                case Element.Holy:
                    return item.Stats.HolySkillDamagePercentBonus;
                case Element.Physical:
                    return item.Stats.PhysicalSkillDamagePercentBonus;
                case Element.Any:

                    var fire = GetElementalDamage(item, Element.Fire);
                    if (fire > 0)
                        return fire;

                    var cold = GetElementalDamage(item, Element.Cold);
                    if (cold > 0)
                        return cold;

                    var lightning = GetElementalDamage(item, Element.Lightning);
                    if (lightning > 0)
                        return lightning;

                    var arcane = GetElementalDamage(item, Element.Arcane);
                    if (arcane > 0)
                        return arcane;

                    var poison = GetElementalDamage(item, Element.Poison);
                    if (poison > 0)
                        return poison;

                    var holy = GetElementalDamage(item, Element.Holy);
                    if (holy > 0)
                        return holy;

                    var physical = GetElementalDamage(item, Element.Physical);
                    if (physical > 0)
                        return physical;

                    break;
            }
            return 0;
        }

        public static double GetElementalResist(ACDItem item, Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return item.Stats.ResistFire;
                case Element.Cold:
                    return item.Stats.ResistCold;
                case Element.Lightning:
                    return item.Stats.ResistLightning;
                case Element.Poison:
                    return item.Stats.ResistPoison;
                case Element.Arcane:
                    return item.Stats.ResistArcane;
                case Element.Holy:
                    return item.Stats.ResistHoly;
                case Element.Physical:
                    return item.Stats.ResistPhysical;
                case Element.Any:

                    var fire = GetElementalResist(item, Element.Fire);
                    if (fire > 0)
                        return fire;

                    var cold = GetElementalResist(item, Element.Cold);
                    if (cold > 0)
                        return cold;

                    var lightning = GetElementalResist(item, Element.Lightning);
                    if (lightning > 0)
                        return lightning;

                    var arcane = GetElementalResist(item, Element.Arcane);
                    if (arcane > 0)
                        return arcane;

                    var poison = GetElementalResist(item, Element.Poison);
                    if (poison > 0)
                        return poison;

                    var holy = GetElementalResist(item, Element.Holy);
                    if (holy > 0)
                        return holy;

                    var physical = GetElementalResist(item, Element.Physical);
                    if (physical > 0)
                        return physical;

                    break;
            }
            return 0;
        }

        public enum StatType
        {
            Unknown = 0,
            Strength,
            Dexterity,
            Intelligence,
            Vitality,
        }

        public static int GetSkillDamagePercent(ACDItem item)
        {
            if (!SkillDamageByItemTypeAndClass.Any())
                return 0;

            var statType = GetMainStatType(item);
            var actorClasses = new List<ActorClass>();
            var itemType = TrinityItemManager.DetermineItemType(item);

            switch (statType)
            {
                case StatType.Dexterity:
                    actorClasses.Add(ActorClass.Monk);
                    actorClasses.Add(ActorClass.DemonHunter);
                    break;

                case StatType.Intelligence:
                    actorClasses.Add(ActorClass.Witchdoctor);
                    actorClasses.Add(ActorClass.Wizard);
                    break;

                case StatType.Strength:
                    actorClasses.Add(ActorClass.Crusader);
                    actorClasses.Add(ActorClass.Barbarian);
                    break;
            }

            if (!actorClasses.Any())
                return 0;

            foreach (var actorClass in actorClasses)
            {
                var kvp = new KeyValuePair<TrinityItemType, ActorClass>(itemType, actorClass);

                foreach (var skill in SkillDamageByItemTypeAndClass[kvp])
                {
                    var skillDamageIncrease = item.GetSkillDamageIncrease(skill.SNOPower);
                    if (skillDamageIncrease > 0)
                    {
                        Logger.Log(string.Format("SkillDamage +{0}% {1}", skillDamageIncrease, skill.Name));
                        return (int)skillDamageIncrease;
                    }
                }
            }

            return 0;
        }

        public static List<Skill> GetSkillsForItemType(TrinityItemType itemType, ActorClass actorClass = ActorClass.Invalid)
        {
            var result = new List<Skill>();
            if (actorClass != ActorClass.Invalid)
            {
                var kvp = new KeyValuePair<TrinityItemType, ActorClass>(itemType, actorClass);
                result.AddRange(SkillDamageByItemTypeAndClass[kvp]);
            }
            else
            {
                var actorClasses = new List<ActorClass>
                {
                        ActorClass.Monk,
                        ActorClass.DemonHunter,
                        ActorClass.Witchdoctor,
                        ActorClass.Wizard,
                        ActorClass.Crusader,
                        ActorClass.Barbarian
                };
                foreach (var ac in actorClasses)
                {
                    var kvp = new KeyValuePair<TrinityItemType, ActorClass>(itemType, ac);
                    result.AddRange(SkillDamageByItemTypeAndClass[kvp]);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns an object with the Min and Max values for a particular property and item
        /// Eg. Fire Damage 15-20%
        /// </summary>
        public static ItemStatRange GetItemStatRange(Item item, ItemProperty prop)
        {
            ItemStatRange statRange;

            var result = new ItemStatRange();

            if (prop == ItemProperty.Ancient)
                return new ItemStatRange { Max = 1, Min = 0 };

            if (ItemPropertyLimitsByItemType.TryGetValue(new KeyValuePair<TrinityItemType, ItemProperty>(item.TrinityItemType, prop), out statRange))
                result = statRange;

            if (SpecialItemsPropertyCases.TryGetValue(new Tuple<Item, ItemProperty>(item, prop), out statRange))
                result = statRange;

            if (prop == ItemProperty.PassivePower && ItemPassivePowers.ContainsKey(item.Id))
                result = ItemPassivePowers[item.Id].Range;

            return result;
        }

        public static ItemStatRange GetItemStatRange(TrinityItemType itemType, ItemProperty prop)
        {
            ItemStatRange statRange;

            var result = new ItemStatRange();

            if (prop == ItemProperty.Ancient)
                return new ItemStatRange { Max = 1, Min = 0 };

            if (ItemPropertyLimitsByItemType.TryGetValue(new KeyValuePair<TrinityItemType, ItemProperty>(itemType, prop), out statRange))
                result = statRange;

            foreach (var pair in SpecialItemsPropertyCases.Where(i => i.Key.Item1.TrinityItemType == itemType && i.Key.Item2 == prop))
            {
                var range = pair.Value;
                if (result.Min == 0 || range.Min < result.Min)
                    result.Min = range.Min;
                if (result.AncientMin == 0 || range.AncientMin < result.AncientMin)
                    result.AncientMin = range.AncientMin;
                if (range.Max > result.Max)
                    result.Max = range.Max;
                if (range.AncientMax > result.AncientMax)
                    result.AncientMax = range.AncientMax;
            }

            return result;
        }


        /// <summary>
        /// Determine if an item can have a given property
        /// </summary>
        /// <param name="item"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsValidPropertyForItem(Item item, ItemProperty prop)
        {
            ItemStatRange statRange;

            if (prop == ItemProperty.Ancient)
                return true;

            if (ItemPropertyLimitsByItemType.TryGetValue(new KeyValuePair<TrinityItemType, ItemProperty>(item.TrinityItemType, prop), out statRange))
                return true;

            if (SpecialItemsPropertyCases.ContainsKey(new Tuple<Item, ItemProperty>(item, prop)))
                return true;

            if (prop == ItemProperty.PassivePower && ItemPassivePowers.ContainsKey(item.Id))
                return true;

            return false;
        }

        /// <summary>
        /// Returns all the possible properties for a given item.
        /// </summary>
        public static List<ItemProperty> GetPropertiesForItem(Item item)
        {
            var props = ItemPropertyLimitsByItemType.Where(pair => pair.Key.Key == item.TrinityItemType).Select(pair => pair.Key.Value).ToList();
            var specialProps = SpecialItemsPropertyCases.Where(pair => pair.Key.Item1 == item).Select(pair => pair.Key.Item2).ToList();
            props = props.Concat(specialProps).Distinct().ToList();

            if (ItemPassivePowers.ContainsKey(item.Id))
            {
                props.Add(ItemProperty.PassivePower);
            }

            if (item.TrinityItemType != TrinityItemType.HealthPotion)
            {
                props.Add(ItemProperty.Ancient);
            }

            props.Sort();
            return props;
        }

        public static List<ItemProperty> GetPropertiesForItemType(TrinityItemType itemType)
        {
            var props = ItemPropertyLimitsByItemType.Where(pair => pair.Key.Key == itemType).Select(pair => pair.Key.Value).ToList();
            var specialProps = SpecialItemsPropertyCases.Where(pair => pair.Key.Item1.TrinityItemType == itemType).Select(pair => pair.Key.Item2).ToList();
            props = props.Concat(specialProps).Distinct().ToList();
            props.Add(ItemProperty.Ancient);
            props.Sort();
            return props;
        }

        /// <summary>
        /// Get all the possible options for multi-value item properties. 
        /// For example Skill Damage for Quiver can be for the Sentry, Cluster Arrow, Multishot etc.
        /// </summary>
        public static List<object> GetItemPropertyVariants(ItemProperty prop, TrinityItemType itemType)
        {
            var result = new List<object>();
            switch (prop)
            {
                case ItemProperty.SkillDamage:
                    var classRestriction = (Item.GetClassRestriction(itemType));
                    result = GetSkillsForItemType(itemType, classRestriction).Cast<object>().ToList();
                    break;

                case ItemProperty.ElementalDamage:
                    result = new List<object>
                    {
                        Element.Poison.ToEnumValue(),
                        Element.Holy.ToEnumValue(),
                        Element.Cold.ToEnumValue(),
                        Element.Arcane.ToEnumValue(),
                        Element.Fire.ToEnumValue(),
                        Element.Physical.ToEnumValue(),
                        Element.Lightning.ToEnumValue(),
                        Element.Any.ToEnumValue()
                    };
                    break;

                case ItemProperty.ElementalResist:
                    result = new List<object>
                    {
                        Element.Poison.ToEnumValue(),
                        Element.Holy.ToEnumValue(),
                        Element.Cold.ToEnumValue(),
                        Element.Arcane.ToEnumValue(),
                        Element.Fire.ToEnumValue(),
                        Element.Physical.ToEnumValue(),
                        Element.Lightning.ToEnumValue(),
                        Element.Any.ToEnumValue()
                    };
                    break;
            }
            return result;
        }

        public static readonly Dictionary<Item, Skill> SpecialItemSkills = new Dictionary<Item, Skill>
        {
            { Legendary.Manticore, Skills.DemonHunter.ClusterArrow }
        };

        public static float GetPassivePowerValue(CachedItem item)
        {
            //todo: bug with attribtues where sometimes the passivepower is not in the attribute list

            var passive = item.Attributes.GetAttributeItem(ActorAttributeType.ItemPowerPassive);

            ItemPowerDescripter desc;
            if (!ItemPassivePowers.TryGetValue(item.ActorSnoId, out desc))
            {
                return -1;
            }

            if (passive != null)
            {
                return desc.IsPercent ? passive.GetValue<float>() * 100 : passive.GetValue<float>();               
            }

            var acdItem = ZetaDia.Actors.GetACDByAnnId(item.AnnId);
            var value = acdItem.GetAttribute<float>(desc.Attribute);
            Logger.LogVerbose(">> PassivePower Attribute found with attribute id on AcdItem");
            return desc.IsPercent ? value * 100 : value;

        }

        public class ItemPowerDescripter
        {
            public ItemPowerDescripter(int powerId, int attribute, double min, double max, bool isPercent)
            {
                PowerId = powerId;
                Attribute = attribute;
                IsPercent = isPercent;
                Range = new ItemStatRange
                {
                    Max = max,
                    Min = min
                };
            }
            public ItemStatRange Range;
            public int Attribute;
            public int PowerId;
            public bool IsPercent;
        }

        public static readonly Dictionary<int, ItemPowerDescripter> ItemPassivePowers = new Dictionary<int, ItemPowerDescripter>
        {
            { 433027, new ItemPowerDescripter(433021, 1773655297, 20, 30, true) }, // Bottomless Potion of Rejuvenation (P2_Legendary_Potion_07) Restores 20–30% resource when used below 50% health.
            { 197813, new ItemPowerDescripter(364343, 1492350209, 240, 320, true) }, // Moonlight Ward (itemPassive_Unique_Amulet_003_x1) Hitting an enemy within 15 yards has a chance to ward you with shards of Arcane energy that explode when enemies get close, dealing 240–320% weapon damage as Arcane to enemies within 15 yards.
            { 193675, new ItemPowerDescripter(423235, 1733571841, 6, 8, false) }, // Belt of the Trove (P2_ItemPassive_Unique_Ring_011) Every 6–8 seconds, call down Bombardment on a random nearby enemy.
            { 205607, new ItemPowerDescripter(446615, 1829336321, 250, 300, true) }, // Heart of Iron (446615) Gain Thorns equal to 250–300% of your Vitality.
            { 299437, new ItemPowerDescripter(359546, 1472701697, 4, 6, false) }, // Golden Flense (ItemPassive_Unique_Ring_705_x1) Sweep Attack restores 4–6 Wrath for each enemy hit.
            { 197817, new ItemPowerDescripter(318716, 1305462017, 10, 15, true) }, // The Star of Azkaranth (ItemPassive_Unique_Ring_545_x1) Prevent all Fire damage taken and heal yourself for 10–15% of the amount prevented.
            { 193657, new ItemPowerDescripter(435040, 1781925121, 20, 25, false) }, // The Gavel of Judgment (P2_ItemPassive_Unique_Ring_059) Hammer of the Ancients returns 20–25 Fury if it hits 3 or fewer enemies.
            { 193692, new ItemPowerDescripter(318772, 1305691393, 20, 30, true) }, // Strongarm Bracers (ItemPassive_Unique_Ring_590_x1) Enemies hit by knockbacks suffer 20–30% increased damage for 5 seconds when they land.
            { 197837, new ItemPowerDescripter(318375, 1304065281, 12, 16, false) }, // Puzzle Ring (ItemPassive_Unique_Ring_513_x1) Summon a treasure goblin who picks up normal-quality items for you. After picking up 12–16 items, he drops a rare item with a chance for a legendary.
            { 298129, new ItemPowerDescripter(318881, 1306137857, 100, 135, true) }, // Harrington Waistguard (ItemPassive_Unique_Ring_685_x1) Opening a chest grants 100–135% increased damage for 10 seconds.
            { 209059, new ItemPowerDescripter(446502, 1828873473, 300, 400, true) }, // Hammer Jammers (446502) Enemies take 300–400% increased damage from your Blessed Hammers for 10 seconds after you hit them with a Blind, Immobilize, or Stun.
            { 446161, new ItemPowerDescripter(446162, 1827480833, 150, 200, true) }, // Bracer of Fury (446162) Heaven's Fury deals 150–200% increased damage to enemies that are Blinded, Immobilized, or Stunned.
            { 61550, new ItemPowerDescripter(318816, 1305871617, 0.5, 1.5, false) }, // Freeze of Deflection (ItemPassive_Unique_Ring_634_x1) Blocking an attack has a chance to Freeze the attacker for 0.5–1.5 seconds.
            { 197216, new ItemPowerDescripter(402416, 1648297217, 80, 100, true) }, // Depth Diggers (ItemPassive_Unique_Ring_908_x1) Primary skills that generate resource deal 80–100% additional damage.
            { 197220, new ItemPowerDescripter(434009, 1777702145, 450, 550, true) }, // Pox Faulds (itemPassive_Unique_Pants_007_p2) When 3 or more enemies are within 12 yards, you release a vile stench that deals 450–550% weapon damage as Poison every second for 5 seconds to enemies within 15 yards.
            { 298190, new ItemPowerDescripter(446142, 1827398913, 15, 20, true) }, // Shield of Fury (446142) Each time an enemy takes damage from your Heaven's Fury, it increases the damage they take from your Heaven&#39;s Fury by 15–20%.
            { 332172, new ItemPowerDescripter(434007, 1777693953, 120, 150, true) }, // St. Archew's Gage (ItemPassive_Unique_Ring_664_p2) The first time an elite pack damages you, gain an absorb shield equal to 120–150% of your maximum Life for 10 seconds.
            { 188185, new ItemPowerDescripter(364325, 1492276481, 20, 40, true) }, // Odyn Son (itemPassive_Unique_Mace_1H_002_x1) 20–40% chance to Chain Lightning enemies when you hit them.
            { 299426, new ItemPowerDescripter(359538, 1472668929, 15, 20, true) }, // Kassar's Retribution (ItemPassive_Unique_Ring_701_x1) Casting Justice increases your movement speed by 15–20% for 2 seconds.
            { 434627, new ItemPowerDescripter(434626, 1780229377, 20, 25, true) }, // Bottomless Potion of Amplification (X1_Legendary_Potion_09) Increases healing from all sources by 20–25% for 5 seconds.
            { 271598, new ItemPowerDescripter(318869, 1306088705, 75, 100, true) }, // Hack (ItemPassive_Unique_Ring_673_x1) 75–100% of your Thorns damage is applied on every attack.
            { 193669, new ItemPowerDescripter(446541, 1829033217, 25, 30, true) }, // String of Ears (446541) Reduces damage from melee attacks by 25–30%.
            { 446057, new ItemPowerDescripter(446008, 1826850049, 400, 500, true) }, // Akkhan’s Manacles (446008) Blessed Shield damage is increased by 400–500% for the first enemy it hits.
            { 197824, new ItemPowerDescripter(318719, 1305474305, 10, 15, true) }, // Mara's Kaleidoscope (ItemPassive_Unique_Ring_548_x1) Prevent all Poison damage taken and heal yourself for 10–15% of the amount prevented.
            { 299414, new ItemPowerDescripter(318888, 1306166529, 20, 25, true) }, // Akarat's Awakening (ItemPassive_Unique_Ring_692_x1) Every successful block has a 20–25% chance to reduce all cooldowns by 1 second.
            { 212230, new ItemPowerDescripter(364341, 1492342017, 100, 130, true) }, // Thundergod's Vigor (ItemPassive_Unique_BarbBelt_003_x1) Blocking, dodging or being hit causes you to discharge bolts of electricity that deal 100–130% weapon damage as Lightning.
            { 433496, new ItemPowerDescripter(430674, 1764041985, 150, 200, true) }, // Convention of Elements (P2_ItemPassive_Unique_Ring_038) Gain 150–200% increased damage to a single element for 4 seconds. This effect rotates through the elements available to your class in the following order: Arcane, Cold, Fire, Holy, Lightning, Physical, Poison.
            { 440432, new ItemPowerDescripter(440790, 1805477121, 60000, 80000, false) }, // Coils of the First Spider (P3_ItemPassive_Unique_Ring_029) While channeling Firebats, you gain 30% damage reduction and 60000–80000 Life per Hit.
            { 191278, new ItemPowerDescripter(318721, 1305482497, 25, 30, true) }, // Uhkapian Serpent (ItemPassive_Unique_Ring_550_x1) 25–30% of the damage you take is redirected to your Zombie Dogs.
            { 299443, new ItemPowerDescripter(318411, 1304212737, 75, 100, true) }, // Mask of Jeram (ItemPassive_Unique_Ring_526_x1) Pets deal 75–100% increased damage.
            { 440426, new ItemPowerDescripter(445427, 1824470273, 9, 12, false) }, // The Shame of Delsere (445427) Your Signature Spells attack 50% faster and restore 9–12 Arcane Power.
            { 415050, new ItemPowerDescripter(451186, 1848059137, 275, 350, true) }, // Nilfur's Boast (451186) Increase the damage of Meteor by 200%. When your Meteor hits 3 or fewer enemies, the damage is increased by 275–350%.
            { 175937, new ItemPowerDescripter(318433, 1304302849, 250, 300, true) }, // The Fist of Az'Turrasq (ItemPassive_Unique_Ring_540_x1) Exploding Palm's on-death explosion damage is increased by 250–300%.
            { 298116, new ItemPowerDescripter(318770, 1305683201, 9, 12, true) }, // Ancient Parthan Defenders (ItemPassive_Unique_Ring_588_x1) Each stunned enemy within 25 yards reduces your damage taken by 9–12%.
            { 181995, new ItemPowerDescripter(450472, 1845134593, 150, 200, true) }, // Fragment of Destiny (450472) Spectral Blade attacks 50% faster and deals 150–200% increased damage.
            { 299413, new ItemPowerDescripter(318887, 1306162433, 45, 60, true) }, // Hallowed Bulwark (ItemPassive_Unique_Ring_691_x1) Iron Skin also increases your Block Amount by 45–60%.
            { 328591, new ItemPowerDescripter(446641, 1829442817, 1, 1.4, true) }, // Sword of Ill Will (446641) Chakram deals 1.0–1.4% increased damage for every point of Hatred you have.
            { 194219, new ItemPowerDescripter(374344, 1533314305, 1, 2, false) }, // Buriza-Do Kyanon (ItemPassive_Unique_XBow_011_x1) Your projectiles pierce 1–2 additional times.
            { 428805, new ItemPowerDescripter(428812, 1756415233, 3, 4, false) }, // Bottomless Potion of Fear (X1_Legendary_Potion_08) Fears enemies within 12 yards for 3–4 seconds.
            { 440428, new ItemPowerDescripter(441517, 1808454913, 30, 35, true) }, // Wraps of Clarity (P3_ItemPassive_Unique_Ring_038) Your Hatred Generators reduce your damage taken by 30–35% for 5 seconds.
            { 298125, new ItemPowerDescripter(434038, 1777820929, 500, 650, true) }, // Sash of Knives (ItemPassive_Unique_Ring_753_p2) With every attack, you throw a dagger at a nearby enemy for 500–650% weapon damage as Physical.
            { 197624, new ItemPowerDescripter(359605, 1472943361, 200, 250, true) }, // Meticulous Bolts (ItemPassive_Unique_Ring_749_x1) Elemental Arrow gains an effect based on the rune: Ball Lightning now travels at 30% speed. Frost Arrow damage and Chilled duration increased by 200–250%. Immolation Arrow ground damage over time increased by 200–250%. Lightning Bolts damage and Stun duration increased by 200–250%. Nether Tentacles damage and healing amount increased by 200–250%.
            { 298124, new ItemPowerDescripter(318241, 1303516417, 300, 400, true) }, // Razor Strop (ItemPassive_Unique_Ring_500_x1) Picking up a Health Globe releases an explosion that deals 300–400% weapon damage as Fire to enemies within 20 yards.
            { 205642, new ItemPowerDescripter(318731, 1305523457, 40, 50, true) }, // Tasker and Theo (ItemPassive_Unique_Ring_554_x1) Increase attack speed of your pets by 40–50%.
            { 221760, new ItemPowerDescripter(446318, 1828119809, 40, 50, true) }, // Manticore (446318) Reduces the Hatred cost of Cluster Arrow by 40–50%.
            { 298118, new ItemPowerDescripter(318818, 1305879809, 25, 30, true) }, // Reaper's Wraps (ItemPassive_Unique_Ring_636_x1) Health globes restore 25–30% of your primary resource.
            { 220326, new ItemPowerDescripter(445765, 1825854721, 45, 60, true) }, // Vile Hive (445765) Locust Swarm gains the effect of the Pestilence rune and deals 45–60% increased damage.
            { 195370, new ItemPowerDescripter(447030, 1831036161, 15, 20, false) }, // Last Breath (447030) Reduces cooldown of Mass Confusion by 15–20 seconds.
            { 193673, new ItemPowerDescripter(318419, 1304245505, 4, 6, false) }, // Pride of Cassius (ItemPassive_Unique_Ring_530_x1) Increases the duration of Ignore Pain by 4–6 seconds.
            { 271875, new ItemPowerDescripter(318379, 1304081665, 3, 4, false) }, // Kridershot (ItemPassive_Unique_Ring_517_x1) Elemental Arrow now generates 3–4 Hatred.
            { 271731, new ItemPowerDescripter(445274, 1823843585, 160, 200, true) }, // Lord Greenstone's Fan (445274) Every second, gain 160–200% increased damage for your next Fan of Knives. Stacks up to 30 times.
            { 197203, new ItemPowerDescripter(449064, 1839367425, 90, 95, true) }, // Aquila Cuirass (449064) While above 90–95% primary resource, all damage taken is reduced by 50%.
            { 222455, new ItemPowerDescripter(318790, 1305765121, 23, 30, true) }, // Cindercoat (ItemPassive_Unique_Ring_608_x1) Reduces the resource cost of Fire skills by 23–30%.
            { 449047, new ItemPowerDescripter(449048, 1839301889, 50, 65, true) }, // Hergbrash’s Binding (449048) Reduces the Arcane Power cost of Arcane Torrent, Disintegrate, and Ray of Frost by 50–65%.
            { 298146, new ItemPowerDescripter(318857, 1306039553, 15, 20, true) }, // Deathseer's Cowl (ItemPassive_Unique_Ring_662_x1) 15–20% chance on being hit by an Undead enemy to charm it for 2 seconds.
            { 271957, new ItemPowerDescripter(318432, 1304298753, 30, 35, false) }, // Jawbreaker (ItemPassive_Unique_Ring_539_x1) When Dashing Strike hits an enemy more than 30–35 yards away, its Charge cost is refunded.
            { 423247, new ItemPowerDescripter(359554, 1472734465, 3000, 4000, true) }, // Crashing Rain (ItemPassive_Unique_Ring_709_x1) Rain of Vengeance also summons a crashing beast that deals 3000–4000% weapon damage.
            { 197205, new ItemPowerDescripter(318349, 1303958785, 15, 20, true) }, // Frostburn (ItemPassive_Unique_Ring_505_x1) Cold skills deal 15–20% increased damage and have a 50% chance to Freeze enemies.
            { 446188, new ItemPowerDescripter(446187, 1827583233, 50, 60, true) }, // Elusive Ring (446187) After casting Shadow Power, Smoke Screen, or Vault, take 50–60% reduced damage for 8 seconds.
            { 212590, new ItemPowerDescripter(446565, 1829131521, 45, 55, true) }, // Justice Lantern (446565) Gain damage reduction equal to 45–55% of your Block Chance.            
            { 298137, new ItemPowerDescripter(446639, 1829434625, 8, 9, true) }, // Zoey's Secret (446639) You take 8.0–9.0% less damage for every Companion you have active.
            { 298127, new ItemPowerDescripter(434008, 1777698049, 3, 4, false) }, // Cord of the Sherma (ItemPassive_Unique_Ring_560_p2) Chance on hit to create a chaos field that Blinds and Slows enemies inside for 3–4 seconds.
            { 271666, new ItemPowerDescripter(318753, 1305613569, 40, 50, true) }, // The Furnace (ItemPassive_Unique_Ring_571_x1) Increases damage against elites by 40–50%.
            { 430567, new ItemPowerDescripter(402455, 1648456961, 5000, 6000, true) }, // Corrupted Ashbringer (ItemPassive_Unique_Ring_916_x1) Chance on kill to raise a skeleton to fight for you. Upon accumulating 5 skeletons, they each explode for 1000% weapon damage and the sword transforms into Ashbringer for a short time. Attacking with Ashbringer burns your enemy for 5000–6000% weapon damage as Holy.
            { 430290, new ItemPowerDescripter(430289, 1762465025, 30, 40, true) }, // Spirit Guards (P2_ItemPassive_Unique_Ring_034) Your Spirit Generators reduce your damage taken by 30–40% for 3 seconds.
            { 447294, new ItemPowerDescripter(447295, 1832121601, 125, 150, true) }, // Pinto's Pride (447295) Wave of Light also Slows enemies by 80% for 3 seconds and deals 125–150% increased damage.
            { 145850, new ItemPowerDescripter(451168, 1847985409, 75, 100, true) }, // Fleshrake (451168) Dashing Strike increases the damage of Dashing Strike by 75–100% for 1 second, stacking up to 5 times.
            { 440427, new ItemPowerDescripter(449222, 1840014593, 150, 200, true) }, // Bindings of the Lesser Gods (449222) Enemies hit by your Cyclone Strike take 150–200% increased damage from your Mystic Ally for 5 seconds.
            { 298158, new ItemPowerDescripter(449236, 1840071937, 40, 50, true) }, // Lefebvre’s Soliloquy (449236) Cyclone Strike reduces your damage taken by 40–50% for 5 seconds.
            { 449038, new ItemPowerDescripter(449031, 1839232257, 300, 400, true) }, // Cesar’s Memento (449031) Enemies take 300–400% increased damage from your Tempest Rush for 5 seconds after you hit them with a Blind, Freeze, or Stun.
            { 229716, new ItemPowerDescripter(318763, 1305654529, 279, 372, true) }, // Thunderfury, Blessed Blade of the Windseeker (ItemPassive_Unique_Ring_581_x1) Chance on hit to blast your enemy with Lightning, dealing 279–372% weapon damage as Lightning and then jumping to additional nearby enemies. Each enemy hit has their attack speed and movement speed reduced by 30% for 3 seconds. Jumps up to 5 targets.
            { 440425, new ItemPowerDescripter(440598, 1804690689, 3, 3.5, true) }, // Binding of the Lost (P3_ItemPassive_Unique_Ring_027) Each hit with Seven-Sided Strike grants 3.0–3.5% damage reduction for 7 seconds.
            { 271728, new ItemPowerDescripter(445279, 1823864065, 10, 15, false) }, // Karlei's Point (445279) Impale returns 10–15 Hatred if it hits an enemy already Impaled.
            { 429681, new ItemPowerDescripter(446640, 1829438721, 20, 25, true) }, // Mantle of Channeling (446640) While channeling Whirlwind, Rapid Fire, Strafe, Tempest Rush, Firebats, Arcane Torrent, Disintegrate, or Ray of Frost, you deal 20–25% increased damage and take 25% reduced damage.
            { 184199, new ItemPowerDescripter(318793, 1305777409, 15, 20, true) }, // Winter Flurry (ItemPassive_Unique_Ring_611_x1) Enemies killed by Cold damage have a 15–20% chance to release a Frost Nova.
            { 195325, new ItemPowerDescripter(434849, 1781142785, 150, 200, true) }, // Triumvirate (P2_ItemPassive_Unique_Ring_052) Your Signature Spells increase the damage of Arcane Orb by 150–200% for 6 seconds, stacking up to 3 times.
            { 331908, new ItemPowerDescripter(449063, 1839363329, 30, 35, true) }, // Deathwish (449063) While channeling Arcane Torrent, Disintegrate, or Ray of Frost, all damage is increased by 30–35%.
            { 298050, new ItemPowerDescripter(318381, 1304089857, 20, 25, true) }, // Countess Julia's Cameo (ItemPassive_Unique_Ring_519_x1) Prevent all Arcane damage taken and heal yourself for 20–25% of the amount prevented.
            { 298055, new ItemPowerDescripter(318410, 1304208641, 3, 4, true) }, // Rakoff's Glass of Life (ItemPassive_Unique_Ring_525_x1) Enemies you kill have a 3–4% additional chance to drop a health globe.
            { 298051, new ItemPowerDescripter(318759, 1305638145, 60, 80, true) }, // The Ess of Johan (ItemPassive_Unique_Ring_577_x1) Chance on hit to pull in enemies toward your target and Slow them by 60–80%.
            { 298052, new ItemPowerDescripter(334880, 1371669761, 4, 6, false) }, // Golden Gorget of Leoric (ItemPassive_Unique_Amulet_105_x1) After earning a massacre bonus, 4–6 Skeletons are summoned to fight by your side for 10 seconds.
            { 193686, new ItemPowerDescripter(449043, 1839281409, 75, 100, true) }, // Ashnagarr’s Blood Bracer (449043) Increases the potency of your shields by 75–100%.
            { 181511, new ItemPowerDescripter(364321, 1492260097, 20, 45, true) }, // Scourge (ItemPassive_Unique_Sword_2H_004_x1) 20–45% chance when attacking to explode with demonic fury for 1800-2000% weapon damage as Fire.
            { 212586, new ItemPowerDescripter(402460, 1648477441, 10, 12, false) }, // Nagelring (ItemPassive_Unique_Ring_921_x1) Summons a Fallen Lunatic to your side every 10–12 seconds.
            { 271597, new ItemPowerDescripter(447029, 1831032065, 100, 125, false) }, // Mordullu's Promise (447029) Firebomb generates 100–125 Mana.
            { 271634, new ItemPowerDescripter(446195, 1827616001, 125, 150, true) }, // The Twisted Sword (446195) Energy Twister damage is increased by 125–150% for each Energy Twister you have out up to a maximum of 8.
            { 218681, new ItemPowerDescripter(440336, 1803617537, 15, 20, false) }, // The Swami (P3_ItemPassive_Unique_Ring_022) The bonuses from Archon stacks now last for 15–20 seconds after Archon expires.
            { 339125, new ItemPowerDescripter(318877, 1306121473, 25, 30, true) }, // Irontoe Mudsputters (ItemPassive_Unique_Ring_681_x1) Gain up to 25–30% increased movement speed based on amount of Life missing.
            { 200310, new ItemPowerDescripter(434005, 1777685761, 25, 35, true) }, // Death Watch Mantle (ItemPassive_Unique_Shoulder_002_p2) 25–35% chance to explode in a fan of knives for 750-950% weapon damage when hit.
            { 426784, new ItemPowerDescripter(449049, 1839305985, 45, 60, true) }, // Warhelm of Kassar (449049) Reduce the cooldown and increase the damage of Phalanx by 45–60%.
            { 197717, new ItemPowerDescripter(434033, 1777800449, 650, 850, true) }, // Schaefer's Hammer (ItemPassive_Unique_Mace_2H_009_p2) Casting a Lightning skill charges you with Lightning, causing you to deal 650–850% weapon damage as Lightning every second for 5 seconds to nearby enemies.
            { 298089, new ItemPowerDescripter(434036, 1777812737, 25, 35, true) }, // Wyrdward (ItemPassive_Unique_Ring_670_p2) Lightning damage has a 25–35% chance to Stun for 1.5 seconds.
            { 271639, new ItemPowerDescripter(318412, 1304216833, 550, 700, true) }, // Stalgard's Decimator (ItemPassive_Unique_Ring_527_x1) Your melee attacks throw a piercing axe at a nearby enemy, dealing 550–700% weapon damage as Physical.
            { 212648, new ItemPowerDescripter(402461, 1648481537, 70, 85, true) }, // Oculus Ring (ItemPassive_Unique_Ring_922_x1) Chance to create an area of focused power on killing a monster. Damage is increased by 70–85% while standing in the area.
            { 271880, new ItemPowerDescripter(428220, 1753990401, 20, 25, true) }, // Odyssey's End (P2_ItemPassive_Unique_Ring_023) Enemies snared by your Entangling Shot take 20–25% increased damage from all sources.





        };

        /// <summary>
        /// Items with unusual properties are listed here
        /// Determines if Property will be available in ItemList rules dropdown.
        /// </summary>
        public static readonly Dictionary<Tuple<Item, ItemProperty>, ItemStatRange> SpecialItemsPropertyCases = new Dictionary<Tuple<Item, ItemProperty>, ItemStatRange>
        {
            { new Tuple<Item, ItemProperty>(Legendary.HellcatWaistguard, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 6, Min = 3}},
            { new Tuple<Item, ItemProperty>(Legendary.HellcatWaistguard, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            { new Tuple<Item, ItemProperty>(Legendary.TheWitchingHour, ItemProperty.CriticalHitDamage), new ItemStatRange { Max = 50, Min = 26 }},
            { new Tuple<Item, ItemProperty>(Legendary.TheWitchingHour, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            { new Tuple<Item, ItemProperty>(Legendary.Magefist, ItemProperty.FireSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.Cindercoat, ItemProperty.FireSkills), new ItemStatRange { Max = 20, Min = 15 }},
            //{ new Tuple<Item, ItemProperty>(Legendary.UnboundBolt, ItemProperty.CriticalHitDamage), new ItemStatRange { Max = 35, Min = 31 }},
            { new Tuple<Item, ItemProperty>(Legendary.LacuniProwlers, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            //{ new Tuple<Item, ItemProperty>(Legendary.SteadyStrikers, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            { new Tuple<Item, ItemProperty>(Legendary.MempoOfTwilight, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            { new Tuple<Item, ItemProperty>(Legendary.AndarielsVisage, ItemProperty.ElementalDamage), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.AndarielsVisage, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            { new Tuple<Item, ItemProperty>(Legendary.SunKeeper, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 30, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.Frostburn, ItemProperty.ColdSkills), new ItemStatRange { Max = 15, Min = 10 }},
            { new Tuple<Item, ItemProperty>(Legendary.ThundergodsVigor, ItemProperty.LightningSkills), new ItemStatRange { Max = 15, Min = 10 }},
            { new Tuple<Item, ItemProperty>(Legendary.SashOfKnives, ItemProperty.ResourceCost), new ItemStatRange { Max = 8, Min = 5 }},
            { new Tuple<Item, ItemProperty>(Legendary.StoneOfJordan, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 30, Min = 25 }},
            { new Tuple<Item, ItemProperty>(Legendary.StoneOfJordan, ItemProperty.ElementalDamage), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.Unity, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 15, Min = 12 }},
            { new Tuple<Item, ItemProperty>(Legendary.Etrayu, ItemProperty.ColdSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.Uskang, ItemProperty.LightningSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.Triumvirate, ItemProperty.LightningSkills), new ItemStatRange { Max = 10, Min = 7 }},
            { new Tuple<Item, ItemProperty>(Legendary.Triumvirate, ItemProperty.FireSkills), new ItemStatRange { Max = 10, Min = 7 }},
            { new Tuple<Item, ItemProperty>(Legendary.Triumvirate, ItemProperty.ArcaneSkills), new ItemStatRange { Max = 10, Min = 7 }},
            { new Tuple<Item, ItemProperty>(Legendary.WinterFlurry, ItemProperty.ColdSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.VigilanteBelt, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            { new Tuple<Item, ItemProperty>(Legendary.SaffronWrap, ItemProperty.ResourceCost), new ItemStatRange { Max = 6, Min = 4 }},
            { new Tuple<Item, ItemProperty>(Legendary.LidlessWall, ItemProperty.ElementalDamage), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.Skycutter, ItemProperty.HolySkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.TheBurningAxeOfSankis, ItemProperty.FireSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.GestureOfOrpheus, ItemProperty.ArcaneSkills), new ItemStatRange { Max = 15, Min = 10 }},
            { new Tuple<Item, ItemProperty>(Legendary.GestureOfOrpheus, ItemProperty.FireSkills), new ItemStatRange { Max = 15, Min = 10 }},
            { new Tuple<Item, ItemProperty>(Legendary.GestureOfOrpheus, ItemProperty.LightningSkills), new ItemStatRange { Max = 15, Min = 10 }},
            { new Tuple<Item, ItemProperty>(Legendary.GestureOfOrpheus, ItemProperty.ColdSkills), new ItemStatRange { Max = 15, Min = 10 }},
            { new Tuple<Item, ItemProperty>(Legendary.Maximus, ItemProperty.FireSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.BalefireCaster, ItemProperty.FireSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.Wormwood, ItemProperty.PoisonSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.HeartSlaughter, ItemProperty.PhysicalSkills), new ItemStatRange { Max = 30, Min = 25 }},
            { new Tuple<Item, ItemProperty>(Legendary.SchaefersHammer, ItemProperty.LightningSkills), new ItemStatRange { Max = 25, Min = 20 }},
            { new Tuple<Item, ItemProperty>(Legendary.WrathOfTheBoneKing, ItemProperty.ColdSkills), new ItemStatRange { Max = 30, Min = 25 }},
            { new Tuple<Item, ItemProperty>(Legendary.WonKhimLau, ItemProperty.LightningSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.Azurewrath, ItemProperty.ColdSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.Doombringer, ItemProperty.PhysicalSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.AkaneshTheHeraldOfRighteousness, ItemProperty.HolySkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.OdynSon, ItemProperty.LightningSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.HolyPointShot, ItemProperty.LightningSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.HolyPointShot, ItemProperty.PhysicalSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.HolyPointShot, ItemProperty.ColdSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.HolyPointShot, ItemProperty.FireSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.HolyPointShot, ItemProperty.SkillDamage), new ItemStatRange { Max = 60, Min = 80 }},
            { new Tuple<Item, ItemProperty>(Legendary.FirebirdsEye, ItemProperty.FireSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.TheEyeOfTheStorm, ItemProperty.FireSkills), new ItemStatRange { Max = 30, Min = 15 }},
        };

        /// <summary>
        /// Properties that are ALWAYS available to the ItemType are listed here.  
        /// Determines if Property will be available in ItemList rules dropdown.     
        /// </summary>
        public static readonly Dictionary<KeyValuePair<TrinityItemType, ItemProperty>, ItemStatRange> ItemPropertyLimitsByItemType = new Dictionary<KeyValuePair<TrinityItemType, ItemProperty>, ItemStatRange>
        {

            // PrimaryStat

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 750, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 750, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 750, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 750, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 650 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 650 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 650 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shoulder, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Chest, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Cloak, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Belt, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Bracer, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Legs, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Boots, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Dagger, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerEnchantress, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 750, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerScoundrel, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 750, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerTemplar, ItemProperty.PrimaryStat), new ItemStatRange {AncientMax = 1000, AncientMin = 750, Max = 750, Min = 626 }},


            //Vitality

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.Vitality), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 750, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 750, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 750, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 750, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 650 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 650 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 650 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shoulder, ItemProperty.Vitality), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Chest, ItemProperty.Vitality), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Cloak, ItemProperty.Vitality), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Belt, ItemProperty.Vitality), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.Vitality), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Bracer, ItemProperty.Vitality), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Legs, ItemProperty.Vitality), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Boots, ItemProperty.Vitality), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1465, AncientMin = 1237, Max = 1125, Min = 946 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.Vitality), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Dagger, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.Vitality), new ItemStatRange {AncientMax = 650, AncientMin = 550, Max = 500, Min = 416 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerEnchantress, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerScoundrel, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerTemplar, ItemProperty.Vitality), new ItemStatRange {AncientMax = 1000, AncientMin = 825, Max = 750, Min = 626 }},


            //ResistAll

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Chest, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shoulder, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Legs, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Belt, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Bracer, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Boots, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerEnchantress, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerScoundrel, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerTemplar, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},


            // Reduce Damage From Elites

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Chest, ItemProperty.DamageFromElites), new ItemStatRange {Max = 11, Min = 10 }},


            // CriticalDamage

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.CriticalHitDamage), new ItemStatRange { Max = 50, Min = 26 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.CriticalHitDamage), new ItemStatRange { Max = 100, Min = 51 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.CriticalHitDamage), new ItemStatRange { Max = 50, Min = 26 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerEnchantress, ItemProperty.CriticalHitDamage), new ItemStatRange { Max = 100, Min = 51 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerScoundrel, ItemProperty.CriticalHitDamage), new ItemStatRange { Max = 100, Min = 51 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerTemplar, ItemProperty.CriticalHitDamage), new ItemStatRange { Max = 100, Min = 51 }},


            // CriticalChance

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 6, Min = 4.5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 6, Min = 4.5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 6, Min = 4.5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 6, Min = 4.5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 6, Min = 4.5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Bracer, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 6, Min = 4.5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.CriticalHitChance), new ItemStatRange { Max = 10, Min = 8 }},

            // IAS

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.AttackSpeed), new ItemStatRange { Max = 20, Min = 15 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Dagger, ItemProperty.AttackSpeed), new ItemStatRange { Max = 7, Min = 5 }},

            // Skill Damage acdItem.GetSkillDamageIncrease() method is fixed.

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shoulder, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Chest, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Cloak, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Belt, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Legs, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Boots, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},


            // Base Damage

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 600, AncientMin = 400, Max = 500, Min = 340 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 600, AncientMin = 400, Max = 500, Min = 340 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1490, Min = 981 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1490, Min = 981 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1490, Min = 981 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1490, Min = 981 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1304, Min = 856 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1490, Min = 981 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1490, Min = 981 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1490, Min = 981 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1490, Min = 981 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1490, Min = 981 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Dagger, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1490, Min = 981 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 2325, AncientMin = 1582, Max = 1788, Min = 1177 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1788, Min = 1177 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 2325, AncientMin = 1582, Max = 1788, Min = 1177 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 1940, AncientMin = 1318, Max = 1788, Min = 1177 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 2325, AncientMin = 1582, Max = 1788, Min = 1177 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 2325, AncientMin = 1582, Max = 1788, Min = 1177 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 2325, AncientMin = 1582, Max = 1788, Min = 1177 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 2325, AncientMin = 1582, Max = 1788, Min = 1177 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 2325, AncientMin = 1582, Max = 1788, Min = 1177 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.BaseMaxDamage), new ItemStatRange { AncientMax = 2325, AncientMin = 1582, Max = 1788, Min = 1177 }},

            // Percent Damage

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Dagger, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.PercentDamage), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.PercentDamage), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.PercentDamage), new ItemStatRange {  Max = 10, Min = 6 }},

            // Sockets

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Chest, ItemProperty.Sockets), new ItemStatRange { Max = 3, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Cloak, ItemProperty.Sockets), new ItemStatRange { Max = 3, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Legs, ItemProperty.Sockets), new ItemStatRange { Max = 2, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Dagger, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.Sockets), new ItemStatRange { Max = 1, Min = 0 }},
            
            // Resource Cost Reduction

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.ResourceCost), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.ResourceCost), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.ResourceCost), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.ResourceCost), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.ResourceCost), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.ResourceCost), new ItemStatRange {Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shoulder, ItemProperty.ResourceCost), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Dagger, ItemProperty.ResourceCost), new ItemStatRange { Max = 10, Min = 8 }},

            // Cooldown Reduction

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.Cooldown), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.Cooldown), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.Cooldown), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.Cooldown), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.Cooldown), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.Cooldown), new ItemStatRange {Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shoulder, ItemProperty.Cooldown), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Dagger, ItemProperty.Cooldown), new ItemStatRange { Max = 10, Min = 6 }},

            // Area Damage

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.AreaDamage), new ItemStatRange {Min = 10, Max = 20 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.AreaDamage), new ItemStatRange {Min = 10, Max = 20 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.AreaDamage), new ItemStatRange {Min = 10, Max = 20 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.AreaDamage), new ItemStatRange {Min = 10, Max = 20 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.AreaDamage), new ItemStatRange {Min = 10, Max = 20 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.AreaDamage), new ItemStatRange {Min = 10, Max = 20 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shoulder, ItemProperty.AreaDamage), new ItemStatRange {Min = 10, Max = 20 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Dagger, ItemProperty.AreaDamage), new ItemStatRange {Min = 16, Max = 24 }},

            // Thorns

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.Thorns), new ItemStatRange {AncientMax = 9500, AncientMin = 7697, Max = 7696, Min = 5334 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shoulder, ItemProperty.Thorns), new ItemStatRange {AncientMax = 9500, AncientMin = 7697, Max = 7696, Min = 5334 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Chest, ItemProperty.Thorns), new ItemStatRange {AncientMax = 9500, AncientMin = 7697, Max = 7696, Min = 5334 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Cloak, ItemProperty.Thorns), new ItemStatRange {AncientMax = 9500, AncientMin = 7697, Max = 7696, Min = 5334 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Belt, ItemProperty.Thorns), new ItemStatRange {AncientMax = 9500, AncientMin = 7697, Max = 7696, Min = 5334 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.Thorns), new ItemStatRange {AncientMax = 9500, AncientMin = 7697, Max = 7696, Min = 5334 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Bracer, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Legs, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Boots, ItemProperty.Thorns), new ItemStatRange {AncientMax = 3500, AncientMin = 2881, Max = 2880, Min = 2401 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.Thorns), new ItemStatRange {AncientMax = 9500, AncientMin = 7697, Max = 7696, Min = 5334 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.Thorns), new ItemStatRange {AncientMax = 9500, AncientMin = 7697, Max = 7696, Min = 5334 }},

            // Damage Against Elites

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Dagger, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.DamageAgainstElites), new ItemStatRange { Max = 10, Min = 8 }},

            // ElementalDamage

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.ElementalDamage), new ItemStatRange { Max = 20, Min = 15 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Bracer, ItemProperty.ElementalDamage), new ItemStatRange { Max = 20, Min = 15 }},

            // New --

            // Critical Hits Grant Arcane

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.CriticalHitsGrantArcane), new ItemStatRange {  Max = 4, Min = 3 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.CriticalHitsGrantArcane), new ItemStatRange {  Max = 4, Min = 3 }},

            // Armor 

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Chest, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.Armor), new ItemStatRange { AncientMax = 516, AncientMin = 436, Max = 397, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shoulder, ItemProperty.Armor), new ItemStatRange { AncientMax = 516, AncientMin = 436, Max = 397, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Bracer, ItemProperty.Armor), new ItemStatRange { AncientMax = 516, AncientMin = 436, Max = 397, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Belt, ItemProperty.Armor), new ItemStatRange { AncientMax = 516, AncientMin = 436, Max = 397, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Legs, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Boots, ItemProperty.Armor), new ItemStatRange { AncientMax = 516, AncientMin = 436, Max = 397, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.Armor), new ItemStatRange { AncientMax = 516, AncientMin = 436, Max = 397, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.Armor), new ItemStatRange { AncientMax = 516, AncientMin = 436, Max = 397, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Cloak, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerEnchantress, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerScoundrel, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerTemplar, ItemProperty.Armor), new ItemStatRange { AncientMax = 775, AncientMin = 436, Max = 595, Min = 373 }},

            // Block Chance

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.ChanceToBlock), new ItemStatRange {  Max = 11, Min = 11 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.ChanceToBlock), new ItemStatRange {  Max = 11, Min = 11 }},

            // Cooldown Reduction

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.Cooldown), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.Cooldown), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.Cooldown), new ItemStatRange { Max = 8, Min = 5 }},

            // Damage From Elites

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.DamageFromElites), new ItemStatRange {Max = 11, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.DamageFromElites), new ItemStatRange {Max = 11, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Cloak, ItemProperty.DamageFromElites), new ItemStatRange {Max = 11, Min = 10 }},

            // Hatred Regen

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.HatredRegen), new ItemStatRange {  Max = 1.5, Min = 1.35 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Cloak, ItemProperty.HatredRegen), new ItemStatRange {  Max = 1.5, Min = 1.35 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.HatredRegen), new ItemStatRange {  Max = 1.5, Min = 1.35 }},

            // Life Percent

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Chest, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shoulder, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Belt, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Cloak, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.LifePercent), new ItemStatRange {  Max = 15, Min = 10 }},

            // Life Per Fury Spent

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.LifePerFury), new ItemStatRange { AncientMax = 1215, AncientMin = 1034, Max = 936, Min = 788 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.LifePerFury), new ItemStatRange { AncientMax = 2435, AncientMin = 2059, Max = 1872, Min = 1572}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.LifePerFury), new ItemStatRange { AncientMax = 1215, AncientMin = 1034, Max = 936, Min = 788 }},

            // Life Per Spirit Spent

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.LifePerSpirit), new ItemStatRange { AncientMax = 540, AncientMin = 456, Max = 415, Min = 353}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.LifePerSpirit), new ItemStatRange { AncientMax = 1080, AncientMin = 915, Max = 832, Min = 703}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.LifePerSpirit), new ItemStatRange { AncientMax = 540, AncientMin = 456, Max = 415, Min = 353}},

            // Life Per Wrath Spent

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.LifePerWrath), new ItemStatRange { AncientMax = 1660, AncientMin = 1408, Max = 1276, Min = 1077 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.LifePerWrath), new ItemStatRange { AncientMax = 3320, AncientMin = 2810, Max = 2555, Min = 2149}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.LifePerWrath), new ItemStatRange { AncientMax = 1660, AncientMin = 1408, Max = 1276, Min = 1077 }},

            // Life Per Hit

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Dagger, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 35940, AncientMin = 30408, Max = 27644, Min = 23211 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 35940, AncientMin = 30408, Max = 27644, Min = 23211 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 35940, AncientMin = 30408, Max = 27644, Min = 23211 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 35940, AncientMin = 30408, Max = 27644, Min = 23211 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 35940, AncientMin = 30408, Max = 27644, Min = 23211 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 35940, AncientMin = 30408, Max = 27644, Min = 23211 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 35940, AncientMin = 30408, Max = 27644, Min = 23211 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 35940, AncientMin = 30408, Max = 27644, Min = 23211 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 11975, AncientMin = 10135, Max = 9214, Min = 7737 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 11975, AncientMin = 10135, Max = 9214, Min = 7737 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Bracer, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 11975, AncientMin = 10135, Max = 9214, Min = 7737 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 11975, AncientMin = 10135, Max = 9214, Min = 7737 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 11975, AncientMin = 10135, Max = 9214, Min = 7737 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 11975, AncientMin = 10135, Max = 9214, Min = 7737 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 11975, AncientMin = 10135, Max = 9214, Min = 7737 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerEnchantress, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerScoundrel, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FollowerTemplar, ItemProperty.LifePerHit), new ItemStatRange { AncientMax = 23950, AncientMin = 20271, Max = 18429, Min = 15474 }},

            // Life Per Second

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Chest, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 10000, AncientMin = 8445, Max = 7678, Min = 6448}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shoulder, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Helm, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 10000, AncientMin = 8445, Max = 7678, Min = 6448}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 10000, AncientMin = 8445, Max = 7678, Min = 6448}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Bracer, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 10000, AncientMin = 8445, Max = 7678, Min = 6448}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Belt, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Legs, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Boots, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Ring, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 10000, AncientMin = 8445, Max = 7678, Min = 6448}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 10000, AncientMin = 8445, Max = 7678, Min = 6448}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 10000, AncientMin = 8445, Max = 7678, Min = 6448}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Cloak, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 7185, AncientMin = 6080, Max = 5528, Min = 4643}},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.RegenerateLifePerSecond), new ItemStatRange { AncientMax = 10000, AncientMin = 8445, Max = 7678, Min = 6448}},

            // Mana Regen

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.ManaRegen), new ItemStatRange {  Max = 14, Min = 12 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.ManaRegen), new ItemStatRange {  Max = 14, Min = 12 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.ManaRegen), new ItemStatRange {  Max = 14, Min = 12 }},

            // Move Speed

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Boots, ItemProperty.MovementSpeed), new ItemStatRange {  Max = 12, Min = 10 }},

            //// Resistance to All

            //{new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            //{new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            //{new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            //{new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            //{new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            //{new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Cloak, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},
            //{new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.ResistAll), new ItemStatRange {AncientMax = 130, AncientMin = 110, Max = 100, Min = 91 }},

            // Resource Reduction 

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.ResourceCost), new ItemStatRange { Max = 8, Min = 5 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.ResourceCost), new ItemStatRange { Max = 8, Min = 5 }},

            // Spirit Regeneration

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.SpiritRegen), new ItemStatRange {  Max = 3, Min = 2.17 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.SpiritRegen), new ItemStatRange {  Max = 6, Min = 4.33 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.SpiritRegen), new ItemStatRange {  Max = 3, Min = 2.17 }},

            // Wrath Regeneration

            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.WrathRegen), new ItemStatRange {  Max = 2, Min = 1.85 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.WrathRegen), new ItemStatRange {  Max = 4, Min = 3.7 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.WrathRegen), new ItemStatRange {  Max = 2, Min = 1.85 }},

            // Max ArcanePower
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Orb, ItemProperty.MaximumArcane), new ItemStatRange {  Max = 14, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.MaximumArcane), new ItemStatRange {  Max = 14, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.WizardHat, ItemProperty.MaximumArcane), new ItemStatRange {  Max = 14, Min = 10 }},

            // Max Discipline
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Quiver, ItemProperty.MaximumDiscipline), new ItemStatRange {  Max = 12, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.MaximumDiscipline), new ItemStatRange {  Max = 12, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.MaximumDiscipline), new ItemStatRange {  Max = 12, Min = 9 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.MaximumDiscipline), new ItemStatRange {  Max = 12, Min = 9 }},

            // Max Fury
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.MaximumFury), new ItemStatRange {  Max = 12, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.MaximumFury), new ItemStatRange {  Max = 12, Min = 10 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.MaximumFury), new ItemStatRange {  Max = 24, Min = 20 }},

            // Max Mana
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mojo, ItemProperty.MaximumMana), new ItemStatRange {  Max = 120, Min = 150 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.MaximumMana), new ItemStatRange {  Max = 120, Min = 150 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.VoodooMask, ItemProperty.MaximumMana), new ItemStatRange {  Max = 120, Min = 150 }},

            // Max Spirit
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.SpiritStone, ItemProperty.MaximumSpirit), new ItemStatRange {  Max = 15, Min = 13 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.MaximumSpirit), new ItemStatRange {  Max = 15, Min = 13 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.MaximumSpirit), new ItemStatRange {  Max = 30, Min = 26 }},

            // Max Wrath
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.MaximumWrath), new ItemStatRange {  Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.MaximumWrath), new ItemStatRange {  Max = 10, Min = 8 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.MaximumWrath), new ItemStatRange {  Max = 16, Min = 20 }},

            // Chance to Blind
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Amulet, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.ChanceToBlind), new ItemStatRange {  Max = 2.6, Min = 1 }},

            // Chance to Freeze
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Belt, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyBelt, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.ChanceToFreeze), new ItemStatRange {  Max = 2.6, Min = 1 }},

            // Chance to Immobilize
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Boots, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.ChanceToImmobilize), new ItemStatRange {  Max = 2.6, Min = 1 }},

            // Chance to Stun
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Gloves, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandAxe, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandBow, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandCrossbow, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandDaibo, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandFlail, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMace, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandMighty, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandPolearm, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandStaff, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.TwoHandSword, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 5.1, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Axe, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CeremonialKnife, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.FistWeapon, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Flail, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.HandCrossbow, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Mace, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.MightyWeapon, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Spear, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Sword, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 2.6, Min = 1 }},
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Wand, ItemProperty.ChanceToStun), new ItemStatRange {  Max = 2.6, Min = 1 }},

        };

        public static readonly LookupList<KeyValuePair<TrinityItemType, ActorClass>, Skill> SkillDamageByItemTypeAndClass = new LookupList<KeyValuePair<TrinityItemType, ActorClass>, Skill>
        {
            // Head Slot
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.DemonHunter), Skills.DemonHunter.ClusterArrow},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.DemonHunter), Skills.DemonHunter.Multishot},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.DemonHunter), Skills.DemonHunter.ElementalArrow},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.DemonHunter), Skills.DemonHunter.Strafe},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.DemonHunter), Skills.DemonHunter.Chakram},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.DemonHunter), Skills.DemonHunter.RapidFire},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.DemonHunter), Skills.DemonHunter.Impale},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Barbarian), Skills.Barbarian.Whirlwind},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Barbarian), Skills.Barbarian.SeismicSlam},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Barbarian), Skills.Barbarian.AncientSpear},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Barbarian), Skills.Barbarian.HammerOfTheAncients},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Wizard), Skills.Wizard.Meteor},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Wizard), Skills.Wizard.Disintegrate},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Wizard), Skills.Wizard.EnergyTwister},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Wizard), Skills.Wizard.ArcaneTorrent},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Wizard), Skills.Wizard.WaveOfForce},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Wizard), Skills.Wizard.ArcaneOrb},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Wizard), Skills.Wizard.RayOfFrost},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Witchdoctor), Skills.WitchDoctor.AcidCloud},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Witchdoctor), Skills.WitchDoctor.SpiritBarrage},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Witchdoctor), Skills.WitchDoctor.ZombieCharger},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Witchdoctor), Skills.WitchDoctor.Sacrifice},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Witchdoctor), Skills.WitchDoctor.Firebats},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.VoodooMask, ActorClass.Witchdoctor), Skills.WitchDoctor.AcidCloud},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.VoodooMask, ActorClass.Witchdoctor), Skills.WitchDoctor.SpiritBarrage},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.VoodooMask, ActorClass.Witchdoctor), Skills.WitchDoctor.ZombieCharger},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.VoodooMask, ActorClass.Witchdoctor), Skills.WitchDoctor.Sacrifice},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.VoodooMask, ActorClass.Witchdoctor), Skills.WitchDoctor.Firebats},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Monk), Skills.Monk.TempestRush},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Monk), Skills.Monk.ExplodingPalm},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Monk), Skills.Monk.WaveOfLight},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Monk), Skills.Monk.LashingTailKick},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.SpiritStone, ActorClass.Monk), Skills.Monk.TempestRush},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.SpiritStone, ActorClass.Monk), Skills.Monk.ExplodingPalm},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.SpiritStone, ActorClass.Monk), Skills.Monk.WaveOfLight},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.SpiritStone, ActorClass.Monk), Skills.Monk.LashingTailKick},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Crusader), Skills.Crusader.BlessedHammer},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Crusader), Skills.Crusader.Phalanx},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Crusader), Skills.Crusader.FistOfTheHeavens},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Crusader), Skills.Crusader.BlessedShield},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Crusader), Skills.Crusader.ShieldBash},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Helm, ActorClass.Crusader), Skills.Crusader.SweepAttack},

            // Shoulders

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.DemonHunter), Skills.DemonHunter.Companion},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.DemonHunter), Skills.DemonHunter.RainOfVengeance},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.DemonHunter), Skills.DemonHunter.Sentry},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.DemonHunter), Skills.DemonHunter.SpikeTrap},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.DemonHunter), Skills.DemonHunter.FanOfKnives},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Barbarian), Skills.Barbarian.FuriousCharge},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Barbarian), Skills.Barbarian.Earthquake},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Barbarian), Skills.Barbarian.Overpower},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Barbarian), Skills.Barbarian.Rend},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Barbarian), Skills.Barbarian.Revenge},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Barbarian), Skills.Barbarian.CallOfTheAncients},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Wizard), Skills.Wizard.BlackHole},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Wizard), Skills.Wizard.Blizzard},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Wizard), Skills.Wizard.Familiar},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Wizard), Skills.Wizard.ExplosiveBlast},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Wizard), Skills.Wizard.Hydra},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Witchdoctor), Skills.WitchDoctor.FetishArmy},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Witchdoctor), Skills.WitchDoctor.Gargantuan},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Witchdoctor), Skills.WitchDoctor.WallOfDeath},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Witchdoctor), Skills.WitchDoctor.GraspOfTheDead},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Witchdoctor), Skills.WitchDoctor.Haunt},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Witchdoctor), Skills.WitchDoctor.LocustSwarm},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Witchdoctor), Skills.WitchDoctor.Piranhas},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Witchdoctor), Skills.WitchDoctor.SummonZombieDogs},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Monk), Skills.Monk.DashingStrike},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Monk), Skills.Monk.SevensidedStrike},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Monk), Skills.Monk.MysticAlly},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Monk), Skills.Monk.SweepingWind},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Monk), Skills.Monk.CycloneStrike},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Crusader), Skills.Crusader.FallingSword},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Crusader), Skills.Crusader.HeavensFury},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Crusader), Skills.Crusader.Condemn},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shoulder, ActorClass.Crusader), Skills.Crusader.Bombardment},

            // Chest

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.DemonHunter), Skills.DemonHunter.Companion},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.DemonHunter), Skills.DemonHunter.RainOfVengeance},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.DemonHunter), Skills.DemonHunter.Sentry},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.DemonHunter), Skills.DemonHunter.SpikeTrap},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.DemonHunter), Skills.DemonHunter.FanOfKnives},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Cloak, ActorClass.DemonHunter), Skills.DemonHunter.Companion},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Cloak, ActorClass.DemonHunter), Skills.DemonHunter.RainOfVengeance},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Cloak, ActorClass.DemonHunter), Skills.DemonHunter.Sentry},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Cloak, ActorClass.DemonHunter), Skills.DemonHunter.SpikeTrap},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Cloak, ActorClass.DemonHunter), Skills.DemonHunter.FanOfKnives},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Barbarian), Skills.Barbarian.Avalanche},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Barbarian), Skills.Barbarian.Earthquake},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Barbarian), Skills.Barbarian.Overpower},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Barbarian), Skills.Barbarian.Rend},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Barbarian), Skills.Barbarian.Revenge},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Barbarian), Skills.Barbarian.CallOfTheAncients},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Barbarian), Skills.Barbarian.FuriousCharge},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Wizard), Skills.Wizard.BlackHole},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Wizard), Skills.Wizard.Blizzard},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Wizard), Skills.Wizard.Familiar},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Wizard), Skills.Wizard.ExplosiveBlast},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Wizard), Skills.Wizard.Hydra},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Witchdoctor), Skills.WitchDoctor.FetishArmy},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Witchdoctor), Skills.WitchDoctor.Gargantuan},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Witchdoctor), Skills.WitchDoctor.WallOfDeath},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Witchdoctor), Skills.WitchDoctor.GraspOfTheDead},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Witchdoctor), Skills.WitchDoctor.Haunt},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Witchdoctor), Skills.WitchDoctor.LocustSwarm},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Witchdoctor), Skills.WitchDoctor.Piranhas},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Witchdoctor), Skills.WitchDoctor.SummonZombieDogs},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Monk), Skills.Monk.DashingStrike},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Monk), Skills.Monk.SevensidedStrike},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Monk), Skills.Monk.MysticAlly},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Monk), Skills.Monk.SweepingWind},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Monk), Skills.Monk.CycloneStrike},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Crusader), Skills.Crusader.FallingSword},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Crusader), Skills.Crusader.HeavensFury},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Crusader), Skills.Crusader.Bombardment},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Chest, ActorClass.Crusader), Skills.Crusader.Condemn},

            // Belt

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.DemonHunter), Skills.DemonHunter.Grenade},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.DemonHunter), Skills.DemonHunter.EvasiveFire},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.DemonHunter), Skills.DemonHunter.Bolas},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.DemonHunter), Skills.DemonHunter.EntanglingShot},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.DemonHunter), Skills.DemonHunter.HungeringArrow},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Barbarian), Skills.Barbarian.Frenzy},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Barbarian), Skills.Barbarian.Cleave},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Barbarian), Skills.Barbarian.Bash},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Barbarian), Skills.Barbarian.WeaponThrow},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.MightyBelt, ActorClass.Barbarian), Skills.Barbarian.Frenzy},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.MightyBelt, ActorClass.Barbarian), Skills.Barbarian.Cleave},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.MightyBelt, ActorClass.Barbarian), Skills.Barbarian.Bash},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.MightyBelt, ActorClass.Barbarian), Skills.Barbarian.WeaponThrow},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Wizard), Skills.Wizard.Electrocute},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Wizard), Skills.Wizard.SpectralBlade},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Wizard), Skills.Wizard.ShockPulse},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Wizard), Skills.Wizard.MagicMissile},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Witchdoctor), Skills.WitchDoctor.Firebomb},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Witchdoctor), Skills.WitchDoctor.PlagueOfToads},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Witchdoctor), Skills.WitchDoctor.CorpseSpiders},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Witchdoctor), Skills.WitchDoctor.PoisonDart},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Monk), Skills.Monk.WayOfTheHundredFists},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Monk), Skills.Monk.CripplingWave},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Monk), Skills.Monk.DeadlyReach},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Monk), Skills.Monk.FistsOfThunder},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Belt, ActorClass.Crusader), Skills.Crusader.Punish},

            // Pants

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.DemonHunter), Skills.DemonHunter.Grenade},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.DemonHunter), Skills.DemonHunter.EvasiveFire},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.DemonHunter), Skills.DemonHunter.Bolas},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.DemonHunter), Skills.DemonHunter.EntanglingShot},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.DemonHunter), Skills.DemonHunter.HungeringArrow},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Barbarian), Skills.Barbarian.Frenzy},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Barbarian), Skills.Barbarian.Cleave},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Barbarian), Skills.Barbarian.Bash},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Barbarian), Skills.Barbarian.WeaponThrow},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Wizard), Skills.Wizard.Electrocute},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Wizard), Skills.Wizard.SpectralBlade},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Wizard), Skills.Wizard.ShockPulse},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Wizard), Skills.Wizard.MagicMissile},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Witchdoctor), Skills.WitchDoctor.Firebomb},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Witchdoctor), Skills.WitchDoctor.PlagueOfToads},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Witchdoctor), Skills.WitchDoctor.CorpseSpiders},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Witchdoctor), Skills.WitchDoctor.PoisonDart},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Monk), Skills.Monk.WayOfTheHundredFists},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Monk), Skills.Monk.CripplingWave},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Monk), Skills.Monk.DeadlyReach},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Monk), Skills.Monk.FistsOfThunder},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Crusader), Skills.Crusader.Punish},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Crusader), Skills.Crusader.Justice},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Crusader), Skills.Crusader.Slash},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Legs, ActorClass.Crusader), Skills.Crusader.Smite},
            
            // Boots

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.DemonHunter), Skills.DemonHunter.ClusterArrow},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.DemonHunter), Skills.DemonHunter.Multishot},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.DemonHunter), Skills.DemonHunter.ElementalArrow},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.DemonHunter), Skills.DemonHunter.Strafe},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.DemonHunter), Skills.DemonHunter.Chakram},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.DemonHunter), Skills.DemonHunter.RapidFire},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.DemonHunter), Skills.DemonHunter.Impale},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Barbarian), Skills.Barbarian.Whirlwind},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Barbarian), Skills.Barbarian.SeismicSlam},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Barbarian), Skills.Barbarian.AncientSpear},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Barbarian), Skills.Barbarian.HammerOfTheAncients},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Wizard), Skills.Wizard.Meteor},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Wizard), Skills.Wizard.Disintegrate},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Wizard), Skills.Wizard.EnergyTwister},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Wizard), Skills.Wizard.ArcaneTorrent},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Wizard), Skills.Wizard.WaveOfForce},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Wizard), Skills.Wizard.ArcaneOrb},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Wizard), Skills.Wizard.RayOfFrost},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Witchdoctor), Skills.WitchDoctor.AcidCloud},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Witchdoctor), Skills.WitchDoctor.SpiritBarrage},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Witchdoctor), Skills.WitchDoctor.ZombieCharger},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Witchdoctor), Skills.WitchDoctor.Sacrifice},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Witchdoctor), Skills.WitchDoctor.Firebats},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Monk), Skills.Monk.TempestRush},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Monk), Skills.Monk.ExplodingPalm},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Monk), Skills.Monk.WaveOfLight},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Monk), Skills.Monk.LashingTailKick},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Crusader), Skills.Crusader.BlessedHammer},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Crusader), Skills.Crusader.Phalanx},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Crusader), Skills.Crusader.FistOfTheHeavens},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Crusader), Skills.Crusader.BlessedShield},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Crusader), Skills.Crusader.ShieldBash},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Boots, ActorClass.Crusader), Skills.Crusader.SweepAttack},

            // Offhand

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Quiver, ActorClass.DemonHunter), Skills.DemonHunter.ClusterArrow},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Quiver, ActorClass.DemonHunter), Skills.DemonHunter.Multishot},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Quiver, ActorClass.DemonHunter), Skills.DemonHunter.ElementalArrow},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Quiver, ActorClass.DemonHunter), Skills.DemonHunter.Strafe},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Quiver, ActorClass.DemonHunter), Skills.DemonHunter.Chakram},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Quiver, ActorClass.DemonHunter), Skills.DemonHunter.RapidFire},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Quiver, ActorClass.DemonHunter), Skills.DemonHunter.Impale},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Quiver, ActorClass.DemonHunter), Skills.DemonHunter.Sentry},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Quiver, ActorClass.DemonHunter), Skills.DemonHunter.RainOfVengeance},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Quiver, ActorClass.DemonHunter), Skills.DemonHunter.FanOfKnives},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.EnergyTwister},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.Electrocute},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.ArcaneOrb},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.Hydra},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.MagicMissile},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.Familiar},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.RayOfFrost},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.Meteor},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.SpectralBlade},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.WaveOfForce},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.Disintegrate},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.ArcaneTorrent},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.ExplosiveBlast},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.Blizzard},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Orb, ActorClass.Wizard), Skills.Wizard.BlackHole},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.SpiritBarrage},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.LocustSwarm},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.PlagueOfToads},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.Haunt},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.Firebats},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.Firebomb},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.GraspOfTheDead},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.Sacrifice},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.FetishArmy},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.ZombieCharger},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.CorpseSpiders},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.AcidCloud},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.PoisonDart},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.WallOfDeath},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.SummonZombieDogs},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.Piranhas},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.Gargantuan},


            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.FallingSword},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.Condemn},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.Justice},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.FistOfTheHeavens},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.Bombardment},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.HeavensFury},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.Phalanx},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.BlessedShield},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.Slash},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.ShieldBash},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.Smite},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.SweepAttack},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.Punish},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.CrusaderShield, ActorClass.Crusader), Skills.Crusader.BlessedHammer},

            // One Hand Weapon

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.MightyWeapon, ActorClass.Barbarian), Skills.Barbarian.WeaponThrow},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.MightyWeapon, ActorClass.Barbarian), Skills.Barbarian.AncientSpear},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Wand, ActorClass.Wizard), Skills.Wizard.Disintegrate},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Wand, ActorClass.Wizard), Skills.Wizard.SpectralBlade},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Flail, ActorClass.Crusader), Skills.Crusader.BlessedHammer},

            // Two Hand Weapon

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.Whirlwind},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.SeismicSlam},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.AncientSpear},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.HammerOfTheAncients},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.Avalanche},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.Earthquake},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.Overpower},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.Rend},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.Revenge},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.Frenzy},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.Cleave},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.Bash},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.WeaponThrow},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.CallOfTheAncients},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandMighty, ActorClass.Barbarian), Skills.Barbarian.FuriousCharge},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.SevensidedStrike},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.LashingTailKick},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.WaveOfLight},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.ExplodingPalm},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.TempestRush},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.FistsOfThunder},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.CripplingWave},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.WayOfTheHundredFists},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.DeadlyReach},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.DashingStrike},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.MysticAlly},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.SweepingWind},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.TwoHandDaibo, ActorClass.Monk), Skills.Monk.CycloneStrike},


        };



    }
}
