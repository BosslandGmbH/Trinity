//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Trinity.Config.Combat;
//using Trinity.Framework;
//using Trinity.Framework.Actors.ActorTypes;
//using Trinity.Objects;
//using Trinity.Reference;
//using Trinity.Technicals;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;
//using Logger = Trinity.Technicals.Logger;

//namespace Trinity.Components.Combat.Abilities
//{
//    public class DemonHunterCombat : CombatBase
//    {
//        static DemonHunterCombat()
//        {
//            SkillUtils.SetSkillMeta(SkillsDefaultMeta.DemonHunter.ToList());
//            SetConditions();                
//        }    

//        /// <summary>
//        /// Main method for selecting a power
//        /// </summary>
//        public static TrinityPower GetPower()
//        {

//            if (Sets.EmbodimentOfTheMarauder.IsFullyEquipped)
//                EnergyReserve = 20;
//            else if (Sets.TheShadowsMantle.IsEquipped && Legendary.LordGreenstonesFan.IsEquipped)
//                EnergyReserve = 0;
//            else
//                EnergyReserve = 25;


//            TrinityPower power;
//            TrinityActor target;

//            if (UseDestructiblePower && TryGetPower(GetDestructablesSkill(), out power))
//            {
//                Logger.LogVerbose("Using Destructible Skill: {0}", power.SNOPower);
//                return power;
//            }

//            if (IsCurrentlyAvoiding)
//            {
//                if (TryGetPower(GetAvoidanceSkill(), out power))
//                {
//                    Logger.LogVerbose("Using General Avoidance Skill: {0}", power.SNOPower);
//                }
//                return power;
//            }

//            if (CurrentTarget != null && CurrentTarget.IsSafeSpot)
//            {
//                return null;
//            }

//            if (ShouldImpaleHighValueTarget(out target))
//            {
//                Logger.LogVerbose(LogCategory.SkillSelection, "ShouldImpaleHighValueTarget");
//                return new TrinityPower(Skills.DemonHunter.Impale.SNOPower, 80f, target.AcdId);
//            }

//            if (TryMoveToBuffedSpot(out power, 50f))
//                return power;

//            if (ShouldRefreshTaegukBuff)
//            {
//                Logger.LogVerbose(LogCategory.SkillSelection, "ShouldRefreshTaegukBuff");
//                return GetTaegukPower();
//            }
                
//            if (IsVaultFree && Player.CurrentHealthPct < 0.5 && LastPowerUsed != SNOPower.DemonHunter_Vault)
//            {                
//                Logger.Log("Emergency Avoidance Vault to SafeZone");
//                return new TrinityPower(Skills.DemonHunter.Vault.SNOPower, 30, Core.Avoidance.Avoider.SafeSpot);
//            }

//            // Vault towards target before casting fan.
//            var fanSkill = Skills.DemonHunter.FanOfKnives;
//            if (IsUsingShadowFanBuild && IsVaultFree && FanOfKnivesCondition(fanSkill.Meta) && fanSkill.Meta.TargetUnitSelector != null)
//            {                
//                target = fanSkill.Meta.TargetUnitSelector(fanSkill.Meta);                
//                if (target != null && target.Distance > 20f)
//                {
//                    Logger.Log(LogCategory.SkillSelection, "Vaulting to Fan target {0} Distance={1}", target.InternalName, target.Distance);
//                    return new TrinityPower(Skills.DemonHunter.Vault.SNOPower, 30, target.Position);
//                }                
//            }

//            if (CurrentTarget == null)
//            {
//                // Out of Combat Buffs
//                if (!IsCurrentlyAvoiding && !Player.IsInTown && TryGetPower(GetBuffSkill(), out power))
//                    return power;  
//            }
//            else
//            {
//                if (TryMaintainHardenedBuff(out power))
//                {
//                    return power;
//                }

//                // Elusive ring gives a damage reduction buff
//                if (ShouldRefreshElusiveBuff)
//                {
//                    Logger.LogVerbose(LogCategory.SkillSelection, "ShouldRefreshElusiveBuff");
//                    if (TryGetElusivePower(out power))
//                        return power;
//                }

//                // Use Generator for the Bastians Ring Set buff
//                if (!IsCurrentlyAvoiding && ShouldRefreshBastiansGeneratorBuff && TryGetPower(GetAttackGenerator(), out power))
//                {
//                    Logger.LogVerbose("Refreshing Bastians Generator Buff");
//                    return power;
//                }
                    
//                // Use Spender for the Bastians Ring Set buff
//                if (!IsCurrentlyAvoiding && ShouldRefreshBastiansSpenderBuff && TryGetPower(GetAttackSpender(), out power))
//                {
//                    Logger.LogVerbose("Refreshing Bastians Spender Buff");
//                    return power;
//                }

