using System.Runtime.Serialization;
using System.Security.Policy;
using System.Threading;
using System.Windows.Input;
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
        public string Description { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetField(ref _isSelected, value); }
        }

        [DataMember]
        public bool IsCurrent
        {
            get { return _isCurrent; }
            set { SetField(ref _isCurrent, value); }
        }
    }
}