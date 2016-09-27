using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Trinity.Framework.Helpers
{


    /// <summary>
    /// Non-generic class allowing properties to be copied from one instance
    /// to another existing instance of a potentially different type.
    /// </summary>
    public static class PropertyCopy
    {
        /// <summary>
        /// Copies all public, readable properties from the source object to the
        /// target. The target type does not have to have a parameterless constructor,
        /// as no new instance needs to be created.
        /// </summary>
        /// <remarks>Only the properties of the source and target types themselves
        /// are taken into account, regardless of the actual types of the arguments.</remarks>
        /// <typeparam name="TSource">Type of the source</typeparam>
        /// <typeparam name="TTarget">Type of the target</typeparam>
        /// <param name="source">Source to copy properties from</param>
        /// <param name="target">Target to copy properties to</param>
        public static void Copy<TSource, TTarget>(TSource source, TTarget target, PropertyCopyOptions options = null)
            where TSource : class
            where TTarget : class
        {            
            PropertyCopier<TSource, TTarget>.Copy(source, target, options ?? new PropertyCopyOptions());
        }

        public static object CopyDerived(object obj, object otherObject)
        {
            var srcFields = otherObject.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            var destFields = obj.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            foreach (var property in srcFields)
            {
                var dest = destFields.FirstOrDefault(x => x.Name == property.Name);
                if (dest != null && dest.CanWrite)
                {
                    dest.SetValue(obj, property.GetValue(otherObject, null), null);
                }
            }
            return obj;
        }
    }

    public class PropertyCopyOptions
    {
        public bool IgnoreNulls;
    }

    /// <summary>
    /// Generic class which copies to its target type from a source
    /// type specified in the Copy method. The types are specified
    /// separately to take advantage of type inference on generic
    /// method arguments.
    /// </summary>
    public static class PropertyCopy<TTarget> where TTarget : class, new()
    {
        /// <summary>
        /// Copies all readable properties from the source to a new instance
        /// of TTarget.
        /// </summary>
        public static TTarget CopyFrom<TSource>(TSource source) where TSource : class
        {
            return PropertyCopier<TSource, TTarget>.Copy(source);
        }
    }

    //public static class DefaultValueCache
    //{
    //    private static Dictionary<Type, object> defaultMap = new Dictionary<Type, object>();

    //    public static object GetDefault(Type type)
    //    {
    //        object value;
    //        if (!defaultMap.TryGetValue(type, out value))
    //        {
    //            var body = Expression.Default(type);
    //            var lambda = Expression.Lambda(body);
    //            var func = lambda.Compile();
    //            value = func.DynamicInvoke();
    //            defaultMap[type] = value;
    //        }
    //        return value;
    //    }
    //}

    /// <summary>
    /// Static class to efficiently store the compiled delegate which can
    /// do the copying. We need a bit of work to ensure that exceptions are
    /// appropriately propagated, as the exception is generated at type initialization
    /// time, but we wish it to be thrown as an ArgumentException.
    /// Note that this type we do not have a constructor constraint on TTarget, because
    /// we only use the constructor when we use the form which creates a new instance.
    /// </summary>
    internal static class PropertyCopier<TSource, TTarget>
    {
        /// <summary>
        /// Delegate to create a new instance of the target type given an instance of the
        /// source type. This is a single delegate from an expression tree.
        /// </summary>
        private static readonly Func<TSource, TTarget> creator;

        /// <summary>
        /// List of properties to grab values from. The corresponding targetProperties 
        /// list contains the same properties in the target type. Unfortunately we can't
        /// use expression trees to do this, because we basically need a sequence of statements.
        /// We could build a DynamicMethod, but that's significantly more work :) Please mail
        /// me if you really need this...
        /// </summary>
        private static readonly List<PropertyInfo> sourceProperties = new List<PropertyInfo>();
        private static readonly List<PropertyInfo> targetProperties = new List<PropertyInfo>();
        private static readonly Exception initializationException;

        internal static TTarget Copy(TSource source)
        {
            if (initializationException != null)
            {
                throw initializationException;
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return creator(source);
        }

        internal static void Copy(TSource source, TTarget target, PropertyCopyOptions options)
        {
            if (initializationException != null)
            {
                throw initializationException;
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            for (int i = 0; i < sourceProperties.Count; i++)
            {
                var value = sourceProperties[i].GetValue(source, null);
                if (options != null && !options.IgnoreNulls || !ReferenceEquals(value, null))
                {
                    if (target != null)
                    {
                        targetProperties[i].SetValue(target, value, null);
                    }
                }
            }

        }

        static PropertyCopier()
        {
            try
            {
                creator = BuildCreator();
                initializationException = null;
            }
            catch (Exception e)
            {
                creator = null;
                initializationException = e;
            }
        }

        private static Func<TSource, TTarget> BuildCreator()
        {
            ParameterExpression sourceParameter = Expression.Parameter(typeof(TSource), "source");
            var bindings = new List<MemberBinding>();
            foreach (PropertyInfo sourceProperty in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!sourceProperty.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name);
                if (targetProperty == null)
                {
                    continue;
                    //throw new ArgumentException("Property " + sourceProperty.Name + " is not present and accessible in " + typeof(TTarget).FullName);
                }
                if (targetProperty.GetCustomAttribute(typeof(IgnoreDataMemberAttribute)) != null)
                {
                    continue;
                }
                if (!targetProperty.CanWrite)
                {
                    continue;
                    //throw new ArgumentException("Property " + sourceProperty.Name + " is not writable in " + typeof(TTarget).FullName);
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                    //throw new ArgumentException("Property " + sourceProperty.Name + " is static in " + typeof(TTarget).FullName);
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    continue;
                    //throw new ArgumentException("Property " + sourceProperty.Name + " has an incompatible type in " + typeof(TTarget).FullName);
                }
                bindings.Add(Expression.Bind(targetProperty, Expression.Property(sourceParameter, sourceProperty)));
                sourceProperties.Add(sourceProperty);
                targetProperties.Add(targetProperty);
            }
            Expression initializer = Expression.MemberInit(Expression.New(typeof(TTarget)), bindings);
            return Expression.Lambda<Func<TSource, TTarget>>(initializer, sourceParameter).Compile();
        }
    }
}