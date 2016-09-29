using Trinity.Framework.Helpers;
using Zeta.Bot.Profile;

namespace Trinity.ProfileTags
{
    public abstract class TrinityProfileBehavior : ProfileBehavior
    {
        protected TrinityProfileBehavior()
        {
            QuestId = QuestId == 0 ? 1 : QuestId;
            
        }

        public override void OnStart() => Logger.LogVerbose($"Started Tag: {GetType().Name}. {ToString()}");
        public override void OnDone() => Logger.LogVerbose($"Finished Tag: {GetType().Name}");
        public override bool IsDone { get; }

    }
}
