using Buddy.Coroutines;
using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;


namespace Trinity.DbProvider
{
    public static class DeathHandler
    {
        private static bool _isDead;
        private static int _deathCounter;
        private static int _deathNeedRepairCounter;
        private static DateTime _resButtonsVisibleStart;
        private static bool _resurrectButtonsVisible;
        private static DateTime _corpseReviveAvailableTime;
        private static Vector3 _deathLocation;

        static DeathHandler()
        {
            GameEvents.OnGameJoined += GameEventsOnOnGameJoined;
        }

        private static void GameEventsOnOnGameJoined(object sender, EventArgs eventArgs)
        {
            DeathsThisGame = 0;
        }

        public static DateTime LastDeathTime { get; private set; }
        public static int DeathsThisGame { get; private set; }
        public static int DeathsThisSession { get; private set; }

        public static async Task<bool> Execute()
        {
            if (!ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld || !ZetaDia.Me.IsValid)
                return false;

            if (Core.IsOutOfGame)
                return false;

            var isDead = ZetaDia.Me.IsDead;
            if (_isDead != isDead)
            {
                if (isDead)
                {
                    _deathCounter = LastDeathTime.Subtract(DateTime.UtcNow).TotalSeconds > 60 ? 0 : _deathCounter + 1;
                    _deathNeedRepairCounter = LastDeathTime.Subtract(DateTime.UtcNow).TotalSeconds < 60 && EquipmentNeedsEmergencyRepair(5) ? _deathNeedRepairCounter + 1 : 0;
                    LastDeathTime = DateTime.UtcNow;
                    _resButtonsVisibleStart = DateTime.MinValue;
                    _resurrectButtonsVisible = false;
                    _deathLocation = ZetaDia.Me.Position;
                    DeathsThisSession++;
                    DeathsThisSession++;

                    Core.Logger.Log("[Death] You died lol! RecentDeaths={0} RecentDeathsNeedingRepair={1}", _deathCounter, _deathNeedRepairCounter);
                }
                else
                {
                    Core.Logger.Log("[Death] No Longer Dead");

                    //if (Core.Settings.Combat.Misc.FleeInGhostMode)
                    //{
                    await MoveWhileGhosted();
                    //}

                    if (EquipmentNeedsEmergencyRepair(5))
                    {
                        BrainBehavior.ForceTownrun("[Death] Item Durability - Need to Repair");
                    }
                }
                _isDead = isDead;
            }

            if (!isDead)
            {
                return false;
            }

            var reviveAtCorpseButton = UIElement.FromHash(0xE3CBD66296A39588);
            var reviveAtCheckPointButton = UIElement.FromHash(0xBFAAF48BA9316742);
            var acceptRessurectionButton = UIElement.FromHash(0x712D458486D6F062);
            var reviveInTownButton = UIElement.FromHash(0x7A2AF9C0F3045ADA);
            var needRepair = EquipmentNeedsEmergencyRepair(5);
            var checkpointButtonReady = reviveAtCheckPointButton.IsVisible && reviveAtCheckPointButton.IsEnabled;
            var corpseButtonReady = reviveAtCorpseButton.IsVisible && reviveAtCorpseButton.IsEnabled;
            var townButtonReady = reviveInTownButton.IsVisible && reviveInTownButton.IsEnabled;
            var isInRift = GameData.RiftWorldIds.Contains(ZetaDia.Globals.WorldSnoId);
            var isInGreaterRift = isInRift && ZetaDia.Storage.CurrentRiftType == RiftType.Greater;
            var noMoreCorpseRevives = !isInGreaterRift && ZetaDia.Me.CommonData.CorpseResurrectionCharges == 0;
            var waitingForCorpseResurrect = ZetaDia.Globals.GameTick < ZetaDia.Me.CommonData.CorpseResurrectionAllowedGameTime;
            var deathCount = ZetaDia.Me.CommonData.DeathCount;
            var corpseResurrectDisabled = ZetaDia.Me.CommonData.CorpseResurrectionDisabled > 0;

            if (reviveAtCheckPointButton.IsVisible)
                _resButtonsVisibleStart = DateTime.UtcNow;

            var resurrectButtonsVisible = _resButtonsVisibleStart != DateTime.MinValue;
            if (resurrectButtonsVisible != _resurrectButtonsVisible)
            {
                if (resurrectButtonsVisible)
                {
                    Core.Logger.Verbose("[Death] Buttons are now visible");
                    var maxWaitTime = ZetaDia.Me.IsParticipatingInTieredLootRun ? Math.Min(deathCount * 5, 30) - 2 : 4;
                    _corpseReviveAvailableTime = new DateTime(_resButtonsVisibleStart.Ticks).Add(TimeSpan.FromSeconds(maxWaitTime));
                }
                _resurrectButtonsVisible = resurrectButtonsVisible;
            }

            var remainingTimeSecs = (_corpseReviveAvailableTime - DateTime.UtcNow).TotalSeconds;
            var resLimit = isInGreaterRift ? 16 : 10;
            if (_deathCounter > resLimit && !ZetaDia.IsInTown && needRepair)
            {
                Core.Logger.Log("Durability is zero and {0} deaths within 60s of each other - emergency leave game", deathCount);
                ZetaDia.Service.Party.LeaveGame(true);
                await CommonCoroutines.LeaveGame("Durability is zero");
                _deathCounter = 0;
                return true;
            }

            if (acceptRessurectionButton.IsVisible && acceptRessurectionButton.IsEnabled)
            {
                acceptRessurectionButton.Click();
            }
            else if (IsBeingRevived())
            {
                Core.Logger.Log("[Death] Waiting while being resurrected");
            }
            else if (ZetaDia.Me.IsInBossEncounter && !Core.Rift.IsInRift && IsAlivePlayerNearby)
            {
                Core.Logger.Log("[Death] Waiting because of boss fight");
            }
            else if (corpseButtonReady && !needRepair && !waitingForCorpseResurrect && !noMoreCorpseRevives && !corpseResurrectDisabled)
            {
                Core.Logger.Log("[Death] Reviving at corpse");
                reviveAtCorpseButton.Click();
            }
            else if (townButtonReady && needRepair && _deathNeedRepairCounter > 4)
            {
                Core.Logger.Log("[Death] We've failed few times to resurrect at checkpoint to repair , now resurrecting in town.");
                reviveInTownButton.Click();
            }
            else if (checkpointButtonReady)
            {
                Core.Logger.Log("[Death] Reviving at checkpoint (NeedRepair={0})", needRepair);
                reviveAtCheckPointButton.Click();
            }
            else if (!corpseButtonReady && !checkpointButtonReady && townButtonReady && DateTime.UtcNow.Subtract(LastDeathTime).TotalSeconds > 45)
            {
                Core.Logger.Log("[Death] Reviving in town");
                reviveInTownButton.Click();
            }
            else
            {
                Core.Logger.Verbose("[Death] Waiting...");
            }

            await Coroutine.Sleep(250);
            return true;
        }

