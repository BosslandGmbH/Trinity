using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Trinity.Framework.Reference;
using System.Threading;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Trinity.Framework.Objects;
using Zeta.Bot.Settings;
using Zeta.Game;
using Trinity.Framework;
using Trinity.Settings;

namespace Trinity.Settings
{
    [DataContract(Namespace = "")]
    public class SenExtendSettings : NotifyBase
    {

        //#region Events
        ///// <summary>
        ///// Occurs when property changed.
        ///// </summary>
        //public event PropertyChangedEventHandler PropertyChanged;
        //#endregion Events

        public SenExtendSettings()
        {
            base.LoadDefaults();
        }

        /// <summary>
        /// 启动自定义经验池经验限制
        /// </summary>
        private bool _enableNephalemRestExperienceCheck;

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool EnableNephalemRestExperienceCheck
        {
            get { return _enableNephalemRestExperienceCheck; }
            set
            {
                if (_enableNephalemRestExperienceCheck != value)
                {
                    _enableNephalemRestExperienceCheck = value;
                    OnPropertyChanged("EnableNephalemRestExperienceCheck");
                }
            }
        }

        /// <summary>
        /// 经验池经验值
        /// </summary>
        private int _miniNormalRiftForXPShrine;

        [DataMember(IsRequired = false)]
        [DefaultValue(50)]
        public int MiniNormalRiftForXPShrine
        {
            get { return _miniNormalRiftForXPShrine; }
            set
            {
                if (_miniNormalRiftForXPShrine != value)
                {
                    _miniNormalRiftForXPShrine = value;
                    OnPropertyChanged("MiniNormalRiftForXPShrine");
                }
            }
        }

        /// <summary>
        /// 是否吃满经验池
        /// </summary>
        private bool _isFullRestExperience;

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool IsFullRestExperience
        {
            get { return _isFullRestExperience; }
            set
            {
                if (_isFullRestExperience != value)
                {
                    _isFullRestExperience = value;
                    OnPropertyChanged("IsFullRestExperience");
                }
            }
        }

        /// <summary>
        /// 吃满经验池数量
        /// </summary>
        private int _restExperienceNum;

        [DataMember(IsRequired = false)]
        [DefaultValue(1)]
        public int RestExperienceNum
        {
            get { return _restExperienceNum; }
            set
            {
                if (_restExperienceNum != value)
                {
                    _restExperienceNum = value;
                    OnPropertyChanged("RestExperienceNum");
                }
            }
        }

        /// <summary>
        /// 开启智能整理包裹
        /// </summary>
        private bool _enableIntelligentFinishing;

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool EnableIntelligentFinishing
        {
            get { return _enableIntelligentFinishing; }
            set
            {
                if (_enableIntelligentFinishing != value)
                {
                    _enableIntelligentFinishing = value;
                    OnPropertyChanged("EnableIntelligentFinishing");
                }
            }
        }

        /// <summary>
        /// 开启智能整理包裹 - 城镇外包裹数量
        /// </summary>
        private int _freeBagSlots;

        [DataMember(IsRequired = false)]
        [DefaultValue(2)]
        public int FreeBagSlots
        {
            get { return _freeBagSlots; }
            set
            {
                if (_freeBagSlots != value)
                {
                    _freeBagSlots = value;
                    OnPropertyChanged("FreeBagSlots");
                }
            }
        }

        /// <summary>
        /// 开启智能整理包裹 - 城镇内包裹数量
        /// </summary>
        private int _freeBagSlotsInTown;

