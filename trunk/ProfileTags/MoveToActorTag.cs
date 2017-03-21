using System;
using Trinity.Framework;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Zeta.Common;
using Zeta.Game;
using Zeta.XmlEngine;


namespace Trinity.ProfileTags
{
    [XmlElement("MoveToActor")] 
    public class MoveToActorTag : MoveToActorProfileBehavior { } 

    public class MoveToActorProfileBehavior : MoveToPositionProfileBehavior
    {

        #region XmlAttributes

        [XmlAttribute("actorId")]
        [DefaultValue(0)]
        [Description("id of actor to be found")]
        public int ActorId { get; set; }

        [XmlAttribute("explore")]
        [Description("If actor should be searched for when not found")]
        [DefaultValue(false)]
        public bool Explore { get; set; }

        [XmlAttribute("maxRange")]
        [Description("Maximum distance actor can be found from player current position")]
        [DefaultValue(300f)]
        public float MaxRange { get; set; }

        #endregion

        public override async Task<bool> StartTask()
        {
            if (await base.StartTask())
            {
                _movementTask = new MoveToActorCoroutine(QuestId, AdvDia.CurrentWorldId, ActorId, (int)MaxRange, Explore);
            }
            return false;
        }

        public override async Task<bool> MainTask()
        {
            return await base.MainTask();
        }

    }
}

