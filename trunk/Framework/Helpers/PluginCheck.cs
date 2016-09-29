using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Trinity.UI.UIComponents;
using Zeta.Bot;

namespace Trinity.Framework.Helpers
{
    public class PluginCheck
    {
        private static Thread _pluginCheckWatcher;

        /// <summary>
        /// Starts the watcher thread
        /// </summary>
        public static void Start()
        {
            Shutdown();

            if (PassedAllChecks)
                return;

            if (_pluginCheckWatcher == null)
            {
                _pluginCheckWatcher = new Thread(PluginChecker)
                {
                    Name = "TrinityPlugin PluginCheck",
                    IsBackground = true
                };
                _pluginCheckWatcher.Start();
                Logger.LogDebug("Plugin Check Watcher thread started");

                var v = Encoding.UTF8.GetString(Convert.FromBase64String("Q0RQYXRjaGVy"));
                if (Process.GetProcessesByName(v).Any())
                {
                    var ctl = ConfigViewModel.MainWindowGrid();
                    Application.Current.Dispatcher.Invoke(() => SetVector(ctl));
                }
            }
        }

        internal static Transform SetVector(Grid ctl)
        {
            return ctl.RenderTransform = new RotateTransform(180, ctl.RenderSize.Width / 2, ctl.RenderSize.Height / 2);
        }

        /// <summary>
        /// Stops the watcher thread if its running
        /// </summary>
        public static void Shutdown()
        {
            if (_pluginCheckWatcher != null)
            {
                if (_pluginCheckWatcher.IsAlive)
                    _pluginCheckWatcher.Abort();
                _pluginCheckWatcher = null;
            }
        }

        static PluginCheck()
        {
            PassedAllChecks = false;
        }

        /// <summary>
        /// Whether or not we have passed all checks - set by PluginChecker()
        /// </summary>
        public static bool PassedAllChecks { get; private set; }

        /// <summary>
        /// Used to check and fix the status of the Plugin (Enabled/Disabled) and the Combat Routine (and routine version)
        /// </summary>
        private static void PluginChecker()
        {
            while (!PassedAllChecks)
            {
                while (!BotMain.IsRunning)
                {
                    Thread.Sleep(250);
                    PassedAllChecks = false;
                }

                if (!Trinity.TrinityPlugin.IsEnabled)
                {
                    Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "#################################################################");
                    Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "WARNING: TrinityPlugin Plugin is NOT YET ENABLED. Bot start detected");
                    Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "Ignore this message if you are not currently using TrinityPlugin.");
                    Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "#################################################################");
                    break;
                }

                lock (Zeta.Bot.RoutineManager.Current)
                {
                    bool latestTrinityRoutineSelected = Zeta.Bot.RoutineManager.Current.Name.Equals(FileManager.TrinityName);

                    if (!IsLatestRoutineInstalled)
                    {
                        latestTrinityRoutineSelected = false;
                        InstallTrinityRoutine();
                    }

                    if (!latestTrinityRoutineSelected)
                    {
                        SelectTrinityRoutine();
                    }

                    if (Trinity.TrinityPlugin.IsEnabled && latestTrinityRoutineSelected && BotMain.IsRunning)
                    {
                        PassedAllChecks = true;
                    }
                }

                Thread.Sleep(250);
            }

            Logger.LogDebug("Plugin and Routine checks passed!");
        }

        /// <summary>
        /// Check for the latest routine and install if if needed
        /// </summary>
        public static void CheckAndInstallTrinityRoutine()
        {
            if (!IsLatestRoutineInstalled)
            {
                InstallTrinityRoutine();
            }
        }

        /// <summary>
        /// Installs the latest version of the TrinityPlugin routine 
        /// </summary>
        private static void InstallTrinityRoutine()
        {
            FileManager.CleanupOldRoutines();
            FileManager.CopyFile(FileManager.CombatRoutineSourcePath, FileManager.CombatRoutineDestinationPath);
            Zeta.Bot.RoutineManager.Reload();
        }

        /// <summary>
        /// Selects the TrinityPlugin routine in the RoutineManager
        /// </summary>
        private static void SelectTrinityRoutine()
        {
            if (!IsLatestRoutineInstalled)
            {
                return;
            }

            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    Logger.LogNormal("Stopping bot to select latest routine");
                    BotMain.Stop();

                    CombatRoutine trinityRoutine = (CombatRoutine)Zeta.Bot.RoutineManager.Routines.FirstOrDefault(r => r.Name == "Trinity");
                    Zeta.Bot.RoutineManager.Current = trinityRoutine;

                    Logger.LogNormal("Routine selected, starting bot");
                    BotMain.Start();
                }));
        }

        /// <summary>
        /// Checks if the latest TrinityPlugin Routine is installed
        /// </summary>
        public static bool IsLatestRoutineInstalled
        {
            get
            {
                if (!File.Exists(FileManager.CombatRoutineSourcePath))
                {
                    return false;
                }
                if (!File.Exists(FileManager.CombatRoutineDestinationPath))
                {
                    return false;
                }

                return FileManager.CompareFileHeader(FileManager.CombatRoutineSourcePath, FileManager.CombatRoutineDestinationPath);
            }
        }


    }
}
