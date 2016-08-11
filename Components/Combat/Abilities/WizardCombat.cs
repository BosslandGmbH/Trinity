using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Config.Combat;
using Trinity.Framework;
using Trinity.Framework.Modules;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Components.Combat.Abilities
{
    class WizardCombat : CombatBase
    {
        public static int SerpentSparkerId = 272084;
        private static DateTime _lastTargetChange = DateTime.MinValue;
        private static readonly bool IsDmoWiz = Sets.DelseresMagnumOpus.IsFullyEquipped && Legendary.Triumvirate.IsEquipped &&
               Passives.Wizard.ArcaneDynamo.IsActive;

        internal static WizardSetting WizardSettings
        {
            get { return Core.Settings.Combat.Wizard; }
        }

        public static bool IsFirebirdArchonBuild => Sets.ChantodosResolve.IsFullyEquipped && Sets.FirebirdsFinery.IsFullyEquipped && Skills.Wizard.Archon.IsActive;

        /// <summary>
        /// Checks and casts Buffs, Avoidance powers, and Combat Powers
        /// </summary>
        /// <returns></returns>
        internal static TrinityPower GetPower()
        {
            TrinityPower power;

            if (TryGetArchonPower(out power))
                return power;

            // Buffs
            if (UseOOCBuff)
            {
                return GetBuffPower();
            }

            // Destructibles
            if (UseDestructiblePower)
                return DestroyObjectPower();

            if (Settings.Combat.Wizard.AlwaysExplosiveBlast && CanCast(SNOPower.Wizard_ExplosiveBlast) && Player.PrimaryResource > 20 && !Player.IsInTown && !Player.IsCastingOrLoading)
            {
                return new TrinityPower(SNOPower.Wizard_ExplosiveBlast);
            }

            // In Combat, Avoiding
            if (IsCurrentlyAvoiding)
            {
                return GetCombatAvoidancePower();
            }

            // In combat, Not Avoiding
            if (CurrentTarget != null)
            {
                return GetCombatPower();
            }

            // Default attacks
            return DefaultPower;
        }

        /// <summary>
        /// Gets the best (non-movement related) avoidance power
        /// </summary>
        /// <returns></returns>
        private static TrinityPower GetCombatAvoidancePower()
        {
            // Defensive Teleport: SafePassage
            if (CanCastTeleport())
            {
                var slightlyForwardPosition = MathEx.GetPointAt(Core.Avoidance.Avoider.SafeSpot, 4f, Player.Rotation);
                return new TrinityPower(SNOPower.Wizard_Teleport, 65f, slightlyForwardPosition);
            }

            // Diamond Skin: Tank mode
            if (CanCast(SNOPower.Wizard_DiamondSkin, CanCastFlags.NoTimer) && LastPowerUsed != SNOPower.Wizard_DiamondSkin && !GetHasBuff(SNOPower.Wizard_DiamondSkin) &&
                (TargetUtil.AnyElitesInRange(25, 1) || TargetUtil.AnyMobsInRange(25, 1) || Player.CurrentHealthPct <= 0.90 || Player.IsIncapacitated || Player.IsRooted || CurrentTarget.RadiusDistance <= 40f))
            {
                return new TrinityPower(SNOPower.Wizard_DiamondSkin);
            }

            // Explosive Blast
            if (CanCast(SNOPower.Wizard_ExplosiveBlast, CanCastFlags.NoTimer) && !Player.IsIncapacitated && !Player.IsInTown)
            {
                return new TrinityPower(SNOPower.Wizard_ExplosiveBlast);
            }

            // Frost Nova
            if (CanCast(SNOPower.Wizard_FrostNova) && !Player.IsIncapacitated && !IsFirebirdArchonBuild &&
                ((Runes.Wizard.DeepFreeze.IsActive && TargetUtil.AnyMobsInRange(25, 5)) || TargetUtil.AnyMobsInRange(25, 1) || Player.CurrentHealthPct <= 0.7) &&
                CurrentTarget.RadiusDistance <= 25f)
            {
                return new TrinityPower(SNOPower.Wizard_FrostNova, 20f);
            }

            return null;
        }

        /// <summary>
        /// Gets the best combat power for the current conditions
        /// </summary>
        /// <returns></returns>
        private static TrinityPower GetCombatPower()
        {
            TrinityPower power = null;

            if (ShouldRefreshBastiansGeneratorBuff)
            {
                power = GetPrimaryPower();
                if (power != null)
                    return power;
            }

            // The three wizard armors, done in an else-if loop so it doesn't keep replacing one with the other            
            if (CastArmorSpell(out power)) return power;

            /*// Lets make sure we move within range if we're DMO wiz and our orbs are up
            if (IsDmoWiz && TimeSincePowerUse(SNOPower.Wizard_ArcaneOrb) <= 2000 && GetBuffStacks(SNOPower.Wizard_Passive_ArcaneDynamo) < 1)
            {
                var clusterPoint = Player.ParticipatingInTieredLootRun
                    ? Enemies.BestRiftValueCluster
                    : Enemies.BestCluster;
                MoveToOrbitPoint(clusterPoint);
            }*/

            // Offensive Teleport
            if (IsFirebirdArchonBuild)
            {
                // Use Teleport to stay close-ish to monsters so they take our AOE pulse and blast damage.
                var monstersWithinKiteRange = TargetUtil.AnyMobsInRange(CurrentTarget.CollisionRadius + KiteDistance);
                var recentlyAvoiding = Core.Avoidance.Avoider.TimeSinceLastAvoid.TotalMilliseconds < 500;
                if (Skills.Wizard.ArchonTeleport.CanCast() && (!monstersWithinKiteRange && !recentlyAvoiding || CurrentTarget.IsSafeSpot))
                {
                    Logger.Log(LogCategory.Routine, $"Casting Teleport ({Skills.Wizard.ArchonTeleport.SNOPower}) on {CurrentTarget.InternalName} {CurrentTarget.Position} Distance={CurrentTarget.Distance}");

                    // Teleport within kite distance of target to prevent porting ontop of people and then just kiting back away again.                
                    var offsetDistance = CurrentTarget.Distance - KiteDistance - CurrentTarget.CollisionRadius;
                    if (offsetDistance > 10f && offsetDistance < 120f)
                    {
                        var position = MathEx.CalculatePointFrom(CurrentTarget.Position, ZetaDia.Me.Position, offsetDistance);
                        return new TrinityPower(SNOPower.Wizard_Teleport, 50f, position);
                    }
                }
            }
            else
            {
                if (CanCastTeleport())
                {
                    Logger.Log(LogCategory.Routine, "Offensive Normal Teleport");
                    var bestClusterPoint = TargetUtil.GetBestClusterUnit(Settings.Combat.Misc.TrashPackClusterRadius);
                    // Teleporting exactly on top of targets prevents them from taking damage
                    var slightlyForwardPosition = MathEx.GetPointAt(bestClusterPoint.Position, 4f, Player.Rotation);
                    var eliteInRange = TargetUtil.BestEliteInRange(50f);
                    if (eliteInRange != null && slightlyForwardPosition.Distance(eliteInRange.Position) > 30f)
                        slightlyForwardPosition = MathEx.GetPointAt(CurrentTarget.Position, 4f, Player.Rotation);
                    if (TargetUtil.AnyBossesInRange(50f))
                        slightlyForwardPosition = MathEx.GetPointAt(CurrentTarget.Position, 4f, Player.Rotation);
                    return new TrinityPower(SNOPower.Wizard_Teleport, 65f, slightlyForwardPosition);
                }
            }

            // Defensive Teleport: SafePassage
            if (CanCastTeleport())
            {
                Logger.Log(LogCategory.Routine, "Defensive Normal Teleport");
                var targetPosition = Core.Avoidance.Avoider.SafeSpot;
                if (targetPosition != Vector3.Zero)
                {
                    // Don't teleport exactly on top of targets or it prevents them from taking damage
                    var slightlyForwardPosition = MathEx.GetPointAt(targetPosition, 4f, Player.Rotation);
                    var distance = slightlyForwardPosition.Distance(ZetaDia.Me.Position);
                    if (distance > 20f && distance < 80f)
                    {
                        return new TrinityPower(SNOPower.Wizard_Teleport, 65f, slightlyForwardPosition);
                    }
                }
            }

            // Magic Weapon (10 minutes)                 
            if (!Player.IsIncapacitated && Player.PrimaryResource >= 25 && CanCast(SNOPower.Wizard_MagicWeapon, CanCastFlags.NoTimer) && !GetHasBuff(SNOPower.Wizard_MagicWeapon))
            {
                return new TrinityPower(SNOPower.Wizard_MagicWeapon);
            }

            // Diamond Skin: Tank mode
            // Flash Wiz needs to spam Explosive Blast and Diamond Skin even if we're still far away from any mobs
            var range = (Sets.TalRashasElements.IsFullyEquipped && Legendary.WandOfWoh.IsEquipped) ? 200 : 12;
            if (CanCast(SNOPower.Wizard_DiamondSkin, CanCastFlags.NoTimer) && LastPowerUsed != SNOPower.Wizard_DiamondSkin && !GetHasBuff(SNOPower.Wizard_DiamondSkin) &&
                (TargetUtil.AnyElitesInRange(range, 1) || TargetUtil.AnyMobsInRange(range, 1) || Player.CurrentHealthPct <= 0.90 || Player.IsIncapacitated || Player.IsRooted || CurrentTarget.RadiusDistance <= 40f))
            {
                return new TrinityPower(SNOPower.Wizard_DiamondSkin);
            }

            // Explosive Blast
            if (CanCastExplosiveBlast() && (!Legendary.EtchedSigil.IsEquipped || Sets.TalRashasElements.IsFullyEquipped))
            {
                return new TrinityPower(SNOPower.Wizard_ExplosiveBlast);
            }

            // Wormhole / Black hole            
            float blackholeRadius = Runes.Wizard.Supermassive.IsActive ? 20f : 15f;
            if (CanCast(SNOPower.X1_Wizard_Wormhole, CanCastFlags.NoTimer) && !ShouldWaitForConventionElement(Skills.Wizard.BlackHole) &&
                (!(Sets.VyrsAmazingArcana.IsFullyEquipped && Sets.ChantodosResolve.IsFirstBonusActive) || GetBuffStacks(SNOPower.P3_ItemPassive_Unique_Ring_021) == 19) &&
                (TargetUtil.ClusterExists(blackholeRadius, 45f, Core.Settings.Combat.Wizard.BlackHoleAoECount) || CurrentTarget.IsElite))
            {
                // Botting with 2+ wizards, stagger blackholes.
                if (Player.IsInParty)
                {
                    var blackHoleExists = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Any(a => DataDictionary.BlackHoleIds.Contains(a.ActorSnoId));
                    if (!blackHoleExists)
                    {
                        return new TrinityPower(SNOPower.X1_Wizard_Wormhole, 65f, TargetUtil.GetBestClusterUnit(blackholeRadius, 45f, 1, false).Position);
                    }
                }
                else
                {
                    return new TrinityPower(SNOPower.X1_Wizard_Wormhole, 65f, TargetUtil.GetBestClusterUnit(blackholeRadius, 45f, 1, false).Position);
                }
            }

            // Meteor: Arcane Dynamo
            bool arcaneDynamoPassiveReady = (Passives.Wizard.ArcaneDynamo.IsActive && GetBuffStacks(SNOPower.Wizard_Passive_ArcaneDynamo) == 5);
            if (!Player.IsIncapacitated && arcaneDynamoPassiveReady && CanCast(SNOPower.Wizard_Meteor, CanCastFlags.NoTimer) && !ShouldWaitForConventionElement(Skills.Wizard.Meteor) &&
                (TargetUtil.EliteOrTrashInRange(65) || TargetUtil.ClusterExists(15f, 65, 2)) && Player.PrimaryResource >= 40)
            {
                var bestMeteorClusterUnit = TargetUtil.GetBestClusterUnit();
                return new TrinityPower(SNOPower.Wizard_Meteor, 65f, bestMeteorClusterUnit.Position);
            }

            // Slow Time for in combat
            if (!Player.IsIncapacitated && Skills.Wizard.SlowTime.CanCast(CanCastFlags.NoTimer) && TargetUtil.AnyMobsInRange(60))
            {
                var bubbles = SpellHistory.History.Where(s => s.Power.SNOPower == SNOPower.Wizard_SlowTime && s.TimeSinceUse.TotalSeconds < 12).ToList();
                var bubblePositions = new HashSet<Vector3>(bubbles.Select(b => b.TargetPosition));
                var clusterPosition = TargetUtil.GetBestClusterPoint(0f, 50f);
                var myPosition = ZetaDia.Me.Position;
                var reason = string.Empty;

                // Function to check if bubble is already in (or close enough to) a position
                var bubbleMaxRange = IsDmoWiz ? 15f : 57f;
                Func<Vector3, bool> isValidBubblePosition = pos => !bubblePositions.Any(b => b.Distance(pos) <= 14f && pos.Distance(myPosition) < bubbleMaxRange);
                var isBubbleAtPlayerValid = isValidBubblePosition(myPosition);
                var slightlyForwardPosition = MathEx.GetPointAt(myPosition, 10f, Player.Rotation);
                TrinityPower bubblePower = null;

                // Always bubble ourselves when cooldown Rune is active.
                if (Runes.Wizard.Exhaustion.IsActive && isBubbleAtPlayerValid)
                {
                    reason = "Bubble for exhaustion rune";
                    bubblePower = new TrinityPower(SNOPower.Wizard_SlowTime, bubbleMaxRange, slightlyForwardPosition);
                }

                // Defensive Bubble is Priority
                else if (Core.Clusters.Nearby.UnitCount >= 8 && (Runes.Wizard.PointOfNoReturn.IsActive || Runes.Wizard.StretchTime.IsActive) && isBubbleAtPlayerValid)
                {
                    reason = "Bubble for defense";
                    bubblePower = new TrinityPower(SNOPower.Wizard_SlowTime, bubbleMaxRange, slightlyForwardPosition);
                }

                // Then casting on elites
                else if (CurrentTarget.IsElite && CurrentTarget.Distance < bubbleMaxRange && isValidBubblePosition(CurrentTarget.Position))
                {
                    reason = "Bubble on some elites";
                    bubblePower = new TrinityPower(SNOPower.Wizard_SlowTime, bubbleMaxRange, CurrentTarget.Position);
                }

                // Then big clusters                
                else if (TargetUtil.ClusterExists(50f, 5) && clusterPosition.Distance(myPosition) < bubbleMaxRange && isValidBubblePosition(clusterPosition))
                {
                    reason = "Bubble on big cluster";
                    bubblePower = new TrinityPower(SNOPower.Wizard_SlowTime, bubbleMaxRange, clusterPosition);
                }

                // Cooldown isnt an issue with Magnum Opus, so just cast it somewhere.
                else if (Sets.DelseresMagnumOpus.IsEquipped)
                {
                    if (Core.Clusters.BestLargeCluster.Exists && Core.Clusters.BestLargeCluster.Position.Distance(myPosition) < bubbleMaxRange && isValidBubblePosition(Core.Clusters.BestLargeCluster.Position))
                    {
                        reason = "Bubble on big cluster (DMO)";
                        bubblePower = new TrinityPower(SNOPower.Wizard_SlowTime, bubbleMaxRange, Core.Clusters.BestLargeCluster.Position);
                    }

                    else if (TargetUtil.AnyMobsInRange(60) && isValidBubblePosition(clusterPosition))
                    {
                        reason = "Bubble on any cluster (DMO)";
                        bubblePower = new TrinityPower(SNOPower.Wizard_SlowTime, bubbleMaxRange, clusterPosition);
                    }
                    else if (isBubbleAtPlayerValid)
                    {
                        reason = "Bubble on self (DMO)";
                        bubblePower = new TrinityPower(SNOPower.Wizard_SlowTime, bubbleMaxRange, myPosition);
                    }
                }

                if (bubblePower != null)
                {
                    if (bubblePower.TargetPosition.Distance(myPosition) > 60f)
                    {
                        Logger.LogVerbose("An invalid Bubble position was selected too far away. SelectorReason={0}", reason);
                    }
                    else
                    {
                        return bubblePower;
                    }
                }

            }

            // Mirror Image  @ half health or 5+ monsters or rooted/incapacitated or last elite left @25% health
            if (CanCast(SNOPower.Wizard_MirrorImage, CanCastFlags.NoTimer) &&
                (Player.CurrentHealthPct <= EmergencyHealthPotionLimit || TargetUtil.AnyMobsInRange(30, 4) || Player.IsIncapacitated || Player.IsRooted ||
                TargetUtil.AnyElitesInRange(30) || CurrentTarget.IsElite))
            {
                return new TrinityPower(SNOPower.Wizard_MirrorImage);
            }

            // Hydra
            if (CanCast(SNOPower.Wizard_Hydra, CanCastFlags.NoTimer))
            {
                // ReSharper disable once InconsistentNaming
                var _14s = TimeSpan.FromSeconds(14);
                const float maxHydraDistance = 25f;
                const float castDistance = 65f;
                const float maxHydraDistSqr = maxHydraDistance * maxHydraDistance;

                // This will check if We have the "Serpent Sparker" wand, and attempt to cast a 2nd hydra immediately after the first

                bool serpentSparkerRecast1 = Legendary.SerpentsSparker.IsEquipped && LastPowerUsed == SNOPower.Wizard_Hydra &&
                    SpellHistory.SpellUseCountInTime(SNOPower.Wizard_Hydra, TimeSpan.FromSeconds(2)) < 2;

                int baseRecastDelay = HasPrimarySkill || Player.PrimaryResource < 60 ? 14 : 3;
                bool baseRecast = TimeSpanSincePowerUse(SNOPower.Wizard_Hydra) > TimeSpan.FromSeconds(baseRecastDelay);
                var lastCast = SpellHistory.History
                    .Where(p => p.Power.SNOPower == SNOPower.Wizard_Hydra && p.TimeSinceUse < _14s)
                    .OrderBy(s => s.TimeSinceUse).ThenBy(p => p.Power.TargetPosition.Distance2DSqr(CurrentTarget.Position))
                    .FirstOrDefault();

                bool distanceRecast = lastCast != null && lastCast.TargetPosition.Distance2DSqr(CurrentTarget.Position) > maxHydraDistSqr;

                bool twoAlredyCastIn5Sec = SpellHistory.SpellUseCountInTime(SNOPower.Wizard_Hydra, TimeSpan.FromSeconds(5)) >= 2;

                if (!Player.IsIncapacitated && CanCast(SNOPower.Wizard_Hydra, CanCastFlags.NoTimer) &&
                    !ShouldWaitForConventionElement(Skills.Wizard.Hydra) &&
                    (baseRecast || distanceRecast || serpentSparkerRecast1) && !twoAlredyCastIn5Sec &&
                    (!Legendary.EtchedSigil.IsEquipped || Sets.TalRashasElements.IsFullyEquipped) &&
                    CurrentTarget.RadiusDistance <= castDistance && Player.PrimaryResource >= 15)
                {
                    var pos = TargetUtil.GetBestClusterPoint(maxHydraDistance);
                    return new TrinityPower(SNOPower.Wizard_Hydra, 55f, pos);
                }

            }

            // Archon
            if (CanCast(SNOPower.Wizard_Archon, CanCastFlags.NoTimer) && ShouldStartArchon())
            {
                return new TrinityPower(SNOPower.Wizard_Archon);
            }

            // Blizzard
            float blizzardRadius = Runes.Wizard.Apocalypse.IsActive ? 30f : 12f;
            if (!Player.IsIncapacitated && CanCast(SNOPower.Wizard_Blizzard, CanCastFlags.NoTimer) && !ShouldWaitForConventionElement(Skills.Wizard.Blizzard) &&
                (TargetUtil.ClusterExists(blizzardRadius, 90f, 2, false) || CurrentTarget.IsElite || !HasPrimarySkill) &&
                (Player.PrimaryResource >= 40 || (Runes.Wizard.Snowbound.IsActive && Player.PrimaryResource >= 20)) &&
                (!Legendary.EtchedSigil.IsEquipped || Sets.TalRashasElements.IsFullyEquipped))
            {
                var bestClusterPoint = TargetUtil.GetBestClusterPoint(blizzardRadius, 65f, false);
                return new TrinityPower(SNOPower.Wizard_Blizzard, 65f, bestClusterPoint);
            }

            // Meteor - no arcane dynamo
            if (!Player.IsIncapacitated && !Passives.Wizard.ArcaneDynamo.IsActive && CanCast(SNOPower.Wizard_Meteor, CanCastFlags.NoTimer) &&
                !ShouldWaitForConventionElement(Skills.Wizard.Meteor) && (!Legendary.EtchedSigil.IsEquipped || Sets.TalRashasElements.IsFullyEquipped) &&
                (TargetUtil.EliteOrTrashInRange(65) || TargetUtil.ClusterExists(15f, 65, 2)) && Player.PrimaryResource >= 40)
            {
                return new TrinityPower(SNOPower.Wizard_Meteor, 65f, TargetUtil.GetBestClusterPoint());
            }

            // Frost Nova
            if (!Legendary.HaloOfArlyse.IsEquipped && !Sets.VyrsAmazingArcana.IsFullyEquipped)
            {
                if (CanCast(SNOPower.Wizard_FrostNova) && !Player.IsIncapacitated &&
                    !ShouldWaitForConventionElement(Skills.Wizard.FrostNova) &&
                    ((Runes.Wizard.DeepFreeze.IsActive && TargetUtil.AnyMobsInRange(25, 5)) ||
                     (!Runes.Wizard.DeepFreeze.IsActive &&
                      (TargetUtil.AnyMobsInRange(25, 1) || Player.CurrentHealthPct <= 0.7)) &&
                     CurrentTarget.RadiusDistance <= 25f))
                {
                    return new TrinityPower(SNOPower.Wizard_FrostNova, 20f);
                }
            }

            // Check to see if we have a signature spell on our hotbar, for energy twister check
            bool hasSignatureSpell = (Hotbar.Contains(SNOPower.Wizard_MagicMissile) || Hotbar.Contains(SNOPower.Wizard_ShockPulse) ||
                Hotbar.Contains(SNOPower.Wizard_SpectralBlade) || Hotbar.Contains(SNOPower.Wizard_Electrocute));

            // Energy Twister SPAMS whenever 35 or more ap to generate Arcane Power
            if (!Player.IsIncapacitated && CanCast(SNOPower.Wizard_EnergyTwister) && !ShouldWaitForConventionElement(Skills.Wizard.EnergyTwister) &&
                Player.PrimaryResource >= 25 && !Settings.Combat.Wizard.NoEnergyTwister &&
                // If using storm chaser, then force a signature spell every 1 stack of the buff, if we have a signature spell
                (!hasSignatureSpell || GetBuffStacks(SNOPower.Wizard_EnergyTwister) < 9))
            {
                Vector3 bestClusterPoint = TargetUtil.GetBestClusterPoint();

                const float twisterRange = 50f;
                return new TrinityPower(SNOPower.Wizard_EnergyTwister, twisterRange, bestClusterPoint);
            }

            // Wave of force
            if (!Player.IsIncapacitated && Player.PrimaryResource >= 25 && CanCast(SNOPower.Wizard_WaveOfForce, CanCastFlags.NoTimer) &&
                !ShouldWaitForConventionElement(Skills.Wizard.WaveOfForce))
            {
                return new TrinityPower(SNOPower.Wizard_WaveOfForce, 15f, CurrentTarget.Position);
            }

            // Disintegrate with Firebird's
            if (Sets.FirebirdsFinery.IsFullyEquipped && Skills.Wizard.Disintegrate.IsActive &&
                Skills.Wizard.Disintegrate.CanCast() && !IsCurrentlyAvoiding)
            {
                if (ShouldSpreadBurn())
                    ChangeTarget();
            }

            // Disintegrate
            if (!Player.IsIncapacitated && CanCast(SNOPower.Wizard_Disintegrate) &&
                Player.PrimaryResource >= 30)
            {
                var disintegrateRange = Runes.Wizard.Entropy.IsActive ? 10f : 35f;
                return new TrinityPower(SNOPower.Wizard_Disintegrate, disintegrateRange, Vector3.Zero, -1, CurrentTarget.AcdId, 0, 0);
            }

            // Arcane Orb
            if (CanCastArcaneOrb())
            {
                return Runes.Wizard.ArcaneOrbit.IsActive ?
                    new TrinityPower(SNOPower.Wizard_ArcaneOrb) :
                    new TrinityPower(SNOPower.Wizard_ArcaneOrb, 35f, CurrentTarget.AcdId);
            }

            // Arcane Torrent
            if (!Player.IsIncapacitated && CanCast(SNOPower.Wizard_ArcaneTorrent) && !ShouldWaitForConventionElement(Skills.Wizard.ArcaneTorrent) &&
                (Player.PrimaryResource >= 20 || Legendary.HergbrashsBinding.IsEquipped && Player.PrimaryResource >= 16))
            {
                return new TrinityPower(SNOPower.Wizard_ArcaneTorrent, 40f, CurrentTarget.AcdId);
            }

            // Ray of Frost
            if (!Player.IsIncapacitated && CanCast(SNOPower.Wizard_RayOfFrost) && !ShouldWaitForConventionElement(Skills.Wizard.RayOfFrost))
            {
                float rayRange = 50f;
                if (Runes.Wizard.SleetStorm.IsActive)
                    rayRange = 5f;

                return new TrinityPower(SNOPower.Wizard_RayOfFrost, rayRange, CurrentTarget.AcdId);
            }

            power = GetPrimaryPower();
            if (power != null)
                return power;

            // Default Attacks
            if (IsNull(power))
            {
                // Decide wheter to use melee or ranged attacks
                var isMeleeWiz = (Sets.TalRashasElements.IsFullyEquipped && Legendary.WandOfWoh.IsEquipped ||
                                  Sets.DelseresMagnumOpus.IsFullyEquipped && Skills.Wizard.ArcaneOrb.IsActive ||
                                  Sets.VyrsAmazingArcana.IsFullyEquipped && Sets.ChantodosResolve.IsFullyEquipped);
                if (!isMeleeWiz && CurrentTarget != null)
                    power = DefaultPower.MinimumRange > 11f ? DefaultPower : new TrinityPower(SNOPower.Walk);

                else if (isMeleeWiz)
                    power = new TrinityPower(SNOPower.Walk, CurrentTarget.RadiusDistance, CurrentTarget.Position);
            }
            return power;
        }

        private static TrinityPower GetPrimaryPower()
        {
            // Magic Missile
            if (CanCast(SNOPower.Wizard_MagicMissile))
            {
                var bestPierceTarget = TargetUtil.GetBestPierceTarget(45f);
                int targetId;

                if (bestPierceTarget != null)
                    targetId = Runes.Wizard.Conflagrate.IsActive ?
                        bestPierceTarget.AcdId :
                        CurrentTarget.AcdId;
                else
                    targetId = CurrentTarget.AcdId;

                return new TrinityPower(SNOPower.Wizard_MagicMissile, 45f, targetId);
            }

            var shockPulseRange = Runes.Wizard.PiercingOrb.IsActive ? 50f : 10f;
            // Shock Pulse
            if (CanCast(SNOPower.Wizard_ShockPulse))
            {
                return new TrinityPower(SNOPower.Wizard_ShockPulse, shockPulseRange, CurrentTarget.AcdId);
            }
            // Spectral Blade
            if (CanCast(SNOPower.Wizard_SpectralBlade))
            {
                var bladeTarget = IsDmoWiz ? TargetUtil.GetClosestUnit() : CurrentTarget;
                var bladeRange = Runes.Wizard.ArcaneOrbit.IsActive ? 4f : 15f;
                if (bladeTarget != null)
                    return new TrinityPower(SNOPower.Wizard_SpectralBlade, bladeRange, bladeTarget.AcdId);
            }

            var isFlashWiz = Sets.TalRashasElements.IsFullyEquipped && Legendary.WandOfWoh.IsEquipped;
            // Electrocute
            if (CanCast(SNOPower.Wizard_Electrocute))
            {
                if (isFlashWiz && TimeSincePowerUse(SNOPower.Wizard_Electrocute) >= 4000)
                    return new TrinityPower(SNOPower.Wizard_Electrocute, 40f, CurrentTarget.AcdId);

                if (!isFlashWiz)
                    return new TrinityPower(SNOPower.Wizard_Electrocute, 40f, CurrentTarget.AcdId);
            }

            return null;
        }

        /// <summary>
        /// Checks and casts buffs if needed
        /// </summary>
        /// <returns></returns>
        private static TrinityPower GetBuffPower()
        {
            TrinityPower buffPower;

            // Illusionist speed boost
            if (Passives.Wizard.Illusionist.IsActive)
            {
                // Slow Time on self for speed boost if we don't have Teleport
                if (CanCast(SNOPower.Wizard_SlowTime) && !Skills.Wizard.Teleport.IsActive)
                {
                    Logger.LogVerbose("Casting SlowTime as Buff (GetBuffPower)");
                    return new TrinityPower(SNOPower.Wizard_SlowTime, 100f, Core.Player.Position);
                }

                // Mirror Image for speed boost
                if (CanCast(SNOPower.Wizard_MirrorImage))
                    return new TrinityPower(SNOPower.Wizard_MirrorImage);

                // Teleport already called from PlayerMover, not here (since it's a "movement" spell, not a buff)
            }
            // Magic Weapon (10 minutes)                 
            if (!Player.IsIncapacitated && Player.PrimaryResource >= 25 && CanCast(SNOPower.Wizard_MagicWeapon, CanCastFlags.NoTimer) && !GetHasBuff(SNOPower.Wizard_MagicWeapon))
            {
                return new TrinityPower(SNOPower.Wizard_MagicWeapon);
            }
            // Diamond Skin off CD
            if ((Runes.Wizard.SleekShell.IsActive ||
                Sets.TalRashasElements.IsFullyEquipped && Legendary.WandOfWoh.IsEquipped && !Player.IsInTown) && CanCast(SNOPower.Wizard_DiamondSkin, CanCastFlags.NoTimer))
            {
                return new TrinityPower(SNOPower.Wizard_DiamondSkin);
            }
            // Familiar
            if (!Player.IsIncapacitated && CanCast(SNOPower.Wizard_Familiar) && Player.PrimaryResource >= 20 && !IsFamiliarActive)
            {
                return new TrinityPower(SNOPower.Wizard_Familiar);
            }

            // The three wizard armors, done in an else-if loop so it doesn't keep replacing one with the other            
            if (CastArmorSpell(out buffPower)) return buffPower;

            // Mirror Image  @ half health or 5+ monsters or rooted/incapacitated or last elite left @25% health
            if (CanCast(SNOPower.Wizard_MirrorImage, CanCastFlags.NoTimer) &&
                (Player.CurrentHealthPct <= EmergencyHealthPotionLimit || TargetUtil.AnyMobsInRange(30, 4) || Player.IsIncapacitated || Player.IsRooted))
            {
                return new TrinityPower(SNOPower.Wizard_MirrorImage);
            }

            // Disintegrate to keep Taeguk's buff up
            if (CanCast(SNOPower.Wizard_Disintegrate) && Gems.Taeguk.IsEquipped && !Player.IsInTown &&
                Skills.Wizard.Disintegrate.TimeSinceUse > 2300 && Skills.Wizard.Disintegrate.TimeSinceUse <= 2700)
            {
                return new TrinityPower(SNOPower.Wizard_Disintegrate, 60f, Player.Position);
            }

            // Explosive Blast for the Tal Rasha's speed builds
            if (CanCast(SNOPower.Wizard_ExplosiveBlast, CanCastFlags.NoTimer) && Player.PrimaryResource >= 20 && !Player.IsInTown &&
                !Player.IsIncapacitated && Legendary.WandOfWoh.IsEquipped && Sets.TalRashasElements.IsFullyEquipped)
            {
                return new TrinityPower(SNOPower.Wizard_ExplosiveBlast);
            }

            // Frost Nova - Frozen Mist for the Tal Rasha's speed builds
            if (Runes.Wizard.FrozenMist.IsActive && CanCast(SNOPower.Wizard_FrostNova) && !Player.IsInTown &&
                !Player.IsIncapacitated && Legendary.WandOfWoh.IsEquipped && Sets.TalRashasElements.IsFullyEquipped)
            {
                return new TrinityPower(SNOPower.Wizard_FrostNova);
            }
            return null;
        }

        private static bool CastArmorSpell(out TrinityPower buffPower)
        {
            if (!Player.IsIncapacitated && Player.PrimaryResource >= 25)
            {
                // Energy armor as priority cast if available and not buffed
                if (Hotbar.Contains(SNOPower.Wizard_EnergyArmor))
                {
                    if ((!GetHasBuff(SNOPower.Wizard_EnergyArmor) && CanCast(SNOPower.Wizard_EnergyArmor, CanCastFlags.NoTimer)) ||
                        (Hotbar.Contains(SNOPower.Wizard_Archon) && !GetHasBuff(SNOPower.Wizard_EnergyArmor)))
                    {
                        {
                            buffPower = new TrinityPower(SNOPower.Wizard_EnergyArmor);
                            return true;
                        }
                    }
                }
                // Ice Armor
                else if (Hotbar.Contains(SNOPower.Wizard_IceArmor))
                {
                    if (!GetHasBuff(SNOPower.Wizard_IceArmor) && CanCast(SNOPower.Wizard_IceArmor, CanCastFlags.NoTimer))
                    {
                        {
                            buffPower = new TrinityPower(SNOPower.Wizard_IceArmor);
                            return true;
                        }
                    }
                }
                // Storm Armor
                else if (Hotbar.Contains(SNOPower.Wizard_StormArmor))
                {
                    if (!GetHasBuff(SNOPower.Wizard_StormArmor) && CanCast(SNOPower.Wizard_StormArmor, CanCastFlags.NoTimer))
                    {
                        {
                            buffPower = new TrinityPower(SNOPower.Wizard_StormArmor);
                            return true;
                        }
                    }
                }
            }
            buffPower = null;
            return false;
        }

        public static bool IsSlowTimeActive()
        {
            return ZetaDia.Actors.GetActorsOfType<DiaObject>().Any(a => a.ActorSnoId == 5422 || a.ActorSnoId == 5423 || a.ActorSnoId == 112585);
        }

        internal static bool ShouldSpreadBurn()
        {
            return CurrentTarget != null && Skills.Wizard.Disintegrate.IsActive &&

            // Current target is valid
            CurrentTarget.IsUnit && !CurrentTarget.IsTreasureGoblin &&
            CurrentTarget.Type != TrinityObjectType.Shrine &&
            CurrentTarget.Type != TrinityObjectType.Interactable &&
            CurrentTarget.Type != TrinityObjectType.HealthWell &&
            CurrentTarget.Type != TrinityObjectType.Door &&
            CurrentTarget.TrinityItemType != TrinityItemType.HealthGlobe &&
            CurrentTarget.TrinityItemType != TrinityItemType.ProgressionGlobe &&

            // Avoid rapidly changing targets
            DateTime.UtcNow.Subtract(_lastTargetChange).TotalMilliseconds > 200 &&

            // We have something to spread to
            TargetUtil.AnyMobsInRange(60f) && TargetUtil.IsUnitWithoutDebuffWithinRange(60f, SNOPower.ItemPassive_Unique_Ring_733_x1) &&

            // Stop spreading when everything is on fire
            TargetUtil.PercentOfMobsDebuffed(SNOPower.ItemPassive_Unique_Ring_733_x1, 60f) < 1;
        }

        internal static void ChangeTarget()
        {
            _lastTargetChange = DateTime.UtcNow;

            var currentTarget = CurrentTarget;
            var lowestHealthTarget = TargetUtil.LowestHealthTarget(60f, Core.Player.Position, SNOPower.Wizard_Disintegrate);

            //Logger.LogNormal("Blacklisting {0} {1} - Changing Target", CurrentTarget.InternalName, CurrentTarget.CommonData.AcdId);
            Trinity.TrinityPlugin.Blacklist3Seconds.Add(CurrentTarget.AnnId);

            // Would like the new target to be different than the one we just blacklisted, or be very close to dead.
            if (lowestHealthTarget.AcdId == currentTarget.AcdId && lowestHealthTarget.HitPointsPct < 0.2) return;

            Trinity.TrinityPlugin.CurrentTarget = lowestHealthTarget;
            //Logger.LogNormal("Found lowest health target {0} {1} ({2:0.##}%)", CurrentTarget.InternalName, CurrentTarget.CommonData.AcdId, lowestHealthTarget.HitPointsPct * 100);
        }

        /// <summary>
        /// Gets the best Archon power for the current conditions
        /// </summary>
        /// <returns></returns>
        private static bool TryGetArchonPower(out TrinityPower power)
        {
            power = default(TrinityPower);

            if (CurrentTarget == null)
                return false;

            if (Settings.Combat.Wizard.AlwaysArchon && Skills.Wizard.Archon.CanCast() && Sets.ChantodosResolve.IsFullyEquipped)
            {
                if (Core.Buffs.HasBuff(SNOPower.P3_ItemPassive_Unique_Ring_021) && GetBuffStacks(SNOPower.P3_ItemPassive_Unique_Ring_021) > 19)
                {
                    Skills.Wizard.Archon.Cast();
                }
            }

            if (!Skills.Wizard.IsArchonActive())
                return false;

            if (IsCurrentlyAvoiding)
            {
                if (CurrentTarget.IsSafeSpot && CurrentTarget.Distance > 20f && Skills.Wizard.ArchonTeleport.CanCast())
                {
                    Logger.LogVerbose($"Avoidance Teleport Distance={CurrentTarget.Distance}");
                    power = new TrinityPower(Skills.Wizard.ArchonTeleport.SNOPower, 50f, CurrentTarget.Position);
                    return true;
                }
                return false;
            }

            if (!Skills.Wizard.IsArchonActive() && Settings.Combat.Wizard.FindClustersWhenNotArchon)
            {
                if (Settings.Combat.Wizard.FindClustersWhenNotArchon)
                {


                    // Double trash range up to max of 10 for 8 seconds or until archon starts or there is a big cluster in range.
                    CombatOverrides.ModifyTrashSizeForDuration(2d, TimeSpan.FromSeconds(8), 4, 10, () => BigClusterOrElitesInRange() || Skills.Wizard.IsArchonActive());
                }
                return false;
            }

            // Use Teleport to stay close-ish to monsters so they take our AOE pulse and blast damage.
            var monstersWithinKiteRange = TargetUtil.AnyMobsInRange(CurrentTarget.CollisionRadius + KiteDistance);
            var recentlyAvoiding = Core.Avoidance.Avoider.TimeSinceLastAvoid.TotalMilliseconds < 500;
            if (Skills.Wizard.ArchonTeleport.CanCast() && (!monstersWithinKiteRange && !recentlyAvoiding || CurrentTarget.IsSafeSpot))
            {
                Logger.Log(LogCategory.Routine, $"Casting ArchonTeleport ({Skills.Wizard.ArchonTeleport.SNOPower}) on {CurrentTarget.InternalName} {CurrentTarget.Position} Distance={CurrentTarget.Distance}");

                // Teleport within kite distance of target to prevent porting ontop of people and then just kiting back away again.                
                var offsetDistance = CurrentTarget.Distance - KiteDistance - CurrentTarget.CollisionRadius;
                if (offsetDistance > 15f)
                {
                    var position = MathEx.CalculatePointFrom(CurrentTarget.Position, ZetaDia.Me.Position, offsetDistance);
                    power = new TrinityPower(Skills.Wizard.ArchonTeleport.SNOPower, 50f, position);
                    return true;
                }
            }

            //392694, 392695, 392696 == Arcane Strike,
            //392697, 392699, 392698 == Disintegration Wave
            //392692, 392693, 392691 == Arcane Blast, Ice Blast 

            //SNOPower
            //    beamPower = SNOPower.Wizard_Archon_ArcaneBlast,
            //    strikePower = SNOPower.Wizard_Archon_ArcaneStrike,
            //    blastPower = SNOPower.Wizard_Archon_DisintegrationWave;

            var beam = Core.Hotbar.ActiveSkills
                .FirstOrDefault(p => p.Power == SNOPower.Wizard_Archon_DisintegrationWave ||
                    p.Power == SNOPower.Wizard_Archon_DisintegrationWave_Cold ||
                    p.Power == SNOPower.Wizard_Archon_DisintegrationWave_Fire ||
                    p.Power == SNOPower.Wizard_Archon_DisintegrationWave_Lightning);

            var strike = Core.Hotbar.ActiveSkills
                .FirstOrDefault(p => p.Power == SNOPower.Wizard_Archon_ArcaneStrike ||
                    p.Power == SNOPower.Wizard_Archon_ArcaneStrike_Fire ||
                    p.Power == SNOPower.Wizard_Archon_ArcaneStrike_Cold ||
                    p.Power == SNOPower.Wizard_Archon_ArcaneStrike_Lightning);

            var blast = Core.Hotbar.ActiveSkills
                .FirstOrDefault(p => p.Power == SNOPower.Wizard_Archon_ArcaneBlast ||
                    p.Power == SNOPower.Wizard_Archon_ArcaneBlast_Cold ||
                    p.Power == SNOPower.Wizard_Archon_ArcaneBlast_Fire ||
                    p.Power == SNOPower.Wizard_Archon_ArcaneBlast_Lightning);

            // Cast Strike if densely surrounded
            if (!Settings.Combat.Wizard.NoArcaneStrike && CurrentTarget.Distance < 12f && TargetUtil.NumMobsInRange(12f) > 6 && strike != null && PowerManager.CanCast(strike.Power))
            {
                power = new TrinityPower(strike.Power, 12f, CurrentTarget.Position);
                return true;
            }

            // Arcane Blast
            if (!Settings.Combat.Wizard.NoArcaneBlast && TargetUtil.AnyMobsInRange(15f) && beam != null && PowerManager.CanCast(blast.Power))
            {
                Logger.Log(LogCategory.Routine, $"Casting ArchonBlast ({Skills.Wizard.ArchonBlast.SNOPower}) on {CurrentTarget.InternalName} {CurrentTarget.Position} Distance={CurrentTarget.Distance}");
                power = new TrinityPower(blast.Power, 50f, CurrentTarget.Position);
                return true;
            }

            // Disintegration
            if (!Settings.Combat.Wizard.DisableDisintegrationWave && CurrentTarget.IsPlayerFacing(60) && TargetUtil.AnyMobsInRange(40f) && beam != null && PowerManager.CanCast(beam.Power))
            {
                Logger.Log(LogCategory.Routine, $"Casting ArchonDisintegrationWave {Skills.Wizard.ArchonDisintegrationWave.SNOPower} on {CurrentTarget.InternalName} {CurrentTarget.Position} Distance={CurrentTarget.Distance}");
                var position = MathEx.CalculatePointFrom(CurrentTarget.Position, ZetaDia.Me.Position, 45f);
                power = new TrinityPower(Skills.Wizard.ArchonDisintegrationWave.SNOPower, 45f, position);
                return true;
            }

            // ArchonStrike
            if (!Settings.Combat.Wizard.NoArcaneStrike && TargetUtil.AnyMobsInRange(12f) && PowerManager.CanCast(strike.Power))
            {
                Logger.Log(LogCategory.Routine, $"Casting ArchonStrike {Skills.Wizard.ArchonStrike.SNOPower} on {CurrentTarget.InternalName} {CurrentTarget.Position} Distance={CurrentTarget.Distance}");
                power = new TrinityPower(strike.Power, 14f, CurrentTarget.Position);
                return true;
            }

            // Teleport if nothing nearby
            if (!TargetUtil.AnyMobsInRange(60f) && Skills.Wizard.ArchonTeleport.CanCast() && CurrentTarget.Distance > 25f)
            {
                power = new TrinityPower(Skills.Wizard.ArchonTeleport.SNOPower, 50f, CurrentTarget.Position);
                return true;
            }

            // Disintegration last resort
            if (!Settings.Combat.Wizard.DisableDisintegrationWave && CurrentTarget.IsPlayerFacing(60) && beam != null && PowerManager.CanCast(beam.Power))
            {
                var position = MathEx.CalculatePointFrom(CurrentTarget.Position, ZetaDia.Me.Position, 45f);
                power = new TrinityPower(beam.Power, 45f, position);
                return true;
            }

            // ArchonStrike last resort 
            if (!Settings.Combat.Wizard.NoArcaneStrike && strike != null && PowerManager.CanCast(strike.Power))
            {
                Logger.Log(LogCategory.Routine, $"Casting ArchonStrike {Skills.Wizard.ArchonStrike.SNOPower} on {CurrentTarget.InternalName} {CurrentTarget.Position} Distance={CurrentTarget.Distance}");
                power = new TrinityPower(strike.Power, 14f, CurrentTarget.Position);
                return true;
            }

            return false;

            //if (!Player.IsIncapacitated && 
            //      CanCast(SNOPower.Wizard_Archon_SlowTime, CanCastFlags.NoTimer) &&
            //     !IsSlowTimeActive())
            //{
            //    return new TrinityPower(SNOPower.Wizard_Archon_SlowTime);
            //}

            //// Archon Teleport in combat for kiting
            //if (!Player.IsIncapacitated && CanCast(SNOPower.Wizard_Archon_Teleport, CanCastFlags.NoTimer) &&
            //    Player.CurrentHealthPct <= 0.40 &&
            //    Settings.Combat.Wizard.KiteLimit > 0 &&
            //    TimeSincePowerUse(SNOPower.Wizard_Teleport) >= Settings.Combat.Wizard.TeleportDelay &&
            //    // Try and teleport-retreat from 1 elite or 3+ greys or a boss at 15 foot range
            //    (TargetUtil.AnyElitesInRange(15, 1) || TargetUtil.AnyMobsInRange(15, 3) || (CurrentTarget.IsBoss && CurrentTarget.RadiusDistance <= 15f)))
            //{
            //    Vector3 vNewTarget = MathEx.CalculatePointFrom(CurrentTarget.Position, Player.Position, -20f);
            //    return new TrinityPower(SNOPower.Wizard_Archon_Teleport, 35f, vNewTarget);
            //}

            //// Archon teleport in combat for no-kite
            //if (!Player.IsIncapacitated && CanCast(SNOPower.Wizard_Archon_Teleport, CanCastFlags.NoTimer) &&
            //    Settings.Combat.Wizard.KiteLimit == 0 && CurrentTarget.RadiusDistance >= 10f &&
            //    TimeSincePowerUse(SNOPower.Wizard_Teleport) >= Settings.Combat.Wizard.TeleportDelay)
            //{
            //    return new TrinityPower(SNOPower.Wizard_Archon_Teleport, 35f, CurrentTarget.Position);
            //}

            //// 2.0.5 Archon elemental runes
            //// This needs some checking on range i think

            ////392694, 392695, 392696 == Arcane Strike,
            ////392697, 392699, 392698 == Disintegration Wave
            ////392692, 392693, 392691 == Arcane Blast, Ice Blast 

            //SNOPower
            //    beamPower = SNOPower.Wizard_Archon_ArcaneBlast,
            //    strikePower = SNOPower.Wizard_Archon_ArcaneStrike,
            //    blastPower = SNOPower.Wizard_Archon_DisintegrationWave;

            //var beamSkill = Core.Hotbar.ActiveSkills
            //    .FirstOrDefault(p => p.Power == SNOPower.Wizard_Archon_DisintegrationWave ||
            //        p.Power == SNOPower.Wizard_Archon_DisintegrationWave_Cold ||
            //        p.Power == SNOPower.Wizard_Archon_DisintegrationWave_Fire ||
            //        p.Power == SNOPower.Wizard_Archon_DisintegrationWave_Lightning);

            //var strikeSkill = Core.Hotbar.ActiveSkills
            //    .FirstOrDefault(p => p.Power == SNOPower.Wizard_Archon_ArcaneStrike ||
            //        p.Power == SNOPower.Wizard_Archon_ArcaneStrike_Fire ||
            //        p.Power == SNOPower.Wizard_Archon_ArcaneStrike_Cold ||
            //        p.Power == SNOPower.Wizard_Archon_ArcaneStrike_Lightning);

            //var blastSkill = Core.Hotbar.ActiveSkills
            //    .FirstOrDefault(p => p.Power == SNOPower.Wizard_Archon_ArcaneBlast ||
            //        p.Power == SNOPower.Wizard_Archon_ArcaneBlast_Cold ||
            //        p.Power == SNOPower.Wizard_Archon_ArcaneBlast_Fire ||
            //        p.Power == SNOPower.Wizard_Archon_ArcaneBlast_Lightning);

            //if (beamSkill != null && beamSkill.Power != default(SNOPower))
            //    beamPower = beamSkill.Power;

            //if (strikeSkill != null && strikeSkill.Power != default(SNOPower))
            //    strikePower = strikeSkill.Power;

            //if (blastSkill != null && blastSkill.Power != default(SNOPower))
            //    blastPower = blastSkill.Power;

            //// Arcane Blast - 2 second cooldown, big AoE
            //if (!Player.IsIncapacitated && CanCast(blastPower, CanCastFlags.NoTimer) && !Settings.Combat.Wizard.NoArcaneBlast)
            //{
            //    return new TrinityPower(blastPower, 10f, CurrentTarget.Position);
            //}

            //// Disintegrate
            //if (!Player.IsIncapacitated && !Settings.Combat.Wizard.DisableDisintegrationWave && CanCast(beamPower, CanCastFlags.NoTimer) &&
            //    (CurrentTarget.CountUnitsBehind(25f) > 2 || Settings.Combat.Wizard.NoArcaneStrike || Settings.Combat.Wizard.KiteLimit > 0))
            //{
            //    return new TrinityPower(beamPower, 49f, CurrentTarget.AcdId);
            //}

            //// Arcane Strike Rapid Spam at close-range only, and no AoE inbetween us and target
            //if (!Player.IsIncapacitated && !Settings.Combat.Wizard.NoArcaneStrike && CanCast(strikePower, CanCastFlags.NoTimer) &&
            //    !CacheData.TimeBoundAvoidance.Any(aoe => MathUtil.IntersectsPath(aoe.Position, aoe.Radius, Player.Position, CurrentTarget.Position)))
            //{
            //    return new TrinityPower(strikePower, 7f, CurrentTarget.AcdId);
            //}

            //// Disintegrate as final option just in case
            //if (!Player.IsIncapacitated && CanCast(beamPower, CanCastFlags.NoTimer))
            //{
            //    return new TrinityPower(beamPower, 49f, CurrentTarget.AcdId);
            //}

            //return null;
        }

        /// <summary>
        /// Gets the best Wizard object destruction power
        /// </summary>
        private static TrinityPower DestroyObjectPower()
        {
            if (CanCast(SNOPower.Wizard_WaveOfForce) && Player.PrimaryResource >= 25)
                return new TrinityPower(SNOPower.Wizard_WaveOfForce, 9f);

            if (CanCast(SNOPower.Wizard_EnergyTwister) && Player.PrimaryResource >= 35)
                return new TrinityPower(SNOPower.Wizard_EnergyTwister, 9f);

            if (CanCast(SNOPower.Wizard_ArcaneOrb) && Player.PrimaryResource >= 35)
                return Runes.Wizard.ArcaneOrbit.IsActive ?
                    new TrinityPower(SNOPower.Wizard_ArcaneOrb) :
                    new TrinityPower(SNOPower.Wizard_ArcaneOrb, 35f);

            if (CanCast(SNOPower.Wizard_MagicMissile))
                return new TrinityPower(SNOPower.Wizard_MagicMissile, 15f);

            if (CanCast(SNOPower.Wizard_ShockPulse))
                return new TrinityPower(SNOPower.Wizard_ShockPulse, 10f);

            if (CanCast(SNOPower.Wizard_SpectralBlade))
                return new TrinityPower(SNOPower.Wizard_SpectralBlade, 5f);

            if (CanCast(SNOPower.Wizard_Electrocute))
                return new TrinityPower(SNOPower.Wizard_Electrocute, 9f);

            //if (CanCast(SNOPower.Wizard_ArcaneTorrent) && Player.PrimaryResource > GetAdjustedCost(16))
            //    return new TrinityPower(SNOPower.Wizard_ArcaneTorrent, 9f);

            if (CanCast(SNOPower.Wizard_Blizzard))
                return new TrinityPower(SNOPower.Wizard_Blizzard, 9f);

            return DefaultPower;
        }

        /// <summary>
        /// Checks for all necessary buffs and combat conditions for starting Archon
        /// </summary>
        /// <returns></returns>
        private static bool ShouldStartArchon()
        {
            bool canCastArchon = (
                 CheckAbilityAndBuff(SNOPower.Wizard_MagicWeapon) &&
                 (!Hotbar.Contains(SNOPower.Wizard_Familiar) || IsFamiliarActive) &&
                 CheckAbilityAndBuff(SNOPower.Wizard_EnergyArmor) &&
                 CheckAbilityAndBuff(SNOPower.Wizard_IceArmor) &&
                 CheckAbilityAndBuff(SNOPower.Wizard_StormArmor)
             );

            var elitesOnly = Settings.Combat.Wizard.ArchonElitesOnly && TargetUtil.AnyElitesInRange(Settings.Combat.Wizard.ArchonEliteDistance);
            var trashInRange = !Settings.Combat.Wizard.ArchonElitesOnly && TargetUtil.AnyMobsInRange(Settings.Combat.Wizard.ArchonMobDistance, Settings.Combat.Wizard.ArchonMobCount);

            // With Chantodos set wait until max stacks before using archon
            if (Sets.ChantodosResolve.IsFullyEquipped && Core.Buffs.HasBuff(SNOPower.P3_ItemPassive_Unique_Ring_021) && GetBuffStacks(SNOPower.P3_ItemPassive_Unique_Ring_021) > 19)
                return true;

            return canCastArchon && (elitesOnly || trashInRange || CurrentTarget.IsBoss);
        }

        private static readonly Func<TargetArea, bool> ArcaneOrbitCriteria = area =>
            //at least 1 Elites or the set number of trash mobs
            (area.EliteCount > 0 || area.UnitCount >= Settings.Combat.Misc.TrashPackSize);


        /// <summary>
        /// Forces bot to move really close to enemies
        /// </summary>
        private static void MoveToOrbitPoint(TargetArea area)
        {
            CombatMovement.Queue(new CombatMovement
            {
                Name = "Arcane Orbit Position",
                Destination = area.Position,
                OnUpdate = m =>
                {
                    if (CanCastTeleport() &&
                    ArcaneOrbitCriteria(Core.Clusters.BestRiftValueCluster))
                        Skills.Wizard.Teleport.Cast(m.Destination);
                },
                Options = new CombatMovementOptions
                {
                    AcceptableDistance = 6f,
                    Logging = LogLevel.Verbose,
                    ChangeInDistanceLimit = 1f,
                    SuccessBlacklistSeconds = 3,
                    FailureBlacklistSeconds = 7,
                    TimeBeforeBlocked = 500
                }

            });
        }

        /// <summary>
        /// Returns true if we have wizard armor in our hotbar and if the buff is active
        /// </summary>
        private static bool IsWizardArmorActive
        {
            get { return (GetHasBuff(SNOPower.Wizard_EnergyArmor) || GetHasBuff(SNOPower.Wizard_IceArmor) || GetHasBuff(SNOPower.Wizard_StormArmor)); }
        }

        /// <summary>
        /// Returns true if Familiar buff is active
        /// </summary>
        private static bool IsFamiliarActive
        {
            get
            {
                double timeSinceDeath = DateTime.UtcNow.Subtract(Trinity.TrinityPlugin.LastDeathTime).TotalMilliseconds;

                // We've died, no longer have familiar
                if (timeSinceDeath < TimeSincePowerUse(SNOPower.Wizard_Familiar))
                    return false;

                // we've used it within the last 5 minutes, we should still have it
                if (TimeSincePowerUse(SNOPower.Wizard_Familiar) < (5 * 60 * 1000))
                    return true;

                return false;
            }
        }

        internal static bool HasPrimarySkill
        {
            get
            {
                return Hotbar.Contains(SNOPower.Wizard_MagicMissile) ||
                    Hotbar.Contains(SNOPower.Wizard_ShockPulse) ||
                    Hotbar.Contains(SNOPower.Wizard_SpectralBlade) ||
                    Hotbar.Contains(SNOPower.Wizard_Electrocute);
            }
        }

        private static bool CanCastTeleport()
        {
            if (!CanCast(SNOPower.Wizard_Teleport, CanCastFlags.NoTimer))
                return false;

            if (TimeSincePowerUse(SNOPower.Wizard_Teleport) < Settings.Combat.Wizard.TeleportDelay)
                return false;

            if (Legendary.AetherWalker.IsEquipped && Player.PrimaryResource < 40)
                return false;

            if (Runes.Wizard.SafePassage.IsActive && Player.CurrentHealthPct > Settings.Combat.Wizard.SafePassageHealthPct)
                return false;

            if (Sets.DelseresMagnumOpus.IsFullyEquipped && Legendary.Triumvirate.IsEquipped &&
                Passives.Wizard.ArcaneDynamo.IsActive && GetBuffStacks(SNOPower.Wizard_Passive_ArcaneDynamo) > 1)
                return false;

            return true;
        }

        private static bool CanCastArcaneOrb()
        {
            if (!CanCast(SNOPower.Wizard_ArcaneOrb, CanCastFlags.NoTimer) || Player.IsIncapacitated ||
                Player.PrimaryResource < 35)
                return false;

            if (!IsDmoWiz && ShouldWaitForConventionElement(Skills.Wizard.ArcaneOrb))
                return false;

            if (Legendary.AquilaCuirass.IsEquipped && Player.PrimaryResourcePct <= .91)
                return false;

            if (IsDmoWiz)
            {
                if (GetBuffStacks(SNOPower.Wizard_Passive_ArcaneDynamo) < 5)
                    return false;

                if (GetBuffStacks(SNOPower.P2_ItemPassive_Unique_Ring_052) < 3)
                    return false;

                if (IsInsideCoeTimeSpan(Element.Arcane, 2000, 0))
                    return false;
            }

            return true;
        }

        private static bool CanCastExplosiveBlast()
        {
            if (!CanCast(SNOPower.Wizard_ExplosiveBlast, CanCastFlags.NoTimer) || Player.IsIncapacitated ||
                Player.PrimaryResource < 20)
                return false;

            if (!IsDmoWiz && ShouldWaitForConventionElement(Skills.Wizard.ExplosiveBlast))
                return false;

            var range = (Sets.TalRashasElements.IsFullyEquipped && Legendary.WandOfWoh.IsEquipped) ? 200 : 12;
            if (!TargetUtil.AnyMobsInRange(range))
                return false;

            if (IsDmoWiz && !IsInsideCoeTimeSpan(Element.Arcane, 2000, 0))
                return true;

            return true;
        }
    }
}
