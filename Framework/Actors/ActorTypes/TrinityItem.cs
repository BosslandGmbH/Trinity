using Trinity.Framework.Actors.Properties;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors.ActorTypes
{
    public class TrinityItem : TrinityActor
    {
        public TrinityItem(ACD seed) : base(seed, ActorType.Item)
        {
        }

        public ACDItem ToAcdItem() => CommonData as ACDItem;

        public override void OnCreated()
        {
            CommonProperties.Populate(this);
        }

        public override void OnUpdated()
        {
            if (ToAcdItem()?.InventorySlot == InventorySlot.SharedStash &&
                !ZetaDia.IsInTown)
            {
                return;
            }

            CommonProperties.Update(this);
        }
    }
}
