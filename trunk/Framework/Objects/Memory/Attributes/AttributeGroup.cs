using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Misc;

namespace Trinity.Framework.Objects.Memory.Attributes
{
    public class AttributeGroup : MemoryWrapper
    {
        public int Id => ReadOffset<int>(0x000);
        public short Index => ReadOffset<short>(0x000);
        public int Flags => ReadOffset<int>(0x004);
        public BasicMap<AttributeItem> PtrMap => ReadPointer<BasicMap<AttributeItem>>(0x00C);
        public BasicMap<AttributeItem> Map => ReadObject<BasicMap<AttributeItem>>(0x010);
        public BasicMap<AttributeItem> Map2 => ReadObject<BasicMap<AttributeItem>>(0x438); //1080 (438)
    }
}
