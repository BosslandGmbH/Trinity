using Trinity.Components.QuestTools;
using Trinity.Framework.Objects;
using Zeta.Bot;

namespace Trinity.Modules
{
    public class ProfilePulsator : Module
    {
        protected override void OnPulse()
        {
            var profile = ProfileManager.CurrentProfileBehavior;
            var baseProfile = profile as BaseProfileBehavior;
            if (baseProfile != null)
            {
                baseProfile.OnPulse();
            }
            else
            {
                (profile as BaseContainerProfileBehavior)?.OnPulse();
            }
        }
    }
}
