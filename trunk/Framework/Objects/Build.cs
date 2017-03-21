using System.Collections.Generic;
using System.Linq;
namespace Trinity.Framework.Objects
{
    public class Build
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<Skill, Rune> Skills { get; set; } = new Dictionary<Skill, Rune>();
        public List<Passive> Passives { get; set; } = new List<Passive>();
        public List<Item> Items { get; set; } = new List<Item>();
        public Dictionary<Set, SetBonus> Sets { get; set; } = new Dictionary<Set, SetBonus>();
        public int Level { get; set; }

        public bool IsEquipped()
        {
            if (Passives != null && !Passives.All(passive => passive.IsActive))
                return false;

            if (Items != null && !Items.All(item => item.IsEquipped))
                return false;

            if (Sets != null && !Sets.All(set => set.Key.IsBonusActive(set.Value)))
                return false;

            if (Skills != null && !Skills.All(keyValuePair => keyValuePair.Key.IsActive && (keyValuePair.Value == null || keyValuePair.Value.Index == 0 || keyValuePair.Value.IsActive)))
                return false;

            return true;
        }

        public int RequirementCount => Skills.Count + Items.Count + Sets.Count + Passives.Count;

        public override string ToString() => $"{GetType().Name}: {Name}";

        public string SkillSummary => Skills.Any() ? Skills.Aggregate(" > Skills:", (s, pair) => s + $" {pair.Key.Name} {pair.Value?.Name}, ") : string.Empty;
        public string PassivesSummary => Passives.Any() ? Passives.Aggregate(" > Passives: ", (s, passive) => s + $"{passive.Name}, ") : string.Empty;
        public string ItemsSummary => Items.Any() ? Items.Aggregate(" > Items: ", (s, item) => s + $"{item.Name}, ") : string.Empty;
        public string SetsSummary => Sets.Any() ? Sets.Aggregate(" > Sets: ", (s, set) => s + $"{set.Key.Name}, ") : string.Empty;
        public string Summary => $@"{SkillSummary}{SetsSummary}{ItemsSummary}{PassivesSummary}";
    }
}