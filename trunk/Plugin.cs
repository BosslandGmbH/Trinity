using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Trinity.DbProvider;
using Trinity.Framework.Reference;
using Trinity.ProfileTags;
using Trinity.Settings;
using Trinity.UI;
using Trinity.UI.Visualizer;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Common.Plugins;
using Zeta.Game;
using Application = System.Windows.Application;

namespace Trinity
{
    public class TrinityPlugin : IPlugin
    {
        private static TrinityPlugin _instance;
        public string Name => "Trinity";
        public Version Version => new Version(2, 250, 750);
        public string Author => "xzjv, TarasBulba, rrrix, jubisman, Phelon and many more";
        public string Description => $"v{Version} provides combat, exploration and much more";
        public Window DisplayWindow => UILoader.GetDisplayWindow(Path.Combine(FileManager.PluginPath, "UI"));
        public bool Equals(IPlugin other) => other?.Name == Name && other?.Version == Version;
        public static TrinityPlugin Instance => _instance ?? (_instance = new TrinityPlugin());
        public static bool IsEnabled { get; private set; }
        public bool IsInitialized { get; private set; }


        public TrinityPlugin()
        {
            _instance = this;
            UILoader.Preload();            
            PluginManager.OnPluginsReloaded += PluginManager_OnPluginsReloaded;
            InstallRoutine();
        }

        private static void InstallRoutine()
        {
            var routineDirectory = Path.GetDirectoryName(FileManager.CombatRoutineDestinationPath);
            if (routineDirectory != null && !Directory.Exists(routineDirectory))
                Directory.CreateDirectory(routineDirectory);

            if (File.Exists(FileManager.CombatRoutineSourcePath))
                File.Copy(FileManager.CombatRoutineSourcePath, FileManager.CombatRoutineDestinationPath, true);
        }

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
            if (IsInitialized)
                return;

            // When DB is started via CMD line argument and includes a login then plugins 
            // are initialized by BotMain thread instead of UI thread, causing problems.
            if (!Application.Current.CheckAccess())
                return;

            IsInitialized = true;
        }

        public void OnPulse()
        {
            HookManager.CheckHooks();
            GameUI.SafeClickUIButtons();
            VisualizerViewModel.Instance.UpdateVisualizer();               
        }

        public void OnEnabled()
        {
            if (IsEnabled || !Application.Current.CheckAccess())
                return;

            Core.Init();
            TrinitySettings.InitializeSettings();
            SkillUtils.UpdateActiveSkills();
            TabUi.InstallTab();
            SetupDemonBuddy();
            UILoader.PreLoadWindowContent();
            ModuleManager.Enable();
            Core.Logger.Log($"is now ENABLED: {Description} - now in action!");                        
            IsEnabled = true;            
        }

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
            TabUi.RemoveTab();
            HookManager.ReplaceTreeHooks();
            Navigator.PlayerMover = new DefaultPlayerMover();
            Navigator.StuckHandler = new DefaultStuckHandler();
            CombatTargeting.Instance.Provider = new DefaultCombatTargetingProvider();
            LootTargeting.Instance.Provider = new DefaultLootTargetingProvider();
            ObstacleTargeting.Instance.Provider = new DefaultObstacleTargetingProvider();
            ItemManager.Current = new BlankItemManager();
            Zeta.Bot.RoutineManager.Current = null;
            ModuleManager.Disable();
            Core.Logger.Log($"is now DISABLED!");
        }

        public void OnShutdown()
        {

        }

    }
}