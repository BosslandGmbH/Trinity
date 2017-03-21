using System.Runtime.InteropServices;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct SnoGameBalanceItem
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        [FieldOffset(0x00)]
        public string InternalName;

        [FieldOffset(0x108)]
        public int ActorSnoId;

        [FieldOffset(0x10C)]
        public int ItemTypesGameBalanceId;

        [FieldOffset(0x16C)]
        public int ItemsGameBalanceId;

        //[FieldOffset(0x170)]
        //public int SetItemBonusesGameBalanceId;

        //[FieldOffset(0x184)]
        //public int RareItemPartAPrefixStringListId;

        //[FieldOffset(0x188)]
        //public int RareItemPartASuffixStringListId;

        //[FieldOffset(0x138)]
        //public int BaseGoldValue;

        [FieldOffset(0x148)]
        public int ItemLevel;

        [FieldOffset(0x134)]
        public int StackSize;

        [FieldOffset(0x430)]
        public GemType GemType;
    }

    //[CompilerGenerated]
    //public class NativeItemType : MemoryWrapper
    //{
    //    public const int SizeOf = 336; // 0x150
    //    public string _1_0x0_String => ReadString(0x0); //      Flags=18437
    //    public int _2_0x100_int => ReadOffset<int>(0x100);
    //    public int _3_0x104_int => ReadOffset<int>(0x104);
    //    public int _4_0x108_ItemTypes_GameBalanceId => ReadOffset<int>(0x108); //      Flags=1
    //    public int _5_0x10C_int => ReadOffset<int>(0x10C); //      Flags=1
    //    public int _6_0x110_int => ReadOffset<int>(0x110); //      Flags=1
    //    public int _7_0x114_int => ReadOffset<int>(0x114); //      Flags=1
    //                                                       // Unknown index=8 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=9 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=10 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=11 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=12 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=13 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=14 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=15 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=16 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=17 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=18 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=19 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=20 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=21 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //                                                       // Unknown index=22 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1
    //    public HealingType _23_0x11C_Enum => ReadOffset<HealingType>(0x11C); //      Flags=17 Max=25
    //    public HealingType _24_0x120_Enum => ReadOffset<HealingType>(0x120); //      Flags=17 Max=25
    //    public HealingType _25_0x124_Enum => ReadOffset<HealingType>(0x124); //      Flags=17 Max=25
    //    public HealingType _26_0x128_Enum => ReadOffset<HealingType>(0x128); //      Flags=17 Max=25
    //    public int _27_0x12C_AffixList_GameBalanceId => ReadOffset<int>(0x12C); //      Flags=1
    //    public int _28_0x130_AffixList_GameBalanceId => ReadOffset<int>(0x130); //      Flags=1
    //    public int _29_0x134_AffixList_GameBalanceId => ReadOffset<int>(0x134); //      Flags=1
    //    public int _30_0x138_AffixGroup_GameBalanceId => ReadOffset<int>(0x138); //      Flags=1
    //    public List<int> _31_0x13C_FixedArray => ReadArray<int>(0x13C, 5); //      Flags=1
    //}

    //    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    public struct ItemRecord
    //    {
    //        [CompilerGenerated, UnsafeValueType]
    //        [StructLayout(LayoutKind.Sequential, Size = 256)]
    //        public struct <InternalName>e__FixedBuffer
    //	{
    //		public byte FixedElementField;
    //    }

    //    [CompilerGenerated, UnsafeValueType]
    //    [StructLayout(LayoutKind.Sequential, Size = 16)]
    //    public struct <Dword194>e__FixedBuffer
    //	{
    //		public int FixedElementField;
    //}

    //[CompilerGenerated, UnsafeValueType]
    //[StructLayout(LayoutKind.Sequential, Size = 384)]
    //public struct <Padding1E0>e__FixedBuffer
    //	{
    //		public int FixedElementField;
    //	}

    //	[CompilerGenerated, UnsafeValueType]
    //[StructLayout(LayoutKind.Sequential, Size = 40)]
    //public struct <RopeGbIds>e__FixedBuffer
    //	{
    //		public int FixedElementField;
    //	}

    //	[CompilerGenerated, UnsafeValueType]
    //[StructLayout(LayoutKind.Sequential, Size = 32)]
    //public struct <AdventureGbIds>e__FixedBuffer
    //	{
    //		public int FixedElementField;
    //	}

    //	[CompilerGenerated, UnsafeValueType]
    //[StructLayout(LayoutKind.Sequential, Size = 24)]
    //public struct <WeatherGbIds>e__FixedBuffer
    //	{
    //		public int FixedElementField;
    //	}

    //	[CompilerGenerated, UnsafeValueType]
    //[StructLayout(LayoutKind.Sequential, Size = 48)]
    //public struct <Ingredients>e__FixedBuffer
    //	{
    //		public int FixedElementField;
    //	}

    //	[CompilerGenerated, UnsafeValueType]
    //[StructLayout(LayoutKind.Sequential, Size = 48)]
    //public struct <Ingredients2>e__FixedBuffer
    //	{
    //		public int FixedElementField;
    //	}

    //	[CompilerGenerated, UnsafeValueType]
    //[StructLayout(LayoutKind.Sequential, Size = 8)]
    //public struct <SerializableData4F8>e__FixedBuffer
    //	{
    //		public int FixedElementField;
    //	}

    //	[CompilerGenerated, UnsafeValueType]
    //[StructLayout(LayoutKind.Sequential, Size = 8)]
    //public struct <SerializableData508>e__FixedBuffer
    //	{
    //		public int FixedElementField;
    //	}

    //	[FixedBuffer(typeof(byte), 256)]
    //public ACDItem.ItemRecord.<InternalName>e__FixedBuffer InternalName;

    //public int dword100;

    //public int dword104;

    //public int ActorSNO1;

    //public int ItemTypeGBId;

    //public int Flags;

    //public int Dword114;

    //public int ItemLevel;

    //public Act WorldType;

    //public int Dword120;

    //public int Dword124;

    //public int Dword128;

    //public int Dword12C;

    //public int Dword130;

    //public int StackSize;

    //public int BaseGoldValue;

    //public int Dword13C;

    //public int Dword140;

    //public int UnsocketCost;

    //public int RequiredLevel;

    //public int Dword14C;

    //public int DurabilityMin;

    //public int DurabilityDelta;

    //public int Dword158;

    //public int Dword15C;

    //public int Dword160;

    //public int Dword164;

    //public int Dword168;

    //public int Dword16C;

    //public int Dword170;

    //public int AdventureGbId;

    //public int SceneGbId;

    //public int TreasureClassNormal;

    //public int TreasureClassMagic;

    //public int TreasureClassRare;

    //public int TreasureClassLegendary;

    //public int RareNamePrefixStringList;

    //public int RareNameSuffixStringList;

    //[FixedBuffer(typeof(int), 4)]
    //public ACDItem.ItemRecord.<Dword194>e__FixedBuffer Dword194;

    //public float float1A4;

    //public float float1A8;

    //public float float1AC;

    //public float float1B0;

    //public float float1B4;

    //public float float1B8;

    //public float float1BC;

    //public int PowerSNO1;

    //public int Dword1C4;

    //public int PowerSNO2;

    //public int Dword1CC;

    //public int PowerSNO3;

    //public int Dword1D4;

    //public int PowerSNO4;

    //public int Dword1DC;

    //[FixedBuffer(typeof(int), 96)]
    //private ACDItem.ItemRecord.<Padding1E0>e__FixedBuffer ‏‮⁪‬‌⁫‍‍‫⁯⁫‫⁯‬‮⁯‬​‏‬‪‭‏‪‌⁬⁫‫‪‎‪⁬‭⁫⁬‪⁯​‎⁮‮;

    //	public ItemQuality ItemQuality;

    //[FixedBuffer(typeof(int), 10)]
    //public ACDItem.ItemRecord.<RopeGbIds>e__FixedBuffer RopeGbIds;

    //public int LightGbId;

    //public int Adventure2GbId;

    //[FixedBuffer(typeof(int), 8)]
    //public ACDItem.ItemRecord.<AdventureGbIds>e__FixedBuffer AdventureGbIds;

    //public int Dword3B4;

    //public int Dword3B8;

    //public int Dword3BC;

    //public int Dword3C0;

    //public int Dword3C4;

    //public int Dword3C8;

    //public int Dword3CC;

    //public int Dword3D0;

    //[FixedBuffer(typeof(int), 6)]
    //public ACDItem.ItemRecord.<WeatherGbIds>e__FixedBuffer WeatherGbIds;

    //public int Dword3EC;

    //public int Dword3F0;

    //public int Dword3F4;

    //public int Dword3F8;

    //public int Dword3FC;

    //public int Dword400;

    //public int Dword404;

    //public int Dword408;

    //public int Dword40C;

    //public int Dword410;

    //public int Dword414;

    //public int Dword418;

    //public int GbId41C;

    //public GemType GemType;

    //public int Dword424;

    //public UnkEnum_ABCD UnkEnum_ABCD;

    //public int ActorSNO2;

    //public int WorldsSNO;

    //public int Worlds2SNO;

    //public int LevelAreaSNO;

    //public int Dword43C;

    //public int Dword440;

    //public int Dword444;

    //public int StirngListGbId;

    //public float Float44C;

    //public float Float450;

    //public float Float454;

    //public float Float458;

    //public float Float45C;

    //public float Float460;

    //public float Float464;

    //public float Float468;

    //public float Float46C;

    //public float Float470;

    //public int Dword474;

    //[FixedBuffer(typeof(int), 12)]
    //public ACDItem.ItemRecord.<Ingredients>e__FixedBuffer Ingredients;

    //public int Dword4A8;

    //[FixedBuffer(typeof(int), 12)]
    //public ACDItem.ItemRecord.<Ingredients2>e__FixedBuffer Ingredients2;

    //public int LevelArea2SNO;

    //public int LevelArea3SNO;

    //public int ActorSNO3;

    //public int Dword4E8;

    //public int Dword4EC;

    //public int Dword4F0;

    //public int AttribParam4F4;

    //public int Formula4F8;

    //public int Formula4FC;

    //[FixedBuffer(typeof(int), 2)]
    //public ACDItem.ItemRecord.<SerializableData4F8>e__FixedBuffer SerializableData4F8;

    //public int Translatable508;

    //public int AttribParam50C;

    //public int Formula510;

    //public int Dword514;

    //[FixedBuffer(typeof(int), 2)]
    //public ACDItem.ItemRecord.<SerializableData508>e__FixedBuffer SerializableData508;

    //public int Dword520;

    //public int Dword524;
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //public struct ItemRecord
    //{
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    //    public byte[] InternalName;
    //    public int GameBalanceId;
    //    public int dword104;
    //    public int ActorSNO1;
    //    public int ItemTypeGBId;
    //    public int Flags;
    //    public int Dword114;
    //    public int ItemLevel;
    //    public Act WorldType;
    //    public int Dword120;
    //    public int Dword124;
    //    public int Dword128;
    //    public int Dword12C;
    //    public int Dword130;
    //    public int StackSize;
    //    public int BaseGoldValue;
    //    public int Dword13C;
    //    public int Dword140;
    //    public int UnsocketCost;
    //    public int RequiredLevel;
    //    public int Dword14C;
    //    public int DurabilityMin;
    //    public int DurabilityDelta;
    //    public int Dword158;
    //    public int Dword15C;
    //    public int Dword160;
    //    public int Dword164;
    //    public int Dword168;
    //    public int Dword16C;
    //    public int Dword170;
    //    public int AdventureGbId;
    //    public int SceneGbId;
    //    public int TreasureClassNormal;
    //    public int TreasureClassMagic;
    //    public int TreasureClassRare;
    //    public int TreasureClassLegendary;
    //    public int RareNamePrefixStringList;
    //    public int RareNameSuffixStringList;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    //    public int[] Dword194;
    //    public float float1A4;
    //    public float float1A8;
    //    public float float1AC;
    //    public float float1B0;
    //    public float float1B4;
    //    public float float1B8;
    //    public float float1BC;
    //    public int PowerSNO1;
    //    public int Dword1C4;
    //    public int PowerSNO2;
    //    public int Dword1CC;
    //    public int PowerSNO3;
    //    public int Dword1D4;
    //    public int PowerSNO4;
    //    public int Dword1DC;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
    //    public int[] unknown1;
    //    public ItemQuality ItemQuality;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    //    public int[] RopeGbIds;
    //    public int LightGbId;
    //    public int Adventure2GbId;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    //    public int[] AdventureGbIds;
    //    public int Dword3B4;
    //    public int Dword3B8;
    //    public int Dword3BC;
    //    public int Dword3C0;
    //    public int Dword3C4;
    //    public int Dword3C8;
    //    public int Dword3CC;
    //    public int Dword3D0;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    //    public int[] WeatherGbIds;
    //    public int Dword3EC;
    //    public int Dword3F0;
    //    public int Dword3F4;
    //    public int Dword3F8;
    //    public int Dword3FC;
    //    public int Dword400;
    //    public int Dword404;
    //    public int Dword408;
    //    public int Dword40C;
    //    public int Dword410;
    //    public int Dword414;
    //    public int Dword418;
    //    public int GbId41C;
    //    public int Unknown; //GemType GemType;
    //    public int Dword424;
    //    public UnkEnum_ABCD UnkEnum_ABCD;
    //    public int ActorSNO2;
    //    public GemType GemType; /* */
    //    public int Worlds2SNO;
    //    public int LevelAreaSNO;
    //    public int Dword43C;
    //    public int Dword440;
    //    public int Dword444;
    //    public int StirngListGbId;
    //    public float Float44C;
    //    public float Float450;
    //    public float Float454;
    //    public float Float458;
    //    public float Float45C;
    //    public float Float460;
    //    public float Float464;
    //    public float Float468;
    //    public float Float46C;
    //    public float Float470;
    //    public int Dword474;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    //    public int[] Ingredients;
    //    public int Dword4A8;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    //    public int[] Ingredients2;
    //    public int LevelArea2SNO;
    //    public int LevelArea3SNO;
    //    public int ActorSNO3;
    //    public int Dword4E8;
    //    public int Dword4EC;
    //    public int Dword4F0;
    //    public int AttribParam4F4;
    //    public int Formula4F8;
    //    public int Formula4FC;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    //    public int[] SerializableData4F8;
    //    public int Translatable508;
    //    public int AttribParam50C;
    //    public int Formula510;
    //    public int Dword514;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    //    public int[] SerializableData508;
    //    public int Dword520;
    //    public int Dword524;
    //}
}