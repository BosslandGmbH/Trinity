using System;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory.Items;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Objects.Memory.Attributes
{
    public class AttributeResult<TModifier, TValue> where TModifier : IConvertible
    {
        public AttributeResult(AttributeItem item)
        {
            AttributeType = item.Key.BaseAttribute;
            ModifierType = item.Descripter.ParameterType;
            Modifier = item.Key.ModifierId.To<TModifier>();
            Value = item.GetValue<TValue>();
        }

        public ActorAttributeType AttributeType;
        public AttributeParameterType ModifierType;
        public TValue Value;
        public TModifier Modifier;
    }
}


