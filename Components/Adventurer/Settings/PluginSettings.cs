using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Components.Adventurer.Util;
using Trinity.Framework.Helpers;
using Trinity.Helpers;
using Zeta.Game;
using JsonSerializer = Trinity.Components.Adventurer.Util.JsonSerializer;

namespace Trinity.Components.Adventurer.Settings
{

    [DataContract]
    public class PluginSettings : NotifyBase
    {
        private static ConcurrentDictionary<int, PluginSettings> _settings = new ConcurrentDictionary<int, PluginSettings>();
        private AdventurerGems _gems;
        private int highestUnlockedRiftLevel;
        private int _highestUnlockedRiftLevel;
        private bool _normalRiftForXpShrine;
        private int _greaterRiftLevel;
        private bool _greaterRiftRunNephalem;
        private int _greaterRiftGemUpgradeChance;
        private bool _greaterRiftPrioritizeEquipedGems;
        private bool _bountyAct1;
        private bool _bountyAct2;
        private bool _bountyAct3;
        private bool _bountyAct4;
        private bool _bountyAct5;
        private bool _bountyZerg;
        private bool _bountyPrioritizeBonusAct;
        private bool? _bountyMode0;
        private bool? _bountyMode1;
        private bool? _bountyMode2;
        private bool? _bountyMode3;
        private bool _nephalemRiftFullExplore;
        private bool? _keywardenZergMode;
        private bool? _debugLogging;
        private bool _useEmpoweredRifts;
        private int _empoweredRiftLevelLimit;
        private int _riftCount;
        private bool _useGemAutoLevel;
        private string _greaterRiftLevelMax;
        private int _gemAutoLevelReductionLimit;

        public static PluginSettings Current { get { return _settings.GetOrAdd(AdvDia.BattleNetHeroId, LoadCurrent()); } }

        [DataMember]
        public int HighestUnlockedRiftLevel
        {
            get { return _highestUnlockedRiftLevel; }
            set { SetField(ref _highestUnlockedRiftLevel, value); }
        }

        [DataMember]
        public bool NormalRiftForXPShrine
        {
            get { return _normalRiftForXpShrine; }
            set { SetField(ref _normalRiftForXpShrine, value); }
        }

        [DataMember]
        public int GreaterRiftLevel
        {
            get { return _greaterRiftLevel; }
            set
            {
                SetField(ref _greaterRiftLevel, value);
                OnPropertyChanged(nameof(GreaterRiftLevelRaw));
            }
        }

        [DataMember]
        public bool GreaterRiftRunNephalem
        {
            get { return _greaterRiftRunNephalem; }
            set { SetField(ref _greaterRiftRunNephalem, value); }
        }

        [DataMember]
        public int GreaterRiftGemUpgradeChance
        {
            get { return _greaterRiftGemUpgradeChance; }
            set { SetField(ref _greaterRiftGemUpgradeChance, value); }
        }

        [DataMember]
        public bool GreaterRiftPrioritizeEquipedGems
        {
            get { return _greaterRiftPrioritizeEquipedGems; }
            set { SetField(ref _greaterRiftPrioritizeEquipedGems, value); }
        }

        [DataMember]
        public bool BountyAct1
        {
            get { return _bountyAct1; }
            set { SetField(ref _bountyAct1, value); }
        }

        [DataMember]
        public bool BountyAct2
        {
            get { return _bountyAct2; }
            set { SetField(ref _bountyAct2, value); }
        }

        [DataMember]
        public bool BountyAct3
        {
            get { return _bountyAct3; }
            set { SetField(ref _bountyAct3, value); }
        }

        [DataMember]
        public bool BountyAct4
        {
            get { return _bountyAct4; }
            set { SetField(ref _bountyAct4, value); }
        }

        [DataMember]
        public bool BountyAct5
        {
            get { return _bountyAct5; }
            set { SetField(ref _bountyAct5, value); }
        }

        [DataMember]
        public bool BountyZerg
        {
            get { return _bountyZerg; }
            set { SetField(ref _bountyZerg, value); }
        }

        [DataMember]
        public bool BountyPrioritizeBonusAct
        {
            get { return _bountyPrioritizeBonusAct; }
            set { SetField(ref _bountyPrioritizeBonusAct, value); }
        }

        [DataMember]
        public bool? BountyMode0
        {
            get { return _bountyMode0; }
            set { SetField(ref _bountyMode0, value); }
        }

        [DataMember]
        public bool? BountyMode1
        {
            get { return _bountyMode1; }
            set { SetField(ref _bountyMode1, value); }
        }

        [DataMember]
        public bool? BountyMode2
        {
            get { return _bountyMode2; }
            set { SetField(ref _bountyMode2, value); }
        }

