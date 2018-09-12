using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Trinity.Framework.Objects;
using Trinity.Routines;
using Trinity.UI.UIComponents;
using Zeta.Game;

namespace Trinity.Settings
{
    [DataContract]
    public class RoutineViewModel : NotifyBase
    {
        private bool _isCurrent;
        private bool _isSelected;
        private readonly IRoutine _routine;
        private object _dataContext;
        private UserControl _control;

        public RoutineViewModel(IRoutine routine)
        {
            _routine = routine;
            RoutineTypeName = routine.GetType().Name;
            DisplayName = routine.DisplayName;
            Description = routine.Description;
            RequiredBuild = routine.BuildRequirements;
            Author = routine.Author;
            Version = routine.Version;
            Class = routine.Class;
            Url = routine.Url;
            BuildControl(routine);
        }

        private void BuildControl(IRoutine routine)
        {
            if (routine == null)
                return;

            DataContext = routine.RoutineSettings?.GetDataContext();

            if (routine.RoutineSettings == null)
            {
                ErrorMessage = "No settings were found for this routine";
                return;
            }

            var control = routine.RoutineSettings.GetControl();
            if (control == null)
            {
                ErrorMessage = "No settings control was found for this routine";
                return;
            }

            Control = new UserControl
            {
                Content = control,
                DataContext = this
            };
        }

        [IgnoreDataMember]
        public ActorClass Class { get; set; }

        [IgnoreDataMember]
        public ICommand ResetCommand => new RelayCommand(param => (DataContext as IDynamicSetting)?.Reset());

        [IgnoreDataMember]
        public Build RequiredBuild { get; set; }

        [IgnoreDataMember]
        public System.Windows.Controls.UserControl Control
        {
            get => _control;
            set => SetField(ref _control, value);
        }

        [IgnoreDataMember]
        public string ErrorMessage { get; set; }

        [IgnoreDataMember]
        public object DataContext
        {
            get => _dataContext;
            set => SetField(ref _dataContext, value);
        }

        [DataMember]
        public string RoutineTypeName { get; set; }

        [IgnoreDataMember]
        public string Description { get; set; }

        [IgnoreDataMember]
        public string DisplayName { get; set; }

        [IgnoreDataMember]
        public string Author { get; set; }

        [IgnoreDataMember]
        public string Version { get; set; }

        [IgnoreDataMember]
        public string Url { get; set; }

        [IgnoreDataMember]
        public bool IsSelected
        {
            get => _isSelected;
            set => SetField(ref _isSelected, value);
        }

        [IgnoreDataMember]
        public bool IsCurrent
        {
            get => _isCurrent;
            set => SetField(ref _isCurrent, value);
        }

        [IgnoreDataMember]
        public ICommand SelectRoutineCommand => new RelayCommand(parameter =>
        {
            RoutineManager.Instance.ManualSelectRoutine(RoutineTypeName);
        });

        [IgnoreDataMember]
        public ICommand OpenLinkCommand => new RelayCommand(parameter =>
        {
            var url = parameter as string;
            if (string.IsNullOrEmpty(url))
                return;

            var linkmatch = RE_URL.Match(url);
            var uri = new Uri(linkmatch.Value);
            Process.Start(uri.ToString());
        });

        private static readonly Regex RE_URL = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");

        public void RefreshControl()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                BuildControl(_routine);
            });                          
        }
    }
}