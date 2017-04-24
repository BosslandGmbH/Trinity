using System;
using Trinity.Framework;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Common;
using Zeta.Game;
using Zeta.XmlEngine;


namespace Trinity.ProfileTags
{
    [XmlElement("MoveToActor")]
    public class MoveToActorTag : MoveToActorProfileBehavior { }

    public class MoveToActorProfileBehavior : MoveToPositionProfileBehavior
    {
        private TrinityActor _actor;
        private string _startAnimLower;
        private string _endAnimLower;

        #region XmlAttributes

        [XmlAttribute("actorId")]
        [DefaultValue(0)]
        [Description("id of actor to be found")]
        public int ActorId { get; set; }

        [XmlAttribute("internalName")]
        [Description("The full or partial SNO name / internal name of actor to be found")]
        public string ActorInternalName { get; set; }

        [XmlAttribute("portalMarkerHash")]
        [XmlAttribute("markerNameHash")]
        [XmlAttribute("exitNameHash")]
        [XmlAttribute("markerHash")]
        [DefaultValue(0)]
        [Description("Find actor nearest to a marker hash")]
        public int MarkerHash { get; set; }

        [XmlAttribute("explore")]
        [Description("If actor should be searched for when not found")]
        [DefaultValue(false)]
        public bool Explore { get; set; }

        [XmlAttribute("maxRange")]
        [Description("Maximum distance actor can be found from player current position")]
        [DefaultValue(300f)]
        public float MaxRange { get; set; }

        [XmlAttribute("endAnimation")]
        [Description("The end/used animation name of the actor to find")]
        public string EndAnimation { get; set; }

        [XmlAttribute("startAnimation")]
        [Description("The start/unused animation name of the actor to find")]
        public string StartAnimation { get; set; }

        #endregion

        public override async Task<bool> StartTask()
        {
            var actorFound = FindActor();
            var positionResult = await base.StartTask();

            // Prioritize provided Abs/Rel position before trying to move to found actor, 
            // ensures specific actor is used when there are multiple of same type.

            if (positionResult)
            {
                if (!actorFound)
                    return true;
                
                _movementTask = new MoveToActorCoroutine(QuestId, AdvDia.CurrentWorldId, ActorId, (int)MaxRange, Explore, CheckActorAnimation, StopDistance);
            }

            _endAnimLower = EndAnimation?.ToLowerInvariant() ?? string.Empty;
            _startAnimLower = StartAnimation?.ToLowerInvariant() ?? string.Empty;

            return false;
        }

        public bool FindActor()
        {
            TrinityActor actor = null;
            if (ActorId != 0)
            {
                actor = BountyHelpers.ScanForActor(ActorId, (int)MaxRange, CheckActorAnimation);
            }
            else if (!string.IsNullOrEmpty(ActorInternalName))
            {
                actor = BountyHelpers.ScanForActor(ActorInternalName, (int)MaxRange, CheckActorAnimation);
            }
            else if (ActorId == 0 && MarkerHash != 0)
            {
                actor = BountyHelpers.GetActorNearMarker(MarkerHash, 10f, CheckActorAnimation);
            }

            if (actor == null)
                return false;

            Core.Logger.Debug($"[{TagClassName}] Actor Found: {actor}");
            ActorId = actor.ActorSnoId;
            StopDistance = actor.Radius;
            return true;
        }

        public bool CheckActorAnimation(TrinityActor a)
        {
            string anim = null;
            if (!string.IsNullOrEmpty(_endAnimLower))
            {
                anim = a.CommonData.CurrentAnimation.ToString().ToLowerInvariant();

                if (anim.Contains(_endAnimLower))
                    return false;
            }
            if (!string.IsNullOrEmpty(_startAnimLower))
            {
                if(string.IsNullOrEmpty(anim))
                    anim = a.CommonData.CurrentAnimation.ToString().ToLowerInvariant();

                if (!anim.Contains(_startAnimLower))
                    return false;
            }
            return true;
        }

        public override async Task<bool> MainTask()
        {
            return await base.MainTask();
        }

    }
}

