using System.ComponentModel;
using System.Runtime.Serialization;

namespace Trinity.Config
{
    [DataContract(Namespace = "")]
    public class MailServerSetting : ITrinitySetting<MailServerSetting>, INotifyPropertyChanged
    {
        #region Fields
        private string _ServerAddress;
        private string _Username;
        private string _Password;
        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MailServerSetting" /> class.
        /// </summary>
        public MailServerSetting()
        {
            Reset();
        }
        #endregion Constructors

        #region Properties
        [DataMember(IsRequired = false)]
        [DefaultValue("")]
        public string ServerAddress
        {
            get
            {
                return _ServerAddress;
            }
            set
            {
                if (_ServerAddress != value)
                {
                    _ServerAddress = value;
                    OnPropertyChanged("ServerAddress");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue("")]
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                if (_Username != value)
                {
                    _Username = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue("")]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    OnPropertyChanged("Password");
                }
            }
        }
        #endregion Properties

        #region Methods
        public void Reset()
        {
            TrinitySetting.Reset(this);
        }

        public void CopyTo(MailServerSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public MailServerSetting Clone()
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