        [DataMember]
        public bool? BountyMode3
        {
            get { return _bountyMode3; }
            set { SetField(ref _bountyMode3, value); }
        }

        [DataMember]
        public bool NephalemRiftFullExplore
        {
            get { return _nephalemRiftFullExplore; }
            set { SetField(ref _nephalemRiftFullExplore, value); }
        }

        [DataMember]
        public bool? KeywardenZergMode
        {
            get { return _keywardenZergMode; }
            set { SetField(ref _keywardenZergMode, value); }
        }

        [DataMember]
        public bool? DebugLogging
        {
            get { return _debugLogging; }
            set { SetField(ref _debugLogging, value); }
        }

        [DataMember]
        public bool UseEmpoweredRifts
        {
            get { return _useEmpoweredRifts; }
            set { SetField(ref _useEmpoweredRifts, value); }
        }

        [DataMember]
        [DefaultValue(40)]
        public int EmpoweredRiftLevelLimit
        {
            get { return _empoweredRiftLevelLimit; }
            set { SetField(ref _empoweredRiftLevelLimit, value); }
        }

        [DataMember]
        public int RiftCount
        {
            get { return _riftCount; }
            set
            {
                SetField(ref _riftCount, value);
                OnPropertyChanged(nameof(RiftCountSetting));
            }
        }

        [DataMember]
        public bool UseGemAutoLevel
        {
            get { return _useGemAutoLevel; }
            set { SetField(ref _useGemAutoLevel, value); }
        }

        [DataMember]
        public string GreaterRiftLevelMax
        {
            get { return _greaterRiftLevelMax; }
            set { SetField(ref _greaterRiftLevelMax, value); }
        }

        [DataMember]
        [DefaultValue(20)]
        public int GemAutoLevelReductionLimit
        {
            get { return _gemAutoLevelReductionLimit; }
            set { SetField(ref _gemAutoLevelReductionLimit, value); }
        }

        [DataMember]
        public AdventurerGems Gems
        {
            get
            {
                if (_gems == null)
                {
                    _gems = new AdventurerGems();
                }
                var greaterRiftLevel = RiftData.GetGreaterRiftLevel();
                _gems.UpdateGems(greaterRiftLevel, GreaterRiftPrioritizeEquipedGems);
                return _gems;
            }
            set
            {
                SetField(ref _gems, value);
            }
        }

        public string GreaterRiftLevelRaw
        {
            get
            {
                switch (GreaterRiftLevel)
                {
                    case 0:
                        return "Max";
                    case -1:
                    case -2:
                    case -3:
                    case -4:
                    case -5:
                    case -6:
                    case -7:
                    case -8:
                    case -9:
                    case -10:
                        return "Max - " + (GreaterRiftLevel * -1);
                    default:
                        return GreaterRiftLevel.ToString();
                }
            }
            set
            {
                if (value == "Max")
                {
                    GreaterRiftLevel = 0;
                }
                else
                {
                    int greaterRiftLevel;
                    if (int.TryParse(value.Replace("Max - ", string.Empty), out greaterRiftLevel))
                    {
                        GreaterRiftLevel = greaterRiftLevel;
                    }
                    if (value.Contains("Max")) GreaterRiftLevel = GreaterRiftLevel * -1;
                }
            }
        }

        [IgnoreDataMember]
        public List<AdventurerGem> GemUpgradePriority => Gems.Gems;

        public PluginSettings() { }
        public PluginSettings(bool initializeDefaults)
        {
            if (!initializeDefaults) return;

            LoadDefaults();
        }

        public void LoadDefaults()
        {
            GreaterRiftLevel = 1;
            GreaterRiftRunNephalem = true;
            GreaterRiftGemUpgradeChance = 60;
            GreaterRiftPrioritizeEquipedGems = true;
            EmpoweredRiftLevelLimit = 60;
            GemAutoLevelReductionLimit = 20;
            BountyAct1 = true;
            BountyAct2 = true;
            BountyAct3 = true;
            BountyAct4 = true;
            BountyAct5 = true;
            BountyZerg = false;
            BountyMode0 = true;
            BountyMode1 = false;
            BountyMode2 = false;
            BountyMode3 = false;
            BountyPrioritizeBonusAct = true;
            NephalemRiftFullExplore = false;
            KeywardenZergMode = false;
        }

