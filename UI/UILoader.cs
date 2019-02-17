using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using Serilog;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Settings;
using Trinity.UI.UIComponents;
using Zeta.Common;
using Zeta.Common.Xml;
using Zeta.Game;
using Application = System.Windows.Application;
using UserControl = System.Windows.Controls.UserControl;

namespace Trinity.UI
{
    public class UILoader
    {
        private static readonly ILogger s_logger = Logger.GetLoggerInstanceForType();
        private static readonly ConcurrentDictionary<string, string> _paths = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, byte[]> _xaml = new ConcurrentDictionary<string, byte[]>();

        static UILoader()
        {
            if (Directory.Exists(FileManager.PluginPath))
            {
                var paths = Directory.GetFiles(FileManager.PluginPath, "*.xaml", SearchOption.AllDirectories);
                var pairs = paths.DistinctBy(Path.GetFileName)
                    .Select(p => new KeyValuePair<string, string>(Path.GetFileName(p)?.ToLower(), p));
                _paths = new ConcurrentDictionary<string, string>(pairs);
            }
        }

        public static void Preload()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            Task.Run(() => Parallel.ForEach(_paths, item => MyTask(item.Value, assemblyName)));
        }

        private static void MyTask(string path, string assm)
        {
            var filecontent = File.ReadAllText(path);
            filecontent = resx.Replace(filecontent, string.Empty);
            filecontent = xmlns.Replace(filecontent, "$1;assembly=" + assm + "\"");
            _xaml.TryAdd(path, Encoding.UTF8.GetBytes(filecontent));
        }

