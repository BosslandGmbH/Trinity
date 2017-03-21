using System;
using Trinity.Framework;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.Util;
using Trinity.UI;


namespace Trinity.Components.Adventurer.UI
{
    public class ConfigWindow : Window
    {
        private static ConfigWindow _instance;
        public static ConfigWindow Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConfigWindow();

                if (_instance.Content == null)
                    return null;

                _instance.Closed += OnClosed;
                return _instance;
            }
        }

        private static void OnClosed(object sender, EventArgs e)
        {
            if (_instance != null)
            {
                _instance.Closed -= OnClosed;
                _instance = null;
            }
        }

        private PluginSettings _pluginSettings;

        public int BattleNetHeroId { get; set; }

        public ConfigWindow()
        {
            UserControl mainControl;
            Height = 580;
            Width = 700;
            var path = Path.Combine(FileUtils.PluginPath, "Components", "Adventurer", "UI", "Config.xaml");
            try
            {
                mainControl = UILoader.LoadAndTransformXamlFile<UserControl>(path);
            }
            catch (Exception)
            {
                Core.Logger.Error($"Couldn't find the file {path}");
                return;
            }
            Content = mainControl;
            Title = "Adventurer Settings";
            ResizeMode = ResizeMode.NoResize;
            if (Top < 0) Top = 0;

            var greaterRiftLevel = LogicalTreeHelper.FindLogicalNode(mainControl, "GreaterRiftLevel") as ComboBox;
            if(greaterRiftLevel != null)
                greaterRiftLevel.SelectionChanged += GreaterRiftLevel_SelectionChanged;

            _pluginSettings = PluginSettings.Current;

            DataContext = _pluginSettings;
        }

        private void GreaterRiftLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _pluginSettings.UpdateGemList();
        }

    }
}