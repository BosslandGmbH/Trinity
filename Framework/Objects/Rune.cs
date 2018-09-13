using System;
using System.Linq;
using Trinity.Framework.Reference;
using Zeta.Game;

namespace Trinity.Framework.Objects
{
    /// <summary>
    /// Contains information about a Rune
    /// </summary>
    public class Rune
    {
        public Rune()
        {
            Name = "None";
            Index = -1;
            Description = string.Empty;
            TypeId = string.Empty;
            Tooltip = string.Empty;
            Class = ActorClass.Invalid;
            SkillIndex = -1;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Proper zero based index for the order runes appear in d3 skills menu
        /// Maps to element position in Runes.[class name]
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// D3's internal rune index
        /// </summary>
        public int RuneIndex { get; set; }

        /// <summary>
        /// D3's internal A-E based letter code used to determine RuneIndex
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Code for mapping this rune to a tooltip using http://us.battle.net/d3/en/tooltip/         
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Class this rune is used by (barbarian/wizard etc).
        /// </summary>
        public ActorClass Class { get; set; }

        /// <summary>
        /// Zero-Based index to the Skills object for this class
        /// </summary>
        public int SkillIndex { get; set; }

        /// <summary>
        /// If this rune is currently selected in the Diablo3 skills menu for the associated skill
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (ZetaDia.IsInGame && ZetaDia.Me.IsValid && Class == ZetaDia.Me.ActorClass && Skill != null && RuneIndex >= 0)
                {
                    return Core.Hotbar.GetHotbarSkill(Skill.SNOPower).RuneIndex == RuneIndex || Skill.IsAllRuneBonusActive;
                }
                return false;
            }
        }

        /// <summary>
        /// Skill that this rune belongs to
        /// </summary>
        public Skill Skill => SkillUtils.ByActorClass(Class).ElementAtOrDefault(SkillIndex);

        /// <summary>
        /// New duration of the associated skill's effect.
        /// If set, this value will be used by the associated skill
        /// </summary>
        public TimeSpan? ModifiedDuration { get; set; }

        /// <summary>
        /// New cooldown of the associated skill.
        /// If set, this value will be used by the associated skill.
        /// </summary>
        public TimeSpan? ModifiedCooldown { get; set; }

        /// <summary>
        /// New cost for the associated skill.
        /// If set, this value will be used by the associated skill.
        /// </summary>
        public int? ModifiedCost { get; set; }

        /// <summary>
        /// New Element for the associated skill.
        /// If set, this value will be used by the associated skill.
        /// </summary>
        public Element? ModifiedElement { get; set; }

        public bool? ModifiedIsDamaging { get; set; }

        public float? ModifiedAreaEffectRadius { get; set; }

        /// <summary>
        /// Unique Identifier so that dictionarys can compare this object properly.
        /// </summary>   
        public override int GetHashCode()
        {
            return Index.GetHashCode() ^ Name.GetHashCode();
        }

    }
}
