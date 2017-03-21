//using System;
//using Trinity.Framework;
//using System.Diagnostics;
//using Trinity.Framework.Reference;
//using Zeta.Bot;
//using Zeta.Bot.Navigation;
//using Zeta.Bot.Profile;
//using Zeta.Game;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;

//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    // TrinityTownRun forces a town-run request
//    [XmlElement("TrinityTownPortal")]
//    [XmlElement("TownPortal")]
//    public class TownPortalTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public static int DefaultWaitTime = -1;

//        [XmlAttribute("waitTime")]
//        [XmlAttribute("wait")]
//        public int WaitTime { get; set; }

//        /// <summary>
//        /// This is the longest time this behavior can run for. Default is 120 seconds.
//        /// </summary>
//        [XmlAttribute("timeout")]
//        public int Timeout { get; set; }

//        public static Stopwatch AreaClearTimer = null;
//        public static Stopwatch PortalCastTimer = null;
//        public static bool ForceClearArea = false;
//        private DateTime _startTime = DateTime.MaxValue;
//        private double _startHealth = -1;

//        private bool _isDone;

//        public override bool IsDone
//        {
//            get
//            {
//                var done = _isDone || !IsActiveQuestStep;

//                CheckTimeout();

//                return done;
//            }
//        }

//        public void CheckTimeout()
//        {
//            if (DateTime.UtcNow.Subtract(_startTime).TotalSeconds <= Timeout)
//                return;

//            Core.Logger.Log("timed out ({0} seconds)", Timeout);
//            _isDone = true;
//        }

//        public TownPortalTag()
//        {
//            AreaClearTimer = new Stopwatch();
//            PortalCastTimer = new Stopwatch();
//            QuestId = QuestId <= 0 ? 1 : QuestId;
//            Timeout = Timeout <= 0 ? 120 : Timeout;   
//        }

//        public override void OnStart()
//        {
//            if (ZetaDia.IsInTown)
//            {
//                _isDone = true;
//                return;
//            }         
//            ForceClearArea = true;
//            AreaClearTimer.Reset();
//            AreaClearTimer.Start();
//            DefaultWaitTime = 2500;
//            if (WaitTime <= 0)
//            {
//                WaitTime = DefaultWaitTime;
//            }
//            _startHealth = ZetaDia.Me.HitpointsCurrent;
//            _startTime = DateTime.UtcNow;
//            Core.Logger.Log("TownPortal started - clearing area, waitTime={0}, startHealth={1:0}", WaitTime, _startHealth);
//        }

//        protected override Composite CreateBehavior()
//        {
//            return new
//            PrioritySelector(
//                new Decorator(ret => ZetaDia.Globals.IsLoadingWorld,
//                    new Action()
//                ),
//                new Decorator(ret => ZetaDia.IsInTown && !GameData.ForceTownPortalLevelAreaIds.Contains(Core.Player.LevelAreaId),
//                    new Action(ret =>
//                    {
//                        ForceClearArea = false;
//                        AreaClearTimer.Reset();
//                        _isDone = true;
//                    })
//                ),
//                new Decorator(ret => !ZetaDia.IsInTown && !ZetaDia.Me.CanUseTownPortal(),
//                    new Action(ret =>
//                    {
//                        ForceClearArea = false;
//                        AreaClearTimer.Reset();
//                        _isDone = true;
//                    })
//                ),
//                new Decorator(ret => ZetaDia.Me.HitpointsCurrent < _startHealth,
//                    new Action(ret =>
//                    {
//                        _startHealth = ZetaDia.Me.HitpointsCurrent;
//                        AreaClearTimer.Restart();
//                        ForceClearArea = true;
//                    })
//                ),
//                new Decorator(ret => AreaClearTimer.IsRunning,
//                    new PrioritySelector(
//                        new Decorator(ret => AreaClearTimer.ElapsedMilliseconds <= WaitTime,
//                            new Action(ret => ForceClearArea = true) // returns RunStatus.Success
//                        ),
//                        new Decorator(ret => AreaClearTimer.ElapsedMilliseconds > WaitTime,
//                            new Action(ret =>
//                            {
//                                Core.Logger.Log("Town Portal timer finished");
//                                ForceClearArea = false;
//                                AreaClearTimer.Reset();
//                            })
//                        )
//                    )
//                ),

//                new Decorator(ret => !ForceClearArea,
//                    new PrioritySelector(

//                        new Decorator(ret => ZetaDia.Me.Movement.IsMoving,
//                            new Sequence(
//                                CommonBehaviors.MoveStop(),
//                                new Sleep(1000)
//                            )
//                        ),

//                        new Decorator(ret => PortalCastTimer.IsRunning && PortalCastTimer.ElapsedMilliseconds >= 7000,
//                            new Sequence(
//                                new ActionRunCoroutine(async ret =>
//                                {
//                                    Core.Logger.Log("Stuck casting town portal, moving a little");
//                                    await Navigator.StuckHandler.DoUnstick();
//                                    //Navigator.MoveTo(Navigator.StuckHandler.GetUnstuckPos());
//                                    PortalCastTimer.Reset();
//                                })
//                            )
//                        ),


//                        new Decorator(ret => PortalCastTimer.IsRunning && ZetaDia.Me.LoopingAnimationEndTime > 0, // Already casting, just wait
//                            new Action(ret => RunStatus.Success)
//                        ),

//                        new Sequence(
//                            new Action(ret =>
//                            {
//                                PortalCastTimer.Restart();
//                                //GameEvents.FireWorldTransferStart();
//                                ZetaDia.Me.UseTownPortal();
//                            }),

//                            new WaitContinue(3, ret => ZetaDia.Me != null && ZetaDia.Me.LoopingAnimationEndTime > 0,
//                                new Sleep(100)
//                            )
//                        )
//                    )
//                )
//            );
//        }

//        public override void ResetCachedDone()
//        {
//            _isDone = false;
//            _startTime = DateTime.MaxValue;
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

