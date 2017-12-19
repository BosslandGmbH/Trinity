using Trinity.Framework.Helpers;
using System.Runtime.Serialization;
using Trinity.Components.Adventurer.Game.Exploration.SceneMapping;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public class ExplorationSettings : NotifyBase
    {
        public ExplorationSettings()
        {
            base.LoadDefaults();
        }

        private IWorldRegion _blacklistedRegions;
        private IWorldRegion _priorityRegions;

        [DataMember]
        public IWorldRegion BlacklistedRegions
        {
            get { return _blacklistedRegions; }
            set { SetField(ref _blacklistedRegions, value); }
        }

        [DataMember]
        public IWorldRegion PriorityRegions
        {
            get { return _priorityRegions; }
            set { SetField(ref _priorityRegions, value); }
        }


    }
}
