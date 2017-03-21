//using System;
//using Trinity.Framework;
//using System.Collections.Generic;
//using System.Linq;
//using Trinity.Components.QuestTools.Helpers;
//using Zeta.Bot.Profile;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals.SNO;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;


//namespace Trinity.Components.QuestTools.ProfileTags
//{
//    /// <summary>
//    /// Move to a random valid location from current position without using navigation or pathfinding.
//    /// Uses basic clicking on location to move. Useful for anti-stuck and areas prone to pathfinding issues.
//    /// </summary>
//    [XmlElement("RandomMove")]
//    public class RandomMoveTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        [XmlAttribute("radius")]
//        public int Radius { get; set; }

//        /// <summary>
//        /// This is the longest time this behavior can run for. Default is 15 seconds.
//        /// </summary>
//        [XmlAttribute("timeout")]
//        public int Timeout { get; set; }

//        private bool _isDone;
//        private List<Vector3> _points = new List<Vector3>();
//        private DateTime _startTime = DateTime.MaxValue;
//        public int Points = 12;

//        public override bool IsDone
//        {
//            get
//            {
//                var done = _isDone || !IsActiveQuestStep;
//                if (!done)
//                    CheckTimeout();
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

//        public RandomMoveTag()
//        {
//            QuestId = QuestId <= 0 ? 1 : QuestId;
//            Radius = Radius < 10 ? 60 : Radius;
//            Timeout = Timeout <= 0 ? 15 : Timeout;
//        }

//        public override void OnStart()
//        {
//            _startTime = DateTime.UtcNow;
//            _points = ProfileUtils.GetCirclePoints(Points, Radius, ZetaDia.Me.Position);
//            ProfileUtils.RandomShuffle(_points);
//            base.OnStart();
//        }

//        private DateTime _lastMoving = DateTime.MinValue;

//        protected override Composite CreateBehavior()
//        {
//            return new Decorator(ret => !_isDone, 

//                new Action(ret =>
//                {                   
//                    if(!_points.Any() || ProfileUtils.IsWithinRange(_points.First()))
//                    {
//                        _isDone = true;
//                        return RunStatus.Failure;
//                    }

//                    if (ZetaDia.Me.Movement.IsMoving)
//                        _lastMoving = DateTime.UtcNow;

//                    var rayResult = ZetaDia.Physics.Raycast(ZetaDia.Me.Position, _points.First(), NavCellFlags.AllowWalk);
//                    var stuckResult = DateTime.UtcNow.Subtract(_lastMoving).TotalMilliseconds > 500;
                    
//                    if (stuckResult || !rayResult)
//                    {
//                        _points.RemoveAt(0);
//                    }                        

//                    if (_points.Any() && _points.First() != Vector3.Zero && ZetaDia.Me.Movement.MoveActor(_points.First()) == 1)
//                        return RunStatus.Success;

//                    _isDone = true;
//                    return RunStatus.Failure;
//                })                
//            );
//        }

//        public override void ResetCachedDone()
//        {
//            _points.Clear();
//            _startTime = DateTime.MaxValue;
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