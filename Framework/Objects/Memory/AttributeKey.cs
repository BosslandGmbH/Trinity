using System.Collections.Generic;
using System.Linq;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory
{
    //public class AttributeKey
    //{
    //    public AttributeKey(int value)
    //    {
    //        Value = value;
    //    }

    //    public int Value;

    //    public AttributeKey(ActorAttributeType attribute, SNOPower power)
    //    {
    //        Value = ((int)power << 12) + ((int)attribute & 0xFFF);
    //    }

    //    public AttributeKey(int attribute, int modifier)
    //    {
    //        Value = (modifier << 12) + (attribute & 0xFFF);
    //    }

    //    public int DescripterId
    //    {
    //        get { return (Value & 0xFFF); }
    //        set { Value = (int)((Value & 0xFFFFF000) + (int)value); }
    //    }

    //    public ActorAttributeType BaseAttribute => (ActorAttributeType)(-1 << 12) + (DescripterId & 0xFFF);

    //    private AttributeDescriptor? _descriptor;
    //    public AttributeDescriptor Descriptor => _descriptor ?? (_descriptor = DescriptorHelper.GetDescriptor(DescripterId)).Value;

    //    public int ModifierId
    //    {
    //        get { return Value >> 12; }
    //        set { Value = (Value & 0x00000FFF) + (value << 12); }
    //    }

    //    public object Modifer
    //    {
    //        get
    //        {
    //            switch (Descriptor.ParameterType)
    //            {
    //                case Zeta.Game.Internals.AttributeParameterType.PowerSnoId:
    //                    return (SNOPower)ModifierId;
    //                case Zeta.Game.Internals.AttributeParameterType.ActorType:
    //                    return (SNOActor)ModifierId;
    //                case Zeta.Game.Internals.AttributeParameterType.ConversationSnoId:
    //                    return (SNOConversation)ModifierId;
    //                case Zeta.Game.Internals.AttributeParameterType.InventoryLocation:
    //                    return (InventorySlot)ModifierId;
    //                case Zeta.Game.Internals.AttributeParameterType.DamageType:
    //                    return (DamageType)ModifierId;
    //                case Zeta.Game.Internals.AttributeParameterType.PowerSnoId2:
    //                    return (SNOPower)ModifierId;
    //                case Zeta.Game.Internals.AttributeParameterType.ResourceType:
    //                    return (ResourceType)ModifierId;
    //                case Zeta.Game.Internals.AttributeParameterType.RequirementType:
    //                    return (RequirementType)ModifierId;
    //            }
    //            return ModifierId;
    //        }
    //    }

    //    public string ModifierInfo => ModifierId > 0 ? $" [ {Descriptor.ParameterType}: {Modifer}: {ModifierId} ]" : ModifierId.ToString();

    //    public override int GetHashCode() => unchecked((int)(Value ^ (Value >> 12)));

    //    public override string ToString() => $"Id: {DescripterId} Attribute: {BaseAttribute} Modifier: {ModifierInfo}";
    //}

    //public static class DescriptorHelper
    //{
    //    private static Dictionary<int, Zeta.Game.Internals.AttributeDescriptor> _descripters;

    //    public static Zeta.Game.Internals.AttributeDescriptor GetDescriptor(int id, bool checkExists = false)
    //    {
    //        if (_descripters == null)
    //            _descripters = ZetaDia.AttributeDescriptors.ToDictionary(descripter => descripter.Id);

    //        return !checkExists || _descripters.ContainsKey(id) ? _descripters[id] : default(Zeta.Game.Internals.AttributeDescriptor);
    //    }
    //}
}