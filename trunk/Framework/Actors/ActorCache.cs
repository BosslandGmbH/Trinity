using System;
using System.Collections;
using Trinity.Framework.Helpers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
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
    public class ActorCache : Module, IEnumerable<TrinityActor>
    {
        private Stopwatch _timer = new Stopwatch();
        public ConcurrentDictionary<int, TrinityActor> RActorsByRActorId { get; private set; } = new ConcurrentDictionary<int, TrinityActor>();
        public ConcurrentDictionary<int, TrinityItem> ItemsByAnnId { get; private set; } = new ConcurrentDictionary<int, TrinityItem>();
        public Dictionary<int, int> _acdToRActorIndex { get; private set; } = new Dictionary<int, int>();
        public TrinityPlayer Me { get; private set; }
        public uint LastUpdatedFrame { get; private set; }
        public int ActivePlayerAcdId { get; set; }
        public Vector3 ActivePlayerPosition { get; set; }
        public int ActivePlayerRActorId { get; set; }

        protected override void OnPulse() => Update();

        public void Update()
        {
            if (!ZetaDia.IsInGame)
            {
                if (RActorsByRActorId.Any())
                {
                    Clear();
                }
                return;
            }

            if (!RActorsByRActorId.Any())
                ZetaDia.Actors.Update();

            UpdateInternal();
        }

        private void UpdateInternal()
        {
            var currentFrame = ZetaDia.Memory.Executor.FrameCount;
            if (LastUpdatedFrame == currentFrame)
                return;

            ActivePlayerRActorId = ZetaDia.ActivePlayerRActorId;
            ActivePlayerAcdId = ZetaDia.ActivePlayerACDId;
            ActivePlayerPosition = ZetaDia.RActors[(short)ActivePlayerRActorId].Position;

            UpdateRActors();
            UpdateInventory();

            LastUpdatedFrame = currentFrame;
        }

        #region Update Methods

        private void UpdateRActors()
        {
            _acdToRActorIndex.Clear();

            var untouchedIds = new List<int>(RActorsByRActorId.Keys.ToList());

            foreach (var rActor in ZetaDia.RActors)
            {
                var rActorId = rActor.RActorId;
                if (rActorId == -1)
                    continue;
    
                TrinityActor oldValue;
                if (RActorsByRActorId.TryGetValue(rActorId, out oldValue))
                {             
                    var newValue = UpdateRActor(rActorId, oldValue, rActor);
                    if (newValue == null)
                        continue;

                    RActorsByRActorId.TryUpdate(rActorId, newValue, oldValue);                    
                }
                else
                {
                    var newValue = AddRActor(rActorId, rActor);
                    if (newValue == null)
                        continue;
                    
                    RActorsByRActorId.TryAdd(rActorId, newValue);                    
                }

                untouchedIds.Remove(rActorId);
            }

            foreach (var key in untouchedIds)
            {
                TrinityActor item;
                if (RActorsByRActorId.TryRemove(key, out item) && item != null && item.IsValid)
                {
                    item.OnDestroyed();
                    item.RActor = null;
                    item.CommonData = null;
                }
            }
        }

        private TrinityActor AddRActor(int id, DiaObject rActor)
        {
            _timer.Restart();
            var actor = ActorFactory.CreateActor(rActor, id);
            if (actor == null)
                return null;

            if (actor.RActorId == ActivePlayerRActorId)
                Me = (TrinityPlayer)actor;

            if (actor.AcdId > 0)
                _acdToRActorIndex[actor.AcdId] = id;

            _timer.Stop();
            actor.CreateTime = _timer.Elapsed.TotalMilliseconds;
            actor.Created = DateTime.UtcNow;
            return actor;
        }

        private TrinityActor UpdateRActor(int id, TrinityActor actor, DiaObject rActor)
        {
            _timer.Restart();
            if (actor == null || !actor.IsRActorValid)
                return null;
        
            actor.RActor = rActor;
            actor.OnUpdated();

            if(actor.AcdId > 0)
                _acdToRActorIndex[actor.AcdId] = id;

            _timer.Stop();
            actor.UpdateTime = _timer.Elapsed.TotalMilliseconds;
            return actor;
        }

        public void UpdateInventory()
        {
            var untouchedIds = new List<int>(ItemsByAnnId.Keys);
            foreach (var commonData in ZetaDia.Actors.GetActorsOfType<ACDItem>())
            {
                var type = commonData.ActorType;
                if (type != ActorType.Item)
                    continue;

                var inventorySlot = commonData.InventorySlot;
                if (inventorySlot == InventorySlot.Merchant || inventorySlot == InventorySlot.None)
                    continue;

                var annId = commonData.AnnId;
                if (annId == -1)
                    continue;

                ItemsByAnnId.AddOrUpdate(annId,
                    id => AddInventoryItem(id, commonData),
                    (id, existingItem) => UpdateInventoryItem(id, existingItem, commonData));

                untouchedIds.Remove(annId);
            }

            foreach (var key in untouchedIds)
            {
                TrinityItem item;
                if (ItemsByAnnId.TryRemove(key, out item) && item != null && item.IsValid)
                {
                    item.OnDestroyed();
                    item.RActor = null;
                    item.CommonData = null;
                }
            }
        }

        private TrinityItem AddInventoryItem(int id, ACD newItem)
        {
            return ActorFactory.CreateActor<TrinityItem>(newItem);
        }

        private TrinityItem UpdateInventoryItem(int id, TrinityItem item, ACD commonData)
        {
            _timer.Start();
            item.CommonData = commonData;
            item.OnUpdated();
            _timer.Stop();
            item.UpdateTime = _timer.Elapsed.TotalMilliseconds;
            return item;
        }

        #endregion Update Methods

        #region Lookup Methods

        public IEnumerable<TrinityActor> AllRActors
            => RActorsByRActorId.Values;

        public IEnumerable<T> OfType<T>() where T : TrinityActor
            => RActorsByRActorId.Values.OfType<T>();

        public IEnumerable<TrinityItem> AllInventory
            => ItemsByAnnId.Values.ToList();

        public TrinityItem GetItemByAnnId(int annId)
        {
            TrinityItem item;
            if (ItemsByAnnId.TryGetValue(annId, out item))
            {
                return item;
            }
            var acd = ZetaDia.Actors.GetACDByAnnId(annId);
            if (acd != null && acd.IsValid)
            {
                return ActorFactory.CreateActor<TrinityItem>(acd);
            }
            return null;
        }

        public ACDItem GetAcdItemByAnnId(int annId)
        {
            var acd = ZetaDia.Actors.GetACDByAnnId(annId);
            if (acd != null && acd.IsValid)
            {
                return acd.BaseAddress.UnsafeCreate<ACDItem>();
            }
            return null;
        }

        public T GetActorByAcdId<T>(int acdId) where T : TrinityActor
        {
            int rActorId;
            _acdToRActorIndex.TryGetValue(acdId, out rActorId);
            if (RActorsByRActorId.ContainsKey(rActorId))
                return RActorsByRActorId[rActorId] as T;

            return default(T);
        }

        #endregion Lookup Methods

        public void Clear()
        {
            Core.Logger.Debug("Resetting ActorCache");
            RActorsByRActorId.ForEach(o => o.Value.RActor = null);
            RActorsByRActorId.Clear();
            _acdToRActorIndex.Clear();
            Me = null;
        }

        IEnumerator IEnumerable.GetEnumerator() => AllRActors.GetEnumerator();
        public IEnumerator<TrinityActor> GetEnumerator() => AllRActors.GetEnumerator();
    }
}