using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Debug;

namespace Trinity.Framework.Objects.Memory.Misc
{
    public class MapManager : MemoryWrapper
    {
        public MapManager(IntPtr ptr)
        {
            Update(ptr);
        }

        public List<Map<TagMapItem>> TagMaps => ReadObjects<Map<TagMapItem>>(0x00,19,0x70).ToList();

        public List<string> TagMapEnums => TagMaps.Select(ExportTagMapEnumFunc).ToList();

        private string ExportTagMapEnumFunc(Map<TagMapItem> map)
            => map.ToEnum((k, v) => new Tuple<string, string, string>(v.InternalName, v.Id.ToString(), v.DisplayName));

        public List<Map<StringMapItem>> SnoMaps { get; } = ReadObjects<Map<StringMapItem>>((IntPtr)0x13036700 + 8, 71, 0x70).ToList();

        public List<string> SnoMapEnums => SnoMaps.Select(ExportSnoMapEnumFunc).ToList();

        private string ExportSnoMapEnumFunc(Map<StringMapItem> map)
            => map.ToEnum((k, v) => new Tuple<string, string, string>(v.InternalName, k.ToString(), v.Id.ToString()));

    }


    

}