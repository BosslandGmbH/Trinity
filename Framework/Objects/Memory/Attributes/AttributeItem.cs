using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory.Attributes;
using Trinity.Framework.Objects.Memory.Containers;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using static System.Int32;
using AttributeKey = Trinity.Framework.Objects.Memory.Attributes.AttributeKey;

namespace Trinity.Framework.Objects.Memory.Items
{


    public class AttributeItem : MemoryWrapper, ITableItem
    {
        public AttributeDescripter Descripter = new AttributeDescripter();
        public ActorAttributeType Attribute => Key.BaseAttribute;
        public int ModKey { get; private set; }
        public int Integer { get; private set; }
        public float Single { get; private set; }

        public int Size = 12;

        public AttributeKey Key;

        public object GetValue()
        {
            return Descripter.IsInteger ? Integer : Single > float.Epsilon ? Single : 0;
        }

        public object Modifer
        {
            get
            {
                switch (Descripter.ParameterType)
                {
                    case AttributeParameterType.PowerSnoId:
                        return (SNOPower)Key.ModifierId;
                    case AttributeParameterType.ActorType:
                        return (SNOActor)Key.ModifierId;
                    case AttributeParameterType.ConversationSnoId:
                        return (SNOConversation)Key.ModifierId;
                    case AttributeParameterType.InventoryLocation:
                        return (InventorySlot)Key.ModifierId;
                    case AttributeParameterType.DamageType:
                        return (DamageType)Key.ModifierId;
                    case AttributeParameterType.PowerSnoId2:
                        return (SNOPower)Key.ModifierId;
                    case AttributeParameterType.ResourceType:
                        return (ResourceType)Key.ModifierId;
                    case AttributeParameterType.RequirementType:
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
                    if (Descripter.IsInteger)
                    {
                        return (T)Convert.ChangeType(Integer, TypeUtil<T>.TypeOf);
                    }

                    if (Single > MinValue && Single < MaxValue)
                    {
                        return (T)Convert.ChangeType(Single, type);
                    }
                }
                if (Single is T)
                {
                    if (Descripter.IsInteger)
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
                Logger.Log($"GetValue<{typeof(T)}>() type conversion failed {ex}");
            }
            return default(T);
        }

        protected override void OnUpdated()
        {
            if (!IsValid) return;

            ModKey = ZetaDia.Memory.Read<int>(BaseAddress + 4);
            Key = new AttributeKey(ModKey);
            AttributeDescripter descripter;
            AttributeManager.AttributeDescriptors.TryGetValue(Key.DescripterId, out descripter);

            if (descripter != null)
            {
                Descripter = descripter;

                if (Descripter.IsInteger)
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
            var modString = Modifer.GetType() != typeof (int) ? $" [ {Descripter.ParameterType}: {Modifer}: {Key.ModifierId} ]" : string.Empty;

            return $"{Key.DescripterId}: {Key.BaseAttribute} ({(int)Key.BaseAttribute}){modString} i:{Integer} f:{Single} Value={((float)GetValue()).ToString("0.##")}";
        }

    }
}


