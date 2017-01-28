using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Rift;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Cache
{
    public static class AdvDia
    {
        //public static uint LastUpdatedFrame { get; private set; }


        //private static Lazy<RiftQuest> _riftQuest = new Lazy<RiftQuest>(() => new RiftQuest());

        //private static PropertyReader<int> _currentWorldId;
        //private static PropertyReader<int> _currentWorldDynamicId;
        //private static PropertyReader<int> _currentLevelAreaId;
        //private static PropertyReader<Vector3> _myPosition;
        //private static PropertyReader<Scene> _currentScene;
        //private static PropertyReader<bool> _isInTown;
        //private static PropertyReader<int> _battleNetHeroId;
        //private static PropertyReader<List<MapMarker>> _currentWorldMarkers;

        static AdvDia()
        {
            // Caching my position on pulse because of access exception from exploration grid amd radar UI thread
            // which was causing exploration grid to be constantly reloaded and radar to flicker.
            Pulsator.OnPulse += (sender, args) =>
            {
                var position = GetMyPositionFromMemory();
                if (position != Vector3.Zero)
                {
                    MyPosition = position;
                }
            };
        }

        public static int CurrentWorldId
        {
            get { return PropertyReader<int>.SafeReadValue(() => ZetaDia.CurrentWorldSnoId); }
        }

        public static INavigationProvider Navigator { get; set; }

        public static MainGridProvider MainGridProvider => (MainGridProvider) Zeta.Bot.Navigation.Navigator.SearchGridProvider;

        public static DefaultNavigationProvider DefaultNavigationProvider => Zeta.Bot.Navigation.Navigator.NavigationProvider as DefaultNavigationProvider;

        public static int CurrentWorldDynamicId
        {
            get { return PropertyReader<int>.SafeReadValue(() => ZetaDia.WorldId); }

        }

        public static int CurrentLevelAreaId
        {
            get { return PropertyReader<int>.SafeReadValue(() => ZetaDia.CurrentLevelAreaSnoId); }

        }

        public static Vector3 MyPosition { get; set; }

        public static Vector3 GetMyPositionFromMemory()
        {                            
            return PropertyReader<Vector3>.SafeReadValue(() => ZetaDia.Me.Position);
        }

        public static float MyZDiff(Vector3 toPosition)
        {
            return Math.Abs(toPosition.Z - AdvDia.MyPosition.Z);
        }

        public static WorldScene CurrentWorldScene
        {
            get { return ScenesStorage.CurrentScene; }

        }

        public static bool IsInTown
        {
            get { return PropertyReader<bool>.SafeReadValue(() => ZetaDia.IsInTown); }

        }

        public static int BattleNetHeroId
        {
            get { return PropertyReader<int>.SafeReadValue(() => ZetaDia.Memory.Read<int>(ZetaDia.Service.Hero.BaseAddress + 200)); }
        }

        public static List<MinimapMarker> CurrentWorldMarkers
        {
            get
            {
                return PropertyReader<List<MinimapMarker>>.SafeReadValue(() => ZetaDia.Minimap.Markers.CurrentWorldMarkers.Where(m => m.IsValid && m.NameHash != -1).ToList());
            }

        }

        public static RiftQuest RiftQuest { get { return PropertyReader<RiftQuest>.SafeReadValue(() => new RiftQuest()); } }

        public static IEnumerable<ACDItem> StashAndBackpackItems
        {
            get
            {
                ZetaDia.Actors.Update();
                return ZetaDia.Me.Inventory.Backpack.Union(ZetaDia.Me.Inventory.StashItems);
            }
        }

        public static SNOAnim MyCurrentAnimation
        {
            get { return PropertyReader<SNOAnim>.SafeReadValue(() => ZetaDia.Me.CommonData.CurrentAnimation); }

        }

        public static string BattleNetBattleTagName
        {
            get { return PropertyReader<string>.SafeReadValue(() => ZetaDia.Service.Hero.BattleTagName); }
        }

        public static bool IsInArchonForm
        {
            get { return ZetaDia.Me.GetAllBuffs().Any(b => b.SNOId == (int) SNOPower.Wizard_Archon); }
        }


        //static AdvDia()
        //{
        //}

        //public static void Update()
        //{
        //    if (ZetaDia.IsInGame)
        //    {
        //        UpdateValues();
        //        ScenesStorage.Update();
        //        //Providers.GridProvider = GridProvider.Instance;
        //    }

        //}

        //private static void UpdateValues()
        //{
        //    var result = SafeFrameLock.ExecuteWithinFrameLock(() =>
        //     {
        //         _currentWorldId = new PropertyReader<int>(() => ZetaDia.CurrentWorldSnoId, (v) => v == 0 || v == -1);
        //         _currentWorldDynamicId = new PropertyReader<int>(() => ZetaDia.WorldId, (v) => v == 0 || v == -1);
        //         _currentLevelAreaId = new PropertyReader<int>(() => ZetaDia.CurrentLevelAreaSnoId, (v) => v == 0 || v == -1);
        //         _myPosition = new PropertyReader<Vector3>(() => ZetaDia.Me.Position, (v) => v == Vector3.Zero);
        //         _currentScene = new PropertyReader<Scene>(() => ZetaDia.Me.CurrentScene, (v) => v == null);
        //         _isInTown = new PropertyReader<bool>(() => ZetaDia.IsInTown);
        //         _currentWorldMarkers = new PropertyReader<List<MapMarker>>(() => ZetaDia.Minimap.Markers.CurrentWorldMarkers.Where(m => m.IsValid && m.NameHash != -1)
        //                         .Select(m => new MapMarker(m))
        //                         .ToList(), (v) => v == null || v.Count == 0);
        //         _battleNetHeroId = new PropertyReader<int>(() => ZetaDia.Memory.Read<int>(ZetaDia.Service.Hero.BaseAddress + 200), (v) => v == 0 || v == -1);

        //     }, true);
        //    if (!result.Success)
        //    {
        //        Logger.Error("[Cache][Update] " + result.Exception.Message);
        //    }

        //}
    }


    public class PropertyReader<T>
    {
        private readonly Func<T> _valueFactory;
        private readonly Func<T, bool> _failureCondition;
        private T _value;
        public PropertyReader(Func<T> valueFactory, Func<T, bool> failureCondition = null, bool lazyEvaluate = false)
        {
            _valueFactory = valueFactory;
            _failureCondition = failureCondition;
            if (!lazyEvaluate) _value = SafeReadValue(_valueFactory);
        }

        public T Value
        {
            get
            {
                if (_failureCondition != null && _failureCondition(_value))
                {
                    _value = SafeReadValue(_valueFactory.Invoke);
                }
                return _value;
            }
        }

        public class ReadEntrance : IDisposable
        {
            public static int Count { get; private set; }
            public ReadEntrance()
            {
                Count++;
            }
            public void Dispose()
            {
                Count--;
            }
        }

        public static T SafeReadValue(Func<T> valueFactory, [CallerMemberName] string caller = "")
        {
            try
            {
                using (new ReadEntrance())
                {
                    if (BotEvents.IsBotRunning || ReadEntrance.Count > 1)
                    {
                        return valueFactory();
                    }
                    using (ZetaDia.Memory.AcquireFrame())
                    {
                        return valueFactory();
                    }
                }

            }
            catch (ACDAttributeLookupFailedException acdEx)
            {
                Util.Logger.Debug("[AdvDia] ACDAttributeLookupFailedException {0}", acdEx.Message);
            }
            catch (Exception ex)
            {
                Util.Logger.Debug("[AdvDia] {0}", ex.Message);
                if (!ex.Message.Contains("ReadProcessMemory"))
                    throw;
            }
            return default(T);
        }

    }

}
