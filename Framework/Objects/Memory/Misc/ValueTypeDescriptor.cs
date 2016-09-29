using System;
using System.Collections.Generic;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory.Debug;

namespace Trinity.Framework.Objects.Memory.Misc
{
    public class ValueTypeDescriptor : MemoryWrapper, IEquatable<ValueTypeDescriptor>
    {
        public const int SizeOf = 0x1C; // = 28
        public int VTable => ReadOffset<int>(0x00);
        public string Name => ReadStringPointer(0x04);
        public int _x08 => ReadOffset<int>(0x08);
        public IEnumerable<FieldDescriptor> FieldDescriptors => ReadObjects<FieldDescriptor>(ReadOffset<IntPtr>(0x0C), FieldCount, 0x8C);
        public int FieldCount => ReadOffset<int>(0x10);
        public int _x14 => ReadOffset<int>(0x14);
        public int Flags => ReadOffset<int>(0x18);

        public override string ToString()
        {
            return $"{GetType().Name} {Name}";
        }

        public string Map => ClassMapper.Map(this);

        public override int GetHashCode()
        {
            return MemoryHelper.GameBalanceNormalHash(Name);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ValueTypeDescriptor);
        }

        public bool Equals(ValueTypeDescriptor other)
        {
            return GetHashCode() == other?.GetHashCode();
        }
    }
}