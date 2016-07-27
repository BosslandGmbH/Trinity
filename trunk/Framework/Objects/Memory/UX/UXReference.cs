namespace Trinity.Framework.Objects.Memory.UX
{
    public class UXReference : MemoryWrapper
    {
        public ulong Hash => ReadOffset<ulong>(0x000);
        public string Name => ReadString(0x008, 0x200);
        public override string ToString() => Name;
    }
}
