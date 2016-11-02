using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Sno.Helpers;
using Trinity.Framework.Objects.Memory.Symbols.Types;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using MonsterRace = Zeta.Game.Internals.SNO.MonsterRace;
using MonsterSize = Zeta.Game.Internals.SNO.MonsterSize;
using MonsterType = Zeta.Game.Internals.SNO.MonsterType;
using ResourceType = Trinity.Framework.Objects.Memory.Symbols.Types.ResourceType;

namespace Trinity.Framework.Objects.Memory.Sno.Types
{

    public class TestItem : MemoryWrapper, ITableItem
    {
        public int ModKey => ReadOffset<int>(0x00);
        public string Value => ReadString(0x34);
    }

    public class TestObject : NativeObject
    {
        public TestObject(IntPtr ptr) : base(IntPtr.Zero)
        {

        }
    }

    [CompilerGenerated]
    public class NativeActor : SnoTableEntry
    {
        public const int SizeOf = 872; // 0x368
        public int _1_0xC_int => ReadOffset<int>(0xC); //      Flags=524289  
        public ActorType _2_0x10_Enum => ReadOffset<ActorType>(0x10); //      Flags=17  SymbolTable=@26705656 Max=11
        public int _3_0x14_Appearance_Sno => ReadOffset<int>(0x14); //      Flags=257  
        public int _4_0x18_PhysMesh_Sno => ReadOffset<int>(0x18); //      Flags=1  
        public NativeAxialCylinder _5_0x1C_Object => ReadObject<NativeAxialCylinder>(0x1C); //      Flags=1  
        public NativeSphere _6_0x30_Object => ReadObject<NativeSphere>(0x30); //      Flags=1  
        public NativeAABB _7_0x40_Object => ReadObject<NativeAABB>(0x40);
        public NativeSerializeData _8_0x58_SerializeData => ReadObject<NativeSerializeData>(0x58);
        //public TagMap _9_0x60_TagMap => ReadOffset<TagMap>(0x60); //    VarArrSerializeOffsetDiff=-8   Flags=33  
        public int _10_0x68_AnimSet_Sno => ReadOffset<int>(0x68); //      Flags=1  
        public int _11_0x6C_Monster_Sno => ReadOffset<int>(0x6C); //      Flags=1  
        public NativeSerializeData _12_0x70_SerializeData => ReadObject<NativeSerializeData>(0x70);
        public int _13_0x78_NativeMsgTriggeredEvent => ReadOffset<int>(0x78); //    VarArrSerializeOffsetDiff=-8   Flags=65536  
        public List<NativeMsgTriggeredEvent> _14_0x80_VariableArray => ReadSerializedObjects<NativeMsgTriggeredEvent>(0x80, 0x70); //    VarArrSerializeOffsetDiff=-16   Flags=33  
        public Vector3 _15_0x88_Vector3 => ReadOffset<Vector3>(0x88); //      Flags=1  
        public List<NativeWeightedLook> _16_0x94_FixedArray => ReadObjects<NativeWeightedLook>(0x94, 8); //      Flags=1  
        public int _17_0x2B4_Physics_Sno => ReadOffset<int>(0x2B4); //      Flags=1  
        public int _18_0x2B8_int => ReadOffset<int>(0x2B8); //      Flags=524289  
        public int _19_0x2BC_int => ReadOffset<int>(0x2BC); //      Flags=1  
        public float _20_0x2C0_float => ReadOffset<float>(0x2C0); //      Flags=1  
        public float _21_0x2C4_float => ReadOffset<float>(0x2C4); //      Flags=1  
        public float _22_0x2C8_float => ReadOffset<float>(0x2C8); //      Flags=1  
        public NativeActorCollisionData _23_0x2CC_Object => ReadObject<NativeActorCollisionData>(0x2CC); //      Flags=1  
        public List<NativeInventoryImages> _24_0x310_FixedArray => ReadObjects<NativeInventoryImages>(0x310, 6); //      Flags=1  
        public int _25_0x340_int => ReadOffset<int>(0x340); //      Flags=1  
        public string _26_0x348_SerializedString => ReadSerializedString(0x348, 0x350); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _27_0x350_SerializeData => ReadObject<NativeSerializeData>(0x350);
        public string _28_0x358_SerializedString => ReadSerializedString(0x358, 0x360); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _29_0x360_SerializeData => ReadObject<NativeSerializeData>(0x360);
    }


    [CompilerGenerated]
    public class NativeAxialCylinder : MemoryWrapper
    {
        public const int SizeOf = 20; // 0x14
        public Vector3 _1_0x0_Vector3 => ReadOffset<Vector3>(0x0); //      Flags=1  
        public float _2_0xC_float => ReadOffset<float>(0xC); //      Flags=1  
        public float _3_0x10_float => ReadOffset<float>(0x10); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeSphere : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public Vector3 _1_0x0_Vector3 => ReadOffset<Vector3>(0x0); //      Flags=1  
        public float _2_0xC_float => ReadOffset<float>(0xC); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeAABB : MemoryWrapper
    {
        public const int SizeOf = 24; // 0x18
        public Vector3 _1_0x0_Vector3 => ReadOffset<Vector3>(0x0); //      Flags=1  
        public Vector3 _2_0xC_Vector3 => ReadOffset<Vector3>(0xC); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeSerializeData : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
        public int Offset => ReadOffset<int>(0x0); //      Flags=1  _1_0x0_int
        public int Length => ReadOffset<int>(0x4); //      Flags=1   _2_0x4_int
        public IntPtr FirstEntry => ReadOffset<IntPtr>(0x8);
    }


