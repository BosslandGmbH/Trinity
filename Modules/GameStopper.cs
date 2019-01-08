using Trinity.Framework;
using System.Media;
using Trinity.Framework.Objects;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Modules
{
    /// <summary>
    /// Stops the bot when certain conditions are met
    /// </summary>
    public class GameStopper : Module
    {
        protected override int UpdateIntervalMs => 1000;

        public const SNOActor RiftStoneSNO = SNOActor.x1_OpenWorld_LootRunObelisk_B;
        public const SNOActor RiftEntryPortalSNO = SNOActor.X1_OpenWorld_LootRunPortal;
        public const SNOActor GreaterRiftEntryPortalSNO = SNOActor.X1_OpenWorld_Tiered_Rifts_Portal;
        public const SNOActor OrekSNO = SNOActor.X1_LR_Nephalem;
        public const SNOActor UrshiSNO = SNOActor.P1_LR_TieredRift_Nephalem;
        public const SNOActor TownstoneSNO = SNOActor.Dungeon_Stone_Portal;
        public const SNOActor HolyCowSNO = SNOActor.TentacleLord;
        public const SNOActor GreaterRiftKeySNO = SNOActor.TieredLootrunKey_0;
        public const SNOActor DeathGateSNO = SNOActor.x1_Fortress_Portal_Switch;

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
            Core.Logger.Warn($"Game Stopped: {reason}");
            SystemSounds.Exclamation.Play();
            BotMain.Stop();
        }
    }
}

