﻿using System.IO;
﻿using System.Windows;
﻿using Trinity.Combat.Abilities;
﻿using Trinity.Technicals;
﻿using Trinity.UI;

namespace Trinity
{
    public partial class Trinity
    {
        // Save Configuration
        private void SaveConfiguration()
        {
            Settings.Save();
            CombatBase.LoadCombatSettings();
        }
        // Load Configuration
        private void LoadConfiguration()
        {
            Settings.Load();
            CombatBase.LoadCombatSettings();
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