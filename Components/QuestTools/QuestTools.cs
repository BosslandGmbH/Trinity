using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Trinity.Framework;
using Trinity.Components.QuestTools.Helpers;
using Trinity.ProfileTags;
using Zeta.Bot;
using Zeta.XmlEngine;
using Module = Trinity.Framework.Objects.Module;

namespace Trinity.Components.QuestTools
{
    public class QuestTools : Module
    {
        public QuestTools()
        {
            ProfileConditions.Initialize();
        }

        public Version Version => PluginVersion;
        public static Version PluginVersion { get; } = new Version(5, 0, 0);

        internal static DateTime LastPluginPulse = DateTime.MinValue;
        public static DateTime GameCount { get; set; }
        public static DateTime LastJoinedGame { get; set; }
        public static DateTime LastProfileReload { get; set; }

        protected override void OnPulse()
        {
            LastPluginPulse = DateTime.UtcNow;
        }

        protected override void OnPluginEnabled()
        {
            Core.Logger.Log("v{0} Enabled", Version);
        }

        protected override void OnPluginDisabled()
        {
            Core.Logger.Log("v{0} Disabled", Version);
        }

        protected override void OnProfileLoaded()
        {
            ProfileUtils.LoadAdditionalGameParams();
            ProfileHistory.Add(ProfileManager.CurrentProfile);
            ProfileUtils.ProcessProfile();
        }

        protected override void OnBotStart()
        {

        }

        protected override void OnGameJoined()
        {
            LastJoinedGame = DateTime.UtcNow;
        }

    }
}
