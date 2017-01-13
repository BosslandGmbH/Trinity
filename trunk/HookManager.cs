using System;
using System.Collections.Generic;
using Trinity.Components.Combat;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework.Helpers;
using Zeta.Bot;
using Zeta.Common;
using Zeta.TreeSharp;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity
{
    public class HookManager
    {
        private static readonly Dictionary<string, Composite> OriginalHooks = new Dictionary<string, Composite>();
        private static Composite _goldInactiveComposite;
        private static Composite _xpInactiveComposite;

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
                Logger.Log("Adding Trinity's Hooks");
                ReplaceCombatHook();
                ReplaceVendorRunHook();
                ReplaceDeathHook();
                InsertOutOfGameHooks();
                HooksAttached = true;
            }
            else
            {
                Logger.Log("Removing Trinity's Hooks");
                ReplaceHookWithOriginal("Combat");
                ReplaceHookWithOriginal("VendorRun");
                ReplaceHookWithOriginal("Death");

                if(TreeHooks.Instance.Hooks.ContainsKey("BotBehavior") && _goldInactiveComposite != null)
                    TreeHooks.Instance.RemoveHook("BotBehavior", _goldInactiveComposite);

                HooksAttached = false;
            }
        }

        public static bool HooksAttached { get; set; }

        private static void ReplaceCombatHook()
        {
            StoreAndReplaceHook("Combat", new ActionRunCoroutine(ret => Combat.MainCombatTask()));
        }

        private static void ReplaceVendorRunHook()
        {
            StoreAndReplaceHook("VendorRun", new ActionRunCoroutine(ret => TrinityTownRun.Execute()));
        }

        private static void ReplaceDeathHook()
        {
            StoreAndReplaceHook("Death", new ActionRunCoroutine(ret => DeathHandler.Execute()));
        }

        private static void InsertOutOfGameHooks()
        {
            const string hookName = "TreeStart";

            if (_goldInactiveComposite == null)
                _goldInactiveComposite = GoldInactivity.CreateGoldInactiveLeaveGame();

            if (_xpInactiveComposite == null)
                _xpInactiveComposite = XpInactivity.CreateXpInactiveLeaveGame();

            Logger.Log("Inserting GoldInactivity into " + hookName);
            TreeHooks.Instance.InsertHook(hookName, 0, _goldInactiveComposite);

            Logger.Log("Inserting XPInactivity into " + hookName);
            TreeHooks.Instance.InsertHook(hookName, 0, _xpInactiveComposite);
        }

        internal static void InstanceOnOnHooksCleared(object sender, EventArgs eventArgs)
        {
            HooksAttached = false;
            ReplaceTreeHooks();
        }

        private static void StoreAndReplaceHook(string hookName, Composite behavior)
        {
            if (!TreeHooks.Instance.Hooks.ContainsKey(hookName))
            {
                //Logger.LogVerbose($"Hook '{hookName}' doesnt exist in TreeHooks.");
                return;
            }

            if (!OriginalHooks.ContainsKey(hookName))
                OriginalHooks.Add(hookName, TreeHooks.Instance.Hooks[hookName][0]);

            Logger.Log("Replacing " + hookName + " Hook");
            TreeHooks.Instance.ReplaceHook(hookName, behavior);
        }

        private static void ReplaceHookWithOriginal(string hook)
        {
            if (OriginalHooks.ContainsKey(hook) && TreeHooks.Instance.Hooks.ContainsKey(hook))
            {
                Logger.Log("Replacing " + hook + " Hook with Original");
                TreeHooks.Instance.ReplaceHook(hook, OriginalHooks[hook]);
            }
        }

    }
}