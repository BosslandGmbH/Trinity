using System.ComponentModel;
using Trinity.Components.QuestTools;
using Trinity.Framework;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{

    [XmlElement("Shuffle")]
    public class ShuffleTag : BaseContainerProfileBehavior
    {
        [XmlAttribute("order")]
        [Description("How child tags should be shuffled")]
        [DefaultValue(default(OrderType))]
        public OrderType Order { get; set; }

        public enum OrderType
        {
            Random = 0,
            Reverse
        }

        public override bool StartMethod()
        {
            Core.Logger.Log("{0} Shuffling {1} tags", Order, Body.Count);

            switch (Order)
            {
                case OrderType.Reverse:
                    Body.Reverse();
                    break;

                default:
                    ProfileUtils.RandomShuffle(Body);
                    break;
            }

            return false;
        }
    }
}