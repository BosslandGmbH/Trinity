﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;

using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using Trinity.Config.Combat;
using Trinity.Framework;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Avoidance.Handlers;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Helpers;
using Trinity.Settings.Loot;
using TrinityCoroutines.Resources;
using Trinity.Technicals;
using log4net;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Config
{
    [DataContract(Namespace = "")]
    public class ParagonSetting : NotifyBase, ITrinitySetting<ParagonSetting>, ITrinitySettingEvents
    {
        private ParagonCollection _coreParagonPriority;
        private ParagonCollection _utilityParagonPriority;
        private ParagonCollection _defenseParagonPriority;
        private ParagonCollection _offenseParagonPriority;
        private bool _isEnabled;

        public class ParagonCollection : FullyObservableCollection<ParagonItem>
        {
            public ParagonCollection() { }

            public ParagonCollection(ParagonCategory category)
            {
                Category = category;
            }

            public ParagonCategory Category { get; set; }
        }

        public ParagonSetting()
        {
            _coreParagonPriority = new ParagonCollection(ParagonCategory.PrimaryAttributes);
            _utilityParagonPriority = new ParagonCollection(ParagonCategory.Utility);
            _defenseParagonPriority = new ParagonCollection(ParagonCategory.Defense);
            _offenseParagonPriority = new ParagonCollection(ParagonCategory.Offense);
        }

        [DataMember(IsRequired = false)]
        public ParagonCollection CoreParagonPriority
        {
            get { return _coreParagonPriority; }
            set { SetField(ref _coreParagonPriority, value); }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetField(ref _isEnabled, value); }
        }

        [DataMember(IsRequired = false)]
        public ParagonCollection OffenseParagonPriority
        {
            get { return _offenseParagonPriority; }
            set { SetField(ref _offenseParagonPriority, value); }
        }

        [DataMember(IsRequired = false)]
        public ParagonCollection DefenseParagonPriority
        {
            get { return _defenseParagonPriority; }
            set { SetField(ref _defenseParagonPriority, value); }
        }

        [DataMember(IsRequired = false)]
        public ParagonCollection UtilityParagonPriority
        {
            get { return _utilityParagonPriority; }
            set { SetField(ref _utilityParagonPriority, value); }
        }

        public void OnSave()
        {
            Logger.LogVerbose("Saving Paragon Priority");
        }

        public void OnLoaded()
        {
            Logger.LogVerbose("Loading Paragon Priority");

            if (!CoreParagonPriority.Any() || !UtilityParagonPriority.Any() || !DefenseParagonPriority.Any() || !OffenseParagonPriority.Any())
            {
                LoadDefaults();
            }
            else
            {
                CoreParagonPriority.ForEach(i => i.Populate());
                OffenseParagonPriority.ForEach(i => i.Populate());
                DefenseParagonPriority.ForEach(i => i.Populate());
                UtilityParagonPriority.ForEach(i => i.Populate());
            }
        }

        public override void LoadDefaults()
        {
            CoreParagonPriority = new ParagonCollection(ParagonCategory.PrimaryAttributes)
            {
                new ParagonItem(TrinityParagonBonusType.PrimaryStat),
                new ParagonItem(TrinityParagonBonusType.Vitality),
                new ParagonItem(TrinityParagonBonusType.Resource),
                new ParagonItem(TrinityParagonBonusType.MovementSpeed),
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

        public void Reset()
        {
            TrinitySetting.Reset(this);
            LoadDefaults();
        }

        public void CopyTo(ParagonSetting setting)
        {
            TrinitySetting.CopyTo(this, setting);
        }

        public ParagonSetting Clone()
        {
            return TrinitySetting.Clone(this);
        }

        /// <summary>
        /// This will set default values for new settings if they were not present in the serialized XML (otherwise they will be the type defaults)
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            foreach (var p in GetType().GetProperties())
            {
                foreach (var dv in p.GetCustomAttributes(true).OfType<DefaultValueAttribute>())
                {
                    p.SetValue(this, dv.Value);
                }
            }
        }

    }

}