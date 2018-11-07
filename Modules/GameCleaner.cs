using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Framework;
using Trinity.Framework.Objects;
using Zeta.Game;

namespace Trinity.Modules
{
    public class GameCleaner : Module
    {
        protected override void OnGameChanged()
        {
            Clean();
        }

        protected override void OnGameLeft()
        {
            Clean();
        }

        private static void Clean()
        {
            ZetaDia.Actors.Clear();
            ZetaDia.Actors.Update();
            Core.Actors.Clear();
            Core.Hotbar.Clear();
            Core.Inventory.Clear();
            Core.Buffs.Clear();
            Core.Targets.Clear();
            Core.Avoidance.Clear();
            QuestCache.Clear();
            Core.Scenes.Reset();
            ExplorationGrid.Clear();
        }
    }
}
