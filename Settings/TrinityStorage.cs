using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Trinity.Framework.Objects;
using Zeta.Bot.Settings;
using Zeta.Game;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]   
    public class TrinityStorage : NotifyBase, ITrinitySetting<TrinityStorage>
    {

        private FileSystemWatcher _FSWatcher;
        private DateTime _LastLoadedSettings;
        public delegate void SettingsEvent();
        public static event SettingsEvent OnSave = () => { };
        public static event SettingsEvent OnLoaded = () => { };
        public static event SettingsEvent OnReset = () => { };
        public static event SettingsEvent OnUserRequestedReset = () => { };

        public TrinityStorage(bool initialize = true)
        {
            if (initialize)
            {
                Dynamic = new DynamicSettingGroup();
            }

            _FSWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(GlobalSettingsFile),
                Filter = Path.GetFileName(GlobalSettingsFile),
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            _FSWatcher.Changed += _FSWatcher_Changed;
            _LastLoadedSettings = DateTime.MinValue;
        }

        [DataMember(IsRequired = false)]
        public int Version { get; set; } = 1;

        [DataMember(IsRequired = false)]
        public DynamicSettingGroup Dynamic
        {
            get { return _dynamic; }
            set { SetField(ref _dynamic, value); }
        }

        [IgnoreDataMember]
        internal static string BattleTagSettingsFile => Path.Combine(FileManager.SpecificSettingsPath, "Trinity.xml");

        private static int _currentHeroId;
        private DynamicSettingGroup _dynamic;


        [IgnoreDataMember]
        internal static string HeroSpecificSettingsFile
        {
            get
            {
                if (ZetaDia.Service.IsValid && ZetaDia.Service.Hero != null && ZetaDia.Service.Hero.IsValid)
                {
                    _currentHeroId = ZetaDia.Service.Hero.HeroId;
                }

                return Path.Combine(FileManager.SpecificSettingsPath, _currentHeroId.ToString(), "Trinity.xml");
            }
        }

        [IgnoreDataMember]
        internal static string OldBattleTagSettingsFile => Path.Combine(FileManager.SpecificSettingsPath, "GilesTrinity.xml");

        [IgnoreDataMember]
        internal static string GlobalSettingsFile => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Settings", "Trinity.xml");

        private void _FSWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(250);
            Load();
        }

        public void UserRequestedReset()
        {
            Core.Logger.Log("用户请求重置");
            Reset(this);
            OnUserRequestedReset();
        }

        public void Reset()
        {
            Reset(this);
        }

        public void CopyTo(TrinityStorage storage)
        {
            CopyTo(this, storage);
        }

        public TrinityStorage Clone()
        {
            return Clone(this);
        }

        public static string GetSettingsFilePath()
        {
            if (File.Exists(GlobalSettingsFile))
            {
                return GlobalSettingsFile;
            }
            if (File.Exists(HeroSpecificSettingsFile))
            {
                return HeroSpecificSettingsFile;
            }
            if (File.Exists(BattleTagSettingsFile))
            {
                return BattleTagSettingsFile;
            }
            if (File.Exists(OldBattleTagSettingsFile))
            {
                return OldBattleTagSettingsFile;
            }
            throw new FileNotFoundException("Unable to find settings file");
        }

        public void Load()
        {
            bool loadSuccessful = false;
            bool migrateConfig = false;

            // Only load once every 500ms (prevents duplicate Load calls)
            if (DateTime.UtcNow.Subtract(_LastLoadedSettings).TotalMilliseconds <= 500)
                return;

            _LastLoadedSettings = DateTime.UtcNow;

            string filename = GlobalSettingsFile;
            lock (this)
            {
                try
                {
                    if (File.Exists(GlobalSettingsFile))
                    {
                        Core.Logger.Log("加载全局设置, 你可以适用战斗标签设置来移除 Trinity.xml 文件 在您Demonbuddy设置目录下");
                        var globalSettings = LoadSettingsFromFile(filename);
                        loadSuccessful = globalSettings != null;
                    }
                    else if (File.Exists(HeroSpecificSettingsFile))
                    {
                        Core.Logger.Log("加载英雄特定设置");
                        filename = HeroSpecificSettingsFile;

                        var settings = LoadSettingsFromFile(filename);
                        loadSuccessful = settings != null;
                    }
                    else if (File.Exists(BattleTagSettingsFile))
                    {
                        Core.Logger.Log("加载战役标签设置");
                        filename = BattleTagSettingsFile;

                        var settings = LoadSettingsFromFile(filename);
                        loadSuccessful = settings != null;
                    }
                    else if (File.Exists(OldBattleTagSettingsFile))
                    {
                        Core.Logger.Debug(LogCategory.None, "旧配置文件找到, 需要迁移!");
                        filename = OldBattleTagSettingsFile;
                        migrateConfig = true;

                        var settings = LoadSettingsFromFile(filename);
                        loadSuccessful = settings != null;
                    }

                    Core.Logger.Debug("Settings Load: FireOnLoadedEvents");
                    FireOnLoadedEvents();
                }
                catch (Exception ex)
                {
                    Core.Logger.Error(LogCategory.None, "加载配置文件时出现错误: {0}", ex);
                    loadSuccessful = false;
                    migrateConfig = false;
                }

                if (migrateConfig && loadSuccessful)
                {
                    Core.Logger.Debug(LogCategory.None, "配置迁移到新的 Trinity.xml");
                    Save();

                    if (File.Exists(OldBattleTagSettingsFile))
                    {
                        File.Delete(OldBattleTagSettingsFile);
                    }
                }                
            }
        }

        public void FireOnLoadedEvents()
        {
            var eventSupporters = GetInterfaceMembers<ITrinitySettingEvents>(this);
            foreach (var eventSupporter in eventSupporters)
            {
                Core.Logger.Debug($"FireOnLoadedEvents: {eventSupporter.GetType().Name}");
                eventSupporter.OnLoaded();
            }
        }

        public static TrinityStorage GetSettingsFromFile(string filename)
        {
            return new TrinityStorage().LoadSettingsFromFile(filename, false);
        }

        public TrinityStorage LoadSettingsFromFile(string filename, bool applyToThis = true)
        {
            TrinityStorage loadedStorages = null;

            if (File.Exists(filename))
            {
                DateTime fsChangeStart = DateTime.UtcNow;
                while (FileManager.IsFileReadLocked(new FileInfo(GlobalSettingsFile)))
                {
                    Thread.Sleep(10);
                    if (DateTime.UtcNow.Subtract(fsChangeStart).TotalMilliseconds > 5000)
                        break;
                }

                var doc = XDocument.Load(filename);        
                if (doc.Root != null)
                {
                    var reader = doc.Root.CreateReader();
                    DataContractSerializer serializer = new DataContractSerializer(this.GetType());
                    loadedStorages = (TrinityStorage)serializer.ReadObject(reader, false);


                    if (applyToThis)
                    {
                        Core.Logger.Debug($"从文件读取配置: 复制存储对象");
                        loadedStorages.CopyTo(this);
                    }

                    LoadDynamicSettings();
                    Core.Logger.Log("配置文件加载");
                    OnLoaded();

                    if (doc.Root.Name == "TrinitySetting")
                    {
                        Core.Logger.Debug("检测到旧设置格式。迁移并保存旧文件副本");
                        try
                        {
                            File.Copy(filename, FileManager.GetUniqueFileName(filename + ".backup.xml"));
                        }
                        catch (Exception)
                        {
                            Core.Logger.Debug("无法保存旧设置文件备份.");
                        }
                        Save();
                    }
                }
            }
            else
            {
                Core.Logger.Debug(LogCategory.None, "没有找到配置文件.");
                Reset();
            }            
            return loadedStorages;
        }

        public void Save(bool useGlobal = false)
        {
            lock (this)
            {
                Core.Logger.Log("保存设置");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnSave();
                    GlobalSettings.Instance.Save();
                    CharacterSettings.Instance.Save();

                    SaveDynamicSettings();

                    string filename;

                    if (File.Exists(GlobalSettingsFile) || useGlobal)
                    {
                        filename = GlobalSettingsFile;
                        SaveToFile(filename, this);
                    }
                    else
                    {
                        filename = HeroSpecificSettingsFile;
                        SaveToFile(filename, this);
                    }

                    FireOnSaveEvents();
                });

            }

        }

        public void SaveDynamicSettings()
        {
            Dynamic.Settings.Clear();

            foreach (var item in SettingsManager.GetDynamicSettings().Where(i => i != null))
            {
                Dynamic.Settings.Add(new DynamicSettingNode
                {
                    Name = item.GetName(),
                    Code = item.GetCode()
                });
            }
        }

        public void LoadDynamicSettings()
        {
            if (Dynamic == null)
                return;

            //Core.Logger.Debug($"LoadDynamicSettings");

            foreach (var item in Dynamic.Settings)
            {
                //Core.Logger.Debug($"LoadDynamicSettings: {item.Name}");

                var setting = item.Setting;
                if (setting == null)
                    continue;

                if (string.IsNullOrEmpty(item.Code))
                {
                    //Core.Logger.Debug($"LoadDynamicSettings: {item.Name} > Reset (null code)");
                    setting.Reset();
                }
                else
                {
                    //Core.Logger.Debug($"LoadDynamicSettings: {item.Name} > Reset");
                    setting.Reset();
                    //Core.Logger.Debug($"LoadDynamicSettings: {item.Name} > Apply Code");
                    setting.ApplyCode(item.Code);
                }
            }
        }

        public void FireOnSaveEvents()
        {
            var eventSupporters = GetInterfaceMembers<ITrinitySettingEvents>(this);
            foreach (var eventSupporter in eventSupporters)
            {
                eventSupporter.OnSave();
            }
        }

        public void SaveToFile(string filePath, TrinityStorage storages = null)
        {
            try
            {
                if (storages == null)
                    storages = this;

                if (filePath == null)
                {
                    throw new ArgumentNullException(nameof(filePath));
                }

                var dirName = Path.GetDirectoryName(filePath);
                if (dirName != null && !Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);

                _FSWatcher.EnableRaisingEvents = false;

                Core.Logger.Log("保存配置文件");
                using (Stream stream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(TrinityStorage));

                    var xmlWriterSettings = new XmlWriterSettings { Indent = true };
                    using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        serializer.WriteObject(xmlWriter, storages);
                    }
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error(LogCategory.None, "保存配置文件出现错误: {0}", ex);
            }
            finally
            {
                _FSWatcher.EnableRaisingEvents = true;
            }
        }

        private IEnumerable<T> GetInterfaceMembers<T>(object obj)
        {
            var type = obj.GetType();
            return from property in type.GetProperties()
                   where typeof(T).IsAssignableFrom(property.PropertyType)
                   select GetValue<T>(obj, property);
        }

        private static T GetValue<T>(object obj, PropertyInfo propertyInfo)
        {
            return (T)propertyInfo.GetValue(obj, null);
        }

        internal static void Reset<T>(ITrinitySetting<T> setting) where T : class, ITrinitySetting<T>
        {
            try
            {
                Type type = typeof(T);
                Core.Logger.Verbose(LogCategory.Configuration, "启动复位 {0}", type.Name);
                foreach (PropertyInfo prop in type.GetProperties(BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (Attribute.IsDefined(prop, typeof(IgnoreDataMemberAttribute)))
                        continue;

                    if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                    {
                        Attribute[] decorators = prop.GetCustomAttributes(typeof(DefaultValueAttribute), true) as Attribute[];
                        if (decorators != null && decorators.Length > 0)
                        {
                            DefaultValueAttribute defaultValue = decorators[0] as DefaultValueAttribute;
                            if (defaultValue != null)
                            {
                                prop.SetValue(setting, defaultValue.Value, null);
                            }
                        }
                    }
                    else
                    {
                        object value = prop.GetValue(setting, null);
                        if (value != null)
                        {
                            MethodBase method = prop.PropertyType.GetMethod("Reset");
                            if (method != null)
                            {
                                method.Invoke(value, new object[] { });
                            }
                        }
                    }
                }

                OnReset();

                Core.Logger.Verbose(LogCategory.Configuration, "结束复位 {0}", type.Name);
            }
            catch (Exception ex)
            {
                Core.Logger.Error(LogCategory.None, "复位设置发生错误 {1} : {0}", ex.Message, typeof(T).Name);
            }
        }


        internal static void CopyTo<T>(ITrinitySetting<T> source, ITrinitySetting<T> destination, IEnumerable<string> ignorePropertyNames = null) where T : class, ITrinitySetting<T>
        {
            try
            {
                Type type = typeof(T);
                Core.Logger.Verbose(LogCategory.Configuration, "开始复制设置 {0}", type.Name);
                foreach (PropertyInfo prop in type.GetProperties(BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                {
                    try
                    {
                        if (Attribute.IsDefined(prop, typeof(IgnoreDataMemberAttribute)))
                            continue;

                        if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(destination, prop.GetValue(source, null), null);
                        }
                        else
                        {
                            object destinationValue = prop.GetValue(destination, null);
                            object sourceValue = prop.GetValue(source, null);

                            if (sourceValue == null || destinationValue == null)
                                continue;

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MethodBase method = prop.PropertyType.GetMethod("CopyTo", new[] { prop.PropertyType });
                                method?.Invoke(sourceValue, new[] { destinationValue });
                            });

                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Error(LogCategory.None, "复制与设置错误 {0} : {1} 属性: {2} {3}", typeof(T).Name, ex.Message, prop.Name, ex);
                    }
                }
                Core.Logger.Verbose(LogCategory.Configuration, "结束复制设置 {0}", type.Name);
            }
            catch (Exception ex)
            {
                Core.Logger.Error(LogCategory.None, "复制与设置错误 {1} : {0} {2}", ex.Message, typeof(T).Name, ex);
            }
        }

        internal static T Clone<T>(ITrinitySetting<T> setting) where T : class, ITrinitySetting<T>
        {
            try
            {
                Core.Logger.Verbose(LogCategory.Configuration, "开始克隆工作 {0}", typeof(T).Name);
                using (MemoryStream ms = new MemoryStream())
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(ms, setting);
                    return (T)serializer.ReadObject(ms);
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error(LogCategory.None, "克隆设置错误 {1} : {0}", ex.Message, typeof(T).Name);
                return null;
            }
            finally
            {
                Core.Logger.Verbose(LogCategory.Configuration, "结束克隆设置 {0}", typeof(T).Name);
            }
        }

        internal static void LoadDefaults<T>(ITrinitySetting<T> setting) where T : ITrinitySetting<T>
        {
            foreach (var p in setting.GetType().GetProperties())
            {
                foreach (var dv in p.GetCustomAttributes(true).OfType<DefaultValueAttribute>())
                {
                    p.SetValue(setting, dv.Value);
                }
            }
        }

        public static string GetSettingsXml<T>(T instance, string rootName = "") where T : ITrinitySetting<T>
        {
            if (string.IsNullOrEmpty(rootName))
                rootName = typeof(T).Name;

            var serializer = new DataContractSerializer(typeof(T), rootName, "");
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                serializer.WriteObject(writer, instance);
            }
            return sb.ToString();
        }

        public static T GetSettingsInstance<T>(string xml)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                return (T)serializer.ReadObject(reader);
            }
        }

    }
}