//                // Main ability selection 
//                if (TryGetPower(GetCombatPower(CombatSkillOrder), out power))
//                    return power;                
//            }

//            Logger.Log(TrinityLogLevel.Verbose, LogCategory.SkillSelection, Player.ActorClass + " GetPower() Returning DefaultPower Target={0}",
//                (CurrentTarget == null) ? "Null" : CurrentTarget.InternalName);

//            return DefaultPower;
//        }

//        private static bool TryMaintainHardenedBuff(out TrinityPower power)
//        {
//            if (Skills.DemonHunter.EvasiveFire.IsActive && Runes.DemonHunter.Hardened.IsActive && !Core.Buffs.HasBuff(SNOPower.X1_DemonHunter_EvasiveFire))
//            {
//                Logger.LogVerbose("Refreshing Hardened Buff");
//                var target = TargetUtil.GetClosestUnit(60f);
//                if (target != null)
//                {
//                    power = new TrinityPower(SNOPower.X1_DemonHunter_EvasiveFire, 18f, target.Position);
//                    return true;
//                }
//            }
//            power = null;
//            return false;
//        }

//        public static bool ShouldImpaleHighValueTarget(out TrinityActor target)
//        {
//            target = null;

//            if (!IsUsingShadowFanBuild || IsInCombat)
//                return false;

//            var minValuePct = 0.65;
//            var highValueTarget = TargetUtil.GetHighValueRiftTargets(60f, minValuePct).FirstOrDefault();
//            if (highValueTarget != null)
//            {
//                Logger.LogVerbose("Selected High Value Target Name={0} ProgressionPoints={1}", highValueTarget.InternalName, highValueTarget.RiftValuePct > minValuePct);
//                target = highValueTarget;
//                return true;
//            }

//            return false;
//        }

//        public static bool IsVaultFree
//        {
//            get
//            {
//                if (!Legendary.ChainOfShadows.IsEquipped)
//                    return false;

//                if (Core.Buffs.HasBuff(445266)) //'P4_ItemPassive_Unique_Ring_006' (445266)
//                    return true;

//                return false;
//            }
//        }

//        public static bool CanAcquireFreeVaultBuff
//        {
//            get
//            {
//                if (!Legendary.ChainOfShadows.IsEquipped)
//                    return false;

//                if (Core.Buffs.HasBuff(445266)) //'P4_ItemPassive_Unique_Ring_006' (445266)
//                    return false;

//                if (!CanCast(SNOPower.DemonHunter_Impale))
//                    return false;

//                return true;
//            }
//        }


//        private static bool IsUsingShadowFanBuild 
//        {
//            get { return Sets.TheShadowsMantle.IsEquipped && (Legendary.LordGreenstonesFan.IsEquipped || Legendary.SwordOfIllWill.IsEquipped) && Skills.DemonHunter.FanOfKnives.IsActive; }
//        }

//        private static bool TryGetElusivePower(out TrinityPower power)
//        {
//            if (SimpleCanCast(Skills.DemonHunter.Vault))
//            {
//                power = new TrinityPower(Skills.DemonHunter.Vault.SNOPower, 60f, Core.Avoidance.Avoider.SafeSpot);
//                return true;
//            }
                
//            if (SimpleCanCast(Skills.DemonHunter.ShadowPower))
//            {
//                power = new TrinityPower(Skills.DemonHunter.ShadowPower.SNOPower);
//                return true;
//            }

//            if (SimpleCanCast(Skills.DemonHunter.SmokeScreen))
//            { 
//                power = new TrinityPower(Skills.DemonHunter.SmokeScreen.SNOPower);
//                return true;
//            }

//            power = null;
//            return false;
//        }

//        private static TrinityPower GetTaegukPower()
//        {
//            var skill = SkillUtils.Active.FirstOrDefault(s => s.IsAttackSpender && SimpleCanCast(s));
//            if (skill != null)
//            {
//                var target = TargetUtil.GetClosestUnit(skill.Meta.CastRange);
//                if (target != null)
//                {
//                    {
//                        return new TrinityPower(skill.SNOPower, skill.Meta.CastRange, target.AcdId);                        
//                    }
//                }

//                var targetPos = skill.Meta.IsCastOnSelf ? Player.Position : MathEx.GetPointAt(Player.Position, 5f, Player.Rotation);
//                return new TrinityPower(skill.SNOPower, 80, targetPos);
               
//            }
//            return null;
//        }

