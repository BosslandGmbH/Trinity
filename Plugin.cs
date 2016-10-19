//!CompilerOption:AddRef:System.Management.dll
//!CompilerOption:AddRef:System.Web.Extensions.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Items;
using Trinity.ProfileTags;
using Trinity.Reference;
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
        public string Name => "Trinity";
        public Version Version => new Version(2, 55, 669);
        public string Author => "xzjv, TarasBulba, rrrix, jubisman, Phelon and many more";
        public string Description => $"v{Version} provides combat, exploration and much more";
        public Window DisplayWindow => UILoader.GetDisplayWindow(Path.Combine(FileManager.PluginPath, "UI"));


        private static TrinityPlugin _instance;
        private static DateTime _lastWindowTitleTick = DateTime.MinValue;
        private static Window _mainWindow;
        private static bool _hasLoggedCurrentBuild;

        public TrinityPlugin()
        {
            _instance = this;

            PluginCheck.CheckAndInstallTrinityRoutine();

            if (CharacterSettings.Instance.EnabledPlugins == null)
                CharacterSettings.Instance.EnabledPlugins = new List<string>();

            if (!CharacterSettings.Instance.EnabledPlugins.Contains("Trinity"))
                CharacterSettings.Instance.EnabledPlugins.Add("Trinity");
        }

        public DateTime LastPulse { get; set; }
        public static bool IsEnabled { get; private set; }
        public bool IsInitialized { get; private set; }
        public static TrinityPlugin Instance => _instance ?? (_instance = new TrinityPlugin());

        public void OnPulse()
        {
            try
            {
                using (new PerformanceLogger($"OnPulse ({DateTime.UtcNow.Subtract(LastPulse).ToString("mm':'ss':'fff")})"))
                {
                    LastPulse = DateTime.UtcNow;

                    HookManager.CheckHooks();

                    if (ZetaDia.Me == null)
                        return;

                    if (!ZetaDia.IsInGame || !ZetaDia.Me.IsValid || ZetaDia.IsLoadingWorld)
                        return;

                    GameUI.SafeClickUIButtons();

                    VisualizerViewModel.Instance.UpdateVisualizer();

                    if (ZetaDia.Me.IsDead)
                        return;

                    using (new PerformanceLogger("LazyRaiderClickToPause"))
                    {
                        if (Core.Settings.Advanced.LazyRaider && !BotMain.IsPaused && MouseLeft())
                        {
                            BotMain.PauseWhile(MouseLeft);
                        }
                    }

                    DebugUtil.LogOnPulse();

                    // Turn off DB's inactivity detection.
                    GlobalSettings.Instance.LogoutInactivityTime = 0;
            
                    if (GoldInactivity.Instance.GoldInactive())
                    {
                        LeaveGame("Gold Inactivity Tripped");
                    }

                    if (XpInactivity.Instance.XpInactive())
                    {
                        LeaveGame("XP Inactivity Tripped");
                    }

                    if (!_hasLoggedCurrentBuild && BotMain.IsRunning && Core.Inventory.PlayerEquippedIds.Any())
                    {
                        DebugUtil.LogBuildAndItems();
                        _hasLoggedCurrentBuild = true;
                    }
                }
            }
            catch (AccessViolationException)
            {
                // woof! 
            }
            catch (Exception ex)
            {
                Logger.Log(LogCategory.UserInformation, $"Exception in Pulse: {ex}");
            }
        }

        public void OnEnabled()
        {
            if (IsEnabled)
                return;

            try
            {
                if (!Application.Current.CheckAccess())
                {
                    Logger.LogVerbose($"TrinityPlugin Skipped OnEnabled() attempt by PID: {Process.GetCurrentProcess().Id} CurrentThread={Thread.CurrentThread.ManagedThreadId} '{Thread.CurrentThread.Name}' CanAccessApplication={Application.Current.CheckAccess()}");
                    return;
                }

                Logger.Log($"Trinity OnEnabled() was called from thread: {Thread.CurrentThread.Name} ({Thread.CurrentThread.ManagedThreadId})");
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                Core.Init();
                TrinitySettings.InitializeSettings();
                Core.Enable();
                Core.PlayerMover.MoveTowards(Core.Player.Position);
                Logger.Log("OnEnable start");
                var dateOnEnabledStart = DateTime.UtcNow;
                BotMain.OnStart += TrinityEventHandlers.TrinityBotStart;
                BotMain.OnStop += TrinityEventHandlers.TrinityBotStop;
                SetWindowTitle();
                TabUi.InstallTab();

                if (!Directory.Exists(FileManager.PluginPath))
                {
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Cannot enable plugin. Invalid path: {0}", FileManager.PluginPath);
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Please check you have installed the plugin to the correct location, and then restart DemonBuddy and re-enable the plugin.");
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, @"Plugin should be installed to \<DemonBuddyFolder>\Plugins\TrinityPlugin\");
                }
                else
                {
                    Navigator.PlayerMover = Core.PlayerMover;
                    Navigator.StuckHandler = Core.StuckHandler;
                    GameEvents.OnPlayerDied += TrinityEventHandlers.TrinityOnDeath;
                    GameEvents.OnGameJoined += TrinityEventHandlers.TrinityOnJoinGame;
                    GameEvents.OnGameLeft += TrinityEventHandlers.TrinityOnLeaveGame;
                    GameEvents.OnGameChanged += TrinityEventHandlers.GameEvents_OnGameChanged;
                    GameEvents.OnWorldChanged += TrinityEventHandlers.GameEvents_OnWorldChanged;

                    CombatTargeting.Instance.Provider = new TrinityCombatProvider();
                    LootTargeting.Instance.Provider = new BlankLootProvider();
                    ObstacleTargeting.Instance.Provider = new BlankObstacleProvider();

                    if (BotMain.IsRunning)
                    {
                        TrinityEventHandlers.TrinityBotStart(null);
                        if (ZetaDia.IsInGame)
                            TrinityEventHandlers.TrinityOnJoinGame(null, null);
                    }

                    SetBotTicksPerSecond();
                    UILoader.PreLoadWindowContent();
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "ENABLED: {0} now in action!", Description);
                }

                Logger.LogDebug("OnEnable took {0}ms", DateTime.UtcNow.Subtract(dateOnEnabledStart).TotalMilliseconds);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in OnEnable: " + ex);
            }

            IsEnabled = true;
            ModuleManager.FireEvent(ModuleEvent.PluginEnabled);
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
            GameEvents.OnPlayerDied -= TrinityEventHandlers.TrinityOnDeath;
            BotMain.OnStop -= TrinityEventHandlers.TrinityBotStop;
            GameEvents.OnPlayerDied -= TrinityEventHandlers.TrinityOnDeath;
            GameEvents.OnGameJoined -= TrinityEventHandlers.TrinityOnJoinGame;
            GameEvents.OnGameLeft -= TrinityEventHandlers.TrinityOnLeaveGame;
            GameEvents.OnGameChanged -= TrinityEventHandlers.GameEvents_OnGameChanged;
            GameEvents.OnWorldChanged -= TrinityEventHandlers.GameEvents_OnWorldChanged;
            ItemManager.Current = new LootRuleItemManager();
            GenericBlacklist.Shutdown();
            Core.Disable();
            ModuleManager.FireEvent(ModuleEvent.PluginDisabled);
            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "DISABLED: TrinityPlugin is now shut down...");
        }

        public void OnShutdown()
        {
            ModuleManager.FireEvent(ModuleEvent.Shutdown);
            GenericBlacklist.Shutdown();
            PluginCheck.Shutdown();
        }

        public void OnInitialize()
        {
            if (IsInitialized)
                return;

            ZetaDia.Actors.Update();

            if (!Application.Current.CheckAccess())
                return;

            TrinityConditions.Initialize();
            IsInitialized = true;
        }

        public bool Equals(IPlugin other)
        {
            return (other.Name == Name) && (other.Version == Version);
        }

        private static bool MouseLeft()
        {
            var result = (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;
            if (result)
                Logger.Log("Mouse Left Down LazyRaider Pause");
            return result;
        }

        private static void LeaveGame(string reason)
        {
            Logger.Log(reason);
            GameEvents.FireWorldTransferStart();
            ZetaDia.Service.Party.LeaveGame();
            BotMain.PauseWhile(() => ZetaDia.IsInGame);
        }

        internal static void SetWindowTitle(string profileName = "")
        {
            if (DateTime.UtcNow.Subtract(_lastWindowTitleTick).TotalMilliseconds < 1000)
                return;

            _lastWindowTitleTick = DateTime.UtcNow;

            if (_mainWindow == null)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => _mainWindow = Application.Current.MainWindow));
            }

            if (_mainWindow == null || !ZetaDia.Service.IsValid || !ZetaDia.Service.Platform.IsValid || !ZetaDia.Service.Platform.IsConnected)
                return;

            var battleTagName = "";
            if (Core.Settings.Advanced.ShowBattleTag)
            {
                try
                {
                    battleTagName = "- " + FileManager.BattleTagName + " ";
                }
                catch
                {
                }
            }
            var heroName = "";
            if (Core.Settings.Advanced.ShowHeroName)
            {
                try
                {
                    heroName = "- " + ZetaDia.Service.Hero.Name;
                }
                catch
                {
                }
            }
            var heroClass = "";
            if (Core.Settings.Advanced.ShowHeroClass)
            {
                try
                {
                    heroClass = "- " + ZetaDia.Service.Hero.Class;
                }
                catch
                {
                }
            }


            var windowTitle = "DB " + battleTagName + heroName + heroClass + "- PID:" + Process.GetCurrentProcess().Id;

            if (profileName.Trim() != string.Empty)
            {
                windowTitle += " - " + profileName;
            }

            BeginInvoke(() =>
            {
                try
                {
                    if (_mainWindow != null && !string.IsNullOrWhiteSpace(windowTitle))
                    {
                        _mainWindow.Title = windowTitle;
                    }
                }
                catch
                {
                }
            });
        }

        internal static void BeginInvoke(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }

        internal static void Invoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        internal static void SetBotTicksPerSecond()
        {
            if (Core.Settings.Advanced.TpsEnabled)
            {
                BotMain.TicksPerSecond = Core.Settings.Advanced.TpsLimit;
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "Bot TPS set to {0}", Core.Settings.Advanced.TpsLimit);
            }
            else
            {
                BotMain.TicksPerSecond = 30;
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "Reset bot TPS to default: {0}", 30);
            }
        }

        internal static void Exit()
        {
            ZetaDia.Memory.Process.Kill();

            try
            {
                if (Thread.CurrentThread != Application.Current.Dispatcher.Thread)
                {
                    Application.Current.Dispatcher.Invoke(Exit);
                    return;
                }

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }
    }
}