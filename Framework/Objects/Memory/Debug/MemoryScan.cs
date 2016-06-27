using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Helpers;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory.Debug
{
    public class MemoryScan : MemoryWrapper
    {
        public int Size { get; set; } = 2000;

        public MemoryScan Init(int size)
        {
            Size = size;
            return this;
        }
        
        public MemoryComparer.MemoryVarianceResult<int> Integers => (_lastIntegerScan = BaseAddress.MemoryCompare(_lastIntegerScan, Size));
        private MemoryComparer.MemoryVarianceResult<int> _lastIntegerScan;

        public MemoryComparer.MemoryVarianceResult<float> Floats => (_lastFloatScan = BaseAddress.MemoryCompare(_lastFloatScan, Size));
        private MemoryComparer.MemoryVarianceResult<float> _lastFloatScan;
    }

    public static class MemoryComparer
    {
        /// <summary>
        /// Scan memory range and compare to previous scan
        /// </summary>
        public static MemoryVarianceResult<T> MemoryCompare<T>(this IntPtr baseAddress, MemoryVarianceResult<T> compareTo, int size) where T : struct
        {
            if (compareTo == null)
                compareTo = new MemoryVarianceResult<T>();

            var variances = new SortedList<int, MemoryVariance<T>>();

            for (int index = 0; index < size; index++)
            {
                if (index % 4 != 0) // Aligned 4 bytes int/float
                    continue;

                var lastComparison = compareTo.Variances.ContainsKey(index) ? compareTo.Variances[index] : new MemoryVariance<T>();

                var value = default(T);
                try
                {
                    value = ZetaDia.Memory.Read<T>(baseAddress + index);
                }
                catch (Exception) { }

                GetVariance(index, value, lastComparison, variances);
            }
            return new MemoryVarianceResult<T>(variances);
        }

        public static void GetVariance<T>(int offset, T val1, MemoryVariance<T> lastComparison, SortedList<int, MemoryVariance<T>> newVariances)
        {
            var v = new MemoryVariance<T>
            {
                Offset = offset,
                ValA = lastComparison.ValB,
                ValB = val1
            };

            if (v.ValA == null && v.ValB == null)
                return;

            if (v.ValA == null || v.ValB == null)
                newVariances.Add(offset, v);

            else if (!v.ValA.Equals(v.ValB))
                newVariances.Add(offset, v);
        }

        public class MemoryVarianceResult<T>
        {
            public MemoryVarianceResult()
            {

            }

            public MemoryVarianceResult(SortedList<int, MemoryVariance<T>> variances)
            {
                Variances = variances;
            }

            public SortedList<int, MemoryVariance<T>> Variances { get; set; } = new SortedList<int, MemoryVariance<T>>();

            public override string ToString()
            {
                if (!Variances.Any())
                    return "No Variances";

                var sb = new StringBuilder();
                sb.AppendLine("");

                //var precision = (12.3456 + "").split(".")[1].length;

                foreach (var item in Variances)
                {
                    var offsetHex = "0x" + item.Value.Offset.ToString("X");               
                    sb.AppendLine($"\r {offsetHex} {item.Value.Offset}: {item.Value.ValA} => {item.Value.ValB} ");
                }
                return sb.ToString();
            }
        }

        public class MemoryVariance<T>
        {
            public int Offset { get; set; }
            public T ValA { get; set; }
            public T ValB { get; set; }

            public override string ToString()
            {
                return $"0x{Offset.ToString("X")} {Offset}: {ValA} => {ValB}";
            }
        }
    }
}
