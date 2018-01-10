using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Zeta.Bot;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("LoadLastProfile")]
    public class LoadLastProfileTag : BaseProfileBehavior
    {
        #region XmlAttributes

        [XmlAttribute("fallbackFile")]
        [Description("Path to a profile if last profile cannot be found")]
        public string FallbackFile { get; set; }

        #endregion

        public override async Task<bool> StartTask()
        {
            var lastProfile = ProfileHistory.LastProfile;
            var currentProfileDirectory = Path.GetDirectoryName(ProfileManager.CurrentProfile.Path);

            if (string.IsNullOrEmpty(currentProfileDirectory))
                currentProfileDirectory = string.Empty;

            var fallbackProfilePath = string.Empty;
            if (!string.IsNullOrEmpty(FallbackFile))
                fallbackProfilePath = Path.Combine(currentProfileDirectory, FallbackFile);

            if (lastProfile != null && File.Exists(lastProfile.Path))
            {
                Core.Logger.Debug("Loading last profile: {0}", lastProfile.Name);
                ProfileManager.Load(lastProfile.Path);
            }
            else if (File.Exists(fallbackProfilePath))
            {
                Core.Logger.Debug("Loading fallback profile: {0}", FallbackFile);
                ProfileManager.Load(fallbackProfilePath);
            }
            else
            {
                Core.Logger.Log("Failed to load profile! file doesnt exist");
            }
            return true;
        }

    }
}

