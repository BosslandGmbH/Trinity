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
            public Dictionary<string, T> Items { get; } = new Dictionary<string, T>();

            public void Load()
            {
                var sw = Stopwatch.StartNew();
                var configType = typeof(T);
                var configTypeFullName = configType.FullName;
                var implementers = configType.Assembly.GetTypes()
                    .Where(p => !p.IsInterface && !p.IsAbstract && p.GetInterface(configTypeFullName) != null).ToList();

                foreach (var item in implementers)
                {
                    try
                    {
                        var instance = (T)Activator.CreateInstance(item);
                        if (instance == null) continue;
                        Core.Logger.Verbose("初始化 {0}", item.Name);
                        Items[item.Name] = instance;
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Error("异常创建实例 {0}", ex);
                    }
                }
                sw.Stop();
                Core.Logger.Verbose($"配置读取完成, {configType.Name} 于 {sw.Elapsed:g}");
            }
        }
    }
}