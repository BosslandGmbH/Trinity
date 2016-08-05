﻿using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml;
using Trinity.Config.Combat;
using Trinity.Framework;
using Trinity.Framework.Objects;
using Trinity.Helpers;
using Trinity.Settings.Loot;
using Trinity.Technicals;
using Zeta.Bot.Settings;
using Zeta.Game;
using Extensions = Zeta.Common.Extensions;

namespace Trinity.Config
{
    [DataContract(Namespace = "")]
    public class TrinitySetting : NotifyBase, ITrinitySetting<TrinitySetting>
    {
        #region Fields
        private CombatSetting _Combat;
        private WorldObjectSetting _WorldObject;
        private ItemSetting _Loot;
        private AdvancedSetting _Advanced;
        private NotificationSetting _Notification;
        private FileSystemWatcher _FSWatcher;
        private DateTime _LastLoadedSettings;
        private KanaisCubeSetting _kanaisCube;
        private GamblingSetting _gambling;
        private AvoidanceSetting _avoidance;
        private ParagonSetting _paragon;

        #endregion Fields

        #region Events
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void SettingsEvent();
        public static event SettingsEvent OnSave = () => { };
        public static event SettingsEvent OnLoaded = () => { };
        public static event SettingsEvent OnReset = () => { };
        public static event SettingsEvent OnUserRequestedReset = () => { };

        #endregion Events

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TrinitySetting" /> class.
        /// </summary>
        public TrinitySetting(bool initialize = true)
        {
            if (initialize)
            {
                Combat = new CombatSetting();
                WorldObject = new WorldObjectSetting();
                Loot = new ItemSetting();
                Advanced = new AdvancedSetting();
                Notification = new NotificationSetting();
                KanaisCube = new KanaisCubeSetting();
                Gambling = new GamblingSetting();
                Avoidance = new AvoidanceSetting();
                Paragon = new ParagonSetting();
                Dynamic = new DynamicSettingGroup();
            }

            _FSWatcher = new FileSystemWatcher()
            {
                Path = Path.GetDirectoryName(GlobalSettingsFile),
                Filter = Path.GetFileName(GlobalSettingsFile),
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            _FSWatcher.Changed += _FSWatcher_Changed;
            _LastLoadedSettings = DateTime.MinValue;
        }

        #endregion Constructors

        #region Properties

        [DataMember(IsRequired = false)]
        public DynamicSettingGroup Dynamic
        {
            get { return _dynamic; }
            set { SetField(ref _dynamic, value); }
        }

        [DataMember(IsRequired = false)]
        public CombatSetting Combat
        {
            get
            {
                return _Combat;
            }
            set
            {
                if (_Combat != value)
                {
                    _Combat = value;
                    OnPropertyChanged("Combat");
                }
            }
        }

        [DataMember(IsRequired = false)]
        public ParagonSetting Paragon
        {
            get
            {
                return _paragon;
            }
            set
            {
                if (_paragon != value)
                {
                    _paragon = value;
                    OnPropertyChanged("Paragon");
                }
            }
        }

        [DataMember(IsRequired = false)]
        public AvoidanceSetting Avoidance
        {
            get
            {
                return _avoidance;
            }
            set
            {
                if (_avoidance != value)
                {
                    _avoidance = value;
                    OnPropertyChanged("Avoidance");
                }
            }
        }


        [DataMember(IsRequired = false)]
        public WorldObjectSetting WorldObject
        {
            get
            {
                return _WorldObject;
            }
            set
            {
                if (_WorldObject != value)
                {
                    _WorldObject = value;
                    OnPropertyChanged("WorldObject");
                }
            }
        }

        [DataMember(IsRequired = false)]
        public ItemSetting Loot
        {
            get
            {
                return _Loot;
            }
            set
            {
                if (_Loot != value)
                {
                    _Loot = value;
                    OnPropertyChanged("Loot");
                }
            }
        }

        [DataMember(IsRequired = false)]
        public AdvancedSetting Advanced
        {
            get
            {
                return _Advanced;
            }
            set
            {
                if (_Advanced != value)
                {
                    _Advanced = value;
                    OnPropertyChanged("Advanced");
                }
            }
        }


        [DataMember(IsRequired = false)]
        public KanaisCubeSetting KanaisCube
        {
            get
            {
                return _kanaisCube;
            }
            set
            {
                if (_kanaisCube != value)
                {
                    _kanaisCube = value;
                    OnPropertyChanged("KanaisCube");
                }
            }
        }

        [DataMember(IsRequired = false)]
        public GamblingSetting Gambling
        {
            get
            {
                return _gambling;
            }
            set
            {
                if (_gambling != value)
                {
                    _gambling = value;
                    OnPropertyChanged("Gambling");
                }
            }
        }

        [DataMember(IsRequired = false)]
        public NotificationSetting Notification
        {
            get
            {
                return _Notification;
            }
            set
            {
                if (_Notification != value)
                {
                    _Notification = value;
                    OnPropertyChanged("Notification");
                }
            }
        }

        [IgnoreDataMember]
        internal string BattleTagSettingsFile
        {
            get
            {
                return Path.Combine(FileManager.SpecificSettingsPath, "Trinity.xml");
            }
        }

        private int _currentHeroId;
        private DynamicSettingGroup _dynamic;

        [IgnoreDataMember]
        internal string HeroSpecificSettingsFile
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
        internal string OldBattleTagSettingsFile
        {
            get
            {
                return Path.Combine(FileManager.SpecificSettingsPath, "GilesTrinity.xml");
            }
        }

        [IgnoreDataMember]
        internal string GlobalSettingsFile
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Settings", "Trinity.xml");
            }
        }