        [DataMember(IsRequired = false)]
        [DefaultValue(30)]
        public int FreeBagSlotsInTown
        {
            get { return _freeBagSlotsInTown; }
            set
            {
                if (_freeBagSlotsInTown != value)
                {
                    _freeBagSlotsInTown = value;
                    OnPropertyChanged("FreeBagSlotsInTown");
                }
            }
        }
        /// <summary>
        /// 小秘是否拾取奥术之尘
        /// </summary>
        private bool _isPickArcaneDust;

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IsPickArcaneDust
        {
            get { return _isPickArcaneDust; }
            set
            {
                if (_isPickArcaneDust != value)
                {
                    _isPickArcaneDust = value;
                    OnPropertyChanged("IsPickArcaneDust");
                }
            }
        }

        /// <summary>
        /// 小秘是否拾取死亡之息
        /// </summary>
        private bool _isPickDeathsBreath;

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IsPickDeathsBreath
        {
            get { return _isPickDeathsBreath; }
            set
            {
                if (_isPickDeathsBreath != value)
                {
                    _isPickDeathsBreath = value;
                    OnPropertyChanged("IsPickDeathsBreath");
                }
            }
        }

        /// <summary>
        /// 小秘是否拾取遗忘之魂
        /// </summary>
        private bool _isPickForgottenSoul;

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IsPickForgottenSoul
        {
            get { return _isPickForgottenSoul; }
            set
            {
                if (_isPickForgottenSoul != value)
                {
                    _isPickForgottenSoul = value;
                    OnPropertyChanged("IsPickForgottenSoul");
                }
            }
        }

        /// <summary>
        /// 小秘是否拾取万用材料
        /// </summary>
        private bool _isPickReusableParts;

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IsPickReusableParts
        {
            get { return _isPickReusableParts; }
            set
            {
                if (_isPickReusableParts != value)
                {
                    _isPickReusableParts = value;
                    OnPropertyChanged("IsPickReusableParts");
                }
            }
        }

        /// <summary>
        /// 小秘是否拾取萦雾水晶
        /// </summary>
        private bool _isPickVeiledCrystal;
        
        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IsPickVeiledCrystal
        {
            get { return _isPickVeiledCrystal; }
            set
            {
                if (_isPickVeiledCrystal != value)
                {
                    _isPickVeiledCrystal = value;
                    OnPropertyChanged("IsPickVeiledCrystal");
                }
            }
        }

        /// <summary>
        /// 小米开箱,大米忽略
        /// </summary>
        private bool _ignoreBoxInGreaterRift;

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IgnoreBoxInGreaterRift
        {
            get { return _ignoreBoxInGreaterRift; }
            set
            {
                if (_ignoreBoxInGreaterRift != value)
                {
                    _ignoreBoxInGreaterRift = value;
                    OnPropertyChanged("IgnoreBoxInGreaterRift");
                }
            }
        }

        /// <summary>
        /// 小秘忽略所有躲避
        /// </summary>
        private bool _ignoreAvoidanceInNephalemRift;

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool IgnoreAvoidanceInNephalemRift
        {
            get { return _ignoreAvoidanceInNephalemRift; }
            set
            {
                if (_ignoreAvoidanceInNephalemRift != value)
                {
                    _ignoreAvoidanceInNephalemRift = value;
                    OnPropertyChanged("IgnoreAvoidanceInNephalemRift");
                }
            }
        }

        /// <summary>
        /// 加强防漏圣塔
        /// </summary>
        private bool _preventSkipShrine;

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PreventSkipShrine
        {
            get { return _preventSkipShrine; }
            set
            {
                if (_preventSkipShrine != value)
                {
                    _preventSkipShrine = value;
                    OnPropertyChanged("IgnoreAvoidanceInNephalemRift");
                }
            }
        }

        /// <summary>
        /// 加强防漏经验池
        /// </summary>
        private bool _preventExpPool;

        [DataMember(IsRequired = false)]
        [DefaultValue(true)]
        public bool PreventExpPool
        {
            get { return _preventExpPool; }
            set
            {
                if (_preventExpPool != value)
                {
                    _preventExpPool = value;
                    OnPropertyChanged("IgnoreAvoidanceInNephalemRift");
                }
            }
        }



    }
}
