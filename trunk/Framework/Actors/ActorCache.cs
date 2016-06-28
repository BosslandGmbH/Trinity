using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Helpers;
using Trinity.Objects.Native;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors
{
    /// <summary>
    /// Maintains the current valid list of actors.
    /// Collections are complete, except for bad data / disposed actors.
    /// Filtered lists for attacking/interacting can be found in Core.Targets   
    /// </summary>
    public class ActorCache : Module
    {
        private Stopwatch _timer = new Stopwatch();
        private ExpandoContainer<RActor> _rActorContainer;
        private ExpandoContainer<ActorCommonData> _commonDataContainer;
        private readonly ConcurrentDictionary<int, ActorBase> _rActors = new ConcurrentDictionary<int, ActorBase>();
        private readonly ConcurrentDictionary<int, ActorCommonData> _commonData = new ConcurrentDictionary<int, ActorCommonData>();
        private readonly ConcurrentDictionary<int, TrinityItem> _inventory = new ConcurrentDictionary<int, TrinityItem>();
        private readonly Dictionary<int, short> _annToAcdIndex = new Dictionary<int, short>();

        public ulong LastUpdatedFrame { get; private set; }
        public int ActivePlayerRActorId { get; private set; }

        public TrinityPlayer Me { get; private set; }

        protected override void OnPulse() => Update();

        protected override void OnWorldChanged() => Reset();

        public void Update()
        {
            if (!ZetaDia.IsInGame)
                return;

            UpdateContainers();
            UpdateObjectsFromMemory();
        }

        private void UpdateContainers()
        {
            if (_rActorContainer == null || !_rActorContainer.IsValid || _rActorContainer.IsDisposed)
                _rActorContainer = MemoryWrapper.Create<ExpandoContainer<RActor>>(Internals.Addresses.RActorManager);

            if (_commonDataContainer == null || !_commonDataContainer.IsValid || _commonDataContainer.IsDisposed)
                _commonDataContainer = MemoryWrapper.Create<ExpandoContainer<ActorCommonData>>(Internals.Addresses.AcdManager);
        }        

        private void UpdateObjectsFromMemory()
        {
            var currentFrame = ZetaDia.Memory.Executor.FrameCount;
            if (LastUpdatedFrame == currentFrame)
                return;

            ActivePlayerRActorId = ZetaDia.ActivePlayerRActorId;

            UpdateAcds();

            UpdateRActors();

            UpdateInventory();

            LastUpdatedFrame = currentFrame;
        }

        #region Update Methods

        /// <summary>
        /// Get new Acds from Memory
        /// * Some Acds like backpack/stash/equipped items do not have an associated RActor
        /// </summary>
        private void UpdateAcds()
        {
            _annToAcdIndex.Clear();

            var oldAcds = _commonData.Keys.ToList();
            foreach (var newAcd in _commonDataContainer)
            {
                var acdId = newAcd.AcdId;
                var ann = newAcd.AnnId;

                _commonData.AddOrUpdate(acdId,
                    id => AddAcd(id, newAcd),
                    UpdateAcd);

                oldAcds.Remove(acdId);
                _annToAcdIndex[ann] = (short)acdId;
            }

            foreach (var key in oldAcds)
            {
                ActorCommonData item;
                if (_commonData.TryRemove(key, out item) && item != null && item.IsValid)
                {
                    item.Update(IntPtr.Zero);
                }
            }
        }

        private ActorCommonData AddAcd(int id, ActorCommonData acd)
        {
            return acd;
        }

        private ActorCommonData UpdateAcd(int id, ActorCommonData actorCommonData)
        {
            actorCommonData.Update(actorCommonData.BaseAddress);
            return actorCommonData;
        }


        /// <summary>
        /// Get new RActors from Memory.
        /// * Some RActors like client effects and environment do not have an associated acd.
        /// </summary>
        private void UpdateRActors()
        {
            var untouchedIds = _rActors.Keys.ToList();
            foreach (var newRActor in _rActorContainer)
            {
                var rActorId = newRActor.RActorId;
                if (rActorId == -1)
                    continue;

                var result = true;

                _rActors.AddOrUpdate(rActorId,
                    id => TryAddRActor(id, newRActor, out result),
                    (id, existingActor) => TryUpdateRActor(id, existingActor, newRActor, out result));

                if(result)
                    untouchedIds.Remove(rActorId);
            }

            foreach (var key in untouchedIds)
            {
                ActorBase item;
                if (_rActors.TryRemove(key, out item) && item != null && item.IsValid)
                {
                    item.OnDestroyed();
                    item.RActor?.Update(IntPtr.Zero);
                    item.CommonData?.Update(IntPtr.Zero);
                }
            }
        }

        private ActorBase TryAddRActor(int id, RActor rActor, out bool result)
        {
            _timer.Restart();
            var actor = ActorFactory.CreateActor(rActor);
            var player = actor as TrinityPlayer;
            if (player != null && player.IsMe)
                Me = player;
            _timer.Stop();
            actor.CreateTime = _timer.Elapsed.TotalMilliseconds;

            result = true;
            return actor;
        }

        private ActorBase TryUpdateRActor(int id, ActorBase actor, RActor rActor, out bool result)
        {
            if (!actor.IsRActorValid)
            {
                result = false;
                return actor;
            }

            _timer.Restart();
            actor.RActor.Update(rActor.BaseAddress);
            actor.OnUpdated();
            _timer.Stop();
            actor.UpdateTime = _timer.Elapsed.TotalMilliseconds;

            result = true;
            return actor;
        }

        /// <summary>
        /// Updates the acds that are inventory items, stash/equipped/backpack etc.
        /// </summary>
        private void UpdateInventory()
        {
            var untouchedIds = _inventory.Keys.ToList();
            foreach (var newItem in _commonData)
            {                
                var commonData = newItem.Value;
                var type = commonData.ActorType;
                if (type != ActorType.Item)
                    continue;

                var inventorySlot = commonData.InventorySlot;
                if (inventorySlot == InventorySlot.Merchant || inventorySlot == InventorySlot.None)
                    continue;

                var annId = commonData.AnnId;
                if (annId == -1)
                    continue;

                _inventory.AddOrUpdate(annId,
                    id => AddInventoryItem(id, commonData),
                    (id, existingItem) => UpdateInventoryItem(id, existingItem, commonData));

                untouchedIds.Remove(annId);
            }

            foreach (var key in untouchedIds)
            {
                TrinityItem item;
                if (_inventory.TryRemove(key, out item) && item != null && item.IsValid)
                {
                    item.OnDestroyed();
                    item.RActor?.Update(IntPtr.Zero);
                    item.CommonData?.Update(IntPtr.Zero);
                }
            }
        }

        private TrinityItem AddInventoryItem(int id, ActorCommonData newItem)
        {
            return ActorFactory.CreateFromAcd<TrinityItem>(newItem);
        }

        private TrinityItem UpdateInventoryItem(int id, TrinityItem item, ActorCommonData commonData)
        {
            _timer.Start();
            item.CommonData = commonData;
            item.OnUpdated();
            _timer.Stop();
            item.UpdateTime = _timer.Elapsed.TotalMilliseconds;
            return item;
        }

        #endregion

        #region Lookup Methods

        public IEnumerable<TrinityActor> AllRActors => _rActors.Values.OfType<TrinityActor>().ToList();

        public IEnumerable<T> GetActorsOfType<T>() where T : ActorBase
        {
            return _rActors.Values.OfType<T>();
        }

        public IEnumerable<TrinityItem> Inventory => _inventory.Values.ToList();

        public IEnumerable<TrinityItem> GetInventoryItems(InventorySlot slot)
        {
            return _inventory.Values.Where(i => i.InventorySlot == slot);
        }

        public ActorCommonData GetCommonDataByAnnId(int annId)
        {
            short index;
            if (_annToAcdIndex.TryGetValue(annId, out index))
            {
                var acd = _commonDataContainer[index];
                if (acd != null && acd.IsValid)
                {
                    return acd;
                }
            }
            return null;
        }

        public ActorCommonData GetCommonDataById(int acdId)
        {
            if (acdId == -1)
                return null;

            var acd = _commonDataContainer[(short)acdId];
            if (acd != null && acd.IsValid)
            {
                return acd;
            }
            return null;
        }

        public ACD GetAcdByAnnId(int annId)
        {
            var acd = GetCommonDataByAnnId(annId);
            if (acd != null && acd.IsValid)
            {
                return acd.BaseAddress.UnsafeCreate<ACD>();
            }
            return null;
        }

        public TrinityItem GetItemByAnnId(int annId)
        {
            TrinityItem item;
            if (_inventory.TryGetValue(annId, out item))
            {
                return item;
            }

            short index;
            if (!_annToAcdIndex.TryGetValue(annId, out index))
                return null;

            var acd = _commonDataContainer[index];
            if (acd != null && acd.IsValid)
            {
                var bones = ActorFactory.GetActorSeed(acd);
                return ActorFactory.CreateActor<TrinityItem>(bones);
            }
            return null;
        }

        public ACDItem GetAcdItemByAnnId(int annId)
        {
            var acd = GetCommonDataByAnnId(annId);
            if (acd != null && acd.IsValid)
            {
                return acd.BaseAddress.UnsafeCreate<ACDItem>();
            }
            return null;
        }

        public bool IsAnnIdValid(int annId)
        {
            short index;
            if (!_annToAcdIndex.TryGetValue(annId, out index))
                return false;

            var acd = _commonDataContainer[index];
            return acd != null && acd.IsValid;
        }

        #endregion

        private void Reset()
        {
            _commonDataContainer = null;
            _rActorContainer = null;
            _annToAcdIndex.Clear();
            _commonData.ForEach(o => o.Value.Update(IntPtr.Zero));
            _rActors.ForEach(o => o.Value.RActor.Update(IntPtr.Zero));
            _inventory.Clear();
            _commonData.Clear();
            _rActors.Clear();
            Me = null;
            Update();
        }

    }


}