//        /// <summary>
//        /// The combat skills and the order they should be evaluated in
//        /// </summary>
//        private static List<Skill> CombatSkillOrder
//        {
//            get
//            {
//                return new List<Skill>
//                {
//                    //Buffs
//                    Skills.DemonHunter.Vengeance,
//                    Skills.DemonHunter.ShadowPower,
//                    Skills.DemonHunter.SmokeScreen,
//                    Skills.DemonHunter.Preparation,
//                    Skills.DemonHunter.Companion,

//                    // Cooldown only
//                    Skills.DemonHunter.RainOfVengeance,

//                    // Spenders
//                    Skills.DemonHunter.Sentry,
//                    Skills.DemonHunter.Caltrops,
//                    Skills.DemonHunter.MarkedForDeath,
//                    Skills.DemonHunter.Vault,
//                    Skills.DemonHunter.FanOfKnives,
//                    Skills.DemonHunter.Multishot,
//                    Skills.DemonHunter.Strafe,
//                    Skills.DemonHunter.SpikeTrap,
//                    Skills.DemonHunter.ClusterArrow,
//                    Skills.DemonHunter.RapidFire,
//                    Skills.DemonHunter.Impale,
//                    Skills.DemonHunter.Chakram,
//                    Skills.DemonHunter.ElementalArrow,

//                    // Generators
//                    Skills.DemonHunter.EvasiveFire,
//                    Skills.DemonHunter.HungeringArrow,
//                    Skills.DemonHunter.EntanglingShot,
//                    Skills.DemonHunter.Bolas,
//                    Skills.DemonHunter.Grenade,
//                };
//            }
//        }

//        /// <summary>
//        /// When skills should be cast, evaluated by CanCast() calls.
//        /// </summary>
//        public static void SetConditions()
//        {
//            Skills.DemonHunter.RainOfVengeance.Meta.CastCondition = RainOfVengeanceCondition;
//            Skills.DemonHunter.Vengeance.Meta.CastCondition = VengeanceCondition;
//            Skills.DemonHunter.ShadowPower.Meta.CastCondition = ShadowPowerCondition;
//            Skills.DemonHunter.SmokeScreen.Meta.CastCondition = SmokeScreenCondition;
//            Skills.DemonHunter.Preparation.Meta.CastCondition = PreperationCondition;
//            Skills.DemonHunter.Sentry.Meta.CastCondition = SentryCondition;
//            Skills.DemonHunter.Caltrops.Meta.CastCondition = CaltropsCondition;
//            Skills.DemonHunter.Companion.Meta.CastCondition = CompanionCondition;
//            Skills.DemonHunter.MarkedForDeath.Meta.CastCondition = MarkedForDeathCondition;
//            Skills.DemonHunter.Vault.Meta.CastCondition = VaultCondition;
//            Skills.DemonHunter.FanOfKnives.Meta.CastCondition = FanOfKnivesCondition;
//            Skills.DemonHunter.Multishot.Meta.CastCondition = MultiShotCondition;
//            Skills.DemonHunter.Strafe.Meta.CastCondition = StrafeCondition;
//            Skills.DemonHunter.SpikeTrap.Meta.CastCondition = SpikeTrapCondition;
//            Skills.DemonHunter.ElementalArrow.Meta.CastCondition = ElementalArrowCondition;
//            Skills.DemonHunter.ClusterArrow.Meta.CastCondition = ClusterArrowCondition;
//            Skills.DemonHunter.Chakram.Meta.CastCondition = ChakramCondition;
//            Skills.DemonHunter.RapidFire.Meta.CastCondition = RapidFireCondition;
//            Skills.DemonHunter.Impale.Meta.CastCondition = ImpaleCondition;
//            Skills.DemonHunter.EvasiveFire.Meta.CastCondition = EvasiveFireCondition;
//            Skills.DemonHunter.HungeringArrow.Meta.CastCondition = HungeringArrowCondition;
//            Skills.DemonHunter.EntanglingShot.Meta.CastCondition = EntanglingShotCondition;
//            Skills.DemonHunter.Bolas.Meta.CastCondition = BolasCondition;
//            Skills.DemonHunter.Grenade.Meta.CastCondition = GrenadeCondition;
//        }

//        /// <summary>
//        /// When Grenade should be cast
//        /// </summary>
//        private static bool GrenadeCondition(SkillMeta meta)
//        {
//            meta.CastRange = 40f;
//            return true;
//        }

//        /// <summary>
//        /// When Bolas should be cast
//        /// </summary>
//        private static bool BolasCondition(SkillMeta meta)
//        {
//            meta.CastRange = 40f;
//            return true;
//        }

//        /// <summary>
//        /// When Entangling Shot should be cast
//        /// </summary>
//        private static bool EntanglingShotCondition(SkillMeta meta)
//        {
//            meta.CastRange = 50f;
//            return true;
//        }

