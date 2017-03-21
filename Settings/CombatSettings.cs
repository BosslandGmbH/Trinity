using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Framework.Helpers;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public class CombatSettings : NotifyBase
    {
        public CombatSettings()
        {
            base.LoadDefaults();
        }

        private FollowerBossFightMode _followerBossFightDialogMode;

        [DataMember(IsRequired = false)]
        [DefaultValue(FollowerBossFightMode.DeclineInBounty)]
        public FollowerBossFightMode FollowerBossFightDialogMode
        {
            get { return _followerBossFightDialogMode; }
            set { SetField(ref _followerBossFightDialogMode, value); }
        }



    }
}