        public async static Task<bool> MoveWhileGhosted()
        {
            var safespot = Core.Avoidance.GridEnricher.SafeNodeLayer.Positions.OrderBy(d =>
                d.Distance(Core.Avoidance.GridEnricher.MonsterCentroid) +
                d.Distance(Core.Avoidance.GridEnricher.AvoidanceCentroid)).FirstOrDefault();

            if (safespot == Vector3.Zero)
            {
                Core.Logger.Log("[Death] Unable to find safe spot to escape to :(");
                return false;
            }

            Core.Logger.Log("[Death] Moving away from revive position");

            var timeout = DateTime.UtcNow.AddSeconds(5);
            while (DateTime.UtcNow < timeout && ZetaDia.Me.IsGhosted && !ZetaDia.Me.IsDead)
            {
                Core.Logger.Log($"[Death] Moving away... Distance={_deathLocation.Distance(ZetaDia.Me.Position)}");
                await Navigator.MoveTo(safespot);
                await Coroutine.Yield();
            }
            return true;
        }

        public static bool EquipmentNeedsEmergencyRepair(int durabilityPct)
        {
            var equippedItems = InventoryManager.Equipped.Where(i => i.DurabilityCurrent < i.DurabilityMax).ToList();
            if (!equippedItems.Any())
                return false;

            double max = equippedItems.Max(i => i.DurabilityPercent);
            return max <= durabilityPct;
        }

        public static bool IsBeingRevived()
        {
            var headstones = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true, true).Where(g => g != null && g.ActorInfo.GizmoType == GizmoType.Headstone && g.Distance < 8f).ToList();
            if (!headstones.Any())
                return false;

            var reviver = ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true, true).FirstOrDefault(p => p?.CommonData != null && (GameData.PlayerUseAnimationIds.Contains((int)p.CommonData.CurrentAnimation) || p.CommonData.LoopingAnimationEndTime > 0) && headstones.Any(h => p.Position.Distance(h.Position) < 8f));

            return reviver != null;
        }

        public static bool IsAlivePlayerNearby
        {
            get { return ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true).FirstOrDefault(p => p?.CommonData != null && p.RActorId != Core.Player.RActorGuid && p.Distance < 100f) != null; }
        }
    }
}