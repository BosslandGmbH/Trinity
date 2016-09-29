using System.Runtime.Serialization;
using Trinity.Framework.Helpers;
using Extensions = Zeta.Common.Extensions;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    [KnownType(typeof(DynamicNodeCollection))]
    public class DynamicSettingGroup : NotifyBase, ITrinitySetting<DynamicSettingGroup>
    {
        private DynamicNodeCollection _settings = new DynamicNodeCollection();

        [DataMember]        
        public DynamicNodeCollection Settings
        {
            get { return _settings; }
            set { SetField(ref _settings, value); }
        }

        public void Reset() => Extensions.ForEach(_settings, s => s.Setting.Reset());

        public void CopyTo(DynamicSettingGroup setting)
        {
            setting.Settings = new DynamicNodeCollection();
            foreach (var item in _settings)
            {
                setting.Settings.Add(new DynamicSettingNode
                {
                    Name = item.Name,
                    Code = item.Code
                });
            }
        }

        public DynamicSettingGroup Clone()
        {
            var settings = new DynamicSettingGroup();
            foreach (var item in _settings)
            {
                settings.Settings.Add(new DynamicSettingNode
                {
                    Name = item.Name,
                    Code = item.Code
                });
            }
            return settings;
        }
    }
}