using System.Runtime.Serialization;
using Zeta.Game;

namespace Trinity.Framework.Objects.API
{
    [DataContract]    
    public class Item
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
    }
}

