using System;
//using Trinity.Components.AutoFollow;
using Trinity.Components.Adventurer;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.DbProvider;
using Trinity.Framework.Actors;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Modules;
using Trinity.ProfileTags;
using Trinity.Routines;
using Trinity.Settings;
using Trinity.UI;
using Trinity.UI.UIComponents;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework
{
    public static class Core
    {
        public static bool IsEnabled { get; private set; }
        public static MemoryModel MemoryModel { get; } = new MemoryModel();
        public static RoutineManager Routines => RoutineManager.Instance;

        // Components
        public static Adventurer Adventurer { get; } = Adventurer.Instance;

        // Modules
        public static InventoryCache Inventory { get; } = new InventoryCache();
        public static ActorCache Actors { get; } = new ActorCache();
        public static HotbarCache Hotbar { get; } = new HotbarCache();
        public static PlayerCache Player { get; } = new PlayerCache();
        public static BuffsCache Buffs { get; } = new BuffsCache();
        public static TargetsCache Targets { get; } = new TargetsCache();
        public static AvoidanceManager Avoidance { get; } = new AvoidanceManager();
        public static CastStatus CastStatus { get; } = new CastStatus();
        public static Cooldowns Cooldowns { get; } = new Cooldowns();
        public static PlayerHistory PlayerHistory { get; } = new PlayerHistory();
        public static Paragon Paragon { get; } = new Paragon();
        public static StatusBar StatusBar { get; } = new StatusBar();
        public static GameStopper GameStopper { get; } = new GameStopper();
        public static MarkersCache Markers { get; } = new MarkersCache();
        public static MinimapCache Minimap { get; } = new MinimapCache();
        public static WorldCache World { get; } = new WorldCache();
        public static Clusters Clusters { get; } = new Clusters();
        public static SessionLogger SessionLogger { get; } = new SessionLogger();
        public static ItemLogger ItemLogger { get; } = new ItemLogger();
        public static HeroDataCache HeroData { get; } = new HeroDataCache();
  
        // Misc
        public static GridHelper Grids { get; } = new GridHelper();
        public static PlayerMover PlayerMover { get; } = new PlayerMover();
        public static StuckHandler StuckHandler { get; } = new StuckHandler();
        public static BlockedCheck BlockedCheck { get; } = new BlockedCheck();
        public static ChangeMonitor ChangeMonitor { get; } = new ChangeMonitor();
        public static ProfileSettings ProfileSettings { get; } = new ProfileSettings();

        public static SettingsModel Settings => TrinitySettings.Settings;
        public static TrinityStorage Storage => TrinitySettings.Storage;

        internal static MainGridProvider DBGridProvider => (MainGridProvider)Navigator.SearchGridProvider;
        internal static DefaultNavigationProvider DBNavProvider => (DefaultNavigationProvider)Navigator.NavigationProvider;

        private static void OnGameJoined(object sender, EventArgs e)
        {
            InGameAndStarted = true;
            ModuleManager.FireEvent(ModuleEvent.GameJoined);
        }

        private static void OnGameLeft(object sender, EventArgs e)
        {
            InGameAndStarted = false;
        }

        public static bool InGameAndStarted { get; set; }

        private static void OnWorldChanged(object sender, EventArgs eventArgs) => ModuleManager.FireEvent(ModuleEvent.WorldChanged);

        public static bool GameIsReady => ZetaDia.IsInGame && ZetaDia.Me.IsValid && !ZetaDia.IsLoadingWorld && !ZetaDia.IsPlayingCutscene;

        private static void Pulse(object sender, EventArgs eventArgs)
        {
            if (InGameAndStarted && GameIsReady)
            {
                Update();
            }
        }

        public static void Disable()
        {
            IsEnabled = false;
            Pulsator.OnPulse -= Pulse;
            GameEvents.OnWorldChanged -= OnWorldChanged;
            GameEvents.OnGameJoined -= OnGameJoined;
            ModuleManager.DisableAll();
        }

        public static void Update(bool force = false)
        {
            ModuleManager.FireEvent(force ? ModuleEvent.ForcedPulse : ModuleEvent.Pulse);
        }

        public static void Init()
        {
            GameEvents.OnGameJoined += (sender, args) => GameJoined = true;
            GameEvents.OnGameLeft += (sender, args) => GameJoined = false;
        }

        public static bool GameJoined { get; set; }

        public static void Enable()
        {                  
            if (!IsEnabled)
            {
                Pulsator.OnPulse += Pulse;
                GameEvents.OnWorldChanged += OnWorldChanged;
                GameEvents.OnGameJoined += OnGameJoined;
                GameEvents.OnGameLeft += OnGameLeft;
                ModuleManager.EnableAll();
                Logger.Log("Trinity Framework Enabled");
                IsEnabled = true;
            }
        }
    }

}







