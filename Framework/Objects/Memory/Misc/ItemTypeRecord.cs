//using System;
//using System.Runtime.InteropServices;
//using Zeta.Game;

//namespace Trinity.Framework.Objects.Memory.Items
//{
//    [StructLayout(LayoutKind.Sequential, Pack = 1)]
//    public struct ItemTypeRecord
//    {
//        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
//        public byte[] InternalName;
//        public int x100;
//        public int x104;
//        public int x108_ItemTypesGameBalanceId;
//        public int x10C;
//        public int x110;
//        public int x114;
//        public UnknownEnumx118 UnknownEnumx118;
//        public InventorySlot InventorySlotPrimary;
//        public InventorySlot InventorySlotSecondary;
//        public InventorySlot InventorySlotHirelingPrimary;
//        public InventorySlot InventorySlotHirelingSecondary;
//        public int x12C_AffixListGameBalanceId;
//        public int x130_AffixListGameBalanceId;
//        public int x134_AffixListGameBalanceId;
//        public int x138_AffixGroupGameBalanceId;
//        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
//        public int[] x13C_ints;
//    }

//    [Flags]
//    public enum UnknownEnumx118
//    {
//        None = 0x00000000,
//        One = 0x00000001,
//        Two = 0x00000002,
//        Three = 0x00000004,
//        Four = 0x00000008,
//        Five = 0x00000010,
//        Six = 0x00000020,
//        Seven = 0x00000040,
//        Eight = 0x00000080,
//        Nine = 0x00000100,
//        Eleven = 0x00000200,
//        Twelve = 0x00000400,
//        Thirteen = 0x00000800,
//        Fourteen = 0x00001000,
//        Fivteen = 0x00002000,
//        Sixteen = 0x00004000,
//        Seventeen = 0x00008000,
//        Eighteen = 0x00010000,
//        Nineteen = 0x00020000,
//        Twenty = 0x00040000,
//        TwentyOne = 0x00080000,
//        TwentyTwo = 0x00100000,
//        TwentyThree = 0x00200000,
//    }
//}
