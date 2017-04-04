using System;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Helpers;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory
{
    public class AttributeItem : MemoryWrapper, ITableItem
    {
        public Zeta.Game.Internals.AttributeDescriptor? Descripter = new Zeta.Game.Internals.AttributeDescriptor();

        public ActorAttributeType Attribute => Key.BaseAttribute;
        public int ModKey { get; private set; }
        public int Integer { get; private set; }
        public float Single { get; private set; }

        public int Size = 12;

        public ActorAttributes.AttributeKey Key;

        public object GetValue()
        {
            return Descripter?.DataType == typeof(Int32) ? Integer : Single > float.Epsilon ? Single : 0;
        }

        public object Modifer
        {
            get
            {
                switch (Descripter?.ParameterType)
                {
                    case Zeta.Game.Internals.AttributeParameterType.PowerSnoId:
                        return (SNOPower)Key.ModifierId;

                    case Zeta.Game.Internals.AttributeParameterType.ActorType:
                        return (SNOActor)Key.ModifierId;

                    case Zeta.Game.Internals.AttributeParameterType.ConversationSnoId:
                        return (SNOConversation)Key.ModifierId;

                    case Zeta.Game.Internals.AttributeParameterType.InventoryLocation:
                        return (InventorySlot)Key.ModifierId;

                    case Zeta.Game.Internals.AttributeParameterType.DamageType:
                        return (DamageType)Key.ModifierId;

                    case Zeta.Game.Internals.AttributeParameterType.PowerSnoId2:
                        return (SNOPower)Key.ModifierId;

                    case Zeta.Game.Internals.AttributeParameterType.ResourceType:
                        return (ResourceType)Key.ModifierId;

                    case Zeta.Game.Internals.AttributeParameterType.RequirementType:
                        return (RequirementType)Key.ModifierId;
                }
                return Key.ModifierId;
            }
        }

        public T GetValue<T>()
        {
            try
            {
                var type = TypeUtil<T>.TypeOf;
                if (Integer is T)
                {
                    if (Descripter != null && Descripter?.DataType == typeof(Int32))
                    {
                        return (T)Convert.ChangeType(Integer, TypeUtil<T>.TypeOf);
                    }
                    if (Single > Int32.MinValue && Single < Int32.MaxValue)
                    {
                        return (T)Convert.ChangeType(Single, type);
                    }
                }
                if (Single is T)
                {
                    if (Descripter != null && Descripter?.DataType == typeof(Int32))
                    {
                        return (T)Convert.ChangeType(Integer, TypeUtil<T>.TypeOf);
                    }
                    return (T)Convert.ChangeType(Single, type);
                }
                if (type.IsEnum)
                {
                    return (T)Enum.ToObject(type, Integer);
                }
                return (T)Convert.ChangeType(GetValue(), type);
            }
            catch (Exception ex)
            {
                Core.Logger.Log($"GetValue<{typeof(T)}>() type conversion failed {ex}");
            }
            return default(T);
        }

        protected override void OnUpdated()
        {
            if (!IsValid) return;

            ModKey = ZetaDia.Memory.Read<int>(BaseAddress + 4);
            Key = new ActorAttributes.AttributeKey(ModKey);

            Zeta.Game.Internals.AttributeDescriptor descripter;

            if (AttributeManager.AttributeDescriptors != null && AttributeManager.AttributeDescriptors.TryGetValue(Key.DescripterId, out descripter))
            {
                Descripter = descripter;

                if (Descripter?.DataType == typeof(Int32))
                {
                    Integer = ZetaDia.Memory.Read<int>(BaseAddress + 8);
                }
                else
                {
                    Single = ZetaDia.Memory.Read<float>(BaseAddress + 8);
                }
            }
            else
            {
                Integer = ZetaDia.Memory.Read<int>(BaseAddress + 8);
                Single = ZetaDia.Memory.Read<float>(BaseAddress + 8);
            }
        }

        public override string ToString()
        {
            var modString = Modifer.GetType() != typeof(int) ? $" [ {Descripter?.ParameterType}: {Modifer}: {Key.ModifierId} ]" : string.Empty;

            return $"{Key.DescripterId}: {Key.BaseAttribute} ({(int)Key.BaseAttribute}){modString} i:{Integer} f:{Single} Value={((float)GetValue()):0.##}";
        }
    }
}