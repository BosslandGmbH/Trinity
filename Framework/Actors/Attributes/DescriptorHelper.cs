using System.Collections.Generic;
using System.Linq;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Framework.Actors.Attributes
{
    /// <summary>
    /// Helper to quickly access AttributeDescriptors
    /// </summary>
    internal static class DescriptorHelper
    {
        private static Dictionary<int, AttributeDescriptor> _descripters;

        public static AttributeDescriptor GetDescriptor(int id, bool checkExists = false)
        {
            if (_descripters == null)
                _descripters = ZetaDia.AttributeDescriptors.ToDictionary(descripter => descripter.Id);

            return !checkExists || _descripters.ContainsKey(id) ? _descripters[id] : default(AttributeDescriptor);
        }
    }
}