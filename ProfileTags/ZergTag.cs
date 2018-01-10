using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Components.QuestTools;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{

    [XmlElement("Zerg")]
    public class ZergTag : BaseContainerProfileBehavior
    {
        // <Zerg>
        //    <SomeTag>
        // </Zerg>

        public override bool StartMethod()
        {
            TrinityCombat.CombatMode = CombatMode.SafeZerg;
            return false;
        }

        public override bool MainMethod()
        {
            TrinityCombat.CombatMode = CombatMode.SafeZerg;
            return false;
        }

        public override void DoneMethod()
        {
            TrinityCombat.CombatMode = CombatMode.Normal;
        }
    }

}