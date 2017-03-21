using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Trinity;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.Components.QuestTools
{
    public abstract class BaseProfileBehavior : ProfileBehavior, IEnhancedProfileBehavior
    {
        protected BaseProfileBehavior()
        {
            QuestId = QuestId <= 0 ? 1 : QuestId;
            TagClassName = GetType().Name;
        }

        public string TagClassName { get; set; }

        [XmlAttribute("timeout")]
        [Description("Time in seconds after which the tag will end")]
        public int Timeout { get; set; }

        [XmlAttribute("stopCondition")]
        [Description("When the tag should be stopped")]
        public string StopCondition { get; set; }

        /// <summary>
        /// Task run every tick. Return true to end the tag, false to continue.
        /// </summary>
        public virtual async Task<bool> MainTask() => false;

        /// <summary>
        /// Task run once when tag starts. Return true to end the tag, false to continue.
        /// </summary>
        public virtual async Task<bool> StartTask() => false;

        public bool IsStarted { get; private set; }
        public DateTime StartTime { get; private set; } = DateTime.MinValue;
        public DateTime EndTime { get; private set; } = DateTime.MinValue;

        private async Task<bool> Run()
        {
            if (_isDone)
                return true;

            if (!IsStarted)
            {
                Core.Logger.Verbose(LogCategory.CrashDebug, "BaseProfileBehavior.StartTask " + GetType().Name);

                StartTime = DateTime.UtcNow;
                IsStarted = true;

                if (await StartTask())
                {
                    Done();
                    return true;
                }      
            }

            if (CheckTimeout())
                Done();

            else if (CheckStopCondition())
                Done();

            else if (await MainTask())
                Done();

            return true;
        }

        private bool CheckTimeout()
        {
            return Timeout > 0 && DateTime.UtcNow.Subtract(StartTime).TotalSeconds > Timeout;
        }

        private bool CheckStopCondition()
        {
            if (!string.IsNullOrEmpty(StopCondition) && ScriptManager.GetCondition(StopCondition).Invoke())
            {
                Core.Logger.Log($"[{TagClassName}] Stop condition was met: ({StopCondition})");
                return true;
            }
            return false;
        }

        private bool _isDone;

        protected sealed override Composite CreateBehavior() => new ActionRunCoroutine(ctx => Run());
        public sealed override void OnStart() => Core.Logger.Verbose($"Started Tag: {TagClassName}. {ToString()}");
        public sealed override void OnDone() => Core.Logger.Verbose($"Finished Tag: {TagClassName} in {EndTime.Subtract(StartTime).TotalSeconds:N2} seconds");
        public sealed override bool IsDone => _isDone;
        public sealed override void ResetCachedDone() => Reset();
        public sealed override void ResetCachedDone(bool force = false) => Reset();

        public void Reset()
        {
            _isDone = false;
            IsStarted = false;
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
        }

        #region IEnhancedProfileBehavior

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            Reset();
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
            IsStarted = false;
            EndTime = DateTime.UtcNow;
        }

        #endregion   
    }
}
