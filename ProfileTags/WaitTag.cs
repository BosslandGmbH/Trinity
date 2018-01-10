using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.QuestTools;
using Trinity.Framework.Helpers;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{

    [XmlElement("WaitForNSeconds")]
    [XmlElement("TrinityRandomWait")]    
    [XmlElement("RandomWait")]
    [XmlElement("Wait")]
    public class WaitTag : BaseProfileBehavior
    {
        [XmlAttribute("min")]
        [XmlAttribute("waitTime")]
        [XmlAttribute("seconds")]
        [Description("Time to wait for in seconds")]
        [DefaultValue(5)]
        public int WaitTimeSeconds { get; set; }

        [XmlAttribute("randomPercent")]
        [Description("Percentage to change wait time by. eg. 20% of 5s = 4-6s")]
        [DefaultValue(10)]
        public int RandomPercent { get; set; }

        public DateTime WaitEndTime { get; set; }
        public int ActualWaitTimeMs { get; set; }

        public override async Task<bool> StartTask()
        {
            if (WaitTimeSeconds <= 0) WaitTimeSeconds = 5;
            if (RandomPercent <= 0) RandomPercent = 80;

            if (RandomPercent > 0)
            {
                var min = (int) Math.Max(0, WaitTimeSeconds * (1 - RandomPercent / 100f)) * 1000;
                var max = (int) (WaitTimeSeconds + WaitTimeSeconds * (RandomPercent / 100f)) * 1000;
                ActualWaitTimeMs = Randomizer.Random(min, max);
            }
            else
            {
                ActualWaitTimeMs = WaitTimeSeconds * 1000;
            }
            
            WaitEndTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(ActualWaitTimeMs);
            return false;
        }
    
        public override async Task<bool> MainTask()
        {
            if (DateTime.UtcNow < WaitEndTime)
            {
                StatusText = $"Waiting... ({(WaitEndTime - DateTime.UtcNow).TotalSeconds:N2}s remaining)";
                await Coroutine.Sleep(100);
                return false;
            }
            return true;
        }

    }
}