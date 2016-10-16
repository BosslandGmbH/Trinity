using Trinity.Framework.Objects.Enums;

namespace Trinity.Framework.Objects.Memory.Misc
{
    public class TagMapItem : MemoryWrapper
    {
        public const int SizeOf = 0x20; // 32
        public int Id => ReadOffset<int>(0x00);
        public TagType TagType => (TagType)Id;
        public MapDataType _DataTypeId => (MapDataType)ReadOffset<int>(0x04);
        public int _x08int => ReadOffset<int>(0x8);
        public float _x08float => ReadOffset<float>(0x8);
        public string _x08stringPtr => ReadStringPointer(0x8);
        public string DisplayName => ReadStringPointer(0x10);
        public string InternalName => ReadStringPointer(0x14);
        public int _0x1C => ReadOffset<int>(0x1C);
        public override string ToString() => $"{InternalName} = {Id} // DataType={_DataTypeId}, {DisplayName},";
    }
}