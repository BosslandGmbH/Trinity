using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Trinity.Framework.Objects.API
{
    [DataContract]
    public class Set
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "equippedCount")]
        public int EquippedCount { get; set; }

        [DataMember(Name = "maxCount")]
        public int MaxCount { get; set; }
    }
}
