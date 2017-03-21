using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
namespace Trinity.Components.QuestTools.Helpers
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (source == null)
                throw new ArgumentNullException("source");
            foreach (var element in source)
                target.Add(element);
        }

        public static void InsertRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (source == null)
                throw new ArgumentNullException("source");

            var newCollection = source.ToList();
            newCollection.AddRange(target);
            target.Clear();
            target.AddRange(newCollection);
        }
    }
}