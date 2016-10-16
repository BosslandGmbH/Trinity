using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Debug;
using Trinity.Framework.Objects.Memory.Sno.Types;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Modules
{
    public class PowerDataCache : Module
    {
        public class PowerData
        {
            public string Name { get; set; }
            public int PowerSnoId { get; set; }
            public SNOPower SnoPower { get; set; }
            public List<KeyValuePair<TagType, string>> Tags { get; set; }
            public List<FormulaData> Formulas { get; set; }
            public override string ToString() => $"{GetType().Name}: {Name}";
        }

        public class FormulaData
        {
            public TagType Type { get; set; }
            public string Name { get; set; }
            public string Formula { get; set; }
            public string Notes { get; set; }
            public float Value { get; set; }
            public override string ToString() => $"{GetType().Name}: {Name} = {Value}";
        }

        public readonly Dictionary<SNOPower, PowerData> Entries = new Dictionary<SNOPower, PowerData>();

        public PowerDataCache()
        {
            var nativePowers = Core.MemoryModel.PowerHelper.GetPowers();

            foreach (var native in nativePowers)
            {
                SNOPower snoPower;
                PowerData powerData;

                if (!TryCreatePowerData(native, out snoPower, out powerData))
                    continue;

                Entries[snoPower] = powerData;
            }
        }

        private static bool TryCreatePowerData(NativePower native, out SNOPower snoPower, out PowerData heroData)
        {
            heroData = null;
            snoPower = SNOPower.None;

            var name = native._1_0xC_String;
            if (string.IsNullOrEmpty(name))
                return false;

            var snoPowerId = native.Header.SnoId;
            snoPower = (SNOPower) snoPowerId;
          
            //var tags = Core.MemoryModel.PowerHelper.GetTagsForPower(native);
            //var tagValues = tags.ToDictionary(k => k.Key,v => v.Value.Value.ToString()).ToList();
            //var formulaTags = tags.Where(t => t.Key.ToString().Contains("FORMULA")).ToList();
            //var formulaNumLookup = formulaTags.ToDictionary(k => int.Parse(k.Key.ToString().Split('_').Last()), v => v.Value);
            //var formulaStringLookup = formulaNumLookup.ToDictionary(k => "SF_" + k.Key, v => v);

            //var formulaValues = formulaTags.Select(f => f.Value.x08_FomulaValue.Value).ToList();

            //var formulas = new List<FormulaData>();
            //for (var i=0; i < native._7_0x438_VariableArray.Count; i++)
            //{
            //    var summary = native._7_0x438_VariableArray[i];

            //    PowerMap.PowerMapItem tagEntry;
            //    formulaNumLookup.TryGetValue(i, out tagEntry);
            //    if (tagEntry == null)
            //        continue;

            //    var formulaDef = tagEntry.x08_FomulaValue;
            //    var tokens = formulaDef.Data.Tokens;

            //    var finalValue = 0f;
            //    var requiresCalculation = tokens.Any(t => t.Type == PowerMap.FormulaCode.TokenType.Link);
            //    if (requiresCalculation)
            //    {
            //        finalValue = -1; // todo
            //    }
            //    else
            //    {
            //        var valueToken = tokens.OfType<PowerMap.FormulaCode.NumberToken>().FirstOrDefault();
            //        if (valueToken != null)
            //        {
            //            finalValue = valueToken.FloatValue;
            //        }
            //    }

            //    formulas.Add(new FormulaData
            //    {
            //        Name = summary._1_0x0_String,
            //        Notes = summary._2_0x100_String,
            //        Formula = formulaDef.Formula,
            //        Type = tagEntry.x04_Key,
            //        Value = finalValue,
            //    });
            //}

            //heroData = new PowerData
            //{
            //    Name = name,
            //    PowerSnoId = snoPowerId,
            //    SnoPower = snoPower,  
            //    Tags = tagValues,
            //    Formulas = formulas,
            //};

            return true;
        }


    }

}