        internal static void PreLoadResources()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() => LoadWindowContent(Path.Combine(FileManager.PluginPath, "UI"))));
        }

        public static Window ConfigWindow;
        private static UserControl _windowContent;
        private static readonly object ContentLock = new object();

        public static ConfigViewModel DataContext { get; set; }

        public static void CloseWindow()
        {
            ConfigWindow.Close();
        }

        public static Window GetDisplayWindow()
        {
            if (!BotEvents.IsBotRunning)
            {
                s_logger.Debug("GetDisplayWindow: Bot is not running");
                using (new PerformanceLogger("Window Data Load", true))
                {
                    s_logger.Debug("GetDisplayWindow: AcquireFrame");
                    using (ZetaDia.Memory.AcquireFrame())
                    {
                        s_logger.Debug("GetDisplayWindow: ZetaDia.Actors.Update");
                        ZetaDia.Actors.Update();
                        s_logger.Debug("GetDisplayWindow: Actors.Update");
                        Core.Actors.Update();
                        s_logger.Debug("GetDisplayWindow: Inventory.Update");
                        Core.Inventory.Update();
                        s_logger.Debug("GetDisplayWindow: Hotbar.Update");
                        Core.Hotbar.Update();
                        s_logger.Debug("GetDisplayWindow: Routines.SelectRoutine");
                        Core.Routines.SelectRoutine();
                        s_logger.Debug("GetDisplayWindow: ChangeMonitor.Update");
                        Core.ChangeMonitor.Update();
                        s_logger.Debug("GetDisplayWindow: UpdateGems");
                        PluginSettings.Current.UpdateGemList();
                    }
                }
            }

            s_logger.Debug("GetDisplayWindow: Loading Window");
            return GetDisplayWindow(Path.Combine(FileManager.PluginPath, "UI"));
        }

        #region Events

        public delegate void LoaderEvent();

        /// <summary>
        /// Occurs when settings window is open and properly loaded
        /// </summary>
        public static event LoaderEvent OnSettingsWindowOpened = () => { };

        #endregion Events

        public static Window GetDisplayWindow(string uiPath)
        {
            using (new PerformanceLogger("GetDisplayWindow"))
            {
                try
                {
                    // Check we can actually find the .xaml file first - if not, report an error
                    if (!File.Exists(Path.Combine(uiPath, "MainView.xaml")))
                    {
                        s_logger.Verbose("MainView.xaml not found {0}", Path.Combine(uiPath, "MainView.xaml"));
                        return null;
                    }
                    s_logger.Verbose("MainView.xaml found");
                    if (ConfigWindow == null)
                    {
                        ConfigWindow = new Window();
                    }
                    s_logger.Verbose("Load Context");

                    var viewmodel = new ConfigViewModel(Core.Storage, Core.Settings);

                    DataContext = viewmodel;
                    ConfigWindow.DataContext = viewmodel;

                    s_logger.Verbose("Load MainView.xaml");
                    if (_windowContent == null)
                    {
                        LoadWindowContent(uiPath);
                    }
                    s_logger.Verbose("Put MainControl to Window");
                    ConfigWindow.Content = _windowContent;
                    s_logger.Verbose("Configure Window");

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
                    s_logger.Verbose("Set Window Events");
                    ConfigWindow.Closed += WindowClosed;

                    Application.Current.Exit += WindowClosed;

                    ConfigWindow.ContentRendered += (sender, args) => OnSettingsWindowOpened();

                    s_logger.Verbose("Window build finished.");
                }
                catch (XamlParseException ex)
                {
                    s_logger.Error("{0}", ex);
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
                Application.Current.Dispatcher.BeginInvoke((Action)(() => LoadWindowContent(Path.Combine(FileManager.PluginPath, "UI"))));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception pre-loadingn window content!");
            }
        }

        internal static void LoadWindowContent(string uiPath)
        {
            try
            {
                lock (ContentLock)
                {
                    _windowContent = LoadAndTransformXamlFile<UserControl>(Path.Combine(uiPath, "MainView.xaml"));
                    s_logger.Verbose("Load Children");
                    LoadChild(_windowContent, uiPath);
                    s_logger.Verbose("Load Resources");
                    LoadResourceForWindow(Path.Combine(uiPath, "Template.xaml"), _windowContent);
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception loading window content!");
            }
        }

        internal static UserControl LoadWindowContent(string uiPath, string filename)
        {
            try
            {
                lock (ContentLock)
                {
                    var content = LoadAndTransformXamlFile<UserControl>(Path.Combine(uiPath, filename));
                    s_logger.Verbose("LoadSettings Children");
                    LoadChild(content, uiPath);
                    s_logger.Verbose("LoadSettings Resources");
                    LoadResourceForWindow(Path.Combine(uiPath, "Template.xaml"), content);
                    return content;
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception loading window content!");
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
                s_logger.Error(ex, "Error loading resources");
            }
        }

        internal static T LoadXamlByFileName<T>(string fileName) where T : UserControl
        {
            if (fileName != null)
            {
                var fileNameLower = fileName.ToLower();
                if (_paths.ContainsKey(fileNameLower))
                {
                    var path = _paths[fileNameLower];

                    //UserControl control;
                    //if (_controls.TryGetValue(path, out control) && control != null)
                    //{
                    //    return (T)_controls[path];
                    //}

                    return LoadAndTransformXamlFile<T>(path);
                }
            }
            return default(T);
        }

        //internal static UserControl LoadAndTransformUserControlXaml(string filePath)
        //{
        //    try
        //    {
        //        Core.Logger.Verbose(LogCategory.UI, "Load XAML file : {0}", filePath);
        //        string filecontent = File.ReadAllText(filePath);

        //        if (string.Concat(filecontent.Skip(1).Take(11)) != "UserControl")
        //            return null;

        //        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        //        filecontent = resx.Replace(filecontent, string.Empty);
        //        filecontent = xmlns.Replace(filecontent, "$1;assembly=" + assemblyName + "\"");

        //        var bytes = Encoding.UTF8.GetBytes(filecontent);

        //        _xaml[filePath] = bytes;

        //        using (var stream = new MemoryStream(bytes))
        //        {
        //            return (UserControl)XamlReader.Load(stream);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Core.Logger.Error("Error loading/transforming XAML {0}", ex);
        //    }
        //    return null;
        //}

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
                s_logger.Verbose("Load XAML file : {filePath}", filePath);
                string filecontent = File.ReadAllText(filePath);
                var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                filecontent = resx.Replace(filecontent, string.Empty);
                filecontent = xmlns.Replace(filecontent, "$1;assembly=" + assemblyName + "\"");
                var bytes = Encoding.UTF8.GetBytes(filecontent);
                _xaml[filePath] = bytes;
                using (var stream = new MemoryStream(bytes))
                {
                    return (T)XamlReader.Load(stream);
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Error loading/transforming XAML");
            }
            return default(T);
        }

        private static Regex xmlns { get; } = new Regex("(xmlns:.+?=\\\"clr-namespace:Trinity[\\W\\S][^\\\"]+)\"", RegexOptions.Compiled);
        private static Regex resx { get; } = new Regex("<ResourceDictionary.MergedDictionaries>.*</ResourceDictionary.MergedDictionaries>", RegexOptions.Singleline | RegexOptions.Compiled);
        public static object Parralel { get; private set; }

        /// <summary>Call when Config Window is closed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void WindowClosed(object sender, EventArgs e)
        {
            s_logger.Verbose("Window closed.");
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
                s_logger.Error(ex, "Exception loading child {uiPath}", uiPath);
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
                    if (ctrl is ContentControl control)
                    {
                        control.Content = xamlContent;
                    }
                    // Or on Decorator control (Border, ...)
                    else if (ctrl is Decorator decorator)
                    {
                        decorator.Child = xamlContent;
                    }
                    // Otherwise, log control where you try to put dynamic tag
                    else
                    {
                        s_logger.Verbose("Control of type '{FullName}' can't be used for dynamic loading.", ctrl.GetType().FullName);
                        return;
                    }
                    // Content added to parent control, try to search dynamic control in children
                    LoadChild(xamlContent, uiPath);
                }
                else
                {
                    s_logger.Error("Error XAML file not found : '{filename}'", filename);
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception loading Dynamic Content");
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
                    s_logger.Error("File Not Found: {path}", path);
                    return null;
                }

                var ownerWindow = Application.Current.MainWindow;

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
                    if (dataContext is XmlSettings context)
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
                s_logger.Error(ex, "XamlParseException loading {title} {xamlRelativePath}", title, xamlRelativePath);
            }

            return window;
        }

        public static Size GetBorderFrameOffset(Window window)
        {
            var result = new Size();

            if (window.ResizeMode == ResizeMode.CanResize || window.ResizeMode == ResizeMode.CanResizeWithGrip)
            {
                result.Height += SystemInformation.FrameBorderSize.Height * 2;
                result.Width += SystemInformation.FrameBorderSize.Width * 2;
            }

            if (window.WindowStyle == WindowStyle.SingleBorderWindow || window.WindowStyle == WindowStyle.ThreeDBorderWindow)
            {
                result.Height += SystemInformation.CaptionHeight;
            }

            if (window.WindowStyle == WindowStyle.ToolWindow)
            {
                result.Height += SystemInformation.ToolWindowCaptionHeight;
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
