using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.Adventurer;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Framework;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Game;
using Trinity.Components.Adventurer.Game;
using Trinity.Components.Adventurer.Game.Exploration;
using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Objects;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Common;
using Zeta.Game.Internals;
using Trinity.Framework.Reference;
using Zeta.Game.Internals;
using System.Collections;

namespace Trinity.Modules
{
    public class SceneStorage : Module, IEnumerable<WorldScene>
    {
        public delegate void GridProviderEventHandler(List<WorldScene> provider);

        private int _currentWorld;
        private HashSet<string> _currentWorldSceneIds;

        private List<WorldScene> _currentWorldScenes;

        public List<WorldScene> CurrentWorldScenes => _currentWorldScenes ?? (_currentWorldScenes = new List<WorldScene>());

        public HashSet<string> CurrentWorldSceneIds => _currentWorldSceneIds ?? (_currentWorldSceneIds = new HashSet<string>());

        public WorldScene CurrentScene
        {
            get
            {
                var worldId = ZetaDia.Globals.WorldId;
                return
                    CurrentWorldScenes.FirstOrDefault(
                        s => s.DynamicWorldId == worldId && AdvDia.MyPosition.X >= s.Min.X && AdvDia.MyPosition.Y >= s.Min.Y && AdvDia.MyPosition.X <= s.Max.X && AdvDia.MyPosition.Y <= s.Max.Y);
            }
        }

        protected override void OnBotStart()
        {
            Reset();
        }

        protected override void OnWorldChanged(ChangeEventArgs<int> args)
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

        protected override void OnPulse() => Update();

        public void Update()
        {
            var currentWorldId = ZetaDia.Globals.WorldId;
            if (currentWorldId <= 0)
                return;

            if (_currentWorld != currentWorldId)
            {
                //if (GameData.MenuWorldSnoIds.Contains(ZetaDia.Globals.WorldSnoId))
                //{
                //    Core.Logger.Debug("[SceneStorage] Left Game....");
                //    Reset();
                //    return;
                //}
                //if (ZetaDia.Globals.IsLoadingWorld)
                //{
                //    Core.Logger.Debug("[SceneStorage] World loading....");
                //    return;
                //}

                Core.Logger.Debug("[SceneStorage] World has changed from {0} to {1}", _currentWorld, currentWorldId);
                _currentWorld = currentWorldId;
                Reset();
            }

            var addedScenes = new List<WorldScene>();
            List<Scene> newScenes;
            try
            {
                newScenes = ZetaDia.Scenes.Where(s => s.IsAlmostValid() && !CurrentWorldSceneIds.Contains(s.GetSceneNameString())).ToList();
            }
            catch (NullReferenceException)
            {
                return;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ReadProcessMemory"))
                {
                    return;
                }
                throw;
            }

            var worldId = 0;
            foreach (var scene in newScenes)
            {
                try
                {
                    if (!scene.IsAlmostValid())
                        continue;

                    var sceneHashName = scene.GetSceneNameString();
                    worldId = scene.Mesh.WorldId;

                    Scene subScene = scene.Mesh.SubScene;
                    if (scene.IsAlmostValid() && scene.Mesh.ParentSceneId <= 0 && worldId == currentWorldId )
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

            if (addedScenes.Count > 0)
            {
                Core.Logger.Debug("[ScenesStorage] Found {0} new scenes", addedScenes.Count);
                var sceneData = CreateSceneData(addedScenes, worldId);
                foreach (var grid in GridStore.GetCurrentGrids())
                {
                    grid.Update(sceneData);
                }
                ScenesAdded?.Invoke(addedScenes);
                Core.Logger.Debug("[ScenesStorage] Updates Finished", addedScenes.Count);
            }
        }

        public SceneData CreateSceneData(IEnumerable<WorldScene> addedScenes, int worldId)
        {
            var nodes = addedScenes.SelectMany(s => s.Nodes).ToList();

            var sceneData = new SceneData
            {
                WorldDynamicId = worldId,
                ExplorationNodes = nodes.Cast<IGroupNode>().ToList(),
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
            Core.Logger.Debug("[ScenesStorage] Reseting");
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
            => CurrentWorldScenes.FirstOrDefault(s => s.IsInScene(position));
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