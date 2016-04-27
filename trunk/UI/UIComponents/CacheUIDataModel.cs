using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using Trinity.Cache;
using Trinity.UI.UIComponents;
using Trinity.UIComponents;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.UIComponents
{
    public class CacheUIDataModel : INotifyPropertyChanged
    {
        public CacheUIDataModel()
        {
        }

        private bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set { SetField(ref _enabled, value); }
        }

        private bool _isDefaultVisible;
        public bool IsDefaultVisible
        {
            get { return _isDefaultVisible; }
            set { SetField(ref _isDefaultVisible, value); }
        }

        private bool _isLazyCacheVisible;
        public bool IsLazyCacheVisible
        {
            get { return _isLazyCacheVisible; }
            set { SetField(ref _isLazyCacheVisible, value); }
        }

        private ObservableCollection<CacheUIObject> _cache = new ObservableCollection<CacheUIObject>();
        public ObservableCollection<CacheUIObject> Cache
        {
            get { return _cache; }
            set { SetField(ref _cache, value); }
        }

        private ObservableCollection<ChartDatum> _cacheUpdateTime = new ObservableCollection<ChartDatum>();
        public ObservableCollection<ChartDatum> CacheUpdateTime
        {
            get
            {
                return Application.Current.Dispatcher.Invoke(() => _cacheUpdateTime);
            }
            set { SetField(ref _cacheUpdateTime, value); }
        }

        private ObservableCollection<ChartDatum> _weightUpdateTime = new ObservableCollection<ChartDatum>();
        public ObservableCollection<ChartDatum> WeightUpdateTime
        {
            get
            {
                return Application.Current.Dispatcher.Invoke(() => _weightUpdateTime);
            }
            set { SetField(ref _weightUpdateTime, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public struct ChartDatum
        {
            public ChartDatum(DateTime dateTime, double value) : this()
            {
                DateTime = dateTime;
                Value = value;
            }

            public DateTime DateTime { set; get; }
            public double Value { set; get; }
        }

        public ICommand LaunchRadarUICommand { set; get; }

        public ICommand CopyCommand
        {
            get
            {
                return new RelayCommand(param =>
                {
                    var objectToString = param.ToString();
                    Clipboard.SetText(objectToString);
                    Logger.Log("Copied to Clipboard: {0}", objectToString);
                });
            }
        }

        private bool _isRadarWindowVisible;
        public bool IsRadarWindowVisible
        {
            get { return _isRadarWindowVisible; }
            set { SetField(ref _isRadarWindowVisible, value); }
        }


    }

}
