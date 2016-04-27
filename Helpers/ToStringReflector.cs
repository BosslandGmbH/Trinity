using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Trinity.Helpers
{
    public class ToStringReflector
    {
        public static string GetObjectString(object obj)
        {
            string output = "";
            Type t = obj.GetType();
            List<PropertyInfo> properties;
            foreach (var property in t.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                output += property.Name + "=" + t.GetProperty(property.Name).GetValue(obj, null) + " ";
            }
            foreach (var field in t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                output += field.Name + "=" + t.GetField(field.Name).GetValue(obj) + " ";
            }
            return output;
        }
    }
}
