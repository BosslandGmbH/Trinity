using System;
using Trinity.Framework;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot;
using Zeta.Game;

//using Adventurer.Game.Grid;

namespace Trinity.Components.Adventurer.Game.Events
{
    public enum ProfileType
    {
        Unknown,
        Rift,
        Bounty,
        Keywarden
    }

    public static class PluginEvents
    {
        public static ProfileType CurrentProfileType { get; internal set; }
        public static long WorldChangeTime { get; set; }

    }
}