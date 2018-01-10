using System.ComponentModel;
using Trinity.Settings;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags.EmbedTags
{
    [XmlElement("Item")]
    public class ItemTag
    {
        [XmlAttribute("actorId")]
        [XmlAttribute("id")]
        [DefaultValue(0)]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("quality")]
        [DefaultValue(TrinityItemQuality.Invalid)]
        public TrinityItemQuality Quality { get; set; }

        [XmlAttribute("quantity")]
        [DefaultValue(1)]
        public int Quantity { get; set; }

        public override string ToString() => $"{GetType().Name}: Id={Id} Name={Name} Quality={Quality}";
    }
}
