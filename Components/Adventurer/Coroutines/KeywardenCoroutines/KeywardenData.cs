using System.Linq;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Coroutines.KeywardenCoroutines
{
    public class KeywardenData
    {
        public Act Act { get; set; }
        public int KeywardenSNO { get; set; }
        public int KeySNO { get; set; }
        public int WorldId { get; set; }
        public int LevelAreaId { get; set; }
        public int WaypointNumber { get; set; }
        public SNOBossEncounter BossEncounter { get; set; }

        public bool IsAlive
        {
            get { return !ZetaDia.IsBossEncounterCompleted(BossEncounter); }
        }

        public long MachinesCount
        {
            get
            {
                var stashCount = ZetaDia.Me.Inventory.StashItems.Where(i => i.IsValid && i.ActorSnoId == KeySNO).Sum(i=>i.ItemStackQuantity);
                var backpackCount= ZetaDia.Me.Inventory.Backpack.Where(i => i.IsValid && i.ActorSnoId == KeySNO).Sum(i => i.ItemStackQuantity);
                return stashCount + backpackCount;
            }
        }
    }
}