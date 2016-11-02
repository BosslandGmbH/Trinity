using Trinity.Framework.Objects.Enums;

namespace Trinity.Framework.Objects.Memory.Misc
{
    public class StringMapItem : MemoryWrapper
    {
        public const int SizeOf = 0x14;
        public int Id => ReadOffset<int>(0x00);
        public string InternalName => ReadStringPointer(0x04);
        public override string ToString() => $"{InternalName} Id={Id}";
    }

}