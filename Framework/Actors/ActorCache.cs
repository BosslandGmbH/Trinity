using System;
using System.Collections;
using Trinity.Framework.Helpers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Service;
using Zeta.Game.Internals.SNO;


namespace Trinity.Framework.Actors
{
    public interface IActorCache : IEnumerable<TrinityActor>
    {
        HashSet<int> CurrentAcdIds { get; }
        HashSet<int> CurrentRActorIds { get; }
        int ActivePlayerRActorId { get; }
        Vector3 ActivePlayerPosition { get; }
        IEnumerable<TrinityActor> Actors { get; }
        IEnumerable<TrinityItem> Inventory { get; }
        TrinityPlayer Me { get; }
        DateTime LastUpdated { get; }
        IEnumerable<T> OfType<T>() where T : TrinityActor;
        T RActorByAcdId<T>(int acdId) where T : TrinityActor;
        TrinityItem ItemByAnnId(int annId);
        ACDItem GetAcdItemByAnnId(int annId);
        bool IsAnnIdValid(int itemAnnId);
        void Update();
        void Clear();
    }

    /// <summary>
    /// Maintains the current valid list of actors.
    /// Collections are complete, except for bad data / disposed actors.
    /// Filtered lists for attacking/interacting can be found in Core.Targets
    /// </summary>
    public class ActorCache : Module, IActorCache
    {
        private Stopwatch _timer = new Stopwatch();

        private readonly ConcurrentDictionary<int, ACD> _commonData = new ConcurrentDictionary<int, ACD>();

        public readonly ConcurrentDictionary<int, TrinityActor> _rActors = new ConcurrentDictionary<int, TrinityActor>();
        private readonly ConcurrentDictionary<int, TrinityItem> _inventory = new ConcurrentDictionary<int, TrinityItem>();

        private readonly Dictionary<int, short> _annToAcdIndex = new Dictionary<int, short>();
        private readonly Dictionary<int, int> _acdToRActorIndex = new Dictionary<int, int>();
        private GameId _gameId;
        public HashSet<int> CurrentAcdIds { get; set; } = new HashSet<int>();
        public HashSet<int> CurrentRActorIds { get; set; } = new HashSet<int>();
        public ulong LastUpdatedFrame { get; private set; }
        public int ActivePlayerRActorId { get; private set; }
        public TrinityPlayer Me { get; private set; }
        public Vector3 ActivePlayerPosition { get; set; }
        public int LastWorldSnoId { get; private set; }

        protected override void OnPulse() => Update();

        protected override void OnWorldChanged(ChangeEventArgs<int> args)
        {
            Clear();
        }

        protected override void OnGameChanged()
        {
            Clear();
        }

        public void Update()
        {
            var currentFrame = ZetaDia.Memory.Executor.FrameCount;

            if (!ZetaDia.IsInGame)
                return;

            var gameId = ZetaDia.Service.CurrentGameId;
            if (_gameId != gameId)
            {
                Core.Logger.Debug("Game Change Detected");
                ZetaDia.Actors.Update();
                _gameId = gameId;
                return;
            }

            if (ZetaDia.Actors.Me == null || !ZetaDia.Actors.Me.IsFullyValid())
                ZetaDia.Actors.Update();

            ActivePlayerRActorId = ZetaDia.ActivePlayerRActorId;
            ActivePlayerPosition = ZetaDia.Me.Position;

            UpdateAcds();
            UpdateRActors();
            UpdateInventory();

            CurrentAcdIds = new HashSet<int>(_commonData.Keys);
            CurrentRActorIds = new HashSet<int>(_rActors.Keys);
            LastUpdatedFrame = currentFrame;
            LastWorldSnoId = ZetaDia.Globals.WorldSnoId;
        }

        #region Update Methods

