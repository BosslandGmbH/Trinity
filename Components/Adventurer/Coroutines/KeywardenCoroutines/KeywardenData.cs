using System.Linq;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Coroutines.KeywardenCoroutines
{
    public class KeywardenData
    {
        public Act Act { get; set; }
        public SNOActor KeywardenSNO { get; set; }
        public SNOActor KeySNO { get; set; }
        public SNOWorld WorldId { get; set; }
        public SNOLevelArea LevelAreaId { get; set; }
        public SNOLevelArea WaypointLevelAreaId { get; set; }
        public SNOBossEncounter BossEncounter { get; set; }

        public bool IsAlive => !ZetaDia.Storage.Quests.IsBossEncounterCompleted(BossEncounter);

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