using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Helpers
{
    public static class EnumExtensions
    {
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

        public static bool Has<T>(this System.Enum type, T value)
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

        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }


        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }


        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

    }

}

