using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using Trinity.Framework.Helpers;
using Trinity.Framework.Helpers.AutoFollow.Resources;
using Trinity.Framework.Objects;
using Trinity.Items.ItemList;
using Trinity.Settings.Paragon;
using Zeta.Bot;

namespace Trinity.Settings
{
    public static class TrinitySettings
    {
        public static TrinityStorage Storage;

        static TrinitySettings()
        {
            BotMain.OnShutdownRequested += (sender, args) => Save();
        }

        private static void Save()
        {
            Storage.Save();
        }

        public static void InitializeSettings()
        {
            Logger.Log("Initializing TrinitySettings");
            Storage = new TrinityStorage();
            Storage.Load();
        }

        public static SettingsModel Settings = new SettingsModel();
    }

    public class SettingsModel
    {
        public ItemSettings Items => _itemSettings.Object;
        private readonly DynamicSetting<ItemSettings> _itemSettings;

        public WeightingSettings Weighting => _weightingSettings.Object;
        private readonly DynamicSetting<WeightingSettings> _weightingSettings;

        public AdvancedSettings Advanced => _advancedSettings.Object;
        private readonly DynamicSetting<AdvancedSettings> _advancedSettings;

        public ParagonSettings Paragon => _paragonSettings.Object;
        private readonly DynamicSetting<ParagonSettings> _paragonSettings;

        public KanaisCubeSetting KanaisCube => _kanaisCubeSetting.Object;
        private readonly DynamicSetting<KanaisCubeSetting> _kanaisCubeSetting;

        public CombatSettings Combat => _combatSettings.Object;
        private readonly DynamicSetting<CombatSettings> _combatSettings;

        public ItemListSettings ItemList => _itemListSettings.Object;
        private readonly DynamicSetting<ItemListSettings> _itemListSettings;

        public RoutineSettings Routine => _routineSettings.Object;
        private readonly DynamicSetting<RoutineSettings> _routineSettings;

        public SettingsModel()
        {
            _itemSettings = new DynamicSetting<ItemSettings>();
            _weightingSettings = new DynamicSetting<WeightingSettings>();
            _advancedSettings = new DynamicSetting<AdvancedSettings>();
            _paragonSettings = new DynamicSetting<ParagonSettings>();
            _kanaisCubeSetting = new DynamicSetting<KanaisCubeSetting>();
            _combatSettings = new DynamicSetting<CombatSettings>();
            _itemListSettings = new DynamicSetting<ItemListSettings>();
            _routineSettings = new DynamicSetting<RoutineSettings>();
        }
        
        public IEnumerable<IDynamicSetting> DynamicSettings => new List<IDynamicSetting>
        {
            _itemSettings,
            _weightingSettings,
            _advancedSettings,
            _paragonSettings,
            _kanaisCubeSetting,
            _combatSettings,
            _itemListSettings,
            _routineSettings,
        };
    }

}


