using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Debug;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Sno;
using Trinity.Framework.Objects.Memory.Sno.Types;

namespace Trinity.Framework.Objects.Memory.Reference
{
    public class ReferenceGenerator
    {
        public static void GenerateFiles()
        {
            foreach (var file in FileMapping)
            {
                var path = Path.Combine(FileManager.ReferencePath, file.Key);
                File.WriteAllText(path, file.Value());
            }
        }

        public static Dictionary<string, Func<string>> FileMapping { get; } = new Dictionary<string, Func<string>>
        {
            { "RawItemType.cs", () => GenerateRawItemTypeEnum() }
        };

        public static string GenerateRawItemTypeEnum()
        {
            var gb = SnoManager.Core.CreateGroup<GameBalanceCollection>(SnoType.GameBalance);
            var table = gb.Container.FirstOrDefault(o => o.Value.GameBalanceType == SnoGameBalanceType.ItemTypes);
            var typeEnum = ReferenceHelper.GenerateEnum(table.Value._4_0x18_NativeItemsTable._2_0x8_VariableArray,
                k => k._1_0x0_String,
                v => v._2_0x100_int, // Trinity.Framework.Helpers.MemoryHelper.GameBalanceItemHash("Bracers_Crusader")
                c => $"_4_0x108_ItemTypes_GameBalanceId={c._4_0x108_ItemTypes_GameBalanceId}", 
                "RawItemType");
            return typeEnum;
        }

        //public static string TagMapDictionary => ReferenceHelper.GenerateDictionary<TagMapItem, TagReference>(
        //    Core.MemoryModel.MapManager.TagMaps.Where(m => !string.IsNullOrEmpty(m.Entries.FirstOrDefault()?.DisplayName)).SelectMany(map => map.Entries).DistinctBy(e => e.Id),
        //    v => v.Id, new Dictionary<string, Func<TagMapItem, string>>
        //    {
        //        { "Id", v => $"{v.Id}" },
        //        { "TagType", v => $"TagType.{v.TagType}" },
        //        { "DataType", v => $"MapDataType.{v._DataTypeId}" },
        //        { "DisplayName", v => $"\"{v.DisplayName}\"" },
        //        { "InternalName", v => $"\"{v.InternalName}\"" },
        //    });
    }
}
