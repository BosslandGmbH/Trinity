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

            if(prop == ItemProperty.Ancient)
                return new ItemStatRange { Max = 1, Min = 0};

            if (ItemPropertyLimitsByItemType.TryGetValue(new KeyValuePair<TrinityItemType, ItemProperty>(item.TrinityItemType,prop), out statRange))
                result = statRange;

            if (SpecialItemsPropertyCases.TryGetValue(new Tuple<Item, ItemProperty>(item, prop), out statRange))
                result = statRange;

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

            foreach(var pair in SpecialItemsPropertyCases.Where(i => i.Key.Item1.TrinityItemType == itemType && i.Key.Item2 == prop))
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
            props.Add(ItemProperty.Ancient);
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

        /// <summary>
        /// Items with unusual properties are listed here
        /// Determines if Property will be available in ItemList rules dropdown.
        /// </summary>
        public static readonly Dictionary<Tuple<Item,ItemProperty>, ItemStatRange> SpecialItemsPropertyCases = new Dictionary<Tuple<Item,ItemProperty>, ItemStatRange>
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
            { new Tuple<Item, ItemProperty>(Legendary.HeartSlaughter, ItemProperty.PhysicalSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.SchaefersHammer, ItemProperty.LightningSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.WrathOfTheBoneKing, ItemProperty.ColdSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.WonKhimLau, ItemProperty.LightningSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.Azurewrath, ItemProperty.ColdSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.Doombringer, ItemProperty.PhysicalSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.AkaneshTheHeraldOfRighteousness, ItemProperty.HolySkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.OdynSon, ItemProperty.LightningSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.HolyPointShot, ItemProperty.LightningSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.HolyPointShot, ItemProperty.PhysicalSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.HolyPointShot, ItemProperty.ColdSkills), new ItemStatRange { Max = 20, Min = 15 }},
            { new Tuple<Item, ItemProperty>(Legendary.HolyPointShot, ItemProperty.FireSkills), new ItemStatRange { Max = 20, Min = 15 }},
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
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.Shield, ItemProperty.SkillDamage), new ItemStatRange { Max = 15, Min = 10 }},


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
            {new KeyValuePair<TrinityItemType, ItemProperty>(TrinityItemType.CrusaderShield, ItemProperty.AreaDamage), new ItemStatRange {Min = 10, Max = 20 }},
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
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.GraspOfTheDead},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.Sacrifice},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.FetishArmy},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.ZombieCharger},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.CorpseSpiders},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.AcidCloud},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.PoisonDart},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Mojo, ActorClass.Witchdoctor), Skills.WitchDoctor.WallOfDeath},

            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.FallingSword},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.Condemn},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.Justice},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.FistOfTheHeavens},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.Bombardment},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.HeavensFury},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.Phalanx},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.BlessedShield},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.Slash},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.ShieldBash},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.Smite},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.SweepAttack},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.Punish},
            {new KeyValuePair<TrinityItemType, ActorClass>(TrinityItemType.Shield, ActorClass.Crusader), Skills.Crusader.BlessedHammer},

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
