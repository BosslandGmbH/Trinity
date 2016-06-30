using System;
using System.Collections.Generic;
using System.Linq;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory.Containers
{
    public class TableItem : MemoryWrapper, ITableItem
    {
        public IntPtr Next => ReadOffset<IntPtr>(0x00);
        public int ModKey => ReadOffset<int>(0x04);
    }

    public interface ITableItem
    {
        int ModKey { get; }
    }

    public class Table<T> : MemoryWrapper where T : MemoryWrapper, ITableItem, new()
    {
        public IntPtr DataPtr => ReadOffset<IntPtr>(0x00);

        public int Size => ReadOffset<int>(0x08);

        public IntPtr[] RowPtrs => ZetaDia.Memory.ReadArray<IntPtr>(DataPtr, Size);

        public Dictionary<int, T> Items = new Dictionary<int, T>();

        public Table(IntPtr ptr)
        {
            Update(ptr);
        }

        protected override void OnUpdated()
        {
            if (!IsValid) return;

            Items.Clear();

            foreach (var ptr in RowPtrs.Where(p => p != IntPtr.Zero))
            {
                var item = Create<T>(ptr);
                Items.Add(item.ModKey, item);
            }
        }

        public IntPtr this[int key]
        {
            get
            {
                return RowPtrs[key];
            }
            set
            {
                RowPtrs[key] = value;
            }
        }
    }
}




