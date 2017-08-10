using System.Collections.Generic;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public class SceneData : ISceneData
    {
        public List<ISceneDataEntry> Scenes { get; set; }
        public int WorldDynamicId { get; set; }
    }

    public class SceneDataEntry : ISceneDataEntry
    {
        public string SceneHash { get; set; }
        public List<ExplorationNode> ExplorationNodes { get; set; }
    }
}