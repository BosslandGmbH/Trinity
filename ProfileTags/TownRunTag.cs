using System.Threading.Tasks;
using Trinity.Components.Coroutines.Town;
using Trinity.Components.QuestTools;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("TrinityTownRun")]
    [XmlElement("TownRun")]
    public class TownRunTag : BaseProfileBehavior
    {
        public override async Task<bool> StartTask()
        {
            TrinityTownRun.IsWantingTownRun = true;
            return true;
        }
    }
}
