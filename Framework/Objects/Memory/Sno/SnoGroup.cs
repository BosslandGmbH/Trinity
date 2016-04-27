using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Containers;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public class SnoGroup<T> : MemoryWrapper where T : SnoTable, new()
    {
        public Container<SnoDefinition<T>> Container => ReadPointer<Container<SnoDefinition<T>>>(0x10);
        public SnoType SnoGroupId => ReadOffset<SnoType>(0x3C);
        public int InvalidSnoId => ReadOffset<int>(0x80);
        public int ItemSize => ReadOffset<int>(0x68);
        public int Flags => ReadOffset<int>(0x18);
        public string Name => ReadString(0x1C, 32);
    }
}