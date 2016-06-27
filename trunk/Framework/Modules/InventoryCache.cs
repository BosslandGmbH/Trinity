using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Modules
{
    public class InventoryCache : Module
    {
        public InventoryCache()
        {
            GameEvents.OnItemLooted += (sender, args) => Update();
            GameEvents.OnItemSalvaged += (sender, args) => Update();
            GameEvents.OnItemSold += (sender, args) => Update();
            GameEvents.OnItemStashed += (sender, args) => Update();
            GameEvents.OnGameJoined += (sender, args) => Update();
            GameEvents.OnWorldChanged += (sender, args) => Update();
        }

        public HashSet<int> KanaisCubeIds { get; private set; } = new HashSet<int>();
        public List<TrinityItem> Backpack { get; private set; } = new List<TrinityItem>();
        public HashSet<int> EquippedIds { get; private set; } = new HashSet<int>();
        public List<TrinityItem> Equipped { get; private set; } = new List<TrinityItem>();

        //public List<ACDItem> Stash { get; private set; }
        //public List<ACDItem> Ground { get; private set; }
        //public List<ACDItem> Buyback { get; private set; }
        //public List<ACDItem> Other { get; private set; }

        protected override void OnPulse()
        {
            Update();
        }

        public void Update()
        {
            using (new PerformanceLogger("UpdateCachedInventoryData"))
            {
                Clear();

                if (!ZetaDia.IsInGame)
                    return;

                KanaisCubeIds = new HashSet<int>(ZetaDia.PlayerData.KanaisPowersAssignedActorSnoIds);

                foreach (var item in Core.Actors.Inventory)
                {
                    if (!item.IsValid)
                        continue;

                    switch (item.InventorySlot)
                    {
                        case InventorySlot.BackpackItems:
                            Backpack.Add(item);
                            break;

                        //case InventorySlot.SharedStash:
                        //    Stash.Add(item);
                        //    break;

                        case InventorySlot.Bracers:
                        case InventorySlot.Feet:
                        case InventorySlot.Hands:
                        case InventorySlot.Head:
                        case InventorySlot.Waist:
                        case InventorySlot.Shoulders:
                        case InventorySlot.Torso:
                        case InventorySlot.LeftFinger:
                        case InventorySlot.RightFinger:
                        case InventorySlot.RightHand:
                        case InventorySlot.LeftHand:
                        case InventorySlot.Legs:
                        case InventorySlot.Neck:
                        case InventorySlot.Socket:
                            Equipped.Add(item);
                            EquippedIds.Add(item.ActorSnoId);
                            break;

                        //case InventorySlot.Buyback:
                        //case InventorySlot.None:
                        default:
                            if ((int)item.InventorySlot == 19)
                            {
                                Equipped.Add(item);
                                EquippedIds.Add(item.ActorSnoId);
                            }
                            //Other.Add(item);
                            break;
                    }
                }
            }
        }

        public void Clear()
        {            
            Backpack.Clear();
            Equipped.Clear();
            EquippedIds.Clear();
            KanaisCubeIds.Clear();
        }


    }
}
