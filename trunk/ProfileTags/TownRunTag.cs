using System.Linq;
using System.Threading.Tasks;
using Trinity.Coroutines.Town;
using Trinity.Framework.Helpers;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("TownRun")]
    [XmlElement("TrinityTownRun")]
    public class TownRunTag : ProfileBehavior
    {
        private bool _isDone;
        public override bool IsDone => _isDone;

        [XmlAttribute("minFreeBagSlots")]
        public int MinFreeBagSlots { get; set; }

        [XmlAttribute("minDurabilityPercent")]
        [XmlAttribute("minDurability")]
        public int MinDurability { get; set; }

        public bool CheckMinBagSlots()
        {
            if (MinFreeBagSlots > 60)
                MinFreeBagSlots = 60;
            var freeSlots = GetFreeSlots();
            Logger.LogDebug("Checking free slots: {0}/{1}", freeSlots, MinFreeBagSlots);
            return freeSlots < MinFreeBagSlots;
        }

        public int GetFreeSlots()
        {
            const int maxFreeSlots = 60;
            int slotsTaken = 0;

            bool participatingInTieredLootRun = ZetaDia.Me.CommonData.GetAttribute<int>(ActorAttributeType.ParticipatingInTieredLootRun) > 0;
            if (participatingInTieredLootRun)
            {
                return maxFreeSlots;
            }

            if (MinFreeBagSlots == 0)
                return maxFreeSlots;

            foreach (var item in ZetaDia.Me.Inventory.Backpack)
            {
                slotsTaken++;
                if (item.IsTwoSquareItem)
                    slotsTaken++;
            }

            return maxFreeSlots - slotsTaken;
        }

        public bool CheckDurability()
        {
            if (MinDurability == 0)
                return false;

            var minDurabilityEquipped = GetMinDurability();
            Logger.LogDebug("Checking minimum durability: {0}/{1}", minDurabilityEquipped, MinDurability);
            return minDurabilityEquipped <= MinDurability;
        }

        public float GetMinDurability() => ZetaDia.Me.Inventory.Equipped.Min(i => i.DurabilityPercent) * 100;

        protected override Composite CreateBehavior() => new ActionRunCoroutine(ret => TownRun());

        public override void OnStart() => Logger.Log("TrinityTownRun, freeBagSlots={0} minDurabilityPercent={1}", MinFreeBagSlots, MinDurability);
    
        private async Task<bool> TownRun() => (CheckDurability() || CheckMinBagSlots()) && (_isDone = true);

        public override void ResetCachedDone()
        {
            _isDone = false;
            base.ResetCachedDone();
        }
    }
}
