using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Bcpg.Sig;
using Trinity.Technicals;
using Zeta.Common;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Cache.Properties
{
    /// <summary>
    /// Facilitates storage and retrieval of a subset of properties.
    /// </summary>
    public class PropertyLoader
    {
        private static readonly Dictionary<int, List<IPropertyCollection>> PropertyCollections = new Dictionary<int, List<IPropertyCollection>>();
        private static readonly IndexedList<int> InsertionOrder = new IndexedList<int>();

        public static int Count => PropertyCollections.Count;

        private const int ActorLimit = 500;

        /// <summary>
        /// Retrieves all property collections for an actor
        /// </summary>
        public static List<IPropertyCollection> GetOrCreate(int rActorGuid)
        {
            ClearOlderItems();

            List<IPropertyCollection> props;

            if (PropertyCollections.TryGetValue(rActorGuid, out props))
                return props;

            props = new List<IPropertyCollection>();
            PropertyCollections.Add(rActorGuid, props);
            InsertionOrder.Add(rActorGuid);
            return props;
        }

        /// <summary>
        /// Keep the number of cached items limited
        /// </summary>
        private static void ClearOlderItems()
        {
            if (PropertyCollections.Count > ActorLimit)
            {
                var removeAmount = ActorLimit/4;
                foreach (var id in InsertionOrder.Take(removeAmount).ToList())
                {
                    RemoveItem(id);
                }
                Logger.LogVerbose(LogCategory.CacheManagement, $"Cleared {removeAmount} property collections");
            }
        }

        private static void RemoveItem(int id)
        {
            PropertyCollections.Remove(id);
            InsertionOrder.Remove(id);
        }

        /// <summary>
        /// Populate properties from many IPropertyCollection.
        /// </summary>
        public static void LoadAll(TrinityCacheObject obj, List<IPropertyCollection> collections)
        {
            collections?.ForEach(p => p.ApplyTo(obj));
        }

        /// <summary>
        /// Populates specific properties.
        /// </summary>
        public static IPropertyCollection Load<T>(TrinityCacheObject obj, List<IPropertyCollection> collections = null) where T : IPropertyCollection, new()
        {
            collections = collections ?? GetOrCreate(obj.RActorGuid);

            if (collections == null)
                return null;
         
            var props = collections.OfType<T>().FirstOrDefault();
            if (props != null)
            {
                try
                {
                    props.ApplyTo(obj);
                }
                catch (Exception ex)
                {                    
                    Logger.LogError($"Exception updating {typeof(T)} properties. {obj?.InternalName} {obj?.ActorType} {ex}");
                }

                if (obj != null && !props.IsValid)
                    RemoveItem(obj.RActorGuid);
                else
                    return props;
            }

            props = CreatePropertyCollection<T>(obj);
            collections.Add(props);
            return props;                                         
        }

        private static T CreatePropertyCollection<T>(TrinityCacheObject obj) where T : IPropertyCollection, new()
        {
            var newProps = new T();
            newProps.OnCreate(obj);  
            newProps.ApplyTo(obj);
            return newProps;
        }
    }

    public interface IPropertyCollection
    {
        DateTime CreationTime { get; }

        /// <summary>
        /// Applies values to a TrinityCacheObject
        /// </summary>
        /// <param name="target">object to receive new values</param>
        void ApplyTo(TrinityCacheObject target);

        /// <summary>
        /// Called once after creating IPropertyCollection
        /// </summary>
        /// <param name="source">object from which to get data</param>
        void OnCreate(TrinityCacheObject source);

        /// <summary>
        /// Refresh properties that change over time.
        /// </summary>
        /// <param name="source">object from which to get data</param>
        void Update(TrinityCacheObject source);

        /// <summary>
        /// if set to true, collection will be deleted.
        /// </summary>
        bool IsValid { get; }
    }
}