//        /// <summary>
//        /// When Hungering Arrow should be cast
//        /// </summary>
//        private static bool HungeringArrowCondition(SkillMeta meta)
//        {
//            meta.CastRange = 50f;
//            return true;
//        }

//        /// <summary>
//        /// When Evasive fire should be cast
//        /// </summary>
//        /// <param name="meta"></param>
//        /// <returns></returns>
//        private static bool EvasiveFireCondition(SkillMeta meta)
//        {
//            meta.CastRange = Settings.Combat.DemonHunter.CastRangeEvasiveFire;

//            if (Skills.DemonHunter.Multishot.IsActive || Skills.DemonHunter.Strafe.IsActive)
//            {
//                // Still generates resource when hitting nothing.
//                if (CurrentTarget.IsBoss && !ShouldRefreshBastiansGeneratorBuff)
//                {
//                    meta.CastRange = Skills.DemonHunter.Multishot.IsActive ? meta.CastRange : 40f;
//                }

//                meta.TargetUnitSelector = ret => TargetUtil.GetClosestUnit();
//            }

//            if (Legendary.YangsRecurve.IsEquipped && Legendary.DeadMansLegacy.IsEquipped)
//            {
//                // Only use when we have to, it deals basically no damage, just generates resource and bastians buff.
//                return (Player.PrimaryResourcePct < 0.5 || ShouldRefreshBastiansGeneratorBuff) && TargetUtil.AnyMobsInRange(meta.CastRange);
//            }

//            return TargetUtil.AnyMobsInRange(meta.CastRange, Settings.Combat.DemonHunter.ClusterSizeEvasiveFire) || UseDestructiblePower;
//        }

//        /// <summary>
//        /// When Impale should be cast
//        /// </summary>
//        private static bool ImpaleCondition(SkillMeta meta)
//        {
//            meta.CastRange = Settings.Combat.DemonHunter.CastRangeImpale;
//            meta.TargetUnitSelector = null;

//            // Not enough resource
//            if (Player.PrimaryResource <= EnergyReserve || Player.PrimaryResource <= GetAdjustedCost(20))
//                return false;

//            if (CurrentTarget.RadiusDistance > meta.CastRange || !TargetUtil.AnyMobsInRange(meta.CastRange))
//                return false;            

//            if (TargetUtil.AnyMobsInRange(meta.CastRange, Settings.Combat.DemonHunter.ClusterSizeImpale) || (IsUsingShadowFanBuild || Legendary.KarleisPoint.IsEquipped) && TargetUtil.AnyMobsInRange(meta.CastRange) || Player.PrimaryResourcePct >= 0.5)
//                return true;
            
//            return false;
//        }

//        /// <summary>
//        /// When Rapid Fire should be cast
//        /// </summary>
//        /// <param name="meta"></param>
//        /// <returns></returns>
//        private static bool RapidFireCondition(SkillMeta meta)
//        {
//            meta.CastFlags = CanCastFlags.NoTimer;
//            meta.CastRange = 45f;

//            // Stay above minimum resource level
//            if (Player.PrimaryResource < EnergyReserve || Player.PrimaryResource < Settings.Combat.DemonHunter.RapidFireMinHatred)
//                return false;

//            // Never use it twice in a row
//            if (LastPowerUsed == SNOPower.DemonHunter_RapidFire)
//                return false;

//            return true;
//        }

//        /// <summary>
//        /// When Chakram should be cast.
//        /// </summary>
//        private static bool ChakramCondition(SkillMeta meta)
//        {
//            meta.CastRange = Settings.Combat.DemonHunter.CastRangeChakram;

//            // Spam it for Shuriken Cloud buff
//            if (Runes.DemonHunter.ShurikenCloud.IsActive && TimeSincePowerUse(SNOPower.DemonHunter_Chakram) >= 110000 &&
//                Player.PrimaryResource >= 10)
//                return true;

//            // Always cast with Spines of Seething Hatred rune, grants 4 hatred
//            if (Legendary.SpinesOfSeethingHatred.IsEquipped)
//                return true;

//            if (!SkillUtils.Active.Any(s => s.IsGeneratorOrPrimary))
//                return true;

//            // Monsters nearby
//            if (TargetUtil.NumMobsInRange(meta.CastRange) >= Settings.Combat.DemonHunter.ClusterSizeChakram)
//                return true;

//            return false;
//        }

//        /// <summary>
//        /// When Cluster Arrow should be cast
//        /// </summary>
//        private static bool ClusterArrowCondition(SkillMeta meta)
//        {
//            meta.CastRange = Settings.Combat.DemonHunter.CastRangeClusterArrow;
//            meta.TargetUnitSelector = ret => GetClusterTarget();

