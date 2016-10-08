using Trinity.Framework.Objects.Enums;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public class SnoHeader : MemoryWrapper
    {
        public const int SizeOf = 0x0C; // 12   
        public int SnoId => ReadOffset<int>(0x00);
        public int LockCount => ReadOffset<int>(0x04);
        public int Flags => ReadOffset<int>(0x08); // 1 = DoNotPurge
    }
}

