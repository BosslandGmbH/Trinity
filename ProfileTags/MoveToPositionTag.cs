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
using Logger = Trinity.Technicals.Logger;

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

        [XmlAttribute("x")]
        [XmlAttribute("sceneX")]
        [DefaultValue(0)]
        public float RelativeSceneX { get; set; }

        [XmlAttribute("y")]
        [XmlAttribute("sceneY")]
        [DefaultValue(0)]
        public float RelativeSceneY { get; set; }

        [XmlAttribute("z")]
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
                if (!ZetaDia.WorldInfo.IsGenerated)
                {
                    _task = new MoveToPositionCoroutine(AdvDia.CurrentWorldId, new Vector3(X, Y, Z), 3);
                }
                else
                {
                    var relativeScenePosition = new Vector3(RelativeSceneX, RelativeSceneY, RelativeSceneZ);
                    if (SceneSnoId > 0)
                    {
                        _task = new MoveToScenePositionCoroutine(SceneSnoId, relativeScenePosition);
                    }
                    else if (!string.IsNullOrEmpty(SceneName))
                    {
                        _task = new MoveToScenePositionCoroutine(SceneName, relativeScenePosition);
                    }
                    else
                    {
                        Logger.LogError("[MoveToPosition] A sceneName or sceneSnoId, and a relative position (sceneX,sceneY,sceneZ) are required for dynamically generated worlds");
                        _isDone = true;
                        return false;
                    }
                }
            }

            if (!await _task.GetCoroutine()) return true;
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

