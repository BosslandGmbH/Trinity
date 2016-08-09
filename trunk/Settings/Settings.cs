﻿using System.IO;
﻿using System.Windows;
﻿using Trinity.Components.Combat.Abilities;
﻿using Trinity.Technicals;
﻿using Trinity.UI;

namespace Trinity
{
    public partial class TrinityPlugin
    {
        // Save Configuration
        private void SaveConfiguration()
        {
            Settings.Save();
        }
        // Load Configuration
        private void LoadConfiguration()
        {
            Settings.Load();
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