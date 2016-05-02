using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Combat.Abilities;
using Trinity.Config.Combat;
using Trinity.DbProvider;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Movement
{
    public class ClassMover
    {
        public static bool SpecialMovement(Vector3 destination)
        {
            if (destination == Vector3.Zero)
                return false;

            switch (CacheData.Player.ActorClass)
            {
                case ActorClass.Barbarian:
                    return BarbMover(destination);
                case ActorClass.Crusader:
                    return CrusaderMover(destination);
                case ActorClass.DemonHunter:
                    return DemonHunterMover(destination);
                case ActorClass.Monk:
                    return MonkMover(destination);
                case ActorClass.Witchdoctor:
                    return WitchdoctorMover(destination);
                case ActorClass.Wizard:
                    return WizardMover(destination);
                default:
                    return false;
            }
        }

        private const int _interactDistance = 7;

        private static bool HasInGeomBuff = CacheData.Buffs.HasBuff(SNOPower.ItemPassive_Unique_Ring_919_x1);

        private static float MinDistance = PlayerMover.IsBlocked || CombatBase.IsCurrentlyAvoiding
            ? 0
            : TrinityPlugin.CurrentTarget != null &&
              (TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Item || TrinityPlugin.CurrentTarget.IsNPC ||
               TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Shrine)
                ? 10
                : HasInGeomBuff ? 10 : 15;

        public static bool OutOfCombatMovementAllowed
        {
            get
            {
                var Player = CacheData.Player;
                switch (CacheData.Player.ActorClass)
                {
                    case ActorClass.Barbarian:
                        return TrinityPlugin.Settings.Combat.Barbarian.WWMoveAlways ||
                               TrinityPlugin.Settings.Combat.Barbarian.UseLeapOOC ||
                               TrinityPlugin.Settings.Combat.Barbarian.SprintMode != BarbarianSprintMode.CombatOnly ||
                               TrinityPlugin.Settings.Combat.Barbarian.UseChargeOOC;
                    case ActorClass.Crusader:
                        return TrinityPlugin.Settings.Combat.Crusader.SteedChargeOOC;
                    case ActorClass.DemonHunter:
                        return TrinityPlugin.Settings.Combat.DemonHunter.VaultMode != DemonHunterVaultMode.CombatOnly;
                    case ActorClass.Monk:
                        return TrinityPlugin.Settings.Combat.Monk.TROption == TempestRushOption.MovementOnly ||
                               TrinityPlugin.Settings.Combat.Monk.TROption == TempestRushOption.Always ||
                               TrinityPlugin.Settings.Combat.Monk.UseDashingStrikeOOC;
                    case ActorClass.Witchdoctor:
                        return TrinityPlugin.Settings.Combat.WitchDoctor.UseSpiritWalkOffCooldown;
                    case ActorClass.Wizard:
                        return TrinityPlugin.Settings.Combat.Wizard.TeleportOOC;
                    default:
                        return false;
                }
            }
        }

        public static bool IsSpecialMovementReady
        {
            get
            {
                var Player = CacheData.Player;
                switch (CacheData.Player.ActorClass)
                {
                    case ActorClass.Barbarian:
                        return Player.PrimaryResource > 10 && CombatBase.CanCast(SNOPower.Barbarian_Whirlwind) ||
                               CombatBase.CanCast(SNOPower.Barbarian_Leap) ||
                               CombatBase.CanCast(SNOPower.Barbarian_Sprint) &&
                               (Runes.Barbarian.Gangway.IsActive || !PlayerMover.IsBlocked) ||
                               CombatBase.CanCast(SNOPower.Barbarian_FuriousCharge);
                    case ActorClass.Crusader:
                        return CombatBase.CanCast(SNOPower.X1_Crusader_SteedCharge);
                    case ActorClass.DemonHunter:
                        return TrinityPlugin.Player.PrimaryResource > 12 && CombatBase.CanCast(SNOPower.DemonHunter_Strafe) ||
                               CombatBase.CanCast(SNOPower.DemonHunter_Vault) ||
                               Skills.DemonHunter.Vault.IsActive && Player.PrimaryResource > 20 &&
                               Legendary.ChainOfShadows.IsEquipped &&
                               CombatBase.CanCast(SNOPower.DemonHunter_Impale);
                    case ActorClass.Monk:
                        return CombatBase.CanCast(SNOPower.Monk_TempestRush) ||
                               CombatBase.CanCast(SNOPower.X1_Monk_DashingStrike);
                    case ActorClass.Witchdoctor:
                        return CombatBase.CanCast(SNOPower.Witchdoctor_SpiritWalk);
                    case ActorClass.Wizard:
                        return CombatBase.CanCast(SNOPower.Wizard_Teleport) &&
                               (!Legendary.AetherWalker.IsEquipped ||
                                Legendary.AetherWalker.IsEquipped && Player.PrimaryResource > 25) ||
                               CombatBase.CanCast(SNOPower.Wizard_Archon_Teleport);
                    default:
                        return false;
                }
            }
        }

        private static void LogMovement(SNOPower power, Vector3 destination)
        {
            if (TrinityPlugin.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement,
                    $"Using {power} for movement. Distance={0:0}", PlayerMover.MyPosition.Distance(destination));
        }

        #region Barb

        public static bool BarbMover(Vector3 destination)
        {
            float destinationDistance = PlayerMover.MyPosition.Distance(destination);

            if (DataDictionary.ChargeAnimations.Contains(CacheData.Player.CurrentAnimation) &&
                DataDictionary.LeapAnimations.Contains(CacheData.Player.CurrentAnimation))
                return false;
            //if (CombatBase.CanCast(SNOPower.Barbarian_FuriousCharge) &&
            //    SpellHistory.TimeSinceUse(SNOPower.Barbarian_FuriousCharge) < new TimeSpan(0, 0, 0, 0, 1250) ||
            //    CombatBase.CanCast(SNOPower.Barbarian_Leap) &&
            //    SpellHistory.TimeSinceUse(SNOPower.Barbarian_Leap) < new TimeSpan(0, 0, 0, 0, 1250))
            //    return false;

            if (destinationDistance >= MinDistance)
            {
                var movementRange = 45f;
                if (destinationDistance > movementRange)
                    destination = PlayerMover.GetCurrentPathFarthestPoint(MinDistance, movementRange);

                if (destination == Vector3.Zero)
                    return false;

                // Furious Charge movement for a barb
                if ((HasInGeomBuff || CombatBase.CanCast(SNOPower.Barbarian_FuriousCharge)) &&
                    (TrinityPlugin.ObjectCache.Count(
                        u =>
                            u.IsUnit &&
                            MathUtil.IntersectsPath(u.Position, u.Radius + 5f, TrinityPlugin.Player.Position,
                                destination))*2 >
                     Skills.Barbarian.FuriousCharge.CooldownRemaining/1000))
                {
                    Skills.Barbarian.FuriousCharge.Cast(destination);
                    LogMovement(SNOPower.Barbarian_FuriousCharge, destination);
                    return true;
                }
                // Leap movement for a barb
                if (CombatBase.CanCast(SNOPower.Barbarian_Leap))
                {
                    Skills.Barbarian.Leap.Cast(destination);
                    LogMovement(SNOPower.Barbarian_Leap, destination);
                    return true;
                }
            }

            //Sprint
            if (CombatBase.CanCast(SNOPower.Barbarian_Sprint) && !CombatBase.GetHasBuff(SNOPower.Barbarian_Sprint) && 
                (TrinityPlugin.Player.PrimaryResource > 20 && !Sets.BulKathossOath.IsFullyEquipped ||
                Sets.BulKathossOath.IsFullyEquipped && TrinityPlugin.Player.PrimaryResourcePct > 0.90))
            {
                Skills.Barbarian.Sprint.Cast(destination);
                LogMovement(SNOPower.Barbarian_Sprint, destination);
            }

            //Don't Channel if Item Shrine or isNPC is near.
            if (TrinityPlugin.CurrentTarget != null && TrinityPlugin.CurrentTarget.Distance < _interactDistance &&
                (TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Item || TrinityPlugin.CurrentTarget.IsNPC ||
                 TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Shrine))
                return false;

            // Whirlwind
            if (CombatBase.CanCast(SNOPower.Barbarian_Whirlwind) && TrinityPlugin.Player.PrimaryResource > 10 &&
                (Sets.BulKathossOath.IsFullyEquipped || TrinityPlugin.Settings.Combat.Barbarian.WWMoveAlways))
            {
                Skills.Barbarian.Whirlwind.Cast(destination);
                LogMovement(SNOPower.Barbarian_Whirlwind, destination);
                return false;
            }
            return false;
        }

        #endregion

        #region Crusader

        public static bool CrusaderMover(Vector3 destination)
        {
            if (CombatBase.CanCast(SNOPower.X1_Crusader_SteedCharge) &&
                !DataDictionary.SteedChargeAnimations.Contains(CacheData.Player.CurrentAnimation))
            {
                Skills.Crusader.SteedCharge.Cast(destination);
                LogMovement(SNOPower.X1_Crusader_SteedCharge, destination);
            }
            return false;
        }

        #endregion

        #region DemonHunter

        public static bool DemonHunterMover(Vector3 destination)
        {
            if (!CombatBase.CanCast(SNOPower.DemonHunter_Vault)) return false;

            var destinationDistance = destination.Distance(CacheData.Player.Position);
            if (destinationDistance < MinDistance) return false;

            var movementRange = 35f;
            if (destinationDistance > movementRange)
                destination = PlayerMover.GetCurrentPathFarthestPoint(MinDistance, movementRange);
            if (destination == Vector3.Zero)
                return false;

            int vaultDelay = TrinityPlugin.Settings.Combat.DemonHunter.VaultMovementDelay;
            var timeSinceUse = CombatBase.TimeSincePowerUse(SNOPower.DemonHunter_Vault);
            var isfree = DemonHunterCombat.IsVaultFree;
            var vaultBaseCost = 8; //Skills.DemonHunter.Vault.Cost*(1 - TrinityPlugin.Player.ResourceCostReductionPct);
            var vaultCost = Runes.DemonHunter.Acrobatics.IsActive || isfree
                ? 0
                : Runes.DemonHunter.Tumble.IsActive && Skills.DemonHunter.Vault.TimeSinceUse < 6000
                    ? Math.Round(vaultBaseCost*0.5)
                    : vaultBaseCost;
            if (TrinityPlugin.Player.SecondaryResource < vaultCost) return false;

            if (DemonHunterCombat.CanAcquireFreeVaultBuff && TrinityPlugin.Player.PrimaryResource > 20)
            {
                Logger.LogVerbose(LogCategory.Movement, "Casting Impale for free Vault. (DemonHunterMover)");
                Skills.DemonHunter.Impale.Cast(MathEx.GetPointAt(TrinityPlugin.Player.Position, 5f,
                    TrinityPlugin.Player.Rotation));
            }

            if ((timeSinceUse > vaultDelay || isfree && timeSinceUse > 250) &&
                // Don't Vault into aboolance/monsters if we're kiting
                (CombatBase.KiteDistance <= 0 ||
                 (CombatBase.KiteDistance > 0 &&
                  (!CacheData.TimeBoundAvoidance.Any(a => a.Position.Distance(destination) <= CombatBase.KiteDistance) ||
                   !CacheData.TimeBoundAvoidance.Any(
                       a => MathEx.IntersectsPath(a.Position, a.Radius, TrinityPlugin.Player.Position, destination)) ||
                   !CacheData.MonsterObstacles.Any(a => a.Position.Distance(destination) <= CombatBase.KiteDistance)))))
            {

                // Prevent the bot from vaulting back and forth over and item without being able to pick it up.
                if (CombatBase.CurrentTarget?.Type == TrinityObjectType.Item && destinationDistance < 20f)
                    return false;

                if (destination == Vector3.Zero)
                    return false;

                Skills.DemonHunter.Vault.Cast(destination);
                LogMovement(SNOPower.DemonHunter_Vault, destination);
                return true;
            }
            return false;
        }

        #endregion

        #region Monk

        //For Tempest Rush Monks
        private static bool _canChannelTempestRush;

        internal static Vector3 LastTempestRushPosition = Vector3.Zero;

        public static bool MonkMover(Vector3 destination)
        {
            float destinationDistance = PlayerMover.MyPosition.Distance(destination);

            // Dashing Strike OOC
            if (CombatBase.CanCast(SNOPower.X1_Monk_DashingStrike))
            {
                var movementRange = 35f;
                if (destinationDistance > movementRange)
                    destination = PlayerMover.GetCurrentPathFarthestPoint(MinDistance, movementRange);
                if (destination == Vector3.Zero)
                    return false;

                var charges = Skills.Monk.DashingStrike.Charges;
                if (charges <= 0) return false;

                if (HasInGeomBuff || Sets.ThousandStorms.IsSecondBonusActive &&
                    ((TrinityPlugin.Player.PrimaryResource >= 75) ||
                     CacheData.BuffsCache.Instance.HasCastingShrine))
                {
                    Skills.Monk.DashingStrike.Cast(destination);
                    LogMovement(SNOPower.X1_Monk_DashingStrike, destination);
                    return true;
                }
            }
            //Don't Channel if Item Shrine or isNPC is near.
            if (TrinityPlugin.CurrentTarget != null && TrinityPlugin.CurrentTarget.Distance < _interactDistance &&
                (TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Item || TrinityPlugin.CurrentTarget.IsNPC ||
                 TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Shrine))
                return false;

            //todo: This will have to be re-done if Tempest Rush ever becomes a thing again
            // Tempest rush for a monk
            if (CombatBase.CanCast(SNOPower.Monk_TempestRush))
            {
                Vector3 vTargetAimPoint = destination;

                vTargetAimPoint = TargetUtil.FindTempestRushTarget();

                if (!_canChannelTempestRush &&
                    ((TrinityPlugin.Player.PrimaryResource >= TrinityPlugin.Settings.Combat.Monk.TR_MinSpirit &&
                      destinationDistance >= TrinityPlugin.Settings.Combat.Monk.TR_MinDist) ||
                     DateTime.UtcNow.Subtract(CacheData.AbilityLastUsed[SNOPower.Monk_TempestRush]).TotalMilliseconds <=
                     150) && PowerManager.CanCast(SNOPower.Monk_TempestRush))
                {
                    _canChannelTempestRush = true;
                }
                else if (_canChannelTempestRush && (TrinityPlugin.Player.PrimaryResource < 10f))
                {
                    _canChannelTempestRush = false;
                }

                double lastUse =
                    DateTime.UtcNow.Subtract(CacheData.AbilityLastUsed[SNOPower.Monk_TempestRush]).TotalMilliseconds;

                if (_canChannelTempestRush)
                {
                    LastTempestRushPosition = vTargetAimPoint;

                    ZetaDia.Me.UsePower(SNOPower.Monk_TempestRush, vTargetAimPoint, TrinityPlugin.CurrentWorldDynamicId, -1);
                    SpellHistory.RecordSpell(SNOPower.Monk_TempestRush);

                    // simulate movement speed of 30
                    SpeedSensor lastSensor =
                        PlayerMover.SpeedSensors.OrderByDescending(s => s.Timestamp).FirstOrDefault();
                    PlayerMover.SpeedSensors.Add(new SpeedSensor()
                    {
                        Location = PlayerMover.MyPosition,
                        TimeSinceLastMove = new TimeSpan(0, 0, 0, 0, 1000),
                        Distance = 5f,
                        WorldID = TrinityPlugin.CurrentWorldDynamicId
                    });

                    if (TrinityPlugin.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement,
                            "Using Tempest Rush for OOC movement, distance={0:0} spirit={1:0} cd={2} lastUse={3:0} V3={4} vAim={5}",
                            destinationDistance, TrinityPlugin.Player.PrimaryResource,
                            PowerManager.CanCast(SNOPower.Monk_TempestRush), lastUse, destination, vTargetAimPoint);
                    return true;
                }
                else
                {
                    if (TrinityPlugin.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement,
                            "Tempest rush failed!: {0:00.0} / {1} distance: {2:00.0} / {3}  MS: {4:0.0} lastUse={5:0}",
                            TrinityPlugin.Player.PrimaryResource,
                            TrinityPlugin.Settings.Combat.Monk.TR_MinSpirit,
                            destinationDistance,
                            TrinityPlugin.Settings.Combat.Monk.TR_MinDist,
                            PlayerMover.GetMovementSpeed(),
                            lastUse);

                    TrinityPlugin.MaintainTempestRush = false;
                }

                // Always set this from PlayerMover
                MonkCombat.LastTempestRushLocation = vTargetAimPoint;

            }
            return false;
        }

        #endregion

        #region Witchdoctor

        public static bool WitchdoctorMover(Vector3 destination)
        {
            if (CombatBase.CanCast(SNOPower.Witchdoctor_SpiritWalk) &&
                !CombatBase.GetHasBuff(SNOPower.Witchdoctor_SpiritWalk))
            {
                Skills.Crusader.SteedCharge.Cast(destination);
                LogMovement(SNOPower.Witchdoctor_SpiritWalk, destination);
            }
            return false;
        }

        #endregion

        #region Wizard

        public static bool WizardMover(Vector3 destination)
        {
            var destinationDistance = destination.Distance(CacheData.Player.Position);
            if (destinationDistance < MinDistance) return false;
            const float movementRange = 50f;

            if (destinationDistance > movementRange)
                destination = PlayerMover.GetCurrentPathFarthestPoint(MinDistance, movementRange);
            if (destination == Vector3.Zero)
                return false;

            // Teleport for a wizard 
            if (CombatBase.CanCast(SNOPower.Wizard_Teleport, CombatBase.CanCastFlags.NoTimer) &&
                SpellHistory.TimeSinceUse(SNOPower.Wizard_Teleport) >=
                new TimeSpan(0, 0, 0, 0, (int) TrinityPlugin.Settings.Combat.Wizard.TeleportDelay))
            {
                Skills.Wizard.Teleport.Cast(destination);
                LogMovement(SNOPower.Wizard_Teleport, destination);
                return true;
            }
            if (CombatBase.CanCast(SNOPower.Wizard_Archon_Teleport))
            {
                Skills.Wizard.ArchonTeleport.Cast(destination);
                LogMovement(SNOPower.Wizard_Archon_Teleport, destination);
                return true;
            }
            return false;
        }

        #endregion
    }
}