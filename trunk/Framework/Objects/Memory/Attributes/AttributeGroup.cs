using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Items;

namespace Trinity.Framework.Objects.Memory.Attributes
{
    public class AttributeGroup : MemoryWrapper
    {
        public int Id => ReadOffset<int>(0x000);

        public short Index => ReadOffset<short>(0x000);

        public int Flags => ReadOffset<int>(0x004);

        public Map<AttributeItem> PtrMap => ReadObject<Map<AttributeItem>>(0x00C);

        public Map<AttributeItem> Map => ReadObject<Map<AttributeItem>>(0x010);
    }
}
