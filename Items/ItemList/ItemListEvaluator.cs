using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Reference;
using Trinity.Technicals;
using Trinity.UI.UIComponents;
using Trinity.UIComponents;
using Zeta.Game.Internals.Actors;

namespace Trinity.Items.ItemList
{
    public class ItemListEvaluator
    {
        internal static bool ShouldStashItem(TrinityItem cItem, bool test = false)
        {
            if (ShouldStashItemType(cItem, test))
            {
                return true;
            }

            var item = Legendary.GetItem(cItem);
            if (item == null)
            {
                Logger.LogVerbose("  >>  Unknown Item {0} {1} - Auto-keeping", cItem.Name, cItem.ActorSnoId);
                return true;
            }

            return ShouldStashItem(item, cItem, test);
        }

        internal static bool ShouldStashItemType(TrinityItem cItem, bool test = false)
        {
            var typeEntry = Core.Settings.Loot.ItemList.GetitemTypeRule(cItem.TrinityItemType);

            if (typeEntry == null)
            {
                Logger.LogVerbose($"  >> {cItem.Name} did not match any item types");
                return false;
            }

            if (!typeEntry.IsSelected)
            {
                Logger.LogVerbose($"  >>  {cItem.Name} ({cItem.TrinityItemType}) is not a selected {typeEntry.Type}");
                return false;
            }

            return typeEntry.IsSelected && EvaluateRules(cItem, typeEntry, test);
        }

        internal static bool ShouldStashItem(Item referenceItem, TrinityItem cItem, bool test = false)
        {
            var id = referenceItem.Id;
            var logLevel = test ? TrinityLogLevel.Info : TrinityLogLevel.Debug;

            if (cItem.IsCrafted)
            {
                Logger.Log(logLevel, "  >>  Crafted Item {0} {1} - Auto-keeping", cItem.Name, id);
                return true;
            }

            if (test)
            {
                var props = ItemDataUtils.GetPropertiesForItem(referenceItem);

                Logger.LogVerbose($"------- Starting Test of {props.Count} supported properties for {cItem.Name}");

                foreach (var prop in props)
                {
                    if (prop == ItemProperty.Attribute)
                        continue;

                    var range = ItemDataUtils.GetItemStatRange(referenceItem, prop);
                    float newValue;

                    var testrule = new LRule(prop)
                    {
                        Value = (float) range.AncientMax,
                    };

                    EvaluateProperty(testrule, cItem, out newValue);
                }

                Logger.LogVerbose("------- Finished Test for {0} against max value", cItem.Name);
            }

            var itemSetting = Core.Settings.Loot.ItemList.SelectedItems.FirstOrDefault(i => referenceItem.Id == i.Id);
            if (itemSetting != null)
            {
                return EvaluateRules(cItem, itemSetting, test);
            }

            Logger.Log($"  >>  Unselected ListItem {cItem.Name} {cItem.ActorSnoId} GbId={cItem.GameBalanceId} IsValid={cItem.IsValid}");
            return false;
        }

        private static bool EvaluateRules(TrinityItem cItem, LItem itemSetting, bool isTest)
        {
            if (itemSetting == null)
            {
                Logger.LogError("Null ItemSetting");
                return false;
            }

            if (cItem == null)
            {
                Logger.LogError("Null TrinityItem");
                return false;
            }

            var typeName = itemSetting.Type == LItem.ILType.Slot ? $"({itemSetting.Name}) " : string.Empty;
            Logger.LogVerbose($"  >>  {cItem.Name} ({itemSetting.Id}) is a selected {itemSetting.Type} {typeName}with {itemSetting.Rules.Count} rules.");

            if (itemSetting.RequiredRules.Any())
            {
                Logger.LogVerbose("  >>  {0} required rules:", itemSetting.RequiredRules.Count);
            }

            var ruleUpgrades = new Dictionary<LRule, float>();
            float newValue;

            // If any of the required rules are false, trash.
            foreach (var itemRule in itemSetting.RequiredRules)
            {                
                if (!EvaluateProperty(itemRule, cItem, out newValue))
                {
                    Logger.LogVerbose($"  >>  Not stashing because of required rule failure: {itemRule.Name}");
                    return false;
                }

                if (itemSetting.Type != LItem.ILType.Slot)
                {
                    ruleUpgrades.Add(itemRule, newValue);
                }
            }

            if (!itemSetting.OptionalRules.Any())
            {
                UpgradeRules(ruleUpgrades);
                return true;
            }

            Logger.LogVerbose($"  >>  item must have {itemSetting.Ops} of {itemSetting.OptionalRules.Count} optional rules:");

            // X optional rules must be true.
            var trueOptionals = 0;
            foreach (var itemRule in itemSetting.OptionalRules)
            {
                if (EvaluateProperty(itemRule, cItem, out newValue))
                {
                    trueOptionals++;

                    if (itemSetting.Type != LItem.ILType.Slot)
                    {
                        ruleUpgrades.Add(itemRule, newValue);
                    }
                }                
            }

            if (trueOptionals >= itemSetting.Ops)
            {
                UpgradeRules(ruleUpgrades);
                return true;
            }

            return false;
        }

