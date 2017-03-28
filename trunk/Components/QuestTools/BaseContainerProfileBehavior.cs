using System;
using System.ComponentModel;
using System.Linq;
using Trinity;
using Trinity.Components.QuestTools.Helpers;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Bot.Profile.Composites;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.Components.QuestTools
{
    public class BaseContainerProfileBehavior : ComplexNodeTag, IEnhancedProfileBehavior
    {
        protected BaseContainerProfileBehavior()
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

        public bool IsStarted { get; private set; }
        public DateTime StartTime { get; private set; } = DateTime.MinValue;
        public DateTime EndTime { get; private set; } = DateTime.MinValue;

        /// <summary>
        /// Method run every tick of self and children. Return true to end the tag, false to continue.
        /// </summary>
        public virtual bool MainMethod() => false;

        /// <summary>
        /// Method run once when tag starts. Return true to end the tag, false to continue.
        /// </summary>
        public virtual bool StartMethod() => false;

        /// <summary>
        /// Method run once tag and children are finished.
        /// </summary>
        public virtual void DoneMethod() { }


        private bool _isDone;

        public override bool IsDone
        {
            get
            {
                if (_isDone)
                    return true;

                if (!IsStarted)
                {
                    StartTime = DateTime.UtcNow;
                    IsStarted = true;
                    if (StartMethod())
                        Done();
                }

                if (!IsActiveQuestStep)
                    Done();

                else if (CheckTimeout())
                    Done();

                else if (CheckStopCondition())
                    Done();

                else if (MainMethod())
                    Done();

                else if (Body.All(p => p.IsDone))
                    Done();

                return _isDone;
            }
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

        protected sealed override Composite CreateBehavior() => null;
        public sealed override void OnStart() => Core.Logger.Verbose($"Started Tag: {TagClassName}. {ToString()}");
        public sealed override void OnDone() => Core.Logger.Verbose($"Finished Tag: {TagClassName} in {EndTime.Subtract(StartTime).TotalSeconds:N2} seconds");
        public sealed override void ResetCachedDone() => Reset();
        public sealed override void ResetCachedDone(bool force = false) => Reset();

        public void Reset()
        {
            _isDone = false;
            IsStarted = false;
            this.ResetChildren();
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
        }

        #region IEnhancedProfileBehavior

        public void Update()
        {
            // Behavior is not created or updated for ProfileBehaviors containing other ProfileBehaviors.
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
            DoneMethod();
            this.SetChildrenDone();
            OnDone();
        }

        #endregion IEnhancedProfileBehavior
    }
}