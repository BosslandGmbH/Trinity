using System.Collections.Generic;
using System.Linq;
using Zeta.Bot.Profile;
using Zeta.Bot.Profile.Common;
using Zeta.Bot.Profile.Composites;
using Zeta.Common;
using Zeta.TreeSharp;
using Zeta.XmlEngine;
using Action = Zeta.TreeSharp.Action;



namespace Trinity.Components.QuestTools
{
    public static class CompositeHelpers
    {
        public delegate bool BoolDelegate(object ret);
        public delegate void VoidDelegate(object ret);
        public delegate object ObjectDelegate(object ret);
        public delegate Composite CompositeDelegate(object ret);
        public delegate Composite LinkCompositeDelegate();
        public delegate Composite CreateBehavior(object ret);

        public static Composite ExecuteReturnAlwaysSuccess(BoolDelegate condition, CreateBehavior behavior)
        {
            return
            new DecoratorContinue(ret => condition.Invoke(null),
                new PrioritySelector(
                    behavior.Invoke(null),
                    new Zeta.TreeSharp.Action(ret => RunStatus.Success)
                )
            );
        }
    }

    public class EnhancedLeaveGameTag : LeaveGameTag, IEnhancedProfileBehavior
    {
        public EnhancedLeaveGameTag()
        {
            QuestId = QuestId <= 0 ? 1 : QuestId;
        }

        private bool _isDone;
        public override bool IsDone => !IsActiveQuestStep || _isDone || base.IsDone;

        #region IEnhancedProfileBehavior

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
        }

        #endregion     
    }

    public class EnhancedLoadProfileTag : LoadProfileTag, IEnhancedProfileBehavior
    {
        public EnhancedLoadProfileTag()
        {
            QuestId = QuestId <= 0 ? 1 : QuestId;
        }

        private bool _isDone;
        public override bool IsDone => !IsActiveQuestStep || _isDone || base.IsDone;

        #region IEnhancedProfileBehavior

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
        }

        #endregion
    }

    public class EnhancedLogMessageTag : LogMessageTag, IEnhancedProfileBehavior
    {
        private bool _isDone;
        public override bool IsDone => _isDone || base.IsDone;

        #region IEnhancedProfileBehavior

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
        }

        #endregion
    }

    public class EnhancedUseWaypointTag : UseWaypointTag, IEnhancedProfileBehavior
    {
        private bool _isDone;
        public override bool IsDone => _isDone || base.IsDone;

        #region IEnhancedProfileBehavior

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
        }

        #endregion
    }

    public class EnhancedWaitTimerTag : WaitTimerTag, IEnhancedProfileBehavior
    {
        public EnhancedWaitTimerTag()
        {
            QuestId = QuestId <= 0 ? 1 : QuestId;
        }

        private bool _isDone;
        public override bool IsDone => !IsActiveQuestStep || _isDone || base.IsDone;

        #region IEnhancedProfileBehavior

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
        }

        #endregion
    }

    public class EnhancedUseObjectTag : UseObjectTag, IEnhancedProfileBehavior
    {
        public EnhancedUseObjectTag()
        {
            QuestId = QuestId <= 0 ? 1 : QuestId;
        }

        private bool _isDone;
        public override bool IsDone => _isDone || base.IsDone;

        #region IEnhancedProfileBehavior

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
        }

        #endregion
    }

    public class EnhancedUsePowerTag : UsePowerTag, IEnhancedProfileBehavior
    {
        public EnhancedUsePowerTag()
        {
            QuestId = QuestId <= 0 ? 1 : QuestId;
        }

        private bool _isDone;
        public override bool IsDone => _isDone || base.IsDone;

        #region IEnhancedProfileBehavior

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
        }

        #endregion
    }

    public class EnhancedToggleTargetingTag : ToggleTargetingTag, IEnhancedProfileBehavior
    {
        public EnhancedToggleTargetingTag()
        {
            QuestId = QuestId <= 0 ? 1 : QuestId;
        }

        private bool _isDone;
        public override bool IsDone => !IsActiveQuestStep || _isDone || base.IsDone;

        public override void OnStart() { }

        protected override Composite CreateBehavior()
        {
            _isDone = true;
            return CompositeHelpers.ExecuteReturnAlwaysSuccess(
                ret => !_isDone,
                ret => new Action(r => base.OnStart())
            );
        }

        #region IEnhancedProfileBehavior

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
        }

        #endregion
    }

    public class EnhancedWaitWhileTag : WaitWhileTag, IEnhancedProfileBehavior
    {
        private bool _isDone;
        public override bool IsDone => _isDone || base.IsDone;

        #region IEnhancedProfileBehavior

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
        }

        #endregion
    }

    [XmlElement("EnhancedIf")]
    public class EnhancedIfTag : IfTag, IEnhancedProfileBehavior
    {
        private bool _isDone;
        public CompositeHelpers.BoolDelegate IsDoneDelegate;

        public EnhancedIfTag(CompositeHelpers.BoolDelegate isDoneDelegate = null, params ProfileBehavior[] children)
        {
            IsDoneDelegate = isDoneDelegate;
            if(children!=null && children.Any())
                Body = children.ToList();
        }

        public override bool IsDone
        {
            get
            {
                if (IsDoneDelegate != null)
                    Conditional = ScriptManager.GetCondition(IsDoneDelegate.Invoke(null) ? "True" : "False");                

                return _isDone || base.IsDone;
            }
        }

        public override void ResetCachedDone()
        {
            _isDone = false;
            base.ResetCachedDone(true);
        }

        #region IEnhancedProfileBehavior : INodeContainer

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
            this.SetChildrenDone();
        }

        #endregion

        public List<ProfileBehavior> Children
        {
            get => Body;
            set => Body = value;
        }

    }

    public class EnhancedWhileTag : WhileTag, IEnhancedProfileBehavior
    {
        private bool _isDone;
        public CompositeHelpers.BoolDelegate IsDoneDelegate;

        public EnhancedWhileTag(CompositeHelpers.BoolDelegate isDoneDelegate = null, params ProfileBehavior[] children)
        {
            IsDoneDelegate = isDoneDelegate;
            if(children!=null && children.Any())
                Body = children.ToList();
        }

        public override bool IsDone
        {
            get
            {                
                if (IsDoneDelegate != null)
                    Conditional = ScriptManager.GetCondition(IsDoneDelegate.Invoke(null) ? "True" : "False");                     

                return _isDone || base.IsDone;
            }
        }

        public new bool GetConditionExec()
        {
            return IsDoneDelegate != null && IsDoneDelegate.Invoke(null) || ScriptManager.GetCondition(Condition).Invoke();
        }

        public override void ResetCachedDone()
        {
            _isDone = false;
            base.ResetCachedDone();
        }

        #region IEnhancedProfileBehavior : INodeContainer

        public void Update()
        {
            UpdateBehavior();
        }

        public void Start()
        {
            OnStart();
        }

        public void Done()
        {
            _isDone = true;
            this.SetChildrenDone();
        }

        #endregion

        public List<ProfileBehavior> Children
        {
            get => Body;
            set => Body = value;
        }

    }

}

