using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags.EmbedTags
{
    [XmlElement("Item")]
    public class ItemTag
    {
        [XmlAttribute("id")]
        [DefaultValue(0)]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("quantity")]
        [DefaultValue(1)]
        public int Quantity { get; set; }
    }
}
