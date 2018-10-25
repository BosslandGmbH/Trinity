using System.ComponentModel;
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

        public override async Task<bool> StartTask()
        {
            if (Level == 0) Level = SelectedRiftType == RiftType.Greater ? RiftData.GetGreaterRiftLevel() : -1;
            return false;
        }

        public override async Task<bool> MainTask()
        {
            if (!await RiftCoroutine.RunRift(SelectedRiftType, Level, PluginSettings.Current.EmpoweredRiftLevelLimit, IsEmpowered || PluginSettings.Current.UseEmpoweredRifts, IsGetXPShrine || PluginSettings.Current.NormalRiftForXPShrine))
                return false;

            return true;
        }

    }
}
