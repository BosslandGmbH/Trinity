//!CompilerOption:AddRef:System.Management.dll
//!CompilerOption:AddRef:System.Web.Extensions.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using Buddy.Overlay;
using Trinity.Cache;
using Trinity.Combat;
using Trinity.Combat.Abilities;
using Trinity.Configuration;
using Trinity.Coroutines;
using Trinity.Coroutines.Town;
using TrinityCoroutines;
using TrinityCoroutines.Resources;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Utilities;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Movement;
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
using Logger = Trinity.Technicals.Logger;
using BotManager = Trinity.BotManager;

namespace Trinity
{
    /// <summary>
    /// TrinityPlugin DemonBuddy Plugin 
    /// </summary>
    public partial class TrinityPlugin : ICommunicationEnabledPlugin
    {
        public const bool IsDeveloperLoggingEnabled = false;

        public PluginCommunicationResponse Receive(IPlugin sender, string command, params object[] args)
        {
            return PluginCommunicator.Receive(sender, command, args);
        }

        private Version _version;
        public Version Version
        {
            get
            {
                if (_version != null) return _version;
                var verXml = XDocument.Load(FileManager.VersionPath).Descendants("Revision").FirstOrDefault();
                if (verXml != null) return new Version(2,41, int.Parse(verXml.Value));
                return new Version(2, 41, 0);
            }
        }

        public string Author
        {
            get
            {
                return "xzjv, rrrix, jubisman, and many more";
            }
        }

        public string Description
        {
            get
            {
                return string.Format("TrinityPlugin v{0}", Version);
            }
        }

        private static bool MouseLeft()
        {
            var result = (System.Windows.Forms.Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) == System.Windows.Forms.MouseButtons.Left;
            if (result)
                Logger.Log("Mouse Left Down LazyRaider Pause");
            return result;
        }
        
        private DateTime _lastTestPulseTime = DateTime.MinValue;

        private bool isRiftBossSpawned;

