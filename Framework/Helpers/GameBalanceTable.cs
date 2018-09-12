using System.Collections;
using System.Collections.Generic;

namespace Trinity.Framework.Helpers
{
    public class GameBalanceTable : IEnumerable<GameBalanceTable.GameBalanceTableEntry>
    {
        private readonly Dictionary<int, GameBalanceTableEntry> _byIndex = new Dictionary<int, GameBalanceTableEntry>();
        private readonly Dictionary<int, GameBalanceTableEntry> _byItemHash = new Dictionary<int, GameBalanceTableEntry>();
        private readonly Dictionary<int, GameBalanceTableEntry> _byNormalHash = new Dictionary<int, GameBalanceTableEntry>();
        private readonly Dictionary<string, GameBalanceTableEntry> _byGameBalanceName = new Dictionary<string, GameBalanceTableEntry>();

        public class GameBalanceTableEntry
        {
            public int Index;
            public int ItemHash;
            public int NormalHash;
            public string Name;
        }

        public void Add(int index, int itemHash, int normalHash, string name)
        {
            var entry = new GameBalanceTableEntry
            {
                Index = index,
                ItemHash = itemHash,
                NormalHash = normalHash,
                Name = name
            };

            _byIndex.Add(index, entry);
            _byItemHash.Add(itemHash, entry);
            _byNormalHash.Add(normalHash, entry);
            _byGameBalanceName.Add(name, entry);
        }

        public GameBalanceTableEntry this[int index]
        {
            get => _byIndex[index];
            set => _byIndex[index] = value;
        }

        public bool Contains(string name)
        {
            return _byGameBalanceName.ContainsKey(name);
        }

        public bool Contains(int hash)
        {
            return ContainsItemHash(hash) || ContainsNormalHash(hash);
        }

        public bool ContainsItemHash(int hash)
        {
            return _byItemHash.ContainsKey(hash);
        }

        public bool ContainsNormalHash(int hash)
        {
            return _byNormalHash.ContainsKey(hash);
        }

        public bool TryGetValue(int hash, out GameBalanceTableEntry entry)
        {
            if (ContainsItemHash(hash))
            {
                entry = _byItemHash[hash];
                return true;
            }
            if (ContainsNormalHash(hash))
            {
                entry = _byNormalHash[hash];
                return true;
            }
            entry = null;
            return false;
        }

        IEnumerator<GameBalanceTableEntry> IEnumerable<GameBalanceTableEntry>.GetEnumerator()
        {
            return _byIndex.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}