        #endregion Properties

        #region Methods
        private void _FSWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(250);
            Load();
        }

        public void UserRequestedReset()
        {
            Logger.Log("UserRequestedReset called");
            Reset(this);
            OnUserRequestedReset();
        }

        public void Reset()
        {
            Reset(this);
        }

        public void CopyTo(TrinitySetting setting)
        {
            CopyTo(this, setting);
        }

        public TrinitySetting Clone()
        {
            return Clone(this);
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
                        Logger.Log("Loading Global Settings, You can use per-battletag settings by removing the Trinity.xml file under your Demonbuddy settings directory");

                        var globalSettings = LoadSettingsFromFile(filename);
                        loadSuccessful = globalSettings != null;

                        //if (Core.Settings.Advanced.ForceSpecificGambleSettings && File.Exists(BattleTagSettingsFile))
                        //{
                        //    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Loading BattleTag Settings for Gambling");
                        //    var specificSettings = LoadSettingsFromFile(BattleTagSettingsFile, false);
                        //    specificSettings.Gambling.CopyTo(this.Gambling);                            
                        //}
                    }
                    else if (File.Exists(HeroSpecificSettingsFile))
                    {
                        Logger.Log("Loading Hero Specific Settings");
                        filename = HeroSpecificSettingsFile;

                        var settings = LoadSettingsFromFile(filename);
                        loadSuccessful = settings != null;
                    }
                    else if (File.Exists(BattleTagSettingsFile))
                    {
                        Logger.Log("Loading BattleTag Settings");
                        filename = BattleTagSettingsFile;

                        var settings = LoadSettingsFromFile(filename);
                        loadSuccessful = settings != null;
                    }
                    else if (File.Exists(OldBattleTagSettingsFile))
                    {
                        Logger.Log(TrinityLogLevel.Debug, LogCategory.UserInformation, "Old configuration file found, need to migrate!");
                        filename = OldBattleTagSettingsFile;
                        migrateConfig = true;

                        var settings = LoadSettingsFromFile(filename);
                        loadSuccessful = settings != null;
                    }

                    FireOnLoadedEvents();
                }
                catch (Exception ex)
                {
                    Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "Error while loading Config file: {0}", ex);
                    loadSuccessful = false;
                    migrateConfig = false;
                }

                if (migrateConfig && loadSuccessful)
                {
                    Logger.Log(TrinityLogLevel.Debug, LogCategory.UserInformation, "Migrating configuration to new Trinity.xml");
                    Save();
                    File.Delete(OldBattleTagSettingsFile);
                }

            }

            Logger.UpdatePrefix();
        }

        public void FireOnLoadedEvents()
        {
            var eventSupporters = GetInterfaceMembers<ITrinitySettingEvents>(this);
            foreach (var eventSupporter in eventSupporters)
            {
                eventSupporter.OnLoaded();
            }
        }

        public static TrinitySetting GetSettingsFromFile(string filename)
        {
            return new TrinitySetting().LoadSettingsFromFile(filename, false);
        }

        public TrinitySetting LoadSettingsFromFile(string filename, bool applyToThis = true)
        {
            TrinitySetting loadedSettings = null;

            if (File.Exists(filename))
            {
                DateTime fsChangeStart = DateTime.UtcNow;
                while (FileManager.IsFileReadLocked(new FileInfo(GlobalSettingsFile)))
                {
                    Thread.Sleep(10);
                    if (DateTime.UtcNow.Subtract(fsChangeStart).TotalMilliseconds > 5000)
                        break;
                }
                using (Stream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    DataContractSerializer serializer = new DataContractSerializer(this.GetType());
                    XmlReader reader = XmlReader.Create(stream);
                    XmlReader migrator = new SettingsMigrator(reader);

                    loadedSettings = (TrinitySetting)serializer.ReadObject(migrator);

                    if (applyToThis)
                    {
                        loadedSettings.CopyTo(this);
                        ApplyDataMigration(this);
                    }
                    else
                    {
                        ApplyDataMigration(loadedSettings);
                    }

                    stream.Close();

                    LoadDynamicSettings();

                    Logger.Log("Configuration file loaded");

                    // this tests to make sure we didn't load anything null, and our load was succesful
                    if (Advanced != null && Combat != null && Combat.Misc != null)
                    {
                        Logger.Log("Configuration loaded successfully.");
                        OnLoaded();
                    }
                }
            }
            else
            {
                Logger.Log(TrinityLogLevel.Debug, LogCategory.UserInformation, "Configuration file not found.");
                Reset();
            }            
            return loadedSettings;
        }

        private void ApplyDataMigration(TrinitySetting loadedSettings)
        {
            if (loadedSettings?.Loot?.Pickup != null && loadedSettings.Loot.Pickup.ItemFilterMode == default(ItemFilterMode))
            {
                if (loadedSettings.Loot.ItemFilterMode == default(ItemFilterMode))
                {
                    loadedSettings.Loot.Pickup.ItemFilterMode = ItemFilterMode.TrinityOnly;
                }
                else
                {
                    Logger.LogVerbose(LogCategory.Configuration, $"Migrating Setting: ItemFilterMode: {loadedSettings.Loot.ItemFilterMode} from .Loot.Pickup.ItemFilterMode to .Loot.Pickup.ItemFilterMode");
                    loadedSettings.Loot.Pickup.ItemFilterMode = loadedSettings.Loot.ItemFilterMode;
                }
            }
        }

        //public class TrinitySettingMigrationResolver : DataContractResolver
        //{
        //    public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        //    {
        //        return knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace);
        //    }

        //    public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        //    {
        //        return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null) ?? declaredType;
        //    }
        //}


        public void Save(bool useGlobal = false)
        {
            lock (this)
            {
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

                        //if (Core.Settings.Advanced.ForceSpecificGambleSettings && File.Exists(BattleTagSettingsFile))
                        //{
                        //    Logger.Log("Saving Gambling settings to Specific settings file");
                        //    filename = BattleTagSettingsFile;
                        //    var settings = LoadSettingsFromFile(filename, false);
                        //    this.Gambling.CopyTo(settings.Gambling);
                        //    SaveToFile(filename, settings);
                        //} 
                    }
                    else
                    {
                        filename = HeroSpecificSettingsFile;
                        SaveToFile(filename, this);
                    }

                    FireOnSaveEvents();
                });

            }
            Logger.UpdatePrefix();
        }

        /// <summary>
        /// Copy the current settings from modules and put them into Trinity settings.
        /// </summary>
        public void SaveDynamicSettings()
        {
            Dynamic.Settings.Clear();
            foreach (var item in ModuleManager.DynamicSettings)
            {
                Dynamic.Settings.Add(new DynamicSettingNode
                {
                    Name = item.Name,
                    Code = item.GetCode()
                });
            }
        }

        /// <summary>
        /// Apply the settings loaded from Trinity settings over to their respective modules.
        /// </summary>
        public void LoadDynamicSettings()
        {
            if (Dynamic == null)
                return;

            foreach (var item in Dynamic.Settings)
            {
                var setting = item.Setting;
                if (setting == null)
                    continue;

                if (string.IsNullOrEmpty(item.Code))
                {
                    setting.Reset();
                }
                else
                {
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

        public void SaveToFile(string filePath, TrinitySetting settings = null)
        {
            try
            {
                if (settings == null)
                    settings = this;

                if (filePath == null)
                {
                    throw new ArgumentNullException(nameof(filePath));
                }

                var dirName = Path.GetDirectoryName(filePath);
                if (dirName != null && !Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);

                _FSWatcher.EnableRaisingEvents = false;

                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Saving Config file");
                using (Stream stream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(TrinitySetting), "TrinitySetting", "");

                    var xmlWriterSettings = new XmlWriterSettings { Indent = true };
                    using (var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        serializer.WriteObject(xmlWriter, settings);
                    }
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "Error while saving Config file: {0}", ex);
            }
            finally
            {
                _FSWatcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                Logger.Log("TrinitySettings Property Changed. {0}", propertyName);
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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

        #endregion Methods

        #region Static Methods
        internal static void Reset<T>(ITrinitySetting<T> setting) where T : class, ITrinitySetting<T>
        {
            try
            {
                Type type = typeof(T);
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.Configuration, "Starting Reset Object {0}", type.Name);
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

                Logger.Log(TrinityLogLevel.Verbose, LogCategory.Configuration, "End Reset Object {0}", type.Name);
            }
            catch (Exception ex)
            {
                Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "Error while Reset Setting {1} : {0}", ex.Message, typeof(T).Name);
            }
        }

        private static IEnumerable<string> _ignorePropertyNames = new List<string>();

        internal static void CopyTo<T>(ITrinitySetting<T> source, ITrinitySetting<T> destination, IEnumerable<string> ignorePropertyNames = null) where T : class, ITrinitySetting<T>
        {
            try
            {
                //if (ignorePropertyNames != null)
                //    _ignorePropertyNames = ignorePropertyNames;

                Type type = typeof(T);
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.Configuration, "Starting CopyTo Object {0}", type.Name);
                foreach (PropertyInfo prop in type.GetProperties(BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                {
                    try
                    {
                        if (Attribute.IsDefined(prop, typeof(IgnoreDataMemberAttribute)))
                            continue;

                        //if (_ignorePropertyNames.Contains(prop.Name))
                        //    continue;

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
                                if (method != null)
                                {
                                    method.Invoke(sourceValue, new[] { destinationValue });
                                }
                                //else
                                //{
                                //    // use a derived collection such as the FullyObservableCollection for ObservableCollection, that implements CopyTo.
                                //    prop.SetValue(destination, sourceValue);
                                //}
                            });

                            //else if (sourceValue != null && destinationValue != null)
                            //{
                            //    MethodBase method = prop.PropertyType.GetMethod("Clone", null);
                            //    if (method != null)
                            //    {
                            //        prop.SetValue(destination, method.Invoke(sourceValue, null), null);
                            //    }
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "Error while CopyTo Setting {0} : {1} Property: {2} {3}", typeof(T).Name, ex.Message, prop.Name, ex);
                    }
                }
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.Configuration, "End CopyTo Object {0}", type.Name);
            }
            catch (Exception ex)
            {
                Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "Error while CopyTo Setting {1} : {0} {2}", ex.Message, typeof(T).Name, ex);
            }
            finally
            {
                //if(ignorePropertyNames != null)
                //    _ignorePropertyNames = new List<string>();
            }
        }

        static bool IsNullable(Type type)
        {
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        internal static T Clone<T>(ITrinitySetting<T> setting) where T : class, ITrinitySetting<T>
        {
            try
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.Configuration, "Starting Clone Object {0}", typeof(T).Name);
                using (MemoryStream ms = new MemoryStream())
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(ms, setting);
                    //ms.Seek(0, SeekOrigin.Begin);
                    return (T)serializer.ReadObject(ms);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(TrinityLogLevel.Error, LogCategory.UserInformation, "Error while Clone Setting {1} : {0}", ex.Message, typeof(T).Name);
                return null;
            }
            finally
            {
                Logger.Log(TrinityLogLevel.Verbose, LogCategory.Configuration, "End Clone Object {0}", typeof(T).Name);
            }
        }

        /// <summary>
        /// Converts a settings object to Xml
        /// </summary>
        /// <typeparam name="T">Type of settings instance</typeparam>
        /// <param name="instance">Settings instance to be serialized to Xml</param>
        /// <param name="rootName">Name of the base node in resulting Xml</param>
        /// <returns>string of settings as Xml</returns>
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

        /// <summary>
        /// Converts Xml of settings to a settings object
        /// </summary>
        /// <typeparam name="T">Type of the settings you want</typeparam>
        /// <param name="xml">Xml string of settings</param>
        /// <returns>Instance of Settings Class</returns>
        public static T GetSettingsInstance<T>(string xml) where T : ITrinitySetting<T>
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                XmlReader migrator = new SettingsMigrator(reader);
                var loadedSetting = (T)serializer.ReadObject(migrator);
                return loadedSetting;
            }
        }

        #endregion Static Methods

    }

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

    [CollectionDataContract(Namespace = "", ItemName = "Node")]
    public class DynamicNodeCollection : ObservableCollection<DynamicSettingNode> { }

    [DataContract(Namespace = "")]
    public class DynamicSettingNode : NotifyBase
    {
        private string _name;
        private string _code;

        [IgnoreDataMember]
        public IDynamicSetting Setting => ModuleManager.DynamicSettings.FirstOrDefault(s => s.Name == Name);

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        [DataMember]
        public string Code
        {
            get { return _code; }
            set { SetField(ref _code, value); }
        }
    }




}