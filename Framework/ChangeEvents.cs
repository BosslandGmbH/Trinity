using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Combat;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory;
using Trinity.Modules;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Service;

namespace Trinity.Framework
{
    public static class ChangeEvents
    {
        public static TimeSpan SlowUpdateInterval { get; } = TimeSpan.FromSeconds(3);

        public static TrinityChangeDetector<ActorClass> PlayerClass { get; } = new TrinityChangeDetector<ActorClass>(()
            => ZetaDia.Service.Hero.Class, SlowUpdateInterval);

        public static TrinityChangeDetector<bool> IsInGame { get; } = new TrinityChangeDetector<bool>(()
            => ZetaDia.IsInGame);

        public static TrinityChangeDetector<HashSet<int>> EquippedItems { get; } = new TrinityChangeDetector<HashSet<int>>(()
            => Core.Inventory.EquippedIds, SlowUpdateInterval);

        public static TrinityChangeDetector<List<TrinityItem>> BackpackItems { get; } = new TrinityChangeDetector<List<TrinityItem>>(()
            => Core.Inventory.Backpack, SlowUpdateInterval);

        public static TrinityChangeDetector<HashSet<SNOPower>> Skills { get; } = new TrinityChangeDetector<HashSet<SNOPower>>(()
            => Core.Hotbar.ActivePowers, SlowUpdateInterval);

        public static TrinityChangeDetector<int> WorldId { get; } = new TrinityChangeDetector<int>(()
            => ZetaDia.CurrentWorldSnoId);

        public static TrinityChangeDetector<int> LevelAreaId { get; } = new TrinityChangeDetector<int>(()
            => ZetaDia.CurrentLevelAreaSnoId);

        public static TrinityChangeDetector<int> HeroId { get; } = new TrinityChangeDetector<int>(()
            => ZetaDia.Service.Hero.HeroId);

        public static TrinityChangeDetector<ILootProvider> LootProvider { get; } = new TrinityChangeDetector<ILootProvider>(()
            => Combat.Loot);

        public static TrinityChangeDetector<bool> IsRunning { get; } = new TrinityChangeDetector<bool>(()
            => BotEvents.IsBotRunning);

        public static TrinityChangeDetector<DateTime> ActorsUpdated { get; } = new TrinityChangeDetector<DateTime>(()
            => Core.Actors.LastUpdated);


    }
}


