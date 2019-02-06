using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Behaviors;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Modules;
using Trinity.Settings;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Routines
{
    public class RoutineBase : NotifyBase
    {
        public virtual float PotionHealthPct => 0.50f;
        public virtual float EmergencyHealthPct => 0.50f;
        public virtual int ClusterSize => 1;
        public virtual float ClusterRadius => 20f;

        protected static PlayerCache Player
            => Core.Player;

        protected static TrinityActor CurrentTarget
            => TrinityCombat.Targeting.CurrentTarget;

        protected static IPartyProvider Party
            => TrinityCombat.Party;

        protected static IPartyMember PartyMe
            => TrinityCombat.Party.Members.FirstOrDefault(m => m.IsMe);

        protected static IPartyMember PartyLeader
            => TrinityCombat.Party.Leader;

        protected static PartyObjective MyPartyObjective
            => PartyMe?.Objective ?? default(PartyObjective);

        protected static TrinityActor PartyTarget
            => PartyLeader.Target != null ? PartyHelper.FindLocalActor(PartyLeader.Target) : null;

        protected static TrinityPower CurrentPower
            => TrinityCombat.Targeting.CurrentPower;

        protected static IAvoider Avoider
            => Core.Avoidance.Avoider;

        protected static TrinityPower Walk(TrinityActor target)
        {
            return new TrinityPower(SNOPower.Walk, Math.Max(7f, target.AxialRadius), target.Position);
        }

        protected static TrinityPower Walk(Vector3 destination, float range = 0f)
        {
            return new TrinityPower(SNOPower.Walk, range, destination);
        }

        public bool IsEliteNearby
            => WeightedUnits.Any(u => u.IsElite || u.IsTreasureGoblin);

        public bool IsEliteClose
            => WeightedUnits.Any(u => (u.IsElite || u.IsTreasureGoblin) && u.Distance < 25f);

        public IEnumerable<TrinityActor> LocalElites
            => WeightedUnits.Where(u => (u.IsElite || u.IsTreasureGoblin) && u.Distance <= 100f);

        public Vector3 EliteCentroid
            => TargetUtil.GetCentroid(LocalElites.Select(u => u.Position));

        public bool IsClusteredElites
            => LocalElites.All(u => u.Position.Distance(EliteCentroid) < 25f);

        public bool IsPartyGroupedTogether
            => Party.Members.Count() <= 1 || Party.Members.Count(p => p.Distance <= 30f) == PartyMembersNearby;

        public int PartyMembersNearby
            => Party.Members.Count(p => p.Distance <= 80f);

        /// <summary>
        /// A safe list of units who are valid and have a weight.
        /// </summary>
        protected static IEnumerable<TrinityActor> WeightedUnits
            => Core.Targets.Entries.Where(u => u.IsUnit && u.Weight > 0 && u.IsHostile);

        /// <summary>
        /// A raw hostile unit list without being filtered for valid targets. Use with caution.
        /// </summary>
        protected static IEnumerable<TrinityActor> HostileMonsters
            => Core.Actors.Actors.Where(u => u.IsUnit && u.IsHostile);

        /// <summary>
        /// A raw hostile unit list who are currently in line of sight without being filtered for valid targets. Use with caution.
        /// </summary>
        protected static IEnumerable<TrinityActor> AllUnitsInSight
            => HostileMonsters.Where(u => u.IsInLineOfSight);

        /// <summary>
        /// If the player is likely unable to move forward at its current facing direction.
        /// </summary>
        protected static bool IsBlocked
            => PlayerMover.IsBlocked;

        /// <summary>
        /// if the player is currently 'Stuck' - a serious matter which causes an unstucker to run.
        /// </summary>
        protected static bool IsStuck
            => Core.StuckHandler.IsStuck;

        /// <summary>
        /// Trinity's definition of IsInCombat is true only when we are actively targetting a unit.
        /// IsInCombatOrBeingAttacked will use D3's internal IsInCombat - which is True when any monster 
        /// has attacked the player recently or player has dealt damage to a monster.
        /// </summary>
        protected static bool IsInCombatOrBeingAttacked =>
            ZetaDia.Me.IsInCombat;

        /// <summary>
        /// A number of units somewhat larger than the current cluster size setting.
        /// </summary>
        protected static int LargeClusterSize
            => (int)Math.Ceiling(Core.Routines.CurrentRoutine.ClusterSize * 1.25d);

        /// <summary>
        /// Current target is a safespot and we are moving to it for avoidance reasons
        /// </summary>
        protected static bool IsCurrentlyAvoiding
            => TrinityCombat.IsCurrentlyAvoiding;

        /// <summary>
        /// Current target is a safespot and we are moving to it for kiting reasons
        /// </summary>
        protected static bool IsCurrentlyKiting
            => TrinityCombat.IsCurrentlyKiting;

        /// <summary>
        /// If our current target is a hostile unit
        /// </summary>
        protected static bool IsInCombat
            => TrinityCombat.IsInCombat;

        /// <summary>
        /// Player's current build has multiple free/generator/primary skills equipped
        /// </summary>
        protected static bool IsMultiPrimary
            => SkillUtils.Active.Count(s => s.IsGeneratorOrPrimary) > 1;

        /// <summary>
        /// Player's current build has multiple costly/spender/secondary skills equipped.
        /// </summary>
        protected static bool IsMultiSpender
            => SkillUtils.Active.Count(s => s.IsAttackSpender) > 1;

        /// <summary>
        /// Players build has no free/generator/primary skills equipped
        /// </summary>
        public static bool IsNoPrimary
            => SkillUtils.Active.Count(s => s.IsGeneratorOrPrimary) == 0;

        public static bool ShouldRefreshBastiansGenerator
            => Sets.BastionsOfWill.IsFullyEquipped && !Core.Buffs.HasBastiansWillGeneratorBuff
            && SpellHistory.TimeSinceGeneratorCast >= 3750;

        public static bool ShouldRefreshBastiansSpender
            => Sets.BastionsOfWill.IsFullyEquipped && !Core.Buffs.HasBastiansWillSpenderBuff
            && SpellHistory.TimeSinceSpenderCast >= 3750;

        public static int EndlessWalkOffensiveStacks
            => Core.Buffs.GetBuffStacks(447541, 1);

        public static bool HasInfiniteCasting
            => Core.Buffs.HasCastingShrine;

        public static bool HasIngeomBuff
            => Player.HasBuff(SNOPower.ItemPassive_Unique_Ring_919_x1);

        public static bool HasInstantCooldowns
            => HasInfiniteCasting || HasIngeomBuff;


        /// <summary>
        /// A helper utility for checking common skill conditions.
        /// </summary>
        protected bool AllowedToUse(SkillSettings settings, Skill skill)
        {
            if (settings == null || skill == null)
                return true;

            if (settings.UseMode == UseTime.Always)
                return true;

            if (settings.UseMode == UseTime.Never)
                return false;

            if (IsRestricted(settings, skill))
                return false;

            if (IsReasonToUse(settings, skill))
                return true;

            if (settings.UseMode == UseTime.Selective)
                return false;

            if (settings.UseMode == UseTime.OutOfCombat &&
                IsInCombat)
            {
                return false;
            }

            return settings.UseMode != UseTime.InCombat || IsInCombat;
        }

        protected bool IsRestricted(SkillSettings settings, Skill skill)
        {
            if (Player.PrimaryResourcePct < settings.PrimaryResourcePct / 100)
                return true;

            if (Player.SecondaryResourcePct < settings.SecondaryResourcePct / 100)
                return true;

            if (Player.CurrentHealthPct > settings.HealthPct)
                return true;

            if (SpellHistory.TimeSinceUse(skill.SNOPower).TotalMilliseconds < settings.RecastDelayMs)
                return true;

            if (settings.ClusterSize > 0 &&
                !TargetUtil.ClusterExists(15f, settings.ClusterSize))
            {
                return true;
            }

            if (settings.WaitForConvention == ConventionMode.GreaterRift &&
                !Core.Rift.IsGreaterRift)
            {
                return true;
            }

            if (settings.WaitForConvention != ConventionMode.Never &&
                settings.ConventionCondition != null &&
                !settings.ConventionCondition())
            {
                return true;
            }

            return false;
        }

        protected bool IsReasonToUse(SkillSettings settings, Skill skill)
        {
            IRoutine routine = Core.Routines.CurrentRoutine;

            if (settings.Reasons.HasFlag(UseReasons.Elites) &&
                TargetUtil.AnyElitesInRange(40f))
            {
                return true;
            }

            if (settings.Reasons.HasFlag(UseReasons.Trash) &&
                TargetUtil.ClusterExists(routine.TrashRange, routine.TrashRange, routine.ClusterSize))
            {
                return true;
            }

            if (settings.Reasons.HasFlag(UseReasons.Surrounded) &&
                TargetUtil.NumMobsInRange(25f) >= Math.Max(ClusterSize, 5))
            {
                return true;
            }

            if (settings.Reasons.HasFlag(UseReasons.Avoiding) &&
                IsCurrentlyAvoiding)
            {
                return true;
            }

            if (settings.Reasons.HasFlag(UseReasons.Blocked) &&
                PlayerMover.IsBlocked)
            {
                return true;
            }

            if (settings.Reasons.HasFlag(UseReasons.DumpResource) &&
                Player.PrimaryResourcePct < 0.8f)
            {
                return true;
            }

            if (settings.Reasons.HasFlag(UseReasons.Goblins) &&
                WeightedUnits.Any(u => u.IsTreasureGoblin))
            {
                return true;
            }

            if (settings.Reasons.HasFlag(UseReasons.HealthEmergency) &&
                Player.CurrentHealthPct < TrinityCombat.Routines.Current.EmergencyHealthPct)
            {
                return true;
            }

            if (settings.Reasons.HasFlag(UseReasons.Buff) &&
                settings.BuffCondition != null &&
                settings.BuffCondition())
            {
                return true;
            }

            return false;
        }

        public static TrinityPower DefaultPower
        {
            get
            {
                if (CurrentTarget != null)
                {
                    return new TrinityPower
                    {
                        SNOPower = DefaultWeaponPower,
                        MinimumRange = DefaultWeaponDistance,
                        TargetAcdId = CurrentTarget.AcdId,
                        TargetPosition = CurrentTarget.Position,
                    };
                }
                return new TrinityPower();
            }
        }

        public static SNOPower DefaultWeaponPower
        {
            get
            {
                ACDItem lhItem = Core.Inventory.Equipped.FirstOrDefault(i => i.InventorySlot == InventorySlot.LeftHand);
                if (lhItem == null)
                    return SNOPower.None;

                switch (lhItem.GetItemType())
                {
                    case ItemType.Wand:
                        return SNOPower.Weapon_Ranged_Wand;
                    case ItemType.Bow:
                    case ItemType.Crossbow:
                    case ItemType.HandCrossbow:
                        return SNOPower.Weapon_Ranged_Projectile;
                }
                return SNOPower.Weapon_Melee_Instant;
            }
        }

        public static float DefaultWeaponDistance
        {
            get
            {
                switch (DefaultWeaponPower)
                {
                    case SNOPower.Weapon_Ranged_Instant:
                    case SNOPower.Weapon_Ranged_Projectile:
                        return 50f;
                    case SNOPower.Weapon_Ranged_Wand:
                        return 50f;
                    default:
                        return 12f;
                }
            }
        }

        #region Hardcore

        public virtual bool SetWeight(TrinityActor cacheObject)
        {
            return false;
        }

        public virtual async Task<bool> HandleKiting()
        {
            if (!Core.Avoidance.Avoider.ShouldKite)
                return false;

            if (!Core.Avoidance.Avoider.TryGetSafeSpot(out Vector3 safespot) ||
                safespot.Distance(ZetaDia.Me.Position) < 5f)
            {
                return false;
            }

            Core.Logger.Log(LogCategory.Avoidance, "Kiting");
            await CastDefensiveSpells();
            return await CommonCoroutines.MoveAndStop(Core.Avoidance.Avoider.SafeSpot, 5f, "Kiting") != MoveResult.ReachedDestination;
        }

        protected virtual bool IsAvoidanceRequired
        {
            get
            {
                if (!Core.Avoidance.Avoider.ShouldAvoid)
                    return false;

                var isCloseToSafeSpot = Core.Player.Position.Distance(Core.Avoidance.Avoider.SafeSpot) < 5f;
                if (Core.Avoidance.Avoider.SafeSpot.Distance(Player.Position) > 5f && (Core.Player.Actor.IsInAvoidance || (CurrentTarget != null && ((CurrentTarget.IsInAvoidance && !isCloseToSafeSpot)
                    || CurrentTarget.Distance > CurrentPower?.MinimumRange
                    || !CurrentTarget.IsAvoidanceOnPath))))
                {
                    return true;
                }

                Core.Logger.Log(LogCategory.Avoidance, "Not avoiding due to being safe and target is within range");
                return false;
            }
        }

        public virtual async Task<bool> HandleAvoiding()
        {
            if (Core.Player.Actor == null ||
                !IsAvoidanceRequired)
            {
                return false;
            }

            var safe = (!Core.Player.IsTakingDamage ||
                         Core.Player.CurrentHealthPct > 0.5f) &&
                        Core.Player.Actor != null &&
                        !Core.Player.Actor.IsInCriticalAvoidance;

            if (!TrinityCombat.IsInCombat &&
                Core.Player.Actor.IsAvoidanceOnPath && safe)
            {
                Core.Logger.Log(LogCategory.Avoidance, "Waiting for avoidance to clear (out of combat)");
                return await CommonCoroutines.MoveAndStop(Core.Avoidance.Avoider.SafeSpot, 5f, "Safe Spot") != MoveResult.ReachedDestination;
            }

            if (Core.Avoidance.Avoider.SafeSpot.Distance(Player.Position) < 5f)
            {
                return false;
            }

            Core.Logger.Log(LogCategory.Avoidance, "Moving away from Critical Avoidance.");
            MoveResult res;
            if ((res = await CommonCoroutines.MoveAndStop(Core.Avoidance.Avoider.SafeSpot, 5f, "Safe Spot")) != MoveResult.ReachedDestination &&
                res != MoveResult.Failed)
            {
                return true;
            }

            await CastDefensiveSpells();
            return true;
        }

        public static async Task<bool> CastDefensiveSpells()
        {
            TrinityPower power = TrinityCombat.Routines.Current.GetDefensivePower();
            if (power != null &&
                power.SNOPower != SpellHistory.LastPowerUsed)
            {
                return await TrinityCombat.Spells.CastTrinityPower(power, "Defensive");
            }
            return false;
        }

        public virtual async Task<bool> HandleTarget(TrinityActor target)
        {
            if (await WaitForRiftBossSpawn())
            {
                return true;
            }

            if (WaitForInteractionChannelling())
            {
                return true;
            }

            if (CurrentPower == null)
            {
                if (!Core.Player.IsPowerUseDisabled)
                {
                    Core.Logger.Log(LogCategory.Targetting, $"No valid power was selected for target: {CurrentTarget}");
                }
                return false;
            }

            if (CurrentPower.SNOPower == SNOPower.None)
            {
                return true;
            }

            if (await TrinityCombat.Spells.CastTrinityPower(CurrentPower))
            {
                return true;
            }

            if (DateTime.UtcNow.Subtract(SpellHistory.LastSpellUseTime).TotalSeconds > 5)
            {
                Core.Logger.Verbose(LogCategory.Targetting, $"Routine power cast failure timeout. Clearing Target: {target?.Name} and Power: {CurrentPower}");
                TrinityCombat.Targeting.Clear();
                return false;
            }

            if (CurrentPower.SNOPower != SNOPower.Walk &&
                CurrentPower.TargetPosition != Vector3.Zero &&
                CurrentPower.TargetPosition.Distance(Core.Player.Position) >
                TrinityCombat.Targeting.MaxTargetDistance)
            {
                Core.Logger.Verbose(LogCategory.Targetting, $"Player is too far away from Target: {target?.Distance} and Power: {CurrentPower}");
                return false;
            }

            return true;
        }

        public async Task<bool> WaitForRiftBossSpawn()
        {
            if (Core.Rift.IsInRift &&
                CurrentTarget.IsBoss)
            {
                if (CurrentTarget.IsSpawningBoss)
                {
                    Core.Logger.Verbose(LogCategory.Targetting, "Waiting while rift boss spawn");

                    if (Core.Avoidance.Avoider.TryGetSafeSpot(out Vector3 safeSpot, 30f, 100f, CurrentTarget.Position))
                    {
                        return await CommonCoroutines.MoveAndStop(safeSpot, 5f, "SafeSpot") != MoveResult.ReachedDestination;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool WaitForInteractionChannelling()
        {
            if (Core.Player.IsCasting &&
                !Core.Player.IsTakingDamage &&
                CurrentTarget != null &&
                CurrentTarget.IsGizmo)
            {
                Core.Logger.Verbose(LogCategory.Targetting, "Waiting while channelling spell");
                return true;
            }
            return false;
        }

        public virtual async Task<bool> HandleOutsideCombat()
        {
            if (!Core.Player.IsCasting &&
                (!TargetUtil.AnyMobsInRange(20f) ||
                 !Core.Player.IsTakingDamage))
            {
                if (await Behaviors.MoveToMarker.While(m => m.MarkerType == WorldMarkerType.LegendaryItem || m.MarkerType == WorldMarkerType.SetItem))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual async Task<bool> HandleBeforeCombat()
        {
            /* If standing on/nearby a portal, then assume it is intended to get in there. */
            if (!Core.Player.IsInRift &&
                BountyHelpers.GetPortalNearPosition(ZetaDia.Me.Position) != null)
            {
                Core.Logger.Debug("MainCombatTask Waiting for portal interaction");
                return false;
            }

            // Wait after elite death until progression globe appears as a valid target or x time has passed.
            if (Core.Rift.IsInRift &&
                await Behaviors.WaitAfterUnitDeath.While(u => u.IsElite &&
                                                              u.Distance < 150 &&
                                                              Core.Targets.Entries.Any(a => a.IsElite &&
                                                                                            a.EliteType != EliteTypes.Minion &&
                                                                                            a.RadiusDistance < 60) &&
                                                              !Core.Targets.Any(p => p.Type == TrinityObjectType.ProgressionGlobe &&
                                                                                     p.Distance < 150),
                "Wait for Progression Globe", 1000))
            {
                return true;
            }

            // Priority movement for progression globes. ** Temporary solution!
            if (TrinityCombat.Targeting.CurrentTarget != null)
            {
                if (await Behaviors.MoveToActor.While(a => a.Type == TrinityObjectType.ProgressionGlobe &&
                                                           !TrinityCombat.Weighting.ShouldIgnore(a) && !a.IsAvoidanceOnPath))
                {
                    return true;
                }
            }

            // Priority interaction for doors. increases door opening reliability for some edge cases ** Temporary solution!
            if (ZetaDia.Storage.RiftStarted &&
                await Behaviors.MoveToInteract.While(a => a.Type == TrinityObjectType.Door &&
                                                          !a.IsUsed && a.Distance < a.CollisionRadius))
            {
                return true;
            }

            return false;
        }

        public virtual TrinityPower GetPowerForTarget(TrinityActor target)
        {
            IRoutine routine = TrinityCombat.Routines.Current;
            if (target == null)
                return null;

            switch (target.Type)
            {
                case TrinityObjectType.BloodShard:
                case TrinityObjectType.Gold:
                case TrinityObjectType.HealthGlobe:
                case TrinityObjectType.PowerGlobe:
                case TrinityObjectType.ProgressionGlobe:
                    return routine.GetMovementPower(target.Position);

                case TrinityObjectType.Door:
                case TrinityObjectType.HealthWell:
                case TrinityObjectType.Shrine:
                case TrinityObjectType.Interactable:
                case TrinityObjectType.CursedShrine:
                    return InteractPower(target, 100, 250);

                case TrinityObjectType.CursedChest:
                case TrinityObjectType.Container:
                    return InteractPower(target, 100, 1200);

                case TrinityObjectType.Item:
                    return InteractPower(target, 15, 15, 6f);

                case TrinityObjectType.Destructible:
                case TrinityObjectType.Barricade:
                    Core.PlayerMover.MoveTowards(target.Position);
                    return routine.GetDestructiblePower();
            }

            if (target.IsQuestGiver)
            {
                Core.PlayerMover.MoveTowards(target.Position);
                return InteractPower(target, 100, 250);
            }

            if (!TrinityCombat.IsInCombat)
                return null;

            TrinityPower routinePower = routine.GetOffensivePower();

            return TryKamakaziPower(target, routinePower, out TrinityPower kamakaziPower) ? kamakaziPower : routinePower;

        }

        public TrinityPower InteractPower(TrinityActor actor, int waitBefore, int waitAfter, float addedRange = 0)
        {
            return new TrinityPower(actor.IsUnit ? SNOPower.Axe_Operate_NPC : SNOPower.Axe_Operate_Gizmo,
                           actor.AxialRadius + addedRange, actor.Position, actor.AcdId, waitBefore, waitAfter);
        }

        public bool TryKamakaziPower(TrinityActor target, TrinityPower routinePower, out TrinityPower power)
        {
            // The routine may want us attack something other than current target, like best cluster, whatever.
            // But for goblin kamakazi we need a special exception to force it to always target the goblin.

            power = null;
            if (!target.IsTreasureGoblin
                || Core.Settings.Weighting.GoblinPriority != TargetPriority.Kamikaze
                || target.Position == Vector3.Zero) return false;

            Core.Logger.Log(LogCategory.Targetting, $"Forcing Kamakazi Target on {target}, routineProvided={routinePower}");

            TrinityPower kamaKaziPower = DefaultPower;
            if (routinePower != null)
            {
                routinePower.SetTarget(target);
                kamaKaziPower = routinePower;
            }

            power = kamaKaziPower;
            return true;
        }
        #endregion

        /// <summary>
        /// Checks if the element for a skill is close enough to the desired element to wait for it.
        /// </summary>
        /// <param name="skill">skill to check the damage type element of</param>
        /// <param name="element">element you want to be the current element</param>
        /// <param name="start"> we can cast for this long before the specified element starts</param>
        /// <param name="finish"> we can cast for this long after the specified element starts</param>
        /// <returns></returns>
        public static bool ShouldWaitForConventionofElements(Skill skill, Element element = Element.Unknown, int start = 0, int finish = 4000)
        {
            if (!Legendary.ConventionOfElements.IsEquipped)
                return false;

            if (Player.HasBuff(SNOPower.Pages_Buff_Infinite_Casting))
                return false;

            Element theElement = element == Element.Unknown ? skill.Element : element;
            var timeTo = TimeToElementStart(theElement);
            var timeSince = TimeFromElementStart(theElement);
            var totalDuration = GetConventionRotation().Count * 4000;

            if (timeTo < start)
                return false;

            if (timeSince < finish)
                return false;

            if ((timeTo - start) > GetRealCooldown(skill) &&
                GetRealCooldown(skill) < totalDuration)
            {
                return false;
            }

            return true;
        }

        public static double GetRealCooldown(Skill skill)
        {
            // todo refactor this, there is no need to hardcode these values

            double baseCd;
            double reduc = 1 - ZetaDia.Me.CommonData.GetAttribute<float>(ActorAttributeType.PowerCooldownReductionPercentAll);
            switch (skill.Name)
            {
                case "Iron Skin":
                    baseCd = 30000;
                    break;
                case "Provoke":
                    baseCd = 20000;
                    reduc = reduc * (1 - 0.35);
                    break;
                case "Bombardment":
                    baseCd = 60000;
                    reduc = reduc * (1 - 0.35);
                    break;
                case "Steed Charge":
                    baseCd = 16000;
                    reduc = reduc * (1 - 0.25);
                    break;
                default:
                    return skill.Cooldown.TotalMilliseconds;
            }
            if (Player.HasBuff(SNOPower.ItemPassive_Unique_Ring_919_x1))
            {
                baseCd = baseCd - 9000;
            }

            return baseCd * reduc;
        }

        /// <summary>
        /// Time (ms) until the start of an element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static double TimeToElementStart(Element element)
        {
            Cooldowns.CooldownData cd = Core.Cooldowns.GetBuffCooldown(SNOPower.P2_ItemPassive_Unique_Ring_038, 8);
            if (cd == null)
                return 0;

            List<Element> rotation = GetConventionRotation();
            var totalDuration = rotation.Count * 4000;
            var timeToFirst = cd.Remaining.TotalMilliseconds;

            var index = rotation.IndexOf(element);
            if (index < 0)
                return 0;

            return ((timeToFirst + index * 4000) % totalDuration);
        }

        /// <summary>
        /// Time (ms) since the last start of an element.
        /// </summary>
        public static double TimeFromElementStart(Element element)
        {
            return GetConventionRotation().Count * 4000 - TimeToElementStart(element);
        }

        /// <summary>
        /// The current rotation of elements for 'convention of elements' ring, starting from physical.
        /// </summary>
        public static List<Element> GetConventionRotation()
        {
            //Element.Arcane, Element.Cold, Element.Fire, Element.Holy, Element.Lightning, Element.Physical, Element.Poison

            switch (Core.Player.ActorClass)
            {
                case ActorClass.Crusader:
                    return CrusaderElements;
                case ActorClass.Wizard:
                    return WizardElements;
                case ActorClass.Barbarian:
                    return BarbarianElements;
                case ActorClass.DemonHunter:
                    return DemonHunterElements;
                case ActorClass.Witchdoctor:
                    return WitchdoctorElements;
                case ActorClass.Monk:
                    return MonkElements;
                case ActorClass.Necromancer:
                    return NecromancerElements;
            }
            return new List<Element>();
        }

        protected static readonly List<Element> CrusaderElements = new List<Element> { Element.Fire, Element.Holy, Element.Lightning, Element.Physical };
        protected static readonly List<Element> WizardElements = new List<Element> { Element.Arcane, Element.Cold, Element.Fire, Element.Lightning, };
        protected static readonly List<Element> BarbarianElements = new List<Element> { Element.Cold, Element.Fire, Element.Lightning, Element.Physical };
        protected static readonly List<Element> DemonHunterElements = new List<Element> { Element.Cold, Element.Fire, Element.Lightning, Element.Physical };
        protected static readonly List<Element> WitchdoctorElements = new List<Element> { Element.Cold, Element.Fire, Element.Physical, Element.Poison };
        protected static readonly List<Element> MonkElements = new List<Element> { Element.Cold, Element.Fire, Element.Holy, Element.Lightning, Element.Physical };
        protected static readonly List<Element> NecromancerElements = new List<Element> { Element.Cold, Element.Physical, Element.Poison };



        /// <summary>
        /// Walk towards a location with positional bonuses e.g. occulus damage bonus / serenity defensive bonus.
        /// </summary>
        /// <param name="power">Trinity power configured to move player towards a buffed position</param>
        /// <param name="maxDistance">maximum distance spot can be from player's current position</param>
        /// <param name="arriveDistance">how close to get to the middle of the spot before stopping walking</param>
        /// <returns>if a location was found and should be moved to</returns>
        public static bool TryMoveToBuffedSpot(out TrinityPower power, float maxDistance, float arriveDistance = 20f)
        {
            power = null;

            if (!IsInCombat ||
                IsCurrentlyKiting ||
                IsCurrentlyAvoiding)
            {
                return false;
            }

            if (!TargetUtil.BestBuffPosition(maxDistance, Player.Position, true, out Vector3 buffedLocation))
                return false;

            var distance = buffedLocation.Distance(Player.Position);

            Core.Logger.Verbose(LogCategory.Routine, $"Buffed location found Dist={distance}");

            if (buffedLocation.Distance(Player.Position) < arriveDistance)
                Core.Logger.Log(LogCategory.Routine, $"Standing in Buffed Position {buffedLocation} Dist={distance}");
            else if (!Core.Avoidance.Grid.CanRayWalk(Player.Position, buffedLocation))
                Core.Logger.Log(LogCategory.Routine, $"Unable to straight-line path to Buffed Position {buffedLocation} Dist={distance}");
            else if (!Core.Avoidance.Grid.CanRayWalk(TrinityCombat.Targeting.CurrentTarget.Position, buffedLocation))
                Core.Logger.Log(LogCategory.Routine, $"Can't see target from buffed position {buffedLocation} Dist={distance}");
            else if (Core.Avoidance.Avoider.IsKiteOnCooldown)
                Core.Logger.Log(LogCategory.Routine, $"Not moving to buffed location while on kite cooldown");
            else if (IsKitingEnabled && TargetUtil.AnyMobsInRangeOfPosition(buffedLocation, TrinityCombat.Routines.Current.KiteDistance))
                Core.Logger.Verbose(LogCategory.Routine, $"Moving to buffed spot would trigger kiting away from it.");
            else
            {
                Core.Logger.Verbose(LogCategory.Routine, $"Moving to Buffed Position {buffedLocation} Dist={distance}");
                power = new TrinityPower(SNOPower.Walk, maxDistance, buffedLocation);
                return true;
            }

            return false;
        }

        internal static bool IsKitingEnabled
        {
            get
            {
                switch (TrinityCombat.Routines.Current.KiteMode)
                {
                    case KiteMode.Always:
                        return true;
                    case KiteMode.Elites:
                        return TargetUtil.ElitesInRange(50f) > 0;
                    case KiteMode.Bosses:
                        return TargetUtil.AnyBossesInRange(50f);
                }
                return false;
            }
        }
    }
}