        private static void UpgradeRules(Dictionary<LRule, float> ruleUpgrades)
        {
            if (!Core.Settings.Loot.ItemList.UpgradeRules)
                return;

            foreach (var pair in ruleUpgrades)
            {
                Logger.Log($"Upgraded Rule {pair.Key.Name} from {pair.Key.Value} to {pair.Value}");
                pair.Key.Value = pair.Value;
            }
        }

        internal static bool EvaluateProperty(LRule itemRule, TrinityItem item, out float newValue)
        {
            var prop = itemRule.ItemProperty;            
            var value = (float) itemRule.Value;
            var variant = itemRule.Variant;

            var result = false;
            string friendlyVariant = string.Empty;
            float itemValue = 0;
            float ruleValue = 0;
            float returnValue = -1;

            switch (prop)
            {
                case ItemProperty.PassivePower:
                    itemValue = ItemDataUtils.GetPassivePowerValue(item);
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.Ancient:
                    itemValue = item.IsAncient ? 1 : 0;
                    ruleValue = value;
                    result = item.IsAncient && Math.Abs(value - 1) < double.Epsilon;
                    returnValue = ruleValue;
                    break;

                case ItemProperty.PrimaryStat:
                    itemValue = item.Attributes.PrimaryStat;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.CriticalHitChance:
                    itemValue = item.Attributes.CritPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.CriticalHitDamage:
                    itemValue = item.Attributes.CritDamagePercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.AttackSpeed:
                    if(item.ItemBaseType == ItemBaseType.Armor)
                        itemValue = item.Attributes.AttacksPerSecondPercent;
                    else
                        itemValue = item.Attributes.AttacksPerSecondItemPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ResourceCost:
                    itemValue = item.Attributes.ResourceCostReductionPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.Cooldown:
                    itemValue = item.Attributes.CooldownPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ResistAll:
                    itemValue = item.Attributes.ResistAll;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.Sockets:
                    itemValue = item.Attributes.Sockets;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = ruleValue;
                    break;

                case ItemProperty.Vitality:
                    itemValue = item.Attributes.Vitality;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.AreaDamage:
                    itemValue = item.Attributes.AreaDamagePercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.Thorns:
                    itemValue = item.Attributes.Thorns;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.FireSkills:
                    itemValue = item.Attributes.FireSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ColdSkills:
                    itemValue = item.Attributes.ColdSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LightningSkills:
                    itemValue = item.Attributes.LightningSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ArcaneSkills:
                    itemValue = item.Attributes.ArcaneSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.HolySkills:
                    itemValue = item.Attributes.HolySkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.PoisonSkills:
                    itemValue = item.Attributes.PoisonSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.PhysicalSkills:
                    itemValue = item.Attributes.PhysicalSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.DamageAgainstElites:
                    itemValue = item.Attributes.DamageAgainstElites;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.DamageFromElites:
                    itemValue = item.Attributes.DamageFromElites;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.BaseMaxDamage:
                    itemValue = item.Attributes.MaxDamage;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.SkillDamage:

                    var skillId = variant;
                    var skill = ItemDataUtils.GetSkillsForItemType(item.TrinityItemType, Core.Player.ActorClass).FirstOrDefault(s => s.Id == skillId);
                    if (skill != null)
                    {
                        friendlyVariant = skill.Name;
                        itemValue = item.Attributes.SkillDamagePercent(skill.SNOPower) *100;
                    }

                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ElementalDamage:
                    var elementId = variant;
                    var element = (Element)elementId;
                    if (element != Element.Unknown)
                    {
                        var damageType = TypeConversions.GetDamageType(element);
                        friendlyVariant = ((EnumValue<Element>)element).Name;
                        itemValue = item.Attributes.GetElementalDamage(damageType);
                    }

                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.PercentDamage:
                    itemValue = item.Attributes.WeaponDamagePercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.CriticalHitsGrantArcane:
                    itemValue = item.Attributes.ArcaneOnCrit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.Armor:
                    itemValue = item.Attributes.ArmorBonusItem;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToBlock:
                    itemValue = item.Attributes.BlockChanceBonusPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToBlockTotal:
                    itemValue = item.Attributes.BlockChanceItemTotal;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.HatredRegen:
                    itemValue = item.Attributes.HatredRegen;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LifePercent:
                    itemValue = item.Attributes.LifePercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LifePerHit:
                    itemValue = item.Attributes.LifeOnHit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.RegenerateLifePerSecond:
                    itemValue = item.Attributes.HealthPerSecond;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ManaRegen:
                    itemValue = item.Attributes.ManaRegen;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MovementSpeed:
                    itemValue = item.Attributes.MovementSpeedPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.SpiritRegen:
                    itemValue = item.Attributes.SpiritRegen;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.WrathRegen:
                    itemValue =
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LifePerFury:
                    itemValue = item.Attributes.LifePerFury;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LifePerSpirit:
                    itemValue = item.Attributes.LifePerSpirit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LifePerWrath:
                    itemValue = item.Attributes.LifePerWrath;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumArcane:
                    itemValue = item.Attributes.MaxArcanePower;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumSpirit:
                    itemValue = item.Attributes.MaxSpirit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumDiscipline:
                    itemValue = item.Attributes.MaxDiscipline;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumFury:
                    itemValue = item.Attributes.MaxFury;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumMana:
                    itemValue = item.Attributes.MaxMana;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumWrath:
                    itemValue = item.Attributes.MaximumWrath;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToBlind:
                    itemValue = item.Attributes.ChanceToBlind;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToFreeze:
                    itemValue = item.Attributes.ChanceToFreeze;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToImmobilize:
                    itemValue = item.Attributes.ChanceToImmobilize;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToStun:
                    itemValue = item.Attributes.ChanceToStun;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.Attribute:

                    try
                    {
                        friendlyVariant = $"{itemRule.AttributeKey} {itemRule.AttributeModifier} {itemRule.AttributeValue}";

                        var error = string.Empty;
                        var key = itemRule.AttributeKey.Trim();
                        var mod = itemRule.AttributeModifier.Trim();
                        var val = itemRule.AttributeValue.Trim();

                        ActorAttributeType attribute;
                        if (!Enum.TryParse(key, true, out attribute))
                            error += $"No ActorAttributeType exists with key '{itemRule.AttributeKey}'. ";

                        var modifierId = -1;
                        if (!string.IsNullOrEmpty(mod))
                        {
                            if (!int.TryParse(mod, out modifierId))
                                error += $"Modifier '{itemRule.AttributeModifier}' is not a number. ";
                        }
                                      
                        if (!float.TryParse(val, out value))
                            error += $"Value '{itemRule.AttributeModifier}' is not a number. ";

                        if (!string.IsNullOrEmpty(error))
                        {
                            Logger.Warn($"Attribute specified in ItemList is invalid. {friendlyVariant} - {error}");
                            break;
                        }
                        
                        itemValue = item.Attributes.GetCachedAttribute<float>(attribute, modifierId);
                        ruleValue = value;
                        result = itemValue >= ruleValue;
                        returnValue = itemValue;

                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Exception evaluating ItemList rule Attribute {ex}");
                    }
                    break;
            }

            Logger.LogVerbose($"  >>  Evaluated {item.Name} -- {prop.ToString().AddSpacesToSentence()} {friendlyVariant} (Item: {itemValue} -v- Rule: {ruleValue}) = {result}");
            newValue = returnValue;
            return result;
        }



    }
}

