using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Buddy.Coroutines;
using Trinity.Combat.Abilities;
using Trinity.Config;
using Trinity.Coroutines;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects.Enums;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Movement;
using Trinity.Settings.Loot;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;
using Logger = Trinity.Technicals.Logger;

namespace Trinity
{
    public class BotManager
    {
        public BotManager()
        {
        }

        private static TrinitySetting Settings { get { return TrinityPlugin.Settings; } }

        private static readonly Dictionary<string, Composite> OriginalHooks = new Dictionary<string, Composite>();

        private static Composite _goldInactiveComposite;
        private static Composite _xpInactiveComposite;

        /// <summary>
        /// This will replace the main BehaviorTree hooks for Combat, Vendoring, and Looting.
        /// </summary>
        internal static void ReplaceTreeHooks()
        {
            if (TrinityPlugin.IsPluginEnabled)
            {
                ReplaceCombatHook();
                ReplaceVendorRunHook();
                ReplaceLootHook();
                ReplaceDeathHook();
                InsertOutOfGameHooks();

                //ReplaceIdentifyHook();
                //ReplaceSellItemsHook();
                //ReplaceStashHook();
                //ReplaceSalvageItemsHook();

                //Pulsator.OnPulse += (s, arg) => Logger.Log("Pulse!");
                //TreeHooks.Instance.InsertHook("TreeStart", 0, new Action(ret => { Logger.Log("TreeStart!"); return RunStatus.Failure; }));
                //TreeHooks.Instance.InsertHook("BotBehavior", 0, new Action(ret => { Logger.Log("BotBehavior!"); return RunStatus.Failure; }));
                //TreeHooks.Instance.InsertHook("VendorRun", 0, new Action(ret => { Logger.Log("VendorRun!"); return RunStatus.Failure; }));
            }
            else
            {
                ReplaceHookWithOriginal("Combat");
                ReplaceHookWithOriginal("VendorRun");
                ReplaceHookWithOriginal("Loot");
                ReplaceHookWithOriginal("Death");
                ReplaceHookWithOriginal("IdentifyItems");

                Logger.Log("Removing GoldInactivity from BotBehavior");
                TreeHooks.Instance.RemoveHook("BotBehavior", _goldInactiveComposite);
            }
        }

        private static void ReplaceCombatHook()
        {
            if (!TreeHooks.Instance.Hooks.ContainsKey("Combat"))
                return;
            // This is the do-all-be-all god-head all encompasing piece of trinity
            StoreAndReplaceHook("Combat", new ActionRunCoroutine(ret => MainCombatTask()));
        }

        private static async Task<bool> MainCombatTask()
        {
            // If we aren't in the game or a world is loading, don't do anything yet
            if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || !ZetaDia.Me.IsValid)
            {
                Logger.LogDebug(LogCategory.GlobalHandler, "Not in game, IsLoadingWorld, or Me is Invalid");
                return true;
            }

            if (!await Overrides.Execute())
                return false;

            var equippedItems = ZetaDia.Me.Inventory.Equipped.Where(i => i.IsValid);
            bool needEmergencyRepair = false;
            //ZetaDia.Me.Inventory.Equipped.Where(i => i.AcdId != 0 && i.IsValid).Average(i => i.DurabilityPercent) < 0.05
            foreach (var item in equippedItems)
            {
                if (item.ACDId == 0) continue;
                float durabilitySum = 0f;
                int itemCount = 0;
                try
                {
                    durabilitySum += item.DurabilityPercent;
                    itemCount++;
                }
                catch
                {
                    // Ring them bells for the chosen few who will judge the many when the game is through
                }
                if (itemCount > 0 && durabilitySum / itemCount < 0.05)
                    needEmergencyRepair = true;
            }

