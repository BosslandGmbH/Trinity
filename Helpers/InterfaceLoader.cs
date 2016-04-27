using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Helpers
{
    #region

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    #endregion

    namespace AutoFollow.Resources
    {
        public class InterfaceLoader<T> where T : class
        {
            public readonly List<Task> TaskWorkers = new List<Task>();
            public ConcurrentDictionary<string, T> Items = new ConcurrentDictionary<string, T>();
            public bool LoadStarted;

            public InterfaceLoader()
            {
                Load();
            }

            private bool _loadCompleted;
            public bool LoadCompleted
            {
                get { return _loadCompleted || (_loadCompleted = TaskWorkers.All(t => t.IsCompleted)); }
            }

            public void Load()
            {
                if (LoadStarted)
                    return;

                LoadStarted = true;

                var configType = typeof(T);
                var configTypeFullName = configType.FullName;
                var configurables = configType.Assembly.GetTypes().Where(p =>
                {
                    if (p.IsInterface || p.IsAbstract)
                        return false;

                    if (p.GetInterface(configTypeFullName) == null)
                        return false;

                    return true;
                });

                foreach (var taskType in configurables)
                {
                    TaskWorkers.Add(Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var instanceProperty = taskType.GetProperty("Instance");
                            var typeInstance = instanceProperty != null ? instanceProperty.GetValue(null) : Activator.CreateInstance(taskType);

                            if (typeInstance != null)
                            {
                                var instance = typeInstance as T;
                                Logger.LogVerbose("Instantiated {0}", taskType.Name);
                                Items.AddOrUpdate(taskType.Name, str => instance, (keys, existing) => instance);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Exception creating instance {0}", ex);
                        }
                    }));
                }
            }
        }
    }

}