//            // Natalyas - Wait for damage buff
//            if (Sets.NatalyasVengeance.IsFullyEquipped && Player.PrimaryResource < 100 && !Core.Buffs.HasBuff(SNOPower.P2_ItemPassive_Unique_Ring_053))
//                return false;

//            // Stay above minimum resource level
//            if (Player.PrimaryResource < EnergyReserve)
//                return false;

//            return true;
//        }


//        private static TrinityActor GetClusterTarget()
//        {
//            return TargetUtil.GetBestClusterUnit();
//        }

//        /// <summary>
//        /// When Elemental Arrow should be cast
//        /// </summary>
//        private static bool ElementalArrowCondition(SkillMeta meta)
//        {
//            meta.CastRange = 100f;

//            // Stay above minimum resource level
//            if (Player.PrimaryResource < EnergyReserve && !Legendary.Kridershot.IsEquipped)
//                return false;

//            // Lightning DH
//            if (Runes.DemonHunter.BallLightning.IsActive && Legendary.AugustinesPanacea.IsEquipped)
//                meta.CastRange = 15f;

//            // Kridershot
//            if (Legendary.Kridershot.IsEquipped)
//                meta.CastRange = 65f;

//            return true;
//        }

//        /// <summary>
//        /// When spike trap should be cast
//        /// </summary>
//        private static bool SpikeTrapCondition(SkillMeta meta)
//        {
//            meta.TargetPositionSelector = SpikeTrapTargetSelector;

//            if (LastPowerUsed != SNOPower.DemonHunter_SpikeTrap)
//                return true;

//            return false;
//        }

//        /// <summary>
//        /// Where Spike trap should be cast
//        /// </summary>
//        private static Vector3 SpikeTrapTargetSelector(SkillMeta skillMeta)
//        {
//            // For distant monsters, try to target a little bit in-front of them (as they run towards us), if it's not a treasure goblin
//            float reducedDistance = 0f;
//            if (CurrentTarget.Distance > 17f && !CurrentTarget.IsTreasureGoblin)
//            {
//                reducedDistance = CurrentTarget.Distance - 17f;
//                if (reducedDistance > 5f)
//                    reducedDistance = 5f;
//            }
//            return MathEx.CalculatePointFrom(CurrentTarget.Position, Player.Position, CurrentTarget.Distance - reducedDistance);
//        }

//        /// <summary>
//        /// When Strafe should be cast
//        /// </summary>
//        private static bool StrafeCondition(SkillMeta meta)
//        {
//            meta.CastRange = 65f;
//            //meta.ReUseDelay = 250;
//            meta.TargetPositionSelector = ret =>
//            {
//                var kitePoint = Core.Avoidance.Avoider.SafeSpot;
//                if (kitePoint == Vector3.Zero)
//                {
//                    return TargetUtil.GetZigZagTarget(CurrentTarget.Position, 20f);
//                }
//                else
//                {
//                    return kitePoint;
//                }
                
//            };
//            //meta.TargetPositionSelector = ret => NavHelper.FindSafeZone(false, 0, CurrentTarget.Position, true, TrinityPlugin.ObjectCache, false);
//            meta.CastFlags = CanCastFlags.NoTimer;
//            meta.RequiredResource = Math.Max(Settings.Combat.DemonHunter.StrafeMinHatred, 12);

//            if (!Player.IsRooted)
//                return true;

//            return false;
//        }

//        /// <summary>
//        /// When Multishot should be cast
//        /// </summary>
//        private static bool MultiShotCondition(SkillMeta meta)
//        {
//            meta.CastRange = 60f;
//            meta.CastFlags = CanCastFlags.NoPowerManager;
//            meta.TargetUnitSelector = ret => TargetUtil.GetClosestUnit();

//            // Natalyas - Wait for damage buff
//            if (Sets.NatalyasVengeance.IsFullyEquipped && Player.PrimaryResource < 100 && !Core.Buffs.HasBuff(SNOPower.P2_ItemPassive_Unique_Ring_053))
//                return false;

//            //if (Sets.UnhallowedEssence.IsMaxBonusActive && TargetUtil.AnyMobsInRange(80f) ||
//            //    Sets.NatalyasVengeance.IsMaxBonusActive && TargetUtil.AnyMobsInRange(80f) ||
//            //    TargetUtil.ClusterExists(45, 2) || TargetUtil.AnyElitesInRange(80f))
//            //    return true;
//            return TargetUtil.AnyMobsInRange(60f);


//            //return false;
//        }

//        /// <summary>
//        /// When Fan of Knives should be cast
//        /// </summary>
//        private static bool FanOfKnivesCondition(SkillMeta meta)
//        {
//            meta.CastRange = 14f;