            // We keep dying because we're spawning in AoE and next to 50 elites and we need to just leave the game
            if (DateTime.UtcNow.Subtract(TrinityPlugin.LastDeathTime).TotalSeconds < 30 && !ZetaDia.IsInTown && needEmergencyRepair && !Settings.Advanced.UseTrinityDeathHandler)
            {
                Logger.Log("Durability is zero, emergency leave game");
                ZetaDia.Service.Party.LeaveGame(true);
                await CommonCoroutines.LeaveGame("Durability is zero");
                Logger.LogDebug(LogCategory.GlobalHandler, "Recently died, durability zero");
                return true;
            }
            
            await AutoEquipSkills.Instance.Execute();
            await AutoEquipItems.Instance.Execute();


            var itemMarker = Core.Markers.CurrentWorldMarkers.FirstOrDefault(m => m.MarkerType == WorldMarkerType.SetItem || m.MarkerType == WorldMarkerType.LegendaryItem && !_visitedItemMarkers.Contains(m.Position));
            if (itemMarker != null && (!CombatBase.IsInCombat && itemMarker.Distance < 500f) && !Navigator.StuckHandler.IsStuck)
            {
                if (itemMarker.Distance < 10f)
                {
                    Logger.Warn($"Arrived at Item Marker: {itemMarker.Position}! Distance {itemMarker.Distance}!");
                    _visitedItemMarkers.Add(itemMarker.Position);
                }
                else
                {
                    Logger.Warn($"Moving to Item Marker at {itemMarker.Position}! Distance {itemMarker.Distance}!");
                    await CommonCoroutines.MoveTo(itemMarker.Position, "ItemMarker"); 
                    TrinityPlugin.Player.CurrentAction = PlayerAction.Moving;
                    return true;
                }
            }

            var isTarget = TrinityPlugin.TargetCheck(null);

            if (CombatBase.CombatMovement.IsQueuedMovement & CombatBase.IsCombatAllowed)
            {
                while (CombatBase.CombatMovement.Execute() == RunStatus.Running)
                {
                    if (!CombatTargeting.Instance.AllowedToKillMonsters)
                        return false;

                    await Coroutine.Yield();
                }
                return true;
            }

            if (!CombatTargeting.Instance.AllowedToKillMonsters && (TrinityPlugin.CurrentTarget == null || TrinityPlugin.CurrentTarget.IsUnit) && CombatBase.CombatMode != CombatMode.KillAll)
            {
                Logger.LogVerbose(LogCategory.Behavior, "Aborting MainCombatTask() AllowCombat={0}", CombatTargeting.Instance.AllowedToKillMonsters);
                return false;
            }

            if (isTarget)
            {
                return await new Action(ret => TrinityPlugin.HandleTarget()).ExecuteCoroutine();
            }

            return false;
            //return await new Decorator(TrinityPlugin.TargetCheck, new Action(ret => TrinityPlugin.HandleTarget())).ExecuteCoroutine();
        }

        public static HashSet<Vector3> _visitedItemMarkers { get; set; } = new HashSet<Vector3>();

        private static void ReplaceVendorRunHook()
        {
            if (!TreeHooks.Instance.Hooks.ContainsKey("VendorRun"))
                return;
            //// We still want the main VendorRun logic, we're just going to take control of *when* this logic kicks in
            //var vendorDecorator = TreeHooks.Instance.Hooks["VendorRun"][0] as Decorator;
            //if (vendorDecorator != null)
            //{
            //    StoreAndReplaceHook("VendorRun", new ActionRunCoroutine(ret => TownRunSelector(vendorDecorator)));
            //}

            StoreAndReplaceHook("VendorRun", new ActionRunCoroutine(ret => TrinityTownRun.Execute()));
        }

        //private static async Task<bool> TownRunSelector(Decorator originalTownRun)
        //{
        //    if (Settings.Advanced.UseExperimentalTownRun)
        //    {
        //        return await TrinityTownRun.Execute();
        //    }

        //    if (TownRun.TownRunCanRun())
        //    {
        //        return await TownRun.TownRunCoroutineWrapper(originalTownRun);
        //    }

