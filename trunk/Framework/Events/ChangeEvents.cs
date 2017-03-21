using System;
using System.Collections.Generic;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Combat;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Modules;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Events
{
    public static class ChangeEvents
    {
        public static TimeSpan SlowUpdateInterval { get; } = TimeSpan.FromSeconds(3);

        public static TrinityChangeDetector<ActorClass> PlayerClass { get; } = new TrinityChangeDetector<ActorClass>(()
            => ZetaDia.Service.Hero.Class, SlowUpdateInterval);

        public static TrinityChangeDetector<bool> IsInGame { get; } = new TrinityChangeDetector<bool>(()
            => ZetaDia.IsInGame);

        public static TrinityChangeDetector<bool> IsInBossEncounter { get; } = new TrinityChangeDetector<bool>(()
            => ZetaDia.Me.IsInBossEncounter);

        public static TrinityChangeDetector<HashSet<int>> EquippedItems { get; } = new TrinityChangeDetector<HashSet<int>>(()
            => Core.Inventory.EquippedIds, SlowUpdateInterval);

        public static TrinityChangeDetector<IEnumerable<TrinityItem>> BackpackItems { get; } = new TrinityChangeDetector<IEnumerable<TrinityItem>>(()
            => Core.Inventory.Backpack, SlowUpdateInterval);

        public static TrinityChangeDetector<HashSet<SNOPower>> Skills { get; } = new TrinityChangeDetector<HashSet<SNOPower>>(()
            => Core.Hotbar.ActivePowers, SlowUpdateInterval);

        public static TrinityChangeDetector<int> WorldId { get; } = new TrinityChangeDetector<int>(()
            => ZetaDia.Globals.WorldSnoId);

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