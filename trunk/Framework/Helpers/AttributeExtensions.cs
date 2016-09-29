using System;
using System.Collections.Generic;
using System.Linq;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Framework.Helpers
{
    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }

        public static T FindMultiAttributeValue<T>(this ACD acd, SNOPower power, IEnumerable<ActorAttributeType> attributes, Func<T, bool> condition) where T : struct
        {
            return acd.FindMultiAttributeValue(new List<SNOPower> { power }, attributes, condition);
        }

        public static T FindMultiAttributeValue<T>(this ACD acd, IEnumerable<SNOPower> powers, IEnumerable<ActorAttributeType> attributes, Func<T, bool> condition) where T : struct
        {
            if (condition == null)
                return default(T);

            foreach (var power in powers)
            {
                foreach (var att in attributes)
                {
                    var newValue = acd.GetAttribute<T>((int)att & ((1 << 12) - 1) | ((int)power << 12));

                    Logger.Log("Power: {0}  Att: {1} Val: {2}", power, att, newValue);

                    if (condition(newValue))
                        return newValue;
                }
            }

            return default(T);
        }

    }
}

