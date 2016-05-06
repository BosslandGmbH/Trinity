using Trinity.Config;
using Trinity.DbProvider;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Utilities;
using Trinity.Helpers;
using Trinity.Movement;

namespace Trinity.Framework
{
    public static class Core
    {
        public static TrinitySetting Settings => TrinityPlugin.Settings;

        // Managers
        public static readonly AvoidanceManager Avoidance = new AvoidanceManager();

        // Memory
        public static readonly Hero Hero = new Hero(Internals.Addresses.Hero);
        public static readonly Globals Globals = new Globals(Internals.Addresses.Globals);

        // Utility
        public static readonly CastStatus CastStatus = new CastStatus();
        public static readonly Cooldowns Cooldowns = new Cooldowns();
        public static readonly PlayerHistory PlayerHistory = new PlayerHistory();
        public static readonly Paragon Paragon = new Paragon();

        // Misc
        public static readonly GridHelper Grids = new GridHelper();
        public static readonly PlayerMover PlayerMover = new PlayerMover();
        public static readonly StuckHandler StuckHandler = new StuckHandler();

        public static void Enable()
        {      
            Utility.EnableAll();
        }

        public static void Disable()
        {
            Utility.DisableAll();
        }

        public static void Update()
        {
            Utility.PulseAll();
        }  
          
    }
}





