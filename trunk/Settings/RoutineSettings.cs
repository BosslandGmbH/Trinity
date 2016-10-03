using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Routines;
using Trinity.UI.UIComponents;
using Zeta.Game;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public class RoutineSettings : NotifyBase
    {
        private string _selectedRoutineClassName;
        private RoutineMode _routineMode;

        [DataMember(IsRequired = false)]
        public string SelectedRoutineClassName
        {
            get { return _selectedRoutineClassName; }
            set { SetField(ref _selectedRoutineClassName, value); }
        }

        [DataMember(IsRequired = false)]
        public RoutineMode RoutineMode
        {
            get { return _routineMode; }
            set
            {
                if (SetField(ref _routineMode, value))
                {
                    RoutineManager.Instance?.SelectRoutine();
                }          
            }
        }

        private RoutineViewModel _selectedRoutine;
        private RoutineViewModel _currentRoutine;

        private List<RoutineViewModel> _allRoutines = new List<RoutineViewModel>();
        private List<RoutineViewModel> _classAvailableRoutines;
        private List<RoutineViewModel> _classRoutines;

        [IgnoreDataMember]
        public List<RoutineViewModel> AllRoutines
        {
            get { return _allRoutines; }
            set { SetField(ref _allRoutines, value); }
        }

        [IgnoreDataMember]
        public List<RoutineViewModel> ClassAvailableRoutines
        {
            get { return _classAvailableRoutines; }
            set { SetField(ref _classAvailableRoutines, value); }
        }

        [IgnoreDataMember]
        public List<RoutineViewModel> ClassRoutines
        {
            get { return _classRoutines; }
            set { SetField(ref _classRoutines, value); }
        }

        public bool IsInGameOrManual => GameInfo.Instance.IsInGame || RoutineMode == RoutineMode.Manual;

        public RoutineSettings()
        {
            base.LoadDefaults();

            Core.Routines.Changed += OnRoutineChanged;
            var current = Core.Routines.CurrentRoutine;

            foreach (var routine in Core.Routines.AllRoutines)
            {
                var routineViewModel = new RoutineViewModel(routine);
                AllRoutines.Add(routineViewModel);

                if (current == routine)
                {
                    CurrentRoutine = routineViewModel;
                    SelectedRoutine = routineViewModel;
                }
            }

            UpdateRoutineLists();

            if (_routineMode == RoutineMode.None)
                _routineMode = RoutineMode.Automatic;
        }

        public ActorClass GetCurrentClass() => Core.Routines.CurrentRoutine?.Class ?? ZetaDia.Service.Hero.Class;

        private void UpdateRoutineLists()
        {
            var actorClass = GetCurrentClass();
            ClassRoutines = AllRoutines.Where(r => r.Class == actorClass || r.Class == ActorClass.Invalid).ToList();
            ClassAvailableRoutines = ClassRoutines.Where(r => r != CurrentRoutine && r.RequiredBuild != null).ToList();
        }

        private void OnRoutineChanged(IRoutine newRoutine)
        {
            var routineViewModel = AllRoutines.FirstOrDefault(r => r.DisplayName == newRoutine.DisplayName);
            if (routineViewModel != null)
            {
                CurrentRoutine = routineViewModel;
                SelectedRoutine = routineViewModel;
                UpdateRoutineLists();
            }
        }

        [IgnoreDataMember]
        public RoutineViewModel CurrentRoutine
        {
            get { return _currentRoutine; }
            set
            {
                if (value != null)
                {
                    AllRoutines.ForEach(r => r.IsCurrent = false);
                    value.IsCurrent = true;
                    SetField(ref _currentRoutine, value);
                }
            }
        }

        [IgnoreDataMember]
        public RoutineViewModel SelectedRoutine
        {
            get { return _selectedRoutine; }
            set
            {
                if (value != null && _selectedRoutine != value)
                {
                    AllRoutines.ForEach(r => r.IsSelected = false);
                    value.IsSelected = true;
                    SetField(ref _selectedRoutine, value);

                    if (RoutineMode == RoutineMode.Manual)
                    {
                        RoutineManager.Instance.ManualSelectRoutine(value.RoutineTypeName);
                    }
                }
            }
        }

    }
}
