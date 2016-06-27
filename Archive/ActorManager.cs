//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows;
//using Buddy.Coroutines;
//using Trinity.Framework.Helpers;
//using Trinity.Framework.Objects.Memory;
//using Trinity.Framework.Objects.Memory.Containers;
//using Trinity.Framework.Objects.Memory.Misc;
//using Trinity.Helpers;
//using Trinity.Objects.Native;
//using Trinity.Technicals;
//using Zeta.Bot;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;
//using Zeta.Game.Internals.Service;
//using Zeta.Game.Internals.SNO;
//using static Trinity.Framework.Actors.ActorManager;
//using ThreadState = System.Threading.ThreadState;

//namespace Trinity.Framework.Actors
//{
//    /// <summary>
//    /// Reads actors from memory
//    /// </summary>
//    public class ActorManager : Module
//    {
//        public uint LastUpdatedFrame;
//        public bool IsStarted;
//        private Dictionary<int, TrinityItem> _currentTrinityItems = new Dictionary<int, TrinityItem>();
//        private Dictionary<int, short> _annToAcdIndex = new Dictionary<int, short>();
//        private readonly HashSet<int> IgnoreAcdIds = new HashSet<int>();
//        private ExpandoContainer<ActorCommonData> _actors;
//        public int TickDelayMs;
//        private int _currentWorldSnoId;
        
//        public List<TrinityItem> Items { get; private set; } = new List<TrinityItem>();
//        public HashSet<int> AnnIds { get; private set; } = new HashSet<int>();
//        public bool IsDisposed => ZetaDia.Memory.Read<int>(_actors.BaseAddress + 0x130 + 0x18) != 1611526157;

//        public ActorManager()
//        {
//            GameEvents.OnGameJoined += GameEventsOnGameJoined;
//        }

//        private void GameEventsOnGameJoined(object sender, EventArgs eventArgs)
//        {
//            Reset();
//            Update();
//        }

//        protected override int UpdateIntervalMs => Core.Player.IsInTown ? 0 : 200;

//        protected override void OnPulse()
//        {
//            Update();
//        }

//        public void Update()
//        {
//            using (new PerformanceLogger("ActorManager.Update"))
//            {
//                try
//                {
//                    var currentFrame = ZetaDia.Memory.Executor.FrameCount;
//                    if (LastUpdatedFrame == currentFrame)
//                        return;

//                    var items = ReadItems();
//                    if (items.Any())
//                    {
//                        Items = items;
//                    }

//                    LastUpdatedFrame = currentFrame;
//                }
//                catch (Exception ex)
//                {
//                    Logger.Log("Exception {0}", ex);
//                }
//            }
//        }

//        private List<TrinityItem> ReadItems()
//        {
//            var newTrinityItems = new Dictionary<int, TrinityItem>();
//            var annToAcdIndex = new Dictionary<int, short>();
//            var validAnnIds = new HashSet<int>();

//            var worldSnoId = ZetaDia.CurrentWorldSnoId;
//            if (worldSnoId != _currentWorldSnoId)
//            {
//                _currentTrinityItems.Clear();
//                IgnoreAcdIds.Clear();
//                _currentWorldSnoId = worldSnoId;
//            }

//            if (_actors == null || IsDisposed)
//            {
//                _actors = MemoryWrapper.Create<ExpandoContainer<ActorCommonData>>(Internals.Addresses.AcdManager);
//                _currentTrinityItems.Clear();
//                Items.Clear();
//                Thread.Sleep(100);
//                return new List<TrinityItem>();
//            }

//            var inTown = ZetaDia.IsInTown;

//            foreach (var acd in _actors)
//            {
//                if (acd == null)
//                    continue;

//                var id = acd.AcdId;
//                if (id == -1)
//                    continue;

//                if (IgnoreAcdIds.Contains(id))
//                    continue;

//                if (!acd.IsValid || acd.IsDisposed)
//                {
//                    IgnoreAcdIds.Add(id);
//                    continue;
//                }

//                if (acd.GameBalanceType != GameBalanceType.Items)
//                {
//                    IgnoreAcdIds.Add(id);
//                    continue;
//                }

