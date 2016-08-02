using System.Collections.Generic;
using System.Linq;
using Zeta.Common;
using Zeta.Game.Internals;

namespace Trinity.Components.Adventurer.Game.Grid
{
    public class GridScene
    {
        public int SceneSNO { get; private set; }
        public int WorldId { get; private set; }
        public int LevelAreaSNO { get; private set; }
        public string Name { get; private set; }
        public string NameHash { get; private set; }
        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }
        public Vector2 Center { get; private set; }
        public float BaseHeight { get; set; }
        public SceneDefinition SceneDefinition { get; private set; }
        public List<GridNode> GridNodes { get; private set; }

        private GridScene() { }

        public static GridScene Create(Scene scene, Scene.NavMesh mesh, NavZone zone)
        {
            var gridScene = new GridScene();
            gridScene.SceneSNO = mesh.SceneSnoId;
            gridScene.WorldId = mesh.WorldId;
            gridScene.LevelAreaSNO = mesh.LevelAreaSnoId;
            gridScene.Name = scene.Name;
            gridScene.NameHash = GridProvider.GetSceneNameHash(mesh, zone);
            gridScene.Min = zone.ZoneMin;
            gridScene.Max = zone.ZoneMax;
            gridScene.Center = (gridScene.Max + gridScene.Min) / 2;
            gridScene.SceneDefinition = GridProvider.SceneDefinitions[mesh.SceneSnoId];
            gridScene.BaseHeight = mesh.BaseHeight;

            //#region GridNodes

            var gridNodes = new List<GridNode>();
            var sceneDimensions = zone.ZoneMax - zone.ZoneMin;
            const float searchBeginning = GridProvider.GridNodeBoxSize / 2;
            for (var x = searchBeginning; x <= sceneDimensions.X; x = x + GridProvider.GridNodeBoxSize)
            {
                for (var y = searchBeginning; y <= sceneDimensions.Y; y = y + GridProvider.GridNodeBoxSize)
                {
                    var cell = gridScene.SceneDefinition.NavCellDefinitions.FirstOrDefault(c => c.IsInCell(x, y));
                    if (cell != null)
                    {
                        var gridNode = GridNode.Create(new Vector2(x, y), gridScene, cell);
                        gridNodes.Add(gridNode);
                    }
                }
            }
            gridScene.GridNodes = gridNodes;
            //#endregion

            return gridScene;
        }
    }
}