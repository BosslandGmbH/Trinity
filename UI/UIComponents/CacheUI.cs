using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Demonbuddy;
using Trinity.Cache;
using Trinity.Combat.Abilities;
using Trinity.Helpers;
using Trinity.Technicals;
using Trinity.UIComponents;
using Zeta.Bot;
using Zeta.Bot.Dungeons;
using Zeta.Bot.Profile.Common;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Application = System.Windows.Application;
using Logger = Trinity.Technicals.Logger;
using UserControl = System.Windows.Controls.UserControl;

namespace Trinity.UI.UIComponents
{
    public class CacheUI
    {
        private static bool _isUpdating;
        private static CacheUIDataModel _dataModel;
        private static DateTime _lastUpdatedDefault = DateTime.MinValue;
        private static DateTime _lastUpdatedLazy = DateTime.MinValue;
        private static DispatcherTimer _internalTimer;
        private static ThreadedWindow _cacheWindow;
        private static ThreadedWindow _radarWindow;

        public static void CreateWindow()
        {
            if (_dataModel == null)
            {
                _dataModel = new CacheUIDataModel();
                _dataModel.PropertyChanged += DataModelOnPropertyChanged;
                Configuration.Events.OnCacheUpdated += Update;
            }

            if (_cacheWindow == null)
            {
                Logger.Log("Loading CacheUI");
                var cacheXamlPath = Path.Combine(FileManager.PluginPath, "UI", "CacheUI.xaml");
                _cacheWindow = new ThreadedWindow(cacheXamlPath, _dataModel, "CacheUI", 750, 1200);
            }

            _cacheWindow.Show();

            CreateRadarWindow();

            _internalTimer = new DispatcherTimer();
            _internalTimer.Tick += InternalTimerTick;
            _internalTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            _internalTimer.Start();
        }

        private static void InternalTimerTick(object sender, EventArgs e)
        {
            if (!_cacheWindow.IsWindowOpen || !_dataModel.Enabled || (!_dataModel.IsLazyCacheVisible && !_dataModel.IsDefaultVisible))
                return;

            if (BotMain.IsRunning)
            {
                Update();
            }
            else
            {                
                using (ZetaDia.Memory.AcquireFrame(true))
                {
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        ZetaDia.Actors.Update();

                        if (_dataModel.IsDefaultVisible)
                            Trinity.RefreshDiaObjectCache();

                        Update();

                    });

                }
            }
        }

        private static void CreateRadarWindow()
        {
            if (_radarWindow == null)
            {
                Logger.Log("Loading RadarUI");
                var radarXamlPath = Path.Combine(FileManager.PluginPath, "UI", "RadarUI.xaml");
                _radarWindow = new ThreadedWindow(radarXamlPath, _dataModel, "RadarUI", 600, 600);

                _dataModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "IsRadarWindowVisible")
                    {
                        if (_dataModel.IsRadarWindowVisible)
                        {
                            _radarWindow.Show();
                        }
                        else
                        {
                            _radarWindow.Hide();
                        }
                    }
                };

                _radarWindow.OnHidden += () =>
                {
                    _dataModel.IsRadarWindowVisible = false;
                };
            }
        }

        private static void DataModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {

        }

        private static void Update()
        {
            try
            {
                using (new PerformanceLogger("CacheUI Update"))
                {
                    if (_isUpdating)
                        return;

                    if (_dataModel.IsDefaultVisible && DateTime.UtcNow.Subtract(_lastUpdatedDefault).TotalMilliseconds > 250)
                    {
                        _isUpdating = true;
                        _dataModel.Cache = new ObservableCollection<CacheUIObject>(GetCacheActorList());
                        _lastUpdatedDefault = DateTime.UtcNow;
                    }

                    if (_dataModel.IsLazyCacheVisible && DateTime.UtcNow.Subtract(_lastUpdatedLazy).TotalMilliseconds > 50)
                    {
                        _isUpdating = true;                        
                        _lastUpdatedLazy = DateTime.UtcNow;
                    }

                    _isUpdating = false;
                }
            }
            catch (Exception ex)
            {
                _isUpdating = false;
                Logger.LogError("Error in CacheUI Worker: " + ex);
            }
        }


        public static List<CacheUIObject> GetCacheActorList()
        {
            using (new PerformanceLogger("CacheUI DefaultActorList"))
            {
                return ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                    .Where(i => i.IsFullyValid())
                    .Select(o => new CacheUIObject(o))
                    .OrderByDescending(o => o.InCache)
                    .ThenByDescending(o => o.Weight)
                    .ThenBy(o => o.Distance)
                    .ToList();
            }
        }

    }

}
