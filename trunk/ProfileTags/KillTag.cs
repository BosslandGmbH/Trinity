using System.ComponentModel;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{

    /// <summary>
    /// Enter a mode whereby the routine, cluster size and target selection settings 
    /// are ignored in order to ensure monsters are always killed.
    /// </summary>
    [XmlElement("Kill")]
    public class KillTag : BaseContainerProfileBehavior
    {
        #region XmlAttributes

        [XmlAttribute("radius")]
        [Description("Distance from player required for monster to be killed")]
        [DefaultValue(100)]
        public float Radius { get; set; }

        #endregion

        public override bool StartMethod()
        {
            Core.Logger.Debug($"{TagClassName} Has Started");
            Radius = Radius > 20 ? Radius : 100;
            return false;
        }

        public override bool MainMethod()
        {
            TrinityCombat.SetKillMode(Radius);
            return false;
        }

        public override void DoneMethod()
        {
            Core.Logger.Debug($"{TagClassName} Has Ended");
            TrinityCombat.ResetCombatMode();
        }
    }

}
