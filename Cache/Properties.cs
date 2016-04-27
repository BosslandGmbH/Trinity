using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeta.Game.Internals.Actors;

namespace Trinity.Cache
{
    /// <summary>
    /// Facilitates storage and retrieval of a subset of properties.
    /// </summary>
    public class Properties
    {
        private static readonly Dictionary<int, List<IPropertyCollection>> _properties = new Dictionary<int, List<IPropertyCollection>>();

        private const int ActorLimit = 500;

        /// <summary>
        /// Retrieves all property collections
        /// </summary>
        public static List<IPropertyCollection> Get(int rActorGuid)
        {
            if (_properties.Count > ActorLimit)
                _properties.Clear();

            List<IPropertyCollection> props;

            if (_properties.TryGetValue(rActorGuid, out props))
                return props;

            props = new List<IPropertyCollection>();
            _properties.Add(rActorGuid, props);
            return props;
        }

        /// <summary>
        /// Populates properties, caching where needed and using previously cached values where appropritate.
        /// </summary>
        public static IPropertyCollection Load<T>(TrinityCacheObject obj, List<IPropertyCollection> collections = null) where T : IPropertyCollection, new()
        {
            var key = obj.RActorGuid;

            collections = collections ?? Get(key);

            if (collections == null)
                return null;
         
            var props = collections.OfType<T>().FirstOrDefault();

            if (props != null)
            {
                props.ApplyTo(obj);
                return props;
            }
    
            props = CreatePropertyCollection<T>(obj);
            collections.Add(props);
            return props;                                         
        }

        private static T CreatePropertyCollection<T>(TrinityCacheObject obj) where T : IPropertyCollection, new()
        {
            var newProps = new T();
            newProps.RefreshFrom(obj);
            return newProps;
        }

        public interface IPropertyCollection
        {
            void ApplyTo(TrinityCacheObject obj);
            void RefreshFrom(TrinityCacheObject obj);
        }


    }
}


