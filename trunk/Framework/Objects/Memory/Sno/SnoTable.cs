namespace Trinity.Framework.Objects.Memory.Sno
{
    public class SnoTable : MemoryWrapper
    {
        public SnoHeader Header => ReadObject<SnoHeader>(0x00);
    }
}