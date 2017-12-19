using System;
using System.Globalization;

namespace Trinity.Components.QuestTools.Helpers
{
    public static class ConvertExtesions
    {


        // converts the source object variable value to the desired result type 
        public static TR ChangeType<TR>(this object value)
        {
            return (TR)ChangeType(value, typeof(TR));
        }

        // converts the source value, and if null returns the specified default value 
        public static TR ChangeType<TR>(this object value, TR whenNull)
        {
            return (value == null || value is DBNull)
                ? whenNull
                : (TR)ChangeType(value, typeof(TR));
        }

        public static object ChangeType(this object value, Type convertToType)
        {
            if (convertToType == null)
                throw new ArgumentNullException("convertToType");

            if (value is string)
            {
                value = (value as string).Trim('\'');
            }

            try
            {
                // return null if the value is null or DBNull
                if (value == null || value is DBNull)
                    return null;

                // non-nullable types, which are not supported by Convert.ChangeType(), // unwrap the types to determine the underlying time
                if (convertToType.IsGenericType && convertToType.GetGenericTypeDefinition() == typeof (Nullable<>))
                    convertToType = Nullable.GetUnderlyingType(convertToType);

                // deal with conversion to enum types when input is a string
                if (convertToType.IsEnum && value is string)
                    return Enum.Parse(convertToType, value as string);

                // deal with conversion to enum types when input is a integral primitive
                if (convertToType.IsEnum && value.GetType().IsPrimitive && !(value is bool) && !(value is char) && !(value is float) && !(value is double))
                    return Enum.ToObject(convertToType, value);

                // use Convert.ChangeType() to do all other conversions
                return Convert.ChangeType(value, convertToType, CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
            }
            catch (OverflowException) 
            {
                // Enum value is outside the range of the underlying type of enumType.
            }
            catch (ArgumentException) 
            {
                //enumType is not an Enum. 
                //-or- value is either an empty string ("") or only contains white space. 
                //-or- value is a name, but not one of the named constants defined for the enumeration.
            }

            return GetDefaultValue(convertToType);
        }

        private static object GetDefaultValue(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }

}
