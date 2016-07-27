using System;
using System.Collections.Generic;
using Trinity.Framework.Objects.Enums;

namespace Trinity.Framework.Objects.Memory.UX
{
    public class UXControl : MemoryWrapper
    {
        public int VTable => ReadOffset<int>(0x000);
        public UXControlFlags Flags => ReadOffset<UXControlFlags>(0x014);
        public bool IsVisible => UXControlFlags.IsVisible == (Flags & UXControlFlags.IsVisible);
        public UXRect Bounds => ReadOffset<UXRect>(0x468);
        public UXControl Previous => ReadPointer<UXControl>(0x018);
        public UXControl Next => ReadPointer<UXControl>(0x01C);
        public UXReference Self => ReadObject<UXReference>(0x020);
        public UXReference Parent => ReadObject<UXReference>(0x228);
        public ControlType Type => ReadOffset<ControlType>(0x430);
        public int Tag => ReadOffset<int>(0x434);
        public override string ToString() => $"[{Type}] {Self.Name}";    
    }

    [Flags]
    public enum UXControlFlags
    {
        None = 1,
        Flag1 = 2,
        Flag2 = 4,
        Flag4 = 8,
        Flag8 = 16,
        IsVisible = 23,
        Flag10 = 32,       
        Flag20 = 64,
        Flag40 = 32768,
        Flag80 = 65536,
        Flag100 = 131072,
        Flag200 = 262144,
        Flag400 = 524288,
        Flag800 = 1048576,
        Flag1000 = 2097152,
        Flag2000 = 4194304,                
    }

}

