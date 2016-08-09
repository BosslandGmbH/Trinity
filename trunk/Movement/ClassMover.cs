using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Abilities;
using Trinity.Config.Combat;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Dungeons;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Movement
{
    public class ClassMover
    {
        public static DateTime LastSpecialMovement = DateTime.MinValue;
        private const int MinimumTimeBetweenSpecialMovement = 50;

        public static bool SpecialMovement(Vector3 destination)
        {
            if (destination == Vector3.Zero)
                return false;

            if (DateTime.UtcNow.Subtract(LastSpecialMovement).TotalMilliseconds < MinimumTimeBetweenSpecialMovement)
                return false;

            if (CombatBase.IsCurrentlyKiting)
            {
                Logger.Log("Not Special Move Due to Kite");
                return false;
            }

            bool result;
            switch (Core.Player.ActorClass)
            {
                case ActorClass.Barbarian:
                    result = BarbMover(destination);
                    break;

                case ActorClass.Crusader:
                    result = CrusaderMover(destination);
                    break;

                case ActorClass.DemonHunter:
                    result = DemonHunterMover(destination);
                    break;

                case ActorClass.Monk:
                    result = MonkMover(destination);
                    break;

                case ActorClass.Witchdoctor:
                    result = WitchdoctorMover(destination);
                    break;

                case ActorClass.Wizard:
                    result = WizardMover(destination);
                    break;
                default:
                    return false;
            }

            if (result)
            {
                LastSpecialMovement = DateTime.UtcNow;
            }
            else
            {
                if (PlayerMover.IsBlocked && IsSpecialMovementReady)
                {
                    Logger.LogVerbose("Badness: Blocked, Not 'completely blocked' and unable to cast movement spell");
                }
            }

            return result;
        }

        private const int _interactDistance = 7;

        public static bool HasInfiniteCasting = CombatBase.GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting) ||
                                                Core.Buffs.HasBuff(SNOPower.ItemPassive_Unique_Ring_919_x1);

        private static float MinDistance = PlayerMover.IsCompletelyBlocked || CombatBase.IsCurrentlyAvoiding || HasInfiniteCasting
            ? 0
            : TrinityPlugin.CurrentTarget != null &&
              (TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Item || TrinityPlugin.CurrentTarget.IsNpc ||
               TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Shrine)
                ? 10
                : 25;

        public static bool OutOfCombatMovementAllowed
        {
            get
            {
                if (!Core.Settings.Combat.Misc.AllowOOCMovement && !PlayerMover.IsCompletelyBlocked)
                    return false;

                var Player = Core.Player;
                switch (Core.Player.ActorClass)
                {
                    case ActorClass.Barbarian:
                        return Core.Settings.Combat.Barbarian.WWMoveAlways ||
                               Core.Settings.Combat.Barbarian.UseLeapOOC ||
                               Core.Settings.Combat.Barbarian.SprintMode != BarbarianSprintMode.CombatOnly ||
                               Core.Settings.Combat.Barbarian.UseChargeOOC || Player.IsFrozen ||
                               Player.IsRooted || Player.IsJailed;
                    case ActorClass.Crusader:
                        return Core.Settings.Combat.Crusader.SteedChargeOOC;
                    case ActorClass.DemonHunter:
                        return Core.Settings.Combat.DemonHunter.VaultMode != DemonHunterVaultMode.CombatOnly;
                    case ActorClass.Monk:
                        return Core.Settings.Combat.Monk.TROption == TempestRushOption.MovementOnly ||
                               Core.Settings.Combat.Monk.TROption == TempestRushOption.Always ||
                               Core.Settings.Combat.Monk.UseDashingStrikeOOC || Player.IsFrozen ||
                               Player.IsRooted || Player.IsJailed;
                    case ActorClass.Witchdoctor:
                        return Core.Settings.Combat.WitchDoctor.UseSpiritWalkOffCooldown;
                    case ActorClass.Wizard:
                        return Core.Settings.Combat.Wizard.TeleportOOC || Player.IsFrozen || Player.IsRooted ||
                               Player.IsJailed;
                    default:
                        return false;
                }
            }
        }

        public static bool IsSpecialMovementReady
        {
            get
            {
                var player = Core.Player;
                switch (Core.Player.ActorClass)
                {
                    case ActorClass.Barbarian:
                        return player.PrimaryResource > 10 && CombatBase.CanCast(SNOPower.Barbarian_Whirlwind) ||
                               CombatBase.CanCast(SNOPower.Barbarian_Leap) ||
                               CombatBase.CanCast(SNOPower.Barbarian_Sprint) &&
                               (Runes.Barbarian.Gangway.IsActive || !PlayerMover.IsBlocked) ||
                               CombatBase.CanCast(SNOPower.Barbarian_FuriousCharge);
                    case ActorClass.Crusader:
                        return CombatBase.CanCast(SNOPower.X1_Crusader_SteedCharge) && !CombatBase.IsWaitingForPower();
                    case ActorClass.DemonHunter:
                        return Core.Player.PrimaryResource > 12 &&
                               CombatBase.CanCast(SNOPower.DemonHunter_Strafe) ||
                               CombatBase.CanCast(SNOPower.DemonHunter_Vault) ||
                               Skills.DemonHunter.Vault.IsActive && player.PrimaryResource > 20 &&
                               Legendary.ChainOfShadows.IsEquipped &&
                               CombatBase.CanCast(SNOPower.DemonHunter_Impale);
                    case ActorClass.Monk:
                        return CombatBase.CanCast(SNOPower.Monk_TempestRush) ||
                               CombatBase.CanCast(SNOPower.X1_Monk_DashingStrike) && Skills.Monk.DashingStrike.Charges > 0;
                    case ActorClass.Witchdoctor:
                        return CombatBase.CanCast(SNOPower.Witchdoctor_SpiritWalk);
                    case ActorClass.Wizard:
                        return Skills.Wizard.Teleport.CanCast() && (!Legendary.AetherWalker.IsEquipped ||
                                Legendary.AetherWalker.IsEquipped && player.PrimaryResource > 25) ||
                                Skills.Wizard.ArchonTeleport.CanCast();
                    default:
                        return false;
                }
            }
        }

        private static void LogMovement(SNOPower power, Vector3 destination)
        {
            if (Core.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement,
                    $"Using {power} for movement. Distance={Core.Player.Position.Distance(destination):0}");
        }

        #region Barb

        public static bool BarbMover(Vector3 destination)
        {
            float destinationDistance = Core.Player.Position.Distance(destination);

            if (DataDictionary.ChargeAnimations.Contains(Core.Player.CurrentAnimation) &&
                DataDictionary.LeapAnimations.Contains(Core.Player.CurrentAnimation))
                return false;
            //if (CombatBase.CanCast(SNOPower.Barbarian_FuriousCharge) &&
            //    SpellHistory.TimeSinceUse(SNOPower.Barbarian_FuriousCharge) < new TimeSpan(0, 0, 0, 0, 1250) ||
            //    CombatBase.CanCast(SNOPower.Barbarian_Leap) &&
            //    SpellHistory.TimeSinceUse(SNOPower.Barbarian_Leap) < new TimeSpan(0, 0, 0, 0, 1250))
            //    return false;

            if (destinationDistance >= MinDistance)
            {
                //var movementRange = 45f;
                //Vector3 point;
                //if (destinationDistance > movementRange && PlayerMover.GetCurrentPathFarthestPoint(MinDistance, movementRange, out point))
                //{
                //    destination = point;
                //}

                //if (destination == Vector3.Zero)
                //    return false;
                // Furious Charge movement for a barb
                var pierceCount = TrinityPlugin.Targets.Count(
                    u =>
                        u.IsUnit &&
                        MathUtil.IntersectsPath(u.Position, u.Radius + 5f, Core.Player.Position,
                            destination));
                if (Skills.Barbarian.FuriousCharge.CanCast() &&
                    (HasInfiniteCasting || pierceCount == 1 ||
                      pierceCount >= Math.Floor(5 * Core.Player.CooldownReductionPct)))
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
                (Core.Player.PrimaryResource > 20 && !Sets.BulKathossOath.IsFullyEquipped ||
                 Sets.BulKathossOath.IsFullyEquipped && Core.Player.PrimaryResourcePct > 0.90))
            {
                Skills.Barbarian.Sprint.Cast(destination);
                LogMovement(SNOPower.Barbarian_Sprint, destination);
            }

            //Don't Channel if Item Shrine or isNPC is near.
            if (TrinityPlugin.CurrentTarget != null && TrinityPlugin.CurrentTarget.Distance < _interactDistance &&
                (TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Item || TrinityPlugin.CurrentTarget.IsNpc ||
                 TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Shrine))
                return false;

            // Whirlwind
            if (CombatBase.CanCast(SNOPower.Barbarian_Whirlwind) && Core.Player.PrimaryResource > 10 &&
                (Sets.BulKathossOath.IsFullyEquipped || Core.Settings.Combat.Barbarian.WWMoveAlways))
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
                !DataDictionary.SteedChargeAnimations.Contains(Core.Player.CurrentAnimation))
            {
                //Logger.LogDebug("Steed Charge out of combat");
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

            var destinationDistance = destination.Distance(Core.Player.Position);
            if (destinationDistance < MinDistance)
            {
                return false;
            }

            if (destinationDistance < 25f)
            {
                destination = MathEx.CalculatePointFrom(destination, ZetaDia.Me.Position, 25);
            }

            //var movementRange = 35f;
            //if (destinationDistance > movementRange)
            //    destination = PlayerMover.GetCurrentPathFarthestPoint(MinDistance, movementRange);
            //if (destination == Vector3.Zero)
            //    return false;

            if (Core.Settings.Combat.DemonHunter.VaultMode == DemonHunterVaultMode.MovementOnly && CombatBase.IsInCombat)
                return false;

            // todo: bring over combat vaulting logic from DemonHunter routine.
            if (CombatBase.IsInCombat)
                return false;

            if (destination == Vector3.Zero)
                return false;

            // Vaulting away from stuff that needs to be interacted with.
            if (ZetaDia.Actors.GetActorsOfType<DiaGizmo>().Any(g => g.Distance < 10f && g.ActorInfo.GizmoType != GizmoType.DestroyableObject))
                return false;

            if (!Core.Avoidance.InCriticalAvoidance(ZetaDia.Me.Position) && Core.Avoidance.Grid.IsIntersectedByFlags(ZetaDia.Me.Position, destination, AvoidanceFlags.CriticalAvoidance))
                return false;

            int vaultDelay = Core.Settings.Combat.DemonHunter.VaultMovementDelay;
            var timeSinceUse = CombatBase.TimeSincePowerUse(SNOPower.DemonHunter_Vault);
            var isfree = DemonHunterCombat.IsVaultFree;
            var vaultBaseCost = 8; //Skills.DemonHunter.Vault.Cost*(1 - Core.Player.ResourceCostReductionPct);
            var vaultCost = Runes.DemonHunter.Acrobatics.IsActive || isfree
                ? 0
                : Runes.DemonHunter.Tumble.IsActive && Skills.DemonHunter.Vault.TimeSinceUse < 6000
                    ? Math.Round(vaultBaseCost * 0.5)
                    : vaultBaseCost;

            if (Core.Player.SecondaryResource < vaultCost) return false;

            if (DemonHunterCombat.CanAcquireFreeVaultBuff && Core.Player.PrimaryResource > 20)
            {
                Logger.LogVerbose(LogCategory.Movement, "Casting Impale for free Vault. (DemonHunterMover)");
                Skills.DemonHunter.Impale.Cast(MathEx.GetPointAt(Core.Player.Position, 5f,
                    Core.Player.Rotation));
            }

            if (timeSinceUse > vaultDelay || isfree && timeSinceUse > 250) //&&
                                                                           //// Don't Vault into aboolance/monsters if we're kiting
                                                                           //(CombatBase.KiteDistance <= 0 ||
                                                                           // (CombatBase.KiteDistance > 0 &&
                                                                           //  (!CacheData.TimeBoundAvoidance.Any(a => a.Position.Distance(destination) <= CombatBase.KiteDistance) ||
                                                                           //   !CacheData.TimeBoundAvoidance.Any(
                                                                           //       a => MathEx.IntersectsPath(a.Position, a.Radius, Core.Player.Position, destination)) ||
                                                                           //   !CacheData.MonsterObstacles.Any(a => a.Position.Distance(destination) <= CombatBase.KiteDistance)))))
            {

                //Logger.Log($"Casting vault OOC timeSinceUse={timeSinceUse} vaultDelay={vaultDelay}");

                // Prevent the bot from vaulting back and forth over and item without being able to pick it up.
                if (CombatBase.CurrentTarget?.Type == TrinityObjectType.Item && destinationDistance < 20f)
                    return false;

                if (destination == Vector3.Zero || destinationDistance < 10f)
                    return false;

                // Prevent trying to vault up walls; spider man he is not.
                if (Math.Abs(destination.Z - Core.Player.Position.Z) > 5)
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

            if (destinationDistance < MinDistance)
                return false;
            // Dashing Strike OOC
            if (CombatBase.CanCast(SNOPower.X1_Monk_DashingStrike))
            {
                var timeSinceUse = SpellHistory.TimeSinceUse(SNOPower.X1_Monk_DashingStrike).TotalMilliseconds;
                var dashDelaySetting = Core.Settings.Combat.Monk.DashingStrikeDelay;
                if (timeSinceUse < dashDelaySetting)
                    return false;

                var charges = Skills.Monk.DashingStrike.Charges;
                if (charges <= 0) return false;

                Skills.Monk.DashingStrike.Cast(destination);
                LogMovement(SNOPower.X1_Monk_DashingStrike, destination);
                return true;
            }
            //Don't Channel if Item Shrine or isNPC is near.
            if (TrinityPlugin.CurrentTarget != null && TrinityPlugin.CurrentTarget.Distance < _interactDistance &&
                (TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Item || TrinityPlugin.CurrentTarget.IsNpc ||
                 TrinityPlugin.CurrentTarget.Type == TrinityObjectType.Shrine))
                return false;

            //todo: This will have to be re-done if Tempest Rush ever becomes a thing again
            // Tempest rush for a monk
            if (CombatBase.CanCast(SNOPower.Monk_TempestRush))
            {
                Vector3 vTargetAimPoint = destination;

                vTargetAimPoint = TargetUtil.FindTempestRushTarget();
                var lastUse = SpellHistory.MillisecondsSinceUse(SNOPower.Monk_TempestRush);

                if (!_canChannelTempestRush &&
                    ((Core.Player.PrimaryResource >= Core.Settings.Combat.Monk.TR_MinSpirit &&
                      destinationDistance >= Core.Settings.Combat.Monk.TR_MinDist) ||
                     lastUse <= 150) && PowerManager.CanCast(SNOPower.Monk_TempestRush))
                {
                    _canChannelTempestRush = true;
                }
                else if (_canChannelTempestRush && (Core.Player.PrimaryResource < 10f))
                {
                    _canChannelTempestRush = false;
                }

                if (_canChannelTempestRush)
                {
                    LastTempestRushPosition = vTargetAimPoint;

                    ZetaDia.Me.UsePower(SNOPower.Monk_TempestRush, vTargetAimPoint, TrinityPlugin.CurrentWorldDynamicId,
                        -1);
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

                    if (Core.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement,
                            "Using Tempest Rush for OOC movement, distance={0:0} spirit={1:0} cd={2} lastUse={3:0} V3={4} vAim={5}",
                            destinationDistance, Core.Player.PrimaryResource,
                            PowerManager.CanCast(SNOPower.Monk_TempestRush), lastUse, destination, vTargetAimPoint);
                    return true;
                }
                else
                {
                    if (Core.Settings.Advanced.LogCategories.HasFlag(LogCategory.Movement))
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.Movement,
                            "Tempest rush failed!: {0:00.0} / {1} distance: {2:00.0} / {3}  MS: {4:0.0} lastUse={5:0}",
                            Core.Player.PrimaryResource,
                            Core.Settings.Combat.Monk.TR_MinSpirit,
                            destinationDistance,
                            Core.Settings.Combat.Monk.TR_MinDist,
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
                Skills.WitchDoctor.SpiritWalk.Cast(destination);
                LogMovement(SNOPower.Witchdoctor_SpiritWalk, destination);
            }
            return false;
        }

        #endregion

        #region Wizard

        public static bool WizardMover(Vector3 destination)
        {
            var destinationDistance = destination.Distance(Core.Player.Position);
            if (destinationDistance < MinDistance) return false;

            // const float movementRange = 50f;
            //if (destinationDistance > movementRange)
            //    destination = PlayerMover.GetCurrentPathFarthestPoint(MinDistance, movementRange);

            // Prevent bot from teleporting very short distance/on top of itself
            if (destination == Vector3.Zero)
                return false;

            // Teleport for a wizard 
            if (CombatBase.CanCast(SNOPower.Wizard_Teleport, CombatBase.CanCastFlags.NoTimer) &&
                SpellHistory.TimeSinceUse(SNOPower.Wizard_Teleport) >=
                new TimeSpan(0, 0, 0, 0, (int)Core.Settings.Combat.Wizard.TeleportDelay) || MinDistance < 10)
            {
                Skills.Wizard.Teleport.Cast(destination);
                LogMovement(SNOPower.Wizard_Teleport, destination);
                Navigator.Clear();
                GridSegmentation.Reset();
                return true;
            }
            if (Skills.Wizard.ArchonTeleport.CanCast())
            {
                Skills.Wizard.ArchonTeleport.Cast(destination);
                LogMovement(SNOPower.Wizard_Archon_Teleport, destination);
                Navigator.Clear();
                GridSegmentation.Reset();
                return true;
            }
            return false;
        }

        #endregion
    }
}