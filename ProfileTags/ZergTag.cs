using System.ComponentModel;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{

    [XmlElement("Zerg")]
    public class ZergTag : BaseContainerProfileBehavior
    {
        //[XmlAttribute("force")]
        //[Description("Make sure zerg mode is on every tick")]
        //[DefaultValue(false)]
        //public bool Force { get; set; }

        public override bool StartMethod()
        {
            Combat.CombatMode = CombatMode.SafeZerg;
            return false;
        }

        //public override bool MainMethod()
        //{
        //    if (Force && Combat.CombatMode != CombatMode.SafeZerg)
        //        Combat.CombatMode = CombatMode.SafeZerg;

        //    return false;
        //}

        public override void OnDone()
        {
            Combat.CombatMode = CombatMode.Normal;
            base.OnDone();
        }
    }

}