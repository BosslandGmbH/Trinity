using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Routines;
using Zeta.Bot;
using Zeta.Game;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public class RoutineSettingsViewModel : NotifyBase
    {
        private RoutineViewModel _selectedRoutine;
        private RoutineViewModel _currentRoutine;

        private List<RoutineViewModel> _allRoutines = new List<RoutineViewModel>();
        private List<RoutineViewModel> _classRoutines;

        [IgnoreDataMember]
        public List<RoutineViewModel> AllRoutines
        {
            get { return _allRoutines; }
            set { SetField(ref _allRoutines, value); }
        }

        [IgnoreDataMember]
        public List<RoutineViewModel> ClassRoutines
        {
            get { return _classRoutines; }
            set { SetField(ref _classRoutines, value); }
        }

        public RoutineSettingsViewModel()
        {
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

            ClassRoutines = AllRoutines.Where(r => r.Class == CurrentRoutine.Class && r != CurrentRoutine && r.RequiredBuild != null).ToList();
        }

        private void OnRoutineChanged(IRoutine newRoutine)
        {
            var routineViewModel = AllRoutines.FirstOrDefault(r => r.DisplayName == newRoutine.DisplayName);
            if (routineViewModel != null)
            {
                CurrentRoutine = routineViewModel;
                SelectedRoutine = routineViewModel;
                ClassRoutines = AllRoutines.Where(r => r.Class == CurrentRoutine.Class && r != CurrentRoutine && r.RequiredBuild != null).ToList();
            }
        }

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

        public RoutineViewModel SelectedRoutine
        {
            get { return _selectedRoutine; }
            set
            {
                if (value != null)
                {
                    AllRoutines.ForEach(r => r.IsSelected = false);
                    value.IsSelected = true;
                    SetField(ref _selectedRoutine, value);
                }
            }
        }

    }

}




