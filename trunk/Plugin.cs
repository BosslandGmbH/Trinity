//!CompilerOption:AddRef:System.Management.dll
//!CompilerOption:AddRef:System.Web.Extensions.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;
using Buddy.Overlay;
using Trinity.Cache;
using Trinity.Components.Combat.Abilities;
using Trinity.Configuration;
using Trinity.Coroutines;
using Trinity.Coroutines.Resources;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory;
using Trinity.Helpers;
using Trinity.Helpers.AutoFollow.Resources;
using Trinity.Items;
using Trinity.Movement;
using Trinity.ProfileTags;
using Trinity.Routines;
using Trinity.Settings.Loot;
using Trinity.Technicals;
using Trinity.UI;
using Trinity.UI.Overlays;
using Trinity.UI.RadarUI;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Action = System.Action;
using Application = System.Windows.Application;
using Logger = Trinity.Technicals.Logger;

namespace Trinity
{
    /// <summary>
    /// TrinityPlugin DemonBuddy Plugin 
    /// </summary>
    public partial class TrinityPlugin : IPlugin
    {
        public const bool IsDeveloperLoggingEnabled = false;



        private Version _version;
        public Version Version
        {
            get
            {
                if (_version != null) return _version;
                var verXml = XDocument.Load(FileManager.VersionPath).Descendants("Revision").FirstOrDefault();
                if (verXml != null) return new Version(2, 55, Int32.Parse(verXml.Value));
                return new Version(2, 55, 0);
            }
        }

        public string Author => "xzjv, TarasBulba, rrrix, jubisman, Phelon and many more";
        public string Description => $"v{Version} BETA. Provides Combat, Exploration and much more";