        /// <summary>
        /// Get new Acds from Memory; update ones we already know about.
        /// * Some Acds like backpack/stash/equipped items do not have an associated RActor
        /// </summary>
        private void UpdateAcds()
        {
            _annToAcdIndex.Clear();

            var oldAcds = new List<int>(_commonData.Keys);
            foreach (var acd in ZetaDia.Storage.ActorCommonData)
            {
                var acdId = acd.ACDId;

                _annToAcdIndex[acd.AnnId] = (short)acdId;
                _commonData.AddOrUpdate(acdId,
                    (id) => AddAcd(id, acd),
                    (id, o) => UpdateAcd(id, acd));

                oldAcds.Remove(acdId);
            }

            foreach (var key in oldAcds)
            {
                ACD item;
                if (_commonData.TryRemove(key, out item) && item != null && item.IsValid)
                {
                    item.UpdatePointer(IntPtr.Zero);
                }
            }
        }

        private ACD AddAcd(int id, ACD newObj)
        {
            return newObj;
        }

        private ACD UpdateAcd(int id, ACD newObj)
        {
            return newObj;
        }

        /// <summary>
        /// Get new RActors from Memory; update ones we already know about.
        /// * Some RActors like client effects and environment do not have an associated acd.
        /// </summary>
        private void UpdateRActors()
        {
            _acdToRActorIndex.Clear();

            var untouchedIds = new List<int>(_rActors.Keys.ToList());

            var actors = ZetaDia.RActors.ToList<DiaObject>();
            foreach (var rActor in actors)
            {
                var rActorId = rActor.RActorId;
                if (rActorId == -1)
                    continue;

                if (rActor.IsACDBased && !rActor.IsFullyValid())
                    continue;

                var result = true;

                _rActors.AddOrUpdate(rActorId,
                    (id) => TryAddRActor(id, rActor, out result),
                    (id, actor) => TryUpdateRActor(id, actor, rActor, out result));

                if (result)
                    untouchedIds.Remove(rActorId);
            }

            foreach (var key in untouchedIds)
            {
                TrinityActor item;
                if (_rActors.TryRemove(key, out item))
                {
                    item?.OnDestroyed();
                }
            }
        }

        private TrinityActor TryAddRActor(int id, DiaObject rActor, out bool result)
        {
            _timer.Restart();
            var actor = ActorFactory.CreateActor(rActor);
            var player = actor as TrinityPlayer;
            if (player != null && player.IsMe)
                Me = player;
            _acdToRActorIndex[actor.AcdId] = actor.RActorId;
            _timer.Stop();
            actor.CreateTime = _timer.Elapsed.TotalMilliseconds;
            actor.Created = DateTime.UtcNow;
            result = true;
            return actor;
        }

        private TrinityActor TryUpdateRActor(int id, TrinityActor actor, DiaObject rActor, out bool result)
        {
            if (!actor.IsRActorValid)
            {
                result = false;
                return actor;
            }

            _timer.Restart();
            actor.RActor = rActor;
            actor.OnUpdated();
            _acdToRActorIndex[actor.AcdId] = actor.RActorId;
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
            var untouchedIds = new List<int>(_inventory.Keys);
            foreach (var newItem in _commonData)
            {
                var commonData = newItem.Value;
                var type = commonData.ActorType;
                if (type != ActorType.Item)
                    continue;

                var annId = commonData.AnnId;
                if (annId == -1)
                    continue;

                if (!commonData.IsValid || commonData.IsDisposed)
                    continue;

                if (!_inventory.ContainsKey(annId))
                {
                    var newObj = ActorFactory.CreateActor<TrinityItem>(commonData);
                    if (newObj.InventorySlot == InventorySlot.Merchant)
                        continue;

                    _inventory.TryAdd(annId, newObj);
                }
                else
                {
                    var oldObj = _inventory[annId];
                    if (!UpdateInventoryItem(oldObj, commonData))
                        continue;

                }

                //_inventory.AddOrUpdate(annId,
                //    id => AddInventoryItem(id, commonData),
                //    (id, existingItem) => UpdateInventoryItem(id, existingItem, commonData));

                untouchedIds.Remove(annId);
            }

            foreach (var key in untouchedIds)
            {
                TrinityItem item;
                if (_inventory.TryRemove(key, out item) && item != null)
                {
                    //item.RActor?.UpdatePointer(IntPtr.Zero);
                    //item.CommonData?.UpdatePointer(IntPtr.Zero);
                    //item.ActorInfo?.UpdatePointer(IntPtr.Zero);
                    //item.MonsterInfo?.UpdatePointer(IntPtr.Zero);
                    item.OnDestroyed();
                }
            }
        }


