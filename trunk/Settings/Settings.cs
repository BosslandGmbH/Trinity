﻿using System;
﻿using System.IO;
﻿using System.Threading;
﻿using System.Windows;
﻿using Trinity.Components.Combat.Abilities;
﻿using Trinity.Config;
﻿using Trinity.Technicals;
﻿using Trinity.UI;
﻿using Zeta.Game;

namespace Trinity
{
    public partial class TrinityPlugin
    {
        // Save Configuration
        private void SaveConfiguration()
        {
            TrinityPluginSettings.Settings.Save();
        }

        // Load Configuration
        private void InitializeSettings()
        {
            Logger.Log("Initializing TrinitySettings");
            TrinityPluginSettings.Settings = new TrinitySetting();
            TrinityPluginSettings.Settings.Load();
        }

        /// <summary>
        /// Gets the configuration Window for UnifiedTrinity.
        /// </summary>
        /// <value>The display window.</value>
        public Window DisplayWindow
        {
            get
            {
                return UILoader.GetDisplayWindow(Path.Combine(FileManager.PluginPath, "UI"));
            }
        }
    }
}