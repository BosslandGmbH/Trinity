using System.ComponentModel;
using System.Runtime.Serialization;

namespace Trinity.Config.Combat
{
    [DataContract(Namespace = "")]
    public class CombatSetting : ITrinitySetting<CombatSetting>, INotifyPropertyChanged
    {
        #region Fields
        private MiscCombatSetting _Misc;
        private AvoidanceRadiusSetting _AvoidanceRadius;
        private BarbarianSetting _Barbarian;
        private CrusaderSetting _Crusader;
        private MonkSetting _Monk;
        private WizardSetting _Wizard;
        private WitchDoctorSetting _WitchDoctor;
        private DemonHunterSetting _DemonHunter;
        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CombatSetting" /> class.
        /// </summary>
        public CombatSetting()
        {
            Misc = new MiscCombatSetting();
            AvoidanceRadius = new AvoidanceRadiusSetting();
            Barbarian = new BarbarianSetting();
            Crusader = new CrusaderSetting();
            Monk = new MonkSetting();
            Wizard = new WizardSetting();
            WitchDoctor = new WitchDoctorSetting();
            DemonHunter = new DemonHunterSetting();
        }
        #endregion Constructors

        #region Properties
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public MiscCombatSetting Misc
        {
            get
            {
                return _Misc;
            }
            set
            {
                if (_Misc != value)
                {
                    _Misc = value;
                    OnPropertyChanged("Misc");
                }
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public AvoidanceRadiusSetting AvoidanceRadius
        {
            get
            {
                return _AvoidanceRadius;
            }
            set
            {
                if (_AvoidanceRadius != value)
                {
                    _AvoidanceRadius = value;
                    OnPropertyChanged("AvoidanceRadius");
                }
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public BarbarianSetting Barbarian
        {
            get
            {
                return _Barbarian;
            }
            set
            {
                if (_Barbarian != value)
                {
                    _Barbarian = value;
                    OnPropertyChanged("Barbarian");
                }
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public CrusaderSetting Crusader
        {
            get
            {
                return _Crusader;
            }
            set
            {
                if (_Crusader != value)
                {
                    _Crusader = value;
                    OnPropertyChanged("Crusader");
                }
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public MonkSetting Monk
        {
            get
            {
                return _Monk;
            }
            set
            {
                if (_Monk != value)
                {
                    _Monk = value;
                    OnPropertyChanged("Monk");
                }
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public WizardSetting Wizard
        {
            get
            {
                return _Wizard;
            }
            set
            {
                if (_Wizard != value)
                {
                    _Wizard = value;
                    OnPropertyChanged("Wizard");
                }
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public WitchDoctorSetting WitchDoctor
        {
            get
            {
                return _WitchDoctor;
            }
            set
            {
                if (_WitchDoctor != value)
                {
                    _WitchDoctor = value;
                    OnPropertyChanged("WitchDoctor");
                }
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public DemonHunterSetting DemonHunter
        {
            get
            {
                return _DemonHunter;
            }
            set
            {
                if (_DemonHunter != value)
                {
                    _DemonHunter = value;
                    OnPropertyChanged("DemonHunter");
                }
            }
        }
        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(CombatSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public CombatSetting Clone()
        {
            return TrinitySetting.Clone(this);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion Methods
    }
}
