using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Modules;
using Zeta.Game.Internals.Actors;

namespace Trinity.Settings.ItemList
{
    public class ItemListEvaluator
    {
        internal static bool ShouldStashItem(ACDItem cItem, bool test = false)
        {
            if (ShouldStashItemType(cItem, test))
            {
                return true;
            }

            var item = Legendary.GetItem(cItem);
            if (item == null)
            {
                Core.Logger.Verbose("  >>  Unknown Item {0} {1} - Auto-keeping", cItem.Name, cItem.ActorSnoId);
                return true;
            }

            return ShouldStashItem(item, cItem, test);
        }

        internal static bool ShouldStashItemType(ACDItem cItem, bool test = false)
        {
            var typeEntry = Core.Settings.ItemList.GetitemTypeRule(cItem.GetTrinityItemType());

            if (typeEntry == null)
            {
                Core.Logger.Verbose($"  >> {cItem.Name} did not match any item types");
                return false;
            }

            if (!typeEntry.IsSelected)
            {
                Core.Logger.Verbose($"  >>  {cItem.Name} ({cItem.GetTrinityItemType()}) is not a selected item type - {typeEntry.Type}");
                return false;
            }

            return typeEntry.IsSelected && EvaluateRules(cItem, typeEntry, test);
        }

        internal static bool ShouldStashItem(Item referenceItem, ACDItem cItem, bool test = false)
        {
            var id = referenceItem.Id;

            if (cItem.IsCrafted)
            {
                Core.Logger.Debug("  >>  Crafted Item {0} {1} - Auto-keeping", cItem.Name, id);
                return true;
            }

            if (test)
            {
                var props = ItemDataUtils.GetPropertiesForItem(referenceItem);

                Core.Logger.Verbose($"------- Starting Test of {props.Count} supported properties for {cItem.Name}");

                foreach (var prop in props)
                {
                    if (prop == ItemProperty.Attribute)
                        continue;

                    var range = ItemDataUtils.GetItemStatRange(referenceItem, prop);
                    float newValue;

                    var testrule = new LRule(prop)
                    {
                        Value = (float)range.AncientMax,
                    };

                    EvaluateProperty(testrule, cItem, out newValue);
                }

                Core.Logger.Verbose("------- Finished Test for {0} against max value", cItem.Name);
            }

            var itemSetting = Core.Settings.ItemList.SelectedItems.FirstOrDefault(i => referenceItem.Id == i.Id);
            if (itemSetting != null)
            {
                return EvaluateRules(cItem, itemSetting, test);
            }

            Core.Logger.Log($"  >>  Unselected ListItem {cItem.Name} {cItem.ActorSnoId} GbId={cItem.GameBalanceId} IsValid={cItem.IsValid}");
            return false;
        }

