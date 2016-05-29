using System;
using System.Collections.Generic;

namespace Trinity.Framework.Helpers
{
    public static class Randomizer
    {
        public static readonly Random Random = new Random();

        public static int GetRandomNumber(int max)
        {
            return Random.Next(max);
        }
        public static int GetRandomNumber(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static IList<T> RandomShuffle<T>(IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
    }
}
