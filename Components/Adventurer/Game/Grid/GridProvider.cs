using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Game.Grid
{
    public static class GridProvider
    {
        public const float GridNodeBoxSize = 2.5f;

        private static bool _isUpdateRunning;


        public static Dictionary<string, GridScene> CachedScenes = new Dictionary<string, GridScene>();
        public static Dictionary<int, SceneDefinition> SceneDefinitions = new Dictionary<int, SceneDefinition>();

        public static void Initialize()
        {
            BotMain.OnStart += BotMain_OnStart;
            BotMain.OnStop += BotMain_OnStop;
            GameEvents.OnWorldChanged += GameEvents_OnWorldChanged;
        }

        private static void BotMain_OnStart(IBot bot)
        {
            Pulsator.OnPulse += Pulsator_OnPulse;
        }

        private static void BotMain_OnStop(IBot bot)
        {
            Pulsator.OnPulse -= Pulsator_OnPulse;
        }

        private static void GameEvents_OnWorldChanged(object sender, EventArgs e)
        {
            if (!Adventurer.IsAdventurerTagRunning()) return;
            Reset();
        }

        private static void Pulsator_OnPulse(object sender, EventArgs e)
        {
            if (!Adventurer.IsAdventurerTagRunning()) return;
            Task.Factory.StartNew(Update);
        }

        private static object _updateLock = new object();
        public static void Update()
        {
            if (_isUpdateRunning) return;
            _isUpdateRunning = true;
            lock (_updateLock)
            {
                using (new PerformanceLogger("[GridProvider] Update", true))
                {
                    var discoveredScenes = new List<GridScene>();
                    foreach (var scene in ZetaDia.Scenes)
                    {
                        if (!scene.IsValid) continue;

                        var mesh = scene.Mesh;
                        if (mesh == null || !mesh.IsValid) continue;

                        var name = scene.Name.ToLowerInvariant();
                        if (string.IsNullOrWhiteSpace(name) || name.Contains("filler") || name.Contains("fillscene") || name.Contains("buffer"))
                            continue;

                        var zone = mesh.Zone;
                        if (zone == null || !zone.IsValid) continue;

                        var sceneNameHash = GetSceneNameHash(mesh, zone);

                        if (CachedScenes.ContainsKey(sceneNameHash)) continue;

                        if (!SceneDefinitions.ContainsKey(mesh.SceneSnoId))
                        {
                            using (new PerformanceLogger("SceneDefinition.Create", true))
                            {

                                var navZoneDef = zone.NavZoneDef;
                                if (navZoneDef == null || !navZoneDef.IsValid) continue;

                                var sceneDefinition = SceneDefinition.Create(mesh, zone, navZoneDef);
                                if (sceneDefinition != null)
                                {
                                    SceneDefinitions.Add(mesh.SceneSnoId, sceneDefinition);
                                }
                            }
                        }
                        GridScene cachedScene;
                        using (new PerformanceLogger("GridScene.Create", true))
                        {
                            cachedScene = GridScene.Create(scene, mesh, zone);
                        }
                        CachedScenes.Add(sceneNameHash, cachedScene);
                        discoveredScenes.Add(cachedScene);
                        Logger.Info("[GridProvider] Added Scene {0} at {1})", cachedScene.Name, cachedScene.Min);
                    }
                    using (new PerformanceLogger("MainGrid.Instance.Update", true))
                    {
                        MainGrid.Instance.Update(discoveredScenes.SelectMany(s => s.GridNodes));
                    }
                }
                _isUpdateRunning = false;
            }
        }

        public static void Reset()
        {
            MainGrid.ResetAll();
            CachedScenes.Clear();
        }

        public static string GetSceneNameHash(Scene.NavMesh mesh, NavZone navZone)
        {
            return string.Format("{0}-{1}-{2}-{3}-{4}", mesh.SceneSnoId, mesh.WorldId, mesh.LevelAreaSnoId, navZone.ZoneMin, navZone.ZoneMax);
        }
    }
}
