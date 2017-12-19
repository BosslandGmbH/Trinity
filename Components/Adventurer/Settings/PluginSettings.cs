using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Components.Adventurer.Util;
using Trinity.UI.UIComponents;


namespace Trinity.Components.Adventurer.Settings
{
    public enum GemPriority
    {
        None = 0,
        Order,
        Chance,
        Rank,
    }

    [DataContract]
    public class PluginSettings : NotifyBase
    {
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
        private bool _debugLogging;
        private bool _useEmpoweredRifts;
        private int _empoweredRiftLevelLimit;
        private int _riftCount;
        private bool _useGemAutoLevel;
        private string _greaterRiftLevelMax;
        private int _gemAutoLevelReductionLimit;
        private int _minimumKeys;
        private long _minimumGold;

        private static Lazy<PluginSettings> _instance = new Lazy<PluginSettings>(() => new PluginSettings());
        private bool _gemUpgradeFocusMode;
        private GemPriority _gemUpgradePriority;

        public static PluginSettings Current => _instance.Value;

        [DataMember]
        public int HighestUnlockedRiftLevel
        {
            get
            {
                //var level = 0;
                //if (ZetaDia.Me != null)
                //{
                //    SafeFrameLock.ExecuteWithinFrameLock(() =>
                //    {
                //        level = PropertyReader<int>.SafeReadValue(() => ZetaDia.Me.HighestUnlockedRiftLevel);
                //    });
                //}
                //return level == 0 ? 120 : level;
                return 150;
            }
            set { }
        }

        public PluginSettings GetDataContext()
        {
            //UpdateGemList(); // >>> causing freeze on ActorCache accessing ActivePlayerData
            return this;
        }

        [DataMember]
        public int MinimumKeys
        {
            get { return _minimumKeys; }
            set { SetField(ref _minimumKeys, value); }
        }

        [DataMember]
        public long MinimumGold
        {
            get { return _minimumGold; }
            set { SetField(ref _minimumGold, value); }
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
        public bool DebugLogging
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
        [DefaultValue(GemPriority.Rank)]
        public GemPriority GemUpgradePriority
        {
            get { return _gemUpgradePriority; }
            set { SetField(ref _gemUpgradePriority, value); }
        }


        [DataMember]
        public AdventurerGems Gems
        {
            get { return _gems ?? (_gems = new AdventurerGems()); }
            set { SetField(ref _gems, value); }
        }

        /// <summary>
        /// Bound to UI rift level dropdown
        /// </summary>
        public string GreaterRiftLevelRaw
        {
            get
            {
                // Convert special values into special strings for the dropdown.
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
                // setter is called only by UI user selection
                if (value == "Max")
                {
                    // user selected max sets level to 0, if riftcoroutine encounters 0 it uses the current highest unlocked when rift is open.
                    GreaterRiftLevel = 0;
                }
                else
                {
                    int greaterRiftLevel;
                    if (value.Contains("Max - "))
                    {
                        if (int.TryParse(value.Replace("Max - ", string.Empty), out greaterRiftLevel))
                        {
                            GreaterRiftLevel = -greaterRiftLevel;
                            return;
                        }
                    }
                    if (int.TryParse(value, out greaterRiftLevel))
                    {
                        GreaterRiftLevel = greaterRiftLevel;
                    }
                }
            }
        }

        public PluginSettings()
        {
            LoadDefaults();
        }

        public override void LoadDefaults()
        {
            GemUpgradePriority = GemPriority.Rank;
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
            BountyMode0 = false;
            BountyMode1 = false;
            BountyMode2 = true;
            BountyMode3 = false;
            BountyPrioritizeBonusAct = true;
            NephalemRiftFullExplore = false;
            KeywardenZergMode = false;
            return;
        }

        [IgnoreDataMember]
        public List<string> GreaterRiftLevels
        {
            get
            {
                var unlockedRiftLevel = HighestUnlockedRiftLevel;

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
            var greaterRiftLevel = RiftData.GetGreaterRiftLevel();
            Gems.UpdateGems(greaterRiftLevel);
        }

        [IgnoreDataMember]
        public List<int> GemUpgradeChances
        {
            get { return new List<int> { 100, 90, 80, 70, 60, 30, 15, 8, 4, 2, 1 }; }
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

        public bool ApplySettingsCode(string code)
        {
            try
            {
                if (!code.StartsWith("{"))
                    code = ExportHelper.Decompress(code);

                var settings = LoadSettingsFromJsonString(code);
                ApplySettings(settings);
                return true;
            }
            catch (Exception ex)
            {
                Core.Logger.Debug($"Error parsing Adventurer settings code {ex}");
            }
            return false;
        }

        private void ApplySettings(PluginSettings settings)
        {
            PropertyCopy.Copy(settings, this);

            // Migration after bonus act removal. clean up later.
            if (settings.BountyMode0 == true || settings.BountyMode1 == true)
            {
                settings.BountyMode0 = false;
                settings.BountyMode1 = false;
                settings.BountyMode2 = true;
            }

            if (GemUpgradePriority == GemPriority.None)
                GemUpgradePriority = GemPriority.Rank;
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
            return new PluginSettings();
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

        public bool Save()
        {
            var result = JsonSerializer.Serialize(this);
            FileUtils.WriteToTextFile(FileUtils.SettingsPath, result);
            Core.Logger.Verbose("FileUtils.SettingsPath {0}", FileUtils.SettingsPath);
            Core.Logger.Log("Settings saved.");
            return false;
        }

        public string GenerateCode()
        {
            return JsonSerializer.Serialize(this);
        }

        #region DragDrop Handler

        public class UpdateOrderDropHandler : DefaultDropHandler
        {
            public override void Drop(IDropInfo dropInfo)
            {
                base.Drop(dropInfo);
                var items = dropInfo.TargetCollection.OfType<AdventurerGemSetting>().ToList();
                for (var index = 0; index < items.Count; index++)
                {
                    var skillSettings = items[index];
                    skillSettings.Order = index;
                }
            }
        }

        [IgnoreDataMember]
        public UpdateOrderDropHandler DropHandler { get; } = new UpdateOrderDropHandler();

        #endregion DragDrop Handler
    }
}