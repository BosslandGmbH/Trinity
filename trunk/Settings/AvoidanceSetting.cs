//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Runtime.Serialization;
//using System.Windows;
//using System.Windows.Input;

//using JetBrains.Annotations;
//using Trinity.Config.Combat;
//using Trinity.Framework;
//using Trinity.Framework.Avoidance;
//using Trinity.Framework.Avoidance.Handlers;
//using Trinity.Framework.Avoidance.Structures;
//using Trinity.Helpers;
//using Trinity.Settings.Loot;
//using Trinity.Technicals;
//using Trinity.UIComponents;
//using Extensions = Zeta.Common.Extensions;

//namespace Trinity.Config
//{
//    [DataContract(Namespace = "", Name = "Avoidance")]
//    public class AvoidanceDataSettingViewModel : NotifyBase
//    {
//        private bool _isEnabled;
//        private string _name;
//        private IAvoidanceHandler _handler;

//        public AvoidanceDataSettingViewModel()
//        {
//        }

//        public AvoidanceDataSettingViewModel(IAvoidanceSetting a)
//        {
//            this.Name = a.Name;
//            this.IsEnabled = a.IsEnabled;
//            this.Handler = a.Handler;
//        }

//        [DataMember]
//        public string Name
//        {
//            get { return _name; }
//            set { SetField(ref _name, value); }
//        }

//        [DataMember]
//        [DefaultValue(true)]
//        public bool IsEnabled
//        {
//            get { return _isEnabled; }
//            set { SetField(ref _isEnabled, value); }
//        }

//        [DataMember(IsRequired = false)]
//        public IAvoidanceHandler Handler
//        {
//            get { return _handler; }
//            set { SetField(ref _handler, value); }
//        }

//        public void CopyTo(IAvoidanceSetting a)
//        {
//            a.Name = this.Name;
//            a.IsEnabled = this.IsEnabled;

//            if (this.Handler != null)
//                a.Handler = this.Handler;
//        }

//        public override void LoadDefaults()
//        {
//            base.LoadDefaults();
//            if (Handler != null)
//            {
//                Handler.LoadDefaults();
//                OnPropertyChanged(nameof(Handler));
//            }
//        }

//        [OnDeserializing]
//        internal void OnDeserializingMethod(StreamingContext context)
//        {
//            LoadDefaults();
//        }

//        public override string ToString()
//        {
//            return base.ToString() + $": {Name}, {Handler?.GetType()}";
//        }
//    }

//    [DataContract(Namespace = "")]
//    public class AvoidanceSetting : NotifyBase, ITrinitySetting<AvoidanceSetting>, ITrinitySettingEvents
//    {
//        private FullyObservableCollection<AvoidanceDataSettingViewModel> _avoidances;
//        private float _avoiderNearbyPctAvgTrigger;
//        private float _avoiderLocalRadius;
//        private int _minimumHighestNodeWeightTrigger;
//        private float _minimumNearbyWeightPctTotalTrigger;
//        private int _selectedTabIndex;
//        private WeightingOptions _weightingOptions;
//        private float _kiteDistance;
//        private int _kiteWeight;
//        private KiteMode _kiteMode;
//        private int _kiteStutterDelay;
//        private int _kiteHealth;
//        private int _kiteStutterDuration;
//        private bool _pathAroundAvoidance;
//        private bool _avoidOutsideCombat;
//        private bool _dontAvoidWhenBlocked;
//        private bool _onlyAvoidWhileInGrifts;

//        public event PropertyChangedEventHandler PropertyChanged;

//        public AvoidanceSetting()
//        {
//            TrinitySetting.Reset(this);
//            Avoidances = new FullyObservableCollection<AvoidanceDataSettingViewModel>();
//        }

//        [IgnoreDataMember]
//        public Tab SelectedTab => (Tab)SelectedTabIndex;

//        public int SelectedTabIndex
//        {
//            get { return _selectedTabIndex; }
//            set { SetField(ref _selectedTabIndex, value); }
//        }

//        public enum Tab
//        {
//            GroundEffects,
//            MonsterAbilities,
//            Kiting,
//            Misc,
//        }

//        [DataMember]
//        [DefaultValue(1)]
//        public int MinimumHighestNodeWeightTrigger { get; set; } = 1;

//        [DataMember]
//        [DefaultValue(3f)]
//        public float MinimumNearbyWeightPctTotalTrigger { get; set; } = 3f;

//        [DataMember]
//        [DefaultValue(15f)]
//        public float AvoiderLocalRadius { get; set; } = 15f;
//        [DataMember]
//        [DefaultValue(0.2f)]
//        public float AvoiderNearbyPctAvgTrigger { get; set; } = 0.2f;

//        [DataMember]
//        public FullyObservableCollection<AvoidanceDataSettingViewModel> Avoidances
//        {
//            get { return _avoidances; }
//            set { SetField(ref _avoidances, value); }
//        }

//        [DataMember]
//        [DefaultValue(WeightingOptions.Backtrack | WeightingOptions.Globes | WeightingOptions.AdjacentSafe |
//                      WeightingOptions.AvoidanceCentroid | WeightingOptions.Kiting | WeightingOptions.MonsterCentroid |
//                      WeightingOptions.Monsters | WeightingOptions.Obstacles)]
//        public WeightingOptions WeightingOptions { get; set; } = WeightingOptions.Backtrack | WeightingOptions.Globes | WeightingOptions.AdjacentSafe |
//                                                                 WeightingOptions.AvoidanceCentroid | WeightingOptions.Kiting | WeightingOptions.MonsterCentroid |
//                                                                 WeightingOptions.Monsters | WeightingOptions.Obstacles;

