using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using Adventurer.UI.UIComponents;
using JetBrains.Annotations;
using Trinity.Config.Combat;
using Trinity.Framework;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Avoidance.Handlers;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Helpers;
using Trinity.Settings.Loot;
using TrinityCoroutines.Resources;
using Trinity.Technicals;

namespace Trinity.Config
{
    [DataContract(Namespace = "", Name = "Avoidance")]
    public class AvoidanceDataSettingViewModel : NotifyBase
    {
        private bool _isEnabled;
        private string _name;
        private IAvoidanceHandler _handler;

        public AvoidanceDataSettingViewModel()
        {
            
        }

        public AvoidanceDataSettingViewModel(IAvoidanceSetting a)
        {
            this.Name = a.Name;
            this.IsEnabled = a.IsEnabled;
            this.Handler = a.Handler;
        }

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        [DataMember]
        [DefaultValue(true)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetField(ref _isEnabled, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(null)]
        public IAvoidanceHandler Handler
        {
            get { return _handler; }
            set { SetField(ref _handler, value); }
        }

        public void CopyTo(IAvoidanceSetting a)
        {
            a.Name = this.Name;
            a.IsEnabled = this.IsEnabled;

            if (this.Handler != null)
                a.Handler = this.Handler;
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            foreach (var p in GetType().GetProperties())
            {
                foreach (var dv in p.GetCustomAttributes(true).OfType<DefaultValueAttribute>())
                {
                    p.SetValue(this, dv.Value);
                }
            }
        }
    }

    [DataContract(Namespace = "")]
    public class AvoidanceSetting : NotifyBase, ITrinitySetting<AvoidanceSetting>, ITrinitySettingEvents
    {
        private FullyObservableCollection<AvoidanceDataSettingViewModel> _avoidances;
        private float _avoiderNearbyPctAvgTrigger;
        private float _avoiderLocalRadius;
        private int _minimumHighestNodeWeightTrigger;
        private float _minimumNearbyWeightPctTotalTrigger;
        public event PropertyChangedEventHandler PropertyChanged;

        public AvoidanceSetting()
        {
            Reset();

            if (Avoidances == null)
                Avoidances = new FullyObservableCollection<AvoidanceDataSettingViewModel>();
        }

        [DataMember]
        [DefaultValue(15f)]
        public float AvoiderLocalRadius
        {
            get { return _avoiderLocalRadius; }
            set { SetField(ref _avoiderLocalRadius, value); }
        }

        [DataMember]
        [DefaultValue(0.1f)]
        public float AvoiderNearbyPctAvgTrigger
        {
            get { return _avoiderNearbyPctAvgTrigger; }
            set { SetField(ref _avoiderNearbyPctAvgTrigger, value); }
        }

        [DataMember]
        public FullyObservableCollection<AvoidanceDataSettingViewModel> Avoidances
        {
            get { return _avoidances; }
            set { SetField(ref _avoidances, value); }
        }

        [DataMember]
        [DefaultValue(1)]
        public int MinimumHighestNodeWeightTrigger
        {
            get { return _minimumHighestNodeWeightTrigger; }
            set { SetField(ref _minimumHighestNodeWeightTrigger, value); }
        }

        [DataMember]
        [DefaultValue(3f)]
        public float MinimumNearbyWeightPctTotalTrigger
        {
            get { return _minimumNearbyWeightPctTotalTrigger; }
            set { SetField(ref _minimumNearbyWeightPctTotalTrigger, value); }
        }

        public void OnSave()
        {
            Logger.Log("Saving Avoidance Data");
            ApplyUserSettingsToDataFactory();
        }

        public void OnLoaded()
        {
            Logger.Log("Loading Avoidance Data");
            LoadSettingsFromDataFactory();             
        }

        public void ApplyUserSettingsToDataFactory()
        {
            var data = AvoidanceDataFactory.AvoidanceData.ToDictionary(k => k.Name, v => v);

            foreach (var avoidanceSetting in Avoidances)
            {
                AvoidanceData def;
                if (avoidanceSetting != null && avoidanceSetting.Name != null && avoidanceSetting .Handler != null && data.TryGetValue(avoidanceSetting.Name, out def))
                {
                    //Logger.Log("Applying Loaded Setting {0} to avoidance", avoidanceSetting.Name);
                    avoidanceSetting.CopyTo(def);
                }
            }
        }

        private void LoadSettingsFromDataFactory()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var data = Avoidances.ToDictionary(k => k.Name, v => v);

                foreach (var def in AvoidanceDataFactory.AvoidanceData)
                {
                    if (def == null || def.Name == null || def.Handler == null)
                        continue;

                    AvoidanceDataSettingViewModel setting;

                    if (data.TryGetValue(def.Name, out setting))
                    {
                        if (setting.Handler == null)
                        {
                            // Handler name was invalid but other settings exist.
                            setting.Handler = def.Handler;
                            LoadHandlerDefaults(def);
                        }

                        //Logger.Log("Applying Loaded Setting {0}", setting.Name);
                        setting.CopyTo(def);              
                    }
                    else 
                    {
                        //Logger.Log("Creating new Setting for {0} ", def.Name);
                        LoadHandlerDefaults(def);
                        Avoidances.Add(new AvoidanceDataSettingViewModel(def));
                    }
                }

            });
        }

        private static void LoadHandlerDefaults(AvoidanceData def)
        {
            var handlerBase = def.Handler as NotifyBase;
            if (handlerBase != null)
            {
                handlerBase.LoadDefaults();
            }
        }

        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(AvoidanceSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public AvoidanceSetting Clone()
        {
            return TrinitySetting.Clone(this);
        }

        /// <summary>
        /// This will set default values for new settings if they were not present in the serialized XML (otherwise they will be the type defaults)
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {            
            foreach (var p in GetType().GetProperties())
            {
                foreach (var dv in p.GetCustomAttributes(true).OfType<DefaultValueAttribute>())
                {
                    p.SetValue(this, dv.Value);
                }
            }
        }

    }
}
