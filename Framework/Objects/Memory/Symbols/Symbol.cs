namespace Trinity.Framework.Objects.Memory.Symbols
{
    public class Symbol : MemoryWrapper
    {
        public static int SizeOf = 8;
        public int Id;
        public string Name;

        protected override void OnUpdated()
        {
            Id = ReadOffset<int>(0x00);
            Name = ReadStringPointer(0x04);
        }

        public override string ToString()
        {
            return $"{GetType().Name}, {Name} ({Id})";
        }
    }
}