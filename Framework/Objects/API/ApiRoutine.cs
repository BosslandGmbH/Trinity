using System.Runtime.Serialization;

namespace Trinity.Framework.Objects.Api
{
    [DataContract]
    public class ApiRoutine
    {
        [DataMember(Name = "internalName")]
        public string InternalName { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }
    }

}



