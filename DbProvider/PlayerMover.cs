using System;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
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
        public static DefaultNavigationProvider NavigationProvider => Navigator.GetNavigationProviderAs<DefaultNavigationProvider>();
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
                    default:
                        return false;
                }
            }
        }

        private DateTime _sleepUntilTime = DateTime.MinValue;
        public bool IsUIBlockingMovement => IsElementVisible(UIElements.ConfirmationDialog) ||
                                            IsElementVisible(UIElements.ConfirmationDialogCancelButton) ||
                                            IsElementVisible(UIElements.ConfirmationDialogOkButton) ||
                                            IsElementVisible(UIElements.ReviveAtLastCheckpointButton);

        /// <inheritdoc />
        public void MoveTowards(Vector3 destination)
        {
            if (Core.Settings.Advanced.DisableAllMovement)
                return;

            if (ZetaDia.Globals.IsLoadingWorld)
                return;

            if (Core.IsOutOfGame)
                return;

            if (DateTime.UtcNow < _sleepUntilTime)
                return;

            if (!ZetaDia.IsInGame ||
                ZetaDia.Me == null ||
                !ZetaDia.Me.IsValid ||
                ZetaDia.Me.IsDead ||
                ZetaDia.Globals.IsLoadingWorld)
                return;

            if (IsUIBlockingMovement)
                return;

            var power = TrinityCombat.Routines.Current.GetMovementPower(destination);
            if (power == null ||
                power.SNOPower == SNOPower.None)
                power = new TrinityPower(SNOPower.Walk, 3f, destination);

            // We'll use the target position from the power, but make sure it's something reasonable.
            var isTooFarAway = power.TargetPosition.Distance(Core.Player.Position) > 80f;
            if (isTooFarAway || power.TargetPosition == Vector3.Zero)
                power.TargetPosition = destination;

            var startPosition = ZetaDia.Me.Position;

            if (!ZetaDia.Me.UsePower(power.SNOPower, power.TargetPosition, Core.Player.WorldDynamicId))
                return;

            if (power.SNOPower != SNOPower.Walk)
                SpellHistory.RecordSpell(power);

            if (!GameData.ResetNavigationPowers.Contains(power.SNOPower))
                return;

            Core.Logger.Log(LogCategory.Movement, $"Cast {power.SNOPower} at {power.TargetPosition} from={startPosition}");
            _sleepUntilTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(300);
            Core.Grids.Avoidance.AdvanceNavigatorPath(power.MinimumRange, RayType.Walk);
            MoveStop();
        }
        
        public void MoveStop()
        {
            if (DateTime.UtcNow.Subtract(s_lastUsedMoveStop).TotalMilliseconds < 250)
                return;

            if (ZetaDia.Globals.IsLoadingWorld)
                return;

            Core.Logger.Verbose(LogCategory.Movement, "MoveStop");
            ZetaDia.Me.UsePower(
                SNOPower.Walk,
                MathEx.GetPointAt(
                    ZetaDia.Me.Position,
                    2f,
                    ZetaDia.Me.Movement.Rotation),
                ZetaDia.Globals.WorldId);
        }

        private bool IsElementVisible(UIElement element)
        {
            if (element == null)
                return false;

            if (!UIElement.IsValidElement(element.Hash))
                return false;

            return element.IsValid && element.IsVisible;
        }
    }
}
