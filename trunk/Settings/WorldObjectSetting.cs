using System.ComponentModel;
using System.Configuration;
using System.Runtime.Serialization;
using Trinity.Components.Combat;
using Trinity.Config.Combat;
using Trinity.Framework.Objects.Attributes;
using Trinity.Helpers;
using Trinity.UIComponents;
using Zeta.Game.Internals.Actors;

namespace Trinity.Config
{
    [DataContract(Namespace = "")]
    public class WorldObjectSetting : NotifyBase, ITrinitySetting<WorldObjectSetting>
    {

        #region Fields
        private int _ContainerOpenRange;
        private int _DestructibleRange;
        private bool _UseShrine;
        private bool _UseFrenzyShrine;
        private bool _UseFortuneShrine;
        private bool _UseProtectionShrine;
        private bool _UseEmpoweredShrine;
        private bool _UseEnlightenedShrine;
        private bool _UseChannelingPylon;
        private bool _UseConduitPylon;
        private bool _UseShieldPylon;
        private bool _UseSpeedPylon;
        private bool _UsePowerPylon;
        private bool _UseFleetingShrine;
        private bool _HiPriorityShrines;
        private bool _HiPriorityContainers;
        private bool _OpenAnyContainer;
        private bool _InspectWeaponRacks;
        private bool _InspectGroundClicky;
        private bool _InspectCorpses;
        private bool _OpenChests;
        private bool _OpenRareChest;
        private int _HealthWellMinHealth;
        private int _OpenContainerDelay;
        private DestructibleIgnoreOption _DestructibleOption;
        private bool _EnableBountyEvents;
        private bool _AllowPlayerResurection;
        private SettingMode _shrineWeighting;
        private ShrineTypes _shrineTypes;
        private SettingMode _containerWeighting;
        private ShrineTypes _containerTypes;

        #endregion Fields

        public WorldObjectSetting()
        {
            Reset();
        }

