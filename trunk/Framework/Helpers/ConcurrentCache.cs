using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
namespace Trinity.Framework.Helpers
{
    public class ConcurrentCache<TK, TCacheValue, TSourceValue> : IEnumerable<TCacheValue>
    {
        private readonly UpdateFactory _updateFactory;
        private readonly CreateFactory _createFactory;
        private readonly SourceProducer _sourceFactory;
        private readonly KeyProducer _keyProducer;
        private readonly RemovedAction _removedAction;

        public delegate IList<TSourceValue> SourceProducer();

        public delegate TK KeyProducer(TSourceValue item);

        public delegate TCacheValue UpdateFactory(TK key, TCacheValue existingItem, TSourceValue newItem, out bool success);

        public delegate TCacheValue CreateFactory(TK key, TSourceValue newItem, out bool success);

        public delegate void RemovedAction(TCacheValue existingItem);

        public ConcurrentCache(SourceProducer sourceProducer, KeyProducer keyProducer, UpdateFactory updateFactory, CreateFactory createFactory, RemovedAction removedAction = null)
        {
            _sourceFactory = sourceProducer;
            _updateFactory = updateFactory;
            _createFactory = createFactory;
            _keyProducer = keyProducer;
            _removedAction = removedAction;
        }

        public ConcurrentDictionary<TK, TCacheValue> Items { get; } = new ConcurrentDictionary<TK, TCacheValue>();

        public IEnumerable<T> ItemsOfType<T>() where T : TCacheValue => Items.Values.OfType<T>();

        public void Update()
        {
            var source = _sourceFactory?.Invoke();
            if (source == null || source.Count == 0)
                return;

            var keys = Items.Keys.ToList();

            foreach (var sourceItem in source)
            {
                var result = false;
                var key = _keyProducer(sourceItem);

                Items.AddOrUpdate(key,
                    k => _createFactory(k, sourceItem, out result),
                    (k, cachedItem) => _updateFactory(k, cachedItem, sourceItem, out result));

                if (result)
                    keys.Remove(key);
            }

            foreach (var key in keys)
            {
                TCacheValue removedItem;
                if (Items.TryRemove(key, out removedItem))
                    _removedAction?.Invoke(removedItem);
            }
        }

        public int Count => Items.Count;

        public void Clear() => Items.Clear();

        public IEnumerator<TCacheValue> GetEnumerator() => Items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}