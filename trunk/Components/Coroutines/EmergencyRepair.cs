using System.Linq;
using System.Threading.Tasks;
using Zeta.Game;

namespace Trinity.Components.Coroutines
{
    public class EmergencyRepair
    {
        public async static Task<bool> Execute()
        {
            var equippedItems = InventoryManager.Equipped.Where(i => i.IsValid);
            bool needEmergencyRepair = false;

            foreach (var item in equippedItems)
            {
                if (item.ACDId == 0) continue;
                float durabilitySum = 0f;
                int itemCount = 0;
                try
                {
                    durabilitySum += item.DurabilityPercent;
                    itemCount++;
                }
                catch
                {
                }
                if (itemCount > 0 && durabilitySum / itemCount < 0.05)
                    needEmergencyRepair = true;
            }
            return needEmergencyRepair;
        }
    }
}