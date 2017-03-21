using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Helpers;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory
{
    public class AttributeManager : MemoryWrapper
    {
        public static Dictionary<int, Zeta.Game.Internals.AttributeDescriptor> AttributeDescriptors;
        private static ExpandoContainer<AttributeGroup> _attributeGroups;

        static AttributeManager()
        {
            Create();
        }

        private static void Create()
        {
            //var descriptors = ReadObjects<AttributeDescripter>(Internals.Addresses.AttributeDescripters, 1441, 0x28).ToList();
            //AttributeDescriptors = descriptors.ToDictionary(descripter => descripter.Id);

            AttributeDescriptors = ZetaDia.AttributeDescriptors.ToDictionary(descripter => descripter.Id);
        }

        public static class DescriptorHelper
        {
            private static Dictionary<int, Zeta.Game.Internals.AttributeDescriptor> _descripters;

            public static Zeta.Game.Internals.AttributeDescriptor GetDescriptor(int id, bool checkExists = false)
            {
                if (_descripters == null)
                    _descripters = ZetaDia.AttributeDescriptors.ToDictionary(descripter => descripter.Id);

                return !checkExists || _descripters.ContainsKey(id) ? _descripters[id] : default(Zeta.Game.Internals.AttributeDescriptor);
            }
        }

        public static void Reset()
        {
            Create();
        }

        public static ExpandoContainer<AttributeGroup> AttributeGroups
        {
            get
            {
                if (!IsValid)
                {
                    _attributeGroups = Create<ExpandoContainer<AttributeGroup>>(ZetaDia.Storage.FastAttribGroups.BaseAddress);
                }
                if (!IsValid)
                {
                    Core.Logger.Error("Failed to find AttributeGroupManager");
                    return null;
                }
                return _attributeGroups;
            }
        }

        private static new bool IsValid => _attributeGroups != null && !_attributeGroups.IsDisposed && _attributeGroups.Count > 0 && _attributeGroups.Bits < 1000 && _attributeGroups.ItemSize > 0;

        private static List<AttributeGroup> Groups => _attributeGroups?.Where(g => g.Id > 0).ToList();

        private static List<AttributeGroup> PlayerAttribtues
        {
            get
            {
                if (ZetaDia.Me == null) return null;
                var playerGroupId = ZetaDia.Me.CommonData.FastAttribGroupId;
                return _attributeGroups?.Where(g => g.Id > 0 && g.Id == playerGroupId).ToList();
            }
        }

        public static AttributeGroup FindGroup(int groupId)
        {
            return AttributeGroups?[(short)groupId];
        }
    }
}