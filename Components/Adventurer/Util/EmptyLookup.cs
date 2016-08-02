using System.Linq;

namespace Trinity.Components.Adventurer.Util
{
    public static class EmptyLookup<TKey, TElement>
    {
        private static readonly ILookup<TKey, TElement> _instance
            = Enumerable.Empty<TElement>().ToLookup(x => default(TKey));

        public static ILookup<TKey, TElement> Instance
        {
            get { return _instance; }
        }
    }
}
