using System.ComponentModel;
using System.Runtime.Serialization;
using Trinity.Framework.Helpers;
using Zeta.Game;


namespace Trinity.Settings.Paragon
{
    [DataContract(Namespace = "")]
    public sealed class ParagonSettings : NotifyBase
    {
        public ParagonSettings()
        {
            LoadDefaults();
        }

        private ParagonCollection _coreParagonPriority = new ParagonCollection(ParagonCategory.PrimaryAttributes);
        private ParagonCollection _utilityParagonPriority = new ParagonCollection(ParagonCategory.Utility);
        private ParagonCollection _defenseParagonPriority = new ParagonCollection(ParagonCategory.Defense);
        private ParagonCollection _offenseParagonPriority = new ParagonCollection(ParagonCategory.Offense);

        private bool _isEnabled;
        private bool _isCustomize;

        public class ParagonCollection : FullyObservableCollection<ParagonItem>
        {
            public ParagonCollection() { }

            public ParagonCollection(ParagonCategory category)
            {
                Category = category;
            }

            public ParagonCategory Category { get; set; }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ParagonCollection CoreParagonPriority
        {
            get => _coreParagonPriority;
            set => SetField(ref _coreParagonPriority, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        [DefaultValue(true)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetField(ref _isEnabled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool IsCustomize
        {
            get => _isCustomize;
            set => SetField(ref _isCustomize, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ParagonCollection OffenseParagonPriority
        {
            get => _offenseParagonPriority;
            set => SetField(ref _offenseParagonPriority, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ParagonCollection DefenseParagonPriority
        {
            get => _defenseParagonPriority;
            set => SetField(ref _defenseParagonPriority, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ParagonCollection UtilityParagonPriority
        {
            get => _utilityParagonPriority;
            set => SetField(ref _utilityParagonPriority, value);
        }

        //public void OnSave()
        //{
        //    Core.Logger.Verbose("Saving Paragon Priority");
        //}

        //public void OnLoaded()
        //{
        //    Core.Logger.Verbose("Loading Paragon Priority");

        //    if (!CoreParagonPriority.Any() || !UtilityParagonPriority.Any() || !DefenseParagonPriority.Any() || !OffenseParagonPriority.Any())
        //    {
        //        LoadDefaults();
        //    }
        //    else
        //    {
        //        CoreParagonPriority.ForEach(i => i.Populate());
        //        OffenseParagonPriority.ForEach(i => i.Populate());
        //        DefenseParagonPriority.ForEach(i => i.Populate());
        //        UtilityParagonPriority.ForEach(i => i.Populate());
        //    }
        //}

        public override void LoadDefaults()
        {
            IsEnabled = true;
            IsCustomize = false;

            CoreParagonPriority = new ParagonCollection(ParagonCategory.PrimaryAttributes)
            {
                new ParagonItem(TrinityParagonBonusType.MovementSpeed),
                new ParagonItem(TrinityParagonBonusType.PrimaryStat),
                new ParagonItem(TrinityParagonBonusType.Vitality),
                new ParagonItem(TrinityParagonBonusType.Resource),
            };

            UtilityParagonPriority = new ParagonCollection(ParagonCategory.Utility)
            {
                new ParagonItem(TrinityParagonBonusType.AreaDamage),
                new ParagonItem(TrinityParagonBonusType.ResourceCost),
                new ParagonItem(TrinityParagonBonusType.LifeOnHit),
                new ParagonItem(TrinityParagonBonusType.GoldFind),
            };

            DefenseParagonPriority = new ParagonCollection(ParagonCategory.Defense)
            {
                new ParagonItem(TrinityParagonBonusType.Armor),
                new ParagonItem(TrinityParagonBonusType.Life),
                new ParagonItem(TrinityParagonBonusType.ResistAll),
                new ParagonItem(TrinityParagonBonusType.LifeRegen),
            };

            OffenseParagonPriority = new ParagonCollection(ParagonCategory.Offense)
            {
                new ParagonItem(TrinityParagonBonusType.CriticalChance),
                new ParagonItem(TrinityParagonBonusType.CriticalDamage),
                new ParagonItem(TrinityParagonBonusType.AttackSpeed),
                new ParagonItem(TrinityParagonBonusType.CooldownReduction),
            };
        }


    }

}
