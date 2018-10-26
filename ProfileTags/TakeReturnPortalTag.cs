using Trinity.Framework;
using System.Threading.Tasks;
using Trinity.Components.Coroutines.Town;
using Trinity.Components.QuestTools;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("ResumeUseTownPortal")]
    [XmlElement("TakeReturnPortal")]
    public class TakeReturnPortalTag : BaseProfileBehavior
    {
        public override async Task<bool> MainTask()
        {
            if (!await TrinityTownRun.TakeReturnPortal())
                return false;
            return true;
        }

    }
}
