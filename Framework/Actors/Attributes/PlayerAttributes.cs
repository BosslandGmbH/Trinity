using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Actors.Attributes
{
    public class PlayerAttributes : ActorAttributes
    {
        public PlayerAttributes() { }

        public PlayerAttributes(int groupId) : base(groupId) {  }

        public ResourceType ResourceTypePrimary => GetAttribute<ResourceType>(ActorAttributeType.ResourceTypePrimary);

        public ResourceType ResourceTypeSecondary => GetAttribute<ResourceType>(ActorAttributeType.ResourceTypeSecondary);

    }
}




