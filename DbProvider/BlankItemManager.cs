using Zeta.Bot;
using Zeta.Bot.Items;
using Zeta.Game.Internals.Actors;

namespace Trinity.DbProvider
{
    public class BlankItemManager : ItemManager
    {
        public override RuleTypePriority Priority => new RuleTypePriority();

        public override bool EvaluateItem(ACDItem item, ItemEvaluationType evaluationType) => false;
    }
}