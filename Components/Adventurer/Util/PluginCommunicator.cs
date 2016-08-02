//using System;

//using Trinity.Components.Adventurer.Coroutines;
//using Trinity.Components.Adventurer.Coroutines.RiftCoroutines;
//using Zeta.Common.Plugins;

//namespace Trinity.Components.Adventurer.Util
//{
//    public enum CombatMode
//    {
//        On,
//        Off,
//        KillAll,
//        SafeZerg,
//    }

//    public class PluginCommunicator
//    {
//        private static readonly ICommunicationEnabledPlugin Me;

//        static PluginCommunicator()
//        {
//            Me = Adventurer.CurrentInstance;
//            CoroutineQueue.Enable();
//        }

//        public static void SetCombatMode(CombatMode combatMode)
//        {
//            var result = Me.SendToAll("COMBATMODE", combatMode.ToString());
//            foreach (var pluginCommunicationResponse in result)
//            {
//                Logger.Debug("[PluginCommunicator][" + pluginCommunicationResponse.Plugin.Name + "] Response: " + pluginCommunicationResponse.Result);
//            }
//        }

//        public static void RequestClearArea(int duration)
//        {
//            var result = Me.SendToAll("CLEARAREA", duration);
//            foreach (var pluginCommunicationResponse in result)
//            {
//                Logger.Debug("[PluginCommunicator][" + pluginCommunicationResponse.Plugin.Name + "] Response: " + pluginCommunicationResponse.Result);
//            }
//        }
        
//        public static void AddToBlacklist(int actorId)
//        {
//            var result = Me.SendToAll("ADDBLACKLIST", actorId);
//            foreach (var pluginCommunicationResponse in result)
//            {
//                Logger.Debug("[PluginCommunicator][" + pluginCommunicationResponse.Plugin.Name + "] Response: " + pluginCommunicationResponse.Result);
//            }
//        }

//        public static void RemoveFromBlacklist(int actorId)
//        {
//            var result = Me.SendToAll("REMOVEBLACKLIST", actorId);
//            foreach (var pluginCommunicationResponse in result)
//            {
//                Logger.Debug("[PluginCommunicator][" + pluginCommunicationResponse.Plugin.Name + "] Response: " + pluginCommunicationResponse.Result);
//            }
//        }

//        public static PluginCommunicationResponse Receive(IPlugin sender, string command, params object[] args)
//        {           
//            switch (command)
//            {
//                case "PING":
//                    return Respond("PONG");
//                case "UPGRADEGEMS":
//                    var riftOptions = 
//                    CoroutineQueue.Enqueue(GemUpgradeCoroutine);
//                    return Respond();

//            }
//            return Respond(PluginCommunicationResult.InvalidCommand);
//        }

//        private static readonly ICoroutine GemUpgradeCoroutine = new UpgradeGemsCoroutine();

//        public static PluginCommunicationResponse Respond(object response = null, PluginCommunicationResult resultType = PluginCommunicationResult.Done)
//        {
//            return new PluginCommunicationResponse(Me, resultType.ToString(), response);
//        }

//        public static bool TryGetNumber<T>(object obj, out T number)
//        {
//            double num;
//            if (!Double.TryParse(obj.ToString(), out num))
//            {
//                number = default(T);
//                return false;
//            }

//            number = (T)Convert.ChangeType(num, typeof(T));
//            return true;
//        }

//        public static bool TryGetEnum<T>(object obj, out T enumValue) where T : struct
//        {
//            if (!typeof(T).IsEnum)
//            {
//                enumValue = default(T);
//                return false;
//            }

//            if (Enum.TryParse(obj.ToString(), out enumValue))
//                return true;

//            enumValue = default(T);
//            return false;
//        }
//    }
//}
