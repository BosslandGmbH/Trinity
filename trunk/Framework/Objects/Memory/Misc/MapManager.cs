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

        public List<Map<TagMapItem>> Maps => ReadObjects<Map<TagMapItem>>(0x00,19,0x70).ToList();

        public List<string> TagMapEnums => Maps.Select(ExportTagMapEnumFunc).ToList();

        private string ExportTagMapEnumFunc(Map<TagMapItem> map)
            => map.ToEnum(a => new Tuple<string, string, string>(a.InternalName, a.Id.ToString(), a.DisplayName));


    }


}