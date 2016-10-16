using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory.Debug;
using Trinity.Framework.Objects.Memory.Misc;

namespace Trinity.Framework.Objects.Memory.Reference
{
    public class ReferenceGenerator
    {
        public static string TagMapDictionary => ReferenceHelper.GenerateDictionary<TagMapItem, TagReference>(
            Core.MemoryModel.MapManager.Maps.Where(m => !string.IsNullOrEmpty(m.Entries.FirstOrDefault()?.DisplayName)).SelectMany(map => map.Entries).DistinctBy(e => e.Id),
            v => v.Id, new Dictionary<string, Func<TagMapItem, string>>
            {
                { "Id", v => $"{v.Id}" },
                { "TagType", v => $"TagType.{v.TagType}" },
                { "DataType", v => $"MapDataType.{v._DataTypeId}" },
                { "DisplayName", v => $"\"{v.DisplayName}\"" },
                { "InternalName", v => $"\"{v.InternalName}\"" },
            });
    }
}
