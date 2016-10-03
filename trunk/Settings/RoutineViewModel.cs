using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Input;
using Trinity.Framework;
using Trinity.Framework.Helpers;
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

        public RoutineViewModel(IRoutine routine)
        {
            RoutineTypeName = routine.GetType().Name;
            DisplayName = routine.DisplayName;
            Description = routine.Description;
            RequiredBuild = routine.BuildRequirements;
            Author = routine.Author;
            Version = routine.Version;
            Class = routine.Class;
            Url = routine.Url;
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

            Control = new System.Windows.Controls.UserControl
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
        public System.Windows.Controls.UserControl Control { get; set; }

        [IgnoreDataMember]
        public string ErrorMessage { get; set; }

        [IgnoreDataMember]
        public object DataContext { get; set; }

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
            get { return _isSelected; }
            set { SetField(ref _isSelected, value); }
        }

        [IgnoreDataMember]
        public bool IsCurrent
        {
            get { return _isCurrent; }
            set { SetField(ref _isCurrent, value); }
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

    }
}