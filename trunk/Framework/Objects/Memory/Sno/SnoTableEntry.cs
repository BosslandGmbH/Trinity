using System.Collections.Generic;
using Trinity.Framework.Helpers;
using Trinity.Framework.Helpers.Exporter;

namespace Trinity.Framework.Objects.Memory.Sno
{
    public class SnoTableEntry : MemoryWrapper
    {
        public SnoHeader Header => ReadObject<SnoHeader>(0x00);

    }
}
