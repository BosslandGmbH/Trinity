
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using Org.BouncyCastle.Asn1.Esf;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects.Attributes;
using Trinity.Helpers;
using Trinity.UIComponents;
using Zeta.Bot.Profile.Common;
using Zeta.Game;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance.Handlers
{
    [DataContract(Namespace = "")]
    public class FireChainsAvoidanceHandler : NotifyBase, IAvoidanceHandler
    {
        private int _healthThresholdPct;
        private float _distanceMultiplier;

        public bool IsAllowed
        {
            get { return Core.Player.CurrentHealthPct <= HealthThresholdPct; }
        }

        public void UpdateNodes(AvoidanceGrid grid, Structures.Avoidance avoidance)
        {  
            var actor = avoidance.Actors.FirstOrDefault();
            if (actor == null)
                return;

            foreach (var otherAvoidance in Core.Avoidance.CurrentAvoidances)
            {
                if (otherAvoidance == avoidance)
                    continue;

                var fireChainFriend = otherAvoidance.Actors.FirstOrDefault(a => a.ActorSnoId == actor.ActorSnoId);
                if (fireChainFriend != null)
                {
                    var nodes = grid.GetRayLineAsNodes(actor.Position, fireChainFriend.Position).SelectMany(n => n.AdjacentNodes);
                    grid.FlagNodes(nodes, AvoidanceFlags.Avoidance, 10);
                }
            }           
        }

        [DataMember]
        [Setting, DefaultValue(90)]
        [UIControl(UIControlType.Slider), Limit(1, 100)]       
        [Description("Player health must be below value for this to be avoided (Default=90)")]
        public int HealthThresholdPct
        {
            get { return _healthThresholdPct; }
            set { SetField(ref _healthThresholdPct, value); }
        }

        [DataMember]
        [Setting, DefaultValue(1)]
        [UIControl(UIControlType.Slider), Limit(0, 2)]
        [Description("The safe distance from this avoidance is multiplied by value (Default=1)")]
        public float DistanceMultiplier
        {
            get { return _distanceMultiplier; }
            set { SetField(ref _distanceMultiplier, value); }
        }

    }
}






