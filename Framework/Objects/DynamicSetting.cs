using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Controls;
using Trinity.Framework.Helpers;
using Trinity.Settings;
using Trinity.UI;
using Extensions = Zeta.Common.Extensions;

namespace Trinity.Framework.Objects
{
    public interface IDynamicSetting
    {
        string GetName();
        UserControl GetControl();
        object GetDataContext();
        string GetCode();
        void ApplyCode(string code);
        void Reset();
        void Save();
    }

    public class DynamicSetting<T> : IDynamicSetting where T : NotifyBase, new()
    {
        public DynamicSetting()
        {
            Object = new T();
        }

        public DynamicSetting(T obj)
        {
            Object = obj;
        }
    
        public T Object;
        public virtual string GetName() => typeof(T).Name;
        public virtual UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
        public virtual object GetDataContext() => Object;
        public virtual string GetCode() => JsonSerializer.Serialize(Object);
        public virtual void ApplyCode(string code) => Update(code);
        public virtual void Reset() => Object.LoadDefaults();
        public virtual void Save() { }

        private void Update(string code)
        {
            JsonSerializer.Deserialize(code, Object);
            Object.OnPopulated();
        }
    }

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

    [CollectionDataContract(Namespace = "", ItemName = "Node")]
    public class DynamicNodeCollection : ObservableCollection<DynamicSettingNode> { }

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