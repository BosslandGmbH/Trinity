using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Reference;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Objects
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
        private SNOPower _snoPower;

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

        public string IconUrl => $"http://media.blizzard.com/d3/icons/skills/42/{IconSlug}.png";
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
        public SNOPower SNOPower
        {
            get
            {
                if (SNOPowers != null && SNOPowers.Any())
                {
                    var activePower = Core.Hotbar.ActivePowers.FirstOrDefault(p => SNOPowers.Contains(p));
                    if (activePower != default(SNOPower))
                    {
                        return activePower;
                    }
                }
                return _snoPower;
            }
            set { _snoPower = value; }
        }

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
        /// This is used for whether a skill can be assigned to the mouse buttons.
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
        public int CooldownRemaining => (int)Core.Cooldowns.GetSkillCooldownRemaining(SNOPower).TotalMilliseconds;

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
        public bool IsActive => SkillUtils.ActiveIds.Contains(SNOPower);

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
        public bool IsBuffActive => Core.Player.HasBuff(SNOPower);

        /// <summary>
        /// Gets the current buff stack count
        /// </summary>
        public int BuffStacks => Core.Buffs.GetBuffStacks(SNOPower);

        public int UncachedBuffStacks => Core.Buffs.GetBuff(SNOPower)?.GetStackCount() ?? 0;

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
                return GameData.AllRuneSetsBySkill.TryGetValue(this, out set) && set.IsMaxBonusActive;
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
                    return Core.Hotbar.GetHotbarSkill(SNOPower).Rune != null;

                return false;
            }
        }

        /// <summary>
        /// Check if the spell is can be cast
        /// </summary>
        public bool CanCast()
        {
            return Combat.Spells.CanCast(this);
        }

        /// <summary>
        /// Checks if a unit is currently being tracked with a given SNOPower. When the spell is properly configured, this can be used to set a "timer" on a DoT re-cast, for example.
        /// </summary>
        public bool IsTrackedOnUnit(TrinityActor unit)
        {
            return unit.HasDebuff(SNOPower);
        }

        /// <summary>
        /// Time since last used in milliseconds
        /// </summary>
        public double TimeSinceUse => DateTime.UtcNow.Subtract(LastUsed).TotalMilliseconds;

        public float DistanceFromLastUsePosition => SpellHistory.DistanceFromLastUsePosition(SNOPower);

        public bool IsLastUsed => SpellHistory.LastPowerUsed == SNOPower;

        /// <summary>
        /// When this spell was last used
        /// </summary>
        public DateTime LastUsed => SpellHistory.PowerLastUsedTime(SNOPower);

        /// <summary>
        /// Gets the current skill charge count
        /// </summary>
        public int Charges => Core.Hotbar.GetSkillCharges(SNOPower);

        /// <summary>
        /// Unique Identifier so that dictionarys can compare Skill objects.
        /// </summary>        
        public override int GetHashCode()
        {
            return Index.GetHashCode() ^ Name.GetHashCode();
        }

        //private PowerData _powerData;
        //public PowerData Data => _powerData ?? (_powerData = Core.MemoryModel.PowerHelper.GetPowerData(SNOPower));

        /// <summary>
        /// A unique identifier for IUnique
        /// </summary>
        public int Id => (int)SNOPower;

        public List<SNOPower> SNOPowers { get; set; }

        public static explicit operator Skill(ActiveSkillEntry x)
        {
            return SkillUtils.GetSkillByPower((SNOPower)x.SNOPower);
        }

        public override string ToString() => $"{GetType().Name}: {Name} {SNOPower} {(Id)}";
    }
}


