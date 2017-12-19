using System;
using Trinity.Framework;
using System.IO;
using System.Reflection;

namespace Trinity.Components.Adventurer.Util
{
    public static class FileUtils
    {
        private static string _demonBuddyPath;

        public static string DemonBuddyPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_demonBuddyPath))
                    _demonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                return _demonBuddyPath;
            }
        }

        private static string _pluginPath;

        public static string PluginPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_pluginPath))
                    _pluginPath = Path.Combine(DemonBuddyPath, "Plugins", "Trinity");
                return _pluginPath;
            }
        }

        private static string _pluginPath2;

        public static string PluginPath2
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_pluginPath2))
                    _pluginPath2 = Path.Combine(DemonBuddyPath, "Plugins", "Plugins", "Trinity");
                return _pluginPath2;
            }
        }

        private static string _logPath;

        public static string LogPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_logPath))
                    _logPath = Path.Combine(DemonBuddyPath, "AdventurerLogs");
                return _logPath;
            }
        }

        public static string SettingsPath
        {
            get
            {
                return Path.Combine(DemonBuddyPath, "Settings", AdvDia.BattleNetBattleTagName, "Adventurer", AdvDia.BattleNetHeroId.ToString(), "Config.json");
            }
        }

        public static void WriteToTextFile(string path, string value)
        {
            Directory.CreateDirectory(Path.Combine(DemonBuddyPath, "Settings", AdvDia.BattleNetBattleTagName, "Adventurer", AdvDia.BattleNetHeroId.ToString()));
            File.WriteAllText(path, value);
        }

        public static void AppendToLogFile(string name, string value)
        {
            Directory.CreateDirectory(LogPath);
            var path = Path.Combine(LogPath, name + ".log");
            File.AppendAllLines(path, new[] { value });
        }

        public static string ReadFromTextFile(string path)
        {
            try
            {
                return File.Exists(path) ? File.ReadAllText(path) : null;
            }
            catch (Exception ex)
            {
                Core.Logger.Debug("Exception in ReadFromTextFile");
                return null;
            }
        }
    }
}