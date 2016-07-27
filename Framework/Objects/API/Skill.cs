using System.Runtime.Serialization;

namespace Trinity.Framework.Objects.API
{
    [DataContract]
    public class Skill
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "runeIndex")]
        public int RuneIndex { get; set; }

        [DataMember(Name = "iconSlug")]
        public string IconSlug { get; set; }

        [DataMember(Name = "runeName")]
        public string RuneName { get; set; }
    }
}

