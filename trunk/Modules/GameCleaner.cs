using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Exploration.SceneMapping;
using Trinity.Framework;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Memory;
using Zeta.Game;

namespace Trinity.Modules
{
    public class GameCleaner : Module
    {
        protected override void OnGameLeft()
        {
            ZetaDia.Actors.Clear();
            ZetaDia.Actors.Update();
            GameBalanceHelper.Cache.Clear();
            Core.Actors.Clear();
            Core.Hotbar.Clear();
            Core.Inventory.Clear();
            Core.Buffs.Clear();
            Core.Targets.Clear();
            Core.Avoidance.Clear();
            QuestCache.Clear();
        }
    }
}
