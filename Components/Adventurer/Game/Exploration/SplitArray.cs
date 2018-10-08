using Zeta.Common;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    /// <summary>
    /// Alternative to 2D array that splits the memory size into many 1D arrays.
    /// May avoid the large object heap and causing fragmentation / OutOfMemoryException.
    /// At the cost of additional overhead and processing.
    /// </summary>
    public class SplitArray<T>
    {
        private readonly T[] _nodes;
        private readonly int _stride;

        public int Columns => _nodes.Length / Rows;
        public int Rows => _nodes.Length / _stride;

        public SplitArray(int sizeX, int sizeY)
        {
            _nodes = new T[sizeX * sizeY];
            _stride = sizeY;
        }

        public T this[int indexX, int indexY]
        {
            get => _nodes[indexX * _stride + indexY];
            set => _nodes[indexX * _stride + indexY] = value;
        }
    }
}