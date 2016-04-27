using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Data;
using Buddy.Coroutines;
using Trinity.Combat.Abilities;
using Trinity.Coroutines.Town;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Helpers;
using Trinity.Items;
using Trinity.Notifications;
using Trinity.Reference;
using Trinity.Settings.Loot;
using Trinity.Technicals;
using TrinityCoroutines;
using TrinityCoroutines.Resources;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Common;
using Zeta.Bot.Profile.Composites;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.DbProvider
{
    internal class TownRun
    {
        // Whether salvage/sell run should go to a middle-waypoint first to help prevent stucks

        internal static bool LastTownRunCheckResult = false;
        // Random variables used during item handling and town-runs

        private static bool _loggedAnythingThisStash;

        private static bool _loggedJunkThisStash;
        internal static string ValueItemStatString = "";
        internal static string JunkItemStatString = "";
        internal static bool TestingBackpack = false;


        // DateTime check to prevent inventory-check spam when looking for repairs being needed
        internal static DateTime LastCheckBackpackDurability = DateTime.UtcNow;
        private static DateTime _LastCompletedTownRun = DateTime.MinValue;
        private static bool lastTownPortalCheckResult;
        private static DateTime lastTownPortalCheckTime = DateTime.MinValue;
        internal static Stopwatch randomTimer = new Stopwatch();
        internal static Random timerRandomizer = new Random();
        internal static int randomTimerVal = -1;
        internal static Stopwatch TownRunCheckTimer = new Stopwatch();

        static TownRun()
        {
            PreTownRunWorldId = -1;
            PreTownRunPosition = Vector3.Zero;
            WasVendoring = false;

            Pulsator.OnPulse += PulsatorOnOnPulse;
        }

        private static void PulsatorOnOnPulse(object sender, EventArgs eventArgs)
        {
        //    Line 862:     IdentifyWithCast = 226757,
	       // Line 989:     IdentifyWithCastLegendary = 259848,
	       // Line 1099:     IdentifyAllWithCast = 293981,

            if (!ZetaDia.IsInTown)
                return;

            var commonData = ZetaDia.Me.CommonData;

            if (Trinity.Player.CheckVisualEffectNoneForPower(commonData, SNOPower.IdentifyWithCast))
            {
                Logger.LogVerbose("Player is casting 'IdentifyWithCast'");
                return;
            }

            if (Trinity.Player.CheckVisualEffectNoneForPower(commonData, SNOPower.IdentifyWithCastLegendary))
            {
                Logger.LogVerbose("Player is casting 'IdentifyWithCast'");
                return;
            }

            if (Trinity.Player.CheckVisualEffectNoneForPower(commonData, SNOPower.IdentifyWithCastLegendary))
            {
                Logger.LogVerbose("Player is casting 'IdentifyAllWithCast'");
                return;
            }
        }

        public static Vector3 PreTownRunPosition { get; set; }
        internal static int PreTownRunWorldId { get; set; }
        internal static bool WasVendoring { get; set; }

        ///// <summary>
        /////     Called from Plugin.Pulse
        ///// </summary>
        //internal static void VendorRunPulseCheck()
        //{
        //    // If we're in town and vendoring
        //    if (Trinity.Player.IsInTown && BrainBehavior.IsVendoring)
        //    {
        //        WasVendoring = true;
        //        Trinity.ForceVendorRunASAP = true;
        //    }

            

        //}

        /// <summary>
        /// 
        /// </summary>
        internal static bool TownRunCanRun()
        {
            // If this returns false DB VendorHook will not be able to tick town run composites.
            try
            {
                using (new PerformanceLogger("TownRunOverlord"))
                {
                    if (ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                        return false;

                    Trinity.WantToTownRun = false;

                    if (Trinity.Player.IsDead)
                    {
                        return false;
                    }

                    if (BrainBehavior.IsVendoring && ZetaDia.IsInTown)
                    {
                        var secondsSinceWorldChange = DateTime.UtcNow.Subtract(Trinity.LastWorldChangeTime).TotalSeconds;
                        if (secondsSinceWorldChange < 2)
                        {
                            if (!_hasResolvedInTown)
                            {
                                _hasResolvedInTown = true;                            
                                //ZetaDia.Actors.Clear();
                                //ZetaDia.Actors.Update();
                                //ItemManager.Current.Refresh();
                            }
                            Logger.LogVerbose("Waiting before starting town run.");
                            return false;
                        }
                        return true;
                    }
                    _hasResolvedInTown = false;

                    // Check if we should be forcing a town-run
                    if (Trinity.ForceVendorRunASAP || BrainBehavior.IsVendoring)
                    {
                        if (!LastTownRunCheckResult)
                        {
                            if (BrainBehavior.IsVendoring)
                            {
                                Logger.Log("Looks like we are being asked to force a town-run by a profile/plugin/new DB feature, now doing so.");
                            }
                        }
                        SetPreTownRunPosition();
                        Trinity.WantToTownRun = true;
                    }

                    // Fix for A1 new game with bags full
                    // center of town x="2959.893" y="2806.495" z="24.04533" (new Vector3(2959.893f,2806.495f,24.04533f))
                    if (Trinity.Player.LevelAreaId == 19947 && ZetaDia.CurrentQuest.QuestSnoId == 87700 && new Vector3(2959.893f, 2806.495f, 24.04533f).Distance(ZetaDia.Me.Position) > 180f)
                    {
                        Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "Can't townrun with the current quest!");
                        Trinity.WantToTownRun = false;
                        return false;
                    }

                    // Time safety switch for more advanced town-run checking to prevent CPU spam
                    if (DateTime.UtcNow.Subtract(LastCheckBackpackDurability).TotalSeconds > 3)
                    {
                        LastCheckBackpackDurability = DateTime.UtcNow;

                        // Check for no space in backpack
                        if (!Trinity.Player.IsInventoryLockedForGreaterRift && (Trinity.Settings.Loot.TownRun.KeepLegendaryUnid || !Trinity.Player.ParticipatingInTieredLootRun))
                        {
                            Vector2 validLocation = TrinityItemManager.FindValidBackpackLocation(true);
                            if (validLocation.X < 0 || validLocation.Y < 0)
                            {
                                Logger.Log("No more space to pickup a 2-slot item, now running town-run routine. (TownRun)");
                                if (!LastTownRunCheckResult)
                                {
                                    LastTownRunCheckResult = true;
                                }
                                Trinity.WantToTownRun = true;

                                Trinity.ForceVendorRunASAP = true;
                                // Record the first position when we run out of bag space, so we can return later
                                SetPreTownRunPosition();
                            }
                        }
                        else
                        {
                            Logger.LogVerbose(LogCategory.Behavior, "TownRun inventory space check skipped to due to being in greater rift.");
                        }

                        if (ZetaDia.Me.IsValid)
                        {
                            List<ACDItem> equippedItems = ZetaDia.Me.Inventory.Equipped.Where(i => i.DurabilityMax > 0 && i.DurabilityCurrent != i.DurabilityMax).ToList();
                            if (equippedItems.Any())
                            {
                                double min = equippedItems.Average(i => i.DurabilityPercent);
                                var dbsetting = CharacterSettings.Instance.RepairWhenDurabilityBelow;

                                float threshold = Trinity.Player.IsInTown ? Math.Min(0.50f, dbsetting) : dbsetting;
                                bool needsRepair = min <= threshold;

                                if (needsRepair)
                                {
                                    Logger.Log("Items may need repair, now running town-run routine.");

                                    Trinity.WantToTownRun = true;
                                    Trinity.ForceVendorRunASAP = true;
                                    SetPreTownRunPosition();
                                }
                            }
                        }
                    }

                    if (ErrorDialog.IsVisible)
                    {
                        Trinity.WantToTownRun = false;
                    }

                    LastTownRunCheckResult = Trinity.WantToTownRun;

                    // Clear blacklists to triple check any potential targets
                    if (Trinity.WantToTownRun)
                    {
                        Trinity.Blacklist1Second = new HashSet<int>();
                        Trinity.Blacklist3Seconds = new HashSet<int>();
                        Trinity.Blacklist15Seconds = new HashSet<int>();
                        Trinity.Blacklist60Seconds = new HashSet<int>();
                        Trinity.Blacklist90Seconds = new HashSet<int>();
                    }

                    if (Trinity.WantToTownRun && !(BrainBehavior.IsVendoring || Trinity.Player.IsInTown))
                    {
                        string cantUseTPreason;
                        if (!ZetaDia.Me.CanUseTownPortal(out cantUseTPreason) && !ZetaDia.IsInTown)
                        {
                            Logger.LogVerbose("It appears we need to town run but can't: {0}", cantUseTPreason);
                            Trinity.WantToTownRun = false;
                            ClearArea.Reset();
                        }
                    }
                        
                    if (Trinity.WantToTownRun && DataDictionary.BossLevelAreaIDs.Contains(Trinity.Player.LevelAreaId))
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.GlobalHandler, "Unable to Town Portal - Boss Area!");
                        return false;
                    }
                    if (Trinity.WantToTownRun && ZetaDia.IsInTown && DeathHandler.EquipmentNeedsEmergencyRepair())
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.GlobalHandler, "EquipmentNeedsEmergencyRepair!");
                        return true;
                    }
                    if (Trinity.WantToTownRun && Trinity.CurrentTarget != null)
                    {
                        TownRunCheckTimer.Restart();
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.GlobalHandler, "Restarting TownRunCheckTimer, we have a target!");
                        return false;
                    }

                    if (Trinity.WantToTownRun && DataDictionary.NeverTownPortalLevelAreaIds.Contains(Trinity.Player.LevelAreaId))
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.GlobalHandler, "Unable to Town Portal in this area!");
                        return false;
                    }
                    if (Trinity.WantToTownRun && (TownRunTimerFinished() || BrainBehavior.IsVendoring) && !ClearArea.ShouldMoveToPortalPosition)
                    {
                        Logger.Log(TrinityLogLevel.Verbose, LogCategory.GlobalHandler, "Town run timer finished {0} or in town {1} or is vendoring {2} (TownRun)",
                            TownRunTimerFinished(), Trinity.Player.IsInTown, BrainBehavior.IsVendoring);

                        Trinity.WantToTownRun = false;
                        return true;
                    }
                    if (Trinity.WantToTownRun && !TownRunCheckTimer.IsRunning)
                    {
                        Logger.Log(TrinityLogLevel.Verbose, LogCategory.UserInformation, "Starting town run timer");
                        TownRunCheckTimer.Start();
                        _loggedAnythingThisStash = false;
                        _loggedJunkThisStash = false;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error Getting TownRun {0}: {1}", ex.Message, ex);

                //if (ex is CoroutineStoppedException)
                //    throw;

                return false;
            }
        }


        /// <summary>
        /// Records the position when we first run out of bag space, so we can return to that same position after a town run
        /// </summary>
        public static void SetPreTownRunPosition()
        {
            if (PreTownRunPosition == Vector3.Zero && PreTownRunWorldId == -1 && !Trinity.Player.IsInTown)
            {
                PreTownRunPosition = Trinity.Player.Position;
            }
        }

        private static void ModifyTownRun(GroupComposite original)
        {
            var firstChild = original.Children.FirstOrDefault() as GroupComposite;
            if (firstChild == null)
            {
                Logger.LogVerbose("Unexpected Composite or no vendor composite found in original town run");
                return;
            }

            //[Trinity 2.14.45] Child 0 type: Zeta.TreeSharp.Action
            //[Trinity 2.14.45] Child 1 type: Zeta.TreeSharp.Decorator
            //[Trinity 2.14.45] Child 2 type: Zeta.Common.HookExecutor IdentifyItems
            //[Trinity 2.14.45] Child 3 type: Zeta.TreeSharp.Action
            //[Trinity 2.14.45] Child 4 type: Zeta.Common.HookExecutor StashItems 
            //[Trinity 2.14.45] Child 5 type: Zeta.Common.HookExecutor SellAndRepair
            //[Trinity 2.14.45] Child 6 type: Zeta.Common.HookExecutor SalvageItems
            //[Trinity 2.14.45] Child 7 type: Zeta.TreeSharp.Decorator
            //[Trinity 2.14.45] Child 8 type: Zeta.TreeSharp.Action

            if (firstChild.Children.Count != 9)
                return;

            firstChild.InsertChild(3, new ActionRunCoroutine(async ret => await VendorHook.ExecutePreVendor()));
            firstChild.InsertChild(8, new ActionRunCoroutine(async ret => await VendorHook.ExecutePostVendor()));
        }

        private static void InsertChild(GroupComposite firstChild, int insertPosition, int index)
        {
            firstChild.InsertChild(insertPosition, new Zeta.TreeSharp.Action(ret =>
            {
                //Logger.Log("Step {0}", index);
                return RunStatus.Failure;
            }));
        }


        public static async Task<bool> TownRunCoroutineWrapper(Decorator original)
        {
            ModifyTownRun(original); 

            // Because ONLY VendorRun hook is executed while DB's vendor behaviors are executing.
            // We need to either insert a move to portal position here, or add a VendorHook @0 to ClearArea class                                    
            if (!ZetaDia.IsInTown)
            {
                Logger.Log("TownRunCoroutineWrapper.MoveToPortalPositionTask");
                await ClearArea.MoveToPortalPositionTask();
            }

            for (int i = 0; i < original.Children.Count; i++)
            {
                try
                {
                    await original.Children[i].ExecuteCoroutine();
                }
                catch (Exception ex)
                {
                    Logger.LogDebug("Exception in TownRunCoroutineWrapper CompositeId={1} IsLoadingWorld={2} IsInTown={3} MeIsValid={4} {0}",
                        ex, i, ZetaDia.IsLoadingWorld, ZetaDia.IsInTown, ZetaDia.Me != null && ZetaDia.Me.IsValid);
                }
            }     

            if (!BrainBehavior.IsVendoring)
            {
                Logger.Log("TownRun complete");
                Trinity.WantToTownRun = false;
                Trinity.ForceVendorRunASAP = false;
                TownRunCheckTimer.Reset();
                Helpers.Notifications.SendEmailNotification();
                Helpers.Notifications.SendMobileNotifications();
                LastTownRunFinishTime = DateTime.UtcNow;
            }
            return true;
        }

        public static DateTime LastTownRunFinishTime = DateTime.MinValue;
        private static bool _hasResolvedInTown;

        internal static bool TownRunTimerFinished()
        {
            return CacheData.Player.IsInTown || (TownRunCheckTimer.IsRunning && TownRunCheckTimer.ElapsedMilliseconds > 2000);
        }

        internal static bool TownRunTimerRunning()
        {
            return TownRunCheckTimer.IsRunning && TownRunCheckTimer.ElapsedMilliseconds < 2000;
        }




        internal static void StopRandomTimer()
        {
            randomTimer.Reset();
        }

        internal static bool RandomTimerIsDone()
        {
            return (randomTimer.IsRunning && randomTimer.ElapsedMilliseconds >= randomTimerVal);
        }

        internal static bool RandomTimerIsNotDone()
        {
            return (randomTimer.IsRunning && randomTimer.ElapsedMilliseconds < randomTimerVal);
        }





        public static Vector3 StashLocation
        {
            get
            {
                switch (Trinity.Player.LevelAreaId)
                {
                    case 19947: // Campaign A1 Hub
                        return new Vector3(2968.16f, 2789.63f, 23.94531f);
                    case 332339: // OpenWorld A1 Hub
                        return new Vector3(388.16f, 509.63f, 23.94531f);
                    case 168314: // A2 Hub
                        return new Vector3(323.0558f, 222.7048f, 0f);
                    case 92945: // A3/A4 Hub
                        return new Vector3(387.6834f, 382.0295f, 0f);
                    case 270011: // A5 Hub
                        return new Vector3(502.8296f, 739.7472f, 2.598635f);
                    default:
                        throw new ValueUnavailableException("Unknown LevelArea Id " + Trinity.Player.LevelAreaId);
                }
            }
        }

        public static DiaGizmo SharedStash
        {
            get
            {
                return ZetaDia.Actors.GetActorsOfType<GizmoPlayerSharedStash>()
                    .FirstOrDefault(o => o.IsFullyValid() && o.ActorInfo.IsValid && o.ActorInfo.GizmoType == GizmoType.SharedStash);
            }
        }
    }
}