//            // Shadow Impale/Fan Build.
//            if (IsUsingShadowFanBuild)
//            {
//                var target = GetShadowFanTargetUnit();
//                var stacks = Core.Buffs.GetBuffStacks(445274);
//                meta.TargetUnitSelector = ret => target;
//                meta.CastRange = 8f;

//                if (Player.CurrentHealthPct < 0.30 && TargetUtil.NumMobsInRangeOfPosition(Player.Position, 14f) > 4)
//                {
//                    Logger.Log("Casting Emergency Fan of Knives < 0.35% health", stacks);
//                    return true;
//                }

//                if (TargetUtil.NumMobsInRangeOfPosition(Player.Position, 16f) > 14)
//                {
//                    Logger.Log("Casting Emergency Fan because omg lots of enemies!", stacks);
//                    return true;
//                }

//                if (stacks > 5 && TargetUtil.NumMobsInRangeOfPosition(Player.Position, 12f) > 8 && Player.CurrentHealthPct <= 0.5)
//                {
//                    Logger.Log("Casting Emergency Fan of Knives, surrounded and on low health", stacks);
//                    return true;
//                }
                
//                // Wait until correct element if applicable
//                if (Legendary.ConventionOfElements.IsEquipped && Core.Buffs.ConventionElement != meta.Skill.Element)
//                    return false;

//                // Don't waste damage potential
//                if (stacks == 30 && TargetUtil.NumMobsInRangeOfPosition(Player.Position, 12f) > 8 && !TargetUtil.AnyElitesInRange(200))
//                {
//                    Logger.Log("Casting Fan, Surrounded with Max Stacks", stacks);
//                    return true;
//                }

//                // Wait until 15+ stacks of fan.                
//                if (stacks < 15) //P4_ItemPassive_Unique_Ring_007
//                {
//                    //Logger.Log("Not enough buff stacks {0}", stacks);
//                    return false;
//                }
                
//                return target != null;
//            }

//            if (TargetUtil.AnyElitesInRange(15) || TargetUtil.AnyTrashInRange(15f, 5, false))
//            {
//                Logger.LogVerbose("Default Fan of Knives on close elite or cluster");
//                return true;
//            }
                
//            return false;
//        }

//        private static TrinityActor GetShadowFanTargetUnit()
//        {
//            if(IsDoingGoblinKamakazi && CurrentTarget.IsTreasureGoblin)
//                return CurrentTarget;

//            var targetUnit = TargetUtil.BestEliteInRange(120f);
//            if (targetUnit == null)
//            {
//                if (TargetUtil.ClusterExists(12f, 10))
//                {
//                    targetUnit = TargetUtil.GetBestClusterUnit(35f);
//                    Logger.Log("Fan Targetted on Large Cluster");
//                }
//                else
//                {
//                    return null;
//                }
//            }
//            Logger.Log("Fan Targetted on Elite {0} Distance={1}", targetUnit.InternalName, targetUnit.Distance);
//            return targetUnit;
//        }

//        /// <summary>
//        /// When Vault should be cast
//        /// </summary>
//        private static bool VaultCondition(SkillMeta meta)
//        {           
//            var closestUnit = TargetUtil.GetClosestUnit();
//            var combatRange = SkillUtils.Active.Where(s => s.IsAttackSpender || s.IsGeneratorOrPrimary).Min(s => s.Meta.CastRange);
//            var closestUnitDistance = closestUnit?.Distance ?? 10;
//            var maxDistance = combatRange - closestUnitDistance;
            
//            // Try not to vault too far away or we'll have to walk back into range to attack.
//            meta.TargetPositionSelector = ret => Core.Avoidance.Avoider.SafeSpot; // NavHelper.KitePoint(Player.Position, 10f, maxDistance);
//            meta.CastRange = 80f;

//            // keep a larger reserve if we might need to cast shadowpower             
//            meta.RequiredResource = Hotbar.Contains(SNOPower.DemonHunter_ShadowPower) ? 22 : 16; 
//            meta.ReUseDelay = Settings.Combat.DemonHunter.VaultMovementDelay;
            
//            // Vaulting around with no reason wastes time that could be spent damaging stuff.
//            if ((closestUnitDistance > combatRange || closestUnitDistance <= KiteDistance) && Player.CurrentHealthPct > 0.85 && !IsCurrentlyAvoiding)
//                return false;

//            if (Settings.Combat.DemonHunter.VaultMode == DemonHunterVaultMode.MovementOnly && (IsInCombat || ZetaDia.Me.IsInCombat))
//                return false;

//            if (Settings.Combat.DemonHunter.VaultMode == DemonHunterVaultMode.CombatOnly && !(IsInCombat || ZetaDia.Me.IsInCombat))
//                return false;

