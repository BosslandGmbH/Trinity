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
using Zeta.Bot;
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
            LoadDefaults();
            QuestId = QuestId <= 0 ? 1 : QuestId;
            TagClassName = GetType().Name;
        }

        public void LoadDefaults()
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
            {
                var myAttribute = (DefaultValueAttribute)property.Attributes[typeof(DefaultValueAttribute)];
                if (myAttribute != null)
                {
                    property.SetValue(this, myAttribute.Value);
                }
            }
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

        public virtual void OnPulse() { }


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
                try
                {
                    if (_isDone)
                        return true;

                    // Containers are never really started or run, 
                    // but IsDone is called when execution is outside the container 
                    // and every tick while a child tag is being executed. 

                    if (!Core.TrinityIsReady)
                    {
                        Core.Logger.Verbose("Waiting for Trinity to become ready");
                        return false;
                    }

                    if (!IsStarted)
                    {
                        var currentTag = ProfileManager.CurrentProfileBehavior;
                        if (currentTag != this && Body.All(t => currentTag != t))
                            return false;

                        StartTime = DateTime.UtcNow;
                        IsStarted = true;

                        OnStart();

                        if (StartMethod())
                            Done();

                        return false;
                    }

                    if (!IsActiveQuest || StepId != 0 && !IsActiveQuestStep)
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
                catch (Exception ex)
                {
                    Core.Logger.Debug($"Exception in {TagClassName}: {ex}");
                }
                Done();
                return true;
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
                Core.Logger.Log($"[{TagClassName}] 满足停止条件: ({StopCondition})");
                return true;
            }
            return false;
        }

        protected sealed override Composite CreateBehavior() => new Zeta.TreeSharp.Action();
        public sealed override void OnStart()
        {
            OnStartFired = true;
            Core.Logger.Verbose($"开始标签: {TagClassName}. {ToString()}");
        }

        public bool OnStartFired { get; set; }

        public sealed override void OnDone()
        {
            if (IsStarted)
            {
                DoneMethod();
            }
        }

        public sealed override void ResetCachedDone() => Reset();
        public sealed override void ResetCachedDone(bool force = false) => Reset();

        public void Reset()
        {
            try
            {
                _isDone = false;
                IsStarted = false;
                OnStartFired = false;
                this.ResetChildren();
                StartTime = DateTime.MinValue;
                EndTime = DateTime.MinValue;
            }
            catch (Exception ex)
            {
                Core.Logger.Log($"[{TagClassName}] Exception resetting profile tag {ex}");
            }

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

            EndTime = DateTime.UtcNow;
            DoneMethod();
            this.SetChildrenDone();

            if (IsStarted)
            {
                Core.Logger.Verbose($"Finished Tag: {TagClassName} in {EndTime.Subtract(StartTime).TotalSeconds:N2} seconds");
            }

            IsStarted = false;
        }

        #endregion IEnhancedProfileBehavior
    }
}