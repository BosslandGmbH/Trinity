using Buddy.Coroutines;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Reference;
using Trinity.Modules;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Components.Coroutines.Town
{
    public class TrinityTownRun
    {
        private static readonly Lazy<PropertyInfo> s_vendorProperty = new Lazy<PropertyInfo>(() => typeof(BrainBehavior).GetProperty("IsVendoring"));
        public static bool StartedOutOfTown { get; set; }
        public static bool IsWantingTownRun { get; set; }
        public static bool IsInTownVendoring { get; set; }

        public static DateTime DontAttemptTownRunUntil = DateTime.MinValue;

        public static async Task<bool> Execute()
        {
            if (!ZetaDia.IsInGame)
            {
                StartedOutOfTown = false;
                IsWantingTownRun = false;
                IsInTownVendoring = false;
                IsVendoring = false;
                return false;
            }

            if (BrainBehavior.GreaterRiftInProgress)
                return false;

            if (await ClearArea.Execute())
            {
                Core.Logger.Debug("Clearing");
                return false;
            }

            CheckForDBVendoringBug();

            if (DateTime.UtcNow < DontAttemptTownRunUntil)
            {
                IsVendoring = false;
                return false;
            }

            // Fix for Campaign quest start of ACT1
            if (ZetaDia.CurrentQuest.QuestSnoId == 87700)
                return false;

            if (Core.CastStatus.StoneOfRecall.LastResult == CastResult.Casting)
            {
                Core.Logger.Verbose(LogCategory.GlobalHandler, "Casting");
                return true;
            }

            if (!ShouldStartTownRun())
            {
                IsVendoring = false;
                return false;
            }

            IsWantingTownRun = true;

            Core.Logger.Debug("Town run started");

            if (ZetaDia.Globals.IsLoadingWorld)
            {
                return true;
            }

            if (!ZetaDia.IsInTown)
            {
                if (ZetaDia.Me.IsInCombat && !ClearArea.IsClearing && ZetaDia.Actors.GetActorsOfType<DiaUnit>().Any(u => u?.CommonData != null && u.CommonData.IsValid && u.IsAlive && u.IsHostile && u.Distance < 16f))
                {
                    ClearArea.Start();
                }

                await GoToTown();

                if (!ZetaDia.IsInTown)
                {
                    if (Core.CastStatus.StoneOfRecall.LastResult == CastResult.Failed)
                    {
                        Core.Logger.Debug("Setting Town Run Cooldown because of cast failure");
                        DontAttemptTownRunUntil = DateTime.UtcNow.AddSeconds(5);
                    }
                    return true;
                }
            }

            Core.Logger.Verbose($"Starting Townrun");

            IsInTownVendoring = true;

            await Coroutine.Wait(TimeSpan.FromSeconds(10), () => !ZetaDia.Globals.IsLoadingWorld && ZetaDia.Globals.WorldSnoId > 0 && Core.Actors.Inventory.Any());
            await Coroutine.Yield();

            Core.Inventory.Backpack.ForEach(i => Core.Logger.Debug($"Backpack Item: {i.Name} ({i.ActorSnoId} / {i.InternalName}) RawItemType={i.RawItemType} TrinityItemType={i.TrinityItemType}"));

            await Coroutine.Yield();

            if (!await Any(
                IdentifyItems.Execute,
                ExtractLegendaryPowers.Execute,
                Gamble.Execute,
                () => CubeRaresToLegendary.Execute(),
                () => CubeItemsToMaterials.Execute(),
                DropItems.Execute,
                () => StashItems.Execute(true),
                SellItems.Execute,
                SalvageItems.Execute,
                () => StashItems.Execute(),
                RepairItems.Execute
            ))
                return false;

            Core.Logger.Log("Finished Town Run woo!");
            DontAttemptTownRunUntil = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            IsWantingTownRun = false;

            if (StartedOutOfTown)
            {
                StartedOutOfTown = false;
                await TakeReturnPortal();
            }

            IsWantingTownRun = false;
            IsInTownVendoring = false;
            IsVendoring = false;
            return false;
        }

        public static async Task<bool> Any(params Func<Task<bool>>[] taskProducers)
        {
            foreach (var task in taskProducers)
            {
                if (!await task())
                    return false;
                await Coroutine.Yield();
            }
            return true;
        }

        private static bool ShouldStartTownRun()
        {
            if (ZetaDia.IsInTown && BrainBehavior.IsVendoring)
                return !IsInTownVendoring;

            if (!CanTownRun())
                return false;

            if (IsWantingTownRun)
            {
                Core.Logger.Debug("Is wanting to town run.");
                return true;
            }

            // Close Normal rift before doing a town run.
            if (ZetaDia.IsInTown && !StartedOutOfTown && ZetaDia.Storage.RiftCompleted && ZetaDia.Storage.RiftStarted)
            {
                if (TownInfo.Orek?.GetActor() is DiaUnit orek && orek.IsQuestGiver)
                {
                    return false;
                }
            }

            var validLocation = DefaultLootProvider.FindBackpackLocation(true, false);
            if (validLocation.X < 0 || validLocation.Y < 0)
            {
                Core.Logger.Log("No more space to pickup a 2-slot item, now running town-run routine. (TownRun)");
                return true;
            }

            var needRepair = RepairItems.EquipmentNeedsRepair();
            if (needRepair)
            {
                Core.Logger.Debug("Townrun for repair.");
                return true;
            }

            return BrainBehavior.IsVendoring;
        }

        public static bool CanTownRun()
        {
            if (!ZetaDia.Me.CanUseTownPortal(out var cantUseTPreason) && !ZetaDia.IsInTown)
            {
                Core.Logger.Verbose("Can't townrun because '{0}'", cantUseTPreason);
                DontAttemptTownRunUntil = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                return false;
            }

            if (Core.Player.IsDead)
                return false;

            if (!RepairItems.EquipmentNeedsRepair())
            {
                if (BrainBehavior.GreaterRiftInProgress)
                {
                    Core.Logger.Verbose("Can't townrun while in greater rift!");
                    DontAttemptTownRunUntil = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                    return false;
                }

                // Close Greater rift before doing a town run.
                if (!Core.Settings.Items.KeepLegendaryUnid && Core.Player.ParticipatingInTieredLootRun)
                {
                    return false;
                }
            }

            if (ErrorDialog.IsVisible)
            {
                Core.Logger.Log("Can't townrun with an error dialog present!");
                DontAttemptTownRunUntil = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                return false;
            }

            if (Core.Player.WorldSnoId == 71150 && ZetaDia.CurrentQuest.QuestSnoId == 87700 && ZetaDia.CurrentQuest.StepId == -1)
            {
                Core.Logger.Debug("Can't townrun with the current quest (A1 New Game) !");
                DontAttemptTownRunUntil = DateTime.UtcNow + TimeSpan.FromSeconds(30);
                return false;
            }

            if (GameData.BossLevelAreaIDs.Contains(Core.Player.LevelAreaId))
            {
                Core.Logger.Debug("Unable to Town Portal - Boss Area!");
                DontAttemptTownRunUntil = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                return false;
            }

            if (GameData.NeverTownPortalLevelAreaIds.Contains(Core.Player.LevelAreaId))
            {
                Core.Logger.Log("Unable to Town Portal in this area!");
                DontAttemptTownRunUntil = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                return false;
            }

            return true;
        }

        public static bool IsVendoring
        {
            get => BrainBehavior.IsVendoring;
            set => s_vendorProperty.Value.SetValue(null, value);
        }

        public static async Task<bool> GoToTown()
        {
            if (ZetaDia.IsInTown || !ZetaDia.Me.IsFullyValid() || !UIElements.BackgroundScreenPCButtonRecall.IsEnabled)
            {
                Core.Logger.Log("Not portaling because its no longer needed or invalid.");
                return false;
            }

            Navigator.PlayerMover.MoveStop();
            await Coroutine.Wait(2000, () => !ZetaDia.Me.Movement.IsMoving);

            Core.Logger.Warn("Starting Town Run");
            StartedOutOfTown = true;

            if (!ZetaDia.IsInTown && !ZetaDia.Globals.IsLoadingWorld)
            {
                ZetaDia.Me.UseTownPortal();
            }

            await Coroutine.Wait(5000, () => !Core.CastStatus.StoneOfRecall.IsCasting && !ZetaDia.IsInTown);

            return true;
        }

        private static bool lastTownPortalCheckResult;
        private static DateTime lastTownPortalCheckTime = DateTime.MinValue;

        public static bool IsTryingToTownPortal()
        {
            if (DateTime.UtcNow.Subtract(lastTownPortalCheckTime).TotalMilliseconds < 100)
                return lastTownPortalCheckResult;

            if (!ZetaDia.Me.CanUseTownPortal() || GameData.NeverTownPortalLevelAreaIds.Contains(ZetaDia.CurrentLevelAreaSnoId))
            {
                lastTownPortalCheckTime = DateTime.UtcNow;
                lastTownPortalCheckResult = false;
                return false;
            }

            if (IsWantingTownRun)
            {
                lastTownPortalCheckTime = DateTime.UtcNow;
                lastTownPortalCheckResult = true;
                return true;
            }

            if (ClearArea.IsClearing)
            {
                lastTownPortalCheckTime = DateTime.UtcNow;
                lastTownPortalCheckResult = true;
                return true;
            }

            if (BrainBehavior.IsVendoring)
            {
                lastTownPortalCheckTime = DateTime.UtcNow;
                lastTownPortalCheckResult = true;
                return true;
            }

            if (Core.Player.IsCastingPortal)
            {
                lastTownPortalCheckTime = DateTime.UtcNow;
                lastTownPortalCheckResult = true;
                return true;
            }

            lastTownPortalCheckTime = DateTime.UtcNow;
            lastTownPortalCheckResult = false;
            return false;
        }

        public static async Task<bool> TakeReturnPortal()
        {
            if (!ZetaDia.IsInTown)
                return false;

            var portalRef = TownInfo.ReturnPortal;
            var actor = portalRef?.GetActor();
            if (actor == null || !actor.IsFullyValid())
            {
                Core.Logger.Debug("Couldn't find a return portal");
                return false;
            }

            Core.Logger.Log("Found a hearth portal, lets use it.");

            if (!await MoveToAndInteract.Execute(actor, 10))
            {
                Core.Logger.Log("Failed to move to return portal :(");
                return false;
            }

            Core.PlayerMover.MoveStop();

            if (actor.IsFullyValid() && !actor.Interact())
            {
                Core.Logger.Debug("Failed to interact with return portal.");
            }

            await Coroutine.Wait(1000, () => !ZetaDia.IsInTown);

            if (ZetaDia.IsInTown && !ZetaDia.Globals.IsLoadingWorld)
            {
                Core.Logger.Log("Trying again to use return portal.");
                var gizmo = ZetaDia.Actors.GetActorsOfType<DiaGizmo>().FirstOrDefault(g => g.ActorInfo.GizmoType == GizmoType.HearthPortal);
                if (gizmo != null)
                {
                    await CommonCoroutines.MoveAndStop(gizmo.Position, gizmo.InteractDistance, "Portal Position");
                    gizmo.Interact();
                }
            }

            await Coroutine.Wait(5000, () => !ZetaDia.Globals.IsLoadingWorld);

            return true;
        }
        
        #region Remove when DB bug is fixed

        private static DateTime _brainVendoringStarted;
        private static bool _isVendoring;

        private static void CheckForDBVendoringBug()
        {
            // An exception in DB core during town run will cause IsVendoring to never be set to false.
            var isVendoring = BrainBehavior.IsVendoring;
            if (isVendoring != _isVendoring)
            {
                if (isVendoring)
                {
                    _brainVendoringStarted = DateTime.UtcNow;
                    _isVendoring = true;
                    return;
                }
            }

            if (!isVendoring || !(DateTime.UtcNow.Subtract(_brainVendoringStarted).TotalSeconds > 200)) return;

            _isVendoring = false;
            IsVendoring = false;
        }

        #endregion Remove when DB bug is fixed
    }
}