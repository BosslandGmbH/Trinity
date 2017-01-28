using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Combat;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Routines.Crusader;
using Trinity.UI.UIComponents;
using Zeta.Bot;
using Zeta.Common.Xml;
using Zeta.Game;
using Application = System.Windows.Application;
using UserControl = System.Windows.Controls.UserControl;

namespace Trinity.UI
{
    public class UILoader
    {

        private static Dictionary<string, string> _paths;
        private static Dictionary<string, byte[]> _xaml;

        static UILoader()
        {
            _xaml = new Dictionary<string, byte[]>();
            var paths = Directory.GetFiles(FileManager.PluginPath, "*.xaml", SearchOption.AllDirectories);
            _paths = paths.DistinctBy(Path.GetFileName).ToDictionary(k => Path.GetFileName(k)?.ToLower(), v => v);
        }

        public static Window ConfigWindow;
        private static UserControl _windowContent;
        private static readonly object ContentLock = new object();

        public static ConfigViewModel DataContext { get; set; }

        public static void CloseWindow()
        {
            ConfigWindow.Close();
        }

        private static int _currentHeroId;

        public static Window GetDisplayWindow()
        {
            if (!BotEvents.IsBotRunning)
            {
                Logger.LogDebug("GetDisplayWindow: Bot is not running");
                using (new PerformanceLogger("Window Data Load", true))
                {
                    Logger.LogDebug("GetDisplayWindow: AcquireFrame");
                    using (ZetaDia.Memory.AcquireFrame())
                    {
                        Logger.LogDebug("GetDisplayWindow: ZetaDia.Actors.Update");
                        ZetaDia.Actors.Update();
                        Logger.LogDebug("GetDisplayWindow: Actors.Update");
                        Core.Actors.Update();
                        Logger.LogDebug("GetDisplayWindow: Inventory.Update");
                        Core.Inventory.Update();
                        Logger.LogDebug("GetDisplayWindow: Hotbar.Update");
                        Core.Hotbar.Update();
                        Logger.LogDebug("GetDisplayWindow: Routines.SelectRoutine");
                        Core.Routines.SelectRoutine();
                        Logger.LogDebug("GetDisplayWindow: ChangeMonitor.Update");
                        Core.ChangeMonitor.Update();
                        Logger.LogDebug("GetDisplayWindow: UpdateGems");
                        PluginSettings.Current.UpdateGemList();
                    }
                }
            }

            Logger.LogDebug("GetDisplayWindow: Loading Window");
            return GetDisplayWindow(Path.Combine(FileManager.PluginPath, "UI"));
        }

        #region Events

        public delegate void LoaderEvent();

        /// <summary>
        /// Occurs when settings window is open and properly loaded
        /// </summary>
        public static event LoaderEvent OnSettingsWindowOpened = () => { };

        #endregion

        public static Window GetDisplayWindow(string uiPath)
        {
            using (new PerformanceLogger("GetDisplayWindow"))
            {
                try
                {
                    // Check we can actually find the .xaml file first - if not, report an error
                    if (!File.Exists(Path.Combine(uiPath, "MainView.xaml")))
                    {
                        Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "MainView.xaml not found {0}", Path.Combine(uiPath, "MainView.xaml"));
                        return null;
                    }
                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "MainView.xaml found");
                    if (ConfigWindow == null)
                    {
                        ConfigWindow = new Window();
                    }
                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "Load Context");

                    var viewmodel = new ConfigViewModel(Core.Storage, Core.Settings);

