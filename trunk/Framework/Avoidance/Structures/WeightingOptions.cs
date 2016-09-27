using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Framework.Avoidance.Structures
{
    [Flags]
    public enum WeightingOptions
    {
        [EnumMember] None = 0,
        [EnumMember] Monsters = 1 << 0,
        [EnumMember] Globes = 1 << 1,
        [EnumMember] Obstacles = 1 << 2,
        [EnumMember] Backtrack = 1 << 3,
        [EnumMember] AdjacentSafe = 1 << 4,
        [EnumMember] AvoidanceCentroid = 1 << 5,
        [EnumMember] MonsterCentroid = 1 << 6,
        [EnumMember] Kiting = 1 << 7,
        [EnumMember] All = ~0,
    }
}
