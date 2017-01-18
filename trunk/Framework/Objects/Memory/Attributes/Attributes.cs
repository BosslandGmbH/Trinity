using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory.Containers;
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
        public BasicMap<AttributeItem> PtrMap;
        public AttributeGroup Group;
        private BasicMap<AttributeItem> Map;
        private static uint AttributeHasher(int key)
        {
            return unchecked((uint)(key ^ (key >> 12)));
        }
        public bool IsValid => Group != null && Group.IsValid && Group.Id != 0 && Map != null && Map.IsValid && Map.Count != 0;

        public Attributes() { }

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

                FastAttributeGroupId = groupId;
                Group = AttributeManager.FindGroup(groupId);

                if (Group == null || !Group.IsValid)
                {
                    Logger.LogVerbose($"Attribute group is invalid with id: {groupId}");
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
                    Logger.LogVerbose($"Attribute Map Invalid for groupId: {groupId}");
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
                Logger.LogDebug($"Exception creating attributes {ex} for groupId {groupId}");
            }
        }

        private Dictionary<int, AttributeItem> _previousItems = new Dictionary<int, AttributeItem>();

        private DateTime LastUpdatedTime = DateTime.MinValue;

        public bool Update()
        {
            if (!IsValid)
                return false;

            var isChanged = Map.Count != _lastRowCount;
             
            //    _previousItems = new Dictionary<int, AttributeItem>(Items);

            //    ReadAttributes(FastAttributeGroupId);

            //    foreach (var newItem in Items)
            //    {
            //        var key = newItem.Key;
            //        if (!_previousItems.ContainsKey(key))
            //        {
            //            Logger.Log("Attribute Added " + newItem.Value);
            //        }
            //        else
            //        {
            //            var oldValue = _previousItems[key].GetValue();
            //            if (!Equals(newItem.Value.GetValue(), oldValue))
            //            {
            //                Logger.Log($"Attribute Changed {newItem.Value }, was {oldValue}");
            //            }
            //        }
            //        _previousItems.Remove(key);
            //    }

            //    foreach (var removedItem in _previousItems)
            //    {
            //        Logger.Log("Attribute Removed " + removedItem);
            //    }

            //    _lastRowCount = Map.Count;
            //    return isChanged;
         
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
                //if (shouldUpdate)
                //{
                    //a.Value.Update();
                //}

                if (a.Value.Descripter.ParameterType == AttributeParameterType.PowerSnoId || a.Value.Descripter.ParameterType == AttributeParameterType.PowerSnoId2)
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

        //public class PowerCooldown
        //{
        //    public SNOPower Power;
        //    public AttributeKey Key;
        //    public int EndTick;
        //}

        //public List<PowerCooldown> _cooldowns;

        //internal List<PowerCooldown> GetCooldowns()
        //{
        //    var shouldUpdate = LastUpdatedFrame != ZetaDia.Memory.Executor.FrameCount && (LastUpdatedFrame = ZetaDia.Memory.Executor.FrameCount) != 0;
        //    if (!shouldUpdate)
        //    {
        //        return _cooldowns;
        //    }

        //    var result = new List<PowerCooldown>();
        //    foreach (var a in Items)
        //    {
        //        a.Value.Update();
        //        if (a.Value.Descripter.ParameterType == AttributeParameterType.PowerSnoId && a.Value.Integer > 0)
        //        {
        //            var index = GetEndTickIndex(a.Value.Key.BaseAttribute);
        //            var endTickAttr = 
        //            var slot = a.Value.Key.BaseAttribute.ToString().Split("BuffIconEndTick").LastOrDefault();
        //            if (string.IsNullOrEmpty())
        //                continue;

        //            var cd = new PowerCooldown
        //            {
        //                Key = a.Value.Key,
        //                Power = a.Value.Key.ModifierId.To<SNOPower>(),
        //                EndTick = GetAttribute<int>(ActorAttributeType.BuffIconEndTick0)
        //            };
        //        }
        //    }

        //    _cooldowns = result;
        //    return _cooldowns;
        //}

        //private int GetEndTickIndex(ActorAttributeType type)
        //{
        //    switch ((int)type)
        //    {
        //        case -3478: return 0;
        //        case -3477: return 1;
        //        case -3476: return 2;
        //        case -3475: return 3;
        //        case -3474: return 4;
        //        case -3473: return 5;
        //        case -3472: return 6;
        //        case -3471: return 7;
        //        case -3470: return 8;
        //        case -3469: return 9;
        //        case -3468: return 10;
        //        case -3467: return 11;
        //        case -3466: return 12;
        //        case -3465: return 13;
        //        case -3464: return 14;
        //        case -3463: return 15;
        //        case -3462: return 16;
        //        case -3461: return 17;
        //        case -3460: return 18;
        //        case -3459: return 19;
        //        case -3458: return 20;
        //        case -3457: return 21;
        //        case -3456: return 22;
        //        case -3455: return 23;
        //        case -3454: return 24;
        //        case -3453: return 25;
        //        case -3452: return 26;
        //        case -3451: return 27;
        //        case -3450: return 28;
        //        case -3449: return 29;
        //        case -3448: return 30;
        //        case -3447: return 31;
        //    }
        //    return -1;
        //}

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
                .Aggregate($"Attribute Properties ({Items.Count}/{Map.Count}) Id={(short)FastAttributeGroupId}/{FastAttributeGroupId} {Environment.NewLine}",
                (current, attr) => current + $" public {attr.Value.Descripter.DataType} " +
                $"{attr.Value.Attribute} => GetCachedAttribute<{attr.Value.Descripter.DataType}>" +
                $"(ActorAttributeType.{attr.Value.Attribute}); // {attr.Value.Attribute} " +
                $"({attr.Value.ModKey}) = i:{attr.Value.Integer} f:{attr.Value.Single} " +
                $"v:{attr.Value.GetValue()} ModifierType={attr.Value.Descripter.ParameterType} " +
                $"Modifier={attr.Value.Key.ModifierId} {Environment.NewLine}");
        }

    }
}

