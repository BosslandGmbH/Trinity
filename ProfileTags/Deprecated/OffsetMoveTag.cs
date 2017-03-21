//using System;
//using Trinity.Framework;
//using Trinity.DbProvider;
//using Zeta.Bot.Navigation;
//using Zeta.Bot.Pathfinding;
//using Zeta.Bot.Profile;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;
//using Action = Zeta.TreeSharp.Action;


//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    /// <summary>
//    /// This profile tag will move the player a a direction given by the offsets x, y. Examples:
//    ///       <OffsetMove questId="101758" stepId="1" offsetX="-1000" offsetY="1000" /> 
//    ///       <OffsetMove questId="101758" stepId="1" offsetX="1000" offsetY="-1000" />
//    ///       <OffsetMove questId="101758" stepId="1" offsetX="-1000" offsetY="-1000" />
//    ///       <OffsetMove questId="101758" stepId="1" offsetX="1000" offsetY="1000" />
//    /// </summary>
//    [XmlElement("OffsetMove")]
//    [XmlElement("TrinityOffsetMove")]
//    public class OffsetMoveTag : ProfileBehavior, IEnhancedProfileBehavior
//    {
//        public OffsetMoveTag() { }

//        private bool _isDone;
//        public override bool IsDone
//        {
//            get { return !IsActiveQuestStep || _isDone; }
//        }

//        /// <summary>
//        /// The distance on the X axis to move
//        /// </summary>
//        [XmlAttribute("x")]
//        [XmlAttribute("offsetX")]
//        [XmlAttribute("offsetx")]
//        public float OffsetX { get; set; }

//        /// <summary>
//        /// The distance on the Y axis to move
//        /// </summary>
//        [XmlAttribute("y")]
//        [XmlAttribute("offsetY")]
//        [XmlAttribute("offsety")]
//        public float OffsetY { get; set; }

//        /// <summary>
//        /// The distance before we've "reached" the destination
//        /// </summary>
//        [XmlAttribute("pathPrecision")]
//        public float PathPrecision { get; set; }

//        public Vector3 Position { get; set; }
//        private static MoveResult _lastMoveResult = MoveResult.Moved;

//        protected override Composite CreateBehavior()
//        {
//            return
//            new PrioritySelector(
//                new Decorator(ret => IsFinished(),
//                    new Sequence(
//                        new Action(ret => Core.Logger.Log("Finished Offset Move x={0} y={1} position={3}", OffsetX, OffsetY, Position.Distance2D(MyPos), Position)),
//                        new Action(ret => _isDone = true)
//                    )
//                ),
//                new Action(ret => MoveToPostion())
//            );
//        }

//        private bool IsFinished()
//        {
//            return Position.Distance2D(MyPos) <= PathPrecision || _lastMoveResult == MoveResult.ReachedDestination;
//        }

//        private void MoveToPostion()
//        {
//            _lastMoveResult = PlayerMover.NavigateTo(Position);

//            if (_lastMoveResult == MoveResult.PathGenerationFailed)
//            {
//                Core.Logger.Log("Error moving to offset x={0} y={1} distance={2:0} position={3}", OffsetX, OffsetY, Position.Distance2D(MyPos), Position);
//                _isDone = true;
//            }
//        }

//        public Vector3 MyPos { get { return ZetaDia.Me.Position; } }
//        private ISearchAreaProvider MainGridProvider => 
//            Core.DBGridProvider;

//        public override void OnStart()
//        {
//            _lastMoveResult = MoveResult.Moved;

//            float x = MyPos.X + OffsetX;
//            float y = MyPos.Y + OffsetY;

//            Position = new Vector3(x, y, MainGridProvider.GetHeight(new Vector2(x, y)));

//            if (Math.Abs(PathPrecision) < 1f)
//                PathPrecision = 10f;
//            Core.Logger.Log("OffsetMove Initialized offset x={0} y={1} distance={2:0} position={3}", OffsetX, OffsetY, Position.Distance2D(MyPos), Position);

//        }
//        public override void OnDone()
//        {

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
