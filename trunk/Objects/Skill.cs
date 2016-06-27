using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Cache;
using Trinity.Combat;
using Trinity.Combat.Abilities;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Objects
{
    /// <summary>
    /// Contains information about a Skill
    /// </summary>
    public class Skill : IUnique
    {
        private int _cost;
        private TimeSpan _duration;
        private TimeSpan _cooldown;
        private Element _element;
        private bool _isDamaging;
        private float _areaEffectRadius;

        public Skill()
        {
            Runes = new List<Rune>();
            Index = 0;
            SNOPower = SNOPower.None;
            Name = string.Empty;
            Category = SpellCategory.Unknown;
            Description = string.Empty;
            RequiredLevel = 0;
            Tooltip = string.Empty;
            Slug = string.Empty;
            Class = ActorClass.Invalid;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string IconSlug { get; set; }
        public ResourceEffectType ResourceEffect { get; set; }

        /// <summary>
        /// The runes that may be selected for this skill
        /// </summary>
        public List<Rune> Runes { get; set; }

        /// <summary>
        /// DBs internal enum value for this skill
        /// </summary>
        public SNOPower SNOPower { get; set; }

        /// <summary>
        /// Name of the group of skills this belongs to as listed in d3 skill selection menu
        /// Ie. Barbarian Primary, Might, Tactics or Rage skill groups.
        /// </summary>
        public SpellCategory Category { get; set; }

        /// <summary>
        /// The level required before this skill may be selected in diablo3
        /// </summary>
        public int RequiredLevel { get; set; }

        /// <summary>
        /// Zero-based index for this skill within the list of skills for this class
        /// Maps to element position in Skills.[class name] 
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Code for mapping this skill to a tooltip using http://us.battle.net/d3/en/tooltip/         
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Class this skill is used by (barbarian/wizard etc).
        /// </summary>
        public ActorClass Class { get; set; }

        /// <summary>
        /// Resource type used by this skill - mana, hatred, discipline etc.
        /// </summary>
        public Resource Resource { get; set; }

        /// <summary>
        /// Blizzards game guide classifies some skills with a primary flag
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// How much this spell costs to cast; uses rune value when applicable.
        /// </summary>
        public int Cost
        {
            get { return CurrentRune.ModifiedCost ?? _cost; }
            set { _cost = value; }
        }

        /// <summary>
        /// How much this spell costs to cast; uses rune value when applicable.
        /// Corrisponds to: Beam=Width, Cone=ArcDegrees
        /// </summary>
        public float AreaEffectRadius
        {
            get { return CurrentRune.ModifiedAreaEffectRadius ?? _areaEffectRadius; }
            set { _areaEffectRadius = value; }
        }

        /// <summary>
        /// How long this spell or its effect will last; uses rune value when applicable.
        /// </summary>
        public TimeSpan Duration
        {
            get { return CurrentRune.ModifiedDuration ?? _duration; }
            set { _duration = value; }
        }

        /// <summary>
        /// If the spell causes direct damage to enemies
        /// </summary>
        public bool IsDamaging
        {
            get { return CurrentRune.ModifiedIsDamaging ?? _isDamaging; }
            set { _isDamaging = value; }
        }
        
        /// <summary>
        /// Cooldown; uses rune value when applicable and cooldown reduction from items.
        /// </summary>
        public TimeSpan Cooldown
        {
            get
            {
                var cd = Core.Cooldowns.GetSkillCooldown(SNOPower);
                if (cd != null)
                    return cd.Duration;

                return CurrentRune.ModifiedDuration ?? _cooldown;
            }
            set { _cooldown = value; }
        }

        /// <summary>
        /// Milliseconds until spell is off cooldown
        /// </summary>
        public int CooldownRemaining
        {
            get { return (int)Core.Cooldowns.GetSkillCooldownRemaining(SNOPower).TotalMilliseconds; }
        }    

        /// <summary>
        /// Element for this skill (lightning/fire etc); uses rune value when applicable.
        /// </summary>
        public Element Element
        {
            get { return CurrentRune.ModifiedElement.HasValue ? CurrentRune.ModifiedElement.Value : _element; }
            set { _element = value; }
        }

        /// <summary>
        /// If this passive is currently selected in the Diablo3 skills menu (skill is on the hotbar).
        /// </summary>
        public bool IsActive
        {
            get { return SkillUtils.ActiveIds.Contains(SNOPower); }
        }

        /// <summary>
        /// Check if skill spend primary ressource
        /// </summary>
        public bool IsAttackSpender
        {
            get
            {
                if (SNOPower == 0 || Resource == Resource.Discipline || Resource == Resource.Unknown)
                    return false;

                if (this == Skills.DemonHunter.Chakram && Legendary.SpinesOfSeethingHatred.IsEquipped)
                    return false;

                if (this == Skills.DemonHunter.ElementalArrow && Legendary.Kridershot.IsEquipped)
                    return false;

                // With Shadows Fan build we're very particular about when it gets used. 
                // So make sure its not used to proc bastians or something else trivial.
                if (this == Skills.DemonHunter.FanOfKnives && Legendary.LordGreenstonesFan.IsEquipped)
                    return false;

                return Cost > 0 && IsDamaging;
            }
        }

        /// <summary>
        /// Check if skill generates resource and can hit
        /// </summary>
        public bool IsGeneratorOrPrimary
        {
            get
            {
                if (this == Skills.DemonHunter.Chakram && Legendary.SpinesOfSeethingHatred.IsEquipped)
                    return true;

                if (this == Skills.DemonHunter.ElementalArrow && Legendary.Kridershot.IsEquipped)
                    return true;

                return Category == SpellCategory.Primary;
            }
        }

        /// <summary>
        /// If the skill's associated buff is currently active, ie, archon, warcry etc.
        /// </summary>
        public bool IsBuffActive
        {
            get { return CombatBase.GetHasBuff(SNOPower); }
        }

        /// <summary>
        /// Gets the current buff stack count
        /// </summary>
        public int BuffStacks
        {
            get { return CombatBase.GetBuffStacks(SNOPower); }
        }

        /// <summary>
        /// The currently selected rune for this skill.        
        /// </summary>
        public Rune CurrentRune
        {
            get
            {
                var rune = Runes.FirstOrDefault(r => r.IsActive);
                return rune ?? new Rune();
            }
        }

        /// <summary>
        /// If all runes for this skill are currently enabled
        /// </summary>
        public bool IsAllRuneBonusActive
        {
            get
            {
                Set set;
                return DataDictionary.AllRuneSetsBySkill.TryGetValue(this, out set) && set.IsMaxBonusActive;
            }
        }

        /// <summary>
        /// If this skill has a rune equipped
        /// </summary>
        public bool HasRuneEquipped
        {
            get
            {
                if (ZetaDia.IsInGame && ZetaDia.Me.IsValid && IsActive)
                    return CacheData.Hotbar.GetSkill(SNOPower).Rune != null;

                return false;
            }
        }

        /// <summary>
        /// Check if skill can and should be cast
        /// </summary>
        public bool CanCast()
        {
            return CombatBase.CanCast(this);
        }

        /// <summary>
        /// Performs basic checks to see if we have and can cast a power (hotbar, power manager). Checks use timer for Wiz, DH, Monk
        /// </summary>
        public bool CanCast(CombatBase.CanCastFlags flags)
        {
            return CombatBase.CanCast(SNOPower, flags);
        }

        /// <summary>
        /// Checks if a unit is currently being tracked with a given SNOPower. When the spell is properly configured, this can be used to set a "timer" on a DoT re-cast, for example.
        /// </summary>
        public bool IsTrackedOnUnit(TrinityActor unit)
        {
            return unit.HasDebuff(SNOPower);
        }

        /// <summary>
        /// Time since last used
        /// </summary>
        public double TimeSinceUse
        {
            get { return DateTime.UtcNow.Subtract(LastUsed).TotalMilliseconds; }
        }

        /// <summary>
        /// When this spell was last used
        /// </summary>
        public DateTime LastUsed => SpellHistory.PowerLastUsedTime(SNOPower);

        /// <summary>
        /// Gets the current skill charge count
        /// </summary>
        public int Charges { get{ return CombatBase.GetSkillCharges(SNOPower); }}

        /// <summary>
        /// This skill as TrinityPower
        /// </summary>
        public TrinityPower ToPower(float minimumRange, int AcdId)
        {
            return new TrinityPower(SNOPower, minimumRange, AcdId);
        }


        /// <summary>
        /// This skill as TrinityPower
        /// </summary>
        public TrinityPower ToPower(float minimumRange, Vector3 targetPosition)
        {
            return new TrinityPower(SNOPower, minimumRange, targetPosition);
        }

        /// <summary>
        /// This skill as TrinityPower
        /// </summary>
        public TrinityPower ToPower(float minimumRange, Vector3 targetPosition, int waitTicksBeforeUse, int waitTicksAfterUse)
        {
            return new TrinityPower(SNOPower, minimumRange, targetPosition, waitTicksBeforeUse, waitTicksAfterUse);
        }

        /// <summary>
        /// This skill as TrinityPower
        /// </summary>
        public TrinityPower ToPower(float minimumRange, Vector3 targetPosition, int AcdId, int waitTicksBeforeUse, int waitTicksAfterUse)
        {
            return new TrinityPower(SNOPower, minimumRange, targetPosition, TrinityPlugin.Player.WorldDynamicID, AcdId, waitTicksBeforeUse, waitTicksAfterUse);
        }

        /// <summary>
        /// This skill as TrinityPower
        /// </summary>
        public TrinityPower ToPower(int waitTicksBeforeUse, int waitTicksAfterUse)
        {
            return new TrinityPower(SNOPower, waitTicksBeforeUse, waitTicksAfterUse);
        }

        /// <summary>
        /// This skill as TrinityPower
        /// </summary>
        public TrinityPower ToPower(float minimumRange)
        {
            return new TrinityPower(SNOPower, minimumRange);
        }

        /// <summary>
        /// This skill as TrinityPower
        /// </summary>
        public TrinityPower ToPower()
        {
            return new TrinityPower(SNOPower);
        }

        /// <summary>
        /// Cast this skill at the current position
        /// </summary>
        public bool Cast()
        {
            return Cast(TrinityPlugin.Player.Position, -1);
        }

        /// <summary>
        /// Cast this skill at the specified position
        /// </summary>
        public bool Cast(Vector3 position)
        {
            return Cast(position, -1);
        }

        /// <summary>
        /// Cast this skill at the specified target
        /// </summary>
        public bool Cast(TrinityActor target)
        {
            return Cast(target.Position, target.AcdId);
        }

        /// <summary>
        /// Cast this speed using TrinityPower
        /// </summary>
        public bool Cast(TrinityPower power)
        {
            return Cast(power.TargetPosition, power.TargetAcdId);
        }

        /// <summary>
        /// Cast this skill
        /// </summary>
        public bool Cast(Vector3 clickPosition, int targetAcdId)
        {
            if (SNOPower != SNOPower.None && clickPosition != Vector3.Zero && IsActive && GameIsReady)
            {
                Logger.LogVerbose(LogCategory.Behavior, "Skill.cs: Using {0}", Name);

                if (ZetaDia.Me.UsePower(SNOPower, clickPosition, TrinityPlugin.CurrentWorldDynamicId, targetAcdId))
                {
                    TrinityPlugin.LastPowerUsed = SNOPower;
                    if (CombatBase.CurrentTarget != null)
                        SpellTracker.TrackSpellOnUnit(CombatBase.CurrentTarget.AcdId, SNOPower);
                    SpellHistory.RecordSpell(SNOPower);
                    return true;
                }
                TrinityPlugin.LastActionTimes.Add(DateTime.UtcNow);
            }

            return false;
        }

        /// <summary>
        /// Game client is not doing anything weird.
        /// </summary>
        private bool GameIsReady
        {
            get { return ZetaDia.IsInGame && ZetaDia.Me.IsValid && !ZetaDia.IsLoadingWorld && !ZetaDia.IsInTown && !ZetaDia.IsPlayingCutscene; }
        }

        /// <summary>
        /// Unique Identifier so that dictionarys can compare Skill objects.
        /// </summary>        
        public override int GetHashCode()
        {
            return Index.GetHashCode() ^ Name.GetHashCode();
        }

        /// <summary>
        /// A unique identifier for IUnique
        /// </summary>
        public int Id
        {
            get { return (int)SNOPower; }
        }

        /// <summary>
        /// Skill metadata
        /// </summary>
        public SkillMeta Meta
        {
            get { return SkillUtils.GetSkillMeta(this); }
            set { SkillUtils.SetSkillMeta(value); }
        }

        public static explicit operator Skill(ActiveSkillEntry x)
        {
            return SkillUtils.ById((SNOPower)x.SNOPower);
        }

        public SkillUsage GetDefaultSetting()
        {
            return new SkillUsage
            {
                SnoPower = this.SNOPower,
            };
        }

    }
}


