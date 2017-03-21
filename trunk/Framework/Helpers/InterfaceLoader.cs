using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace Trinity.Framework.Helpers
{
    namespace AutoFollow.Resources
    {
        public class InterfaceLoader<T> where T : class
        {
            public Dictionary<string, T> Items { get; private set; } = new Dictionary<string, T>();

            public void Load()
            {
                var sw = Stopwatch.StartNew();
                var configType = typeof(T);
                var configTypeFullName = configType.FullName;
                var configurables = configType.Assembly.GetTypes().Where(p =>
                {
                    if (p.IsInterface || p.IsAbstract)
                        return false;

                    return p.GetInterface(configTypeFullName) != null;
                }).ToList();

                foreach (var taskType in configurables)
                {
                    try
                    {
                        var instance = (T)Activator.CreateInstance(taskType);
                        if (instance == null) continue;
                        Core.Logger.Verbose("Instantiated {0}", taskType.Name);
                        Items[taskType.Name] = instance;
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Error("Exception creating instance {0}", ex);
                    }
                }
                sw.Stop();
                Core.Logger.Verbose($"Finished Loading {configType.Name} in {sw.Elapsed:g}");
            }
        }
    }
}