        #region Properties
        [DataMember(IsRequired = false)]
        [DefaultValue(45)]
        public int ContainerOpenRange
        {
            get
            {
                return _ContainerOpenRange;
            }
            set 
            {
                if (_ContainerOpenRange != value)
                {
                    _ContainerOpenRange = value;
                    OnPropertyChanged("ContainerOpenRange");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(4)]
        public int DestructibleRange
        {
            get
            {
                return _DestructibleRange;
            }
            set
            {
                if (_DestructibleRange != value)
                {
                    _DestructibleRange = value;
                    OnPropertyChanged("DestructibleRange");
                }
            }
        }

        //[DataMember(IsRequired = false)]
        //[DefaultValue(true)]
        //public bool UseShrine
        //{
        //    get
        //    {
        //        return _UseShrine;
        //    }
        //    set
        //    {
        //        if (_UseShrine != value)
        //        {
        //            _UseShrine = value;
        //            OnPropertyChanged("UseShrine");
        //        }
        //    }
        //}



        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseEnlightenedShrine
        {
            get
            {
                return _UseEnlightenedShrine;
            }
            set
            {
                if (_UseEnlightenedShrine != value)
                {
                    _UseEnlightenedShrine = value;
                    OnPropertyChanged("UseEnlightenedShrine");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseFrenzyShrine
        {
            get
            {
                return _UseFrenzyShrine;
            }
            set
            {
                if (_UseFrenzyShrine != value)
                {
                    _UseFrenzyShrine = value;
                    OnPropertyChanged("UseFrenzyShrine");
                }
            }
        }
        
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseFortuneShrine
        {
            get
            {
                return _UseFortuneShrine;
            }
            set
            {
                if (_UseFortuneShrine != value)
                {
                    _UseFortuneShrine = value;
                    OnPropertyChanged("UseFortuneShrine");
                }
            }
        }
        
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseProtectionShrine
        {
            get
            {
                return _UseProtectionShrine;
            }
            set
            {
                if (_UseProtectionShrine != value)
                {
                    _UseProtectionShrine = value;
                    OnPropertyChanged("UseProtectionShrine");
                }
            }
        }
        
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseEmpoweredShrine
        {
            get
            {
                return _UseEmpoweredShrine;
            }
            set
            {
                if (_UseEmpoweredShrine != value)
                {
                    _UseEmpoweredShrine = value;
                    OnPropertyChanged("UseEmpoweredShrine");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseFleetingShrine
        {
            get
            {
                return _UseFleetingShrine;
            }
            set
            {
                if (_UseFleetingShrine != value)
                {
                    _UseFleetingShrine = value;
                    OnPropertyChanged("UseFleetingShrine");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseChannelingPylon
        {
            get
            {
                return _UseChannelingPylon;
            }
            set
            {
                if (_UseChannelingPylon != value)
                {
                    _UseChannelingPylon = value;
                    OnPropertyChanged("UseChannelingPylon");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseConduitPylon
        {
            get
            {
                return _UseConduitPylon;
            }
            set
            {
                if (_UseConduitPylon != value)
                {
                    _UseConduitPylon = value;
                    OnPropertyChanged("UseConduitPylon");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseShieldPylon
        {
            get
            {
                return _UseShieldPylon;
            }
            set
            {
                if (_UseShieldPylon != value)
                {
                    _UseShieldPylon = value;
                    OnPropertyChanged("UseShieldPylon");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UseSpeedPylon
        {
            get
            {
                return _UseSpeedPylon;
            }
            set
            {
                if (_UseSpeedPylon != value)
                {
                    _UseSpeedPylon = value;
                    OnPropertyChanged("UseSpeedPylon");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool UsePowerPylon
        {
            get
            {
                return _UsePowerPylon;
            }
            set
            {
                if (_UsePowerPylon != value)
                {
                    _UsePowerPylon = value;
                    OnPropertyChanged("UsePowerPylon");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool HiPriorityShrines
        {
            get
            {
                return _HiPriorityShrines;
            }
            set
            {
                if (_HiPriorityShrines != value)
                {
                    _HiPriorityShrines = value;
                    OnPropertyChanged("HiPriorityShrines");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool HiPriorityContainers
        {
            get
            {
                return _HiPriorityContainers;
            }
            set
            {
                if (_HiPriorityContainers != value)
                {
                    _HiPriorityContainers = value;
                    OnPropertyChanged("HiPriorityContainers");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool OpenAnyContainer
        {
            get
            {
                return _OpenAnyContainer;
            }
            set
            {
                if (_OpenAnyContainer != value)
                {
                    _OpenAnyContainer = value;
                    OnPropertyChanged("OpenAnyContainer");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool InspectWeaponRacks
        {
            get
            {
                return _InspectWeaponRacks;
            }
            set
            {
                if (_InspectWeaponRacks != value)
                {
                    _InspectWeaponRacks = value;
                    OnPropertyChanged("InspectWeaponRacks");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool InspectGroundClicky
        {
            get
            {
                return _InspectGroundClicky;
            }
            set
            {
                if (_InspectGroundClicky != value)
                {
                    _InspectGroundClicky = value;
                    OnPropertyChanged("InspectGroundClicky");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool InspectCorpses
        {
            get
            {
                return _InspectCorpses;
            }
            set
            {
                if (_InspectCorpses != value)
                {
                    _InspectCorpses = value;
                    OnPropertyChanged("InspectCorpses");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool OpenChests
        {
            get
            {
                return _OpenChests;
            }
            set
            {
                if (_OpenChests != value)
                {
                    _OpenChests = value;
                    OnPropertyChanged("OpenChests");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool OpenRareChests
        {
            get
            {
                return _OpenRareChest;
            }
            set
            {
                if (_OpenRareChest != value)
                {
                    _OpenRareChest = value;
                    OnPropertyChanged("OpenRareChest");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(50)]
        public int HealthWellMinHealth
        {
            get
            {
                return _HealthWellMinHealth;
            }
            set
            {
                if (_HealthWellMinHealth != value)
                {
                    _HealthWellMinHealth = value;
                    OnPropertyChanged("HealthWellMinHealth");
                }
            }
        }
        
        [DataMember(IsRequired = false)]
        [DefaultValue(500)]
        [Description("Delay in milliseconds to wait after opening a container")]
        public int OpenContainerDelay
        {
            get
            {
                return _OpenContainerDelay;
            }
            set
            {
                if (_OpenContainerDelay != value)
                {
                    _OpenContainerDelay = value;
                    OnPropertyChanged("OpenContainerDelay");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(DestructibleIgnoreOption.OnlyIfStuck)]
        public DestructibleIgnoreOption DestructibleOption
        {
            get
            {
                return _DestructibleOption;
            }
            set
            {
                if (_DestructibleOption != value)
                {
                    _DestructibleOption = value;
                    OnPropertyChanged("DestructibleOption");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool EnableBountyEvents
        {
            get
            {
                return _EnableBountyEvents;
            }
            set
            {
                if (_EnableBountyEvents != value)
                {
                    _EnableBountyEvents = value;
                    OnPropertyChanged("EnableBountyEvents");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool AllowPlayerResurection
        {
            get
            {
                return _AllowPlayerResurection;
            }
            set
            {
                if (_AllowPlayerResurection != value)
                {
                    _AllowPlayerResurection = value;
                    OnPropertyChanged("AllowPlayerResurection");
                }
            }
        }



        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(WorldObjectSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public WorldObjectSetting Clone()
        {
            return TrinitySetting.Clone(this);
        }


        /// <summary>
        /// This will set default values for new settings if they were not present in the serialized XML (otherwise they will be the type defaults)
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            LoadDefaults();

            UseEmpoweredShrine = true;
            UseEnlightenedShrine = true;
            UseFleetingShrine = true;
            UseFortuneShrine = true;
            UseFrenzyShrine = true;
            UseProtectionShrine = true;
            HealthWellMinHealth = 75;
            DestructibleOption = DestructibleIgnoreOption.OnlyIfStuck;
            OpenChests = true;
            OpenRareChests = true;
            OpenContainerDelay = 500;
            HiPriorityShrines = false;
            HiPriorityContainers = false;
            EnableBountyEvents = true;
            UseChannelingPylon = true;
            UseConduitPylon = true;
            UsePowerPylon = true;
            UseShieldPylon = true;
            UseSpeedPylon = true;

        }
        #endregion Methods
    }
}
