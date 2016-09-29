using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Trinity.Components.Combat;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Helpers.AutoFollow.Resources;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Routines;
using Trinity.Settings;
using Trinity.UI;
using Trinity.UI.UIComponents;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework
{
    public sealed class RoutineManager
    {
        private static Lazy<RoutineManager> _instance = new Lazy<RoutineManager>(() => new RoutineManager());

        public static RoutineManager Instance => _instance.Value;

        private RoutineManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            Logger.Log($"RoutineManager Initializing from thread {Thread.CurrentThread.ManagedThreadId}");

            _routineLoader = new InterfaceLoader<IRoutine>();
            _routineLoader.Load();

            ChangeEvents.IsInGame.Changed += IsInGameOnChanged;
            ChangeEvents.EquippedItems.Changed += EquippedItemsOnChanged;
            ChangeEvents.Skills.Changed += SkillsOnChanged;

            foreach (var routine in _routineLoader.Items)
            {
                var settings = routine.Value.RoutineSettings;
                settings.Reset();
            }
        }

        private void SkillsOnChanged(ChangeDetectorEventArgs<HashSet<SNOPower>> args)
        {
            if (ChangeEvents.IsInGame.Value && !ZetaDia.Me.SkillOverrideActive)
            {
                AutoSelectRoutine();
            }
        }

        private void EquippedItemsOnChanged(ChangeDetectorEventArgs<HashSet<int>> args)
        {
            if (ChangeEvents.IsInGame.Value)
            {
                AutoSelectRoutine();
            }
        }

        private void IsInGameOnChanged(ChangeDetectorEventArgs<bool> item)
        {
            if (item.NewValue)
            {
                Thread.Sleep(500);                
                AutoSelectRoutine();
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
                    Logger.Warn($"Routine Changed to: {value.DisplayName} {value.BuildRequirements?.Summary}");
                     _currentRoutine = value;
                    Changed?.Invoke(value);
                }
            }
        }

        public IEnumerable<IRoutine> AllRoutines => _routineLoader.Items.Values;

        public IEnumerable<IRoutine> CurrentClassRoutines => AllRoutines.Where(r => r.Class == ZetaDia.Service.Hero?.Class);

        public IEnumerable<IDynamicSetting> DynamicSettings => _routineLoader.Items.Values.Select(r => r.RoutineSettings);

        public void AutoSelectRoutine()
        {
            var genericRoutines = new List<IRoutine>();
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
            CurrentRoutine = genericRoutines.FirstOrDefault();
        }
    }
}

