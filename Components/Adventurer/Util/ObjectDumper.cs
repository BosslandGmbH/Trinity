using System.Collections;
using System.Linq; using Trinity.Framework;
using System.Reflection;

namespace Trinity.Components.Adventurer.Util
{
    public class ObjectDumper
    {
        private readonly int _maxDepth;

        public static void Write(object element, int maxDepth)
        {
            var dumper = new ObjectDumper(maxDepth);
            dumper.WriteObject(element, 0);
        }

        private ObjectDumper(int maxDepth)
        {
            _maxDepth = maxDepth;
        }

        private void WriteObject(object element, int level, int extraDepth = 0)
        {
            if (element is IEnumerable)
            {
                var enumValues = element as IEnumerable;
                foreach (var enumValue in enumValues)
                {
                    if (enumValue.GetType().IsValueType || enumValue is string)
                    {
                        LogValue(level, extraDepth, enumValue);
                    }
                    else
                    {
                        Core.Logger.Raw("{0}{{", GetIndent(level, extraDepth + 1));
                        WriteObject(enumValue, level, extraDepth + 1);
                        Core.Logger.Raw("{0}}},", GetIndent(level, extraDepth + 1));
                    }
                }
                return;
            }
            var members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance).OrderBy(m => m.Name).ToList();
            foreach (var memberInfo in members)
            {
                if (memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
                {
                    continue;
                }
                var fieldInfo = memberInfo as FieldInfo;
                var propertyInfo = memberInfo as PropertyInfo;

                object value = null;
                var isValueType = false;

                if (fieldInfo != null)
                {
                    value = fieldInfo.GetValue(element);
                    isValueType = fieldInfo.FieldType.IsValueType || fieldInfo.FieldType == typeof(string);
                }
                if (value == null && propertyInfo != null)
                {
                    value = propertyInfo.GetValue(element, null);
                    isValueType = propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string);
                }
                if (value == null)
                {
                    Core.Logger.Raw("{0}{1}: {2}", GetIndent(level, extraDepth), memberInfo.Name, "null");
                    continue;
                }

                if (isValueType || level == _maxDepth)
                {
                    // Output might be to confusing to read if enabled
                    //if (!(value is string) && value is IEnumerable)
                    //{
                    //    var enumValues = (value as IEnumerable).Cast<object>();
                    //    Core.Logger.Raw("{0}{1}: {{ {2} }}", GetIndent(level, extraDepth), memberInfo.Name, string.Join(", ", string.Join(", ", enumValues)));
                    //}
                    //else
                    //{
                    LogValue(level, extraDepth, memberInfo.Name, value);
                    //}
                }
                else
                {
                    var beginning = "{";
                    var end = "}";
                    if (value is IEnumerable)
                    {
                        beginning = "[";
                        end = "]";
                    }
                    Core.Logger.Raw("{0}{1}: {2}", GetIndent(level, extraDepth), memberInfo.Name, beginning);
                    WriteObject(value, level + 1, extraDepth);
                    Core.Logger.Raw("{0}{1}", GetIndent(level, extraDepth), end);
                }
            }
        }

        private static void LogValue(int level, int extraDepth, object value)
        {
            if (value is string)
            {
                Core.Logger.Raw("{0}\"{1},\"", GetIndent(level, extraDepth), RemoveNewLines(value.ToString()));
            }
            else
            {
                Core.Logger.Raw("{0}{1},", GetIndent(level, extraDepth), RemoveNewLines(value.ToString()));
            }
        }

        private static void LogValue(int level, int extraDepth, string name, object value)
        {
            if (value is string)
            {
                Core.Logger.Raw("{0}{1}: \"{2}\"", GetIndent(level, extraDepth), name, RemoveNewLines(value.ToString()));
            }
            else
            {
                Core.Logger.Raw("{0}{1}: {2}", GetIndent(level, extraDepth), name, RemoveNewLines(value.ToString()));
            }
        }

        private static string RemoveNewLines(string value)
        {
            const string replaceWith = " ";
            return value.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
        }

        private static string GetIndent(int level, int extraDepth)
        {
            return new string(' ', (level + extraDepth) * 4);
        }
    }
}