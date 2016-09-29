using System;
using Trinity.Framework.Helpers;

namespace Trinity.Framework.Objects.Memory.Misc
{
    public class FieldDescriptor : MemoryWrapper, IEquatable<FieldDescriptor>
    {
        public const int SizeOf = 0x8C; // = 140
        public string Name => ReadStringPointer(0x00);
        public ValueTypeDescriptor Type => ReadPointer<ValueTypeDescriptor>(0x04);
        public int Offset => ReadOffset<int>(0x08);
        public int DefaultValuePtr => ReadOffset<int>(0x0C);
        public int Min => ReadOffset<int>(0x10);
        public int Max => ReadOffset<int>(0x14);
        public FieldDescriptorFlags Flags => ReadOffset<FieldDescriptorFlags>(0x18); // 0x02 = (address + 8 = PtrToSerializedData), 0x10 = HasMinMaxBounds, 0x80000 = Bin2Text
        public ValueTypeDescriptor BaseType => ReadPointer<ValueTypeDescriptor>(0x1C);
        public int VariableArraySerializeOffsetDiff => ReadOffset<int>(0x20);
        public int FixedArrayLength => ReadOffset<int>(0x24); // -1 if not an array
        public int FixedArraySerializeOffsetDiff => ReadOffset<int>(0x28);
        public short UsedBits => ReadOffset<short>(0x2C); // Most likely used for transmission.
        public short _x2E => ReadOffset<short>(0x2E);
        public int GroupId => ReadOffset<int>(0x30); // -1 if none, used for DT_SNO and DT_GBID.
        public int _x34 => ReadOffset<int>(0x34);
        public int SymbolTable => ReadOffset<int>(0x38); // Used for DT_ENUM // int MemoryAddress at start of Table
        public int BitOffset => ReadOffset<int>(0x3C);
        public int TranslateFromString => ReadOffset<int>(0x40);
        public int TranslateFromValue => ReadOffset<int>(0x44);
        public int _x48 => ReadOffset<int>(0x48);
        public string _x4C_Text => ReadString(0x4C, 64);

        public override string ToString()
        {
            return $"{GetType().Name} {Name} Offset: {Offset} Type: {Type.Name} ({BaseType.Name})";
        }

        public override int GetHashCode()
        {
            return unchecked(MemoryHelper.GameBalanceNormalHash(Type.Name) + MemoryHelper.GameBalanceNormalHash(BaseType.Name) + Offset * 31 + SizeOf * 31);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FieldDescriptor);
        }

        public bool Equals(FieldDescriptor other)
        {
            return GetHashCode() == other?.GetHashCode();
        }
    }

    [Flags]
    public enum FieldDescriptorFlags
    {
        None = 0,
        PtrToSerializedData = 2,
        HasMinMaxBounds = 16,
        Bin2Text = 524288
    }
}