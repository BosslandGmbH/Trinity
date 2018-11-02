using log4net;
using System;
using System.Collections.Generic;
using Trinity.Components.Combat;
using Trinity.Components.Coroutines.Town;
using Trinity.DbProvider;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Common;
using Zeta.TreeSharp;

namespace Trinity.Framework
{
    public class HookManager
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();
        private static readonly Dictionary<string, Composite> s_originalHooks = new Dictionary<string, Composite>();
        private static bool _hooksAttached;

        static HookManager()
        {
            TreeHooks.Instance.OnHooksCleared += InstanceOnOnHooksCleared;
        }

        public static void CheckHooks()
        {
            if (!_hooksAttached)
            {
                ReplaceTreeHooks();
            }
        }

        private static void ReplaceTreeHooks()
        {
            if (TrinityPlugin.IsEnabled)
            {
                ReplaceCombatHook();
                ReplaceVendorRunHook();
                ReplaceDeathHook();
                InsertOutOfGameHook();
                _hooksAttached = true;
            }
            else
            {
                ReplaceHookWithOriginal("Combat");
                ReplaceHookWithOriginal("VendorRun");
                ReplaceHookWithOriginal("Death");
                _hooksAttached = false;
            }
        }

        public static void InsertOutOfGameHook()
        {
            TreeHooks.Instance.InsertHook("OutOfGame", 0, new Zeta.TreeSharp.Action(ret =>
            {
                ModuleManager.OutOfGamePulse();
                return RunStatus.Failure;
            }));
        }

        private static void ReplaceCombatHook()
        {
            StoreAndReplaceHook("Combat", new ActionRunCoroutine(ret =>
                TrinityCombat.MainCombatTask()));
        }

        private static void ReplaceVendorRunHook()
        {
            StoreAndReplaceHook("VendorRun", new ActionRunCoroutine(async ret =>
                await TrinityTownRun.DoTownRun() == CoroutineResult.Running));
        }

        private static void ReplaceDeathHook()
        {
            StoreAndReplaceHook("Death", new ActionRunCoroutine(ret =>
                DeathHandler.Execute()));
        }

        private static void InstanceOnOnHooksCleared(object sender, EventArgs eventArgs)
        {
            _hooksAttached = false;
            CheckHooks();
        }

        private static void StoreAndReplaceHook(string hookName, Composite behavior)
        {
            if (!s_originalHooks.ContainsKey(hookName) &&
                TreeHooks.Instance.Hooks.ContainsKey(hookName))
            {
                s_originalHooks.Add(hookName, TreeHooks.Instance.Hooks[hookName][0]);
            }

            s_logger.Info($"Replacing {hookName} Hook");
            TreeHooks.Instance.ReplaceHook(hookName, behavior);
        }

        private static void ReplaceHookWithOriginal(string hook)
        {
            if (!s_originalHooks.ContainsKey(hook) ||
                !TreeHooks.Instance.Hooks.ContainsKey(hook))
            {
                return;
            }
            s_logger.Info($"Replacing {hook} Hook with Original");
            TreeHooks.Instance.ReplaceHook(hook, s_originalHooks[hook]);
        }
    }
}