    [CompilerGenerated]
    public class NativeMsgTriggeredEvent : MemoryWrapper
    {
        public const int SizeOf = 412; // 0x19C
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public NativeTriggerEvent _2_0x4_Object => ReadObject<NativeTriggerEvent>(0x4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeWeightedLook : MemoryWrapper
    {
        public const int SizeOf = 68; // 0x44
        public NativeLookLink _1_0x0_Object => ReadObject<NativeLookLink>(0x0); //      Flags=1  
        public int _2_0x40_int => ReadOffset<int>(0x40); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeActorCollisionData : MemoryWrapper
    {
        public const int SizeOf = 68; // 0x44
        public NativeActorCollisionFlags _1_0x0_Object => ReadObject<NativeActorCollisionFlags>(0x0); //      Flags=1  
        public int _2_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
        public NativeAxialCylinder _3_0x14_Object => ReadObject<NativeAxialCylinder>(0x14); //      Flags=1  
        public NativeAABB _4_0x28_Object => ReadObject<NativeAABB>(0x28); //      Flags=1  
        public float _5_0x40_float => ReadOffset<float>(0x40); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeInventoryImages : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeTriggerEvent : MemoryWrapper
    {
        public const int SizeOf = 408; // 0x198
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public NativeTriggerConditions _2_0x4_Object => ReadObject<NativeTriggerConditions>(0x4); //      Flags=1  
        public int _3_0x28_int => ReadOffset<int>(0x28); //      Flags=1  
        public NativeSNOName _4_0x2C_Object => ReadObject<NativeSNOName>(0x2C); //      Flags=1  
        public int _5_0x34_int => ReadOffset<int>(0x34); //      Flags=1  
        public int _6_0x38_int => ReadOffset<int>(0x38); //      Flags=524289  
        public int _7_0x3C_int => ReadOffset<int>(0x3C);
        public int _8_0x40_int => ReadOffset<int>(0x40);
        public List<NativeHardpointLink> _9_0x44_FixedArray => ReadObjects<NativeHardpointLink>(0x44, 2); //      Flags=1  
        public NativeLookLink _10_0xCC_Object => ReadObject<NativeLookLink>(0xCC); //      Flags=1  
        public NativeConstraintLink _11_0x10C_Object => ReadObject<NativeConstraintLink>(0x10C); //      Flags=1  
        public int _12_0x14C_int => ReadOffset<int>(0x14C); //      Flags=1  
        public float _13_0x150_float => ReadOffset<float>(0x150); //      Flags=1  
        public int _14_0x154_int => ReadOffset<int>(0x154); //      Flags=1  
        public int _15_0x158_int => ReadOffset<int>(0x158); //      Flags=1  
        public int _16_0x15C_int => ReadOffset<int>(0x15C); //      Flags=524289  
        public int _17_0x160_int => ReadOffset<int>(0x160); //      Flags=1  
        public int _18_0x164_int => ReadOffset<int>(0x164); //      Flags=1  
        public float _19_0x168_float => ReadOffset<float>(0x168); //      Flags=1  
        public float _20_0x16C_float => ReadOffset<float>(0x16C); //      Flags=1  
        public int _21_0x170_int => ReadOffset<int>(0x170); //      Flags=1  
        public float _22_0x174_float => ReadOffset<float>(0x174); //      Flags=1  
        public int _23_0x178_int => ReadOffset<int>(0x178); //      Flags=1  
                                                            // Unknown index=24 @Offset0x17C Type=DT_VELOCITY BaseType=DT_NULL //      Flags=1  
        public int _25_0x180_int => ReadOffset<int>(0x180); //      Flags=1  
                                                            // Unknown index=26 @Offset0x184 Type=DT_TIME BaseType=DT_NULL //      Flags=1  
                                                            // Unknown index=27 @Offset0x188 Type=DT_RGBACOLOR BaseType=DT_NULL //      Flags=1  
                                                            // Unknown index=28 @Offset0x18C Type=DT_TIME BaseType=DT_NULL //      Flags=1  
                                                            // Unknown index=29 @Offset0x190 Type=DT_RGBACOLOR BaseType=DT_NULL //      Flags=1  
                                                            // Unknown index=30 @Offset0x194 Type=DT_TIME BaseType=DT_NULL //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeLookLink : MemoryWrapper
    {
        public const int SizeOf = 64; // 0x40
        public string _1_0x0_String => ReadString(0x0); //      Flags=2049  
    }


    [CompilerGenerated]
    public class NativeActorCollisionFlags : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=524289  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=524289  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=524289  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=524289  
    }


    [CompilerGenerated]
    public class NativeTriggerConditions : MemoryWrapper
    {
        public const int SizeOf = 36; // 0x24
                                // Unknown index=1 @Offset0x0 Type=DT_PERCENT BaseType=DT_NULL //      Flags=1  
                                // Unknown index=2 @Offset0x4 Type=DT_TIME BaseType=DT_NULL //      Flags=1  
                                // Unknown index=3 @Offset0x8 Type=DT_TIME BaseType=DT_NULL //      Flags=1  
                                // Unknown index=4 @Offset0xC Type=DT_TIME BaseType=DT_NULL //      Flags=1  
                                // Unknown index=5 @Offset0x10 Type=DT_TIME BaseType=DT_NULL //      Flags=1  
                                // Unknown index=6 @Offset0x14 Type=DT_IMPULSE BaseType=DT_NULL //      Flags=1  
                                // Unknown index=7 @Offset0x18 Type=DT_IMPULSE BaseType=DT_NULL //      Flags=1  
        public int _8_0x1C_int => ReadOffset<int>(0x1C); //      Flags=1  
        public int _9_0x20_int => ReadOffset<int>(0x20); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeSNOName : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
                               // Unknown index=1 @Offset0x0 Type=DT_SNO_GROUP BaseType=DT_NULL //      Flags=1  
                               // Unknown index=2 @Offset0x4 Type=DT_SNONAME_HANDLE BaseType=DT_NULL //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeHardpointLink : MemoryWrapper
    {
        public const int SizeOf = 68; // 0x44
        public string _1_0x0_String => ReadString(0x0); //      Flags=2049  
        public int _2_0x40_int => ReadOffset<int>(0x40); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeConstraintLink : MemoryWrapper
    {
        public const int SizeOf = 64; // 0x40
        public string _1_0x0_String => ReadString(0x0); //      Flags=2049  
    }


    [CompilerGenerated]
    public class NativeMonster : SnoTableEntry
    {
        public const int SizeOf = 1344; // 0x540
        public int _1_0xC_int => ReadOffset<int>(0xC); //      Flags=524289  
        public int _2_0x10_Actor_Sno => ReadOffset<int>(0x10); //      Flags=1  
        public int _3_0x14_int => ReadOffset<int>(0x14); //      Flags=1  
        public MonsterType _4_0x18_Enum => ReadOffset<MonsterType>(0x18); //      Flags=17  SymbolTable=@26700864 Min=-1Max=12
        public MonsterRace _5_0x1C_Enum => ReadOffset<MonsterRace>(0x1C); //      Flags=17  SymbolTable=@26700984 Max=12
        public MonsterSize _6_0x20_Enum => ReadOffset<MonsterSize>(0x20); //      Flags=17  SymbolTable=@26701320 Min=-1Max=7
        public NativeMonsterExtra _7_0x24_Object => ReadObject<NativeMonsterExtra>(0x24); //      Flags=1  
        public ElementType _8_0x38_Enum => ReadOffset<ElementType>(0x38); //      Flags=17  SymbolTable=@26701096 Min=-1Max=6
        public int _9_0x3C_int => ReadOffset<int>(0x3C); //      Flags=1  
        public int _10_0x40_int => ReadOffset<int>(0x40); //      Flags=1  
        public List<float> _11_0x44_FixedArray => ReadArray<float>(0x44, 146); //      Flags=1  
        public float _12_0x28C_float => ReadOffset<float>(0x28C); //      Flags=1  
        public float _13_0x290_float => ReadOffset<float>(0x290); //      Flags=1  
        public float _14_0x294_float => ReadOffset<float>(0x294); //      Flags=1  
        public float _15_0x298_float => ReadOffset<float>(0x298); //      Flags=1  
        public float _16_0x29C_float => ReadOffset<float>(0x29C); //      Flags=1  
        public float _17_0x2A0_float => ReadOffset<float>(0x2A0); //      Flags=1  
        public int _18_0x2A4_int => ReadOffset<int>(0x2A4); //      Flags=1  
        public NativeHealthDropInfo _19_0x2A8_Object => ReadObject<NativeHealthDropInfo>(0x2A8); //      Flags=1  
        public NativeHealthDropInfo _20_0x2B4_Object => ReadObject<NativeHealthDropInfo>(0x2B4); //      Flags=1  
        public NativeHealthDropInfo _21_0x2C0_Object => ReadObject<NativeHealthDropInfo>(0x2C0); //      Flags=1  
        public NativeHealthDropInfo _22_0x2CC_Object => ReadObject<NativeHealthDropInfo>(0x2CC); //      Flags=1  
        public int _23_0x2D8_SkillKit_Sno => ReadOffset<int>(0x2D8); //      Flags=1  
        public List<NativeSkillDeclaration> _24_0x2DC_FixedArray => ReadObjects<NativeSkillDeclaration>(0x2DC, 8); //      Flags=1  
        public List<NativeMonsterSkillDeclaration> _25_0x31C_FixedArray => ReadObjects<NativeMonsterSkillDeclaration>(0x31C, 8); //      Flags=1  
        public int _26_0x39C_TreasureClass_Sno => ReadOffset<int>(0x39C); //      Flags=1  
        public int _27_0x3A0_TreasureClass_Sno => ReadOffset<int>(0x3A0); //      Flags=1  
        public int _28_0x3A4_TreasureClass_Sno => ReadOffset<int>(0x3A4); //      Flags=1  
        public int _29_0x3A8_TreasureClass_Sno => ReadOffset<int>(0x3A8); //      Flags=1  
        public int _30_0x3AC_TreasureClass_Sno => ReadOffset<int>(0x3AC); //      Flags=1  
        public float _31_0x3B0_float => ReadOffset<float>(0x3B0); //      Flags=1  
        public float _32_0x3B4_float => ReadOffset<float>(0x3B4); //      Flags=1  
        public float _33_0x3B8_float => ReadOffset<float>(0x3B8); //      Flags=1  
        public float _34_0x3BC_float => ReadOffset<float>(0x3BC); //      Flags=1  
        public int _35_0x3C0_int => ReadOffset<int>(0x3C0); //      Flags=1  
        public float _36_0x3C4_float => ReadOffset<float>(0x3C4); //      Flags=1  
        public int _37_0x3C8_int => ReadOffset<int>(0x3C8); //      Flags=1  
        public int _38_0x3CC_int => ReadOffset<int>(0x3CC); //      Flags=1  
        public int _39_0x3D0_TreasureClass_Sno => ReadOffset<int>(0x3D0); //      Flags=1  
        public int _40_0x3D4_TreasureClass_Sno => ReadOffset<int>(0x3D4); //      Flags=1  
        public int _41_0x3D8_Lore_Sno => ReadOffset<int>(0x3D8); //      Flags=1  
        //public List<NativeAiBehavior> _42_0x3DC_FixedArray => ReadObjects<NativeAiBehavior>(0x3DC, 6); //      Flags=1  
        public List<int> _43_0x3F4_FixedArray => ReadArray<int>(0x3F4, 8); //      Flags=1  
        public List<NativeActor> _44_0x414_FixedArray => ReadObjects<NativeActor>(0x414, 6); //      Flags=1  
        public int _45_0x42C_int => ReadOffset<int>(0x42C); //      Flags=1  
        public List<int> _46_0x430_FixedArray => ReadArray<int>(0x430, 4); //      Flags=1  
        public List<int> _47_0x440_FixedArray => ReadArray<int>(0x440, 6); //      Flags=1  
        public int _48_0x458_int => ReadOffset<int>(0x458); //      Flags=1  
        public int _49_0x45C_int => ReadOffset<int>(0x45C); //      Flags=1  
        public int _50_0x460_int => ReadOffset<int>(0x460); //      Flags=1  
        public ResourceType _51_0x464_Enum => ReadOffset<ResourceType>(0x464); //      Flags=17  SymbolTable=@26702016 Max=7
        public NativeSerializeData _52_0x480_SerializeData => ReadObject<NativeSerializeData>(0x480);
        //    public TagMap _53_0x488_TagMap => ReadOffset<TagMap>(0x488); //    VarArrSerializeOffsetDiff=-8   Flags=33  
        public int _54_0x490_NativeMonsterMinionSpawnGroup => ReadOffset<int>(0x490); //    VarArrSerializeOffsetDiff=16   Flags=65536  
        public List<NativeMonsterMinionSpawnGroup> _55_0x498_VariableArray => ReadSerializedObjects<NativeMonsterMinionSpawnGroup>(0x498, 0x4A0); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _56_0x4A0_SerializeData => ReadObject<NativeSerializeData>(0x4A0);
        public int _57_0x4A8_NativeMonsterChampionSpawnGroup => ReadOffset<int>(0x4A8); //    VarArrSerializeOffsetDiff=16   Flags=65536  
        public List<NativeMonsterChampionSpawnGroup> _58_0x4B0_VariableArray => ReadSerializedObjects<NativeMonsterChampionSpawnGroup>(0x4B0, 0x4B8); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _59_0x4B8_SerializeData => ReadObject<NativeSerializeData>(0x4B8);
        public string _60_0x4C0_String => ReadString(0x4C0); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeMonsterExtra : MemoryWrapper
    {
        public const int SizeOf = 20; // 0x14
        public float _1_0x0_float => ReadOffset<float>(0x0); //      Flags=1  
        public float _2_0x4_float => ReadOffset<float>(0x4); //      Flags=1  
        public float _3_0x8_float => ReadOffset<float>(0x8); //      Flags=1  
        public float _4_0xC_float => ReadOffset<float>(0xC); //      Flags=1  
        public int _5_0x10_int => ReadOffset<int>(0x10); //      Flags=17  Max=1
    }


    [CompilerGenerated]
    public class NativeHealthDropInfo : MemoryWrapper
    {
        public const int SizeOf = 12; // 0xC
        public float _1_0x0_float => ReadOffset<float>(0x0); //      Flags=1  
        public int _2_0x4_Items_GameBalanceId => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeSkillDeclaration : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
        public int _1_0x0_Power_Sno => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeMonsterSkillDeclaration : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public float _1_0x0_float => ReadOffset<float>(0x0); //      Flags=1  
        public float _2_0x4_float => ReadOffset<float>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public float _4_0xC_float => ReadOffset<float>(0xC); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeMonsterMinionSpawnGroup : MemoryWrapper
    {
        public const int SizeOf = 24; // 0x18
        public float _1_0x0_float => ReadOffset<float>(0x0); //      Flags=1  
        public int _2_0x4_NativeMonsterMinionSpawnItem => ReadOffset<int>(0x4); //    VarArrSerializeOffsetDiff=12   Flags=65536  
        public List<NativeMonsterMinionSpawnItem> _3_0x8_VariableArray => ReadSerializedObjects<NativeMonsterMinionSpawnItem>(0x8, 0x10); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _4_0x10_SerializeData => ReadObject<NativeSerializeData>(0x10);
    }


    [CompilerGenerated]
    public class NativeMonsterChampionSpawnGroup : MemoryWrapper
    {
        public const int SizeOf = 24; // 0x18
        public int _1_0x0_NativeMonsterChampionSpawnItem => ReadOffset<int>(0x0); //    VarArrSerializeOffsetDiff=16   Flags=65536  
        public float _2_0x4_float => ReadOffset<float>(0x4); //      Flags=1  
        public List<NativeMonsterChampionSpawnItem> _3_0x8_VariableArray => ReadSerializedObjects<NativeMonsterChampionSpawnItem>(0x8, 0x10); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _4_0x10_SerializeData => ReadObject<NativeSerializeData>(0x10);
    }


    [CompilerGenerated]
    public class NativeMonsterMinionSpawnItem : MemoryWrapper
    {
        public const int SizeOf = 20; // 0x14
        public int _1_0x0_Actor_Sno => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public int _5_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeMonsterChampionSpawnItem : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
        public int _1_0x0_Actor_Sno => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeHero : SnoTableEntry
    {
        public const int SizeOf = 32; // 0x20
        public NativeSerializeData _1_0xC_SerializeData => ReadObject<NativeSerializeData>(0xC);
        public string _2_0x18_SerializedString => ReadSerializedString(0x18, 0xC); //    VarArrSerializeOffsetDiff=-12   Flags=32  
    }


    [CompilerGenerated]
    public class NativeStringList : SnoTableEntry
    {
        public const int SizeOf = 40; // 0x28
        public List<NativeStringTableEntry> _1_0x10_VariableArray => ReadSerializedObjects<NativeStringTableEntry>(0x10, 0x18); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _2_0x18_SerializeData => ReadObject<NativeSerializeData>(0x18);
    }


    [CompilerGenerated]
    public class NativeStringTableEntry : MemoryWrapper
    {
        public const int SizeOf = 40; // 0x28
        public string _1_0x0_SerializedString => ReadSerializedString(0x0, 0x8); //    VarArrSerializeOffsetDiff=8   Flags=2081  
        public NativeSerializeData _2_0x8_SerializeData => ReadObject<NativeSerializeData>(0x8); //      Flags=2048  
        public string _3_0x10_SerializedString => ReadSerializedString(0x10, 0x18); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _4_0x18_SerializeData => ReadObject<NativeSerializeData>(0x18);
        public int _5_0x20_int => ReadOffset<int>(0x20); //GameBalanceNameNormalHashed
        public int _6_0x24_int => ReadOffset<int>(0x24);
    }


    [CompilerGenerated]
    public class NativeGlobals : SnoTableEntry
    {
        public const int SizeOf = 760; // 0x2F8
        public List<NativeGlobalServerData> _1_0x10_VariableArray => ReadSerializedObjects<NativeGlobalServerData>(0x10, 0x18); //    VarArrSerializeOffsetDiff=8   Flags=1057  
        public NativeSerializeData _2_0x18_SerializeData => ReadObject<NativeSerializeData>(0x18);
        public int _3_0x20_int => ReadOffset<int>(0x20); //      Flags=1  
        public List<NativeStartLocationName> _4_0x28_VariableArray => ReadSerializedObjects<NativeStartLocationName>(0x28, 0x30); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _5_0x30_SerializeData => ReadObject<NativeSerializeData>(0x30);
        public float _6_0x38_float => ReadOffset<float>(0x38); //      Flags=1  
        public float _7_0x3C_float => ReadOffset<float>(0x3C); //      Flags=1  
        public float _8_0x40_float => ReadOffset<float>(0x40); //      Flags=1  
        public float _9_0x44_float => ReadOffset<float>(0x44); //      Flags=1  
        public int _10_0x48_int => ReadOffset<int>(0x48); //      Flags=1  
        public int _11_0x4C_int => ReadOffset<int>(0x4C); //      Flags=1  
        public float _12_0x50_float => ReadOffset<float>(0x50); //      Flags=1  
        public float _13_0x54_float => ReadOffset<float>(0x54); //      Flags=1  
        public int _14_0x58_int => ReadOffset<int>(0x58); //      Flags=1  
        public float _15_0x5C_float => ReadOffset<float>(0x5C); //      Flags=1  
        public float _16_0x60_float => ReadOffset<float>(0x60); //      Flags=1  
        public float _17_0x64_float => ReadOffset<float>(0x64); //      Flags=1  
        public float _18_0x68_float => ReadOffset<float>(0x68); //      Flags=1  
        public float _19_0x6C_float => ReadOffset<float>(0x6C); //      Flags=1  
        public float _20_0x70_float => ReadOffset<float>(0x70); //      Flags=1  
        public float _21_0x74_float => ReadOffset<float>(0x74); //      Flags=1  
        public float _22_0x78_float => ReadOffset<float>(0x78); //      Flags=1  
        public float _23_0x7C_float => ReadOffset<float>(0x7C); //      Flags=1  
        public int _24_0x80_int => ReadOffset<int>(0x80); //      Flags=1  
        public List<int> _25_0x84_FixedArray => ReadArray<int>(0x84, 5); //      Flags=1  
        public NativeBannerParams _26_0x98_Object => ReadObject<NativeBannerParams>(0x98); //      Flags=1  
        public int _27_0x188_int => ReadOffset<int>(0x188); //      Flags=1  
        public int _28_0x18C_int => ReadOffset<int>(0x18C); //      Flags=1  
        public int _29_0x190_int => ReadOffset<int>(0x190); //      Flags=1  
        public int _30_0x194_int => ReadOffset<int>(0x194); //      Flags=1  
        public float _31_0x198_float => ReadOffset<float>(0x198); //      Flags=1  
        public int _32_0x19C_int => ReadOffset<int>(0x19C); //      Flags=1  
        public float _33_0x1A0_float => ReadOffset<float>(0x1A0); //      Flags=1  
        public float _34_0x1A4_float => ReadOffset<float>(0x1A4); //      Flags=1  
        public float _35_0x1A8_float => ReadOffset<float>(0x1A8); //      Flags=1  
        public float _36_0x1AC_float => ReadOffset<float>(0x1AC); //      Flags=1  
        public float _37_0x1B0_float => ReadOffset<float>(0x1B0); //      Flags=1  
        public float _38_0x1B4_float => ReadOffset<float>(0x1B4); //      Flags=1  
        public float _39_0x1B8_float => ReadOffset<float>(0x1B8); //      Flags=1  
        public float _40_0x1BC_float => ReadOffset<float>(0x1BC); //      Flags=1  
        public float _41_0x1C0_float => ReadOffset<float>(0x1C0); //      Flags=1  
        public float _42_0x1C4_float => ReadOffset<float>(0x1C4); //      Flags=1  
        public float _43_0x1C8_float => ReadOffset<float>(0x1C8); //      Flags=1  
        public float _44_0x1CC_float => ReadOffset<float>(0x1CC); //      Flags=1  
        public float _45_0x1D0_float => ReadOffset<float>(0x1D0); //      Flags=1  
        public float _46_0x1D4_float => ReadOffset<float>(0x1D4); //      Flags=1  
        public float _47_0x1D8_float => ReadOffset<float>(0x1D8); //      Flags=1  
        public float _48_0x1DC_float => ReadOffset<float>(0x1DC); //      Flags=1  
        public float _49_0x1E0_float => ReadOffset<float>(0x1E0); //      Flags=1  
        public float _50_0x1E4_float => ReadOffset<float>(0x1E4); //      Flags=1  
        public float _51_0x1E8_float => ReadOffset<float>(0x1E8); //      Flags=1  
        public float _52_0x1EC_float => ReadOffset<float>(0x1EC); //      Flags=1  
        public float _53_0x1F0_float => ReadOffset<float>(0x1F0); //      Flags=1  
        public int _54_0x1F4_int => ReadOffset<int>(0x1F4); //      Flags=1  
        public float _55_0x1F8_float => ReadOffset<float>(0x1F8); //      Flags=1  
        public int _56_0x1FC_int => ReadOffset<int>(0x1FC); //      Flags=1  
        public int _57_0x200_int => ReadOffset<int>(0x200); //      Flags=1  
        public float _58_0x204_float => ReadOffset<float>(0x204); //      Flags=1  
        public float _59_0x208_float => ReadOffset<float>(0x208); //      Flags=1  
        public float _60_0x20C_float => ReadOffset<float>(0x20C); //      Flags=1  
        public float _61_0x210_float => ReadOffset<float>(0x210); //      Flags=1  
        public float _62_0x214_float => ReadOffset<float>(0x214); //      Flags=1  
        public float _63_0x218_float => ReadOffset<float>(0x218); //      Flags=1  
        public float _64_0x21C_float => ReadOffset<float>(0x21C); //      Flags=1  
        public List<NativeAssetList> _65_0x220_FixedArray => ReadObjects<NativeAssetList>(0x220, 9); //      Flags=1  
        public float _66_0x2B0_float => ReadOffset<float>(0x2B0); //      Flags=1  
        public float _67_0x2B4_float => ReadOffset<float>(0x2B4); //      Flags=1  
        public float _68_0x2B8_float => ReadOffset<float>(0x2B8); //      Flags=1  
        public float _69_0x2BC_float => ReadOffset<float>(0x2BC); //      Flags=1  
        public float _70_0x2C0_float => ReadOffset<float>(0x2C0); //      Flags=1  
        public float _71_0x2C4_float => ReadOffset<float>(0x2C4); //      Flags=1  
        public int _72_0x2C8_int => ReadOffset<int>(0x2C8); //      Flags=1  
        public int _73_0x2CC_int => ReadOffset<int>(0x2CC); //      Flags=1  
        public float _74_0x2D0_float => ReadOffset<float>(0x2D0); //      Flags=1  
        public float _75_0x2D4_float => ReadOffset<float>(0x2D4); //      Flags=1  
        public float _76_0x2D8_float => ReadOffset<float>(0x2D8); //      Flags=1  
        public float _77_0x2DC_float => ReadOffset<float>(0x2DC); //      Flags=1  
        public float _78_0x2E0_float => ReadOffset<float>(0x2E0); //      Flags=1  
        public float _79_0x2E4_float => ReadOffset<float>(0x2E4); //      Flags=1  
        public float _80_0x2E8_float => ReadOffset<float>(0x2E8); //      Flags=1  
        public float _81_0x2EC_float => ReadOffset<float>(0x2EC); //      Flags=1  
        public float _82_0x2F0_float => ReadOffset<float>(0x2F0); //      Flags=1  
        public float _83_0x2F4_float => ReadOffset<float>(0x2F4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeGlobalServerData : SnoTableEntry
    {
        public const int SizeOf = 816; // 0x330
        public List<NativeActorGroup> _1_0x0_VariableArray => ReadSerializedObjects<NativeActorGroup>(0x0, 0x8); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _2_0x8_SerializeData => ReadObject<NativeSerializeData>(0x8);
        public List<NativeGlobalScriptVariable> _3_0x10_VariableArray => ReadSerializedObjects<NativeGlobalScriptVariable>(0x10, 0x18); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _4_0x18_SerializeData => ReadObject<NativeSerializeData>(0x18);
        public List<NativeDifficultyTuningParams> _5_0x20_FixedArray => ReadObjects<NativeDifficultyTuningParams>(0x20, 6); //      Flags=1  
        public float _6_0x158_float => ReadOffset<float>(0x158); //      Flags=1  
        public float _7_0x15C_float => ReadOffset<float>(0x15C); //      Flags=1  
        public float _8_0x160_float => ReadOffset<float>(0x160); //      Flags=1  
        public float _9_0x164_float => ReadOffset<float>(0x164); //      Flags=1  
        public float _10_0x168_float => ReadOffset<float>(0x168); //      Flags=1  
        public float _11_0x16C_float => ReadOffset<float>(0x16C); //      Flags=1  
        public float _12_0x170_float => ReadOffset<float>(0x170); //      Flags=1  
        public float _13_0x174_float => ReadOffset<float>(0x174); //      Flags=1  
        public int _14_0x178_int => ReadOffset<int>(0x178); //      Flags=1  
        public float _15_0x17C_float => ReadOffset<float>(0x17C); //      Flags=1  
        public float _16_0x180_float => ReadOffset<float>(0x180); //      Flags=1  
        public float _17_0x184_float => ReadOffset<float>(0x184); //      Flags=1  
        public float _18_0x188_float => ReadOffset<float>(0x188); //      Flags=1  
        public float _19_0x18C_float => ReadOffset<float>(0x18C); //      Flags=1  
        public float _20_0x190_float => ReadOffset<float>(0x190); //      Flags=1  
        public float _21_0x194_float => ReadOffset<float>(0x194); //      Flags=1  
        public float _22_0x198_float => ReadOffset<float>(0x198); //      Flags=1  
        public float _23_0x19C_float => ReadOffset<float>(0x19C); //      Flags=1  
        public float _24_0x1A0_float => ReadOffset<float>(0x1A0); //      Flags=1  
        public int _25_0x1A4_int => ReadOffset<int>(0x1A4); //      Flags=1  
        public int _26_0x1A8_int => ReadOffset<int>(0x1A8); //      Flags=1  
        public int _27_0x1AC_int => ReadOffset<int>(0x1AC); //      Flags=1  
        public float _28_0x1B0_float => ReadOffset<float>(0x1B0); //      Flags=1  
        public float _29_0x1B4_float => ReadOffset<float>(0x1B4); //      Flags=1  
        public float _30_0x1B8_float => ReadOffset<float>(0x1B8); //      Flags=1  
        public float _31_0x1BC_float => ReadOffset<float>(0x1BC); //      Flags=1  
        public float _32_0x1C0_float => ReadOffset<float>(0x1C0); //      Flags=1  
        public float _33_0x1C4_float => ReadOffset<float>(0x1C4); //      Flags=1  
        public float _34_0x1C8_float => ReadOffset<float>(0x1C8); //      Flags=1  
        public int _35_0x1CC_int => ReadOffset<int>(0x1CC); //      Flags=1  
        public float _36_0x1D0_float => ReadOffset<float>(0x1D0); //      Flags=1  
        public int _37_0x1D4_int => ReadOffset<int>(0x1D4); //      Flags=1  
        public int _38_0x1D8_int => ReadOffset<int>(0x1D8); //      Flags=1  
        public int _39_0x1DC_int => ReadOffset<int>(0x1DC); //      Flags=1  
        public int _40_0x1E0_int => ReadOffset<int>(0x1E0); //      Flags=1  
        public int _41_0x1E4_int => ReadOffset<int>(0x1E4); //      Flags=1  
        public int _42_0x1E8_int => ReadOffset<int>(0x1E8); //      Flags=1  
        public int _43_0x1EC_int => ReadOffset<int>(0x1EC); //      Flags=1  
        public int _44_0x1F0_int => ReadOffset<int>(0x1F0); //      Flags=1  
        public int _45_0x1F4_int => ReadOffset<int>(0x1F4); //      Flags=1  
        public float _46_0x1F8_float => ReadOffset<float>(0x1F8); //      Flags=1  
        public float _47_0x1FC_float => ReadOffset<float>(0x1FC); //      Flags=1  
        public List<float> _48_0x200_FixedArray => ReadArray<float>(0x200, 14); //      Flags=1  
        public float _49_0x238_float => ReadOffset<float>(0x238); //      Flags=1  
        public float _50_0x23C_float => ReadOffset<float>(0x23C); //      Flags=1  
        public int _51_0x240_int => ReadOffset<int>(0x240); //      Flags=1  
        public int _52_0x244_int => ReadOffset<int>(0x244); //      Flags=1  
        public int _53_0x248_int => ReadOffset<int>(0x248); //      Flags=1  
        public float _54_0x24C_float => ReadOffset<float>(0x24C); //      Flags=1  
        public int _55_0x250_int => ReadOffset<int>(0x250); //      Flags=1  
        public float _56_0x254_float => ReadOffset<float>(0x254); //      Flags=1  
        public float _57_0x258_float => ReadOffset<float>(0x258); //      Flags=1  
        public float _58_0x25C_float => ReadOffset<float>(0x25C); //      Flags=1  
        public float _59_0x260_float => ReadOffset<float>(0x260); //      Flags=1  
        public float _60_0x264_float => ReadOffset<float>(0x264); //      Flags=1  
        public float _61_0x268_float => ReadOffset<float>(0x268); //      Flags=1  
        public float _62_0x26C_float => ReadOffset<float>(0x26C); //      Flags=1  
        public float _63_0x270_float => ReadOffset<float>(0x270); //      Flags=1  
        public float _64_0x274_float => ReadOffset<float>(0x274); //      Flags=1  
        public float _65_0x278_float => ReadOffset<float>(0x278); //      Flags=1  
        public List<float> _66_0x27C_FixedArray => ReadArray<float>(0x27C, 2); //      Flags=1  
        public float _67_0x284_float => ReadOffset<float>(0x284); //      Flags=1  
        public float _68_0x288_float => ReadOffset<float>(0x288); //      Flags=1  
        public float _69_0x28C_float => ReadOffset<float>(0x28C); //      Flags=1  
        public float _70_0x290_float => ReadOffset<float>(0x290); //      Flags=1  
        public float _71_0x294_float => ReadOffset<float>(0x294); //      Flags=1  
        public float _72_0x298_float => ReadOffset<float>(0x298); //      Flags=1  
        public float _73_0x29C_float => ReadOffset<float>(0x29C); //      Flags=1  
        public float _74_0x2A0_float => ReadOffset<float>(0x2A0); //      Flags=1  
        public int _75_0x2A4_int => ReadOffset<int>(0x2A4); //      Flags=1  
        public int _76_0x2A8_int => ReadOffset<int>(0x2A8); //      Flags=1  
        public int _77_0x2AC_int => ReadOffset<int>(0x2AC); //      Flags=1  
        public int _78_0x2B0_int => ReadOffset<int>(0x2B0); //      Flags=1  
        public int _79_0x2B4_int => ReadOffset<int>(0x2B4); //      Flags=1  
        public int _80_0x2B8_int => ReadOffset<int>(0x2B8); //      Flags=1  
        public float _81_0x2BC_float => ReadOffset<float>(0x2BC); //      Flags=1  
        public float _82_0x2C0_float => ReadOffset<float>(0x2C0); //      Flags=1  
        public float _83_0x2C4_float => ReadOffset<float>(0x2C4); //      Flags=1  
        public List<float> _84_0x2C8_FixedArray => ReadArray<float>(0x2C8, 10); //      Flags=1  
        public float _85_0x2F0_float => ReadOffset<float>(0x2F0); //      Flags=1  
        public int _86_0x2F4_int => ReadOffset<int>(0x2F4); //      Flags=1  
        public int _87_0x2F8_int => ReadOffset<int>(0x2F8); //      Flags=1  
        public int _88_0x2FC_int => ReadOffset<int>(0x2FC); //      Flags=1  
        public int _89_0x300_int => ReadOffset<int>(0x300); //      Flags=1  
        public int _90_0x304_int => ReadOffset<int>(0x304); //      Flags=1  
        public int _91_0x308_int => ReadOffset<int>(0x308); //      Flags=1  
        public int _92_0x30C_int => ReadOffset<int>(0x30C); //      Flags=1  
        public int _93_0x310_int => ReadOffset<int>(0x310); //      Flags=1  
        public int _94_0x314_int => ReadOffset<int>(0x314); //      Flags=1  
        public int _95_0x318_int => ReadOffset<int>(0x318); //      Flags=1  
        public int _96_0x31C_int => ReadOffset<int>(0x31C); //      Flags=1  
        public int _97_0x320_int => ReadOffset<int>(0x320); //      Flags=1  
        public int _98_0x324_int => ReadOffset<int>(0x324); //      Flags=1  
        public int _99_0x328_int => ReadOffset<int>(0x328); //      Flags=1  
        public float _100_0x32C_float => ReadOffset<float>(0x32C); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeStartLocationName : MemoryWrapper
    {
        public const int SizeOf = 68; // 0x44
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public string _2_0x4_String => ReadString(0x4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeBannerParams : MemoryWrapper
    {
        public const int SizeOf = 240; // 0xF0
        public List<NativeBannerTexturePair> _1_0x0_VariableArray => ReadSerializedObjects<NativeBannerTexturePair>(0x0, 0x8); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _2_0x8_SerializeData => ReadObject<NativeSerializeData>(0x8);
        public int _3_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
        public List<NativeBannerTexturePair> _4_0x18_VariableArray => ReadSerializedObjects<NativeBannerTexturePair>(0x18, 0x20); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _5_0x20_SerializeData => ReadObject<NativeSerializeData>(0x20);
        public int _6_0x28_int => ReadOffset<int>(0x28); //      Flags=1  
        public List<NativeBannerTexturePair> _7_0x30_VariableArray => ReadSerializedObjects<NativeBannerTexturePair>(0x30, 0x38); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _8_0x38_SerializeData => ReadObject<NativeSerializeData>(0x38);
        public List<NativeBannerTexturePair> _9_0x40_VariableArray => ReadSerializedObjects<NativeBannerTexturePair>(0x40, 0x48); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _10_0x48_SerializeData => ReadObject<NativeSerializeData>(0x48);
        public int _11_0x50_int => ReadOffset<int>(0x50); //      Flags=1  
        public List<NativeBannerTexturePair> _12_0x58_VariableArray => ReadSerializedObjects<NativeBannerTexturePair>(0x58, 0x60); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _13_0x60_SerializeData => ReadObject<NativeSerializeData>(0x60);
        public int _14_0x68_int => ReadOffset<int>(0x68); //      Flags=1  
        public List<NativeBannerColorSet> _15_0x70_VariableArray => ReadSerializedObjects<NativeBannerColorSet>(0x70, 0x78); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _16_0x78_SerializeData => ReadObject<NativeSerializeData>(0x78);
        public List<NativeBannerSigilPlacement> _17_0x80_VariableArray => ReadSerializedObjects<NativeBannerSigilPlacement>(0x80, 0x88); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _18_0x88_SerializeData => ReadObject<NativeSerializeData>(0x88);
        public List<NativeActor> _19_0x90_VariableArray => ReadSerializedObjects<NativeActor>(0x90, 0x98); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _20_0x98_SerializeData => ReadObject<NativeSerializeData>(0x98);
        public List<NativeActor> _21_0xA0_VariableArray => ReadSerializedObjects<NativeActor>(0xA0, 0xA8); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _22_0xA8_SerializeData => ReadObject<NativeSerializeData>(0xA8);
        public List<NativeActor> _23_0xB0_VariableArray => ReadSerializedObjects<NativeActor>(0xB0, 0xB8); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _24_0xB8_SerializeData => ReadObject<NativeSerializeData>(0xB8);
        public List<NativeActor> _25_0xC0_VariableArray => ReadSerializedObjects<NativeActor>(0xC0, 0xC8); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _26_0xC8_SerializeData => ReadObject<NativeSerializeData>(0xC8);
        public List<NativeEpicBannerDescription> _27_0xD0_VariableArray => ReadSerializedObjects<NativeEpicBannerDescription>(0xD0, 0xD8); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _28_0xD8_SerializeData => ReadObject<NativeSerializeData>(0xD8);
    }


    [CompilerGenerated]
    public class NativeAssetList : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public List<NativeAssetListEntry> _1_0x0_VariableArray => ReadSerializedObjects<NativeAssetListEntry>(0x0, 0x8); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _2_0x8_SerializeData => ReadObject<NativeSerializeData>(0x8);
    }


    [CompilerGenerated]
    public class NativeActorGroup : MemoryWrapper
    {
        public const int SizeOf = 68; // 0x44
        public int _1_0x0_int => ReadOffset<int>(0x0);
        public string _2_0x4_String => ReadString(0x4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeGlobalScriptVariable : MemoryWrapper
    {
        public const int SizeOf = 40; // 0x28
        public int _1_0x0_int => ReadOffset<int>(0x0);
        public string _2_0x4_String => ReadString(0x4); //      Flags=1  
        public float _3_0x24_float => ReadOffset<float>(0x24); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeDifficultyTuningParams : MemoryWrapper
    {
        public const int SizeOf = 52; // 0x34
        public float _1_0x0_float => ReadOffset<float>(0x0); //      Flags=1  
        public float _2_0x4_float => ReadOffset<float>(0x4); //      Flags=1  
        public float _3_0x8_float => ReadOffset<float>(0x8); //      Flags=1  
        public float _4_0xC_float => ReadOffset<float>(0xC); //      Flags=1  
        public float _5_0x10_float => ReadOffset<float>(0x10); //      Flags=1  
        public float _6_0x14_float => ReadOffset<float>(0x14); //      Flags=1  
        public float _7_0x18_float => ReadOffset<float>(0x18); //      Flags=1  
        public float _8_0x1C_float => ReadOffset<float>(0x1C); //      Flags=1  
        public float _9_0x20_float => ReadOffset<float>(0x20); //      Flags=1  
        public float _10_0x24_float => ReadOffset<float>(0x24); //      Flags=1  
        public float _11_0x28_float => ReadOffset<float>(0x28); //      Flags=1  
        public float _12_0x2C_float => ReadOffset<float>(0x2C); //      Flags=1  
        public float _13_0x30_float => ReadOffset<float>(0x30); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeBannerTexturePair : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public int _1_0x0_Textures_Sno => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeBannerColorSet : MemoryWrapper
    {
        public const int SizeOf = 84; // 0x54

        //public List<TypeNameNotHandled(DT_RGBACOLOR:DT_RGBACOLOR)> _1_0x0_FixedArray => ReadObjects<TypeNameNotHandled(DT_RGBACOLOR:DT_RGBACOLOR)>(0x0, 2); //      Flags=1  
        public string _2_0x8_String => ReadString(0x8); //      Flags=1  
        public int _3_0x48_int => ReadOffset<int>(0x48); //      Flags=1  
        public int _4_0x4C_int => ReadOffset<int>(0x4C); //      Flags=524289  
    }


    [CompilerGenerated]
    public class NativeBannerSigilPlacement : MemoryWrapper
    {
        public const int SizeOf = 68; // 0x44
        public string _1_0x0_String => ReadString(0x0); //      Flags=1  
        public int _2_0x40_int => ReadOffset<int>(0x40); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeEpicBannerDescription : MemoryWrapper
    {
        public const int SizeOf = 148; // 0x94
        public int _1_0x0_Textures_Sno => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_Actor_Sno => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_Actor_Sno => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public string _5_0x10_String => ReadString(0x10); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeAssetListEntry : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
    }

    [CompilerGenerated]
    public class NativeGameBalance : SnoTableEntry
    {
        public const int SizeOf = 552; // 0x228
        public override string ToString() => $"{GetType().Name}: {(GameBalanceTableId)Header.SnoId}";             
        public int _1_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public int _2_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
        public int _3_0x14_int => ReadOffset<int>(0x14); //      Flags=1  
        public NativeItemsTable _4_0x18_NativeItemsTable => ReadObject<NativeItemsTable>(0x18);
        public NativeItemsTable _5_0x28_NativeItemsTable => ReadObject<NativeItemsTable>(0x28);
        public NativeExperienceTable _6_0x38_NativeExperienceTable => ReadObject<NativeExperienceTable>(0x38);
        public NativeExperienceTableAlt _7_0x48_NativeExperienceTableAlt => ReadObject<NativeExperienceTableAlt>(0x48);
        public NativeHelpCodes _8_0x58_NativeHelpCodes => ReadObject<NativeHelpCodes>(0x58);
        public NativeMonsterLevelTable _9_0x68_NativeMonsterLevelTable => ReadObject<NativeMonsterLevelTable>(0x68);
        public NativeAffixTable _10_0x78_NativeAffixTable => ReadObject<NativeAffixTable>(0x78);
        public NativeHeros _11_0x88_NativeHeros => ReadObject<NativeHeros>(0x88);
        public NativeMovementStyles _12_0x98_NativeMovementStyles => ReadObject<NativeMovementStyles>(0x98);
        public NativeLabels _13_0xA8_NativeLabels => ReadObject<NativeLabels>(0xA8);
        public NativeLootDistributionTable _14_0xB8_NativeLootDistributionTable => ReadObject<NativeLootDistributionTable>(0xB8);
        public NativeRareItemNamesTable _15_0xC8_NativeRareItemNamesTable => ReadObject<NativeRareItemNamesTable>(0xC8);
        public NativeMonsterAffixesTable _16_0xD8_NativeMonsterAffixesTable => ReadObject<NativeMonsterAffixesTable>(0xD8);
        public NativeRareMonsterNamesTable _17_0xE8_NativeRareMonsterNamesTable => ReadObject<NativeRareMonsterNamesTable>(0xE8);
        public NativeSocketedEffectsTable _18_0xF8_NativeSocketedEffectsTable => ReadObject<NativeSocketedEffectsTable>(0xF8);
        public NativeItemDropTable _19_0x108_NativeItemDropTable => ReadObject<NativeItemDropTable>(0x108);
        public NativeItemLevelModTable _20_0x118_NativeItemLevelModTable => ReadObject<NativeItemLevelModTable>(0x118);
        public NativeQualityClassTable _21_0x128_NativeQualityClassTable => ReadObject<NativeQualityClassTable>(0x128);
        public NativeHandicapLevelTable _22_0x138_NativeHandicapLevelTable => ReadObject<NativeHandicapLevelTable>(0x138);
        public NativeItemSalvageLevelTable _23_0x148_NativeItemSalvageLevelTable => ReadObject<NativeItemSalvageLevelTable>(0x148);
        public NativeHirelings _24_0x158_NativeHirelings => ReadObject<NativeHirelings>(0x158);
        public NativeSetItemBonusTable _25_0x168_NativeSetItemBonusTable => ReadObject<NativeSetItemBonusTable>(0x168);
        public NativeEliteModifiers _26_0x178_NativeEliteModifiers => ReadObject<NativeEliteModifiers>(0x178);
        public NativeItemTiers _27_0x188_NativeItemTiers => ReadObject<NativeItemTiers>(0x188);
        public NativePowerFormulaTable _28_0x198_NativePowerFormulaTable => ReadObject<NativePowerFormulaTable>(0x198);
        public NativeRecipesTable _29_0x1A8_NativeRecipesTable => ReadObject<NativeRecipesTable>(0x1A8);
        public NativeScriptedAchievementEventsTable _30_0x1B8_NativeScriptedAchievementEventsTable => ReadObject<NativeScriptedAchievementEventsTable>(0x1B8);
        public NativeLootRunQuestTierTable _31_0x1C8_NativeLootRunQuestTierTable => ReadObject<NativeLootRunQuestTierTable>(0x1C8);
        public NativeParagonBonusesTable _32_0x1D8_NativeParagonBonusesTable => ReadObject<NativeParagonBonusesTable>(0x1D8);
        public NativeLegacyItemConversionTable _33_0x1E8_NativeLegacyItemConversionTable => ReadObject<NativeLegacyItemConversionTable>(0x1E8);
        public NativeEnchantItemAffixUseCountCostScalarsTable _34_0x1F8_NativeEnchantItemAffixUseCountCostScalarsTable => ReadObject<NativeEnchantItemAffixUseCountCostScalarsTable>(0x1F8);
        public NativeTieredLootRunLevelTable _35_0x208_NativeTieredLootRunLevelTable => ReadObject<NativeTieredLootRunLevelTable>(0x208);
        public NativeTransmuteRecipesTable _36_0x218_NativeTransmuteRecipesTable => ReadObject<NativeTransmuteRecipesTable>(0x218);
    }


    [CompilerGenerated]
    public class NativeItemsTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeItemType> _2_0x8_VariableArray => ReadSerializedObjects<NativeItemType>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeExperienceTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeExperienceLevel> _2_0x8_VariableArray => ReadSerializedObjects<NativeExperienceLevel>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeExperienceTableAlt : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeExperienceLevelAlt> _2_0x8_VariableArray => ReadSerializedObjects<NativeExperienceLevelAlt>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeHelpCodes : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeHelpCode> _2_0x8_VariableArray => ReadSerializedObjects<NativeHelpCode>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeMonsterLevelTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeMonsterLevelDefinition> _2_0x8_VariableArray => ReadSerializedObjects<NativeMonsterLevelDefinition>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeAffixTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeAffixTableEntry> _2_0x8_VariableArray => ReadSerializedObjects<NativeAffixTableEntry>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeHeros : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeHeroData> _2_0x8_VariableArray => ReadSerializedObjects<NativeHeroData>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeMovementStyles : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeMovementStyleDefinition> _2_0x8_VariableArray => ReadSerializedObjects<NativeMovementStyleDefinition>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeLabels : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeLabelGBID> _2_0x8_VariableArray => ReadSerializedObjects<NativeLabelGBID>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeLootDistributionTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeLootDistribution> _2_0x8_VariableArray => ReadSerializedObjects<NativeLootDistribution>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeRareItemNamesTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeRareItemNamesEntry> _2_0x8_VariableArray => ReadSerializedObjects<NativeRareItemNamesEntry>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeMonsterAffixesTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeMonsterAffixesEntry> _2_0x8_VariableArray => ReadSerializedObjects<NativeMonsterAffixesEntry>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeRareMonsterNamesTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeRareMonsterNamesEntry> _2_0x8_VariableArray => ReadSerializedObjects<NativeRareMonsterNamesEntry>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeSocketedEffectsTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeSocketedEffectsTableEntry> _2_0x8_VariableArray => ReadSerializedObjects<NativeSocketedEffectsTableEntry>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeItemDropTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeItemDropTableEntry> _2_0x8_VariableArray => ReadSerializedObjects<NativeItemDropTableEntry>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeItemLevelModTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeLootDistribution> _2_0x8_VariableArray => ReadSerializedObjects<NativeLootDistribution>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeQualityClassTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeQualityClass> _2_0x8_VariableArray => ReadSerializedObjects<NativeQualityClass>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeHandicapLevelTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeHandicapLevel> _2_0x8_VariableArray => ReadSerializedObjects<NativeHandicapLevel>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeItemSalvageLevelTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeItemSalvageLevel> _2_0x8_VariableArray => ReadSerializedObjects<NativeItemSalvageLevel>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeHirelings : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeHirelingEntry> _2_0x8_VariableArray => ReadSerializedObjects<NativeHirelingEntry>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeSetItemBonusTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeSetItemBonusTableEntry> _2_0x8_VariableArray => ReadSerializedObjects<NativeSetItemBonusTableEntry>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeEliteModifiers : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeEliteModifierData> _2_0x8_VariableArray => ReadSerializedObjects<NativeEliteModifierData>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeItemTiers : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeItemTierData> _2_0x8_VariableArray => ReadSerializedObjects<NativeItemTierData>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativePowerFormulaTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativePowerFormulaTableEntry> _2_0x8_VariableArray => ReadSerializedObjects<NativePowerFormulaTableEntry>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeRecipesTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeRecipeEntry> _2_0x8_VariableArray => ReadSerializedObjects<NativeRecipeEntry>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeScriptedAchievementEventsTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeScriptedAchievementEvent> _2_0x8_VariableArray => ReadSerializedObjects<NativeScriptedAchievementEvent>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeLootRunQuestTierTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeLootRunQuestTierEntry> _2_0x8_VariableArray => ReadSerializedObjects<NativeLootRunQuestTierEntry>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeParagonBonusesTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeParagonBonus> _2_0x8_VariableArray => ReadSerializedObjects<NativeParagonBonus>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeLegacyItemConversionTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeLegacyItemConversion> _2_0x8_VariableArray => ReadSerializedObjects<NativeLegacyItemConversion>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeEnchantItemAffixUseCountCostScalarsTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeEnchantItemAffixUseCountCostScalar> _2_0x8_VariableArray => ReadSerializedObjects<NativeEnchantItemAffixUseCountCostScalar>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeTieredLootRunLevelTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeTieredLootRunLevel> _2_0x8_VariableArray => ReadSerializedObjects<NativeTieredLootRunLevel>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeTransmuteRecipesTable : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeTransmuteRecipe> _2_0x8_VariableArray => ReadSerializedObjects<NativeTransmuteRecipe>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeItemType : MemoryWrapper
    {
        public const int SizeOf = 336; // 0x150
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_ItemTypes_GameBalanceId => ReadOffset<int>(0x108); //      Flags=1  
        public int _5_0x10C_int => ReadOffset<int>(0x10C); //      Flags=1  
        public int _6_0x110_int => ReadOffset<int>(0x110); //      Flags=1  
        public int _7_0x114_int => ReadOffset<int>(0x114); //      Flags=1  
                                                           // Unknown index=8 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=9 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=10 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=11 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=12 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=13 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=14 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=15 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=16 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=17 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=18 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=19 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=20 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=21 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
                                                           // Unknown index=22 @Offset0x118 Type=DT_FLAG BaseType=DT_NULL //      Flags=1  
        public VendorSlot _23_0x11C_Enum => ReadOffset<VendorSlot>(0x11C); //      Flags=17  SymbolTable=@26701512 Max=25
        public VendorSlot _24_0x120_Enum => ReadOffset<VendorSlot>(0x120); //      Flags=17  SymbolTable=@26701512 Max=25
        public VendorSlot _25_0x124_Enum => ReadOffset<VendorSlot>(0x124); //      Flags=17  SymbolTable=@26701512 Max=25
        public VendorSlot _26_0x128_Enum => ReadOffset<VendorSlot>(0x128); //      Flags=17  SymbolTable=@26701512 Max=25
        public int _27_0x12C_AffixList_GameBalanceId => ReadOffset<int>(0x12C); //      Flags=1  
        public int _28_0x130_AffixList_GameBalanceId => ReadOffset<int>(0x130); //      Flags=1  
        public int _29_0x134_AffixList_GameBalanceId => ReadOffset<int>(0x134); //      Flags=1  
        public int _30_0x138_AffixGroup_GameBalanceId => ReadOffset<int>(0x138); //      Flags=1  
        public List<int> _31_0x13C_FixedArray => ReadArray<int>(0x13C, 5); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeExperienceLevel : MemoryWrapper
    {
        public const int SizeOf = 472; // 0x1D8
                                 // Unknown index=1 @Offset0x0 Type=DT_INT64 BaseType=DT_NULL //      Flags=1  
        public int _2_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public int _3_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public float _4_0x10_float => ReadOffset<float>(0x10); //      Flags=1  
        public float _5_0x14_float => ReadOffset<float>(0x14); //      Flags=1  
        public int _6_0x18_int => ReadOffset<int>(0x18); //      Flags=1  
        public int _7_0x1C_int => ReadOffset<int>(0x1C); //      Flags=1  
        public int _8_0x20_int => ReadOffset<int>(0x20); //      Flags=1  
        public int _9_0x24_int => ReadOffset<int>(0x24); //      Flags=1  
        public int _10_0x28_int => ReadOffset<int>(0x28); //      Flags=1  
        public int _11_0x2C_int => ReadOffset<int>(0x2C); //      Flags=1  
        public int _12_0x30_int => ReadOffset<int>(0x30); //      Flags=1  
        public int _13_0x34_int => ReadOffset<int>(0x34); //      Flags=1  
        public int _14_0x38_int => ReadOffset<int>(0x38); //      Flags=1  
        public int _15_0x3C_int => ReadOffset<int>(0x3C); //      Flags=1  
        public int _16_0x40_int => ReadOffset<int>(0x40); //      Flags=1  
        public int _17_0x44_int => ReadOffset<int>(0x44); //      Flags=1  
        public int _18_0x48_int => ReadOffset<int>(0x48); //      Flags=1  
        public int _19_0x4C_int => ReadOffset<int>(0x4C); //      Flags=1  
        public int _20_0x50_int => ReadOffset<int>(0x50); //      Flags=1  
        public int _21_0x54_int => ReadOffset<int>(0x54); //      Flags=1  
        public int _22_0x58_int => ReadOffset<int>(0x58); //      Flags=1  
        public int _23_0x5C_int => ReadOffset<int>(0x5C); //      Flags=1  
        public int _24_0x60_int => ReadOffset<int>(0x60); //      Flags=1  
        public int _25_0x64_int => ReadOffset<int>(0x64); //      Flags=1  
        public int _26_0x68_int => ReadOffset<int>(0x68); //      Flags=1  
        public int _27_0x6C_int => ReadOffset<int>(0x6C); //      Flags=1  
        public int _28_0x70_int => ReadOffset<int>(0x70); //      Flags=1  
        public int _29_0x74_int => ReadOffset<int>(0x74); //      Flags=1  
        public int _30_0x78_int => ReadOffset<int>(0x78); //      Flags=1  
        public int _31_0x7C_int => ReadOffset<int>(0x7C); //      Flags=1  
        public int _32_0x80_int => ReadOffset<int>(0x80); //      Flags=1  
        public int _33_0x84_int => ReadOffset<int>(0x84); //      Flags=1  
        public int _34_0x88_int => ReadOffset<int>(0x88); //      Flags=1  
        public int _35_0x8C_int => ReadOffset<int>(0x8C); //      Flags=1  
        public int _36_0x90_int => ReadOffset<int>(0x90); //      Flags=1  
        public int _37_0x94_int => ReadOffset<int>(0x94); //      Flags=1  
        public int _38_0x98_int => ReadOffset<int>(0x98); //      Flags=1  
        public int _39_0x9C_int => ReadOffset<int>(0x9C); //      Flags=1  
        public int _40_0xA0_int => ReadOffset<int>(0xA0); //      Flags=1  
        public int _41_0xA4_int => ReadOffset<int>(0xA4); //      Flags=1  
        public int _42_0xA8_int => ReadOffset<int>(0xA8); //      Flags=1  
        public float _43_0xAC_float => ReadOffset<float>(0xAC); //      Flags=1  
        public float _44_0xB0_float => ReadOffset<float>(0xB0); //      Flags=1  
        public float _45_0xB4_float => ReadOffset<float>(0xB4); //      Flags=1  
        public int _46_0xB8_int => ReadOffset<int>(0xB8); //      Flags=1  
        public int _47_0xBC_int => ReadOffset<int>(0xBC); //      Flags=1  
        public int _48_0xC0_int => ReadOffset<int>(0xC0); //      Flags=1  
        public int _49_0xC4_int => ReadOffset<int>(0xC4); //      Flags=1  
        public int _50_0xC8_int => ReadOffset<int>(0xC8); //      Flags=1  
        public int _51_0xCC_int => ReadOffset<int>(0xCC); //      Flags=1  
        public int _52_0xD0_int => ReadOffset<int>(0xD0); //      Flags=1  
        public int _53_0xD4_int => ReadOffset<int>(0xD4); //      Flags=1  
        public int _54_0xD8_int => ReadOffset<int>(0xD8); //      Flags=1  
        public int _55_0xDC_int => ReadOffset<int>(0xDC); //      Flags=1  
        public int _56_0xE0_int => ReadOffset<int>(0xE0); //      Flags=1  
        public int _57_0xE4_int => ReadOffset<int>(0xE4); //      Flags=1  
        public int _58_0xE8_int => ReadOffset<int>(0xE8); //      Flags=1  
        public int _59_0xEC_int => ReadOffset<int>(0xEC); //      Flags=1  
        public int _60_0xF0_int => ReadOffset<int>(0xF0); //      Flags=1  
        public int _61_0xF4_int => ReadOffset<int>(0xF4); //      Flags=1  
        public int _62_0xF8_int => ReadOffset<int>(0xF8); //      Flags=1  
        public int _63_0xFC_int => ReadOffset<int>(0xFC); //      Flags=1  
        public int _64_0x100_int => ReadOffset<int>(0x100); //      Flags=1  
        public int _65_0x104_int => ReadOffset<int>(0x104); //      Flags=1  
        public int _66_0x108_int => ReadOffset<int>(0x108); //      Flags=1  
        public int _67_0x10C_int => ReadOffset<int>(0x10C); //      Flags=1  
        public int _68_0x110_int => ReadOffset<int>(0x110); //      Flags=1  
        public int _69_0x114_int => ReadOffset<int>(0x114); //      Flags=1  
        public int _70_0x128_int => ReadOffset<int>(0x128); //      Flags=1  
        public int _71_0x12C_int => ReadOffset<int>(0x12C); //      Flags=1  
        public int _72_0x130_int => ReadOffset<int>(0x130); //      Flags=1  
        public int _73_0x134_int => ReadOffset<int>(0x134); //      Flags=1  
        public int _74_0x138_int => ReadOffset<int>(0x138); //      Flags=1  
        public int _75_0x13C_int => ReadOffset<int>(0x13C); //      Flags=1  
        public int _76_0x140_int => ReadOffset<int>(0x140); //      Flags=1  
        public int _77_0x144_int => ReadOffset<int>(0x144); //      Flags=1  
        public int _78_0x148_int => ReadOffset<int>(0x148); //      Flags=1  
        public int _79_0x14C_int => ReadOffset<int>(0x14C); //      Flags=1  
        public int _80_0x150_int => ReadOffset<int>(0x150); //      Flags=1  
        public int _81_0x154_int => ReadOffset<int>(0x154); //      Flags=1  
        public float _82_0x15C_float => ReadOffset<float>(0x15C); //      Flags=1  
        public float _83_0x160_float => ReadOffset<float>(0x160); //      Flags=1  
        public float _84_0x164_float => ReadOffset<float>(0x164); //      Flags=1  
                                                                  // Unknown index=85 @Offset0x168 Type=DT_INT64 BaseType=DT_NULL //      Flags=1  
        public int _86_0x170_int => ReadOffset<int>(0x170); //      Flags=1  
        public int _87_0x174_int => ReadOffset<int>(0x174); //      Flags=1  
        public int _88_0x178_int => ReadOffset<int>(0x178); //      Flags=1  
        public int _89_0x17C_int => ReadOffset<int>(0x17C); //      Flags=1  
        public int _90_0x180_int => ReadOffset<int>(0x180); //      Flags=1  
        public int _91_0x184_int => ReadOffset<int>(0x184); //      Flags=1  
        public int _92_0x188_int => ReadOffset<int>(0x188); //      Flags=1  
        public int _93_0x18C_int => ReadOffset<int>(0x18C); //      Flags=1  
        public int _94_0x190_int => ReadOffset<int>(0x190); //      Flags=1  
        public int _95_0x194_int => ReadOffset<int>(0x194); //      Flags=1  
        public int _96_0x198_int => ReadOffset<int>(0x198); //      Flags=1  
        public int _97_0x19C_int => ReadOffset<int>(0x19C); //      Flags=1  
        public int _98_0x1A0_int => ReadOffset<int>(0x1A0); //      Flags=1  
        public int _99_0x1A4_int => ReadOffset<int>(0x1A4); //      Flags=1  
        public int _100_0x1A8_int => ReadOffset<int>(0x1A8); //      Flags=1  
        public int _101_0x1AC_int => ReadOffset<int>(0x1AC); //      Flags=1  
        public int _102_0x1B0_int => ReadOffset<int>(0x1B0); //      Flags=1  
        public int _103_0x1B4_int => ReadOffset<int>(0x1B4); //      Flags=1  
        public float _104_0x1B8_float => ReadOffset<float>(0x1B8); //      Flags=1  
        public int _105_0x1BC_int => ReadOffset<int>(0x1BC); //      Flags=1  
        public int _106_0x1C0_int => ReadOffset<int>(0x1C0); //      Flags=1  
        public int _107_0x1C4_int => ReadOffset<int>(0x1C4); //      Flags=1  
        public int _108_0x1C8_int => ReadOffset<int>(0x1C8); //      Flags=1  
        public int _109_0x1CC_int => ReadOffset<int>(0x1CC); //      Flags=1  
        public int _110_0x1D0_int => ReadOffset<int>(0x1D0); //      Flags=1  
        public float _111_0x1D4_float => ReadOffset<float>(0x1D4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeExperienceLevelAlt : MemoryWrapper
    {
        public const int SizeOf = 112; // 0x70
                                 // Unknown index=1 @Offset0x0 Type=DT_INT64 BaseType=DT_NULL //      Flags=1  
        public int _2_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public int _3_0xC_int => ReadOffset<int>(0xC);
        public int _4_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
        public int _5_0x14_int => ReadOffset<int>(0x14); //      Flags=1  
        public int _6_0x18_int => ReadOffset<int>(0x18); //      Flags=1  
        public int _7_0x1C_int => ReadOffset<int>(0x1C); //      Flags=1  
        public int _8_0x20_int => ReadOffset<int>(0x20); //      Flags=1  
        public int _9_0x24_int => ReadOffset<int>(0x24); //      Flags=1  
        public int _10_0x28_int => ReadOffset<int>(0x28); //      Flags=1  
        public int _11_0x2C_int => ReadOffset<int>(0x2C); //      Flags=1  
        public int _12_0x30_int => ReadOffset<int>(0x30); //      Flags=1  
        public int _13_0x34_int => ReadOffset<int>(0x34); //      Flags=1  
        public int _14_0x38_int => ReadOffset<int>(0x38); //      Flags=1  
        public int _15_0x3C_int => ReadOffset<int>(0x3C); //      Flags=1  
        public int _16_0x40_int => ReadOffset<int>(0x40); //      Flags=1  
        public int _17_0x44_int => ReadOffset<int>(0x44); //      Flags=1  
        public int _18_0x48_int => ReadOffset<int>(0x48); //      Flags=1  
        public int _19_0x4C_int => ReadOffset<int>(0x4C); //      Flags=1  
        public int _20_0x50_int => ReadOffset<int>(0x50); //      Flags=1  
        public int _21_0x54_int => ReadOffset<int>(0x54); //      Flags=1  
        public int _22_0x58_int => ReadOffset<int>(0x58); //      Flags=1  
        public int _23_0x5C_int => ReadOffset<int>(0x5C); //      Flags=1  
        public int _24_0x60_int => ReadOffset<int>(0x60); //      Flags=1  
        public int _25_0x64_int => ReadOffset<int>(0x64); //      Flags=1  
        public int _26_0x68_int => ReadOffset<int>(0x68); //      Flags=1  
        public int _27_0x6C_int => ReadOffset<int>(0x6C); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeHelpCode : MemoryWrapper
    {
        public const int SizeOf = 640; // 0x280
        public string _1_0x0_String => ReadString(0x0); //      Flags=1  
        public string _2_0x100_String => ReadString(0x100); //      Flags=1  
        public string _3_0x200_String => ReadString(0x200); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeMonsterLevelDefinition : MemoryWrapper
    {
        public const int SizeOf = 240; // 0xF0
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public float _2_0x4_float => ReadOffset<float>(0x4); //      Flags=1  
        public float _3_0x8_float => ReadOffset<float>(0x8); //      Flags=1  
        public float _4_0xC_float => ReadOffset<float>(0xC); //      Flags=1  
        public float _5_0x10_float => ReadOffset<float>(0x10); //      Flags=1  
        public float _6_0x14_float => ReadOffset<float>(0x14); //      Flags=1  
        public float _7_0x18_float => ReadOffset<float>(0x18); //      Flags=1  
        public float _8_0x1C_float => ReadOffset<float>(0x1C); //      Flags=1  
        public float _9_0x20_float => ReadOffset<float>(0x20); //      Flags=1  
        public float _10_0x24_float => ReadOffset<float>(0x24); //      Flags=1  
        public float _11_0x28_float => ReadOffset<float>(0x28); //      Flags=1  
        public float _12_0x2C_float => ReadOffset<float>(0x2C); //      Flags=1  
        public float _13_0x30_float => ReadOffset<float>(0x30); //      Flags=1  
        public float _14_0x34_float => ReadOffset<float>(0x34); //      Flags=1  
        public float _15_0x38_float => ReadOffset<float>(0x38); //      Flags=1  
        public float _16_0x3C_float => ReadOffset<float>(0x3C); //      Flags=1  
        public float _17_0x40_float => ReadOffset<float>(0x40); //      Flags=1  
        public float _18_0x44_float => ReadOffset<float>(0x44); //      Flags=1  
        public float _19_0x48_float => ReadOffset<float>(0x48); //      Flags=1  
        public float _20_0x4C_float => ReadOffset<float>(0x4C); //      Flags=1  
        public float _21_0x50_float => ReadOffset<float>(0x50); //      Flags=1  
        public float _22_0x54_float => ReadOffset<float>(0x54); //      Flags=1  
        public float _23_0x58_float => ReadOffset<float>(0x58); //      Flags=1  
        public float _24_0x5C_float => ReadOffset<float>(0x5C); //      Flags=1  
        public float _25_0x60_float => ReadOffset<float>(0x60); //      Flags=1  
        public float _26_0x64_float => ReadOffset<float>(0x64); //      Flags=1  
        public float _27_0x68_float => ReadOffset<float>(0x68); //      Flags=1  
        public float _28_0x6C_float => ReadOffset<float>(0x6C); //      Flags=1  
        public float _29_0x70_float => ReadOffset<float>(0x70); //      Flags=1  
        public float _30_0x74_float => ReadOffset<float>(0x74); //      Flags=1  
        public float _31_0x78_float => ReadOffset<float>(0x78); //      Flags=1  
        public float _32_0x7C_float => ReadOffset<float>(0x7C); //      Flags=1  
        public float _33_0x80_float => ReadOffset<float>(0x80); //      Flags=1  
        public float _34_0x84_float => ReadOffset<float>(0x84); //      Flags=1  
        public float _35_0x88_float => ReadOffset<float>(0x88); //      Flags=1  
        public float _36_0x8C_float => ReadOffset<float>(0x8C); //      Flags=1  
        public float _37_0x90_float => ReadOffset<float>(0x90); //      Flags=1  
        public float _38_0x94_float => ReadOffset<float>(0x94); //      Flags=1  
        public float _39_0x98_float => ReadOffset<float>(0x98); //      Flags=1  
        public float _40_0x9C_float => ReadOffset<float>(0x9C); //      Flags=1  
        public float _41_0xA0_float => ReadOffset<float>(0xA0); //      Flags=1  
        public float _42_0xA4_float => ReadOffset<float>(0xA4); //      Flags=1  
        public float _43_0xA8_float => ReadOffset<float>(0xA8); //      Flags=1  
        public float _44_0xAC_float => ReadOffset<float>(0xAC); //      Flags=1  
        public float _45_0xB0_float => ReadOffset<float>(0xB0); //      Flags=1  
        public float _46_0xB4_float => ReadOffset<float>(0xB4); //      Flags=1  
        public float _47_0xB8_float => ReadOffset<float>(0xB8); //      Flags=1  
        public float _48_0xBC_float => ReadOffset<float>(0xBC); //      Flags=1  
        public float _49_0xC0_float => ReadOffset<float>(0xC0); //      Flags=1  
        public float _50_0xC4_float => ReadOffset<float>(0xC4); //      Flags=1  
        public float _51_0xC8_float => ReadOffset<float>(0xC8); //      Flags=1  
        public float _52_0xCC_float => ReadOffset<float>(0xCC); //      Flags=1  
        public float _53_0xD0_float => ReadOffset<float>(0xD0); //      Flags=1  
        public float _54_0xD4_float => ReadOffset<float>(0xD4); //      Flags=1  
        public float _55_0xD8_float => ReadOffset<float>(0xD8); //      Flags=1  
        public float _56_0xDC_float => ReadOffset<float>(0xDC); //      Flags=1  
        public float _57_0xE0_float => ReadOffset<float>(0xE0); //      Flags=1  
        public float _58_0xE4_float => ReadOffset<float>(0xE4); //      Flags=1  
        public float _59_0xE8_float => ReadOffset<float>(0xE8); //      Flags=1  
        public int _60_0xEC_int => ReadOffset<int>(0xEC);
    }


    [CompilerGenerated]
    public class NativeAffixTableEntry : MemoryWrapper
    {
        public const int SizeOf = 784; // 0x310
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_int => ReadOffset<int>(0x108); //      Flags=1  
        public int _5_0x10C_int => ReadOffset<int>(0x10C); //      Flags=1  
        public int _6_0x110_int => ReadOffset<int>(0x110); //      Flags=1  
        public int _7_0x114_int => ReadOffset<int>(0x114); //      Flags=1  
        public int _8_0x118_int => ReadOffset<int>(0x118); //      Flags=1  
        public int _9_0x11C_int => ReadOffset<int>(0x11C); //      Flags=1  
        public int _10_0x120_int => ReadOffset<int>(0x120); //      Flags=1  
        public int _11_0x124_int => ReadOffset<int>(0x124); //      Flags=1  
        public int _12_0x128_int => ReadOffset<int>(0x128); //      Flags=1  
        public int _13_0x12C_int => ReadOffset<int>(0x12C); //      Flags=1  
        public int _14_0x130_int => ReadOffset<int>(0x130); //      Flags=1  
        public int _15_0x134_int => ReadOffset<int>(0x134); //      Flags=1  
        public int _16_0x138_int => ReadOffset<int>(0x138); //      Flags=1  
        public int _17_0x13C_int => ReadOffset<int>(0x13C); //      Flags=1  
        public int _18_0x140_int => ReadOffset<int>(0x140); //      Flags=1  
        public int _19_0x144_int => ReadOffset<int>(0x144); //      Flags=1  
        public int _20_0x148_int => ReadOffset<int>(0x148); //      Flags=1  
        public int _21_0x14C_int => ReadOffset<int>(0x14C); //      Flags=1  
        public BonusType _22_0x150_Enum => ReadOffset<BonusType>(0x150); //      Flags=17  SymbolTable=@26700224 Max=14
        public int _23_0x154_int => ReadOffset<int>(0x154); //      Flags=1  
        public int _24_0x158_AffixList_GameBalanceId => ReadOffset<int>(0x158); //      Flags=1  
        public int _25_0x15C_AffixList_GameBalanceId => ReadOffset<int>(0x15C); //      Flags=1  *** matches string list affixes.stl gbid (normal hash of gbname)
        public int _26_0x160_StringList_Sno => ReadOffset<int>(0x160); //      Flags=1  
        public int _27_0x164_StringList_Sno => ReadOffset<int>(0x164); //      Flags=1  
        public int _28_0x168_AffixGroup_GameBalanceId => ReadOffset<int>(0x168); //      Flags=1  
        public int _29_0x16C_AffixGroup_GameBalanceId => ReadOffset<int>(0x16C); //      Flags=1  
        public ActorClass _30_0x170_Enum => ReadOffset<ActorClass>(0x170); //      Flags=17  SymbolTable=@26705560 Min=-1Max=5
        public int _31_0x174_AffixList_GameBalanceId => ReadOffset<int>(0x174); //      Flags=1  
        public List<int> _32_0x178_FixedArray => ReadArray<int>(0x178, 6); //      Flags=1  
        public List<int> _33_0x190_FixedArray => ReadArray<int>(0x190, 24); //      Flags=1  
        public List<int> _34_0x1F0_FixedArray => ReadArray<int>(0x1F0, 24); //      Flags=1  
        public int _35_0x250_int => ReadOffset<int>(0x250); //      Flags=1  
        public AffixType _36_0x254_Enum => ReadOffset<AffixType>(0x254); //      Flags=17  SymbolTable=@26697912 Max=11
        public int _37_0x258_AffixList_GameBalanceId => ReadOffset<int>(0x258); //      Flags=1  
        public List<NativeAttributeSpecifier> _38_0x260_FixedArray => ReadObjects<NativeAttributeSpecifier>(0x260, 4); //      Flags=1  

        //public _39_0x308_Enum => ReadOffset<>(0x308); //      Flags=17  SymbolTable=@26697992 Max=1
    }


    [CompilerGenerated]
    public class NativeHeroData : MemoryWrapper
    {
        public const int SizeOf = 504; // 0x1F8
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_Actor_Sno => ReadOffset<int>(0x108); //      Flags=1  
        public int _5_0x10C_Actor_Sno => ReadOffset<int>(0x10C); //      Flags=1  
        public int _6_0x110_TreasureClass_Sno => ReadOffset<int>(0x110); //      Flags=1  
        public int _7_0x114_int => ReadOffset<int>(0x114); //      Flags=1  
        public int _8_0x118_Power_Sno => ReadOffset<int>(0x118); //      Flags=1  
        public int _9_0x11C_Power_Sno => ReadOffset<int>(0x11C); //      Flags=1  
        public int _10_0x120_SkillKit_Sno => ReadOffset<int>(0x120); //      Flags=1  
        public int _11_0x124_SkillKit_Sno => ReadOffset<int>(0x124); //      Flags=1  
        public int _12_0x128_SkillKit_Sno => ReadOffset<int>(0x128); //      Flags=1  
        public int _13_0x12C_SkillKit_Sno => ReadOffset<int>(0x12C); //      Flags=1  
        public ResourceType _14_0x130_Enum => ReadOffset<ResourceType>(0x130); //      Flags=17  SymbolTable=@26702016 Max=7
        public ResourceType _15_0x134_Enum => ReadOffset<ResourceType>(0x134); //      Flags=17  SymbolTable=@26702016 Max=7
        public PlayerAttribute _16_0x138_Enum => ReadOffset<PlayerAttribute>(0x138); //      Flags=17  SymbolTable=@26708552 Min=-1Max=2
        public float _17_0x13C_float => ReadOffset<float>(0x13C); //      Flags=1  
        public int _18_0x140_int => ReadOffset<int>(0x140); //      Flags=1  
        public float _19_0x144_float => ReadOffset<float>(0x144); //      Flags=1  
        public float _20_0x148_float => ReadOffset<float>(0x148); //      Flags=1  
        public float _21_0x14C_float => ReadOffset<float>(0x14C); //      Flags=1  
        public float _22_0x150_float => ReadOffset<float>(0x150); //      Flags=1  
        public float _23_0x154_float => ReadOffset<float>(0x154); //      Flags=1  
        public float _24_0x158_float => ReadOffset<float>(0x158); //      Flags=1  
        public float _25_0x15C_float => ReadOffset<float>(0x15C); //      Flags=1  
        public float _26_0x160_float => ReadOffset<float>(0x160); //      Flags=1  
        public float _27_0x164_float => ReadOffset<float>(0x164); //      Flags=1  
        public float _28_0x168_float => ReadOffset<float>(0x168); //      Flags=1  
        public float _29_0x16C_float => ReadOffset<float>(0x16C); //      Flags=1  
        public float _30_0x170_float => ReadOffset<float>(0x170); //      Flags=1  
        public float _31_0x174_float => ReadOffset<float>(0x174); //      Flags=1  
        public float _32_0x178_float => ReadOffset<float>(0x178); //      Flags=1  
        public float _33_0x17C_float => ReadOffset<float>(0x17C); //      Flags=1  
        public float _34_0x180_float => ReadOffset<float>(0x180); //      Flags=1  
        public float _35_0x184_float => ReadOffset<float>(0x184); //      Flags=1  
        public float _36_0x188_float => ReadOffset<float>(0x188); //      Flags=1  
        public float _37_0x18C_float => ReadOffset<float>(0x18C); //      Flags=1  
        public float _38_0x190_float => ReadOffset<float>(0x190); //      Flags=1  
        public float _39_0x194_float => ReadOffset<float>(0x194); //      Flags=1  
        public float _40_0x198_float => ReadOffset<float>(0x198); //      Flags=1  
        public float _41_0x19C_float => ReadOffset<float>(0x19C); //      Flags=1  
        public float _42_0x1A0_float => ReadOffset<float>(0x1A0); //      Flags=1  
        public float _43_0x1A4_float => ReadOffset<float>(0x1A4); //      Flags=1  
        public float _44_0x1A8_float => ReadOffset<float>(0x1A8); //      Flags=1  
        public float _45_0x1AC_float => ReadOffset<float>(0x1AC); //      Flags=1  
        public float _46_0x1B0_float => ReadOffset<float>(0x1B0); //      Flags=1  
        public float _47_0x1B4_float => ReadOffset<float>(0x1B4); //      Flags=1  
        public float _48_0x1B8_float => ReadOffset<float>(0x1B8); //      Flags=1  
        public float _49_0x1BC_float => ReadOffset<float>(0x1BC); //      Flags=1  
        public float _50_0x1C0_float => ReadOffset<float>(0x1C0); //      Flags=1  
        public float _51_0x1C4_float => ReadOffset<float>(0x1C4); //      Flags=1  
        public float _52_0x1C8_float => ReadOffset<float>(0x1C8); //      Flags=1  
        public float _53_0x1CC_float => ReadOffset<float>(0x1CC); //      Flags=1  
        public float _54_0x1D0_float => ReadOffset<float>(0x1D0); //      Flags=1  
        public float _55_0x1D4_float => ReadOffset<float>(0x1D4); //      Flags=1  
        public float _56_0x1D8_float => ReadOffset<float>(0x1D8); //      Flags=1  
        public float _57_0x1DC_float => ReadOffset<float>(0x1DC); //      Flags=1  
        public float _58_0x1E0_float => ReadOffset<float>(0x1E0); //      Flags=1  
        public float _59_0x1E4_float => ReadOffset<float>(0x1E4); //      Flags=1  
        public float _60_0x1E8_float => ReadOffset<float>(0x1E8); //      Flags=1  
        public float _61_0x1EC_float => ReadOffset<float>(0x1EC); //      Flags=1  
        public float _62_0x1F0_float => ReadOffset<float>(0x1F0); //      Flags=1  
        public float _63_0x1F4_float => ReadOffset<float>(0x1F4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeMovementStyleDefinition : MemoryWrapper
    {
        public const int SizeOf = 392; // 0x188
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_int => ReadOffset<int>(0x108); //      Flags=1  
        public int _5_0x10C_int => ReadOffset<int>(0x10C); //      Flags=1  
        public int _6_0x110_int => ReadOffset<int>(0x110); //      Flags=1  
        public int _7_0x114_int => ReadOffset<int>(0x114); //      Flags=1  
        public int _8_0x118_int => ReadOffset<int>(0x118); //      Flags=1  
        public int _9_0x11C_int => ReadOffset<int>(0x11C); //      Flags=1  
        public int _10_0x120_int => ReadOffset<int>(0x120); //      Flags=1  
        public int _11_0x124_int => ReadOffset<int>(0x124); //      Flags=1  
        public float _12_0x128_float => ReadOffset<float>(0x128); //      Flags=1  
        public float _13_0x12C_float => ReadOffset<float>(0x12C); //      Flags=1  
        public float _14_0x130_float => ReadOffset<float>(0x130); //      Flags=1  
        public float _15_0x134_float => ReadOffset<float>(0x134); //      Flags=1  
        public float _16_0x138_float => ReadOffset<float>(0x138); //      Flags=1  
        public float _17_0x13C_float => ReadOffset<float>(0x13C); //      Flags=1  
        public float _18_0x140_float => ReadOffset<float>(0x140); //      Flags=1  
        public float _19_0x144_float => ReadOffset<float>(0x144); //      Flags=1  
        public float _20_0x148_float => ReadOffset<float>(0x148); //      Flags=1  
        public float _21_0x14C_float => ReadOffset<float>(0x14C); //      Flags=1  
        public float _22_0x150_float => ReadOffset<float>(0x150); //      Flags=1  
        public float _23_0x154_float => ReadOffset<float>(0x154); //      Flags=1  
        public float _24_0x158_float => ReadOffset<float>(0x158); //      Flags=1  
        public float _25_0x15C_float => ReadOffset<float>(0x15C); //      Flags=1  
        public float _26_0x160_float => ReadOffset<float>(0x160); //      Flags=1  
        public float _27_0x164_float => ReadOffset<float>(0x164); //      Flags=1  
        public float _28_0x168_float => ReadOffset<float>(0x168); //      Flags=1  
        public float _29_0x16C_float => ReadOffset<float>(0x16C); //      Flags=1  
        public float _30_0x170_float => ReadOffset<float>(0x170); //      Flags=1  
        public float _31_0x174_float => ReadOffset<float>(0x174); //      Flags=1  
        public float _32_0x178_float => ReadOffset<float>(0x178); //      Flags=1  
        public float _33_0x17C_float => ReadOffset<float>(0x17C); //      Flags=1  
        public int _34_0x180_Power_Sno => ReadOffset<int>(0x180); //      Flags=1  
        public int _35_0x184_int => ReadOffset<int>(0x184);
    }


    [CompilerGenerated]
    public class NativeLabelGBID : MemoryWrapper
    {
        public const int SizeOf = 272; // 0x110
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_int => ReadOffset<int>(0x108); //      Flags=1  
        public int _5_0x10C_int => ReadOffset<int>(0x10C);
    }


    [CompilerGenerated]
    public class NativeLootDistribution : MemoryWrapper
    {
        public const int SizeOf = 92; // 0x5C
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public int _5_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
        public int _6_0x14_int => ReadOffset<int>(0x14); //      Flags=1  
        public int _7_0x18_int => ReadOffset<int>(0x18); //      Flags=1  
        public int _8_0x1C_int => ReadOffset<int>(0x1C); //      Flags=1  
        public int _9_0x20_int => ReadOffset<int>(0x20); //      Flags=1  
        public int _10_0x24_int => ReadOffset<int>(0x24); //      Flags=1  
        public float _11_0x28_float => ReadOffset<float>(0x28); //      Flags=1  
        public float _12_0x2C_float => ReadOffset<float>(0x2C); //      Flags=1  
        public float _13_0x30_float => ReadOffset<float>(0x30); //      Flags=1  
        public float _14_0x34_float => ReadOffset<float>(0x34); //      Flags=1  
        public float _15_0x38_float => ReadOffset<float>(0x38); //      Flags=1  
        public float _16_0x3C_float => ReadOffset<float>(0x3C); //      Flags=1  
        public float _17_0x40_float => ReadOffset<float>(0x40); //      Flags=1  
        public float _18_0x44_float => ReadOffset<float>(0x44); //      Flags=1  
        public float _19_0x48_float => ReadOffset<float>(0x48); //      Flags=1  
        public float _20_0x4C_float => ReadOffset<float>(0x4C); //      Flags=1  
        public float _21_0x50_float => ReadOffset<float>(0x50); //      Flags=1  
        public int _22_0x54_int => ReadOffset<int>(0x54); //      Flags=1  
        public int _23_0x58_int => ReadOffset<int>(0x58); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeRareItemNamesEntry : MemoryWrapper
    {
        public const int SizeOf = 280; // 0x118
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public GameBalanceType _4_0x108_Enum => ReadOffset<GameBalanceType>(0x108); //      Flags=17  SymbolTable=@26700352 Min=1Max=50
        public int _5_0x10C_AxeBadData_GameBalanceId => ReadOffset<int>(0x10C); //      Flags=1  
        public AffixType _6_0x110_Enum => ReadOffset<AffixType>(0x110); //      Flags=17  SymbolTable=@26697912 Max=11
        public int _7_0x114_int => ReadOffset<int>(0x114);
    }


    [CompilerGenerated]
    public class NativeMonsterAffixesEntry : MemoryWrapper
    {
        public const int SizeOf = 904; // 0x388
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_int => ReadOffset<int>(0x108); //      Flags=1  
        public int _5_0x10C_int => ReadOffset<int>(0x10C); //      Flags=1  
        public int _6_0x110_int => ReadOffset<int>(0x110); //      Flags=1  
        //public _7_0x114_Enum => ReadOffset<>(0x114); //      Flags=17  SymbolTable=@26701888 Max=3
        public ElementType _8_0x118_Enum => ReadOffset<ElementType>(0x118); //      Flags=17  SymbolTable=@26701096 Min=-1Max=6
        public AffixType _9_0x11C_Enum => ReadOffset<AffixType>(0x11C); //      Flags=17  SymbolTable=@26697912 Max=11
        public int _10_0x120_int => ReadOffset<int>(0x120); //      Flags=1  
        public int _11_0x124_int => ReadOffset<int>(0x124); //      Flags=1  
        public int _12_0x128_int => ReadOffset<int>(0x128); //      Flags=1  
        public int _13_0x12C_int => ReadOffset<int>(0x12C); //      Flags=1  
                                                            // Unknown index=14 @Offset0x130 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
                                                            // Unknown index=15 @Offset0x134 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
                                                            // Unknown index=16 @Offset0x138 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _17_0x140_SerializeData => ReadObject<NativeSerializeData>(0x140);
        // Unknown index=18 @Offset0x148 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=19 @Offset0x14C Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=20 @Offset0x150 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _21_0x158_SerializeData => ReadObject<NativeSerializeData>(0x158);
        // Unknown index=22 @Offset0x160 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=23 @Offset0x164 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=24 @Offset0x168 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _25_0x170_SerializeData => ReadObject<NativeSerializeData>(0x170);
        // Unknown index=26 @Offset0x178 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=27 @Offset0x17C Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=28 @Offset0x180 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _29_0x188_SerializeData => ReadObject<NativeSerializeData>(0x188);
        // Unknown index=30 @Offset0x190 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=31 @Offset0x194 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=32 @Offset0x198 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _33_0x1A0_SerializeData => ReadObject<NativeSerializeData>(0x1A0);
        // Unknown index=34 @Offset0x1A8 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=35 @Offset0x1AC Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=36 @Offset0x1B0 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _37_0x1B8_SerializeData => ReadObject<NativeSerializeData>(0x1B8);
        // Unknown index=38 @Offset0x1C0 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=39 @Offset0x1C4 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=40 @Offset0x1C8 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _41_0x1D0_SerializeData => ReadObject<NativeSerializeData>(0x1D0);
        // Unknown index=42 @Offset0x1D8 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=43 @Offset0x1DC Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=44 @Offset0x1E0 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _45_0x1E8_SerializeData => ReadObject<NativeSerializeData>(0x1E8);
        // Unknown index=46 @Offset0x1F0 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=47 @Offset0x1F4 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=48 @Offset0x1F8 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _49_0x200_SerializeData => ReadObject<NativeSerializeData>(0x200);
        // Unknown index=50 @Offset0x208 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=51 @Offset0x20C Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=52 @Offset0x210 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _53_0x218_SerializeData => ReadObject<NativeSerializeData>(0x218);
        // Unknown index=54 @Offset0x220 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=55 @Offset0x224 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=56 @Offset0x228 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _57_0x230_SerializeData => ReadObject<NativeSerializeData>(0x230);
        // Unknown index=58 @Offset0x238 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=59 @Offset0x23C Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=60 @Offset0x240 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _61_0x248_SerializeData => ReadObject<NativeSerializeData>(0x248);
        // Unknown index=62 @Offset0x250 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=63 @Offset0x254 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=64 @Offset0x258 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _65_0x260_SerializeData => ReadObject<NativeSerializeData>(0x260);
        // Unknown index=66 @Offset0x268 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=67 @Offset0x26C Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=68 @Offset0x270 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _69_0x278_SerializeData => ReadObject<NativeSerializeData>(0x278);
        // Unknown index=70 @Offset0x280 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=71 @Offset0x284 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=72 @Offset0x288 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _73_0x290_SerializeData => ReadObject<NativeSerializeData>(0x290);
        // Unknown index=74 @Offset0x298 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=75 @Offset0x29C Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=76 @Offset0x2A0 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _77_0x2A8_SerializeData => ReadObject<NativeSerializeData>(0x2A8);
        // Unknown index=78 @Offset0x2B0 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=79 @Offset0x2B4 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=80 @Offset0x2B8 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _81_0x2C0_SerializeData => ReadObject<NativeSerializeData>(0x2C0);
        // Unknown index=82 @Offset0x2C8 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=83 @Offset0x2CC Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=84 @Offset0x2D0 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _85_0x2D8_SerializeData => ReadObject<NativeSerializeData>(0x2D8);
        // Unknown index=86 @Offset0x2E0 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=87 @Offset0x2E4 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=88 @Offset0x2E8 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _89_0x2F0_SerializeData => ReadObject<NativeSerializeData>(0x2F0);
        // Unknown index=90 @Offset0x2F8 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=91 @Offset0x2FC Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=92 @Offset0x300 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _93_0x308_SerializeData => ReadObject<NativeSerializeData>(0x308);
        public int _94_0x314_Power_Sno => ReadOffset<int>(0x314); //      Flags=1  
        public int _95_0x318_Power_Sno => ReadOffset<int>(0x318); //      Flags=1  
        public int _96_0x31C_Power_Sno => ReadOffset<int>(0x31C); //      Flags=1  
        public byte _97_0x320_byte => ReadOffset<byte>(0x320); //      Flags=1  
        public byte _98_0x321_byte => ReadOffset<byte>(0x321); //      Flags=1  
        public byte _99_0x322_byte => ReadOffset<byte>(0x322); //      Flags=1  
        public byte _100_0x323_byte => ReadOffset<byte>(0x323); //      Flags=1  
        public byte _101_0x324_byte => ReadOffset<byte>(0x324); //      Flags=1  
        public byte _102_0x325_byte => ReadOffset<byte>(0x325); //      Flags=1  
        public byte _103_0x326_byte => ReadOffset<byte>(0x326); //      Flags=1  
        public byte _104_0x327_byte => ReadOffset<byte>(0x327); //      Flags=1  
        public byte _105_0x328_byte => ReadOffset<byte>(0x328); //      Flags=1  
        public byte _106_0x329_byte => ReadOffset<byte>(0x329); //      Flags=1  
        public byte _107_0x32A_byte => ReadOffset<byte>(0x32A); //      Flags=1  
        public byte _108_0x32B_byte => ReadOffset<byte>(0x32B); //      Flags=1  
        public byte _109_0x32C_byte => ReadOffset<byte>(0x32C); //      Flags=1  
        public byte _110_0x32D_byte => ReadOffset<byte>(0x32D); //      Flags=1  
        public byte _111_0x32E_byte => ReadOffset<byte>(0x32E); //      Flags=1  
        public byte _112_0x32F_byte => ReadOffset<byte>(0x32F); //      Flags=1  
        public byte _113_0x330_byte => ReadOffset<byte>(0x330); //      Flags=1  
        public byte _114_0x331_byte => ReadOffset<byte>(0x331); //      Flags=1  
        public byte _115_0x332_byte => ReadOffset<byte>(0x332); //      Flags=1  
        public byte _116_0x333_byte => ReadOffset<byte>(0x333); //      Flags=1  
        public byte _117_0x334_byte => ReadOffset<byte>(0x334); //      Flags=1  
        public byte _118_0x335_byte => ReadOffset<byte>(0x335); //      Flags=1  
        public byte _119_0x336_byte => ReadOffset<byte>(0x336); //      Flags=1  
        public byte _120_0x337_byte => ReadOffset<byte>(0x337); //      Flags=1  
        public byte _121_0x338_byte => ReadOffset<byte>(0x338); //      Flags=1  
        public byte _122_0x339_byte => ReadOffset<byte>(0x339); //      Flags=1  
        public byte _123_0x33A_byte => ReadOffset<byte>(0x33A); //      Flags=1  
        public byte _124_0x33B_byte => ReadOffset<byte>(0x33B); //      Flags=1  
        public byte _125_0x33C_byte => ReadOffset<byte>(0x33C); //      Flags=1  
        public byte _126_0x33D_byte => ReadOffset<byte>(0x33D); //      Flags=1  
        public byte _127_0x33E_byte => ReadOffset<byte>(0x33E); //      Flags=1  
        public byte _128_0x33F_byte => ReadOffset<byte>(0x33F); //      Flags=1  
        public byte _129_0x340_byte => ReadOffset<byte>(0x340); //      Flags=1  
        public byte _130_0x341_byte => ReadOffset<byte>(0x341); //      Flags=1  
        public byte _131_0x342_byte => ReadOffset<byte>(0x342); //      Flags=1  
        public byte _132_0x343_byte => ReadOffset<byte>(0x343); //      Flags=1  
        public byte _133_0x344_byte => ReadOffset<byte>(0x344); //      Flags=1  
        public byte _134_0x345_byte => ReadOffset<byte>(0x345); //      Flags=1  
        public byte _135_0x346_byte => ReadOffset<byte>(0x346); //      Flags=1  
        public byte _136_0x347_byte => ReadOffset<byte>(0x347); //      Flags=1  
        public byte _137_0x348_byte => ReadOffset<byte>(0x348); //      Flags=1  
        public byte _138_0x349_byte => ReadOffset<byte>(0x349); //      Flags=1  
        public byte _139_0x34A_byte => ReadOffset<byte>(0x34A); //      Flags=1  
        public byte _140_0x34B_byte => ReadOffset<byte>(0x34B); //      Flags=1  
        public byte _141_0x34C_byte => ReadOffset<byte>(0x34C); //      Flags=1  
        public byte _142_0x34D_byte => ReadOffset<byte>(0x34D); //      Flags=1  
        public byte _143_0x34E_byte => ReadOffset<byte>(0x34E); //      Flags=1  
        public byte _144_0x34F_byte => ReadOffset<byte>(0x34F); //      Flags=1  
        public byte _145_0x350_byte => ReadOffset<byte>(0x350); //      Flags=1  
        public byte _146_0x351_byte => ReadOffset<byte>(0x351); //      Flags=1  
        public byte _147_0x352_byte => ReadOffset<byte>(0x352); //      Flags=1  
        public byte _148_0x353_byte => ReadOffset<byte>(0x353); //      Flags=1  
        public byte _149_0x354_byte => ReadOffset<byte>(0x354); //      Flags=1  
        public byte _150_0x355_byte => ReadOffset<byte>(0x355); //      Flags=1  
        public byte _151_0x356_byte => ReadOffset<byte>(0x356); //      Flags=1  
        public byte _152_0x357_byte => ReadOffset<byte>(0x357); //      Flags=1  
        public byte _153_0x358_byte => ReadOffset<byte>(0x358); //      Flags=1  
        public byte _154_0x359_byte => ReadOffset<byte>(0x359); //      Flags=1  
        public byte _155_0x35A_byte => ReadOffset<byte>(0x35A); //      Flags=1  
        public byte _156_0x35B_byte => ReadOffset<byte>(0x35B); //      Flags=1  
        public byte _157_0x35C_byte => ReadOffset<byte>(0x35C); //      Flags=1  
        public byte _158_0x35D_byte => ReadOffset<byte>(0x35D); //      Flags=1  
        public byte _159_0x35E_byte => ReadOffset<byte>(0x35E); //      Flags=1  
        public byte _160_0x35F_byte => ReadOffset<byte>(0x35F); //      Flags=1  
        public byte _161_0x360_byte => ReadOffset<byte>(0x360); //      Flags=1  
        public byte _162_0x361_byte => ReadOffset<byte>(0x361); //      Flags=1  
        public byte _163_0x362_byte => ReadOffset<byte>(0x362); //      Flags=1  
        public byte _164_0x363_byte => ReadOffset<byte>(0x363); //      Flags=1  
        public byte _165_0x364_byte => ReadOffset<byte>(0x364); //      Flags=1  
        public byte _166_0x365_byte => ReadOffset<byte>(0x365); //      Flags=1  
        public byte _167_0x366_byte => ReadOffset<byte>(0x366); //      Flags=1  
        public byte _168_0x367_byte => ReadOffset<byte>(0x367); //      Flags=1  
        public byte _169_0x368_byte => ReadOffset<byte>(0x368); //      Flags=1  
        public byte _170_0x369_byte => ReadOffset<byte>(0x369); //      Flags=1  
        public byte _171_0x36A_byte => ReadOffset<byte>(0x36A); //      Flags=1  
        public byte _172_0x36B_byte => ReadOffset<byte>(0x36B); //      Flags=1  
        public byte _173_0x36C_byte => ReadOffset<byte>(0x36C); //      Flags=1  
        public byte _174_0x36D_byte => ReadOffset<byte>(0x36D); //      Flags=1  
        public byte _175_0x36E_byte => ReadOffset<byte>(0x36E); //      Flags=1  
        public byte _176_0x36F_byte => ReadOffset<byte>(0x36F); //      Flags=1  
        public byte _177_0x370_byte => ReadOffset<byte>(0x370); //      Flags=1  
        public byte _178_0x371_byte => ReadOffset<byte>(0x371); //      Flags=1  
        public byte _179_0x372_byte => ReadOffset<byte>(0x372); //      Flags=1  
        public byte _180_0x373_byte => ReadOffset<byte>(0x373); //      Flags=1  
        public byte _181_0x374_byte => ReadOffset<byte>(0x374); //      Flags=1  
        public byte _182_0x375_byte => ReadOffset<byte>(0x375); //      Flags=1  
        public byte _183_0x376_byte => ReadOffset<byte>(0x376); //      Flags=1  
        public byte _184_0x377_byte => ReadOffset<byte>(0x377); //      Flags=1  
        public byte _185_0x378_byte => ReadOffset<byte>(0x378); //      Flags=1  
        public byte _186_0x379_byte => ReadOffset<byte>(0x379); //      Flags=1  
        public byte _187_0x37A_byte => ReadOffset<byte>(0x37A); //      Flags=1  
        public byte _188_0x37B_byte => ReadOffset<byte>(0x37B); //      Flags=1  
        public byte _189_0x37C_byte => ReadOffset<byte>(0x37C); //      Flags=1  
        public byte _190_0x37D_byte => ReadOffset<byte>(0x37D); //      Flags=1  
        public byte _191_0x37E_byte => ReadOffset<byte>(0x37E); //      Flags=1  
        public byte _192_0x37F_byte => ReadOffset<byte>(0x37F); //      Flags=1  
        public byte _193_0x380_byte => ReadOffset<byte>(0x380); //      Flags=1  
        public byte _194_0x381_byte => ReadOffset<byte>(0x381); //      Flags=1  
        public byte _195_0x382_byte => ReadOffset<byte>(0x382); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeRareMonsterNamesEntry : MemoryWrapper
    {
        public const int SizeOf = 400; // 0x190
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public AffixType _4_0x108_Enum => ReadOffset<AffixType>(0x108); //      Flags=17  SymbolTable=@26697912 Max=11
        public string _5_0x10C_String => ReadString(0x10C); //      Flags=1  
        public int _6_0x18C_int => ReadOffset<int>(0x18C);
    }


    [CompilerGenerated]
    public class NativeSocketedEffectsTableEntry : MemoryWrapper
    {
        public const int SizeOf = 1416; // 0x588
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_Items_GameBalanceId => ReadOffset<int>(0x108); //      Flags=1  
        public int _5_0x10C_ItemTypes_GameBalanceId => ReadOffset<int>(0x10C); //      Flags=1  
                                                                               // Unknown index=6 @Offset0x110 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
                                                                               // Unknown index=7 @Offset0x114 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
                                                                               // Unknown index=8 @Offset0x118 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _9_0x120_SerializeData => ReadObject<NativeSerializeData>(0x120);
        // Unknown index=10 @Offset0x128 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=11 @Offset0x12C Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=12 @Offset0x130 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _13_0x138_SerializeData => ReadObject<NativeSerializeData>(0x138);
        // Unknown index=14 @Offset0x140 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=15 @Offset0x144 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=16 @Offset0x148 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _17_0x150_SerializeData => ReadObject<NativeSerializeData>(0x150);
        // Unknown index=18 @Offset0x158 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=19 @Offset0x15C Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=20 @Offset0x160 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _21_0x168_SerializeData => ReadObject<NativeSerializeData>(0x168);
        // Unknown index=22 @Offset0x170 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
        // Unknown index=23 @Offset0x174 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
        // Unknown index=24 @Offset0x178 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _25_0x180_SerializeData => ReadObject<NativeSerializeData>(0x180);
        public string _26_0x188_String => ReadString(0x188); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeItemDropTableEntry : MemoryWrapper
    {
        public const int SizeOf = 1144; // 0x478
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_int => ReadOffset<int>(0x108); //      Flags=1  
        public int _5_0x10C_int => ReadOffset<int>(0x10C); //      Flags=1  
        public int _6_0x110_int => ReadOffset<int>(0x110); //      Flags=1  
        public int _7_0x114_int => ReadOffset<int>(0x114); //      Flags=1  
        public int _8_0x118_int => ReadOffset<int>(0x118); //      Flags=1  
        public int _9_0x11C_int => ReadOffset<int>(0x11C); //      Flags=1  
        public int _10_0x120_int => ReadOffset<int>(0x120); //      Flags=1  
        public int _11_0x124_int => ReadOffset<int>(0x124); //      Flags=1  
        public int _12_0x128_int => ReadOffset<int>(0x128); //      Flags=1  
        public int _13_0x12C_int => ReadOffset<int>(0x12C); //      Flags=1  
        public int _14_0x130_int => ReadOffset<int>(0x130); //      Flags=1  
        public int _15_0x134_int => ReadOffset<int>(0x134); //      Flags=1  
        public int _16_0x138_int => ReadOffset<int>(0x138); //      Flags=1  
        public int _17_0x13C_int => ReadOffset<int>(0x13C); //      Flags=1  
        public int _18_0x140_int => ReadOffset<int>(0x140); //      Flags=1  
        public int _19_0x144_int => ReadOffset<int>(0x144); //      Flags=1  
        public int _20_0x148_int => ReadOffset<int>(0x148); //      Flags=1  
        public int _21_0x14C_int => ReadOffset<int>(0x14C); //      Flags=1  
        public int _22_0x150_int => ReadOffset<int>(0x150); //      Flags=1  
        public int _23_0x154_int => ReadOffset<int>(0x154); //      Flags=1  
        public int _24_0x158_int => ReadOffset<int>(0x158); //      Flags=1  
        public int _25_0x15C_int => ReadOffset<int>(0x15C); //      Flags=1  
        public int _26_0x160_int => ReadOffset<int>(0x160); //      Flags=1  
        public int _27_0x164_int => ReadOffset<int>(0x164); //      Flags=1  
        public int _28_0x168_int => ReadOffset<int>(0x168); //      Flags=1  
        public int _29_0x16C_int => ReadOffset<int>(0x16C); //      Flags=1  
        public int _30_0x170_int => ReadOffset<int>(0x170); //      Flags=1  
        public int _31_0x174_int => ReadOffset<int>(0x174); //      Flags=1  
        public int _32_0x178_int => ReadOffset<int>(0x178); //      Flags=1  
        public int _33_0x17C_int => ReadOffset<int>(0x17C); //      Flags=1  
        public int _34_0x180_int => ReadOffset<int>(0x180); //      Flags=1  
        public int _35_0x184_int => ReadOffset<int>(0x184); //      Flags=1  
        public int _36_0x188_int => ReadOffset<int>(0x188); //      Flags=1  
        public int _37_0x18C_int => ReadOffset<int>(0x18C); //      Flags=1  
        public int _38_0x190_int => ReadOffset<int>(0x190); //      Flags=1  
        public int _39_0x194_int => ReadOffset<int>(0x194); //      Flags=1  
        public int _40_0x198_int => ReadOffset<int>(0x198); //      Flags=1  
        public int _41_0x19C_int => ReadOffset<int>(0x19C); //      Flags=1  
        public int _42_0x1A0_int => ReadOffset<int>(0x1A0); //      Flags=1  
        public int _43_0x1A4_int => ReadOffset<int>(0x1A4); //      Flags=1  
        public int _44_0x1A8_int => ReadOffset<int>(0x1A8); //      Flags=1  
        public int _45_0x1AC_int => ReadOffset<int>(0x1AC); //      Flags=1  
        public int _46_0x1B0_int => ReadOffset<int>(0x1B0); //      Flags=1  
        public int _47_0x1B4_int => ReadOffset<int>(0x1B4); //      Flags=1  
        public int _48_0x1B8_int => ReadOffset<int>(0x1B8); //      Flags=1  
        public int _49_0x1BC_int => ReadOffset<int>(0x1BC); //      Flags=1  
        public int _50_0x1C0_int => ReadOffset<int>(0x1C0); //      Flags=1  
        public int _51_0x1C4_int => ReadOffset<int>(0x1C4); //      Flags=1  
        public int _52_0x1C8_int => ReadOffset<int>(0x1C8); //      Flags=1  
        public int _53_0x1CC_int => ReadOffset<int>(0x1CC); //      Flags=1  
        public int _54_0x1D0_int => ReadOffset<int>(0x1D0); //      Flags=1  
        public int _55_0x1D4_int => ReadOffset<int>(0x1D4); //      Flags=1  
        public int _56_0x1D8_int => ReadOffset<int>(0x1D8); //      Flags=1  
        public int _57_0x1DC_int => ReadOffset<int>(0x1DC); //      Flags=1  
        public int _58_0x1E0_int => ReadOffset<int>(0x1E0); //      Flags=1  
        public int _59_0x1E4_int => ReadOffset<int>(0x1E4); //      Flags=1  
        public int _60_0x1E8_int => ReadOffset<int>(0x1E8); //      Flags=1  
        public int _61_0x1EC_int => ReadOffset<int>(0x1EC); //      Flags=1  
        public int _62_0x1F0_int => ReadOffset<int>(0x1F0); //      Flags=1  
        public int _63_0x1F4_int => ReadOffset<int>(0x1F4); //      Flags=1  
        public int _64_0x1F8_int => ReadOffset<int>(0x1F8); //      Flags=1  
        public int _65_0x1FC_int => ReadOffset<int>(0x1FC); //      Flags=1  
        public int _66_0x200_int => ReadOffset<int>(0x200); //      Flags=1  
        public int _67_0x204_int => ReadOffset<int>(0x204); //      Flags=1  
        public int _68_0x208_int => ReadOffset<int>(0x208); //      Flags=1  
        public int _69_0x20C_int => ReadOffset<int>(0x20C); //      Flags=1  
        public int _70_0x210_int => ReadOffset<int>(0x210); //      Flags=1  
        public int _71_0x214_int => ReadOffset<int>(0x214); //      Flags=1  
        public int _72_0x218_int => ReadOffset<int>(0x218); //      Flags=1  
        public int _73_0x21C_int => ReadOffset<int>(0x21C); //      Flags=1  
        public int _74_0x220_int => ReadOffset<int>(0x220); //      Flags=1  
        public int _75_0x224_int => ReadOffset<int>(0x224); //      Flags=1  
        public int _76_0x228_int => ReadOffset<int>(0x228); //      Flags=1  
        public int _77_0x22C_int => ReadOffset<int>(0x22C); //      Flags=1  
        public int _78_0x230_int => ReadOffset<int>(0x230); //      Flags=1  
        public int _79_0x234_int => ReadOffset<int>(0x234); //      Flags=1  
        public int _80_0x238_int => ReadOffset<int>(0x238); //      Flags=1  
        public int _81_0x23C_int => ReadOffset<int>(0x23C); //      Flags=1  
        public int _82_0x240_int => ReadOffset<int>(0x240); //      Flags=1  
        public int _83_0x244_int => ReadOffset<int>(0x244); //      Flags=1  
        public int _84_0x248_int => ReadOffset<int>(0x248); //      Flags=1  
        public int _85_0x24C_int => ReadOffset<int>(0x24C); //      Flags=1  
        public int _86_0x250_int => ReadOffset<int>(0x250); //      Flags=1  
        public int _87_0x254_int => ReadOffset<int>(0x254); //      Flags=1  
        public int _88_0x258_int => ReadOffset<int>(0x258); //      Flags=1  
        public int _89_0x25C_int => ReadOffset<int>(0x25C); //      Flags=1  
        public int _90_0x260_int => ReadOffset<int>(0x260); //      Flags=1  
        public int _91_0x264_int => ReadOffset<int>(0x264); //      Flags=1  
        public int _92_0x268_int => ReadOffset<int>(0x268); //      Flags=1  
        public int _93_0x26C_int => ReadOffset<int>(0x26C); //      Flags=1  
        public int _94_0x270_int => ReadOffset<int>(0x270); //      Flags=1  
        public int _95_0x274_int => ReadOffset<int>(0x274); //      Flags=1  
        public int _96_0x278_int => ReadOffset<int>(0x278); //      Flags=1  
        public int _97_0x27C_int => ReadOffset<int>(0x27C); //      Flags=1  
        public int _98_0x280_int => ReadOffset<int>(0x280); //      Flags=1  
        public int _99_0x284_int => ReadOffset<int>(0x284); //      Flags=1  
        public int _100_0x288_int => ReadOffset<int>(0x288); //      Flags=1  
        public int _101_0x28C_int => ReadOffset<int>(0x28C); //      Flags=1  
        public int _102_0x290_int => ReadOffset<int>(0x290); //      Flags=1  
        public int _103_0x294_int => ReadOffset<int>(0x294); //      Flags=1  
        public int _104_0x298_int => ReadOffset<int>(0x298); //      Flags=1  
        public int _105_0x29C_int => ReadOffset<int>(0x29C); //      Flags=1  
        public int _106_0x2A0_int => ReadOffset<int>(0x2A0); //      Flags=1  
        public int _107_0x2A4_int => ReadOffset<int>(0x2A4); //      Flags=1  
        public int _108_0x2A8_int => ReadOffset<int>(0x2A8); //      Flags=1  
        public int _109_0x2AC_int => ReadOffset<int>(0x2AC); //      Flags=1  
        public int _110_0x2B0_int => ReadOffset<int>(0x2B0); //      Flags=1  
        public int _111_0x2B4_int => ReadOffset<int>(0x2B4); //      Flags=1  
        public int _112_0x2B8_int => ReadOffset<int>(0x2B8); //      Flags=1  
        public int _113_0x2BC_int => ReadOffset<int>(0x2BC); //      Flags=1  
        public int _114_0x2C0_int => ReadOffset<int>(0x2C0); //      Flags=1  
        public int _115_0x2C4_int => ReadOffset<int>(0x2C4); //      Flags=1  
        public int _116_0x2C8_int => ReadOffset<int>(0x2C8); //      Flags=1  
        public int _117_0x2CC_int => ReadOffset<int>(0x2CC); //      Flags=1  
        public int _118_0x2D0_int => ReadOffset<int>(0x2D0); //      Flags=1  
        public int _119_0x2D4_int => ReadOffset<int>(0x2D4); //      Flags=1  
        public int _120_0x2D8_int => ReadOffset<int>(0x2D8); //      Flags=1  
        public int _121_0x2DC_int => ReadOffset<int>(0x2DC); //      Flags=1  
        public int _122_0x2E0_int => ReadOffset<int>(0x2E0); //      Flags=1  
        public int _123_0x2E4_int => ReadOffset<int>(0x2E4); //      Flags=1  
        public int _124_0x2E8_int => ReadOffset<int>(0x2E8); //      Flags=1  
        public int _125_0x2EC_int => ReadOffset<int>(0x2EC); //      Flags=1  
        public int _126_0x2F0_int => ReadOffset<int>(0x2F0); //      Flags=1  
        public int _127_0x2F4_int => ReadOffset<int>(0x2F4); //      Flags=1  
        public int _128_0x2F8_int => ReadOffset<int>(0x2F8); //      Flags=1  
        public int _129_0x2FC_int => ReadOffset<int>(0x2FC); //      Flags=1  
        public int _130_0x300_int => ReadOffset<int>(0x300); //      Flags=1  
        public int _131_0x304_int => ReadOffset<int>(0x304); //      Flags=1  
        public int _132_0x308_int => ReadOffset<int>(0x308); //      Flags=1  
        public int _133_0x30C_int => ReadOffset<int>(0x30C); //      Flags=1  
        public int _134_0x310_int => ReadOffset<int>(0x310); //      Flags=1  
        public int _135_0x314_int => ReadOffset<int>(0x314); //      Flags=1  
        public int _136_0x318_int => ReadOffset<int>(0x318); //      Flags=1  
        public int _137_0x31C_int => ReadOffset<int>(0x31C); //      Flags=1  
        public int _138_0x320_int => ReadOffset<int>(0x320); //      Flags=1  
        public int _139_0x324_int => ReadOffset<int>(0x324); //      Flags=1  
        public int _140_0x328_int => ReadOffset<int>(0x328); //      Flags=1  
        public int _141_0x32C_int => ReadOffset<int>(0x32C); //      Flags=1  
        public int _142_0x330_int => ReadOffset<int>(0x330); //      Flags=1  
        public int _143_0x334_int => ReadOffset<int>(0x334); //      Flags=1  
        public int _144_0x338_int => ReadOffset<int>(0x338); //      Flags=1  
        public int _145_0x33C_int => ReadOffset<int>(0x33C); //      Flags=1  
        public int _146_0x340_int => ReadOffset<int>(0x340); //      Flags=1  
        public int _147_0x344_int => ReadOffset<int>(0x344); //      Flags=1  
        public int _148_0x348_int => ReadOffset<int>(0x348); //      Flags=1  
        public int _149_0x34C_int => ReadOffset<int>(0x34C); //      Flags=1  
        public int _150_0x350_int => ReadOffset<int>(0x350); //      Flags=1  
        public int _151_0x354_int => ReadOffset<int>(0x354); //      Flags=1  
        public int _152_0x358_int => ReadOffset<int>(0x358); //      Flags=1  
        public int _153_0x35C_int => ReadOffset<int>(0x35C); //      Flags=1  
        public int _154_0x360_int => ReadOffset<int>(0x360); //      Flags=1  
        public int _155_0x364_int => ReadOffset<int>(0x364); //      Flags=1  
        public int _156_0x368_int => ReadOffset<int>(0x368); //      Flags=1  
        public int _157_0x36C_int => ReadOffset<int>(0x36C); //      Flags=1  
        public int _158_0x370_int => ReadOffset<int>(0x370); //      Flags=1  
        public int _159_0x374_int => ReadOffset<int>(0x374); //      Flags=1  
        public int _160_0x378_int => ReadOffset<int>(0x378); //      Flags=1  
        public int _161_0x37C_int => ReadOffset<int>(0x37C); //      Flags=1  
        public int _162_0x380_int => ReadOffset<int>(0x380); //      Flags=1  
        public int _163_0x384_int => ReadOffset<int>(0x384); //      Flags=1  
        public int _164_0x388_int => ReadOffset<int>(0x388); //      Flags=1  
        public int _165_0x38C_int => ReadOffset<int>(0x38C); //      Flags=1  
        public int _166_0x390_int => ReadOffset<int>(0x390); //      Flags=1  
        public int _167_0x394_int => ReadOffset<int>(0x394); //      Flags=1  
        public int _168_0x398_int => ReadOffset<int>(0x398); //      Flags=1  
        public int _169_0x39C_int => ReadOffset<int>(0x39C); //      Flags=1  
        public int _170_0x3A0_int => ReadOffset<int>(0x3A0); //      Flags=1  
        public int _171_0x3A4_int => ReadOffset<int>(0x3A4); //      Flags=1  
        public int _172_0x3A8_int => ReadOffset<int>(0x3A8); //      Flags=1  
        public int _173_0x3AC_int => ReadOffset<int>(0x3AC); //      Flags=1  
        public int _174_0x3B0_int => ReadOffset<int>(0x3B0); //      Flags=1  
        public int _175_0x3B4_int => ReadOffset<int>(0x3B4); //      Flags=1  
        public int _176_0x3B8_int => ReadOffset<int>(0x3B8); //      Flags=1  
        public int _177_0x3BC_int => ReadOffset<int>(0x3BC); //      Flags=1  
        public int _178_0x3C0_int => ReadOffset<int>(0x3C0); //      Flags=1  
        public int _179_0x3C4_int => ReadOffset<int>(0x3C4); //      Flags=1  
        public int _180_0x3C8_int => ReadOffset<int>(0x3C8); //      Flags=1  
        public int _181_0x3CC_int => ReadOffset<int>(0x3CC); //      Flags=1  
        public int _182_0x3D0_int => ReadOffset<int>(0x3D0); //      Flags=1  
        public int _183_0x3D4_int => ReadOffset<int>(0x3D4); //      Flags=1  
        public int _184_0x3D8_int => ReadOffset<int>(0x3D8); //      Flags=1  
        public int _185_0x3DC_int => ReadOffset<int>(0x3DC); //      Flags=1  
        public int _186_0x3E0_int => ReadOffset<int>(0x3E0); //      Flags=1  
        public int _187_0x3E4_int => ReadOffset<int>(0x3E4); //      Flags=1  
        public int _188_0x3E8_int => ReadOffset<int>(0x3E8); //      Flags=1  
        public int _189_0x3EC_int => ReadOffset<int>(0x3EC); //      Flags=1  
        public int _190_0x3F0_int => ReadOffset<int>(0x3F0); //      Flags=1  
        public int _191_0x3F4_int => ReadOffset<int>(0x3F4); //      Flags=1  
        public int _192_0x3F8_int => ReadOffset<int>(0x3F8); //      Flags=1  
        public int _193_0x3FC_int => ReadOffset<int>(0x3FC); //      Flags=1  
        public int _194_0x404_int => ReadOffset<int>(0x404); //      Flags=1  
        public int _195_0x408_int => ReadOffset<int>(0x408); //      Flags=1  
        public int _196_0x40C_int => ReadOffset<int>(0x40C); //      Flags=1  
        public int _197_0x410_int => ReadOffset<int>(0x410); //      Flags=1  
        public int _198_0x414_int => ReadOffset<int>(0x414); //      Flags=1  
        public int _199_0x418_int => ReadOffset<int>(0x418); //      Flags=1  
        public int _200_0x41C_int => ReadOffset<int>(0x41C); //      Flags=1  
        public int _201_0x420_int => ReadOffset<int>(0x420); //      Flags=1  
        public int _202_0x424_int => ReadOffset<int>(0x424); //      Flags=1  
        public int _203_0x428_int => ReadOffset<int>(0x428); //      Flags=1  
        public int _204_0x42C_int => ReadOffset<int>(0x42C); //      Flags=1  
        public int _205_0x430_int => ReadOffset<int>(0x430); //      Flags=1  
        public int _206_0x434_int => ReadOffset<int>(0x434); //      Flags=1  
        public int _207_0x438_int => ReadOffset<int>(0x438); //      Flags=1  
        public int _208_0x43C_int => ReadOffset<int>(0x43C); //      Flags=1  
        public int _209_0x440_int => ReadOffset<int>(0x440); //      Flags=1  
        public int _210_0x444_int => ReadOffset<int>(0x444); //      Flags=1  
        public int _211_0x448_int => ReadOffset<int>(0x448); //      Flags=1  
        public int _212_0x44C_int => ReadOffset<int>(0x44C); //      Flags=1  
        public int _213_0x450_int => ReadOffset<int>(0x450); //      Flags=1  
        public int _214_0x454_int => ReadOffset<int>(0x454); //      Flags=1  
        public int _215_0x458_int => ReadOffset<int>(0x458); //      Flags=1  
        public int _216_0x45C_int => ReadOffset<int>(0x45C); //      Flags=1  
        public int _217_0x460_int => ReadOffset<int>(0x460); //      Flags=1  
        public int _218_0x464_int => ReadOffset<int>(0x464); //      Flags=1  
        public int _219_0x468_int => ReadOffset<int>(0x468); //      Flags=1  
        public int _220_0x46C_int => ReadOffset<int>(0x46C); //      Flags=1  
        public int _221_0x470_int => ReadOffset<int>(0x470); //      Flags=1  
        public int _222_0x474_int => ReadOffset<int>(0x474); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeQualityClass : MemoryWrapper
    {
        public const int SizeOf = 360; // 0x168
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public float _4_0x10C_float => ReadOffset<float>(0x10C); //      Flags=1  
        public float _5_0x110_float => ReadOffset<float>(0x110); //      Flags=1  
        public float _6_0x114_float => ReadOffset<float>(0x114); //      Flags=1  
        public float _7_0x118_float => ReadOffset<float>(0x118); //      Flags=1  
        public float _8_0x11C_float => ReadOffset<float>(0x11C); //      Flags=1  
        public float _9_0x120_float => ReadOffset<float>(0x120); //      Flags=1  
        public float _10_0x124_float => ReadOffset<float>(0x124); //      Flags=1  
        public float _11_0x128_float => ReadOffset<float>(0x128); //      Flags=1  
        public float _12_0x12C_float => ReadOffset<float>(0x12C); //      Flags=1  
        public float _13_0x130_float => ReadOffset<float>(0x130); //      Flags=1  
        public float _14_0x134_float => ReadOffset<float>(0x134); //      Flags=1  
        public float _15_0x138_float => ReadOffset<float>(0x138); //      Flags=1  
        public float _16_0x13C_float => ReadOffset<float>(0x13C); //      Flags=1  
        public float _17_0x140_float => ReadOffset<float>(0x140); //      Flags=1  
        public float _18_0x144_float => ReadOffset<float>(0x144); //      Flags=1  
        public float _19_0x148_float => ReadOffset<float>(0x148); //      Flags=1  
        public float _20_0x14C_float => ReadOffset<float>(0x14C); //      Flags=1  
        public float _21_0x150_float => ReadOffset<float>(0x150); //      Flags=1  
        public float _22_0x154_float => ReadOffset<float>(0x154); //      Flags=1  
        public float _23_0x158_float => ReadOffset<float>(0x158); //      Flags=1  
        public float _24_0x15C_float => ReadOffset<float>(0x15C); //      Flags=1  
        public float _25_0x160_float => ReadOffset<float>(0x160); //      Flags=1  
        public int _26_0x164_int => ReadOffset<int>(0x164);
    }


    [CompilerGenerated]
    public class NativeHandicapLevel : MemoryWrapper
    {
        public const int SizeOf = 32; // 0x20
        public float _1_0x0_float => ReadOffset<float>(0x0); //      Flags=1  
        public float _2_0x4_float => ReadOffset<float>(0x4); //      Flags=1  
        public float _3_0x8_float => ReadOffset<float>(0x8); //      Flags=1  
        public float _4_0xC_float => ReadOffset<float>(0xC); //      Flags=1  
        public float _5_0x10_float => ReadOffset<float>(0x10); //      Flags=1  
        public float _6_0x14_float => ReadOffset<float>(0x14); //      Flags=1  
        public int _7_0x18_int => ReadOffset<int>(0x18); //      Flags=1  
        public int _8_0x1C_int => ReadOffset<int>(0x1C);
    }


    [CompilerGenerated]
    public class NativeItemSalvageLevel : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public int _1_0x0_TreasureClass_Sno => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_TreasureClass_Sno => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_TreasureClass_Sno => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_TreasureClass_Sno => ReadOffset<int>(0xC); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeHirelingEntry : MemoryWrapper
    {
        public const int SizeOf = 328; // 0x148
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_Actor_Sno => ReadOffset<int>(0x108); //      Flags=1  
        public int _5_0x10C_Actor_Sno => ReadOffset<int>(0x10C); //      Flags=1  
        public int _6_0x110_Actor_Sno => ReadOffset<int>(0x110); //      Flags=1  
        public int _7_0x114_TreasureClass_Sno => ReadOffset<int>(0x114); //      Flags=1  
        public PlayerAttribute _8_0x118_Enum => ReadOffset<PlayerAttribute>(0x118); //      Flags=17  SymbolTable=@26708552 Min=-1Max=2
        public float _9_0x11C_float => ReadOffset<float>(0x11C); //      Flags=1  
        public float _10_0x120_float => ReadOffset<float>(0x120); //      Flags=1  
        public float _11_0x124_float => ReadOffset<float>(0x124); //      Flags=1  
        public float _12_0x128_float => ReadOffset<float>(0x128); //      Flags=1  
        public float _13_0x12C_float => ReadOffset<float>(0x12C); //      Flags=1  
        public float _14_0x130_float => ReadOffset<float>(0x130); //      Flags=1  
        public float _15_0x134_float => ReadOffset<float>(0x134); //      Flags=1  
        public float _16_0x138_float => ReadOffset<float>(0x138); //      Flags=1  
        public float _17_0x13C_float => ReadOffset<float>(0x13C); //      Flags=1  
        public float _18_0x140_float => ReadOffset<float>(0x140); //      Flags=1  
        public float _19_0x144_float => ReadOffset<float>(0x144); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeSetItemBonusTableEntry : MemoryWrapper
    {
        public const int SizeOf = 464; // 0x1D0
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_SetItemBonuses_GameBalanceId => ReadOffset<int>(0x108); //      Flags=1  
        public int _5_0x10C_int => ReadOffset<int>(0x10C); //      Flags=1  
        public List<NativeAttributeSpecifier> _6_0x110_FixedArray => ReadObjects<NativeAttributeSpecifier>(0x110, 8); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeEliteModifierData : MemoryWrapper
    {
        public const int SizeOf = 352; // 0x160
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public float _4_0x108_float => ReadOffset<float>(0x108); //      Flags=1  
                                                                 // Unknown index=5 @Offset0x10C Type=DT_TIME BaseType=DT_NULL //      Flags=1  
        public float _6_0x110_float => ReadOffset<float>(0x110); //      Flags=1  
                                                                 // Unknown index=7 @Offset0x114 Type=DT_TIME BaseType=DT_NULL //      Flags=1  
        public float _8_0x118_float => ReadOffset<float>(0x118); //      Flags=1  
                                                                 // Unknown index=9 @Offset0x11C Type=DT_TIME BaseType=DT_NULL //      Flags=1  
        public float _10_0x120_float => ReadOffset<float>(0x120); //      Flags=1  
                                                                  // Unknown index=11 @Offset0x124 Type=DT_TIME BaseType=DT_NULL //      Flags=1  
        public float _12_0x128_float => ReadOffset<float>(0x128); //      Flags=1  
                                                                  // Unknown index=13 @Offset0x12C Type=DT_TIME BaseType=DT_NULL //      Flags=1  
        public float _14_0x130_float => ReadOffset<float>(0x130); //      Flags=1  
                                                                  // Unknown index=15 @Offset0x134 Type=DT_TIME BaseType=DT_NULL //      Flags=1  
        public float _16_0x138_float => ReadOffset<float>(0x138); //      Flags=1  
                                                                  // Unknown index=17 @Offset0x13C Type=DT_TIME BaseType=DT_NULL //      Flags=1  
        public float _18_0x140_float => ReadOffset<float>(0x140); //      Flags=1  
        public float _19_0x144_float => ReadOffset<float>(0x144); //      Flags=1  
                                                                  // Unknown index=20 @Offset0x148 Type=DT_TIME BaseType=DT_NULL //      Flags=1  
        public float _21_0x14C_float => ReadOffset<float>(0x14C); //      Flags=1  
        public float _22_0x150_float => ReadOffset<float>(0x150); //      Flags=1  
        public float _23_0x154_float => ReadOffset<float>(0x154); //      Flags=1  
        public float _24_0x158_float => ReadOffset<float>(0x158); //      Flags=1  
        public int _25_0x15C_int => ReadOffset<int>(0x15C);
    }


    [CompilerGenerated]
    public class NativeItemTierData : MemoryWrapper
    {
        public const int SizeOf = 32; // 0x20
        public int _1_0x0_Items_GameBalanceId => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_Items_GameBalanceId => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_Items_GameBalanceId => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_Items_GameBalanceId => ReadOffset<int>(0xC); //      Flags=1  
        public int _5_0x10_Items_GameBalanceId => ReadOffset<int>(0x10); //      Flags=1  
        public int _6_0x14_Items_GameBalanceId => ReadOffset<int>(0x14); //      Flags=1  
        public int _7_0x18_Items_GameBalanceId => ReadOffset<int>(0x18); //      Flags=1  
        public int _8_0x1C_Items_GameBalanceId => ReadOffset<int>(0x1C); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativePowerFormulaTableEntry : MemoryWrapper
    {
        public const int SizeOf = 1328; // 0x530
        public string _1_0x0_String => ReadString(0x0); //      Flags=1  
        public float _2_0x400_float => ReadOffset<float>(0x400); //      Flags=1  
        public float _3_0x404_float => ReadOffset<float>(0x404); //      Flags=1  
        public float _4_0x408_float => ReadOffset<float>(0x408); //      Flags=1  
        public float _5_0x40C_float => ReadOffset<float>(0x40C); //      Flags=1  
        public float _6_0x410_float => ReadOffset<float>(0x410); //      Flags=1  
        public float _7_0x414_float => ReadOffset<float>(0x414); //      Flags=1  
        public float _8_0x418_float => ReadOffset<float>(0x418); //      Flags=1  
        public float _9_0x41C_float => ReadOffset<float>(0x41C); //      Flags=1  
        public float _10_0x420_float => ReadOffset<float>(0x420); //      Flags=1  
        public float _11_0x424_float => ReadOffset<float>(0x424); //      Flags=1  
        public float _12_0x428_float => ReadOffset<float>(0x428); //      Flags=1  
        public float _13_0x42C_float => ReadOffset<float>(0x42C); //      Flags=1  
        public float _14_0x430_float => ReadOffset<float>(0x430); //      Flags=1  
        public float _15_0x434_float => ReadOffset<float>(0x434); //      Flags=1  
        public float _16_0x438_float => ReadOffset<float>(0x438); //      Flags=1  
        public float _17_0x43C_float => ReadOffset<float>(0x43C); //      Flags=1  
        public float _18_0x440_float => ReadOffset<float>(0x440); //      Flags=1  
        public float _19_0x444_float => ReadOffset<float>(0x444); //      Flags=1  
        public float _20_0x448_float => ReadOffset<float>(0x448); //      Flags=1  
        public float _21_0x44C_float => ReadOffset<float>(0x44C); //      Flags=1  
        public float _22_0x450_float => ReadOffset<float>(0x450); //      Flags=1  
        public float _23_0x454_float => ReadOffset<float>(0x454); //      Flags=1  
        public float _24_0x458_float => ReadOffset<float>(0x458); //      Flags=1  
        public float _25_0x45C_float => ReadOffset<float>(0x45C); //      Flags=1  
        public float _26_0x460_float => ReadOffset<float>(0x460); //      Flags=1  
        public float _27_0x464_float => ReadOffset<float>(0x464); //      Flags=1  
        public float _28_0x468_float => ReadOffset<float>(0x468); //      Flags=1  
        public float _29_0x46C_float => ReadOffset<float>(0x46C); //      Flags=1  
        public float _30_0x470_float => ReadOffset<float>(0x470); //      Flags=1  
        public float _31_0x474_float => ReadOffset<float>(0x474); //      Flags=1  
        public float _32_0x478_float => ReadOffset<float>(0x478); //      Flags=1  
        public float _33_0x47C_float => ReadOffset<float>(0x47C); //      Flags=1  
        public float _34_0x480_float => ReadOffset<float>(0x480); //      Flags=1  
        public float _35_0x484_float => ReadOffset<float>(0x484); //      Flags=1  
        public float _36_0x488_float => ReadOffset<float>(0x488); //      Flags=1  
        public float _37_0x48C_float => ReadOffset<float>(0x48C); //      Flags=1  
        public float _38_0x490_float => ReadOffset<float>(0x490); //      Flags=1  
        public float _39_0x494_float => ReadOffset<float>(0x494); //      Flags=1  
        public float _40_0x498_float => ReadOffset<float>(0x498); //      Flags=1  
        public float _41_0x49C_float => ReadOffset<float>(0x49C); //      Flags=1  
        public float _42_0x4A0_float => ReadOffset<float>(0x4A0); //      Flags=1  
        public float _43_0x4A4_float => ReadOffset<float>(0x4A4); //      Flags=1  
        public float _44_0x4A8_float => ReadOffset<float>(0x4A8); //      Flags=1  
        public float _45_0x4AC_float => ReadOffset<float>(0x4AC); //      Flags=1  
        public float _46_0x4B0_float => ReadOffset<float>(0x4B0); //      Flags=1  
        public float _47_0x4B4_float => ReadOffset<float>(0x4B4); //      Flags=1  
        public float _48_0x4B8_float => ReadOffset<float>(0x4B8); //      Flags=1  
        public float _49_0x4BC_float => ReadOffset<float>(0x4BC); //      Flags=1  
        public float _50_0x4C0_float => ReadOffset<float>(0x4C0); //      Flags=1  
        public float _51_0x4C4_float => ReadOffset<float>(0x4C4); //      Flags=1  
        public float _52_0x4C8_float => ReadOffset<float>(0x4C8); //      Flags=1  
        public float _53_0x4CC_float => ReadOffset<float>(0x4CC); //      Flags=1  
        public float _54_0x4D0_float => ReadOffset<float>(0x4D0); //      Flags=1  
        public float _55_0x4D4_float => ReadOffset<float>(0x4D4); //      Flags=1  
        public float _56_0x4D8_float => ReadOffset<float>(0x4D8); //      Flags=1  
        public float _57_0x4DC_float => ReadOffset<float>(0x4DC); //      Flags=1  
        public float _58_0x4E0_float => ReadOffset<float>(0x4E0); //      Flags=1  
        public float _59_0x4E4_float => ReadOffset<float>(0x4E4); //      Flags=1  
        public float _60_0x4E8_float => ReadOffset<float>(0x4E8); //      Flags=1  
        public float _61_0x4EC_float => ReadOffset<float>(0x4EC); //      Flags=1  
        public float _62_0x4F0_float => ReadOffset<float>(0x4F0); //      Flags=1  
        public float _63_0x4F4_float => ReadOffset<float>(0x4F4); //      Flags=1  
        public float _64_0x4F8_float => ReadOffset<float>(0x4F8); //      Flags=1  
        public float _65_0x4FC_float => ReadOffset<float>(0x4FC); //      Flags=1  
        public float _66_0x500_float => ReadOffset<float>(0x500); //      Flags=1  
        public float _67_0x504_float => ReadOffset<float>(0x504); //      Flags=1  
        public float _68_0x508_float => ReadOffset<float>(0x508); //      Flags=1  
        public float _69_0x50C_float => ReadOffset<float>(0x50C); //      Flags=1  
        public float _70_0x510_float => ReadOffset<float>(0x510); //      Flags=1  
        public float _71_0x514_float => ReadOffset<float>(0x514); //      Flags=1  
        public float _72_0x518_float => ReadOffset<float>(0x518); //      Flags=1  
        public float _73_0x51C_float => ReadOffset<float>(0x51C); //      Flags=1  
        public float _74_0x520_float => ReadOffset<float>(0x520); //      Flags=1  
        public float _75_0x524_float => ReadOffset<float>(0x524); //      Flags=1  
        public float _76_0x528_float => ReadOffset<float>(0x528); //      Flags=1  
        public float _77_0x52C_float => ReadOffset<float>(0x52C); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeRecipeEntry : MemoryWrapper
    {
        public const int SizeOf = 336; // 0x150
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_Recipe_Sno => ReadOffset<int>(0x108); //      Flags=1  
        public NpcType _5_0x10C_Enum => ReadOffset<NpcType>(0x10C); //      Flags=17  SymbolTable=@26702568 Min=-1Max=4
        public int _6_0x110_int => ReadOffset<int>(0x110); //      Flags=1  
        public int _7_0x114_int => ReadOffset<int>(0x114); //      Flags=1  
        public int _8_0x118_int => ReadOffset<int>(0x118); //      Flags=1  
        public int _9_0x11C_int => ReadOffset<int>(0x11C); //      Flags=1  
        public List<NativeRecipeIngredient> _10_0x120_FixedArray => ReadObjects<NativeRecipeIngredient>(0x120, 6); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeScriptedAchievementEvent : MemoryWrapper
    {
        public const int SizeOf = 264; // 0x108
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
    }


    [CompilerGenerated]
    public class NativeLootRunQuestTierEntry : MemoryWrapper
    {
        public const int SizeOf = 520; // 0x208
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_Quest_Sno => ReadOffset<int>(0x108); //      Flags=1  
        public float _5_0x10C_float => ReadOffset<float>(0x10C); //      Flags=1  
        public int _6_0x110_AxeBadData_GameBalanceId => ReadOffset<int>(0x110); //      Flags=1  
        public int _7_0x114_Items_GameBalanceId => ReadOffset<int>(0x114); //      Flags=1  
        public int _8_0x118_Quest_Sno => ReadOffset<int>(0x118); //      Flags=1  
        public float _9_0x11C_float => ReadOffset<float>(0x11C); //      Flags=1  
        public int _10_0x120_AxeBadData_GameBalanceId => ReadOffset<int>(0x120); //      Flags=1  
        public int _11_0x124_Items_GameBalanceId => ReadOffset<int>(0x124); //      Flags=1  
        public int _12_0x128_Quest_Sno => ReadOffset<int>(0x128); //      Flags=1  
        public float _13_0x12C_float => ReadOffset<float>(0x12C); //      Flags=1  
        public int _14_0x130_AxeBadData_GameBalanceId => ReadOffset<int>(0x130); //      Flags=1  
        public int _15_0x134_Items_GameBalanceId => ReadOffset<int>(0x134); //      Flags=1  
        public int _16_0x138_Quest_Sno => ReadOffset<int>(0x138); //      Flags=1  
        public float _17_0x13C_float => ReadOffset<float>(0x13C); //      Flags=1  
        public int _18_0x140_AxeBadData_GameBalanceId => ReadOffset<int>(0x140); //      Flags=1  
        public int _19_0x144_Items_GameBalanceId => ReadOffset<int>(0x144); //      Flags=1  
        public int _20_0x148_Quest_Sno => ReadOffset<int>(0x148); //      Flags=1  
        public float _21_0x14C_float => ReadOffset<float>(0x14C); //      Flags=1  
        public int _22_0x150_AxeBadData_GameBalanceId => ReadOffset<int>(0x150); //      Flags=1  
        public int _23_0x154_Items_GameBalanceId => ReadOffset<int>(0x154); //      Flags=1  
        public int _24_0x158_Quest_Sno => ReadOffset<int>(0x158); //      Flags=1  
        public float _25_0x15C_float => ReadOffset<float>(0x15C); //      Flags=1  
        public int _26_0x160_AxeBadData_GameBalanceId => ReadOffset<int>(0x160); //      Flags=1  
        public int _27_0x164_Items_GameBalanceId => ReadOffset<int>(0x164); //      Flags=1  
        public int _28_0x168_Quest_Sno => ReadOffset<int>(0x168); //      Flags=1  
        public float _29_0x16C_float => ReadOffset<float>(0x16C); //      Flags=1  
        public int _30_0x170_AxeBadData_GameBalanceId => ReadOffset<int>(0x170); //      Flags=1  
        public int _31_0x174_Items_GameBalanceId => ReadOffset<int>(0x174); //      Flags=1  
        public int _32_0x178_Quest_Sno => ReadOffset<int>(0x178); //      Flags=1  
        public float _33_0x17C_float => ReadOffset<float>(0x17C); //      Flags=1  
        public int _34_0x180_AxeBadData_GameBalanceId => ReadOffset<int>(0x180); //      Flags=1  
        public int _35_0x184_Items_GameBalanceId => ReadOffset<int>(0x184); //      Flags=1  
        public int _36_0x188_Quest_Sno => ReadOffset<int>(0x188); //      Flags=1  
        public float _37_0x18C_float => ReadOffset<float>(0x18C); //      Flags=1  
        public int _38_0x190_AxeBadData_GameBalanceId => ReadOffset<int>(0x190); //      Flags=1  
        public int _39_0x194_Items_GameBalanceId => ReadOffset<int>(0x194); //      Flags=1  
        public int _40_0x198_Quest_Sno => ReadOffset<int>(0x198); //      Flags=1  
        public float _41_0x19C_float => ReadOffset<float>(0x19C); //      Flags=1  
        public int _42_0x1A0_AxeBadData_GameBalanceId => ReadOffset<int>(0x1A0); //      Flags=1  
        public int _43_0x1A4_Items_GameBalanceId => ReadOffset<int>(0x1A4); //      Flags=1  
        public int _44_0x1A8_Quest_Sno => ReadOffset<int>(0x1A8); //      Flags=1  
        public float _45_0x1AC_float => ReadOffset<float>(0x1AC); //      Flags=1  
        public int _46_0x1B0_AxeBadData_GameBalanceId => ReadOffset<int>(0x1B0); //      Flags=1  
        public int _47_0x1B4_Items_GameBalanceId => ReadOffset<int>(0x1B4); //      Flags=1  
        public int _48_0x1B8_Quest_Sno => ReadOffset<int>(0x1B8); //      Flags=1  
        public float _49_0x1BC_float => ReadOffset<float>(0x1BC); //      Flags=1  
        public int _50_0x1C0_AxeBadData_GameBalanceId => ReadOffset<int>(0x1C0); //      Flags=1  
        public int _51_0x1C4_Items_GameBalanceId => ReadOffset<int>(0x1C4); //      Flags=1  
        public int _52_0x1C8_Quest_Sno => ReadOffset<int>(0x1C8); //      Flags=1  
        public float _53_0x1CC_float => ReadOffset<float>(0x1CC); //      Flags=1  
        public int _54_0x1D0_AxeBadData_GameBalanceId => ReadOffset<int>(0x1D0); //      Flags=1  
        public int _55_0x1D4_Items_GameBalanceId => ReadOffset<int>(0x1D4); //      Flags=1  
        public int _56_0x1D8_Quest_Sno => ReadOffset<int>(0x1D8); //      Flags=1  
        public float _57_0x1DC_float => ReadOffset<float>(0x1DC); //      Flags=1  
        public int _58_0x1E0_AxeBadData_GameBalanceId => ReadOffset<int>(0x1E0); //      Flags=1  
        public int _59_0x1E4_Items_GameBalanceId => ReadOffset<int>(0x1E4); //      Flags=1  
        public int _60_0x1E8_Quest_Sno => ReadOffset<int>(0x1E8); //      Flags=1  
        public float _61_0x1EC_float => ReadOffset<float>(0x1EC); //      Flags=1  
        public int _62_0x1F0_AxeBadData_GameBalanceId => ReadOffset<int>(0x1F0); //      Flags=1  
        public int _63_0x1F4_Items_GameBalanceId => ReadOffset<int>(0x1F4); //      Flags=1  
        public int _64_0x1F8_Quest_Sno => ReadOffset<int>(0x1F8); //      Flags=1  
        public float _65_0x1FC_float => ReadOffset<float>(0x1FC); //      Flags=1  
        public int _66_0x200_AxeBadData_GameBalanceId => ReadOffset<int>(0x200); //      Flags=1  
        public int _67_0x204_Items_GameBalanceId => ReadOffset<int>(0x204); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeParagonBonus : MemoryWrapper
    {
        public const int SizeOf = 640; // 0x280
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_int => ReadOffset<int>(0x108); //      Flags=1  
        public List<NativeAttributeSpecifier> _5_0x110_FixedArray => ReadObjects<NativeAttributeSpecifier>(0x110, 4); //      Flags=1  
        public int _6_0x170_int => ReadOffset<int>(0x170); //      Flags=1  
        public int _7_0x174_int => ReadOffset<int>(0x174); //      Flags=1  
        public ActorClass _8_0x178_Enum => ReadOffset<ActorClass>(0x178); //      Flags=17  SymbolTable=@26705560 Min=-1Max=5
        public string _9_0x17C_String => ReadString(0x17C); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeLegacyItemConversion : MemoryWrapper
    {
        public const int SizeOf = 280; // 0x118
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_Items_GameBalanceId => ReadOffset<int>(0x108); //      Flags=1  
        public int _5_0x10C_Items_GameBalanceId => ReadOffset<int>(0x10C); //      Flags=1  
        public int _6_0x110_int => ReadOffset<int>(0x110); //      Flags=1  
        public int _7_0x114_int => ReadOffset<int>(0x114); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeEnchantItemAffixUseCountCostScalar : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public float _2_0x4_float => ReadOffset<float>(0x4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeTieredLootRunLevel : MemoryWrapper
    {
        public const int SizeOf = 56; // 0x38
        public float _1_0x0_float => ReadOffset<float>(0x0); //      Flags=1  
        public float _2_0x4_float => ReadOffset<float>(0x4); //      Flags=1  
        public float _3_0x8_float => ReadOffset<float>(0x8); //      Flags=1  
        public float _4_0xC_float => ReadOffset<float>(0xC); //      Flags=1  
        public float _5_0x10_float => ReadOffset<float>(0x10); //      Flags=1  
        public float _6_0x14_float => ReadOffset<float>(0x14); //      Flags=1  
        public float _7_0x18_float => ReadOffset<float>(0x18); //      Flags=1  
        public float _8_0x1C_float => ReadOffset<float>(0x1C); //      Flags=1  
        public int _9_0x20_int => ReadOffset<int>(0x20); //      Flags=1  
        public int _10_0x24_int => ReadOffset<int>(0x24); //      Flags=1  
                                                          // Unknown index=11 @Offset0x28 Type=DT_INT64 BaseType=DT_NULL //      Flags=1  
        public float _12_0x30_float => ReadOffset<float>(0x30); //      Flags=1  
        public float _13_0x34_float => ReadOffset<float>(0x34); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeTransmuteRecipe : MemoryWrapper
    {
        public const int SizeOf = 376; // 0x178
        public string _1_0x0_String => ReadString(0x0); //      Flags=18437  
        public int _2_0x100_int => ReadOffset<int>(0x100);
        public int _3_0x104_int => ReadOffset<int>(0x104);
        public int _4_0x108_int => ReadOffset<int>(0x108); //      Flags=1  
        public KanaiRecipeType _5_0x108_Enum => ReadOffset<KanaiRecipeType>(0x108); //      Flags=17  SymbolTable=@26697656 Min=-1Max=12
        public List<NativeTransmuteRecipeIngredient> _6_0x10C_FixedArray => ReadObjects<NativeTransmuteRecipeIngredient>(0x10C, 8); //      Flags=1  
        public int _7_0x16C_int => ReadOffset<int>(0x16C); //      Flags=1  
        public int _8_0x170_int => ReadOffset<int>(0x170); //      Flags=1  
        public int _9_0x174_int => ReadOffset<int>(0x174); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeAttributeSpecifier : MemoryWrapper
    {
        public const int SizeOf = 24; // 0x18
                                // Unknown index=1 @Offset0x0 Type=DT_TRANSLATEABLE BaseType=DT_NULL //      Flags=1  
                                // Unknown index=2 @Offset0x4 Type=DT_ATTRIBUTEPARAM BaseType=DT_NULL //      Flags=1  
                                // Unknown index=3 @Offset0x8 Type=DT_FORMULA BaseType=DT_NULL //    VarArrSerializeOffsetDiff=8    
        public NativeSerializeData _4_0x10_SerializeData => ReadObject<NativeSerializeData>(0x10);
    }


    [CompilerGenerated]
    public class NativeRecipeIngredient : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
        public int _1_0x0_Items_GameBalanceId => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeTransmuteRecipeIngredient : MemoryWrapper
    {
        public const int SizeOf = 12; // 0xC
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
    }


    //[CompilerGenerated]
    //public class NativeActor : MemoryWrapper
    //{
    //    public const int SizeOf = 872; // 0x368
    //    public int _1_0xC_int => ReadOffset<int>(0xC); //      Flags=524289  
    //    public ActorType _2_0x10_Enum => ReadOffset<ActorType>(0x10); //      Flags=17  SymbolTable=@26705656 Max=11
    //    public int _3_0x14_Appearance_Sno => ReadOffset<int>(0x14); //      Flags=257  
    //    public int _4_0x18_PhysMesh_Sno => ReadOffset<int>(0x18); //      Flags=1  
    //    public NativeAxialCylinder _5_0x1C_Object => ReadObject<NativeAxialCylinder>(0x1C); //      Flags=1  
    //    public NativeSphere _6_0x30_Object => ReadObject<NativeSphere>(0x30); //      Flags=1  
    //    public NativeAABB _7_0x40_Object => ReadObject<NativeAABB>(0x40);
    //    public NativeSerializeData _8_0x58_SerializeData => ReadObject<NativeSerializeData>(0x58);
    //    //    public TagMap _9_0x60_TagMap => ReadOffset<TagMap>(0x60); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    //    public int _10_0x68_AnimSet_Sno => ReadOffset<int>(0x68); //      Flags=1  
    //    public int _11_0x6C_Monster_Sno => ReadOffset<int>(0x6C); //      Flags=1  
    //    public NativeSerializeData _12_0x70_SerializeData => ReadObject<NativeSerializeData>(0x70);
    //    public int _13_0x78_NativeMsgTriggeredEvent => ReadOffset<int>(0x78); //    VarArrSerializeOffsetDiff=-8   Flags=65536  
    //    public List<NativeMsgTriggeredEvent> _14_0x80_VariableArray => ReadSerializedObjects<NativeMsgTriggeredEvent>(0x80, 0x70); //    VarArrSerializeOffsetDiff=-16   Flags=33  
    //    public Vector3 _15_0x88_Vector3 => ReadOffset<Vector3>(0x88); //      Flags=1  
    //    public List<NativeWeightedLook> _16_0x94_FixedArray => ReadObjects<NativeWeightedLook>(0x94, 8); //      Flags=1  
    //    public int _17_0x2B4_Physics_Sno => ReadOffset<int>(0x2B4); //      Flags=1  
    //    public int _18_0x2B8_int => ReadOffset<int>(0x2B8); //      Flags=524289  
    //    public int _19_0x2BC_int => ReadOffset<int>(0x2BC); //      Flags=1  
    //    public float _20_0x2C0_float => ReadOffset<float>(0x2C0); //      Flags=1  
    //    public float _21_0x2C4_float => ReadOffset<float>(0x2C4); //      Flags=1  
    //    public float _22_0x2C8_float => ReadOffset<float>(0x2C8); //      Flags=1  
    //    public NativeActorCollisionData _23_0x2CC_Object => ReadObject<NativeActorCollisionData>(0x2CC); //      Flags=1  
    //    public List<NativeInventoryImages> _24_0x310_FixedArray => ReadObjects<NativeInventoryImages>(0x310, 6); //      Flags=1  
    //    public int _25_0x340_int => ReadOffset<int>(0x340); //      Flags=1  
    //    public string _26_0x348_SerializedString => ReadSerializedString(0x348, 0x350); //    VarArrSerializeOffsetDiff=8   Flags=33  
    //    public NativeSerializeData _27_0x350_SerializeData => ReadObject<NativeSerializeData>(0x350);
    //    public string _28_0x358_SerializedString => ReadSerializedString(0x358, 0x360); //    VarArrSerializeOffsetDiff=8   Flags=33  
    //    public NativeSerializeData _29_0x360_SerializeData => ReadObject<NativeSerializeData>(0x360);
    //}


    [CompilerGenerated]
    public class NativeLevelArea : SnoTableEntry
    {
        public const int SizeOf = 40; // 0x28
        public int _1_0xC_int => ReadOffset<int>(0xC); //      Flags=524289  
        public int _2_0x10_LevelArea_Sno => ReadOffset<int>(0x10); //      Flags=1  
        public int _3_0x14_LevelArea_Sno => ReadOffset<int>(0x14); //      Flags=1  
        public List<NativeLevelAreaServerData> _4_0x18_VariableArray => ReadSerializedObjects<NativeLevelAreaServerData>(0x18, 0x20); //    VarArrSerializeOffsetDiff=8   Flags=1057  
        public NativeSerializeData _5_0x20_SerializeData => ReadObject<NativeSerializeData>(0x20);
    }


    [CompilerGenerated]
    public class NativeLevelAreaServerData : MemoryWrapper
    {
        public const int SizeOf = 864; // 0x360
        public int _1_0x0_LevelArea_Sno => ReadOffset<int>(0x0); //      Flags=1  
        public NativeGizmoLocSet _2_0x8_Object => ReadObject<NativeGizmoLocSet>(0x8); //      Flags=1  
        public int _3_0x348_LevelArea_Sno => ReadOffset<int>(0x348); //      Flags=1  
        public int _4_0x34C_NativeLevelAreaSpawnPopulation => ReadOffset<int>(0x34C); //    VarArrSerializeOffsetDiff=12   Flags=65536  
        public List<NativeLevelAreaSpawnPopulation> _5_0x350_VariableArray => ReadSerializedObjects<NativeLevelAreaSpawnPopulation>(0x350, 0x358); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _6_0x358_SerializeData => ReadObject<NativeSerializeData>(0x358);
    }


    [CompilerGenerated]
    public class NativeGizmoLocSet : MemoryWrapper
    {
        public const int SizeOf = 832; // 0x340
        public List<NativeGizmoLocSpawnType> _1_0x0_FixedArray => ReadObjects<NativeGizmoLocSpawnType>(0x0, 52); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeLevelAreaSpawnPopulation : MemoryWrapper
    {
        public const int SizeOf = 152; // 0x98
        public string _1_0x0_String => ReadString(0x0); //      Flags=1  
        public int _2_0x40_int => ReadOffset<int>(0x40); //      Flags=1  
        public float _3_0x44_float => ReadOffset<float>(0x44); //      Flags=1  
        public float _4_0x48_float => ReadOffset<float>(0x48); //      Flags=1  
        public List<int> _5_0x4C_FixedArray => ReadArray<int>(0x4C, 4); //      Flags=1  
        public List<int> _6_0x5C_FixedArray => ReadArray<int>(0x5C, 4); //      Flags=1  
        public int _7_0x6C_NativeLevelAreaSpawnGroup => ReadOffset<int>(0x6C); //    VarArrSerializeOffsetDiff=12   Flags=65536  
        public List<NativeLevelAreaSpawnGroup> _8_0x70_VariableArray => ReadSerializedObjects<NativeLevelAreaSpawnGroup>(0x70, 0x78); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _9_0x78_SerializeData => ReadObject<NativeSerializeData>(0x78);
        //public List<NativeWeather> _10_0x80_VariableArray => ReadSerializedObjects<NativeWeather>(0x80, 0x88); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _11_0x88_SerializeData => ReadObject<NativeSerializeData>(0x88);
        public int _12_0x90_Encounter_Sno => ReadOffset<int>(0x90); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeGizmoLocSpawnType : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public List<NativeGizmoLocSpawnEntry> _1_0x0_VariableArray => ReadSerializedObjects<NativeGizmoLocSpawnEntry>(0x0, 0x8); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _2_0x8_SerializeData => ReadObject<NativeSerializeData>(0x8);
    }


    [CompilerGenerated]
    public class NativeLevelAreaSpawnGroup : MemoryWrapper
    {
        public const int SizeOf = 56; // 0x38
        public DensityType _1_0x0_Enum => ReadOffset<DensityType>(0x0); //      Flags=17  SymbolTable=@26702088 Max=1
        public float _2_0x4_float => ReadOffset<float>(0x4); //      Flags=1  
        public float _3_0x8_float => ReadOffset<float>(0x8); //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public int _5_0x10_NativeLevelAreaSpawnItem => ReadOffset<int>(0x10); //    VarArrSerializeOffsetDiff=16   Flags=65536  
        public List<NativeLevelAreaSpawnItem> _6_0x18_VariableArray => ReadSerializedObjects<NativeLevelAreaSpawnItem>(0x18, 0x20); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _7_0x20_SerializeData => ReadObject<NativeSerializeData>(0x20);
        public int _8_0x28_int => ReadOffset<int>(0x28); //      Flags=1  
        public int _9_0x2C_int => ReadOffset<int>(0x2C); //      Flags=1  
        public int _10_0x30_Condition_Sno => ReadOffset<int>(0x30); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeGizmoLocSpawnEntry : MemoryWrapper
    {
        public const int SizeOf = 40; // 0x28
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
                                                       // Unknown index=5 @Offset0x10 Type=DT_HIGHPRECISIONPERCENT BaseType=DT_NULL //      Flags=1  
        public int _6_0x14_Condition_Sno => ReadOffset<int>(0x14); //      Flags=1  
        public List<NativeGizmoLocSpawnChoice> _7_0x18_VariableArray => ReadSerializedObjects<NativeGizmoLocSpawnChoice>(0x18, 0x20); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _8_0x20_SerializeData => ReadObject<NativeSerializeData>(0x20);
    }


    [CompilerGenerated]
    public class NativeLevelAreaSpawnItem : MemoryWrapper
    {
        public const int SizeOf = 32; // 0x20
        public NativeSNOName _1_0x0_Object => ReadObject<NativeSNOName>(0x0); //      Flags=1  
        public MonsterQuality _2_0x8_Enum => ReadOffset<MonsterQuality>(0x8); //      Flags=17  SymbolTable=@26701376 Max=7
        public int _3_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public int _4_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
        public int _5_0x14_int => ReadOffset<int>(0x14); //      Flags=1  
        public int _6_0x18_int => ReadOffset<int>(0x18); //      Flags=1  
        public float _7_0x1C_float => ReadOffset<float>(0x1C); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeGizmoLocSpawnChoice : MemoryWrapper
    {
        public const int SizeOf = 20; // 0x14
        public NativeSNOName _1_0x0_Object => ReadObject<NativeSNOName>(0x0); //      Flags=1  
        public int _2_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public float _3_0xC_float => ReadOffset<float>(0xC); //      Flags=1  
        public int _4_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativePower : SnoTableEntry
    {
        public const int SizeOf = 1120; // 0x460
        public string _1_0xC_String => ReadString(0xC); //      Flags=2049  
        public NativePowerDef _2_0x50_Object => ReadObject<NativePowerDef>(0x50, BaseAddress); //      Flags=1  
        public int _3_0x328_int => ReadOffset<int>(0x328); //      Flags=17  Max=1
        public int _4_0x32C_int => ReadOffset<int>(0x32C); //      Flags=17  Max=1
        public string _5_0x330_String => ReadString(0x330); //      Flags=2049  
        public NativeSerializeData _6_0x430_SerializeData => ReadObject<NativeSerializeData>(0x430); //      Flags=2048  
        public List<NativeScriptFormulaDetails> _7_0x438_VariableArray => ReadSerializedObjects<NativeScriptFormulaDetails>(0x438, 0x430); //    VarArrSerializeOffsetDiff=-8   Flags=2145  
        public int _8_0x440_NativeScriptFormulaDetails => ReadOffset<int>(0x440); //    VarArrSerializeOffsetDiff=-16   Flags=65536  
        public string _9_0x448_SerializedString => ReadSerializedString(0x448, 0x450); //    VarArrSerializeOffsetDiff=8   Flags=32  
        public NativeSerializeData _10_0x450_SerializeData => ReadObject<NativeSerializeData>(0x450);
        public int _11_0x458_Quest_Sno => ReadOffset<int>(0x458); //      Flags=1  

        public override string ToString() => $"{Header.SnoId}, {(NativePowerSno)Header.SnoId} {_1_0xC_String} {_5_0x330_String}";
    }


    [CompilerGenerated]
    public class NativePowerDef : MemoryWrapper
    {
        public const int SizeOf = 728; // 0x2D8
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);

        //public PowerMap _2_0x18_TagMap => ReadPointer<PowerMap>(0x18);

        public PowerMap _2_0x8_TagMap => ReadPointer<PowerMap>(0x8); //    VarArrSerializeOffsetDiff=-8   Flags=33  
        public NativeSerializeData _3_0x10_SerializeData => ReadObject<NativeSerializeData>(0x10);
        public PowerMap _4_0x18_TagMap => ReadPointer<PowerMap>(0x18); //    VarArrSerializeOffsetDiff=-8   Flags=33  
        public NativeSerializeData _5_0x20_SerializeData => ReadObject<NativeSerializeData>(0x20);
        public PowerMap _6_0x28_TagMap => ReadPointer<PowerMap>(0x28); //    VarArrSerializeOffsetDiff=-8   Flags=33  
        public NativeSerializeData _7_0x30_SerializeData => ReadObject<NativeSerializeData>(0x30);
        public NativeSerializeData _8_0x38_SerializeData => ReadObject<NativeSerializeData>(0x38);
        public NativeSerializeData _9_0x40_SerializeData => ReadObject<NativeSerializeData>(0x40);
        public NativeSerializeData _10_0x48_SerializeData => ReadObject<NativeSerializeData>(0x48);
        public PowerMap _11_0x50_TagMap => ReadPointer<PowerMap>(0x50); //    VarArrSerializeOffsetDiff=-32   Flags=33  
        public PowerMap _12_0x58_TagMap => ReadPointer<PowerMap>(0x58); //    VarArrSerializeOffsetDiff=-32   Flags=33  
        public PowerMap _13_0x60_TagMap => ReadPointer<PowerMap>(0x60); //    VarArrSerializeOffsetDiff=-32   Flags=33  
        public PowerMap _14_0x68_TagMap => ReadPointer<PowerMap>(0x68); //    VarArrSerializeOffsetDiff=-32   Flags=33  
        public NativeSerializeData _15_0x70_SerializeData => ReadObject<NativeSerializeData>(0x70);
        public NativeSerializeData _16_0x78_SerializeData => ReadObject<NativeSerializeData>(0x78);
        public NativeSerializeData _17_0x80_SerializeData => ReadObject<NativeSerializeData>(0x80);
        public NativeSerializeData _18_0x88_SerializeData => ReadObject<NativeSerializeData>(0x88);
        public PowerMap _19_0x90_TagMap => ReadPointer<PowerMap>(0x90); //    VarArrSerializeOffsetDiff=-32   Flags=33  
        public PowerMap _20_0x98_TagMap => ReadPointer<PowerMap>(0x98); //    VarArrSerializeOffsetDiff=-32   Flags=33  
        public PowerMap _21_0xA0_TagMap => ReadPointer<PowerMap>(0xA0); //    VarArrSerializeOffsetDiff=-32   Flags=33  
        public PowerMap _22_0xA8_TagMap => ReadPointer<PowerMap>(0xA8); //    VarArrSerializeOffsetDiff=-32   Flags=33  
        public int _23_0xB0_int => ReadOffset<int>(0xB0); //      Flags=524289  
        public NativeActorCollisionFlags _24_0xB4_Object => ReadObject<NativeActorCollisionFlags>(0xB4); //      Flags=1  
        public NativeActorCollisionFlags _25_0xC4_Object => ReadObject<NativeActorCollisionFlags>(0xC4); //      Flags=1  
        public List<NativeBuffDef> _26_0xD8_FixedArray => ReadObjects<NativeBuffDef>(0xD8, 32, BaseSerializationAddress); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeScriptFormulaDetails : MemoryWrapper
    {
        public const int SizeOf = 776; // 0x308
        public string _1_0x0_String => ReadString(0x0); //      Flags=1  
        public string _2_0x100_String => ReadString(0x100); //      Flags=1  
        public int _3_0x300_int => ReadOffset<int>(0x300); //      Flags=1  
        public int _4_0x304_int => ReadOffset<int>(0x304); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeBuffDef : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeBuffFilterDef> _2_0x8_VariableArray => ReadSerializedObjects<NativeBuffFilterDef>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeBuffFilterDef : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
        public int _1_0x0_Power_Sno => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeTreasureClass : SnoTableEntry
    {
        public const int SizeOf = 48; // 0x30
                                // Unknown index=1 @Offset0xC Type=DT_HIGHPRECISIONPERCENT BaseType=DT_NULL //      Flags=1  
        public int _2_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
        public int _3_0x14_int => ReadOffset<int>(0x14); //      Flags=1  
        public int _4_0x18_NativeLootDropModifier => ReadOffset<int>(0x18); //    VarArrSerializeOffsetDiff=16   Flags=65536  
        public List<NativeLootDropModifier> _5_0x20_VariableArray => ReadSerializedObjects<NativeLootDropModifier>(0x20, 0x28); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _6_0x28_SerializeData => ReadObject<NativeSerializeData>(0x28);
    }


    [CompilerGenerated]
    public class NativeLootDropModifier : MemoryWrapper
    {
        public const int SizeOf = 132; // 0x84
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_TreasureClass_Sno => ReadOffset<int>(0x4); //      Flags=1  
                                                                     // Unknown index=3 @Offset0x8 Type=DT_HIGHPRECISIONPERCENT BaseType=DT_NULL //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public int _5_0x10_QualityClasses_GameBalanceId => ReadOffset<int>(0x10); //      Flags=1  
        public int _6_0x14_int => ReadOffset<int>(0x14); //      Flags=1  
        public int _7_0x18_int => ReadOffset<int>(0x18); //      Flags=1  
        public int _8_0x1C_Condition_Sno => ReadOffset<int>(0x1C); //      Flags=1  
        public NativeItemSpecifierData _9_0x20_Object => ReadObject<NativeItemSpecifierData>(0x20); //      Flags=1  
        public int _10_0x58_int => ReadOffset<int>(0x58); //      Flags=1  
        public int _11_0x5C_int => ReadOffset<int>(0x5C); //      Flags=1  
        public List<int> _12_0x60_FixedArray => ReadArray<int>(0x60, 5);
        public int _13_0x74_int => ReadOffset<int>(0x74); //      Flags=1  
        public int _14_0x78_int => ReadOffset<int>(0x78); //      Flags=1  
        public float _15_0x7C_float => ReadOffset<float>(0x7C); //      Flags=1  
        public int _16_0x80_int => ReadOffset<int>(0x80); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeItemSpecifierData : MemoryWrapper
    {
        public const int SizeOf = 56; // 0x38
        public int _1_0x0_Items_GameBalanceId => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public List<int> _3_0x8_FixedArray => ReadArray<int>(0x8, 6); //      Flags=1  
        public int _4_0x20_int => ReadOffset<int>(0x20); //      Flags=1  
        public int _5_0x24_int => ReadOffset<int>(0x24); //      Flags=1  
        public int _6_0x28_int => ReadOffset<int>(0x28); //      Flags=1  
        public int _7_0x2C_int => ReadOffset<int>(0x2C); //      Flags=1  
        public int _8_0x30_int => ReadOffset<int>(0x30); //      Flags=1  
        public int _9_0x34_int => ReadOffset<int>(0x34); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeAct : SnoTableEntry
    {
        public const int SizeOf = 4192; // 0x1060
        public NativeSerializeData _1_0xC_SerializeData => ReadObject<NativeSerializeData>(0xC);
        public List<NativeActQuestInfo> _2_0x18_VariableArray => ReadSerializedObjects<NativeActQuestInfo>(0x18, 0xC); //    VarArrSerializeOffsetDiff=-12   Flags=33  
        public List<NativeWaypointInfo> _3_0x20_FixedArray => ReadObjects<NativeWaypointInfo>(0x20, 100); //      Flags=1  
        public NativeResolvedPortalDestination _4_0xFC0_Object => ReadObject<NativeResolvedPortalDestination>(0xFC0); //      Flags=1  
        public List<NativeActStartLocOverride> _5_0xFCC_FixedArray => ReadObjects<NativeActStartLocOverride>(0xFCC, 6); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeActQuestInfo : MemoryWrapper
    {
        public const int SizeOf = 4; // 0x4
        public int _1_0x0_Quest_Sno => ReadOffset<int>(0x0); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeWaypointInfo : MemoryWrapper
    {
        public const int SizeOf = 40; // 0x28
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_Worlds_Sno => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_LevelArea_Sno => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public int _5_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
        public int _6_0x14_int => ReadOffset<int>(0x14); //      Flags=1  
        public int _7_0x18_QuestRange_Sno => ReadOffset<int>(0x18); //      Flags=1  
        public int _8_0x1C_int => ReadOffset<int>(0x1C); //      Flags=1  
        public Vector2 _9_0x20_Vector2 => ReadOffset<Vector2>(0x20); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeResolvedPortalDestination : MemoryWrapper
    {
        public const int SizeOf = 12; // 0xC
        public int _1_0x0_Worlds_Sno => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_LevelArea_Sno => ReadOffset<int>(0x8); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeActStartLocOverride : MemoryWrapper
    {
        public const int SizeOf = 24; // 0x18
        public NativeResolvedPortalDestination _1_0x0_Object => ReadObject<NativeResolvedPortalDestination>(0x0); //      Flags=1  
        public int _2_0xC_QuestRange_Sno => ReadOffset<int>(0xC); //      Flags=1  
        public int _3_0x10_Worlds_Sno => ReadOffset<int>(0x10); //      Flags=1  
        public int _4_0x14_int => ReadOffset<int>(0x14); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeAccount : SnoTableEntry
    {
        public const int SizeOf = 32; // 0x20
        public NativeSerializeData _1_0xC_SerializeData => ReadObject<NativeSerializeData>(0xC);
        public string _2_0x18_SerializedString => ReadSerializedString(0x18, 0xC); //    VarArrSerializeOffsetDiff=-12   Flags=32  
    }


    [CompilerGenerated]
    public class NativeScene : SnoTableEntry
    {
        public const int SizeOf = 528; // 0x210
        public int _1_0xC_int => ReadOffset<int>(0xC); //      Flags=524289  
        public NativeAABB _2_0x10_Object => ReadObject<NativeAABB>(0x10);
        public NativeAABB _3_0x28_Object => ReadObject<NativeAABB>(0x28);
        public NativeNavMeshDef _4_0x40_Object => ReadObject<NativeNavMeshDef>(0x40); //      Flags=1  
        public NativeSerializeData _5_0x68_SerializeData => ReadObject<NativeSerializeData>(0x68);
        public List<NativeScene> _6_0x70_VariableArray => ReadSerializedObjects<NativeScene>(0x70, 0x68); //    VarArrSerializeOffsetDiff=-8   Flags=291  
        public NativeSerializeData _7_0xA8_SerializeData => ReadObject<NativeSerializeData>(0xA8);
        public List<NativeScene> _8_0xB0_VariableArray => ReadSerializedObjects<NativeScene>(0xB0, 0xA8); //    VarArrSerializeOffsetDiff=-8   Flags=291  
        public NativeSerializeData _9_0xE8_SerializeData => ReadObject<NativeSerializeData>(0xE8);
        //public List<NativeMarkerSet> _10_0xF0_VariableArray => ReadSerializedObjects<NativeMarkerSet>(0xF0, 0xE8); //    VarArrSerializeOffsetDiff=-8   Flags=291  
        public NativeLookLink _11_0x128_Object => ReadObject<NativeLookLink>(0x128); //      Flags=1  
        public NativeSerializeData _12_0x168_SerializeData => ReadObject<NativeSerializeData>(0x168);
        public int _13_0x170_NativeMsgTriggeredEvent => ReadOffset<int>(0x170); //    VarArrSerializeOffsetDiff=-8   Flags=65536  
        public List<NativeMsgTriggeredEvent> _14_0x178_VariableArray => ReadSerializedObjects<NativeMsgTriggeredEvent>(0x178, 0x168); //    VarArrSerializeOffsetDiff=-16   Flags=33  
        public NativeNavZoneDefinition _15_0x180_Object => ReadObject<NativeNavZoneDefinition>(0x180);
        public int _16_0x208_Appearance_Sno => ReadOffset<int>(0x208); //      Flags=257  
        public int _17_0x20C_PhysMesh_Sno => ReadOffset<int>(0x20C); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeNavMeshDef : MemoryWrapper
    {
        public const int SizeOf = 40; // 0x28
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public float _5_0x10_float => ReadOffset<float>(0x10); //      Flags=2049  
        public NativeSerializeData _6_0x14_SerializeData => ReadObject<NativeSerializeData>(0x14); //      Flags=2048  
        public int _7_0x1C_int => ReadOffset<int>(0x1C); //      Flags=2048  
        public List<NativeNavMeshSquare> _8_0x20_VariableArray => ReadSerializedObjects<NativeNavMeshSquare>(0x20, 0x14); //    VarArrSerializeOffsetDiff=-12   Flags=2081  
    }


    [CompilerGenerated]
    public class NativeNavZoneDefinition : MemoryWrapper
    {
        public const int SizeOf = 136; // 0x88
        public int _1_0x0_NativeNavCell => ReadOffset<int>(0x0); //    VarArrSerializeOffsetDiff=16   Flags=65536  
        public List<NativeNavCell> _2_0x8_VariableArray => ReadSerializedObjects<NativeNavCell>(0x8, 0x10); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _3_0x10_SerializeData => ReadObject<NativeSerializeData>(0x10);
        public int _4_0x18_NativeNavCellLookup => ReadOffset<int>(0x18); //    VarArrSerializeOffsetDiff=16   Flags=65536  
        public List<NativeNavCellLookup> _5_0x20_VariableArray => ReadSerializedObjects<NativeNavCellLookup>(0x20, 0x28); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _6_0x28_SerializeData => ReadObject<NativeSerializeData>(0x28);
        public float _7_0x30_float => ReadOffset<float>(0x30); //      Flags=1  
        public float _8_0x34_float => ReadOffset<float>(0x34); //      Flags=1  
        public int _9_0x38_int => ReadOffset<int>(0x38); //      Flags=1  
        public Vector2 _10_0x3C_Vector2 => ReadOffset<Vector2>(0x3C); //      Flags=1  
        public List<NativeNavGridSquare> _11_0x48_VariableArray => ReadSerializedObjects<NativeNavGridSquare>(0x48, 0x50); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _12_0x50_SerializeData => ReadObject<NativeSerializeData>(0x50);
        public int _13_0x58_NativeNavCellLookup => ReadOffset<int>(0x58); //    VarArrSerializeOffsetDiff=16   Flags=65536  
        public List<NativeNavCellLookup> _14_0x60_VariableArray => ReadSerializedObjects<NativeNavCellLookup>(0x60, 0x68); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _15_0x68_SerializeData => ReadObject<NativeSerializeData>(0x68);
        public int _16_0x70_NativeNavCellBorderData => ReadOffset<int>(0x70); //    VarArrSerializeOffsetDiff=16   Flags=65536  
        public List<NativeNavCellBorderData> _17_0x78_VariableArray => ReadSerializedObjects<NativeNavCellBorderData>(0x78, 0x80); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _18_0x80_SerializeData => ReadObject<NativeSerializeData>(0x80);
    }


    [CompilerGenerated]
    public class NativeNavMeshSquare : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
        public float _1_0x0_float => ReadOffset<float>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeNavCell : MemoryWrapper
    {
        public const int SizeOf = 32; // 0x20
        public Vector3 _1_0x0_Vector3 => ReadOffset<Vector3>(0x0); //      Flags=1  
        public Vector3 _2_0xC_Vector3 => ReadOffset<Vector3>(0xC); //      Flags=1  
                                                                   // Unknown index=3 @Offset0x18 Type=DT_WORD BaseType=DT_NULL //      Flags=1  
                                                                   // Unknown index=4 @Offset0x1A Type=DT_WORD BaseType=DT_NULL //      Flags=1  
        public int _5_0x1C_int => ReadOffset<int>(0x1C); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeNavCellLookup : MemoryWrapper
    {
        public const int SizeOf = 4; // 0x4
                               // Unknown index=1 @Offset0x0 Type=DT_WORD BaseType=DT_NULL //      Flags=1  
                               // Unknown index=2 @Offset0x2 Type=DT_WORD BaseType=DT_NULL //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeNavGridSquare : MemoryWrapper
    {
        public const int SizeOf = 6; // 0x6
                               // Unknown index=1 @Offset0x0 Type=DT_WORD BaseType=DT_NULL //      Flags=1  
                               // Unknown index=2 @Offset0x2 Type=DT_WORD BaseType=DT_NULL //      Flags=1  
                               // Unknown index=3 @Offset0x4 Type=DT_WORD BaseType=DT_NULL //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeNavCellBorderData : MemoryWrapper
    {
        public const int SizeOf = 4; // 0x4
                               // Unknown index=1 @Offset0x0 Type=DT_WORD BaseType=DT_NULL //      Flags=1  
                               // Unknown index=2 @Offset0x2 Type=DT_WORD BaseType=DT_NULL //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeSceneGroup : SnoTableEntry
    {
        public const int SizeOf = 40; // 0x28
        public int _1_0xC_NativeSceneGroupItem => ReadOffset<int>(0xC); //    VarArrSerializeOffsetDiff=4   Flags=65536  
        public NativeSerializeData _2_0x10_SerializeData => ReadObject<NativeSerializeData>(0x10);
        public List<NativeSceneGroupItem> _3_0x18_VariableArray => ReadSerializedObjects<NativeSceneGroupItem>(0x18, 0x10); //    VarArrSerializeOffsetDiff=-8   Flags=33  
        public int _4_0x20_int => ReadOffset<int>(0x20); //      Flags=524289  
    }


    [CompilerGenerated]
    public class NativeSceneGroupItem : MemoryWrapper
    {
        public const int SizeOf = 12; // 0xC
        public int _1_0x0_Scene_Sno => ReadOffset<int>(0x0); //      Flags=257  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_Labels_GameBalanceId => ReadOffset<int>(0x8); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeWorlds : SnoTableEntry
    {
        public const int SizeOf = 184; // 0xB8
        public int _1_0xC_int => ReadOffset<int>(0xC); //      Flags=17  Max=1
        public NativeSerializeData _2_0x10_SerializeData => ReadObject<NativeSerializeData>(0x10);
        public List<NativeWorldServerData> _3_0x18_VariableArray => ReadSerializedObjects<NativeWorldServerData>(0x18, 0x10); //    VarArrSerializeOffsetDiff=-8   Flags=1057  
        public NativeSerializeData _4_0x20_SerializeData => ReadObject<NativeSerializeData>(0x20);
        //public List<NativeMarkerSet> _5_0x28_VariableArray => ReadSerializedObjects<NativeMarkerSet>(0x28, 0x20); //    VarArrSerializeOffsetDiff=-8   Flags=291  
        public NativeEnvironment _6_0x60_Object => ReadObject<NativeEnvironment>(0x60); //      Flags=1  
        public float _7_0xAC_float => ReadOffset<float>(0xAC); //      Flags=1  
        public int _8_0xB0_int => ReadOffset<int>(0xB0); //      Flags=524289  
        public float _9_0xB4_float => ReadOffset<float>(0xB4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeWorldServerData : MemoryWrapper
    {
        public const int SizeOf = 144; // 0x90
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeDRLGParams> _2_0x8_VariableArray => ReadSerializedObjects<NativeDRLGParams>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=33  
        public NativeSerializeData _3_0x10_SerializeData => ReadObject<NativeSerializeData>(0x10);
        public List<NativeSceneParams> _4_0x18_VariableArray => ReadSerializedObjects<NativeSceneParams>(0x18, 0x10); //    VarArrSerializeOffsetDiff=-8   Flags=33  
        public NativeLabelRuleSet _5_0x20_Object => ReadObject<NativeLabelRuleSet>(0x20); //      Flags=1  
        public int _6_0x38_int => ReadOffset<int>(0x38); //      Flags=1  
        public NativeSceneClusterSet _7_0x40_Object => ReadObject<NativeSceneClusterSet>(0x40); //      Flags=1  
        public List<NativePower> _8_0x58_FixedArray => ReadObjects<NativePower>(0x58, 4); //      Flags=1  
        public List<int> _9_0x68_FixedArray => ReadArray<int>(0x68, 3); //      Flags=1  
        public int _10_0x74_Script_Sno => ReadOffset<int>(0x74); //      Flags=1  
        public int _11_0x78_int => ReadOffset<int>(0x78); //      Flags=1  
        //public List<NativeWeather> _12_0x80_VariableArray => ReadSerializedObjects<NativeWeather>(0x80, 0x88); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _13_0x88_SerializeData => ReadObject<NativeSerializeData>(0x88);
    }


    [CompilerGenerated]
    public class NativeEnvironment : MemoryWrapper
    {
        public const int SizeOf = 76; // 0x4C
                                // Unknown index=1 @Offset0x0 Type=DT_RGBACOLOR BaseType=DT_NULL //      Flags=1  
        public NativePostFXParams _2_0x4_Object => ReadObject<NativePostFXParams>(0x4); //      Flags=1  
        public int _3_0x24_Actor_Sno => ReadOffset<int>(0x24); //      Flags=1  
        public int _4_0x28_Music_Sno => ReadOffset<int>(0x28); //      Flags=1  
        public int _5_0x2C_Music_Sno => ReadOffset<int>(0x2C); //      Flags=1  
        public float _6_0x30_float => ReadOffset<float>(0x30); //      Flags=1  
        public float _7_0x34_float => ReadOffset<float>(0x34); //      Flags=1  
        public int _8_0x38_AmbientSound_Sno => ReadOffset<int>(0x38); //      Flags=1  
        public int _9_0x3C_Reverb_Sno => ReadOffset<int>(0x3C); //      Flags=1  
        public int _10_0x40_Weather_Sno => ReadOffset<int>(0x40); //      Flags=1  
        public int _11_0x44_Textures_Sno => ReadOffset<int>(0x44); //      Flags=1  
        public int _12_0x48_Textures_Sno => ReadOffset<int>(0x48); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeDRLGParams : MemoryWrapper
    {
        public const int SizeOf = 120; // 0x78
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public List<NativeTileInfo> _2_0x8_VariableArray => ReadSerializedObjects<NativeTileInfo>(0x8, 0x0); //    VarArrSerializeOffsetDiff=-8   Flags=35  
        public int _3_0x40_NativeDRLGCommand => ReadOffset<int>(0x40); //    VarArrSerializeOffsetDiff=4   Flags=65536  
        public NativeSerializeData _4_0x44_SerializeData => ReadObject<NativeSerializeData>(0x44);
        public List<NativeDRLGCommand> _5_0x50_VariableArray => ReadSerializedObjects<NativeDRLGCommand>(0x50, 0x44); //    VarArrSerializeOffsetDiff=-12   Flags=33  
        public NativeSerializeData _6_0x58_SerializeData => ReadObject<NativeSerializeData>(0x58);
        public string _7_0x60_SerializedString => ReadSerializedString(0x60, 0x58); //    VarArrSerializeOffsetDiff=-8   Flags=33  
        public NativeSerializeData _8_0x68_SerializeData => ReadObject<NativeSerializeData>(0x68);
        //    public TagMap _9_0x70_TagMap => ReadOffset<TagMap>(0x70); //    VarArrSerializeOffsetDiff=-8   Flags=33  
    }


    [CompilerGenerated]
    public class NativeSceneParams : MemoryWrapper
    {
        public const int SizeOf = 24; // 0x18
        public NativeSerializeData _1_0x0_SerializeData => ReadObject<NativeSerializeData>(0x0);
        public int _2_0x8_NativeSceneChunk => ReadOffset<int>(0x8); //    VarArrSerializeOffsetDiff=-8   Flags=65536  
        public List<NativeSceneChunk> _3_0x10_VariableArray => ReadSerializedObjects<NativeSceneChunk>(0x10, 0x0); //    VarArrSerializeOffsetDiff=-16   Flags=33  
    }


    [CompilerGenerated]
    public class NativeLabelRuleSet : MemoryWrapper
    {
        public const int SizeOf = 24; // 0x18
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public List<NativeLabelRule> _2_0x8_VariableArray => ReadSerializedObjects<NativeLabelRule>(0x8, 0x10); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _3_0x10_SerializeData => ReadObject<NativeSerializeData>(0x10);
    }


    [CompilerGenerated]
    public class NativeSceneClusterSet : MemoryWrapper
    {
        public const int SizeOf = 24; // 0x18
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public List<NativeSceneCluster> _2_0x8_VariableArray => ReadSerializedObjects<NativeSceneCluster>(0x8, 0x10); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _3_0x10_SerializeData => ReadObject<NativeSerializeData>(0x10);
    }


    [CompilerGenerated]
    public class NativePostFXParams : MemoryWrapper
    {
        public const int SizeOf = 32; // 0x20
        public List<float> _1_0x0_FixedArray => ReadArray<float>(0x0, 4); //      Flags=1  
        public List<float> _2_0x10_FixedArray => ReadArray<float>(0x10, 4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeTileInfo : MemoryWrapper
    {
        public const int SizeOf = 80; // 0x50
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_Scene_Sno => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public NativeSerializeData _5_0x10_SerializeData => ReadObject<NativeSerializeData>(0x10);
        //    public TagMap _6_0x18_TagMap => ReadOffset<TagMap>(0x18); //    VarArrSerializeOffsetDiff=-8   Flags=33  
        public NativeCustomTileInfo _7_0x20_Object => ReadObject<NativeCustomTileInfo>(0x20); //      Flags=1  
        public int _8_0x48_int => ReadOffset<int>(0x48); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeDRLGCommand : MemoryWrapper
    {
        public const int SizeOf = 152; // 0x98
        public string _1_0x0_String => ReadString(0x0); //      Flags=1  
        public int _2_0x80_int => ReadOffset<int>(0x80); //      Flags=1  
        public NativeSerializeData _3_0x84_SerializeData => ReadObject<NativeSerializeData>(0x84);
        //    public TagMap _4_0x90_TagMap => ReadOffset<TagMap>(0x90); //    VarArrSerializeOffsetDiff=-12   Flags=33  
    }


    [CompilerGenerated]
    public class NativeSceneChunk : MemoryWrapper
    {
        public const int SizeOf = 256; // 0x100
        public NativeSNOName _1_0x0_Object => ReadObject<NativeSNOName>(0x0); //      Flags=1  
        public NativePRTransform _2_0x8_Object => ReadObject<NativePRTransform>(0x8); //      Flags=1  
        public NativeSceneSpecification _3_0x24_Object => ReadObject<NativeSceneSpecification>(0x24); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeLabelRule : MemoryWrapper
    {
        public const int SizeOf = 176; // 0xB0
        public string _1_0x0_String => ReadString(0x0); //      Flags=1  
        public NativeLabelCondition _2_0x80_Object => ReadObject<NativeLabelCondition>(0x80); //      Flags=1  
        public int _3_0x98_int => ReadOffset<int>(0x98); //      Flags=1  
        public int _4_0x9C_int => ReadOffset<int>(0x9C); //      Flags=1  
        public List<NativeLabelEntry> _5_0xA0_VariableArray => ReadSerializedObjects<NativeLabelEntry>(0xA0, 0xA8); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _6_0xA8_SerializeData => ReadObject<NativeSerializeData>(0xA8);
    }


    [CompilerGenerated]
    public class NativeSceneCluster : MemoryWrapper
    {
        public const int SizeOf = 184; // 0xB8
        public string _1_0x0_String => ReadString(0x0); //      Flags=1  
        public int _2_0x80_int => ReadOffset<int>(0x80); //      Flags=1  
        public int _3_0x84_int => ReadOffset<int>(0x84); //      Flags=1  
        public List<NativeSubSceneGroup> _4_0x88_VariableArray => ReadSerializedObjects<NativeSubSceneGroup>(0x88, 0x90); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _5_0x90_SerializeData => ReadObject<NativeSerializeData>(0x90);
        public NativeSubSceneGroup _6_0x98_Object => ReadObject<NativeSubSceneGroup>(0x98); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeCustomTileInfo : MemoryWrapper
    {
        public const int SizeOf = 40; // 0x28
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=17  Max=2
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public Vector2 _4_0xC_Vector2 => ReadOffset<Vector2>(0xC); //      Flags=1  
        public NativeSerializeData _5_0x14_SerializeData => ReadObject<NativeSerializeData>(0x14);
        public List<NativeCustomTileCell> _6_0x20_VariableArray => ReadSerializedObjects<NativeCustomTileCell>(0x20, 0x14); //    VarArrSerializeOffsetDiff=-12   Flags=33  
    }


    [CompilerGenerated]
    public class NativePRTransform : MemoryWrapper
    {
        public const int SizeOf = 28; // 0x1C
        public NativeQuaternion _1_0x0_Object => ReadObject<NativeQuaternion>(0x0); //      Flags=1  
        public Vector3 _2_0x10_Vector3 => ReadOffset<Vector3>(0x10); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeSceneSpecification : MemoryWrapper
    {
        public const int SizeOf = 220; // 0xDC
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public Vector2 _2_0x4_Vector2 => ReadOffset<Vector2>(0x4); //      Flags=1  
        public List<NativeLevelArea> _3_0xC_FixedArray => ReadObjects<NativeLevelArea>(0xC, 4); //      Flags=1  
        public int _4_0x1C_Worlds_Sno => ReadOffset<int>(0x1C); //      Flags=1  
        public int _5_0x20_int => ReadOffset<int>(0x20); //      Flags=1  
        public int _6_0x24_LevelArea_Sno => ReadOffset<int>(0x24); //      Flags=1  
        public int _7_0x28_Worlds_Sno => ReadOffset<int>(0x28); //      Flags=1  
        public int _8_0x2C_int => ReadOffset<int>(0x2C); //      Flags=1  
        public int _9_0x30_LevelArea_Sno => ReadOffset<int>(0x30); //      Flags=513  
        public int _10_0x34_Music_Sno => ReadOffset<int>(0x34); //      Flags=1  
        public int _11_0x38_AmbientSound_Sno => ReadOffset<int>(0x38); //      Flags=1  
        public int _12_0x3C_Reverb_Sno => ReadOffset<int>(0x3C); //      Flags=1  
        public int _13_0x40_Weather_Sno => ReadOffset<int>(0x40); //      Flags=1  
        public int _14_0x44_Worlds_Sno => ReadOffset<int>(0x44); //      Flags=1  
        public int _15_0x48_int => ReadOffset<int>(0x48); //      Flags=1  
        public int _16_0x4C_int => ReadOffset<int>(0x4C); //      Flags=1  
        public int _17_0x50_int => ReadOffset<int>(0x50); //      Flags=1  
        public int _18_0x88_int => ReadOffset<int>(0x88); //      Flags=1  
        public NativeSceneCachedValues _19_0x8C_Object => ReadObject<NativeSceneCachedValues>(0x8C);
    }


    [CompilerGenerated]
    public class NativeLabelCondition : MemoryWrapper
    {
        public const int SizeOf = 24; // 0x18

        //public _1_0x0_Enum => ReadOffset<>(0x0); //      Flags=17  SymbolTable=@26705624 Max=3
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=524289  
        public List<int> _3_0x8_FixedArray => ReadArray<int>(0x8, 4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeLabelEntry : MemoryWrapper
    {
        public const int SizeOf = 20; // 0x14
        public int _1_0x0_Labels_GameBalanceId => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=524289  
        public float _3_0x8_float => ReadOffset<float>(0x8); //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public int _5_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeSubSceneGroup : MemoryWrapper
    {
        public const int SizeOf = 32; // 0x20
        public int _1_0x0_Condition_Sno => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public List<NativeSubSceneEntry> _4_0x10_VariableArray => ReadSerializedObjects<NativeSubSceneEntry>(0x10, 0x18); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _5_0x18_SerializeData => ReadObject<NativeSerializeData>(0x18);
    }


    [CompilerGenerated]
    public class NativeCustomTileCell : MemoryWrapper
    {
        public const int SizeOf = 36; // 0x24
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=17  Max=1
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_Scene_Sno => ReadOffset<int>(0xC); //      Flags=1  
        public int _5_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
        public List<int> _6_0x14_FixedArray => ReadArray<int>(0x14, 4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeQuaternion : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public Vector3 _1_0x0_Vector3 => ReadOffset<Vector3>(0x0); //      Flags=1  
        public float _2_0xC_float => ReadOffset<float>(0xC); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeSceneCachedValues : MemoryWrapper
    {
        public const int SizeOf = 80; // 0x50
        public int _1_0x0_int => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public NativeAABB _4_0xC_Object => ReadObject<NativeAABB>(0xC); //      Flags=1  
        public NativeAABB _5_0x24_Object => ReadObject<NativeAABB>(0x24); //      Flags=1  
        public List<int> _6_0x3C_FixedArray => ReadArray<int>(0x3C, 4); //      Flags=1  
        public int _7_0x4C_int => ReadOffset<int>(0x4C); //      Flags=524289  
    }


    [CompilerGenerated]
    public class NativeSubSceneEntry : MemoryWrapper
    {
        public const int SizeOf = 32; // 0x20
        public int _1_0x0_Scene_Sno => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public List<NativeSubSceneLabel> _4_0x10_VariableArray => ReadSerializedObjects<NativeSubSceneLabel>(0x10, 0x18); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _5_0x18_SerializeData => ReadObject<NativeSerializeData>(0x18);
    }


    [CompilerGenerated]
    public class NativeSubSceneLabel : MemoryWrapper
    {
        public const int SizeOf = 8; // 0x8
        public int _1_0x0_AxeBadData_GameBalanceId => ReadOffset<int>(0x0); //      Flags=1  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeSkillKit : SnoTableEntry
    {
        public const int SizeOf = 64; // 0x40
        public List<NativeTraitEntry> _1_0x10_VariableArray => ReadSerializedObjects<NativeTraitEntry>(0x10, 0x18); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _2_0x18_SerializeData => ReadObject<NativeSerializeData>(0x18);
        public List<NativeActiveSkillEntry> _3_0x20_VariableArray => ReadSerializedObjects<NativeActiveSkillEntry>(0x20, 0x28); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _4_0x28_SerializeData => ReadObject<NativeSerializeData>(0x28);
        public string _5_0x30_SerializedString => ReadSerializedString(0x30, 0x38); //    VarArrSerializeOffsetDiff=8   Flags=33  
        public NativeSerializeData _6_0x38_SerializeData => ReadObject<NativeSerializeData>(0x38);
    }


    [CompilerGenerated]
    public class NativeTraitEntry : MemoryWrapper
    {
        public const int SizeOf = 16; // 0x10
        public int _1_0x0_Power_Sno => ReadOffset<int>(0x0); //      Flags=257  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
    }


    [CompilerGenerated]
    public class NativeActiveSkillEntry : MemoryWrapper
    {
        public const int SizeOf = 64; // 0x40
        public int _1_0x0_Power_Sno => ReadOffset<int>(0x0); //      Flags=257  
        public int _2_0x4_int => ReadOffset<int>(0x4); //      Flags=1  
        public int _3_0x8_int => ReadOffset<int>(0x8); //      Flags=1  
        public int _4_0xC_int => ReadOffset<int>(0xC); //      Flags=1  
        public int _5_0x10_int => ReadOffset<int>(0x10); //      Flags=1  
        public int _6_0x14_int => ReadOffset<int>(0x14); //      Flags=1  
        public int _7_0x18_int => ReadOffset<int>(0x18); //      Flags=1  
        public int _8_0x1C_int => ReadOffset<int>(0x1C); //      Flags=1  
        public int _9_0x20_int => ReadOffset<int>(0x20); //      Flags=1  
        public int _10_0x24_int => ReadOffset<int>(0x24); //      Flags=1  
        public int _11_0x28_int => ReadOffset<int>(0x28); //      Flags=1  
        public List<int> _12_0x2C_FixedArray => ReadArray<int>(0x2C, 5); //      Flags=1  
    }


}
