//using System;
//using System.Linq;
//using Trinity.Components.Combat.Abilities;
//using Zeta.Bot.Navigation;
//using Zeta.Common;
//using Zeta.Common.Plugins;
//using Zeta.Game.Internals;
//using Logger = Trinity.Technicals.Logger;

//namespace Trinity.Helpers
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
//            Me = TrinityPlugin.Instance;
//        }

//        public static void SetCombatMode(CombatMode combatMode)
//        {
//            var result = Me.SendToAll("COMBATMODE", combatMode.ToString());
//            foreach (var pluginCommunicationResponse in result)
//            {
//                Logger.LogDebug("[PluginCommunicator][" + pluginCommunicationResponse.Plugin.Name + "] Response: " + pluginCommunicationResponse.Result);
//            }
//        }

//        public static PluginCommunicationResponse Receive(IPlugin sender, string command, params object[] args)
//        {
//            Logger.LogVerbose("Command '{0}' Received from: {1}", command, sender.Name);

//            switch (command)
//            {
//                case "PING":
//                    return Respond("PONG");

//                case "MOVETO":
//                    var destination = (Vector3) args[0];
//                    var moveResult = Navigator.NavigationProvider.MoveTo(destination);
//                    return Respond(moveResult);

//                case "CLEARAREA":
//                    int numSecs;
//                    TryGetNumber(args, out numSecs);
//                    if (numSecs < 10)
//                        numSecs = 10;

//                    Logger.Log("Setting Combat Mode to {0} at request of {0}", sender.Name);
//                    CombatBase.CombatOverrides.ModifyTrashSizeForDuration(-100, TimeSpan.FromSeconds(numSecs), 1, 1, () =>
//                    {
//                        return DateTime.UtcNow.Subtract(TrinityPlugin.LastWorldChangeTime).TotalSeconds < 15;
//                    });

//                    return Respond(true);

//                case "COMBATMODE":
//                    CombatMode mode;
//                    if (TryGetEnum<CombatMode>(args[0], out mode))
//                    {
//                        Logger.Log("Setting Combat Mode to {0} at request of {0}", sender.Name);
//                        CombatBase.CombatMode = mode;
//                        return Respond(true);
//                    }
//                    return Respond(false);

//                case "RESETINACTIVITY":
//                    Logger.Log("Resetting Gold/XP Inactivity at request of {0}", sender.Name);
//                    GoldInactivity.Instance.ResetCheckGold();
//                    XpInactivity.Instance.ResetCheckXp();
//                    return Respond(true);                   
//            }
//            return Respond(PluginCommunicationResult.InvalidCommand);
//        }

//        public static PluginCommunicationResponse Respond(object response, PluginCommunicationResult resultType = PluginCommunicationResult.Done)
//        {
//            return new PluginCommunicationResponse(Me, resultType.ToString(), response);
//        }

//        public static bool TryGetNumber<T>(object obj, out T number)
//        {
//            double num;
//            if (!double.TryParse(obj.ToString(), out num))
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
