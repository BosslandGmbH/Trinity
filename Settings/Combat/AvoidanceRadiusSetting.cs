using System.ComponentModel;
using System.Runtime.Serialization;

namespace Trinity.Config.Combat
{
    [DataContract(Namespace = "")]
    public class AvoidanceRadiusSetting : ITrinitySetting<AvoidanceRadiusSetting>, INotifyPropertyChanged
    {
        #region Fields
        private int _Arcane;
        private int _AzmoBodies;
        private int _AzmoFireBall;
        private int _AzmoPools;
        private int _BeesWasps;
        private int _Belial;
        private int _ButcherFloorPanel;
        private int _Desecrator;
        private int _DiabloMeteor;
        private int _DiabloPrison;
        private int _DiabloRingOfFire;
        private int _FrozenPulse;
        private int _GhomGas;
        private int _Grotesque;
        private int _IceBalls;
        private int _IceTrail;
        private int _Orbiter;
        private int _MageFire;
        private int _MaghdaProjectille;
        private int _MoltenBall;
        private int _MoltenCore;
        private int _MoltenTrail;
        private int _PlagueCloud;
        private int _PlagueHands;
        private int _PoisonTree;
        private int _PoisonEnchanted;
        private int _ShamanFire;
        private int _Thunderstorm;
        private int _WallOfFire;
        private int _Wormhole;
        private int _ZoltBubble;
        private int _ZoltTwister;
        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AvoidanceRadiusSetting" /> class.
        /// </summary>
        public AvoidanceRadiusSetting()
        {
            Reset();
        }
        #endregion Constructors

