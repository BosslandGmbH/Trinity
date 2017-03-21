using System.Collections.Generic;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;

namespace Trinity.Settings.Mock
{
    public class RoutineMockData : NotifyBase
    {
        private List<SkillSettings> _activeSkills;
        private List<WeightSettings> _weightOverrides;

        public RoutineMockData()
        {
            ActiveSkills = new List<SkillSettings>
            {
                new SkillSettings(Skills.Monk.DashingStrike),
                new SkillSettings(Skills.Monk.BlindingFlash),
                new SkillSettings(Skills.Monk.Epiphany),
            };

            WeightOverrides = new List<WeightSettings>
            {
                new WeightSettings
                {
                    Name = "Shrines",
                    Formula = "1000"
                },
                new WeightSettings
                {
                    Name = "Trash",
                    Formula = "3000 * Distance"
                },
                new WeightSettings
                {
                    Name = "Elites",
                    Formula = "2000"
                },
            };
        }

        public List<SkillSettings> ActiveSkills
        {
            get { return _activeSkills; }
            set { SetField(ref _activeSkills, value); }
        }

        public List<WeightSettings> WeightOverrides
        {
            get { return _weightOverrides; }
            set { SetField(ref _weightOverrides, value); }
        }

    }
}