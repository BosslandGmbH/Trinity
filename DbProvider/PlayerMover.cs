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
        private static readonly DateTime s_lastUsedMoveStop = DateTime.MinValue;

        public PlayerMover()
        {
            _instance = this;
        }

        public static PlayerMover Instance => _instance ?? (_instance = new PlayerMover());
        public static bool IsBlocked => Core.BlockedCheck.IsBlocked;
        public static bool IsCompletelyBlocked => IsBlocked;
        private static Vector3 LastDestination { get; set; }
        public static DefaultNavigationProvider NavigationProvider => Navigator.GetNavigationProviderAs<DefaultNavigationProvider>();
        
        /// <inheritdoc />
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

        public void MoveStop()
        {
            if (DateTime.UtcNow.Subtract(s_lastUsedMoveStop).TotalMilliseconds < 250)
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