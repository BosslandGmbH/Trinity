using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity
{
    public class InventoryCache
    {
        public InventoryCache()
        {
            GameEvents.OnItemLooted += (sender, args) => Update(true);
            GameEvents.OnItemSalvaged += (sender, args) => Update(true);
            GameEvents.OnItemSold += (sender, args) => Update(true);
            GameEvents.OnItemStashed += (sender, args) => Update(true);
            GameEvents.OnGameJoined += (sender, args) => Update(true);
            GameEvents.OnWorldChanged += (sender, args) => Update(true);
        }

        public HashSet<int> KanaisCubeIds { get; private set; } = new HashSet<int>();
        public List<ACDItem> Backpack { get; private set; } = new List<ACDItem>();
        public HashSet<int> EquippedIds { get; private set; } = new HashSet<int>();
        public List<ACDItem> Equipped { get; private set; } = new List<ACDItem>();

        //public List<ACDItem> Stash { get; private set; }
        //public List<ACDItem> Ground { get; private set; }
        //public List<ACDItem> Buyback { get; private set; }
        //public List<ACDItem> Other { get; private set; }

        
        private DateTime LastUpdate = DateTime.MinValue;

        public void Update(bool force = false)
        {
            if (!force && DateTime.UtcNow.Subtract(LastUpdate).TotalMilliseconds < 2500)
                return;

            LastUpdate = DateTime.UtcNow;

            using (new PerformanceLogger("UpdateCachedInventoryData"))
            {
                Clear();

                if (!ZetaDia.IsInGame || ZetaDia.Me == null || !ZetaDia.Me.IsValid || ZetaDia.PlayerData == null || !ZetaDia.PlayerData.IsValid)
                    return;

                KanaisCubeIds = new HashSet<int>(ZetaDia.PlayerData.KanaisPowersAssignedActorSnoIds);                

                // this turns out to be much faster than accessing ZetaDia.Me.Inventory
                foreach (var item in ZetaDia.Actors.GetActorsOfType<ACDItem>().ToList())
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
                            if ((int) item.InventorySlot == 19)
                            {
                                Equipped.Add(item);
                                EquippedIds.Add(item.ActorSnoId);
                            }
                            //Other.Add(item);
                            break;
                    }
                }

                //Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement,
                //    "Refreshed Inventory: Backpack={0} Stash={1} Equipped={2} Ground={3}",
                //    Backpack.Count,
                //    Stash.Count,
                //    Equipped.Count,
                //    Ground.Count);
            }
        }

        public void Clear()
        {
            Backpack = new List<ACDItem>();
            //Stash = new List<ACDItem>();
            Equipped = new List<ACDItem>();
            EquippedIds = new HashSet<int>();
            KanaisCubeIds = new HashSet<int>();
            //Ground = new List<ACDItem>();
            //Buyback = new List<ACDItem>();
            //Other = new List<ACDItem>();
        }


    }
}