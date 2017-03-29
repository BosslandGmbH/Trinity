using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory
{
    public class Attributes
    {
        public AttributeItem this[short index] => Items[index];
        public Dictionary<int, AttributeItem> Items = new Dictionary<int, AttributeItem>();
        public int FastAttributeGroupId;
        public BasicMap<AttributeItem> PtrMap;
        public AttributeGroup Group;
        private BasicMap<AttributeItem> Map;

        private static uint AttributeHasher(int key)
        {
            return unchecked((uint)(key ^ (key >> 12)));            
        }

        public bool IsValid => Group != null && Group.IsValid && Group.Id != 0 && Map != null && Map.IsValid && Map.Count != 0;

        public void Destroy()
        {
            Items.Clear();
            Group = null;
            Map = null;
        }

        public Attributes()
        {
        }

        public int _lastRowCount;

        public Attributes(int groupId)
        {
            ReadAttributes(groupId);
        }

        public void ReadAttributes(int groupId)
        {
            try
            {
                if (groupId == -1)
                    return;

                if (ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                {
                    Destroy();
                    return;
                }

                FastAttributeGroupId = groupId;
                Group = AttributeManager.FindGroup(groupId);

                if (Group == null || !Group.IsValid)
                {
                    Core.Logger.Verbose($"Attribute group is invalid with id: {groupId}");
                    return;
                }

                if ((Group.Flags & 4) != 0)
                {
                    Map = Group.PtrMap;
                }
                else
                {
                    Map = Group.Map;
                }

                if (Map == null || !Map.IsValid)
                {
                    Core.Logger.Verbose($"Attribute Map Invalid for groupId: {groupId}");
                    return;
                }

                Items = Map.Data.Items;
                _lastRowCount = Map.Count;

                if (Map.Count != Items.Count && Group.Map2.Count > 0)
                {
                    Items.AddRangeNewOnly(Group.Map2.Data.Items);
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Debug($"Exception creating attributes {ex} for groupId {groupId}");
            }
        }        

        private Dictionary<int, AttributeItem> _previousItems = new Dictionary<int, AttributeItem>();

        private DateTime LastUpdatedTime = DateTime.MinValue;

        public bool Update()
        {
            if (!IsValid || ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                return false;

            var isChanged = Map.Count != _lastRowCount;
            if (isChanged || DateTime.UtcNow.Subtract(LastUpdatedTime).TotalSeconds > 1)
            {
                ReadAttributes(FastAttributeGroupId);
                _lastRowCount = Map.Count;
                LastUpdatedTime = DateTime.UtcNow;
                return true;
            }
            return false;
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

        public ulong LastUpdatedFrame { get; set; }

        internal Dictionary<TModifier, TValue> GetAttributes<TModifier, TValue>(AttributeParameterType parameterType) where TModifier : IConvertible
        {
            // todo: this is not working, sometimes powers stop showing up here. needs investigation

            //var shouldUpdate = LastUpdatedFrame != ZetaDia.Memory.Executor.FrameCount && (LastUpdatedFrame = ZetaDia.Memory.Executor.FrameCount) != 0;

            var result = new Dictionary<TModifier, TValue>();
            foreach (var a in Items)
            {
                if (a.Value.Descripter.Value.ParameterType == Zeta.Game.Internals.AttributeParameterType.PowerSnoId || a.Value.Descripter.Value.ParameterType == Zeta.Game.Internals.AttributeParameterType.PowerSnoId2)
                {
                    a.Value.Update();

                    if (a.Value.GetValue<int>() != 0)
                    {
                        var key = a.Value.Key.ModifierId.To<TModifier>();

                        if (!result.ContainsKey(key))
                        {
                            result.Add(key, a.Value.GetValue<TValue>());
                        }
                    }
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

        internal AttributeItem GetFirstCachedAttributeItem(ActorAttributeType attr)
        {
            var foundAttribute = Items.FirstOrDefault(IsAttributeMatch(attr));
            return foundAttribute.Value;
        }

        internal T GetCachedAttribute<T>(Predicate<AttributeItem> condition = null)
        {
            foreach (var item in Items.Where(item => condition == null || condition(item.Value)))
            {
                return item.Value.GetValue<T>();
            }
            return default(T);
        }

        /// <summary>
        /// Checks both the base attribute and attribute+modifier numbers for a match.
        /// </summary>
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

        internal T GetAttributeDirectlyFromTable<T>(ActorAttributeType attr, int modifier = -1)
        {
            var key = new AttributeKey((int)attr, modifier);

            AttributeItem foundAttribute;
            if (Map == null || !Map.TryGetItemByKey(key.Value, out foundAttribute, AttributeHasher))
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

        internal T GetAttribute<T>(ActorAttributeType attr, SNOPower modifier)
        {
            var key = new AttributeKey((int)attr, (int)modifier);
            return GetAttribute<T>(key);
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

        /// <summary>
        /// Get an attribute, if its not found return a custom default value.
        /// </summary>
        internal T GetAttributeOrCustomDefault<T>(ActorAttributeType attr, Func<T> defaultValue)
        {
            AttributeItem foundAttribute;
            if (!Items.TryGetValue((int)attr, out foundAttribute))
                return defaultValue();

            foundAttribute.Update();
            return foundAttribute.GetValue<T>();
        }

        internal T GetAttribute<T>(ActorAttributeType attr)
        {
            var foundAttribute = Items.FirstOrDefault(IsAttributeMatch(attr));
            if (foundAttribute.Value != null)
            {
                foundAttribute.Value.Update();
                return foundAttribute.Value.GetValue<T>();
            }
            return default(T);
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
                .Aggregate($"Attribute Properties ({Items.Count}/{Map.Count}) Id={(short)FastAttributeGroupId}/{FastAttributeGroupId} {Environment.NewLine}",
                (current, attr) => current + $" public {attr.Value.Descripter.Value.DataType} " +
                $"{attr.Value.Attribute} => GetCachedAttribute<{attr.Value.Descripter.Value.DataType}>" +
                $"(ActorAttributeType.{attr.Value.Attribute}); // {attr.Value.Attribute} " +
                $"({attr.Value.ModKey}) = i:{attr.Value.Integer} f:{attr.Value.Single} " +
                $"v:{attr.Value.GetValue()} ModifierType={attr.Value.Descripter.Value.ParameterType} " +
                $"Modifier={attr.Value.Key.ModifierId} {Environment.NewLine}");
        }
    }
}