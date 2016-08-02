using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Trinity.Components.Adventurer.UI.UIComponents;
using Trinity.Components.Adventurer.Util;

namespace Trinity.Components.Adventurer.UI
{
    public class MapWindow : Window
    {
        public MapWindow()
        {
            Height = 580;
            Width = 700;
            try
            {
                Content = UILoader.LoadAndTransformXamlFile<UserControl>(Path.Combine(FileUtils.PluginPath, "UI", "MapUI.xaml")); ;
            }
            catch (Exception)
            {
                Content = UILoader.LoadAndTransformXamlFile<UserControl>(Path.Combine(FileUtils.PluginPath2, "UI", "MapUI.xaml")); ;
            }
            Title = "MapUI Powered by Trinity";
        }
        
    }
}
