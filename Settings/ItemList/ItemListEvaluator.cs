using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Game.Internals.Actors;

namespace Trinity.Settings.ItemList
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
                Core.Logger.Verbose("  >>  未知 道具 {0} {1} - 自动保存", cItem.Name, cItem.ActorSnoId);
                return true;
            }

            return ShouldStashItem(item, cItem, test);
        }

        internal static bool ShouldStashItemType(TrinityItem cItem, bool test = false)
        {
            var typeEntry = Core.Settings.ItemList.GetitemTypeRule(cItem.TrinityItemType);

            if (typeEntry == null)
            {
                Core.Logger.Verbose($"  >> {cItem.Name} 不匹配任何物品类型");
                return false;
            }

            if (!typeEntry.IsSelected)
            {
                Core.Logger.Verbose($"  >>  {cItem.Name} ({cItem.TrinityItemType}) is not a selected item type - {typeEntry.Type}");
                return false;
            }

            return typeEntry.IsSelected && EvaluateRules(cItem, typeEntry, test);
        }

        internal static bool ShouldStashItem(Item referenceItem, TrinityItem cItem, bool test = false)
        {
            var id = referenceItem.Id;

            if (cItem.IsCrafted)
            {
                Core.Logger.Debug("  >>  特别 道具 {0} {1} - 自动保存", cItem.Name, id);
                return true;
            }

            if (test)
            {
                var props = ItemDataUtils.GetPropertiesForItem(referenceItem);

                Core.Logger.Verbose($"------- 开始测试的 {props.Count} 支持特性 {cItem.Name}");

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

                Core.Logger.Verbose("------- 已完成对 {0} 的最大值测试", cItem.Name);
            }

            var itemSetting = Core.Settings.ItemList.SelectedItems.FirstOrDefault(i => referenceItem.Id == i.Id);
            if (itemSetting != null)
            {
                return EvaluateRules(cItem, itemSetting, test);
            }

            Core.Logger.Log($"  >>  捡取列表未选中的 {cItem.Name} {cItem.ActorSnoId} 游戏的平衡性={cItem.GameBalanceId} 是否有效={cItem.IsValid}");
            return false;
        }

        private static bool EvaluateRules(TrinityItem cItem, LItem itemSetting, bool isTest)
        {
            if (itemSetting == null)
            {
                Core.Logger.Error("无效的物品设置");
                return false;
            }

            if (cItem == null)
            {
                Core.Logger.Error("无效的 Trinity物品");
                return false;
            }

            var typeName = itemSetting.Type == LItem.ILType.Slot ? $"({itemSetting.Name}) " : string.Empty;
            Core.Logger.Verbose($"  >>  {cItem.Name} ({itemSetting.Id}) 是选定 {itemSetting.Type} {typeName}与 {itemSetting.Rules.Count} 规则.");

            if (itemSetting.RequiredRules.Any())
            {
                Core.Logger.Verbose("  >>  {0} 所需的规则:", itemSetting.RequiredRules.Count);
            }

            var ruleUpgrades = new Dictionary<LRule, float>();
            float newValue;

            // If any of the required rules are false, trash.
            foreach (var itemRule in itemSetting.RequiredRules)
            {
                if (!EvaluateProperty(itemRule, cItem, out newValue))
                {
                    Core.Logger.Verbose($"  >>  由于所需的规则失败，不会锁定: {itemRule.Name}");
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

            Core.Logger.Verbose($"  >>  物品必须具有 {itemSetting.Ops} 个 {itemSetting.OptionalRules.Count} 可选规则:");

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
                Core.Logger.Log($"升级后的规则 {pair.Key.Name} 从 {pair.Key.Value} 到 {pair.Value}");
                pair.Key.Value = pair.Value;
            }
        }

        internal static bool EvaluateProperty(LRule itemRule, TrinityItem item, out float newValue)
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
                case ItemProperty.威能:
                    itemValue = ItemDataUtils.GetPassivePowerValue(item);
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.远古:
                    itemValue = item.IsAncient ? 1 : 0;
                    ruleValue = value;
                    result = item.IsAncient && Math.Abs(value - 1) < double.Epsilon;
                    returnValue = ruleValue;
                    break;

                case ItemProperty.主要属性:
                    itemValue = item.Attributes.PrimaryStat;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.暴击几率:
                    itemValue = item.Attributes.CritPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.暴击伤害:
                    itemValue = item.Attributes.CritDamagePercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.攻击速度:

                    itemValue = item.Attributes.AttacksPerSecondPercent;
                    if (itemValue == 0)
                    {
                        itemValue = item.Attributes.AttacksPerSecondItemPercent;
                    }
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.减少耗能:
                    itemValue = item.Attributes.ResourceCostReductionPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.冷却时间缩短:
                    itemValue = item.Attributes.CooldownPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.所有抗性:
                    itemValue = item.Attributes.ResistAll;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.镶孔:
                    itemValue = item.Attributes.Sockets;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = ruleValue;
                    break;

                case ItemProperty.体能:
                    itemValue = item.Attributes.Vitality;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.范围伤害:
                    itemValue = item.Attributes.AreaDamagePercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.荆棘伤害:
                    itemValue = item.Attributes.Thorns;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.火焰技能:
                    itemValue = item.Attributes.FireSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.冰霜技能:
                    itemValue = item.Attributes.ColdSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.闪电技能:
                    itemValue = item.Attributes.LightningSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.奥术技能:
                    itemValue = item.Attributes.ArcaneSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.神圣技能:
                    itemValue = item.Attributes.HolySkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.毒性技能:
                    itemValue = item.Attributes.PoisonSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.物理技能:
                    itemValue = item.Attributes.PhysicalSkillDamagePercentBonus;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.对精英怪伤害提高:
                    itemValue = item.Attributes.DamageAgainstElites;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.减少精英怪造成的伤害:
                    itemValue = item.Attributes.DamageFromElites;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.基础最大伤害:
                    itemValue = item.Attributes.MaxDamage;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.技能伤害:
                    var skillId = variant;
                    var skill = ItemDataUtils.GetSkillsForItemType(item.TrinityItemType, Core.Player.ActorClass).FirstOrDefault(s => s.Id == skillId);
                    if (skill != null)
                    {
                        friendlyVariant = skill.Name;
                        itemValue = item.Attributes.SkillDamagePercent(skill.SNOPower) * 100;
                    }
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.元素伤害:
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

                case ItemProperty.百分比伤害:
                    itemValue = item.Attributes.WeaponDamagePercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.暴击恢复奥能:
                    itemValue = item.Attributes.ArcaneOnCrit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.护甲:
                    itemValue = item.Attributes.ArmorBonusItem;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.格挡几率:
                    itemValue = item.Attributes.BlockChanceBonusPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.格挡几率总计:
                    itemValue = item.Attributes.BlockChanceItemTotal;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.憎恨再生:
                    itemValue = item.Attributes.HatredRegen;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.百分比生命:
                    itemValue = item.Attributes.LifePercent *100;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.击中回复生命:
                    itemValue = item.Attributes.LifeOnHit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.每秒恢复生命:
                    itemValue = item.Attributes.HealthPerSecond;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.法力恢复:
                    itemValue = item.Attributes.ManaRegen;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.移动速度:
                    itemValue = item.Attributes.MovementSpeedPercent;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.内力再生:
                    itemValue = item.Attributes.SpiritRegen;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.愤怒值再生:
                    itemValue =
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.消耗怒气恢复生命:
                    itemValue = item.Attributes.LifePerFury;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.消耗内力恢复生命:
                    itemValue = item.Attributes.LifePerSpirit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.消耗愤怒值恢复生命:
                    itemValue = item.Attributes.LifePerWrath;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.奥能上限:
                    itemValue = item.Attributes.MaxArcanePower;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.内力上限:
                    itemValue = item.Attributes.MaxSpirit;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.戒律上限:
                    itemValue = item.Attributes.MaxDiscipline;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.怒气上限:
                    itemValue = item.Attributes.MaxFury;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.法力上限:
                    itemValue = item.Attributes.MaxMana;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.愤怒上限:
                    itemValue = item.Attributes.MaximumWrath;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.击中产生致盲效果几率:
                    itemValue = item.Attributes.ChanceToBlind;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.击中产生冰冻效果几率:
                    itemValue = item.Attributes.ChanceToFreeze;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.击中产生定身效果几率:
                    itemValue = item.Attributes.ChanceToImmobilize;
                    ruleValue = value;
                    result = itemValue >= ruleValue;
                    returnValue = itemValue;
                    break;

                case ItemProperty.击中产生昏迷效果几率:
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
                            Core.Logger.Warn($"捡取列表中指定的属性无效. {friendlyVariant} - {error}");
                            break;
                        }

                        itemValue = item.Attributes.GetAttribute<float>(attribute, modifierId);
                        ruleValue = value;
                        result = itemValue >= ruleValue;
                        returnValue = itemValue;

                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Error($"评估捡取列表规则属性异常 {ex}");
                    }
                    break;
            }

            Core.Logger.Verbose($"  >>  评估 {item.Name} -- {prop.ToString().AddSpacesToSentence()} {friendlyVariant} (Item: {itemValue} -v- 规则: {ruleValue}) = {result}");
            newValue = returnValue;
            return result;
        }



    }
}

