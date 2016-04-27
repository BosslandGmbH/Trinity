using System.Runtime.InteropServices;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory.Misc
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
        [FieldOffset(0x170)]
        public int SetItemBonusesGameBalanceId;
        [FieldOffset(0x184)]
        public int RareItemPartAPrefixStringListId;
        [FieldOffset(0x188)]
        public int RareItemPartASuffixStringListId;
        [FieldOffset(0x138)]
        public int BaseGoldValue;
        [FieldOffset(0x148)]
        public int ItemLevel;
        [FieldOffset(0x134)]
        public int StackSize;
        [FieldOffset(0x430)]
        public GemType GemType;
    }

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
