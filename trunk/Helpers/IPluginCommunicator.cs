using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zeta.Common.Plugins;

namespace Trinity.Helpers
{
    public class PluginCommunicationResponse
    {
        public IPlugin Plugin { get; private set; }
        public string Result { get; private set; }
        public object ResponseData { get; set; }

        private PluginCommunicationResponse()
        {
        }

        public PluginCommunicationResponse(IPlugin responder, string result,
            object responseData = null)
        {
            Plugin = responder;
            Result = result;
            ResponseData = responseData;
        }
    }

    public enum PluginCommunicationResult
    {
        PluginNotExists,
        PluginReceiveError,
        InvalidCommand,
        InvalidArguments,
        NotAuthorized,
        Done
    }

    public interface ICommunicationEnabledPlugin : IPlugin
    {
        PluginCommunicationResponse Receive(IPlugin sender, string command, params object[] args);
    }

    public static class PluginCommunication
    {
        private static readonly string Method = "Reflection";

        public static PluginCommunicationResponse Send(this ICommunicationEnabledPlugin sender, string pluginName, string command, params object[] args)
        {
            IPlugin plugin;
            if (Method == "Reflection")
            {
                plugin = CommunicationEnabledPluginsViaReflection.FirstOrDefault(p => String.Equals(p.Name, pluginName, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                plugin = CommunicationEnabledPlugins.FirstOrDefault(p => String.Equals(p.Name, pluginName, StringComparison.InvariantCultureIgnoreCase));
            }
            if (plugin == null)
            {
                return PluginNotExistsResponse;
            }
            try
            {
                if (Method == "Reflection")
                {
                    var receiveMethod = GetReceiveMethod(plugin);
                    if (receiveMethod != null)
                    {
                        return receiveMethod.InvokeReceive(sender, plugin, command, args);
                    }
                    return new PluginCommunicationResponse(null, PluginCommunicationResult.PluginNotExists.ToString());
                }
                else
                {
                    return ((ICommunicationEnabledPlugin)plugin).Receive(sender, command, args);
                }
            }
            catch (Exception ex)
            {
                //Log something?
                return new PluginCommunicationResponse(plugin, PluginCommunicationResult.PluginReceiveError.ToString());
            }
        }

        public static List<PluginCommunicationResponse> SendToAll(this ICommunicationEnabledPlugin sender, string command, params object[] args)
        {
            var result = new List<PluginCommunicationResponse>();

            IEnumerable<IPlugin> plugins;
            if (Method == "Reflection")
            {
                plugins = CommunicationEnabledPluginsViaReflection;
            }
            else
            {
                plugins = CommunicationEnabledPlugins;
            }
            foreach (var plugin in plugins.Where(p => !String.Equals(p.Name, sender.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                try
                {
                    if (Method == "Reflection")
                    {
                        var receiveMethod = GetReceiveMethod(plugin);
                        if (receiveMethod != null)
                        {
                            result.Add(receiveMethod.InvokeReceive(sender, plugin, command, args));
                        }
                    }
                    else
                    {
                        result.Add(((ICommunicationEnabledPlugin)plugin).Receive(sender, command, args));
                    }
                }
                catch (Exception ex)
                {
                    //Log something?
                    result.Add(new PluginCommunicationResponse(plugin, PluginCommunicationResult.PluginReceiveError.ToString()));
                }
            }
            return result;
        }

        private static readonly PluginCommunicationResponse PluginNotExistsResponse = new PluginCommunicationResponse(null, PluginCommunicationResult.PluginNotExists.ToString());

        private static IEnumerable<ICommunicationEnabledPlugin> CommunicationEnabledPlugins
        {
            get
            {
                if (PluginManager.Plugins == null || PluginManager.Plugins.Count == 0)
                {
                    return new List<ICommunicationEnabledPlugin>();
                }
                return
                    PluginManager.Plugins
                    .Where(pluginContainer =>
                        pluginContainer.Plugin != null &&
                        pluginContainer.Enabled &&
                        pluginContainer.Plugin is ICommunicationEnabledPlugin)
                        .Select(pluginContainer => (ICommunicationEnabledPlugin)pluginContainer.Plugin);
            }
        }

        #region Using Reflection

        private static IEnumerable<IPlugin> CommunicationEnabledPluginsViaReflection
        {
            get
            {
                if (PluginManager.Plugins == null || PluginManager.Plugins.Count == 0)
                {
                    return new List<ICommunicationEnabledPlugin>();
                }
                var plugins =
                    PluginManager.Plugins
                        .Where(pluginContainer =>
                            pluginContainer.Plugin != null &&
                            pluginContainer.Plugin.GetType()
                                .GetInterfaces()
                                .Select(type => type.Name)
                                .Any(n => n == "ICommunicationEnabledPlugin")).Select(pluginContainer => pluginContainer.Plugin);
                return plugins;
            }
        }

        private static MethodInfo GetReceiveMethod(IPlugin plugin)
        {
            return plugin.GetType().GetMethod("Receive");
        }

        private static PluginCommunicationResponse InvokeReceive(this MethodInfo method, IPlugin sender, IPlugin plugin, string command, params object[] args)
        {
            var response = method.Invoke(plugin, new object[] { sender, command, args });
            var param1 = response.GetType().GetProperty("Result").GetValue(response);
            var param2 = response.GetType().GetProperty("ResponseData").GetValue(response);
            return new PluginCommunicationResponse(plugin, param1.ToString(), param2);
        }
        #endregion
    }
}
