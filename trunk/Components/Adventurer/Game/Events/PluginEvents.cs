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

        private static uint _lastUpdate;


        public static void GameEvents_OnWorldChanged(object sender, EventArgs e)
        {

        }

        public static void GameEvents_OnGameJoined(object sender, EventArgs e)
        {

        }

        public static void OnBotStart(IBot bot)
        {

        }

        public static void OnBotStop(IBot bot)
        {
            BountyStatistics.Report();
        }



    }
}