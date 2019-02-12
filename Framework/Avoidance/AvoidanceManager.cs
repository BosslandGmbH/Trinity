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
        // private readonly Dictionary<int, TrinityActor> _cachedActors = new Dictionary<int, TrinityActor>();
        private HashSet<int> _currentRActorIds = new HashSet<int>();

        public List<Structures.Avoidance> CurrentAvoidances = new List<Structures.Avoidance>();
        public AvoidanceAreaStats NearbyStats = new AvoidanceAreaStats();

        public TrinityGrid Grid => TrinityGrid.Instance;

        protected override void OnPulse()
        {
            if (!Plugin.IsEnabled || ZetaDia.Globals.IsLoadingWorld)
                return;

            UpdateAvoidances();
            RemoveExpiredAvoidances();
            
            NearbyStats.Update(GridEnricher.NearbyNodes);
        }

        private void UpdateAvoidances()
        {
            _currentRActorIds = new HashSet<int>();
            foreach (var actor in Core.Actors.Actors)
            {
                if (actor == null || !actor.IsValid)
                    continue;

                var id = actor.RActorId;
                _currentRActorIds.Add(id);

                if (!CurrentAvoidances.Any(c => c.RActorId == id))
                {
                    Structures.Avoidance avoidance;
                    if (AvoidanceFactory.TryCreateAvoidance(actor, out avoidance))
                    {
                        Core.Logger.Log(LogCategory.Avoidance, $"Created new Avoidance from {actor.InternalName} RActorId={actor.RActorId} ({avoidance.Definition.Name}, Immune: {avoidance.IsImmune})");
                        CurrentAvoidances.Add(avoidance);
                    }
                }
            }
        }

        private void RemoveExpiredAvoidances()
        {
            var previousCount = CurrentAvoidances.Count;
            foreach (var avoidance in CurrentAvoidances)
            {
                if (!_currentRActorIds.Contains(avoidance.RActorId))
                    avoidance.RActorId = -1;
            }

            CurrentAvoidances.RemoveAll(a => a.RActorId == -1);
            CurrentAvoidances.RemoveAll(a => a.IsExpired);

            if (previousCount != 0 || CurrentAvoidances.Count != 0)
                Core.Logger.Log(LogCategory.Avoidance, $"Cleanup Avoidance before {previousCount}, now {CurrentAvoidances.Count}");
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
            _currentRActorIds.Clear();
            CurrentAvoidances.Clear();
        }

    }
}