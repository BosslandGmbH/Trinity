using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Buddy.Coroutines;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Helpers;
using Trinity.Objects.Native;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Service;
using Zeta.Game.Internals.SNO;
using ThreadState = System.Threading.ThreadState;

namespace Trinity.Framework.Actors
{
    public static class ActorManager
    {
        public static uint LastUpdatedFrame;
        public static bool IsStarted;
        private static Dictionary<int, CachedItem> _currentCachedItems = new Dictionary<int, CachedItem>();
        private static Dictionary<int, short> _annToAcdIndex = new Dictionary<int, short>();
        private static readonly HashSet<int> IgnoreAcdIds = new HashSet<int>();
        private static ExpandoContainer<ActorCommonData> Actors;
        public static int TickDelayMs;
        private static int _currentWorldSnoId;

        static ActorManager()
        {
            Pulsator.OnPulse += (sender, args) => Update();        
        }

        private static int[] AnnToAcd => MemoryWrapper.ReadArray<int>(Actors.BaseAddress + 0x04, 8764);
        public static List<CachedItem> Items { get; private set; } = new List<CachedItem>();
        public static HashSet<int> AnnIds { get; private set; } = new HashSet<int>();
        public static bool IsDisposed => ZetaDia.Memory.Read<int>(Actors.BaseAddress + 0x130 + 0x18) != 1611526157;

        public static void Update()
        {
            try
            {

                var currentFrame = ZetaDia.Memory.Executor.FrameCount;
                if (LastUpdatedFrame == currentFrame)
                    return;

                var items = ReadItems();
                if (items.Any())
                {
                    Items = items;
                }

                LastUpdatedFrame = currentFrame;
            }
            catch (Exception ex)
            {
                Logger.Log("Exception {0}", ex);
            }
        }

        private static List<CachedItem> ReadItems()
        {
            var newCachedItems = new Dictionary<int, CachedItem>();
            var annToAcdIndex = new Dictionary<int, short>();
            var validAnnIds = new HashSet<int>();

            var worldSnoId = ZetaDia.CurrentWorldSnoId;
            if (worldSnoId != _currentWorldSnoId)
            {
                _currentCachedItems.Clear();
                IgnoreAcdIds.Clear();
                _currentWorldSnoId = worldSnoId;
            }

            if (Actors == null || IsDisposed)
            {
                Actors = MemoryWrapper.Create<ExpandoContainer<ActorCommonData>>(Internals.Addresses.AcdManager);
                _currentCachedItems.Clear();
                Items.Clear();
                Thread.Sleep(100);
                return new List<CachedItem>();
            }

            var inTown = ZetaDia.IsInTown;

            foreach (var acd in Actors)
            {
                if (acd == null)
                    continue;

                var id = acd.AcdId;
                if (id == -1)
                    continue;

                if (IgnoreAcdIds.Contains(id))
                    continue;

                if (!acd.IsValid || acd.IsDisposed)
                {
                    IgnoreAcdIds.Add(id);
                    continue;
                }

                if (acd.GameBalanceType != GameBalanceType.Items)
                {
                    IgnoreAcdIds.Add(id);
                    continue;
                }

                var slot = acd.InventorySlot;
                if (slot != InventorySlot.BackpackItems && slot != InventorySlot.BaseHealthPotion &&
                    slot != InventorySlot.None && // Ground
                    (!inTown || slot != InventorySlot.SharedStash))
                {
                    IgnoreAcdIds.Add(id);
                    continue;
                }

                var annId = acd.AnnId;

                CachedItem item;
                if (_currentCachedItems.TryGetValue(id, out item))
                {
                    item.LastUpdatedTime = DateTime.UtcNow;
                    item.LastUpdatedFrame = LastUpdatedFrame;
                    item.Update(acd);
                    newCachedItems.Add(id, item);
                    validAnnIds.Add(annId);
                    annToAcdIndex.Add(annId, (short)id);
                    _currentCachedItems.Remove(id);
                    continue;
                }

                item = new CachedItem(acd);
                item.LastUpdatedTime = DateTime.UtcNow;
                item.LastUpdatedFrame = LastUpdatedFrame;
                newCachedItems.Add(id, item);
                validAnnIds.Add(annId);
                annToAcdIndex.Add(annId, (short)id);
                _currentCachedItems.Remove(id);
            }

            foreach (var item in _currentCachedItems.ToList())
            {
                item.Value.OnDestroyed();
                _currentCachedItems.Remove(item.Key);
            }

            AnnIds = validAnnIds;
            _currentCachedItems = newCachedItems;
            _annToAcdIndex = annToAcdIndex;
            return _currentCachedItems.Values.ToList();
        }

        public static ActorCommonData GetAcdByAnnId(int annId)
        {
            short index;
            if (_annToAcdIndex.TryGetValue(annId, out index))
            {
                return Actors[index];
            }
            Logger.LogVerbose("Lookup AnnToAcd failed");
            return null;
        }

        public static CachedItem GetItemByAnnId(int annId)
        {
            short index;
            if (_annToAcdIndex.TryGetValue(annId, out index))
            {
                var acd = Actors[index];
                CachedItem item;

                if (_currentCachedItems.TryGetValue(acd.AcdId, out item))
                {
                    return item;
                }

                Logger.LogVerbose("Failed to find existing CachedItem");
                return new CachedItem(Actors[index]);
            }

            //todo figure out AnnToAcd table - result isn't a pointer, can't find the number it produces anywhere.
            //AnnId == (short)AnnId == Index of AnnToAcd array.
            //AcdId == (short)AcdId == Index of ACD collection.
            //AnnToAcd[(short)annId] is not an index in ACD Collection, maybe it needs transform? Index to pointer somewhere else?
            //When AcdIds change in Actormanager the new AcdId (short) form of both old and new ids still reference the same row index.            

            Logger.LogVerbose("Lookup AnnToAcd failed");
            return _currentCachedItems.Values.FirstOrDefault(i => i.AnnId == annId);
        }

        public static ACDItem GetAcdItemByAnnId(int annId)
        {
            short index;
            if (_annToAcdIndex.TryGetValue(annId, out index))
            {
                var acd = Actors[index];
                if (acd != null && acd.IsValid)
                {
                    return acd.BaseAddress.UnsafeCreate<ACDItem>();
                }
            }

            Logger.LogVerbose("Lookup AnnToAcd failed");
            return null;
        }

        public static bool IsAnnIdValid(int annId)
        {
            short index;
            if (!_annToAcdIndex.TryGetValue(annId, out index))
                return false;

            var acd = Actors[index];
            return acd != null && acd.IsValid;
        }


    }
}