//        [DataMember]
//        [DefaultValue(KiteMode.Never)]
//        public KiteMode KiteMode
//        {
//            get { return _kiteMode; }
//            set { SetField(ref _kiteMode, value); }
//        }

//        [DataMember]
//        [DefaultValue(15f)]
//        public float KiteDistance
//        {
//            get { return _kiteDistance; }
//            set { SetField(ref _kiteDistance, value); }
//        }

//        [DataMember]
//        [DefaultValue(100)]
//        public int KiteHealth
//        {
//            get { return _kiteHealth; }
//            set { SetField(ref _kiteHealth, value); }
//        }

//        [DataMember]
//        [DefaultValue(1)]
//        public int KiteWeight { get; set; } = 1;

//        [DataMember]
//        [DefaultValue(1000)]
//        public int KiteStutterDelay = 1000;

//        [DataMember]
//        [DefaultValue(550)]
//        public int KiteStutterDuration { get; set; } = 550;

//        [DataMember]
//        [DefaultValue(true)]
//        public bool PathAroundAvoidance { get; set; } = true;

//        [DataMember]
//        [DefaultValue(true)]
//        public bool AvoidOutsideCombat { get; set; } = true;

//        [DataMember]
//        [DefaultValue(true)]
//        public bool DontAvoidWhenBlocked { get; set; } = true;

//        [DataMember]
//        [DefaultValue(false)]
//        public bool OnlyAvoidWhileInGrifts { get; set; } = false;

//        public void OnSave()
//        {
//            Logger.Log("Saving Avoidance Data");
//            ApplyUserSettingsToDataFactory();
//        }

//        public void OnLoaded()
//        {
//            Logger.Log("Loading Avoidance Data");
//            LoadSettingsFromDataFactory();
//        }

//        public void ApplyUserSettingsToDataFactory()
//        {
//            var data = AvoidanceDataFactory.AvoidanceData.ToDictionary(k => k.Name, v => v);

//            foreach (var avoidanceSetting in Avoidances.ToList())
//            {
//                AvoidanceData def;
//                if (avoidanceSetting?.Name != null && avoidanceSetting.Handler != null && data.TryGetValue(avoidanceSetting.Name, out def))
//                {
//                    avoidanceSetting.CopyTo(def);
//                }
//                else
//                {
//                    Avoidances.Remove(avoidanceSetting);
//                }
//            }
//        }

//        private void LoadSettingsFromDataFactory(bool reset = false)
//        {
//            Application.Current.Dispatcher.Invoke(() =>
//            {
//                var data = Avoidances.ToDictionary(k => k.Name, v => v);

//                foreach (var def in AvoidanceDataFactory.AvoidanceData)
//                {
//                    if (def?.Name == null || def.Handler == null)
//                        continue;

//                    AvoidanceDataSettingViewModel setting;

//                    if (data.TryGetValue(def.Name, out setting))
//                    {
//                        if (setting.Handler == null)
//                        {
//                            // Handler name was invalid but other settings exist.
//                            setting.Handler = def.Handler;
//                            (def.Handler as NotifyBase)?.LoadDefaults();
//                        }
//                        setting.CopyTo(def);
//                    }
//                    else
//                    {
//                        if (reset)
//                            (def.Handler as NotifyBase)?.LoadDefaults();

//                        var newAvoidance = new AvoidanceDataSettingViewModel(def);

//                        if (!data.Any()) // Xml Settings have been loaded
//                            newAvoidance.Handler.LoadDefaults();

//                        if (def.IsEnabledByDefault)
//                            newAvoidance.IsEnabled = true;
//                        Avoidances.Add(newAvoidance);
//                    }
//                }

//            });
//        }

//        public ICommand SelectAllCommand
//        {
//            get
//            {
//                return new RelayCommand(param =>
//                {
//                    foreach (var item in Avoidances)
//                    {
//                        item.IsEnabled = true;
//                    }
//                });
//            }
//        }

//        public ICommand SelectNoneCommand
//        {
//            get
//            {
//                return new RelayCommand(param =>
//                {
//                    foreach (var item in Avoidances)
//                    {
//                        item.IsEnabled = false;
//                    }
//                });
//            }
//        }

//        public ICommand SelectDefaultsCommand
//        {
//            get
//            {
//                return new RelayCommand(param =>
//                {
//                    Avoidances.Clear();
//                    LoadSettingsFromDataFactory(true);
//                });
//            }
//        }

//        public void Reset()
//        {
//            TrinitySetting.Reset(this);
//            Avoidances = new FullyObservableCollection<AvoidanceDataSettingViewModel>();
//            LoadSettingsFromDataFactory(true);
//        }

//        public void CopyTo(AvoidanceSetting setting)
//        {
//            TrinitySetting.CopyTo(this, setting);
//        }

//        public AvoidanceSetting Clone()
//        {
//            return TrinitySetting.Clone(this);
//        }

//        /// <summary>
//        /// This will set default values for new settings if they were not present in the serialized XML (otherwise they will be the type defaults)
//        /// </summary>
//        /// <param name="context"></param>
//        [OnDeserializing]
//        internal void OnDeserializingMethod(StreamingContext context)
//        {
//            foreach (var p in GetType().GetProperties())
//            {
//                foreach (var dv in p.GetCustomAttributes(true).OfType<DefaultValueAttribute>())
//                {
//                    p.SetValue(this, dv.Value);
//                }
//            }
//        }

//    }
//}
