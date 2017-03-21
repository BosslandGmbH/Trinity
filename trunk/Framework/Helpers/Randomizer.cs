using System;
using System.Collections.Generic;

namespace Trinity.Framework.Helpers
{
    public static class Randomizer
    {
        private static Random _random { get; } = new Random();

        public static int Fudge(int input, double min = 0.5, double max = 1.5)
            => Boolean ? _random.Next((int)(input * min), (int)(input * max)) : input * 4;

        public static bool Boolean => _random.Next(0, 1) == 1;

        public static int Max(int max)
        {
            return _random.Next(max);
        }

        public static int Random(int min, int max)
        {
            return _random.Next(min, max);
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