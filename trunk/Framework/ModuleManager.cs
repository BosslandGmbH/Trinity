using System;
using System.Collections.Generic;
using System.Linq;
using IronPython.Modules;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;

namespace Trinity.Framework
{
    public class ModuleManager
    {
        public static List<WeakReference<Module>> Instances = new List<WeakReference<Module>>();            

        public static void EnableAll()
        {
            ExecuteOnInstances(util => util.IsEnabled = true);
        }

        public static void DisableAll()
        {
            ExecuteOnInstances(util => util.IsEnabled = false);
        }

        public static void FireEvent(ModuleEvent moduleEvent)
        {
            ExecuteOnInstances(util => util.FireEvent(moduleEvent));
        }

        private static void ExecuteOnInstances(Action<Module> action)
        {
            foreach (var utilReference in Instances.ToList())
            {
                Module util;
                if (utilReference.TryGetTarget(out util))
                {
                    action?.Invoke(util);
                }
                else
                {
                    Instances.Remove(utilReference);
                }
            }
        }

        private static IEnumerable<Module> GetModules()
        {
            foreach (var utilReference in Instances.ToList())
            {
                Module util;
                if (utilReference.TryGetTarget(out util))
                {
                    yield return util;
                }
                else
                {
                    Instances.Remove(utilReference);
                }
            }
        }

        public static IEnumerable<IDynamicSetting> DynamicSettings => GetModules().OfType<IDynamicSetting>();        
    }
}