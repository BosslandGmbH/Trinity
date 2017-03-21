using System.Linq;
namespace Trinity.Framework.Objects.Memory
{
    public class UXReference : MemoryWrapper
    {
        public const int SizeOf = 0x210;
        public ulong Hash => ReadOffset<ulong>(0x000);
        public string Name => ReadString(0x008, 0x200);
        public UXControl Control => UXHelper.GetControl(Hash);
        public override string ToString() => Name.Split('.').Last();
    }
}
