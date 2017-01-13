using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Objects;
using Trinity.ProfileTags.EmbedTags;
using Zeta.Bot;
using Zeta.Game;
using Zeta.XmlEngine;

namespace Trinity.Modules
{
    public class ProfileSettings : Module
    {
        private readonly XmlEngine _xmlEngine = new XmlEngine();

        protected override void OnGameJoined()
        {
            var trinityElement = ProfileManager.CurrentProfile.Element.Element("Trinity");
            var options = new TrinityOptions();
            _xmlEngine.Load(options, trinityElement);
            Options = options;
        }

        public TrinityOptions Options { get; private set; } = new TrinityOptions();

        [XmlElement("Trinity")]
        public class TrinityOptions
        {
            [XmlElement("Scenes")]
            public List<SceneOptions> Scenes { get; set; }

            public SceneOptions DefaultSceneOptions { get; } = new SceneOptions();

            public SceneOptions CurrentSceneOptions => GetSceneOptions(ZetaDia.Me.CurrentScene.SceneInfo.SNOId);

            public SceneOptions GetSceneOptions(int sceneSnoId)
            {
                return Scenes?.FirstOrDefault(s => s.SnoId == sceneSnoId) ?? DefaultSceneOptions;
            }

            public SceneOptions GetSceneOptions(string sceneName)
            {
                return Scenes?.FirstOrDefault(s => string.Equals(s.Name, sceneName, StringComparison.InvariantCultureIgnoreCase)) ?? DefaultSceneOptions;
            }
        }

        [XmlElement("Scene")]
        public class SceneOptions
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlAttribute("id")]
            public int SnoId { get; set; }

            [XmlAttribute("alwaysRayWalk")]
            public bool AlwaysRayWalk { get; set; }

        }

    }

}

