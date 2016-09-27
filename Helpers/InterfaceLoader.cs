using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Trinity.Technicals;

namespace Trinity.Helpers
{
    namespace AutoFollow.Resources
    {
        public class InterfaceLoader<T> where T : class
        {
            public Dictionary<string, T> Items { get; private set; } = new Dictionary<string, T>();

            public void Load()
            {
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
                        Logger.LogVerbose("Instantiated {0}", taskType.Name);
                        Items[taskType.Name] = instance;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Exception creating instance {0}", ex);
                    }
                }             
            }
        }
    }

}

