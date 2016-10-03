using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Trinity.Framework.Objects.Api
{
    [DataContract]
    public class ApiBuild
    {
        [DataMember(Name = "equippedItems")]
        public IEnumerable<ApiItem> EquippedItems { get; set; }

        [DataMember(Name = "cubedItems")]
        public IEnumerable<ApiItem> CubedItems { get; set; }

        [DataMember(Name = "socketedItems")]
        public IEnumerable<ApiItem> SocketedItems { get; set; }

        [DataMember(Name = "skills")]
        public IEnumerable<ApiSkill> Skills { get; set; }

        [DataMember(Name = "passives")]
        public IEnumerable<ApiPassive> Passives { get; set; }

        [DataMember(Name = "sets")]
        public IEnumerable<ApiSet> Sets { get; set; }
    }

}




