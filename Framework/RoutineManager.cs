using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Trinity.Framework.Events;
using Trinity.Framework.Helpers.AutoFollow.Resources;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Routines;
using Trinity.Settings;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework
{
    public sealed class RoutineManager
    {
        private static Lazy<RoutineManager> _instance = new Lazy<RoutineManager>(() => new RoutineManager());
        public static RoutineManager Instance => _instance.Value;

        public RoutineSettings Settings => Core.Settings.Routine;

        private RoutineManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            Core.Logger.Log($"RoutineManager Initializing from thread {Thread.CurrentThread.ManagedThreadId}");

            _routineLoader = new InterfaceLoader<IRoutine>();
            _routineLoader.Load();

            ChangeEvents.IsInGame.Changed += IsInGameOnChanged;
            ChangeEvents.EquippedItems.Changed += EquippedItemsOnChanged;
            ChangeEvents.Skills.Changed += SkillsOnChanged;

            foreach (var routine in _routineLoader.Items)
            {
                var settings = routine.Value.RoutineSettings;
                settings?.Reset();
            }
        }

        private void SkillsOnChanged(ChangeEventArgs<HashSet<SNOPower>> args)
        {
            if (ChangeEvents.IsInGame.Value && ZetaDia.Me != null && !ZetaDia.Me.SkillOverrideActive)
            {
                SelectRoutine();
            }
        }

        private void EquippedItemsOnChanged(ChangeEventArgs<HashSet<int>> args)
        {
            if (ChangeEvents.IsInGame.Value)
            {
                SelectRoutine();
            }
        }

        private void IsInGameOnChanged(ChangeEventArgs<bool> item)
        {
            if (item.NewValue)
            {
                Thread.Sleep(500);
                SelectRoutine();
            }
        }

        public delegate void RoutineChangedEvent(IRoutine newRoutine);

        public event RoutineChangedEvent Changed;

        private InterfaceLoader<IRoutine> _routineLoader;



        private IRoutine _currentRoutine;
        public IRoutine CurrentRoutine
        {
            get { return _currentRoutine; }
            set
            {
                if (value != null && _currentRoutine != value)
                {
                    Core.Logger.Warn($"Routine Changed to: {value.DisplayName} {value.BuildRequirements?.Summary}");
                     _currentRoutine = value;
                    Changed?.Invoke(value);
                }
            }
        }

        public IEnumerable<IRoutine> AllRoutines => _routineLoader.Items.Values;

        //public IEnumerable<IRoutine> CurrentClassRoutines => AllRoutines.Where(r => r.Class == ZetaDia.Service.Hero?.Class);
        public IEnumerable<IRoutine> CurrentClassRoutines => AllRoutines.Where(r => r.Class == ZetaDia.Storage.PlayerDataManager.ActivePlayerData.HeroClass);
        
        public IEnumerable<IDynamicSetting> DynamicSettings => _routineLoader.Items.Values.Select(r => r.RoutineSettings);


        public void SelectRoutine()
        {
            if (Core.Settings.Routine == null)
                return;

            // Ignore wizards going into archon mode.
            if (Core.Player.ActorClass == ActorClass.Wizard && Core.Hotbar.ActivePowers.Any(p => GameData.ArchonSkillIds.Contains((int)p)))
                return;

            var genericRoutines = new List<IRoutine>();
            var manualSelectionName = Core.Settings.Routine.SelectedRoutineClassName;

            if (Settings.RoutineMode == RoutineMode.Manual && !string.IsNullOrEmpty(manualSelectionName))
            {
                Core.Logger.Log($"Loading Selected Routine: {manualSelectionName}");
                var routine = AllRoutines.FirstOrDefault(r => r.GetType().Name == manualSelectionName);
                if (routine != null)
                {
                    CurrentRoutine = routine;
                    return;
                }
            }

            if(!Core.Actors.AllRActors.Any())
                Core.Actors.Update();

            foreach (var routine in CurrentClassRoutines.OrderBy(r => r.BuildRequirements?.RequirementCount))
            {
                if (routine.BuildRequirements == null)
                {
                    genericRoutines.Add(routine);
                    continue;
                }
                if (routine.BuildRequirements.IsEquipped())
                {
                    CurrentRoutine = routine;
                    return;
                }
            }
            Core.Logger.Log($"Auto-Selecting default routine for class");
            CurrentRoutine = genericRoutines.FirstOrDefault();           
        }

        //public void ManualSelectRoutine(IRoutine routine)
        //{
        //    CurrentRoutine = routine;
        //    Settings.SelectedRoutineClassName = routine.GetType().Name;
        //    Settings.RoutineMode = RoutineMode.Manual;
        //    Core.Logger.Log($"Set Routine Selection: {Settings.SelectedRoutineClassName}");

        //}

        public void ManualSelectRoutine(string typeName)
        {
            var routine = AllRoutines.FirstOrDefault(r => r.GetType().Name == typeName);
            if (routine != null)
            {
                CurrentRoutine = routine;
                Settings.SelectedRoutineClassName = routine.GetType().Name;
                Settings.RoutineMode = RoutineMode.Manual;
                Core.Logger.Log($"Set Routine Selection: {Settings.SelectedRoutineClassName}");
            }
        }

    }
}