        [IgnoreDataMember]
        public List<string> GreaterRiftLevels
        {
            get
            {
                var unlockedRiftLevel = 0;

                var result = SafeFrameLock.ExecuteWithinFrameLock(() =>
                {
                    unlockedRiftLevel = ZetaDia.Me.HighestUnlockedRiftLevel;

                }, true);

                if (!result.Success)
                {
                    //Logger.Error("[Settings][GreaterRiftLevels] " + result.Exception.Message);
                    unlockedRiftLevel = HighestUnlockedRiftLevel;
                }
                else
                {
                    HighestUnlockedRiftLevel = unlockedRiftLevel;
                }

                if (unlockedRiftLevel == 0)
                {
                    unlockedRiftLevel = 1;
                }

                var levels = new List<string>();
                for (var i = 1; i <= unlockedRiftLevel; i++)
                {
                    levels.Add(i.ToString());
                }

                var highest = unlockedRiftLevel;
                if (highest > 10) highest = 10;

                for (var i = highest - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        levels.Insert(0, "Max");
                    }
                    else
                    {
                        levels.Insert(0, "Max - " + i);
                    }
                }

                return levels;
            }
        }

        public void UpdateGemList()
        {
            if (_gems != null)
            {
                var greaterRiftLevel = RiftData.GetGreaterRiftLevel();
                _gems.UpdateGems(greaterRiftLevel, GreaterRiftPrioritizeEquipedGems);
            }
        }

        [IgnoreDataMember]
        public List<int> GemUpgradeChances
        {
            get { return new List<int> { 100, 90, 80, 70, 60, 30, 15, 8, 4, 2, 1, 0 }; }
        }

        [IgnoreDataMember]

        public List<string> RiftCounts
        {
            get { return new List<string> { "Infinity", "1", "5", "10", "20", "50" }; }
        }

        public string RiftCountSetting
        {
            get
            {
                return RiftCount <= 0 ? "Infinity" : RiftCount.ToString();
            }
            set
            {
                if (value == "Infinity")
                {
                    RiftCount = 0;
                }
                else
                {
                    int riftCount;
                    if (int.TryParse(value, out riftCount))
                    {
                        RiftCount = riftCount;
                    }
                }
            }
        }

        public static PluginSettings LoadCurrent()
        {
            return LoadSettingsFromJsonString(FileUtils.ReadFromTextFile(FileUtils.SettingsPath));
        }

        public void ApplySettingsCode(string code)
        {
            var settings = LoadSettingsFromJsonString(ExportHelper.Decompress(code));
            ApplySettings(settings);
        }

        private void ApplySettings(PluginSettings settings)
        {
            PropertyCopy.Copy(settings, this);
        }

        public static PluginSettings LoadSettingsFromJsonString(string json)
        {        
            if (!string.IsNullOrEmpty(json))
            {
                var current = JsonSerializer.Deserialize<PluginSettings>(json);
                if (current != null)
                {
                    if (!current.KeywardenZergMode.HasValue)
                        current.KeywardenZergMode = true;

                    var isMode0 = current.BountyMode0.HasValue && current.BountyMode0.Value;
                    var isMode1 = current.BountyMode1.HasValue && current.BountyMode1.Value;
                    var isMode2 = current.BountyMode2.HasValue && current.BountyMode2.Value;
                    var isMode3 = current.BountyMode3.HasValue && current.BountyMode3.Value;

                    var modes = (isMode0 ? 1 : 0) + (isMode1 ? 1 : 0) + (isMode2 ? 1 : 0) + (isMode3 ? 1 : 0);

                    if (modes > 1 || modes == 0)
                    {
                        current.BountyMode0 = true;
                        current.BountyMode1 = false;
                        current.BountyMode2 = false;
                        current.BountyMode3 = false;
                    }
                    else
                    {
                        if (!current.BountyMode0.HasValue)
                            current.BountyMode0 = false;
                        if (!current.BountyMode1.HasValue)
                            current.BountyMode1 = false;
                        if (!current.BountyMode2.HasValue)
                            current.BountyMode2 = false;
                        if (!current.BountyMode3.HasValue)
                            current.BountyMode3 = false;
                    }
                    return current;
                }
            }
            return new PluginSettings(true);
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            foreach (var p in GetType().GetProperties())
            {
                foreach (var dv in p.GetCustomAttributes(true).OfType<DefaultValueAttribute>())
                {
                    p.SetValue(this, dv.Value);
                }
            }
        }

        public void Save()
        {
            var result = JsonSerializer.Serialize(this);
            FileUtils.WriteToTextFile(FileUtils.SettingsPath, result);
            Logger.Verbose("FileUtils.SettingsPath {0}", FileUtils.SettingsPath);
            Logger.Info("Settings saved.");
        }

        public string GenerateCode()
        {
            return ExportHelper.Compress(JsonSerializer.Serialize(this));
        }
    }

}
