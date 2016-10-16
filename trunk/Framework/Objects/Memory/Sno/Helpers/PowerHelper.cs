using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Helpers;
using Trinity.Framework.Modules;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Sno.Types;

namespace Trinity.Framework.Objects.Memory.Sno.Helpers
{
    public class PowerHelper
    {
        public List<NativePower> GetPowers()
        {
            return SnoManager.Groups.Power.Container.Where(p => p.SnoGroupId == (int)SnoType.Power).Select(p => p.Value).ToList();           
        }

        public Dictionary<TagType, PowerMap.PowerMapItem> GetTagsForPower(NativePower power)
        {
            var result = new Dictionary<TagType, PowerMap.PowerMapItem>();
            var powerDefinition = power._2_0x50_Object;

            if (powerDefinition._2_0x8_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._2_0x8_TagMap.Entries);

            if (powerDefinition._4_0x18_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._4_0x18_TagMap.Entries);

            if (powerDefinition._6_0x28_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._6_0x28_TagMap.Entries);

            if (powerDefinition._11_0x50_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._11_0x50_TagMap.Entries);

            if (powerDefinition._12_0x58_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._12_0x58_TagMap.Entries);

            if (powerDefinition._13_0x60_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._13_0x60_TagMap.Entries);

            if (powerDefinition._14_0x68_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._14_0x68_TagMap.Entries);

            if (powerDefinition._19_0x90_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._19_0x90_TagMap.Entries);

            if (powerDefinition._20_0x98_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._20_0x98_TagMap.Entries);

            if (powerDefinition._21_0xA0_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._21_0xA0_TagMap.Entries);

            if (powerDefinition._22_0xA8_TagMap.x0C_EntryCount > 0)
                result.AddRangeOverride(powerDefinition._22_0xA8_TagMap.Entries);

            return result;
        }

    }
}
