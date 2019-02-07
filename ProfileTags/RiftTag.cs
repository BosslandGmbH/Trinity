using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.RiftCoroutines;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.QuestTools;
using Zeta.Game.Internals;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("NephalemRift")]
    public class NephalemRiftTag : RiftProfileBehavior
    {
        public NephalemRiftTag() { SelectedRiftType = RiftType.Nephalem; }
    }

    [XmlElement("GreaterRift")]
    public class GreaterRiftTag : RiftProfileBehavior
    {
        public GreaterRiftTag() { SelectedRiftType = RiftType.Greater; }
    }

    [XmlElement("Rift")]
    public class RiftTag : RiftProfileBehavior { }

    public class RiftProfileBehavior : BaseProfileBehavior
    {
        #region XmlAttributes

        [XmlAttribute("type")]
        [XmlAttribute("riftType")]
        [Description("whether greater or nephalem rifts should be opened")]
        public RiftType SelectedRiftType { get; set; }
        // Greater, Nephalem,

        [XmlAttribute("level")]
        [Description("level of greater rifts")]
        public virtual int Level { get; set; }

        [XmlAttribute("empowered")]
        [Description("If rift should be empowered when opened")]
        public bool IsEmpowered { get; set; }

        [XmlAttribute("getXPShrine")]
        [Description("If nephalem rifts should be run until bonux XP shrine is found")]
        public bool IsGetXPShrine { get; set; }

        [XmlAttribute("count")]
        [XmlAttribute("riftCount")]
        [Description("The number of rifts to complete before tag finishes")]
        public int RiftCount { get; set; }

        #endregion

        private int _remainingRuns;

        public override async Task<bool> StartTask()
        {
            if (Level == 0)
            {
                Level = SelectedRiftType == RiftType.Greater ?
                    RiftData.GetGreaterRiftLevel() :
                    -1;
            }

            _remainingRuns = Math.Max(RiftCount, PluginSettings.Current.RiftCount);
            _remainingRuns = _remainingRuns == 0 ? -1 : _remainingRuns;
            return false;
        }

        public override async Task<bool> MainTask()
        {
            if (!await RiftCoroutine.RunRift(SelectedRiftType,
                Level,
                PluginSettings.Current.EmpoweredRiftLevelLimit,
                IsEmpowered || PluginSettings.Current.UseEmpoweredRifts,
                IsGetXPShrine || PluginSettings.Current.NormalRiftForXPShrine))
            {
                return false;
            }

            // When _remainingRuns has a negative start value we just keep going.
            if (_remainingRuns < 0)
                return false;

            // We just completed a rift. Let's decrement remaining runs. 
            Interlocked.Decrement(ref _remainingRuns);
            // When remaining runs equals 0 we are done.
            return _remainingRuns == 0;
        }
    }
}