        //    return false;
        //}

        private static void ReplaceIdentifyHook()
        {
            if (!TreeHooks.Instance.Hooks.ContainsKey("IdentifyItems"))
                return;

            StoreAndReplaceHook("IdentifyItems", new ActionRunCoroutine(ret => IdentifyItems.Execute()));
        }

        private static void ReplaceSellItemsHook()
        {
            if (!TreeHooks.Instance.Hooks.ContainsKey("SellAndRepair"))
                return;

            StoreAndReplaceHook("SellAndRepair", new ActionRunCoroutine(ret => SellItems.Execute()));
        }

        private static void ReplaceSalvageItemsHook()
        {
            if (!TreeHooks.Instance.Hooks.ContainsKey("SalvageItems"))
                return;

            StoreAndReplaceHook("SalvageItems", new ActionRunCoroutine(ret => SalvageItems.Execute()));
        }

        private static void ReplaceStashHook()
        {
            if (!TreeHooks.Instance.Hooks.ContainsKey("StashItems"))
                return;

            StoreAndReplaceHook("StashItems", new ActionRunCoroutine(ret => StashItems.Execute()));
        }

        private static void ReplaceLootHook()
        {
            if (!TreeHooks.Instance.Hooks.ContainsKey("Loot"))
                return;
            // Loot tree is now empty and never runs (Loot is handled through combat)
            // This is for special out of combat handling like Horadric Cache
            Composite lootComposite = TreeHooks.Instance.Hooks["Loot"][0];
            StoreAndReplaceHook("Loot", Composites.CreateLootBehavior(lootComposite));
        }

        private static void ReplaceDeathHook()
        {
            if (!TreeHooks.Instance.Hooks.ContainsKey("Death"))
                return;

            //if (TrinityPlugin.Settings.Advanced.UseTrinityDeathHandler)
            //{
            StoreAndReplaceHook("Death", new ActionRunCoroutine(ret => DeathHandler.Execute()));
            //}
            //else
            //{
            //    ReplaceHookWithOriginal("Death");
            //}
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
            ReplaceTreeHooks();
        }

        private static void StoreAndReplaceHook(string hookName, Composite behavior)
        {
            if (!OriginalHooks.ContainsKey(hookName))
                OriginalHooks.Add(hookName, TreeHooks.Instance.Hooks[hookName][0]);

            Logger.Log("Replacing " + hookName + " Hook");
            TreeHooks.Instance.ReplaceHook(hookName, behavior);
        }

        private static bool UpdateOriginalHook(string hookName, Action<Composite> updater)
        {
            if (!OriginalHooks.ContainsKey(hookName))
                return false;

            updater(OriginalHooks[hookName]);
            return true;
        }


        private static void ReplaceHookWithOriginal(string hook)
        {
            if (OriginalHooks.ContainsKey(hook))
            {
                Logger.Log("Replacing " + hook + " Hook with Original");
                TreeHooks.Instance.ReplaceHook(hook, OriginalHooks[hook]);
            }
        }


        internal static void SetBotTicksPerSecond()
        {
            if (Settings.Advanced.TPSEnabled)
            {
                BotMain.TicksPerSecond = Settings.Advanced.TPSLimit;
                //ActorManager.TickDelayMs = Settings.Advanced.TPSLimit < 0 ? 0 : 1000 / Settings.Advanced.TPSLimit;
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "Bot TPS set to {0}", Settings.Advanced.TPSLimit);
            }
            else
            {
                BotMain.TicksPerSecond = 30;
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "Reset bot TPS to default: {0}", 30);
            }
        }

        internal static void Exit()
        {
            ZetaDia.Memory.Process.Kill();

            try
            {
                if (Thread.CurrentThread != Application.Current.Dispatcher.Thread)
                {
                    Application.Current.Dispatcher.Invoke(new System.Action(Exit));
                    return;
                }

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }
    }
}
