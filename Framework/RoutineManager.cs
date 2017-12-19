using System;
using System.Collections.Generic;
using Trinity.Framework.Helpers;
using System.Linq;
using System.Threading;
using Trinity;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers.AutoFollow.Resources;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Routines;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Framework
{
    public sealed class RoutineManager
    {
        public delegate void RoutineChangedEvent(IRoutine newRoutine);

        private static Lazy<RoutineManager> _instance = new Lazy<RoutineManager>(() => new RoutineManager());

        private IRoutine _currentRoutine;

        private InterfaceLoader<IRoutine> _routineLoader;

        private RoutineManager()
        {
            Initialize();
        }

        public static RoutineManager Instance => _instance.Value;

        public RoutineSettings Settings => Core.Settings.Routine;

        public IRoutine CurrentRoutine
        {
            get { return _currentRoutine; }
            set
            {
                if (value != null && _currentRoutine != value)
                {
                    Core.Logger.Warn($"战斗策略已变更至: {value.DisplayName} {value.BuildRequirements?.Summary}");
                    _currentRoutine = value;
                    Changed?.Invoke(value);
                }
            }
        }

        public IEnumerable<IRoutine> AllRoutines => _routineLoader.Items.Values;
        public IEnumerable<IRoutine> CurrentClassRoutines => AllRoutines.Where(r => r.Class == ZetaDia.Service.Hero.Class);
        public IEnumerable<IDynamicSetting> DynamicSettings => _routineLoader.Items.Values.Select(r => r.RoutineSettings);

        private void Initialize()
        {
            Core.Logger.Log($"策略管理为线程 {Thread.CurrentThread.ManagedThreadId} 初始化");

            _routineLoader = new InterfaceLoader<IRoutine>();
            _routineLoader.Load();

            foreach (var routine in _routineLoader.Items)
            {
                var settings = routine.Value.RoutineSettings;
                settings?.Reset();
            }
        }

        public event RoutineChangedEvent Changed;


        public void SelectRoutine()
        {
            if (Core.Settings.Routine == null)
                return;

            if (!TrinityPlugin.IsEnabled)
                return;

            // Ignore wizards going into archon mode.
            if (Core.Player.ActorClass == ActorClass.Wizard && Core.Hotbar.ActivePowers.Any(p
                    => GameData.ArchonSkillIds.Contains((int) p)))
                return;

            var genericRoutines = new List<IRoutine>();
            var manualSelectionName = Core.Settings.Routine.SelectedRoutineClassName;

            if (Settings.RoutineMode == RoutineMode.Manual && !string.IsNullOrEmpty(manualSelectionName))
            {                
                var routine = AllRoutines.FirstOrDefault(r => r.GetType().Name == manualSelectionName);
                if (routine != null)
                {
                    if (routine != CurrentRoutine)
                    {
                        Core.Logger.Log($"加载战斗策略:");
                        CurrentRoutine = routine;
                        Core.Logger.Warn($"作者: {CurrentRoutine.Author}");
                        Core.Logger.Warn($"职业: {CurrentRoutine.Class}");
                        Core.Logger.Warn($"名称: {CurrentRoutine.DisplayName}");
                        //Range
                        Core.Logger.Warn($"怪群半径: {CurrentRoutine.ClusterRadius}");
                        Core.Logger.Warn($"怪群数量: {CurrentRoutine.ClusterSize}");
                        Core.Logger.Warn($"精英范围: {CurrentRoutine.EliteRange}");
                        Core.Logger.Warn($"垃圾怪范围: {CurrentRoutine.TrashRange}");
                        Core.Logger.Warn($"圣坛范围: {CurrentRoutine.ShrineRange}");
                        //Health
                        Core.Logger.Warn($"药水血量 %: {CurrentRoutine.PotionHealthPct}");
                        Core.Logger.Warn($"安全血量 % : {CurrentRoutine.EmergencyHealthPct}");
                        Core.Logger.Warn($"血球拾取范围: {CurrentRoutine.HealthGlobeRange}");

                        Core.Logger.Warn($"主要资源: {CurrentRoutine.PrimaryEnergyReserve}");
                    }
                    
                    return;
                }
            }

            if (!Core.Actors.Actors.Any())
                Core.Actors.Update();

            foreach (var routine in CurrentClassRoutines.OrderBy(r => r.BuildRequirements?.RequirementCount))
            {
                if (routine.BuildRequirements == null && routine.Class != ActorClass.Invalid)
                {
                    genericRoutines.Add(routine);
                    continue;
                }
                if (routine.BuildRequirements?.IsEquipped() ?? false)
                {
                    CurrentRoutine = routine;
                    return;
                }
            }

            CurrentRoutine = genericRoutines.FirstOrDefault();
        }

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