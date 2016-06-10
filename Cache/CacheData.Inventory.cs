using System.Collections.Generic;
using System.Linq;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity
{
    public partial class CacheData
    {
        /// <summary>
        /// Fast Inventory Cache, Self-Updating, Use instead of ZetaDia.Inventory
        /// </summary>
        public class InventoryCache
        {
            static InventoryCache()
            {
                //Pulsator.OnPulse += (sender, args) => Instance.UpdateInventoryCache();
                GameEvents.OnItemLooted += (sender, args) => Instance.UpdateInventoryCache();
                GameEvents.OnItemSalvaged += (sender, args) => Instance.UpdateInventoryCache();
                GameEvents.OnItemSold += (sender, args) => Instance.UpdateInventoryCache();
                GameEvents.OnItemStashed += (sender, args) => Instance.UpdateInventoryCache();
                GameEvents.OnGameJoined += (sender, args) => Instance.UpdateInventoryCache();
                GameEvents.OnWorldChanged += (sender, args) => Instance.UpdateInventoryCache();
            }

            public InventoryCache()
            {
                Clear();

                //// Make sure data is immediately available 
                //// while bot is not running or before pulse starts
                //if (!BotMain.IsRunning)
                //{
                //    using (new MemoryHelper())
                //    {
                //        UpdateInventoryCache();
                //    }
                //}            
            }

            private static InventoryCache _instance;
            public static InventoryCache Instance
            {
                get { return _instance ?? (_instance = new InventoryCache()); }
                set { _instance = value; }
            }

            public List<ACDItem> Backpack { get; private set; }
            public List<ACDItem> Stash { get; private set; }
            public List<ACDItem> Equipped { get; private set; }
            public List<ACDItem> Ground { get; private set; }
            public List<ACDItem> Buyback { get; private set; }
            public List<ACDItem> Other { get; private set; }
            public HashSet<int> EquippedIds { get; private set; }
            public HashSet<int> KanaisCubeIds { get; private set; } 
            public bool IsGroundItemOverload { get; private set; }

            public void UpdateInventoryCache()
            {
                using (new PerformanceLogger("UpdateCachedInventoryData"))
                {
                    Clear();

                    if (!ZetaDia.IsInGame || ZetaDia.Me == null || !ZetaDia.Me.IsValid || ZetaDia.PlayerData == null || !ZetaDia.PlayerData.IsValid)
                        return;

                    

                    //var itemList = ZetaDia.Actors.GetActorsOfType<ACDItem>();
                    // Using backpack only for now, as grabbing all ACDItems is just slow and we don't have a use for them yet.
                    //var itemList = ZetaDia.Me.Inventory.Backpack;
                    //var inventory = ZetaDia.Me.Inventory;
                    //Equipped = inventory.Equipped.ToList();
                    //EquippedIds = new HashSet<int>(Equipped.Select(i => i.ActorSnoId));
                    //Backpack = inventory.Backpack.ToList();
                    //Stash = inventory.StashItems.ToList();

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
                                if ((int)item.InventorySlot == 19)
                                {
                                    Equipped.Add(item);
                                    EquippedIds.Add(item.ActorSnoId);
                                }
                                //Other.Add(item);
                                break;

                        }
                    }

                    //IsGroundItemOverload = (Ground.Count > 50);

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
                Stash = new List<ACDItem>();
                Equipped = new List<ACDItem>();
                EquippedIds = new HashSet<int>();
                KanaisCubeIds = new HashSet<int>();
                Ground = new List<ACDItem>();
                Buyback = new List<ACDItem>();
                Other = new List<ACDItem>();
            }

        }

    }
}