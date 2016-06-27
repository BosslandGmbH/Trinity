//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Buddy.Coroutines;
//using Trinity.Coroutines;
//using Trinity.Coroutines.Town;
//using TrinityCoroutines;
//using TrinityCoroutines.Resources;
//using Trinity.Helpers;
//using Zeta.Bot;
//using Zeta.Bot.Coroutines;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals;
//using Zeta.TreeSharp;
//using Logger = Trinity.Technicals.Logger;

//namespace Trinity.Items
//{
//    public class CleanStash
//    {
//        private const int ItemMovementDelay = 100;
//        const string HookName = "VendorRun";

//        private static Composite _cleanBehavior;
//        private static bool _hookInserted;
//        public static bool IsCleaning { get; private set; }

//        public static void RunCleanStash()
//        {
//            if (!BotMain.IsRunning)
//            {
//                TaskDispatcher.Start(ret => CleanTask(), ret => !IsCleaning);
//                return;
//            }

//            try
//            {
//                GoldInactivity.Instance.ResetCheckGold();
//                XpInactivity.Instance.ResetCheckXp();

//                if (!_hookInserted)
//                {
//                    _cleanBehavior = CreateCleanBehavior();
//                    TreeHooks.Instance.InsertHook(HookName, 0, _cleanBehavior);
//                    _hookInserted = true;
//                    BotMain.OnStop += bot => RemoveBehavior("Bot stopped");
//                }
//            }
//            catch (Exception ex)
//            {
//                Logger.LogError("Error running clean stash: " + ex);
//                RemoveBehavior("Exception");
//            }

//        }

//        private static void RemoveBehavior(string reason)
//        {
//            IsCleaning = false;

//            if (_cleanBehavior != null)
//            {
//                try
//                {
//                    if (_hookInserted)
//                    {
//                        Logger.LogDebug("Removing CleanStash Hook: " + reason);
//                        TreeHooks.Instance.RemoveHook(HookName, _cleanBehavior);
//                        _hookInserted = false;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Logger.LogDebug("Sort behavior not inserted? " + ex);
//                }
//            }
//        }

//        public static Composite CreateCleanBehavior()
//        {
//            return new ActionRunCoroutine(ret => CleanTask());
//        }

//        private static bool _isFinished = false;
//        public static async Task<bool> CleanTask()
//        {
//            IsCleaning = true;
//            try
//            {
//                if (!ZetaDia.IsInGame)
//                    return false;
//                if (ZetaDia.IsLoadingWorld)
//                    return false;
//                if (!ZetaDia.Me.IsFullyValid())
//                    return false;

//                if (ZetaDia.Me.IsParticipatingInTieredLootRun)
//                {
//                    Logger.LogNormal("Cannot clean stash while in trial/greater rift");
//                    RemoveBehavior("Cannot clean stash while in trial/greater rift");
//                    return false;
//                }

//                if (TrinityItemManager.FindValidBackpackLocation(true) == new Vector2(-1, -1))
//                {
//                    TrinityPlugin.ForceVendorRunASAP = true;
//                    return false;
//                }
//                if (!await ReturnToStash.Execute())
//                {
//                    _isFinished = true;
//                    return false;
//                }
//                if (GameUI.IsElementVisible(GameUI.StashDialogMainPage))
//                {
//                    Logger.Log("Cleaning stash...");

//                    foreach (var item in ZetaDia.Me.Inventory.StashItems.Where(i => i.AcdId != 0 && i.IsValid).ToList())
//                    {
//                        CachedACDItem cItem = CachedACDItem.GetTrinityItem(item);
//                        // Don't take potions from the stash
//                        if (cItem.TrinityItemType == TrinityItemType.HealthPotion)
//                            continue;

//                        try
//                        {
//                            if (!ItemManager.Current.ShouldStashItem(item))
//                            {
//                                Logger.Log("Removing {0} from stash", item.Name);
//                                ZetaDia.Me.Inventory.QuickWithdraw(item);
//                                await Coroutine.Sleep(ItemMovementDelay);
//                                await Coroutine.Yield();

//                                if (TrinityItemManager.FindValidBackpackLocation(true) == new Vector2(-1, -1))
//                                {
//                                    TrinityPlugin.ForceVendorRunASAP = true;
//                                    return false;
//                                }
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Logger.LogError(ex.ToString());
//                        }
//                    }

//                    _isFinished = true;
//                    TrinityPlugin.ForceVendorRunASAP = true;
//                    Logger.Log("Waiting 5 seconds...");
//                    BotMain.StatusText = "Waiting 5 seconds...";
//                    await Coroutine.Sleep(5000);

//                    if (ReturnToStash.StartedOutOfTown && ZetaDia.IsInTown)
//                        await CommonBehaviors.TakeTownPortalBack().ExecuteCoroutine();
//                }
//                if (_isFinished)
//                    RemoveBehavior("finished!");
//                return true;

//            }
//            catch (Exception ex)
//            {
//                _isFinished = true;
//                Logger.LogError(ex.ToString());
//                return false;
//            }
//        }
//    }
//}