//            // Use free vault but make sure we're still casting enough impale's
//            if (IsVaultFree &&
//                (!IsUsingShadowFanBuild ||
//                 SpellHistory.TimeSinceUse(Skills.DemonHunter.Vault.SNOPower).TotalMilliseconds < 500))
//            {
//                Logger.LogVerbose("Using Free Vault");
//                meta.RequiredResource = 0;
//                return true;
//            }

//            meta.RequiredResource = Skills.DemonHunter.Vault.Cost;

//            if (!Player.IsRooted && (TargetUtil.AnyMobsInRange(12f, 2) && Player.CurrentHealthPct <= 0.9))
//                return true;

//            return false;
//        }

//        /// <summary>
//        /// When Marked for Death should be cast
//        /// </summary>
//        private static bool MarkedForDeathCondition(SkillMeta meta)
//        {
//            meta.CastRange = 80f;
//            meta.CastFlags = CanCastFlags.NoTimer;

//            if (!CurrentTarget.HasDebuff(SNOPower.DemonHunter_MarkedForDeath) && !SpellTracker.IsUnitTracked(CurrentTarget, SNOPower.DemonHunter_MarkedForDeath))                
//                return true;

//            return false;
//        }

//        /// <summary>
//        /// When Companion should be cast
//        /// </summary>
//        private static bool CompanionCondition(SkillMeta meta)
//        {
//            meta.CastFlags = CanCastFlags.NoTimer;

//            // Use Spider Slow on 4 or more trash mobs in an area or on Unique/Elite/Champion
//            if (Runes.DemonHunter.SpiderCompanion.IsActive && TargetUtil.ClusterExists(25f, 4) && TargetUtil.EliteOrTrashInRange(25f))
//                return true;

//            //Use Bat when Hatred is Needed
//            if (Runes.DemonHunter.BatCompanion.IsActive && Player.PrimaryResourceMissing >= 60)
//                return true;

//            // Use Boar Taunt on 3 or more trash mobs in an area or on Unique/Elite/Champion
//            if (Runes.DemonHunter.BoarCompanion.IsActive && ((TargetUtil.ClusterExists(20f, 4) && TargetUtil.EliteOrTrashInRange(20f)) || (CurrentTarget.IsElite && CurrentTarget.Distance <= 20f)))
//                return true;

//            // Ferrets used for picking up Health Globes when low on Health
//            if (Runes.DemonHunter.FerretCompanion.IsActive && Trinity.TrinityPlugin.Targets.Any(o => o.Type == TrinityObjectType.HealthGlobe && o.Distance < 60f) && Player.CurrentHealthPct < EmergencyHealthPotionLimit)
//                return true;

//            // Use Wolf Howl on Unique/Elite/Champion - Would help for farming trash, but trash farming should not need this - Used on Elites to reduce Deaths per hour
//            if (Runes.DemonHunter.WolfCompanion.IsActive && (TargetUtil.AnyElitesInRange(100f) || TargetUtil.AnyMobsInRange(40, Settings.Combat.DemonHunter.ClusterSizeCompanionWolf)))            
//                return true;

//            // Companion off CD
//            if (Settings.Combat.DemonHunter.CompanionOffCooldown && TargetUtil.AnyMobsInRange(60))
//                return true;
            
//            return false;
//        }

//        /// <summary>
//        /// When Caltrops should be cast
//        /// </summary>
//        private static bool CaltropsCondition(SkillMeta meta)
//        {        
//            return TargetUtil.AnyMobsInRange(40) && !GetHasBuff(SNOPower.DemonHunter_Caltrops);
//        }

//        /// <summary>
//        /// When Sentry should be cast
//        /// </summary>
//        /// <param name="meta"></param>
//        /// <returns></returns>
//        private static bool SentryCondition(SkillMeta meta)
//        {
//            meta.CastRange = 85f;
//            meta.CastFlags = CanCastFlags.NoTimer;
//            meta.TargetPositionSelector = ret => TargetUtil.BestSentryPosition();
//            meta.ReUseDelay = 250;

//            if (meta.Skill.Charges == 0)
//                return false;

//            if (TargetUtil.AnyMobsInRange(65) && Player.Summons.DHSentryCount < MaxSentryCount)
//                return true;

//            return false;
//        }



//        /// <summary>
//        /// When Rain of Vengeance should be cast
//        /// </summary>
//        private static bool RainOfVengeanceCondition(SkillMeta meta)
//        {
//            meta.CastRange = 90f;            
//            meta.CastFlags = CanCastFlags.NoTimer;

//            if (Legendary.CrashingRain.IsEquipped)
//                meta.TargetPositionSelector = skillMeta => TargetUtil.GetBestClusterPoint(30f, 80f); 

