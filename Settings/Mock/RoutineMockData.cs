using System.Collections.Generic;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;

namespace Trinity.Settings.Mock
{
    public class RoutineMockData : NotifyBase
    {
        private List<SkillSettings> _activeSkills;

        public RoutineMockData()
        {
            ActiveSkills = new List<SkillSettings>
            {
                new SkillSettings(Reference.Skills.Monk.DashingStrike),
                new SkillSettings(Reference.Skills.Monk.BlindingFlash),
                new SkillSettings(Reference.Skills.Monk.Epiphany),
            };
        }

        public List<SkillSettings> ActiveSkills
        {
            get { return _activeSkills; }
            set { SetField(ref _activeSkills, value); }
        }
    }
}