using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Modules;
using Trinity.Framework.Objects.Memory;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Service;

namespace Trinity.Framework
{
    public static class ChangeEvents
    {
        public static TimeSpan SlowUpdateInterval { get; } = TimeSpan.FromSeconds(3);

        public static TrinityChangeDetector<ActorClass> PlayerClass = new TrinityChangeDetector<ActorClass>(()
            => ZetaDia.Service.Hero.Class, SlowUpdateInterval);

        public static TrinityChangeDetector<bool> IsInGame = new TrinityChangeDetector<bool>(()
            => ZetaDia.IsInGame);

        //public static TrinityChangeDetector<GameState> GameState = new TrinityChangeDetector<GameState>(()
        //    => Core.MemoryModel.GameInfo.GameState);

        public static TrinityChangeDetector<HashSet<int>> EquippedItems = new TrinityChangeDetector<HashSet<int>>(()
            => Core.Inventory.EquippedIds, SlowUpdateInterval);

        public static TrinityChangeDetector<HashSet<SNOPower>> Skills = new TrinityChangeDetector<HashSet<SNOPower>>(()
            => Core.Hotbar.ActivePowers, SlowUpdateInterval);

    }
}


