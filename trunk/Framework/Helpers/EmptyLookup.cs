using System.Linq;
namespace Trinity.Framework.Helpers
{
    public static class EmptyLookup<TKey, TElement>
    {
        public static ILookup<TKey, TElement> Instance { get; } = Enumerable.Empty<TElement>().ToLookup(x => default(TKey));
    }
}