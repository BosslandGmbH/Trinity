using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Technicals;
using TrinityCoroutines.Resources;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace Trinity.DbProvider
{
    public static class DeathHandler
    {
        private static bool _isDead;
        private static DateTime _deathTime;
        private static int _deathCounter;
        private static int _deathNeedRepairCounter;
        private static DateTime _resButtonsVisibleStart;
        private static bool _resurrectButtonsVisible;
        private static DateTime _corpseReviveAvailableTime;

        public static async Task<bool> Execute()
        {
            var isDead = ZetaDia.Me.IsDead;
            if (_isDead != isDead)
            {
                if (isDead)
                {                    
                    _deathCounter = _deathTime.Subtract(DateTime.UtcNow).TotalSeconds > 60 ? 0 : _deathCounter +1;
                    _deathNeedRepairCounter = _deathTime.Subtract(DateTime.UtcNow).TotalSeconds < 60 && EquipmentNeedsEmergencyRepair() ? _deathNeedRepairCounter + 1 : 0;
                    _deathTime = DateTime.UtcNow;
                    _resButtonsVisibleStart = DateTime.MinValue;
                    _resurrectButtonsVisible = false;

                    Logger.Log("[Death] You died lol! RecentDeaths={0} RecentDeathsNeedingRepair={1}", 
                        _deathCounter, _deathNeedRepairCounter);
                }
                else
                {
                    if (EquipmentNeedsEmergencyRepair())
                        BrainBehavior.ForceTownrun("[Death] Item Durability - Need to Repair");
                }
                _isDead = isDead;
            }

            if (!isDead)
                return false;

            var reviveAtCorpseButton = UIElement.FromHash(0xE3CBD66296A39588);
            var reviveAtCheckPointButton = UIElement.FromHash(0xBFAAF48BA9316742);
            var acceptRessurectionButton = UIElement.FromHash(0x712D458486D6F062);
            var reviveInTownButton = UIElement.FromHash(0x7A2AF9C0F3045ADA);
            var needRepair = EquipmentNeedsEmergencyRepair();
            var checkpointButtonReady = reviveAtCheckPointButton.IsVisible && reviveAtCheckPointButton.IsEnabled;
            var corpseButtonReady = reviveAtCorpseButton.IsVisible && reviveAtCorpseButton.IsEnabled;
            var townButtonReady = reviveInTownButton.IsVisible && reviveInTownButton.IsEnabled;
            var isInRift = DataDictionary.RiftWorldIds.Contains(ZetaDia.CurrentWorldSnoId);
            var isInGreaterRift = isInRift && ZetaDia.CurrentRift != null && ZetaDia.CurrentRift.Type == RiftType.Greater;
            var noMoreCorpseRevives = !isInGreaterRift && ZetaDia.Me.CommonData.CorpseResurrectionCharges == 0;
            var waitingForCorpseResurrect = ZetaDia.CurrentTime < ZetaDia.Me.CommonData.CorpseResurrectionAllowedGameTime;
            var deathCount = ZetaDia.Me.CommonData.DeathCount;
            var corpseResurrectDisabled = ZetaDia.Me.CommonData.CorpseResurrectionDisabled > 0;

            if (reviveAtCheckPointButton.IsVisible)
                _resButtonsVisibleStart = DateTime.UtcNow;
           
            var resurrectButtonsVisible = _resButtonsVisibleStart != DateTime.MinValue;
            if (resurrectButtonsVisible != _resurrectButtonsVisible)
            {                
                if (resurrectButtonsVisible)
                {
                    Logger.LogVerbose("[Death] Buttons are now visible");
                    var maxWaitTime = ZetaDia.Me.IsParticipatingInTieredLootRun ? Math.Min(deathCount * 5, 30) -2 : 4;
                    _corpseReviveAvailableTime = new DateTime(_resButtonsVisibleStart.Ticks).Add(TimeSpan.FromSeconds(maxWaitTime));
                }
                _resurrectButtonsVisible = resurrectButtonsVisible;
            }                

            var remainingTimeSecs = (_corpseReviveAvailableTime - DateTime.UtcNow).TotalSeconds;
            var resLimit = isInGreaterRift ? 16 : 10;
            if (_deathCounter > resLimit && !ZetaDia.IsInTown && needRepair)
            {
                Logger.Log("Durability is zero and {0} deaths within 60s of each other - emergency leave game", deathCount);
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
                Logger.Log("[Death] Waiting while being resurrected");
            }
            else if (corpseButtonReady && !needRepair && !waitingForCorpseResurrect && !noMoreCorpseRevives && !corpseResurrectDisabled)
            {
                Logger.Log("[Death] Reviving at corpse");
                reviveAtCorpseButton.Click();
            }
            else if (townButtonReady && needRepair && _deathNeedRepairCounter > 4)
            {
                Logger.Log("[Death] We've failed few times to resurrect at checkpoint to repair , now resurrecting in town.");
                reviveInTownButton.Click();
            }
            else if (checkpointButtonReady)
            {
                Logger.Log("[Death] Reviving at checkpoint (NeedRepair={0})", needRepair);
                reviveAtCheckPointButton.Click();
            }
            else if (!corpseButtonReady && !checkpointButtonReady && townButtonReady && DateTime.UtcNow.Subtract(_deathTime).TotalSeconds > 45)
            {
                Logger.Log("[Death] Reviving in town");
                reviveInTownButton.Click();
            }
            else
            {                
                Logger.LogVerbose("[Death] Waiting...");
            }

            await Coroutine.Sleep(250);
            return true;
        }

        public static bool EquipmentNeedsEmergencyRepair()
        {
            var equippedItems = ZetaDia.Me.Inventory.Equipped.Where(i => i.DurabilityCurrent < i.DurabilityMax).ToList();
            if (!equippedItems.Any())
                return false;

            double max = equippedItems.Max(i => i.DurabilityPercent);
            return max <= 1;
        }

        public static bool IsBeingRevived()
        {
            var headstones = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true, true).Where(g => g.ActorInfo.GizmoType == GizmoType.Headstone && g.Distance < 8f).ToList();
            if (!headstones.Any())
                return false;

            var reviver = ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true, true).FirstOrDefault(p =>
                (DataDictionary.PlayerUseAnimationIds.Contains((int)p.CommonData.CurrentAnimation) || p.CommonData.LoopingAnimationEndTime > 0) &&
                headstones.Any(h => p.Position.Distance(h.Position) < 8f));

            return reviver != null;
        }

    }
}
