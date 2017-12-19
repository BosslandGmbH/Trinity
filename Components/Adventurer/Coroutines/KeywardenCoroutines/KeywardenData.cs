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
        public int WaypointLevelAreaId { get; set; }
        public SNOBossEncounter BossEncounter { get; set; }

        public bool IsAlive
        {
            get { return !ZetaDia.Storage.Quests.IsBossEncounterCompleted(BossEncounter); }
        }

        public long MachinesCount
        {
            get
            {
                var stashCount = InventoryManager.StashItems.Where(i => i.IsValid && i.ActorSnoId == KeySNO).Sum(i => i.ItemStackQuantity);
                var backpackCount = InventoryManager.Backpack.Where(i => i.IsValid && i.ActorSnoId == KeySNO).Sum(i => i.ItemStackQuantity);
                return stashCount + backpackCount;
            }
        }
    }
}