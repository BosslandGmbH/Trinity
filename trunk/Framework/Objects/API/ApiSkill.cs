using System.Runtime.Serialization;

namespace Trinity.Framework.Objects.Api
{
    [DataContract]
    public class ApiSkill
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

