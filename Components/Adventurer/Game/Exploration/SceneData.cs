using System.Collections.Generic;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public class SceneData : ISceneData
    {
        public int WorldDynamicId { get; set; }
        public List<IGroupNode> ExplorationNodes { get; set; }
    }
}