using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.UI.UIComponents;
using Trinity.Components.Adventurer.Util;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.UI
{
    public class ConfigWindow : Window
    {
        #region Static Stuff

        private static ConfigWindow _instance;
        public static ConfigWindow Instance
        {
            get
            {
                //var heroId = ZetaDia.Service?.Hero?.IsValid;

                //if (BotEvents.IsBotRunning || BotMain.IsPausedForStateExecution || BotMain.IsPaused || ZetaDia.IsLoadingWorld)
                //{
                //    Logger.Error("SettingsStorage are not accessible while the bot is running, please stop the bot and try again.");
                //    return null;
                //}
                ////AdvDia.Update();
                //if (_instance != null)
                //{
                //    var result = SafeFrameLock.ExecuteWithinFrameLock(() =>
                //    {
                //        if (AdvDia.BattleNetHeroId == 0 || AdvDia.BattleNetHeroId != _instance.BattleNetHeroId)
                //        {
                //            Logger.Error("Invalid hero or switched hero, closing the settings window.");
                //            Logger.Debug("Instance HeroID: {0}, Current HeroID: {1}", _instance.BattleNetHeroId,
                //                AdvDia.BattleNetHeroId);
                //            _instance.Close();
                //        }
                //    });
                //    if (!result.Success)
                //    {
                //        return null;
                //    }
                //    return _instance;
                //}

                //if (!ZetaDia.IsInGame)
                //{
                //    Logger.Error("You must be in a game to open the settings");
                //    return null;
                //}

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
                _instance.SaveButton_Click(null, new RoutedEventArgs());
                _instance.Closed -= OnClosed;
                _instance = null;
            }
        }

        #endregion

        private UserControl _mainControl;
        private Button _saveButton;
        private Button _cancelButton;
        private Button _gemPriorityUp;
        private Button _gemPriorityDown;
        private ListBox _gemPriorityList;
        private CheckBox _prioritizeEquippedGems;
        private ComboBox _greaterRiftLevel;
        private PluginSettings _pluginSettings;

        private CheckBox _bountyAct1;
        private CheckBox _bountyAct2;
        private CheckBox _bountyAct3;
        private CheckBox _bountyAct4;
        private CheckBox _bountyAct5;
        private CheckBox _bountyPrioritizeBonusAct;

        public int BattleNetHeroId { get; set; }


        public ConfigWindow()
        {
            Height = 580;
            Width = 700;
            var path = Path.Combine(FileUtils.PluginPath, "Components", "Adventurer", "UI", "Config.xaml");
            try
            {
                _mainControl = UILoader.LoadAndTransformXamlFile<UserControl>(path);
            }
            catch (Exception)
            {
                Logger.Error($"Couldn't find the file {path}");
                return;
            }
            Content = _mainControl;
            //Title = "Adventurer Config for " + ZetaDia.Service.Hero.Name + " - " + ZetaDia.Service.Hero.Class;
            Title = "Adventurer Settings";
            ResizeMode = ResizeMode.NoResize;
            //Top = Application.Current.MainWindow.Top + (Application.Current.MainWindow.Height - Height) / 2;
            //Left = Application.Current.MainWindow.Left + (Application.Current.MainWindow.Width - Width) / 2;
            if (Top < 0) Top = 0;

            //_saveButton = LogicalTreeHelper.FindLogicalNode(_mainControl, "SaveButton") as Button;
            //_saveButton.Click += SaveButton_Click;

            //_cancelButton = LogicalTreeHelper.FindLogicalNode(_mainControl, "CancelButton") as Button;
            //_cancelButton.Click += CancelButton_Click;


            _gemPriorityUp = LogicalTreeHelper.FindLogicalNode(_mainControl, "GemPriorityUp") as Button;
            _gemPriorityUp.Click += GemPriorityUp_Click;

            _gemPriorityDown = LogicalTreeHelper.FindLogicalNode(_mainControl, "GemPriorityDown") as Button;
            _gemPriorityDown.Click += GemPriorityDown_Click;

            _gemPriorityList = LogicalTreeHelper.FindLogicalNode(_mainControl, "GemPriorityList") as ListBox;

            _prioritizeEquippedGems = LogicalTreeHelper.FindLogicalNode(_mainControl, "GreaterRiftPrioritizeEquipedGems") as CheckBox;
            _prioritizeEquippedGems.Click += PrioritizeEquippedGems_Click;

            _greaterRiftLevel = LogicalTreeHelper.FindLogicalNode(_mainControl, "GreaterRiftLevel") as ComboBox;
            _greaterRiftLevel.SelectionChanged += GreaterRiftLevel_SelectionChanged;


            _bountyAct1 = LogicalTreeHelper.FindLogicalNode(_mainControl, "BountyAct1") as CheckBox;
            _bountyAct2 = LogicalTreeHelper.FindLogicalNode(_mainControl, "BountyAct2") as CheckBox;
            _bountyAct3 = LogicalTreeHelper.FindLogicalNode(_mainControl, "BountyAct3") as CheckBox;
            _bountyAct4 = LogicalTreeHelper.FindLogicalNode(_mainControl, "BountyAct4") as CheckBox;
            _bountyAct5 = LogicalTreeHelper.FindLogicalNode(_mainControl, "BountyAct5") as CheckBox;
            _bountyPrioritizeBonusAct = LogicalTreeHelper.FindLogicalNode(_mainControl, "BountyPrioritizeBonusAct") as CheckBox;

            _pluginSettings = PluginSettings.Current;

            DataContext = _pluginSettings;
            //BattleNetHeroId = heroId;
        }

        private void GreaterRiftLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _pluginSettings.UpdateGemList();
            Settings_GemListUpdated();
        }

        private void PrioritizeEquippedGems_Click(object sender, RoutedEventArgs e)
        {
            _pluginSettings.UpdateGemList();
            Settings_GemListUpdated();
        }

        private void Settings_GemListUpdated()
        {
            _gemPriorityList.ItemsSource = null;
            _gemPriorityList.ItemsSource = _pluginSettings.GemUpgradePriority;
        }

        private void GemPriorityDown_Click(object sender, RoutedEventArgs e)
        {
            var settings = _pluginSettings;
            if (settings == null) return;
            if (_gemPriorityList == null) return;
            if (_gemPriorityList.SelectedItems == null || _gemPriorityList.SelectedItems.Count == 0) return;
            var prioritizeEquipedGems = false;
            if (_prioritizeEquippedGems != null)
            {
                prioritizeEquipedGems = _prioritizeEquippedGems.IsChecked.GetValueOrDefault();
            }
            var selectedIndex = _gemPriorityList.SelectedIndex;
            var selectedItem = settings.GemUpgradePriority[selectedIndex];
            var nextIndex = selectedIndex + 1;
            if (nextIndex + 1 >= settings.GemUpgradePriority.Count) return;
            var nextItem = settings.GemUpgradePriority[nextIndex];
            if (nextItem.IsMaxRank) return;
            if (prioritizeEquipedGems && selectedItem.IsEquiped && !nextItem.IsEquiped) return;
            Swap(settings.GemUpgradePriority, selectedIndex, nextIndex);
            Settings_GemListUpdated();
            _gemPriorityList.SelectedIndex = nextIndex;
        }

        private void GemPriorityUp_Click(object sender, RoutedEventArgs e)
        {
            var settings = _pluginSettings;
            if (settings == null) return;
            if (_gemPriorityList == null) return;
            if (_gemPriorityList.SelectedItems == null || _gemPriorityList.SelectedItems.Count == 0) return;
            var prioritizeEquipedGems = false;
            if (_prioritizeEquippedGems != null)
            {
                prioritizeEquipedGems = _prioritizeEquippedGems.IsChecked.GetValueOrDefault();
            }
            var selectedIndex = _gemPriorityList.SelectedIndex;
            var selectedItem = settings.GemUpgradePriority[selectedIndex];
            var nextIndex = selectedIndex - 1;
            if (nextIndex < 0) return;
            var nextItem = settings.GemUpgradePriority[nextIndex];
            if (prioritizeEquipedGems && nextItem.IsEquiped && !selectedItem.IsEquiped) return;
            Swap(settings.GemUpgradePriority, selectedIndex, nextIndex);
            Settings_GemListUpdated();
            _gemPriorityList.SelectedIndex = nextIndex;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = _pluginSettings;
            if (settings != null)
            {
                settings.Save();
            }
            Instance.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Instance.Close();
        }

        private static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

    }
}
