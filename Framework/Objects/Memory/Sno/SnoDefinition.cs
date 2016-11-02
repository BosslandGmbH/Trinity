using System;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public class SnoDefinition<T> : MemoryWrapper where T : SnoTableEntry, new()
    {
        public const int SizeOf = 16;
        public int Id => ReadOffset<int>(0x00);
        public int LastTouched => ReadOffset<int>(0x04);
        public byte SnoGroupId => ReadOffset<byte>(0x07);
        public int Size => ReadOffset<int>(0x08);
        public T Value => ReadPointer<T>(0x0C);
        public IntPtr ValuePtr => ReadOffset<IntPtr>(0x0C);
        public override string ToString() => $"{GetType().Name}: {Value}";
    }
}