using System.ComponentModel;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.ProfileTags
{
    [XmlElement("MoveToPosition")]
    public class MoveToPositionTag : TrinityProfileBehavior
    {
        #region Position

        [XmlAttribute("x")]
        [DefaultValue(0)]
        public float X { get; set; }

        [XmlAttribute("y")]
        [DefaultValue(0)]
        public float Y { get; set; }

        [XmlAttribute("z")]
        [DefaultValue(0)]
        public float Z { get; set; }

        #endregion

        #region ScenePosition

        [XmlAttribute("sceneX")]
        [DefaultValue(0)]
        public float RelativeSceneX { get; set; }

        [XmlAttribute("sceneY")]
        [DefaultValue(0)]
        public float RelativeSceneY { get; set; }

        [XmlAttribute("sceneZ")]
        [DefaultValue(0)]
        public float RelativeSceneZ { get; set; }

        [XmlAttribute("sceneSnoId")]
        [DefaultValue(0)]
        public int SceneSnoId { get; set; }

        [XmlAttribute("sceneName")]
        [DefaultValue("")]
        public string SceneName { get; set; }

        #endregion

        private bool _isDone;
        public override bool IsDone => _isDone;

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => Routine());
        }

        private ISubroutine _task;

        public async Task<bool> Routine()
        {
            if (_task == null)
            {
                var targetAbsolutePosition = new Vector3(X, Y, Z);
                var targetRelativePosition = new Vector3(RelativeSceneX, RelativeSceneY, RelativeSceneZ);

                if (ZetaDia.WorldInfo.IsGenerated && targetAbsolutePosition != Vector3.Zero && targetRelativePosition == Vector3.Zero)
                {
                    Logger.LogError("[MoveToPosition] The current world is auto-generatd, you need to use sceneX,sceneY,sceneZ + either 'sceneName' or 'sceneSnoId' attributes");
                    _isDone = true;
                    return true;
                }

                if (!ZetaDia.WorldInfo.IsGenerated && targetAbsolutePosition != Vector3.Zero)
                {
                    _task = new MoveToPositionCoroutine(AdvDia.CurrentWorldId, targetAbsolutePosition, 3);
                }
                else if (targetRelativePosition != Vector3.Zero)
                {
                    if (SceneSnoId > 0)
                    {
                        _task = new MoveToScenePositionCoroutine(SceneSnoId, targetRelativePosition);
                    }
                    else if (!string.IsNullOrEmpty(SceneName))
                    {
                        _task = new MoveToScenePositionCoroutine(SceneName, targetRelativePosition);
                    }
                    else
                    {
                        Logger.LogError("[MoveToPosition] A sceneName or sceneSnoId, and a relative position (sceneX,sceneY,sceneZ) are required for dynamically generated worlds");
                    }
                }
                else
                {
                    Logger.LogError("[MoveToPosition] No valid coodinates were specified");
                }
            }

            if (_task != null && !await _task.GetCoroutine())
                return true;

            _isDone = true;
            return true;
        }

        public override void ResetCachedDone(bool force = false)
        {
            _task?.Reset();
            _isDone = false;
        }

    }
}

