using System;
using System.Collections.Generic;
using System.Linq;

namespace Trinity.Framework.Helpers
{
    public static class EnumExtensions
    {
        // todo: refactor extension methods with an central 'operate' method that can perform enum operation XOR/OR for two values handling detection/conversion of signed and unsigmed.

        private static bool IsSignedTypeCode(TypeCode code)
        {
            switch (code)
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return false;
                default:
                    return true;
            }
        }

        public static bool IsOptionSet(this Enum value, Enum option)
        {
            if (IsSignedTypeCode(value.GetTypeCode()))
            {
                long longVal = Convert.ToInt64(value);
                long longOpt = Convert.ToInt64(option);
                return (longVal & longOpt) == longOpt;
            }
            else
            {
                ulong longVal = Convert.ToUInt64(value);
                ulong longOpt = Convert.ToUInt64(option);
                return (longVal & longOpt) == longOpt;
            }
        }

        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        public static Dictionary<int, string> ToDictionary(this Enum @enum, bool lowerCase = false)
        {
            var type = @enum.GetType();
            return Enum.GetValues(type).Cast<int>().ToDictionary(e => e, e =>
            {
                var name = Enum.GetName(type, e);
                return lowerCase && name != null ? name.ToLower() : Enum.GetName(type, e);
            });
        }

        public static List<T> ToList<T>(this Enum input, bool skipDefault = false)
        {
            var defaultValue = default(T);
            return Enum.GetValues(input.GetType()).Cast<T>().Where(e => !defaultValue.Equals(e)).ToList();
        }

        public static HashSet<T> ToHashSet<T>(this Enum input, bool skipDefault = false)
        {
            return new HashSet<T>(input.ToList<T>(skipDefault));
        }

        public static bool Contains<T>(this Enum input, T value)
        {
            return new HashSet<T>(input.ToList<T>(true)).Contains(value);
        }

        public static IEnumerable<T> GetFlags<T>(this Enum input, bool excludeDefault = true) where T : struct
        {
            var defaultValue = default(T);
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value) && !defaultValue.Equals(value))
                    yield return (T)(object)value;
        }

        public static bool HasAny<T>(this Enum type, T value) where T : struct
        {
            try
            {
                if (!type.Is(value))
                    return false;

                var findMask = Convert.ToInt64(value);
                return type.GetFlags<T>().Any(f =>
                {
                    var num = Convert.ToInt64(f);
                    return num > 0 && (findMask & num) == num;
                });
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return Convert.ToInt64(type) == Convert.ToInt64(value);
            }
            catch
            {
                return false;
            }
        }


        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(Convert.ToInt64(type) | Convert.ToInt64(value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    String.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }


        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(Convert.ToInt64(type) & ~Convert.ToInt64(value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    String.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }
    }

}

