using System;
using Trinity.Framework;
using System.Windows;
using System.Windows.Controls;


namespace Trinity.Components.Adventurer.UI
{
    internal static class MainUI
    {
        private static Grid _mainTabGrid;
        private static Button _configureAdventurerButton;

        internal static void InstallButtons()
        {
            Window mainWindow;
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    try
                    {
                        mainWindow = Application.Current.MainWindow;
                        var tabs = mainWindow.FindName("tabControlMain") as TabControl;
                        if (tabs == null)
                            return;
                        var mainTab = (TabItem)tabs.Items[0];
                        _mainTabGrid = (Grid)mainTab.Content;
                        if (_configureAdventurerButton == null)
                        {
                            //_toggleAdventurerStateButton = CreateToggleButton();
                            _configureAdventurerButton = CreateConfigureButton();
                            //_mainTabGrid.Children.Add(_toggleAdventurerStateButton);
                            _mainTabGrid.Children.Add(_configureAdventurerButton);
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Error("[MainUI][InstallButtons] " + ex.Message);
                    }
                });
        }

        internal static void RemoveButtons()
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    try
                    {
                        if (_configureAdventurerButton != null)
                        {
                            _configureAdventurerButton.Visibility = Visibility.Collapsed;
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Error("[MainUI][RemoveButtons] " + ex.Message);
                    }
                });
        }

        private static Button CreateConfigureButton()
        {
            var button = new Button
            {
                Name = "btnConfigureAdventurer",
                Content = "Adventurer Settings",
                Width = 120,
                Height = 20,
                Margin = new Thickness(428, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            button.Click += Configure_Click; ;
            return button;
        }

        private static void Configure_Click(object sender, RoutedEventArgs e)
        {
            var instance = ConfigWindow.Instance;
            if (instance != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        instance.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Error("[MainUI][Configure] " + ex.Message);
                    }
                });
            }
        }
    }
}