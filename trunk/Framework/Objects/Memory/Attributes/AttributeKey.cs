using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory.Attributes
{
    public struct AttributeKey
    {
        public AttributeKey(int value)
        {
            Value = value;
        }

        public int Value;

        public AttributeKey(ActorAttributeType attribute, SNOPower power)
        {
            Value = ((int)power << 12) + ((int)attribute & 0xFFF);
        }

        public AttributeKey(int attribute, int modifier)
        {
            Value = (modifier << 12) + (attribute & 0xFFF);
        }

        public int DescripterId
        {
            get { return (Value & 0xFFF); }
            set { Value = (int)((Value & 0xFFFFF000) + (int)value); }
        }

        public ActorAttributeType BaseAttribute => (ActorAttributeType)(-1 << 12) + (DescripterId & 0xFFF);

        public int ModifierId
        {
            get { return Value >> 12; }
            set { Value = (Value & 0x00000FFF) + (value << 12); }
        }

        public override string ToString()
        {
            return $"Id: {DescripterId} Attribute: {BaseAttribute} Modifier: {ModifierId}";
        }

        public override int GetHashCode()
        {
            return unchecked((int)(Value ^ (Value >> 12)));
        }
    }
}
