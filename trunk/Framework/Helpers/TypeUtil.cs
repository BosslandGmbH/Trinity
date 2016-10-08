using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Objects.Memory;

namespace Trinity.Framework.Helpers
{
    public static class TypeUtil<T>
    {
        public static readonly int SizeOf;
        public static readonly bool IsMemoryWrapperType;
        public static readonly bool IsNumericType;
        public static readonly Type TypeOf;

        static TypeUtil()
        {
            TypeOf = typeof(T);
            IsMemoryWrapperType = TypeOf.IsMemoryWrapperType();
            SizeOf = TypeOf.SizeOf();      
            IsNumericType = TypeOf.IsNumericType();
        }
    }

    public static class TypeUtil
    { 
        private static readonly ConcurrentDictionary<Type, int> _cachedSizeOf = new ConcurrentDictionary<Type, int>();
        
        private static readonly ConcurrentDictionary<Type, bool> _cachedIsMemoryObject = new ConcurrentDictionary<Type, bool>();

        public static bool IsMemoryWrapperType(this Type type)
        {
            return _cachedIsMemoryObject.GetOrAdd(type, (t) => t.IsSubclassOf(typeof(MemoryWrapper)) || t == typeof(MemoryWrapper));
        }

        public static int SizeOf(this Type type)
        {
            return _cachedSizeOf.GetOrAdd(type, (t) =>
            {
                if (t.IsMemoryWrapperType())
                    return t.GetMemoryWrapperSize();

                if(type.IsValueType)
                    return Marshal.SizeOf(t.IsEnum ? t.GetEnumUnderlyingType() : t);

                return 0;
            });
        }

        public static int GetMemoryWrapperSize(this Type type)
        {
            var value = 0;
            if (type.IsSubclassOf(typeof(MemoryWrapper)))
            {
                var field = type.GetField("SizeOf",
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Static |
                    BindingFlags.Public);

                if (field == null)
                    return 0;
                value = field.IsLiteral ? (int) field.GetRawConstantValue() : (int) field.GetValue(null);
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(type), "Negative value is not allowed");
                return value;
            }
            return value;
        }

        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
