namespace Trinity.Framework.Objects.Memory.Sno
{
    public class SnoTableEntry : MemoryWrapper
    {
        public SnoHeader Header => ReadObject<SnoHeader>(0x00);

    }
}