//                var slot = acd.InventorySlot;
//                if (slot != InventorySlot.BackpackItems && 
//                    slot != InventorySlot.None && // Ground
//                    (!inTown || slot != InventorySlot.SharedStash))
//                {
//                    IgnoreAcdIds.Add(id);
//                    continue;
//                }

//                var annId = acd.AnnId;

//                TrinityItem item;
//                if (_currentTrinityItems.TryGetValue(id, out item))
//                {
//                    item.LastUpdatedTime = DateTime.UtcNow;
//                    item.LastUpdatedFrame = LastUpdatedFrame;
//                    item.Update(acd);
//                    newTrinityItems.Add(id, item);
//                    validAnnIds.Add(annId);
//                    annToAcdIndex.Add(annId, (short)id);
//                    _currentTrinityItems.Remove(id);
//                    continue;
//                }

//                item = new TrinityItem(acd)
//                {
//                    LastUpdatedTime = DateTime.UtcNow,
//                    LastUpdatedFrame = LastUpdatedFrame
//                };

//                newTrinityItems.Add(id, item);
//                validAnnIds.Add(annId);
//                annToAcdIndex.Add(annId, (short)id);
//                _currentTrinityItems.Remove(id);
//            }

//            foreach (var item in _currentTrinityItems.ToList())
//            {
//                item.Value.OnDestroyed();
//                _currentTrinityItems.Remove(item.Key);
//            }

//            AnnIds = validAnnIds;
//            _currentTrinityItems = newTrinityItems;
//            _annToAcdIndex = annToAcdIndex;
//            return _currentTrinityItems.Values.ToList();
//        }

//        public ActorCommonData GetAcdByAnnId(int annId)
//        {
//            short index;
//            if (_annToAcdIndex.TryGetValue(annId, out index))
//            {
//                return _actors[index];
//            }
//            Logger.LogVerbose("Lookup AnnToAcd failed");
//            return null;
//        }

//        public TrinityItem GetItemByAnnId(int annId)
//        {
//            short index;
//            if (_annToAcdIndex.TryGetValue(annId, out index))
//            {
//                var acd = _actors[index];
//                TrinityItem item;

//                if (_currentTrinityItems.TryGetValue(acd.AcdId, out item))
//                {
//                    return item;
//                }

//                Logger.LogVerbose("Failed to find existing TrinityItem");
//                return new TrinityItem(_actors[index]);
//            }

//            //todo figure out AnnToAcd table - result isn't a pointer, can't find the number it produces anywhere.
//            //AnnId == (short)AnnId == Index of AnnToAcd array.
//            //AcdId == (short)AcdId == Index of ACD collection.
//            //AnnToAcd[(short)annId] is not an index in ACD Collection, maybe it needs transform? Index to pointer somewhere else?
//            //When AcdIds change in Actormanager the new AcdId (short) form of both old and new ids still reference the same row index.            

//            Logger.LogVerbose("Lookup AnnToAcd failed");
//            return _currentTrinityItems.Values.FirstOrDefault(i => i.AnnId == annId);
//        }

//        public ACDItem GetAcdItemByAnnId(int annId)
//        {
//            short index;
//            if (_annToAcdIndex.TryGetValue(annId, out index))
//            {
//                var acd = _actors[index];
//                if (acd != null && acd.IsValid)
//                {
//                    return acd.BaseAddress.UnsafeCreate<ACDItem>();
//                }
//            }

//            Logger.LogVerbose("Lookup AnnToAcd failed");
//            return null;
//        }

//        public bool IsAnnIdValid(int annId)
//        {
//            short index;
//            if (!_annToAcdIndex.TryGetValue(annId, out index))
//                return false;

//            var acd = _actors[index];
//            return acd != null && acd.IsValid;
//        }

//        public void Reset()
//        {
//            ZetaDia.Actors.Clear();
//            ZetaDia.Actors.Update();
//            Items.Clear();
//            LastUpdatedFrame = 0;
//            IgnoreAcdIds.Clear();
//            AnnIds.Clear();
//            _currentTrinityItems.Clear();
//            _annToAcdIndex.Clear();
//            _currentWorldSnoId = 0;
//            _actors = null;
//        }

//    }
//}
