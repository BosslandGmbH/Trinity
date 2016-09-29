using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trinity.Framework.Helpers
{
    /// <summary>
    /// Utility for finding private/internal and static types.
    /// </summary>
    public class TypeCrawler
    {
        private static Dictionary<string, Type> _types = new Dictionary<string, Type>();

        public static Type GetType(Assembly assembly, string typeName)
        {
            return GetType(assembly, new List<string> { typeName }).Values.FirstOrDefault();
        }

        public static Dictionary<string, Type> GetType(Assembly assembly, IEnumerable<string> typeNames)
        {
            var result = new Dictionary<string, Type>();
            var toFind = new Dictionary<string, Type>();

            foreach (var type in typeNames)
            {
                if (_types.ContainsKey(type))
                {
                    result.Add(type, _types[type]);
                    continue;
                }

                toFind.Add(type, null);
            }

            if (toFind.Count == 0)
                return result;

            var numToFind = toFind.Count;
            foreach (var type in assembly.GetTypes())
            {
                if (!toFind.ContainsKey(type.Name))
                    continue;

                _types.Add(type.Name, type);
                result.Add(type.Name, type);

                numToFind--;
                if (numToFind == 0)
                    break;
            }

            return result;
        }
    }
}