        private bool UpdateInventoryItem(TrinityItem item, ACD commonData)
        {
            _timer.Start();
            item.CommonData = commonData;
            item.OnUpdated();
            if (!item.IsValid)
                return false;
            _timer.Stop();
            item.UpdateTime = _timer.Elapsed.TotalMilliseconds;
            return true;
        }

        #endregion Update Methods

        #region Lookup Methods

        public IEnumerable<TrinityActor> Actors => _rActors.Values;

        public IEnumerable<T> OfType<T>() where T : TrinityActor
            => _rActors.Values.OfType<T>();

        public IEnumerable<TrinityItem> Inventory => _inventory.Values.ToList();

        public IEnumerable<TrinityItem> GetInventoryItems(InventorySlot slot)
        {
            return _inventory.Values.Where(i => i.InventorySlot == slot);
        }

        public ACD GetCommonDataByAnnId(int annId)
        {
            var acdIndex = _annToAcdIndex[annId];
            var acd = ZetaDia.Storage.ActorCommonData[acdIndex];
            if (acd != null && acd.IsValid)
            {
                return acd;
            }
            return null;
        }

        public ACD GetCommonDataById(int acdId)
        {
            if (acdId == -1)
                return null;

            var acd = ZetaDia.Storage.ActorCommonData[(short)acdId];
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

        public TrinityItem ItemByAnnId(int annId)
        {
            TrinityItem item;
            if (_inventory.TryGetValue(annId, out item))
            {
                return item;
            }

            short index;
            if (!_annToAcdIndex.TryGetValue(annId, out index))
                return null;

            var acd = ZetaDia.Storage.ActorCommonData[index];
            if (acd != null && acd.IsValid)
            {
                var bones = ActorFactory.GetActorSeed(acd);
                return ActorFactory.CreateActor<TrinityItem>(bones);
            }
            return null;
        }

        public ACDItem GetAcdItemByAcdId(int acdId)
        {
            var acd = ZetaDia.Storage.ActorCommonData[(short)acdId];
            if (acd != null && acd.IsValid)
            {
                return acd.BaseAddress.UnsafeCreate<ACDItem>();
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

            var acd = ZetaDia.Storage.ActorCommonData[index];
            return acd != null && acd.IsValid;
        }

        #endregion Lookup Methods

        public void Clear()
        {
            Core.Logger.Warn("Resetting ActorCache");
            _annToAcdIndex.Clear();
            foreach (var pair in _rActors)
            {
                pair.Value.Attributes?.Destroy();
                pair.Value.RActor = null;
                pair.Value.CommonData = null;
                pair.Value.Attributes = null;
            }
            _rActors.Clear();
            _inventory.Clear();
            _commonData.Clear();
            CurrentAcdIds.Clear();
            CurrentRActorIds.Clear();
            _acdToRActorIndex.Clear();
            ActivePlayerRActorId = 0;
            Me = null;
        }

        public T RActorByAcdId<T>(int acdId) where T : TrinityActor
        {
            if (acdId == 0 || acdId == -1)
                return default(T);

            if (!_acdToRActorIndex.ContainsKey(acdId))
                return default(T);

            var rActorId = _acdToRActorIndex[acdId];
            if (!_rActors.ContainsKey(rActorId))
                return default(T);

            return _rActors[rActorId] as T;
        }

        IEnumerator IEnumerable.GetEnumerator() => Actors.GetEnumerator();
        public IEnumerator<TrinityActor> GetEnumerator() => Actors.GetEnumerator();
    }
}