        private static bool MouseLeft()
        {
            var result = (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;
            if (result)
                Logger.Log("Mouse Left Down LazyRaider Pause");
            return result;
        }

        private DateTime _lastTestPulseTime = DateTime.MinValue;

        public DateTime LastPulse { get; set; }

        /// <summary>
        /// Receive Pulse event from DemonBuddy.
        /// </summary>
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

                    if (RiftProgression.IsInRift && DateTime.UtcNow.Subtract(_lastTestPulseTime).TotalMilliseconds > 5000)
                    {
                        _lastTestPulseTime = DateTime.UtcNow;
                        Logger.LogSpecial(() => $"RiftProgression%={Core.MemoryModel.Globals.RiftProgressionPct} RiftSouls={Core.MemoryModel.Globals.RiftSouls}");
                    }

                    GameUI.SafeClickUIButtons();

                    //Core.Avoidance.UpdateGrid();
                    VisualizerViewModel.Instance.UpdateVisualizer();

                    if (ZetaDia.Me.IsDead)
                        return;

                    using (new PerformanceLogger("LazyRaiderClickToPause"))
                    {

                        if (Settings.Advanced.LazyRaiderClickToPause && !BotMain.IsPaused && MouseLeft())
                        {
                            BotMain.PauseWhile(MouseLeft);
                        }
                    }

                    // See if we should update the stats file
                    if (DateTime.UtcNow.Subtract(ItemDropStats.ItemStatsLastPostedReport).TotalSeconds > 10)
                    {
                        ItemDropStats.ItemStatsLastPostedReport = DateTime.UtcNow;
                        ItemDropStats.OutputReport();
                    }

                    // Recording of all the XML's in use this run
                    UsedProfileManager.RecordProfile();

                    DebugUtil.LogOnPulse();

                    if (!Settings.Advanced.IsDBInactivityEnabled)
                    {
                        GlobalSettings.Instance.LogoutInactivityTime = 0;
                    }
                    else
                    {
                        GlobalSettings.Instance.LogoutInactivityTime = (float)TimeSpan.FromSeconds(Settings.Advanced.InactivityTimer).TotalMinutes;
                    }

                    if (GoldInactivity.Instance.GoldInactive())
                    {
                        LeaveGame("Gold Inactivity Tripped");
                    }

                    if (XpInactivity.Instance.XpInactive())
                    {
                        LeaveGame("XP Inactivity Tripped");
                    }

                    Gamble.CheckShouldTownRunForGambling();



                    KillAllBountyDetector.Check();

                    //RiftProgression.Pulse();

                    if (!HasLoggedCurrentBuild && BotMain.IsRunning && Core.Inventory.PlayerEquippedIds.Any())
                    {
                        // Requires Inventory Cache to be up to date.                
                        DebugUtil.LogBuildAndItems();
                        HasLoggedCurrentBuild = true;
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

        private static void LeaveGame(string reason)
        {
            Logger.Log(reason);
            GameEvents.FireWorldTransferStart();
            ZetaDia.Service.Party.LeaveGame();
            BotMain.PauseWhile(() => ZetaDia.IsInGame);
        }

        public static bool IsEnabled { get; private set; }

        public bool IsInitialized { get; private set; }

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

                InitializeSettings();

                Core.Enable();
    
                // Kickstart Navigation Server
                PlayerMover.MoveTowards(Player.Position);

                Logger.Log("OnEnable start");
                DateTime dateOnEnabledStart = DateTime.UtcNow;

                BotMain.OnStart += TrinityBotStart;
                BotMain.OnStop += TrinityBotStop;

               

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
                    

                    //PluginCheck.Start();




                    Navigator.PlayerMover = Core.PlayerMover;
                    Navigator.StuckHandler = Core.StuckHandler;
                    GameEvents.OnPlayerDied += TrinityOnDeath;
                    GameEvents.OnGameJoined += TrinityOnJoinGame;
                    GameEvents.OnGameLeft += TrinityOnLeaveGame;
                    //GameEvents.OnItemSold += ItemEvents.TrinityOnItemSold;
                    //GameEvents.OnItemSalvaged += ItemEvents.TrinityOnItemSalvaged;
                    //GameEvents.OnItemDropped += ItemEvents.TrinityOnItemDropped;
                    //GameEvents.OnItemStashed += ItemEvents.TrinityOnItemStashed;
                    //GameEvents.OnItemIdentificationRequest += ItemEvents.TrinityOnOnItemIdentificationRequest;
                    GameEvents.OnGameChanged += GameEvents_OnGameChanged;
                    GameEvents.OnWorldChanged += GameEvents_OnWorldChanged;

                    CombatTargeting.Instance.Provider = new TrinityCombatProvider();
                    LootTargeting.Instance.Provider = new BlankLootProvider();
                    ObstacleTargeting.Instance.Provider = new BlankObstacleProvider();

                    //if (Settings.Loot.ItemFilterMode != ItemFilterMode.DemonBuddy)
                    //{
                    //    ItemManager.Current = new TrinityItemManager();
                    //}

                    // Safety check incase DB "OnStart" event didn't fire properly
                    if (BotMain.IsRunning)
                    {
                        TrinityBotStart(null);
                        if (ZetaDia.IsInGame)
                            TrinityOnJoinGame(null, null);
                    }

                    SetBotTicksPerSecond();

                    UILoader.PreLoadWindowContent();

                    //OverlayLoader.Enable();

                    //ClearArea.Enable();

                    //Core.Enable();

                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "ENABLED: {0} now in action!", Description);
                }

