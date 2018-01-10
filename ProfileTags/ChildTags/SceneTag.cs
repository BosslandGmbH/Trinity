using System.ComponentModel;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags.EmbedTags
{
    [XmlElement("Scene")]
    public class SceneTag
    {
        [XmlAttribute("sceneId")]
        [XmlAttribute("id")]
        [DefaultValue(0)]
        public int Id { get; set; }

        [XmlAttribute("sceneName")]
        [XmlAttribute("internalName")]
        [XmlAttribute("name")]
        public string Name { get; set; }

        public override string ToString() => $"{GetType().Name}: Id={Id} Name={Name}";
    }
}