        private static bool EvaluateRules(ACDItem cItem, LItem itemSetting, bool isTest)
        {
            if (itemSetting == null)
            {
                Core.Logger.Error("Null ItemSetting");
                return false;
            }

            if (cItem == null)
            {
                Core.Logger.Error("Null TrinityItem");
                return false;
            }

            var typeName = itemSetting.Type == LItem.ILType.Slot ? $"({itemSetting.Name}) " : string.Empty;
            Core.Logger.Verbose($"  >>  {cItem.Name} ({itemSetting.Id}) is a selected {itemSetting.Type} {typeName}with {itemSetting.Rules.Count} rules.");

            if (itemSetting.RequiredRules.Any())
            {
                Core.Logger.Verbose("  >>  {0} required rules:", itemSetting.RequiredRules.Count);
            }

            var ruleUpgrades = new Dictionary<LRule, float>();
            float newValue;

            // If any of the required rules are false, trash.
            foreach (var itemRule in itemSetting.RequiredRules)
            {
                if (!EvaluateProperty(itemRule, cItem, out newValue))
                {
                    Core.Logger.Verbose($"  >>  Not stashing because of required rule failure: {itemRule.Name}");
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

            Core.Logger.Verbose($"  >>  item must have {itemSetting.Ops} of {itemSetting.OptionalRules.Count} optional rules:");

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
            if (!Core.Settings.ItemList.UpgradeRules)
                return;

            foreach (var pair in ruleUpgrades)
            {
                Core.Logger.Log($"Upgraded Rule {pair.Key.Name} from {pair.Key.Value} to {pair.Value}");
                pair.Key.Value = pair.Value;
            }
        }

        internal static bool EvaluateProperty(LRule itemRule, ACDItem item, out float newValue)
        {
            var prop = itemRule.ItemProperty;
            var value = (float)itemRule.Value;
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
                    itemValue = item.Stats.IsAncient ? 1 : 0;
                    ruleValue = value;
                    result = item.Stats.IsAncient && Math.Abs(value - 1) < double.Epsilon;
                    returnValue = ruleValue;
                    break;

                case ItemProperty.PrimaryStat:
                    itemValue = item.Stats.HighestPrimaryAttribute;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.CriticalHitChance:
                    itemValue = item.Stats.CritPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.CriticalHitDamage:
                    itemValue = item.Stats.CritDamagePercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.AttackSpeed:
                    itemValue = item.Stats.AttackSpeedPercent;
                    // TODO: Check if this is required.
                    if (Math.Abs(itemValue) < double.Epsilon)
                    {
                        itemValue = item.AttacksPerSecondItemPercent;
                    }
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ResourceCost:
                    itemValue = item.Stats.ResourceCostReductionPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.Cooldown:
                    itemValue = item.Stats.PowerCooldownReductionPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ResistAll:
                    itemValue = item.Stats.ResistAll;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.Sockets:
                    itemValue = item.Stats.Sockets;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = ruleValue;
                    break;

                case ItemProperty.Vitality:
                    itemValue = item.Stats.Vitality;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.AreaDamage:
                    itemValue = item.SplashDamageEffectPercent * 100f;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.Thorns:
                    itemValue = item.Stats.Thorns;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.FireSkills:
                    itemValue = item.Stats.FireSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ColdSkills:
                    itemValue = item.Stats.ColdSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LightningSkills:
                    itemValue = item.Stats.LightningSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ArcaneSkills:
                    itemValue = item.Stats.ArcaneSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.HolySkills:
                    itemValue = item.Stats.HolySkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.PoisonSkills:
                    itemValue = item.Stats.PosionSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.PhysicalSkills:
                    itemValue = item.Stats.PhysicalSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.DamageAgainstElites:
                    itemValue = item.Stats.DamagePercentBonusVsElites;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.DamageFromElites:
                    itemValue = item.Stats.DamagePercentReductionFromElites;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.BaseMaxDamage:
                    itemValue = item.Stats.MaxDamage;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.SkillDamage:
                    var skillId = variant;
                    var skill = ItemDataUtils.GetSkillsForItemType(item.GetTrinityItemType(), Core.Player.ActorClass).FirstOrDefault(s => s.Id == skillId);
                    if (skill != null)
                    {
                        friendlyVariant = skill.Name;
                        itemValue = item.SkillDamagePercent(skill.SNOPower) * 100;
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
                        itemValue = item.GetElementalDamage(damageType);
                    }
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.PercentDamage:
                    itemValue = item.Stats.WeaponDamagePercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.CriticalHitsGrantArcane:
                    itemValue = item.Stats.ArcaneOnCrit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.Armor:
                    itemValue = item.Stats.ArmorBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToBlock:
                    itemValue = item.Stats.BlockChanceBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToBlockTotal:
                    itemValue = item.Stats.BlockChance;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.HatredRegen:
                    itemValue = item.Stats.HatredRegen;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LifePercent:
                    itemValue = item.Stats.LifePercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LifePerHit:
                    itemValue = item.Stats.LifeOnHit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.RegenerateLifePerSecond:
                    itemValue = item.Stats.HealthPerSecond;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ManaRegen:
                    itemValue = item.Stats.ManaRegen;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MovementSpeed:
                    itemValue = item.Stats.MovementSpeed;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.SpiritRegen:
                    itemValue = item.Stats.SpiritRegen;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.WrathRegen:
                    itemValue = item.Stats.FaithRegen;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LifePerFury:
                    itemValue = item.Stats.HealPerFury;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LifePerSpirit:
                    itemValue = item.Stats.HealPerSpirit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.LifePerWrath:
                    itemValue = item.Stats.HealPerFaith;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumArcane:
                    itemValue = item.Stats.MaxArcanum;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumSpirit:
                    itemValue = item.Stats.MaxSpirit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumDiscipline:
                    itemValue = item.Stats.MaxDiscipline;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumFury:
                    itemValue = item.Stats.MaxFury;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumMana:
                    itemValue = item.Stats.MaxMana;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumWrath:
                    itemValue = item.Stats.MaxFaith;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToBlind:
                    itemValue = item.Stats.WeaponOnHitBlindProcChance;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToFreeze:
                    itemValue = item.Stats.WeaponOnHitFreezeProcChance;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToImmobilize:
                    itemValue = item.Stats.WeaponOnHitImmobilizeProcChance;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.ChanceToStun:
                    itemValue = item.Stats.WeaponOnHitStunProcChance;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.MaximumEssence:
                    itemValue = item.ResourceMaxEssence;
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

                        if (!Enum.TryParse(key, true, out ActorAttributeType attribute))
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
                            Core.Logger.Warn($"Attribute specified in ItemList is invalid. {friendlyVariant} - {error}");
                            break;
                        }

                        itemValue = item.GetAttribute<float>(attribute, modifierId);
                        ruleValue = value;
                        result = itemValue >= ruleValue;
                        returnValue = itemValue;

                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Error($"Exception evaluating ItemList rule Attribute {ex}");
                    }
                    break;
            }

            Core.Logger.Verbose($"  >>  Evaluated {item.Name} -- {prop.ToString().AddSpacesToSentence()} {friendlyVariant} (Item: {itemValue} -v- Rule: {ruleValue}) = {result}");
            newValue = returnValue;
            return result;
        }
    }
}
