using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventurer;
using Trinity.Cache;
using Trinity.Config;
using Trinity.Coroutines;
using Trinity.DbProvider;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Utilities;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Movement;
using Trinity.UI.UIComponents;
using Zeta.Bot.Navigation;
using Zeta.Game;

namespace Trinity.Framework
{
    public static class Core
    {
        public static TrinitySetting Settings => TrinityPlugin.Settings;
        public static readonly AvoidanceManager Avoidance = new AvoidanceManager();        
        public static readonly Cooldowns Cooldowns = new Cooldowns();
        public static readonly PlayerHistory PlayerHistory = new PlayerHistory();
        public static readonly Paragon Paragon = new Paragon();
        public static readonly GridHelper Grids = new GridHelper();
        public static readonly StuckHandler StuckHandler = new StuckHandler();
        public static readonly PlayerMover PlayerMover = new PlayerMover();
        public static readonly Hero Hero = new Hero(Internals.Addresses.Hero);
        public static readonly Globals Globals = new Globals(Internals.Addresses.Globals);

        public static void Enable()
        {
            Cooldowns.Enable();
            Avoidance.Enable();            
            PlayerHistory.Enable();
            Paragon.Enable();
        }

        public static void Disable()
        {
            Cooldowns.Disable();
            Avoidance.Disable();
            PlayerHistory.Disable();
            Paragon.Disable();
        }

    
    }
}