        #region Properties
        [DataMember(IsRequired = false)]
        [DefaultValue(10)]
        public int Arcane
        {
            get
            {
                return _Arcane;
            }
            set
            {
                if (_Arcane != value)
                {
                    _Arcane = value;
                    OnPropertyChanged("Arcane");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(47)]
        public int AzmoBodies
        {
            get
            {
                return _AzmoBodies;
            }
            set
            {
                if (_AzmoBodies != value)
                {
                    _AzmoBodies = value;
                    OnPropertyChanged("AzmoBodies");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(30)]
        public int AzmoFireBall
        {
            get
            {
                return _AzmoFireBall;
            }
            set
            {
                if (_AzmoFireBall != value)
                {
                    _AzmoFireBall = value;
                    OnPropertyChanged("AzmoFireBall");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(54)]
        public int AzmoPools
        {
            get
            {
                return _AzmoPools;
            }
            set
            {
                if (_AzmoPools != value)
                {
                    _AzmoPools = value;
                    OnPropertyChanged("AzmoPools");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(6)]
        public int BeesWasps
        {
            get
            {
                return _BeesWasps;
            }
            set
            {
                if (_BeesWasps != value)
                {
                    _BeesWasps = value;
                    OnPropertyChanged("BeesWasps");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(20)]
        public int Belial
        {
            get
            {
                return _Belial;
            }
            set
            {
                if (_Belial != value)
                {
                    _Belial = value;
                    OnPropertyChanged("Belial");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(40)]
        public int ButcherFloorPanel
        {
            get
            {
                return _ButcherFloorPanel;
            }
            set
            {
                if (_ButcherFloorPanel != value)
                {
                    _ButcherFloorPanel = value;
                    OnPropertyChanged("ButcherFloorPanel");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(10)]
        public int Desecrator
        {
            get
            {
                return _Desecrator;
            }
            set
            {
                if (_Desecrator != value)
                {
                    _Desecrator = value;
                    OnPropertyChanged("Desecrator");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(28)]
        public int DiabloMeteor
        {
            get
            {
                return _DiabloMeteor;
            }
            set
            {
                if (_DiabloMeteor != value)
                {
                    _DiabloMeteor = value;
                    OnPropertyChanged("DiabloMeteor");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(15)]
        public int DiabloPrison
        {
            get
            {
                return _DiabloPrison;
            }
            set
            {
                if (_DiabloPrison != value)
                {
                    _DiabloPrison = value;
                    OnPropertyChanged("DiabloPrison");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(50)]
        public int DiabloRingOfFire
        {
            get
            {
                return _DiabloRingOfFire;
            }
            set
            {
                if (_DiabloRingOfFire != value)
                {
                    _DiabloRingOfFire = value;
                    OnPropertyChanged("DiabloRingOfFire");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(5)]
        public int FrozenPulse
        {
            get
            {
                return _FrozenPulse;
            }
            set
            {
                if (_FrozenPulse != value)
                {
                    _FrozenPulse = value;
                    OnPropertyChanged("FrozenPulse");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(25)]
        public int GhomGas
        {
            get
            {
                return _GhomGas;
            }
            set
            {
                if (_GhomGas != value)
                {
                    _GhomGas = value;
                    OnPropertyChanged("GhomGas");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(30)]
        public int Grotesque
        {
            get
            {
                return _Grotesque;
            }
            set
            {
                if (_Grotesque != value)
                {
                    _Grotesque = value;
                    OnPropertyChanged("Grotesque");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(20)]
        public int IceBalls
        {
            get
            {
                return _IceBalls;
            }
            set
            {
                if (_IceBalls != value)
                {
                    _IceBalls = value;
                    OnPropertyChanged("IceBalls");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(6)]
        public int IceTrail
        {
            get
            {
                return _IceTrail;
            }
            set
            {
                if (_IceTrail != value)
                {
                    _IceTrail = value;
                    OnPropertyChanged("IceTrail");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(15)]
        public int Orbiter
        {
            get
            {
                return _Orbiter;
            }
            set
            {
                if (_Orbiter != value)
                {
                    _Orbiter = value;
                    OnPropertyChanged("Orbiter");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(10)]
        public int MageFire
        {
            get
            {
                return _MageFire;
            }
            set
            {
                if (_MageFire != value)
                {
                    _MageFire = value;
                    OnPropertyChanged("MageFire");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(8)]
        public int MaghdaProjectille
        {
            get
            {
                return _MaghdaProjectille;
            }
            set
            {
                if (_MaghdaProjectille != value)
                {
                    _MaghdaProjectille = value;
                    OnPropertyChanged("MaghdaProjectille");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(8)]
        public int MoltenBall
        {
            get
            {
                return _MoltenBall;
            }
            set
            {
                if (_MoltenBall != value)
                {
                    _MoltenBall = value;
                    OnPropertyChanged("MoltenBall");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(19)]
        public int MoltenCore
        {
            get
            {
                return _MoltenCore;
            }
            set
            {
                if (_MoltenCore != value)
                {
                    _MoltenCore = value;
                    OnPropertyChanged("MoltenCore");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(6)]
        public int MoltenTrail
        {
            get
            {
                return _MoltenTrail;
            }
            set
            {
                if (_MoltenTrail != value)
                {
                    _MoltenTrail = value;
                    OnPropertyChanged("MoltenTrail");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(10)]
        public int PlagueCloud
        {
            get
            {
                return _PlagueCloud;
            }
            set
            {
                if (_PlagueCloud != value)
                {
                    _PlagueCloud = value;
                    OnPropertyChanged("PlagueCloud");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(12)]
        public int PlagueHands
        {
            get
            {
                return _PlagueHands;
            }
            set
            {
                if (_PlagueHands != value)
                {
                    _PlagueHands = value;
                    OnPropertyChanged("PlagueHands");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(14)]
        public int PoisonEnchanted
        {
            get
            {
                return _PoisonEnchanted;
            }
            set
            {
                if (_PoisonEnchanted != value)
                {
                    _PoisonEnchanted = value;
                    OnPropertyChanged("PoisonEnchanted");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(14)]
        public int PoisonTree
        {
            get
            {
                return _PoisonTree;
            }
            set
            {
                if (_PoisonTree != value)
                {
                    _PoisonTree = value;
                    OnPropertyChanged("PoisonTree");
                }
            }
        }
        [DataMember(IsRequired = false)]
        [DefaultValue(8)]
        public int ShamanFire
        {
            get
            {
                return _ShamanFire;
            }
            set
            {
                if (_ShamanFire != value)
                {
                    _ShamanFire = value;
                    OnPropertyChanged("ShamanFire");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(5)]
        public int Thunderstorm
        {
            get
            {
                return _Thunderstorm;
            }
            set
            {
                if (_Thunderstorm != value)
                {
                    _Thunderstorm = value;
                    OnPropertyChanged("Thunderstorm");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(10)]
        public int WallOfFire
        {
            get
            {
                return _WallOfFire;
            }
            set
            {
                if (_WallOfFire != value)
                {
                    _WallOfFire = value;
                    OnPropertyChanged("WallOfFire");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(20)]
        public int Wormhole
        {
            get
            {
                return _Wormhole;
            }
            set
            {
                if (_Wormhole != value)
                {
                    _Wormhole = value;
                    OnPropertyChanged("Wormhole");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(20)]
        public int ZoltBubble
        {
            get
            {
                return _ZoltBubble;
            }
            set
            {
                if (_ZoltBubble != value)
                {
                    _ZoltBubble = value;
                    OnPropertyChanged("ZoltBubble");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(12)]
        public int ZoltTwister
        {
            get
            {
                return _ZoltTwister;
            }
            set
            {
                if (_ZoltTwister != value)
                {
                    _ZoltTwister = value;
                    OnPropertyChanged("ZoltTwister");
                }
            }
        }
        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(AvoidanceRadiusSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public AvoidanceRadiusSetting Clone()
        {
            return TrinitySetting.Clone(this);
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// This will set default values for new settings if they were not present in the serialized XML (otherwise they will be the type defaults)
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing()]
        internal void OnDeserializingMethod(StreamingContext context)
        {

            this.Grotesque = 30;
            this.Wormhole = 15;
            this.Thunderstorm = 15;
        }
        #endregion Methods
    }
}
