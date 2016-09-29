using System.IO;
using System.Windows;
using Trinity.Framework.Helpers;
using Trinity.UI;

namespace Trinity.Settings
{
    public static class TrinityPluginSettings
    {
        public static TrinitySetting Settings;

        private static void SaveConfiguration()
        {
            Settings.Save();
        }

        public static void InitializeSettings()
        {
            Logger.Log("Initializing TrinitySettings");
            Settings = new TrinitySetting();
            Settings.Load();
        }

    }

}