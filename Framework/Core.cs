using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Cache;
using Trinity.Config;
using Trinity.Coroutines;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Grid;
using Trinity.Framework.Utilities;
using Trinity.Items;
using Trinity.UI.UIComponents;

namespace Trinity.Framework
{
    public static class Core
    {
        public static readonly AvoidanceManager Avoidance = new AvoidanceManager();        
        public static readonly Cooldowns Cooldowns = new Cooldowns();
        public static readonly PlayerHistory PlayerHistory = new PlayerHistory();
        public static readonly Paragon Paragon = new Paragon();
        public static readonly GridHelper Grids = new GridHelper();

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

        public static TrinitySetting Settings 
        {
            get { return Trinity.Settings;  }
        }

    }
}




