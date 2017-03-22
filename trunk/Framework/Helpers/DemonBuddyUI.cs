using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Demonbuddy;
using Zeta.Bot;

namespace Trinity.Framework.Helpers
{
    public static class DemonbuddyUI
    {
        private static TabControl _mainMenu;
        public static TabControl MainMenu
        {
            get { return _mainMenu ?? (_mainMenu = FindMainMenu()); }
            set { _mainMenu = value; }
        }

        private static TabControl FindMainMenu()
        {
            return GetElement<TabControl>(MainWindow.FindName("tabControlMain"));
        }

        private static Version _productVersion;
        public static Version ProductVersion
        {
            get { return _productVersion ?? (_productVersion = new Version(System.Windows.Forms.Application.ProductVersion)); }
            set { _productVersion = value; }
        }

        private static Version _fileVersion;
        public static Version FileVersion
        {
            get { return _fileVersion ?? (_fileVersion = new Version(System.Windows.Forms.Application.ProductVersion)); }
            set { _fileVersion = value; }
        }

        private static MainWindow _mainWindow;
        public static MainWindow MainWindow
        {
            get { return _mainWindow ?? (_mainWindow = FindMainWindow()); }
            set { _mainWindow = value; }
        }

        private static MainWindow FindMainWindow()
        {
            return GetElement<MainWindow>(Application.Current.MainWindow);
        }

        private static StatusBar _statusBar;
        public static StatusBar StatusBar
        {
            get { return _statusBar ?? (_statusBar = FindStatusBar()); }
            set { _statusBar = value; }
        }

        private static StatusBar FindStatusBar()
        {
            return FindParent<StatusBar>(StatusBarText);            
        }

        private static StatusBarItem _statusBarText;
        public static StatusBarItem StatusBarText
        {
            get { return _statusBarText ?? (_statusBarText = FindStatusBarText()); }
            set { _statusBarText = value; }
        }

        private static StatusBarItem FindStatusBarText()
        {
            return GetElement<StatusBarItem>(MainWindow.FindName("lblActivityText"));
        }

        private static SplitButton _settingsButton;
        public static SplitButton SettingsButton
        {
            get { return _settingsButton ?? (_settingsButton = FindSettingsButton()); }
            set { _settingsButton = value; }
        }

        private static SplitButton FindSettingsButton()
        {
            return GetElement<SplitButton>(MainWindow.FindName("btnSettings"));
        }      

        public static T GetElement<T>(object element) where T : class, new()
        {
            return element is T ? element as T : new T();
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);

            if (parent is T)
                return parent as T;

            return FindParent<T>(parent);
        }

    }    
}
