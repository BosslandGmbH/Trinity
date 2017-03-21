//using System; using Trinity.Framework; using Trinity.Framework.Helpers;
//using System.Linq; using Trinity.Framework;
//using System.Threading.Tasks;
//using Trinity.Components.QuestTools.Helpers;
//using Zeta.Bot;
//using Zeta.Bot.Coroutines;
//using Zeta.Bot.Profile;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;

//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    [XmlElement("ResumeUseTownPortal")]
//    public class ResumeUseTownPortalTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public ResumeUseTownPortalTag() { }

//        private bool _isDone = false;
//        public override bool IsDone { get { return _isDone; } }

//        [XmlAttribute("forceUsePortal")]
//        public bool ForceUsePortal { get; set; }

//        [XmlAttribute("timeLimit")]
//        public int TimeLimit { get; set; }

//        public override void OnStart()
//        {
//            if (TimeLimit == 0)
//                TimeLimit = 30;

//            Core.Logger.Log("ResumeUseTownPortal initialized");
//        }

//        private const int TownPortalSNO = 191492;

//        protected override Composite CreateBehavior()
//        {

//            return
//            new PrioritySelector(
//                new Decorator(ret => !ZetaDia.IsInTown,
//                    new Action(ret => _isDone = true)
//                ),
//                new Decorator(ret => DateTime.UtcNow.Subtract(BotEvents.LastJoinedGame).TotalSeconds > TimeLimit && !ForceUsePortal,
//                    new Action(ret => ResumeWindowBreached())
//                ),
//                new Decorator(ret => DateTime.UtcNow.Subtract(BotEvents.LastJoinedGame).TotalSeconds <= TimeLimit || ForceUsePortal,
//                    new Decorator(ret => IsTownPortalNearby,
//                        new Sequence(
//                            new Action(ret => Core.Logger.Log("Taking town portal back")),
//                            new ActionRunCoroutine(ctx => TakeTownPortalBackTask()),
//                            new Sleep(500)
//                        )
//                    )
//                ),
//                new Action(ret => _isDone = true)
//            );
//        }

//        private async Task<bool> TakeTownPortalBackTask()
//        {
//            var portal = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).FirstOrDefault(o => o.ActorSnoId == TownPortalSNO);

//            if (portal == null)
//                return false;

//            if (portal.Distance > 10f)
//                await CommonCoroutines.MoveTo(portal.Position, "Return Portal");

//            if (portal.Distance <= 10f)
//                portal.Interact();

//            return true;

//        }

//        private bool IsTownPortalNearby
//        {
//            get { return ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Any(o => o.ActorSnoId == TownPortalSNO); }
//        }

//        private void ResumeWindowBreached()
//        {
//            Core.Logger.Log("ResumeUseTownPortal resume window breached, tag finished (no action taken)");
//            _isDone = true;
//        }

//        public override void ResetCachedDone()
//        {
//            _isDone = false;
//            base.ResetCachedDone();
//        }

//        #region IEnhancedProfileBehavior

//        public void Update()
//        {
//            UpdateBehavior();
//        }

//        public void Start()
//        {
//            OnStart();
//        }

//        public void Done()
//        {
//            _isDone = true;
//        }

//        #endregion
//    }
//}
