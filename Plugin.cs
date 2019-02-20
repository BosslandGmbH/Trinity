using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.UI;
using Trinity.UI.Visualizer;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Application = System.Windows.Application;

namespace Trinity
{
    public class Plugin : IPlugin
    {
        private static readonly ILogger s_logger = Logger.GetLoggerInstanceForType();
        private static Plugin _instance;
        public static Plugin Instance => _instance ?? (_instance = new Plugin());
        public static bool IsEnabled { get; private set; }

        public string Name { get; } = typeof(Plugin).Assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
        public Version Version { get; } = new Version(typeof(Plugin).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
        public string Author => "xzjv, TarasBulba, rrrix, jubisman, Phelon and many more";
        public string Description { get; } = typeof(Plugin).Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
        public Window DisplayWindow => UILoader.GetDisplayWindow(Path.Combine(FileManager.PluginPath, "UI"));
        public bool Equals(IPlugin other) => other?.Name == Name && other?.Version == Version;
        
        public bool IsInitialized { get; private set; }

        public Plugin()
        {
            _instance = this;
            Logger.Enricher.AddOrUpdate(Name, () => new ScalarValue(Version));
            UILoader.Preload();
            PluginManager.OnPluginsReloaded += PluginManager_OnPluginsReloaded;
            InstallRoutine();
        }

        /// <summary>
        /// Copy db provider routine to Db\Routines\. This is required for hooks and YAR Relogger.
        /// The routine does not actually doing anything except route a settings button click.
        /// </summary>
        private static void InstallRoutine()
        {
            var routineDirectory = Path.GetDirectoryName(FileManager.CombatRoutineDestinationPath);
            if (routineDirectory != null && !Directory.Exists(routineDirectory))
                Directory.CreateDirectory(routineDirectory);

            if (File.Exists(FileManager.CombatRoutineSourcePath))
                File.Copy(FileManager.CombatRoutineSourcePath, FileManager.CombatRoutineDestinationPath, true);
        }

        /// <summary>
        /// Makes sure trinity plugin is enabled.
        /// </summary>
        private void PluginManager_OnPluginsReloaded(object sender, EventArgs e)
        {
            foreach (var plugin in PluginManager.Plugins)
            {
                if (plugin.Plugin == this && !plugin.Enabled)
                {
                    plugin.Enabled = true;
                }
            }
        }

        public void OnInitialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
        }

        private void OnStart(IBot bot)
        {
            HookManager.CheckHooks();
        }

        public void OnPulse()
        {
            if (ZetaDia.CurrentQuest == null || ZetaDia.CurrentQuest.QuestSnoId == SNOQuest.Invalid)
            {
                s_logger.Verbose("Waiting while Quest is invalid (-1)");
                BotMain.PauseFor(TimeSpan.FromSeconds(1));
            }

            HookManager.CheckHooks();
            GameUI.SafeClickUIButtons();
            VisualizerViewModel.Instance.UpdateVisualizer();
        }

        public void OnEnabled()
        {
            using (ZetaDia.Memory.AcquireFrame())
            {
                if (IsEnabled || !Application.Current.CheckAccess())
                    return;

                Core.Init();
                BotMain.OnStart += OnStart;
                BotMain.OnStop += OnStop;
                TrinitySettings.InitializeSettings();
                SkillUtils.UpdateActiveSkills();
                HookManager.CheckHooks();
                TabUi.InstallTab();
                SetupDemonBuddy();
                UILoader.PreLoadWindowContent();
                ModuleManager.Enable();
                s_logger.Information($@"{Name} v{Version} is now ENABLED.
{Description}");
                IsEnabled = true;
            }
        }

        /// <summary>
        /// Install empty providers to Demonbuddy otherwise it will try to use its 
        /// in-built default ones and interfere with Trinity operations.
        /// </summary>
        private static void SetupDemonBuddy()
        {
            Navigator.PlayerMover = Core.PlayerMover;
            Navigator.StuckHandler = Core.StuckHandler;
            ItemManager.Current = new BlankItemManager();
            CombatTargeting.Instance.Provider = new TrinityCombatProvider();
            LootTargeting.Instance.Provider = new BlankLootProvider();
            ObstacleTargeting.Instance.Provider = new BlankObstacleProvider();
            GlobalSettings.Instance.LogoutInactivityTime = 0;
        }

        public void OnDisabled()
        {
            IsEnabled = false;
            BotMain.OnStart -= OnStart;
            BotMain.OnStop -= OnStop;
            TabUi.RemoveTab();
            HookManager.CheckHooks();
            Navigator.PlayerMover = new DefaultPlayerMover();
            Navigator.StuckHandler = new DefaultStuckHandler();
            CombatTargeting.Instance.Provider = new DefaultCombatTargetingProvider();
            LootTargeting.Instance.Provider = new DefaultLootTargetingProvider();
            ObstacleTargeting.Instance.Provider = new DefaultObstacleTargetingProvider();
            ItemManager.Current = new BlankItemManager();
            Zeta.Bot.RoutineManager.Current = null;
            ModuleManager.Disable();
            s_logger.Information($@"{Name} v{Version} is now DISABLED.");
        }

        private static void OnStop(IBot bot)
        {
            BotMain.SetCurrentStatusTextProvider(null);
        }

        public void OnShutdown()
        {
        }
    }
}
