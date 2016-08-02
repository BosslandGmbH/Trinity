using System;
using Trinity.Components.Adventurer.Cache;
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
        public static long WorldChangeTime { get; private set; }
        private static uint _lastUpdate;

        public static long TimeSinceWorldChange
        {
            get
            {
                if (WorldChangeTime == 0)
                {
                    return Int32.MaxValue;
                }
                return PluginTime.CurrentMillisecond - WorldChangeTime;
            }
        }

        public static void GameEvents_OnWorldChanged(object sender, EventArgs e)
        {
            if (!Adventurer.IsAdventurerTagRunning())
            {
                Logger.Debug("[BotEvents] Reseting the grids.");
                ScenesStorage.Reset();
            }
            WorldChangeTime = PluginTime.CurrentMillisecond;
            Logger.Debug("[BotEvents] World has changed to WorldId: {0} LevelAreaSnoIdId: {1}", AdvDia.CurrentWorldId, AdvDia.CurrentLevelAreaId);
            EntryPortals.AddEntryPortal();
        }

        public static void GameEvents_OnGameJoined(object sender, EventArgs e)
        {
            ScenesStorage.Reset();
            //AdvDia.Update();
        }

        public static void OnBotStart(IBot bot)
        {
            Pulsator.OnPulse += Pulsator_OnPulse;
        }

        public static void OnBotStop(IBot bot)
        {
            Pulsator.OnPulse -= Pulsator_OnPulse;
            BountyStatistics.Report();
        }


        private static void Pulsator_OnPulse(object sender, EventArgs e)
        {            
            PulseUpdates();
        }

        public static void PulseUpdates()
        {
            var curFrame = ZetaDia.Memory.Executor.FrameCount;
            if (curFrame == _lastUpdate) return;
            _lastUpdate = curFrame;

            // Trinity uses adventurer scenestorage and grid base 
            ScenesStorage.Update();

            if (!Adventurer.IsAdventurerTagRunning()) return;
            ExplorationGrid.PulseSetVisited();
            BountyStatistics.Pulse();
        }





    }
}
