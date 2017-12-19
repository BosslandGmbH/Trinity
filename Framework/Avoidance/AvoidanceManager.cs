using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Settings;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Modules;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;


namespace Trinity.Framework.Avoidance
{
    public class AvoidanceManager : Module, IDynamicSetting
    {
        public IAvoider Avoider { get; set; }
        public GridEnricher GridEnricher => Core.GridEnricher;

        public AvoidanceManager()
        {
            Avoider = new DefaultAvoider();
        }

        public bool InAvoidance(Vector3 position)
        {
            return Grid.IsLocationInFlags(position, AvoidanceFlags.Avoidance);
        }

        public bool InCriticalAvoidance(Vector3 position)
        {
            return Grid.IsLocationInFlags(position, AvoidanceFlags.CriticalAvoidance);
        }

        public IEnumerable<Structures.Avoidance> GetAvoidancesAtPosition(Vector3 position)
        {
            var hashCodes = Grid.GetNearestNode(position).AdjacentNodes.SelectMany(n => n.AvoidanceHashCodes).Distinct();
            return CurrentAvoidances.Where(a => hashCodes.Contains(a.GetHashCode()));
        }

        public const float AvoidanceWeightRadiusFactor = 1f;
        private readonly Dictionary<int, TrinityActor> _cachedActors = new Dictionary<int, TrinityActor>();
        private readonly HashSet<int> _currentRActorIds = new HashSet<int>();

        public IEnumerable<TrinityActor> ActiveAvoidanceActors => CurrentAvoidances.SelectMany(a => a.Actors);

        public List<Structures.Avoidance> CurrentAvoidances = new List<Structures.Avoidance>();
        public AvoidanceAreaStats NearbyStats = new AvoidanceAreaStats();

        public TrinityGrid Grid => TrinityGrid.Instance;

        protected override void OnPulse()
        {
            if (!TrinityPlugin.IsEnabled || ZetaDia.Globals.IsLoadingWorld)
                return;

            UpdateAvoidances();
            RemoveExpiredAvoidances();
            
            NearbyStats.Update(GridEnricher.NearbyNodes);
        }

        private void UpdateAvoidances()
        {
            _currentRActorIds.Clear();

            // SENY
            if (Core.Settings.SenExtend.IgnoreAvoidanceInNephalemRift && Core.Rift.IsNephalemRift &&  Core.Player.IsInRift)
            {
                return;
            }

            if (!Settings.Entries.Any(s => s.IsEnabled))
                return;

            var source = Core.Actors.Actors.ToList();

            foreach (var actor in source)
            {
                if (actor == null)
                    continue;

                var rActorId = actor.RActorId;

                TrinityActor existingActor;

                _currentRActorIds.Add(rActorId);

                var isValid = actor.IsValid;

                if (_cachedActors.TryGetValue(rActorId, out existingActor))
                {
                    if (!isValid)
                    {
                        _cachedActors.Remove(rActorId);
                    }
                    else
                    {
                        //Core.Logger.Verbose($"Updated Avoidance Actor {actor}");
                        existingActor.Position = actor.Position;
                        existingActor.Distance = actor.Distance;
                        existingActor.Animation = actor.Animation;
                    }
                    continue;
                }

                if (!isValid)
                    continue;

                Structures.Avoidance avoidance;
                if (AvoidanceFactory.TryCreateAvoidance(source, actor, out avoidance))
                {
                    Core.Logger.Log(LogCategory.Avoidance, $"创建新的规避 {actor.InternalName} RActorId={actor.RActorId} ({avoidance.Definition.Name}, 免疫: {avoidance.IsImmune})");
                    _cachedActors.Add(rActorId, actor);
                    CurrentAvoidances.Add(avoidance);
                }
            }
        }

        private void RemoveExpiredAvoidances()
        {
            foreach (var avoidance in CurrentAvoidances)
            {
                avoidance.Actors.RemoveAll(a => !_currentRActorIds.Contains(a.RActorId));
            }
            CurrentAvoidances.RemoveAll(a => !a.Actors.Any(actor => actor.IsValid));
        }

        #region Settings

        public AvoidanceSettings Settings { get; set; } = new AvoidanceSettings();

        public string GetName() => "AvoidanceSettings";

        public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>("AvoidanceSettings.xaml");

        public object GetDataContext() => Settings;

        public string GetCode() => Settings.Save();

        public void ApplyCode(string code) => Settings.Load(code);

        public void Reset() => Settings.LoadDefaults();

        public void Save()
        {
        }

        #endregion Settings

        public void Clear()
        {
            _cachedActors.Clear();
            _currentRActorIds.Clear();
            CurrentAvoidances.Clear();
        }

    }
}