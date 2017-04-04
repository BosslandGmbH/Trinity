using System;
using Trinity.Framework;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Zeta.Common;
using Zeta.Game;
using Zeta.XmlEngine;


namespace Trinity.ProfileTags
{    
    [XmlElement("TrinityInteract")]
    [XmlElement("Interact")]
    public class InteractTag : InteractProfileBehavior { }

    public class InteractProfileBehavior : MoveToActorProfileBehavior
    {
        private ISubroutine _interactTask;

        #region XmlAttributes
    
        [XmlAttribute("interactAttempts")]
        [DefaultValue(8)]
        [Description("Number of times to interact")]
        public int InteractAttempts { get; set; }

        [XmlAttribute("ignoreSanityChecks")]
        [DefaultValue(true)]
        [Description("Attempt to interact even when the actor doesn't look interactable")]
        public bool IgnoreSanityChecks { get; set; }

        #endregion

        public override async Task<bool> StartTask()
        {
            _interactTask = new InteractionCoroutine(ActorId, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(1), InteractAttempts, IgnoreSanityChecks);

            return await base.StartTask();
        }

        public override async Task<bool> MainTask()
        {
            if (!await base.MainTask())
                return false;

            if (!_interactTask.IsDone && !await _interactTask.GetCoroutine())
                return false;

            return true;
        }

    }
}

