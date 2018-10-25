using System;
using System.Collections.Generic;
using Trinity.Components.Combat;
using Trinity.Components.Coroutines.Town;
using Trinity.DbProvider;
using Zeta.Bot;
using Zeta.Common;
using Zeta.TreeSharp;


namespace Trinity.Framework
{
    public class HookManager
    {
        static HookManager()
        {
            TreeHooks.Instance.OnHooksCleared += InstanceOnOnHooksCleared;
        }

        private static readonly Dictionary<string, Composite> OriginalHooks = new Dictionary<string, Composite>();

        public static void CheckHooks()
        {
            if (!HooksAttached)
            {
                ReplaceTreeHooks();
            }
        }

        internal static void ReplaceTreeHooks()
        {
            if (TrinityPlugin.IsEnabled)
            {
                ReplaceCombatHook();
                ReplaceVendorRunHook();
                ReplaceDeathHook();
                InsertOutOfGameHook();
                HooksAttached = true;
            }
            else
            {
                ReplaceHookWithOriginal("Combat");
                ReplaceHookWithOriginal("VendorRun");
                ReplaceHookWithOriginal("Death");
                HooksAttached = false;
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

        public static bool HooksAttached { get; set; }

        private static void ReplaceCombatHook()
        {
            StoreAndReplaceHook("Combat", new ActionRunCoroutine(ret => TrinityCombat.MainCombatTask()));
        }

        private static void ReplaceVendorRunHook()
        {
            StoreAndReplaceHook("VendorRun", new ActionRunCoroutine(ret => TrinityTownRun.Execute()));
        }

        private static void ReplaceDeathHook()
        {
            StoreAndReplaceHook("Death", new ActionRunCoroutine(ret => DeathHandler.Execute()));
        }

        internal static void InstanceOnOnHooksCleared(object sender, EventArgs eventArgs)
        {
            HooksAttached = false;
            ReplaceTreeHooks();
        }

        private static void StoreAndReplaceHook(string hookName, Composite behavior)
        {
            if (!OriginalHooks.ContainsKey(hookName) && TreeHooks.Instance.Hooks.ContainsKey(hookName))
                OriginalHooks.Add(hookName, TreeHooks.Instance.Hooks[hookName][0]);

            Core.Logger.Log("Replacing " + hookName + " Hook");
            TreeHooks.Instance.ReplaceHook(hookName, behavior);
        }

        private static void ReplaceHookWithOriginal(string hook)
        {
            if (OriginalHooks.ContainsKey(hook) && TreeHooks.Instance.Hooks.ContainsKey(hook))
            {
                Core.Logger.Log("Replacing " + hook + " Hook with Original");
                TreeHooks.Instance.ReplaceHook(hook, OriginalHooks[hook]);
            }
        }
    }
}