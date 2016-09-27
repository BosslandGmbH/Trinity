using Trinity.Components.Combat.Abilities;
using Trinity.Framework;
using Trinity.Reference;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Objects
{
    /// <summary>
    /// Contains information about a Passive
    /// </summary>
    public class Passive : IUnique
    {
        public Passive()
        {
            Index = 0;
            SNOPower = SNOPower.None;
            Name = string.Empty;
            Description = string.Empty;
            RequiredLevel = 0;
            Tooltip = string.Empty;
            Slug = string.Empty;
            Class = ActorClass.Invalid;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }

        /// <summary>
        /// Zero-based index for this passive within the list of passives for this class
        /// Maps to element position in Passives.[class name] 
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// DBs internal enum value for this passive
        /// </summary>
        public SNOPower SNOPower { get; set; }

        /// <summary>
        /// The level required before this passive may be selected in diablo3
        /// </summary>
        public int RequiredLevel { get; set; }

        /// <summary>
        /// Code for mapping this passive to a tooltip using http://us.battle.net/d3/en/tooltip/         
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Actor class (barbarian/wizard etc) that this passive applies to
        /// </summary>
        public ActorClass Class { get; set; }

        /// <summary>
        /// If this passive is currently selected in the Diablo3 skills menu
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (ZetaDia.IsInGame && ZetaDia.Me.IsValid && Class == ZetaDia.Me.ActorClass)
                {
                    return Core.Hotbar.PassiveSkills.Contains(SNOPower);
                }
                return false;
            }
        }

        /// <summary>
        /// If the associated buff is currently active, ie, barbarian rampage, wizard dominance.
        /// </summary>
        public bool IsBuffActive => Core.Player.HasBuff(SNOPower);

        /// <summary>
        /// Gets the associated buff stack count
        /// </summary>
        public int BuffStacks => Core.Buffs.GetBuffStacks(SNOPower);

        /// <summary>
        /// Unique Identifier so that dictionarys can compare  this object properly.
        /// </summary>        
        public override int GetHashCode()
        {
            return Index.GetHashCode() ^ Name.GetHashCode();
        }

        public int Id => (int)SNOPower;

        public string IconSlug { get; set; }

        public static explicit operator Passive(TraitEntry x)
        {
            return PassiveUtils.ById((SNOPower)x.SNOPower);
        }
    }
}
