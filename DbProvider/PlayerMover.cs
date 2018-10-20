using Buddy.Coroutines;
using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Reference;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;


namespace Trinity.DbProvider
{
    public class PlayerMover : IPlayerMover
    {
        private static PlayerMover _instance;
        private static readonly DateTime LastUsedMoveStop = DateTime.MinValue;
        private static Coroutine _navigateToCoroutine;
        private static MoveResult _lastResult;

        public PlayerMover()
        {
            _instance = this;
        }

        public static PlayerMover Instance => _instance ?? (_instance = new PlayerMover());
        public static bool IsBlocked => Core.BlockedCheck.IsBlocked;
        public static bool IsCompletelyBlocked => IsBlocked;
        private static Vector3 LastDestination { get; set; }
        public static DefaultNavigationProvider NavigationProvider => Navigator.GetNavigationProviderAs<DefaultNavigationProvider>();

        /// <summary>
        /// Moves to a location by either PathFinding or 'Straight Line Pathing'.
        /// </summary>
        public static MoveResult MoveTo(Vector3 destination)
        {
            // Relieve pressure on Navigation Server and reduce minor delays where possible.
            var zDiff = Math.Abs(destination.Z - Core.Player.Position.Z);
            if (zDiff < 3f && !IsBlocked && !Core.StuckHandler.IsStuck)
            {
                if (Core.Player.Position.Distance(destination) < double.Epsilon)
                    return MoveResult.ReachedDestination;

                var isVeryClose = Core.Player.Position.Distance(destination) <= 8f;
                var canRayWalk = Core.Grids.CanRayWalk(Core.Player.Position, destination);

                if (isVeryClose && canRayWalk)
                {
                    //Core.Logger.Log(LogCategory.Movement, $"Trinity MoveTowards called to {destination}");
                    _instance.MoveTowards(destination);
                    return MoveResult.Moved;
                }
            }
            //Core.Logger.Log(LogCategory.Movement, $"Trinity MoveTo called to {destination}");
            return NavigateTo(destination);
        }

        /// <summary>
        /// Moves to a location by 'Straight Line Pathing'
        /// </summary>
        public void MoveTowards(Vector3 destination)
        {
            if (Core.Settings.Advanced.DisableAllMovement)
                return;

            if (ZetaDia.Globals.IsLoadingWorld)
                return;

            if (Core.IsOutOfGame)
                return;

            if (DateTime.UtcNow < SleepUntilTime)
                return;

            if (!ZetaDia.IsInGame || ZetaDia.Me == null || !ZetaDia.Me.IsValid || ZetaDia.Me.IsDead || ZetaDia.Globals.IsLoadingWorld)
                return;

            if (UiSafetyCheck())
                return;

            var power = TrinityCombat.Routines.Current.GetMovementPower(destination);
            if (power == null || power.SNOPower == SNOPower.None)
                power = new TrinityPower(SNOPower.Walk, 3f, destination);

            // We'll use the target position from the power, but make sure it's something reasonable.
            var isTooFarAway = power.TargetPosition.Distance(Core.Player.Position) > 80f;
            if (isTooFarAway || power.TargetPosition == Vector3.Zero)
                power.TargetPosition = destination;

            var startPosition = ZetaDia.Me.Position;

            if (ZetaDia.Me.UsePower(power.SNOPower, power.TargetPosition, Core.Player.WorldDynamicId))
            {
                if (power.SNOPower != SNOPower.Walk)
                {
                    SpellHistory.RecordSpell(power);
                }
                if (GameData.ResetNavigationPowers.Contains(power.SNOPower))
                {
                    Core.Logger.Log(LogCategory.Movement, $"Cast {power.SNOPower} at {power.TargetPosition} from={startPosition}");
                    SleepUntilTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(300);
                    Core.Grids.Avoidance.AdvanceNavigatorPath(power.MinimumRange, RayType.Walk);
                    MoveStop();
                }
            }
        }

        public DateTime SleepUntilTime = DateTime.MinValue;

        /// <summary>
        /// Moves to a location with DB's PathFinding
        /// </summary>
        public static MoveResult NavigateTo(Vector3 destination, string destinationName = "")
        {
            if (_navigateToCoroutine == null || _navigateToCoroutine.IsFinished)
            {
                if (!ZetaDia.Me.Movement.IsMoving && LastDestination != destination && ZetaDia.IsInGame)
                {
                    Core.Logger.Verbose(LogCategory.Movement, "NavigateTo: Starting Movement Towards {0} ({1})", destination, destinationName);
                    Instance.MoveTowards(destination);
                }
                _navigateToCoroutine = new Coroutine(async () =>
                {
                    _lastResult = await Navigator.MoveTo(destination, destinationName);
                    return _lastResult;
                });
            }

            LastDestination = destination;
            Core.Logger.Verbose(LogCategory.Movement, $"Resume: NavigateTo: {destination} {destinationName}");
            _navigateToCoroutine?.Resume();

            if (_navigateToCoroutine?.Status == CoroutineStatus.RanToCompletion)
            {
                return (MoveResult)_navigateToCoroutine?.Result;
            }

            return MoveResult.Moved;
        }

        public void MoveStop()
        {
            if (DateTime.UtcNow.Subtract(LastUsedMoveStop).TotalMilliseconds < 250)
                return;

            if (ZetaDia.Globals.IsLoadingWorld)
                return;

            Core.Logger.Verbose(LogCategory.Movement, "MoveStop");
            ZetaDia.Me.UsePower(SNOPower.Walk, MathEx.GetPointAt(ZetaDia.Me.Position, 2f, ZetaDia.Me.Movement.Rotation), ZetaDia.Globals.WorldId);
        }

        private bool ElementIsVisible(UIElement element)
        {
            if (element == null)
                return false;
            if (!UIElement.IsValidElement(element.Hash))
                return false;
            if (!element.IsValid)
                return false;
            if (!element.IsVisible)
                return false;

            return true;
        }

        public bool UiSafetyCheck()
        {
            return ElementIsVisible(UIElements.ConfirmationDialog) ||
                   ElementIsVisible(UIElements.ConfirmationDialogCancelButton) ||
                   ElementIsVisible(UIElements.ConfirmationDialogOkButton) ||
                   ElementIsVisible(UIElements.ReviveAtLastCheckpointButton);
        }

        public static bool CanMoveUnhindered
        {
            get
            {
                if (Legendary.IllusoryBoots.IsEquipped)
                    return true;

                switch (Core.Player.ActorClass)
                {
                    case ActorClass.Witchdoctor:
                        return Core.Buffs.HasBuff(SNOPower.Witchdoctor_SpiritWalk);

                    case ActorClass.Barbarian:
                        return Core.Buffs.HasBuff(SNOPower.Barbarian_Sprint) && Runes.Barbarian.Gangway.IsActive;

                    case ActorClass.Monk:
                        return Core.Buffs.HasBuff(SNOPower.Monk_TempestRush) || Runes.Monk.InstantKarma.IsActive && Core.Buffs.HasBuff(SNOPower.Monk_Serenity);

                    case ActorClass.Crusader:
                        return Core.Buffs.HasBuff(SNOPower.X1_Crusader_SteedCharge);
                }
                return false;
            }
        }
    }
}