//            if (Settings.Combat.DemonHunter.RainOfVengeanceOffCD || Sets.NatalyasVengeance.IsEquipped)
//                return true;

//            if (TargetUtil.ClusterExists(45f, 4) || TargetUtil.AnyElitesInRange(90f))
//                return true;

//            return false;
//        }

//        /// <summary>
//        /// When Preperation should be cast
//        /// </summary>
//        private static bool PreperationCondition(SkillMeta meta)
//        {
//            meta.ReUseDelay = Runes.DemonHunter.FocusedMind.IsActive ? 15000 : 500;
//            meta.CastFlags = CanCastFlags.NoTimer;

//            if (!Runes.DemonHunter.Punishment.IsActive && Player.SecondaryResourcePct <= Settings.Combat.DemonHunter.PreperationResourcePct)
//                return true;

//            if (Runes.DemonHunter.Punishment.IsActive && Player.PrimaryResourcePct <= Settings.Combat.DemonHunter.PreperationResourcePct && (TargetUtil.AnyElitesInRange(50f) || Core.Clusters.Nearby.UnitCount > 5))
//                return true;

//            return false;
//        }

//        /// <summary>
//        /// When Smoke Screen should be cast
//        /// </summary>
//        private static bool SmokeScreenCondition(SkillMeta meta)
//        {
//            meta.CastFlags = CanCastFlags.NoTimer;

//            // Buff Already Active
//            if (GetHasBuff(SNOPower.DemonHunter_ShadowPower))
//                return false;

//            // Mobs in range
//            if (TargetUtil.AnyMobsInRange(15) || (Legendary.AugustinesPanacea.IsEquipped && TargetUtil.AnyMobsInRange(60)))
//                return true;

//            // Defensive Cast
//            if((Player.CurrentHealthPct <= 0.50 || Player.IsRooted || Player.IsIncapacitated))
//                return true;

//            // Spam Setting
//            if (Settings.Combat.DemonHunter.SpamSmokeScreen)
//                return true;

//            return false;
//        }

//        /// <summary>
//        /// When Shadow Power should be cast
//        /// </summary>
//        private static bool ShadowPowerCondition(SkillMeta meta)
//        {
//            // Buff Already Active
//            if(GetHasBuff(SNOPower.DemonHunter_ShadowPower))
//                return false;

//            // Not Enough Discipline
//            if (Player.SecondaryResource < GetAdjustedCost(14) || Runes.DemonHunter.WellOfDarkness.IsActive && Player.SecondaryResource < GetAdjustedCost(8))
//                return false;

//            // Used Recently
//            if (TimeSincePowerUse(SNOPower.DemonHunter_ShadowPower) < 4500)
//                return false;

//            // Low Health
//            if(Player.CurrentHealthPct <= Settings.Combat.DemonHunter.ShadowPowerHealth)
//                return true;

//            // Defensive Cast
//            if (Player.IsRooted || TargetUtil.AnyMobsInRange(6) || IsCurrentlyAvoiding)
//                return true;

//            if (Settings.Combat.DemonHunter.UseShadowPowerWhenSurrounded && TargetUtil.AnyMobsInRange(6))
//                return true;

//            if (Settings.Combat.DemonHunter.SpamShadowPowerWhenElitesNearby && TargetUtil.AnyElitesInRange(40f))
//                return true;

//            if (Settings.Combat.DemonHunter.UseShadowPowerWhileAvoiding && (IsCurrentlyAvoiding || TargetUtil.AvoidancesInRange(15f)))
//                return true;

//            // Spam Setting
//            if (Settings.Combat.DemonHunter.SpamShadowPower)
//                return true;

//            return false;
//        }

//        /// <summary>
//        /// When Vengeance should be cast
//        /// </summary>
//        private static bool VengeanceCondition(SkillMeta meta)
//        {
//            meta.CastFlags = CanCastFlags.NoTimer;

//            if (Settings.Combat.DemonHunter.AlwaysVengeance)
//                return true;

//            if (Legendary.Dawn.IsEquipped)
//                return true;

//            if (!Settings.Combat.DemonHunter.VengeanceElitesOnly && TargetUtil.AnyMobsInRange(60f, 6))
//                return true;

//            if (TargetUtil.IsEliteTargetInRange(100f))
//                return true;

//            return false;
//        }

//        /// <summary>
//        /// Maximum number of sentries allowed from Equipped items and Passives
//        /// </summary>
//        public static int MaxSentryCount => 2 + (Legendary.BombardiersRucksack.IsEquipped ? 2 : 0) + (Passives.DemonHunter.CustomEngineering.IsActive ? 1 : 0);
//    }
//}
