using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Demonbuddy;

namespace Trinity.Framework.Helpers
{
    public static class DemonBuddyUI
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

        private static Button _startButton;
        public static Button StartButton
        {
            get { return _startButton ?? (_startButton = FindStartButton()); }
            set { _startButton = value; }
        }

        private static SplitButton FindSettingsButton()
        {
            return GetElement<SplitButton>(MainWindow.FindName("btnSettings"));
        }

        private static Button FindStartButton()
        {
            return GetElement<Button>(MainWindow.FindName("btnStart"));
        }

        public static T GetElement<T>(object element) where T : class, new()
        {
            return element is T ? (T) element : new T();
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);

            if (parent is T)
                return parent as T;

            return FindParent<T>(parent);
        }

        private static FieldInfo GetEventField(this Type type, string eventName)
        {
            FieldInfo field = null;
            while (type != null)
            {
                /* Find events defined as field */
                field = type.GetField(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null && (field.FieldType == typeof(MulticastDelegate) || field.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                    break;

                /* Find events defined as property { add; remove; } */
                field = type.GetField("EVENT_" + eventName.ToUpper(), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    break;
                type = type.BaseType;
            }
            return field;
        }

    }
}
