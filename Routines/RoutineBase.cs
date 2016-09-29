using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Helpers;
using Trinity.Framework.Modules;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Settings.Combat;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines
{
    public class RoutineBase : NotifyBase
    {
        public virtual float EmergencyHealthPct => 0.50f;
        public virtual int ClusterSize => 1;
        public virtual float ClusterRadius => 20f;

        protected static PlayerCache Player
            => Core.Player;

        protected static TrinityActor CurrentTarget
            => Combat.Targeting.CurrentTarget;

        protected static IPartyProvider Party
            => Combat.Party;

        protected static IPartyMember PartyMe
            => Combat.Party.Members.FirstOrDefault(m => m.IsMe);

        protected static IPartyMember PartyLeader
            => Combat.Party.Leader;

        protected static PartyObjective MyPartyObjective
            => Combat.Party.Members.FirstOrDefault(m => m.IsMe)?.Objective ?? default(PartyObjective);

        protected static TrinityActor PartyTarget
            => PartyLeader.Target != null ? PartyHelper.FindLocalActor(PartyLeader.Target) : null;

        protected static TrinityPower CurrentPower
            => Combat.Targeting.CurrentPower;

        protected static IAvoider Avoider
            => Core.Avoidance.Avoider;        

        protected static IEnumerable<TrinityActor> Units
            => Core.Targets.Entries.Where(u => u.IsUnit && u.Weight > 0);

        protected static IEnumerable<TrinityActor> AllUnits
            => Core.Actors.AllRActors.Where(u => u.IsUnit && u.IsHostile);

        protected static IEnumerable<TrinityActor> AllUnitsInSight
            => AllUnits.Where(u => u.IsInLineOfSight);

        protected static TrinityPower Walk(TrinityActor target)
            => new TrinityPower(SNOPower.Walk, Math.Max(7f, target.AxialRadius), target.Position);

        protected static TrinityPower Walk(Vector3 destination, float range = 7f)
            => new TrinityPower(SNOPower.Walk, 7f, destination);

        protected static bool IsBlocked
            => PlayerMover.IsBlocked;

        protected static bool IsStuck
            => Core.StuckHandler.IsStuck;

        protected static int LargeClusterSize
            => (int)Math.Ceiling(Core.Routines.CurrentRoutine.ClusterSize * 1.25d);

        protected static bool IsCurrentlyAvoiding
            => Combat.IsCurrentlyAvoiding;

        protected static bool IsCurrentlyKiting
            => Combat.IsCurrentlyKiting;

        protected static bool IsInCombat
            => Combat.IsInCombat;

        protected static bool IsMultiPrimary
            => SkillUtils.Active.Count(s => s.IsGeneratorOrPrimary) > 1;

        protected static bool IsMultiSpender
            => SkillUtils.Active.Count(s => s.IsAttackSpender) > 1;

        protected static bool IsNoPrimary
            => SkillUtils.Active.Count(s => s.IsGeneratorOrPrimary) == 0;

        protected static bool ShouldRefreshBastiansGenerator
            => Sets.BastionsOfWill.IsFullyEquipped && !Core.Buffs.HasBastiansWillGeneratorBuff
            && SpellHistory.TimeSinceGeneratorCast >= 3750;

        protected static bool ShouldRefreshBastiansSpender
            => Sets.BastionsOfWill.IsFullyEquipped && !Core.Buffs.HasBastiansWillGeneratorBuff
            && SpellHistory.TimeSinceSpenderCast >= 3750;

        protected static int EndlessWalkOffensiveStacks
            => Core.Buffs.GetBuffStacks(447541, 1);

        protected static int EndlessWalkDefensiveStacks
            => Core.Buffs.GetBuffStacks(447541, 2);

        protected static bool HasInfiniteCasting
            => Core.Buffs.HasCastingShrine;

        protected static bool HasIngeomBuff
            => Player.HasBuff(SNOPower.ItemPassive_Unique_Ring_919_x1);

        protected static bool HasInstantCooldowns 
            => HasInfiniteCasting || HasIngeomBuff;

        protected bool AllowedToUse(SkillSettings settings, Skill skill)
        {
            if (settings == null || skill == null)
                return true;

            if (settings.UseTime == UseTime.Always)
                return true;

            if (settings.UseTime == UseTime.Never)
                return false;

            if (IsRestricted(settings, skill))
                return false;

            if (IsReasonToUse(settings, skill))
                return true;

            if (settings.UseTime == UseTime.Selective)
                return false;

            if (settings.UseTime == UseTime.OutOfCombat && IsInCombat)
                return false;

            if (settings.UseTime == UseTime.InCombat && !IsInCombat)
                return false;

            return true;
        }

        protected bool IsRestricted(SkillSettings settings, Skill skill)
        {
            if (Player.PrimaryResourcePct < settings.PrimaryResourcePct / 100)
                return true;

            if (Player.SecondaryResourcePct < settings.SecondaryResourcePct / 100)
                return true;

            if (SpellHistory.TimeSinceUse(skill.SNOPower).TotalMilliseconds < settings.RecastDelayMs)
                return true;
  
            if (settings.ClusterSize > 0 && !TargetUtil.ClusterExists(15f, settings.ClusterSize))
                return true;

            if (settings.WaitForConvention == ConventionMode.Never)
                return true;

            if (settings.WaitForConvention == ConventionMode.GreaterRift && !RiftProgression.IsGreaterRift)
                return true;

            if (settings.WaitForConvention == ConventionMode.Always && settings.ConventionCondition != null && !settings.ConventionCondition())
                return true;

            return false;
        }

        protected bool IsReasonToUse(SkillSettings settings, Skill skill)
        {
            if (settings.Reasons.HasFlag(UseReasons.Elites) && TargetUtil.AnyElitesInRange(40f))
                return true;

            if (settings.Reasons.HasFlag(UseReasons.Surrounded) && TargetUtil.NumMobsInRange(25f) >= Math.Max(ClusterSize, 5))
                return true;

            if (settings.Reasons.HasFlag(UseReasons.Avoiding) && IsCurrentlyAvoiding)
                return true;

            if (settings.Reasons.HasFlag(UseReasons.Blocked) && PlayerMover.IsBlocked)
                return true;

            if (settings.Reasons.HasFlag(UseReasons.DumpResource) && Player.PrimaryResourcePct < 0.8f)
                return true;

            if (settings.Reasons.HasFlag(UseReasons.Goblins) && Units.Any(u => u.IsTreasureGoblin))
                return true;

            if (settings.Reasons.HasFlag(UseReasons.HealthEmergency) && Player.CurrentHealthPct < Combat.Routines.Current.EmergencyHealthPct)
                return true;

            if (settings.Reasons.HasFlag(UseReasons.Buff) && settings.BuffCondition != null && settings.BuffCondition())
                return true;             
         
            return false;
        }

        protected static TrinityPower DefaultPower
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

        protected static SNOPower DefaultWeaponPower
        {
            get
            {
                var lhItem = Core.Inventory.Equipped.FirstOrDefault(i => i.InventorySlot == InventorySlot.LeftHand);
                if (lhItem == null)
                    return SNOPower.None;

                switch (lhItem.ItemType)
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

        protected internal static float DefaultWeaponDistance
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

        // returning true == you've handled it, and default handling is skipped.
        public virtual async Task<bool> HandleKiting() => false;
        public virtual async Task<bool> HandleAvoiding() => false;
        public virtual async Task<bool> HandleTargetInRange() => false;
        public virtual async Task<bool> MoveToTarget() => false;
        public virtual bool SetWeight(TrinityActor cacheObject) => false;

        #endregion

        /// <summary>
        /// Checks if the element for a skill is close enough to the desired element to wait for it.
        /// </summary>
        /// <param name="skill">skill to check the damage type element of</param>
        /// <param name="element">element you want to be the current element</param>
        /// <param name="start"> we can cast for this long before the specified element starts</param>
        /// <param name="finish"> we can cast for this long after the specified element starts</param>
        /// <returns></returns>
        protected static bool ShouldWaitForConventionofElements(Skill skill, Element element = Element.Unknown, int start = 0, int finish = 4000)
        {
            if (!Legendary.ConventionOfElements.IsEquipped)
                return false;

            if (Player.HasBuff(SNOPower.Pages_Buff_Infinite_Casting))
                return false;

            var theElement = element == Element.Unknown ? skill.Element : element;
            var timeTo = TimeToElementStart(theElement);
            var timeSince = TimeFromElementStart(theElement);
            var totalDuration = GetConventionRotation().Count * 4000;

            if (timeTo < start)
                return false;

            if (timeSince < finish)
                return false;

            if ((timeTo - start) > GetRealCooldown(skill) && GetRealCooldown(skill) < totalDuration)
            {
                return false;
            }

            return true;
        }

        protected static double GetRealCooldown(Skill skill)
        {
            // todo refactor this, there is no need to hardcode these values

            double baseCd = 0;
            double reduc = 1 - ZetaDia.Me.CommonData.GetAttribute<float>(ActorAttributeType.PowerCooldownReductionPercentAll);
            switch (skill.Name)
            {
                case "Iron Skin":
                    baseCd = 30000;
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
                baseCd = baseCd - 9000;
            return baseCd * reduc;
        }

        /// <summary>
        /// Time (ms) until the start of an element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected static double TimeToElementStart(Element element)
        {
            var cd = Core.Cooldowns.GetBuffCooldown(SNOPower.P2_ItemPassive_Unique_Ring_038, 8);
            if (cd == null)
                return 0;

            var rotation = GetConventionRotation();
            var totalDuration = rotation.Count * 4000;
            var timeToFirst = cd.Remaining.TotalMilliseconds;

            var index = rotation.IndexOf(element);
            if (index < 0) return 0;

            return ((timeToFirst + index * 4000) % totalDuration);
        }

        /// <summary>
        /// Time (ms) since the last start of an element.
        /// </summary>
        protected static double TimeFromElementStart(Element element)
        {
            return GetConventionRotation().Count * 4000 - TimeToElementStart(element);
        }

        /// <summary>
        /// The current rotation of elements for 'convention of elements' ring, starting from physical.
        /// </summary>
        protected static List<Element> GetConventionRotation()
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
            }
            return new List<Element>();
        }

        protected static readonly List<Element> CrusaderElements = new List<Element> { Element.Fire, Element.Holy, Element.Lightning, Element.Physical };
        protected static readonly List<Element> WizardElements = new List<Element> { Element.Arcane, Element.Cold, Element.Fire, Element.Lightning, };
        protected static readonly List<Element> BarbarianElements = new List<Element> { Element.Cold, Element.Fire, Element.Lightning, Element.Physical };
        protected static readonly List<Element> DemonHunterElements = new List<Element> { Element.Cold, Element.Fire, Element.Lightning, Element.Physical };
        protected static readonly List<Element> WitchdoctorElements = new List<Element> { Element.Cold, Element.Fire, Element.Physical, Element.Poison };
        protected static readonly List<Element> MonkElements = new List<Element> { Element.Cold, Element.Fire, Element.Holy, Element.Lightning, Element.Physical };



        /// <summary>
        /// Walk towards a location with positional bonuses e.g. occulus damage bonus / serenity defensive bonus.
        /// </summary>
        /// <param name="power">Trinity power configured to move player towards a buffed position</param>
        /// <param name="maxDistance">maximum distance spot can be from player's current position</param>
        /// <param name="arriveDistance">how close to get to the middle of the spot before stopping walking</param>
        /// <returns>if a location was found and should be moved to</returns>
        protected static bool TryMoveToBuffedSpot(out TrinityPower power, float maxDistance, float arriveDistance = 20f)
        {
            power = null;

            if (IsInCombat && !IsCurrentlyKiting && !IsCurrentlyAvoiding)
            {
                Vector3 buffedLocation;
                if (TargetUtil.BestBuffPosition(maxDistance, Player.Position, true, out buffedLocation))
                {
                    //var lastPower = SpellHistory.LastPower;
                    var distance = buffedLocation.Distance(Player.Position);

                    Logger.LogVerbose(LogCategory.Routine, $"Buffed location found Dist={distance}");

                    if (buffedLocation.Distance(Player.Position) < arriveDistance)
                    {
                        Logger.Log(LogCategory.Routine, $"Standing in Buffed Position {buffedLocation} Dist={distance}");
                    }
                    else if (!Core.Avoidance.Grid.CanRayWalk(Player.Position, buffedLocation))
                    {
                        Logger.Log(LogCategory.Routine, $"Unable to straight-line path to Buffed Position {buffedLocation} Dist={distance}");
                    }
                    else if (!Core.Avoidance.Grid.CanRayWalk(Combat.Targeting.CurrentTarget.Position, buffedLocation))
                    {
                        Logger.Log(LogCategory.Routine, $"Can't see target from buffed position {buffedLocation} Dist={distance}");
                    }
                    else if (Core.Avoidance.Avoider.IsKiteOnCooldown)
                    {
                        Logger.Log(LogCategory.Routine, $"Not moving to buffed location while on kite cooldown");
                    }
                    //else if (checkPowerRange && lastPower != null && buffedLocation.Distance(Combat.Targeting.CurrentTarget.Position) > lastPower.MinimumRange + Combat.Targeting.CurrentTarget.CollisionRadius + Player.Radius)
                    //{
                    //    Logger.LogVerbose(LogCategory.Routine, $"Buffed spot outside attack range for power {lastPower.SNOPower} Range={lastPower.MinimumRange} TimeSinceUse={lastPower.TimeSinceUseMs} Dist={distance}");
                    //}
                    else if (IsKitingEnabled && TargetUtil.AnyMobsInRangeOfPosition(buffedLocation, Combat.Routines.Current.KiteDistance))
                    {
                        Logger.LogVerbose(LogCategory.Routine, $"Moving to buffed spot would trigger kiting away from it.");
                    }
                    else
                    {
                        Logger.LogVerbose(LogCategory.Routine, $"Moving to Buffed Position {buffedLocation} Dist={distance}");
                        power = new TrinityPower(SNOPower.Walk, maxDistance, buffedLocation);
                        return true;                           
                    } 
                }
            }            
            return false;
        }

        internal static bool IsKitingEnabled
        {
            get
            {
                switch (Combat.Routines.Current.KiteMode)
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



