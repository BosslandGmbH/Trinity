using Trinity.Framework.Objects.Memory;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using DamageType = Zeta.Game.DamageType;

namespace Trinity.Framework.Actors.Attributes
{
    /// <summary>
    /// A helper object for easy bit translations of key used to index Attributes in memory.
    /// </summary>
    internal class AttributeKey
    {
        /// <summary>
        /// The key value as found in Memory Attribute table.
        /// </summary>
        public int Value;

        /// <summary>
        /// Create AttributeKey with a key value
        /// </summary>
        /// <param name="value"></param>
        public AttributeKey(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Create AttributeKey using seperate attribute and modifier.
        /// </summary>
        /// <param name="attribute">the ActorAttributeType</param>
        /// <param name="modifier">the attribute specific modifier</param>
        public AttributeKey(int attribute, int modifier)
        {
            Value = (modifier << 12) + (attribute & 0xFFF);
        }

        /// <summary>
        /// The descripter id portion of a key value.
        /// </summary>
        public int DescripterId
        {
            get => (Value & 0xFFF);
            set => Value = (int)((Value & 0xFFFFF000) + (int)value);
        }

        /// <summary>
        /// The base ActorAttributeType portion of a key value.
        /// </summary>
        public ActorAttributeType BaseAttribute => (ActorAttributeType)(-1 << 12) + (DescripterId & 0xFFF);

        private AttributeDescriptor? _descriptor;

        public AttributeDescriptor Descriptor
            => _descriptor ?? (_descriptor = DescriptorHelper.GetDescriptor(DescripterId, true)).Value;

        /// <summary>
        /// The modifier portion of a key value.
        /// </summary>
        public int ModifierId
        {
            get => Value >> 12;
            set => Value = (Value & 0x00000FFF) + (value << 12);
        }

        /// <summary>
        /// The ModifierId cast to known Parameter Type if possible
        /// </summary>
        public object Modifer
        {
            get
            {
                switch (Descriptor.ParameterType)
                {
                    case AttributeParameterType.PowerSnoId:
                        return (SNOPower)ModifierId;
                    case AttributeParameterType.ActorType:
                        return (SNOActor)ModifierId;
                    case AttributeParameterType.ConversationSnoId:
                        return (SNOConversation)ModifierId;
                    case AttributeParameterType.InventoryLocation:
                        return (InventorySlot)ModifierId;
                    case AttributeParameterType.DamageType:
                        return (DamageType)ModifierId;
                    case AttributeParameterType.PowerSnoId2:
                        return (SNOPower)ModifierId;
                    case AttributeParameterType.ResourceType:
                        return (ResourceType)ModifierId;
                    case AttributeParameterType.RequirementType:
                        return (RequirementType)ModifierId;
                }
                return ModifierId;
            }
        }

        public string ModifierInfo
            => ModifierId > 0 ? $" [ {Descriptor.ParameterType}: {Modifer}: {ModifierId} ]" : ModifierId.ToString();

        public override int GetHashCode() => unchecked((int)(Value ^ (Value >> 12)));

        public override string ToString()
            => $"Id: {DescripterId} Attribute: {BaseAttribute} Modifier: {ModifierInfo}";
    }
}