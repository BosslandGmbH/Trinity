using System.Linq;
using System.Runtime.Serialization;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public class DynamicSettingNode : NotifyBase
    {
        private string _name;
        private string _code;

        [IgnoreDataMember]
        public IDynamicSetting Setting => SettingsManager.GetDynamicSettings().FirstOrDefault(s => s.GetName() == Name);

        [DataMember(Order = 0)]
        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        [DataMember(Order = 1)]
        public string Code
        {
            get { return _code; }
            set { SetField(ref _code, value); }
        }

        public override string ToString() => $"{GetType().Name}: {Name}";
    }
}