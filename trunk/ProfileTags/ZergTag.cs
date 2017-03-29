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
        #region XmlAttributes

        [XmlAttribute("mode")]
        [Description("Turn zerg on or off when tag starts")]
        public bool? Enabled { get; set; }

        #endregion

        public override bool StartMethod()
        {
            if (Enabled.HasValue)
            {
                TrinityCombat.CombatMode = Enabled.Value ? CombatMode.SafeZerg : CombatMode.SafeZerg;
                return true;
            }
            TrinityCombat.CombatMode = CombatMode.SafeZerg;
            return false;
        }

        public override void DoneMethod()
        {
            TrinityCombat.CombatMode = CombatMode.Normal;
        }
    }

}