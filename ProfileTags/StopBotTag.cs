using System.Threading.Tasks;
using Trinity.Components.QuestTools;
using Zeta.Bot;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("StopBot")]
    public class StopBotTag : BaseProfileBehavior
    {
        public override async Task<bool> StartTask()
        {            
            BotMain.Stop(false, "StopBotTag stopping from profile");
            return true;
        }
    }
}

