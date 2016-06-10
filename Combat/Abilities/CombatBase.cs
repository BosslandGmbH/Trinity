using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Trinity.Cache;
using Trinity.Combat.Abilities.PhelonsPlayground;
using Trinity.Config;
using Trinity.Config.Combat;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Utilities;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Reference;
using Trinity.Technicals;
using Trinity.UIComponents;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Combat.Abilities
{
    public class CombatBase
    {
        static CombatBase()
        {
            GameEvents.OnGameJoined += (sender, args) => LoadCombatSettings();

            LoadCombatSettings();

            Pulsator.OnPulse += (sender, args) => Debug();
        }

        public static bool IsInParty => ZetaDia.Service?.Party?.NumPartyMembers > 1;
        private static TrinityPower _currentPower = new TrinityPower();
        private static bool _isCombatAllowed = true;
        private static KiteMode _kiteMode = KiteMode.Never;

        public static CombatMovementManager CombatMovement = new CombatMovementManager();
        internal static Vector3 PositionLastZigZagCheck { get; set; }

        public enum CanCastFlags
        {
            All = 2,
            NoTimer = 4,
            NoPowerManager = 8,
            Timer = 16
        }

        /// <summary>
        /// Returns an amount of resource adjusted for resource cost reduction and rounded.
        /// </summary>
        public static double GetAdjustedCost(int amount)
        {
            return Math.Round(amount * (1 - Player.ResourceCostReductionPct), 0, MidpointRounding.AwayFromZero);
        }


        internal static void LoadCombatSettings()
        {
            EmergencyHealthPotionLimit = Settings.Combat.Misc.PotionLevel;
            EmergencyHealthGlobeLimit = Settings.Combat.Misc.HealthGlobeLevel;
            HealthGlobeResource = Settings.Combat.Misc.HealthGlobeLevelResource;

            KiteMode = Settings.Avoidance.KiteMode;
            KiteDistance = (int)Settings.Avoidance.KiteDistance;

            //switch (Player.ActorClass)
            //{
            //    case ActorClass.Barbarian:
            //        KiteDistance = Settings.Combat.Barbarian.KiteLimit;
            //        KiteMode = KiteMode.Never;
            //        break;

            //    case ActorClass.Crusader:
            //        KiteDistance = 0;
            //        KiteMode = KiteMode.Never;
            //        break;

            //    case ActorClass.Monk:
            //        KiteDistance = 0;
            //        KiteMode = KiteMode.Never;
            //        break;

            //    case ActorClass.Wizard:
            //        KiteDistance = Settings.Combat.Wizard.KiteLimit;
            //        KiteMode = KiteMode.Always;
            //        break;

            //    case ActorClass.Witchdoctor:
            //        KiteDistance = Settings.Combat.WitchDoctor.KiteLimit;
            //        KiteMode = KiteMode.Always;
            //        break;

            //    case ActorClass.DemonHunter:
            //        KiteDistance = Settings.Combat.DemonHunter.KiteLimit;
            //        KiteMode = Settings.Combat.DemonHunter.KiteMode;
            //        break;
            //}

            // Monk Seven Sided Strike: Sustained Attack
            if (Player.ActorClass == ActorClass.Monk && CacheData.Hotbar.ActiveSkills.Any(s => s.Power == SNOPower.Monk_SevenSidedStrike && s.RuneIndex == 3))
                SkillsDefaultMeta.Monk.SevensidedStrike.ReUseDelay = 17000;

            if (Player.ActorClass == ActorClass.Witchdoctor && Passives.WitchDoctor.GraveInjustice.IsActive)
            {
                SkillsDefaultMeta.WitchDoctor.SoulHarvest.ReUseDelay = 1000;
                SkillsDefaultMeta.WitchDoctor.SpiritWalk.ReUseDelay = 1000;
                SkillsDefaultMeta.WitchDoctor.Horrify.ReUseDelay = 1000;
                SkillsDefaultMeta.WitchDoctor.Gargantuan.ReUseDelay = 20000;
                SkillsDefaultMeta.WitchDoctor.SummonZombieDogs.ReUseDelay = 20000;
                SkillsDefaultMeta.WitchDoctor.GraspOfTheDead.ReUseDelay = 500;
                SkillsDefaultMeta.WitchDoctor.SpiritBarrage.ReUseDelay = 2000;
                SkillsDefaultMeta.WitchDoctor.LocustSwarm.ReUseDelay = 2000;
                SkillsDefaultMeta.WitchDoctor.Haunt.ReUseDelay = 2000;
                SkillsDefaultMeta.WitchDoctor.Hex.ReUseDelay = 3000;
                SkillsDefaultMeta.WitchDoctor.MassConfusion.ReUseDelay = 15000;
                SkillsDefaultMeta.WitchDoctor.FetishArmy.ReUseDelay = 20000;
                SkillsDefaultMeta.WitchDoctor.BigBadVoodoo.ReUseDelay = 20000;
            }

            if (Player.ActorClass == ActorClass.Barbarian && CacheData.Hotbar.PassiveSkills.Contains(SNOPower.Barbarian_Passive_BoonOfBulKathos))
            {
                SkillsDefaultMeta.Barbarian.Earthquake.ReUseDelay = 90500;
                SkillsDefaultMeta.Barbarian.CallOfTheAncients.ReUseDelay = 90500;
                SkillsDefaultMeta.Barbarian.WrathOfTheBerserker.ReUseDelay = 90500;
            }

            if (Player.ActorClass == ActorClass.Barbarian)
            {
                // Barbarian Spells
                SkillsDefaultMeta.Barbarian.Bash.ReUseDelay = 5;
                SkillsDefaultMeta.Barbarian.Cleave.ReUseDelay = 5;
                SkillsDefaultMeta.Barbarian.Frenzy.ReUseDelay = 5;
                SkillsDefaultMeta.Barbarian.HammerOfTheAncients.ReUseDelay = 150;
                SkillsDefaultMeta.Barbarian.Rend.ReUseDelay = 3500;
                SkillsDefaultMeta.Barbarian.SeismicSlam.ReUseDelay = 200;
                SkillsDefaultMeta.Barbarian.Whirlwind.ReUseDelay = 5;
                SkillsDefaultMeta.Barbarian.GroundStomp.ReUseDelay = 12200;
                SkillsDefaultMeta.Barbarian.Leap.ReUseDelay = 10200;
                SkillsDefaultMeta.Barbarian.Sprint.ReUseDelay = 2800;
                SkillsDefaultMeta.Barbarian.IgnorePain.ReUseDelay = 30200;
                SkillsDefaultMeta.Barbarian.AncientSpear.ReUseDelay = 300;
                SkillsDefaultMeta.Barbarian.Revenge.ReUseDelay = 600;
                SkillsDefaultMeta.Barbarian.FuriousCharge.ReUseDelay = 500;
                SkillsDefaultMeta.Barbarian.Overpower.ReUseDelay = 200;
                SkillsDefaultMeta.Barbarian.ThreateningShout.ReUseDelay = 10200;
                SkillsDefaultMeta.Barbarian.BattleRage.ReUseDelay = 118000;
                SkillsDefaultMeta.Barbarian.WarCry.ReUseDelay = 20500;
                SkillsDefaultMeta.Barbarian.Earthquake.ReUseDelay = 120500;
                SkillsDefaultMeta.Barbarian.CallOfTheAncients.ReUseDelay = 120500;
                SkillsDefaultMeta.Barbarian.WrathOfTheBerserker.ReUseDelay = 120500;
            }

            if (Player.ActorClass == ActorClass.Monk)
            {
                // Monk skills
                SkillsDefaultMeta.Monk.FistsOfThunder.ReUseDelay = 5;
                SkillsDefaultMeta.Monk.DeadlyReach.ReUseDelay = 5;
                SkillsDefaultMeta.Monk.CripplingWave.ReUseDelay = 5;
                SkillsDefaultMeta.Monk.WayOfTheHundredFists.ReUseDelay = 5;
                SkillsDefaultMeta.Monk.LashingTailKick.ReUseDelay = 250;
                SkillsDefaultMeta.Monk.TempestRush.ReUseDelay = 15;
                SkillsDefaultMeta.Monk.WaveOfLight.ReUseDelay = 750;
                SkillsDefaultMeta.Monk.BlindingFlash.ReUseDelay = 15200;
                SkillsDefaultMeta.Monk.BreathOfHeaven.ReUseDelay = 15200;
                SkillsDefaultMeta.Monk.Serenity.ReUseDelay = 20200;
                SkillsDefaultMeta.Monk.InnerSanctuary.ReUseDelay = 20200;
                SkillsDefaultMeta.Monk.DashingStrike.ReUseDelay = 1000;
                SkillsDefaultMeta.Monk.ExplodingPalm.ReUseDelay = 250;
                SkillsDefaultMeta.Monk.SweepingWind.ReUseDelay = 1500;
                SkillsDefaultMeta.Monk.CycloneStrike.ReUseDelay = 900;
                SkillsDefaultMeta.Monk.SevensidedStrike.ReUseDelay = 30200;
                SkillsDefaultMeta.Monk.MysticAlly.ReUseDelay = 30000;
                SkillsDefaultMeta.Monk.MantraOfSalvation.ReUseDelay = 2800;
                SkillsDefaultMeta.Monk.MantraOfRetribution.ReUseDelay = 2800;
                SkillsDefaultMeta.Monk.MantraOfHealing.ReUseDelay = 2800;
                SkillsDefaultMeta.Monk.MantraOfConviction.ReUseDelay = 2800;
                SkillsDefaultMeta.Monk.Epiphany.ReUseDelay = 60000;
            }

            if (Player.ActorClass == ActorClass.Wizard)
            {
                // Wizard skills
                SkillsDefaultMeta.Wizard.MagicMissile.ReUseDelay = 5;
                SkillsDefaultMeta.Wizard.ShockPulse.ReUseDelay = 5;
                SkillsDefaultMeta.Wizard.SpectralBlade.ReUseDelay = 5;
                SkillsDefaultMeta.Wizard.Electrocute.ReUseDelay = 5;
                SkillsDefaultMeta.Wizard.RayOfFrost.ReUseDelay = 5;
                SkillsDefaultMeta.Wizard.ArcaneOrb.ReUseDelay = 5;
                SkillsDefaultMeta.Wizard.ArcaneTorrent.ReUseDelay = 5;
                SkillsDefaultMeta.Wizard.Disintegrate.ReUseDelay = 5;
                SkillsDefaultMeta.Wizard.FrostNova.ReUseDelay = 5;
                SkillsDefaultMeta.Wizard.DiamondSkin.ReUseDelay = 15000;
                SkillsDefaultMeta.Wizard.SlowTime.ReUseDelay = 16000;
                SkillsDefaultMeta.Wizard.Teleport.ReUseDelay = 250;
                SkillsDefaultMeta.Wizard.WaveOfForce.ReUseDelay = 5;
                SkillsDefaultMeta.Wizard.EnergyTwister.ReUseDelay = 5;
                SkillsDefaultMeta.Wizard.Hydra.ReUseDelay = 12000;
                SkillsDefaultMeta.Wizard.Meteor.ReUseDelay = 1000;
                SkillsDefaultMeta.Wizard.Blizzard.ReUseDelay = 500;
                SkillsDefaultMeta.Wizard.IceArmor.ReUseDelay = 60000;
                SkillsDefaultMeta.Wizard.StormArmor.ReUseDelay = 60000;
                SkillsDefaultMeta.Wizard.EnergyArmor.ReUseDelay = 60000;
                SkillsDefaultMeta.Wizard.MagicWeapon.ReUseDelay = 60000;
                SkillsDefaultMeta.Wizard.Familiar.ReUseDelay = 60000;
                SkillsDefaultMeta.Wizard.ExplosiveBlast.ReUseDelay = 6000;
                SkillsDefaultMeta.Wizard.MirrorImage.ReUseDelay = 5000;
                SkillsDefaultMeta.Wizard.Archon.ReUseDelay = 100000;
            }
        }

        private static int _kiteDistance;
        /// <summary>
        /// Distance to kite, read settings (class independant)
        /// </summary>
        public static int KiteDistance
        {
            get
            {
                // Conduit Pylon buff is active, no kite distance
                if (CacheData.Buffs.HasConduitPylon)
                    return 0;

                if (KiteMode == KiteMode.Never)
                    return 0;

                return _kiteDistance;
            }
            set { _kiteDistance = value; }
        }

        public static float EmergencyHealthPotionLimit { get; set; }
        public static float EmergencyHealthGlobeLimit { get; set; }
        public static float HealthGlobeResource { get; set; }

        // When to Kite
        public static KiteMode KiteMode
        {
            get { return _kiteMode; }
            set { _kiteMode = value; }
        }

        /// <summary>
        /// Allows for completely disabling combat. Settable through API only.
        /// </summary>
        public static bool IsCombatAllowed
        {
            get
            {
                if (CombatMode == CombatMode.KillAll)
                    return true;

                // if disabled in the profile, or disabled through api
                if (!CombatTargeting.Instance.AllowedToKillMonsters)
                    return false;

                if (!_isCombatAllowed)
                    return false;

                if (CombatMode == CombatMode.On)
                    return true;

                if (CombatMode == CombatMode.Off || CombatMode == CombatMode.SafeZerg && TargetUtil.NumMobsInRangeOfPosition(TrinityPlugin.Player.Position, 10f) > 4)
                    return true;

                return true;
            }
            set { _isCombatAllowed = value; }
        }

        public static CombatMode CombatMode
        {
            get
            {
                return _combatMode;
            }
            set
            {
                Logger.Log("CombatMode was set to: {0}", value);
                LastCombatMode = _combatMode;
                _combatMode = value;
            }
        }

        public static CombatMode LastCombatMode { get; set; }

        public static bool IsQuestingMode { get; set; }

        /// <summary>
        /// A dictionary containing the date time we last used a specific spell
        /// </summary>
        public static Dictionary<SNOPower, DateTime> AbilityLastUsedCache
        {
            get
            {
                return CacheData.AbilityLastUsed;
            }
            set
            {
                CacheData.AbilityLastUsed = value;
            }
        }

        /// <summary>
        /// Always contains the last power used
        /// </summary>
        public static SNOPower LastPowerUsed
        {
            get
            {
                return TrinityPlugin.LastPowerUsed;
            }
        }

        /// <summary>
        /// Minimum energy reserve for using "Big" spells/powers
        /// </summary>
        public static int EnergyReserve { get; set; }


        /// <summary>
        /// Arcane, Frozen, Jailer, Molten, Electrified+Reflect Damage elites
        /// </summary>
        public static bool HardElitesPresent
        {
            get
            {
                return
                   TrinityPlugin.ObjectCache.Any(o => o.IsEliteRareUnique &&
                          o.MonsterAffixes.HasFlag(MonsterAffixes.ArcaneEnchanted | MonsterAffixes.Frozen | MonsterAffixes.Jailer | MonsterAffixes.Molten | MonsterAffixes.Nightmarish) ||
                          (o.MonsterAffixes.HasFlag(MonsterAffixes.Electrified) && o.MonsterAffixes.HasFlag(MonsterAffixes.ReflectsDamage))) ||
                        TrinityPlugin.ObjectCache.Any(o => o.IsBoss);
            }
        }

        public static bool IgnoringElites
        {
            get
            {
                return !IsQuestingMode && Settings.Combat.Misc.IgnoreElites;
            }
        }

        public static TrinitySetting Settings
        {
            get { return TrinityPlugin.Settings; }
        }

        public static bool UseOOCBuff
        {
            get
            {
                if (CurrentTarget == null)
                    return true;
                return false;
            }
        }

        public static readonly Func<bool> BigClusterOrElitesInRange = () => TargetUtil.AnyElitesInRange(20f) || TargetUtil.NumMobsInRange(30f) >= 10;

        public static bool IsCurrentlyAvoiding
        {
            get
            {
                if (CurrentTarget == null)
                    return false;

                if (CurrentTarget.IsSafeSpot && Core.Avoidance.Avoider.ShouldAvoid)
                    return true;

                return false;
            }
        }

        public static bool IsCurrentlyKiting
        {
            get
            {
                if (CurrentTarget == null)
                    return false;

                if (CurrentTarget.IsSafeSpot && Core.Avoidance.Avoider.ShouldKite)
                    return true;

                return false;
            }
        }

        public static bool UseDestructiblePower
        {
            get
            {
                if (CurrentTarget == null)
                    return false;

                switch (CurrentTarget.Type)
                {
                    case TrinityObjectType.Destructible:
                    case TrinityObjectType.Barricade:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public static TrinityPower CurrentPower
        {
            get { return _currentPower; }
            set { _currentPower = value; }
        }

        public static HashSet<SNOPower> Hotbar
        {
            get
            {
                return CacheData.Hotbar.ActivePowers;
            }
        }
        public static CacheData.PlayerCache Player
        {
            get
            {
                return CacheData.Player;
            }
        }

        public static TrinityCacheObject CurrentTarget
        {
            get
            {
                return TrinityPlugin.CurrentTarget;
            }
        }

        public static TrinityPower DefaultPower
        {
            get
            {
                // Default attacks
                if (!UseOOCBuff && !IsCurrentlyAvoiding)
                {
                    return new TrinityPower
                    {
                        SNOPower = DefaultWeaponPower,
                        MinimumRange = DefaultWeaponDistance,
                        TargetACDGUID = CurrentTarget.ACDGuid,
                    };
                }
                return new TrinityPower();
            }
        }

        /// <summary>
        /// Gets the default weapon power based on the current equipped primary weapon
        /// </summary>
        /// <returns></returns>
        public static SNOPower DefaultWeaponPower
        {
            get
            {
                ACDItem lhItem = CacheData.Inventory.Equipped.FirstOrDefault(i => i.InventorySlot == InventorySlot.LeftHand);
                if (lhItem == null)
                    return SNOPower.None;

                switch (lhItem.ItemType)
                {
                    default:
                        return SNOPower.Weapon_Melee_Instant;
                    case ItemType.Axe:
                    case ItemType.CeremonialDagger:
                    case ItemType.Dagger:
                    case ItemType.Daibo:
                    case ItemType.FistWeapon:
                    case ItemType.Mace:
                    case ItemType.Polearm:
                    case ItemType.Spear:
                    case ItemType.Staff:
                    case ItemType.Sword:
                    case ItemType.MightyWeapon:
                        return SNOPower.Weapon_Melee_Instant;
                    case ItemType.Wand:
                        return SNOPower.Weapon_Ranged_Wand;
                    case ItemType.Bow:
                    case ItemType.Crossbow:
                    case ItemType.HandCrossbow:
                        return SNOPower.Weapon_Ranged_Projectile;
                }
            }
        }
        /// <summary>
        /// Gets the default weapon distance based on the current equipped primary weapon
        /// </summary>
        /// <returns></returns>
        public static float DefaultWeaponDistance
        {
            get
            {
                switch (DefaultWeaponPower)
                {
                    case SNOPower.Weapon_Ranged_Instant:
                    case SNOPower.Weapon_Ranged_Projectile:
                        return 65f;
                    case SNOPower.Weapon_Ranged_Wand:
                        return 55f;
                    default:
                        return 12f;
                }
            }
        }

        /// <summary>
        /// Performs basic checks to see if we have and can cast a power (hotbar, power manager). Checks use timer for Wiz, DH
        /// </summary>
        /// <param name="power"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool CanCast(SNOPower power, CanCastFlags flags = CanCastFlags.All)
        {
            bool hasPower = Hotbar.Contains(power);
            if (!hasPower)
                return false;

            var isTimerCheckClass = Player.ActorClass == ActorClass.DemonHunter;
            if ((isTimerCheckClass || flags.HasFlag(CanCastFlags.Timer)) && !flags.HasFlag(CanCastFlags.NoTimer))
            {
                var skill = SkillUtils.Active.FirstOrDefault(s => s.SNOPower == power);
                if (skill == null)
                    return false;

                var meta = skill.Meta;
                var hasBeenUsedTooRecently = meta.ReUseDelay > 0 && TimeSincePowerUse(skill.SNOPower) < meta.ReUseDelay;
                if (hasBeenUsedTooRecently)
                    return false;
            }

            bool powerManager = flags.HasFlag(CanCastFlags.NoPowerManager) || PowerManager.CanCast(power);
            if (!powerManager)
                return false;

            return true;
        }

        /// <summary>
        /// Check if a particular buff is present
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public static bool GetHasBuff(SNOPower power)
        {
            return CacheData.Buffs.HasBuff(power);
        }

        /// <summary>
        /// Returns how many stacks of a particular buff there are
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public static int GetBuffStacks(SNOPower power)
        {
            return CacheData.Buffs.GetBuffStacks(power);
        }

        ///// <summary>
        ///// Check re-use timers on skills
        ///// </summary>
        ///// <param name="power">The power.</param>
        ///// <param name="recheck">if set to <c>true</c> check again.</param>
        ///// <returns>
        ///// Returns whether or not we can use a skill, or if it's on our own internal TrinityPlugin cooldown timer
        ///// </returns>
        //public static bool SNOPowerUseTimer(SNOPower power, bool recheck = false)
        //{
        //    if (TimeSincePowerUse(power) >= GetSNOPowerUseDelay(power))
        //        return true;
        //    if (recheck && TimeSincePowerUse(power) >= 150 && TimeSincePowerUse(power) <= 600)
        //        return true;
        //    return false;
        //}

        //public static void SetSNOPowerUseDelay(SNOPower power, double delay)
        //{
        //    string key = "SpellDelay." + power.ToString();

        //    if (!V.Data.ContainsKey(key))
        //    {
        //        Logger.LogDebug("Failed to Set TVar {0} - key doesnt exist");
        //        return;
        //    }


        //    TVar v = V.Data[key];

        //    bool hasDefaultValue = v.Value == v.DefaultValue;

        //    if (hasDefaultValue)
        //    {
        //        // Create a new TVar (changes the default value)
        //        V.Set(new TVar(v.Name, delay, v.Description));
        //    }
        //}

        //public static double GetSNOPowerUseDelay(SNOPower power)
        //{
        //    double delay = V.D("SpellDelay." + power);

        //    if (GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting))
        //    {
        //        delay = delay * 0.25d;
        //    }

        //    return delay;
        //}

        private static int _lastComboLevel;

        [Execution.TrackMethod]
        public static int GetCurrentComboLevel()
        {
            if (Execution.Restrict("GetCurrentComboLevel", 100)) return _lastComboLevel;
            return _lastComboLevel = ZetaDia.Me.CommonData.ComboLevel;
        }

        /// <summary>
        /// Returns true if we have the ability and the buff is up, or true if we don't have the ability in our hotbar
        /// </summary>
        /// <param name="snoPower"></param>
        /// <returns></returns>
        internal static bool CheckAbilityAndBuff(SNOPower snoPower)
        {
            return
                (!CacheData.Hotbar.ActivePowers.Contains(snoPower) || (CacheData.Hotbar.ActivePowers.Contains(snoPower) && GetHasBuff(snoPower)));
        }

        /// <summary>
        /// Returns how many stacks of a particular skill there are
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public static int GetSkillCharges(SNOPower power)
        {
            return CacheData.Hotbar.GetSkillCharges(power);
        }

        /// <summary>
        /// Gets the time in Millseconds since we've used the specified power
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        internal static double TimeSincePowerUse(SNOPower power)
        {
            return SpellHistory.TimeSinceUse(power).TotalMilliseconds;
        }

        internal static bool WasUsedWithinMilliseconds(SNOPower power, float timeMs)
        {
            var timeSinceCast = TimeSincePowerUse(power);
            return timeSinceCast > 0 && timeSinceCast < timeMs;
        }

        /// <summary>
        /// Gets the time in Millseconds since we've used the specified power
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        internal static TimeSpan TimeSpanSincePowerUse(SNOPower power)
        {
            if (CacheData.AbilityLastUsed.ContainsKey(power))
                return DateTime.UtcNow.Subtract(CacheData.AbilityLastUsed[power]);
            return TimeSpan.MinValue;
        }

        /// <summary>
        /// Check if a power is null
        /// </summary>
        /// <param name="power"></param>
        protected static bool IsNull(TrinityPower power)
        {
            return power == null;
        }

        /// <summary>
        /// Checks a skill against the convention of elements ring
        /// </summary>
        internal static bool ShouldWaitForConventionElement(Skill skill)
        {
            if (!Settings.Combat.Misc.UseConventionElementOnly)
                return false;

            if (Settings.Combat.Misc.IgnoreCoEunlessGRift && ZetaDia.CurrentRift.Type != RiftType.Greater)
                return false;

            return Legendary.ConventionOfElements.IsEquipped && CacheData.Buffs.ConventionElement != skill.Element;
        }

        public static void Debug()
        {
            //Logger.Log(
            //   "CD={0} ToElement={1} FromElement={2} ShouldWait={3} CurEl={4}",
            //   Skills.Crusader.Bombardment.CooldownRemaining,
            //   TimeToElementStart(Element.Physical),
            //   TimeFromElementStart(Element.Physical),
            //   ShouldWaitForConventionofElements(Skills.Crusader.Bombardment, Element.Physical, 1500, 1500),
            //   CacheData.Buffs.ConventionElement
            //);
        }

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
            if (!Settings.Combat.Misc.UseConventionElementOnly)
                return false;

            if (!Legendary.ConventionOfElements.IsEquipped)
                return false;

            if (GetHasBuff(SNOPower.Pages_Buff_Infinite_Casting))
                return false;

            if (Settings.Combat.Misc.IgnoreCoEunlessGRift && ZetaDia.CurrentRift.Type != RiftType.Greater)
                return false;

            var theElement = element == Element.Unknown ? skill.Element : element;
            var timeTo = TimeToElementStart(theElement);
            var timeSince = TimeFromElementStart(theElement);
            var totalDuration = GetConventionRotation().Count*4000;

            if (timeTo < start)
                return false;

            if (timeSince < finish)
                return false;

            //if (timeTo - start > skill.Cooldown.TotalMilliseconds &&
            //    skill.Cooldown.TotalMilliseconds < totalDuration)
            //    return false;

            if ((timeTo - start) > GetRealCooldown(skill) &&
                GetRealCooldown(skill) < totalDuration)
            {
                return false;
            }

            return true;
        }

        private static double GetRealCooldown(Skill skill)
        {
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
            if (GetHasBuff(SNOPower.ItemPassive_Unique_Ring_919_x1))
                baseCd = baseCd - 9000;
            return baseCd * reduc;
        }

        /// <summary>
        /// Checks if we're inside a given time span based on a convention element.
        /// </summary>
        /// <param name="element">element you want to be the current element</param>
        /// <param name="start"> how long before the specified element starts</param>
        /// <param name="finish"> how long after the specified element starts</param>
        /// <returns></returns>
        public static bool IsInsideCoeTimeSpan(Element element = Element.Unknown, int start = 0, int finish = 4000)
        {

            if (!Settings.Combat.Misc.UseConventionElementOnly)
                return false;

            if (!Legendary.ConventionOfElements.IsEquipped)
                return false;

            var timeTo = TimeToElementStart(element);
            var timeSince = TimeFromElementStart(element);

            if (timeTo < start)
                return true;

            if (timeSince < finish)
                return true;

            return false;
        }

        /// <summary>
        /// Time (ms) until the start of an element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static double TimeToElementStart(Element element)
        {
            var cd = Core.Cooldowns.GetBuffCooldown(SNOPower.P2_ItemPassive_Unique_Ring_038, 8);
            if (cd == null)
                return 0;

            var rotation = GetConventionRotation();
            var totalDuration = rotation.Count * 4000;
            var timeToFirst = cd.Remaining.TotalMilliseconds;

            var index = rotation.IndexOf(element);
            if (index < 0) return 0;

            //var time = timeToFirst - (rotation.Count - index) * 4000;
            //return time > 0 ? time : totalDuration - Math.Abs(time);

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

            switch (TrinityPlugin.Player.ActorClass)
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
                    return CrusaderElements;
            }
            return new List<Element>();
        }

        public static readonly List<Element> CrusaderElements = new List<Element> { Element.Fire, Element.Holy, Element.Lightning, Element.Physical };
        public static readonly List<Element> WizardElements = new List<Element> { Element.Arcane, Element.Cold, Element.Fire, Element.Lightning, };
        public static readonly List<Element> BarbarianElements = new List<Element> { Element.Cold, Element.Fire, Element.Lightning, Element.Physical };
        public static readonly List<Element> DemonHunterElements = new List<Element> { Element.Cold, Element.Fire, Element.Lightning, Element.Physical };
        public static readonly List<Element> WitchdoctorElements = new List<Element> { Element.Cold, Element.Fire, Element.Physical, Element.Poison };
        public static readonly List<Element> MonkElements = new List<Element> { Element.Cold, Element.Fire, Element.Holy, Element.Lightning, Element.Physical };


        /// <summary>
        /// If we should attack to aquire the Bastians generator buff
        /// </summary>
        internal static bool ShouldRefreshBastiansGeneratorBuff
        {
            get
            {
                if (Sets.BastionsOfWill.IsFullyEquipped && !CacheData.Buffs.HasBastiansWillGeneratorBuff)
                    return true;

                if (!Sets.BastionsOfWill.IsFullyEquipped)
                    return false;

                // Some Generators take a while to actually hit something (chakram for example)
                return SpellHistory.TimeSinceGeneratorCast >= 4000;
            }
        }

        /// <summary>
        /// If we should attack to aquire the Bastians spender buff
        /// </summary>
        internal static bool ShouldRefreshBastiansSpenderBuff
        {
            get
            {
                if (Sets.BastionsOfWill.IsFullyEquipped && !CacheData.Buffs.HasBastiansWillSpenderBuff)
                    return true;

                if (!Sets.BastionsOfWill.IsFullyEquipped)
                    return false;

                return SpellHistory.TimeSinceSpenderCast >= 4750;
            }
        }

        internal static bool ShouldRefreshElusiveBuff
        {
            get
            {
                if (!Legendary.ElusiveRing.IsEquipped || Player.IsInTown)
                    return false;

                if (!CacheData.Buffs.HasBuff(446187))
                    return true;

                if (SpellHistory.TimeSinceUse(SNOPower.DemonHunter_ShadowPower).TotalMilliseconds < 7000)
                    return false;

                if (SpellHistory.TimeSinceUse(SNOPower.DemonHunter_SmokeScreen).TotalMilliseconds < 7000)
                    return false;

                if (SpellHistory.TimeSinceUse(SNOPower.DemonHunter_Vault).TotalMilliseconds < 7000)
                    return false;

                return true;
            }
        }

        internal static bool ShouldRefreshTaegukBuff
        {
            get
            {
                if (Gems.Taeguk.IsEquipped)
                    return false;

                if (!GetHasBuff(SNOPower.ItemPassive_Unique_Gem_015_x1))
                    return false;

                return SpellHistory.TimeSinceSpenderCast >= 4000;
            }
        }

        /// <summary>
        /// Select an attacking skill that is primary or a generator
        /// </summary>
        /// <returns></returns>
        internal static TrinityPower GetAttackGenerator()
        {
            return FindSkill("Generator", s => s.IsGeneratorOrPrimary && s.CanCast());
        }

        /// <summary>
        /// Select an attacking skill that spends resource
        /// </summary>
        internal static TrinityPower GetAttackSpender()
        {
            return FindSkill("Spender", s => s.IsAttackSpender && s.CanCast());
        }

        /// <summary>
        /// Select a skill for breaking urns and wooden carts etc.
        /// </summary>
        internal static TrinityPower GetDestructablesSkill()
        {
            var power = FindSkill("Destructable", s => (s.Meta.IsDestructableSkill || s.IsGeneratorOrPrimary) && s.CanCast());
            if (power != null)
                return power;

            return new TrinityPower(DefaultWeaponPower, DefaultWeaponDistance);
        }

        /// <summary>
        /// Select a skill for moving places fast
        /// </summary>
        internal static TrinityPower GetMovementSkill()
        {
            return FindSkill("Movement", s => (s.Meta.IsMovementSkill) && s.CanCast());
        }

        /// <summary>
        /// Select a skill for noping the hell out of there, includes skills that provide immunity etc.
        /// </summary>
        internal static TrinityPower GetAvoidanceSkill()
        {
            return FindSkill("Avoidance", s => (s.Meta.IsAvoidanceSkill || s.Meta.IsMovementSkill) && s.CanCast());
        }

        /// <summary>
        /// Select a skill that is a buff
        /// </summary>
        internal static TrinityPower GetBuffSkill()
        {
            return FindSkill("Buff", s => s.Meta.IsBuffingSkill && s.CanCast());
        }

        /// <summary>
        /// Gets the best power for combat
        /// </summary>
        /// <returns></returns>
        public static TrinityPower GetCombatPower(List<Skill> skills)
        {
            return FindSkill("Combat", s => s.CanCast(), skills);
        }

        /// <summary>
        /// Searches for a skill matching some criteria
        /// </summary>
        /// <param name="typeName">name for the type of skill, used in logging</param>
        /// <param name="condition">condition to be applied to skill list FirstOrDefault lamda</param>
        /// <param name="skillCollection">colleciton of skills to search against, defaults to all Active skills</param>
        /// <returns>a TrinityPower</returns>
        internal static TrinityPower FindSkill(string typeName, Func<Skill, bool> condition, List<Skill> skillCollection = null)
        {
            Logger.Log(TrinityLogLevel.Verbose, LogCategory.SkillSelection, "Finding {0} Skill", typeName);

            skillCollection = skillCollection ?? SkillUtils.Active;

            if (condition == null)
                return null;

            var skill = skillCollection.FirstOrDefault(condition);
            if (skill == null)
                return null;

            var power = GetTrinityPower(skill);

            Logger.Log(TrinityLogLevel.Verbose, LogCategory.SkillSelection, "   >>   Selected {0} Skill: {1} ({2}) Target={3}",
                typeName, power.SNOPower, (int)power.SNOPower, (CurrentTarget == null) ? "None" : CurrentTarget.InternalName);

            return power;
        }

        /// <summary>
        /// Checks if a skill can and should be cast.
        /// </summary>
        /// <param name="setting">combat data to use</param>
        public static bool CanCast(SkillMeta setting)
        {
            return setting.Skill != null && CanCast(setting.Skill);
        }

        /// <summary>
        /// Checks if a skill can and should be cast.
        /// </summary>
        /// <param name="skill">the Skill to check</param>
        /// <param name="condition">function to test against</param>
        //public static bool CanCast(Skill skill, Func<SkillMeta, bool> condition)
        //{
        //    return CanCast(skill, null, condition);
        //}

        /// <summary>
        /// Checks if a skill can and should be cast.
        /// </summary>
        /// <param name="skill">the Skill to check</param>
        /// <param name="changes">action to modify existing skill data</param>
        //public static bool CanCast(Skill skill, Action<SkillMeta> changes)
        //{
        //    return CanCast(skill, null, c => { changes(c); return true; });
        //}

        /// <summary>
        /// Checks if a skill can and should be cast.
        /// </summary>
        /// <param name="skill">the Skill to check</param>
        /// <param name="cd">Optional combat data to use</param>
        /// <param name="adhocCondition">Optional function to test against</param>
        public static bool CanCast(Skill skill, SkillMeta sm = null)
        {
            try
            {
                var meta = (sm != null) ? skill.Meta.Apply(sm) : skill.Meta;

                Func<string> check = () =>
                {
                    if (!Hotbar.Contains(skill.SNOPower))
                        return "NotOnHotbar";

                    if (Player.IsIncapacitated)
                        return "IsIncapacitated";

                    //var adhocConditionResult = (adhocCondition == null) || adhocCondition(meta);
                    var metaConditionResult = (meta.CastCondition == null) || meta.CastCondition(meta);

                    if (!meta.CastFlags.HasFlag(CanCastFlags.NoPowerManager) && !PowerManager.CanCast(skill.SNOPower))
                        return "PowerManager";

                    // Note: ZetaDia.Me.IsInCombat is unrealiable and only kicks in after an ability has hit a monster
                    if (meta.IsCombatOnly && CurrentTarget == null)
                        return "IsInCombat";

                    // This is already checked above...?
                    if (meta.ReUseDelay > 0 && TimeSincePowerUse(skill.SNOPower) < meta.ReUseDelay)
                        return "ReUseDelay";

                    //if (meta.IsEliteOnly && Enemies.Nearby.EliteCount == 0)
                    //    return false;

                    //if (meta.MaxTargetDistance > CurrentTarget.Distance)
                    //    return false;

                    var resourceCost = (meta.RequiredResource > 0) ? meta.RequiredResource : skill.Cost;
                    if (resourceCost > 0 && !skill.IsGeneratorOrPrimary)
                    {
                        var actualResource = (skill.Resource == Resource.Discipline) ? Player.SecondaryResource : Player.PrimaryResource;
                        if (actualResource < resourceCost)
                            return string.Format("NotEnoughResource({0}/{1})", Math.Round(actualResource), resourceCost);
                    }

                    //if (meta.IsEliteOnly && !CurrentTarget.IsBossOrEliteRareUnique)
                    //    return false;

                    //if (!adhocConditionResult)
                    //    return "AdHocConditionFailure";

                    if (!metaConditionResult)
                        return "ConditionFailure";

                    return string.Empty;
                };

                var failReason = check();

                if (!string.IsNullOrEmpty(failReason))
                {
                    Logger.Log(TrinityLogLevel.Verbose, LogCategory.SkillSelection, "   >>   CanCast Failed: {0} ({1}) Reason={2}",
                        skill.Name, (int)skill.SNOPower, failReason);

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in CanCast for {0}. {1} {2}", skill.Name, ex.Message, ex.InnerException);
            }

            return false;
        }

        public static bool SimpleCanCast(Skill skill)
        {
            if (!Hotbar.Contains(skill.SNOPower))
                return false;

            if (Player.IsIncapacitated)
                return false;

            if (!PowerManager.CanCast(skill.SNOPower))
                return false;

            var resourceCost = skill.Cost;
            if (resourceCost > 0 && !skill.IsGeneratorOrPrimary)
            {
                var actualResource = (skill.Resource == Resource.Discipline) ? Player.SecondaryResource : Player.PrimaryResource;
                if (actualResource < resourceCost)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// If the players build currently has no primary/generator ability
        /// </summary>
        public static bool HasNoPrimary
        {
            get { return CacheData.Hotbar.ActiveSkills.All(s => s.Skill.IsGeneratorOrPrimary); }
        }

        /// <summary>
        /// Some sugar for Null/Invalid checking on given power;
        /// </summary>
        public static bool TryGetPower(TrinityPower powerToCheck, out TrinityPower power)
        {
            power = powerToCheck;
            return (power != null && power.SNOPower != SNOPower.None); // && power != DefaultPower);
        }

        /// <summary>
        /// Converts a skill into a TrinityPower for casting
        /// </summary>
        /// <returns></returns>
        public static TrinityPower GetTrinityPower(Skill skill)
        {
            var ticksBefore = skill.Meta.BeforeUseDelay == 0 ? 0 : (int)Math.Round(BotMain.TicksPerSecond * (skill.Meta.BeforeUseDelay / 1000));
            var ticksAfter = skill.Meta.AfterUseDelay == 0 ? 0 : (int)Math.Round(BotMain.TicksPerSecond * (skill.Meta.AfterUseDelay / 1000));

            if (skill.Meta.IsCastOnSelf)
            {
                Logger.Log(LogCategory.Targetting, "Calculating TargetPosition for {0} as Self. CurrentTarget={1}", skill.Name, CurrentTarget != null ? CurrentTarget.InternalName : "");
                return skill.ToPower(ticksBefore, ticksAfter);
            }

            var castRange = (skill.Meta.CastRange <= 0) ? (int)Math.Round(skill.Meta.MaxTargetDistance * 0.5) : skill.Meta.CastRange;

            if (skill.Meta.TargetPositionSelector != null)
            {
                var targetPosition = skill.Meta.TargetPositionSelector(skill.Meta);
                if (targetPosition != Vector3.Zero)
                {
                    Logger.Log(LogCategory.Targetting, "Calculating TargetPosition for {0} using TargetPositionSelector at {1} Dist={2} PlayerIsFacing(CastPosition={3} CurrentTarget={4}) CurrentTarget={5}",
                        skill.Name,
                        targetPosition,
                        Player.Position.Distance(targetPosition),
                        Player.IsFacing(targetPosition),
                        Player.IsFacing(CurrentTarget.Position),
                        CurrentTarget != null ? CurrentTarget.InternalName : "Null"
                        );

                    return skill.ToPower(castRange, targetPosition, ticksBefore, ticksAfter);
                }
            }

            if (skill.Meta.TargetUnitSelector != null)
            {
                var targetUnit = skill.Meta.TargetUnitSelector(skill.Meta) ?? GetBestAreaEffectTarget(skill);
                if (targetUnit != null)
                {
                    Logger.Log(LogCategory.Targetting, "Calculating TargetPosition for {0} using TargetUnitSelector at {1} Dist={2} CurrentTarget={3}",
                        skill.Name,
                        targetUnit.Position,
                        Player.Position.Distance(targetUnit.Position),
                        CurrentTarget != null ? CurrentTarget.InternalName : "Null"
                        );

                    return skill.ToPower(castRange, targetUnit.Position, targetUnit.ACDGuid, ticksBefore, ticksAfter);
                }
            }

            if (skill.Meta.IsAreaEffectSkill)
            {
                var target = GetBestAreaEffectTarget(skill);

                Logger.Log(LogCategory.Targetting, "Calculating TargetPosition for {0} using AreaEffectTargetting at {1} Dist={2} PlayerIsFacing(CastPosition={3} CurrentTarget={4}) CurrentTarget={5} AreaShape={6} AreaRadius={7} ",
                    skill.Name,
                    target,
                    Player.Position.Distance(target.Position),
                    Player.IsFacing(target.Position),
                    Player.IsFacing(CurrentTarget.Position),
                    CurrentTarget != null ? CurrentTarget.InternalName : "Null",
                    skill.Meta.AreaEffectShape,
                    skill.AreaEffectRadius
                    );

                return skill.ToPower(castRange, target.Position, target.ACDGuid, ticksBefore, ticksAfter);
            }

            return skill.ToPower(castRange, CurrentTarget.Position);
        }

        /// <summary>
        /// Finds a target location using skill metadata.
        /// </summary>
        /// <param name="skill">skill to be used</param>
        /// <returns>target position</returns>
        public static TrinityCacheObject GetBestAreaEffectTarget(Skill skill)
        {
            // Avoid bot choosing a target that is too far away (and potentially running towards it) when there is danger close by.
            var searchRange = (float)(skill.IsGeneratorOrPrimary && Enemies.CloseNearby.Units.Any() ? skill.Meta.CastRange * 0.5 : skill.Meta.CastRange);

            TrinityCacheObject target;
            switch (skill.Meta.AreaEffectShape)
            {
                case AreaEffectShapeType.Beam:
                    target = TargetUtil.GetBestPierceTarget(searchRange);
                    break;
                case AreaEffectShapeType.Circle:
                    target = TargetUtil.GetBestClusterUnit(skill.AreaEffectRadius, searchRange);
                    break;
                case AreaEffectShapeType.Cone:
                    target = TargetUtil.GetBestArcTarget(searchRange, skill.AreaEffectRadius);
                    break;
                default:
                    target = TargetUtil.GetBestClusterUnit(skill.AreaEffectRadius, searchRange);
                    break;
            }

            if (target != null)
                return target;

            return TargetUtil.GetClosestUnit();
        }

        public static List<MonsterAffixes> HardEliteAddAffixes = new List<MonsterAffixes> { MonsterAffixes.ArcaneEnchanted, MonsterAffixes.Frozen, MonsterAffixes.FrozenPulse, MonsterAffixes.Plagued };

        public static bool IsInCombat
        {
            get { return CurrentTarget != null && CurrentTarget.ActorType == ActorType.Monster; }
        }

        /// <summary>
        /// Utility to change combat settings temporarily
        /// DO NOT ABUSE THIS > Forums will start complaining about their settings not working.
        /// </summary>
        public static class CombatOverrides
        {
            // Intended to temporarily change the combat settings without actually changing the user's settings,
            // for example: having a higher trash pack size in settings to look for clusters
            // then when the convention element is correct making the bot engage by reducing the radius.
            // Weighting and other consumers should reference the 'effective' properties.

            static CombatOverrides()
            {
                Pulsator.OnPulse += OnPulse;
            }

            private static List<Func<bool>> _trashSizeRevertConditions = new List<Func<bool>>();
            private static List<Func<bool>> _trashRadiusRevertConditions = new List<Func<bool>>();

            private static void OnPulse(object sender, EventArgs eventArgs)
            {
                if (_revertTrashSizeTime != DateTime.MaxValue && DateTime.UtcNow > _revertTrashSizeTime)
                {
                    RevertTrashSize();
                }

                if (_revertTrashRadiusTime != DateTime.MaxValue && DateTime.UtcNow > _revertTrashRadiusTime)
                {
                    RevertTrashRadius();
                }

                if (_trashSizeRevertConditions.Any(func => func != null && func()))
                {
                    RevertTrashSize();
                }

                if (_trashRadiusRevertConditions.Any(func => func != null && func()))
                {
                    RevertTrashRadius();
                }
            }

            public static void Reset()
            {
                RevertTrashSize();
                RevertTrashRadius();
            }

            public static void RevertTrashSize()
            {
                TrashSizeModifierPct = 1;
                _revertTrashSizeTime = DateTime.MaxValue;
                _trashSizeRevertConditions.Clear();
            }

            public static void RevertTrashRadius()
            {
                TrashRadiusModifierPct = 1;
                _revertTrashRadiusTime = DateTime.MaxValue;
                _trashRadiusRevertConditions.Clear();
            }


            private static DateTime _revertTrashSizeTime = DateTime.MaxValue;
            private static DateTime _revertTrashRadiusTime = DateTime.MaxValue;

            public static bool ModifyTrashSizeForDuration(double percentage, TimeSpan duration, int min = 1, int max = 10, Func<bool> revertCondition = null)
            {
                var beforeApply = _trashSizeModifierPct;

                if (revertCondition != null && revertCondition())
                    return false;

                // test the pct and adjust so its within min/max.
                var effective = GetEffectiveValue(Settings.Combat.Misc.TrashPackSize, percentage);
                if (effective > max && max > 1 && Settings.Combat.Misc.TrashPackSize > 0)
                    percentage = max / (double)Settings.Combat.Misc.TrashPackSize;
                else if (effective < min && min > 1 && Settings.Combat.Misc.TrashPackSize > 0)
                    percentage = min / (double)Settings.Combat.Misc.TrashPackSize;

                TrashSizeModifierPct = percentage;
                
                if (Math.Abs(beforeApply - _trashSizeModifierPct) > double.Epsilon)
                {
                    _trashSizeRevertConditions.Add(revertCondition);
                    _revertTrashSizeTime = DateTime.UtcNow.Add(duration);
                    Logger.LogVerbose($"Trash Size Changed by ModifyTrashSizeForDuration");
                    return true;
                }
                return false;
            }

            public static bool ModifyTrashRadiusForDuration(double percentage, TimeSpan duration)
            {
                var beforeApply = _trashRadiusModifierPct;
                TrashRadiusModifierPct = percentage;
                if (Math.Abs(beforeApply - _trashRadiusModifierPct) > double.Epsilon)
                {
                    _revertTrashRadiusTime = DateTime.UtcNow.Add(duration);
                    return true;
                }
                return false;
            }

            private static double _trashSizeModifierPct = 1;
            private static double TrashSizeModifierPct
            {
                get { return _trashSizeModifierPct; }
                set
                {
                    if (Math.Abs(_trashSizeModifierPct - value) > double.Epsilon && value > -4 && value < 4)
                    {
                        _trashSizeModifierPct = value;
                        Logger.Log("[CombatOverrides] Trash Pack Size modifier set to {0}%, effective value = {1}", value * 100, EffectiveTrashSize);
                    }
                }
            }

            private static double _trashRadiusModifierPct = 1;
            private static double TrashRadiusModifierPct
            {
                get { return _trashRadiusModifierPct; }
                set
                {
                    if (Math.Abs(_trashRadiusModifierPct - value) > double.Epsilon && value > -4 && value < 4)
                    {
                        _trashRadiusModifierPct = value;
                        Logger.Log("[CombatOverrides] Trash Pack Radius modifier set to {0}%, effective value = {1}", value * 100, EffectiveTrashRadius);
                    }
                }
            }

            private static int GetEffectiveValue(int setting, double pct)
            {
                var result = Math.Round(setting * pct, MidpointRounding.AwayFromZero);
                return result <= 0 ? 1 : (int)result;
            }

            public static int EffectiveTrashSize
            {
                get { return GetEffectiveValue(Settings.Combat.Misc.TrashPackSize, _trashSizeModifierPct); }
            }

            /// <summary>
            /// This is the setting in Combat > Misc > "Trash Pack Cluster Radius" multiplied by a modifier.
            /// </summary>
            public static int EffectiveTrashRadius
            {
                get { return GetEffectiveValue((int)Settings.Combat.Misc.TrashPackClusterRadius, _trashRadiusModifierPct); }
            }
        }

        public static bool IsDoingGoblinKamakazi { get; set; }
        public static TrinityCacheObject KamakaziGoblin { get; set; }
        public static Func<bool> IsWaitingForPower = () => false;

        public static HashSet<string> HighHitPointTrashMobNames = new HashSet<string>
        {
            "mallet", //
            "monstrosity", //
            "triune_berserker", //
            "beast_d",
            "thousandpounder", //5581
            "westmarchbrute", //258678, 332679
            "unburied" //6359
        };

        private static CombatMode _combatMode;

        /// <summary>
        /// Walk towards a location with positional bonuses e.g. occulus damage bonus / serenity defensive bonus.
        /// </summary>
        /// <param name="power">Trinity power configured to move player towards a buffed position</param>
        /// <param name="maxDistance">maximum distance spot can be from player's current position</param>
        /// <param name="arriveDistance">how close to get to the middle of the spot before stopping walking</param>
        /// <returns>if a location was found and should be moved to</returns>
        public static bool TryMoveToBuffedSpot(out TrinityPower power, float maxDistance, float arriveDistance = 20f)
        {
            if (IsInCombat && !IsCurrentlyKiting && !IsCurrentlyAvoiding)
            {
                Vector3 buffedLocation;
                if (PhelonUtils.BestBuffPosition(maxDistance, Player.Position, true, out buffedLocation))
                {
                    var lastPower = SpellHistory.LastPower;
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
                    else if (Core.Avoidance.Avoider.IsKiteOnCooldown)
                    {
                        Logger.Log(LogCategory.Routine, $"Not moving to buffed location while on kite cooldown");
                    }
                    else if (lastPower != null && buffedLocation.Distance(CurrentTarget.Position) > lastPower.MinimumRange + CurrentTarget.CollisionRadius + Player.Radius)
                    {
                        Logger.LogVerbose(LogCategory.Routine, $"Buffed spot outside attack range for power {lastPower.SNOPower} Range={lastPower.MinimumRange} TimeSinceUse={lastPower.TimeSinceUse} Dist={distance}");
                    }
                    else if (KiteDistance <= 0 || !TargetUtil.AnyMobsInRangeOfPosition(buffedLocation, KiteDistance))
                    {
                        Logger.LogVerbose(LogCategory.Routine, $"Moving to Buffed Position {buffedLocation} Dist={distance}");                       
                        power = new TrinityPower(SNOPower.Walk, maxDistance, buffedLocation);
                        return true;                       
                    }
                }
            }
            power = null;
            return false;
        }


    }

}
