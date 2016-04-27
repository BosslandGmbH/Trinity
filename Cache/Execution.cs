using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Cache
{
    public static class Execution
    {
        private static bool _isInitialized;
        private static Dictionary<string, ExecutionRecord> _trackedMethods = new Dictionary<string, ExecutionRecord>();

        /// <summary>
        /// Enables method execution to be aborted if a certain amount of time hasn't passed yet.
        /// Call "if (Execution.Restrict("MethodName")) return;" within method.
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        public class TrackMethodAttribute : Attribute { }

        /// <summary>
        /// Only call this if containing method has a TrackMethod Attribute
        /// </summary>
        /// <param name="key">name of the containing method</param>
        /// <param name="milliseconds">time in Milliseconds that must have passed.</param>
        /// <returns>true if execution should abort, and false if it should continue</returns>
        public static bool Restrict(string key, int milliseconds)
        {
            var executionRecord = _trackedMethods[key];
            var now = DateTime.UtcNow;

            if (now.Subtract(executionRecord.LastAccessTime).TotalMilliseconds < milliseconds)
                return true;

            executionRecord.LastAccessTime = now;
            return false;
        }

        public class ExecutionRecord
        {
            public DateTime LastAccessTime { get; set; }
            public int ExecutionLimitMs { get; set; }
            public string Key { get; set; }
            public Type ClassType { get; set; }
            public MethodInfo MethodInfo { get; set; }

            public ExecutionRecord(Type classType, MethodInfo methodInfo)
            {
                ClassType = classType;
                MethodInfo = methodInfo;
                LastAccessTime = DateTime.MinValue;
                Key = methodInfo.Name;
            }
        }

        public static void Initialize()
        {
            if (_isInitialized) return;

            var assembly = Assembly.GetExecutingAssembly();

            var methodsWithAttribute =
                    (from t in assembly.GetTypes().AsParallel()
                     from methodType in t.GetMethods(
                           BindingFlags.Public | BindingFlags.NonPublic |
                           BindingFlags.Instance | BindingFlags.Static)
                     let attributes = methodType.GetCustomAttributes(typeof(TrackMethodAttribute), true)
                     where attributes != null && attributes.Length > 0
                     select new { ClassType = t, MethodType = methodType, Attributes = attributes.Cast<TrackMethodAttribute>() }).ToList();

            _trackedMethods = methodsWithAttribute.ToDictionary(k => k.MethodType.Name, v => new ExecutionRecord(v.ClassType, v.MethodType));

            _isInitialized = true;
        }
    }
}
