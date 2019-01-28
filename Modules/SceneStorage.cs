using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GreyMagic;
using log4net;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Modules
{
    public class SceneStorage : Module, IEnumerable<WorldScene>
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();

        public delegate void GridProviderEventHandler(List<WorldScene> provider);

        private SNOWorld _currentWorld;
        private HashSet<string> _currentWorldSceneIds;

        private List<WorldScene> _currentWorldScenes;

        public List<WorldScene> CurrentWorldScenes => _currentWorldScenes ?? (_currentWorldScenes = new List<WorldScene>());

        public HashSet<string> CurrentWorldSceneIds => _currentWorldSceneIds ?? (_currentWorldSceneIds = new HashSet<string>());

        public WorldScene CurrentScene
        {
            get
            {
                var cachedPos = Core.Player.Position;
                var pos = cachedPos == Vector3.Zero ? ZetaDia.Me.Position : cachedPos;
                var worldId = ZetaDia.Globals.WorldId;

                return CurrentWorldScenes.FirstOrDefault(
                        s => s.DynamicWorldId == worldId && pos.X >= s.Min.X && pos.Y >= s.Min.Y && pos.X <= s.Max.X && pos.Y <= s.Max.Y);
            }
        }

        protected override void OnBotStart()
        {
            Reset();
        }

        protected override void OnWorldChanged(ChangeEventArgs<SNOWorld> args)
        {
            ExplorationHelpers.ClearExplorationPriority();
            PurgeOldScenes();
            Update();
        }

        private void PurgeOldScenes()
        {
            var currentWorldDynamicId = ZetaDia.Globals.WorldId;
            var now = DateTime.UtcNow;
            CurrentWorldScenes.RemoveAll(s => s.DynamicWorldId != currentWorldDynamicId && now.Subtract(s.GridCreatedTime).TotalSeconds > 240);
        }

        protected override void OnPulse()
        {
            Update();
        }

        public void Update()
        {
            var currentWorldId = ZetaDia.Globals.WorldId;
            if (currentWorldId <= 0)
                return;

            if (_currentWorld != currentWorldId)
            {
                if (!ZetaDia.IsInGame)
                {
                    s_logger.Debug($"[{nameof(Update)}] Left Game...");
                    Reset();
                    return;
                }
                if (ZetaDia.Globals.IsLoadingWorld)
                {
                    s_logger.Debug($"[{nameof(Update)}] World loading...");
                    return;
                }

                s_logger.Debug($"[{nameof(Update)}] World has changed from {_currentWorld} to {currentWorldId}.");
                _currentWorld = currentWorldId;
                Reset();
            }

            var addedScenes = new List<WorldScene>();
            List<Scene> newScenes;
            try
            {
                newScenes = ZetaDia.Scenes
                    .Where(s => s.IsAlmostValid() && !CurrentWorldSceneIds.Contains(s.GetSceneNameString())).ToList();
            }
            catch (NullReferenceException)
            {
                return;
            }
            catch (PartialReadWriteException)
            {
                return;
            }

            SNOWorld worldId = 0;
            foreach (var scene in newScenes)
            {
                try
                {
                    if (!scene.IsAlmostValid())
                        continue;

                    var sceneHashName = scene.GetSceneNameString();
                    worldId = scene.Mesh.WorldId;

                    var subScene = scene.Mesh.SubScene;
                    if (scene.IsAlmostValid() && scene.Mesh.ParentSceneId <= 0 && worldId == currentWorldId)
                    {
                        if (scene.Mesh.Zone.GridSquares.Length <= 1 && (subScene != null && !subScene.HasGridSquares()))
                            continue;

                        var adventurerScene = new WorldScene(scene, ExplorationData.ExplorationNodeBoxSize,
                            ExplorationData.ExplorationNodeBoxTolerance);
                        if (adventurerScene.Cells.Count > 0)
                        {
                            CurrentWorldScenes.Add(adventurerScene);
                            addedScenes.Add(adventurerScene);
                        }
                    }
                    CurrentWorldSceneIds.Add(sceneHashName);
                }
                catch (NullReferenceException)
                {
                }
            }

            if (addedScenes.Count <= 0)
                return;

            s_logger.Debug($"[{nameof(Update)}] Found {addedScenes.Count} new scenes.");
            var sceneData = CreateSceneData(addedScenes, worldId);
            foreach (var grid in GridStore.GetCurrentGrids())
            {
                grid.Update(sceneData);
            }

            if (CurrentScene != null)
            {
                foreach (var scene in CurrentWorldScenes.Where(s => !s.HasPlayerConnection))
                {
                    if (scene == CurrentScene || CurrentScene.IsConnected(scene))
                    {
                        scene.HasPlayerConnection = true;
                    }
                }
            }

            ScenesAdded?.Invoke(addedScenes);
            s_logger.Debug($"[{nameof(Update)}] Updates Finished.");
        }

        public SceneData CreateSceneData(IEnumerable<WorldScene> addedScenes, SNOWorld worldId)
        {
            var scenes = addedScenes.Select(scene => new SceneDataEntry
            {
                SceneHash = scene.HashName,
                ExplorationNodes = scene.Nodes,

            }).Cast<ISceneDataEntry>().ToList();

            var sceneData = new SceneData
            {
                WorldDynamicId = worldId,
                Scenes = scenes
            };

            return sceneData;
        }

        public event GridProviderEventHandler ScenesAdded;

        public IEnumerable<ExplorationNode> SceneNodes(Scene scene)
        {
            if (scene.IsAlmostValid())
            {
                var sceneNameHash = scene.GetSceneNameString();
                if (CurrentWorldSceneIds.Contains(sceneNameHash))
                {
                    var adventurerScene = CurrentWorldScenes.FirstOrDefault(s => s.HashName == sceneNameHash);
                    if (adventurerScene != null)
                    {
                        return adventurerScene.Nodes;
                    }
                }
            }
            return new List<ExplorationNode>();
        }

        public void Reset()
        {
            s_logger.Debug($"[{nameof(Reset)}] Started");
            CurrentWorldSceneIds.Clear();
            CurrentWorldScenes.Clear();
            foreach (var grid in GridStore.GetCurrentGrids())
            {
                grid.Reset();
            }
        }

        public void ResetVisited()
        {
            foreach (var node in CurrentWorldScenes.SelectMany(s => s.Nodes))
            {
                node.IsVisited = false;
            }
        }

        public IEnumerator<WorldScene> GetEnumerator()
        {
            return ((IEnumerable<WorldScene>)CurrentWorldScenes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<WorldScene>)CurrentWorldScenes).GetEnumerator();
        }

        public WorldScene GetScene(Vector3 position)
        {
            return CurrentWorldScenes.FirstOrDefault(s => s.IsInScene(position));
        }
    }

    public static class SceneExtensions
    {
        public static bool HasParent(this Scene scene)
        {
            return scene.Mesh.ParentSceneId > 0;
        }

        public static bool HasGridSquares(this Scene scene)
        {
            return scene.Mesh.Zone.GridSquares.Length > 0;
        }

        public static bool IsAlmostValid(this Scene scene)
        {
            return scene?.Mesh?.Zone != null;
        }

        public static bool IsFullyValid(this Scene scene)
        {
            return scene != null && scene.IsValid && scene.Mesh?.Zone != null && scene.Mesh.Zone.IsValid && scene.Mesh.Zone.NavZoneDef != null && scene.Mesh.Zone.NavZoneDef.IsValid;
        }

        public static string GetSceneNameString(this Scene scene)
        {
            return $"{scene.Name}-{scene.Mesh.SceneSnoId}-{scene.Mesh.LevelAreaSnoId}-{scene.Mesh.Zone.ZoneMin}-{scene.Mesh.Zone.ZoneMax}";
        }
    }
}
