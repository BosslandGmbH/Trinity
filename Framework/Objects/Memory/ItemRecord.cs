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

        [FieldOffset(0x14c)]
        public int ItemLevel;

        [FieldOffset(0x134)]
        public int StackSize;

        [FieldOffset(0x434)]
        public GemType GemType;
    }
}