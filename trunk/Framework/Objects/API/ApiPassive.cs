using System.Runtime.Serialization;

namespace Trinity.Framework.Objects.Api
{
    [DataContract]
    public class ApiPassive
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "iconSlug")]
        public string IconSlug { get; set; }

        public override string ToString()
            => $"{GetType().Name}: {Name} ({Id})";
    }
}
