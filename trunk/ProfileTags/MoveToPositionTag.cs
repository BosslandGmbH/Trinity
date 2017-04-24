
using System;
using Trinity.Framework;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.QuestTools;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;


namespace Trinity.ProfileTags
{
    [XmlElement("TrinityMoveTo")]
    [XmlElement("SafeMoveTo")]
    [XmlElement("MoveToPosition")]
    public class MoveToPositionTag : MoveToPositionProfileBehavior
    {
    }

    public class MoveToPositionProfileBehavior : BaseProfileBehavior
    {
        protected ISubroutine _movementTask;

        #region XmlAttributes

        [XmlAttribute("x")]
        [DefaultValue(0)]
        [Description("X position to be moved to")]
        public float X { get; set; }

        [XmlAttribute("y")]
        [DefaultValue(0)]
        [Description("Y position to be moved to")]
        public float Y { get; set; }

        [XmlAttribute("z")]
        [Description("Z position to be moved to")]
        [DefaultValue(0)]
        public float Z { get; set; }
        
        [XmlAttribute("sceneX")]
        [Description("X position relative to scene")]
        [DefaultValue(0)]
        public float RelativeSceneX { get; set; }

        [XmlAttribute("sceneY")]
        [Description("Y position relative to scene")]
        [DefaultValue(0)]
        public float RelativeSceneY { get; set; }

        [XmlAttribute("sceneZ")]
        [Description("Z position relative to scene")]
        [DefaultValue(0)]
        public float RelativeSceneZ { get; set; }

        [XmlAttribute("sceneSnoId")]
        [Description("Scene id for relative movement")]
        [DefaultValue(0)]
        public int SceneSnoId { get; set; }

        [XmlAttribute("sceneName")]
        [Description("Scene name for relative movement")]
        [DefaultValue("")]
        public string SceneName { get; set; }

        [XmlAttribute("interactRange")]
        [XmlAttribute("distance")]
        [Description("How close to get to the position")]
        [DefaultValue(5)]
        public float StopDistance { get; set; }

        [XmlAttribute("useNavigation")]
        [Description("Move without any navigation - Use with caution only on areas known to have bugged navigation")]
        [DefaultValue(true)]
        public bool UseNavigation { get; set; }

        #endregion

        public Vector3 AbsolutePosition => new Vector3(X, Y, Z);
        public Vector3 RelativePosition => new Vector3(RelativeSceneX, RelativeSceneY, RelativeSceneZ);
        public Vector3 Destination { get; set; }
    
        public override async Task<bool> StartTask()
        {
            if (TrySetAbsoluteDestination(AbsolutePosition))
            {
                return false;
            }
            if (TrySetRelativeDestination(RelativePosition, SceneSnoId, SceneName))
            {
                return false;
            }
            return true;
        }

        public bool TrySetAbsoluteDestination(Vector3 absPos)
        {
            if (absPos != Vector3.Zero && !ZetaDia.WorldInfo.IsGenerated)
            {
                _movementTask = new MoveToPositionCoroutine(AdvDia.CurrentWorldId, absPos, (int)StopDistance, !UseNavigation);
                return true;
            }
            return false;
        }

        public bool TrySetRelativeDestination(Vector3 relPos, int sceneId, string sceneName)
        {
            if (relPos == Vector3.Zero)
                return false;

            if (sceneId > 0)
            {
                _movementTask = new MoveToScenePositionCoroutine(sceneId, relPos, !UseNavigation);
                return true;
            }

            if (string.IsNullOrEmpty(sceneName))
                return false;

            _movementTask = new MoveToScenePositionCoroutine(sceneName, relPos, !UseNavigation);
            return true;
        }

        public override async Task<bool> MainTask()
        {
            if (_movementTask == null)
            {
                Core.Logger.Debug($"_movementTask was null in {TagClassName}");
                return true;
            }

            if (!_movementTask.IsDone && !await _movementTask.GetCoroutine())
                return false;

            return true;
        }

    }
}
