using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Components.Adventurer.Util;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;

namespace Trinity.Components.Adventurer.Game.Grid
{
    [DataContract]
    public class SceneDefinition
    {
        private const string SCENES_PATH = "c:\\db\\scenes\\";
        private const bool USE_DISK_CACHE = false;

        [DataMember]
        public int SceneSNO { get; set; }
        [DataMember]
        public List<NavCellDefinition> NavCellDefinitions { get; set; }

        private SceneDefinition() { }

        public static SceneDefinition Create(Scene.NavMesh mesh, NavZone zone, NavZoneDef navZoneDef)
        {
            var filePath = string.Format("{0}{1}.json", SCENES_PATH, mesh.SceneSnoId);
            if (USE_DISK_CACHE)
            {
                if (File.Exists(filePath))
                {
                    var fileContent = File.ReadAllText(filePath);
                    if (!string.IsNullOrWhiteSpace(fileContent))
                    {
                        var savedScene = JsonSerializer.Deserialize<SceneDefinition>(fileContent);
                        if (savedScene != null)
                        {
                            return savedScene;
                        }
                    }
                }
            }
            var sceneDefinition = new SceneDefinition { SceneSNO = mesh.SceneSnoId };

            #region NavCells
            var cachedNavCells = new List<NavCellDefinition>();
            var sceneNavCells = navZoneDef.NavCells.ToList();

            if (sceneNavCells.Count > 0)
            {
                cachedNavCells.AddRange(sceneNavCells.Select(NavCellDefinition.Create));
            }
            sceneDefinition.NavCellDefinitions = cachedNavCells;
            #endregion


            if (USE_DISK_CACHE)
            {

                var serializedScene = JsonSerializer.Serialize(sceneDefinition);
                File.WriteAllText(filePath, serializedScene);
            }
            Logger.Warn("[GridProvider] Added SceneDefinition {0} (Dimensions: {1}, NavCells: {2})", sceneDefinition.SceneSNO, zone.ZoneMax - zone.ZoneMin, sceneDefinition.NavCellDefinitions.Count);
            return sceneDefinition;
        }
    }
}