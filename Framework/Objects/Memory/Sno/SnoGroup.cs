using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Buddy.Auth.Math;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Sno;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public class SnoGroup<T> : MemoryWrapper where T : SnoTableEntry, new()
    {
        public Container<SnoDefinition<T>> Container => ReadPointer<Container<SnoDefinition<T>>>(0x10);
        public Container<Ptr> DefContainer => ReadPointer<Container<Ptr>>(0x14);
        public IEnumerable<T> Entries => Container.Where(i => i.SnoGroupId == (int)SnoType).Select(v => v.Value);
        public SnoType SnoType => ReadOffset<SnoType>(0x3C);
        public int InvalidSnoId => ReadOffset<int>(0x80);
        public int ItemSize => ReadOffset<int>(0x68);
        public int Limit => ReadOffset<int>(0x64);
        public int Flags => ReadOffset<int>(0x18);
        public string Name => ReadString(0x1C, 32);
        public ValueTypeDescriptor DataType => ReadPointer<ValueTypeDescriptor>(0x74);
    }
}
