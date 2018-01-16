using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Actors
{
    public static class ActorExtensions
    {
        public static InventorySlot GetInventorySlot(this ACD acd)
        {
            return ZetaDia.Memory.Read<InventorySlot>(acd.BaseAddress + 0x114);
        }
    }
}