                if (StashRule != null)
                {
                    // reseting stash rules
                    BeginInvoke(() => StashRule.reset());
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

        /// <summary>
        /// Called when user disable the plugin.
        /// </summary>
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
            //Navigator.SearchGridProvider = new MainGridProvider();

            GameEvents.OnPlayerDied -= TrinityOnDeath;
            BotMain.OnStop -= TrinityBotStop;
            GameEvents.OnPlayerDied -= TrinityOnDeath;
            GameEvents.OnGameJoined -= TrinityOnJoinGame;
            GameEvents.OnGameLeft -= TrinityOnLeaveGame;
            GameEvents.OnGameChanged -= GameEvents_OnGameChanged;
            GameEvents.OnWorldChanged -= GameEvents_OnWorldChanged;

            ItemManager.Current = new LootRuleItemManager();

            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "");
            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "DISABLED: TrinityPlugin is now shut down...");
            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "");
            //GenericCache.Shutdown();
            GenericBlacklist.Shutdown();
            //ClearArea.Disable();
            OverlayLoader.Disable();
            Core.Disable();

            ModuleManager.FireEvent(ModuleEvent.PluginDisabled);
        }

        /// <summary>
        /// Called when DemonBuddy shut down.
        /// </summary>
        public void OnShutdown()
        {
            ModuleManager.FireEvent(ModuleEvent.Shutdown);
            GenericBlacklist.Shutdown();
            PluginCheck.Shutdown();
        }
        
        /// <summary>
        /// Called when DemonBuddy initialize the plugin.
        /// </summary>
        public void OnInitialize()
        {
            if (IsInitialized)
                return;

            ZetaDia.Actors.Update();

            if (!Application.Current.CheckAccess())
                return;

            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    Logger.LogVerbose("TrinityPlugin Initializing with PID: {0} CurrentThread={1} '{2}' CanAccessApplication={3}",
            //        Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId,
            //        Thread.CurrentThread.Name, Application.Current.CheckAccess());

            TrinityConditions.Initialize();

            //    Logger.Log("Initialized v{0}", Version);
            //    IsInitialized = true;

            //    var trinityPluginContainer = PluginManager.Plugins.FirstOrDefault(p => p.Plugin == this);
            //    if (trinityPluginContainer != null && !trinityPluginContainer.Enabled)
            //    {
            //        trinityPluginContainer.Enabled = true;
            //    }
            //});
        }

        public string Name => "Trinity";

        public bool Equals(IPlugin other)
        {
            return (other.Name == Name) && (other.Version == Version);
        }

        private static TrinityPlugin _instance;
        public static TrinityPlugin Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TrinityPlugin();
                }
                return _instance;
            }
        }

        public TrinityPlugin()
        {
            _instance = this;

            PluginCheck.CheckAndInstallTrinityRoutine();
            
            if (CharacterSettings.Instance.EnabledPlugins == null)
                CharacterSettings.Instance.EnabledPlugins = new List<string>();

            if (!CharacterSettings.Instance.EnabledPlugins.Contains("Trinity"))
                CharacterSettings.Instance.EnabledPlugins.Add("Trinity");
        }


        private static DateTime _lastWindowTitleTick = DateTime.MinValue;
        private static Window _mainWindow;
        private static bool HasLoggedCurrentBuild;
        private bool _postedWarnings;

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

            string battleTagName = "";
            if (Settings.Advanced.ShowBattleTag)
            {
                try
                {
                    battleTagName = "- " + FileManager.BattleTagName + " ";
                }
                catch
                { }
            }
            string heroName = "";
            if (Settings.Advanced.ShowHeroName)
            {
                try
                {
                    heroName = "- " + ZetaDia.Service.Hero.Name;
                }
                catch { }
            }
            string heroClass = "";
            if (Settings.Advanced.ShowHeroClass)
            {
                try
                {
                    heroClass = "- " + ZetaDia.Service.Hero.Class;
                }
                catch { }
            }


            string windowTitle = "DB " + battleTagName + heroName + heroClass + "- PID:" + Process.GetCurrentProcess().Id;

            if (profileName.Trim() != String.Empty)
            {
                windowTitle += " - " + profileName;
            }

            BeginInvoke(() =>
            {
                try
                {
                    if (_mainWindow != null && !String.IsNullOrWhiteSpace(windowTitle))
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

        public static TimeSpan RunningTime => DateTime.UtcNow.Subtract(BotStartTime);

        internal static void SetBotTicksPerSecond()
        {
            if (Core.Settings.Advanced.TPSEnabled)
            {
                BotMain.TicksPerSecond = Core.Settings.Advanced.TPSLimit;
                //ActorManager.TickDelayMs = Settings.Advanced.TPSLimit < 0 ? 0 : 1000 / Settings.Advanced.TPSLimit;
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "Bot TPS set to {0}", Core.Settings.Advanced.TPSLimit);
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
                    Application.Current.Dispatcher.Invoke(new System.Action(Exit));
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
