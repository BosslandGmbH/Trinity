using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Trinity.Framework.Helpers;
using Trinity.Framework.Modules;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Sno.Types;
using Trinity.Framework.Objects.Memory.Symbols.Types;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory.Sno.Helpers
{
    public class PowerData
    {
        public string Name { get; set; }
        public int PowerSnoId { get; set; }
        public SNOPower SnoPower { get; set; }
        public string InternalName { get; set; }
        public Dictionary<TagType, ITag> Tags { get; set; }

        public bool IsPrimary
            => GetTag<bool>(TagType.TAG_POWER_IS_PRIMARY);

        public bool IsOffensive
            => GetTag<bool>(TagType.TAG_POWER_IS_OFFENSIVE);

        public bool IsUsableInTown
            => GetTag<bool>(TagType.TAG_POWER_IS_USABLE_IN_TOWN);

        public bool IsHotbarAssignable
            => GetTag<bool>(TagType.TAG_POWER_IS_HOTBAR_ASSIGNABLE);

        public bool IsMouseAssignable
            => GetTag<bool>(TagType.TAG_POWER_IS_MOUSE_ASSIGNABLE);

        public bool IsGroundTargeted
            => GetTag<bool>(TagType.TAG_POWER_IS_AIMED_AT_GROUND);

        public bool IsTargetGroundOnly
            => GetTag<bool>(TagType.TAG_POWER_TARGET_GROUND_ONLY);

        public bool IsTargetNavMeshOnly
            => GetTag<bool>(TagType.TAG_POWER_TARGET_GROUND_ONLY);

        public bool IsTargetRequired
            => GetTag<bool>(TagType.TAG_POWER_DOESNT_REQUIRE_ACTOR_TARGET) ||
               !GetTag<bool>(TagType.TAG_POWER_REQUIRES_TARGET);

        public float AttackRadius
            => Math.Max(GetTag<float>(TagType.TAG_POWER_ATTACK_RADIUS),
                GetTag<float>(TagType.TAG_POWER_ESCAPE_ATTACK_RADIUS));

        public float CastRange 
            => Math.Max(GetTag<float>(TagType.TAG_POWER_CONTROLLER_MIN_RANGE), AttackRadius);

        public int ChargesCost
            => GetTag<int>(TagType.TAG_POWER_COST_CHARGES);

        public int ChargesMax
            => GetTag<int>(TagType.TAG_POWER_MAX_CHARGES);

        public int ResourceCost
            => Math.Max(GetTag<int>(TagType.TAG_POWER_COST_RESOURCE), 
                GetTag<int>(TagType.TAG_POWER_COST_RESOURCE_MIN_TO_CAST));
        public int Cooldown
            => GetTag<int>(TagType.TAG_POWER_COOLDOWN);

        public bool IsPassive
            => GetTag<bool>(TagType.TAG_POWER_IS_PASSIVE);

        public bool IsItemPower
            => GetTag<bool>(TagType.TAG_POWER_IS_ITEM_POWER);

        public bool BreaksImmobilize
            => GetTag<bool>(TagType.TAG_POWER_BREAKS_IMMOBILIZE);

        public bool BreaksFear
            => GetTag<bool>(TagType.TAG_POWER_BREAKS_FEAR);

        public bool BreaksStun
            => GetTag<bool>(TagType.TAG_POWER_BREAKS_STUN);

        public bool BreaksRoot
            => GetTag<bool>(TagType.TAG_POWER_BREAKS_ROOT);

        public bool BreaksSnare
            => GetTag<bool>(TagType.TAG_POWER_BREAKS_SNARE);

        public bool IsChannelled
            => GetTag<bool>(TagType.TAG_POWER_IS_CHANNELLED);

        public bool IsBasicAttack
            => GetTag<bool>(TagType.TAG_POWER_IS_BASIC_ATTACK);
        
        public int ItemTypeRequirement
            => GetTag<int>(TagType.TAG_POWER_ITEM_TYPE_REQUIREMENT);

        public float MovementSpeedMultiplier
            => GetTag<float>(TagType.TAG_POWER_WALKING_SPEED_MULTIPLIER);

        public Element GetElementForRune(RuneSlot rune)
        {
            switch (rune)
            {
                case RuneSlot.RuneA:
                    return GetTag<Element>(TagType.TAG_POWER_RUNEA_DAMAGE_TYPE);
                case RuneSlot.RuneB:
                    return GetTag<Element>(TagType.TAG_POWER_RUNEB_DAMAGE_TYPE);
                case RuneSlot.RuneC:
                    return GetTag<Element>(TagType.TAG_POWER_RUNEC_DAMAGE_TYPE);
                case RuneSlot.RuneD:
                    return GetTag<Element>(TagType.TAG_POWER_RUNEC_DAMAGE_TYPE);
                case RuneSlot.RuneE:
                    return GetTag<Element>(TagType.TAG_POWER_RUNEC_DAMAGE_TYPE);
            }
            return GetTag<Element>(TagType.TAG_POWER_NORUNE_DAMAGE_TYPE);
        }

        public float OnHitProcCoefficient
            => GetTag<float>(TagType.TAG_POWER_ON_HIT_PROC_COEFFICIENT);

        public Element GetProcScalarForRune(RuneSlot rune, int combo = 0)
        {
            switch (rune)
            {
                case RuneSlot.RuneA:
                    return GetTag<Element>(TagType.TAG_POWER_RUNEA_PROC_SCALAR);
                case RuneSlot.RuneB:
                    return GetTag<Element>(TagType.TAG_POWER_RUNEB_PROC_SCALAR);
                case RuneSlot.RuneC:
                    return GetTag<Element>(TagType.TAG_POWER_RUNEC_PROC_SCALAR);
                case RuneSlot.RuneD:
                    return GetTag<Element>(TagType.TAG_POWER_RUNEC_PROC_SCALAR);
                case RuneSlot.RuneE:
                    return GetTag<Element>(TagType.TAG_POWER_RUNEC_PROC_SCALAR);
            }
            return GetTag<Element>(TagType.TAG_POWER_NORUNE_PROC_SCALAR);
        }

        public ActorClass ClassRestriction
            => Core.HeroData.GetActorClass(GetTag<int>(TagType.TAG_POWER_CLASS_RESTRICTION));

        public bool IsInvulnerableWhileCasting
             => GetTag<bool>(TagType.TAG_POWER_CANNOT_DIE_DURING);

        public T GetTag<T>(TagType tagType) where T : IConvertible
            => !Tags.ContainsKey(tagType) ? default(T) : Tags[tagType].GetValue<T>();

        public override string ToString() => $"{GetType().Name}: {Name}";

        public FormulaTag GetFormulaByIndex(int index)
            => Tags.Values.Cast<FormulaTag>().FirstOrDefault(f => f.Index == index);
    }

    public interface ITag
    {
        object Value { get; }
        T GetValue<T>();
    }

    public class Tag<T> : ITag where T : IConvertible
    {
        public TagType Type { get; set; }
        public string Name { get; set; }
        public T Value { get; set; }
        object ITag.Value => Value;
        public TResult GetValue<TResult>() => Value.To<TResult>();
        public override string ToString() => $"{GetType().Name}: {Name} = {Value}";
    }

    public class FormulaTag : ITag
    {
        private Func<float> _compiledFormula;
        public TagType Type { get; set; }
        public int Index { get; set; } = -1;
        public string Name { get; set; }
        public string Notes { get; set; }
        public float SimpleValue { get; set; }
        public string OriginalFormula { get; set; }
        public string FlattenedFormula { get; set; }
        public string FinalFormula { get; set; }
        public bool IsSimpleValue { get; set; }
        public float? LastValue { get; set; }
        public FormulaDataRetriever Retriever { get; set; }
        object ITag.Value => Value;
        public TResult GetValue<TResult>() => Value.To<TResult>();
        public override string ToString() => $"{GetType().Name}: {Name} = {Value}";

        public float Value
        {
            get
            {
                try
                {
                    if (IsSimpleValue && LastValue.HasValue)
                        return LastValue.Value;

                    LastValue = EvalFormula();
                    return LastValue.Value;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"FormulaTag Exception {ex}");
                }
                return 0f;
            }
        }

        private float EvalFormula()
        {
            if (_compiledFormula != null)
                return _compiledFormula();

            var compiler = new CompiledExpression<float>();
            compiler.RegisterType("ActorAttributeType", typeof(ActorAttributeType));
            compiler.RegisterType("NativePowerSno", typeof(NativePowerSno));
            compiler.RegisterType("Data", Retriever);
            compiler.StringToParse = FinalFormula;
            _compiledFormula = compiler.Compile();
            return _compiledFormula();
        }
    }

    public class FormulaDataRetriever
    {
        private readonly SNOPower _power;

        public FormulaDataRetriever(NativePower nativePower)
        {
            _power = (SNOPower)nativePower.Header.SnoId;
        }

        // todo
        //Monk_LashingTailKick [111676] TAG_POWER_ATTACK_RADIUS: PIN((Rune_C * 40), 5, 40)
        //Monk_Passive_GuidingLight [156492] TAG_POWER_SCRIPT_FORMULA_0: sLevel? 0.15 : 0
        //Monk_ResistAura_RuneC_Holy [144322] TAG_POWER_SCRIPT_FORMULA_0: 0.1 * Rune_C#Monk_ResistAura
        //Monk_CycloneStrike [223473]TAG_POWER_SCRIPT_FORMULA_20: 0.057857 * Table(Healing,Effective_Level)
        //X1_Monk_Epiphany [312307] TAG_POWER_SCRIPT_FORMULA_47: (0.030) * Table(Healing, Level)
        //P4_Forest_MysteriousHermit_ArcaneFireball+ [445864] mDamageMin#Fire * 0.5
        //P4_Forest_MysteriousHermit_ArcaneFireball+ TAG_POWER_SCRIPT_FORMULA_3: mDamageDelta#Fire * 0.5
        //Barbarian_CallOfTheAncients_Whirlwind [168830] TAG_POWER_SCRIPT_FORMULA_12: Damage_Min
        //Barbarian_Frenzy [78548] TAG_POWER_ATTACK_SPEED: Attacks_Per_Second_Total * (1 + (Buff_Icon_Count3 + Buff_Icon_Count2 + Buff_Icon_Count0) * SF_6) * PowerTag.P4_ItemPassive_Unique_Ring_055."Script Formula 0"
        //Barbarian_Leap [93409] TAG_POWER_FURY_COEFFICIENT: 0.5 + (Rune_D#Barbarian_Leap * 0.25)
        //MIN(0.5, Rune_E#Barbarian_Sprint * 0.5) + Rune_E#Barbarian_Sprint * 0.1

        public float EvalFormulaLink(NativePowerSno power, int formulaIndex)
        {
            try
            {
                var powerData = PowerHelper.Instance.GetPowerData((int)power);
                if (powerData == null)
                    return -1;

                var formula = powerData.GetFormulaByIndex(formulaIndex);
                return formula?.Value ?? -1;
            }
            catch (Exception ex)
            {
                Logger.LogError($"PowerHelper Exception in EvalRemoteFormula. {ex}");
            }
            return -1;
        }

        public float EvalAttributeLink(ActorAttributeType attributeType)
        {
            try
            {
                if (Core.Actors.Me == null)
                    Core.Actors.Update();
                
                var att = Core.Actors.Me?.Attributes.GetAttributeItem(attributeType);
                if (att == null)
                    return -1;

                return att.GetValue<float>();
            }
            catch (Exception ex)
            {
                Logger.LogError($"PowerHelper Exception in EvalRemoteFormula. {ex}");
            }
            return -1;
        }

        private HotbarCache.HotbarSkill CurrentHotbarSkill => Core.Hotbar.ActiveSkills.FirstOrDefault(s => s.Power == _power);
        public bool Rune_A => CurrentHotbarSkill?.RuneIndex == 0;
        public bool Rune_B => CurrentHotbarSkill?.RuneIndex == 1;
        public bool Rune_C => CurrentHotbarSkill?.RuneIndex == 2;
        public bool Rune_D => CurrentHotbarSkill?.RuneIndex == 3;
        public bool Rune_E => CurrentHotbarSkill?.RuneIndex == 4;
    }

    public class PowerHelper
    {
        private static readonly Lazy<PowerHelper> _instance = new Lazy<PowerHelper>(() => new PowerHelper());
        public static PowerHelper Instance => _instance.Value;
        private PowerHelper() { }

        public NativePower GetNativePower(SNOPower power) => GetNativePower((int)power);
        public NativePower GetNativePower(NativePowerSno power) => GetNativePower((int)power);
        public NativePower GetNativePower(int powerSnoId)
        {
            var entityId = SnoManager.Core.GetEntityId(SnoType.Power, powerSnoId);
            if (entityId == 0)
            {
                return SnoSearchProxy.GetRecord<NativePower>(SnoType.Power, powerSnoId);
            }            
            return SnoManager.Groups.Power.Container[entityId].Value;
        }

        public readonly Dictionary<int, PowerData> Entries = new Dictionary<int, PowerData>();

        public PowerData GetPowerData(SNOPower power) => GetPowerData((int)power);
        public PowerData GetPowerData(NativePowerSno power) => GetPowerData((int)power);
        public PowerData GetPowerData(int powerSnoId)
        {
            if (Entries.ContainsKey(powerSnoId))
                return Entries[powerSnoId];

            var native = Core.MemoryModel.PowerHelper.GetNativePower(powerSnoId);
            PowerData powerData;

            if (!TryCreatePowerData(native, out powerData))
                return null;

            Entries[powerSnoId] = powerData;
            return powerData;
        }

        private static bool TryCreatePowerData(NativePower native, out PowerData powerData)
        {
            var name = native._1_0xC_String;
            var snoPowerId = native.Header.SnoId;
            var snoPower = (SNOPower)snoPowerId;
            var internalName = snoPower.ToString();
            var tags = Core.MemoryModel.PowerHelper.GetTagsForPower(native);

            var formulaTags = tags.Where(t => t.Value.TagRef.DataType == MapDataType.Formula).ToDictionary(k => k.Key, v => v.Value);

            var formulaByIndex = formulaTags.Where(t => t.Key.ToString().Contains("TAG_POWER_SCRIPT_FORMULA_"))
                .ToDictionary(k => int.Parse(k.Key.ToString().Split('_').Last()), v => v.Value);

            var formulaDataRetriever = new FormulaDataRetriever(native);
            var formulas = new Dictionary<TagType, ITag>();

            // Main power formulae.
            for (var i = 0; i < native._7_0x438_VariableArray.Count; i++)
            {
                var summary = native._7_0x438_VariableArray[i];

                PowerMap.PowerMapItem tagEntry;
                formulaByIndex.TryGetValue(i, out tagEntry);
                if (tagEntry == null)
                    continue;

                var formulaDef = tagEntry.x08_FomulaValue;
                var flatFormula = FlattenFormula(formulaDef.Formula, formulaByIndex);
                var preppedFormula = PrepFormula(flatFormula);
                var tagType = tagEntry.x04_Key;

                formulas.Add(tagType, new FormulaTag
                {
                    Name = summary._1_0x0_String,
                    Index = i,
                    Notes = summary._2_0x100_String,
                    OriginalFormula = formulaDef.Formula,
                    FlattenedFormula = flatFormula,
                    FinalFormula = preppedFormula,
                    Type = tagType,
                    IsSimpleValue = !formulaDef.RequiresEvaluation,
                    Retriever = formulaDataRetriever,
                });
            }

            // Other Formula type tags
            foreach (var formula in formulaTags.Values.Except(formulaByIndex.Values))
            {
                var formulaDef = formula.x08_FomulaValue;
                var flatFormula = FlattenFormula(formulaDef.Formula, formulaByIndex);
                var preppedFormula = PrepFormula(flatFormula);
                var tagRef = formula.TagRef;
                var tagType = tagRef.TagType;

                formulas.Add(tagType, new FormulaTag
                {
                    Name = tagRef.DisplayName,
                    OriginalFormula = formulaDef.Formula,
                    FlattenedFormula = flatFormula,
                    FinalFormula = preppedFormula,
                    Type = tagType,
                    IsSimpleValue = !formulaDef.RequiresEvaluation,
                    Retriever = formulaDataRetriever,
                });
            }

            // Non-Formula Tags           
            foreach (var tag in tags.Except(formulaTags))
            {
                var tagRef = tag.Value.TagRef;
                var floatTag = tag.Value.Value as PowerMap.TagValueType<float>;
                if (floatTag != null)
                {
                    formulas.Add(tag.Key, new Tag<float>
                    {
                        Name = tagRef.DisplayName,
                        Type = tag.Key,
                        Value = floatTag.Value,
                    });
                    continue;
                }
                var intTag = tag.Value.Value as PowerMap.TagValueType<int>;
                if (intTag != null)
                {
                    formulas.Add(tag.Key, new Tag<int>
                    {
                        Name = tagRef.DisplayName,
                        Type = tag.Key,
                        Value = intTag.Value,
                    });
                }
            }

            powerData = new PowerData
            {
                Name = name,
                InternalName = internalName,
                PowerSnoId = snoPowerId,
                SnoPower = snoPower,
                Tags = formulas,
            };

            return true;
        }

        /// <summary>
        /// Recursively follow SF_12 style links within a formula and join their contents.
        /// </summary>
        public static string FlattenFormula(string formula, Dictionary<int, PowerMap.PowerMapItem> formulaByIndex)
        {
            var result = Regex.Replace(formula, "SF_(\\d+)", match =>
            {
                var r = match.Value;
                var parts = match.Value.Split('_');
                var prefix = parts.FirstOrDefault();
                if (prefix == null)
                {
                    return r;
                }
                if (prefix == "SF")
                {
                    var index = int.Parse(parts[1]);
                    var f = formulaByIndex[index].x08_FomulaValue;
                    r = f.ContainsLink ? FlattenFormula(f.Formula, formulaByIndex) : f.Formula;
                }
                return $"( {r} )";
            });
            return result;
        }

        /// <summary>
        /// Formats a formula for ExpressionEvaluator syntax.
        /// </summary>
        public static string PrepFormula(string formula)
        {
            formula = formula.Replace("Rune_A", "Data.Rune_A");
            formula = formula.Replace("Rune_B", "Data.Rune_B");
            formula = formula.Replace("Rune_C", "Data.Rune_C");
            formula = formula.Replace("Rune_D", "Data.Rune_D");
            formula = formula.Replace("Rune_E", "Data.Rune_E");

            formula = Regex.Replace(formula, @"PowerTag.(?<PowerTag>.*?)\..*?Formula\s(?<ScriptFormula>\d+).*?[""]", match =>
            {
                var powerGroup = match.Groups["PowerTag"];
                var scriptGroup = match.Groups["ScriptFormula"];
                if (powerGroup != null && scriptGroup != null)
                {
                    return $"Data.EvalFormulaLink(NativePowerSno.{powerGroup.Value},{scriptGroup.Value})";
                }
                return string.Empty;
            });

            foreach(var attLinkDef in AttributLinkDefinitions)
            {
                formula = formula.Replace(attLinkDef.Key, $"Data.EvalAttributeLink(ActorAttributeType.{attLinkDef.Value})");
            }

            return formula;
        }

        public static Dictionary<string, ActorAttributeType> AttributLinkDefinitions = new Dictionary<string, ActorAttributeType>
        {
            {"Attacks_Per_Second_Total", ActorAttributeType.AttacksPerSecondTotal}
        };

        public List<NativePower> GetNativePowers()
        {
            return SnoManager.Groups.Power.Container.Where(p => p.SnoGroupId == (int)SnoType.Power).Select(p => p.Value).ToList();           
        }

        public Dictionary<TagType, PowerMap.PowerMapItem> GetTagsForPower(NativePower power)
        {
            var result = new Dictionary<TagType, PowerMap.PowerMapItem>();
            var powerDefinition = power._2_0x50_Object;

            //if (powerDefinition._2_0x8_TagMap.x0C_EntryCount > 0)
            //    result.AddRangeOverride(powerDefinition._2_0x8_TagMap.Entries);

            if (powerDefinition._4_0x18_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._4_0x18_TagMap.Entries);

            //if (powerDefinition._6_0x28_TagMap.x0C_EntryCount > 0)
            //    result.AddRangeOverride(powerDefinition._6_0x28_TagMap.Entries);

            if (powerDefinition._11_0x50_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._11_0x50_TagMap.Entries);

            //if (powerDefinition._12_0x58_TagMap.x0C_EntryCount > 0)
            //    result.AddRangeOverride(powerDefinition._12_0x58_TagMap.Entries);

            //if (powerDefinition._13_0x60_TagMap.x0C_EntryCount > 0)
            //    result.AddRangeOverride(powerDefinition._13_0x60_TagMap.Entries);

            //if (powerDefinition._14_0x68_TagMap.x0C_EntryCount > 0)
            //    result.AddRangeOverride(powerDefinition._14_0x68_TagMap.Entries);

            //if (powerDefinition._19_0x90_TagMap.x0C_EntryCount > 0)
            //    result.AddRangeOverride(powerDefinition._19_0x90_TagMap.Entries);

            //if (powerDefinition._20_0x98_TagMap.x0C_EntryCount > 0)
            //    result.AddRangeOverride(powerDefinition._20_0x98_TagMap.Entries);

            //if (powerDefinition._21_0xA0_TagMap.x0C_EntryCount > 0)
            //    result.AddRangeOverride(powerDefinition._21_0xA0_TagMap.Entries);

            //if (powerDefinition._22_0xA8_TagMap.x0C_EntryCount > 0)
            //    result.AddRangeOverride(powerDefinition._22_0xA8_TagMap.Entries);

            return result;
        }

    }
}
