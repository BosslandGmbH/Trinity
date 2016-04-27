using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Trinity.Objects;
using Trinity.Reference;
using Trinity.Settings.Loot;
using Trinity.UI.UIComponents;
using Trinity.UIComponents;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Settings.Mock
{
    public class SkillSettingsMockData
    {
        public SkillSettings Settings { get; set; }

        public SkillSettingsMockData()
        {
            var skills = new FullyObservableCollection<SkillUsage>();            
            Settings = new SkillSettings
            {
                Name = "Default DH Skill Settings",
                Author = "xzjv",
                ActorClass = ActorClass.DemonHunter,
                Description = "Mock data for testing interface design",
                Version = "1.0",
                Items = skills
            };
            Settings.Items.Add(new SkillUsage
            {
                CastRange = 80f,
                SnoPower = SNOPower.X1_DemonHunter_EvasiveFire,
                ClusterSize = 4,
                HealthPct = 80,
                Reasons = SkillUseReasons.Avoidance | SkillUseReasons.Elites | SkillUseReasons.HealthEmergency,
                RecastDelayMs = 2000,
                ResourcePct = 50,                
                UseTime = SkillUseTime.AnyTime
            });
            //Settings.LoadDefaults();
        }
    }
}



