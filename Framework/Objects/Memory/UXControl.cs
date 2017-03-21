using System;
using Trinity.Framework.Objects.Enums;
using Zeta.Game.Internals;

namespace Trinity.Framework.Objects.Memory
{
    public class UXHeader : MemoryWrapper
    {
        public const int SizeOf = 0x10;
        public int EntryId => ReadOffset<int>(0x00);
        public IntPtr ParentHeaderPtr => ReadOffset<IntPtr>(0x08);
        public int Size => ReadOffset<int>(0x0C);
    }

    public class UXControl : MemoryWrapper
    {
        public int VTable => ReadOffset<int>(0x000);
        public UXControlFlags Flags => ReadOffset<UXControlFlags>(0x014);
        public ulong Hash => ReadOffset<ulong>(0x20);
        public string Name => ReadString(0x28, 0x200);
        public UXHeader Header => ReadObject<UXHeader>(-0x10);
        public bool IsVisible => Flags.HasFlag(UXControlFlags.VisibleA | UXControlFlags.VisibleB);
        public bool IsEnabled => ReadOffset<int>(0xB5C) == 1;
        public bool IsMouseOver => ReadOffset<int>(0xBF4) == 1;
        public UXRect Bounds => ReadOffset<UXRect>(0x468);
        public UXControl Previous => ReadPointer<UXControl>(0x018);
        public UXControl Next => ReadPointer<UXControl>(0x01C);
        public UXReference Self => ReadObject<UXReference>(0x020);
        public UXReference Parent => ReadObject<UXReference>(0x228);
        public ControlType Type => ReadOffset<ControlType>(0x430);
        public int Tag => ReadOffset<int>(0x434);
        public LightVector<UXReference> Children => ReadObject<LightVector<UXReference>>(0x450);
        public void Click() => UIElement.FromHash(Hash).Click();
        public override string ToString() => $"[{Type}] {Name}";
    }

    [Flags]
    public enum UXControlFlags : uint
    {
        None = 0x00000000,
        X00000001 = 0x00000001,
        X00000002 = 0x00000002,
        VisibleA = 0x00000004,
        X00000008 = 0x00000008,
        VisibleB = 0x00000010,
        X00000020 = 0x00000020,
        X00000040 = 0x00000040,
        X00000080 = 0x00000080,
        X00000100 = 0x00000100,
        X00000200 = 0x00000200,
        X00000400 = 0x00000400,
        X00000800 = 0x00000800,
        X00001000 = 0x00001000,
        X00002000 = 0x00002000,
        X00004000 = 0x00004000,
        X00008000 = 0x00008000,
        X00010000 = 0x00010000,
        X00020000 = 0x00020000,
        X00040000 = 0x00040000,
        X00080000 = 0x00080000,
        X00100000 = 0x00100000,
        X00200000 = 0x00200000,
        X00400000 = 0x00400000,
        X00800000 = 0x00800000,
        X01000000 = 0x01000000,
        X02000000 = 0x02000000,
        X04000000 = 0x04000000,
        X08000000 = 0x08000000,
        X10000000 = 0x10000000,
        X20000000 = 0x20000000,
        X40000000 = 0x40000000,
        X80000000 = 0x80000000,
    };

}
