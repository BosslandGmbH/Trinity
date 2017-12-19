using Trinity.Framework;
using System.Media;
using Trinity.Framework.Objects;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Game.Internals.Actors;

namespace Trinity.Modules
{
    /// <summary>
    /// Stops the bot when certain conditions are met
    /// </summary>
    public class GameStopper : Module
    {
        protected override int UpdateIntervalMs => 1000;

        public const int RiftStoneSNO = 364715;
        public const int RiftEntryPortalSNO = 345935;
        public const int GreaterRiftEntryPortalSNO = 396751;
        public const int OrekSNO = 363744;
        public const int UrshiSNO = 398682;
        public const int TownstoneSNO = 135248;
        public const int HolyCowSNO = 209133;
        public const int GreaterRiftKeySNO = 408416;
        public const int DeathGateSNO = 328830;

        protected override void OnPulse()
        {
            if (!BotMain.IsRunning)
                return;

            var reasons = Core.Settings.Advanced.StopReasons;
            if (reasons != GameStopReasons.None)
            {
                foreach(var actor in Core.Actors.Actors)
                {
                    if (actor.IsTreasureGoblin && reasons.HasFlag(GameStopReasons.GoblinFound))
                        Stop($"Goblin '{actor.Name}' Found at distance {actor.Distance}");

                    if (actor.ActorSnoId == UrshiSNO && reasons.HasFlag(GameStopReasons.UrshiFound))
                        Stop($"Urshi '{actor.Name}' Found at distance {actor.Distance}");

                    if (actor.ActorSnoId == DeathGateSNO && reasons.HasFlag(GameStopReasons.DeathGateFound))
                        Stop($"Death Gate '{actor.Name}' Found at distance {actor.Distance}");

                    if (actor.MonsterQuality == MonsterQuality.Unique && reasons.HasFlag(GameStopReasons.UniqueFound))
                        Stop($"Unique Monster '{actor.Name}' Found at distance {actor.Distance}");

                }
            }
        }

        private void Stop(string reason)
        {
            Core.Logger.Warn($"游戏停止: {reason}");
            SystemSounds.Exclamation.Play();
            BotMain.Stop();
        }
    }
}

