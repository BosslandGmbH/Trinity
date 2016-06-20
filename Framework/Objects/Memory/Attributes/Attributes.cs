using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using IronPython.Compiler.Ast;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Framework.Objects.Memory.Items;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory.Attributes
{
    public class Attributes
    {
        public AttributeItem this[short index] => Items[index];
        public Dictionary<int, AttributeItem> Items = new Dictionary<int, AttributeItem>();
        public Func<ACD, int> GetFastAttributeGroupId = acd => ZetaDia.Memory.Read<int>(acd.BaseAddress + 0x120);
        public int FastAttributeGroupId;
        public Map<AttributeItem> PtrMap;
        public AttributeGroup Group;
        private Map<AttributeItem> Map;

        public bool IsValid => Group != null && Group.IsValid && Group.Id != 0 && Map != null && Map.IsValid && Map.Count != 0;

        public Attributes(int groupId)
        {
            Create(groupId);
        }

        protected void Create(int groupId)
        {
            FastAttributeGroupId = groupId;
            Group = AttributeManager.FindGroup(groupId);

            if (Group == null || !Group.IsValid)
            {
                Logger.LogVerbose($"Attribute group is invalid with id: {groupId}");
                return;
            }

            Map = (Group.Flags & 4) != 0 ? Group.PtrMap : Group.Map;
            if (!Map.IsValid)
            {
                ActorManager.Reset();
                Logger.LogVerbose($"Attribute Map Invalid for groupId: {groupId}");
                return;
            }

            Items = Map.Data.Items;
           
            if (Group.Map.Count != Items.Count && Group.Map2.IsValid)
            {
                Items.AddRangeNewOnly(Group.Map2.Data.Items);
            }


        }

        internal List<TValue> GetCachedAttributes<TValue>(ActorAttributeType attr)
        {
            var result = new List<TValue>();
            foreach (var a in Items)
            {
                if (a.Value.Key.BaseAttribute == attr)
                {
                    result.Add(a.Value.GetValue<TValue>());
                }
            }
            return result;
        }

        internal Dictionary<TModifier, TValue> GetCachedAttributes<TModifier, TValue>(ActorAttributeType attr) where TModifier : IConvertible
        {
            var result = new Dictionary<TModifier, TValue>();        
            foreach (var a in Items)
            {
                if (a.Value.Key.BaseAttribute == attr)
                {
                    result.Add(a.Value.Key.ModifierId.To<TModifier>(), a.Value.GetValue<TValue>());
                }
            }
            return result;
        }

        internal T GetFirstCachedAttribute<T>(ActorAttributeType attr)
        {
            var foundAttribute = Items.FirstOrDefault(IsAttributeMatch(attr));
            if (foundAttribute.Value != null)
            {
                return foundAttribute.Value.GetValue<T>();
            }
            return default(T);
        }

        internal T GetCachedAttribute<T>(Predicate<AttributeItem> condition = null)
        {
            foreach (var item in Items.Where(item => condition == null || condition(item.Value)))
            {
                return item.Value.GetValue<T>();
            }
            return default(T);
        }

        private static Func<KeyValuePair<int, AttributeItem>, bool> IsAttributeMatch(ActorAttributeType attr)
        {
            return item => (int)attr == item.Key || attr == item.Value.Key.BaseAttribute;
        }

        internal T GetCachedAttribute<T>(ActorAttributeType attr, int modifier)
        {
            var key = new AttributeKey((int)attr, modifier);
            return GetCachedAttribute<T>(key);
        }

        internal T GetCachedAttribute<T>(AttributeKey key)
        {

            AttributeItem foundAttribute;
            if (!Items.TryGetValue(key.Value, out foundAttribute))
                return default(T);

            return foundAttribute.GetValue<T>();
        }

        internal T GetCachedAttribute<T>(ActorAttributeType attr)
        {
            AttributeItem foundAttribute;
            if (!Items.TryGetValue((int)attr, out foundAttribute))
                return default(T);

            return foundAttribute.GetValue<T>();
        }

        internal T GetAttribute<T>(ActorAttributeType attr, int modifier)
        {
            var key = new AttributeKey((int)attr, modifier);
            return GetAttribute<T>(key);
        }

        internal T GetAttribute<T>(AttributeKey key)
        {
            AttributeItem foundAttribute;
            if (!Items.TryGetValue(key.Value, out foundAttribute))
                return default(T);

            foundAttribute.Update();
            return foundAttribute.GetValue<T>();
        }

        internal T GetAttribute<T>(ActorAttributeType attr)
        {
            AttributeItem foundAttribute;
            if (!Items.TryGetValue((int)attr, out foundAttribute))
                return default(T);

            foundAttribute.Update();
            return foundAttribute.GetValue<T>();
        }

        internal AttributeItem GetAttributeItem(ActorAttributeType attr)
        {
            var attributePair = Items.FirstOrDefault(a => a.Value.Attribute == attr || a.Value.Key.BaseAttribute == attr);            
            attributePair.Value?.Update();
            return attributePair.Value;
        }

        public override string ToString()
        {
            return Items.Aggregate($"Attributes ({Items.Count}) Id={(short)FastAttributeGroupId}/{FastAttributeGroupId} {Environment.NewLine}",
                (current, attr) => current + $" {attr.Value} {Environment.NewLine}");
        }

        public string ToProperties()
        {
            return Items.Where(i => i.Value.Integer != 0 || i.Value.Single > float.Epsilon)
                .Aggregate($"Attribute Properties ({Items.Count}) Id={(short)FastAttributeGroupId}/{FastAttributeGroupId} {Environment.NewLine}",
                (current, attr) => current + $" public {attr.Value.Descripter.DataType} " +
                $"{attr.Value.Attribute} => GetCachedAttribute<{attr.Value.Descripter.DataType}>" +
                $"(ActorAttributeType.{attr.Value.Attribute}); // {attr.Value.Attribute} " +
                $"({attr.Value.ModKey}) = i:{attr.Value.Integer} f:{attr.Value.Single} " +
                $"v:{attr.Value.GetValue()} ModifierType={attr.Value.Descripter.ParameterType} " +
                $"Modifier={attr.Value.Key.ModifierId} {Environment.NewLine}");
        }

    }
}

