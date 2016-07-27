using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Trinity.Framework.Objects.API
{
    [DataContract]
    public class Build
    {
        [DataMember(Name = "equippedItems")]
        public IEnumerable<Item> EquippedItems { get; set; }

        [DataMember(Name = "cubedItems")]
        public IEnumerable<Item> CubedItems { get; set; }

        [DataMember(Name = "socketedItems")]
        public IEnumerable<Item> SocketedItems { get; set; }

        [DataMember(Name = "skills")]
        public IEnumerable<Skill> Skills { get; set; }

        [DataMember(Name = "passives")]
        public IEnumerable<Passive> Passives { get; set; }

        [DataMember(Name = "sets")]
        public IEnumerable<Set> Sets { get; set; }
    }

}



