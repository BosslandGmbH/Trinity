using System;
using Trinity.Config;
using Trinity.DbProvider;
using Trinity.Framework.Actors;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Grid;
using Trinity.Framework.Modules;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Helpers;
using Trinity.Movement;
using Zeta.Bot;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework
{
    public static class Core
    {
        public static bool IsEnabled { get; private set; }

        // Memory
        public static Hero Hero { get; } = new Hero(Internals.Addresses.Hero);
        public static Globals Globals { get; } = new Globals(Internals.Addresses.Globals);

        // Modules
        public static ActorCache Actors { get; } = new ActorCache();
        public static HotbarCache Hotbar { get; } = new HotbarCache();
        public static InventoryCache Inventory { get; } = new InventoryCache();
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

        // Misc
        public static GridHelper Grids { get; } = new GridHelper();
        public static PlayerMover PlayerMover { get; } = new PlayerMover();
        public static StuckHandler StuckHandler { get; } = new StuckHandler();
        public static TrinitySetting Settings => TrinityPlugin.Settings;

        public static void Enable()
        {
            if (!IsEnabled)
            {
                IsEnabled = true;                
                Pulsator.OnPulse += Pulse;
                GameEvents.OnWorldChanged += OnWorldChanged;
                ModuleManager.EnableAll();
            }
        }

        private static void OnWorldChanged(object sender, EventArgs eventArgs)
        {
            ModuleManager.FireEventAll(ModuleEventType.WorldChanged);
        }

        public static void Disable()
        {
            IsEnabled = false;
            Pulsator.OnPulse -= Pulse;
            GameEvents.OnWorldChanged -= OnWorldChanged;
            ModuleManager.DisableAll();
        }

        private static void Pulse(object sender, EventArgs eventArgs)
        {
            Update();
        }

        public static void Update(bool force = false)
        {
            ModuleManager.FireEventAll(force ? ModuleEventType.ForcedPulse : ModuleEventType.Pulse);
        }

        public static void Init()
        {            
            Enable();
        }

    }

}







