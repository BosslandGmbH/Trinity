//!CompilerOption:AddRef:Microsoft.CSharp.dll
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Objects.Native
{
    public static class ReflectionHelper
    {

        public static Func<T> GetStaticAccessor<T>(Type containingClassType, string memberName)
        {
            var param = Expression.Parameter(containingClassType, "arg");
            var member = StaticPropertyOrField(containingClassType, memberName);
            var lambda = Expression.Lambda(member);
            return (Func<T>)lambda.Compile();
        }

        public static Func<T> GetStaticPropertyAccessor<T>(Type containingType)
        {
            var type = typeof(T);
            var member = StaticPropertyExressionByType(containingType, type);
            var lambda = Expression.Lambda(member);
            return (Func<T>)lambda.Compile();
        }

        public static Func<T> GetStaticFieldAccessor<T>(Type containingType)
        {
            var type = typeof(T);
            var member = StaticPropertyExressionByType(containingType, type);
            var lambda = Expression.Lambda(member);
            return (Func<T>)lambda.Compile();
        }

        public static T CreateNonPublicInstance<T>(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null, args, null);
        }

        public static object CreateNonPublicInstance(Type type, params object[] args)
        {
            return Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, args, null);
        }

        public static T UnsafeCreate<T>(this IntPtr instance)
        {
            return (T)Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { instance }, null);
        }

        public static T To<T>(this ActorCommonData instance) where T : ACD
        {
            return (T)Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { instance.BaseAddress }, null);
        }

        public static T UnsafeCreate<T>(this ACD instance) where T : ACD
        {
            return (T)Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { instance.BaseAddress }, null);
        }

        public static string DumpOffsets<T>() where T : struct
        {
            var type = typeof (T);
            var db = new StringBuilder();
            foreach(var field in type.GetFields())
            {
                db.AppendLine($" {field.Name} = ReadOffset<{field.FieldType.Name}>(0x{Marshal.OffsetOf(type, field.Name).ToString("x")});");
            }
            return db.ToString();
        }

        public static string DumpOffsets<T>(this T obj) where T : NativeObject
        {
            var type = typeof(T);
            var db = new StringBuilder();
            foreach (var property in type.GetProperties())
            {
                dynamic value = Convert.ChangeType(property.GetValue(obj), property.PropertyType);
                if (property.PropertyType.IsValueType)
                {
                    var offset = MemoryHelper.GetOffetOfValue(obj.BaseAddress, value);
                    db.AppendLine($" public {property.PropertyType.Name} {property.Name} => ReadOffset<{property.PropertyType.Name}>(0x{offset.ToString("x")});");
                }
            }
            return db.ToString();
        }

        public static Func<T, TR> GetInstanceAccessor<T, TR>(string memberName)
        {
            var type = typeof(T);
            var param = Expression.Parameter(type, "arg");
            var member = Expression.PropertyOrField(param, memberName);
            var lambda = Expression.Lambda(typeof(Func<T, TR>), member, param);
            return (Func<T, TR>)lambda.Compile();
        }

        public static Type GetEnumerableType(Type type)
        {
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return intType.GetGenericArguments()[0];
                }
            }
            return null;
        }



        public static MemberExpression StaticPropertyOrField(Type type, string propertyOrFieldName)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            PropertyInfo property = type.GetProperty(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static);
            if (property != null)
            {
                return Expression.Property(null, property);
            }
            FieldInfo field = type.GetField(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static);
            if (field == null)
            {
                property = type.GetProperty(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Static);
                if (property != null)
                {
                    return Expression.Property(null, property);
                }
                field = type.GetField(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Static);
                if (field == null)
                {
                    throw new ArgumentException(String.Format("{0} NotAMemberOfType {1}", propertyOrFieldName, type));
                }
            }
            return Expression.Field(null, field);
        }

        public static MemberExpression StaticPropertyExressionByType(Type parentType, Type propertyOrFieldType)
        {
            if (parentType == null)
                throw new ArgumentNullException(nameof(parentType));

            var result = (from propInfo in parentType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static)
                          where propInfo.PropertyType == propertyOrFieldType
                          select Expression.Property(null, propInfo)).FirstOrDefault();

            if (result != null)
                return result;

            return (from propInfo in parentType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Static)
                    where propInfo.PropertyType == propertyOrFieldType
                    select Expression.Property(null, propInfo)).FirstOrDefault();
        }

        public static MemberExpression StaticFieldExressionByType(Type parentType, Type propertyOrFieldType)
        {
            if (parentType == null)
                throw new ArgumentNullException(nameof(parentType));

            var result = (from fieldInfo in parentType.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static)
                          where fieldInfo.FieldType == propertyOrFieldType
                          select Expression.Field(null, fieldInfo)).FirstOrDefault();

            if (result != null)
                return result;

            return (from fieldInfo in parentType.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Static)
                    where fieldInfo.FieldType == propertyOrFieldType
                    select Expression.Field(null, fieldInfo)).FirstOrDefault();
        }

        private static IEnumerable<Type> _hiddenClasses;

        public static IEnumerable<Type> GetClasses()
        {
            if (_hiddenClasses != null)
                return _hiddenClasses;

            _hiddenClasses = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass && t.Module.Name == "Demonbuddy.exe" && !t.IsNotPublic);

            return _hiddenClasses;
        }

        public static bool HasEmptyConstructor(this Type t)
        {
            return t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) != null;
        }

        public static bool IsMemberPattern(this Type t, bool isClass, int nonPublicFieldCount, int nonPublicPropertyCount, int nonPublicMethodCount)
        {
            if (!t.IsClass)
                return false;

            if (t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Count() != nonPublicFieldCount)
                return false;

            if (t.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic).Count() != nonPublicPropertyCount)
                return false;

            if (t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Count() != nonPublicMethodCount)
                return false;

            return true;

        }
    }
}