        /// <summary>
        /// Receive Pulse event from DemonBuddy.
        /// </summary>
        public void OnPulse()
        {
            try
            {
                using (new PerformanceLogger("TrinityPlugin Pulse"))
                {

                    if (!_postedWarnings)
                    {
                        if (Settings.Advanced.PhelonsPlayground)
                            PPWarning();

                        _postedWarnings = true;
                    }

                    if (ZetaDia.Me == null)
                        return;

                    if (!ZetaDia.IsInGame || !ZetaDia.Me.IsValid || ZetaDia.IsLoadingWorld)
                        return;

                    if (RiftProgression.IsInRift && DateTime.UtcNow.Subtract(_lastTestPulseTime).TotalMilliseconds > 5000)
                    {
                        _lastTestPulseTime = DateTime.UtcNow;
                        Logger.LogSpecial(() => $"RiftProgression%={Core.Globals.RiftProgressionPct} RiftSouls={Core.Globals.RiftSouls}");
                    }

                    using (new PerformanceLogger("OnPulse"))
                    {
                        GameUI.SafeClickUIButtons();

                        using (new PerformanceLogger("TargetCheck.RefreshCache"))
                        {  
                            RefreshDiaObjectCache();
                        }

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

                        MonkCombat.RunOngoingPowers();

                        KillAllBountyDetector.Check();

                        //RiftProgression.Pulse();

                        if (!HasLoggedCurrentBuild && BotMain.IsRunning && CacheData.Inventory.EquippedIds.Any())
                        {
                            // Requires Inventory Cache to be up to date.                
                            DebugUtil.LogBuildAndItems();
                            HasLoggedCurrentBuild = true;
                        }
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

        /// <summary>
        /// Called when user Enable the plugin.
        /// </summary>
        public void OnEnabled()
        {
            Logger.Log($"Trinity OnEnabled was called from thread: {Thread.CurrentThread.Name} ({Thread.CurrentThread.ManagedThreadId})");

            if (!Application.Current.CheckAccess()) return;

            try
            {
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
                    _isPluginEnabled = true;

                    //PluginCheck.Start();

                    // Settings are available after this... 
                    LoadConfiguration();


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

                    BotManager.SetBotTicksPerSecond();

                    UILoader.PreLoadWindowContent();

                    Events.OnCacheUpdated += Enemies.Update;

                    //OverlayLoader.Enable();

                    //ClearArea.Enable();

                    Core.Enable();

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
        }

        private static void PPWarning()
        {
            Logger.Warn("-------------------------------------------------------------------------------");
            Logger.Warn("---   ");
            Logger.Warn("---   Phelons Playground mode is Enabled");
            Logger.Warn("---   This mode is experimental and not yet released or supported ");
            Logger.Warn("---   DO NOT USE THIS MODE unless you are a developer/tester and know what you're doing");
            Logger.Warn("---   DO NOT REPORT TRINITY BUGS while in this mode");
            Logger.Warn("---   To Disable: Uncheck box in Trinity Settings > Advanced Tab > Phelons Playground");
            Logger.Warn("---   ");
            Logger.Warn("------------------------------------------------------------------------------");
        }

        /// <summary>
        /// Called when user disable the plugin.
        /// </summary>
        public void OnDisabled()
        {
            _isPluginEnabled = false;

            TabUi.RemoveTab();
            BotManager.ReplaceTreeHooks();



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
            //GameEvents.OnItemSold -= ItemEvents.TrinityOnItemSold;
            //GameEvents.OnItemSalvaged -= ItemEvents.TrinityOnItemSalvaged;
            //GameEvents.OnItemStashed -= ItemEvents.TrinityOnItemStashed;
            //GameEvents.OnItemIdentificationRequest -= ItemEvents.TrinityOnOnItemIdentificationRequest;
            GameEvents.OnGameChanged -= GameEvents_OnGameChanged;
            GameEvents.OnWorldChanged -= GameEvents_OnWorldChanged;

            ItemManager.Current = new LootRuleItemManager();

            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "");
            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "DISABLED: TrinityPlugin is now shut down...");
            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "");
            GenericCache.Shutdown();
            GenericBlacklist.Shutdown();
            //ClearArea.Disable();
            OverlayLoader.Disable();
            Core.Disable();

        }

        /// <summary>
        /// Called when DemonBuddy shut down.
        /// </summary>
        public void OnShutdown()
        {
            GenericCache.Shutdown();
            GenericBlacklist.Shutdown();
            PluginCheck.Shutdown();
        }

        /// <summary>
        /// Called when DemonBuddy initialize the plugin.
        /// </summary>
        public void OnInitialize()
        {
            // YAR Login Support (YARKickstart Ibot will call this from the proper thread)
            if (!Application.Current.CheckAccess()) return;

            PluginCheck.CheckAndInstallTrinityRoutine();
            Logger.Log("Initialized v{0}", Version);

            Logger.LogVerbose("TrinityPlugin Initialized with PID: {0} CurrentThread={1} '{2}' CanAccessApplication={3}",
                Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId,
                Thread.CurrentThread.Name, Application.Current.CheckAccess());

            Core.Init();
            //ActorManager.Initialize();

        }

        public string Name
        {
            get
            {
                return "Trinity";
            }
        }

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

        public static StuckHandler StuckHandler { get; set; }

        //public static bool IsMoveRequested { get; set; }

        public TrinityPlugin()
        {
            _instance = this;
            PluginCheck.CheckAndInstallTrinityRoutine();

            Execution.Initialize();
        }

        /// <summary>
        ///  Do not remove nav server logging. We need to be able to identify the cause of stuck issues users report
        ///  and to do that we need to be able to differentiate between bugs in trinity and navigation server problems.
        /// </summary>
        public static void NavServerReport(bool silent = false, MoveResult moveResult = default(MoveResult), [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            //if (Navigator.SearchGridProvider.Width == 0 || Navigator.SearchGridProvider.SearchArea.Length == 0)
            //{
            //    if (silent)
            //        Logger.LogVerbose("Waiting for Navigation Server... ({0})", caller);
            //    else
            //        Logger.Log("Waiting for Navigation Server... ({0})", caller);
            //}

            //else if (moveResult == MoveResult.PathGenerating)
            //{
            //    Logger.LogVerbose("Navigation Path is Generating... ({0})", caller);
            //}

            //else if (moveResult == MoveResult.PathGenerationFailed)
            //{
            //    Logger.LogVerbose("Navigation Path Failed to Generate... ({0})", caller);
            //}
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
                Application.Current.Dispatcher.BeginInvoke(new System.Action(() => _mainWindow = Application.Current.MainWindow));
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


            string windowTitle = "DB " + battleTagName + heroName + heroClass + "- PID:" + System.Diagnostics.Process.GetCurrentProcess().Id;

            if (profileName.Trim() != String.Empty)
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

        internal static void BeginInvoke(System.Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }

        internal static void Invoke(System.Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }




    }
}