                    DataContext = viewmodel;
                    ConfigWindow.DataContext = viewmodel;

                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "Load MainView.xaml");
                    if (_windowContent == null)
                    {
                        LoadWindowContent(uiPath);
                    }
                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "Put MainControl to Window");
                    ConfigWindow.Content = _windowContent;
                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "Configure Window");

                    if (Screen.PrimaryScreen.Bounds.Width <= 1920 || Screen.PrimaryScreen.Bounds.Height <= 1080)
                    {
                        ConfigWindow.Height = 650;
                        ConfigWindow.Width = 550;
                    }
                    else
                    {
                        ConfigWindow.Height = 800;
                        ConfigWindow.Width = 600;
                    }

                    ConfigWindow.MinHeight = 650;
                    ConfigWindow.MinWidth = 500;
                    ConfigWindow.Title = "Trinity";

                    // Event handling for the config window loading up/closing
                    //configWindow.Loaded += configWindow_Loaded;
                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "Set Window Events");
                    ConfigWindow.Closed += WindowClosed;

                    Application.Current.Exit += WindowClosed;

                    ConfigWindow.ContentRendered += (sender, args) => OnSettingsWindowOpened();

                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "Window build finished.");
                }
                catch (XamlParseException ex)
                {
                    Logger.Log(TrinityLogLevel.Error, LogCategory.UI, "{0}", ex);
                    return ConfigWindow;
                }
                return ConfigWindow;
            }
        }

        /// <summary>
        /// Loads the plugin config window XAML asyncronously, for fast re-use later.
        /// </summary>
        internal static void PreLoadWindowContent()
        {
            try
            {
                TrinityPlugin.BeginInvoke(() => LoadWindowContent(Path.Combine(FileManager.PluginPath, "UI")));
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception pre-loadingn window content! " + ex);
            }
        }

        internal static void LoadWindowContent(string uiPath)
        {
            try
            {
                lock (ContentLock)
                {
                    _windowContent = LoadAndTransformXamlFile<UserControl>(Path.Combine(uiPath, "MainView.xaml"));
                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "Load Children");
                    LoadChild(_windowContent, uiPath);
                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "Load Resources");
                    LoadResourceForWindow(Path.Combine(uiPath, "Template.xaml"), _windowContent);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception loading window content! {0}", ex);
            }
        }

        internal static UserControl LoadWindowContent(string uiPath, string filename)
        {
            try
            {
                lock (ContentLock)
                {
                    var content = LoadAndTransformXamlFile<UserControl>(Path.Combine(uiPath, filename));
                    Logger.LogVerbose(LogCategory.UI, "LoadSettings Children");
                    LoadChild(content, uiPath);
                    Logger.LogVerbose(LogCategory.UI, "LoadSettings Resources");
                    LoadResourceForWindow(Path.Combine(uiPath, "Template.xaml"), content);
                    return content;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception loading window content! {0}", ex);
            }

            return null;
        }

        private static void LoadResourceForWindow(string filename, UserControl control)
        {
            try
            {
                using (new PerformanceLogger("LoadResourceForWindow"))
                {
                    ResourceDictionary resource = LoadAndTransformXamlFile<ResourceDictionary>(filename);
                    foreach (System.Collections.DictionaryEntry res in resource)
                    {
                        if (!control.Resources.Contains(res.Key))
                            control.Resources.Add(res.Key, res.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error loading resources {0}", ex);
            }
        }

        internal static T LoadXamlByFileName<T>(string fileName)
        {
            if (fileName != null)
            {
                var fileNameLower = fileName.ToLower();
                if (_paths.ContainsKey(fileNameLower))
                {                    
                    return LoadAndTransformXamlFile<T>(_paths[fileNameLower]);
                }
            }
            return default(T);
        }

        /// <summary>Loads the and transform xaml file.</summary>
        /// <param name="filePath">The absolute path to xaml file.</param>
        /// <returns><see cref="Stream"/> which contains transformed XAML file.</returns>
        internal static T LoadAndTransformXamlFile<T>(string filePath)
        {
            if (_xaml.ContainsKey(filePath))
            {
                return (T)XamlReader.Load(new MemoryStream(_xaml[filePath]));
            }

            try
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "Load XAML file : {0}", filePath);
                string filecontent = File.ReadAllText(filePath);
                var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

                filecontent = filecontent.Replace("xmlns:radarCanvas=\"clr-namespace:Trinity.UI.Visualizer.RadarCanvas\"", "xmlns:radarCanvas=\"clr-namespace:Trinity.UI.Visualizer.RadarCanvas;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:itemlist=\"clr-namespace:Trinity.Settings.ItemList\"", "xmlns:itemlist=\"clr-namespace:Trinity.Settings.ItemList;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:markupExtensions=\"clr-namespace:Trinity.UI.UIComponents.MarkupExtensions\"", "xmlns:markupExtensions=\"clr-namespace:Trinity.UI.UIComponents.MarkupExtensions;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:markup=\"clr-namespace:Trinity.UI.UIComponents.MarkupExtensions\"", "xmlns:markup=\"clr-namespace:Trinity.UI.UIComponents.MarkupExtensions;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:input=\"clr-namespace:Trinity.UI.UIComponents.Input\"", "xmlns:input=\"clr-namespace:Trinity.UI.UIComponents.Input;assembly=" + assemblyName + "\"");            
                filecontent = filecontent.Replace("xmlns:behaviors=\"clr-namespace:Trinity.UI.UIComponents.Behaviors\"", "xmlns:behaviors=\"clr-namespace:Trinity.UI.UIComponents.Behaviors;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:controls=\"clr-namespace:Trinity.UI.UIComponents.Controls\"", "xmlns:controls=\"clr-namespace:Trinity.UI.UIComponents.Controls;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:ut=\"clr-namespace:Trinity.UI.UIComponents\"", "xmlns:ut=\"clr-namespace:Trinity.UI.UIComponents;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:objects=\"clr-namespace:Trinity.Framework.Objects\"", "xmlns:objects=\"clr-namespace:Trinity.Framework.Objects;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:mock=\"clr-namespace:Trinity.Settings.Mock\"", "xmlns:mock=\"clr-namespace:Trinity.Settings.Mock;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:settings=\"clr-namespace:Trinity.Settings\"", "xmlns:settings=\"clr-namespace:Trinity.Settings;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:charts=\"clr-namespace:LineChartLib\"", "xmlns:charts=\"clr-namespace:LineChartLib;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:ut=\"clr-namespace:Trinity.UI.UIComponents\"", "xmlns:ut=\"clr-namespace:Trinity.UI.UIComponents;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:converters=\"clr-namespace:Trinity.UI.UIComponents.Converters\"", "xmlns:converters=\"clr-namespace:Trinity.UI.UIComponents.Converters;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:radarCanvas=\"clr-namespace:Trinity.UI.UIComponents.RadarCanvas\"", "xmlns:radarCanvas=\"clr-namespace:Trinity.UI.UIComponents.RadarCanvas;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:ui=\"clr-namespace:Trinity.UI.UIComponents\"", "xmlns:ui=\"clr-namespace:Trinity.UI.UIComponents;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:dd=\"clr-namespace:GongSolutions.Wpf.DragDrop\"", "xmlns:dd=\"clr-namespace:GongSolutions.Wpf.DragDrop;assembly=" + assemblyName + "\"");
                filecontent = filecontent.Replace("xmlns:enums=\"clr-namespace:Trinity.Framework.Objects.Enums\"", "xmlns:enums=\"clr-namespace:Trinity.Framework.Objects.Enums;assembly=" + assemblyName + "\"");
                filecontent = Regex.Replace(filecontent, "<ResourceDictionary.MergedDictionaries>.*</ResourceDictionary.MergedDictionaries>", string.Empty, RegexOptions.Singleline | RegexOptions.Compiled);

                var bytes = Encoding.UTF8.GetBytes(filecontent);
                _xaml[filePath] = bytes;
                return (T)XamlReader.Load(new MemoryStream(bytes));
         
            }
            catch (Exception ex)
            {
                Logger.LogError("Error loading/transforming XAML {0}", ex);
            }
            return default(T);
        }

        /// <summary>Call when Config Window is closed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        static void WindowClosed(object sender, EventArgs e)
        {
            Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "Window closed.");
            ConfigWindow = null;
        }

        /// <summary>Loads recursivly the child in ContentControl or Decorator with Tag.</summary>
        /// <param name="parentControl">The parent control.</param>
        /// <param name="uiPath">The UI path.</param>
        private static void LoadChild(FrameworkElement parentControl, string uiPath)
        {
            try
            {
                // Loop in Children of parent control of type FrameworkElement 
                foreach (FrameworkElement ctrl in LogicalTreeHelper.GetChildren(parentControl).OfType<FrameworkElement>())
                {
                    string contentName = ctrl.Tag as string;
                    // Tag contains a string end with ".xaml" : It's dymanic content 
                    if (!string.IsNullOrWhiteSpace(contentName) && contentName.EndsWith(".xaml"))
                    {
                        // combine and handle relative '..\' in path.
                        var dirtyFullPath = Path.Combine(uiPath, contentName); 
                        var cleanFullPath = new Uri(dirtyFullPath).LocalPath;

                        // Load content from XAML file
                        LoadDynamicContent(uiPath, ctrl, cleanFullPath);
                    }
                    else
                    {
                        // Try again with children of control
                        LoadChild(ctrl, uiPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception loading child {0}", ex);
            }
        }

        /// <summary>Loads the dynamic content from XAML file.</summary>
        /// <param name="uiPath">The UI path.</param>
        /// <param name="ctrl">The CTRL.</param>
        /// <param name="filename">Name of the content.</param>
        private static void LoadDynamicContent(string uiPath, FrameworkElement ctrl, string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    UserControl xamlContent = LoadAndTransformXamlFile<UserControl>(filename);

                    // Dynamic load of content is possible on Content control (UserControl, ...)
                    if (ctrl is ContentControl)
                    {
                        ((ContentControl)ctrl).Content = xamlContent;
                    }
                    // Or on Decorator control (Border, ...)
                    else if (ctrl is Decorator)
                    {
                        ((Decorator)ctrl).Child = xamlContent;
                    }
                    // Otherwise, log control where you try to put dynamic tag
                    else
                    {
                        Logger.Log(TrinityLogLevel.Verbose, LogCategory.UI, "Control of type '{0}' can't be used for dynamic loading.", ctrl.GetType().FullName);
                        return;
                    }
                    // Content added to parent control, try to search dynamic control in children
                    LoadChild(xamlContent, uiPath);
                }
                else
                {
                    Logger.Log(TrinityLogLevel.Error, LogCategory.UI, "Error XAML file not found : '{0}'", filename);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception loading Dynamic Content {0}", ex);
            }
        }

        public static Window CreateNonModalWindow(string xamlRelativePath, string title, object dataContext, int width = 500, int height = 300)
        {
            Window window = null;

            try
            {
                var path = Path.Combine(FileManager.UiPath, xamlRelativePath);

                if (!File.Exists(path))
                {
                    Logger.LogError("File Not Found: {0}", path);
                    return null;
                }

                var ownerWindow = DemonBuddyUI.MainWindow;

                window = new Window
                {
                    DataContext = dataContext,
                    Content = LoadWindowContent(FileManager.UiPath, xamlRelativePath),
                    MinHeight = 400,
                    MinWidth = 200,
                    Title = title,
                    ResizeMode = ResizeMode.CanResizeWithGrip,  
                    //SizeToContent = SizeToContent.WidthAndHeight,
                    SnapsToDevicePixels = true,
                    Topmost = false,
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    WindowStyle = WindowStyle.SingleBorderWindow,
                    Owner = null,
                };

                //.Location = new System.Drawing.Point(form.Owner.Location.X + (form.Owner.Width – form.Width) / 2, form.Owner.Location.Y + (form.Owner.Height – form.Height) / 2);

                var offset = GetBorderFrameOffset(window);
                window.Height = height + offset.Height;
                window.Width = width + offset.Width;
                window.Top = Math.Max(30, (ownerWindow.Top + ownerWindow.Height / 2) - (window.Height / 2));
                window.Left = Math.Max(30, (ownerWindow.Left + ownerWindow.Width / 2) - (window.Width / 2));

                window.Closed += (s, e) =>
                {
                    var context = dataContext as XmlSettings;
                    if (context != null)
                    {
                        context.Save();
                    }
                };

                ownerWindow.Closed += (s, e) =>
                {
                    window.Close();
                    window = null;
                };

                Application.Current.Exit += (s, e) =>
                {
                    window.Close();
                    window = null;
                };

                //ownerWindow.ContentRendered += (sender, args) => OnModalWindowOpened();               
            }
            catch (XamlParseException ex)
            {
                Logger.LogError("XamlParseException loading {0} {1} {2}", title, xamlRelativePath, ex);
            }

            return window;
        }

        public static Size GetBorderFrameOffset(Window window)
        {
            var result = new Size();

            if (window.ResizeMode == ResizeMode.CanResize || window.ResizeMode == ResizeMode.CanResizeWithGrip)
            {
                result.Height += System.Windows.Forms.SystemInformation.FrameBorderSize.Height * 2;
                result.Width += System.Windows.Forms.SystemInformation.FrameBorderSize.Width * 2;
            }

            if (window.WindowStyle == WindowStyle.SingleBorderWindow || window.WindowStyle == WindowStyle.ThreeDBorderWindow)
            {
                result.Height += System.Windows.Forms.SystemInformation.CaptionHeight;
            }

            if (window.WindowStyle == WindowStyle.ToolWindow)
            {
                result.Height += System.Windows.Forms.SystemInformation.ToolWindowCaptionHeight;
            }

            return result;
        }

        public static void ExtendDimensionsForBorderFrame(Window window)
        {
            var size = GetBorderFrameOffset(window);
            window.Width = window.Width + size.Width;
            window.Height = window.Height + size.Height;
        }


    }
}
