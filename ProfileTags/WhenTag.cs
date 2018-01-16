using System.ComponentModel;
using Trinity.Components.QuestTools;
using Zeta.Common;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("When")]
    public class WhenTag : WhenTagProfileBehavior { }

    public class WhenTagProfileBehavior : BaseContainerProfileBehavior
    {
        #region XmlAttributes

        [XmlAttribute("startCondition")]
        [XmlAttribute("condition")]
        [Description("When to trigger child tags to run")]
        public string Condition { get; set; }

        [XmlAttribute("name")]
        [Description("A unique name for the tag")]
        public string Name { get; set; }

        #endregion

        public override bool StartMethod()
        {
            BotBehaviorQueue.Queue(new QueueItem
            {
                Condition = ret => ScriptManager.GetCondition(Condition).Invoke(),
                Name = Name ?? Condition,
                Nodes = Body,
            });

            return true;
        }

    }
}
