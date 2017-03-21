using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{

    [XmlElement("WaitForNSeconds")]
    [XmlElement("TrinityRandomWait")]    
    [XmlElement("Wait")]
    [XmlElement("RandomWait")]
    public class RandomWaitTag : BaseProfileBehavior
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

        public override async Task<bool> StartTask()
        {
            if (WaitTimeSeconds <= 0)
                WaitTimeSeconds = 5;

            if (RandomPercent <= 0)
                RandomPercent = 80;

            return false;
        }

        public override async Task<bool> MainTask()
        {
            if (RandomPercent > 0)
            {
                var min = (int)Math.Max(0, WaitTimeSeconds * (1 - RandomPercent / 100f)) * 1000;
                var max = (int)(WaitTimeSeconds + WaitTimeSeconds * (RandomPercent / 100f)) * 1000;
                var time = Randomizer.Random(min, max);
                StatusText = $"Waiting... ({time}s)";
                await Coroutine.Sleep(time);
                return true;
            }

            StatusText = $"Waiting... ({WaitTimeSeconds}s)";
            await Coroutine.Sleep(WaitTimeSeconds);
            return true;
        }

    }
}