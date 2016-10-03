using System.Runtime.Serialization;
using Zeta.Game;

namespace Trinity.Framework.Objects.Api
{
    [DataContract]    
    public class ApiItem
    {
        [DataMember(Name = "slot")]
        public InventorySlot Slot { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "isAncient")]
        public bool IsAncient { get; set; }

        [DataMember(Name = "iconSlug")]
        public string IconSlug { get; set; }

        public override string ToString()
            => $"{GetType().Name}: {Name} ({Id}) {Slot}";
    }
}

