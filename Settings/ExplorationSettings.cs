using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Trinity.Components.Adventurer.Game.Exploration.SceneMapping;
using Trinity.Components.Combat;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Attributes;
using Trinity.UI.UIComponents;
using Zeta.Game.Internals.Actors;

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
